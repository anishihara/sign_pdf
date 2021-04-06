using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sign_pdf
{
    class PDFSigner
    {
        public void Sign(string sourceDocument, string destinationPath, string privateKeyPath, string keyPassword, string reason, string location)
        {
            using (FileStream privateKeyStream = new FileStream(privateKeyPath, System.IO.FileMode.Open))
            {
                Pkcs12Store pk12 = new Pkcs12Store(privateKeyStream, keyPassword.ToCharArray());
                privateKeyStream.Dispose();

                //then Iterate throught certificate entries to find the private key entry
                string alias = null;
                foreach (string tAlias in pk12.Aliases)
                {
                    if (pk12.IsKeyEntry(tAlias))
                    {
                        alias = tAlias;
                        break;
                    }
                }
                var pk = pk12.GetKey(alias).Key;
                var ce = pk12.GetCertificateChain(alias);
                var chain = new X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                    chain[k] = ce[k].Certificate;
                // reader and stamper
                PdfReader reader = new PdfReader(sourceDocument);
                FileStream fout = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite);
                StampingProperties properties = new StampingProperties();
                PdfSigner signer = new PdfSigner(reader, fout, properties);

                PdfSignatureAppearance appearance = signer.GetSignatureAppearance().SetReason(reason).SetLocation(location);
                IExternalSignature pks = new PrivateKeySignature(pk, "SHA-512");
                signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
            }
        }
    }
}

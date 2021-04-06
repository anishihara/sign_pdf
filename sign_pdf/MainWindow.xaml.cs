using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sign_pdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void selectPrivateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Certificado (*.pfx)|*.pfx";
            if (openFileDialog.ShowDialog() == true) this.privateKeyPathTextBox.Text = openFileDialog.FileName;
        }

        private void selectPDFButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.privateKeyPathTextBox.Text == "")
                {
                    MessageBox.Show("Selecione um certificado!");
                    return;
                }
                if (this.privateKeyPassword.Password == "")
                {
                    MessageBox.Show("Digite a senha do certificado!");
                    return;
                }
                string privateKeyFilePath = this.privateKeyPathTextBox.Text;
                string privateKeyPassword = this.privateKeyPassword.Password;
                string pdfFilePath = null;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF (*.pdf)|*.pdf";
                if (openFileDialog.ShowDialog() == true) pdfFilePath = openFileDialog.FileName;
                var pdfSigner = new PDFSigner();
                var pdfFilePathParts = System.IO.Path.GetFileName(pdfFilePath).Split('.');
                string destinationFilePath = System.IO.Path.GetDirectoryName(pdfFilePath) + "\\" + pdfFilePathParts[0] + "_assinado.pdf";
                pdfSigner.Sign(pdfFilePath, destinationFilePath, privateKeyFilePath, privateKeyPassword, "", "");
                MessageBox.Show("PDF assinado com sucesso! A cópia assinada encontra-se na mesma pasta do pdf original.");
            }
            catch(Exception exp){
                MessageBox.Show("Houve um erro durante a assinatura. Verifique se a senha do certificado está correto.", "Erro durante a assinatura", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

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
using System.IO;
using System.Net;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCHomepage.xaml
    /// </summary>
    public partial class UCManual : UserControl
    {
        public UCManual()
        {
            InitializeComponent();
        }

        private string ManualURL
        {
            get
            {
                return HPTConfig.MyDocumentsPath + "HPT53Manual.pdf";
            }
        }

        private bool userControlLoaded;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (this.wbManual.Source != null || this.userControlLoaded)
            {
                return;
            }
            this.userControlLoaded = true;

            if (!File.Exists(this.ManualURL))
            {
                MessageBoxResult result = MessageBox.Show("Ladda ner manual från www.hpt.nu?", "Ladda ner manual", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    GetLatestManualVersion();
                }
                else
                {
                    return;
                }
            }
            else
            {
                this.wbManual.Source = new Uri(this.ManualURL); 
            }
        }

        private void btGetLatestManualVersion_Click(object sender, RoutedEventArgs e)
        {
            GetLatestManualVersion();
        }

        private string GetLatestManualVersion()
        {
            Cursor = Cursors.Wait;
            WebClient wc = new WebClient();
            try
            {
                Cursor = Cursors.Wait;
                wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc.DownloadFileAsync(new Uri("http://www.hpt.nu/ladda-ner/HPT53Manual.pdf"), this.ManualURL);
            }
            catch (WebException we)
            {
                HPTConfig.AddToErrorLogStatic(we);
                return string.Empty;
            }
            Cursor = Cursors.Arrow;
            return this.ManualURL;
        }

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                this.wbManual.Source = new Uri(this.ManualURL);
                this.wbManual.Refresh();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Collections.ObjectModel;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCHomepage.xaml
    /// </summary>
    public partial class UCExamples : UserControl
    {
        public event Action<Uri, EventArgs> ExampleSelected;

        public ObservableCollection<HPTSystemFile> ExampleFileList { get; set; }

        public UCExamples()
        {
            this.ExampleFileList = new ObservableCollection<HPTSystemFile>();
            InitializeComponent();
        }

        private string ExamplesURL
        {
            get
            {
                return HPTConfig.MyDocumentsPath + "Exempel4\\HPTExempel.mht";
            }
        }

        private string ExamplesZipURL
        {
            get
            {
                return HPTConfig.MyDocumentsPath + "HPT4Exempel.zip";
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.wbExamples.Source != null || this.downloadingFinishedOrInProgress == true)
            {
                return;
            }
            this.downloadingFinishedOrInProgress = true;
            AddExampleFilesToList();
            if (!File.Exists(this.ExamplesURL))
            {
                MessageBoxResult result = MessageBox.Show("Ladda ner exempel från www.hpt.nu?", "Ladda ner exempel", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    GetLatestExamplesVersion();
                }
                else
                {
                    return;
                }
            }
            this.wbExamples.Source = new Uri(this.ExamplesURL); 
        }

        private void AddExampleFilesToList()
        {
            this.ExampleFileList.Clear();
            string examplesDirectory = HPTConfig.MyDocumentsPath + "Exempel4";
            if (!Directory.Exists(examplesDirectory))
            {
                Directory.CreateDirectory(examplesDirectory);
                return;
            }
            DirectoryInfo diExamplesDirectory = new DirectoryInfo(examplesDirectory);
            foreach (FileInfo fiHPTFile in diExamplesDirectory.GetFiles("*.hpt?"))
            {
                HPTSystemFile hptSystemFile = new HPTSystemFile();
                hptSystemFile.FileName = fiHPTFile.FullName;
                hptSystemFile.FileNameShort = fiHPTFile.Name;
                hptSystemFile.FileType = fiHPTFile.Extension.Replace(".", string.Empty);
                this.ExampleFileList.Add(hptSystemFile);
            }
        }

        private void btGetLatestManualVersion_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                GetLatestExamplesVersion();
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            Cursor = Cursors.Arrow;
        }

        private bool downloadingFinishedOrInProgress = false;
        private string GetLatestExamplesVersion()
        {
            WebClient wc = new WebClient();
            try
            {
                Cursor = Cursors.Wait;
                this.downloadingFinishedOrInProgress = true;
                byte[] examplesBA = wc.DownloadData("http://www.hpt.nu/ladda-ner/HPT4Exempel.zip");

                Ionic.Zip.ZipFile zf = Ionic.Zip.ZipFile.Read(examplesBA);
                foreach (var ze in zf.Entries)
                {
                    ze.Extract(HPTConfig.MyDocumentsPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                    //ze.Extract(this.ExamplesZipURL, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                }

                this.downloadingFinishedOrInProgress = true;
                AddExampleFilesToList();
                this.wbExamples.Navigate(this.ExamplesURL);
                //this.wbExamples.Refresh();
            }
            catch (WebException we)
            {
                this.downloadingFinishedOrInProgress = false;
                HPTConfig.AddToErrorLogStatic(we);
                return string.Empty;
            }
            catch(System.Runtime.InteropServices.COMException ce)
            {
                System.Threading.Thread.Sleep(100);
                //this.wbExamples.Refresh();
            }

            Cursor = Cursors.Arrow;
            return this.ExamplesURL;
        }

        private void btnGetLatestExamplesVersion_Click(object sender, RoutedEventArgs e)
        {
            GetLatestExamplesVersion();
        }

        private void wbExamples_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //Regex rexExample = new Regex("Exempel\\s\\d{1,2}-[\\w\\s-]+?\\.hpt3", RegexOptions.IgnoreCase);
            //if (rexExample.IsMatch(e.Uri.OriginalString))
            //{
            //    if (this.ExampleSelected != null)
            //    {
            //        this.ExampleSelected(e.Uri, e);
            //    }
            //    e.Cancel = true;
            //}
        }

        private void btnOpenExample_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ExampleSelected != null)
                {
                    HPTSystemFile systemFile = (HPTSystemFile)this.cmbExampleFile.SelectedItem;
                    this.ExampleSelected(new Uri(systemFile.FileName), e);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

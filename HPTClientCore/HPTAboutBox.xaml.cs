using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for HPTAboutBox.xaml
    /// </summary>
    public partial class HPTAboutBox : Window
    {
        public HPTAboutBox()
        {
            InitializeComponent();
        }

        private void hlHomePage_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            GoToUrl(hl.NavigateUri.OriginalString);
            //System.Diagnostics.Process.Start(hl.NavigateUri.OriginalString);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void hlATGSpelAnsvar_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            GoToUrl(hl.NavigateUri.OriginalString);
            //System.Diagnostics.Process.Start(hl.NavigateUri.OriginalString);
        }

        private void GoToUrl(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}

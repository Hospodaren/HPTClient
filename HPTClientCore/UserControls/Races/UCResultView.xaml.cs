using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCResultView.xaml
    /// </summary>
    public partial class UCResultView : UserControl
    {
        public UCResultView()
        {
            InitializeComponent();
        }

        private void hlSTHorseLinkExternal_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var hl = (Hyperlink)e.OriginalSource;
            GoToUrl(hl.NavigateUri.AbsoluteUri);
            //System.Diagnostics.Process.Start(hl.NavigateUri.AbsoluteUri);
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

        private void btnGetMoreResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var horse = DataContext as HPTHorse;
                if (horse.ResultList.Count < 5)
                {
                    btnGetMoreResults.Visibility = Visibility.Collapsed;
                    return;
                }

                var serviceConnector = new HPTServiceConnector();
                //serviceConnector.GetHorseResultListFromATG(this.DataContext as HPTHorse);
                serviceConnector.GetHorseStartInformationFromATG(horse);
                btnGetMoreResults.Visibility = Visibility.Collapsed;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext.GetType() == typeof(HPTHorse))
                {
                    var horse = DataContext as HPTHorse;
                    if (horse.ResultList.Count != 5)
                    {
                        btnGetMoreResults.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}

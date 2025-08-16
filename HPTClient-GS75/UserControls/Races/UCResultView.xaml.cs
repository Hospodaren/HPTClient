using System;
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
            System.Diagnostics.Process.Start(hl.NavigateUri.AbsoluteUri);
        }

        private void btnGetMoreResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var horse = this.DataContext as HPTHorse;
                if (horse.ResultList.Count < 5)
                {
                    this.btnGetMoreResults.Visibility = Visibility.Collapsed;
                    return;
                }

                var serviceConnector = new HPTServiceConnector();
                //serviceConnector.GetHorseResultListFromATG(this.DataContext as HPTHorse);
                serviceConnector.GetHorseStartInformationFromATG(horse);
                this.btnGetMoreResults.Visibility = Visibility.Collapsed;
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
                if (this.DataContext.GetType() == typeof(HPTHorse))
                {
                    var horse = this.DataContext as HPTHorse;
                    if (horse.ResultList.Count != 5)
                    {
                        this.btnGetMoreResults.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}

using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRacesTabControl.xaml
    /// </summary>
    public partial class UCRacesTabControl : UCMarkBetControl
    {
        public UCRacesTabControl()
        {
            InitializeComponent();
        }

        internal void UpdateSortOrder()
        {
            var raceViewList = this.tcRaces.Items.Cast<UCRaceView>();
        }

        private void hlVPOnAtgSe_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var hl = (Hyperlink)e.OriginalSource;
            GoToUrl(hl.NavigateUri.OriginalString);
            //System.Diagnostics.Process.Start(hl.NavigateUri.ToString());
            e.Handled = true;
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

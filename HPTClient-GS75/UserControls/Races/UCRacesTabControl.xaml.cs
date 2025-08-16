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
            System.Diagnostics.Process.Start(hl.NavigateUri.ToString());
            e.Handled = true;
        }
    }
}

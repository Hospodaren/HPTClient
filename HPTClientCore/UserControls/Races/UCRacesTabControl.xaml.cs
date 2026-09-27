using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
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
            var raceViewList = tcRaces.Items.Cast<UCRaceView>();
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

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            int selectedIndex = 0;
            switch (e.Key)
            {
                case Key.D1:
                case Key.NumPad1:
                    selectedIndex = 0;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    selectedIndex = 1;
                    break;
                case Key.D3:
                case Key.NumPad3:
                    selectedIndex = 2;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    selectedIndex = 3;
                    break;
                case Key.D5:
                case Key.NumPad5:
                    selectedIndex = 4;
                    break;
                case Key.D6:
                case Key.NumPad6:
                    selectedIndex = 5;
                    break;
                case Key.D7:
                case Key.NumPad7:
                    selectedIndex = 6;
                    break;
                case Key.D8:
                case Key.NumPad8:
                    selectedIndex = 7;
                    break;
            }

            if (tcRaces.Items.Count > selectedIndex)
            {
                tcRaces.SelectedIndex = selectedIndex;
            }
        }
    }
}

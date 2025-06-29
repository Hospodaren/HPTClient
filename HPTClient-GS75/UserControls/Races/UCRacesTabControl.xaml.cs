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

using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCGeneralSettings.xaml
    /// </summary>
    public partial class UCGeneralSettings : UserControl
    {
        public UCGeneralSettings()
        {
            InitializeComponent();
            if (!HPTConfig.Config.IsPayingCustomer)
            {
                //this.chkAlwaysCreateSingleRows.IsEnabled = false;
                //this.chkCopyCouponsToClipboard.IsEnabled = false;
                //this.chkCopySingleRowsToClipboard.IsEnabled = false;
                //this.chkUseDefaultRankTemplate.IsEnabled = false;
                //this.chkWarnIfNoReserv.IsEnabled = false;
                //this.chkWarnIfOverlappingComplementaryRules.IsEnabled = false;
                //this.chkWarnIfUncoveredHorses.IsEnabled = false;
                //this.chkWarnIfSuperfluousXReduction.IsEnabled = false;
                //this.cmbRankTemplate.IsEnabled = false;
            }
        }

        private void cmbUpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUpdateInterval.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)cmbUpdateInterval.SelectedItem;
                int updatePeriod = Convert.ToInt32(cbi.Tag);
                if (HPTConfig.Config.DefaultUpdateInterval != updatePeriod)
                {
                    HPTConfig.Config.DefaultUpdateInterval = updatePeriod;
                }
            }
        }

        private void cmbReservHandling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbReservHandling.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)cmbReservHandling.SelectedItem;
                ReservHandling rh = (ReservHandling)Convert.ToInt32(cbi.Tag);
                if (HPTConfig.Config.DefaultReservHandling != rh)
                {
                    HPTConfig.Config.DefaultReservHandling = rh;
                }
            }
        }

        private void ucGeneralSettings_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                switch (HPTConfig.Config.DefaultReservHandling)
                {
                    case ReservHandling.Own:
                    case ReservHandling.None:
                        cmbReservHandling.SelectedIndex = 0;
                        break;
                    case ReservHandling.MarksSelected:
                        cmbReservHandling.SelectedIndex = 1;
                        break;
                    case ReservHandling.MarksNotSelected:
                        cmbReservHandling.SelectedIndex = 2;
                        break;
                    case ReservHandling.OwnRankSelected:
                        cmbReservHandling.SelectedIndex = 3;
                        break;
                    case ReservHandling.OwnRankNotSelected:
                        cmbReservHandling.SelectedIndex = 4;
                        break;
                    case ReservHandling.RankMeanSelected:
                        cmbReservHandling.SelectedIndex = 5;
                        break;
                    case ReservHandling.RankMeanNotSelected:
                        cmbReservHandling.SelectedIndex = 6;
                        break;
                    case ReservHandling.OddsSelected:
                        cmbReservHandling.SelectedIndex = 7;
                        break;
                    case ReservHandling.OddsNotSelected:
                        cmbReservHandling.SelectedIndex = 8;
                        break;
                    case ReservHandling.NextRankSelected:
                        cmbReservHandling.SelectedIndex = 9;
                        break;
                    case ReservHandling.NextRankNotSelected:
                        cmbReservHandling.SelectedIndex = 10;
                        break;
                    default:
                        break;
                }

                switch (HPTConfig.Config.DefaultUpdateInterval)
                {
                    case 3:
                        cmbUpdateInterval.SelectedIndex = 1;
                        break;
                    case 5:
                        cmbUpdateInterval.SelectedIndex = 2;
                        break;
                    case 10:
                        cmbUpdateInterval.SelectedIndex = 3;
                        break;
                    case 15:
                        cmbUpdateInterval.SelectedIndex = 4;
                        break;
                    case 20:
                        cmbUpdateInterval.SelectedIndex = 5;
                        break;
                    case 30:
                        cmbUpdateInterval.SelectedIndex = 6;
                        break;
                    case 60:
                        cmbUpdateInterval.SelectedIndex = 7;
                        break;
                    default:
                        cmbUpdateInterval.SelectedIndex = 0;
                        break;
                }
            }
        }
    }
}

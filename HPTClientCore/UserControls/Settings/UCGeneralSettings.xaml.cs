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
            if (this.cmbUpdateInterval.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)this.cmbUpdateInterval.SelectedItem;
                int updatePeriod = Convert.ToInt32(cbi.Tag);
                if (HPTConfig.Config.DefaultUpdateInterval != updatePeriod)
                {
                    HPTConfig.Config.DefaultUpdateInterval = updatePeriod;
                }
            }
        }

        private void cmbReservHandling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbReservHandling.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)this.cmbReservHandling.SelectedItem;
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
                        this.cmbReservHandling.SelectedIndex = 0;
                        break;
                    case ReservHandling.MarksSelected:
                        this.cmbReservHandling.SelectedIndex = 1;
                        break;
                    case ReservHandling.MarksNotSelected:
                        this.cmbReservHandling.SelectedIndex = 2;
                        break;
                    case ReservHandling.OwnRankSelected:
                        this.cmbReservHandling.SelectedIndex = 3;
                        break;
                    case ReservHandling.OwnRankNotSelected:
                        this.cmbReservHandling.SelectedIndex = 4;
                        break;
                    case ReservHandling.RankMeanSelected:
                        this.cmbReservHandling.SelectedIndex = 5;
                        break;
                    case ReservHandling.RankMeanNotSelected:
                        this.cmbReservHandling.SelectedIndex = 6;
                        break;
                    case ReservHandling.OddsSelected:
                        this.cmbReservHandling.SelectedIndex = 7;
                        break;
                    case ReservHandling.OddsNotSelected:
                        this.cmbReservHandling.SelectedIndex = 8;
                        break;
                    case ReservHandling.NextRankSelected:
                        this.cmbReservHandling.SelectedIndex = 9;
                        break;
                    case ReservHandling.NextRankNotSelected:
                        this.cmbReservHandling.SelectedIndex = 10;
                        break;
                    default:
                        break;
                }

                switch (HPTConfig.Config.DefaultUpdateInterval)
                {
                    case 3:
                        this.cmbUpdateInterval.SelectedIndex = 1;
                        break;
                    case 5:
                        this.cmbUpdateInterval.SelectedIndex = 2;
                        break;
                    case 10:
                        this.cmbUpdateInterval.SelectedIndex = 3;
                        break;
                    case 15:
                        this.cmbUpdateInterval.SelectedIndex = 4;
                        break;
                    case 20:
                        this.cmbUpdateInterval.SelectedIndex = 5;
                        break;
                    case 30:
                        this.cmbUpdateInterval.SelectedIndex = 6;
                        break;
                    case 60:
                        this.cmbUpdateInterval.SelectedIndex = 7;
                        break;
                    default:
                        this.cmbUpdateInterval.SelectedIndex = 0;
                        break;
                }
            }
        }
    }
}

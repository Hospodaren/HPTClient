using System.Windows;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCOverviewFromTips.xaml
    /// </summary>
    public partial class UCOverviewFromTips : UCMarkBetControl
    {
        public UCOverviewFromTips()
        {
            InitializeComponent();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            var selectedHorsesFromTips = this.MarkBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.SelectedFromTip)
                .ToList();

            if ((bool)this.chkClear.IsChecked)
            {
                this.MarkBet.Clear(false, true, true);
            }
            //if ((bool)this.chkSetRank.IsChecked)
            //{

            //}
            //if ((bool)this.chkSetABC.IsChecked)
            //{
            //    foreach (var horse in selectedHorsesFromTips)
            //    {
            //        horse.Prio = horse.PrioFromTips;
            //    }
            //}
            if ((bool)this.chkSelect.IsChecked)
            {
                foreach (var horse in selectedHorsesFromTips)
                {
                    horse.Selected = true;
                }
            }
        }
    }
}

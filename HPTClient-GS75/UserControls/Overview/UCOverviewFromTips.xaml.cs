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
    /// Interaction logic for UCOverviewFromTips.xaml
    /// </summary>
    public partial class UCOverviewFromTips :  UCMarkBetControl
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

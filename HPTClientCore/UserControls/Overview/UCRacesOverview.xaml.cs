using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRacesOverview.xaml
    /// </summary>
    public partial class UCRacesOverview : UCMarkBetControl
    {
        public UCRacesOverview()
        {
            InitializeComponent();
        }



        public List<UCRaceForOverview> RaceList
        {
            get { return (List<UCRaceForOverview>)GetValue(RaceListProperty); }
            set { SetValue(RaceListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RaceList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RaceListProperty =
            DependencyProperty.Register("RaceList", typeof(List<UCRaceForOverview>), typeof(UCRacesOverview), new PropertyMetadata(null));



        //private bool dragDropEnabled;
        //private List<DependencyObject> depObjList;
        //private List<ItemsControl> icList;
        //private List<ItemsControl> ICList
        //{
        //    get
        //    {
        //        if (this.icList == null || this.icList.Count == 0)
        //        {
        //            this.depObjList = new List<DependencyObject>();
        //            FindDependencyObjectChild(this.lstOverview, typeof(ItemsControl));
        //            this.icList = this.depObjList.Cast<ItemsControl>().ToList();
        //        }
        //        return this.icList;
        //    }
        //}


        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.RaceList == null && !CreateRaceList())
            {
                return;
            }
            ComboBoxItem cbi = (ComboBoxItem)this.cmbSort.SelectedItem;
            string sortVariable = (string)cbi.Tag;
            this.RaceList.ForEach(r =>
                {
                    r.Sort(sortVariable);
                    r.DragDropEnabled = sortVariable == "RankOwn";
                });
        }

        private void btnSetRankOwn_Click(object sender, RoutedEventArgs e)
        {
            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            try
            {
                this.MarkBet.pauseRecalculation = true;
                if (this.cmbSort.SelectedIndex == -1)
                {
                    foreach (var race in this.MarkBet.RaceDayInfo.RaceList)
                    {
                        int rankToSet = 1;
                        int numberOfScratchedHorses = 0;
                        foreach (var horse in race.HorseList)
                        {
                            if (horse.Scratched == true)
                            {
                                horse.RankOwn = 16;
                                numberOfScratchedHorses++;
                            }
                            else
                            {
                                horse.RankOwn = horse.StartNr;
                                horse.RankOwn = rankToSet;
                                rankToSet++;
                            }
                        }
                    }
                }
                else
                {
                    this.RaceList.ForEach(r =>
                        {
                            r.SetRank();
                        });

                }

                this.cmbSort.SelectedIndex = 0;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
            this.MarkBet.RecalculateReduction(RecalculateReason.All);

            //bool recalculationPaused = this.MarkBet.pauseRecalculation;
            //try
            //{
            //    this.MarkBet.pauseRecalculation = true;
            //    if (this.cmbSort.SelectedIndex == -1)
            //    {
            //        foreach (var race in this.MarkBet.RaceDayInfo.RaceList)
            //        {
            //            int rankToSet = 1;
            //            int numberOfScratchedHorses = 0;
            //            foreach (var horse in race.HorseList)
            //            {
            //                if (horse.Scratched == true)
            //                {
            //                    horse.RankOwn = 16;
            //                    numberOfScratchedHorses++;
            //                }
            //                else
            //                {
            //                    horse.RankOwn = horse.StartNr;
            //                    horse.RankOwn = rankToSet;
            //                    rankToSet++;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        foreach (ItemsControl ic in this.ICList)
            //        {
            //            int rankToSet = 1;
            //            var viewSource = (ListCollectionView)ic.ItemsSource;
            //            for (int i = 0; i < viewSource.Count; i++)
            //            {
            //                var horse = (HPTHorse)viewSource.GetItemAt(i);
            //                try
            //                {
            //                    if (horse.Scratched == true)
            //                    {
            //                        horse.RankOwn = 16;
            //                    }
            //                    else
            //                    {
            //                        horse.RankOwn = rankToSet;
            //                        rankToSet++;
            //                    }

            //                }
            //                catch (NullReferenceException exc)
            //                {
            //                    // NumericUpDowns som inte finns... :-(
            //                }
            //            }
            //        }
            //    }

            //    this.cmbSort.SelectedIndex = 0;
            //    this.dragDropEnabled = true;
            //}
            //catch (Exception exc)
            //{
            //    string s = exc.Message;
            //}
            //this.MarkBet.pauseRecalculation = recalculationPaused;
            //this.MarkBet.RecalculateReduction(RecalculateReason.All);
        }

        #region Drag-and-Drop

        //private void icHorseOverview_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (!this.dragDropEnabled)
        //    {
        //        return;
        //    }
        //    if (sender.GetType() == typeof(UCCompactHorse))
        //    {
        //        var uc = (UCCompactHorse)sender;
        //        if (uc != null && e.LeftButton == MouseButtonState.Pressed)
        //        {
        //            DragDrop.DoDragDrop(uc, uc, DragDropEffects.Move);
        //        }
        //    }
        //}

        //private void UCCompactHorse_DragOver(object sender, DragEventArgs e)
        //{
        //    if (!this.dragDropEnabled)
        //    {
        //        return;
        //    }
        //    var ucTarget = (UCCompactHorse)sender;
        //    var horseTarget = (HPTHorse)ucTarget.DataContext;
        //    var ucSource = (UCCompactHorse)e.Source;
        //    var horseSource = (HPTHorse)ucSource.DataContext;
        //    if (horseSource == null ||horseTarget == null ||  horseSource == horseTarget)
        //    {
        //        return;
        //    }
        //    if (horseTarget.ParentRace == horseSource.ParentRace)
        //    {
        //        e.Effects = DragDropEffects.Move;
        //    }
        //}

        //private void UCCompactHorse_Drop(object sender, DragEventArgs e)
        //{
        //    //bool recalculationPaused = this.MarkBet.pauseRecalculation;
        //    //if (!this.dragDropEnabled)
        //    //{
        //    //    return;
        //    //}
        //    //var ucTarget = (UCCompactHorse)sender;
        //    //var horseTarget = (HPTHorse)ucTarget.DataContext;
        //    //var ucSource = (UCCompactHorse)e.Data.GetData(typeof(UCCompactHorse));
        //    //var horseSource = (HPTHorse)ucSource.DataContext;
        //    //if (horseSource == null || horseTarget == null || horseSource == horseTarget)
        //    //{
        //    //    return;
        //    //}
        //    //if (horseTarget.ParentRace == horseSource.ParentRace)
        //    //{
        //    //    if (horseTarget.RankOwn == horseSource.RankOwn)
        //    //    {
        //    //        return;
        //    //    }
        //    //    this.MarkBet.pauseRecalculation = true;
        //    //    if (horseTarget.RankOwn > horseSource.RankOwn)
        //    //    {
        //    //        List<HPTHorse> horsesToAlter = horseTarget.ParentRace.HorseList
        //    //            .Where(h => h.StartNr != horseSource.StartNr
        //    //                    && h.StartNr != horseTarget.StartNr
        //    //                    && h.RankOwn > horseSource.RankOwn
        //    //                    && h.RankOwn <= horseTarget.RankOwn).ToList();

        //    //        foreach (var horse in horsesToAlter)
        //    //        {
        //    //            horse.RankOwn--;
        //    //        }
        //    //        horseSource.RankOwn = horseTarget.RankOwn;
        //    //        horseTarget.RankOwn--;
        //    //    }
        //    //    else if (horseTarget.RankOwn < horseSource.RankOwn)
        //    //    {
        //    //        List<HPTHorse> horsesToAlter = horseTarget.ParentRace.HorseList
        //    //            .Where(h => h.StartNr != horseSource.StartNr
        //    //                    && h.StartNr != horseTarget.StartNr
        //    //                    && h.RankOwn <= horseSource.RankOwn
        //    //                    && h.RankOwn > horseTarget.RankOwn).ToList();

        //    //        foreach (var horse in horsesToAlter)
        //    //        {
        //    //            horse.RankOwn++;
        //    //        }
        //    //        horseSource.RankOwn = horseTarget.RankOwn;
        //    //        horseTarget.RankOwn++;
        //    //    }
        //    //    this.MarkBet.pauseRecalculation = recalculationPaused;
        //    //    ItemsControl icToUpdate = this.ICList.FirstOrDefault(ic => ic.Items.Contains(horseSource));
        //    //    if (icToUpdate != null)
        //    //    {
        //    //        var viewSource = (ListCollectionView)icToUpdate.ItemsSource;
        //    //        viewSource.SortDescriptions.Clear();
        //    //        viewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription("RankOwn", System.ComponentModel.ListSortDirection.Ascending));
        //    //    }

        //    //    if (this.MarkBet.HasOwnRankReduction()) // Gör om hela reduceringsberäkningen
        //    //    {
        //    //        this.MarkBet.RecalculateReduction(RecalculateReason.All);
        //    //    }
        //    //    else    // Beräkna bara om ranken
        //    //    {
        //    //        this.MarkBet.RecalculateAllRanks();
        //    //        this.MarkBet.RecalculateRank();
        //    //    }
        //    //}
        //}

        #endregion

        #region Dölj strukna hästar / Visa bara valda hästar

        private void chkHideScratched_Checked(object sender, RoutedEventArgs e)
        {
            this.RaceList.ForEach(r =>
            {
                r.HideScratched = (bool)this.chkHideScratched.IsChecked;
                r.SetFilter();
            });
        }

        private void chkShowOnlySelected_Checked(object sender, RoutedEventArgs e)
        {
            this.RaceList.ForEach(r =>
            {
                r.ShowOnlySelected = (bool)this.chkShowOnlySelected.IsChecked;
                r.SetFilter();
            });
        }

        public bool FilterHorses(object obj)
        {
            var horse = obj as HPTHorse;
            bool showScratched = this.chkHideScratched.IsChecked == false;
            bool showOnlySelected = this.chkShowOnlySelected.IsChecked == true;

            return (showScratched == horse.Scratched || horse.Scratched == false) && (showOnlySelected == horse.Selected || !showOnlySelected);
        }

        //private void chkShowOnlySelected_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    this.RaceList.ForEach(r =>
        //        {
        //            r.ShowOnlySelected = false;
        //            r.SetFilter();
        //        });
        //}

        #endregion

        private void icHorseOverview_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            //var uc = (UCCompactHorse)sender;
            //var horse = (HPTHorse)uc.DataContext;            
            //var icToUpdate = this.ICList.FirstOrDefault(ic => ic.Items.Contains(horse));

            //var fe = (FrameworkElement)e.OriginalSource;
            //if (fe.GetType() == typeof(TextBlock))
            //{
            //    var tb = (TextBlock)fe;

            //    // Startnummer
            //    int startNumber = 0;
            //    bool isStartNumber = int.TryParse(tb.Text, out startNumber);
            //    if (isStartNumber && startNumber > 0 && startNumber < 16)
            //    {
            //        if (icToUpdate != null)
            //        {
            //            bool select = true;
            //            var horseListSorted = (ListCollectionView)icToUpdate.ItemsSource;
            //            foreach (var o in horseListSorted)
            //            {
            //                var horseToSelect = (HPTHorse)o;
            //                if (horseToSelect.Scratched != true)
            //                {
            //                    horseToSelect.Selected = select;
            //                }
            //                else
            //                {
            //                    horseToSelect.Selected = false;
            //                }
            //                if (horse == horseToSelect)
            //                {
            //                    select = false;
            //                }
            //            }
            //        }
            //    }

            //    // ABCD-reducering
            //    var prio = EnumHelper.GetHPTPrioFromShortString(tb.Text);
            //    if (prio != HPTPrio.M)
            //    {
            //        if (icToUpdate != null)
            //        {
            //            bool select = true;
            //            var horseListSorted = (ListCollectionView)icToUpdate.ItemsSource;
            //            foreach (var o in horseListSorted)
            //            {
            //                var horseToSelect = (HPTHorse)o;
            //                if (horseToSelect.Scratched != true)
            //                {
            //                    if ((horseToSelect.Prio == HPTPrio.M && select) || horse == horseToSelect)
            //                    {
            //                        var xReductionToSelect = horseToSelect.HorseXReductionList.FirstOrDefault(xr => xr.Prio == prio);
            //                        if (xReductionToSelect != null)
            //                        {
            //                            xReductionToSelect.Selected = true;
            //                        }
            //                    }
            //                }
            //                if (horse == horseToSelect)
            //                {
            //                    select = false;
            //                }
            //                else if (!select && horse.Selected)
            //                {
            //                    horseToSelect.Selected = false;
            //                }
            //            }
            //        }
            //    }
            //    var viewSource = (ListCollectionView)icToUpdate.ItemsSource;
            //    if (this.chkShowOnlySelected.IsChecked == true)
            //    {
            //        viewSource.Filter = new Predicate<object>(FilterHorses);
            //    }
            //}
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (this.MarkBet != null)
            {
                try
                {
                    this.MarkBet.pauseRecalculation = true;
                    foreach (var race in this.MarkBet.RaceDayInfo.RaceList)
                    {
                        foreach (var horse in race.HorseList.Where(h => h.Scratched != true))
                        {
                            horse.Selected = true;
                        }
                    }
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
                this.MarkBet.pauseRecalculation = false;
                this.MarkBet.RecalculateReduction(RecalculateReason.All);
            }
        }

        private void lstOverview_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        // Egen rank ändrad på någon av hästarna
        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.IsLoaded && this.MarkBet != null && !this.MarkBet.pauseRecalculation)
            {
                this.MarkBet.RecalculateAllRanks();
                this.MarkBet.RecalculateRank();
                if (this.MarkBet.HasOwnRankReduction())
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
            }
        }

        // Poäng ändrad på någon av hästarna
        private void iudRankAlternate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.IsLoaded && this.MarkBet != null && !this.MarkBet.pauseRecalculation)
            {
                this.MarkBet.RecalculateAllRanks();
                this.MarkBet.RecalculateRank();
                if (this.MarkBet.HasAlternateRankReduction())
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
            }
        }

        private void IntegerUpDown_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.MarkBet != null)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.All);
            }
        }

        private void ucRacesOverview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CreateRaceList();
        }

        private bool CreateRaceList()
        {
            if (this.MarkBet != null && this.RaceList == null)
            {
                this.RaceList = this.MarkBet.RaceDayInfo.RaceList.Select(r => new UCRaceForOverview()
                {
                    DataContext = r,
                    Race = r,
                    MarkBet = this.MarkBet
                }).ToList();

                this.RaceList.ForEach(r =>
                    {
                        r.Sort("RankOwn");
                        r.UpdateHorseOrder();
                    });

                return true;
            }
            return false;
        }

        private void UCMarkBetControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}

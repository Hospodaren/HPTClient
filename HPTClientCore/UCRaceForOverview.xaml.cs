using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRaceForOverview.xaml
    /// </summary>
    public partial class UCRaceForOverview : UserControl
    {
        public UCRaceForOverview()
        {
            InitializeComponent();
        }

        #region Dependency properties

        public HPTRace Race
        {
            get { return (HPTRace)GetValue(RaceProperty); }
            set { SetValue(RaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Race.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RaceProperty =
            DependencyProperty.Register("Race", typeof(HPTRace), typeof(UCRaceForOverview), new PropertyMetadata(null));



        public List<UCCompactHorse> HorseList
        {
            get { return (List<UCCompactHorse>)GetValue(HorseListProperty); }
            set { SetValue(HorseListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorseList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorseListProperty =
            DependencyProperty.Register("HorseList", typeof(List<UCCompactHorse>), typeof(UCRaceForOverview), new PropertyMetadata(null));


        #endregion

        public HPTMarkBet MarkBet { get; set; }

        public bool ShowOnlySelected { get; set; }

        public bool HideScratched { get; set; }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CreateRace();
        }

        private bool CreateRace()
        {
            if (this.DataContext.GetType() == typeof(HPTRace))
            {
                if (this.HorseList == null)
                {
                    this.Race = (HPTRace)this.DataContext;
                    this.HorseList = this.Race.HorseList.Select(h => new UCCompactHorse()
                    {
                        DataContext = h,
                        Horse = h,
                        AllowDrop = true,
                        ClipToBounds = false
                    }).ToList();
                    this.HorseList.ForEach(h =>
                        {
                            h.MouseMove += icHorseOverview_MouseMove;
                            h.DragOver += UCCompactHorse_DragOver;
                            h.Drop += UCCompactHorse_Drop;
                            h.MouseRightButtonUp += icHorseOverview_MouseRightButtonUp;
                        });
                }
                return true;
            }
            return false;
        }

        internal void SetRank()
        {
            int rankToSet = 1;
            this.HorseList.ForEach(uc =>
                {
                    try
                    {
                        if (uc.Horse.Scratched == true)
                        {
                            uc.Horse.RankOwn = 16;
                        }
                        else
                        {
                            uc.Horse.RankOwn = rankToSet;
                            rankToSet++;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        // NumericUpDowns som inte finns... :-(
                    }
                });
            this.DragDropEnabled = true;
        }

        private void icHorseOverview_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var uc = (UCCompactHorse)sender;

            var fe = (FrameworkElement)e.OriginalSource;
            if (fe.GetType() == typeof(TextBlock))
            {
                var tb = (TextBlock)fe;

                bool recalculationPaused = this.MarkBet.pauseRecalculation;
                try
                {
                    this.MarkBet.pauseRecalculation = true;
                    // Startnummer
                    int startNumber = 0;
                    bool isStartNumber = int.TryParse(tb.Text, out startNumber);
                    if (isStartNumber && startNumber > 0 && startNumber < 21)
                    {
                        if (this.HorseList != null)
                        {
                            bool select = true;
                            this.HorseList.ForEach(h =>
                                {
                                    if (h.Horse.Scratched != true)
                                    {
                                        h.Horse.Selected = select;
                                    }
                                    else
                                    {
                                        h.Horse.Selected = false;
                                    }
                                    if (h == uc)
                                    {
                                        select = false;
                                    }
                                });
                        }
                    }

                    // ABCD-reducering
                    var prio = EnumHelper.GetHPTPrioFromShortString(tb.Text);
                    if (prio != HPTPrio.M)
                    {
                        if (this.HorseList != null)
                        {
                            bool select = true;
                            this.HorseList.ForEach(h =>
                            {
                                if (h.Horse.Scratched != true)
                                {
                                    if ((h.Horse.Prio == HPTPrio.M && select) || h == uc)
                                    {
                                        var xReductionToSelect = h.Horse.HorseXReductionList.FirstOrDefault(xr => xr.Prio == prio);
                                        if (xReductionToSelect != null)
                                        {
                                            xReductionToSelect.Selected = true;
                                        }
                                    }
                                }
                                if (h == uc)
                                {
                                    select = false;
                                }
                                else if (!select && h.Horse.Selected)
                                {
                                    h.Horse.Selected = false;
                                }
                            });
                        }
                    }
                    this.MarkBet.pauseRecalculation = recalculationPaused;
                    this.MarkBet.RecalculateReduction(RecalculateReason.All);
                }
                catch (Exception)
                {
                    this.MarkBet.pauseRecalculation = recalculationPaused;
                }


                //var viewSource = (ListCollectionView)icToUpdate.ItemsSource;
                //if (this.chkShowOnlySelected.IsChecked == true)
                //{
                //    viewSource.Filter = new Predicate<object>(FilterHorses);
                //}
            }
        }

        internal void SetFilter()
        {
            if (this.ShowOnlySelected)
            {
                this.HorseList
                    .Where(h => !h.Horse.Selected || h.Horse.Scratched == true)
                    .ToList()
                    .ForEach(h => h.Visibility = System.Windows.Visibility.Collapsed);
            }
            else
            {
                this.HorseList
                            .ForEach(h => h.Visibility = System.Windows.Visibility.Visible);

                if (this.HideScratched)
                {
                    this.HorseList
                        .Where(h => h.Horse.Scratched == true)
                        .ToList()
                        .ForEach(h => h.Visibility = System.Windows.Visibility.Collapsed);
                }
            }
        }

        public bool FilterHorses(object obj)
        {
            var uc = obj as UCCompactHorse;

            return (this.HideScratched != (bool)uc.Horse.Scratched || (bool)uc.Horse.Scratched == false) && (this.ShowOnlySelected == uc.Horse.Selected || !this.ShowOnlySelected);
        }

        #region Drag-and-drop

        private void icHorseOverview_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.DragDropEnabled)
            {
                return;
            }
            if (sender.GetType() == typeof(UCCompactHorse))
            {
                var uc = (UCCompactHorse)sender;
                if (uc != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(uc, uc, DragDropEffects.Move);
                }
            }
        }

        private void UCCompactHorse_DragOver(object sender, DragEventArgs e)
        {
            if (!this.DragDropEnabled)
            {
                return;
            }
            var ucTarget = (UCCompactHorse)sender;
            var horseTarget = (HPTHorse)ucTarget.DataContext;
            var ucSource = (UCCompactHorse)e.Source;
            var horseSource = (HPTHorse)ucSource.DataContext;
            if (horseSource == null || horseTarget == null || horseSource == horseTarget)
            {
                return;
            }
            if (horseTarget.ParentRace == horseSource.ParentRace)
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void UCCompactHorse_Drop(object sender, DragEventArgs e)
        {
            var ucTarget = (UCCompactHorse)sender;
            var ucSource = (UCCompactHorse)e.Data.GetData(typeof(UCCompactHorse));
            if (ucSource == null || ucTarget == null || ucTarget == ucSource)
            {
                return;
            }
            if (ucTarget.Horse.ParentRace == ucSource.Horse.ParentRace)
            {
                if (ucTarget.Horse.RankOwn == ucSource.Horse.RankOwn)
                {
                    return;
                }
                bool recalculationPaused = this.MarkBet.pauseRecalculation;
                this.MarkBet.pauseRecalculation = true;
                if (ucTarget.Horse.RankOwn > ucSource.Horse.RankOwn)
                {
                    List<HPTHorse> horsesToAlter = ucTarget.Horse.ParentRace.HorseList
                        .Where(h => h.StartNr != ucSource.Horse.StartNr
                                && h.StartNr != ucTarget.Horse.StartNr
                                && h.RankOwn > ucSource.Horse.RankOwn
                                && h.RankOwn <= ucTarget.Horse.RankOwn).ToList();

                    foreach (var horse in horsesToAlter)
                    {
                        horse.RankOwn--;
                    }
                    ucSource.Horse.RankOwn = ucTarget.Horse.RankOwn;
                    ucTarget.Horse.RankOwn--;
                }
                else if (ucTarget.Horse.RankOwn < ucSource.Horse.RankOwn)
                {
                    List<HPTHorse> horsesToAlter = ucTarget.Horse.ParentRace.HorseList
                        .Where(h => h.StartNr != ucSource.Horse.StartNr
                                && h.StartNr != ucTarget.Horse.StartNr
                                && h.RankOwn <= ucSource.Horse.RankOwn
                                && h.RankOwn > ucTarget.Horse.RankOwn).ToList();

                    foreach (var horse in horsesToAlter)
                    {
                        horse.RankOwn++;
                    }
                    ucSource.Horse.RankOwn = ucTarget.Horse.RankOwn;
                    ucTarget.Horse.RankOwn++;
                }

                Sort("RankOwn");
                UpdateHorseOrder();

                this.MarkBet.pauseRecalculation = recalculationPaused;
                if (this.MarkBet.HasOwnRankReduction())
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
                else
                {
                    this.MarkBet.RecalculateAllRanks();
                    this.MarkBet.RecalculateRank();
                }
            }
        }

        public bool DragDropEnabled { get; set; }

        #endregion

        // Egen rank ändrad på någon av hästarna
        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.IsLoaded && this.MarkBet != null && !this.MarkBet.pauseRecalculation)
            {
                if (this.MarkBet.HasOwnRankReduction())
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
                else
                {
                    this.MarkBet.RecalculateAllRanks();
                    this.MarkBet.RecalculateRank();
                }
            }
        }

        // Poäng ändrad på någon av hästarna
        private void iudRankAlternate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.IsLoaded && this.MarkBet != null && !this.MarkBet.pauseRecalculation)
            {
                if (this.MarkBet.HasAlternateRankReduction())
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
                else
                {
                    this.MarkBet.RecalculateAllRanks();
                    this.MarkBet.RecalculateRank();
                }
            }
        }


        #region Sortering

        public void Sort(string sortVariable)
        {
            this.DragDropEnabled = false;
            switch (sortVariable)
            {
                case "StakeDistributionShare":
                    this.HorseList.Sort(CompareHorsesStakeShare);
                    break;
                case "RankOwn":
                    this.HorseList.Sort(CompareHorsesRankOwn);
                    this.DragDropEnabled = true;
                    break;
                case "StartNr":
                    this.HorseList.Sort(CompareHorsesStartNr);
                    break;
                case "VinnarOdds":
                    this.HorseList.Sort(CompareHorsesVinnarOdds);
                    break;
                case "RankWeighted":
                    this.HorseList.Sort(CompareHorsesRankWeighted);
                    break;
                case "RankTip":
                    this.HorseList.Sort(CompareHorsesRankTip);
                    break;
                case "HorseName":
                    this.HorseList.Sort(CompareHorsesName);
                    break;
                default:
                    break;
            }

            UpdateHorseOrder();
        }

        internal void UpdateHorseOrder()
        {
            var itemsView = CollectionViewSource.GetDefaultView(this.icHorseOverview.ItemsSource);
            itemsView.Refresh();
        }

        private int CompareHorsesStakeShare(UCCompactHorse h1, UCCompactHorse h2)
        {
            try
            {
                if (h1.Horse.Scratched == true && h2.Horse.Scratched == true)
                {
                    return 0;
                }
                if (h1.Horse.Scratched == true)
                {
                    return 1;
                }
                if (h2.Horse.Scratched == true)
                {
                    return -1;
                }

                decimal exactDiff = h1.Horse.StakeDistributionShare - h2.Horse.StakeDistributionShare;
                if (exactDiff < 0)
                {
                    return 1;
                }
                if (exactDiff > 0)
                {
                    return -1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int CompareHorsesStartNr(UCCompactHorse h1, UCCompactHorse h2)
        {
            try
            {
                if (h1.Horse.Scratched == true && h2.Horse.Scratched == true)
                {
                    return 0;
                }
                if (h1.Horse.Scratched == true)
                {
                    return 1;
                }
                if (h2.Horse.Scratched == true)
                {
                    return -1;
                }

                int exactDiff = h1.Horse.StartNr - h2.Horse.StartNr;
                if (exactDiff < 0)
                {
                    return -1;
                }
                if (exactDiff > 0)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int CompareHorsesRankOwn(UCCompactHorse h1, UCCompactHorse h2)
        {
            try
            {
                if (h1.Horse.Scratched == true && h2.Horse.Scratched == true)
                {
                    return 0;
                }
                if (h1.Horse.Scratched == true)
                {
                    return 1;
                }
                if (h2.Horse.Scratched == true)
                {
                    return -1;
                }

                int exactDiff = h1.Horse.RankOwn - h2.Horse.RankOwn;
                if (exactDiff < 0)
                {
                    return -1;
                }
                if (exactDiff > 0)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int CompareHorsesRankTip(UCCompactHorse h1, UCCompactHorse h2)
        {
            try
            {
                if (h1.Horse.Scratched == true && h2.Horse.Scratched == true)
                {
                    return 0;
                }
                if (h1.Horse.Scratched == true)
                {
                    return 1;
                }
                if (h2.Horse.Scratched == true)
                {
                    return -1;
                }

                int exactDiff = (int)h1.Horse.RankTip - (int)h2.Horse.RankTip;
                if (exactDiff < 0)
                {
                    return -1;
                }
                if (exactDiff > 0)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int CompareHorsesRankWeighted(UCCompactHorse h1, UCCompactHorse h2)
        {
            try
            {
                if (h1.Horse.Scratched == true && h2.Horse.Scratched == true)
                {
                    return 0;
                }
                if (h1.Horse.Scratched == true)
                {
                    return 1;
                }
                if (h2.Horse.Scratched == true)
                {
                    return -1;
                }

                decimal exactDiff = h1.Horse.RankWeighted - h2.Horse.RankWeighted;
                if (exactDiff < 0M)
                {
                    return -1;
                }
                if (exactDiff > 0M)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int CompareHorsesVinnarOdds(UCCompactHorse h1, UCCompactHorse h2)
        {
            try
            {
                if (h1.Horse.Scratched == true && h2.Horse.Scratched == true)
                {
                    return 0;
                }
                if (h1.Horse.Scratched == true)
                {
                    return 1;
                }
                if (h2.Horse.Scratched == true)
                {
                    return -1;
                }

                decimal exactDiff = h1.Horse.VinnarOddsShare - h2.Horse.VinnarOddsShare;
                if (exactDiff < 0)
                {
                    return 1;
                }
                if (exactDiff > 0)
                {
                    return -1;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int CompareHorsesName(UCCompactHorse h1, UCCompactHorse h2)
        {
            try
            {
                return h1.Horse.HorseName.CompareTo(h2.Horse.HorseName);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
    }
}

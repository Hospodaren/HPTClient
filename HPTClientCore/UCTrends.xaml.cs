using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTrends.xaml
    /// </summary>
    public partial class UCTrends : UCMarkBetControl
    {
        public UCTrends()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                atgUpdateTimer = new Timer(new TimerCallback(GetRaceDayInfoHistory));

                InitializeComponent();
                lvwLopp.Items.GroupDescriptions.Add(new PropertyGroupDescription("ParentRace.LegNrString"));
                //this.staticColumns = new GridViewColumn[] { this.gvcStartNr, this.gvcABCD, this.gvcNamn, this.gvcRelativeDifference, this.gvcRelativeDifferenceVinnare, this.gvcRelativeDifferencePlats, this.gvcHistory };
                staticColumns = new GridViewColumn[] { gvcStartNr, gvcABCD, gvcNamn, gvcRelativeDifference, gvcHistory };
                //this.TimeList = new ObservableCollection<DateTime>();
            }
        }

        public ObservableCollection<DateTime> TimeList
        {
            get { return (ObservableCollection<DateTime>)GetValue(TimeListProperty); }
            set { SetValue(TimeListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeListProperty =
            DependencyProperty.Register("TimeList", typeof(ObservableCollection<DateTime>), typeof(UCTrends), new PropertyMetadata(new ObservableCollection<DateTime>()));



        private GridViewColumn[] staticColumns;

        private void btnGetRaceDayInfoHistory_Click(object sender, RoutedEventArgs e)
        {
            //this.btnGetRaceDayInfoHistory.IsEnabled = false;
            GetRaceDayInfoHistory();
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
            }
        }

        private void GetRaceDayInfoHistory()
        {
            try
            {
                // Deaktivera knappen så man inte ska kunna göra parallella anrop
                btnGetRaceDayInfoHistory.IsEnabled = false;

                var connector = new HPTServiceConnector();
                // TODO: Ny lösning
                //connector.GetRaceDayInfoHistoryGrouped(MarkBet.RaceDayInfo.BetType.Code, MarkBet.RaceDayInfo.TrackId, MarkBet.RaceDayInfo.RaceDayDate, GetRaceDayInfoHistory);
            }
            catch (Exception)
            {
                btnGetRaceDayInfoHistory.IsEnabled = true;
            }
            Cursor = Cursors.Arrow;
        }

        // TODO: Helt ny lösning behövs
        //private void GetRaceDayInfoHistory(HPTService.HPTRaceDayInfoHistoryInfoGrouped raceDayInfoHistory)
        //{
        //    try
        //    {
        //        Dispatcher.Invoke(new Action<HPTService.HPTRaceDayInfoHistoryInfoGrouped>(GetRaceDayInfoHistoryInvoke), raceDayInfoHistory);
        //    }
        //    catch (Exception)
        //    {
        //        btnGetRaceDayInfoHistory.IsEnabled = true;
        //    }
        //}

        //private void GetRaceDayInfoHistoryInvoke(HPTService.HPTRaceDayInfoHistoryInfoGrouped raceDayInfoHistory)
        //{
        //    try
        //    {
        //        // Konvertera datat
        //        HPTServiceToHPTHelper.ConvertRaceDayInfoHistory(raceDayInfoHistory, MarkBet.RaceDayInfo);

        //        // Se till att en av de hämtade blir vald
        //        if (SelectedIndex == 0)
        //        {
        //            SelectedTurnoverHistory = MarkBet.RaceDayInfo.TurnoverHistoryList.Last();
        //            SelectedIndex = MarkBet.RaceDayInfo.TurnoverHistoryList.Length - 1;
        //        }
        //        else
        //        {
        //            SelectedTurnoverHistory = MarkBet.RaceDayInfo.TurnoverHistoryList[SelectedIndex];
        //        }
        //        SelectedTurnoverHistory.IsSelected = true;

        //        // Uppdatera vilka tidpunkter som är tillgängliga
        //        TimeList.Clear();
        //        MarkBet.RaceDayInfo.TimestampListMarkBetHistory
        //            .ToList()
        //            .ForEach(ts => TimeList.Add(ts));

        //        // Uppdatera gränssnittet med ny data
        //        UpdateHistoryData();

        //        // Stäng av timern om den är igång
        //        if (TimeList.Max() > MarkBet.RaceDayInfo.BetType.StartTime)
        //        {
        //            cmbUpdateInterval.SelectedIndex = 0;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    btnGetRaceDayInfoHistory.IsEnabled = true;
        //    Cursor = Cursors.Arrow;
        //}

        private string GroupDescriptionName = "ParentRace.LegNrString";
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = e.OriginalSource as GridViewColumnHeader;
            if (column == null)
            {
                return;
            }

            String field = column.Tag as String;
            if (string.IsNullOrEmpty(field))
            {
                field = "StakeDistributionShare";
            }

            ListSortDirection newDir = ListSortDirection.Ascending;

            if (lvwLopp.Items.SortDescriptions.Count > 1)
            {
                SortDescription sd = lvwLopp.Items.SortDescriptions[1];
                if (sd.PropertyName == field)
                {
                    SortDescription sdNew = new SortDescription();
                    sdNew.PropertyName = sd.PropertyName;
                    sdNew.Direction = sd.Direction == ListSortDirection.Ascending
                                          ? ListSortDirection.Descending
                                          : ListSortDirection.Ascending;
                    lvwLopp.Items.SortDescriptions.RemoveAt(1);
                    lvwLopp.Items.SortDescriptions.Insert(1, sdNew);
                    return;
                }
            }

            // Hantera fält som ska sorteras fallande som default
            switch (field)
            {
                case "Markability":
                case "SelectedForComplimentaryRule":
                case "EarningsMeanLast5":
                case "TotalStatistics.Earning":
                case "TrioIndex":
                case "PercentFirst":
                case "PercentSecond":
                case "PercentThird":
                case "StakeDistributionShare":
                case "HistoryRelativeDifferenceUnadjusted":
                case "HistoryRelativeDifferenceVinnareUnadjusted":
                case "HistoryRelativeDifferencePlatsUnadjusted":
                    newDir = ListSortDirection.Descending;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(GroupDescriptionName))
            {
                // Aldrig sortering på mer än två variabler
                if (lvwLopp.Items.SortDescriptions.Count > 1)
                {
                    lvwLopp.Items.SortDescriptions.RemoveAt(1); ;
                }
                lvwLopp.Items.SortDescriptions.Insert(0, new SortDescription(field, newDir));
            }
            else
            {
                lvwLopp.Items.SortDescriptions.Clear();
                lvwLopp.Items.SortDescriptions.Add(new SortDescription(GroupDescriptionName, ListSortDirection.Ascending));
                lvwLopp.Items.SortDescriptions.Add(new SortDescription(field, newDir));
            }
        }

        //private void cmbChooseTrendData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (this.cmbChooseTrendData.SelectedItem != null && this.MarkBet != null && !this.MarkBet.IsDeserializing)
        //    {
        //        string historyType = (string)((ComboBoxItem)this.cmbChooseTrendData.SelectedItem).Tag;

        //        this.MarkBet.RaceDayInfo.RaceList
        //            .ForEach(r =>
        //                r.HorseList
        //                    .ToList()
        //                    .ForEach(h => h.SetHistoryRelativeDifferenceUnadjusted(this.SelectedIndex))  // Valt index på ComboBox med tider...
        //                    );

        //        string sortVariable = string.Empty;
        //        switch (historyType)
        //        {
        //            case "I":
        //                sortVariable = "HistoryRelativeDifferenceUnadjusted";
        //                break;
        //            case "V":
        //                sortVariable = "HistoryRelativeDifferenceVinnareUnadjusted";
        //                break;
        //            case "P":
        //                sortVariable = "HistoryRelativeDifferencePlatsUnadjusted";
        //                break;
        //            default:
        //                break;
        //        }

        //        this.gvwVxxSpel.Columns.Clear();

        //        this.staticColumns
        //            //.Where(sc => sc != null)
        //            .ToList()
        //            .ForEach(sc => this.gvwVxxSpel.Columns.Add(sc));

        //        this.chkShowAllTrends_Checked(this.chkShowAllTrends, new RoutedEventArgs());

        //        if (string.IsNullOrEmpty(this.GroupDescriptionName))
        //        {
        //            // Aldrig sortering på mer än två variabler
        //            if (this.lvwLopp.Items.SortDescriptions.Count > 1)
        //            {
        //                this.lvwLopp.Items.SortDescriptions.RemoveAt(1); ;
        //            }
        //            this.lvwLopp.Items.SortDescriptions.Insert(0, new SortDescription(sortVariable, ListSortDirection.Descending));
        //        }
        //        else
        //        {
        //            this.lvwLopp.Items.SortDescriptions.Clear();
        //            this.lvwLopp.Items.SortDescriptions.Add(new SortDescription(this.GroupDescriptionName, ListSortDirection.Ascending));
        //            this.lvwLopp.Items.SortDescriptions.Add(new SortDescription(sortVariable, ListSortDirection.Descending));
        //        }

        //        if (this.MarkBet.HasRankReduction())
        //        {
        //            this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
        //        }
        //        else
        //        {
        //            this.MarkBet.RecalculateRank();
        //        }

        //        this.lvwLopp.Items.Refresh();
        //    }
        //}

        private void UpdateHistoryData()
        {
            if (MarkBet != null && !MarkBet.IsDeserializing)
            {
                MarkBet.RaceDayInfo.RaceList
                    .ForEach(r =>
                        r.HorseList
                            .ToList()
                            .ForEach(h => h.SetHistoryRelativeDifferenceUnadjusted(SelectedIndex))  // Valt index på ComboBox med tider...
                            );

                gvwVxxSpel.Columns.Clear();

                staticColumns
                    .ToList()
                    .ForEach(sc => gvwVxxSpel.Columns.Add(sc));

                if (string.IsNullOrEmpty(GroupDescriptionName))
                {
                    // Aldrig sortering på mer än två variabler
                    if (lvwLopp.Items.SortDescriptions.Count > 1)
                    {
                        lvwLopp.Items.SortDescriptions.RemoveAt(1); ;
                    }
                    lvwLopp.Items.SortDescriptions.Insert(0, new SortDescription("HistoryRelativeDifferenceUnadjusted", ListSortDirection.Descending));
                }
                else
                {
                    lvwLopp.Items.SortDescriptions.Clear();
                    lvwLopp.Items.SortDescriptions.Add(new SortDescription(GroupDescriptionName, ListSortDirection.Ascending));
                    lvwLopp.Items.SortDescriptions.Add(new SortDescription("HistoryRelativeDifferenceUnadjusted", ListSortDirection.Descending));
                }

                if (MarkBet.HasRankReduction())
                {
                    MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
                else
                {
                    MarkBet.RecalculateRank();
                }
                //this.MarkBet.HorseList.Clear();
                lvwLopp.Items.Refresh();
            }
        }

        Timer atgUpdateTimer;
        private void GetRaceDayInfoHistory(object timerData)
        {
            Dispatcher.Invoke(GetRaceDayInfoHistory);
        }

        private void cmbUpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUpdateInterval.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)cmbUpdateInterval.SelectedItem;
                int updatePeriod = Convert.ToInt32(cbi.Tag) * 60 * 1000;
                if (updatePeriod == 0)
                {
                    atgUpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
                else
                {
                    atgUpdateTimer.Change(updatePeriod, updatePeriod);
                }
            }
        }

        //private void chkShowAllTrends_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (this.chkShowAllTrends.IsChecked == true)
        //    {
        //        this.gvwVxxSpel.Columns.Remove(this.CurrentHistoryColumn);
        //        this.gvwVxxSpel.Columns.Remove(this.CurrentRelativeDifferenceColumn);
        //        this.RelativeDifferenceColumnList
        //            .ForEach(gvc =>
        //            {
        //                if (!this.gvwVxxSpel.Columns.Contains(gvc))
        //                {
        //                    this.gvwVxxSpel.Columns.Add(gvc);
        //                }
        //            });
        //        this.gvwVxxSpel.Columns.Add(this.CurrentHistoryColumn);
        //    }
        //    else
        //    {
        //        this.gvwVxxSpel.Columns.Remove(this.CurrentHistoryColumn);
        //        this.RelativeDifferenceColumnList
        //            .ForEach(gvc =>
        //            {
        //                if (this.gvwVxxSpel.Columns.Contains(gvc))
        //                {
        //                    this.gvwVxxSpel.Columns.Remove(gvc);
        //                }
        //            });                
        //        this.gvwVxxSpel.Columns.Add(this.CurrentRelativeDifferenceColumn);
        //        this.gvwVxxSpel.Columns.Add(this.CurrentHistoryColumn);
        //    }
        //}

        //private List<GridViewColumn> RelativeDifferenceColumnList
        //{
        //    get
        //    {
        //        switch (this.cmbChooseTrendData.SelectedIndex)
        //        {
        //            case 0:
        //                return new List<GridViewColumn>()
        //                {
        //                    this.gvcRelativeDifference,
        //                    this.gvcRelativeDifferenceVinnare,
        //                    this.gvcRelativeDifferencePlats
        //                };
        //            case 1:
        //                return new List<GridViewColumn>()
        //                {
        //                    this.gvcRelativeDifferenceVinnare,
        //                    this.gvcRelativeDifference,
        //                    this.gvcRelativeDifferencePlats
        //                };
        //            case 2:
        //                return new List<GridViewColumn>()
        //                {
        //                    this.gvcRelativeDifferencePlats,
        //                    this.gvcRelativeDifference,
        //                    this.gvcRelativeDifferenceVinnare
        //                };
        //            default:
        //                return new List<GridViewColumn>();
        //        }
        //    }
        //}

        //private GridViewColumn CurrentRelativeDifferenceColumn
        //{
        //    get
        //    {
        //        switch (this.cmbChooseTrendData.SelectedIndex)
        //        {
        //            case 0:
        //                return this.gvcRelativeDifference;
        //            case 1:
        //                return this.gvcRelativeDifferenceVinnare;
        //            case 2:
        //                return this.gvcRelativeDifferencePlats;
        //            default:
        //                return null;
        //        }
        //    }
        //}

        //private GridViewColumn CurrentHistoryColumn
        //{
        //    get
        //    {
        //        switch (this.cmbChooseTrendData.SelectedIndex)
        //        {
        //            case 0:
        //                return this.gvcHistory;
        //            case 1:
        //                return this.gvcHistoryVinnare;
        //            case 2:
        //                return this.gvcHistoryPlats;
        //            default:
        //                return null;
        //        }
        //    }
        //}

        public HPTTurnoverHistory SelectedTurnoverHistory { get; set; }

        private void btnSelectStartPoint_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTurnoverHistory != null)
            {
                SelectedTurnoverHistory.IsSelected = false;
            }

            var fe = (FrameworkElement)sender;
            SelectedTurnoverHistory = (HPTTurnoverHistory)fe.DataContext;
            SelectedIndex = Array.FindIndex<HPTTurnoverHistory>(MarkBet.RaceDayInfo.TurnoverHistoryList, toh => toh.Turnover == SelectedTurnoverHistory.Turnover);
            SelectedTurnoverHistory.IsSelected = true;
            UpdateHistoryData();
            //cmbChooseTrendData_SelectionChanged(new object(), null);
        }
    }
}

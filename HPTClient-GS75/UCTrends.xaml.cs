﻿using System;
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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTrends.xaml
    /// </summary>
    public partial class UCTrends : UCMarkBetControl
    {
        public UCTrends()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.atgUpdateTimer = new System.Threading.Timer(new System.Threading.TimerCallback(GetRaceDayInfoHistory));

                InitializeComponent();
                this.lvwLopp.Items.GroupDescriptions.Add(new PropertyGroupDescription("ParentRace.LegNrString"));
                //this.staticColumns = new GridViewColumn[] { this.gvcStartNr, this.gvcABCD, this.gvcNamn, this.gvcRelativeDifference, this.gvcRelativeDifferenceVinnare, this.gvcRelativeDifferencePlats, this.gvcHistory };
                this.staticColumns = new GridViewColumn[] { this.gvcStartNr, this.gvcABCD, this.gvcNamn, this.gvcRelativeDifference, this.gvcHistory };
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
                return this.selectedIndex;
            }
            set
            {
                this.selectedIndex = value;
            }
        }

        private void GetRaceDayInfoHistory()
        {
            try
            {
                // Deaktivera knappen så man inte ska kunna göra parallella anrop
                this.btnGetRaceDayInfoHistory.IsEnabled = false;

                var connector = new HPTServiceConnector();
                connector.GetRaceDayInfoHistoryGrouped(this.MarkBet.RaceDayInfo.BetType.Code, this.MarkBet.RaceDayInfo.TrackId, this.MarkBet.RaceDayInfo.RaceDayDate, GetRaceDayInfoHistory);
            }
            catch (Exception exc)
            {
                this.btnGetRaceDayInfoHistory.IsEnabled = true;
            }
            Cursor = Cursors.Arrow;
        }

        private void GetRaceDayInfoHistory(HPTService.HPTRaceDayInfoHistoryInfoGrouped raceDayInfoHistory)
        {
            try
            {
                Dispatcher.Invoke(new Action<HPTService.HPTRaceDayInfoHistoryInfoGrouped>(GetRaceDayInfoHistoryInvoke), raceDayInfoHistory);
            }
            catch (Exception exc)
            {
                this.btnGetRaceDayInfoHistory.IsEnabled = true;
            }
        }

        private void GetRaceDayInfoHistoryInvoke(HPTService.HPTRaceDayInfoHistoryInfoGrouped raceDayInfoHistory)
        {
            try
            {
                // Konvertera datat
                HPTServiceToHPTHelper.ConvertRaceDayInfoHistory(raceDayInfoHistory, this.MarkBet.RaceDayInfo);

                // Se till att en av de hämtade blir vald
                if (this.SelectedIndex == 0)
                {
                    this.SelectedTurnoverHistory = this.MarkBet.RaceDayInfo.TurnoverHistoryList.Last();
                    this.SelectedIndex = this.MarkBet.RaceDayInfo.TurnoverHistoryList.Length - 1;
                }
                else
                {
                    this.SelectedTurnoverHistory = this.MarkBet.RaceDayInfo.TurnoverHistoryList[this.SelectedIndex];
                }
                this.SelectedTurnoverHistory.IsSelected = true;

                // Uppdatera vilka tidpunkter som är tillgängliga
                this.TimeList.Clear();
                this.MarkBet.RaceDayInfo.TimestampListMarkBetHistory
                    .ToList()
                    .ForEach(ts => this.TimeList.Add(ts));

                // Uppdatera gränssnittet med ny data
                UpdateHistoryData();

                // Stäng av timern om den är igång
                if (this.TimeList.Max() > this.MarkBet.RaceDayInfo.BetType.StartTime)
                {
                    this.cmbUpdateInterval.SelectedIndex = 0;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            this.btnGetRaceDayInfoHistory.IsEnabled = true;
            Cursor = Cursors.Arrow;
        }

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

            if (this.lvwLopp.Items.SortDescriptions.Count > 1)
            {
                SortDescription sd = this.lvwLopp.Items.SortDescriptions[1];
                if (sd.PropertyName == field)
                {
                    SortDescription sdNew = new SortDescription();
                    sdNew.PropertyName = sd.PropertyName;
                    sdNew.Direction = sd.Direction == ListSortDirection.Ascending
                                          ? ListSortDirection.Descending
                                          : ListSortDirection.Ascending;
                    this.lvwLopp.Items.SortDescriptions.RemoveAt(1);
                    this.lvwLopp.Items.SortDescriptions.Insert(1, sdNew);
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

            if (string.IsNullOrEmpty(this.GroupDescriptionName))
            {
                // Aldrig sortering på mer än två variabler
                if (this.lvwLopp.Items.SortDescriptions.Count > 1)
                {
                    this.lvwLopp.Items.SortDescriptions.RemoveAt(1); ;
                }
                this.lvwLopp.Items.SortDescriptions.Insert(0, new SortDescription(field, newDir));
            }
            else
            {
                this.lvwLopp.Items.SortDescriptions.Clear();
                this.lvwLopp.Items.SortDescriptions.Add(new SortDescription(this.GroupDescriptionName, ListSortDirection.Ascending));
                this.lvwLopp.Items.SortDescriptions.Add(new SortDescription(field, newDir));
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
            if (this.MarkBet != null && !this.MarkBet.IsDeserializing)
            {
                this.MarkBet.RaceDayInfo.RaceList
                    .ForEach(r =>
                        r.HorseList
                            .ToList()
                            .ForEach(h => h.SetHistoryRelativeDifferenceUnadjusted(this.SelectedIndex))  // Valt index på ComboBox med tider...
                            );
                
                this.gvwVxxSpel.Columns.Clear();

                this.staticColumns
                    .ToList()
                    .ForEach(sc => this.gvwVxxSpel.Columns.Add(sc));
                
                if (string.IsNullOrEmpty(this.GroupDescriptionName))
                {
                    // Aldrig sortering på mer än två variabler
                    if (this.lvwLopp.Items.SortDescriptions.Count > 1)
                    {
                        this.lvwLopp.Items.SortDescriptions.RemoveAt(1); ;
                    }
                    this.lvwLopp.Items.SortDescriptions.Insert(0, new SortDescription("HistoryRelativeDifferenceUnadjusted", ListSortDirection.Descending));
                }
                else
                {
                    this.lvwLopp.Items.SortDescriptions.Clear();
                    this.lvwLopp.Items.SortDescriptions.Add(new SortDescription(this.GroupDescriptionName, ListSortDirection.Ascending));
                    this.lvwLopp.Items.SortDescriptions.Add(new SortDescription("HistoryRelativeDifferenceUnadjusted", ListSortDirection.Descending));
                }

                if (this.MarkBet.HasRankReduction())
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
                else
                {
                    this.MarkBet.RecalculateRank();
                }

                this.lvwLopp.Items.Refresh();
            }
        }

        System.Threading.Timer atgUpdateTimer;
        private void GetRaceDayInfoHistory(object timerData)
        {
            this.Dispatcher.Invoke(GetRaceDayInfoHistory);
        }

        private void cmbUpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbUpdateInterval.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)this.cmbUpdateInterval.SelectedItem;
                int updatePeriod = Convert.ToInt32(cbi.Tag) * 60 * 1000;
                if (updatePeriod == 0)
                {
                    this.atgUpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
                else
                {
                    this.atgUpdateTimer.Change(updatePeriod, updatePeriod);
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
            if (this.SelectedTurnoverHistory != null)
            {
                this.SelectedTurnoverHistory.IsSelected = false;
            }

            var fe = (FrameworkElement)sender;
            this.SelectedTurnoverHistory = (HPTTurnoverHistory)fe.DataContext;
            this.SelectedIndex = Array.FindIndex<HPTTurnoverHistory>(this.MarkBet.RaceDayInfo.TurnoverHistoryList, toh => toh.Turnover == this.SelectedTurnoverHistory.Turnover);
            this.SelectedTurnoverHistory.IsSelected = true;
            UpdateHistoryData();
            //cmbChooseTrendData_SelectionChanged(new object(), null);
        }
    }
}

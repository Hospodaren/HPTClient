using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCHorseRankSumReduction.xaml
    /// </summary>
    public partial class UCHorseRankSumReduction : UCMarkBetControl
    {
        public UCHorseRankSumReduction()
        {
            InitializeComponent();
        }

        #region Hantera ändringar i reduceringsvillkoren

        private void icGroupRule_Checked(object sender, RoutedEventArgs e)
        {
            MarkBet.RecalculateReduction(RecalculateReason.Rank);
        }

        private void btnNewGroupRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                var rule = (HPTHorseRankSumReductionRule)btn.DataContext;
                if (rule.ReductionRuleList == null)
                {
                    rule.ReductionRuleList = new System.Collections.ObjectModel.ObservableCollection<HPTHorseRankReductionRule>();
                }
                rule.ReductionRuleList.Add(new HPTHorseRankReductionRule(rule.NumberOfRaces, true)
                {
                    ParentHorseRankSumReductionRule = rule,
                    Name = rule.PropertyName
                });
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnClearGroupRules_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                var rule = (HPTHorseRankSumReductionRule)btn.DataContext;
                if (rule.ReductionRuleList != null && rule.ReductionRuleList.Count > 0)
                {
                    rule.ReductionRuleList.Clear();
                    MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void iudMinSum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var iud = (IntegerUpDown)sender;
                var rule = (HPTHorseRankSumReductionRule)iud.DataContext;
                if (rule.Use)
                {
                    MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void chkUseSumReductionRule_Checked(object sender, RoutedEventArgs e)
        {
            //int numberOfRulesToUse = this.MarkBet.HorseRankSumReductionRuleList.Count(hr => hr.Use);
            //if (numberOfRulesToUse == 0)
            //{
            //    this.MarkBet.ReductionHorseRank = false;
            //}
            //this.MarkBet.ReductionHorseRank = 
            MarkBet.RecalculateReduction(RecalculateReason.Rank);
        }

        private void iudMinNumberOfWinners_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var iud = (IntegerUpDown)sender;
                var rule = (HPTHorseRankReductionRule)iud.DataContext;
                MarkBet.RecalculateReduction(RecalculateReason.Rank);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void dudMinValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MarkBet.ReductionRank)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Rank);
            }
        }

        private void btnRemoveRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                var rule = (HPTHorseRankReductionRule)btn.DataContext;
                rule.ParentHorseRankSumReductionRule.ReductionRuleList.Remove(rule);
                rule.ParentHorseRankSumReductionRule = null;
                MarkBet.RecalculateReduction(RecalculateReason.Rank);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #endregion

        #region Visa hästar i gruppregel

        private System.Windows.Controls.Primitives.Popup pu;
        // KOMMANDE
        private void btnShowGroupRuleHorses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                var rule = (HPTHorseRankReductionRule)btn.DataContext;
                if (pu == null)
                {
                    pu = new System.Windows.Controls.Primitives.Popup()
                    {
                        Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                        PlacementTarget = btn//,
                        //HorizontalOffset = -10D,
                        //VerticalOffset = -10D
                    };
                    pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
                }

                // Skapa innehållet för popupen
                Border b = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1D),
                    Width = double.NaN,
                    Child = new UCRaceViewGrouped()
                    {
                        BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                        BorderThickness = new Thickness(6D),
                        Width = double.NaN,
                        GroupDescriptionName = "ParentRace.LegNrString"
                    }
                };

                // Plocka ut hästarna som ligger i intervallet
                var pi = rule.ParentHorseRankSumReductionRule.HorseRankVariable.HorseProperty;

                //var selectedNumberOfWinners = rule.NumberOfWinnersList.Where(now => now.Selected).Select(now => now.NumberOfWinners);
                var horseList = new List<HPTHorse>();
                foreach (var horse in MarkBet.RaceDayInfo.HorseListSelected)
                {
                    var horseRank = horse.RankList.FirstOrDefault(hr => hr.Name == rule.Name);
                    if (horseRank != null && horseRank.Rank >= rule.LowerBoundary && horseRank.Rank <= rule.UpperBoundary)
                    {
                        horseList.Add(horse);
                    }
                }


                IOrderedEnumerable<HPTHorse> orderedHorseList = horseList
                    .OrderBy(h => h.ParentRace.LegNr)
                    .ThenBy(h => h.StartNr);

                // Skapa en IHorseListContainer med valda hästar
                HPTHorseListContainer horseCollection = new HPTHorseListContainer()
                {
                    //HorseList = new System.Collections.ObjectModel.ObservableCollection<HPTHorse>(orderedHorseList),
                    HorseList = new List<HPTHorse>(orderedHorseList),
                    ParentRaceDayInfo = new HPTRaceDayInfo()
                    {
                        DataToShow = new HPTHorseDataToShow()
                        {
                            ShowLegNrText = true,
                            ShowStartNr = true,
                            ShowName = true,
                            ShowPrio = true,
                            ShowVinnarOdds = true,
                            ShowStakeDistributionPercent = true,
                            ShowMarksPercent = true,
                            ShowVinnarOddsShare = true,
                            ShowPlatsOdds = true,
                            ShowRankATG = true,
                            ShowRankOwn = true,
                            ShowRankMean = true,
                            ShowMarkability = true,
                            ShowDaysSinceLastStart = true
                        }
                    }
                };

                // Visa popupen
                pu.DataContext = horseCollection;
                pu.Child = b;
                pu.IsOpen = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        // KOMMANDE
        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            var pu = (System.Windows.Controls.Primitives.Popup)sender;
            pu.Child = null;
            pu.IsOpen = false;
        }

        #endregion

        private void btnSelectTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (cmbHorseRankSumRulesCollection.SelectedItem != null)
            {
                if (MarkBet.HorseRankSumReductionRuleList == null || MarkBet.HorseRankSumReductionRuleList.Count == 0)
                {
                    return;
                }

                var horseRankSumReductionRuleCollection = (HPTHorseRankSumReductionRuleCollection)cmbHorseRankSumRulesCollection.SelectedItem;

                bool recalculationPaused = MarkBet.pauseRecalculation;
                MarkBet.pauseRecalculation = true;
                foreach (var horseRankSumReductionRule in horseRankSumReductionRuleCollection.RankSumReductionRuleList)
                {
                    var ruleToSet = MarkBet.HorseRankSumReductionRuleList.FirstOrDefault(r => r.PropertyName == horseRankSumReductionRule.PropertyName);
                    if (ruleToSet != null)
                    {
                        ruleToSet.ApplyRule(horseRankSumReductionRule);
                        ruleToSet.Use = true;
                    }
                }
                MarkBet.pauseRecalculation = recalculationPaused;
                if (MarkBet.ReductionHorseRank)
                {
                    MarkBet.RecalculateReduction(RecalculateReason.Other);
                }
            }
        }

        private void btnSaveAsTemplate_Click(object sender, RoutedEventArgs e)
        {
            string templateName = txtNewTemplateName.Text;
            if (string.IsNullOrEmpty(templateName))
            {
                templateName = "Ny rankreduceringsmall " + DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToShortTimeString();
                txtNewTemplateName.Text = templateName;
            }
            Config.RankSumReductionRuleCollection.Add(new HPTHorseRankSumReductionRuleCollection()
            {
                TypeCategory = MarkBet.BetType.TypeCategory,
                Name = templateName,
                RankSumReductionRuleList = new System.Collections.ObjectModel.ObservableCollection<HPTHorseRankSumReductionRule>(MarkBet.HorseRankSumReductionRuleList.Where(r => r.Use).Select(r => r.Clone()))
            });
        }
    }
}

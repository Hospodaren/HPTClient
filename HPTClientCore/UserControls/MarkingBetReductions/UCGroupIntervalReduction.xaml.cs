using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCGroupIntervalReduction.xaml
    /// </summary>
    public partial class UCGroupIntervalReduction : UCMarkBetControl
    {
        public UCGroupIntervalReduction()
        {
            InitializeComponent();
        }

        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)e.OriginalSource;
            ItemsControl ic = (ItemsControl)sender;

            HPTGroupIntervalReductionRule rule = (HPTGroupIntervalReductionRule)ic.DataContext;
            if (rule.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTGroupIntervalReductionRule rule = (HPTGroupIntervalReductionRule)btn.DataContext;
            MarkBet.GroupIntervalRulesCollection.ReductionRuleList.Remove(rule);
            if (rule.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void cmbVariable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            HPTGroupIntervalReductionRule rule = (HPTGroupIntervalReductionRule)cmb.Tag;
            HPTHorseVariable hv = (HPTHorseVariable)cmb.SelectedItem;
            if (rule.HorseVariable == null || rule.HorseVariable.PropertyName != hv.PropertyName)
            {
                rule.HorseVariable = hv;
                rule.LowerBoundary = rule.HorseVariable.GroupReductionInfo.MinValue;
                rule.UpperBoundary = rule.HorseVariable.GroupReductionInfo.MaxValue;
            }
        }

        private void btnNewRule_Click(object sender, RoutedEventArgs e)
        {
            if (MarkBet != null)
            {
                HPTGroupIntervalReductionRule rule = new HPTGroupIntervalReductionRule(MarkBet.RaceDayInfo.RaceList.Count, false);
                MarkBet.GroupIntervalRulesCollection.ReductionRuleList.Add(rule);
            }
        }

        private void btnAddNewRule_Click(object sender, RoutedEventArgs e)
        {
            if (MarkBet != null)
            {
                HPTGroupIntervalReductionRule rule = new HPTGroupIntervalReductionRule(MarkBet.RaceDayInfo.RaceList.Count, false);
                MarkBet.GroupIntervalRulesCollection.ReductionRuleList.Add(rule);
            }
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            bool recalculationPaused = MarkBet.pauseRecalculation;
            MarkBet.pauseRecalculation = true;
            try
            {
                foreach (HPTGroupIntervalReductionRule rule in MarkBet.GroupIntervalRulesCollection.ReductionRuleList)
                {
                    rule.Use = false;
                }
                MarkBet.GroupIntervalRulesCollection.ReductionRuleList.Clear();
            }
            catch (Exception exc)
            {
                Config.AddToErrorLog(exc);
            }
            MarkBet.pauseRecalculation = recalculationPaused;
            MarkBet.RecalculateReduction(RecalculateReason.All);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MarkBet.RecalculateReduction(RecalculateReason.Other);
            //if (this.MarkBet.GroupIntervalRulesCollection.Use)
            //{
            //    this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            //}
        }

        private void Dud_ValueChanged(object sender, RoutedEventArgs e)
        {
            var dud = (DecimalUpDown)sender;
            var groupIntervalReductionRule = (HPTGroupIntervalReductionRule)dud.DataContext;
            if (groupIntervalReductionRule.Use && MarkBet.GroupIntervalRulesCollection.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private HPTGroupIntervalRulesCollection newGroupIntervalRulesCollection;
        private void btnSaveAsTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (newGroupIntervalRulesCollection == null)
            {
                newGroupIntervalRulesCollection = new HPTGroupIntervalRulesCollection(MarkBet.GroupIntervalRulesCollection.NumberOfRaces, MarkBet.GroupIntervalRulesCollection.Use);
            }
            newGroupIntervalRulesCollection.TypeCategory = MarkBet.BetType.TypeCategory;
            newGroupIntervalRulesCollection.ReductionRuleList.Clear();
            foreach (HPTGroupIntervalReductionRule rule in MarkBet.GroupIntervalRulesCollection.ReductionRuleList)
            {
                newGroupIntervalRulesCollection.ReductionRuleList.Add(rule.Clone());
            }
            if (string.IsNullOrEmpty(txtNewTemplateName.Text))
            {
                newGroupIntervalRulesCollection.Name = "Gruppintervall " + MarkBet.BetType.Code + " " + MarkBet.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd"); ;
            }
            else
            {
                newGroupIntervalRulesCollection.Name = txtNewTemplateName.Text;
            }
            if (!HPTConfig.Config.GroupIntervalRulesCollectionList.Contains(newGroupIntervalRulesCollection))
            {
                HPTConfig.Config.GroupIntervalRulesCollectionList.Add(newGroupIntervalRulesCollection);
            }
        }

        private void btnSelectTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGroupIntervalRulesCollection.SelectedItem != null)
            {
                var groupIntervalRulesCollection = (HPTGroupIntervalRulesCollection)cmbGroupIntervalRulesCollection.SelectedItem;

                bool recalculationPaused = MarkBet.pauseRecalculation;
                MarkBet.pauseRecalculation = true;
                if (MarkBet.GroupIntervalRulesCollection.ReductionRuleList == null)
                {
                    MarkBet.GroupIntervalRulesCollection.ReductionRuleList = new ObservableCollection<HPTNumberOfWinnersReductionRule>();
                }
                else
                {
                    MarkBet.GroupIntervalRulesCollection.ReductionRuleList.Clear();
                }
                foreach (HPTGroupIntervalReductionRule groupIntervalReductionRule in groupIntervalRulesCollection.ReductionRuleList)
                {
                    groupIntervalReductionRule.NumberOfRaces = groupIntervalReductionRule.NumberOfWinnersList.Max(now => now.NumberOfWinners);
                    var clonedRule = groupIntervalReductionRule.Clone();
                    MarkBet.GroupIntervalRulesCollection.ReductionRuleList.Add(clonedRule);
                }
                MarkBet.pauseRecalculation = recalculationPaused;
                if (MarkBet.GroupIntervalRulesCollection.Use)
                {
                    MarkBet.RecalculateReduction(RecalculateReason.Other);
                }
            }
        }

        private void itNumberOfGroupIntervalRules_Checked(object sender, RoutedEventArgs e)
        {
            if (MarkBet.GroupIntervalRulesCollection.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private System.Windows.Controls.Primitives.Popup pu;
        private void btnShowHorses_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var rule = (HPTGroupIntervalReductionRule)btn.DataContext;
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
            var pi = rule.HorseVariable.HorseProperty;
            IOrderedEnumerable<HPTHorse> orderedHorseList = MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => Convert.ToDecimal(pi.GetValue(h, null)) >= rule.LowerBoundary
                    && Convert.ToDecimal(pi.GetValue(h, null)) <= rule.UpperBoundary)
                .OrderBy(h => h.ParentRace.LegNr)
                .ThenBy(h => h.StartNr);

            if (rule.OnlyInSpecifiedLegs)
            {
                orderedHorseList = orderedHorseList
                    .Where(h => rule.LegSelectionList.First(l => l.LegNumber == h.ParentRace.LegNr).Selected)
                    .OrderBy(h => h.ParentRace.LegNr)
                    .ThenBy(h => h.StartNr);
            }

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

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            var pu = (System.Windows.Controls.Primitives.Popup)sender;
            pu.Child = null;
            pu.IsOpen = false;
        }
    }
}

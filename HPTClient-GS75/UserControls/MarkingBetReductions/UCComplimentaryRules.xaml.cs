using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCComplimentaryRules.xaml
    /// </summary>
    public partial class UCComplimentaryRules : UCMarkBetControl, IHorseListContainer
    {
        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }
        public ICollection<HPTHorse> HorseList { get; set; }

        public UCComplimentaryRules()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.ParentRaceDayInfo = new HPTRaceDayInfo()
                {
                    DataToShow = HPTConfig.Config.DataToShowComplementaryRules
                };
                this.HorseList = new ObservableCollection<HPTHorse>();
            }
            InitializeComponent();
        }

        private void btnNewRule_Click(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
        }

        private void CreateNewRule()
        {
            HPTComplementaryReductionRule oldRule = this.CurrentComplimentaryReductionRule;
            HPTComplementaryReductionRule newRule = new HPTComplementaryReductionRule(this.MarkBet.RaceDayInfo.RaceList.Count, true);

            this.MarkBet.ComplementaryRulesCollection.ReductionRuleList.Add(newRule);
            this.CurrentComplimentaryReductionRule = newRule;

            if (oldRule != null)
            {
                //oldRule.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(newRule_PropertyChanged);
                if (oldRule.HorseList.Count == 0)
                {
                    this.MarkBet.ComplementaryRulesCollection.ReductionRuleList.Remove(oldRule);

                }
                else if (oldRule.HorseList.Count > 0)
                {
                    List<HPTHorse> horsesToUnselect = oldRule.HorseList.Where(h => h.SelectedForComplimentaryRule).ToList();
                    foreach (var hptHorse in horsesToUnselect)
                    {
                        hptHorse.SelectedForComplimentaryRule = false;
                    }
                }
            }
            newRule.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(newRule_PropertyChanged);
        }

        void newRule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Use")
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.All);
            }
        }

        public HPTComplementaryReductionRule CurrentComplimentaryReductionRule
        {
            get { return (HPTComplementaryReductionRule)GetValue(CurrentComplimentaryReductionRuleProperty); }
            set { SetValue(CurrentComplimentaryReductionRuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentComplimentaryReductionRule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentComplimentaryReductionRuleProperty =
            DependencyProperty.Register("CurrentComplimentaryReductionRule", typeof(HPTComplementaryReductionRule), typeof(UCComplimentaryRules), new UIPropertyMetadata(null));

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            try
            {
                this.MarkBet.pauseRecalculation = true;
                Button btn = (Button)sender;
                HPTComplementaryReductionRule rule = (HPTComplementaryReductionRule)btn.DataContext;
                for (int i = rule.HorseList.Count - 1; i >= 0; i--)
                {
                    //rule.HorseList[i].SelectedForComplimentaryRule = false;
                    rule.HorseList.ElementAt(i).SelectedForComplimentaryRule = false;
                }

                // Recalculate
                this.MarkBet.ComplementaryRulesCollection.ReductionRuleList.Remove(rule);
                this.MarkBet.pauseRecalculation = recalculationPaused;
                if (rule.Use && this.MarkBet.ComplementaryRulesCollection.Use)
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Other);
                }
            }
            catch (Exception exc)
            {
                this.MarkBet.pauseRecalculation = recalculationPaused;
                Config.AddToErrorLog(exc);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            try
            {
                this.MarkBet.pauseRecalculation = true;
                Button btn = (Button)sender;
                HPTComplementaryReductionRule rule = (HPTComplementaryReductionRule)btn.DataContext;

                HPTComplementaryReductionRule oldRule = this.CurrentComplimentaryReductionRule;
                this.CurrentComplimentaryReductionRule = rule;

                if (oldRule != null && oldRule != rule)
                {
                    for (int i = oldRule.HorseList.Count - 1; i >= 0; i--)
                    {
                        //oldRule.HorseList[i].SelectedForComplimentaryRule = false;
                        oldRule.HorseList.ElementAt(i).SelectedForComplimentaryRule = false;
                    }
                }

                foreach (HPTHorse horse in rule.HorseList)
                {
                    horse.SelectedForComplimentaryRule = true;
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            try
            {
                this.MarkBet.pauseRecalculation = true;
                foreach (HPTComplementaryReductionRule rule in this.MarkBet.ComplementaryRulesCollection.ReductionRuleList)
                {
                    for (int i = rule.HorseList.Count - 1; i >= 0; i--)
                    {
                        //rule.HorseList[i].SelectedForComplimentaryRule = false;
                        rule.HorseList.ElementAt(i).SelectedForComplimentaryRule = false;
                    }
                }
                this.MarkBet.ComplementaryRulesCollection.ReductionRuleList.Clear();

                if (this.cmbTemplateList.SelectedIndex != -1)
                {
                    this.cmbTemplateList.SelectedIndex = -1;
                }
                this.MarkBet.pauseRecalculation = recalculationPaused;
                this.MarkBet.RecalculateReduction(RecalculateReason.All);
            }
            catch (Exception exc)
            {
                this.MarkBet.pauseRecalculation = recalculationPaused;
                Config.AddToErrorLog(exc);
            }
        }

        private void UCRaceView_Checked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(CheckBox))
            {
                return;
            }
            try
            {
                CheckBox chk = (CheckBox)e.OriginalSource;
                if (chk.Name == "chkComplimentarySelect")
                {
                    HPTHorse horse = (HPTHorse)chk.DataContext;
                    if (this.CurrentComplimentaryReductionRule == null || this.CurrentComplimentaryReductionRule.NumberOfWinnersList == null)
                    {
                        btnNewRule_Click(this.btnNewRule, new RoutedEventArgs());
                    }
                    if ((bool)chk.IsChecked)
                    {
                        if (!this.CurrentComplimentaryReductionRule.HorseList.Contains(horse))
                        {
                            this.CurrentComplimentaryReductionRule.HorseList.Add(horse);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (!this.MarkBet.pauseRecalculation)   // Manuell uppdatering av häst, inte föranledd av Välj-knappen
                            {
                                this.CurrentComplimentaryReductionRule.HorseList.Remove(horse);
                            }
                        }
                        catch (Exception exc)
                        {
                            string s = exc.Message;
                            HPTConfig.AddToErrorLogStatic(exc);
                        }
                    }
                    this.CurrentComplimentaryReductionRule.UpdateSelectable();
                    if (this.CurrentComplimentaryReductionRule.NumberOfSelected > 0 && this.MarkBet.ComplementaryRulesCollection.Use && !this.MarkBet.pauseRecalculation)
                    {
                        this.MarkBet.RecalculateReduction(RecalculateReason.Other);
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void ucComplimentaryRules_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (e.NewValue != null && e.NewValue.GetType() == typeof(HPTMarkBet))
                {
                    this.HorseList = this.MarkBet.RaceDayInfo.HorseListSelected;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }
            ItemsControl it = (ItemsControl)sender;
            HPTComplementaryReductionRule rule = (HPTComplementaryReductionRule)it.DataContext;
            if (this.MarkBet.ComplementaryRulesCollection.Use && rule.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        #region Predefined complementary rules

        private void cbiMarksNumberOne_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            this.MarkBet.RecalculateAllRanks();
            IEnumerable<HPTHorse> marksNumberOneHorseList = this.MarkBet.RaceDayInfo.HorseListSelected.Where(h => h.RankList.FirstOrDefault(hr => hr.Name == "StakeDistributionShare" && hr.Rank == 1) != null);

            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in marksNumberOneHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
        }

        private void cbiOddsNumberOne_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            this.MarkBet.RecalculateAllRanks();
            IEnumerable<HPTHorse> oddsNumberOneHorseList = this.MarkBet.RaceDayInfo.HorseListSelected.Where(h => h.RankList.FirstOrDefault(hr => hr.Name == "VinnarOdds" && hr.Rank == 1) != null);

            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in oddsNumberOneHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
        }

        private void cbiHomeHorses_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            IEnumerable<HPTHorse> homeTrackHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.IsHomeTrack && h.ParentRace.NumberOfSelectedHorses > 1);

            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in homeTrackHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
        }

        private void cbiBackTrackHorse_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            IEnumerable<HPTHorse> backTrackHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.StartNr > 8 && h.ParentRace.NumberOfSelectedHorses > 1);

            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in backTrackHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
        }

        private void cbi1Or2Favourites_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            this.MarkBet.RecalculateAllRanks();
            IEnumerable<HPTHorse> marksNumberOneHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.ParentRace.NumberOfSelectedHorses > 1 && h.RankList.FirstOrDefault(hr => hr.Name == "StakeDistributionShare" && hr.Rank == 1) != null)
                .OrderByDescending(h => h.StakeDistributionShare).Take(2);

            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in marksNumberOneHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            this.MarkBet.pauseRecalculation = false;
            this.MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void cbi1Or2FavouritesOf3_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            this.MarkBet.RecalculateAllRanks();
            IEnumerable<HPTHorse> marksNumberOneHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.ParentRace.NumberOfSelectedHorses > 1 && h.RankList.FirstOrDefault(hr => hr.Name == "StakeDistributionShare" && hr.Rank == 1) != null)
                .OrderByDescending(h => h.StakeDistributionShare)
                .Take(3);

            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in marksNumberOneHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;
            this.MarkBet.pauseRecalculation = recalculationPaused;
            this.MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void cbiEffectHorses_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            this.MarkBet.RecalculateAllRanks();
            IEnumerable<HPTHorse> homeTrackHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.ParentRace.NumberOfSelectedHorses > 1
                    && h.RankList.FirstOrDefault(hr => hr.Name == "StakeDistributionShare" && hr.Rank > 3) != null && h.Markability > 1.5M);

            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in homeTrackHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
        }

        private void cbiOddNumbers_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            IEnumerable<HPTHorse> numbersHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.StartNr % 2 == 1);

            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in numbersHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
        }

        private void cbiEvenNumbers_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            IEnumerable<HPTHorse> numbersHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.StartNr % 2 == 0);

            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in numbersHorseList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = false;
        }

        private void cbiNoShoes_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            IEnumerable<HPTHorse> noShoesList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.ShoeInfoCurrent.Foreshoes == false && h.ShoeInfoCurrent.Hindshoes == false);

            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in noShoesList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = false;
        }

        private void cbiShoesChanged_Selected(object sender, RoutedEventArgs e)
        {
            CreateNewRule();
            IEnumerable<HPTHorse> noShoesList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.ShoeInfoCurrent.ForeshoesChanged || h.ShoeInfoCurrent.HindshoesChanged);

            this.MarkBet.pauseRecalculation = true;
            foreach (HPTHorse horse in noShoesList)
            {
                horse.SelectedForComplimentaryRule = true;
            }
            this.MarkBet.pauseRecalculation = false;
        }

        private void cbiATGTillsammans_Selected(object sender, RoutedEventArgs e)
        {
            // Alla lopp som inte bara har en spik
            var allRacesWithoutSpike = this.MarkBet.RaceDayInfo.RaceList
                .Where(r => r.NumberOfSelectedHorses > 1);

            // Om det inte finns tillräckligt med lopp att välja från är det ingen ide att fortsätta
            if (allRacesWithoutSpike.Count() < 4)
            {
                return;
            }

            // Plocka ut de två största favoriterna bland valda hästar
            var biggestFavourites = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.ParentRace.NumberOfSelectedHorses > 1)
                .OrderByDescending(h => h.StakeDistributionShare)
                .Take(2);

            // Plocka ut de två lopp utan spik/storfavorit som har flest valda hästar
            var racesToSelectFrom = allRacesWithoutSpike
                .Where(r => r.NumberOfSelectedHorses > 2 && r != biggestFavourites.First().ParentRace && r != biggestFavourites.Last().ParentRace)
                .OrderByDescending(r => r.NumberOfSelectedHorses)
                .Take(2);

            // Om det är för få valda är det ingen ide att fortsätta
            if (racesToSelectFrom.Count() < 2)
            {
                return;
            }

            this.MarkBet.pauseRecalculation = true;

            // Skapa regel för favoriter
            CreateNewRule();
            biggestFavourites.First().SelectedForComplimentaryRule = true;
            biggestFavourites.Last().SelectedForComplimentaryRule = true;
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 2).Selected = true;

            // Skapa regel för skrällar
            CreateNewRule();
            foreach (var race in racesToSelectFrom)
            {
                var horsesToSelect = race.HorseListSelected
                    .OrderBy(h => h.StakeDistributionShare)
                    .Take(race.NumberOfSelectedHorses - 2);

                foreach (var horse in horsesToSelect)
                {
                    horse.SelectedForComplimentaryRule = true;
                }
            }
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 0).Selected = true;
            this.CurrentComplimentaryReductionRule.NumberOfWinnersList.First(now => now.NumberOfWinners == 1).Selected = true;

            this.MarkBet.pauseRecalculation = false;
        }

        #endregion

        private void chkUseReduction_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void itNumberOfComplementaryRules_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded && this.MarkBet.ComplementaryRulesCollection.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }
    }
}

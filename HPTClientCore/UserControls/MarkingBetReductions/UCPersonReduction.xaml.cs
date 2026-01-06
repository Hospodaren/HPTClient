using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCPersonReduction.xaml
    /// </summary>
    public partial class UCPersonReduction : UCMarkBetControl, IHorseListContainer
    {
        public ICollection<HPTHorse> HorseList { get; set; }
        //public ObservableCollection<HPTHorse> HorseList { get; set; }
        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        public UCPersonReduction()
        {
            //this.HorseList = new ObservableCollection<HPTHorse>();
            HorseList = new ObservableCollection<HPTHorse>();
            PersonList = new ObservableCollection<HPTPerson>();
            InitializeComponent();
            SetRaceDayInfo();
        }

        private HPTPersonRulesCollection personRulesCollection;
        public HPTPersonRulesCollection PersonRulesCollection
        {
            get
            {
                if (personRulesCollection == null)
                {
                    personRulesCollection = (HPTPersonRulesCollection)DataContext;
                }
                return personRulesCollection;
            }
        }

        public HPTPersonReductionRule CurrentReductionRule
        {
            get { return (HPTPersonReductionRule)GetValue(CurrentReductionRuleProperty); }
            set { SetValue(CurrentReductionRuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentReductionRule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentReductionRuleProperty =
            DependencyProperty.Register("CurrentReductionRule", typeof(HPTPersonReductionRule), typeof(UCPersonReduction), new UIPropertyMetadata(null));


        public ObservableCollection<HPTPerson> PersonList
        {
            get { return (ObservableCollection<HPTPerson>)GetValue(PersonListProperty); }
            set { SetValue(PersonListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PersonList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PersonListProperty =
            DependencyProperty.Register("PersonList", typeof(ObservableCollection<HPTPerson>), typeof(UCPersonReduction), new UIPropertyMetadata(null));



        private void SetRaceDayInfo()
        {
            ParentRaceDayInfo = new HPTRaceDayInfo();
            ParentRaceDayInfo.DataToShow = new HPTHorseDataToShow()
            {
                ShowDriver = true,
                ShowDriverPopup = true,
                ShowHorsePopup = true,
                ShowLegNrText = true,
                ShowMarkability = true,
                ShowMarksPercent = true,
                ShowMarksQuantity = true,
                ShowMarksShare = true,
                ShowName = true,
                ShowPlatsOdds = true,
                ShowPrio = true,
                ShowStartNr = true,
                ShowTrainer = true,
                ShowTrainerPopup = true,
                ShowVinnarOdds = true
            };


        }

        private void chkPerson_Checked(object sender, RoutedEventArgs e)
        {
            if (isSelecting)
            {
                return;
            }
            bool recalculationPaused = MarkBet.pauseRecalculation;
            MarkBet.pauseRecalculation = true;
            CheckBox chk = (CheckBox)sender;
            HPTPerson person = (HPTPerson)chk.DataContext;
            if ((bool)chk.IsChecked)
            {
                if (CurrentReductionRule == null)
                {
                    CurrentReductionRule = PersonRulesCollection.ReductionRuleFactory();
                }
                foreach (HPTHorse horse in person.HorseList)
                {
                    if (!HorseList.Contains(horse))
                    {
                        HorseList.Add(horse);
                    }
                }
                if (!CurrentReductionRule.PersonList.Contains(person))
                {
                    CurrentReductionRule.PersonList.Add(person);
                }
                if (!PersonRulesCollection.ReductionRuleList.Contains(CurrentReductionRule))
                {
                    PersonRulesCollection.ReductionRuleList.Add(CurrentReductionRule);
                }
            }
            else
            {
                foreach (HPTHorse horse in person.HorseList)
                {
                    HorseList.Remove(horse);
                }
                CurrentReductionRule.PersonList.Remove(person);
            }
            if (CurrentReductionRule.NumberOfWinnersList != null)
            {
                CurrentReductionRule.UpdateSelectable(HorseList);
            }
            MarkBet.pauseRecalculation = recalculationPaused;
            MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void btnAddRule_Click(object sender, RoutedEventArgs e)
        {
            if (PersonRulesCollection.ReductionRuleList.Contains(CurrentReductionRule))
            {
                return;
            }
            CurrentReductionRule.SetShortDescriptionString();
            PersonRulesCollection.ReductionRuleList.Add(CurrentReductionRule);
            //this.CurrentReductionRule = new HPTPersonReductionRule();
            CurrentReductionRule = PersonRulesCollection.ReductionRuleFactory();
            ResetPersonList(PersonList);
        }

        private bool isSelecting = false;

        private void btnNewRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentReductionRule != null && (CurrentReductionRule.PersonList == null || CurrentReductionRule.PersonList.Count == 0))
                {
                    PersonRulesCollection.ReductionRuleList.Remove(CurrentReductionRule);
                }
                HPTPersonReductionRule rule = PersonRulesCollection.ReductionRuleFactory();
                CurrentReductionRule = rule;
                PersonRulesCollection.ReductionRuleList.Add(rule);

                foreach (HPTPerson person in PersonList.Where(p => p.Selected))
                {
                    person.Selected = false;
                }
            }
            catch (Exception exc)
            {
                Config.AddToErrorLog(exc);
            }
        }

        void CurrentReductionRule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "NumberOfWinnersSelected")
                {
                    if (PersonRulesCollection.Use)
                    {
                        MarkBet.RecalculateReduction(RecalculateReason.Other);
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnClearRule_Click(object sender, RoutedEventArgs e)
        {
            bool recalculationPaused = MarkBet.pauseRecalculation;
            MarkBet.pauseRecalculation = true;
            ResetPersonList(CurrentReductionRule.PersonList);
            foreach (HPTPerson person in PersonList)
            {
                if (person.Selected)
                {
                    person.Selected = false;
                }
            }

            PersonRulesCollection.ReductionRuleList.Remove(CurrentReductionRule);
            CurrentReductionRule = PersonRulesCollection.ReductionRuleFactory();

            PersonRulesCollection.ReductionRuleList.Add(CurrentReductionRule);

            MarkBet.pauseRecalculation = recalculationPaused;
            if (PersonRulesCollection.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnRemoveRule_Click(object sender, RoutedEventArgs e)
        {
            PersonRulesCollection.ReductionRuleList.Remove(CurrentReductionRule);

            if (PersonRulesCollection.Use && CurrentReductionRule.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void SelectPersons(IEnumerable<HPTPerson> personList)
        {
            MarkBet.pauseRecalculation = true;
            ResetPersonList(personList);
            foreach (HPTPerson person in personList)
            {
                PersonRulesCollection.PersonList.First(p => p.ShortName == person.ShortName).Selected = true;
            }
            MarkBet.pauseRecalculation = false;
        }

        private void ResetPersonList(IEnumerable<HPTPerson> personList)
        {
            foreach (HPTPerson person in PersonList
                .Where(p => p.Selected && !personList.Contains(p)))
            {
                person.Selected = false;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTPersonReductionRule rule = (HPTPersonReductionRule)btn.DataContext;
            PersonRulesCollection.ReductionRuleList.Remove(rule);
            if (rule == CurrentReductionRule)
            {
                //this.CurrentReductionRule = new HPTPersonReductionRule();
                CurrentReductionRule = PersonRulesCollection.ReductionRuleFactory();
            }

            if (PersonRulesCollection.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            PersonRulesCollection.ReductionRuleList.Clear();

            MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void chkOnlyWithSelected_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePersonList();
        }

        private void chkOnlyTwoOrMore_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePersonList();
        }

        private void UpdatePersonList()
        {
            bool onlyWithSelected = (bool)chkOnlyWithSelected.IsChecked;
            bool onlyTwoOrMore = (bool)chkOnlyTwoOrMore.IsChecked;
            PersonList.Clear();

            IOrderedEnumerable<HPTPerson> orderedPersonList = PersonRulesCollection.PersonList
                .Where(p => ((onlyWithSelected && p.NumberOfSelectedHorse > 0) || !onlyWithSelected)
                    && ((onlyTwoOrMore && p.HorseList.Count > 1) || !onlyTwoOrMore))
                    .OrderBy(p => p.ShortName);

            foreach (HPTPerson person in orderedPersonList)
            {
                PersonList.Add(person);
            }
        }

        private void ucPersonReduction_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && IsVisible)
            {
                UpdatePersonList();
            }
        }

        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            if (PersonRulesCollection.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void chkUseReduction_Checked(object sender, RoutedEventArgs e)
        {
            MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void itNumberOfPersonRules_Checked(object sender, RoutedEventArgs e)
        {
            if (PersonRulesCollection.Use)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            isSelecting = true;
            try
            {
                var btn = (Button)sender;
                HPTPersonReductionRule rule = (HPTPersonReductionRule)btn.DataContext;
                CurrentReductionRule = rule;
                ResetPersonList(rule.PersonList);
                SelectPersons(rule.PersonList);
                HorseList.Clear();
                foreach (HPTHorse horse in CurrentReductionRule.PersonList.SelectMany(p => p.HorseList))
                {
                    HorseList.Add(horse);
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            isSelecting = false;
        }
    }
}

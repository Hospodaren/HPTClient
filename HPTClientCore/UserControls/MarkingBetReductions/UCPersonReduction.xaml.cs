using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            this.HorseList = new ObservableCollection<HPTHorse>();
            this.PersonList = new ObservableCollection<HPTPerson>();
            InitializeComponent();
            SetRaceDayInfo();
        }

        private HPTPersonRulesCollection personRulesCollection;
        public HPTPersonRulesCollection PersonRulesCollection
        {
            get
            {
                if (this.personRulesCollection == null)
                {
                    this.personRulesCollection = (HPTPersonRulesCollection)this.DataContext;
                }
                return this.personRulesCollection;
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
            this.ParentRaceDayInfo = new HPTRaceDayInfo();
            this.ParentRaceDayInfo.DataToShow = new HPTHorseDataToShow()
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
            if (this.isSelecting)
            {
                return;
            }
            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            CheckBox chk = (CheckBox)sender;
            HPTPerson person = (HPTPerson)chk.DataContext;
            if ((bool)chk.IsChecked)
            {
                if (this.CurrentReductionRule == null)
                {
                    this.CurrentReductionRule = this.PersonRulesCollection.ReductionRuleFactory();
                }
                foreach (HPTHorse horse in person.HorseList)
                {
                    if (!this.HorseList.Contains(horse))
                    {
                        this.HorseList.Add(horse);
                    }
                }
                if (!this.CurrentReductionRule.PersonList.Contains(person))
                {
                    this.CurrentReductionRule.PersonList.Add(person);
                }
                if (!this.PersonRulesCollection.ReductionRuleList.Contains(this.CurrentReductionRule))
                {
                    this.PersonRulesCollection.ReductionRuleList.Add(this.CurrentReductionRule);
                }
            }
            else
            {
                foreach (HPTHorse horse in person.HorseList)
                {
                    this.HorseList.Remove(horse);
                }
                this.CurrentReductionRule.PersonList.Remove(person);
            }
            if (this.CurrentReductionRule.NumberOfWinnersList != null)
            {
                this.CurrentReductionRule.UpdateSelectable(this.HorseList);
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
            this.MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void btnAddRule_Click(object sender, RoutedEventArgs e)
        {
            if (this.PersonRulesCollection.ReductionRuleList.Contains(this.CurrentReductionRule))
            {
                return;
            }
            this.CurrentReductionRule.SetShortDescriptionString();
            this.PersonRulesCollection.ReductionRuleList.Add(this.CurrentReductionRule);
            //this.CurrentReductionRule = new HPTPersonReductionRule();
            this.CurrentReductionRule = this.PersonRulesCollection.ReductionRuleFactory();
            ResetPersonList(this.PersonList);
        }

        private bool isSelecting = false;

        private void btnNewRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.CurrentReductionRule != null && (this.CurrentReductionRule.PersonList == null || this.CurrentReductionRule.PersonList.Count == 0))
                {
                    this.PersonRulesCollection.ReductionRuleList.Remove(this.CurrentReductionRule);
                }
                HPTPersonReductionRule rule = this.PersonRulesCollection.ReductionRuleFactory();
                this.CurrentReductionRule = rule;
                this.PersonRulesCollection.ReductionRuleList.Add(rule);

                foreach (HPTPerson person in this.PersonList.Where(p => p.Selected))
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
                    if (this.PersonRulesCollection.Use)
                    {
                        this.MarkBet.RecalculateReduction(RecalculateReason.Other);
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
            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            ResetPersonList(this.CurrentReductionRule.PersonList);
            foreach (HPTPerson person in this.PersonList)
            {
                if (person.Selected)
                {
                    person.Selected = false;
                }
            }

            this.PersonRulesCollection.ReductionRuleList.Remove(this.CurrentReductionRule);
            this.CurrentReductionRule = this.PersonRulesCollection.ReductionRuleFactory();

            this.PersonRulesCollection.ReductionRuleList.Add(this.CurrentReductionRule);

            this.MarkBet.pauseRecalculation = recalculationPaused;
            if (this.PersonRulesCollection.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnRemoveRule_Click(object sender, RoutedEventArgs e)
        {
            this.PersonRulesCollection.ReductionRuleList.Remove(this.CurrentReductionRule);

            if (this.PersonRulesCollection.Use && this.CurrentReductionRule.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void SelectPersons(IEnumerable<HPTPerson> personList)
        {
            this.MarkBet.pauseRecalculation = true;
            ResetPersonList(personList);
            foreach (HPTPerson person in personList)
            {
                this.PersonRulesCollection.PersonList.First(p => p.ShortName == person.ShortName).Selected = true;
            }
            this.MarkBet.pauseRecalculation = false;
        }

        private void ResetPersonList(IEnumerable<HPTPerson> personList)
        {
            foreach (HPTPerson person in this.PersonList
                .Where(p => p.Selected && !personList.Contains(p)))
            {
                person.Selected = false;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTPersonReductionRule rule = (HPTPersonReductionRule)btn.DataContext;
            this.PersonRulesCollection.ReductionRuleList.Remove(rule);
            if (rule == this.CurrentReductionRule)
            {
                //this.CurrentReductionRule = new HPTPersonReductionRule();
                this.CurrentReductionRule = this.PersonRulesCollection.ReductionRuleFactory();
            }

            if (this.PersonRulesCollection.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            this.PersonRulesCollection.ReductionRuleList.Clear();

            this.MarkBet.RecalculateReduction(RecalculateReason.Other);
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
            bool onlyWithSelected = (bool)this.chkOnlyWithSelected.IsChecked;
            bool onlyTwoOrMore = (bool)this.chkOnlyTwoOrMore.IsChecked;
            this.PersonList.Clear();

            IOrderedEnumerable<HPTPerson> orderedPersonList = this.PersonRulesCollection.PersonList
                .Where(p => ((onlyWithSelected && p.NumberOfSelectedHorse > 0) || !onlyWithSelected)
                    && ((onlyTwoOrMore && p.HorseList.Count > 1) || !onlyTwoOrMore))
                    .OrderBy(p => p.ShortName);

            foreach (HPTPerson person in orderedPersonList)
            {
                this.PersonList.Add(person);
            }
        }

        private void ucPersonReduction_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            {
                UpdatePersonList();
            }
        }

        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            if (this.PersonRulesCollection.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void chkUseReduction_Checked(object sender, RoutedEventArgs e)
        {
            this.MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        private void itNumberOfPersonRules_Checked(object sender, RoutedEventArgs e)
        {
            if (this.PersonRulesCollection.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Other);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            this.isSelecting = true;
            try
            {
                var btn = (Button)sender;
                HPTPersonReductionRule rule = (HPTPersonReductionRule)btn.DataContext;
                this.CurrentReductionRule = rule;
                ResetPersonList(rule.PersonList);
                SelectPersons(rule.PersonList);
                this.HorseList.Clear();
                foreach (HPTHorse horse in this.CurrentReductionRule.PersonList.SelectMany(p => p.HorseList))
                {
                    this.HorseList.Add(horse);
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            this.isSelecting = false;
        }
    }
}

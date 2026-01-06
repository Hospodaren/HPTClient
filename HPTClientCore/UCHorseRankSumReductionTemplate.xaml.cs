using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCHorseRankSumReductionTemplate.xaml
    /// </summary>
    public partial class UCHorseRankSumReductionTemplate : UserControl
    {
        public UCHorseRankSumReductionTemplate()
        {
            InitializeComponent();
        }

        public HPTConfig Config
        {
            get { return (HPTConfig)GetValue(ConfigProperty); }
            set { SetValue(ConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Config.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCHorseRankSumReductionTemplate), new UIPropertyMetadata(HPTConfig.Config));

        private HPTHorseRankSumReductionRuleCollection horseRankSumReductionRuleCollection;
        internal HPTHorseRankSumReductionRuleCollection HorseRankSumReductionRuleCollection
        {
            get
            {
                if (horseRankSumReductionRuleCollection == null || horseRankSumReductionRuleCollection != (HPTHorseRankSumReductionRuleCollection)DataContext)
                {
                    horseRankSumReductionRuleCollection = (HPTHorseRankSumReductionRuleCollection)DataContext;
                }
                return horseRankSumReductionRuleCollection;
            }
        }

        //private void btnRemove_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btn = (Button)sender;
        //    var rule = (HPTHorseRankSumReductionRule)btn.DataContext;
        //    this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Remove(rule);
        //}

        //private void cmbVariable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var cmb = (ComboBox)sender;
        //    var rule = (HPTHorseRankSumReductionRule)cmb.Tag;
        //    HPTHorseVariable hv = (HPTHorseVariable)cmb.SelectedItem;
        //    if (rule.HorseVariable == null || rule.HorseVariable.PropertyName != hv.PropertyName)
        //    {
        //        rule.HorseVariable = hv;
        //        rule.LowerBoundary = rule.HorseVariable.GroupReductionInfo.MinValue;
        //        rule.UpperBoundary = rule.HorseVariable.GroupReductionInfo.MaxValue;
        //    }
        //}

        //private void btnNewRule_Click(object sender, RoutedEventArgs e)
        //{
        //    // TODO: Antal beroende på speltyp
        //    HPTGroupIntervalReductionRule rule = new HPTGroupIntervalReductionRule(this.NumberOfRaces, false)
        //    {
        //        Use = true
        //    };
        //    this.GroupIntervalRulesCollection.ReductionRuleList.Add(rule);
        //    string s = this.DataContext.ToString();
        //}

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var rule in HorseRankSumReductionRuleCollection.RankSumReductionRuleList)
            {
                rule.Use = false;
                foreach (var horseRankReductionRule in rule.ReductionRuleList)
                {
                    horseRankReductionRule.Use = false;
                }
                rule.ReductionRuleList.Clear();
            }
            HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Clear();
        }

        private int NumberOfRaces;
        private BetTypeCategory TypeCategory;
        private void cmbBetTypeCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                TypeCategory = (BetTypeCategory)e.AddedItems[0];
                switch (TypeCategory)
                {
                    case BetTypeCategory.None:
                        break;
                    case BetTypeCategory.V4:
                        NumberOfRaces = 4;
                        break;
                    case BetTypeCategory.V5:
                        NumberOfRaces = 5;
                        break;
                    case BetTypeCategory.V6X:
                        NumberOfRaces = 6;
                        break;
                    case BetTypeCategory.V75:
                        NumberOfRaces = 7;
                        break;
                    case BetTypeCategory.V86:
                    case BetTypeCategory.V85:
                        NumberOfRaces = 8;
                        break;
                    case BetTypeCategory.Double:
                        NumberOfRaces = 2;
                        break;
                    case BetTypeCategory.Trio:
                    case BetTypeCategory.Twin:
                        NumberOfRaces = 1;
                        break;
                    default:
                        NumberOfRaces = 0;
                        break;
                }

                foreach (var horseRankSumReductionRule in HorseRankSumReductionRuleCollection.RankSumReductionRuleList)
                {
                    foreach (var horseRankReductionRule in horseRankSumReductionRule.ReductionRuleList)
                    {
                        if (horseRankReductionRule.NumberOfWinnersList != null && horseRankReductionRule.NumberOfWinnersList.Count > 0)
                        {
                            int maxNumberOfWinners = horseRankReductionRule.NumberOfWinnersList.Max(now => now.NumberOfWinners);
                            if (maxNumberOfWinners > NumberOfRaces)
                            {
                                List<HPTNumberOfWinners> numberOfWinnersToRemove = horseRankReductionRule.NumberOfWinnersList
                                    .Where(now => now.NumberOfWinners > NumberOfRaces).ToList();

                                foreach (var numberOfWinners in numberOfWinnersToRemove)
                                {
                                    numberOfWinners.Selected = false;
                                    horseRankReductionRule.NumberOfWinnersList.Remove(numberOfWinners);
                                }
                            }
                            else if (maxNumberOfWinners < NumberOfRaces)
                            {
                                for (int i = maxNumberOfWinners + 1; i <= NumberOfRaces; i++)
                                {
                                    var numberOfWinners = new HPTNumberOfWinners()
                                    {
                                        Selectable = true,
                                        NumberOfWinners = i
                                    };
                                    horseRankReductionRule.NumberOfWinnersList.Add(numberOfWinners);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnRemoveTemplate_Click(object sender, RoutedEventArgs e)
        {
            Config.RankSumReductionRuleCollection.Remove(HorseRankSumReductionRuleCollection);
        }

        private void btnRemoveRankReductionRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var horseRankReductionRule = (HPTHorseRankSumReductionRule)sender;
                if (horseRankReductionRule != null)
                {
                    HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Remove(horseRankReductionRule);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnClone_Click(object sender, RoutedEventArgs e)
        {
            var clonedHorseRankSumReductionRuleCollection = new HPTHorseRankSumReductionRuleCollection()
            {
                Name = HorseRankSumReductionRuleCollection.Name + " (kopia)",
                RankSumReductionRuleList = new ObservableCollection<HPTHorseRankSumReductionRule>(HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Select(r => r.Clone())),
                TypeCategory = TypeCategory
            };

            var horseRankVariableList = HPTHorseRankVariable.CreateVariableList();
            foreach (var horseRankSumReductionRule in clonedHorseRankSumReductionRuleCollection.RankSumReductionRuleList)
            {
                horseRankSumReductionRule.HorseRankVariable = horseRankVariableList.FirstOrDefault(hrv => hrv.PropertyName == horseRankSumReductionRule.PropertyName);
            }

            Config.RankSumReductionRuleCollection.Add(clonedHorseRankSumReductionRuleCollection);
        }

        private void cmbHorseRankVariable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // KOD HÄR?
        }

        private void btnAddVariable_Click(object sender, RoutedEventArgs e)
        {
            var horseRankVariableBase = (HPTHorseRankVariableBase)cmbHorseRankVariable.SelectedItem;
            if (horseRankVariableBase == null)
            {
                return;
            }
            var horseRankSumReductionRule = HorseRankSumReductionRuleCollection.RankSumReductionRuleList.FirstOrDefault(r => r.PropertyName == horseRankVariableBase.PropertyName);
            if (horseRankSumReductionRule == null)
            {
                var horseRankVariable = new HPTHorseRankVariable()
                {
                    CategoryText = horseRankVariableBase.CategoryText,
                    PropertyName = horseRankVariableBase.PropertyName,
                    Show = horseRankVariableBase.Show,
                    Text = horseRankVariableBase.Text
                };
                horseRankSumReductionRule = new HPTHorseRankSumReductionRule(horseRankVariable, NumberOfRaces)
                {
                    Use = true
                };
                HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Add(horseRankSumReductionRule);

                //try
                //{
                //    BindingOperations.GetBindingExpression(this.lstHorseRankSumReductionRules, ListBox.ItemsSourceProperty).UpdateTarget();
                //}
                //catch (Exception exc)
                //{
                //    string s = exc.Message;
                //}
            }
        }

    }
}

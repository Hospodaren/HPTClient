using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCGroupIntervalTemplate.xaml
    /// </summary>
    public partial class UCGroupIntervalTemplate : UserControl
    {
        public UCGroupIntervalTemplate()
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
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCGroupIntervalTemplate), new UIPropertyMetadata(HPTConfig.Config));

        private HPTGroupIntervalRulesCollection groupIntervalRulesCollection;
        internal HPTGroupIntervalRulesCollection GroupIntervalRulesCollection
        {
            get
            {
                if (groupIntervalRulesCollection == null || groupIntervalRulesCollection != (HPTGroupIntervalRulesCollection)DataContext)
                {
                    groupIntervalRulesCollection = (HPTGroupIntervalRulesCollection)DataContext;
                }
                return groupIntervalRulesCollection;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTGroupIntervalReductionRule rule = (HPTGroupIntervalReductionRule)btn.DataContext;
            //this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList.Remove(rule);
            GroupIntervalRulesCollection.ReductionRuleList.Remove(rule);
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
            // TODO: Antal beroende på speltyp
            HPTGroupIntervalReductionRule rule = new HPTGroupIntervalReductionRule(NumberOfRaces, false)
            {
                Use = true
            };
            GroupIntervalRulesCollection.ReductionRuleList.Add(rule);
            string s = DataContext.ToString();
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            //foreach (HPTGroupIntervalReductionRule rule in this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList)
            foreach (HPTGroupIntervalReductionRule rule in GroupIntervalRulesCollection.ReductionRuleList)
            {
                rule.Use = false;
            }
            //this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList.Clear();
            GroupIntervalRulesCollection.ReductionRuleList.Clear();
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

                //foreach (var groupIntervalReductionRule in this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList)
                foreach (var groupIntervalReductionRule in GroupIntervalRulesCollection.ReductionRuleList)
                {
                    if (groupIntervalReductionRule.NumberOfWinnersList != null && groupIntervalReductionRule.NumberOfWinnersList.Count > 0)
                    {
                        int maxNumberOfWinners = groupIntervalReductionRule.NumberOfWinnersList.Max(now => now.NumberOfWinners);
                        if (maxNumberOfWinners > NumberOfRaces)
                        {
                            List<HPTNumberOfWinners> numberOfWinnersToRemove = groupIntervalReductionRule.NumberOfWinnersList
                                .Where(now => now.NumberOfWinners > NumberOfRaces).ToList();

                            foreach (var numberOfWinners in numberOfWinnersToRemove)
                            {
                                numberOfWinners.Selected = false;
                                groupIntervalReductionRule.NumberOfWinnersList.Remove(numberOfWinners);
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
                                groupIntervalReductionRule.NumberOfWinnersList.Add(numberOfWinners);
                            }
                        }
                    }
                }
            }
        }

        private void btnRemoveTemplate_Click(object sender, RoutedEventArgs e)
        {
            Config.GroupIntervalRulesCollectionList.Remove(GroupIntervalRulesCollection);
        }

        private void btnClone_Click(object sender, RoutedEventArgs e)
        {
            var clonedGroupIntervalRulesCollection =
                new HPTGroupIntervalRulesCollection(GroupIntervalRulesCollection.NumberOfRaces,
                                                    GroupIntervalRulesCollection.Use);

            clonedGroupIntervalRulesCollection.Name = GroupIntervalRulesCollection.Name + " (kopia)";
            clonedGroupIntervalRulesCollection.TypeCategory = GroupIntervalRulesCollection.TypeCategory;

            var clonedRules =
                GroupIntervalRulesCollection.ReductionRuleList.Cast<HPTGroupIntervalReductionRule>().Select(
                    r => r.Clone());
            clonedGroupIntervalRulesCollection.ReductionRuleList = new ObservableCollection<HPTNumberOfWinnersReductionRule>(clonedRules);
            Config.GroupIntervalRulesCollectionList.Add(clonedGroupIntervalRulesCollection);
        }
    }
}

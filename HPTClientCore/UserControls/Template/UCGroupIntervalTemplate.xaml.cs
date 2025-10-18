using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                if (this.groupIntervalRulesCollection == null || this.groupIntervalRulesCollection != (HPTGroupIntervalRulesCollection)this.DataContext)
                {
                    this.groupIntervalRulesCollection = (HPTGroupIntervalRulesCollection)this.DataContext;
                }
                return this.groupIntervalRulesCollection;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTGroupIntervalReductionRule rule = (HPTGroupIntervalReductionRule)btn.DataContext;
            //this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList.Remove(rule);
            this.GroupIntervalRulesCollection.ReductionRuleList.Remove(rule);
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
            HPTGroupIntervalReductionRule rule = new HPTGroupIntervalReductionRule(this.NumberOfRaces, false)
            {
                Use = true
            };
            this.GroupIntervalRulesCollection.ReductionRuleList.Add(rule);
            string s = this.DataContext.ToString();
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            //foreach (HPTGroupIntervalReductionRule rule in this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList)
            foreach (HPTGroupIntervalReductionRule rule in this.GroupIntervalRulesCollection.ReductionRuleList)
            {
                rule.Use = false;
            }
            //this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList.Clear();
            this.GroupIntervalRulesCollection.ReductionRuleList.Clear();
        }

        private int NumberOfRaces;
        private BetTypeCategory TypeCategory;
        private void cmbBetTypeCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.TypeCategory = (BetTypeCategory)e.AddedItems[0];
                switch (this.TypeCategory)
                {
                    case BetTypeCategory.None:
                        break;
                    case BetTypeCategory.V4:
                        this.NumberOfRaces = 4;
                        break;
                    case BetTypeCategory.V5:
                        this.NumberOfRaces = 5;
                        break;
                    case BetTypeCategory.V6X:
                        this.NumberOfRaces = 6;
                        break;
                    case BetTypeCategory.V75:
                        this.NumberOfRaces = 7;
                        break;
                    case BetTypeCategory.V86:
                        this.NumberOfRaces = 8;
                        break;
                    case BetTypeCategory.Double:
                        this.NumberOfRaces = 2;
                        break;
                    case BetTypeCategory.Trio:
                    case BetTypeCategory.Twin:
                        this.NumberOfRaces = 1;
                        break;
                    default:
                        this.NumberOfRaces = 0;
                        break;
                }

                //foreach (var groupIntervalReductionRule in this.GroupIntervalRulesCollection.GroupIntervalReductionRuleList)
                foreach (var groupIntervalReductionRule in this.GroupIntervalRulesCollection.ReductionRuleList)
                {
                    if (groupIntervalReductionRule.NumberOfWinnersList != null && groupIntervalReductionRule.NumberOfWinnersList.Count > 0)
                    {
                        int maxNumberOfWinners = groupIntervalReductionRule.NumberOfWinnersList.Max(now => now.NumberOfWinners);
                        if (maxNumberOfWinners > this.NumberOfRaces)
                        {
                            List<HPTNumberOfWinners> numberOfWinnersToRemove = groupIntervalReductionRule.NumberOfWinnersList
                                .Where(now => now.NumberOfWinners > this.NumberOfRaces).ToList();

                            foreach (var numberOfWinners in numberOfWinnersToRemove)
                            {
                                numberOfWinners.Selected = false;
                                groupIntervalReductionRule.NumberOfWinnersList.Remove(numberOfWinners);
                            }
                        }
                        else if (maxNumberOfWinners < this.NumberOfRaces)
                        {
                            for (int i = maxNumberOfWinners + 1; i <= this.NumberOfRaces; i++)
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
            this.Config.GroupIntervalRulesCollectionList.Remove(this.GroupIntervalRulesCollection);
        }

        private void btnClone_Click(object sender, RoutedEventArgs e)
        {
            var clonedGroupIntervalRulesCollection =
                new HPTGroupIntervalRulesCollection(this.GroupIntervalRulesCollection.NumberOfRaces,
                                                    this.GroupIntervalRulesCollection.Use);

            clonedGroupIntervalRulesCollection.Name = this.GroupIntervalRulesCollection.Name + " (kopia)";
            clonedGroupIntervalRulesCollection.TypeCategory = this.GroupIntervalRulesCollection.TypeCategory;

            var clonedRules =
                this.GroupIntervalRulesCollection.ReductionRuleList.Cast<HPTGroupIntervalReductionRule>().Select(
                    r => r.Clone());
            clonedGroupIntervalRulesCollection.ReductionRuleList = new ObservableCollection<HPTNumberOfWinnersReductionRule>(clonedRules);
            this.Config.GroupIntervalRulesCollectionList.Add(clonedGroupIntervalRulesCollection);
        }
    }
}

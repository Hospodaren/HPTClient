using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

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
                if (this.horseRankSumReductionRuleCollection == null || this.horseRankSumReductionRuleCollection != (HPTHorseRankSumReductionRuleCollection)this.DataContext)
                {
                    this.horseRankSumReductionRuleCollection = (HPTHorseRankSumReductionRuleCollection)this.DataContext;
                }
                return this.horseRankSumReductionRuleCollection;
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
            foreach (var rule in this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList)
            {
                rule.Use = false;
                foreach (var horseRankReductionRule in rule.ReductionRuleList)
                {
                    horseRankReductionRule.Use = false;
                }
                rule.ReductionRuleList.Clear();
            }            
            this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Clear();
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

                foreach (var horseRankSumReductionRule in this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList)
                {
                    foreach (var horseRankReductionRule in horseRankSumReductionRule.ReductionRuleList)
                    {
                        if (horseRankReductionRule.NumberOfWinnersList != null && horseRankReductionRule.NumberOfWinnersList.Count > 0)
                        {
                            int maxNumberOfWinners = horseRankReductionRule.NumberOfWinnersList.Max(now => now.NumberOfWinners);
                            if (maxNumberOfWinners > this.NumberOfRaces)
                            {
                                List<HPTNumberOfWinners> numberOfWinnersToRemove = horseRankReductionRule.NumberOfWinnersList
                                    .Where(now => now.NumberOfWinners > this.NumberOfRaces).ToList();

                                foreach (var numberOfWinners in numberOfWinnersToRemove)
                                {
                                    numberOfWinners.Selected = false;
                                    horseRankReductionRule.NumberOfWinnersList.Remove(numberOfWinners);
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
            this.Config.RankSumReductionRuleCollection.Remove(this.HorseRankSumReductionRuleCollection);
        }
        
        private void btnRemoveRankReductionRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var horseRankReductionRule = (HPTHorseRankSumReductionRule)sender;
                if (horseRankReductionRule != null)
                {
                    this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Remove(horseRankReductionRule);
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
                    Name = this.HorseRankSumReductionRuleCollection.Name + " (kopia)",
                    RankSumReductionRuleList = new ObservableCollection<HPTHorseRankSumReductionRule>(this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Select(r => r.Clone())),
                    TypeCategory = this.TypeCategory
                };

            var horseRankVariableList = HPTHorseRankVariable.CreateVariableList();
            foreach (var horseRankSumReductionRule in clonedHorseRankSumReductionRuleCollection.RankSumReductionRuleList)
            {
                horseRankSumReductionRule.HorseRankVariable = horseRankVariableList.FirstOrDefault(hrv => hrv.PropertyName == horseRankSumReductionRule.PropertyName);
            }

            this.Config.RankSumReductionRuleCollection.Add(clonedHorseRankSumReductionRuleCollection);
        }

        private void cmbHorseRankVariable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // KOD HÄR?
        }

        private void btnAddVariable_Click(object sender, RoutedEventArgs e)
        {
            var horseRankVariableBase = (HPTHorseRankVariableBase)this.cmbHorseRankVariable.SelectedItem;
            if (horseRankVariableBase == null)
            {
                return;
            }
            var horseRankSumReductionRule = this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList.FirstOrDefault(r => r.PropertyName == horseRankVariableBase.PropertyName);
            if (horseRankSumReductionRule == null)
            {
                var horseRankVariable = new HPTHorseRankVariable()
                {
                    CategoryText = horseRankVariableBase.CategoryText,
                    PropertyName = horseRankVariableBase.PropertyName,
                    Show = horseRankVariableBase.Show,
                    Text = horseRankVariableBase.Text
                };
                horseRankSumReductionRule = new HPTHorseRankSumReductionRule(horseRankVariable, this.NumberOfRaces)
                {
                    Use = true
                };
                this.HorseRankSumReductionRuleCollection.RankSumReductionRuleList.Add(horseRankSumReductionRule);

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

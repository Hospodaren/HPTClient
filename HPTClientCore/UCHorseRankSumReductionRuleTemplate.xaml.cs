using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCHorseRankSumReductionRuleTemplate.xaml
    /// </summary>
    public partial class UCHorseRankSumReductionRuleTemplate : UserControl
    {
        public UCHorseRankSumReductionRuleTemplate()
        {
            InitializeComponent();
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
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
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
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public event RoutedEventHandler Click;
        private void btnRemoveEntireRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Click != null)
                    this.Click(this.DataContext, e);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

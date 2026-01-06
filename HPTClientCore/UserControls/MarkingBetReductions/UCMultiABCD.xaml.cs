using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMultiABCD.xaml
    /// </summary>
    public partial class UCMultiABCD : UCMarkBetControl
    {
        public UCMultiABCD()
        {
            InitializeComponent();
        }

        private void btnNewRule_Click(object sender, RoutedEventArgs e)
        {
            object o = icRules.DataContext;
            MarkBet.MultiABCDEFReductionRule.ABCDEFReductionRuleList.Add(new HPTABCDEFReductionRule(MarkBet));
            MarkBet.RecalculateNumberOfX();
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            MarkBet.MultiABCDEFReductionRule.ABCDEFReductionRuleList.Clear();
        }

        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            if (MarkBet != null)
            {
                if (!MarkBet.IsDeserializing)
                {
                    if (MarkBet.ABCDEFReductionRule.Use)
                    {
                        MarkBet.RecalculateReduction(RecalculateReason.XReduction);
                    }
                    else
                    {
                        MarkBet.RecalculateNumberOfX();
                    }
                }
            }
        }

        private void gbReduction_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTABCDEFReductionRule rule = (HPTABCDEFReductionRule)btn.DataContext;
            MarkBet.MultiABCDEFReductionRule.ABCDEFReductionRuleList.Remove(rule);
            MarkBet.RecalculateReduction(RecalculateReason.XReduction);
        }

        private void btnClearABCD_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTABCDEFReductionRule rule = (HPTABCDEFReductionRule)btn.DataContext;
            rule.Clear();
        }

        private void chkUseMultiABCDReduction_Checked(object sender, RoutedEventArgs e)
        {
            MarkBet.RecalculateReduction(RecalculateReason.All);
        }

    }
}

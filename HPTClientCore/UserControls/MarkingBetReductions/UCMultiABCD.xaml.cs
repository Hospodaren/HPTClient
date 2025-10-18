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
            object o = this.icRules.DataContext;
            this.MarkBet.MultiABCDEFReductionRule.ABCDEFReductionRuleList.Add(new HPTABCDEFReductionRule(this.MarkBet));
            this.MarkBet.RecalculateNumberOfX();
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.MultiABCDEFReductionRule.ABCDEFReductionRuleList.Clear();
        }

        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            if (this.MarkBet != null)
            {
                if (!this.MarkBet.IsDeserializing)
                {
                    if (this.MarkBet.ABCDEFReductionRule.Use)
                    {
                        this.MarkBet.RecalculateReduction(RecalculateReason.XReduction);
                    }
                    else
                    {
                        this.MarkBet.RecalculateNumberOfX();
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
            this.MarkBet.MultiABCDEFReductionRule.ABCDEFReductionRuleList.Remove(rule);
            this.MarkBet.RecalculateReduction(RecalculateReason.XReduction);
        }

        private void btnClearABCD_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            HPTABCDEFReductionRule rule = (HPTABCDEFReductionRule)btn.DataContext;
            rule.Clear();
        }

        private void chkUseMultiABCDReduction_Checked(object sender, RoutedEventArgs e)
        {
            this.MarkBet.RecalculateReduction(RecalculateReason.All);
        }

    }
}

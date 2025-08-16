using System.Windows;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCReductionRuleStatistics.xaml
    /// </summary>
    public partial class UCReductionRuleStatistics : UCMarkBetControl
    {
        public UCReductionRuleStatistics()
        {
            InitializeComponent();
        }

        private void btnCalculateReductionRuleStatistics_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            this.MarkBet.CalculateRuleStatistics();
            Cursor = Cursors.Arrow;
        }

        private void btnCalculateSystemStatistics_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            this.MarkBet.CalculateRuleStatistics();
            Cursor = Cursors.Arrow;
        }

        private void btnCalculateJackpotStatistics_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            this.MarkBet.CalculateJackpotRows();
            Cursor = Cursors.Arrow;
        }

        private void btnCalculateRowValueStatistics_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            this.MarkBet.CalculateRowValueStatistics();
            Cursor = Cursors.Arrow;
        }
    }
}

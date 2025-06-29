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

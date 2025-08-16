using System.Windows;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCV6BetMultiplier.xaml
    /// </summary>
    public partial class UCV6BetMultiplier : UCMarkBetControl
    {
        private int ruleNumber = 0;
        public UCV6BetMultiplier()
        {
            InitializeComponent();
        }

        private void btnNewRule_Click(object sender, RoutedEventArgs e)
        {
            ruleNumber++;
            this.MarkBet.V6BetMultiplierRuleList.Add(new HPTV6BetMultiplierRule(this.MarkBet, ruleNumber));
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.V6BetMultiplierRuleList.Clear();
            this.MarkBet.UpdateV6BetMultiplierSingleRows();
            //this.MarkBet.SetV6BetMultiplierSingleRows();
        }

        //private void ucV6BetMultiplier_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    object o = this.DataContext;
        //    o = this.MarkBet;
        //}
    }
}

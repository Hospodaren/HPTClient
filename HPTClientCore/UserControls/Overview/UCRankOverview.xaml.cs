using System.Windows;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRankOverview.xaml
    /// </summary>
    public partial class UCRankOverview : UCMarkBetControl
    {
        public UCRankOverview()
        {
            InitializeComponent();
        }

        private void ListBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!MarkBet.IsDeserializing)
            {
                MarkBet.RecalculateRank();
                if (MarkBet.ReductionRank)
                {
                    MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))// && this.IsVisible)
            {
                if (MarkBet != null && !MarkBet.IsDeserializing)
                {
                    MarkBet.RecalculateAllRanks();
                }
            }
        }

        private void dudMinValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MarkBet.ReductionRank)
            {
                MarkBet.RecalculateReduction(RecalculateReason.Rank);
            }
        }

        private void btnExportToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rankExport = MarkBet.ExportHorseRanksToExcelFormat();
                Clipboard.SetDataObject(rankExport);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

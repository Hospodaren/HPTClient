using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (!this.MarkBet.IsDeserializing)
            {
                this.MarkBet.RecalculateRank();
                if (this.MarkBet.ReductionRank)
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
            }            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))// && this.IsVisible)
            {
                if (this.MarkBet != null && !this.MarkBet.IsDeserializing)
                {
                    this.MarkBet.RecalculateAllRanks();
                }
            }
        }

        private void dudMinValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.MarkBet.ReductionRank)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.Rank);
            }
        }

        private void btnExportToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rankExport = this.MarkBet.ExportHorseRanksToExcelFormat();
                Clipboard.SetDataObject(rankExport);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

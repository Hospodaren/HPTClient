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
    /// Interaction logic for UCResultAnalyzer.xaml
    /// </summary>
    public partial class UCResultAnalyzer : UserControl
    {
        public UCResultAnalyzer()
        {
            InitializeComponent();
        }

        private void btnExportToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.btnExportToClipboard.IsOpen = false;
                var resultAnalyzer = (HPTResultAnalyzer)this.DataContext;
                if (resultAnalyzer != null)
                {
                    string analysisData = resultAnalyzer.ExportToExcel();
                    Clipboard.SetDataObject(analysisData);
                }
            }
            catch (Exception exc)
            {
                string fel = exc.Message;
            }
        }

        private void miOverviewWithHeaders_Click(object sender, RoutedEventArgs e)
        {
            this.btnExportToClipboard.IsOpen = false;
            var resultAnalyzer = (HPTResultAnalyzer)this.DataContext;
            if (resultAnalyzer != null)
            {
                string analysisData = resultAnalyzer.ExportToExcel(true);
                Clipboard.SetDataObject(analysisData);
            }
        }

        private void miOverviewWithoutHeaders_Click(object sender, RoutedEventArgs e)
        {
            this.btnExportToClipboard.IsOpen = false;
            var resultAnalyzer = (HPTResultAnalyzer)this.DataContext;
            if (resultAnalyzer != null)
            {
                string analysisData = resultAnalyzer.ExportToExcel(false);
                Clipboard.SetDataObject(analysisData);
            }
        }
    }
}

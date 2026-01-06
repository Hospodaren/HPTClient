using System.Windows;
using System.Windows.Controls;

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
                btnExportToClipboard.IsOpen = false;
                var resultAnalyzer = (HPTResultAnalyzer)DataContext;
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
            btnExportToClipboard.IsOpen = false;
            var resultAnalyzer = (HPTResultAnalyzer)DataContext;
            if (resultAnalyzer != null)
            {
                string analysisData = resultAnalyzer.ExportToExcel(true);
                Clipboard.SetDataObject(analysisData);
            }
        }

        private void miOverviewWithoutHeaders_Click(object sender, RoutedEventArgs e)
        {
            btnExportToClipboard.IsOpen = false;
            var resultAnalyzer = (HPTResultAnalyzer)DataContext;
            if (resultAnalyzer != null)
            {
                string analysisData = resultAnalyzer.ExportToExcel(false);
                Clipboard.SetDataObject(analysisData);
            }
        }
    }
}

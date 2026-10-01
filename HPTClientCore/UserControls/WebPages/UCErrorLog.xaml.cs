using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCErrorLog.xaml
    /// </summary>
    public partial class UCErrorLog : UserControl
    {
        public UCErrorLog()
        {
            InitializeComponent();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (Button)e.OriginalSource;
                var exc = (Exception)btn.DataContext;
                var sb = new StringBuilder();
                sb.AppendLine("Felmeddelande:");
                sb.AppendLine(exc.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(exc.StackTrace);
                Clipboard.SetDataObject(sb.ToString());
            }
            catch (Exception exc)
            {
                var s = exc.Message;
            }
        }

        private void btnCopyAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = (HPTConfig)DataContext;
                var sb = new StringBuilder();
                foreach (var exc in config.ErrorLog)
                {
                    sb.AppendLine("Felmeddelande:");
                    sb.AppendLine(exc.Message);
                    sb.AppendLine("StackTrace:");
                    sb.AppendLine(exc.StackTrace);
                    sb.AppendLine();
                }
                Clipboard.SetDataObject(sb.ToString());
            }
            catch (Exception exc)
            {
                var s = exc.Message;
            }
        }

        private void btnMailErrorLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HPTConfig.Config.MailErrorLog();
            }
            catch (Exception exc)
            {
                var s = exc.Message;
            }
        }
    }
}

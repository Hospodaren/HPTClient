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
                Button btn = (Button)e.OriginalSource;
                Exception exc = (Exception)btn.DataContext;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Felmeddelande:");
                sb.AppendLine(exc.Message);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(exc.StackTrace);
                Clipboard.SetDataObject(sb.ToString());
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnCopyAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HPTConfig config = (HPTConfig)this.DataContext;
                StringBuilder sb = new StringBuilder();
                foreach (Exception exc in config.ErrorLog)
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
                string s = exc.Message;
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
                string s = exc.Message;
            }
        }
    }
}

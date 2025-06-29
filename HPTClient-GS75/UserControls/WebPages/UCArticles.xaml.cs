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
    /// Interaction logic for UCFacebook.xaml
    /// </summary>
    public partial class UCArticles : UserControl
    {
        public UCArticles()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Hyperlink hl = (Hyperlink)sender;
                System.Diagnostics.Process.Start(hl.NavigateUri.ToString());
                e.Handled = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

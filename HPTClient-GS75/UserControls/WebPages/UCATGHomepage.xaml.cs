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
    /// Interaction logic for UCATGHomepage.xaml
    /// </summary>
    public partial class UCATGHomepage : UserControl
    {
        public UCATGHomepage()
        {
            InitializeComponent();
        }

        private void hlATG_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(this.hlATG.NavigateUri.ToString());
            e.Handled = true;
        }
    }
}

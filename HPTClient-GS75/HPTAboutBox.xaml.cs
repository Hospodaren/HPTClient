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
using System.Windows.Shapes;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for HPTAboutBox.xaml
    /// </summary>
    public partial class HPTAboutBox : Window
    {
        public HPTAboutBox()
        {
            InitializeComponent();
        }

        private void hlHomePage_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            System.Diagnostics.Process.Start(hl.NavigateUri.OriginalString);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void hlATGSpelAnsvar_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            System.Diagnostics.Process.Start(hl.NavigateUri.OriginalString);
        }
    }
}

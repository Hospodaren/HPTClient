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
    /// Interaction logic for UCNotes.xaml
    /// </summary>
    public partial class UCNotes : UserControl
    {
        public UCNotes()
        {
            InitializeComponent();
        }

        internal HPTHorse Horse
        {
            get { return (HPTHorse) this.DataContext; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.Horse.OwnInformation.Comment = this.txtNotes.Text;
        }
    }
}

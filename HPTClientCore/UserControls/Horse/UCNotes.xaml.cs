using System.Windows;
using System.Windows.Controls;

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
            get { return (HPTHorse)DataContext; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Horse.OwnInformation.Comment = txtNotes.Text;
        }
    }
}

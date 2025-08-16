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
            get { return (HPTHorse)this.DataContext; }
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

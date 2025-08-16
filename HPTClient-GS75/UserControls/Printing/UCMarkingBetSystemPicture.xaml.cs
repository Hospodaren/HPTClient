using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarkingBetSystemDocument.xaml
    /// </summary>
    public partial class UCMarkingBetSystemPicture : UserControl
    {
        public UCMarkingBetSystemPicture()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            object o = this.DataContext;
        }
    }
}

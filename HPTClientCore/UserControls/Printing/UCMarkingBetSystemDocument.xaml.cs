using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarkingBetSystemDocument.xaml
    /// </summary>
    public partial class UCMarkingBetSystemDocument : UserControl
    {
        public UCMarkingBetSystemDocument()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            object o = DataContext;
        }
    }
}

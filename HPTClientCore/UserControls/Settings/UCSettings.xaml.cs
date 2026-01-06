using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCSettings.xaml
    /// </summary>
    public partial class UCSettings : UserControl
    {
        public UCSettings()
        {
            InitializeComponent();
            //if (!HPTConfig.Config.IsPayingCustomer)
            //{
            //    this.tiMailLists.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ATGCalendar ac = (ATGCalendar)DataContext;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

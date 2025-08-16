using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarkBetXToShow.xaml
    /// </summary>
    public partial class UCMarkBetXToShow : UserControl
    {
        public UCMarkBetXToShow()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            {
                //if (!HPTConfig.Config.IsPayingCustomer)
                //{
                //    this.chkD.IsEnabled = false;
                //    this.chkE.IsEnabled = false;
                //    this.chkF.IsEnabled = false;
                //}
            }
        }
    }
}

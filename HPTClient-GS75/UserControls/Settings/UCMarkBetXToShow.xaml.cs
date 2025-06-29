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

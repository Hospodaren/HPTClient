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
                ATGCalendar ac = (ATGCalendar)this.DataContext;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

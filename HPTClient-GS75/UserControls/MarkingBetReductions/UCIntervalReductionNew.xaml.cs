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
using Xceed.Wpf.Toolkit;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCIntervalReductionNew.xaml
    /// </summary>
    public partial class UCIntervalReductionNew : UCMarkBetControl
    {
        public UCIntervalReductionNew()
        {
            InitializeComponent();
        }

        private void iudValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var iud = (IntegerUpDown)e.OriginalSource;
            var rule = (HPTIntervalReductionRule)iud.DataContext;
            if (rule.Use)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.All);
            }
        }

        private void lvwIntervalReduction_Checked(object sender, RoutedEventArgs e)
        {
            this.MarkBet.RecalculateReduction(RecalculateReason.All);
        }

        private bool loaded = false;
        private void UCMarkBetControl_Loaded(object sender, RoutedEventArgs e)
        {
            //this.loaded = true;
        }
    }
}

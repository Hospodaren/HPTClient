using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    public class UCCombBetControl : UserControl
    {
        static void CombBetChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var uc = (UCCombBetControl)property;
            uc.CombBet = (HPTCombBet)args.NewValue;
        }

        public HPTCombBet CombBet
        {
            get { return (HPTCombBet)GetValue(CombBetProperty); }
            set { SetValue(CombBetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReductionCheckBoxList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CombBetProperty =
            DependencyProperty.Register("CombBet", typeof(HPTCombBet), typeof(UCCombBetControl), new UIPropertyMetadata(null, new PropertyChangedCallback(CombBetChangedCallBack)));


        public HPTConfig Config
        {
            get { return (HPTConfig)GetValue(ConfigProperty); }
            set { SetValue(ConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Config.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCCombBetControl), new UIPropertyMetadata(HPTConfig.Config));



    }
}

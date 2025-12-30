using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    public class UCMarkBetControl : UserControl
    {
        static void MarkBetChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var uc = (UCMarkBetControl)property;
            uc.MarkBet = (HPTMarkBet)args.NewValue;
        }

        public HPTMarkBet MarkBet
        {
            get
            {
                object o = GetValue(MarkBetProperty);
                if (o == null || o.GetType() != typeof(HPTMarkBet))
                {
                    try
                    {
                        HPTMarkBet markBet = (HPTMarkBet)this.DataContext;
                        SetValue(MarkBetProperty, markBet);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return (HPTMarkBet)GetValue(MarkBetProperty);
            }
            set
            {
                SetValue(MarkBetProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ReductionCheckBoxList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkBetProperty =
            DependencyProperty.Register("MarkBet", typeof(HPTMarkBet), typeof(UCMarkBetControl), new UIPropertyMetadata(null, new PropertyChangedCallback(MarkBetChangedCallBack)));


        public HPTConfig Config
        {
            get { return (HPTConfig)GetValue(ConfigProperty); }
            set { SetValue(ConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Config.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCMarkBetControl), new UIPropertyMetadata(HPTConfig.Config));



    }
}

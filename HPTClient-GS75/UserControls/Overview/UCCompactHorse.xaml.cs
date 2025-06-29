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
    /// Interaction logic for UCCompactHorse.xaml
    /// </summary>
    public partial class UCCompactHorse : UserControl
    {
        public UCCompactHorse()
        {
            InitializeComponent();
        }


        public HPTHorse Horse
        {
            get { return (HPTHorse)GetValue(HorseProperty); }
            set { SetValue(HorseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Horse.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorseProperty =
            DependencyProperty.Register("Horse", typeof(HPTHorse), typeof(UCCompactHorse), new PropertyMetadata(null));




        private System.Windows.Controls.Primitives.Popup pu;
        public System.Windows.Controls.Primitives.Popup PU
        {
            get
            {
                if (this.pu == null)
                {
                    this.pu = new System.Windows.Controls.Primitives.Popup()
                    {
                        Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                        HorizontalOffset = -10D,
                        VerticalOffset = -10D
                    };
                    this.pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
                }
                return this.pu;
            }
        }

        private void txtHorseName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            TextBlock tb = (TextBlock)sender;
            this.PU.DataContext = tb.DataContext;
            this.PU.Child = new UCResultView();
            this.PU.IsOpen = true;
        }

        private void txtDriverName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var fe = e.OriginalSource as FrameworkElement;
            if (fe.DataContext.GetType() == typeof(HPTHorse))
            {
                var horse = fe.DataContext as HPTHorse;
                var serviceConnector = new HPTServiceConnector();
                //serviceConnector.GetDriverInfoFromATG(horse);
                serviceConnector.GetHorseStartInformationFromATG(horse);

                if (horse.DriverInfo != null)
                {
                    var b = new Border()
                    {
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(1D),
                        Child = new UCDriverInfo()
                        {
                            BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                            BorderThickness = new Thickness(6D),
                            DataContext = horse.DriverInfo
                        }
                    };

                    this.PU.Child = b;
                    this.PU.IsOpen = true;
                }
            }
        }

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            //System.Windows.Controls.Primitives.Popup pu = (System.Windows.Controls.Primitives.Popup)sender;
            this.PU.Child = null;
            this.PU.IsOpen = false;
        }

        private void ucCompactHorse_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext == null)
            {
                return;
            }
            if (this.DataContext.GetType() == typeof(HPTHorse))
            {
                this.Horse = (HPTHorse)this.DataContext;
            }
        }

        private void ucCompactHorse_DataContextChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        //protected override Geometry GetLayoutClip(Size layoutSlotSize)
        //{
        //    return new RectangleGeometry(new Rect(new Size(layoutSlotSize.Height + 50, layoutSlotSize.Width + 50)));
        //}

        //public bool Expanded
        //{
        //    get { return (bool)GetValue(ExpandedProperty); }
        //    set { SetValue(ExpandedProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Expanded.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ExpandedProperty =
        //    DependencyProperty.Register("Expanded", typeof(bool), typeof(UCCompactHorse), new UIPropertyMetadata(false));

    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
                if (pu == null)
                {
                    pu = new System.Windows.Controls.Primitives.Popup()
                    {
                        Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                        HorizontalOffset = -10D,
                        VerticalOffset = -10D
                    };
                    pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
                }
                return pu;
            }
        }

        private void txtHorseName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            TextBlock tb = (TextBlock)sender;
            PU.DataContext = tb.DataContext;
            PU.Child = new UCResultView();
            PU.IsOpen = true;
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

                    PU.Child = b;
                    PU.IsOpen = true;
                }
            }
        }

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            //System.Windows.Controls.Primitives.Popup pu = (System.Windows.Controls.Primitives.Popup)sender;
            PU.Child = null;
            PU.IsOpen = false;
        }

        private void ucCompactHorse_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }
            if (DataContext.GetType() == typeof(HPTHorse))
            {
                Horse = (HPTHorse)DataContext;
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCGenericTabItemHeader.xaml
    /// </summary>
    public partial class UCGenericTabItemHeader : UserControl
    {
        public UCGenericTabItemHeader()
        {
            InitializeComponent();
        }

        public string TextSmall
        {
            get { return (string)GetValue(TextSmallProperty); }
            set { SetValue(TextSmallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextSmallProperty =
            DependencyProperty.Register("TextSmall", typeof(string), typeof(UCGenericTabItemHeader), new UIPropertyMetadata(string.Empty));


        public string TextBig
        {
            get { return (string)GetValue(TextBigProperty); }
            set { SetValue(TextBigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBigProperty =
            DependencyProperty.Register("TextBig", typeof(string), typeof(UCGenericTabItemHeader), new UIPropertyMetadata(string.Empty));


        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(UCGenericTabItemHeader), new UIPropertyMetadata(null));

        // Create a custom routed event by first registering a RoutedEventID
        // This event uses the bubbling routing strategy
        public static readonly RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent(
            "Close", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UCGenericTabItemHeader));


        // Provide CLR accessors for the event
        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseEvent, value); }
            remove { RemoveHandler(CloseEvent, value); }
        }

        private void StackPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(CloseEvent, Tag);
            RaiseEvent(newEventArgs);
        }
    }
}

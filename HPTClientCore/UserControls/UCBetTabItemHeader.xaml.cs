using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCBetTabItemHeader.xaml
    /// </summary>
    public partial class UCBetTabItemHeader : UserControl
    {
        public UCBetTabItemHeader()
        {
            InitializeComponent();
        }

        // Create a custom routed event by first registering a RoutedEventID
        // This event uses the bubbling routing strategy
        public static readonly RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent(
            "Close", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UCBetTabItemHeader));

        // Provide CLR accessors for the event
        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseEvent, value); }
            remove { RemoveHandler(CloseEvent, value); }
        }

        private void StackPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(UCBetTabItemHeader.CloseEvent, this.Tag);
            RaiseEvent(newEventArgs);
        }
    }
}

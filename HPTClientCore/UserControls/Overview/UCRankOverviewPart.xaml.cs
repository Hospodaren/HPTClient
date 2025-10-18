using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRankOverviewPart.xaml
    /// </summary>
    public partial class UCRankOverviewPart : UserControl
    {
        public UCRankOverviewPart()
        {
            InitializeComponent();
        }



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UCRankOverviewPart), new UIPropertyMetadata(string.Empty));


    }
}

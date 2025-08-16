using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTabItemHeader.xaml
    /// </summary>
    public partial class UCTabItemHeader : UserControl
    {
        public UCTabItemHeader()
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
            DependencyProperty.Register("Text", typeof(string), typeof(UCTabItemHeader), new UIPropertyMetadata(string.Empty));




        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Brush), typeof(UCTabItemHeader), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



    }
}

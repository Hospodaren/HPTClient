using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarkBetTemplateABCD.xaml
    /// </summary>
    public partial class UCMarkBetTemplateABCD : UserControl
    {
        public UCMarkBetTemplateABCD()
        {
            InitializeComponent();
        }

        public HPTConfig Config
        {
            get { return (HPTConfig)GetValue(ConfigProperty); }
            set { SetValue(ConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Config.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCMarkBetTemplateABCD), new UIPropertyMetadata(HPTConfig.Config));


        private void btnNewTemplate_Click(object sender, RoutedEventArgs e)
        {
            var newTemplate = new HPTMarkBetTemplateABCD();
            newTemplate.InitializeTemplate(new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C });
            Config.MarkBetTemplateABCDList.Add(newTemplate);
        }

        //private void btnClear_Click(object sender, RoutedEventArgs e)
        //{
        //    var btn = (Button)sender;
        //    var markBetTemplate = (HPTMarkBetTemplateABCD)btn.DataContext;
        //    markBetTemplate.
        //}

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var markBetTemplate = (HPTMarkBetTemplateABCD)btn.DataContext;
            Config.MarkBetTemplateABCDList.Remove(markBetTemplate);
        }
    }
}

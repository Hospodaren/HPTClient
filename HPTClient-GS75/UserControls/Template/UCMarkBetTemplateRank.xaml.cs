using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarkBetTemplateRank.xaml
    /// </summary>
    public partial class UCMarkBetTemplateRank : UserControl
    {
        public UCMarkBetTemplateRank()
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
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCMarkBetTemplateRank), new UIPropertyMetadata(HPTConfig.Config));


        private void btnNewTemplate_Click(object sender, RoutedEventArgs e)
        {
            var newTemplate = new HPTMarkBetTemplateRank();
            this.Config.MarkBetTemplateRankList.Add(newTemplate);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var markBetTemplate = (HPTMarkBetTemplateRank)btn.DataContext;
            this.Config.MarkBetTemplateRankList.Remove(markBetTemplate);
        }
    }
}

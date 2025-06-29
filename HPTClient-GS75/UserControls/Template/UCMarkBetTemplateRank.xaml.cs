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

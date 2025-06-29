using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for UCGroupIntervalTemplateList.xaml
    /// </summary>
    public partial class UCGroupIntervalTemplateList : UserControl
    {
        public UCGroupIntervalTemplateList()
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
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCGroupIntervalTemplateList), new UIPropertyMetadata(HPTConfig.Config));

        private void btnNewGroupIntervalTemplate_Click(object sender, RoutedEventArgs e)
        {
            this.Config.GroupIntervalRulesCollectionList.Add(new HPTGroupIntervalRulesCollection()
            {
                TypeCategory = BetTypeCategory.V75,
                Name = "Ny gruppintervallmall " + DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToShortTimeString(),
                ReductionRuleList = new ObservableCollection<HPTNumberOfWinnersReductionRule>()
            });
        }
    }
}

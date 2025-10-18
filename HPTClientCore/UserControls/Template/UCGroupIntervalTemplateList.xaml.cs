using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCHorseRankSumReductionTemplateList.xaml
    /// </summary>
    public partial class UCHorseRankSumReductionTemplateList : UserControl
    {
        public UCHorseRankSumReductionTemplateList()
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
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCHorseRankSumReductionTemplateList), new UIPropertyMetadata(HPTConfig.Config));

        private void btnNewHorseRankReductionTemplate_Click(object sender, RoutedEventArgs e)
        {
            this.Config.RankSumReductionRuleCollection.Add(new HPTHorseRankSumReductionRuleCollection()
            {
                TypeCategory = BetTypeCategory.V75,
                Name = "Ny rankreduceringsmall " + DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToShortTimeString(),
                RankSumReductionRuleList = new ObservableCollection<HPTHorseRankSumReductionRule>()
            });
        }
    }
}

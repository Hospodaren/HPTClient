using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

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

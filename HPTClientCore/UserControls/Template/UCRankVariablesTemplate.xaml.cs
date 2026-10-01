using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRankVariablesTemplate.xaml
    /// </summary>
    public partial class UCRankVariablesTemplate : UserControl
    {
        public UCRankVariablesTemplate()
        {
            InitializeComponent();
        }

        private HPTRankTemplate rankTemplate;
        public HPTRankTemplate RankTemplate
        {
            get
            {
                if (DataContext.GetType() == typeof(HPTRankTemplate))
                {
                    return (HPTRankTemplate)DataContext;
                }
                return null;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            RankTemplate.Name = txtTemplateName.Text;

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var hptHorseRankVariable in RankTemplate.HorseRankVariableList)
            {
                hptHorseRankVariable.Use = false;
                hptHorseRankVariable.Weight = 1M;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            HPTConfig.Config.RankTemplateList.Remove(RankTemplate);
        }
    }
}

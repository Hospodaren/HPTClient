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
                if (this.DataContext.GetType() == typeof(HPTRankTemplate))
                {
                    return (HPTRankTemplate)this.DataContext;
                }
                return null;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.RankTemplate.Name = this.txtTemplateName.Text;

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var hptHorseRankVariable in this.RankTemplate.HorseRankVariableList)
            {
                hptHorseRankVariable.Use = false;
                hptHorseRankVariable.Weight = 1M;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            HPTConfig.Config.RankTemplateList.Remove(this.RankTemplate);
        }
    }
}

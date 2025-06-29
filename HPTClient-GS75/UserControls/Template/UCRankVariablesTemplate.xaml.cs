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
using Xceed.Wpf.Toolkit;

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

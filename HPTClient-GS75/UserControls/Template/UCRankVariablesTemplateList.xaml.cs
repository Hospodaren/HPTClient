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
using Microsoft.Win32;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRankVariablesTemplateList.xaml
    /// </summary>
    public partial class UCRankVariablesTemplateList : UserControl
    {
        public UCRankVariablesTemplateList()
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
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCRankVariablesTemplateList), new UIPropertyMetadata(HPTConfig.Config));

        private void btnNewRankVariableTemplate_Click(object sender, RoutedEventArgs e)
        {
            var rankVariableTemplate = new HPTRankTemplate()
                                           {
                                               Name = "Ny mall (" + DateTime.Now.ToString("yyyy-MM-dd") + ", " + DateTime.Now.ToShortTimeString()
                                           };
            rankVariableTemplate.InitializeTemplate();

            if (this.Config.RankTemplateList == null)
            {
                this.Config.RankTemplateList = new System.Collections.ObjectModel.ObservableCollection<HPTRankTemplate>();
            }
            this.Config.RankTemplateList.Add(rankVariableTemplate);
        }

    }
}

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
    /// Interaction logic for UCMarkBetTemplateABCD.xaml
    /// </summary>
    public partial class UCMarkBetTemplateABCD : UserControl
    {
        public UCMarkBetTemplateABCD()
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
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCMarkBetTemplateABCD), new UIPropertyMetadata(HPTConfig.Config));

        
        private void btnNewTemplate_Click(object sender, RoutedEventArgs e)
        {
            var newTemplate = new HPTMarkBetTemplateABCD();
            newTemplate.InitializeTemplate(new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C });
            this.Config.MarkBetTemplateABCDList.Add(newTemplate);
        }

        //private void btnClear_Click(object sender, RoutedEventArgs e)
        //{
        //    var btn = (Button)sender;
        //    var markBetTemplate = (HPTMarkBetTemplateABCD)btn.DataContext;
        //    markBetTemplate.
        //}

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var markBetTemplate = (HPTMarkBetTemplateABCD)btn.DataContext;
            this.Config.MarkBetTemplateABCDList.Remove(markBetTemplate);
        }
    }
}

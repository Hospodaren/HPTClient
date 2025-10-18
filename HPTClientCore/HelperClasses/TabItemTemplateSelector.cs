using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    public class TabItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
}

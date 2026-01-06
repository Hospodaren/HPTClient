using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCGUISettings.xaml
    /// </summary>
    public partial class UCGUISettings : UserControl
    {
        public UCGUISettings()
        {
            InitializeComponent();
        }

        private HPTGUIElementsToShow guiElementsToShow;
        internal HPTGUIElementsToShow GUIElementsToShow
        {
            get
            {
                guiElementsToShow = (HPTGUIElementsToShow)DataContext;
                return guiElementsToShow;
            }
        }

        private void cmbGUIProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGUIProfile.SelectedIndex == (int)GUIElementsToShow.GUIProfile)
            {
                return;
            }

            var guiProfile = (GUIProfile)cmbGUIProfile.SelectedIndex;
            GUIElementsToShow.IsDefault = false;
            var guiElementsToShow = HPTConfig.Config.GUIElementsToShowList.First(ets => ets.GUIProfile == guiProfile);
            guiElementsToShow.IsDefault = true;
            DataContext = guiElementsToShow;
            HPTConfig.Config.GUIElementsToShow = GUIElementsToShow;
        }

        private void btnResetProfile_Click(object sender, RoutedEventArgs e)
        {
            HPTConfig.Config.GUIElementsToShowList.Remove(GUIElementsToShow);
            var guiElementsToShow = HPTConfig.Config.GetElementsToShow(GUIElementsToShow.GUIProfile);
            guiElementsToShow.IsDefault = true;
            DataContext = guiElementsToShow;
            HPTConfig.Config.GUIElementsToShow = GUIElementsToShow;
            HPTConfig.Config.GUIElementsToShowList.Add(guiElementsToShow);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                switch (GUIElementsToShow.GUIProfile)
                {
                    case GUIProfile.Simple:
                        cmbGUIProfile.SelectedIndex = 0;
                        break;
                    case GUIProfile.Normal:
                        cmbGUIProfile.SelectedIndex = 1;
                        break;
                    case GUIProfile.Advanced:
                        cmbGUIProfile.SelectedIndex = 2;
                        break;
                    case GUIProfile.Custom:
                        break;
                    default:
                        break;
                }
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}

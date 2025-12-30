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
                this.guiElementsToShow = (HPTGUIElementsToShow)this.DataContext;
                return this.guiElementsToShow;
            }
        }

        private void cmbGUIProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbGUIProfile.SelectedIndex == (int)this.GUIElementsToShow.GUIProfile)
            {
                return;
            }

            var guiProfile = (GUIProfile)this.cmbGUIProfile.SelectedIndex;
            this.GUIElementsToShow.IsDefault = false;
            var guiElementsToShow = HPTConfig.Config.GUIElementsToShowList.First(ets => ets.GUIProfile == guiProfile);
            guiElementsToShow.IsDefault = true;
            this.DataContext = guiElementsToShow;
            HPTConfig.Config.GUIElementsToShow = this.GUIElementsToShow;
        }

        private void btnResetProfile_Click(object sender, RoutedEventArgs e)
        {
            HPTConfig.Config.GUIElementsToShowList.Remove(this.GUIElementsToShow);
            var guiElementsToShow = HPTConfig.Config.GetElementsToShow(this.GUIElementsToShow.GUIProfile);
            guiElementsToShow.IsDefault = true;
            this.DataContext = guiElementsToShow;
            HPTConfig.Config.GUIElementsToShow = this.GUIElementsToShow;
            HPTConfig.Config.GUIElementsToShowList.Add(guiElementsToShow);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                switch (this.GUIElementsToShow.GUIProfile)
                {
                    case GUIProfile.Simple:
                        this.cmbGUIProfile.SelectedIndex = 0;
                        break;
                    case GUIProfile.Normal:
                        this.cmbGUIProfile.SelectedIndex = 1;
                        break;
                    case GUIProfile.Advanced:
                        this.cmbGUIProfile.SelectedIndex = 2;
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

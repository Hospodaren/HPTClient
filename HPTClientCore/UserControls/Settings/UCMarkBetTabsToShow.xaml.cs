using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarkBetTabsToShow_.xaml
    /// </summary>
    public partial class UCMarkBetTabsToShow : UserControl
    {
        public UCMarkBetTabsToShow()
        {
            MarkBetTabsToShowList = new ObservableCollection<CheckBox>();
            InitializeComponent();
        }

        private HPTMarkBetTabsToShow markBetTabsToShow;
        internal HPTMarkBetTabsToShow MarkBetTabsToShow
        {
            get
            {
                markBetTabsToShow = (HPTMarkBetTabsToShow)DataContext;
                return markBetTabsToShow;
            }
        }

        public ObservableCollection<CheckBox> MarkBetTabsToShowList
        {
            get { return (ObservableCollection<CheckBox>)GetValue(MarkBetTabsToShowListProperty); }
            set { SetValue(MarkBetTabsToShowListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsToShowList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkBetTabsToShowListProperty =
            DependencyProperty.Register("MarkBetTabsToShowList", typeof(ObservableCollection<CheckBox>), typeof(UCMarkBetTabsToShow), new UIPropertyMetadata(null));

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                object o = DataContext;
                //ATGCalendar ac = (ATGCalendar)this.DataContext;
                //this.DataContext = ac.Config.MarkBetTabsToShow;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Skapa context menu för vilk flikar man vill visa
                switch (MarkBetTabsToShow.GUIProfile)
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
                CreateMarkBetTabsToShowList();
            }
        }

        private void CreateMarkBetTabsToShowList()
        {
            List<HPTMarkBetTabsToShowAttribute> tabsToShowAttributeList =
                    HPTConfig.Config.MarkBetTabsToShow.GetMarkBetTabsToShowAttributes();
            MarkBetTabsToShowList.Clear();
            foreach (HPTMarkBetTabsToShowAttribute hda in tabsToShowAttributeList)
            {
                CheckBox chk = new CheckBox()
                {
                    IsChecked = (bool)MarkBetTabsToShow.GetType().GetProperty(hda.PropertyName).GetValue(MarkBetTabsToShow, null),
                    Content = hda.Name
                };
                chk.SetBinding(CheckBox.IsCheckedProperty, hda.PropertyName);
                //if (!HPTConfig.Config.IsPayingCustomer && hda.RequiresPRO)
                //{
                //    chk.IsEnabled = false;
                //}
                //else
                //{
                //    chk.SetBinding(MenuItem.IsCheckedProperty, hda.PropertyName);
                //}
                chk.SetBinding(MenuItem.IsCheckedProperty, hda.PropertyName);
                MarkBetTabsToShowList.Add(chk);
            }
            BindingOperations.GetBindingExpression(icMarkBetTabsToShow, ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        private void cmbGUIProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGUIProfile.SelectedIndex == (int)MarkBetTabsToShow.GUIProfile)
            {
                return;
            }

            var guiProfile = (GUIProfile)cmbGUIProfile.SelectedIndex;
            MarkBetTabsToShow.IsDefault = false;
            var markBetTabsToShow = HPTConfig.Config.GetMarkBetTabsToShow(guiProfile);
            markBetTabsToShow.IsDefault = true;
            DataContext = markBetTabsToShow;
            HPTConfig.Config.SetMarkBetTabsToShow(markBetTabsToShow);
            CreateMarkBetTabsToShowList();
        }

        private void btnResetProfile_Click(object sender, RoutedEventArgs e)
        {
            var markBetTabsToShow = HPTConfig.CreateMarkBetTabsToShow(MarkBetTabsToShow.GUIProfile);
            markBetTabsToShow.IsDefault = true;
            DataContext = markBetTabsToShow;
            HPTConfig.Config.SetMarkBetTabsToShow(markBetTabsToShow);
            CreateMarkBetTabsToShowList();
        }
    }
}

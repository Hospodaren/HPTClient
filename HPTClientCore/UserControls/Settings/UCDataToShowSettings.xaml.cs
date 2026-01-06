using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCSettings.xaml
    /// </summary>
    public partial class UCDataToShowSettings : UserControl
    {
        public UCDataToShowSettings()
        {
            //this.ColumnsToShowList = new ObservableCollection<CheckBox>();
            InitializeComponent();
        }

        public object Text
        {
            get
            {
                return gbSettings.Header;
            }
            set
            {
                gbSettings.Header = value;
            }
        }

        private HPTDataToShow dataToShow;
        internal HPTDataToShow DataToShow
        {
            get
            {
                //this.dataToShow = (HPTHorseDataToShow)this.DataContext;
                dataToShow = (HPTDataToShow)DataContext;
                return dataToShow;
            }
        }



        public Visibility ProfileSelectVisibility
        {
            get { return (Visibility)GetValue(ProfileSelectVisibilityProperty); }
            set { SetValue(ProfileSelectVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProfileSelectVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProfileSelectVisibilityProperty =
            DependencyProperty.Register("ProfileSelectVisibility", typeof(Visibility), typeof(UCDataToShowSettings), new PropertyMetadata(Visibility.Visible));



        public ObservableCollection<ListBoxItem> ColumnsToShowList
        {
            get { return (ObservableCollection<ListBoxItem>)GetValue(ColumnsToShowListProperty); }
            set { SetValue(ColumnsToShowListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsToShowList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsToShowListProperty =
            DependencyProperty.Register("ColumnsToShowList", typeof(ObservableCollection<ListBoxItem>), typeof(UCDataToShowSettings), new UIPropertyMetadata(null));

        //public ObservableCollection<CheckBox> ColumnsToShowList
        //{
        //    get { return (ObservableCollection<CheckBox>)GetValue(ColumnsToShowListProperty); }
        //    set { SetValue(ColumnsToShowListProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ColumnsToShowList.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ColumnsToShowListProperty =
        //    DependencyProperty.Register("ColumnsToShowList", typeof(ObservableCollection<CheckBox>), typeof(UCDataToShowSettings), new UIPropertyMetadata(null));



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Skapa kontextmeny för att visa/dölja kolumner
                if (ColumnsToShowList == null || ColumnsToShowList.Count == 0)
                {
                    CreateColumnsToShowList();
                    switch (DataToShow.GUIProfile)
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
        }

        private void CreateColumnsToShowList()
        {
            if (ColumnsToShowList == null)
            {
                ColumnsToShowList = new ObservableCollection<ListBoxItem>();
            }
            ColumnsToShowList.Clear();
            BindingOperations.GetBindingExpression(icColumnsToShow, ListBox.ItemsSourceProperty).UpdateTarget();
            List<HorseDataToShowAttribute> attributeList = DataToShow.GetHorseDataToShowAttributes();
            foreach (HorseDataToShowAttribute hda in attributeList)
            {
                CheckBox chk = new CheckBox()
                {
                    IsChecked = (bool)DataToShow.GetType().GetProperty(hda.PropertyName).GetValue(DataToShow, null),
                    Content = hda.Name,
                    IsEnabled = hda.RequiresPro ? HPTConfig.Config.IsPayingCustomer : true
                };
                ListBoxItem lbi = new ListBoxItem()
                {
                    Content = chk
                };
                chk.SetBinding(CheckBox.IsCheckedProperty, hda.PropertyName);
                ColumnsToShowList.Add(lbi);
            }
            BindingOperations.GetBindingExpression(icColumnsToShow, ListBox.ItemsSourceProperty).UpdateTarget();
        }

        private void cmbGUIProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGUIProfile.SelectedIndex == (int)DataToShow.GUIProfile)
            {
                return;
            }

            var guiProfile = (GUIProfile)cmbGUIProfile.SelectedIndex;
            DataToShow.IsDefault = false;
            var dataToShow = HPTConfig.Config.GetDataToShow(DataToShow.Usage, guiProfile);
            dataToShow.IsDefault = true;
            DataContext = dataToShow;
            HPTConfig.Config.SetDataToShow(dataToShow);
            CreateColumnsToShowList();
        }

        private void btnResetProfile_Click(object sender, RoutedEventArgs e)
        {
            var dataToShow = HPTConfig.CreateDataToShow(DataToShow.Usage, DataToShow.GUIProfile);
            DataToShow.IsDefault = true;
            DataContext = dataToShow;
            HPTConfig.Config.SetDataToShow(dataToShow);
            CreateColumnsToShowList();
        }
    }
}

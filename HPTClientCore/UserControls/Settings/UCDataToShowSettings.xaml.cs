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
                return this.gbSettings.Header;
            }
            set
            {
                this.gbSettings.Header = value;
            }
        }

        private HPTDataToShow dataToShow;
        internal HPTDataToShow DataToShow
        {
            get
            {
                //this.dataToShow = (HPTHorseDataToShow)this.DataContext;
                this.dataToShow = (HPTDataToShow)this.DataContext;
                return this.dataToShow;
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
                if (this.ColumnsToShowList == null || this.ColumnsToShowList.Count == 0)
                {
                    CreateColumnsToShowList();
                    switch (this.DataToShow.GUIProfile)
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
        }

        private void CreateColumnsToShowList()
        {
            if (this.ColumnsToShowList == null)
            {
                this.ColumnsToShowList = new ObservableCollection<ListBoxItem>();
            }
            this.ColumnsToShowList.Clear();
            BindingOperations.GetBindingExpression(this.icColumnsToShow, ListBox.ItemsSourceProperty).UpdateTarget();
            List<HorseDataToShowAttribute> attributeList = this.DataToShow.GetHorseDataToShowAttributes();
            foreach (HorseDataToShowAttribute hda in attributeList)
            {
                CheckBox chk = new CheckBox()
                {
                    IsChecked = (bool)this.DataToShow.GetType().GetProperty(hda.PropertyName).GetValue(this.DataToShow, null),
                    Content = hda.Name,
                    IsEnabled = hda.RequiresPro ? HPTConfig.Config.IsPayingCustomer : true
                };
                ListBoxItem lbi = new ListBoxItem()
                {
                    Content = chk
                };
                chk.SetBinding(CheckBox.IsCheckedProperty, hda.PropertyName);
                this.ColumnsToShowList.Add(lbi);
            }
            BindingOperations.GetBindingExpression(this.icColumnsToShow, ListBox.ItemsSourceProperty).UpdateTarget();
        }

        private void cmbGUIProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbGUIProfile.SelectedIndex == (int)this.DataToShow.GUIProfile)
            {
                return;
            }

            var guiProfile = (GUIProfile)this.cmbGUIProfile.SelectedIndex;
            this.DataToShow.IsDefault = false;
            var dataToShow = HPTConfig.Config.GetDataToShow(this.DataToShow.Usage, guiProfile);
            dataToShow.IsDefault = true;
            this.DataContext = dataToShow;
            HPTConfig.Config.SetDataToShow(dataToShow);
            CreateColumnsToShowList();
        }

        private void btnResetProfile_Click(object sender, RoutedEventArgs e)
        {
            var dataToShow = HPTConfig.CreateDataToShow(this.DataToShow.Usage, this.DataToShow.GUIProfile);
            DataToShow.IsDefault = true;
            this.DataContext = dataToShow;
            HPTConfig.Config.SetDataToShow(dataToShow);
            CreateColumnsToShowList();
        }
    }
}

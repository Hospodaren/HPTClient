using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCRaceViewGrouped.xaml
    /// </summary>
    public partial class UCRaceViewGrouped : UCMarkBetControl
    {
        public UCRaceViewGrouped()
        {
            CMColumnsToShow = new ContextMenu();
            InitializeComponent();
        }

        ~UCRaceViewGrouped()
        {
            // Ta bort eventhandlers för ändringar i konfigurationen (programmet läcker minne annars)
            try
            {
                HPTConfig.Config.PropertyChanged -= new PropertyChangedEventHandler(Config_PropertyChanged);
                if (HorseListContainer != null)
                {
                    HorseListContainer.ParentRaceDayInfo.DataToShow.PropertyChanged -= new PropertyChangedEventHandler(DataToShow_PropertyChanged);
                }
                if (gvwVxxSpel != null)
                {
                    gvwVxxSpel.Columns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #region Groupdescription

        static void GroupDescriptionChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var uc = (UCRaceViewGrouped)property;
            uc.lvwLopp.Items.GroupDescriptions.Clear();
            uc.lvwLopp.Items.GroupDescriptions.Add(new PropertyGroupDescription(uc.GroupDescriptionName));
        }

        public string GroupDescriptionName
        {
            get { return (string)GetValue(GroupDescriptionNameProperty); }
            set { SetValue(GroupDescriptionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupDescriptionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupDescriptionNameProperty =
            DependencyProperty.Register("GroupDescriptionName", typeof(string), typeof(UCRaceViewGrouped), new UIPropertyMetadata(null, new PropertyChangedCallback(GroupDescriptionChangedCallBack)));

        #endregion

        #region Visa bara valda

        static void ShowOnlySelectedChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var uc = (UCRaceViewGrouped)property;
            uc.lvwLopp.Items.Filter = new Predicate<object>(uc.FilterSelected);
        }

        public bool ShowOnlySelected
        {
            get { return (bool)GetValue(ShowOnlySelectedProperty); }
            set { SetValue(ShowOnlySelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowOnlySelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowOnlySelectedProperty =
            DependencyProperty.Register("ShowOnlySelected", typeof(bool), typeof(UCRaceViewGrouped), new UIPropertyMetadata(false, new PropertyChangedCallback(ShowOnlySelectedChangedCallBack)));

        public void FilterSelected()
        {
            lvwLopp.Items.Filter = new Predicate<object>(FilterSelected);
        }

        public bool FilterSelected(object obj)
        {
            var horse = obj as HPTHorse;
            return horse.Selected == ShowOnlySelected;
        }

        #endregion

        public HPTBetType BetType
        {
            get
            {
                if (MarkBet != null)
                {
                    return MarkBet.BetType;
                }
                if (horseListContainer.ParentRaceDayInfo.BetType != null)
                {
                    return horseListContainer.ParentRaceDayInfo.BetType;
                }
                if (DataContext.GetType() == typeof(HPTRace))
                {
                    var race = (HPTRace)DataContext;
                    return race.ParentRaceDayInfo.BetType;
                }
                return new HPTBetType()
                {
                    Code = string.Empty
                };
            }
        }

        internal IHorseListContainer horseListContainer;
        internal IHorseListContainer HorseListContainer
        {
            get
            {
                if (horseListContainer == null || (DataContext != null && !DataContext.Equals(horseListContainer)))
                {
                    if (DataContext != null)
                    {
                        horseListContainer = (IHorseListContainer)DataContext;
                    }
                }
                return horseListContainer;
            }
        }

        void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                HPTRace race = (HPTRace)DataContext;
                if (e.PropertyName == "UseA" || e.PropertyName == "UseB" || e.PropertyName == "UseC" || e.PropertyName == "UseD" || e.PropertyName == "UseE" || e.PropertyName == "UseF")
                {
                    if (double.IsNaN(gvcABCD.Width) && race.ParentRaceDayInfo.DataToShow.ShowPrio)
                    {
                        gvcABCD.Width = gvcABCD.ActualWidth;
                        gvcABCD.Width = double.NaN;
                    }
                }
            }
            catch (Exception exc)
            {
                string fel = exc.Message;
            }
        }

        private bool HandleUpDown_KeyUp(object sender, KeyEventArgs e)
        {
            var fe = e.OriginalSource as FrameworkElement;
            object horse = fe.DataContext;

            int indexChange = 0;
            switch (e.Key)
            {
                case Key.PageDown:
                    indexChange = 1;
                    break;
                case Key.PageUp:
                    indexChange = -1;
                    break;
                case Key.Left:
                case Key.Subtract:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        indexChange = -1;
                    }
                    else
                    {
                        e.Handled = false;
                        return false;
                    }
                    break;
                case Key.Right:
                case Key.Add:
                    if (Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        indexChange = 1;
                    }
                    else
                    {
                        e.Handled = false;
                        return false;
                    }
                    break;
                default:
                    e.Handled = false;
                    return false;
            }

            var container = lvwLopp.ItemContainerGenerator.ContainerFromItem(horse);
            int containerIndex = lvwLopp.ItemContainerGenerator.IndexFromContainer(container);
            int newIndex = containerIndex + indexChange;
            if (newIndex == -1)
            {
                newIndex = lvwLopp.Items.Count - 1;
            }
            else if (newIndex >= lvwLopp.Items.Count)
            {
                newIndex = 0;
            }
            lvwLopp.SelectedIndex = newIndex;
            return true;
        }

        private void dudOwnProbability_KeyUp(object sender, KeyEventArgs e)
        {
            //if (this.lvwLopp.SelectedIndex == -1)
            //{
            //    e.Handled = true;
            //    return;
            //}

            //HandleUpDown_KeyUp(sender, e);

            if (HandleUpDown_KeyUp(sender, e))
            {
                var newItem = (ListViewItem)lvwLopp.ItemContainerGenerator.ContainerFromIndex(lvwLopp.SelectedIndex);
                var dud = (DecimalUpDown)FindNamedChild(newItem, "dudOwnProbability");
                if (dud != null)
                {
                    dud.Focus();
                }
            }


            e.Handled = true;
        }

        private void iudOwnRank_KeyUp(object sender, KeyEventArgs e)
        {
            //HandleUpDown_KeyUp(sender, e);

            //if (this.lvwLopp.SelectedIndex == -1)
            //{
            //    return;
            //}
            if (HandleUpDown_KeyUp(sender, e))
            {
                var newItem = (ListViewItem)lvwLopp.ItemContainerGenerator.ContainerFromIndex(lvwLopp.SelectedIndex);
                var iud = (IntegerUpDown)FindNamedChild(newItem, "iudOwnrank");
                if (iud != null)
                {
                    iud.Focus();
                }
            }
            e.Handled = true;
        }

        private void iudAlternateRank_KeyUp(object sender, KeyEventArgs e)
        {
            HandleUpDown_KeyUp(sender, e);

            if (lvwLopp.SelectedIndex == -1)
            {
                return;
            }
            var newItem = (ListViewItem)lvwLopp.ItemContainerGenerator.ContainerFromIndex(lvwLopp.SelectedIndex);
            var iud = (IntegerUpDown)FindNamedChild(newItem, "iudAlternateRank");
            if (iud != null)
            {
                iud.Focus();
            }
            e.Handled = true;
        }

        public static object FindNamedChild(DependencyObject container, string name)
        {
            if (container is FrameworkElement)
            {
                if ((container as FrameworkElement).Name == name) return container;
            }
            var ccount = VisualTreeHelper.GetChildrenCount(container);
            for (int i = 0; i < ccount; i++)
            {
                var child = VisualTreeHelper.GetChild(container, i);
                var target = FindNamedChild(child, name);
                if (target != null)
                {
                    return target;
                }
            }
            return null;
        }

        private void lvwLopp_KeyUp(object sender, KeyEventArgs e)
        {
            if (lvwLopp.SelectedItem != null)
            {
                HPTHorse horse = (HPTHorse)lvwLopp.SelectedItem;
                if (horse.Scratched == true)
                {
                    e.Handled = false;
                    return;
                }
                HPTHorseXReduction reduction = null;
                switch (e.Key)
                {
                    case Key.A:
                        reduction = horse.HorseXReductionList.First(xr => xr.Prio == HPTPrio.A);
                        reduction.Selected = !reduction.Selected;
                        horse.Selected = reduction.Selected;
                        break;
                    case Key.B:
                        reduction = horse.HorseXReductionList.First(xr => xr.Prio == HPTPrio.B);
                        reduction.Selected = !reduction.Selected;
                        horse.Selected = reduction.Selected;
                        break;
                    case Key.C:
                        reduction = horse.HorseXReductionList.First(xr => xr.Prio == HPTPrio.C);
                        reduction.Selected = !reduction.Selected;
                        horse.Selected = reduction.Selected;
                        break;
                    case Key.D:
                        reduction = horse.HorseXReductionList.First(xr => xr.Prio == HPTPrio.D);
                        reduction.Selected = !reduction.Selected;
                        horse.Selected = reduction.Selected;
                        break;
                    case Key.E:
                        reduction = horse.HorseXReductionList.First(xr => xr.Prio == HPTPrio.E);
                        reduction.Selected = !reduction.Selected;
                        horse.Selected = reduction.Selected;
                        break;
                    case Key.F:
                        reduction = horse.HorseXReductionList.First(xr => xr.Prio == HPTPrio.F);
                        reduction.Selected = !reduction.Selected;
                        horse.Selected = reduction.Selected;
                        break;
                    case Key.D0:
                        break;
                    case Key.D1:
                        break;
                    case Key.D2:
                        break;
                    case Key.D3:
                        break;
                    case Key.D4:
                        break;
                    case Key.D5:
                        break;
                    case Key.D6:
                        break;
                    case Key.D7:
                        break;
                    case Key.D8:
                        break;
                    case Key.D9:
                        break;
                    case Key.R:
                        // Reserv
                        break;
                    case Key.Space:
                        horse.Selected = !horse.Selected;
                        break;
                    default:
                        break;
                }
            }
            e.Handled = false;

            //var item = 
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            //var gd = new PropertyGroupDescription("ParentRace.LegNrString");
            GridViewColumnHeader column = e.OriginalSource as GridViewColumnHeader;
            if (column == null)
            {
                return;
            }

            String field = column.Tag as String;
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            ListSortDirection newDir = ListSortDirection.Ascending;

            if (lvwLopp.Items.SortDescriptions.Count > 0)
            {
                if (string.IsNullOrEmpty(GroupDescriptionName))
                {
                    SortDescription sd = lvwLopp.Items.SortDescriptions[0];
                    if (sd.PropertyName == field)
                    {
                        SortDescription sdNew = new SortDescription();
                        sdNew.PropertyName = sd.PropertyName;
                        sdNew.Direction = sd.Direction == ListSortDirection.Ascending
                                              ? ListSortDirection.Descending
                                              : ListSortDirection.Ascending;
                        lvwLopp.Items.SortDescriptions.Clear();
                        lvwLopp.Items.SortDescriptions.Add(sdNew);
                        return;
                    }
                }
                else
                {
                    if (lvwLopp.Items.SortDescriptions.Count > 1)
                    {
                        SortDescription sd = lvwLopp.Items.SortDescriptions[1];
                        if (sd.PropertyName == field)
                        {
                            SortDescription sdNew = new SortDescription();
                            sdNew.PropertyName = sd.PropertyName;
                            sdNew.Direction = sd.Direction == ListSortDirection.Ascending
                                                  ? ListSortDirection.Descending
                                                  : ListSortDirection.Ascending;
                            lvwLopp.Items.SortDescriptions.RemoveAt(1);
                            lvwLopp.Items.SortDescriptions.Insert(1, sdNew);
                            return;
                        }
                    }
                }
            }

            // Hantera fält som ska sorteras fallande som default
            switch (field)
            {
                //case "MarksQuantity":
                //case "MarksShare":
                case "Markability":
                case "SelectedForComplimentaryRule":
                case "EarningsMeanLast5":
                case "TotalStatistics.Earning":
                case "TrioIndex":
                case "PercentFirst":
                case "PercentSecond":
                case "PercentThird":
                case "StakeDistributionShare":
                    newDir = ListSortDirection.Descending;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(GroupDescriptionName))
            {
                // Aldrig sortering på mer än två variabler
                if (lvwLopp.Items.SortDescriptions.Count > 1)
                {
                    lvwLopp.Items.SortDescriptions.RemoveAt(1); ;
                }
                lvwLopp.Items.SortDescriptions.Insert(0, new SortDescription(field, newDir));
            }
            else
            {
                lvwLopp.Items.SortDescriptions.Clear();
                lvwLopp.Items.SortDescriptions.Add(new SortDescription(GroupDescriptionName, ListSortDirection.Ascending));
                lvwLopp.Items.SortDescriptions.Add(new SortDescription(field, newDir));
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // TEMPORÄRT BORTTAGET!
                //    // Ta bort eventhandlers för ändringar i konfigurationen (programmet läcker minne annars)
                //    try
                //    {
                //        HPTConfig.Config.PropertyChanged -= new PropertyChangedEventHandler(Config_PropertyChanged);
                //        if (this.HorseListContainer != null)
                //        {
                //            this.HorseListContainer.ParentRaceDayInfo.DataToShow.PropertyChanged -= new PropertyChangedEventHandler(DataToShow_PropertyChanged);
                //        }
                //        if (this.gvwVxxSpel != null)
                //        {
                //            this.gvwVxxSpel.Columns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                //        }
                //    }
                //    catch (Exception exc)
                //    {
                //        string s = exc.Message;
                //    }
            }
        }

        private void SortColumns()
        {
            if (HorseListContainer.ParentRaceDayInfo.DataToShow.ColumnsInOrder == null
                || HorseListContainer.ParentRaceDayInfo.DataToShow.ColumnsInOrder.Count == 0
                || HorseListContainer.ParentRaceDayInfo.DataToShow.ColumnsInOrder.First() == string.Empty)
            {
                return;
            }

            //this.gvwVxxSpel.Columns.Clear();

            for (int i = 0; i < HorseListContainer.ParentRaceDayInfo.DataToShow.ColumnsInOrder.Count; i++)
            {
                string columnName = HorseListContainer.ParentRaceDayInfo.DataToShow.ColumnsInOrder[i];
                ColumnHandler columnHandler = ColumnHandlerList.FirstOrDefault(ch => columnName == ch.Name);
                if (columnHandler != null)
                {
                    int currentPosition = gvwVxxSpel.Columns.IndexOf(columnHandler.Column);
                    if (currentPosition != -1)
                    {
                        int newPosition = i;
                        if (i >= gvwVxxSpel.Columns.Count)
                        {
                            newPosition = gvwVxxSpel.Columns.Count - 1;
                        }
                        gvwVxxSpel.Columns.Move(currentPosition, newPosition);
                    }
                    //this.gvwVxxSpel.Columns.Add(columnHandler.Column);
                }
            }
        }

        void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SaveColumnOrder();
        }

        private void SaveColumnOrder()
        {
            HorseListContainer.ParentRaceDayInfo.DataToShow.ColumnsInOrder = new List<string>();
            for (int i = 0; i < gvwVxxSpel.Columns.Count; i++)
            {
                ColumnHandler columnHandler = ColumnHandlerList.First(ch => ch.Column == gvwVxxSpel.Columns[i]);
                columnHandler.Position = i;
                HorseListContainer.ParentRaceDayInfo.DataToShow.ColumnsInOrder.Add(columnHandler.Name);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Initialize();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void Initialize()
        {
            //if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // Skapa kontextmeny för att visa/dölja kolumner
                //if (this.HorseListContainer != null && this.HorseListContainer.ParentRaceDayInfo.DataToShow.EnableConfiguration)
                if (HorseListContainer != null)
                {
                    //if (this.HorseListContainer.ParentRaceDayInfo.DataToShow.CMColumnsToShow == null || !this.HorseListContainer.ParentRaceDayInfo.DataToShow.CMColumnsToShow.HasItems)
                    if (!CMColumnsToShow.HasItems)
                    {
                        //this.CMColumnsToShow = new ContextMenu();
                        List<HorseDataToShowAttribute> attributeList = HorseListContainer.ParentRaceDayInfo.DataToShow.GetHorseDataToShowAttributes();
                        CMColumnsToShow.Items.Clear();
                        CMColumnsToShow.DataContext = HorseListContainer.ParentRaceDayInfo.DataToShow;
                        foreach (HorseDataToShowAttribute hda in attributeList)
                        {
                            MenuItem mi = new MenuItem()
                            {
                                IsCheckable = true,
                                Header = hda.Name,
                                IsEnabled = true    //hda.RequiresPro ? HPTConfig.Config.IsPayingCustomer : true
                            };
                            mi.SetBinding(MenuItem.IsCheckedProperty, hda.PropertyName);
                            CMColumnsToShow.Items.Add(mi);
                        }
                    }

                    // Skapa en lista med alla kolumner i listvyn
                    if (ColumnHandlerList == null || ColumnHandlerList.Count == 0)
                    {
                        CreateColumnHandlerList();

                        List<string> propertyNamesList = ColumnHandlerList.Select(ch => ch.BindingField).ToList();

                        foreach (string propertyName in propertyNamesList)
                        {
                            // Ta bort de kolumner man inte vill visa
                            bool show = (bool)HorseListContainer.ParentRaceDayInfo
                                .DataToShow.GetType().GetProperty(propertyName)
                                .GetValue(HorseListContainer.ParentRaceDayInfo.DataToShow, null);

                            HandleColumn(propertyName, show);
                        }
                        SortColumns();
                    }

                    // Lägg till eventhandler för om kolumner ändrar ordning eller tas bort
                    gvwVxxSpel.Columns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                    HPTConfig.Config.PropertyChanged += new PropertyChangedEventHandler(Config_PropertyChanged);
                    HorseListContainer.ParentRaceDayInfo.DataToShow.PropertyChanged += new PropertyChangedEventHandler(DataToShow_PropertyChanged);
                }
            }
        }

        private void HandleColumn(string showText, bool show)
        {
            List<ColumnHandler> columnsToHandle =
                ColumnHandlerList.Where(ch => ch.BindingField == showText).ToList();
            foreach (var columnHandler in columnsToHandle)
            {
                GridViewColumn gvc = columnHandler.Column;
                if (show && !gvwVxxSpel.Columns.Contains(gvc))
                {
                    gvwVxxSpel.Columns.Add(gvc);
                }
                else if (!show && gvwVxxSpel.Columns.Contains(gvc))
                {
                    gvwVxxSpel.Columns.Remove(gvc);
                }
            }
        }

        private List<ColumnHandler> ColumnHandlerList;
        private void CreateColumnHandlerList()
        {
            ColumnHandlerList = new List<ColumnHandler>();

            // Rättning och Utgångar
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcComplimentaryRuleSelect, Name = "gvcComplimentaryRuleSelect", Position = 0, BindingField = "ShowComplimentaryRuleSelect" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcLegNr, Name = "gvcLegNr", Position = 0, BindingField = "ShowLegNrText" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcSystemsLeft, Name = "gvcSystemsLeft", Position = 0, BindingField = "ShowSystemsLeft" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcSystemValue, Name = "gvcSystemValue", Position = 0, BindingField = "ShowSystemValue" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcATGResultLink, Name = "gvcATGResultLink", Position = 0, BindingField = "ShowATGResultLink" });

            // Allmän hästinformation
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStartNr, Name = "gvcStartNr", Position = 0, BindingField = "ShowStartNr" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcABCD, Name = "gvcABCD", Position = 0, BindingField = "ShowPrio" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcNamn, Name = "gvcNamn", Position = 0, BindingField = "ShowName" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAge, Name = "gvcAge", Position = 0, BindingField = "ShowAge" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcSex, Name = "gvcSex", Position = 0, BindingField = "ShowSex" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTrack, Name = "gvcTrack", Position = 0, BindingField = "ShowTrack" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcDriver, Name = "gvcDriver", Position = 0, BindingField = "ShowDriver" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTrainer, Name = "gvcTrainer", Position = 0, BindingField = "ShowTrainer" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcBreeder, Name = "gvcBreeder", Position = 0, BindingField = "ShowBreeder" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcOwner, Name = "gvcOwner", Position = 0, BindingField = "ShowOwner" });

            // Trio
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTrio1, Name = "gvcTrio1", Position = 0, BindingField = "ShowTrio" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTrio2, Name = "gvcTrio2", Position = 0, BindingField = "ShowTrio" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTrio3, Name = "gvcTrio3", Position = 0, BindingField = "ShowTrio" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTrioIndex, Name = "gvcTrioIndex", Position = 0, BindingField = "ShowTrio" });

            // Spelinfo
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStakeDistributionPercent, Name = "gvcStakeDistributionPercent", Position = 0, BindingField = "ShowStakeDistributionPercent" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRelativeDifference, Name = "gvcRelativeDifference", Position = 0, BindingField = "ShowRelativeDifference" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStakeDistributionShareAccumulated, Name = "gvcStakeDistributionShareAccumulated", Position = 0, BindingField = "ShowStakeDistributionShareAccumulated" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcMarkability, Name = "gvcMarkability", Position = 0, BindingField = "ShowMarkability" });

            // Rank
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRankMean, Name = "gvcRankMean", Position = 0, BindingField = "ShowRankMean" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRankTip, Name = "gvcRankTip", Position = 0, BindingField = "ShowRankTip" });

            // Egna inställningar
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcLocked, Name = "gvcLocked", Position = 0, BindingField = "ShowLocked" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcNextTimer, Name = "gvcNextTimer", Position = 0, BindingField = "ShowOwnInformation" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcReserv, Name = "gvcReserv", Position = 0, BindingField = "ShowReserv" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRankOwn, Name = "gvcRankOwn", Position = 0, BindingField = "ShowRankOwn" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRankAlternate, Name = "gvcRankAlternate", Position = 0, BindingField = "ShowRankAlternate" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcOwnProbability, Name = "gvcOwnProbability", Position = 0, BindingField = "ShowOwnProbability" });

            // Historisk information
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcEarnings, Name = "gvcEarnings", Position = 0, BindingField = "ShowEarnings" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcEarningsMeanLast5, Name = "gvcEarningsMeanLast5", Position = 0, BindingField = "ShowEarningsMeanLast5" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcLastStartDate, Name = "gvcLastStartDate", Position = 0, BindingField = "ShowLastStartDate" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRecord, Name = "gvcRecord", Position = 0, BindingField = "ShowRecord" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcResultRow, Name = "gvcResultRow", Position = 0, BindingField = "ShowResultRow" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcHeadToHead, Name = "gvcHeadToHead", Position = 0, BindingField = "ShowHeadToHead" });

            // Information om aktuell start
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcDistance, Name = "gvcDistance", Position = 0, BindingField = "ShowDistance" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcShoeInfo, Name = "gvcShoeInfo", Position = 0, BindingField = "ShowShoeInfo" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcSulkyInfo, Name = "gvcSulkyInfo", Position = 0, BindingField = "ShowSulkyInfo" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRankATG, Name = "gvcRankATG", Position = 0, BindingField = "ShowRankATG" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcShape, Name = "gvcShape", Position = 0, BindingField = "ShowShape" });

            // Systeminformation
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcSystemCoverage, Name = "gvcSystemCoverage", Position = 0, BindingField = "ShowSystemCoverage" });

            // Vinnarspel
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcVinnarOdds, Name = "gvcVinnarOdds", Position = 0, BindingField = "ShowVinnarOdds" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRelativeDifferenceVinnare, Name = "gvcRelativeDifferenceVinnare", Position = 0, BindingField = "ShowRelativeDifferenceVinnare" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcVinnarOddsRelative, Name = "gvcVinnarOddsRelative", Position = 0, BindingField = "ShowVinnarOddsRelative" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcVinnarOddsShare, Name = "gvcVinnarOddsShare", Position = 0, BindingField = "ShowVinnarOddsShare" });

            // Platsodds
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcPlatsOdds, Name = "gvcPlatsOdds", Position = 0, BindingField = "ShowPlatsOdds" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcPlatsoddsShare, Name = "gvcPlatsoddsShare", Position = 0, BindingField = "ShowPlatsShare" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRelativeDifferencePlats, Name = "gvcRelativeDifferencePlats", Position = 0, BindingField = "ShowRelativeDifferencePlats" });

            // Alternativa insatsfördelningar
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStakeShareAlternate, Name = "gvcStakeShareAlternate", Position = 0, BindingField = "ShowStakeShare" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStakeShareAlternate2, Name = "gvcStakeShareAlternate2", Position = 0, BindingField = "ShowStakeShare" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcDoubleShare, Name = "gvcDoubleShare", Position = 0, BindingField = "ShowDoubleShare" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTrioShare, Name = "gvcTrioShare", Position = 0, BindingField = "ShowTrioShare" });
            ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTvillingShare, Name = "gvcTvillingShare", Position = 0, BindingField = "ShowTvillingShare" });

            gvwVxxSpel.Columns.Clear();
        }

        void DataToShow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var hdts = (HPTHorseDataToShow)sender;
            bool show = (bool)hdts.GetType().GetProperty(e.PropertyName).GetValue(hdts, null);
            HandleColumn(e.PropertyName, show);
        }

        #region Context menu handling



        public ContextMenu CMHorse
        {
            get { return (ContextMenu)GetValue(CMHorseProperty); }
            set { SetValue(CMHorseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMHorse.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMHorseProperty =
            DependencyProperty.Register("CMHorse", typeof(ContextMenu), typeof(UCRaceViewGrouped), new UIPropertyMetadata(null));



        public ContextMenu CMColumnsToShow
        {
            get { return (ContextMenu)GetValue(CMColumnsToShowProperty); }
            set { SetValue(CMColumnsToShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMColumnsToShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMColumnsToShowProperty =
            DependencyProperty.Register("CMColumnsToShow", typeof(ContextMenu), typeof(UCRaceViewGrouped), new UIPropertyMetadata(null));

        private void ClearContextMenu()
        {
            if (CMColumnsToShow == null)
            {
                return;
            }
            var separator = CMColumnsToShow.Items.OfType<Separator>().FirstOrDefault();
            if (separator != null)
            {
                while (CMColumnsToShow.Items[0] != separator)
                {
                    CMColumnsToShow.Items.RemoveAt(0);
                }
                CMColumnsToShow.Items.Remove(separator);
            }
        }

        private void AddContextMenuItem(MenuItem itemToAdd)
        {
            //ClearContextMenu();
            CMColumnsToShow.Items.Insert(0, new Separator());
            CMColumnsToShow.Items.Insert(0, itemToAdd);
        }

        private void AddContextMenuItems(IEnumerable<MenuItem> itemsToAdd)
        {
            ClearContextMenu();
            CMColumnsToShow.Items.Insert(0, new Separator());
            foreach (var item in itemsToAdd)
            {
                CMColumnsToShow.Items.Insert(0, item);
            }
        }

        private IEnumerable<HPTHorse> SelectSpikes(string rankVariableName, int numberOfSpikes)
        {
            MarkBet.Clear(false, true, false);
            if (rankVariableName == "RankWeighted")
            {
                List<HPTHorse> horseListBestRankWeighted = new List<HPTHorse>();
                foreach (var race in MarkBet.RaceDayInfo.RaceList)
                {
                    var horse = race.HorseList.OrderBy(h => h.RankWeighted).First();
                    if (horse.ParentRace.NumberOfSelectedHorses == 0)
                    {
                        horseListBestRankWeighted.Add(horse);
                    }
                }
                return horseListBestRankWeighted;
            }
            if (rankVariableName == "RankMean")
            {
                List<HPTHorse> horseListBestRankMean = new List<HPTHorse>();
                foreach (var race in MarkBet.RaceDayInfo.RaceList)
                {
                    //var horse = race.HorseList.OrderBy(h => h.RankMean).First();
                    var horse = race.HorseList.OrderBy(h => h.RankWeighted).First();
                    if (horse.ParentRace.NumberOfSelectedHorses == 0)
                    {
                        horseListBestRankMean.Add(horse);
                    }
                }
                return horseListBestRankMean;
            }

            // Vanlig rankvariabel
            return MarkBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.ParentRace.NumberOfSelectedHorses == 0)
                .Where(h => h.RankList.FirstOrDefault(hr => hr.Name == rankVariableName && hr.Rank == 1) != null);
        }

        private void SetSpikes(IEnumerable<HPTHorse> spikesHorseList)
        {
            foreach (var horse in spikesHorseList)
            {
                horse.ParentRace.SelectAll(false);
                horse.Selected = true;
                if (horse.Prio != HPTPrio.M)
                {
                    horse.HorseXReductionList.First(hx => hx.Prio == horse.Prio).Selected = false;
                }
                horse.ParentRace.Locked = true;
                horse.Locked = true;
            }
        }

        //private void gvc_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    ClearContextMenu();
        //}

        #endregion

        #region Välj spikar och ABC-hästar

        private MenuItem abcSelectAndSetMenuItem;
        private MenuItem abcSetMenuItem;
        private MenuItem systemMenuItem;
        private MenuItem spikeMenuItem;
        private MenuItem rankTemplateMenuItem;
        private void gvcSpikes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            #region kontroller

            //if (e.ChangedButton != MouseButton.Right || !HPTConfig.Config.IsPayingCustomer)
            if (e.ChangedButton != MouseButton.Right)
            {
                return;
            }

            GridViewColumnHeader column = sender as GridViewColumnHeader;
            if (column == null)
            {
                return;
            }

            String field = column.Tag as String;
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            // Specialhantering för snittrank
            if (field == "RankMean" || field == "RankWeighted")
            {
                if (rankTemplateMenuItem == null)
                {
                    if (HPTConfig.Config.RankTemplateList == null)
                    {
                        HPTConfig.Config.RankTemplateList = new System.Collections.ObjectModel.ObservableCollection<HPTRankTemplate>();
                    }
                    rankTemplateMenuItem = new MenuItem()
                    {
                        Header = "Rankvariabelmall",
                        ItemsSource = HPTConfig.Config.RankTemplateList,
                        DisplayMemberPath = "Name"
                    };
                    rankTemplateMenuItem.Click += new RoutedEventHandler(rankTemplateMenuItem_Click);
                }
                AddContextMenuItems(new List<MenuItem>() { rankTemplateMenuItem });

                //// Undvik att eventet skickas vidare
                //e.Handled = true;
                //return;
            }

            //if (this.HorseListContainer.ParentRaceDayInfo.DataToShow.Usage != DataToShowUsage.Vxx)
            //{
            //    // Specialhantering för snittrank
            //    if (field == "RankMean" || field == "RankWeighted")
            //    {
            //        if (rankTemplateMenuItem == null)
            //        {
            //            if (HPTConfig.Config.RankTemplateList == null)
            //            {
            //                HPTConfig.Config.RankTemplateList = new System.Collections.ObjectModel.ObservableCollection<HPTRankTemplate>();
            //            }
            //            rankTemplateMenuItem = new MenuItem()
            //            {
            //                Header = "Rankvariabelmall",
            //                ItemsSource = HPTConfig.Config.RankTemplateList,
            //                DisplayMemberPath = "Name"
            //            };
            //            rankTemplateMenuItem.Click += new RoutedEventHandler(rankTemplateMenuItem_Click);
            //        }
            //        AddContextMenuItems(new List<MenuItem>() { rankTemplateMenuItem });

            //        // Undvik att eventet skickas vidare
            //        e.Handled = true;
            //    }
            //    return;
            //}

            #endregion

            var dataToShowUsages = DataToShowUsage.ComplementaryRule | DataToShowUsage.Correction | DataToShowUsage.Vxx;
            if (dataToShowUsages.HasFlag(HorseListContainer.ParentRaceDayInfo.DataToShow.Usage))
            {
                // Välj spikar automatiskt
                if (spikeMenuItem == null)
                {
                    spikeMenuItem = new MenuItem()
                    {
                        Header = "Spikar",
                        Tag = field
                    };
                    for (int i = 1; i < 5; i++)
                    {
                        MenuItem miSelectSpikes = new MenuItem()
                        {
                            Header = i.ToString() + " spik" + (i == 1 ? string.Empty : "ar"),
                            Tag = field
                        };
                        spikeMenuItem.Items.Add(miSelectSpikes);
                        miSelectSpikes.Click += new RoutedEventHandler(miSelectSpikes_Click);
                    }
                }
                else
                {
                    foreach (MenuItem menuItem in spikeMenuItem.Items)
                    {
                        menuItem.Tag = field;
                    }
                }

                // Välj ett ABC-system automatiskt
                if (abcSelectAndSetMenuItem == null)
                {
                    abcSelectAndSetMenuItem = new MenuItem()
                    {
                        Header = "Ramsystem med ABC",
                        Tag = field
                    };

                    foreach (var numberOfRows in HPTConfig.Config.SystemSizesToShow)
                    {
                        MenuItem miABC = new MenuItem()
                        {
                            Header = numberOfRows.ToString() + " rader",
                            Tag = numberOfRows
                        };
                        abcSelectAndSetMenuItem.Items.Add(miABC);
                        miABC.Click += new RoutedEventHandler(miABC_Click);
                    }
                }
                else
                {
                    abcSelectAndSetMenuItem.Tag = field;
                }

                // Välj ett ramsystem automatiskt
                if (systemMenuItem == null)
                {
                    systemMenuItem = new MenuItem()
                    {
                        Header = "Ramsystem",
                        Tag = field
                    };
                    foreach (var numberOfRows in HPTConfig.Config.SystemSizesToShow)
                    {
                        MenuItem miSystem = new MenuItem()
                        {
                            Header = numberOfRows.ToString() + " rader",
                            Tag = numberOfRows
                        };
                        systemMenuItem.Items.Add(miSystem);
                        miSystem.Click += new RoutedEventHandler(miSystem_Click);
                    }
                }
                else
                {
                    systemMenuItem.Tag = field;
                }

                // Sätt ABC på valda hästar och reducera automatiskt
                if (abcSetMenuItem == null)
                {
                    abcSetMenuItem = new MenuItem()
                    {
                        Header = "Sätt ABC och reducera",
                        Tag = field
                    };

                    var reductionPercentagesArray = new int[] { 55, 60, 65, 70, 75, 80, 85 };
                    foreach (var reductionPercentage in reductionPercentagesArray)
                    {
                        MenuItem miSetABC = new MenuItem()
                        {
                            Header = reductionPercentage.ToString() + "%",
                            Tag = reductionPercentage
                        };
                        abcSetMenuItem.Items.Add(miSetABC);
                        miSetABC.Click += new RoutedEventHandler(miSetABC_Click);
                    }
                }
                else
                {
                    abcSetMenuItem.Tag = field;
                }
            }

            // Specialhantering för snittrank
            if (field == "RankMean" || field == "RankWeighted")
            {
                if (rankTemplateMenuItem == null)
                {
                    rankTemplateMenuItem = new MenuItem()
                    {
                        Header = "Rankvariabelmall",
                        ItemsSource = HPTConfig.Config.RankTemplateList,
                        DisplayMemberPath = "Name"
                    };
                    rankTemplateMenuItem.Click += new RoutedEventHandler(rankTemplateMenuItem_Click);
                }
                if (abcSetMenuItem == null)
                {
                    AddContextMenuItems(new List<MenuItem>() { rankTemplateMenuItem });
                }
                else
                {
                    AddContextMenuItems(new List<MenuItem>() { abcSetMenuItem, systemMenuItem, abcSelectAndSetMenuItem, spikeMenuItem, rankTemplateMenuItem });
                }
            }
            else
            {
                AddContextMenuItems(new List<MenuItem>() { abcSetMenuItem, systemMenuItem, abcSelectAndSetMenuItem, spikeMenuItem });
            }

            // Undvik att eventet skickas vidare
            e.Handled = true;
        }

        void rankTemplateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)e.OriginalSource;
            var rankTemplate = (HPTRankTemplate)mi.DataContext;
            horseListContainer.ParentRaceDayInfo.SetRankTemplateChanged(rankTemplate);
        }

        IEnumerable<HPTHorse> SelectHorses(string field, int systemSize)
        {
            IEnumerable<HPTHorse> horseList = MarkBet.RaceDayInfo.RaceList
                .Where(r => !r.Locked)
                .SelectMany(r => r.HorseList)
                .Where(h => h.Scratched == false || h.Scratched == null);

            foreach (var horse in horseList)
            {
                horse.Selected = horse.Locked == true;
            }

            IOrderedEnumerable<HPTHorse> orderedHorseList = null;

            switch (field)
            {
                //case "MarksQuantity":
                //    orderedHorseList = horseList.OrderByDescending(h => h.MarksShare);
                //    break;
                case "StakeDistributionShare":
                    orderedHorseList = horseList.OrderByDescending(h => h.StakeDistributionShare);
                    break;
                case "Markability":
                    orderedHorseList = horseList.OrderByDescending(h => h.Markability);
                    break;
                case "VinnarOdds":
                    orderedHorseList = horseList.OrderBy(h => h.VinnarOdds);
                    break;
                case "TvillingShare":
                    orderedHorseList = horseList.OrderByDescending(h => h.TvillingShare);
                    break;
                //case "RankMean":
                //    orderedHorseList = horseList.OrderBy(h => h.RankMean);
                //    break;
                case "RankMean":
                case "RankWeighted":
                    orderedHorseList = horseList.OrderBy(h => h.RankWeighted);
                    break;
                case "VinnarOddsRelative":
                    orderedHorseList = horseList.OrderBy(h => h.VinnarOddsRelative);
                    break;
                default:
                    return null;
            }

            try
            {
                MarkBet.Clear(false, false, true);
                HPTHorse[] orderedHorseArray = orderedHorseList.ToArray();
                int rankPosition = 0;
                while (MarkBet.SystemSize < systemSize && rankPosition < orderedHorseArray.Count())
                {
                    HPTHorse horse = orderedHorseArray[rankPosition];
                    horse.Selected = true;
                    rankPosition++;
                }
                if (MarkBet.SystemSize == 0)
                {
                    MarkBet.UpdateSystemSize();
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            //this.MarkBet.pauseRecalculation = false;
            return orderedHorseList.Where(h => h.Selected);
        }

        void SelectABC(IEnumerable<HPTHorse> orderedHorseList)
        {
            try
            {
                IEnumerable<HPTRace> raceList = MarkBet.RaceDayInfo.RaceList.Where(r => !r.Locked);
                foreach (var race in raceList)
                {
                    orderedHorseList.First(h => h.ParentRace == race).HorseXReductionList.First(x => x.Prio == HPTPrio.A).Selected = true;
                }

                int numberOfHorsesToTake = (orderedHorseList.Count() - raceList.Count()) / 2;
                IEnumerable<HPTHorse> horseListB = orderedHorseList.Where(h => h.Prio != HPTPrio.A && !h.ParentRace.Locked).Take(numberOfHorsesToTake);
                foreach (var horse in horseListB)
                {
                    horse.HorseXReductionList.First(x => x.Prio == HPTPrio.B).Selected = true;
                }

                IEnumerable<HPTHorse> horseListC = orderedHorseList.Where(h => h.Prio != HPTPrio.B && h.Prio != HPTPrio.A && !h.ParentRace.Locked);
                foreach (var horse in horseListC)
                {
                    horse.HorseXReductionList.First(x => x.Prio == HPTPrio.C).Selected = true;
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        void miSystem_Click(object sender, RoutedEventArgs e)
        {
            bool recalculationPaused = MarkBet.pauseRecalculation;
            MarkBet.pauseRecalculation = true;

            var item = (MenuItem)sender;
            var field = (string)((MenuItem)item.Parent).Tag;
            var systemSize = Convert.ToInt32(item.Tag);

            IEnumerable<HPTHorse> orderedHorseList = SelectHorses(field, systemSize);
            MarkBet.pauseRecalculation = recalculationPaused;
            MarkBet.RecalculateReduction(RecalculateReason.Other);
        }

        void miABC_Click(object sender, RoutedEventArgs e)
        {
            MarkBet.pauseRecalculation = true;

            var item = (MenuItem)sender;
            var field = (string)((MenuItem)item.Parent).Tag;
            var systemSize = Convert.ToInt32(item.Tag);

            IEnumerable<HPTHorse> horseList = SelectHorses(field, systemSize);
            if (horseList != null)
            {
                SelectABC(horseList);
            }
            MarkBet.RecalculateNumberOfX();
            MarkBet.pauseRecalculation = false;
            MarkBet.RecalculateReduction(RecalculateReason.XReduction);
        }

        void miSetABC_Click(object sender, RoutedEventArgs e)
        {
            if (MarkBet.SystemSize == 0)
            {
                return;
            }

            bool recalculationPaused = MarkBet.pauseRecalculation;
            MarkBet.pauseRecalculation = true;

            var item = (MenuItem)sender;
            var field = (string)((MenuItem)item.Parent).Tag;
            var reductionPercentage = Convert.ToInt32(item.Tag);

            var racesToSetABC = MarkBet.RaceDayInfo.RaceList.Where(r => !r.Locked && r.NumberOfSelectedHorses == 1);
            foreach (var race in racesToSetABC)
            {
                race.Locked = true;
            }
            var horseList = MarkBet.RaceDayInfo.HorseListSelected.Where(h => !h.ParentRace.Locked);
            IOrderedEnumerable<HPTHorse> orderedHorseList = null;
            switch (field)
            {
                //case "MarksQuantity":
                //    orderedHorseList = horseList.OrderByDescending(h => h.MarksShare);
                //    break;
                case "StakeDistributionShare":
                    orderedHorseList = horseList.OrderByDescending(h => h.StakeDistributionShare);
                    break;
                case "Markability":
                    orderedHorseList = horseList.OrderByDescending(h => h.Markability);
                    break;
                case "VinnarOdds":
                    orderedHorseList = horseList.OrderBy(h => h.VinnarOdds);
                    break;
                case "TvillingShare":
                    orderedHorseList = horseList.OrderByDescending(h => h.TvillingShare);
                    break;
                case "RankMean":
                    orderedHorseList = horseList.OrderBy(h => h.RankMean);
                    break;
                case "RankWeighted":
                    orderedHorseList = horseList.OrderBy(h => h.RankWeighted);
                    break;
                case "VinnarOddsRelative":
                    orderedHorseList = horseList.OrderBy(h => h.VinnarOddsRelative);
                    break;
                default:
                    break;
            }

            if (horseList != null)
            {
                SelectABC(orderedHorseList);
            }

            MarkBet.MarkBetTemplateABCD = new HPTMarkBetTemplateABCD()
            {
                DesiredSystemSize = MarkBet.SystemSize * (100 - reductionPercentage) / 100,
                ReductionPercentage = reductionPercentage,
                Use = true
            };
            MarkBet.MarkBetTemplateABCD.InitializeTemplate(new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C, });
            MarkBet.CreateSystemsFromTemplateABCD();
            MarkBet.pauseRecalculation = recalculationPaused;
        }

        void miSelectSpikes_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var field = (string)item.Tag;
            var header = (string)item.Header;
            var numberOfSpikes = Convert.ToInt32(header.First().ToString());

            IEnumerable<HPTHorse> rankedFirstHorseList = SelectSpikes(field, numberOfSpikes);
            IEnumerable<HPTHorse> orderedHorseList = null;

            #region Sortering
            switch (field)
            {
                //case "MarksQuantity":
                //    orderedHorseList = rankedFirstHorseList
                //        .OrderByDescending(h => h.MarksQuantity)
                //        .Take(numberOfSpikes);
                //    break;
                case "StakeDistributionShare":
                    orderedHorseList = rankedFirstHorseList
                        .OrderByDescending(h => h.StakeDistributionShare)
                        .Take(numberOfSpikes);
                    break;
                case "Markability":
                    orderedHorseList = rankedFirstHorseList
                        .OrderByDescending(h => h.Markability)
                        .Take(numberOfSpikes);
                    break;
                case "VinnarOdds":
                    orderedHorseList = rankedFirstHorseList
                        .OrderBy(h => h.VinnarOdds)
                        .Take(numberOfSpikes);
                    break;
                case "TvillingShare":
                    orderedHorseList = rankedFirstHorseList
                        .OrderByDescending(h => h.TvillingShare)
                        .Take(numberOfSpikes);
                    break;
                //case "RankMean":
                //    orderedHorseList = rankedFirstHorseList
                //        .OrderBy(h => h.RankMean)
                //        .Take(numberOfSpikes);
                //    break;
                case "RankMean":
                case "RankWeighted":
                    orderedHorseList = rankedFirstHorseList
                        .OrderBy(h => h.RankWeighted)
                        .Take(numberOfSpikes);
                    break;
                case "VinnarOddsRelative":
                    orderedHorseList = rankedFirstHorseList
                        .OrderBy(h => h.VinnarOddsRelative)
                        .Take(numberOfSpikes);
                    break;
                default:
                    break;
            }
            #endregion

            SetSpikes(orderedHorseList);
        }

        #endregion

        #region Menyhantering reserver

        private List<MenuItem> reservMenuItems;
        private void gvcReservHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right
                || HorseListContainer.ParentRaceDayInfo.DataToShow.Usage != DataToShowUsage.Vxx)
            {
                //e.Handled = true;
                return;
            }
            if (reservMenuItems == null)
            {
                MenuItem miClearReservRace = new MenuItem()
                {
                    Header = "Rensa reserver",
                    IsEnabled = string.IsNullOrEmpty(GroupDescriptionName)
                };
                miClearReservRace.Click += new RoutedEventHandler(miClearReservRace_Click);

                MenuItem miClearReservAll = new MenuItem()
                {
                    Header = "Rensa alla reserver"
                };
                miClearReservAll.Click += new RoutedEventHandler(miClearReservAll_Click);

                MenuItem miSetReservRank = new MenuItem()
                {
                    Header = "Sätt reserver efter snittrank"
                };
                miSetReservRank.Click += new RoutedEventHandler(miSetReservRank_Click);
                reservMenuItems = new List<MenuItem>() { miSetReservRank, miClearReservAll, miClearReservRace };
            }
            AddContextMenuItems(reservMenuItems);
            e.Handled = true;
        }

        void miClearReservAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var race in HorseListContainer.ParentRaceDayInfo.RaceList)
            {
                ClearReservRace(race.HorseList);
            }
        }

        void miSetReservRank_Click(object sender, RoutedEventArgs e)
        {
            foreach (var race in HorseListContainer.ParentRaceDayInfo.RaceList)
            {
                List<HPTHorse> horsesNotSelected = race.HorseList
                    .Where(h => !h.Selected)
                    .OrderBy(h => h.RankWeighted)
                    .Take(2)
                    .ToList();

                if (horsesNotSelected.Count > 0)
                {
                    horsesNotSelected.First().Reserv1 = true;
                }
                if (horsesNotSelected.Count == 2)
                {
                    horsesNotSelected.Last().Reserv2 = true;
                }
            }
        }

        void miClearReservRace_Click(object sender, RoutedEventArgs e)
        {
            ClearReservRace(HorseListContainer.HorseList);
        }

        void ClearReservRace(IEnumerable<HPTHorse> horseList)
        {
            HPTHorse reserv1 = horseList.FirstOrDefault(h => h.Reserv1 == true);
            if (reserv1 != null)
            {
                reserv1.Reserv1 = null;
            }

            HPTHorse reserv2 = horseList.FirstOrDefault(h => h.Reserv2 == true);
            if (reserv2 != null)
            {
                reserv2.Reserv2 = null;
            }
        }

        #endregion

        #region Popup handling

        private System.Windows.Controls.Primitives.Popup pu;
        public System.Windows.Controls.Primitives.Popup PU
        {
            get
            {
                if (pu == null)
                {
                    pu = new System.Windows.Controls.Primitives.Popup()
                    {
                        Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                        HorizontalOffset = -10D,
                        VerticalOffset = -10D
                    };
                    pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
                }
                return pu;
            }
        }

        private void txtHorseName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Left && HPTConfig.Config.IsPayingCustomer)
            if (e.ChangedButton == MouseButton.Left)
            {
                TextBlock tb = (TextBlock)sender;
                PU.DataContext = tb.DataContext;
                PU.Child = new UCResultView();
                PU.IsOpen = true;
            }
        }

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup pu = (System.Windows.Controls.Primitives.Popup)sender;
            pu.Child = null;
            //pu.MouseLeave -= pu_MouseLeave;
            pu.IsOpen = false;
        }

        private void txtDriverName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left
                || HorseListContainer.ParentRaceDayInfo.DataToShow.Usage != DataToShowUsage.Vxx)
            {
                return;
            }
            TextBlock tb = (TextBlock)sender;
            HPTHorse horse = (HPTHorse)tb.DataContext;

            Border b = new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1D),
                Child = new UCRaceView()
                {
                    BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                    BorderThickness = new Thickness(6D)
                }
            };

            PU.DataContext = horse.Driver;
            PU.Child = b;
            PU.IsOpen = true;
        }

        private void txtTrainerName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left
                || HorseListContainer.ParentRaceDayInfo.DataToShow.Usage != DataToShowUsage.Vxx)
            {
                return;
            }
            TextBlock tb = (TextBlock)sender;
            HPTHorse horse = (HPTHorse)tb.DataContext;

            Border b = new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1D),
                Child = new UCRaceView()
                {
                    BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                    BorderThickness = new Thickness(6D)
                }
            };

            PU.DataContext = horse.Trainer;
            PU.Child = b;
            PU.IsOpen = true;
        }

        #endregion

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MarkBet != null && !MarkBet.pauseRecalculation)
            {
                if (MarkBet.HasOwnRankReduction())
                {
                    MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
                else
                {
                    MarkBet.RecalculateAllRanks();
                    MarkBet.RecalculateRank();
                }
            }
        }

        #region Menyhantering välj/ta bort alla

        private List<MenuItem> selectAllMenuItems;
        private void GridViewColumnHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right
                || HorseListContainer.ParentRaceDayInfo.DataToShow.Usage != DataToShowUsage.Vxx)
            {
                return;
            }
            if (selectAllMenuItems == null && DataContext != null && DataContext.GetType() == typeof(HPTRace))
            {
                MenuItem miSelectAll = new MenuItem()
                {
                    Header = "Välj alla"
                };
                miSelectAll.Click += new RoutedEventHandler(miSelectAll_Click);

                MenuItem miDeselectAll = new MenuItem()
                {
                    Header = "Ta bort alla"
                };
                miDeselectAll.Click += new RoutedEventHandler(miDeselectAll_Click);

                MenuItem miLockAllSelected = new MenuItem()
                {
                    Header = "Lås alla valda"
                };
                miLockAllSelected.Click += miLockAllSelected_Click;
                selectAllMenuItems = new List<MenuItem>() { miSelectAll, miDeselectAll, miLockAllSelected };
            }
            AddContextMenuItems(selectAllMenuItems);
            e.Handled = true;
        }

        void miLockAllSelected_Click(object sender, RoutedEventArgs e)
        {
            var race = (HPTRace)DataContext;
            foreach (var horse in race.ParentRaceDayInfo.HorseListSelected)
            {
                horse.Locked = true;
            }
        }

        private void miSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HPTRace race = (HPTRace)DataContext;
                race.SelectAll(true);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void miDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HPTRace race = (HPTRace)DataContext;
                race.SelectAll(false);
                //race.Locked = false;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #endregion

        private void btnNotes_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var btn = (Button)sender;

            Border b = new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1D),
                Child = new UCHorseCommentList()
                {
                    BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                    BorderThickness = new Thickness(6D)
                }
            };

            PU.DataContext = btn.DataContext;
            PU.Child = b;
            PU.IsOpen = true;
        }

        #region Menyhantering rensa ABCD

        private List<MenuItem> abcdMenuItems;
        private void gvcABCD_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right
                || HorseListContainer.ParentRaceDayInfo.DataToShow.Usage != DataToShowUsage.Vxx)
            {
                return;
            }
            if (abcdMenuItems == null)
            {
                MenuItem miClearABCD = new MenuItem()
                {
                    Header = "Rensa ABCD",
                    IsEnabled = string.IsNullOrEmpty(GroupDescriptionName)
                };
                miClearABCD.Click += MiClearAbcdOnClick;

                MenuItem miClearAllABCD = new MenuItem()
                {
                    Header = "Rensa alla ABCD"
                };
                miClearAllABCD.Click += MiClearAllAbcdOnClick;
                abcdMenuItems = new List<MenuItem>() { miClearABCD, miClearAllABCD };
            }
            AddContextMenuItems(abcdMenuItems);
            e.Handled = true;
        }

        private void MiClearAllAbcdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            HorseListContainer.ParentRaceDayInfo.ClearABCDRaceDayInfo();
        }

        private void MiClearAbcdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            HPTRace race = (HPTRace)DataContext;
            race.ClearABCDRace();
        }

        #endregion

        private void lvwLopp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right)
            {
                return;
            }
            ClearContextMenu();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            TextBlock tb = (TextBlock)sender;

            PU.DataContext = tb.DataContext;
            PU.Child = new UCHeadToHeadRace();
            PU.IsOpen = true;
        }

        private List<MenuItem> nextTimerMenuItems;
        private void gvcNextTimer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right)
            {
                return;
            }
            if (nextTimerMenuItems == null)
            {
                MenuItem miSelectNextTimer = new MenuItem()
                {
                    Header = "Välj"
                };
                miSelectNextTimer.Click += MiSelectNextTimerOnClick;

                MenuItem miSelectNextTimerAsSpikes = new MenuItem()
                {
                    Header = "Välj som lås"
                };
                miSelectNextTimerAsSpikes.Click += new RoutedEventHandler(miSelectNextTimerAsSpikes_Click);

                MenuItem miSelectNextTimerAsA = new MenuItem()
                {
                    Header = "Välj som A-Hästar"
                };
                miSelectNextTimerAsA.Click += MiSelectNextTimerAsAOnClick;
                nextTimerMenuItems = new List<MenuItem>() { miSelectNextTimer, miSelectNextTimerAsSpikes, miSelectNextTimerAsA };
            }
            // Undvik att eventet skickas vidare
            e.Handled = true;
            AddContextMenuItems(nextTimerMenuItems);
        }

        private void MiSelectNextTimerAsAOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            IEnumerable<HPTHorse> nextTimerHorseList = MarkBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.OwnInformation != null && h.OwnInformation.NextTimer == true);

            bool recalculationPaused = MarkBet.pauseRecalculation;
            MarkBet.pauseRecalculation = true;
            foreach (var horse in nextTimerHorseList)
            {
                horse.HorseXReductionList.First(r => r.Prio == HPTPrio.A).Selected = true;
                horse.ParentRace.Locked = false;
                horse.Locked = true;
            }
            MarkBet.pauseRecalculation = recalculationPaused;
            MarkBet.RecalculateReduction(RecalculateReason.All);
        }

        void miSelectNextTimerAsSpikes_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<HPTHorse> nextTimerHorseList = MarkBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.OwnInformation != null && h.OwnInformation.NextTimer == true);

            MarkBet.pauseRecalculation = true;
            foreach (var horse in nextTimerHorseList)
            {
                horse.Selected = true;
                horse.ParentRace.Locked = true;
                horse.Locked = true;
            }
            MarkBet.pauseRecalculation = false;
            MarkBet.RecalculateReduction(RecalculateReason.All);
        }

        private void MiSelectNextTimerOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            IEnumerable<HPTHorse> nextTimerHorseList = MarkBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.OwnInformation != null && h.OwnInformation.NextTimer == true);

            bool recalculationPaused = MarkBet.pauseRecalculation;
            MarkBet.pauseRecalculation = true;
            foreach (var horse in nextTimerHorseList)
            {
                horse.Selected = true;
                horse.ParentRace.Locked = false;
                horse.Locked = true;
            }
            MarkBet.pauseRecalculation = recalculationPaused;
            MarkBet.RecalculateReduction(RecalculateReason.All);
        }

        private void txtLastStartDate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton != MouseButton.Left)
            //{
            //    return;
            //}
            //TextBlock tb = (TextBlock)sender;
            //if (this.pu == null)
            //{
            //    this.pu = new System.Windows.Controls.Primitives.Popup()
            //    {
            //        Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
            //        HorizontalOffset = -10D,
            //        VerticalOffset = -10D
            //    };
            //    this.pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
            //}
            //this.pu.DataContext = tb.DataContext;
            ////this.pu.Child = new UCResultView();
            //this.pu.IsOpen = true;
        }

        #region Kontextmeny på häst

        private void txtHorseName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (!HPTConfig.Config.IsPayingCustomer)
            //{
            //    return;
            //}

            if (CMHorse == null)
            {
                CMHorse = new ContextMenu();
                //switch (this.MarkBet.BetType.Code)
                switch (BetType.Code)
                {
                    case "V64":
                    case "V65":
                    case "V75":
                    case "GS75":
                    case "V86":
                        CreateHorseContextMenuVxx();
                        break;
                    case "V4":
                    case "V5":
                        CMHorse = null;
                        //CreateHorseContextMenuVx();
                        break;
                    case "TV":
                        //CreateHorseContextMenuDouble();
                        CreateHorseContextMenuTvilling();
                        break;
                    case "DD":
                    case "LD":
                        CreateHorseContextMenuDouble();
                        break;
                    case "T":
                        CMHorse = null;
                        //CreateHorseContextMenuTrio();
                        break;
                    default:
                        break;
                }
            }
        }

        private void CreateHorseContextMenuVxx()
        {
            // Spela V6/V7/V8
            var miV6 = new MenuItem()
            {
                //Header = "Spela V6/V7/V8",
                Header = "Spela V6/V7",
                Tag = "V6"
            };
            miV6.Click += new RoutedEventHandler(miV6Betmultiplier_Click);

            // Flerbong
            var miBetMultiplierHeader = new MenuItem()
            {
                Header = "Spela flerbong",
                Tag = "FB"
            };

            // V6/V7/V8 OCH flerbong
            var miV6AndBetMultiplierHeader = new MenuItem()
            {
                //Header = "Spela V6/V7/V8 och flerbong",
                Header = "Spela V6/V7 och flerbong",
                Tag = "V6"
            };

            // Skapa enskilda val
            foreach (var bm in MarkBet.BetType.BetMultiplierList)
            {
                MenuItem miBetMultiplier = new MenuItem()
                {
                    Header = bm.ToString(),
                    Tag = bm.ToString()
                };
                miBetMultiplier.Click += new RoutedEventHandler(miV6Betmultiplier_Click);
                miBetMultiplierHeader.Items.Add(miBetMultiplier);

                MenuItem miV6BetMultiplier = new MenuItem()
                {
                    Header = bm.ToString(),
                    Tag = "V6-" + bm.ToString()
                };
                miV6BetMultiplier.Click += new RoutedEventHandler(miV6Betmultiplier_Click);
                miV6AndBetMultiplierHeader.Items.Add(miV6BetMultiplier);
            }

            // Lägg till i kontextmenyn
            CMHorse.Items.Add(miV6);
            CMHorse.Items.Add(miV6AndBetMultiplierHeader);
            CMHorse.Items.Add(miBetMultiplierHeader);
        }

        private void CreateHorseContextMenuVx()
        {
        }

        private void CreateHorseContextMenuDouble()
        {
            // Välj alla kombinationer hästen är med på
            var miShowOnlyThisHorsesCombinations = new MenuItem()
            {
                Header = "Visa bara kombinationer med vald häst",
                Tag = ""
            };
            miShowOnlyThisHorsesCombinations.Click += new RoutedEventHandler(miShowOnlyThisHorsesCombinations_Click);
            CMHorse.Items.Add(miShowOnlyThisHorsesCombinations);

            // Välj alla kombinationer hästen är med på
            var miSelectAllDoubleCombinations = new MenuItem()
            {
                Header = "Välj alla kombinationer",
                Tag = ""
            };
            miSelectAllDoubleCombinations.Click += new RoutedEventHandler(miSelectAllDoubleCombinations_Click);
            CMHorse.Items.Add(miSelectAllDoubleCombinations);

            // Välj alla kombinationer med bra spelbarhet
            var miSelectDoubleCombinationsWithGoodPlayability = new MenuItem()
            {
                Header = "Välj med bra spelbarhet",
                Tag = ""
            };
            miSelectDoubleCombinationsWithGoodPlayability.Click += new RoutedEventHandler(miSelectDoubleCombinationsWithGoodPlayability_Click);
            CMHorse.Items.Add(miSelectDoubleCombinationsWithGoodPlayability);

            // Välj alla kombinationer med bra oddskvot
            var miSelectDoubleCombinationsWithGoodQuota = new MenuItem()
            {
                Header = "Välj med bra oddskvot",
                Tag = ""
            };
            miSelectDoubleCombinationsWithGoodQuota.Click += new RoutedEventHandler(miSelectDoubleCombinationsWithGoodQuota_Click);
            CMHorse.Items.Add(miSelectDoubleCombinationsWithGoodQuota);
        }

        void miShowOnlyThisHorsesCombinations_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var selectedHorse = (HPTHorse)mi.DataContext;
            foreach (var horse in selectedHorse.ParentRace.HorseList.Except(new HPTHorse[] { selectedHorse }).ToList())
            {
                horse.Selected = false;
            }
        }

        void miSelectDoubleCombinationsWithGoodQuota_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var horse = (HPTHorse)fe.DataContext;
            if (!horse.Selected)
            {
                horse.Selected = true;
            }
            List<HPTCombination> combinationList = GetDoubleCombinationsForHorse(horse);

            foreach (var comb in combinationList.Where(c => c.OddsQuota > 1.2M))
            {
                comb.Selected = true;
            }
        }

        void miSelectDoubleCombinationsWithGoodPlayability_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var horse = (HPTHorse)fe.DataContext;
            if (!horse.Selected)
            {
                horse.Selected = true;
            }
            List<HPTCombination> combinationList = GetDoubleCombinationsForHorse(horse);

            foreach (var comb in combinationList.Where(c => c.Playability < 1M))
            {
                comb.Selected = true;
            }
        }

        void miSelectAllDoubleCombinations_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var horse = (HPTHorse)fe.DataContext;
            if (!horse.Selected)
            {
                horse.Selected = true;
            }
            List<HPTCombination> combinationList = GetDoubleCombinationsForHorse(horse);

            foreach (var comb in combinationList)
            {
                comb.Selected = true;
            }
        }

        private List<HPTCombination> GetDoubleCombinationsForHorse(HPTHorse horse)
        {
            return horse.ParentRace.ParentRaceDayInfo.CombinationListInfoDouble.CombinationsToShowList
                .Where(c => c.Horse1 == horse || c.Horse2 == horse).ToList();
        }

        private void CreateHorseContextMenuTvilling()
        {
            // Välj alla kombinationer hästen är med på
            var miShowOnlyThisHorsesCombinationsTvilling = new MenuItem()
            {
                Header = "Visa bara kombinationer med vald häst",
                Tag = ""
            };
            miShowOnlyThisHorsesCombinationsTvilling.Click += new RoutedEventHandler(miShowOnlyThisHorsesCombinationsTvilling_Click);
            CMHorse.Items.Add(miShowOnlyThisHorsesCombinationsTvilling);

            // Välj alla kombinationer hästen är med på
            var miSelectAllTvillingCombinations = new MenuItem()
            {
                Header = "Välj alla kombinationer",
                Tag = ""
            };
            miSelectAllTvillingCombinations.Click += new RoutedEventHandler(miSelectAllTvillingCombinations_Click);
            CMHorse.Items.Add(miSelectAllTvillingCombinations);

            // Välj alla kombinationer med bra spelbarhet
            var miSelectTvillingCombinationsWithGoodPlayability = new MenuItem()
            {
                Header = "Välj med bra spelbarhet",
                Tag = ""
            };
            miSelectTvillingCombinationsWithGoodPlayability.Click += new RoutedEventHandler(miSelectTvillingCombinationsWithGoodPlayability_Click);
            CMHorse.Items.Add(miSelectTvillingCombinationsWithGoodPlayability);

            // Välj alla kombinationer med bra oddskvot
            var miSelectTvillingCombinationsWithGoodQuota = new MenuItem()
            {
                Header = "Välj med bra oddskvot",
                Tag = ""
            };
            miSelectTvillingCombinationsWithGoodQuota.Click += new RoutedEventHandler(miSelectTvillingCombinationsWithGoodQuota_Click);
            CMHorse.Items.Add(miSelectTvillingCombinationsWithGoodQuota);
        }

        private List<HPTCombination> GetTvillingCombinationsForHorse(HPTHorse horse)
        {
            return horse.ParentRace.CombinationListInfoTvilling.CombinationsToShowList
                .Where(c => c.Horse1 == horse || c.Horse2 == horse).ToList();
        }

        void miSelectTvillingCombinationsWithGoodQuota_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var horse = (HPTHorse)fe.DataContext;
            if (!horse.Selected)
            {
                horse.Selected = true;
            }
            List<HPTCombination> combinationList = GetTvillingCombinationsForHorse(horse);

            foreach (var comb in combinationList.Where(c => c.OddsQuota > 1.2M))
            {
                comb.Selected = true;
            }
        }

        void miSelectTvillingCombinationsWithGoodPlayability_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var horse = (HPTHorse)fe.DataContext;
            if (!horse.Selected)
            {
                horse.Selected = true;
            }
            List<HPTCombination> combinationList = GetTvillingCombinationsForHorse(horse);

            foreach (var comb in combinationList.Where(c => c.Playability < 1M))
            {
                comb.Selected = true;
            }
        }

        void miSelectAllTvillingCombinations_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var horse = (HPTHorse)fe.DataContext;
            if (!horse.Selected)
            {
                horse.Selected = true;
            }
            List<HPTCombination> combinationList = GetTvillingCombinationsForHorse(horse);

            foreach (var comb in combinationList)
            {
                comb.Selected = true;
            }
        }

        void miShowOnlyThisHorsesCombinationsTvilling_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var selectedHorse = (HPTHorse)mi.DataContext;
            foreach (var horse in selectedHorse.ParentRace.HorseList.Except(new HPTHorse[] { selectedHorse }).ToList())
            {
                horse.Selected = false;
            }
        }

        private void CreateHorseContextMenuTrio()
        {
        }

        void miV6Betmultiplier_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var horse = (HPTHorse)item.DataContext;
            if (!horse.Selected)
            {
                horse.Selected = true;
            }
            var rule = MarkBet.V6BetMultiplierRuleList
                .FirstOrDefault(r => r.HorseList.Contains(horse)
                                && r.HorseList.Count == 1);

            if (rule == null)
            {
                int maxRuleNumber = 0;
                if (MarkBet.V6BetMultiplierRuleList.Count > 0)
                {
                    maxRuleNumber = MarkBet.V6BetMultiplierRuleList.Max(r => r.RuleNumber);
                }
                rule = new HPTV6BetMultiplierRule(MarkBet, maxRuleNumber + 1);
            }

            var itemTag = (string)item.Tag;

            // Koll om V6/V7/V8
            if (itemTag.StartsWith("V6"))
            {
                rule.V6 = true;
            }

            // Koll om flerbong
            int betMultiplier = 1;
            string bmString = itemTag;
            if (itemTag.Contains("-"))
            {
                bmString = itemTag.Split('-')[1];
                betMultiplier = Convert.ToInt32(bmString);
            }
            if (int.TryParse(bmString, out betMultiplier))
            {
                rule.BetMultiplier = betMultiplier;
            }
            rule.RaceList.First(r => r.LegNr == horse.ParentRace.LegNr)
                .HorseList
                .First(h => h.StartNr == horse.StartNr)
                .Selected = true;
            MarkBet.V6BetMultiplierRuleList.Add(rule);
            MarkBet.ReductionV6BetMultiplierRule = true;
            rule.Use = true;
            MarkBet.UpdateV6BetMultiplierSingleRows();
            //this.MarkBet.SetV6BetMultiplierSingleRows();
        }

        #endregion

        private List<Expander> groupExpanderList;
        private void Expander_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && IsVisible)
            {
                var loadedExpander = (Expander)sender;
                if (groupExpanderList == null)
                {
                    groupExpanderList = new List<Expander>();
                }
                else
                {
                    var existingExpander = groupExpanderList.FirstOrDefault(exp => exp.Tag == loadedExpander.Tag);
                    if (existingExpander != null)
                    {
                        groupExpanderList.Remove(existingExpander);
                    }
                }
                groupExpanderList.Add(loadedExpander);
            }
        }

        private void btnHideOthers_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            foreach (var expander in groupExpanderList)
            {
                if (expander.Tag == fe.Tag)
                {
                    expander.IsExpanded = true;
                }
                else
                {
                    expander.IsExpanded = false;
                }
            }
            e.Handled = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (MarkBet != null && MarkBet.CouponCorrector != null && MarkBet.CouponCorrector.CouponHelper != null)
            {
                MarkBet.CouponCorrector.CouponHelper.HandleReserverForCoupons(MarkBet.ReservHandling);
            }
        }

        // Vy för att se alla rankpoäng
        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton == MouseButton.Left && HPTConfig.Config.IsPayingCustomer)
            if (e.ChangedButton == MouseButton.Left)
            {
                var tb = (TextBlock)sender;
                var horse = (HPTHorse)tb.DataContext;

                //this.PU.DataContext = tb.DataContext;

                var rankVariableList = HPTHorseRankVariable.CreateVariableList();
                var lstRanks = new ListBox()
                {
                    Padding = new Thickness(2)
                };
                foreach (var rankVariable in rankVariableList)
                {
                    // Hästens rank
                    var horseRank = horse.RankList.First(r => r.Name == rankVariable.PropertyName);

                    // Text med variabelnamn och kategori
                    var stInner = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    var tbRankVariableName = new TextBlock()
                    {
                        Text = rankVariable.Text + " (" + rankVariable.CategoryText + ")",
                        FontWeight = horseRank.Use ? FontWeights.Bold : FontWeights.Normal,
                        Width = 200D
                    };
                    stInner.Children.Add(tbRankVariableName);

                    // Text för hästens variabelrank
                    var tbRankValue = new TextBlock()
                    {
                        Text = horseRank.Rank.ToString() + " (" + horseRank.RankWeighted.ToString() + ")",
                        Width = 50D,
                        FontWeight = horseRank.Use ? FontWeights.Bold : FontWeights.Normal,
                        Background = horseRank.BackColor,
                        TextAlignment = TextAlignment.Right
                    };
                    stInner.Children.Add(tbRankValue);

                    // Textproperty för hästens variabelvärde
                    string displayValue = null;
                    if (string.IsNullOrEmpty(rankVariable.DisplayPropertyName))
                    {
                        displayValue = Convert.ToString(rankVariable.HorseProperty.GetValue(horse, null));
                    }
                    else
                    {
                        displayValue = Convert.ToString(horse.GetType().GetProperty(rankVariable.DisplayPropertyName).GetValue(horse, null));
                    }
                    decimal propertyValue = 0M;
                    bool isNumber = decimal.TryParse(displayValue, out propertyValue);

                    // Lägg i strängvariabel och formatera om det behövs
                    string textToShow = displayValue;
                    if (!isNumber)
                    {
                    }
                    else if (propertyValue == rankVariable.ValueForMissing || !rankVariable.Sort)
                    {
                        textToShow = "-";
                    }
                    else
                    {
                        textToShow = propertyValue.ToString(rankVariable.StringFormat);
                    }

                    // Skapa textblocket med ursprungsvärdet
                    var tbVariableValue = new TextBlock()
                    {
                        Text = textToShow,
                        Width = 60D,
                        FontWeight = horseRank.Use ? FontWeights.Bold : FontWeights.Normal,
                        Background = horseRank.BackColor,
                        TextAlignment = TextAlignment.Right
                    };
                    stInner.Children.Add(tbVariableValue);

                    lstRanks.Items.Add(stInner);
                }

                PU.Child = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1D),
                    Child = lstRanks
                };
                PU.IsOpen = true;
            }
        }

        private void chkNrSelect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var horse = (HPTHorse)fe.DataContext;

            var horseListSorted = (ListCollectionView)lvwLopp.ItemsSource;
            bool select = true;
            foreach (var o in horseListSorted)
            {
                var horseToSelect = (HPTHorse)o;
                if (horseToSelect.Scratched != true || horseToSelect.Scratched == null)
                {
                    horseToSelect.Selected = select;
                }
                if (horse == horseToSelect)
                {
                    select = false;
                }
            }
            e.Handled = true;
        }

        private void gvchABCD_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var xReduction = (HPTHorseXReduction)fe.DataContext;

            var horseListSorted = (ListCollectionView)lvwLopp.ItemsSource;
            foreach (var o in horseListSorted)
            {
                var horseToSelect = (HPTHorse)o;
                if (horseToSelect.Prio == HPTPrio.M || xReduction.Horse == horseToSelect)
                {
                    var xReductionToSelect = horseToSelect.HorseXReductionList.FirstOrDefault(xr => xr.Prio == xReduction.Prio);
                    if (xReductionToSelect != null)
                    {
                        xReductionToSelect.Selected = true;
                    }

                }
                if (xReduction.Horse == horseToSelect)
                {
                    break;
                }
            }
            e.Handled = true;
        }

        private void IntegerUpDown_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (MarkBet != null && !MarkBet.pauseRecalculation)
            {
                if (MarkBet.HasAlternateRankReduction())
                {
                    MarkBet.RecalculateReduction(RecalculateReason.Rank);
                }
                else
                {
                    MarkBet.RecalculateAllRanks();
                    MarkBet.RecalculateRank();
                }
            }
        }

        // KOMMANDE
        private void dudOwnProbability_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (MarkBet != null && (MarkBet.OwnProbabilityReductionRule.Use || MarkBet.OwnProbabilityCost))
                {
                    MarkBet.RecalculateReduction(RecalculateReason.All);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void TextBlock_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            //if (!HPTConfig.Config.IsPayingCustomer)
            //{
            //    return;
            //}
            var tb = (TextBlock)sender;
            var horse = (HPTHorse)tb.DataContext;

            Border b = new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1D),
                Child = new UCRaceView()
                {
                    BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                    BorderThickness = new Thickness(6D)
                }
            };

            PU.DataContext = horse.ParentRace;
            PU.Child = b;
            PU.IsOpen = true;
        }

        private void ucRaceView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && !CMColumnsToShow.HasItems)
            {
                Initialize();
            }
        }

        private MenuItem miGetTrends;
        private void GridViewColumnHeader_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right
                            || HorseListContainer.ParentRaceDayInfo.DataToShow.Usage != DataToShowUsage.Vxx
                            || !HPTConfig.Config.IsPayingCustomer)
            {
                return;
            }
            ClearContextMenu();
            if (miGetTrends == null && DataContext != null && DataContext.GetType() == typeof(HPTRace))
            {
                miGetTrends = new MenuItem()
                {
                    Header = "Hämta trender"
                };
                miGetTrends.Click += miGetTrends_Click;
            }
            AddContextMenuItem(miGetTrends);
            e.Handled = true;
        }

        void miGetTrends_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                var connector = new HPTServiceConnector();
                var raceDayInfoHistory = connector.GetRaceDayInfoHistoryGrouped(MarkBet.RaceDayInfo);

                MarkBet.RaceDayInfo.RaceList
                    .ForEach(r =>
                        r.HorseList
                            .ToList()
                            .ForEach(h => h.SetHistoryRelativeDifferenceUnadjusted())
                            );
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            Cursor = Cursors.Arrow;
        }
    }

    //class ColumnHandler
    //{
    //    public GridViewColumn Column { get; set; }

    //    public string Name { get; set; }

    //    public string BindingField { get; set; }

    //    public int Position { get; set; }
    //}
}

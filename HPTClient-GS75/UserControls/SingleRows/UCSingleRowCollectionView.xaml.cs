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
using System.ComponentModel;
using Xceed.Wpf.Toolkit;
using System.Collections.ObjectModel;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCSingleRowCollectionView.xaml
    /// </summary>
    public partial class UCSingleRowCollectionView : UCMarkBetControl
    {
        public UCSingleRowCollectionView()
        {
            this.CMColumnsToShow = new System.Windows.Controls.ContextMenu();
            this.CMSingleRow = new System.Windows.Controls.ContextMenu();
            this.SingleRowsObservable = new ObservableCollection<HPTMarkBetSingleRow>();
            InitializeComponent();
            //this.MarkBet.RaceDayInfo.RaceList[0].LegNrString
        }

        public ObservableCollection<HPTMarkBetSingleRow> SingleRowsObservable { get; set; }

        //public ObservableCollection<HPTMarkBetSingleRow> SingleRowsObservable
        //{
        //    get { return (ObservableCollection<HPTMarkBetSingleRow>)GetValue(SingleRowsObservableProperty); }
        //    set { SetValue(SingleRowsObservableProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SingleRowsObservable.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SingleRowsObservableProperty =
        //    DependencyProperty.Register("SingleRowsObservable", typeof(ObservableCollection<HPTMarkBetSingleRow>), typeof(UCSingleRowCollectionView), new PropertyMetadata(new ObservableCollection<HPTMarkBetSingleRow>()));


        #region Högerklicksmeny

        public ContextMenu CMColumnsToShow
        {
            get { return (ContextMenu)GetValue(CMColumnsToShowProperty); }
            set { SetValue(CMColumnsToShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMColumnsToShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMColumnsToShowProperty =
            DependencyProperty.Register("CMColumnsToShow", typeof(ContextMenu), typeof(UCSingleRowCollectionView), new UIPropertyMetadata(null));

        private void ClearContextMenu()
        {
            if (this.CMColumnsToShow == null)
            {
                return;
            }
            var separator = this.CMColumnsToShow.Items.OfType<Separator>().FirstOrDefault();
            if (separator != null)
            {
                while (this.CMColumnsToShow.Items[0] != separator)
                {
                    this.CMColumnsToShow.Items.RemoveAt(0);
                }
                this.CMColumnsToShow.Items.Remove(separator);
            }
        }

        private void AddContextMenuItem(MenuItem itemToAdd)
        {
            //ClearContextMenu();
            this.CMColumnsToShow.Items.Insert(0, new Separator());
            this.CMColumnsToShow.Items.Insert(0, itemToAdd);
        }

        private void AddContextMenuItems(IEnumerable<MenuItem> itemsToAdd)
        {
            ClearContextMenu();
            this.CMColumnsToShow.Items.Insert(0, new Separator());
            foreach (var item in itemsToAdd)
            {
                this.CMColumnsToShow.Items.Insert(0, item);
            }
        }

        private List<MenuItem> betMultiplierItems;
        private void gvcV6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearContextMenu();
            if (this.betMultiplierItems == null)
            {
                this.betMultiplierItems = new List<MenuItem>();
                if (this.MarkBet.BetType.Code == "V64" || this.MarkBet.BetType.Code == "V65")
                {
                    var miV6 = new MenuItem()
                    {
                        Header = "Sätt V6 på filtrerade rader"
                    };
                    miV6.Click += (o, s) =>
                        {
                            this.SingleRowsObservable
                                .ToList()
                                .ForEach(sr =>
                                {
                                    sr.V6 = true;
                                    sr.Edited = true;
                                }); 
                            this.MarkBet.UpdateV6BetMultiplierSingleRows();
                            this.isSettingV6BetMultiplier = false;
                            this.MarkBet.UpdateCoupons();
                        };
                    this.betMultiplierItems.Add(miV6);

                    var miRemoveV6 = new MenuItem()
                    {
                        Header = "Ta bort V6 på filtrerade rader"
                    };
                    miRemoveV6.Click += (o, s) =>
                    {
                        this.SingleRowsObservable
                            .ToList()
                            .ForEach(sr => 
                                {
                                    sr.V6 = false;
                                    sr.Edited = true;
                                });
                        this.MarkBet.UpdateV6BetMultiplierSingleRows();
                        this.isSettingV6BetMultiplier = false;
                        this.MarkBet.UpdateCoupons();
                    };
                    this.betMultiplierItems.Add(miRemoveV6);
                }
                var miBetMultiplierHeader = new MenuItem()
                {
                    Header = "Sätt flerbong på filtrerade rader"
                };

                var betMultiplierMenuItems = Enumerable
                    .Range(1, 10)
                    .Select(bm => new MenuItem()
                    {
                        Header = bm.ToString(),
                        Tag = bm
                    });

                betMultiplierMenuItems
                    .ToList()
                    .ForEach(miBetMultiplier =>
                    {
                        miBetMultiplierHeader.Items.Add(miBetMultiplier);
                        miBetMultiplier.Click += (o, s) =>
                        {
                            try
                            {
                                isSettingV6BetMultiplier = true;
                                this.SingleRowsObservable
                                    .ToList()
                                    .ForEach(sr =>
                                        {
                                            sr.BetMultiplier = Convert.ToInt32(miBetMultiplier.Tag); isSettingV6BetMultiplier = true;
                                            sr.CreateBetMultiplierList(this.MarkBet);
                                            sr.Edited = true;
                                        });
                                this.MarkBet.UpdateV6BetMultiplierSingleRows();
                                this.isSettingV6BetMultiplier = false;
                                this.MarkBet.UpdateCoupons();
                            }
                            catch (Exception exc)
                            {
                                string s2 = exc.Message;
                            }
                            isSettingV6BetMultiplier = false;
                        };
                    });
                this.betMultiplierItems.Add(miBetMultiplierHeader);
            }
            AddContextMenuItems(this.betMultiplierItems);
            e.Handled = true;
        }

        private void gvc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearContextMenu();
            e.Handled = true;
        }

        #endregion

        private void gvcRowSingleRow_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            String field = column.Tag as String;

            ListSortDirection newDir = ListSortDirection.Ascending;

            if (this.lvwSingleRows.Items.SortDescriptions.Count > 0)
            {
                SortDescription sd = this.lvwSingleRows.Items.SortDescriptions[0];
                if (sd.PropertyName == field)
                {
                    SortDescription sdNew = new SortDescription();
                    sdNew.PropertyName = sd.PropertyName;
                    sdNew.Direction = sd.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    this.lvwSingleRows.Items.SortDescriptions.Clear();
                    this.lvwSingleRows.Items.SortDescriptions.Add(sdNew);
                    return;
                }
            }

            switch (field)
            {
                case "PercentSum":
                case "ShareSum":
                case "StakePercentSum":
                case "V6":
                    newDir = ListSortDirection.Descending;
                    break;
                default:
                    break;
            }

            this.lvwSingleRows.Items.SortDescriptions.Clear();
            this.lvwSingleRows.Items.SortDescriptions.Add(new SortDescription(field, newDir));
        }

        private void lvwSingleRows_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (!HPTConfig.Config.IsPayingCustomer)
            //{
            //    return;
            //}

            if (!this.CMSingleRow.HasItems)
            {
                this.CMSingleRow.Items.Clear();
                CreateHorseContextMenuVxx();
            }
            //this.lvwSingleRows.ContextMenu = this.CMSingleRow;
        }

        void miSimulateResult_Click(object sender, RoutedEventArgs e)
        {
            if (this.lvwSingleRows.SelectedItem != null && this.lvwSingleRows.SelectedItem.GetType() == typeof(HPTMarkBetSingleRow))
            {
                var singleRow = (HPTMarkBetSingleRow)this.lvwSingleRows.SelectedItem;
                //this.MarkBet.CouponCorrector.CorrectCouponsSimulatedResult();
            }
        }

        private bool isSettingV6BetMultiplier;
        private void chkV6_Checked(object sender, RoutedEventArgs e)
        {
            var selectedRowList = GetSelectedRows();
            if (isSettingV6BetMultiplier || selectedRowList.Count == 0)
            {
                return;
            }
            isSettingV6BetMultiplier = true;
            try
            {
                var chk = (CheckBox)sender;
                var singleRow = (HPTMarkBetSingleRow)chk.DataContext;
                bool v6 = (bool)chk.IsChecked;
                singleRow.V6 = v6;
                singleRow.Edited = true;

                //var selectedRowList = GetSelectedRows();
                foreach (var sr in selectedRowList)
                {
                    sr.V6 = v6;
                    sr.Edited = true;
                }
                //if (this.MarkBet.CouponCompression != CouponCompression.SingleRows)
                //{
                //    this.MarkBet.CouponCompression = CouponCompression.SingleRows;
                //}
                //this.MarkBet.UpdateCoupons();
                this.MarkBet.UpdateV6BetMultiplierSingleRows();
            }
            catch (Exception exc)
            {                
            }
            isSettingV6BetMultiplier = false;
        }

        private void iudBetMultiplierSingleRow_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedRowList = GetSelectedRows();
            if (isSettingV6BetMultiplier || selectedRowList.Count == 0)
            {
                return;
            }
            SetV6BetMultiplierOnSingleRows(sender, selectedRowList);
        }

        private void SetV6BetMultiplierOnSingleRows(object sender, List<HPTMarkBetSingleRow> selectedRowList)
        {
            isSettingV6BetMultiplier = true;
            try
            {
                var iud = (IntegerUpDown)sender;
                var singleRow = (HPTMarkBetSingleRow)iud.DataContext;
                int betMultiplier = (int)iud.Value;
                singleRow.BetMultiplier = betMultiplier;
                singleRow.CreateBetMultiplierList(this.MarkBet);
                singleRow.Edited = true;

                //var selectedRowList = GetSelectedRows();
                foreach (var sr in selectedRowList)
                {
                    sr.BetMultiplier = betMultiplier;
                    sr.CreateBetMultiplierList(this.MarkBet);
                    sr.Edited = true;
                }
                this.MarkBet.UpdateV6BetMultiplierSingleRows();
                isSettingV6BetMultiplier = false;
                this.MarkBet.UpdateCoupons();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private List<HPTMarkBetSingleRow> GetSelectedRows()
        {
            return this.MarkBet.SingleRowCollection.SingleRows
                .Where(sr => sr.SelectedForEditing)
                .ToList();
        }

        private void miClearSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedRowList = GetSelectedRows();

            foreach (var sr in selectedRowList)
            {
                sr.SelectedForEditing = false;
            }
        }

        #region Context menu handling

        public ContextMenu CMSingleRow
        {
            get { return (ContextMenu)GetValue(CMSingleRowProperty); }
            set { SetValue(CMSingleRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMHorse.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMSingleRowProperty =
            DependencyProperty.Register("CMSingleRow", typeof(ContextMenu), typeof(UCSingleRowCollectionView), new UIPropertyMetadata(null));

        
        private void CreateHorseContextMenuVxx()
        {
            // Markera flera enkelrader
            var miSelectMultiple = new MenuItem()
            {
                Header = "Välj markerade",
                Tag = "SelectMultiple"
            };
            miSelectMultiple.Click += new RoutedEventHandler(miSelectMultiple_Click);

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
                Header = "Spela V6/V7 och flerbong",
                Tag = "V6"
            };

            // Skapa enskilda val
            foreach (var bm in this.MarkBet.BetType.BetMultiplierList)
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

            // Simulera resultat
            var miSimulateResult = new MenuItem()
            {
                Header = "Simulera utdelning",
                IsEnabled = false
            };
            miSimulateResult.Click += new RoutedEventHandler(miSimulateResult_Click);

            // Lägg till i kontextmenyn
            this.CMSingleRow.Items.Add(miSelectMultiple);
            this.CMSingleRow.Items.Add(miV6);
            this.CMSingleRow.Items.Add(miV6AndBetMultiplierHeader);
            this.CMSingleRow.Items.Add(miBetMultiplierHeader);
            this.CMSingleRow.Items.Add(miSimulateResult);
        }

        void miSelectMultiple_Click(object sender, RoutedEventArgs e)
        {
            var singleRowsSelected = GetSelectedRowsInListview();
            foreach (var singleRow in singleRowsSelected)
            {
                singleRow.SelectedForEditing = true;
            }
        }

        void miV6Betmultiplier_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var itemTag = (string)item.Tag;

            // Koll om V6/V7/V8
            bool v6 = itemTag.StartsWith("V6");

            // Koll om flerbong
            int betMultiplier = 1;
            string bmString = itemTag;
            if (itemTag.Contains("-"))
            {
                bmString = itemTag.Split('-')[1];
                betMultiplier = Convert.ToInt32(bmString);
            }

            // Sätt värden på enkelrader
            var singleRowsSelected = GetSelectedRowsInListview();
            foreach (var singleRow in singleRowsSelected)
            {
                singleRow.Edited = true;
                singleRow.V6 = v6;
                singleRow.BetMultiplier = betMultiplier;
                singleRow.CreateBetMultiplierList(this.MarkBet);
            }
            //this.MarkBet.UpdateTotalCouponSize();
            this.MarkBet.UpdateV6BetMultiplierSingleRows();
        }

        private List<HPTMarkBetSingleRow> GetSelectedRowsInListview()
        {
            return this.lvwSingleRows.SelectedItems
                .Cast<HPTMarkBetSingleRow>()
                .ToList();
        }

        #endregion

        private void miCalculateRowValueWithError_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.CalculateSingleRowsPotential();
        }

        private void ucSingleRowCollectionView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Skapa kontextmeny för att visa/dölja kolumner
                //if (this.CombBet.DataToShow.CMColumnsToShow == null)
                if (!this.CMColumnsToShow.HasItems)
                {
                    this.CMColumnsToShow = new ContextMenu();
                    List<HorseDataToShowAttribute> attributeList = HPTConfig.Config.SingleRowDataToShow.GetHorseDataToShowAttributes();
                    this.CMColumnsToShow.Items.Clear();
                    this.CMColumnsToShow.DataContext = HPTConfig.Config.SingleRowDataToShow;
                    foreach (HorseDataToShowAttribute hda in attributeList)
                    {
                        MenuItem mi = new MenuItem()
                        {
                            IsCheckable = true,
                            Header = hda.Name,
                            IsEnabled = true
                            //IsEnabled = hda.RequiresPro ? HPTConfig.Config.IsPayingCustomer : true
                        };
                        mi.SetBinding(MenuItem.IsCheckedProperty, hda.PropertyName);
                        this.CMColumnsToShow.Items.Add(mi);
                    }
                    HPTConfig.Config.SingleRowDataToShow.PropertyChanged += new PropertyChangedEventHandler(DataToShow_PropertyChanged);
                }

                // Skapa en lista med alla kolumner i listvyn
                if (this.ColumnHandlerList == null || this.ColumnHandlerList.Count == 0)
                {
                    CreateColumnHandlerList();

                    List<string> propertyNamesList = this.ColumnHandlerList.Select(ch => ch.BindingField).ToList();

                    foreach (string propertyName in propertyNamesList)
                    {
                        // Ta bort de kolumner man inte vill visa
                        bool show = (bool)HPTConfig.Config
                            .SingleRowDataToShow.GetType().GetProperty(propertyName)
                            .GetValue(HPTConfig.Config.SingleRowDataToShow, null);

                        HandleColumn(propertyName, show);
                    }
                    //SortColumns();
                }
                if (this.MarkBet != null && this.MarkBet.SingleRowCollection != null)
                {
                    this.MarkBet.SingleRowCollection.PropertyChanged -= SingleRowCollection_PropertyChanged;
                    this.MarkBet.SingleRowCollection.PropertyChanged += SingleRowCollection_PropertyChanged;
                    if (this.MarkBet.SingleRowCollection.SingleRows.Count > 0 && this.SingleRowsObservable.Count == 0)
                    {
                        //Dispatcher.Invoke(FillSingleRows);
                        Dispatcher.Invoke(FilterRows);
                    }
                }
                // Lägg till eventhandler för om kolumner ändrar ordning eller tas bort
                //this.gvwSingleRows.Columns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
            }
        }

        void SingleRowCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SingleRows")
            {
                //Dispatcher.Invoke(FillSingleRows);
                Dispatcher.Invoke(FilterRows);
            }
        }

        internal void FillSingleRows()
        {
            try
            {
                this.SingleRowsObservable.Clear();
                this.MarkBet.SingleRowCollection.SingleRows
                    .ToList()
                    .ForEach(sr => this.SingleRowsObservable.Add(sr));
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private List<ColumnHandler> ColumnHandlerList;
        private void CreateColumnHandlerList()
        {
            this.ColumnHandlerList = new List<ColumnHandler>();

            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRowNumber, Name = "gvcRowNumber", Position = 0, BindingField = "ShowRowNumber" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRowValue, Name = "gvcRowValue", Position = 0, BindingField = "ShowRowValue" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcV6, Name = "gvcV6", Position = 0, BindingField = "ShowV6" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRowValueV6, Name = "gvcRowValueV6", Position = 0, BindingField = "ShowRowValueV6" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcBetMultiplier, Name = "gvcBetMultiplier", Position = 0, BindingField = "ShowBetMultiplier" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRowValueBetMultiplier, Name = "gvcRowValueBetMultiplier", Position = 0, BindingField = "ShowRowValueBetMultiplier" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRowValueOneError, Name = "gvcRowValueOneError", Position = 0, BindingField = "ShowRowValue1And2Errors" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRowValueTwoErrors, Name = "gvcRowValueTwoErrors", Position = 0, BindingField = "ShowRowValue1And2Errors" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStakePercentSum, Name = "gvcStakePercentSum", Position = 0, BindingField = "ShowStakeShareSum" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStartNumberSum, Name = "gvcStartNumberSum", Position = 0, BindingField = "ShowStartNumberSum" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRankSum, Name = "gvcRankSum", Position = 0, BindingField = "ShowRankSum" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcOwnProbability, Name = "gvcOwnProbability", Position = 0, BindingField = "ShowOwnProbability" });

            // All hästarna på raden
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd1, Name = "gvcAvd1", Position = 0, BindingField = "ShowHorses" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd2, Name = "gvcAvd2", Position = 0, BindingField = "ShowHorses" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd3, Name = "gvcAvd3", Position = 0, BindingField = "ShowHorses" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd4, Name = "gvcAvd4", Position = 0, BindingField = "ShowHorses" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd5, Name = "gvcAvd5", Position = 0, BindingField = "ShowHorses" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd6, Name = "gvcAvd6", Position = 0, BindingField = "ShowHorses" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd7, Name = "gvcAvd7", Position = 0, BindingField = "ShowHorses" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcAvd8, Name = "gvcAvd8", Position = 0, BindingField = "ShowHorses" });

            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcRowValueWithoutScratchings, Name = "gvcRowValueWithoutScratchings", Position = 0, BindingField = "ShowRowValue" });
        }

        void DataToShow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender.GetType().BaseType == typeof(HPTDataToShow))
            {
                bool show = (bool)sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null);
                HandleColumn(e.PropertyName, show);
            }
        }

        private void HandleColumn(string showText, bool show)
        {
            List<ColumnHandler> columnsToHandle =
                this.ColumnHandlerList.Where(ch => ch.BindingField == showText).ToList();
            foreach (var columnHandler in columnsToHandle)
            {
                GridViewColumn gvc = columnHandler.Column;
                if (show && !this.gvwSingleRows.Columns.Contains(gvc))
                {
                    this.gvwSingleRows.Columns.Add(gvc);
                }
                else if (!show && this.gvwSingleRows.Columns.Contains(gvc))
                {
                    this.gvwSingleRows.Columns.Remove(gvc);
                }
            }
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        // KOMMANDE
        private void dudMinOwnProbability_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (this.MarkBet.OwnProbabilityReductionRule.Use)
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.All);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #region Filtering och radvärdeberäkning

        //internal List<HPTMarkBetSingleRow> hiddenRows = new List<HPTMarkBetSingleRow>();
        private void lstRowValueCalculation_Checked(object sender, RoutedEventArgs e)
        {
            FilterRows();
        }

        private void FilterRows()
        {
            var selectedHorses = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.SelectedForRowValueCalculation)
                .GroupBy(h => h.ParentRace.LegNr)
                .ToList();

            if (!selectedHorses.Any())
            {
                FillSingleRows();
                return;
            }

            var selectedRows = this.MarkBet.SingleRowCollection.SingleRows.AsParallel();

            selectedHorses
                .ForEach(h =>
                {
                    selectedRows = selectedRows.Where(sr => h.Contains(sr.HorseList[h.Key - 1]));
                });

            this.SingleRowsObservable.Clear();

            int numberOfPools = this.MarkBet.BetType.PayOutDummyList.Length;

            selectedRows
                .ToList()
                .ForEach(sr =>
                {
                    if (numberOfPools > 1)
                    {
                        sr.RowValueOneError = this.MarkBet.CouponCorrector.CalculatePayOutOneError(sr.HorseList, this.MarkBet.BetType.PoolShareOneError * this.MarkBet.BetType.RowCost);
                        if (numberOfPools > 2)
                        {
                            sr.RowValueTwoErrors = this.MarkBet.CouponCorrector.CalculatePayOutTwoErrors(sr.HorseList, this.MarkBet.BetType.PoolShareTwoErrors * this.MarkBet.BetType.RowCost);
                        }
                    }
                    this.SingleRowsObservable.Add(sr);
                });

            var orderedPayOutList = this.MarkBet.BetType.PayOutDummyList
                .OrderByDescending(po => po.NumberOfCorrect);

            try
            {
                if (numberOfPools > 0)
                {
                    orderedPayOutList.ElementAt(0).MinRowValue = selectedRows.Min(sr => sr.RowValue);
                    orderedPayOutList.ElementAt(0).MaxRowValue = selectedRows.Max(sr => sr.RowValue);

                    if (numberOfPools > 1)
                    {
                        orderedPayOutList.ElementAt(1).MinRowValue = selectedRows.Min(sr => sr.RowValueOneError);
                        orderedPayOutList.ElementAt(1).MaxRowValue = selectedRows.Max(sr => sr.RowValueOneError);

                        if (numberOfPools > 2)
                        {
                            orderedPayOutList.ElementAt(2).MinRowValue = selectedRows.Min(sr => sr.RowValueTwoErrors);
                            orderedPayOutList.ElementAt(2).MaxRowValue = selectedRows.Max(sr => sr.RowValueTwoErrors);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #endregion

        private void miSelectAllFiltered_Click(object sender, RoutedEventArgs e)
        {
            this.SingleRowsObservable.ToList().ForEach(sr =>
                {
                    sr.SelectedForEditing = true;
                });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Reflection;
using Xceed.Wpf.Toolkit;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCCombinationView.xaml
    /// </summary>
    public partial class UCCombinationView : UCCombBetControl
    {
        public UCCombinationView()
        {
            this.CMColumnsToShow = new System.Windows.Controls.ContextMenu();
            InitializeComponent();
        }

        #region Dependency properties

        static void CombBetChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var uc = (UCCombinationView)property;
            uc.CombBet = (HPTCombBet)args.NewValue;
        }

        //public HPTCombBet CombBet
        //{
        //    get { return (HPTCombBet)GetValue(CombBetProperty); }
        //    set { SetValue(CombBetProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ReductionCheckBoxList.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CombBetProperty =
        //    DependencyProperty.Register("CombBet", typeof(HPTCombBet), typeof(UCCombinationView), new UIPropertyMetadata(null, new PropertyChangedCallback(CombBetChangedCallBack)));




        public ContextMenu CMBet
        {
            get { return (ContextMenu)GetValue(CMBetProperty); }
            set { SetValue(CMBetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMBet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMBetProperty =
            DependencyProperty.Register("CMBet", typeof(ContextMenu), typeof(UCCombinationView), new UIPropertyMetadata(null));



        public ContextMenu CMColumnsToShow
        {
            get { return (ContextMenu)GetValue(CMColumnsToShowProperty); }
            set { SetValue(CMColumnsToShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMColumnsToShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMColumnsToShowProperty =
            DependencyProperty.Register("CMColumnsToShow", typeof(ContextMenu), typeof(UCCombinationView), new UIPropertyMetadata(null));

        #endregion

        private void chkSelected_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if ((bool)chk.IsChecked)
            {
                HPTCombination comb = (HPTCombination)chk.DataContext;
                if (this.CombBet.TargetReturn > 0)
                {
                    this.CombBet.CalculateStake(comb);
                }
                comb.Stake = comb.Stake > this.CombBet.BetType.LowestStake ? comb.Stake : this.CombBet.BetType.LowestStake;
            }
            //if (this.CombBet.TargetReturn != 0 && (bool)chk.IsChecked)
            //{
            //    HPTCombination comb = (HPTCombination)chk.DataContext;
            //    this.CombBet.CalculateStake();
            //}
            this.CombBet.SetStakeAndNumberOfSelected();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            String field = column.Tag as String;

            ListSortDirection newDir = ListSortDirection.Ascending;

            if (this.lvwCombinations.Items.SortDescriptions.Count > 0)
            {
                SortDescription sd = this.lvwCombinations.Items.SortDescriptions[0];
                if (sd.PropertyName == field)
                {
                    SortDescription sdNew = new SortDescription();
                    sdNew.PropertyName = sd.PropertyName;
                    sdNew.Direction = sd.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    this.lvwCombinations.Items.SortDescriptions.Clear();
                    this.lvwCombinations.Items.SortDescriptions.Add(sdNew);
                    return;
                }
            }

            switch (field)
            {
                case "Stake":
                case "Selected":
                case "Profit":
                case "OddsQuota":
                case "VPQuota":
                case "VQuota":
                case "PQuota":
                case "TVQuota":
                case "StakeQuota":
                case "OPQuota":
                    newDir = ListSortDirection.Descending;
                    break;
                default:
                    break;
            }

            this.lvwCombinations.Items.SortDescriptions.Clear();
            this.lvwCombinations.Items.SortDescriptions.Add(new SortDescription(field, newDir));
        }

        private void lvwCombinationOdds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.RemovedItems.Count > 0)
                {
                    HPTCombination combOld = (HPTCombination)e.RemovedItems[0];
                    combOld.Horse1.IsHighlighted = false;
                    combOld.Horse2.IsHighlighted = false;
                    if (combOld.Horse3 != null)
                    {
                        combOld.Horse3.IsHighlighted = false;
                    }
                }
                HPTCombination combNew = (HPTCombination)e.AddedItems[0];
                combNew.Horse1.IsHighlighted = true;
                combNew.Horse2.IsHighlighted = true;
                if (combNew.Horse3 != null)
                {
                    combNew.Horse3.IsHighlighted = true;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #region Column handling

        private List<ColumnHandler> ColumnHandlerList;
        private void CreateColumnHandlerList()
        {
            this.ColumnHandlerList = new List<ColumnHandler>();

            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcCombinationRank, Name = "gvcCombinationRank", Position = 0, BindingField = "ShowCombinationOddsRank" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcCombinationOdds, Name = "gvcCombinationOdds", Position = 0, BindingField = "ShowCombinationOdds" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcMultipliedOdds, Name = "gvcMultipliedOdds", Position = 0, BindingField = "ShowMultipliedOdds" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcOddsRank, Name = "gvcOddsRank", Position = 0, BindingField = "ShowMultipliedOddsRank" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcPlayability, Name = "gvcPlayability", Position = 0, BindingField = "ShowPlayability" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcProfit, Name = "gvcProfit", Position = 0, BindingField = "ShowProfit" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcSelected, Name = "gvcSelected", Position = 0, BindingField = "ShowSelected" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStake, Name = "gvcStake", Position = 0, BindingField = "ShowStake" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStartNr1, Name = "gvcStartNr1", Position = 0, BindingField = "ShowStartNr1" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStartNr2, Name = "gvcStartNr2", Position = 0, BindingField = "ShowStartNr2" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcStartNr3, Name = "gvcStartNr3", Position = 0, BindingField = "ShowStartNr3" });

            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcVPQuota, Name = "gvcVPQuota", Position = 0, BindingField = "ShowVPQuota" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcVQuota, Name = "gvcVQuota", Position = 0, BindingField = "ShowVQuota" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcPQuota, Name = "gvcPQuota", Position = 0, BindingField = "ShowPQuota" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcOPQuota, Name = "gvcOPQuota", Position = 0, BindingField = "ShowOPQuota" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcTVQuota, Name = "gvcTVQuota", Position = 0, BindingField = "ShowTVQuota" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcDQuota, Name = "gvcDQuota", Position = 0, BindingField = "ShowDQuota" });
            this.ColumnHandlerList.Add(new ColumnHandler() { Column = gvcVxxQuota, Name = "gvcVxxQuota", Position = 0, BindingField = "ShowStakeQuota" });
        }


        #endregion

        private HPTCombinationListInfo combinationListInfo;
        public HPTCombinationListInfo CombinationListInfo
        {
            get
            {
                if (this.combinationListInfo == null)
                {
                    this.combinationListInfo = (HPTCombinationListInfo)this.DataContext;
                }
                return this.combinationListInfo;
            }
        }

        private void SortColumns()
        {
            if (this.CombBet.DataToShow.ColumnsInOrder == null
                || this.CombBet.DataToShow.ColumnsInOrder.Count == 0
                || this.CombBet.DataToShow.ColumnsInOrder.First() == string.Empty)
            {
                return;
            }

            this.gvwCombinations.Columns.Clear();
            foreach (var columnName in this.CombBet.DataToShow.ColumnsInOrder)
            {
                ColumnHandler columnHandler = this.ColumnHandlerList.FirstOrDefault(ch => columnName == ch.Name);
                if (columnHandler != null)
                {
                    GridViewColumn gvc = columnHandler.Column;
                    this.gvwCombinations.Columns.Add(gvc);
                }
            }
        }

        void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SaveColumnOrder();
        }

        private void SaveColumnOrder()
        {
            this.CombBet.DataToShow.ColumnsInOrder = new List<string>();
            for (int i = 0; i < this.gvwCombinations.Columns.Count; i++)
            {
                ColumnHandler columnHandler = this.ColumnHandlerList.First(ch => ch.Column == this.gvwCombinations.Columns[i]);
                columnHandler.Position = i;
                this.CombBet.DataToShow.ColumnsInOrder.Add(columnHandler.Name);
            }
        }

        private void ucCombinationView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            {
                // Skapa kontextmeny för att visa/dölja kolumner
                if (!this.CMColumnsToShow.HasItems)
                {
                    this.CMColumnsToShow = new ContextMenu();
                    List<HorseDataToShowAttribute> attributeList = this.CombBet.DataToShow.GetHorseDataToShowAttributes();
                    this.CMColumnsToShow.Items.Clear();
                    this.CMColumnsToShow.DataContext = this.CombBet.DataToShow;
                    foreach (HorseDataToShowAttribute hda in attributeList)
                    {
                        MenuItem mi = new MenuItem()
                        {
                            IsCheckable = true,
                            Header = hda.Name,
                            IsEnabled = true    // hda.RequiresPro ? HPTConfig.Config.IsPayingCustomer : true
                        };
                        mi.SetBinding(MenuItem.IsCheckedProperty, hda.PropertyName);
                        this.CMColumnsToShow.Items.Add(mi);
                    }
                }

                // Skapa en lista med alla kolumner i listvyn
                if (this.ColumnHandlerList == null || this.ColumnHandlerList.Count == 0)
                {
                    CreateColumnHandlerList();

                    List<string> propertyNamesList = this.ColumnHandlerList.Select(ch => ch.BindingField).ToList();

                    foreach (string propertyName in propertyNamesList)
                    {
                        // Ta bort de kolumner man inte vill visa
                        bool show = (bool)this.CombBet
                            .DataToShow.GetType().GetProperty(propertyName)
                            .GetValue(this.CombBet.DataToShow, null);

                        HandleColumn(propertyName, show);
                    }
                    SortColumns();
                }
                // Lägg till eventhandler för om kolumner ändrar ordning eller tas bort
                this.gvwCombinations.Columns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
                this.CombBet.DataToShow.PropertyChanged += new PropertyChangedEventHandler(DataToShow_PropertyChanged);
            }
        }

        private void HandleColumn(string showText, bool show)
        {
            List<ColumnHandler> columnsToHandle =
                this.ColumnHandlerList.Where(ch => ch.BindingField == showText).ToList();
            foreach (var columnHandler in columnsToHandle)
            {
                GridViewColumn gvc = columnHandler.Column;
                if (show && !this.gvwCombinations.Columns.Contains(gvc))
                {
                    this.gvwCombinations.Columns.Add(gvc);
                }
                else if (!show && this.gvwCombinations.Columns.Contains(gvc))
                {
                    this.gvwCombinations.Columns.Remove(gvc);
                }
            }
        }

        void DataToShow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //HPTHorseDataToShow hdts = (HPTHorseDataToShow)sender;

            if (sender.GetType().BaseType == typeof(HPTDataToShow))
            {
                bool show = (bool)sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null);
                HandleColumn(e.PropertyName, show);
            }
        }

        private void ucCombinationView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Ta bort eventhandlers för ändringar i konfigurationen (programmet läcker minne annars)
                this.CombBet.DataToShow.PropertyChanged -= new PropertyChangedEventHandler(DataToShow_PropertyChanged);
                this.gvwCombinations.Columns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
            }
        }

        private void GroupBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.CombBet.SetStakeAndNumberOfSelected();
        }

        #region Högerklicksmenyer

        private void miSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var comb in this.CombinationListInfo.CombinationsToShowList)
            {
                comb.Selected = true;
            }
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var comb in this.CombinationListInfo.CombinationsToShowList)
            {
                comb.Selected = false;
            }
        }

        private void CreateCombinationContextMenu()
        {
            // Kontrollera om menyn redan är skapad eller ej
            if (this.CMBet == null)
            {
                this.CMBet = new ContextMenu();
            }
            else
            {
                return;
            }

            // Välj att spela för ett visst belopp
            var miBetForSum = new MenuItem()
            {
                Header = "Spela för belopp",
                Tag = ""
            };
            miBetForSum.Items.Add(new MenuItem()
            {
                Header = "50",
                Tag = "50"
            });
            miBetForSum.Items.Add(new MenuItem()
            {
                Header = "100",
                Tag = "100"
            });
            miBetForSum.Items.Add(new MenuItem()
            {
                Header = "200",
                Tag = "200"
            });
            miBetForSum.Items.Add(new MenuItem()
            {
                Header = "300",
                Tag = "300"
            });
            miBetForSum.Items.Add(new MenuItem()
            {
                Header = "500",
                Tag = "500"
            });
            miBetForSum.Items.Add(new MenuItem()
            {
                Header = "1000",
                Tag = "1000"
            });
            this.CMBet.Items.Add(miBetForSum);
            foreach (MenuItem itemBetForSum in miBetForSum.Items)
            {
                itemBetForSum.Click += new RoutedEventHandler(itemBetForSum_Click);
            }

            // Välj att spela ett visst antal kombinationer
            var miBetNumberOfCombinations = new MenuItem()
            {
                Header = "Spela antal kombinationer",
                Tag = ""
            };
            miBetNumberOfCombinations.Items.Add(new MenuItem()
            {
                Header = "3",
                Tag = "3"
            });
            miBetNumberOfCombinations.Items.Add(new MenuItem()
            {
                Header = "5",
                Tag = "5"
            });
            miBetNumberOfCombinations.Items.Add(new MenuItem()
            {
                Header = "7",
                Tag = "7"
            });
            miBetNumberOfCombinations.Items.Add(new MenuItem()
            {
                Header = "10",
                Tag = "10"
            });
            miBetNumberOfCombinations.Items.Add(new MenuItem()
            {
                Header = "15",
                Tag = "15"
            });
            miBetNumberOfCombinations.Items.Add(new MenuItem()
            {
                Header = "25",
                Tag = "25"
            });
            this.CMBet.Items.Add(miBetNumberOfCombinations);
            foreach (MenuItem itemBetNumberOfCombinations in miBetNumberOfCombinations.Items)
            {
                itemBetNumberOfCombinations.Click += new RoutedEventHandler(itemBetNumberOfCombinations_Click);
            }
        }

        void itemBetNumberOfCombinations_Click(object sender, RoutedEventArgs e)
        {
            if (this.CombBet.TargetReturn == 0)
            {
                this.CombBet.TargetReturn = 500;
            }
            var mi = (MenuItem)sender;
            var numberOfCombinationsString = (string)mi.Header;
            var numberOfCombinations = Convert.ToInt32(numberOfCombinationsString);
            SelectNumberOfCombinations(numberOfCombinations, (string)mi.Tag);
        }

        void itemBetForSum_Click(object sender, RoutedEventArgs e)
        {
            if (this.CombBet.TargetReturn == 0)
            {
                this.CombBet.TargetReturn = 500;
            }
            var mi = (MenuItem)sender;
            var betSumtring = (string)mi.Header;
            var betSum = Convert.ToInt32(betSumtring);
            SelectBetSum(betSum, (string)mi.Tag);
        }

        private void chQuota_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateCombinationContextMenu();
            SetTagOnContextMenuChildren(this.CMBet.Items, "Quota");
        }

        private void chPlayability_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateCombinationContextMenu();
            SetTagOnContextMenuChildren(this.CMBet.Items, "Playability");
        }

        private void SetTagOnContextMenuChildren(ItemCollection ic, object tag)
        {
            foreach (MenuItem item in ic)
            {
                if (item.HasItems)
                {
                    SetTagOnContextMenuChildren(item.Items, tag);
                }
                item.Tag = tag;
            }
        }

        private void SelectNumberOfCombinations(int numberOfCombinations, string sortVariable)
        {
            List<HPTCombination> combinationsToSelect = null;
            switch (sortVariable)
            {
                case "Playability":
                    combinationsToSelect = this.CombinationListInfo.CombinationsToShowList
                        .OrderByDescending(c => c.Playability)
                        .Take(numberOfCombinations).ToList();
                    break;
                case "Quota":
                    combinationsToSelect = this.CombinationListInfo.CombinationsToShowList
                        .OrderByDescending(c => c.OddsQuota)
                        .Take(numberOfCombinations).ToList();
                    break;
                default:
                    break;
            }
            foreach (var comb in combinationsToSelect)
            {
                comb.Selected = true;
            }
        }

        private void SelectBetSum(int betSum, string sortVariable)
        {
            List<HPTCombination> combinationsToSelect = null;
            switch (sortVariable)
            {
                case "Playability":
                    combinationsToSelect = this.CombinationListInfo.CombinationsToShowList
                        .OrderByDescending(c => c.Playability).ToList();
                    break;
                case "Quota":
                    combinationsToSelect = this.CombinationListInfo.CombinationsToShowList
                        .OrderByDescending(c => c.OddsQuota).ToList();
                    break;
                default:
                    break;
            }
            int totalStake = 0;
            foreach (var comb in combinationsToSelect)
            {
                comb.Selected = true;
                this.CombBet.CalculateStake(comb);
                totalStake += (int)comb.Stake;
                if (totalStake > betSum)
                {
                    break;
                }
            }
        }

        #endregion

        private void hlResetToRecommended_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            switch (this.CombBet.BetType.Code)
            {
                case "TV":
                    ResetCombinationDataToShowTvilling();
                    break;
                case "DD":
                case "LD":
                    ResetCombinationDataToShowDouble();
                    break;
                case "T":
                    ResetCombinationDataToShowTrio();
                    break;
                default:
                    break;
            }
        }

        internal void ResetCombinationDataToShowTrio()
        {
            // Standarduppsättningen av kolumner
            this.CombBet.DataToShow.ShowCombinationOdds = true;
            this.CombBet.DataToShow.ShowSelected = true;
            this.CombBet.DataToShow.ShowStartNr1 = true;
            this.CombBet.DataToShow.ShowStartNr2 = true;
            this.CombBet.DataToShow.ShowStartNr3 = true;
            this.CombBet.DataToShow.ShowVPQuota = true;
            this.CombBet.DataToShow.ShowStake = true;
            this.CombBet.DataToShow.ShowProfit = true;

            // Övriga kolumner
            this.CombBet.DataToShow.ShowVQuota = false;
            this.CombBet.DataToShow.ShowPQuota = false;
            this.CombBet.DataToShow.ShowCombinationOddsRank = false;
            this.CombBet.DataToShow.ShowPlayability = false;
            this.CombBet.DataToShow.ShowStakeQuota = false;
            this.CombBet.DataToShow.ShowDQuota = false;
            this.CombBet.DataToShow.ShowMultipliedOdds = false;
            this.CombBet.DataToShow.ShowMultipliedOddsRank = false;
            this.CombBet.DataToShow.ShowOddsQuota = false;
            this.CombBet.DataToShow.ShowOPQuota = false;
            this.CombBet.DataToShow.ShowTVQuota = false;
        }

        internal void ResetCombinationDataToShowTvilling()
        {
            // Standarduppsättningen av kolumner
            this.CombBet.DataToShow.ShowCombinationOdds = true;
            this.CombBet.DataToShow.ShowSelected = true;
            this.CombBet.DataToShow.ShowStartNr1 = true;
            this.CombBet.DataToShow.ShowStartNr2 = true;
            this.CombBet.DataToShow.ShowVPQuota = true;
            this.CombBet.DataToShow.ShowStake = true;
            this.CombBet.DataToShow.ShowProfit = true;

            // Övriga kolumner
            this.CombBet.DataToShow.ShowStartNr3 = false;
            this.CombBet.DataToShow.ShowVQuota = false;
            this.CombBet.DataToShow.ShowPQuota = false;
            this.CombBet.DataToShow.ShowCombinationOddsRank = false;
            this.CombBet.DataToShow.ShowPlayability = false;
            this.CombBet.DataToShow.ShowStakeQuota = false;
            this.CombBet.DataToShow.ShowDQuota = false;
            this.CombBet.DataToShow.ShowMultipliedOdds = false;
            this.CombBet.DataToShow.ShowMultipliedOddsRank = false;
            this.CombBet.DataToShow.ShowOddsQuota = false;
            this.CombBet.DataToShow.ShowOPQuota = false;
            this.CombBet.DataToShow.ShowTVQuota = false;
        }

        internal void ResetCombinationDataToShowDouble()
        {
            // Standarduppsättningen av kolumner
            this.CombBet.DataToShow.ShowCombinationOdds = true;
            this.CombBet.DataToShow.ShowSelected = true;
            this.CombBet.DataToShow.ShowStartNr1 = true;
            this.CombBet.DataToShow.ShowStartNr2 = true;
            this.CombBet.DataToShow.ShowVQuota = true;
            this.CombBet.DataToShow.ShowStakeQuota = true;
            this.CombBet.DataToShow.ShowStake = true;
            this.CombBet.DataToShow.ShowProfit = true;

            // Övriga kolumner
            this.CombBet.DataToShow.ShowVPQuota = false;
            this.CombBet.DataToShow.ShowPQuota = false;
            this.CombBet.DataToShow.ShowCombinationOddsRank = false;
            this.CombBet.DataToShow.ShowPlayability = false;
            this.CombBet.DataToShow.ShowDQuota = false;
            this.CombBet.DataToShow.ShowMultipliedOdds = false;
            this.CombBet.DataToShow.ShowMultipliedOddsRank = false;
            this.CombBet.DataToShow.ShowOddsQuota = false;
            this.CombBet.DataToShow.ShowOPQuota = false;
            this.CombBet.DataToShow.ShowTVQuota = false;
            this.CombBet.DataToShow.ShowStartNr3 = false;
        }
    }
}

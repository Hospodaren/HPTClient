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
using System.Threading;
using System.Collections.ObjectModel;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCResultAnalyzerList.xaml
    /// </summary>
    public partial class UCResultAnalyzerList : UserControl
    {
        public UCResultAnalyzerList()
        {
            InitializeComponent();
        }

        public ObservableCollection<HPTResultAnalyzer> ResultAnalyzerList
        {
            get { return (ObservableCollection<HPTResultAnalyzer>)GetValue(ResultAnalyzerListProperty); }
            set { SetValue(ResultAnalyzerListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResultAnalyzerList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultAnalyzerListProperty =
            DependencyProperty.Register("ResultAnalyzerList", typeof(ObservableCollection<HPTResultAnalyzer>), typeof(UCResultAnalyzerList), new UIPropertyMetadata(new ObservableCollection<HPTResultAnalyzer>()));



        private void btnImportResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HPTResultAnalyzer.ResultAnalyzerAdded += HPTResultAnalyzer_ResultAnalyzerAdded;
                ThreadPool.QueueUserWorkItem(new WaitCallback(HPTResultAnalyzer.CreateResultAnalyzerList), ThreadPriority.Lowest);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue.GetType() == typeof(ObservableCollection<HPTResultAnalyzer>))
            {
                //foreach (var resultAnalyzer in HPTResultAnalyzer.ResultAnalyzerList)
                //{
                //    this.ResultAnalyzerList.Add(resultAnalyzer);
                //}

                //HPTResultAnalyzer.ResultAnalyzerAdded += new HPTResultAnalyzer.ResultAnalyzerAddedEventDelegate(HPTResultAnalyzer_ResultAnalyzerAdded);
                //var resultAnalyzerList = (ObservableCollection<HPTResultAnalyzer>)e.NewValue;
                //resultAnalyzerList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(resultAnalyzerList_CollectionChanged);
            }
        }

        void HPTResultAnalyzer_ResultAnalyzerAdded(IEnumerable<HPTHorse> horseList, HPTMarkBet markBet)
        {
            Dispatcher.Invoke(new Action<IEnumerable<HPTHorse>, HPTMarkBet>(AddResultAnalyzer), horseList, markBet);
            //HPTResultAnalyzer.ResultAnalyzerList.Add(resultAnalyzer);
        }

        void AddResultAnalyzer(IEnumerable<HPTHorse> horseList, HPTMarkBet markBet)
        {
            try
            {
                var resultAnalyzer = new HPTResultAnalyzer(horseList, markBet);
                HPTResultAnalyzer.ResultAnalyzerList.Add(resultAnalyzer);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        void resultAnalyzerList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                try
                {
                    Dispatcher.Invoke(new Action(UpdateBindings), null);
                }
                catch (Exception exc)
                {
                    try
                    {
                        UpdateBindings();
                    }
                    catch { }
                }
            }
        }

        void UpdateBindings()
        {
            try
            {
                BindingOperations.GetBindingExpression(this.lvwResultAnalyzerList, ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }            
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

            if (this.lvwResultAnalyzerList.Items.SortDescriptions.Count > 0)
            {
                SortDescription sd = this.lvwResultAnalyzerList.Items.SortDescriptions[0];
                if (sd.PropertyName == field)
                {
                    SortDescription sdNew = new SortDescription();
                    sdNew.PropertyName = sd.PropertyName;
                    sdNew.Direction = sd.Direction == ListSortDirection.Ascending
                                            ? ListSortDirection.Descending
                                            : ListSortDirection.Ascending;
                    this.lvwResultAnalyzerList.Items.SortDescriptions.Clear();
                    this.lvwResultAnalyzerList.Items.SortDescriptions.Add(sdNew);
                    return;
                }
            }

            // Aldrig sortering på mer än två variabler
            if (this.lvwResultAnalyzerList.Items.SortDescriptions.Count > 1)
            {
                this.lvwResultAnalyzerList.Items.SortDescriptions.RemoveAt(1); ;
            }
            this.lvwResultAnalyzerList.Items.SortDescriptions.Insert(0, new SortDescription(field, newDir));
        }

        private void btnExportToClipboard_Click(object sender, RoutedEventArgs e)
        {
            this.btnExportToClipboard.IsOpen = false;

            var sb = new StringBuilder();

            var resultAnalyzerGroupingList = HPTResultAnalyzer.ResultAnalyzerList.OrderBy(ra => ra.RaceDate).GroupBy(ra => ra.BetTypeCode);
            foreach (var resultAnalyzerList in resultAnalyzerGroupingList)
            {
                var firstResultAnalyzer = resultAnalyzerList.First();
                sb.Append(firstResultAnalyzer.BetTypeCode);
                sb.Append("\r\n\r\n");
                sb.Append(firstResultAnalyzer.ExportToExcel(true));
                sb.Append("\r\n");
                foreach (var resultAnalyzer in resultAnalyzerList.Except(new HPTResultAnalyzer[]{firstResultAnalyzer}))
                {
                    sb.Append(resultAnalyzer.ExportToExcel(false));
                    sb.Append("\r\n");
                }
                sb.Append("\r\n\r\n");
            }

            Clipboard.SetDataObject(sb.ToString());
        }

        private void miCompleteResults_Click(object sender, RoutedEventArgs e)
        {
            this.btnExportToClipboard.IsOpen = false;

            var sb = new StringBuilder();

            var resultAnalyzerGroupingList = HPTResultAnalyzer.ResultAnalyzerList.OrderBy(ra => ra.RaceDate).GroupBy(ra => ra.BetTypeCode);
            foreach (var resultAnalyzerList in resultAnalyzerGroupingList)
            {
                sb.Append(resultAnalyzerList.First().BetTypeCode);
                foreach (var resultAnalyzer in resultAnalyzerList)
                {
                    sb.Append(resultAnalyzer.ExportToExcel());
                    sb.Append("\r\n");
                }
                sb.Append("\r\n\r\n");
            }

            Clipboard.SetDataObject(sb.ToString());
        }


        //internal void CreateTreeview()
        //{
        //    var yearList = HPTResultAnalyzer.ResultAnalyzerList
        //        .Select(ra => ra.RaceDate.Year)
        //        .Distinct()
        //        .ToList();

        //    foreach (var year in yearList)
        //    {
        //        var tnYear = new TreeViewItem()
        //        {
        //            Header = year
        //        };
        //        var monthList = HPTResultAnalyzer.ResultAnalyzerList
        //            .Where(ra => ra.RaceDate.Year == year)
        //            .Select(ra => ra.RaceDate.Month)
        //            .Distinct()
        //            .ToList();

        //        foreach (var month in monthList)
        //        {
        //            var tnMonth = new TreeViewItem()
        //            {
        //                Header = month
        //            };
        //            tnYear.Items.Add(tnMonth);

        //            var dayList = HPTResultAnalyzer.ResultAnalyzerList
        //            .Where(ra => ra.RaceDate.Year == year && ra.RaceDate.Month == month)
        //            .Select(ra => ra.RaceDate.Day)
        //            .Distinct()
        //            .ToList();

        //            foreach (var day in dayList)
        //            {
        //                var tnDay = new TreeViewItem()
        //                {
        //                    Header = day
        //                };
        //                tnMonth.Items.Add(tnDay);

        //                var resultAnalyzerList = HPTResultAnalyzer.ResultAnalyzerList
        //                .Where(ra => ra.RaceDate.Year == year && ra.RaceDate.Month == month && ra.RaceDate.Day == day)
        //                .ToList();
        //            }
        //        }
        //    }
        //}
    }
}

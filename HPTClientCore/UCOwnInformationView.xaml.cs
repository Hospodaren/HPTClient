using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCOwnInformationView.xaml
    /// </summary>
    public partial class UCOwnInformationView : UserControl
    {
        public event Action<int, DateTime, string, int> RaceDayInfoSelected;

        public UCOwnInformationView()
        {
            InitializeComponent();
        }



        public HPTHorse SelectedOwnInformation
        {
            get { return (HPTHorse)GetValue(SelectedOwnInformationProperty); }
            set { SetValue(SelectedOwnInformationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedOwnInformation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedOwnInformationProperty =
            DependencyProperty.Register("SelectedOwnInformation", typeof(HPTHorse), typeof(UCOwnInformationView), new PropertyMetadata(null));



        private void btnGetNextStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var serviceConnector = new HPTServiceConnector();
                serviceConnector.GetHorseNextStartList();

                HPTConfig.Config.HorseOwnInformationCollection.CleanUpOldNextStarts();
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void hlSTHorseLinkExternal_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //https://sportapp.travsport.se/sportinfo/horse/ts771078/pedigree
            var hl = (Hyperlink)e.OriginalSource;
            GoToUrl(hl.NavigateUri.AbsoluteUri);
            //System.Diagnostics.Process.Start(hl.NavigateUri.AbsoluteUri);
        }

        private void GoToUrl(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            //var gd = new PropertyGroupDescription("ParentRace.LegNrString");
            var column = e.OriginalSource as GridViewColumnHeader;
            if (column == null)
            {
                return;
            }

            String field = column.Tag as String;
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            var newDir = ListSortDirection.Ascending;

            if (lvwOwnInformationList.Items.SortDescriptions.Count > 0)
            {
                var sd = lvwOwnInformationList.Items.SortDescriptions[0];
                if (sd.PropertyName == field)
                {
                    SortDescription sdNew = new SortDescription();
                    sdNew.PropertyName = sd.PropertyName;
                    sdNew.Direction = sd.Direction == ListSortDirection.Ascending
                                          ? ListSortDirection.Descending
                                          : ListSortDirection.Ascending;
                    lvwOwnInformationList.Items.SortDescriptions.Clear();
                    lvwOwnInformationList.Items.SortDescriptions.Add(sdNew);
                    return;
                }
            }

            // Hantera fält som ska sorteras fallande som default
            switch (field)
            {
                case "Markability":
                case "SelectedForComplimentaryRule":
                case "EarningsMeanLast5":
                case "TotalStatistics.Earning":
                case "TrioIndex":
                case "PercentFirst":
                case "PercentSecond":
                case "PercentThird":
                case "StakeDistributionShare":
                case "TrioInfo.PlaceInfo1.Investment":
                case "TrioInfo.PlaceInfo2.Investment":
                case "TrioInfo.PlaceInfo3.Investment":
                    newDir = ListSortDirection.Descending;
                    break;
                default:
                    break;
            }

            // Aldrig sortering på mer än två variabler
            if (lvwOwnInformationList.Items.SortDescriptions.Count > 1)
            {
                lvwOwnInformationList.Items.SortDescriptions.RemoveAt(1); ;
            }
            lvwOwnInformationList.Items.SortDescriptions.Insert(0, new SortDescription(field, newDir));
        }

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
            pu.IsOpen = false;
        }

        //private void txtDriverName_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton != MouseButton.Left
        //        || !HPTConfig.Config.IsPayingCustomer)
        //    {
        //        return;
        //    }
        //    TextBlock tb = (TextBlock)sender;
        //    HPTHorse horse = (HPTHorse)tb.DataContext;

        //    Border b = new Border()
        //    {
        //        BorderBrush = new SolidColorBrush(Colors.Black),
        //        BorderThickness = new Thickness(1D),
        //        Child = new UCRaceView()
        //        {
        //            BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
        //            BorderThickness = new Thickness(6D)
        //        }
        //    };

        //    this.PU.DataContext = horse.Driver;
        //    this.PU.Child = b;
        //    this.PU.IsOpen = true;
        //}

        #endregion

        private void btnSelectHorse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Välj häst och visa detaljer på sidan
                var fe = (FrameworkElement)sender;
                var ownInformation = (HPTHorseOwnInformation)fe.DataContext;
                SelectedOwnInformation = new HPTHorse()
                {
                    OwnInformation = ownInformation,
                    HorseName = ownInformation.Name,
                    ATGId = ownInformation.ATGId
                };
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kod för att ladda rätt spel på ny flik
                if (RaceDayInfoSelected != null)
                {
                    var feOriginal = (FrameworkElement)e.OriginalSource;
                    var betType = (HPTBetType)feOriginal.DataContext;

                    var feSender = (FrameworkElement)sender;
                    var ownInformation = (HPTHorseOwnInformation)feSender.DataContext;

                    RaceDayInfoSelected(ownInformation.NextStart.TrackId, ownInformation.NextStart.StartDate, betType.Code, ownInformation.NextStart.RaceNumber);
                }
            }
            catch (Exception)
            {
                // Fulläsning, alla klick verkar hamna här...
            }
        }

        private void Label_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hl = (Hyperlink)e.OriginalSource;
                Process.Start(hl.NavigateUri.ToString());
                e.Handled = true;
            }
            catch (Exception)
            {

            }
        }

        private void ucHorseCommentList_Loaded(object sender, RoutedEventArgs e)
        {
            // KOD HÄR...
        }

        private void chkShowOnlyWithNextStart_Checked(object sender, RoutedEventArgs e)
        {
            HandleOwnInformationFilter();
        }

        private void chkShowOnlyNextTimer_Checked(object sender, RoutedEventArgs e)
        {
            HandleOwnInformationFilter();
        }

        #region Filtrering

        private void HandleOwnInformationFilter()
        {
            var collectionView = CollectionViewSource.GetDefaultView(lvwOwnInformationList.ItemsSource);
            collectionView.Filter = new Predicate<object>(FilterOwnInformation);
        }

        public bool FilterOwnInformation(object obj)
        {
            var oi = obj as HPTHorseOwnInformation;
            bool showOwnInformation = true;
            if (chkShowOnlyWithNextStart.IsChecked == true)
            {
                showOwnInformation = oi.NextStart != null && oi.NextStart.StartDate > DateTime.Now;
            }
            if (showOwnInformation && chkShowOnlyNextTimer.IsChecked == true)
            {
                showOwnInformation = oi.NextTimer == true;
            }
            return showOwnInformation;
        }

        #endregion

        #region Borttag av egen info-objekt

        private void btnRemoveOwnInformation_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var ownInformation = (HPTHorseOwnInformation)fe.DataContext;
            var ownInformationsToRemove = new List<HPTHorseOwnInformation>() { ownInformation };

            RemoveNextTimersFromCollection("Ta bort " + ownInformation.Name + "?", ownInformationsToRemove);
        }

        private void btnRemoveWithoutNextStart_Click(object sender, RoutedEventArgs e)
        {
            var ownInformationsToRemove = HPTConfig.Config.HorseOwnInformationCollection.HorseOwnInformationList
                .Where(oi => oi.NextStart == null || oi.NextStart.StartDate < DateTime.Now.AddDays(-7D));

            RemoveNextTimersFromCollection("Ta bort nästagångare som inte har någon aktuell start?", ownInformationsToRemove);
        }

        private void btnRemoveOldNextTimers_Click(object sender, RoutedEventArgs e)
        {
            var ownInformationsToRemove = HPTConfig.Config.HorseOwnInformationCollection.HorseOwnInformationList
                .Where(oi => oi.LastUpdate < DateTime.Now.AddDays(-28D));

            RemoveNextTimersFromCollection("Ta bort nästagångare som inte har någon aktuell start?", ownInformationsToRemove);
        }

        private void RemoveNextTimersFromCollection(string question, IEnumerable<HPTHorseOwnInformation> ownInformationsToRemove)
        {
            var result = MessageBox.Show(question, "Bekräfta borttag", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.None);
            if (result == MessageBoxResult.Yes)
            {
                ownInformationsToRemove
                    .ToList()
                    .ForEach(oi => HPTConfig.Config.HorseOwnInformationCollection.HorseOwnInformationList.Remove(oi));
            }
        }

        #endregion

        private void chkNextTimer_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

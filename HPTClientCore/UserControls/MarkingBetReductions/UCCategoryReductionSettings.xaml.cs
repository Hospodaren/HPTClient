using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCABCDReductionSettings.xaml
    /// </summary>
    public partial class UCCategoryReductionSettings : UCMarkBetControl
    {
        public UCCategoryReductionSettings()
        {
            InitializeComponent();

        }

        public Visibility ShowLegList
        {
            get { return (Visibility)GetValue(ShowLegListProperty); }
            set { SetValue(ShowLegListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLegList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLegListProperty =
            DependencyProperty.Register("ShowLegList", typeof(Visibility), typeof(UCCategoryReductionSettings), new PropertyMetadata(Visibility.Collapsed));




        public HPTHorseListContainer HorseListContainer
        {
            get { return (HPTHorseListContainer)GetValue(HorseListContainerProperty); }
            set { SetValue(HorseListContainerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorseListContainer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorseListContainerProperty =
            DependencyProperty.Register(nameof(HorseListContainer), typeof(HPTHorseListContainer), typeof(UCCategoryReductionSettings), new PropertyMetadata(null));




        private HPTCategoryReductionRuleCollection categoryReductionRuleCollection;
        internal HPTCategoryReductionRuleCollection CategoryReductionRuleCollection
        {
            get
            {
                if (categoryReductionRuleCollection == null)
                {
                    categoryReductionRuleCollection = (HPTCategoryReductionRuleCollection)DataContext;
                }
                return categoryReductionRuleCollection;
            }
        }

        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded && MarkBet != null)
            {
                if (!MarkBet.IsDeserializing)
                {
                    MarkBet.RecalculateReduction(RecalculateReason.All);
                }
            }
        }

        //private System.Windows.Controls.Primitives.Popup pu;
        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            var tb = (TextBlock)sender;
            var rule = (HPTCategoryReductionRule)tb.DataContext;
            //if (pu == null)
            //{
            //    pu = new System.Windows.Controls.Primitives.Popup()
            //    {
            //        Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
            //        PlacementTarget = tb
            //    };
            //    pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
            //}

            // Skapa innehållet för popupen
            Border b = new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1D),
                Width = double.NaN,
                Child = new UCRaceView()
                {
                    BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                    BorderThickness = new Thickness(6D),
                    Width = double.NaN
                }
            };

            // Plocka ut hästarna med rätt Prio
            var orderedHorseList = MarkBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.CategoryCode.HasFlag(rule.CategoryCode))
                .OrderBy(h => h.ParentRace.LegNr)
                .ThenBy(h => h.StartNr);

            if (rule.OnlyInSpecifiedLegs)
            {
                orderedHorseList = orderedHorseList
                    .Where(h => rule.LegSelectionList.First(l => l.LegNumber == h.ParentRace.LegNr).Selected)
                    .OrderBy(h => h.ParentRace.LegNr)
                    .ThenBy(h => h.StartNr); ;
            }

            // Skapa en IHorseListContainer med valda hästar
            var horseCollection = new HPTHorseListContainer()
            {
                //HorseList = new System.Collections.ObjectModel.ObservableCollection<HPTHorse>(orderedHorseList),
                HorseList = new List<HPTHorse>(orderedHorseList),
                ParentRaceDayInfo = new HPTRaceDayInfo()
                {
                    DataToShow = new HPTHorseDataToShow()
                    {
                        ShowLegNrText = true,
                        ShowStartNr = true,
                        ShowName = true,
                        ShowPrio = true,
                        ShowVinnarOdds = true,
                        ShowStakeDistributionPercent = true,
                        ShowStakeShareRelativeToFavourite = true,
                        ShowStakeShareRelativeToNext = true,
                        ShowATGTrend = true,
                        ShowTrends = true,
                    }
                }
            };

            HorseListContainer = horseCollection;
            gbCategoryRaceView.Header = rule.CategoryCode.GetString();

            //// Visa popupen
            //pu.DataContext = horseCollection;
            //pu.Child = b;
            //pu.IsOpen = true;
        }

        private void chkUseABCDRule_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                MarkBet.RecalculateReduction(RecalculateReason.XReduction);
            }
        }

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup pu = (System.Windows.Controls.Primitives.Popup)sender;
            pu.Child = null;
            pu.IsOpen = false;
        }
    }
}

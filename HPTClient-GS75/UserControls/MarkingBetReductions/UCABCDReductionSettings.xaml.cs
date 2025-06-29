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

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCABCDReductionSettings.xaml
    /// </summary>
    public partial class UCABCDReductionSettings : UCMarkBetControl
    {
        public UCABCDReductionSettings()
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
            DependencyProperty.Register("ShowLegList", typeof(Visibility), typeof(UCABCDReductionSettings), new PropertyMetadata(Visibility.Collapsed));



        private HPTABCDEFReductionRule abcdefreductionRule;
        internal HPTABCDEFReductionRule ABCDEFreductionRule
        {
            get
            {
                if (this.abcdefreductionRule == null)
                {
                    this.abcdefreductionRule = (HPTABCDEFReductionRule) this.DataContext;
                }
                return this.abcdefreductionRule;
            }
        }
        
        private void ItemsControl_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded && this.MarkBet != null)
            {
                if (!this.MarkBet.IsDeserializing)
                {
                    if (this.MarkBet.MultiABCDEFReductionRule.Use && this.ABCDEFreductionRule.Use)
                    {
                        this.MarkBet.RecalculateReduction(RecalculateReason.XReduction);
                    }
                    else if (this.ABCDEFreductionRule == this.MarkBet.ABCDEFReductionRule && this.MarkBet.ABCDEFReductionRule.Use)
                    {
                        this.MarkBet.RecalculateReduction(RecalculateReason.XReduction);
                    }
                }
            }
        }

        private System.Windows.Controls.Primitives.Popup pu;
        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            TextBlock tb = (TextBlock)sender;
            HPTXReductionRule rule = (HPTXReductionRule)tb.DataContext;
            if (this.pu == null)
            {
                this.pu = new System.Windows.Controls.Primitives.Popup()
                {
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                    PlacementTarget = tb//,
                    //HorizontalOffset = -10D,
                    //VerticalOffset = -10D
                };
                this.pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
            }

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
            var orderedHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.Prio == rule.Prio)
                .OrderBy(h => h.ParentRace.LegNr)
                .ThenBy(h => h.StartNr);

            if (rule.OnlyInSpecifiedLegs)
            {
                orderedHorseList = orderedHorseList
                    .Where(h => rule.LegSelectionList.First(l => l.LegNumber == h.ParentRace.LegNr).Selected)
                    .OrderBy(h => h.ParentRace.LegNr)
                    .ThenBy(h => h.StartNr); ;
            }

            //// TEST AV BERÄKNING
            //string result = this.MarkBet.CalculateBestABCDCombination(orderedHorseList);
            //Clipboard.SetText(result);

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
                        ShowMarksPercent = true
                    }
                }
            };

            // Visa popupen
            this.pu.DataContext = horseCollection;
            this.pu.Child = b;
            this.pu.IsOpen = true;
        }

        private void chkUseABCDRule_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.MarkBet.RecalculateReduction(RecalculateReason.XReduction);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarkBetWarnings.xaml
    /// </summary>
    public partial class UCMarkBetWarnings : UCMarkBetControl
    {
        public UCMarkBetWarnings()
        {
            InitializeComponent();
        }

        private void btnIgnoreReservChoice_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.NoReservChoiceMade = false;
        }

        private System.Windows.Controls.Primitives.Popup pu;
        private void btnShowUncoveredHorses_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            if (this.pu == null)
            {
                this.pu = new System.Windows.Controls.Primitives.Popup()
                {
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                    PlacementTarget = fe//,
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
            IOrderedEnumerable<HPTHorse> orderedHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.NumberOfCoveredRows == 0)
                .OrderBy(h => h.ParentRace.LegNr)
                .ThenBy(h => h.StartNr);

            // Skapa en IHorseListContainer med valda hästar
            HPTHorseListContainer horseCollection = new HPTHorseListContainer()
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
                        ShowSystemCoverage = true
                    }
                }
            };

            // Visa popupen
            this.pu.DataContext = horseCollection;
            this.pu.Child = b;
            this.pu.IsOpen = true;
        }

        private void btnIgnoreUncoveredHorses_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.HasUncoveredHorses = false;
        }

        private void btnDeselectUncoveredHorses_Click(object sender, RoutedEventArgs e)
        {
            bool pauseRecalculation = this.MarkBet.pauseRecalculation;
            this.MarkBet.pauseRecalculation = true;
            try
            {
                foreach (var horse in this.MarkBet.HorseListUncovered)
                {
                    horse.Selected = false;
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            this.MarkBet.pauseRecalculation = pauseRecalculation;
            this.MarkBet.RecalculateReduction(RecalculateReason.All);
        }

        private void btnIgnoreOverlappingHorses_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.HasOverlappingComplementaryRuleHorses = false;
        }

        private void btnShowOverlappingHorses_Click(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            if (this.pu == null)
            {
                this.pu = new System.Windows.Controls.Primitives.Popup()
                {
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Top,
                    PlacementTarget = fe//,
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
            IOrderedEnumerable<HPTHorse> orderedHorseList = this.MarkBet.RaceDayInfo.HorseListSelected
                .Where(h => h.NumberOfCoveredRows == 0)
                .OrderBy(h => h.ParentRace.LegNr)
                .ThenBy(h => h.StartNr);

            // Skapa en IHorseListContainer med valda hästar
            HPTHorseListContainer horseCollection = new HPTHorseListContainer()
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
                        ShowSystemCoverage = true
                    }
                }
            };

            // Visa popupen
            this.pu.DataContext = horseCollection;
            this.pu.Child = b;
            this.pu.IsOpen = true;
        }

        private void btnDeSelectScratchedHorses_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.RaceDayInfo.ScratchedHorseInfo.DeSelectAll();
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                race.NumberOfSelectedHorses = race.HorseListSelected.Count;
            }
        }

        private void btnIgnorScratchedHorses_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.RaceDayInfo.ScratchedHorseInfo.HaveSelectedScratchedHorse = false;
        }

        private void btnCloseSuperfluousXReduction_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.HasSuperfluousXReduction = false;
        }

        private void btnIgnoreTooManyCoupons_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.HasTooManySystems = false;
        }

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup pu = (System.Windows.Controls.Primitives.Popup)sender;
            pu.Child = null;
            pu.IsOpen = false;
        }
    }
}

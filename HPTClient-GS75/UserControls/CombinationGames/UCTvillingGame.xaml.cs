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
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.ComponentModel;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTvillingGame.xaml
    /// </summary>
    public partial class UCTvillingGame : UCCombBetControl
    {
        public ObservableCollection<HPTRace> RaceList { get; set; }
        public HPTRace Race { get; set; }

        public UCTvillingGame()
        {
            InitializeComponent();
        }

        public UCTvillingGame(HPTCombBet combBet)
        {
            this.CombBet = combBet;
            this.RaceList = new ObservableCollection<HPTRace>(combBet.RaceDayInfo.RaceList);

            foreach (HPTRace hptRace in this.RaceList)
            {
                int numberOfSelectedHorses = hptRace.HorseList.Count(h => h.Selected);

                if (numberOfSelectedHorses == 0)
                {
                    foreach (HPTHorse horse in hptRace.HorseList.Where(h => h.Scratched == null ||h.Scratched == false))
                    {
                        horse.Selected = true;
                    }
                }
                hptRace.CombinationListInfoTvilling.UpdateCombinationsToShow();
                hptRace.NumberOfSelectedChanged += race_NumberOfSelectedChanged;
            }

            InitializeComponent();
        }

        void race_NumberOfSelectedChanged(int raceNr, int startNr, bool selected)
        {
            var race = this.RaceList.FirstOrDefault(r => r.RaceNr == raceNr);
            if (race != null)
            {
                race.CombinationListInfoTvilling.UpdateCombinationsToShow();
            }

            //if (horse.ParentRace == null)
            //{
            //    return;
            //}
            //horse.ParentRace.CombinationListInfoTvilling.UpdateCombinationsToShow();

            //HPTRace hptRace = horse.ParentRace;
            //for (int i = 0; i < hptRace.HorseList.Count - 1; i++)
            //{
            //    HPTHorse horse1 = hptRace.HorseList[i];
            //    if (horse1.Selected)
            //    {
            //        for (int j = i + 1; j < hptRace.HorseList.Count; j++)
            //        {
            //            HPTHorse horse2 = hptRace.HorseList[j];
            //            if (horse2.Selected)
            //            {
            //                string uniqueCode = horse1.HexCode + horse2.HexCode;
            //                HPTCombination hptComb = hptRace.CombinationListInfoTvilling.CombinationList.Where(c => c.UniqueCode == uniqueCode).FirstOrDefault();
            //            }
            //        }
            //    }
            //}
        }

        private void tcTvillingGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    if (e.OriginalSource.GetType() == typeof(TabControl))
            //    {
            //        HPTRace hptRace = (HPTRace)e.AddedItems[0];
            //        this.Race = hptRace;
            //        race_NumberOfSelectedChanged(hptRace.HorseList[0], new EventArgs());
            //    }
            //}
            //catch (Exception exc)
            //{
            //    string s = exc.Message;
            //}
        }

        #region Timer update handling

        HPTRace upcomingRace;
        internal void SetCountDown()
        {
            // Återställ textinställningar
            this.txtCountdownTimer.FontWeight = FontWeights.Normal;
            this.txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Black);

            // Hämta nästa lopp
            this.upcomingRace = this.CombBet.RaceDayInfo.RaceList
                .OrderBy(r => r.PostTime)
                .FirstOrDefault(r => r.PostTime > DateTime.Now);

            // Dra igång nedräknare om tävlingen är idag
            if (this.upcomingRace != null && this.upcomingRace.PostTime.Date == DateTime.Today)
            {
                TimeSpan ts = this.upcomingRace.PostTime - DateTime.Now;
                Countdown(ts, cur =>
                {
                    if ((int)cur.TotalSeconds == 600)
                    {
                        this.txtCountdownTimer.FontWeight = FontWeights.Bold;
                        this.txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    this.txtCountdownTimer.Text = cur.ToString(@"hh\:mm\:ss");
                });
                this.txtCountdownInfo.Text = this.upcomingRace.LegNrString + ":";
            }
            else
            {
                this.txtCountdownTimer.Text = string.Empty;
                this.txtCountdownInfo.Text = string.Empty;
            }
        }

        void Countdown(TimeSpan timeLeft, Action<TimeSpan> ts)
        {
            int count = (int)timeLeft.TotalSeconds;
            var dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1D);
            dt.Tick += (_, a) =>
            {
                TimeSpan tsTemp = this.upcomingRace.PostTime - DateTime.Now;

                if (tsTemp.TotalSeconds < 1D)
                {
                    dt.Stop();
                    dt = null;
                    SetCountDown();
                }
                else
                {
                    ts(tsTemp);
                }
            };
            ts(timeLeft);
            dt.Start();
        }

        #endregion

        private void ucTvillingGame_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.CombBet != null && this.CombBet.RaceDayInfo != null && this.CombBet.RaceNumberToLoad > 0)
            {
                SelectTabItemFRomRaceNumber(this.CombBet.RaceNumberToLoad);
            }
        }

        internal void SelectTabItemFRomRaceNumber(int raceNumber)
        {
            try
            {
                var race = this.CombBet.RaceDayInfo.RaceList.FirstOrDefault(r => r.RaceNr == raceNumber);
                if (race != null)
                {
                    this.tcTvillingGame.SelectedItem = race;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

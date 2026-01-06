using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            CombBet = combBet;
            RaceList = new ObservableCollection<HPTRace>(combBet.RaceDayInfo.RaceList);

            foreach (HPTRace hptRace in RaceList)
            {
                int numberOfSelectedHorses = hptRace.HorseList.Count(h => h.Selected);

                if (numberOfSelectedHorses == 0)
                {
                    foreach (HPTHorse horse in hptRace.HorseList.Where(h => h.Scratched == null || h.Scratched == false))
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
            var race = RaceList.FirstOrDefault(r => r.RaceNr == raceNr);
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
            txtCountdownTimer.FontWeight = FontWeights.Normal;
            txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Black);

            // Hämta nästa lopp
            upcomingRace = CombBet.RaceDayInfo.RaceList
                .OrderBy(r => r.PostTime)
                .FirstOrDefault(r => r.PostTime > DateTime.Now);

            // Dra igång nedräknare om tävlingen är idag
            if (upcomingRace != null && upcomingRace.PostTime.Date == DateTime.Today)
            {
                TimeSpan ts = upcomingRace.PostTime - DateTime.Now;
                Countdown(ts, cur =>
                {
                    if ((int)cur.TotalSeconds == 600)
                    {
                        txtCountdownTimer.FontWeight = FontWeights.Bold;
                        txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    txtCountdownTimer.Text = cur.ToString(@"hh\:mm\:ss");
                });
                txtCountdownInfo.Text = upcomingRace.LegNrString + ":";
            }
            else
            {
                txtCountdownTimer.Text = string.Empty;
                txtCountdownInfo.Text = string.Empty;
            }
        }

        void Countdown(TimeSpan timeLeft, Action<TimeSpan> ts)
        {
            int count = (int)timeLeft.TotalSeconds;
            var dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1D);
            dt.Tick += (_, a) =>
            {
                TimeSpan tsTemp = upcomingRace.PostTime - DateTime.Now;

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
            if (CombBet != null && CombBet.RaceDayInfo != null && CombBet.RaceNumberToLoad > 0)
            {
                SelectTabItemFRomRaceNumber(CombBet.RaceNumberToLoad);
            }
        }

        internal void SelectTabItemFRomRaceNumber(int raceNumber)
        {
            try
            {
                var race = CombBet.RaceDayInfo.RaceList.FirstOrDefault(r => r.RaceNr == raceNumber);
                if (race != null)
                {
                    tcTvillingGame.SelectedItem = race;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

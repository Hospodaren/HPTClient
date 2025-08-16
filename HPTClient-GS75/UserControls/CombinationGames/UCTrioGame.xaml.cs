using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTrioGame.xaml
    /// </summary>
    public partial class UCTrioGame : UCCombBetControl
    {
        public ObservableCollection<HPTRace> RaceList { get; set; }
        public HPTRace Race { get; set; }

        public UCTrioGame()
        {
            InitializeComponent();
        }

        public UCTrioGame(HPTCombBet combBet)
        {
            this.CombBet = combBet;
            this.Race = combBet.RaceDayInfo.RaceList[0];
            this.RaceList = new ObservableCollection<HPTRace>(combBet.RaceDayInfo.RaceList);

            foreach (HPTRace race in this.CombBet.RaceDayInfo.RaceList)
            {
                int numberOfSelectedHorses = race.HorseList.Count(h => h.Selected);
                if (numberOfSelectedHorses == 0)
                {
                    foreach (HPTHorse horse in race.HorseList.Where(h => h.Scratched == null || h.Scratched == false))
                    {
                        if (horse.VinnarOdds < 300 || horse.TrioInfo.PlaceInfo3.InvestmentShare > 0.05M)
                        {
                            horse.Selected = true;
                            horse.TrioInfo.PlaceInfo3.Selected = true;
                            if (horse.VinnarOdds < 200 || horse.TrioInfo.PlaceInfo2.InvestmentShare > 0.1M)
                            {
                                horse.TrioInfo.PlaceInfo2.Selected = true;
                                if (horse.VinnarOdds < 100 || horse.TrioInfo.PlaceInfo1.InvestmentShare > 0.2M)
                                {
                                    horse.TrioInfo.PlaceInfo1.Selected = true;
                                }
                            }
                        }
                    }
                }
                UpdateSelectedCombinations(race);
                race.NumberOfSelectedChanged += race_NumberOfSelectedChanged;
            }

            InitializeComponent();

            // Hantering av gratisanvändare som öppnar fil med Trio
            if (!HPTConfig.Config.IsPayingCustomer)
            {
                //this.btnSave.IsEnabled = false;
                //this.btnSaveAs.IsEnabled = false;
                //this.btnCreateCouponsAll.IsEnabled = false;
                //this.btnCreateCouponsAllAs.IsEnabled = false;
                //this.btnUpdateAll.IsEnabled = false;
            }
        }

        //private bool firstLoad = true;
        private void ucTrioGame_Loaded(object sender, RoutedEventArgs e)
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
                    this.tcTrioGame.SelectedItem = race;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        void race_TrioSelectionChanged(HPTHorse horse, EventArgs e)
        {
            if (horse.TrioInfo.PlaceInfo1.Selected || horse.TrioInfo.PlaceInfo2.Selected || horse.TrioInfo.PlaceInfo3.Selected)
            {
                if (!horse.Selected)
                {
                    horse.Selected = true;
                }
            }
            if (horse.Selected && !horse.TrioInfo.PlaceInfo1.Selected && !horse.TrioInfo.PlaceInfo2.Selected && !horse.TrioInfo.PlaceInfo3.Selected)
            {
                horse.Selected = false;
            }
            if (horse.ParentRace != null)
            {
                UpdateSelectedCombinations(horse.ParentRace);
            }
        }

        void UpdateSelectedCombinations(HPTRace race)
        {
            this.Race.CombinationListInfoTrio.UpdateCombinationsToShow();
        }

        void race_NumberOfSelectedChanged(int raceNr, int startNr, bool selected)
        {
            var race = this.RaceList.FirstOrDefault(r => r.RaceNr == raceNr);
            if (race != null)
            {
                var horse = race.HorseList.FirstOrDefault(h => h.StartNr == startNr);
                race.TrioSelectionChanged -= race_TrioSelectionChanged;
                horse.TrioInfo.PlaceInfo1.Selected = horse.Selected;
                horse.TrioInfo.PlaceInfo2.Selected = horse.Selected;
                horse.TrioInfo.PlaceInfo3.Selected = horse.Selected;
                UpdateSelectedCombinations(race);
                race.TrioSelectionChanged += race_TrioSelectionChanged;
            }
        }

        private void tcTrioGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    if (e.OriginalSource.GetType() == typeof(TabControl))
            //    {
            //        HPTRace hptRace = (HPTRace)e.AddedItems[0];
            //        this.Race = hptRace;
            //        UpdateSelectedCombinations(hptRace);
            //        hptRace.CombinationListInfoTrio.SetStakeAndNumberOfSelected();
            //    }
            //}
            //catch (Exception exc)
            //{
            //    string s = exc.Message;
            //}
        }

        private void UCRaceView_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var chk = (CheckBox)e.OriginalSource;
                //if (chk.Name == "chkNrSelect" || chk.Name == "chkTrio1" || chk.Name == "chkTrio2" || chk.Name == "chkTrio3")
                if (chk.Name == "chkNrSelect" || chk.Name == "chkTrioPlace")
                {
                    UpdateSelectedCombinations(this.Race);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                throw;
            }
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
    }
}

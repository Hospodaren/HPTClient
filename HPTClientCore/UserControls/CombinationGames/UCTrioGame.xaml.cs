using System.Collections.ObjectModel;
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
            CombBet = combBet;
            Race = combBet.RaceDayInfo.RaceList[0];
            RaceList = new ObservableCollection<HPTRace>(combBet.RaceDayInfo.RaceList);

            foreach (HPTRace race in CombBet.RaceDayInfo.RaceList)
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
                    tcTrioGame.SelectedItem = race;
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
            Race.CombinationListInfoTrio.UpdateCombinationsToShow();
        }

        void race_NumberOfSelectedChanged(int raceNr, int startNr, bool selected)
        {
            var race = RaceList.FirstOrDefault(r => r.RaceNr == raceNr);
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
                    UpdateSelectedCombinations(Race);
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
    }
}

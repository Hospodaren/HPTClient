﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCDoubleGame.xaml
    /// </summary>
    public partial class UCDoubleGame : UCCombBetControl
    {
        //public HPTRace Race1 { get; set; }
        //public HPTRace Race2 { get; set; }
        public UCDoubleGame()
        {

            InitializeComponent();
        }

        public UCDoubleGame(HPTCombBet combBet)
        {
            this.CombBet = combBet;
            this.RaceList = new ObservableCollection<HPTRace>(combBet.RaceDayInfo.RaceList);
            this.Race1 = combBet.RaceDayInfo.RaceList[0];
            this.Race2 = combBet.RaceDayInfo.RaceList[1];

            var selectedHorses = this.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.Selected)
                .ToList();

            if (selectedHorses.Count == 0)
            {
                this.RaceList.ToList().ForEach(r =>
                {
                    r.HorseList
                        .Where(h => h.Scratched == null || h.Scratched == false)
                        .ToList()
                        .ForEach(h => h.Selected = true);

                    r.NumberOfSelectedChanged += race_NumberOfSelectedChanged;
                });

                //foreach (HPTRace race in this.RaceList)
                //{
                //    foreach (HPTHorse horse in race.HorseList)
                //    {
                //        horse.Selected = true;
                //    }
                //    race.NumberOfSelectedChanged += new HPTRace.NumberOfSelectedChangedEventDelegate(race_NumberOfSelectedChanged);
                //}
            }
            this.CombBet.RaceDayInfo.CombinationListInfoDouble.UpdateCombinationsToShow();
            InitializeComponent();

            //// Hantering av gratisanvändare som öppnar fil med icke stödda spelformer
            //if (!HPTConfig.Config.IsPayingCustomer)
            //{
            //    this.btnCreateCoupons.IsEnabled = false;
            //    this.btnCreateCouponsAs.IsEnabled = false;
            //    this.btnUpdate.IsEnabled = false;
            //}
        }

        public ObservableCollection<HPTRace> RaceList { get; set; }

        public HPTRace Race1
        {
            get { return (HPTRace)GetValue(Race1Property); }
            set { SetValue(Race1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Race1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Race1Property =
            DependencyProperty.Register("Race1", typeof(HPTRace), typeof(UCDoubleGame), new PropertyMetadata(null));


        public HPTRace Race2
        {
            get { return (HPTRace)GetValue(Race2Property); }
            set { SetValue(Race2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Race2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Race2Property =
            DependencyProperty.Register("Race2", typeof(HPTRace), typeof(UCDoubleGame), new PropertyMetadata(null));



        void race_NumberOfSelectedChanged(int raceNr, int startNr, bool selected)
        {
            //if (horse.ParentRace == null || horse.ParentRace.ParentRaceDayInfo == null)
            //{
            //    return;
            //}
            //horse.ParentRace.ParentRaceDayInfo.CombinationListInfoDouble.UpdateCombinationsToShow();
            this.CombBet.RaceDayInfo.CombinationListInfoDouble.UpdateCombinationsToShow();
        }

        private bool firstLoad = true;
        private void ucDoubleGame_Loaded(object sender, RoutedEventArgs e)
        {
            SetCountDown();
        }

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
                if (this.upcomingRace == null)
                {
                    dt.Stop();
                    dt = null;
                    SetCountDown();
                }
                else
                {
                    TimeSpan tsTemp = this.upcomingRace.PostTime - DateTime.Now;
                    ts(tsTemp);
                }
            };
            ts(timeLeft);
            dt.Start();
        }

    }
}

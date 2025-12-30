using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTvillingRace.xaml
    /// </summary>
    public partial class UCTvillingTrioRace : UserControl
    {
        public UCTvillingTrioRace()
        {
            InitializeComponent();
        }

        #region Timerhantering

        //private bool firstLoad = true;
        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            SetCountDown();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.race = null;
            if (this.DataContext != null && this.DataContext.GetType() == typeof(HPTRace))
            {
                this.race = this.DataContext as HPTRace;
            }
        }

        private HPTRace race;
        private HPTRace Race
        {
            get
            {
                if (this.race == null)
                {
                    this.race = this.DataContext as HPTRace;
                }
                return this.race;
            }
        }
        internal void SetCountDown()
        {
            // Återställ textinställningar
            this.txtCountdownTimer.FontWeight = FontWeights.Normal;
            this.txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Black);

            // Dra igång nedräknare om tävlingen är idag
            if (this.Race != null && this.Race.PostTime.Date == DateTime.Today)
            {
                TimeSpan ts = this.Race.PostTime - DateTime.Now;
                Countdown(ts, cur =>
                {
                    if ((int)cur.TotalSeconds < 600)
                    {
                        this.txtCountdownTimer.FontWeight = FontWeights.Bold;
                        this.txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        this.txtCountdownTimer.FontWeight = FontWeights.Normal;
                        this.txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Black);
                    }

                    // Ska texten sättas?
                    if ((int)cur.TotalSeconds < 0)
                    {
                        this.txtCountdownTimer.Text = string.Empty;
                        this.txtCountdownInfo.Text = string.Empty;
                    }
                    else
                    {
                        this.txtCountdownTimer.Text = cur.ToString(@"hh\:mm\:ss");
                        this.txtCountdownInfo.Text = "Start om:";
                    }
                });
            }
            else
            {
                this.txtCountdownTimer.Text = string.Empty;
                this.txtCountdownInfo.Text = string.Empty;
            }
        }

        void Countdown(TimeSpan timeLeft, Action<TimeSpan> ts)
        {
            try
            {
                int count = (int)timeLeft.TotalSeconds;
                var dt = new System.Windows.Threading.DispatcherTimer();
                dt.Interval = TimeSpan.FromSeconds(1D);
                dt.Tick += (_, a) =>
                {
                    TimeSpan tsTemp = this.Race.PostTime - DateTime.Now;

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
            catch (Exception)
            {

            }
        }

        #endregion
    }
}

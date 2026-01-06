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
            race = null;
            if (DataContext != null && DataContext.GetType() == typeof(HPTRace))
            {
                race = DataContext as HPTRace;
            }
        }

        private HPTRace race;
        private HPTRace Race
        {
            get
            {
                if (race == null)
                {
                    race = DataContext as HPTRace;
                }
                return race;
            }
        }
        internal void SetCountDown()
        {
            // Återställ textinställningar
            txtCountdownTimer.FontWeight = FontWeights.Normal;
            txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Black);

            // Dra igång nedräknare om tävlingen är idag
            if (Race != null && Race.PostTime.Date == DateTime.Today)
            {
                TimeSpan ts = Race.PostTime - DateTime.Now;
                Countdown(ts, cur =>
                {
                    if ((int)cur.TotalSeconds < 600)
                    {
                        txtCountdownTimer.FontWeight = FontWeights.Bold;
                        txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        txtCountdownTimer.FontWeight = FontWeights.Normal;
                        txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Black);
                    }

                    // Ska texten sättas?
                    if ((int)cur.TotalSeconds < 0)
                    {
                        txtCountdownTimer.Text = string.Empty;
                        txtCountdownInfo.Text = string.Empty;
                    }
                    else
                    {
                        txtCountdownTimer.Text = cur.ToString(@"hh\:mm\:ss");
                        txtCountdownInfo.Text = "Start om:";
                    }
                });
            }
            else
            {
                txtCountdownTimer.Text = string.Empty;
                txtCountdownInfo.Text = string.Empty;
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
                    TimeSpan tsTemp = Race.PostTime - DateTime.Now;

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

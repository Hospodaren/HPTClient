using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCV6BetMultiplierRule.xaml
    /// </summary>
    public partial class UCV6BetMultiplierRule : UCMarkBetControl
    {
        public UCV6BetMultiplierRule()
        {
            InitializeComponent();
        }

        private HPTV6BetMultiplierRule v6BetMultiplierRule;
        internal HPTV6BetMultiplierRule V6BetMultiplierRule
        {
            get
            {
                if (v6BetMultiplierRule == null && DataContext != null && DataContext.GetType() == typeof(HPTV6BetMultiplierRule))
                {
                    v6BetMultiplierRule = (HPTV6BetMultiplierRule)DataContext;
                    if (MarkBet == null)
                    {
                        MarkBet = v6BetMultiplierRule.MarkBet;
                    }
                }
                return v6BetMultiplierRule;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearRule();
        }

        private void ClearRule()
        {
            foreach (var horse in V6BetMultiplierRule.RaceList.SelectMany(r => r.HorseList))
            {
                horse.Selected = false;
            }
            foreach (var raceLight in V6BetMultiplierRule.RaceList)
            {
                raceLight.SelectedHorse = null;
            }
            V6BetMultiplierRule.Use = false;
            V6BetMultiplierRule.V6 = false;
            V6BetMultiplierRule.BetMultiplier = 1;
            V6BetMultiplierRule.HorseList.Clear();
            MarkBet.UpdateV6BetMultiplierSingleRows();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ClearRule();
            MarkBet.UpdateV6BetMultiplierSingleRows();
            MarkBet.V6BetMultiplierRuleList.Remove(V6BetMultiplierRule);
        }

        private void chkUseRule_Checked(object sender, RoutedEventArgs e)
        {
            if (MarkBet != null)
            {
                MarkBet.UpdateV6BetMultiplierSingleRows();
                //this.MarkBet.SetV6BetMultiplierSingleRows();
            }
        }

        private void lst_Checked(object sender, RoutedEventArgs e)
        {
            if (V6BetMultiplierRule != null && V6BetMultiplierRule.Use && MarkBet != null)
            {
                MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void cmbBetMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (V6BetMultiplierRule != null && V6BetMultiplierRule.Use && MarkBet != null)
            {
                MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void chkV6_Checked(object sender, RoutedEventArgs e)
        {
            if (V6BetMultiplierRule != null && V6BetMultiplierRule.Use && MarkBet != null)
            {
                MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void iudBetMultiplier_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (V6BetMultiplierRule != null && V6BetMultiplierRule.Use && MarkBet != null)
            {
                MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void AddEventHandlers()
        {
            chkUseRule.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            chkUseRule.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            if (chkV6 != null && chkV6.Visibility == Visibility.Visible)
            {
                chkV6.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkV6_Checked));
                chkV6.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkV6_Checked));
            }
            lst.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(lst_Checked));
            iudBetMultiplier.AddHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(iudBetMultiplier_ValueChanged));
        }

        private void RemoveEventHandlers()
        {
            chkUseRule.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            chkUseRule.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            if (chkV6 != null && chkV6.Visibility == Visibility.Visible)
            {
                chkV6.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkV6_Checked));
                chkV6.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkV6_Checked));
            }
            lst.RemoveHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(lst_Checked));
            iudBetMultiplier.RemoveHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(iudBetMultiplier_ValueChanged));

        }

        private void ucV6BetMultiplierRule_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && IsVisible)
            {
                RemoveEventHandlers();
                AddEventHandlers();

                if (MarkBet.BetType.V6Factor == 1M)
                {
                    bdrV6.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void btnClearRace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                var race = (HPTRaceLight)btn.DataContext;
                race.SelectedHorse.Selected = false;
                race.SelectedHorse = null;
                if (V6BetMultiplierRule != null && V6BetMultiplierRule.Use && MarkBet != null)
                {
                    MarkBet.UpdateV6BetMultiplierSingleRows();
                    //this.MarkBet.SetV6BetMultiplierSingleRows();
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }
    }
}

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
                if (this.v6BetMultiplierRule == null && this.DataContext != null && this.DataContext.GetType() == typeof(HPTV6BetMultiplierRule))
                {
                    this.v6BetMultiplierRule = (HPTV6BetMultiplierRule)this.DataContext;
                    if (this.MarkBet == null)
                    {
                        this.MarkBet = this.v6BetMultiplierRule.MarkBet;
                    }
                }
                return this.v6BetMultiplierRule;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearRule();
        }

        private void ClearRule()
        {
            foreach (var horse in this.V6BetMultiplierRule.RaceList.SelectMany(r => r.HorseList))
            {
                horse.Selected = false;
            }
            foreach (var raceLight in this.V6BetMultiplierRule.RaceList)
            {
                raceLight.SelectedHorse = null;
            }
            this.V6BetMultiplierRule.Use = false;
            this.V6BetMultiplierRule.V6 = false;
            this.V6BetMultiplierRule.BetMultiplier = 1;
            this.V6BetMultiplierRule.HorseList.Clear();
            this.MarkBet.UpdateV6BetMultiplierSingleRows();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ClearRule();
            this.MarkBet.UpdateV6BetMultiplierSingleRows();
            this.MarkBet.V6BetMultiplierRuleList.Remove(this.V6BetMultiplierRule);
        }

        private void chkUseRule_Checked(object sender, RoutedEventArgs e)
        {
            if (this.MarkBet != null)
            {
                this.MarkBet.UpdateV6BetMultiplierSingleRows();
                //this.MarkBet.SetV6BetMultiplierSingleRows();
            }
        }

        private void lst_Checked(object sender, RoutedEventArgs e)
        {
            if (this.V6BetMultiplierRule != null && this.V6BetMultiplierRule.Use && this.MarkBet != null)
            {
                this.MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void cmbBetMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.V6BetMultiplierRule != null && this.V6BetMultiplierRule.Use && this.MarkBet != null)
            {
                this.MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void chkV6_Checked(object sender, RoutedEventArgs e)
        {
            if (this.V6BetMultiplierRule != null && this.V6BetMultiplierRule.Use && this.MarkBet != null)
            {
                this.MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void iudBetMultiplier_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.V6BetMultiplierRule != null && this.V6BetMultiplierRule.Use && this.MarkBet != null)
            {
                this.MarkBet.UpdateV6BetMultiplierSingleRows();
            }
        }

        private void AddEventHandlers()
        {
            this.chkUseRule.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            this.chkUseRule.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            if (this.chkV6 != null && this.chkV6.Visibility == System.Windows.Visibility.Visible)
            {
                this.chkV6.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkV6_Checked));
                this.chkV6.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkV6_Checked));
            }
            this.lst.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(lst_Checked));
            this.iudBetMultiplier.AddHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(iudBetMultiplier_ValueChanged));
        }

        private void RemoveEventHandlers()
        {
            this.chkUseRule.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            this.chkUseRule.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkUseRule_Checked));
            if (this.chkV6 != null && this.chkV6.Visibility == System.Windows.Visibility.Visible)
            {
                this.chkV6.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkV6_Checked));
                this.chkV6.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkV6_Checked));
            }
            this.lst.RemoveHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(lst_Checked));
            this.iudBetMultiplier.RemoveHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(iudBetMultiplier_ValueChanged));

        }

        private void ucV6BetMultiplierRule_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            {
                RemoveEventHandlers();
                AddEventHandlers();

                if (this.MarkBet.BetType.V6Factor == 1M)
                {
                    this.bdrV6.Visibility = System.Windows.Visibility.Collapsed;
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
                if (this.V6BetMultiplierRule != null && this.V6BetMultiplierRule.Use && this.MarkBet != null)
                {
                    this.MarkBet.UpdateV6BetMultiplierSingleRows();
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

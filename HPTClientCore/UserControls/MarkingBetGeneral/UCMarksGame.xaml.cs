using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Xps.Packaging;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCMarksGame.xaml
    /// </summary>
    public partial class UCMarksGame : UCMarkBetControl
    {
        public ObservableCollection<HPTRace> RaceList { get; set; }
        public HPTRaceDayInfo rdi;
        private HPTBetType BetType { get; set; }

        public UCMarksGame(HPTMarkBet markBet)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.MarkBet = markBet;

                this.rdi = markBet.RaceDayInfo;
                this.RaceList = new ObservableCollection<HPTRace>(markBet.RaceDayInfo.RaceList);
                this.atgUpdateTimer = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateFromATG));

                InitializeComponent();

                // Hantering av gratisanvändare som öppnar fil med V3, V4 eller V5
                //if (!HPTConfig.Config.IsPayingCustomer)
                //{
                //    if (this.MarkBet.BetType.Code == "V3" || this.MarkBet.BetType.Code == "V75" || this.MarkBet.BetType.Code == "V5" || this.MarkBet.BetType.Code == "GS75")
                //    {
                //        this.btnCreateCoupons.IsEnabled = false;
                //    }

                //    // Spara som
                //    this.btnCreateCouponsAs.IsEnabled = false;
                //    this.miSaveATGFileAs.IsEnabled = false;
                //    this.miSaveHPT4FileAs.IsEnabled = false;

                //    // Skriv ut
                //    this.btnPrint.IsEnabled = false;
                //    this.miPrintText.IsEnabled = false;
                //    this.miPrintXPS.IsEnabled = false;

                //    // Ladda upp
                //    this.btnUploadSystem.IsEnabled = false;
                //    this.miCopySystemGUIDToClipboard.IsEnabled = false;
                //    //this.miMailSystemInfoToOwnMail.IsEnabled = false;
                //    this.miOpenCorrectionURL.IsEnabled = false;
                //    //this.miUploadCompleteSystem.IsEnabled = false;

                //    // Nybörjarwizard
                //    this.btnBeginnerWizard.IsEnabled = false;

                //    this.txtBetMultiplier.Foreground = new SolidColorBrush(Colors.Gray);
                //    this.iudBetMultiplier.IsEnabled = false;
                //    this.chkV6.IsEnabled = false;
                //    //this.chkCouponCompression.IsEnabled = false;
                //    //this.cmbCouponCompression.IsEnabled = false;

                //    // Kopiera
                //    this.btnCopy.IsEnabled = false;
                //    this.miCopyCoupons.IsEnabled = false;
                //    this.miCopyHorseRank.IsEnabled = false;
                //    this.miCopyOwnRank.IsEnabled = false;
                //    this.miCopySingleRows.IsEnabled = false;
                //    this.miCopySystem.IsEnabled = false;

                //    // Spara
                //    this.miSaveATGFile.IsEnabled = false;
                //    this.miSaveHPT4File.IsEnabled = false;

                //    //// Automatisk omberäkning
                //    //this.chkAutomaticRecalculation.IsEnabled = false;

                //    //// Uppdatering
                //    //this.btnUpdate.IsEnabled = false;
                //    //this.cmbUpdateInterval.IsEnabled = false;
                //}
                this.ReductionCheckBoxList = new ObservableCollection<CheckBox>();
                this.CMMarkBetTabsToShow = new ContextMenu();

                //if (HPTConfig.Config.FirstTimeHPT5User)
                //{
                //    HPTConfig.Config.FirstTimeHPT5User = false;
                //    this.bdrGUIProfile.Visibility = System.Windows.Visibility.Visible;
                //    this.GUIElementsToShow = HPTConfig.Config.GetElementsToShow(GUIProfile.Simple);
                //    ApplyGUIElementsToShow(this.GUIElementsToShow);
                //    ApplyProfile(GUIProfile.Simple);
                //}
                //else
                //{
                //    ApplyGUIElementsToShow(HPTConfig.Config.GUIElementsToShow);
                //}

                ApplyGUIElementsToShow(HPTConfig.Config.GUIElementsToShow);

                // Skapa valen på expressnybörjarsystem
                CreateBeginnerSystemSizesMenu();

                // Skapa de tabbar man valt att visa
                CreateTabsToShow();
                HPTConfig.Config.MarkBetTabsToShow.PropertyChanged += new PropertyChangedEventHandler(MarkBetTabsToShow_PropertyChanged);
                HPTConfig.Config.GUIElementsToShow.PropertyChanged += new PropertyChangedEventHandler(GUIElementsToShow_PropertyChanged);
            }
        }

        void GUIElementsToShow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ApplyGUIElementsToShow(HPTConfig.Config.GUIElementsToShow);
        }

        void MarkBetTabsToShow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith("Show"))
            {
                //bool show = (bool)HPTConfig.Config.MarkBetTabsToShow.GetType().GetProperty(e.PropertyName).GetValue(HPTConfig.Config.MarkBetTabsToShow);
                //if (show)
                //{
                //    HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Remove(e.PropertyName);
                //}
                //else
                //{
                //    HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Add(e.PropertyName);
                //}
                CreateTabsToShow();
            }
        }



        public double OverviewMinWidth
        {
            get { return (double)GetValue(OverviewMinWidthProperty); }
            set { SetValue(OverviewMinWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverviewMinWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverviewMinWidthProperty =
            DependencyProperty.Register("OverviewMinWidth", typeof(double), typeof(UCMarksGame), new PropertyMetadata(360D));



        //public double OverviewMinWidth
        //{
        //    get
        //    {
        //        int maxNumberOfHorses = this.MarkBet.RaceDayInfo.RaceList.Max(r => r.HorseList.Count);
        //        return maxNumberOfHorses * 24D;
        //    }
        //}

        internal void CreateBeginnerSystemSizesMenu()
        {
            var spBeginnerSizes = new StackPanel();
            var beginneSizesList = new List<MenuItem>();

            foreach (int beginnerSize in HPTConfig.Config.BeginnerSizesToShow)
            {
                var miBeginnerSize = new MenuItem()
                {
                    Header = "Spela för " + beginnerSize.ToString() + " kr",
                    Tag = beginnerSize
                };
                miBeginnerSize.Click += miBeginnerSize_Click;
                spBeginnerSizes.Children.Add(miBeginnerSize);
                //beginneSizesList.Add(miBeginnerSize);
            }

            this.btnBeginnerWizard.DropDownContent = spBeginnerSizes;
        }

        void miBeginnerSize_Click(object sender, RoutedEventArgs e)
        {
            this.btnBeginnerWizard.IsOpen = false;
            if (this.MarkBet.RaceDayInfo.HorseListSelected.Count > 0)
            {
                var result = MessageBox.Show("Du har redan valt hästar, vill du behålla dessa på ditt system?", "Behåll valda hästar?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var horse in this.MarkBet.RaceDayInfo.HorseListSelected)
                    {
                        horse.Locked = true;
                    }
                }
                else
                {
                    this.MarkBet.ClearAll();
                }
            }
            Cursor = Cursors.Wait;
            try
            {
                var fe = (FrameworkElement)sender;
                int systemStake = (int)fe.Tag;
                int numberOfSpikes = 1;

                switch (this.MarkBet.BetType.Code)
                {
                    case "V4":
                        if (systemStake > 100)
                        {
                            numberOfSpikes = 0;
                        }
                        break;
                    case "V5":
                        if (systemStake > 300)
                        {
                            numberOfSpikes = 0;
                        }
                        break;
                    case "V64":
                    case "V65":
                        if (systemStake < 200)
                        {
                            numberOfSpikes = 2;
                        }
                        break;
                    case "V75":
                    case "GS75":
                        if (systemStake < 300)
                        {
                            numberOfSpikes = 2;
                        }
                        break;
                    case "V86":
                    case "V85":
                        if (systemStake < 400)
                        {
                            numberOfSpikes = 2;
                        }
                        else if (systemStake < 200)
                        {
                            numberOfSpikes = 3;
                        }
                        break;
                    default:
                        break;
                }

                // Skapa nybörjarmall med defaultvärden
                this.MarkBet.TemplateForBeginners = new HPTTemplateForBeginners()
                {
                    DesiredProfit = HPTDesiredProfit.Medium,
                    HorseRankVariableList = HPTConfig.Config.DefaultRankTemplate.HorseRankVariableList.Where(rv => rv.Use).ToList(),
                    NumberOfSpikes = numberOfSpikes,
                    ReductionRisk = HPTReductionRisk.Medium,
                    Stake = systemStake
                };

                Cursor = Cursors.Wait;
                // Skapa systemförslag utifrån valen...
                this.MarkBet.SelectFromBeginnerTemplate();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            Cursor = Cursors.Arrow;
        }

        #region Button clicks

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateFromATG();
        }

        private void UpdateFromATG()
        {
            try
            {
                // Hindra krockar om det tar lång tid att beräkna mallresultat
                if (this.MarkBet.IsCalculatingTemplates)
                {
                    return;
                }

                // Deaktivera knappen
                this.btnUpdate.IsEnabled = false;

                var serviceConnector = new HPTServiceConnector();
                serviceConnector.GetRaceDayInfoUpdate(this.MarkBet.RaceDayInfo.BetType.Code, this.MarkBet.RaceDayInfo.TrackId, this.MarkBet.RaceDayInfo.RaceDayDate, UpdateFromATG);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                Cursor = Cursors.Arrow;
            }
        }

        public void UpdateFromATG(HPTService.HPTRaceDayInfo rdi)
        {
            try
            {
                Dispatcher.Invoke(new Action<HPTService.HPTRaceDayInfo>(UpdateFromATGInvoke), rdi);
            }
            catch (Exception)
            {
                this.btnUpdate.IsEnabled = true;
            }
        }

        private void UpdateFromATGInvoke(HPTService.HPTRaceDayInfo rdi)
        {
            try
            {
                this.MarkBet.RaceDayInfo.Merge(rdi);
                this.MarkBet.TimeStamp = DateTime.Now;

                // Ska alltid göras

                this.MarkBet.RecalculateAllRanks();
                if (this.MarkBet.HasDynamicReductionRule()) // Regler som påverkas av uppdaterade hästvärden
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.All);
                }
                else   // Bara andra regler, så mindre beräkningar
                {
                    this.MarkBet.SingleRowCollection.RecalculateRowValues();
                    this.MarkBet.RecalculateRank();
                    this.MarkBet.SingleRowCollection.HandleHighestAndLowestSums();
                    this.MarkBet.SingleRowCollection.HandleHighestAndLowestIncludedSums();
                    //this.MarkBet.SingleRowCollection.RecalculateRowValues();
                }

                if (this.MarkBet.V6SingleRows || this.MarkBet.SingleRowBetMultiplier)
                {
                    this.MarkBet.UpdateV6BetMultiplierSingleRows();
                }

            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            Cursor = Cursors.Arrow;
            this.btnUpdate.IsEnabled = true;
        }

        #endregion

        #region Timer update handling

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

        System.Threading.Timer atgUpdateTimer;
        private void UpdateFromATG(object timerData)
        {
            this.Dispatcher.Invoke(new Action(UpdateFromATG), null);
        }

        #endregion

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
        }

        private bool userSelection;
        private void cmbTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.userSelection)
            {
                this.userSelection = true;
                this.cmbTemplates.SelectedItem = null;
                return;
            }
            if (this.cmbTemplates.SelectedItem != null)
            {
                Cursor = Cursors.Wait;
                //bool currentCouponCompression = this.MarkBet.CompressCoupons;
                try
                {
                    this.MarkBet.CompressCoupons = false;

                    if (this.cmbTemplates.SelectedItem.GetType() == typeof(HPTMarkBetTemplateABCD))
                    {
                        this.MarkBet.MarkBetTemplateABCD = (HPTMarkBetTemplateABCD)this.cmbTemplates.SelectedItem;
                        this.MarkBet.Clear(false, true, true);
                        this.MarkBet.SelectFromTemplateABCD();
                    }
                    else if (this.cmbTemplates.SelectedItem.GetType() == typeof(HPTMarkBetTemplateRank))
                    {
                        this.MarkBet.MarkBetTemplateRank = (HPTMarkBetTemplateRank)this.cmbTemplates.SelectedItem;
                        this.MarkBet.Clear(false, true, true);
                        this.MarkBet.SelectFromTemplateRank();
                    }
                }
                catch (Exception exc)
                {
                    Config.AddToErrorLog(exc);
                }
                this.MarkBet.CompressCoupons = true;//currentCouponCompression;
                Cursor = Cursors.Arrow;
            }
        }

        private void cmbTemplateResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbTemplateResults.SelectedItem != null)
            {
                Cursor = Cursors.Wait;
                HPTMarkBetTemplateResult tr = (HPTMarkBetTemplateResult)this.cmbTemplateResults.SelectedItem;
                this.MarkBet.ApplyTemplateResult(tr);
                Cursor = Cursors.Arrow;
            }
        }

        private void cmbUpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbUpdateInterval.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)this.cmbUpdateInterval.SelectedItem;
                int updatePeriod = Convert.ToInt32(cbi.Tag) * 60 * 1000;
                if (updatePeriod == 0)
                {
                    this.atgUpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
                else
                {
                    this.atgUpdateTimer.Change(updatePeriod, updatePeriod);
                }
            }
        }

        private bool isLoaded;
        private void ucMarksGame_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            {
                try
                {
                    if (this.MarkBet == null)
                    {
                        this.MarkBet = (HPTMarkBet)this.DataContext;
                    }
                    if (!this.isLoaded)
                    {
                        // V6/GS7
                        this.chkV6.Visibility = MarkBet.V6Visibility;

                        // Hur breda ska raderna i översikten vara?
                        int maxNumberOfHorses = this.MarkBet.RaceDayInfo.RaceList.Max(r => r.HorseList.Count);
                        this.OverviewMinWidth = maxNumberOfHorses * 24D;

                        // Se till att ingen beräkning sker
                        this.MarkBet.pauseRecalculation = true;

                        // Sätt rätt val i Comboboxarna
                        //this.cmbCouponCompression.SelectedIndex = (int)this.MarkBet.CouponCompression;
                        this.cmbReservHandling.SelectedIndex = (int)this.MarkBet.ReservHandling - 1;

                        // Skapa upp eventhandlers som behövs för att uppdateringen ska funka
                        this.MarkBet.SetEventHandlers();

                        // Skapa listan med reduceringsalternativ utifrå om man är betalande kund eller ej.
                        List<HPTReductionAttribute> attributeList = this.MarkBet.GetReductionAttributes();
                        this.ReductionCheckBoxList.Clear();
                        foreach (HPTReductionAttribute ra in attributeList.OrderBy(a => a.Order))
                        {
                            var chk = new CheckBox()
                            {
                                Content = ra.Name,
                                DataContext = this.MarkBet,
                                Margin = new Thickness(1D, 0.5D, 5D, 0.5D)
                            };
                            chk.SetBinding(CheckBox.IsCheckedProperty, ra.PropertyName);
                            //chk.Checked += (cs, ce) => this.MarkBet.RecalculateReduction(RecalculateReason.All);
                            //chk.Unchecked += (cs, ce) => this.MarkBet.RecalculateReduction(RecalculateReason.All);
                            this.ReductionCheckBoxList.Add(chk);
                        }

                        //// Skapa context menu för vilk flikar man vill visa
                        CreateMarkBetTabsToShowContextMenu();

                        // Ställ tillbaka flaggan så att reducering kan ske igen
                        this.MarkBet.pauseRecalculation = false;
                        this.MarkBet.IsDeserializing = false;

                        // Beräkna reducering
                        this.MarkBet.RecalculateAllRanks();
                        this.MarkBet.RecalculateRank();
                        if (this.MarkBet.SystemSize > 0)
                        {
                            this.MarkBet.RecalculateReduction(RecalculateReason.All);
                        }

                        // Begränsningar för gratisanvändarna
                        if (!this.Config.IsPayingCustomer)
                        {
                            this.gbFile.IsEnabled = false;
                            this.btnCreateCoupons.IsEnabled = false;
                            this.btnCreateCouponsAs.IsEnabled = false;

                            //IEnumerable<HPTXReductionRule> proXReductionRules = this.MarkBet.ABCDEFReductionRule.XReductionRuleList.Where(x => x.Use && (x.Prio == HPTPrio.D || x.Prio == HPTPrio.E || x.Prio == HPTPrio.F));
                            //if ((this.MarkBet.ABCDEFReductionRule.Use && this.MarkBet.ReductionRulesToApply.Count > 1)
                            //    || proXReductionRules.Count() > 0)
                            //{
                            //    this.btnCreateCoupons.IsEnabled = false;
                            //    this.btnCreateCouponsAs.IsEnabled = false;
                            //}
                        }

                        if (this.MarkBet.LastSaveTime == DateTime.MinValue)
                        {
                            // Hantera standardreservhantering
                            switch (HPTConfig.Config.DefaultReservHandling)
                            {
                                case ReservHandling.Own:
                                case ReservHandling.None:
                                    this.cmbReservHandling.SelectedIndex = 0;
                                    break;
                                case ReservHandling.MarksSelected:
                                    this.cmbReservHandling.SelectedIndex = 1;
                                    break;
                                case ReservHandling.MarksNotSelected:
                                    this.cmbReservHandling.SelectedIndex = 2;
                                    break;
                                case ReservHandling.OwnRankSelected:
                                    this.cmbReservHandling.SelectedIndex = 3;
                                    break;
                                case ReservHandling.OwnRankNotSelected:
                                    this.cmbReservHandling.SelectedIndex = 4;
                                    break;
                                case ReservHandling.RankMeanSelected:
                                    this.cmbReservHandling.SelectedIndex = 5;
                                    break;
                                case ReservHandling.RankMeanNotSelected:
                                    this.cmbReservHandling.SelectedIndex = 6;
                                    break;
                                case ReservHandling.OddsSelected:
                                    this.cmbReservHandling.SelectedIndex = 7;
                                    break;
                                case ReservHandling.OddsNotSelected:
                                    this.cmbReservHandling.SelectedIndex = 8;
                                    break;
                                case ReservHandling.NextRankSelected:
                                    this.cmbReservHandling.SelectedIndex = 9;
                                    break;
                                case ReservHandling.NextRankNotSelected:
                                    this.cmbReservHandling.SelectedIndex = 10;
                                    break;
                                default:
                                    break;
                            }

                            // Hantera standarduppdateringsfrekvens
                            switch (HPTConfig.Config.DefaultUpdateInterval)
                            {
                                case 3:
                                    this.cmbUpdateInterval.SelectedIndex = 1;
                                    break;
                                case 5:
                                    this.cmbUpdateInterval.SelectedIndex = 2;
                                    break;
                                case 10:
                                    this.cmbUpdateInterval.SelectedIndex = 3;
                                    break;
                                case 15:
                                    this.cmbUpdateInterval.SelectedIndex = 4;
                                    break;
                                case 20:
                                    this.cmbUpdateInterval.SelectedIndex = 5;
                                    break;
                                case 30:
                                    this.cmbUpdateInterval.SelectedIndex = 6;
                                    break;
                                case 60:
                                    this.cmbUpdateInterval.SelectedIndex = 7;
                                    break;
                                default:
                                    this.cmbUpdateInterval.SelectedIndex = 0;
                                    break;
                            }
                        }


                        SetCountDown();

                        // Sätt flagga att första laddningen är klar
                        this.isLoaded = true;
                    }
                }
                catch (Exception exc)
                {
                    HPTConfig.AddToErrorLogStatic(exc);
                }
            }
        }

        private void Chk_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal void CreateMarkBetTabsToShowContextMenu()
        {
            // Skapa context menu för vilk flikar man vill visa
            List<HPTMarkBetTabsToShowAttribute> tabsToShowAttributeList = HPTConfig.Config.MarkBetTabsToShow.GetMarkBetTabsToShowAttributes();
            this.CMMarkBetTabsToShow.Items.Clear();
            this.CMMarkBetTabsToShow.DataContext = HPTConfig.Config.MarkBetTabsToShow;
            foreach (HPTMarkBetTabsToShowAttribute hda in tabsToShowAttributeList)
            {
                MenuItem mi = new MenuItem()
                {
                    IsCheckable = true,
                    Header = hda.Name,
                    DataContext = HPTConfig.Config.MarkBetTabsToShow
                };

                mi.SetBinding(MenuItem.IsCheckedProperty, hda.PropertyName);
                this.CMMarkBetTabsToShow.Items.Add(mi);
            }
        }

        HPTRace upcomingRace;
        internal void SetCountDown()
        {
            // Återställ textinställningar
            this.txtCountdownTimer.FontWeight = FontWeights.Normal;
            this.txtCountdownTimer.Foreground = new SolidColorBrush(Colors.Black);

            // Hämta nästa lopp
            this.upcomingRace = this.MarkBet.RaceDayInfo.RaceList
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

        private void gbReduction_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right)
            {
                e.Handled = true;
            }
        }

        public ObservableCollection<CheckBox> ReductionCheckBoxList
        {
            get { return (ObservableCollection<CheckBox>)GetValue(ReductionCheckBoxListProperty); }
            set { SetValue(ReductionCheckBoxListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReductionCheckBoxList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReductionCheckBoxListProperty =
            DependencyProperty.Register("ReductionCheckBoxList", typeof(ObservableCollection<CheckBox>), typeof(UCMarksGame), new UIPropertyMetadata(null));


        private void btnApplyTemplate_Click(object sender, RoutedEventArgs e)
        {
            //bool couponsCompressed = this.MarkBet.CompressCoupons;
            try
            {
                Cursor = Cursors.Wait;
                this.MarkBet.CompressCoupons = false;

                if (this.cmbTemplates.SelectedItem.GetType() == typeof(HPTMarkBetTemplateABCD))
                {
                    this.MarkBet.CreateSystemsFromTemplateABCD();
                }
                else if (this.cmbTemplates.SelectedItem.GetType() == typeof(HPTMarkBetTemplateRank))
                {
                    this.MarkBet.ChangeRankSize(this.MarkBet.MarkBetTemplateRank);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            this.MarkBet.CompressCoupons = true;//couponsCompressed;
            Cursor = Cursors.Arrow;
        }

        private void hlATGFile_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + this.MarkBet.SystemFilename + "\"");
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void tcMarksGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems[0].GetType() != typeof(TabItem))
                {
                    return;
                }

                var ti = (TabItem)e.AddedItems[0];
                // Expandera/fäll ihop delen med inställningar
                if (ti.Name == "tiRaces" || ti.Name == "tiRacesGrouped")
                {
                    this.expanderMarksgame.IsExpanded = true;
                }
                else
                {
                    this.expanderMarksgame.IsExpanded = false;
                }

                // Ladda XML-fil till ATG
                if (ti.Name == "tiXmlFile")
                {
                    try
                    {
                        this.MarkBet.CouponCorrector.CouponHelper.OnlyCreateTempFile = true;
                        this.MarkBet.CouponCorrector.CouponHelper.CreateATGFile();
                        this.MarkBet.SystemFilename = this.MarkBet.CouponCorrector.CouponHelper.TempFileName;
                        var encodedUrl = "file:///" + this.MarkBet.CouponCorrector.CouponHelper.TempFileName.Replace("\\", "/");
                        this.wbXmlFile.Navigate(new Uri(encodedUrl));
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                    this.MarkBet.CouponCorrector.CouponHelper.OnlyCreateTempFile = false;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnClone_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbReservHandling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBoxItem cbi = (ComboBoxItem)e.AddedItems[0];
                ReservHandling rh = (ReservHandling)Convert.ToInt32(cbi.Tag);
                this.MarkBet.ReservHandling = rh;
                this.MarkBet.CouponCorrector.CouponHelper.HandleReserverForCoupons(this.MarkBet.ReservHandling);
            }
        }

        private void ucMarksGame_KeyUp(object sender, KeyEventArgs e)
        {
            switch (Keyboard.Modifiers)
            {
                case ModifierKeys.Alt:
                    if (e.SystemKey == Key.D1)
                    {
                        this.MarkBet.CreateInternalSystemV4ABC();
                    }
                    if (e.SystemKey == Key.D2)
                    {
                        this.MarkBet.CreateInternalSystemV4PlaceThisYear();
                    }
                    if (e.SystemKey == Key.D3)
                    {
                        this.MarkBet.CreateInternalSystemV4EarningsThisYear();
                    }
                    if (e.SystemKey == Key.D4)
                    {
                        this.MarkBet.CreateInternalSystemV4CombinedRankingThisYear();
                    }
                    if (e.SystemKey == Key.D5)
                    {
                        this.MarkBet.CreateInternalSystemV4CombinedRankingLastYear();
                    }
                    if (e.SystemKey == Key.D6)
                    {
                        this.MarkBet.CreateInternalSystemV4CombinedRankingTotal();
                    }
                    if (e.SystemKey == Key.D8)
                    {
                        this.MarkBet.CreateInternalSystem();
                    }
                    if (e.SystemKey == Key.D7)
                    {
                        this.MarkBet.CreateInternalSystemV4StakeDistributionRanking();
                    }
                    if (e.SystemKey == Key.C)
                    {
                        string result = this.MarkBet.CalculateBestABCDCombination();
                        Clipboard.SetDataObject(result);
                    }
                    if (e.SystemKey == Key.D)
                    {
                        string result = this.MarkBet.CalculateBestAPlusBPlusCCombinations();
                        Clipboard.SetDataObject(result);
                    }
                    if (e.SystemKey == Key.A)
                    {
                        //string result = this.MarkBet.CalculateDifficulty();
                        string result = this.MarkBet.CalculateDifficultyAlt();
                        Clipboard.SetDataObject(result);
                    }
                    if (e.SystemKey == Key.J)
                    {
                        string result = this.MarkBet.CalculateJackpotRows();
                        Clipboard.SetDataObject(result);
                    }
                    break;
                case ModifierKeys.Control:
                    if (Config.IsPayingCustomer)
                    {
                        switch (e.Key)
                        {
                            case Key.S:
                                btnCreateCoupons_Click(sender, new RoutedEventArgs());
                                break;
                            case Key.P:
                                btnPrint_Click(sender, new RoutedEventArgs());
                                break;
                            case Key.C:
                                btnCopy_Click(sender, new RoutedEventArgs());
                                break;
                            case Key.R:
                                btnClearAll_Click(sender, new RoutedEventArgs());
                                break;
                            case Key.V:
                                HandlePastedText();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ModifierKeys.None:
                    break;
                case ModifierKeys.Shift:
                    break;
                case ModifierKeys.Windows:
                    break;
                default:
                    break;
            }
        }

        private void HandlePastedText()
        {
            string tips = Clipboard.GetText();
            if (this.MarkBet.ParseTips(tips))
            {
                ShowTipsWindow();
            }
        }

        internal void ShowTipsWindow()
        {
            var wndShowTipsOverview = new Window()
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                Title = "Använd tips!",
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                Owner = App.Current.MainWindow
            };
            wndShowTipsOverview.Content = new UCOverviewFromTips()
            {
                DataContext = this.MarkBet,
                MarkBet = this.MarkBet
            };
            wndShowTipsOverview.ShowDialog();
        }

        //private void cmbCouponCompression_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count > 0 && this.MarkBet != null)
        //    {
        //        ComboBoxItem cbi = (ComboBoxItem)e.AddedItems[0];
        //        CouponCompression cc = (CouponCompression)Convert.ToInt32(cbi.Tag);

        //        this.MarkBet.CouponCompression = cc;
        //        this.MarkBet.SingleRowCollection_AnalyzingFinished();             
        //    }
        //}

        public ContextMenu CMMarkBetTabsToShow
        {
            get { return (ContextMenu)GetValue(CMMarkBetTabsToShowProperty); }
            set { SetValue(CMMarkBetTabsToShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMMarkBetTabsToShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMMarkBetTabsToShowProperty =
            DependencyProperty.Register("CMMarkBetTabsToShow", typeof(ContextMenu), typeof(UCMarksGame), new UIPropertyMetadata(null));


        #region Overview

        private void SetRaceListCollectionViewSource()
        {
            var raceListView = CollectionViewSource.GetDefaultView(this.RaceList);
            //raceListView.SortDescriptions
        }

        #endregion

        #region Spara filer

        internal void SaveFiles(bool saveHPT4File, bool saveXMLFile, bool setFileNames)
        {
            Cursor = Cursors.Wait;
            Exception exc = null;
            try
            {
                PrepareForSave(setFileNames);
                if (saveXMLFile)
                {
                    try
                    {
                        this.MarkBet.CouponCorrector.CouponHelper.CreateATGFile();
                        if (HPTConfig.Config.MarkBetTabsToShow.ShowATGXmlFile)
                        {
                            var encodedUrl = "file:///" + this.MarkBet.SystemFilename.Replace("\\", "/");
                            this.wbXmlFile.Navigate(new Uri(encodedUrl));
                        }
                    }
                    catch (Exception atgExc)
                    {
                        HPTConfig.Config.AddToErrorLog(atgExc);
                        exc = atgExc;
                    }
                }
                if (saveHPT4File)
                {
                    try
                    {
                        HPTSerializer.SerializeHPTSystem(this.MarkBet.MailSender.HPT3FileName, this.MarkBet);
                    }
                    catch (Exception hptExc)
                    {
                        HPTConfig.Config.AddToErrorLog(hptExc);
                        exc = hptExc;
                    }
                }
            }
            catch (Exception outerExc)
            {
                HPTConfig.Config.AddToErrorLog(outerExc);
                exc = outerExc;
            }
            if (exc != null)
            {
                MessageBox.Show("Något gick fel vid sparning av system och/eller kupongfil. Kontrollera fellogg för detaljer", "Fel vid sparning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Cursor = Cursors.Arrow;
        }

        private void miSaveHPT4File_Click(object sender, RoutedEventArgs e)
        {
            this.btnCreateCoupons.IsOpen = false;
            SaveFiles(true, false, true);
            //Cursor = Cursors.Wait;
            //PrepareForSave();
            //HPTSerializer.SerializeHPTSystem(this.MarkBet.MailSender.HPT3FileName, this.MarkBet);
            //Cursor = Cursors.Arrow;
        }

        private void btnCreateCoupons_Click(object sender, RoutedEventArgs e)
        {
            // Om man kommit hit genom att trycka Ctrl + S, men inte har rättighet att spara filen
            if (!btnCreateCoupons.IsEnabled)
            {
                return;
            }
            SaveFiles(true, true, true);
            //Cursor = Cursors.Wait;
            //try
            //{
            //    PrepareForSave();
            //    this.MarkBet.CouponCorrector.CouponHelper.CreateATGFile();
            //    HPTSerializer.SerializeHPTSystem(this.MarkBet.MailSender.HPT3FileName, this.MarkBet);
            //    if (HPTConfig.Config.MarkBetTabsToShow.ShowATGXmlFile)
            //    {
            //        var encodedUrl = "file:///" + this.MarkBet.SystemFilename.Replace("\\", "/");
            //        this.wbXmlFile.Navigate(new Uri(encodedUrl));
            //    }
            //}
            //catch (Exception exc)
            //{
            //    HPTConfig.Config.AddToErrorLog(exc);
            //    MessageBox.Show("Något gick fel vid sparning av system och/eller kupongfil. Kontrollera fellogg för detaljer", "Fel vid sparning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
            //Cursor = Cursors.Arrow;
        }

        private void btnCreateCouponsAs_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.CheckForWarnings();

            // Varna för att det är fler kuponger än tillåtet
            this.MarkBet.HandleTooManyCoupons();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = this.MarkBet.SaveDirectory;
            sfd.FileName = this.MarkBet.ToFileNameString() + ".hpt7";
            sfd.Filter = "Hjälp på traven-system|*.hpt7";
            sfd.FileOk += new System.ComponentModel.CancelEventHandler(sfd_FileOk);
            sfd.ShowDialog();
        }

        internal void PrepareForSave(bool setFileNames)
        {
            this.MarkBet.CheckForWarnings();

            // Varna för att det är fler kuponger än tillåtet
            this.MarkBet.HandleTooManyCoupons();

            if (setFileNames)
            {
                string fileName = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString();
                this.MarkBet.SystemFilename = fileName + ".xml";

                string hpt3Filename = fileName + ".hpt7";
                this.MarkBet.MailSender.HPT3FileName = hpt3Filename;
            }
            try
            {
                Clipboard.SetDataObject(this.MarkBet.SystemFilename);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            this.MarkBet.SetSerializerValues();
        }

        void sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFileDialog sfd = (SaveFileDialog)sender;
            string fileName = sfd.FileName;
            this.MarkBet.SystemFilename = fileName;
            string hpt4Filename = fileName.Replace(".xml", ".hpt7");
            this.MarkBet.MailSender.HPT3FileName = hpt4Filename;
            SaveFiles(true, true, false);
        }

        private void miSaveATGFile_Click(object sender, RoutedEventArgs e)
        {
            this.btnCreateCoupons.IsOpen = false;
            SaveFiles(false, true, true);
        }

        private void miSaveHPT4FileAs_Click(object sender, RoutedEventArgs e)
        {
            this.btnCreateCouponsAs.IsOpen = false;
            //PrepareForSave(false);
            var sfdHPT4 = new SaveFileDialog();
            sfdHPT4.InitialDirectory = this.MarkBet.SaveDirectory;
            sfdHPT4.FileName = this.MarkBet.ToFileNameString() + ".hpt7";
            sfdHPT4.Filter = "Hjälp på traven-system|*.hpt7";
            sfdHPT4.FileOk += new System.ComponentModel.CancelEventHandler(sfdHPT4_FileOk);
            sfdHPT4.ShowDialog();
        }

        void sfdHPT4_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFileDialog sfd = (SaveFileDialog)sender;
            string fileName = sfd.FileName;
            this.MarkBet.MailSender.HPT3FileName = fileName;
            SaveFiles(true, false, false);
        }

        private void miSaveATGFileAs_Click(object sender, RoutedEventArgs e)
        {
            this.btnCreateCouponsAs.IsOpen = false;
            //PrepareForSave();
            var sfdATG = new SaveFileDialog();
            sfdATG.InitialDirectory = this.MarkBet.SaveDirectory;
            sfdATG.FileName = this.MarkBet.ToFileNameString() + ".xml";
            sfdATG.Filter = "ATG kupongfil (*.xml)|*.xml|Alla filer (*.*)|*.*";
            sfdATG.FileOk += new System.ComponentModel.CancelEventHandler(sfdATG_FileOk);
            sfdATG.ShowDialog();
        }

        void sfdATG_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFileDialog sfd = (SaveFileDialog)sender;
            string fileName = sfd.FileName;
            this.MarkBet.SystemFilename = fileName;
            SaveFiles(false, true, false);
            Cursor = Cursors.Arrow;
        }

        #endregion

        #region Kopiering

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            CopySystemInformationToClipboard();
        }

        internal string CopySystemInformationToClipboard()
        {
            this.MarkBet.SetReductionRuleString();
            string systemInfo = this.MarkBet.ToClipboardString();
            if (HPTConfig.Config.CopyCouponsToClipboard)
            {
                systemInfo += this.MarkBet.CouponCorrector.CouponHelper.ToCouponsString();
            }
            if (HPTConfig.Config.CopySingleRowsToClipboard)
            {
                systemInfo += this.MarkBet.SingleRowCollection.ToSingleRowsString();
            }
            try
            {
                Clipboard.SetDataObject(systemInfo);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            return systemInfo;
        }

        private void miCopySystem_Click(object sender, RoutedEventArgs e)
        {
            this.btnCopy.IsOpen = false;
            this.MarkBet.SetReductionRuleString();
            string systemInfo = this.MarkBet.ToClipboardString();
            try
            {
                Clipboard.SetDataObject(systemInfo);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void miCopySingleRows_Click(object sender, RoutedEventArgs e)
        {
            this.btnCopy.IsOpen = false;
            try
            {
                Clipboard.SetDataObject(this.MarkBet.SingleRowCollection.ToSingleRowsString());
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void miCopyCoupons_Click(object sender, RoutedEventArgs e)
        {
            this.btnCopy.IsOpen = false;
            try
            {
                Clipboard.SetDataObject(this.MarkBet.CouponCorrector.CouponHelper.ToCouponsString());
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void miCopyHorseRank_Click(object sender, RoutedEventArgs e)
        {
            this.btnCopy.IsOpen = false;
            try
            {
                Clipboard.SetDataObject(this.MarkBet.SetRankMeanString());
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void miCopyOwnRank_Click(object sender, RoutedEventArgs e)
        {
            this.btnCopy.IsOpen = false;
            try
            {
                Clipboard.SetDataObject(this.MarkBet.SetRankOwnString());
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void miCopyCompactSystemInfo_Click(object sender, RoutedEventArgs e)
        {
            this.btnCopy.IsOpen = false;
            try
            {
                Clipboard.SetDataObject(this.MarkBet.RaceInformationStringCompact);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        #endregion

        #region Rensning

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.ClearAll();
            this.cmbTemplates.SelectedIndex = -1;
            this.cmbTemplateResults.SelectedIndex = -1;
        }

        private void miClearReductions_Click(object sender, RoutedEventArgs e)
        {
            this.btnClearAll.IsOpen = false;
            this.MarkBet.ClearReductions();
        }

        private void miClearABCD_Click(object sender, RoutedEventArgs e)
        {
            this.btnClearAll.IsOpen = false;
            this.MarkBet.ClearABCD();
        }

        #endregion

        #region Utskrift

        private void miPrintText_Click(object sender, RoutedEventArgs e)
        {
            // Skapa utskriftsytan
            this.btnPrint.IsOpen = false;
            this.MarkBet.SetReductionRuleString();
            string systemInfo = this.MarkBet.ToClipboardString();
            PrintUsingDocumentCondensed(systemInfo, "HPT-uskrift text");
        }

        private void PrintUsingDocumentCondensed(string text, string printCaption = "")
        {
            //Create the document, passing a new paragraph and new run using text
            FlowDocument doc = new FlowDocument(new Paragraph(new Run(text)));
            doc.PagePadding = new Thickness(100);
            //Creates margin around the page

            PrintDialog diag = new PrintDialog();
            //Used to perform printing

            //Send the document to the printer
            diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, printCaption);
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.SetReductionRuleString();
            UCMarkingBetSystemDocument uc = new UCMarkingBetSystemDocument();
            uc.DataContext = this.MarkBet;
            PrintDialog pd = new PrintDialog();
            if ((bool)pd.ShowDialog().GetValueOrDefault())
            {
                uc.Measure(new Size(816, 1500));
                uc.Arrange(new Rect(new Size(816, 1300)));
                uc.UpdateLayout();
                pd.PrintVisual(uc, "HPT");
            }
        }

        private void miPrintXPS_Click(object sender, RoutedEventArgs e)
        {
            // Skapa utskriftsytan
            this.btnPrint.IsOpen = false;
            this.MarkBet.SetReductionRuleString();
            UCMarkingBetSystemDocument uc = new UCMarkingBetSystemDocument();
            uc.DataContext = this.MarkBet;
            uc.Measure(new Size(816, 1500));
            uc.Arrange(new Rect(new Size(816, 1300)));
            uc.UpdateLayout();

            // Initiera nödvändiga hjäpobjekt
            var fixedDoc = new FixedDocument();
            var pageContent = new PageContent();
            var fixedPage = new FixedPage();

            // Skapa första sidan av dokumentet
            fixedPage.Children.Add(uc);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);

            string fileName = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString() + ".xps";
            var xpsDoc = new XpsDocument(fileName, System.IO.FileAccess.ReadWrite);
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
            xw.Write(fixedDoc);
            xpsDoc.Close();
        }

        private void miSavePicture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.btnCreateCoupons.IsOpen = false;
                this.MarkBet.SetReductionRuleString();
                var uc = new UCMarkingBetSystemDocument();
                uc.DataContext = this.MarkBet;
                uc.Measure(new Size(816, 1300));
                uc.Arrange(new Rect(new Size(816, 1300)));

                uc.UpdateLayout();

                string fileName = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString() + ".png";
                //string fileName = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString() + ".jpg";

                CreateImageFromVisual(uc, fileName);

                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal void CreateImageFromVisual(FrameworkElement fe, string fileName)
        {
            try
            {
                var bmp = new RenderTargetBitmap((int)fe.ActualWidth * 2, (int)fe.ActualHeight * 2, 192, 192, PixelFormats.Pbgra32);
                bmp.Render(fe);
                var img = new Image()
                {
                    Source = bmp
                };

                var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
                enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

                using (var stm = System.IO.File.Create(fileName))
                {
                    enc.Save(stm);
                }

            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #endregion

        private void btnInterruptTemplateCalculation_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.InterruptSystemsCreation = true;
        }

        private void chkAutomaticRecalculation_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool automaticRecalculation = (bool)this.chkAutomaticRecalculation.IsChecked;
                this.MarkBet.pauseRecalculation = !automaticRecalculation;
                if (automaticRecalculation)
                {
                    this.MarkBet.RecalculateReduction(RecalculateReason.All);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnBeginnerWizard_Click(object sender, RoutedEventArgs e)
        {
            OpenBeginnerWindow();
        }

        private void OpenBeginnerWindow()
        {
            this.MarkBet.TemplateForBeginners = this.MarkBet.CreateTemplateForBeginners();
            var wndBeginnerWizard = new Window()
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                Title = "Skapa ett expressystem!",
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                Owner = App.Current.MainWindow
            };
            wndBeginnerWizard.Content = new UCBeginnerWizard(wndBeginnerWizard)
            {
                DataContext = this.MarkBet.TemplateForBeginners,
                MarkBet = this.MarkBet
            };
            wndBeginnerWizard.ShowDialog();
        }

        #region Ladda upp system

        private void btnUploadSystem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Ladda upp system för mobilrättning till HPTs server. HPT- och ATG-filer kommer automatiskt att sparas med automatgenerade namn först. Gör du några ändringar i systemet innan inlämning eller mailskickning måste du ladda upp det på nytt.", "Ladda upp system", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Yes)
                {
                    // Spara hpt5- och ATG-fil
                    if (this.MarkBet.CouponCorrector.CouponHelper.CouponList.Count == 0)
                    {
                        this.MarkBet.CouponCorrector.CouponHelper.CreateCoupons();
                    }
                    this.MarkBet.CouponCorrector.CouponHelper.CreateATGFile();  // Skapa alltid ATG-fil

                    string hpt3Filename = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString() + ".hpt7";
                    this.MarkBet.MailSender.HPT3FileName = hpt3Filename;
                    HPTSerializer.SerializeHPTSystem(hpt3Filename, this.MarkBet);

                    // Ladda upp fil till servern
                    var serviceConnector = new HPTServiceConnector();
                    serviceConnector.SaveSystem(this.MarkBet);

                    // Fixa till mailet
                    string url = "http://correction.hpt.nu/Default.aspx?SystemGUID=" + this.MarkBet.UploadedSystemGUID;
                    Clipboard.SetDataObject("http://correction.hpt.nu/Default.aspx?SystemGUID=" + this.MarkBet.UploadedSystemGUID);
                    MessageBox.Show("System uppladdat till HPTs server.", "Uppladdning klar", MessageBoxButton.OK);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                MessageBox.Show("Uppladdningen av system misslyckades, var vänlig försök igen senare.", "Uppladning misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetBody()
        {
            if (this.MarkBet == null)
            {
                return;
            }

            //this.txtBody.Text = sb.ToString();
        }

        private string GetSubject()
        {
            return this.MarkBet.BetType.Code + " " + this.MarkBet.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd") + ", " + this.MarkBet.ReducedSize.ToString() + " rader.";
        }

        private void miCopySystemGUIDToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.MarkBet.UploadedSystemGUID))
            {
                try
                {
                    //Clipboard.SetText("http://correction.hpt.nu/Default.aspx?SystemGUID=" + this.MarkBet.UploadedSystemGUID);
                    Clipboard.SetDataObject(this.MarkBet.SystemURL);
                }
                catch (Exception exc)
                {
                    HPTConfig.AddToErrorLogStatic(exc);
                }
            }
        }

        //private void miMailSystemInfoToOwnMail_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var mailSender = new HPTMailSender()
        //        {
        //            MailRecipients = new ObservableCollection<HPTMailRecipient>()
        //            {
        //                new HPTMailRecipient()
        //                {
        //                    EMailAddress = HPTConfig.Config.EMailAddress, 
        //                    Name = HPTConfig.Config.UserName, 
        //                    Selected = true 
        //                }
        //            },
        //            Body = CopySystemInformationToClipboard(),
        //            Subject = GetSubject(),
        //            AttachHPT3File = true,
        //            AttachATGSystemFile = true,
        //            ATGSystemFileName = this.MarkBet.SystemFilename
        //            //ATGSystemFileName = this.MarkBet.MailSender.ATGSystemFileName,
        //        };

        //        var serviceConnector = new HPTServiceConnector();
        //        HPTServiceConnector.SendMail(mailSender, this.MarkBet);

        //        //mailSender.SendMail();
        //        MessageBox.Show("Mailet har nu skickats till din Inkorg.", "E-Post skickat", MessageBoxButton.OK);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.AddToErrorLogStatic(exc);
        //        MessageBox.Show("Det gick inte att sända mailet:\r\n" + exc.Message, "E-Postskickning misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void miOpenCorrectionURL_Click(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(this.MarkBet.UploadedSystemGUID))
            if (!string.IsNullOrEmpty(this.MarkBet.SystemURL))
            {
                //System.Diagnostics.Process.Start("http://correction.hpt.nu/Default.aspx?SystemGUID=" + this.MarkBet.UploadedSystemGUID);
                System.Diagnostics.Process.Start(this.MarkBet.SystemURL);
            }
        }

        private void miUploadCompleteSystem_Click(object sender, RoutedEventArgs e)
        {
            var wndUploadSystem = new Window()
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                Title = "Ladda upp system!",
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                Owner = App.Current.MainWindow
            };
            wndUploadSystem.Content = new UCUserSystemUpload()
            {
                DataContext = this.MarkBet,
                MarkBet = this.MarkBet
            };
            wndUploadSystem.ShowDialog();
        }

        #endregion

        private System.Windows.Controls.Primitives.Popup pu;
        private void lst_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var fe = (FrameworkElement)e.OriginalSource;
            if (fe.DataContext.GetType() != typeof(HPTHorse))
            {
                e.Handled = true;
                return;
            }
            var horse = (HPTHorse)fe.DataContext;
            if (this.pu == null)
            {
                this.pu = new System.Windows.Controls.Primitives.Popup()
                {
                    Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                    PlacementTarget = fe,
                    HorizontalOffset = -5D,
                    VerticalOffset = -5D
                };
                this.pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
            }

            // Skapa innehållet för popupen
            Border b = new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1D),
                Width = double.NaN,
                Child = new UCCompactHorse()
                {
                    BorderBrush = new SolidColorBrush(Colors.WhiteSmoke),
                    BorderThickness = new Thickness(6D),
                    Width = double.NaN
                }
            };

            // Hindra eventet från att routas uppåt
            e.Handled = true;

            // Visa popupen
            this.pu.DataContext = horse;
            this.pu.Child = b;
            this.pu.IsOpen = true;
        }

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup pu = (System.Windows.Controls.Primitives.Popup)sender;
            pu.Child = null;
            pu.IsOpen = false;
        }

        #region gränssnittsprofilhantering

        public GUIProfile Profile { get; set; }

        public HPTGUIElementsToShow GUIElementsToShow { get; set; }

        //private void btnSelectGUIProfile_Click(object sender, RoutedEventArgs e)
        //{
        //    HPTConfig.Config.GUIElementsToShow = this.GUIElementsToShow;
        //    this.bdrGUIProfile.Visibility = System.Windows.Visibility.Collapsed;
        //}

        //private void btnIgnoreGUIProfileSelection_Click(object sender, RoutedEventArgs e)
        //{
        //    this.bdrGUIProfile.Visibility = System.Windows.Visibility.Collapsed;
        //}

        private void cmbGUIProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var cbi = (ComboBoxItem)e.AddedItems[0];
                GUIProfile gp = (GUIProfile)Convert.ToInt32(cbi.Tag);
                this.Profile = gp;
                this.GUIElementsToShow = HPTConfig.Config.GetElementsToShow(this.Profile);
                ApplyGUIElementsToShow(this.GUIElementsToShow);
                ApplyProfile(this.Profile);
            }
        }

        internal void ApplyProfile(GUIProfile profile)
        {
            HPTConfig.Config.SetMarkBetProfile(profile);
            this.tcMarksGame.Items.Clear();
            HPTConfig.Config.MarkBetTabsToShow.PropertyChanged -= new PropertyChangedEventHandler(MarkBetTabsToShow_PropertyChanged);
            this.MarkBet.RaceDayInfo.DataToShow = HPTConfig.Config.DataToShowVxx;
            CreateMarkBetTabsToShowContextMenu();
            HPTConfig.Config.MarkBetTabsToShow.PropertyChanged += new PropertyChangedEventHandler(MarkBetTabsToShow_PropertyChanged);
            CreateTabsToShow();
        }

        internal void ApplyGUIElementsToShow(HPTGUIElementsToShow guiElementsToShow)
        {
            // Översikt
            this.gbOverview.Visibility = GetVisibility(guiElementsToShow.ShowOverview);
            this.RaceLockVisibility = GetVisibility(guiElementsToShow.ShowRaceLock);

            // Inställningar
            this.txtReservHandling.Visibility = GetVisibility(guiElementsToShow.ShowReservHandling);
            this.cmbReservHandling.Visibility = GetVisibility(guiElementsToShow.ShowReservHandling);

            this.txtBetMultiplier.Visibility = GetVisibility(guiElementsToShow.ShowBetMultiplier);
            this.iudBetMultiplier.Visibility = GetVisibility(guiElementsToShow.ShowBetMultiplier);

            //this.chkCouponCompression.Visibility = GetVisibility(guiElementsToShow.ShowCouponCompression);
            //this.cmbCouponCompression.Visibility = GetVisibility(guiElementsToShow.ShowCouponCompression);

            // V6/V7/V8
            if (this.MarkBet.BetType.NumberOfRaces > 5)
            {
                this.chkV6.Visibility = GetVisibility(guiElementsToShow.ShowV6);
            }
            else
            {
                this.chkV6.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.txtBetMultiplier.Visibility = GetVisibility(guiElementsToShow.ShowBetMultiplier);
            this.iudBetMultiplier.Visibility = GetVisibility(guiElementsToShow.ShowBetMultiplier);

            this.chkAutomaticRecalculation.Visibility = GetVisibility(guiElementsToShow.ShowAutomaticCalculation);

            // Mallar
            this.gbTemplates.Visibility = GetVisibility(guiElementsToShow.ShowTemplates);

            // Systeminformation
            this.grdReductionPercentage.Visibility = GetVisibility(guiElementsToShow.ShowReductionPercentage);
            this.grdCouponInfo.Visibility = GetVisibility(guiElementsToShow.ShowCouponInfo);
            this.grdNumberOfSystems.Visibility = GetVisibility(guiElementsToShow.ShowNumberOfSystems);
            this.grdNumberOfGambledRows.Visibility = GetVisibility(guiElementsToShow.ShowNumberOfGambledRows);
            this.grdSystemCostChange.Visibility = GetVisibility(guiElementsToShow.ShowSystemCostChange);
            this.spRowValueInterval.Visibility = GetVisibility(guiElementsToShow.ShowRowValueInterval);
            this.grdLiveCalculation.Visibility = GetVisibility(guiElementsToShow.ShowLiveCalculation);

            // Reducering
            this.gbReductionList.Visibility = GetVisibility(guiElementsToShow.ShowReductionList);

            // Fil
            this.btnCreateCouponsAs.Visibility = GetVisibility(guiElementsToShow.ShowSaveAs);
            this.btnCopy.Visibility = GetVisibility(guiElementsToShow.ShowCopy);
            this.btnPrint.Visibility = GetVisibility(guiElementsToShow.ShowPrint);
            this.btnClearAll.Visibility = GetVisibility(guiElementsToShow.ShowClear);
            //this.btnUploadSystem.Visibility = GetVisibility(guiElementsToShow.ShowUpload);
        }

        private Visibility GetVisibility(bool show)
        {
            if (show)
            {
                return Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;
        }



        public Visibility RaceLockVisibility
        {
            get { return (Visibility)GetValue(RaceLockVisibilityProperty); }
            set { SetValue(RaceLockVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RaceLockVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RaceLockVisibilityProperty =
            DependencyProperty.Register("RaceLockVisibility", typeof(Visibility), typeof(UCMarksGame), new UIPropertyMetadata(Visibility.Visible));



        #endregion

        //private void chkUseABCD_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (gbReductionList == null || this.gbReductionList.Visibility != System.Windows.Visibility.Visible)
        //    {
        //        this.MarkBet.RecalculateReduction(RecalculateReason.All);
        //    }
        //}

        #region Skapa tabbar

        internal void CreateTabsToShow()
        {
            if (HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder == null || HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Count == 0)
            {
                HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder = new List<string>()
                {
                    "tiRaces", "tiOverview", "tiRacesGrouped", "tiRankOverview", "tiDrivers", "tiTrainers", "tiTrends", "tiComments", "tiRankReduction", "tiSingleRows", "tiComplimentaryRules", "tiIntervalReduction", "tiGroupInterval", "tiMultiABCD", "tiV6BetMultiplier", "tiCorrection", "tiCompanyGambling", "tiXmlFile", "tiTemplateWorkshop"
                };
            }

            // Avdelningar
            if (HPTConfig.Config.MarkBetTabsToShow.ShowRaces && (this.tiRaces == null || !this.tcMarksGame.Items.Contains(this.tiRaces)))
            {
                CreateRacesTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowRaces && this.tiRaces != null)
            {
                this.tcMarksGame.Items.Remove(this.tiRaces);
                this.tiRaces = null;
            }
            HandleTabItemOrder(this.tiRaces, "tiRaces");

            // Översikt
            if (HPTConfig.Config.MarkBetTabsToShow.ShowOverview && (this.tiOverview == null || !this.tcMarksGame.Items.Contains(this.tiOverview)))
            {
                CreateOverviewTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowOverview && this.tiOverview != null)
            {
                this.tcMarksGame.Items.Remove(this.tiOverview);
                this.tiOverview = null;
            }
            HandleTabItemOrder(this.tiOverview, "tiOverview");

            // Avdelningar (grupperade)
            if (HPTConfig.Config.MarkBetTabsToShow.ShowRacesGrouped && (this.tiRacesGrouped == null || !this.tcMarksGame.Items.Contains(this.tiRacesGrouped)))
            {
                CreateRacesGroupedTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowRacesGrouped && this.tiRacesGrouped != null)
            {
                this.tcMarksGame.Items.Remove(this.tiRacesGrouped);
                this.tiRacesGrouped = null;
            }
            HandleTabItemOrder(this.tiRacesGrouped, "tiRacesGrouped");

            // Ranköversikt
            if (HPTConfig.Config.MarkBetTabsToShow.ShowRankOverview && (this.tiRankOverview == null || !this.tcMarksGame.Items.Contains(this.tiRankOverview)))
            {
                CreateRankOverviewTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowRankOverview && this.tiRankOverview != null)
            {
                this.tcMarksGame.Items.Remove(this.tiRankOverview);
                this.tiRankOverview = null;
            }
            HandleTabItemOrder(this.tiRankOverview, "tiRankOverview");

            // Kuskar
            if (HPTConfig.Config.MarkBetTabsToShow.ShowDriverReduction && (this.tiDrivers == null || !this.tcMarksGame.Items.Contains(this.tiDrivers)))
            {
                CreateDriversTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowDriverReduction && this.tiDrivers != null)
            {
                this.tcMarksGame.Items.Remove(this.tiDrivers);
                this.tiDrivers = null;
            }
            HandleTabItemOrder(this.tiDrivers, "tiDrivers");

            // Tränare
            if (HPTConfig.Config.MarkBetTabsToShow.ShowTrainerReduction && (this.tiTrainers == null || !this.tcMarksGame.Items.Contains(this.tiTrainers)))
            {
                CreateTrainersTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowTrainerReduction && this.tiTrainers != null)
            {
                this.tcMarksGame.Items.Remove(this.tiTrainers);
                this.tiTrainers = null;
            }
            HandleTabItemOrder(this.tiTrainers, "tiTrainers");

            // Trender
            if (HPTConfig.Config.MarkBetTabsToShow.ShowTrends && (this.tiTrends == null || !this.tcMarksGame.Items.Contains(this.tiTrends)))
            {
                CreateTrendsTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowTrends && this.tiTrends != null)
            {
                this.tcMarksGame.Items.Remove(this.tiTrends);
                this.tiTrends = null;
            }
            HandleTabItemOrder(this.tiTrends, "tiTrends");

            // Villkorsstatistik
            if (HPTConfig.Config.MarkBetTabsToShow.ShowReductionStatistics && (this.tiReductionStatistics == null || !this.tcMarksGame.Items.Contains(this.tiReductionStatistics)))
            {
                CreateReductionStatisticsTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowReductionStatistics && this.tiReductionStatistics != null)
            {
                this.tcMarksGame.Items.Remove(this.tiReductionStatistics);
                this.tiReductionStatistics = null;
            }
            HandleTabItemOrder(this.tiReductionStatistics, "tiReductionStatistics");

            // Kommentarer
            if (HPTConfig.Config.MarkBetTabsToShow.ShowComments && (this.tiComments == null || !this.tcMarksGame.Items.Contains(this.tiComments)))
            {
                CreateCommentsTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowComments && this.tiComments != null)
            {
                this.tcMarksGame.Items.Remove(this.tiComments);
                this.tiComments = null;
            }
            HandleTabItemOrder(this.tiComments, "tiComments");

            // Rankreducering
            if (HPTConfig.Config.MarkBetTabsToShow.ShowRankReduction && (this.tiRankReduction == null || !this.tcMarksGame.Items.Contains(this.tiRankReduction)))
            {
                CreateRankReductionTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowRankReduction && this.tiRankReduction != null)
            {
                this.tcMarksGame.Items.Remove(this.tiRankReduction);
                this.tiRankReduction = null;
            }
            HandleTabItemOrder(this.tiRankReduction, "tiRankReduction");

            // Rankreducering
            if (HPTConfig.Config.MarkBetTabsToShow.ShowSingleRows && (this.tiSingleRows == null || !this.tcMarksGame.Items.Contains(this.tiSingleRows)))
            {
                CreateSingleRowsTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowSingleRows && this.tiSingleRows != null)
            {
                this.tcMarksGame.Items.Remove(this.tiSingleRows);
                this.tiSingleRows = null;
            }
            HandleTabItemOrder(this.tiSingleRows, "tiSingleRows");

            // Utgångar
            if (HPTConfig.Config.MarkBetTabsToShow.ShowComplimentaryRules && (this.tiComplimentaryRules == null || !this.tcMarksGame.Items.Contains(this.tiComplimentaryRules)))
            {
                CreateComplimentaryRulesTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowComplimentaryRules && this.tiComplimentaryRules != null)
            {
                this.tcMarksGame.Items.Remove(this.tiComplimentaryRules);
                this.tiComplimentaryRules = null;
            }
            HandleTabItemOrder(this.tiComplimentaryRules, "tiComplimentaryRules");

            // Intervall
            if (HPTConfig.Config.MarkBetTabsToShow.ShowIntervalReduction && (this.tiIntervalReduction == null || !this.tcMarksGame.Items.Contains(this.tiIntervalReduction)))
            {
                CreateIntervalReductionTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowIntervalReduction && this.tiIntervalReduction != null)
            {
                this.tcMarksGame.Items.Remove(this.tiIntervalReduction);
                this.tiIntervalReduction = null;
            }
            HandleTabItemOrder(this.tiIntervalReduction, "tiIntervalReduction");

            // Gruppintervall
            if (HPTConfig.Config.MarkBetTabsToShow.ShowGroupIntervalReduction && (this.tiGroupInterval == null || !this.tcMarksGame.Items.Contains(this.tiGroupInterval)))
            {
                CreateGroupIntervalTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowGroupIntervalReduction && this.tiGroupInterval != null)
            {
                this.tcMarksGame.Items.Remove(this.tiGroupInterval);
                this.tiGroupInterval = null;
            }
            HandleTabItemOrder(this.tiGroupInterval, "tiGroupInterval");

            // Multi-ABCD
            if (HPTConfig.Config.MarkBetTabsToShow.ShowMultiABCD && (this.tiMultiABCD == null || !this.tcMarksGame.Items.Contains(this.tiMultiABCD)))
            {
                CreateMultiABCDTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowMultiABCD && this.tiMultiABCD != null)
            {
                this.tcMarksGame.Items.Remove(this.tiMultiABCD);
                this.tiMultiABCD = null;
            }
            HandleTabItemOrder(this.tiMultiABCD, "tiMultiABCD");

            // V6/V7/V8/Flerbong
            if (HPTConfig.Config.MarkBetTabsToShow.ShowV6BetMultiplier && (this.tiV6BetMultiplier == null || !this.tcMarksGame.Items.Contains(this.tiV6BetMultiplier)))
            {
                CreateV6BetMultiplierTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowV6BetMultiplier && this.tiV6BetMultiplier != null)
            {
                this.tcMarksGame.Items.Remove(this.tiV6BetMultiplier);
                this.tiV6BetMultiplier = null;
            }
            HandleTabItemOrder(this.tiV6BetMultiplier, "tiV6BetMultiplier");

            // Kuponger/Rättning
            if (HPTConfig.Config.MarkBetTabsToShow.ShowCorrection && (this.tiCorrection == null || !this.tcMarksGame.Items.Contains(this.tiCorrection)))
            {
                CreateCorrectionTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowCorrection && this.tiCorrection != null)
            {
                this.tcMarksGame.Items.Remove(this.tiCorrection);
                this.tiCorrection = null;
            }
            HandleTabItemOrder(this.tiCorrection, "tiCorrection");

            //// Bolagsspel
            //if (HPTConfig.Config.MarkBetTabsToShow.ShowCompanyGambling && (this.tiCompanyGambling == null || !this.tcMarksGame.Items.Contains(this.tiCompanyGambling)))
            //{
            //    CreateCompanyGamblingTabItem();
            //}
            //else if (!HPTConfig.Config.MarkBetTabsToShow.ShowCompanyGambling && this.tiCompanyGambling != null)
            //{
            //    this.tcMarksGame.Items.Remove(this.tiCompanyGambling);
            //    this.tiCompanyGambling = null;
            //}
            //HandleTabItemOrder(this.tiCompanyGambling, "tiCompanyGambling");

            //// XML-fil
            //if (HPTConfig.Config.MarkBetTabsToShow.ShowATGXmlFile && (this.tiXmlFile == null || !this.tcMarksGame.Items.Contains(this.tiXmlFile)))
            //{
            //    CreateXmlFileTabItem();
            //}
            //else if (!HPTConfig.Config.MarkBetTabsToShow.ShowATGXmlFile && this.tiXmlFile != null)
            //{
            //    this.tcMarksGame.Items.Remove(this.tiXmlFile);
            //    this.tiXmlFile = null;
            //}
            //HandleTabItemOrder(this.tiXmlFile, "tiXmlFile");

            // Mallverkstad
            if (HPTConfig.Config.MarkBetTabsToShow.ShowTemplateWorkshop && (this.tiTemplateWorkshop == null || !this.tcMarksGame.Items.Contains(this.tiTemplateWorkshop)))
            {
                CreateTemplateWorkshopTabItem();
            }
            else if (!HPTConfig.Config.MarkBetTabsToShow.ShowTemplateWorkshop && this.tiTemplateWorkshop != null)
            {
                this.tcMarksGame.Items.Remove(this.tiTemplateWorkshop);
                this.tiTemplateWorkshop = null;
            }
            HandleTabItemOrder(this.tiTemplateWorkshop, "tiTemplateWorkshop");

            // Sortera tabbarna
            foreach (var tabName in HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder)
            {
                if (!string.IsNullOrEmpty(tabName))
                {
                    object o = this.GetType().GetProperty(tabName)?.GetValue(this);

                    if (o != null && o.GetType() == typeof(TabItem))
                    {
                        var tabItem = (TabItem)o;
                        if (!this.tcMarksGame.Items.Contains(tabItem))
                        {
                            this.tcMarksGame.Items.Add(tabItem);
                        }
                    }
                }
            }
        }

        internal void HandleTabItemOrder(TabItem tabItem, string tabName)
        {
            if (tabItem == null)
            {
                HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Remove(tabName);
            }
            else if (string.IsNullOrEmpty(tabName))
            {
                return;
            }
            else
            {
                if (!HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Contains(tabName))
                {
                    HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Add(tabName);
                }
            }
        }

        public TabItem tiRaces { get; set; }
        internal void CreateRacesTabItem()
        {
            this.tiRaces = new TabItem()
            {
                Name = "tiRaces",
                Header = new UCTabItemHeader()
                {
                    Text = "Avdelningar",
                    TextColor = new SolidColorBrush(Colors.DarkGreen),
                    ToolTip = "Visar vy med detaljinformation om samtliga avdelningar"
                },
                //ToolTip = "Visar vy med detaljinformation om samtliga avdelningar",
                Content = new UCRacesTabControl()
                {
                    MarkBet = this.MarkBet
                }
            };
        }

        public TabItem tiReductionStatistics { get; set; }
        internal void CreateReductionStatisticsTabItem()
        {
            this.tiReductionStatistics = new TabItem()
            {
                Name = "tiReductionStatistics",
                Header = new UCTabItemHeader()
                {
                    Text = "Villkorsstatistik",
                    TextColor = new SolidColorBrush(Colors.DarkGreen),
                    ToolTip = "Visar vy med statistik om reduceringsvillkoren"
                },
                Content = new UCReductionRuleStatistics()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiRacesGrouped { get; set; }
        internal void CreateRacesGroupedTabItem()
        {
            this.tiRacesGrouped = new TabItem()
            {
                Name = "tiRacesGrouped",
                Header = new UCTabItemHeader()
                {
                    //Text = "Avdelningar (grupperat)",
                    ToolTip = "Visar vy med egen rank/poäng/chansvärdering och tillhärande reduceringsvillkor",
                    Text = "Poäng",
                    TextColor = new SolidColorBrush(Colors.DarkGreen)
                },
                Content = new UCOwnRankSumReduction()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiRankOverview { get; set; }
        internal void CreateRankOverviewTabItem()
        {
            this.tiRankOverview = new TabItem()
            {
                Name = "tiRankOverview",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Visar översikt med alla hästar i alla lopp",
                    Text = "Ranköversikt",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCRankOverview()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiOverview { get; set; }
        internal void CreateOverviewTabItem()
        {
            this.tiOverview = new TabItem()
            {
                Name = "tiOverview",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Visar vy med översiktlig information om samtliga startande hästar på en skärmbild",
                    Text = "Översikt",
                    TextColor = new SolidColorBrush(Colors.DarkGreen)
                },
                Content = new UCRacesOverview()
                {
                    MarkBet = this.MarkBet
                }
            };
        }

        public TabItem tiDrivers { get; set; }
        internal void CreateDriversTabItem()
        {
            this.tiDrivers = new TabItem()
            {
                Name = "tiDrivers",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik för att skapa reduceringsregler utifrån kuskar",
                    Text = "Kuskar",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCPersonReduction()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet.DriverRulesCollection
                }
            };
        }

        public TabItem tiTrainers { get; set; }
        internal void CreateTrainersTabItem()
        {
            this.tiTrainers = new TabItem()
            {
                Name = "tiTrainers",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik för att skapa reduceringsregler utifrån tränare",
                    Text = "Tränare",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCPersonReduction()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet.TrainerRulesCollection
                }
            };
        }

        public TabItem tiTrends { get; set; }
        internal void CreateTrendsTabItem()
        {
            this.tiTrends = new TabItem()
            {
                Name = "tiTrends",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Visar vy med trender för varje häst gällande insatsfördelning, vinnarodds osv.",
                    Text = "Trender",
                    TextColor = new SolidColorBrush(Colors.DarkRed)
                },
                Content = new UCTrends()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiComments { get; set; }
        internal void CreateCommentsTabItem()
        {
            this.tiComments = new TabItem()
            {
                Name = "tiComments",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Visar vy med översiktlig information om samtliga startande hästar på en skärmbild",
                    Text = "Kommentarer",
                    TextColor = new SolidColorBrush(Colors.DarkRed)
                },
                Content = new UCHorseCommentView()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiRankReduction { get; set; }
        internal void CreateRankReductionTabItem()
        {
            this.tiRankReduction = new TabItem()
            {
                Name = "tiRankReduction",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Rankreducering för alla rankvariabler",
                    Text = "Rankreducering",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCHorseRankSumReduction()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiSingleRows { get; set; }
        internal void CreateSingleRowsTabItem()
        {
            this.tiSingleRows = new TabItem()
            {
                Name = "tiSingleRows",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Visar alla enkelrader i reducerat system utifrån gjorda val",
                    Text = "Enkelrader",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCSingleRowCollectionView()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiComplimentaryRules { get; set; }
        internal void CreateComplimentaryRulesTabItem()
        {
            this.tiComplimentaryRules = new TabItem()
            {
                Name = "tiComplimentaryRules",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Visar alla valda hästar i samma lista och har funktionalitet för att skapa utgångsvillkor.",
                    Text = "Utgångar/Valda hästar",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCComplimentaryRules()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiIntervalReduction { get; set; }
        internal void CreateIntervalReductionTabItem()
        {
            this.tiIntervalReduction = new TabItem()
            {
                Name = "tiIntervalReduction",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Reducera utifrån radvärde, spårsumma mm",
                    Text = "Intervall",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCIntervalReductionNew()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiGroupInterval { get; set; }
        internal void CreateGroupIntervalTabItem()
        {
            this.tiGroupInterval = new TabItem()
            {
                Name = "tiGroupInterval",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik för att skapa reduceringsregler utifrån vinnarfördelning av exempelvis insatsfördelning",
                    Text = "Gruppintervall",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCGroupIntervalReduction()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiMultiABCD { get; set; }
        internal void CreateMultiABCDTabItem()
        {
            this.tiMultiABCD = new TabItem()
            {
                Name = "tiMultiABCD",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik för att skapa ELLER-villkor med flera olika ABCD-reduceringar",
                    Text = "Multi-ABCD",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = new UCMultiABCD()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiV6BetMultiplier { get; set; }
        internal void CreateV6BetMultiplierTabItem()
        {
            this.tiV6BetMultiplier = new TabItem()
            {
                Name = "tiV6BetMultiplier",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik för att spela V6/Flerbong utifrån en eller flera hästar",
                    //Text = "V6/V7/V8/Flerbong",
                    Text = "V6/Flerbong",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                ToolTip = "Flik för att spela V6/Flerbong utifrån en eller flera hästar",
                Content = new UCV6BetMultiplier()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiCorrection { get; set; }
        internal void CreateCorrectionTabItem()
        {
            this.tiCorrection = new TabItem()
            {
                Name = "tiCorrection",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik där man kan se och rätta kupongerna som skapats utifrån gjorda reduceringsval",
                    Text = "Kuponger/Rättning",
                    TextColor = new SolidColorBrush(Colors.DarkRed)
                },
                Content = new UCCorrectionMarkingBet()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet.CouponCorrector,
                    Name = "ucCorrection"
                }
            };
        }

        //public TabItem tiCompanyGambling { get; set; }
        //internal void CreateCompanyGamblingTabItem()
        //{
        //    this.tiCompanyGambling = new TabItem()
        //    {
        //        Name = "tiCompanyGambling",
        //        Header = new UCTabItemHeader()
        //        {
        //            ToolTip = "Flik med funktionalitet för att skicka mail och ladda upp system till HPTs server.",
        //            Text = "Bolagsspel",
        //            TextColor = new SolidColorBrush(Colors.DarkRed)
        //        },
        //        Content = new UCSendMail()
        //        {
        //            MarkBet = this.MarkBet,
        //            DataContext = this.MarkBet,
        //            Name = "ucSendMail"
        //        }
        //    };
        //}

        public TabItem tiXmlFile { get; set; }
        internal WebBrowser wbXmlFile;
        internal void CreateXmlFileTabItem()
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            grid.RowDefinitions.Add(new RowDefinition());

            var tb = new TextBlock();
            tb.SetValue(Grid.RowProperty, 0);
            tb.SetBinding(TextBlock.TextProperty, "MarkBet.SystemFilename");
            grid.Children.Add(tb);

            this.wbXmlFile = new WebBrowser();
            this.wbXmlFile.SetValue(Grid.RowProperty, 1);
            grid.Children.Add(this.wbXmlFile);

            this.tiXmlFile = new TabItem()
            {
                Name = "tiXmlFile",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik där du kan titta på den råa XML-filen som laddas upp till ATG.",
                    Text = "XML-fil till ATG",
                    TextColor = new SolidColorBrush(Colors.DarkRed)
                },
                Content = grid
            };
        }

        public TabItem tiTemplateWorkshop { get; set; }
        internal void CreateTemplateWorkshopTabItem()
        {
            this.tiTemplateWorkshop = new TabItem()
            {
                Name = "tiTemplateWorkshop",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik där man kan tweaka rankvariabelmallar och direkt se hur det slår igenom på valda hästar",
                    Text = "Mallverkstad",
                    TextColor = new SolidColorBrush(Colors.DarkRed)
                },
                Content = new UCTemplateWorkshop()
                {
                    MarkBet = this.MarkBet,
                    DataContext = this.MarkBet
                }
            };
        }

        public TabItem tiATG { get; set; }
        internal WebBrowser wbATG;
        internal void CreateATGTabItem()
        {
            this.wbATG = new WebBrowser();
            this.tiATG = new TabItem()
            {
                Name = "tiATG",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik där du kan titta på resultat och video på atg.se",
                    Text = "ATG",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = this.wbATG
            };
            this.tcMarksGame.Items.Add(this.tiATG);
        }

        public TabItem tiST { get; set; }
        internal WebBrowser wbST;
        internal void CreateSTTabItem()
        {
            this.wbST = new WebBrowser();
            this.tiST = new TabItem()
            {
                Name = "tiST",
                Header = new UCTabItemHeader()
                {
                    ToolTip = "Flik där du kan titta på hästinformation på svensktravsport.se",
                    Text = "Svensk Travsport",
                    TextColor = new SolidColorBrush(Colors.DarkBlue)
                },
                Content = this.wbST
            };
            this.tcMarksGame.Items.Add(this.tiST);
        }

        #endregion

        #region Ta bort/lägg till rader från kupongfil

        internal void RemoveRowsFromFile()
        {
            var ofdRemoveRowsFromFile = new OpenFileDialog();
            ofdRemoveRowsFromFile.InitialDirectory = this.MarkBet.SaveDirectory;
            ofdRemoveRowsFromFile.Filter = "ATG-kupongfiler|*.xml";
            ofdRemoveRowsFromFile.FileOk += ofdRemoveRowsFromFile_FileOk;
            ofdRemoveRowsFromFile.ShowDialog();

        }

        void ofdRemoveRowsFromFile_FileOk(object sender, CancelEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }
            var ofd = (OpenFileDialog)sender;
            try
            {
                this.MarkBet.RemoveRowsInFileFromSystem(ofd.FileName);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal void AddRowsFromFile()
        {
            var ofdAddRowsFromFile = new OpenFileDialog();
            ofdAddRowsFromFile.InitialDirectory = this.MarkBet.SaveDirectory;
            ofdAddRowsFromFile.Filter = "ATG-kupongfiler|*.xml";
            ofdAddRowsFromFile.FileOk += ofdAddRowsFromFile_FileOk;
            ofdAddRowsFromFile.ShowDialog();
        }

        internal void CalculateJackpotRisk()
        {
            try
            {
                Cursor = Cursors.Wait;
                string result = this.MarkBet.CalculateJackpotRows();
                MessageBox.Show(result, "Jackpottrisk", MessageBoxButton.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Något gick fel vid beräkning!", "Fel vid beräkning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Cursor = Cursors.Arrow;
        }

        internal void CalculateNumberOfSingleRows()
        {
            try
            {
                Cursor = Cursors.Wait;
                int result = this.MarkBet.CalculateNumberOfSingleRows();
                MessageBox.Show(result.ToString(), " ensamma rader", MessageBoxButton.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Något gick fel vid beräkning!", "Fel vid beräkning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Cursor = Cursors.Arrow;
        }

        void ofdAddRowsFromFile_FileOk(object sender, CancelEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }
            var ofd = (OpenFileDialog)sender;
            try
            {
                this.MarkBet.AddRowsInFileToSystem(ofd.FileName);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #endregion

        #region Sortera tabbar

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var tabItem = e.Source as TabItem;

                if (tabItem == null)
                    return;

                if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Source.GetType() != typeof(UCTabItemHeader))
                {
                    return;
                }
                var tabItemHeaderTarget = e.Source as UCTabItemHeader;
                var tabItemTarget = tabItemHeaderTarget.Parent as TabItem;
                var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

                if (!tabItemTarget.Equals(tabItemSource))
                {
                    var tabControl = tabItemTarget.Parent as TabControl;
                    int sourceIndex = tabControl.Items.IndexOf(tabItemSource);

                    tabControl.Items.Remove(tabItemSource);
                    int targetIndex = tabControl.Items.IndexOf(tabItemTarget);
                    if (targetIndex >= sourceIndex)
                    {
                        targetIndex++;
                    }
                    tabControl.Items.Insert(targetIndex, tabItemSource);


                    //tabControl.Items.Remove(tabItemTarget);
                    //tabControl.Items.Insert(sourceIndex, tabItemTarget);
                    tabControl.SelectedIndex = targetIndex;
                    CreateColumnOrderList();
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal void CreateColumnOrderList()
        {
            HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Clear();
            foreach (TabItem tabItem in this.tcMarksGame.Items)
            {
                if (!string.IsNullOrEmpty(tabItem.Name))
                {
                    HPTConfig.Config.MarkBetTabsToShow.ColumnsInOrder.Add(tabItem.Name);
                }
            }
        }

        #endregion


        //private void miCalculateProbabilities_Click(object sender, RoutedEventArgs e)
        //{
        //    string header = Enumerable.Range(0, this.MarkBet.NumberOfRaces + 1)
        //        .Select(nr => nr.ToString())
        //        .Aggregate((nr, next) => nr + "\t" + next);

        //    var sb = new StringBuilder(header);
        //    sb.AppendLine();
        //    foreach (var xReductionRule in this.MarkBet.ABCDEFReductionRule.XReductionRuleList)
        //    {
        //        sb.Append(xReductionRule.Prio);
        //        sb.Append("-Hästar\t");
        //        string result = this.MarkBet.CalculateBestABCDCombination(xReductionRule.Prio);
        //        sb.AppendLine(result);
        //    }
        //    Clipboard.SetText(sb.ToString());
        //}

        private void miUploadOnlineSystem_Click(object sender, RoutedEventArgs e)
        {
            var serviceConnector = new HPTServiceConnector();
            serviceConnector.UploadSystemOnline(this.MarkBet);
        }

        private void tcMarksGame_RequestNavigate(object sender, RoutedEventArgs e)
        {
            try
            {
                var hl = (Hyperlink)e.OriginalSource;
                string url = hl.NavigateUri.AbsoluteUri;

                // Länk till atg.se, normalt sett resultat
                if (url.Contains("atg.se"))
                {
                    if (this.wbATG == null)
                    {
                        CreateATGTabItem();
                    }
                    this.wbATG.Navigate(hl.NavigateUri);
                    this.tcMarksGame.SelectedItem = this.tiATG;
                    return;
                }

                // Länk till utökad hästinformation på 
                if (url.Contains("travsport.se"))
                {
                    if (this.wbST == null)
                    {
                        CreateSTTabItem();
                    }
                    this.wbST.Navigate(hl.NavigateUri);
                    this.tcMarksGame.SelectedItem = this.tiST;
                    return;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void miSaveAndGoToATG_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.btnCreateCoupons.IsOpen = false;
                SaveFiles(true, true, true);
                Clipboard.SetDataObject(this.MarkBet.SystemFilename);
                if (this.wbATG == null)
                {
                    CreateATGTabItem();
                }
                this.wbATG.Navigate("https://www.atg.se/spel/fil");
                this.tcMarksGame.SelectedItem = this.tiATG;
            }
            catch (Exception exc)
            {
                string fel = exc.Message;
            }
        }
    }
}

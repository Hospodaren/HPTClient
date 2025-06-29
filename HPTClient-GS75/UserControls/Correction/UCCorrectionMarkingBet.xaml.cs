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
using System.ComponentModel;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCCorrectionMarkingBet.xaml
    /// </summary>
    public partial class UCCorrectionMarkingBet : UCMarkBetControl
    {
        System.Threading.Timer tmrCorrection;

        public UCCorrectionMarkingBet()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.tmrCorrection = new System.Threading.Timer(new System.Threading.TimerCallback(AutoCorrect));
                this.HorseListRemainingRows = new ObservableCollection<HPTHorse>();
                this.SingleRowsCorrect = new List<HPTMarkBetSingleRow>();
                InitializeComponent();

                //if (!HPTConfig.Config.IsPayingCustomer)
                //{
                //    this.chkLockCoupons.IsEnabled = false;
                //    this.chkSimulate.IsEnabled = false;
                //    this.btnAnalyzeResult.IsEnabled = false;
                //}
            }
        }

        public List<HPTMarkBetSingleRow> SingleRowsCorrect { get; set; }

        public ObservableCollection<HPTHorse> HorseListRemainingRows { get; set; }

        public HPTCouponCorrector CouponCorrector
        {
            get { return (HPTCouponCorrector)GetValue(CouponCorrectorProperty); }
            set { SetValue(CouponCorrectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CouponCorrector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CouponCorrectorProperty =
            DependencyProperty.Register("CouponCorrector", typeof(HPTCouponCorrector), typeof(UCCorrectionMarkingBet), new UIPropertyMetadata(null));



        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            String field = column.Tag as String;

            ListSortDirection newDir = ListSortDirection.Ascending;

            switch (field)
            {
                case "NumberOfCorrect":
                    newDir = ListSortDirection.Descending;
                    break;
                default:
                    break;
            }
            this.lvwCoupons.Items.SortDescriptions.Clear();
            this.lvwCoupons.Items.SortDescriptions.Add(new SortDescription(field, newDir));
        }

        private void SortByNumberOfCorrect()
        {
            this.lvwCoupons.Items.SortDescriptions.Clear();
            this.lvwCoupons.Items.SortDescriptions.Add(new SortDescription("NumberOfCorrect", ListSortDirection.Descending));
        }

        private void btnCorrect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.numberOfFinishedRaces = 0;
                AutoCorrectOptimized();
                //UpdateResult();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Problem vid rättning av kuponger:\r\nObservera att endast V4, V5, V64, V65 och V75 som är mindre än en vecka gamla kan automaträttas i nuläget.\r\n" + exc.Message, "Rättning av kuponger misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
                HPTConfig.Config.AddToErrorLog(exc);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                this.CouponCorrector = (HPTCouponCorrector)this.DataContext;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        //private void cmbUpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (this.cmbUpdateInterval.SelectedItem != null)
        //    {
        //        ComboBoxItem cbi = (ComboBoxItem)this.cmbUpdateInterval.SelectedItem;
        //        int updatePeriod = Convert.ToInt32(cbi.Tag) * 60 * 1000;
        //        if (updatePeriod == 0)
        //        {
        //            this.automaticCorrection = false;
        //            this.tmrCorrection.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        //        }
        //        else
        //        {
        //            this.automaticCorrection = true;
        //            this.tmrCorrection.Change(0, updatePeriod);
        //        }
        //    }
        //}

        private void AutoCorrect(object timerData)
        {
            this.Dispatcher.Invoke(new Action(AutoCorrectOptimized), null);
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            {
                try
                {
                    if (this.HorseListRemainingRows.Count == 0 && this.numberOfFinishedRaces == 0)
                    {
                        this.MarkBet.RaceDayInfo.RaceList
                            .First()
                            .HorseListSelected
                            .ForEach(h =>
                            {
                                this.HorseListRemainingRows.Add(h);
                            });

                        this.MarkBet.RaceDayInfo.RaceList
                            .SelectMany(r => r.HorseList)
                            .ToList()
                            .ForEach(h =>
                            {
                                h.NumberOfRowsWithAllCorrect = h.NumberOfCoveredRows;
                                h.NumberOfRowsWithPossibility = "(" + h.NumberOfRowsWithAllCorrect.ToString();
                                if (h.Selected)
                                {
                                    if (this.MarkBet.BetType.PayOutDummyList.Length > 1)
                                    {
                                        h.NumberOfRowsWithPossibility += "/0";
                                        if (this.MarkBet.BetType.PayOutDummyList.Length > 2)
                                        {
                                            h.NumberOfRowsWithPossibility += "/0";
                                        }
                                    }
                                    h.NumberOfRowsWithPossibility += ")";
                                }
                                else
                                {
                                    h.NumberOfRowsWithPossibility = string.Empty;
                                }
                            });
                    }
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
            }
        }

        //private bool correctionInProgress = false;
        internal void UpdateResult()
        {
            if ((bool)this.chkSimulate.IsChecked)
            {
                this.chkSimulate.IsChecked = false;
                System.Threading.Thread.Sleep(200);
            }
            this.isSneakCorrection = false;
            this.numberOfFinishedRaces = this.MarkBet.RaceDayInfo.NumberOfFinishedRaces;
            if (this.MarkBet.RaceDayInfo.ResultComplete && this.MarkBet.RaceDayInfo.PayOutList.Count > 0 && this.winnerList == null)    // Vafan gör jag här...?
            {
                //this.chkAutomaticCorrection.IsChecked = false;
                Correct();
            }
            else
            {
                //this.correctionInProgress = true;
                HPTServiceConnector connector = new HPTServiceConnector();
                connector.GetResultMarkingBetByTrackAndDate(this.CouponCorrector.RaceDayInfo.BetType.Code, this.CouponCorrector.RaceDayInfo.TrackId, this.CouponCorrector.RaceDayInfo.RaceDayDate, ReceiveResult); 
            }
        }

        public void ReceiveResult(HPTService.HPTResultMarkingBet result)
        {            
            Dispatcher.Invoke(new Action<HPTService.HPTResultMarkingBet>(ReceiveResultInvoke), result);
        }

        public void ReceiveResultInvoke(HPTService.HPTResultMarkingBet result)
        {
            try
            {
                if (result.TrackId == 0)
                {
                    //this.txtError.Text = DateTime.Now.ToShortTimeString() +
                    //                     " - Rättning misslyckades, se fellogg för mer info.";
                    return;
                }
                this.txtError.Text = string.Empty;
                HPTServiceToHPTHelper.ConvertResultMarkingBet(result, this.CouponCorrector.RaceDayInfo, true);

                Correct();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            //this.correctionInProgress = false;
        }

        private int numberOfFinishedRaces;
        private void Correct()
        {
            try
            {
                if (this.CouponCorrector.RaceDayInfo == null)
                {
                    this.CouponCorrector.RaceDayInfo = this.MarkBet.RaceDayInfo;
                }
                if (this.winnerList == null && this.CouponCorrector.RaceDayInfo.WinnerList != null)
                {
                    this.winnerList = this.CouponCorrector.RaceDayInfo.WinnerList;
                    this.btnWinnerList.IsEnabled = true;
                }

                // Hämta info om strykningar och resultat utan att uppdatera insatsfördelning etc.
                var serviceConnector = new HPTServiceConnector();
                var rdi = serviceConnector.GetRaceDayInfoUpdateNoMerge(this.MarkBet.RaceDayInfo);

                // Om alla resultat är klar ska vi aktiver nästagångsknappen
                //if (this.MarkBet.RaceDayInfo.AllResultsComplete)
                if (this.MarkBet.RaceDayInfo.AnyResultComplete)
                {
                    this.btnSuggestNextTimers.IsEnabled = true;
                }

                if (this.chkHandleScratchedHorseAutomatically.IsChecked == true && rdi != null)
                {
                    HandleScratchedHorsesInCoupons(rdi);
                    BindingOperations.GetBindingExpression(this.lvwCoupons, ListView.ItemsSourceProperty).UpdateTarget();
                }
                if (this.chkSimulate.IsChecked != true && !this.isSneakCorrection)
                {
                    if (this.numberOfFinishedRaces < this.MarkBet.RaceDayInfo.NumberOfFinishedRaces && (bool)this.chkPlaySound.IsChecked)
                    {
                        var stream = HPTConfig.Config.GetEmbeddedResource("HPTClient.Sounds.horse.wav");
                        var player = new System.Media.SoundPlayer(stream);
                        player.Play();
                    }
                    this.numberOfFinishedRaces = this.MarkBet.RaceDayInfo.NumberOfFinishedRaces;
                    this.CouponCorrector.CorrectCoupons(this.numberOfFinishedRaces);
                    SortByNumberOfCorrect();

                    // Uppdatera resultatanalysen
                    this.CouponCorrector.ResultAnalyzer = new HPTResultAnalyzer(this.CouponCorrector.HorseList, this.MarkBet);
                    

                    // Spara om resultatet är färdigt
                    if (this.MarkBet.RaceDayInfo.ResultComplete && this.MarkBet.RaceDayInfo.HasAllPayOutInformation)
                    {
                        Cursor = Cursors.Wait;
                        this.numberOfFinishedRaces = this.MarkBet.BetType.NumberOfRaces;
                        if (this.MarkBet.RaceDayInfo.PayOutList.Count > 0)
                        {
                            HPTResultAnalyzer.AddResultAnalyzer(this.CouponCorrector.ResultAnalyzer);
                            PrepareForSave();
                            HPTSerializer.SerializeHPTSystem(this.MarkBet.MailSender.HPT3FileName, this.MarkBet);
                        }

                        // Nu behöver vi inte visa hur många rader som är kvar till nästa lopp
                        this.HorseListRemainingRows.Clear();

                        Cursor = Cursors.Arrow;
                    }

                    try
                    {
                        // Beräkna hur mycket du kan vinna som mest
                        CalculateWorstCaseScenario();
                        CalculateBestCaseScenario();
                        CalculateRemainingRowsForHorses();
                    }
                    catch (Exception exc)
                    {
                        HPTConfig.AddToErrorLogStatic(exc);
                    }
                }

                if (this.chkShowOnlyCouponsWithPotential.IsChecked == true)
                {
                    var collectionView = CollectionViewSource.GetDefaultView(this.lvwCoupons.ItemsSource);
                    if (collectionView.Filter == null)
                    {
                        collectionView.Filter = new Predicate<object>(FilterCoupons);
                    }
                }

                if (this.numberOfFinishedRaces < this.MarkBet.BetType.NumberOfRaces)
                {
                    this.gbCorrectionStatus.Visibility = System.Windows.Visibility.Visible;
                    this.gbSmallestPayout.Visibility = System.Windows.Visibility.Visible;

                    // Text med informtion medan rättningen pågår
                    var numberOfCorrect = this.MarkBet.RaceDayInfo.NumberOfFinishedRaces;
                    StringBuilder sb = new StringBuilder();
                    
                    var numberOfCorrectRows = this.MarkBet.SingleRowCollection.SingleRows
                        .Count(sr => sr.HorseList
                            .Intersect(this.MarkBet.CouponCorrector.HorseList)
                            .Count() == numberOfCorrect);

                    sb.Append(numberOfCorrectRows);
                    sb.Append(" rader med ");
                    sb.Append(numberOfCorrect);
                    sb.Append(" rätt");

                    // Ett fel
                    if (this.MarkBet.BetType.PoolShareOneError > 0M && this.numberOfFinishedRaces > 0)
                    {
                        numberOfCorrect--;
                        sb.AppendLine();
                        numberOfCorrectRows = this.MarkBet.SingleRowCollection.SingleRows
                            .Count(sr => sr.HorseList.Intersect(this.MarkBet.CouponCorrector.HorseList).Count() == numberOfCorrect);
                        sb.Append(numberOfCorrectRows);
                        sb.Append(" rader med ");
                        sb.Append(numberOfCorrect);
                        sb.Append(" rätt");
                    }

                    // Två fel
                    if (this.MarkBet.BetType.PoolShareTwoErrors > 0M && this.numberOfFinishedRaces > 1)
                    {
                        numberOfCorrect--;
                        sb.AppendLine();
                        numberOfCorrectRows = this.MarkBet.SingleRowCollection.SingleRows
                            .Count(sr => sr.HorseList.Intersect(this.MarkBet.CouponCorrector.HorseList).Count() == numberOfCorrect);
                        sb.Append(numberOfCorrectRows);
                        sb.Append(" rader med ");
                        sb.Append(numberOfCorrect);
                        sb.Append(" rätt");
                    }
                    this.txtRowStatus.Text = sb.ToString();

                    // Beräkna antalet kvarvarande rader per häst i nästa lopp
                    this.HorseListRemainingRows.Clear();

                    if (this.SingleRowsCorrect.Any())
                    {
                        this.SingleRowsCorrect = this.SingleRowsCorrect
                            .Where(sr => sr.HorseList.Intersect(this.MarkBet.CouponCorrector.HorseList).Count() == this.MarkBet.RaceDayInfo.NumberOfFinishedRaces)
                            .ToList();
                    }
                    else
                    {
                        this.SingleRowsCorrect.AddRange(this.MarkBet.SingleRowCollection.SingleRows);
                    }

                    this.MarkBet.RaceDayInfo.RaceList
                        .First(r => r.LegNr == this.numberOfFinishedRaces + 1)
                        .HorseListSelected
                        .ForEach(h =>
                        {
                            // Rader med alla rätt
                            h.NumberOfRowsWithAllCorrect = this.SingleRowsCorrect.Count(sr => sr.HorseList.Contains(h));
                            this.HorseListRemainingRows.Add(h);
                        });
                }
                else
                {
                    this.txtRowStatus.Text = string.Empty;
                    this.gbCorrectionStatus.Visibility = System.Windows.Visibility.Collapsed;
                    this.gbHorsesInNextRace.Visibility = System.Windows.Visibility.Collapsed;
                    this.gbSmallestPayout.Visibility = System.Windows.Visibility.Collapsed;
                    this.gbCorrectionResult.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        internal void SaveProfitToClipboard()
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append(this.MarkBet.RaceDayInfo.RaceDayDate.ToShortDateString());
                sb.Append("\t");
                sb.Append(this.MarkBet.BetType.Code);
                sb.Append("\t");
                sb.Append(this.MarkBet.RaceDayInfo.Trackname);
                sb.Append("\t");
                sb.Append(this.MarkBet.RaceDayInfo.PayOutList[0].PayOutAmount);
                sb.Append("\t");
                sb.Append(this.MarkBet.SystemCost);
                sb.Append("\t");
                sb.Append(this.MarkBet.CouponCorrector.CouponHelper.TotalWinnings);
                sb.Append("\t");
                sb.Append(this.MarkBet.CouponCorrector.CouponHelper.TotalWinnings - this.MarkBet.SystemCost);
                Clipboard.SetDataObject(sb.ToString());
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void PrepareForSave()
        {
            string fileName = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString();
            string hpt3Filename = fileName + ".hpt5";
            this.MarkBet.MailSender.HPT3FileName = hpt3Filename;
            this.MarkBet.SetSerializerValues();
        }

        private bool isSneakCorrection = false;
        private void btnSneak_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int racesToCorrect = (int)btn.Tag;

                if (racesToCorrect == this.MarkBet.BetType.NumberOfRaces)
                {
                    HPTServiceToHPTHelper.ConvertResultMarkingBet(this.MarkBet.RaceDayInfo.ResultMarkingBet, this.CouponCorrector.RaceDayInfo, true);
                }
                this.numberOfFinishedRaces = racesToCorrect;
                this.CouponCorrector.CorrectCoupons(racesToCorrect);
                this.isSneakCorrection = true;
                Correct();
                SortByNumberOfCorrect();
                CalculateWorstCaseScenario();
                CalculateBestCaseScenario();
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        private void btnRetrieveResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CouponCorrector.RetrieveResult(false);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        //private bool selectionInProgress = false;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.chkSimulate.IsChecked == false)
            {
                return;
            }
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    //selectionInProgress = true;
                    HPTHorse horse = (HPTHorse)e.AddedItems[0];
                    horse.Correct = true;
                    HPTHorse removedHorse = null;
                    if (e.RemovedItems.Count > 0)
                    {
                        removedHorse = (HPTHorse)e.RemovedItems[0];
                        removedHorse.Correct = false;
                    }

                    if (horse.ParentRace.LegResult == null)
                    {
                        horse.ParentRace.LegResult = new HPTLegResult();
                    }

                    if (horse.ParentRace.LegResult.WinnerList == null || horse.ParentRace.LegResult.WinnerList.Count() == 0)
                    {
                        horse.ParentRace.LegResult.WinnerList = new HPTHorse[] { horse };
                        this.CouponCorrector.RaceDayInfo.CalculateSimulatedRaceValues(this.MarkBet);
                    }
                    else if (horse.ParentRace.LegResult.WinnerList.Count() > 0 && horse != removedHorse)
                    {
                        horse.ParentRace.LegResult.WinnerList[0] = horse;
                        this.CouponCorrector.RaceDayInfo.CalculateSimulatedRaceValues(this.MarkBet);
                    }

                    HPTLegResult legResult = horse.ParentRace.LegResult;
                    if (legResult.Winners == null || legResult.Winners.Count() == 0)
                    {
                        legResult.Winners = new int[] { horse.StartNr };
                        legResult.WinnerStrings = new string[] { horse.HorseName };
                    }
                    else
                    {
                        legResult.Winners[0] = horse.StartNr;
                        legResult.WinnerStrings[0] = horse.HorseName;
                    }
                }
                if (e.AddedItems.Count > 0 && (bool)this.chkSimulate.IsChecked)
                {
                    var numberOfSimulatedRaces = this.MarkBet.RaceDayInfo.RaceList
                        .Where(r => r.LegResult != null && r.LegResult.Winners != null && r.LegResult.Winners.Length > 0)
                        .Count();
                    this.MarkBet.RaceDayInfo.NumberOfFinishedRaces = numberOfSimulatedRaces;
                    this.numberOfFinishedRaces = numberOfSimulatedRaces;
                    this.CouponCorrector.CorrectCouponsSimulatedResult();
                    Correct();
                    SortByNumberOfCorrect();
                }
            }
            catch (Exception exc)
            {
            }
            //selectionInProgress = false;
        }

        private void chkSimulate_Unchecked(object sender, RoutedEventArgs e)
        {
            ClearSimulatedCorrection();
        }

        private void ClearSimulatedCorrection()
        {
            foreach (HPTHorse horse in this.CouponCorrector.HorseList)
            {
                horse.Correct = false;
                //if (horse.ParentRace.LegResult != null && horse.ParentRace.LegResult.WinnerList != null)
                //{
                //    horse.ParentRace.LegResult.Value = null;
                //    horse.ParentRace.LegResult.Winners = null;
                //    horse.ParentRace.LegResult.WinnerList = new HPTHorse[] { null };
                //    horse.ParentRace.LegResult = null;
                //    horse.ParentRace.HasResult = false;
                //    horse.ParentRace.SplitVictory = false;
                //}
            }

            foreach (var race in this.MarkBet.RaceDayInfo.RaceList)
            {
                if (race.LegResult != null)
                {
                    race.LegResult.Value = null;
                    race.LegResult.Winners = null;
                    race.LegResult.WinnerList = new HPTHorse[] { null };
                    race.LegResult = null;
                    race.HasResult = false;
                    race.SplitVictory = false;
                }
            }

            if (this.CouponCorrector.RaceDayInfo.PayOutList != null)
            {
                this.CouponCorrector.RaceDayInfo.PayOutList.Clear();
            }
            this.MarkBet.RaceDayInfo.NumberOfFinishedRaces = 0;
            this.MarkBet.RaceDayInfo.ResultComplete = false;
            this.numberOfFinishedRaces = 0;
            this.CouponCorrector.RaceDayInfo.HasResult = false;
            this.CouponCorrector.HorseList.Clear();
            this.CouponCorrector.ClearCorrection();
            BindingOperations.GetBindingExpression(this.icResult, ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        private void chkLockCoupons_Unchecked(object sender, RoutedEventArgs e)
        {
            this.MarkBet.UpdateCoupons();
        }

        private void btnAnalyzeResult_Click(object sender, RoutedEventArgs e)
        {
            if (this.CouponCorrector.HorseList == null ||this.CouponCorrector.HorseList.Count == 0)
            {
                return;
            }
            this.CouponCorrector.ResultAnalyzer = new HPTResultAnalyzer(this.CouponCorrector.HorseList, this.MarkBet);
            var wndResultAnalyzer = new Window()
                                        {
                                            Content = new UCResultAnalyzer()
                                                          {
                                                              DataContext = this.CouponCorrector.ResultAnalyzer
                                                          },
                                            Title = "Resultatanalys",
                                            SizeToContent = SizeToContent.Width,
                                            MaxHeight = HPTConfig.Config.ApplicationHeight,
                                            ShowInTaskbar = false,
                                            ResizeMode = ResizeMode.NoResize,
                                            Owner = App.Current.MainWindow
                                        };

            wndResultAnalyzer.ShowDialog();
        }

        private void chkSimulate_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearCorrection_Click(object sender, RoutedEventArgs e)
        {
            ClearSimulatedCorrection();
        }

        private void chkHandleScratchedHorseAutomatically_Checked(object sender, RoutedEventArgs e)
        {
            // KOD HÄR?
        }

        //private bool reserverReplaced;
        private List<HPTMarkBetSingleRow> singleRowsAfterReservHandling;
        private List<HPTMarkBetSingleRow> SingleRowsAfterReservHandling
        {
            get
            {
                if (this.singleRowsAfterReservHandling == null || this.singleRowsAfterReservHandling.Count != this.MarkBet.ReducedSize)
                {
                    this.singleRowsAfterReservHandling = this.CouponCorrector.CouponHelper.CreateSingleRowsFromCoupons();
                }
                return this.singleRowsAfterReservHandling;
            }
        }
        internal void HandleScratchedHorsesInCoupons(HPTService.HPTRaceDayInfo rdi)
        {
            //var serviceConnector = new HPTServiceConnector();
            //var rdi = serviceConnector.GetRaceDayInfoUpdateNoMerge(this.MarkBet.RaceDayInfo);
            bool reserverReplaced = false;
            foreach (var leg in rdi.LegList)
            {
                var scratchedHorses = leg.HorseList
                    .Where(h => h.Scratched == true)
                    .Select(h => h.StartNr)
                    .ToList();

                if (scratchedHorses.Count > 0)
                {
                    reserverReplaced = true;

                    var race = this.MarkBet.RaceDayInfo.RaceList
                        .FirstOrDefault(r => r.LegNr == leg.LegNr);

                    if (race != null)
                    {
                        foreach (var startNr in scratchedHorses)
                        {
                            var horse = race.HorseList
                                .FirstOrDefault(h => h.Selected && h.StartNr == startNr);

                            if (horse != null)
                            {
                                foreach (var coupon in this.MarkBet.CouponCorrector.CouponHelper.CouponList)
                                {
                                    var couponRace = coupon.CouponRaceList.FirstOrDefault(cr => cr.LegNr == race.LegNr);
                                    if (couponRace != null && couponRace.HorseList.Contains(horse))
                                    {
                                        if (!couponRace.HorseList.Contains(couponRace.Reserv1Horse))
                                        {
                                            couponRace.HorseList.Add(couponRace.Reserv1Horse);
                                            couponRace.HorseList.Remove(horse);
                                        }
                                        else if (!couponRace.HorseList.Contains(couponRace.Reserv2Horse))
                                        {
                                            couponRace.HorseList.Add(couponRace.Reserv2Horse);
                                            couponRace.HorseList.Remove(horse);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (reserverReplaced)
            {
                this.singleRowsAfterReservHandling = null;
            }
            this.CouponCorrector.CouponHelper.CreateStartnumberListsForCoupons();
        }

        private void chkShowOnlyCouponsWithPotential_Checked(object sender, RoutedEventArgs e)
        {
            var collectionView = CollectionViewSource.GetDefaultView(this.lvwCoupons.ItemsSource);
            collectionView.Filter = new Predicate<object>(FilterCoupons);
        }

        public bool FilterCoupons(object obj)
        {
            var coupon = obj as HPTCoupon;
            return coupon.CanWin;
        }

        private void chkShowOnlyCouponsWithPotential_Unchecked(object sender, RoutedEventArgs e)
        {
            var collectionView = CollectionViewSource.GetDefaultView(this.lvwCoupons.ItemsSource);
            collectionView.Filter = null;
        }

        private void lvwCoupons_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (this.chkShowOnlyCouponsWithPotential.IsChecked == true)
            {
                var collectionView = CollectionViewSource.GetDefaultView(this.lvwCoupons.ItemsSource);
                collectionView.Filter = new Predicate<object>(FilterCoupons);
            }
        }

        #region Optimerad rättning

        private void AutoCorrectOptimized(object timerData)
        {
            this.Dispatcher.Invoke(new Action(AutoCorrectOptimized), null);
        }

        private void AutoCorrectOptimized()
        {
            try
            {
                if (DateTime.Now.AddMinutes(-4D) < this.MarkBet.RaceDayInfo.RaceList.Min(r => r.PostTime))
                {
                    if (this.chkAutomaticCorrection.IsChecked == true)
                    {
                        TimeSpan nextCorrectionTime = this.MarkBet.RaceDayInfo.RaceList.Min(r => r.PostTime).AddMinutes(4D) - DateTime.Now;
                        this.tmrCorrection.Change(nextCorrectionTime, new TimeSpan(0, 0, 30));
                    }
                    return;
                }

                // Hitta första loppet som inte är rättat
                var race = this.MarkBet.RaceDayInfo.RaceList
                    .OrderByDescending(r => r.PostTime)
                    .FirstOrDefault(r => r.PostTime.AddMinutes(4D) < DateTime.Now);

                if (race != null)
                {
                    if (this.numberOfFinishedRaces < race.LegNr || (this.numberOfFinishedRaces == this.MarkBet.RaceDayInfo.RaceList.Count && this.MarkBet.RaceDayInfo.PayOutList.Count == 0))
                    {
                        // Lopp som körts är inte klart än
                        UpdateResult();

                        if (this.chkAutomaticCorrection.IsChecked == true && this.numberOfFinishedRaces < race.LegNr)
                        {
                            // Kör igen om 30 sekunder tills resultatet är hämtat
                            this.tmrCorrection.Change(30000, 30000);
                        }
                    }
                    else if (this.chkAutomaticCorrection.IsChecked == true)
                    {
                        var nextRace = this.MarkBet.RaceDayInfo.RaceList
                            .OrderBy(r => r.LegNr)
                            .FirstOrDefault(r => r.LegNr > race.LegNr);

                        if (nextRace != null)
                        {
                            // Kör fyra minuter efter att nästa lopp startat
                            TimeSpan nextCorrectionTime = nextRace.PostTime.AddMinutes(4D) - DateTime.Now;
                            this.tmrCorrection.Change(nextCorrectionTime, new TimeSpan(0, 0, 30));
                        }
                        else if (!this.MarkBet.RaceDayInfo.AllResultsComplete)
                        {
                            // Kör om 30 sekunder igen
                            this.tmrCorrection.Change(30000, 30000);
                        }
                        else
                        {
                            // Stäng av timern, allt är klart
                            this.btnSuggestNextTimers.IsEnabled = true;
                            //this.chkAutomaticCorrection.IsChecked = false;
                            //this.tmrCorrection.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                        }
                    }
                    else
                    {
                        UpdateResult();
                    }
                }   
                else
                {
                    UpdateResult();
                }
            }
            catch (Exception exc)
            {
                //this.tmrCorrection.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                //var o = new object();
                this.chkAutomaticCorrection.IsChecked = false;
                //MessageBox.Show("Problem vid rättning av kuponger:\r\nObservera att endast V4, V5, V64, V65, V75 och V86 som är mindre än en vecka gamla kan automaträttas i nuläget.\r\n" + exc.Message, "Rättning av kuponger misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
                HPTConfig.Config.AddToErrorLog(exc);
            }
        }

        internal bool IsCorrectionFinished
        {
            get
            {
                return this.MarkBet.RaceDayInfo.PayOutList.Count > 0
                    && this.MarkBet.RaceDayInfo.PayOutList.Sum(po => po.PayOutAmount) > 0;
            }
        }

        #endregion

        private void chkAutomaticCorrection_Checked(object sender, RoutedEventArgs e)
        {
            if (this.chkAutomaticCorrection.IsChecked == true)
            {
                this.btnCorrect.IsEnabled = false;
                this.numberOfFinishedRaces = 0;
                AutoCorrectOptimized();
            }
            else
            {
                this.btnCorrect.IsEnabled = true;
                this.tmrCorrection.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
        }

        private HPTService.HPTVinstlista winnerList;
        private void btnWinnerList_Click(object sender, RoutedEventArgs e)
        {
            if (this.winnerList != null)
            {
                var wndWinnerList = new Window()
                {
                    Content = new UCWinnerList()
                    {
                        DataContext = this.winnerList
                    },
                    Title = "Vinstlista",
                    SizeToContent = SizeToContent.Width,
                    MaxHeight = HPTConfig.Config.ApplicationHeight,
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize,
                    Owner = App.Current.MainWindow
                };

                wndWinnerList.ShowDialog();
            }
        }

        private void CalculateWorstCaseScenario()
        {
            try
            {
                // Vilka är de rätta hästarna hittills
                var correctHorses = this.MarkBet.RaceDayInfo.RaceList
                    .Where(r => r.LegNr <= this.numberOfFinishedRaces)
                    .SelectMany(r => r.HorseList)
                    .Where(h => h.Correct)
                    .ToList();

                this.MarkBet.RaceDayInfo.RaceList
                    .Where(r => r.LegNr > this.numberOfFinishedRaces)
                    .ToList()
                    .ForEach(r => correctHorses.Add(r.HorseList.OrderByDescending(h => h.StakeDistributionShareFinal).First()));

                // Beräkna utdelning
                var sr = new HPTMarkBetSingleRow(correctHorses.ToArray());
                sr.EstimateRowValueFinalStakeShare(this.MarkBet);
                
                // Antal vinstgrupper
                int numberOfPools = this.MarkBet.BetType.PayOutDummyList.Length;
                this.MarkBet.BetType.PayOutDummyList[0].PayOutAmount = sr.RowValueFinalStakeShare;
                if (numberOfPools > 1)
                {
                    this.MarkBet.BetType.PayOutDummyList[1].PayOutAmount = sr.RowValueOneErrorFinalStakeShare;
                    if (numberOfPools > 2)
                    {
                        this.MarkBet.BetType.PayOutDummyList[2].PayOutAmount = sr.RowValueTwoErrorsFinalStakeShare;  
                    }
                }
                this.icWorstCaseScenario.ItemsSource = this.MarkBet.BetType.PayOutDummyList;
                BindingOperations.GetBindingExpression(this.icWorstCaseScenario, ItemsControl.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception exc)
            {
                //this.MarkBet.MaxPotentialWinnings = null;
            }
        }

        private void CalculateBestCaseScenario()
        {
            try
            {
                // Vilka är de rätta hästarna hittills
                var correctHorses = this.MarkBet.RaceDayInfo.RaceList
                    .Where(r => r.LegNr <= this.numberOfFinishedRaces)
                    .SelectMany(r => r.HorseList)
                    .Where(h => h.Correct)
                    .ToList();

                // Beräkna utdelning
                this.SingleRowsAfterReservHandling.ForEach(sr =>
                {
                    sr.EstimateRowValueFinalStakeShare(this.MarkBet);
                });

                // Raden med högst potential
                var bestRow = this.singleRowsAfterReservHandling
                    .OrderByDescending(sr => sr.HorseList.Intersect(correctHorses).Count())
                    .ThenByDescending(sr => sr.RowValueFinalStakeShare * sr.BetMultiplier)
                    .First();

                // Antal fel på raden med bäst potential
                int numberOfCorrect = bestRow.HorseList.Intersect(correctHorses).Count();
                int numberOfErrors = this.numberOfFinishedRaces - numberOfCorrect;

                // Antal vinstgrupper
                int numberOfPools = this.MarkBet.BetType.PayOutDummyList.Length;

                // Man kan inte vinna några pengar alls
                if (numberOfErrors >= numberOfPools)
                {
                    this.MarkBet.MaxPotentialWinnings = 0;
                    return;
                }

                // Kombinera rad med alla rätt hittills och egen rad som ger bäst utdelning
                if (numberOfErrors > 0)
                {
                    var bestHorseCombination = correctHorses
                        .Take(this.numberOfFinishedRaces).Concat(bestRow.HorseList.Skip(this.numberOfFinishedRaces))
                        .ToArray();

                    bestRow = new HPTMarkBetSingleRow(bestHorseCombination);
                    bestRow.EstimateRowValueFinalStakeShare(this.MarkBet);
                }

                // "Alla" rader med 0 fel, kan bara bli lista om det blivit dött lopp...
                //var rowsWith0Errors = this.MarkBet.SingleRowCollection.SingleRows
                var rowsWith0Errors = this.singleRowsAfterReservHandling
                    .Where(sr => sr.HorseList.Intersect(bestRow.HorseList).Count() == this.MarkBet.RaceDayInfo.RaceList.Count)
                    .ToList();                

                // Hur mycket kommer man kunna vinna i bästa fall
                int totalPotentialWinnings = rowsWith0Errors.Sum(sr => sr.RowValueFinalStakeShare * sr.BetMultiplier);

                if (numberOfPools > 1)
                {
                    // Utdelning på ett fel i bästa fall
                    bestRow.RowValueOneError = this.MarkBet.CouponCorrector.CalculatePayOutOneError(bestRow.HorseList, this.MarkBet.BetType.PoolShareOneError * this.MarkBet.BetType.RowCost);

                    // Blir bara vinst om utdelningen överstiger jackpottgränsen
                    if (bestRow.RowValueOneErrorFinalStakeShare >= this.MarkBet.BetType.JackpotLimit)
                    {
                        // Alla rader som har ett fel givet gynnsammaste utfallet
                        var rowsWith1Error = this.SingleRowsAfterReservHandling
                            .Where(sr => !sr.V6)
                            .Where(sr => sr.HorseList.Intersect(bestRow.HorseList).Count() == this.MarkBet.RaceDayInfo.RaceList.Count - 1)
                            .ToList();

                        //totalPotentialWinnings += rowsWith1Error.Sum(sr => sr.BetMultiplier) * bestRow.RowValueOneError;
                        totalPotentialWinnings += rowsWith1Error.Sum(sr => sr.BetMultiplier) * bestRow.RowValueOneErrorFinalStakeShare;

                        if (numberOfPools > 2)
                        {
                            // Utdelning på två fel i bästa fall
                            bestRow.RowValueTwoErrors = this.MarkBet.CouponCorrector.CalculatePayOutTwoErrors(bestRow.HorseList, this.MarkBet.BetType.PoolShareTwoErrors * this.MarkBet.BetType.RowCost);

                            if (bestRow.RowValueTwoErrorsFinalStakeShare >= this.MarkBet.BetType.JackpotLimit)
                            {
                                // Alla rader som har två fel givet gynsammaste utfallet
                                //var rowsWith2Errors = this.MarkBet.SingleRowCollection.SingleRows
                                var rowsWith2Errors = this.SingleRowsAfterReservHandling
                                    .Where(sr => !sr.V6)
                                    .Where(sr => sr.HorseList.Intersect(bestRow.HorseList).Count() == this.MarkBet.RaceDayInfo.RaceList.Count - 2)
                                    .ToList();
                                
                                totalPotentialWinnings += rowsWith2Errors.Sum(sr => sr.BetMultiplier) * bestRow.RowValueTwoErrorsFinalStakeShare;
                            }
                        }
                    }
                }
                this.MarkBet.MaxPotentialWinnings = totalPotentialWinnings;
            }
            catch (Exception exc)
            {
                this.MarkBet.MaxPotentialWinnings = null;
            }
        }

        private void CalculateRemainingRowsForHorses()
        {
            // Ingen mening att beräkna nåt...
            if (this.numberOfFinishedRaces < 1 || this.numberOfFinishedRaces > this.MarkBet.RaceDayInfo.RaceList.Count)
            {
                return;
            }

            Enumerable.Range(1, this.numberOfFinishedRaces)
                .ToList()
                .ForEach(leg => CalculateRemainingRowsForHorses(leg));
        }

        private void CalculateRemainingRowsForHorses(int legToCalculate)
        {
            // Vilka är de rätta hästarna hittills
            var correctHorses = this.MarkBet.RaceDayInfo.RaceList
                .Where(r => r.LegNr <= legToCalculate)
                .SelectMany(r => r.HorseList)
                .Where(h => h.Correct)
                .ToList();

            // Alla valda hästar i framtida lopp
            var futureHorses = this.MarkBet.RaceDayInfo.RaceList
                .Where(r => r.LegNr >= legToCalculate)
                .SelectMany(r => r.HorseListSelected)
                .ToList();

            // Rader med alla rätt
            //var rowsWithAllCorrect = this.MarkBet.SingleRowCollection.SingleRows
            var rowsWithAllCorrect = this.SingleRowsAfterReservHandling
                .Where(sr => sr.HorseList.Intersect(correctHorses).Count() == legToCalculate)
                .ToList();

            futureHorses
                .ForEach(h =>
                {
                    h.NumberOfRowsWithAllCorrect = rowsWithAllCorrect.Count(sr => sr.HorseList.Contains(h));
                    h.NumberOfRowsWithPossibility = "(" + h.NumberOfRowsWithAllCorrect.ToString();
                });

            if (this.numberOfFinishedRaces > 0 && this.MarkBet.BetType.PayOutDummyList.Length > 1)
            {
                // Rader med ett fel
                var rowsWithOneError = this.SingleRowsAfterReservHandling
                    .Where(sr => sr.HorseList.Intersect(correctHorses).Count() == legToCalculate - 1)
                    .ToList();

                futureHorses
                    .ForEach(h =>
                    {
                        h.NumberOfRowsWithOneError = rowsWithOneError.Count(sr => sr.HorseList.Contains(h));
                        h.NumberOfRowsWithPossibility += "/" + h.NumberOfRowsWithOneError.ToString();
                    });
            }

            if (this.numberOfFinishedRaces > 1 && this.MarkBet.BetType.PayOutDummyList.Length > 2)
            {
                // Rader med två fel
                var rowsWithTwoErrors = this.SingleRowsAfterReservHandling
                    .Where(sr => sr.HorseList.Intersect(correctHorses).Count() == legToCalculate - 2)
                    .ToList();

                futureHorses
                    .ForEach(h =>
                    {
                        h.NumberOfRowsWithTwoErrors = rowsWithTwoErrors.Count(sr => sr.HorseList.Contains(h));
                        h.NumberOfRowsWithPossibility += "/" + h.NumberOfRowsWithTwoErrors.ToString();
                    });
            }

            futureHorses
                .ForEach(h =>
                {
                    h.NumberOfRowsWithPossibility += ")";
                });
        }

        private void btnSuggestNextTimers_Click(object sender, RoutedEventArgs e)
        {
            this.MarkBet.SuggestNextTimers();
        }
    }
}

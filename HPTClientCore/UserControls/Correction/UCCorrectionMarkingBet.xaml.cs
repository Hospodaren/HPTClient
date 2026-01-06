using ATGDownloader;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCCorrectionMarkingBet.xaml
    /// </summary>
    public partial class UCCorrectionMarkingBet : UCMarkBetControl
    {
        Timer tmrCorrection;

        public UCCorrectionMarkingBet()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                tmrCorrection = new Timer(new TimerCallback(AutoCorrect));
                HorseListRemainingRows = new ObservableCollection<HPTHorse>();
                SingleRowsCorrect = new List<HPTMarkBetSingleRow>();
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
            lvwCoupons.Items.SortDescriptions.Clear();
            lvwCoupons.Items.SortDescriptions.Add(new SortDescription(field, newDir));
        }

        private void SortByNumberOfCorrect()
        {
            lvwCoupons.Items.SortDescriptions.Clear();
            lvwCoupons.Items.SortDescriptions.Add(new SortDescription("NumberOfCorrect", ListSortDirection.Descending));
        }

        private void btnCorrect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                numberOfFinishedRaces = 0;
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
                CouponCorrector = (HPTCouponCorrector)DataContext;
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
            Dispatcher.Invoke(new Action(AutoCorrectOptimized), null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && IsVisible)
            {
                try
                {
                    if (HorseListRemainingRows.Count == 0 && numberOfFinishedRaces == 0)
                    {
                        MarkBet.RaceDayInfo.RaceList
                            .First()
                            .HorseListSelected
                            .ForEach(h =>
                            {
                                HorseListRemainingRows.Add(h);
                            });

                        MarkBet.RaceDayInfo.RaceList
                            .SelectMany(r => r.HorseList)
                            .ToList()
                            .ForEach(h =>
                            {
                                h.NumberOfRowsWithAllCorrect = h.NumberOfCoveredRows;
                                h.NumberOfRowsWithPossibility = "(" + h.NumberOfRowsWithAllCorrect.ToString();
                                if (h.Selected)
                                {
                                    if (MarkBet.BetType.PayOutDummyList.Length > 1)
                                    {
                                        h.NumberOfRowsWithPossibility += "/0";
                                        if (MarkBet.BetType.PayOutDummyList.Length > 2)
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

        // TODO: Ta bort vinstlista?
        internal void UpdateResult()
        {
            if ((bool)chkSimulate.IsChecked)
            {
                chkSimulate.IsChecked = false;
                Thread.Sleep(200);
            }
            isSneakCorrection = false;
            numberOfFinishedRaces = MarkBet.RaceDayInfo.NumberOfFinishedRaces;
            if (MarkBet.RaceDayInfo.ResultComplete && MarkBet.RaceDayInfo.PayOutList.Count > 0)// && winnerList == null)    // Vafan gör jag här...?
            {
                //this.chkAutomaticCorrection.IsChecked = false;
                Correct();
            }
            else
            {
                HPTServiceConnector connector = new HPTServiceConnector();
                // TODO: Ny lösning
                //connector.GetResultMarkingBetByTrackAndDate(CouponCorrector.RaceDayInfo.BetType.Code, CouponCorrector.RaceDayInfo.TrackId, CouponCorrector.RaceDayInfo.RaceDayDate, ReceiveResult);
            }
        }

        //public void ReceiveResult(HPTService.HPTResultMarkingBet result)
        //{
        //    Dispatcher.Invoke(new Action<HPTService.HPTResultMarkingBet>(ReceiveResultInvoke), result);
        //}

        // TODO: Ny lösning, använd ATGGameBase istället
        //public void ReceiveResultInvoke(HPTService.HPTResultMarkingBet result)
        //{
        //    try
        //    {
        //        if (result.TrackId == 0)
        //        {
        //            //this.txtError.Text = DateTime.Now.ToShortTimeString() +
        //            //                     " - Rättning misslyckades, se fellogg för mer info.";
        //            return;
        //        }
        //        txtError.Text = string.Empty;
        //        HPTServiceToHPTHelper.ConvertResultMarkingBet(result, CouponCorrector.RaceDayInfo, true);

        //        Correct();
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //    //this.correctionInProgress = false;
        //}

        private int numberOfFinishedRaces;
        private void Correct()
        {
            try
            {
                if (CouponCorrector.RaceDayInfo == null)
                {
                    CouponCorrector.RaceDayInfo = MarkBet.RaceDayInfo;
                }
                //if (winnerList == null && CouponCorrector.RaceDayInfo.WinnerList != null)
                //{
                //    winnerList = CouponCorrector.RaceDayInfo.WinnerList;
                //    btnWinnerList.IsEnabled = true;
                //}

                // Hämta info om strykningar och resultat utan att uppdatera insatsfördelning etc.
                var serviceConnector = new HPTServiceConnector();
                // TODO: Använd ATGDonloader
                //var rdi = serviceConnector.GetRaceDayInfoUpdateNoMerge(MarkBet.RaceDayInfo);
                var rdi = new HPTRaceDayInfo();

                // Om alla resultat är klar ska vi aktiver nästagångsknappen
                //if (this.MarkBet.RaceDayInfo.AllResultsComplete)
                if (MarkBet.RaceDayInfo.AnyResultComplete)
                {
                    btnSuggestNextTimers.IsEnabled = true;
                }

                // TODO: Använd ATGGameBase
                //if (chkHandleScratchedHorseAutomatically.IsChecked == true && rdi != null)
                //{
                //    HandleScratchedHorsesInCoupons(rdi);
                //    BindingOperations.GetBindingExpression(lvwCoupons, ListView.ItemsSourceProperty).UpdateTarget();
                //}
                if (chkSimulate.IsChecked != true && !isSneakCorrection)
                {
                    if (numberOfFinishedRaces < MarkBet.RaceDayInfo.NumberOfFinishedRaces && (bool)chkPlaySound.IsChecked)
                    {
                        var stream = HPTConfig.Config.GetEmbeddedResource("HPTClient.Sounds.horse.wav");
                        var player = new System.Media.SoundPlayer(stream);
                        player.Play();
                    }
                    numberOfFinishedRaces = MarkBet.RaceDayInfo.NumberOfFinishedRaces;
                    CouponCorrector.CorrectCoupons(numberOfFinishedRaces);
                    SortByNumberOfCorrect();

                    // Uppdatera resultatanalysen
                    CouponCorrector.ResultAnalyzer = new HPTResultAnalyzer(CouponCorrector.HorseList, MarkBet);


                    // Spara om resultatet är färdigt
                    if (MarkBet.RaceDayInfo.ResultComplete && MarkBet.RaceDayInfo.HasAllPayOutInformation)
                    {
                        Cursor = Cursors.Wait;
                        numberOfFinishedRaces = MarkBet.BetType.NumberOfRaces;
                        if (MarkBet.RaceDayInfo.PayOutList.Count > 0)
                        {
                            HPTResultAnalyzer.AddResultAnalyzer(CouponCorrector.ResultAnalyzer);
                            PrepareForSave();
                            HPTSerializer.SerializeHPTSystem(MarkBet.MailSender.HPT3FileName, MarkBet);
                        }

                        // Nu behöver vi inte visa hur många rader som är kvar till nästa lopp
                        HorseListRemainingRows.Clear();

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

                if (chkShowOnlyCouponsWithPotential.IsChecked == true)
                {
                    var collectionView = CollectionViewSource.GetDefaultView(lvwCoupons.ItemsSource);
                    if (collectionView.Filter == null)
                    {
                        collectionView.Filter = new Predicate<object>(FilterCoupons);
                    }
                }

                if (numberOfFinishedRaces < MarkBet.BetType.NumberOfRaces)
                {
                    gbCorrectionStatus.Visibility = Visibility.Visible;
                    gbSmallestPayout.Visibility = Visibility.Visible;

                    // Text med informtion medan rättningen pågår
                    var numberOfCorrect = MarkBet.RaceDayInfo.NumberOfFinishedRaces;
                    StringBuilder sb = new StringBuilder();

                    var numberOfCorrectRows = MarkBet.SingleRowCollection.SingleRows
                        .Count(sr => sr.HorseList
                            .Intersect(MarkBet.CouponCorrector.HorseList)
                            .Count() == numberOfCorrect);

                    sb.Append(numberOfCorrectRows);
                    sb.Append(" rader med ");
                    sb.Append(numberOfCorrect);
                    sb.Append(" rätt");

                    // Ett fel
                    if (MarkBet.BetType.PoolShareOneError > 0M && numberOfFinishedRaces > 0)
                    {
                        numberOfCorrect--;
                        sb.AppendLine();
                        numberOfCorrectRows = MarkBet.SingleRowCollection.SingleRows
                            .Count(sr => sr.HorseList.Intersect(MarkBet.CouponCorrector.HorseList).Count() == numberOfCorrect);
                        sb.Append(numberOfCorrectRows);
                        sb.Append(" rader med ");
                        sb.Append(numberOfCorrect);
                        sb.Append(" rätt");
                    }

                    // Två fel
                    if (MarkBet.BetType.PoolShareTwoErrors > 0M && numberOfFinishedRaces > 1)
                    {
                        numberOfCorrect--;
                        sb.AppendLine();
                        numberOfCorrectRows = MarkBet.SingleRowCollection.SingleRows
                            .Count(sr => sr.HorseList.Intersect(MarkBet.CouponCorrector.HorseList).Count() == numberOfCorrect);
                        sb.Append(numberOfCorrectRows);
                        sb.Append(" rader med ");
                        sb.Append(numberOfCorrect);
                        sb.Append(" rätt");
                    }
                    txtRowStatus.Text = sb.ToString();

                    // Beräkna antalet kvarvarande rader per häst i nästa lopp
                    HorseListRemainingRows.Clear();

                    if (SingleRowsCorrect.Any())
                    {
                        SingleRowsCorrect = SingleRowsCorrect
                            .Where(sr => sr.HorseList.Intersect(MarkBet.CouponCorrector.HorseList).Count() == MarkBet.RaceDayInfo.NumberOfFinishedRaces)
                            .ToList();
                    }
                    else
                    {
                        SingleRowsCorrect.AddRange(MarkBet.SingleRowCollection.SingleRows);
                    }

                    MarkBet.RaceDayInfo.RaceList
                        .First(r => r.LegNr == numberOfFinishedRaces + 1)
                        .HorseListSelected
                        .ForEach(h =>
                        {
                            // Rader med alla rätt
                            h.NumberOfRowsWithAllCorrect = SingleRowsCorrect.Count(sr => sr.HorseList.Contains(h));
                            HorseListRemainingRows.Add(h);
                        });
                }
                else
                {
                    txtRowStatus.Text = string.Empty;
                    gbCorrectionStatus.Visibility = Visibility.Collapsed;
                    gbHorsesInNextRace.Visibility = Visibility.Collapsed;
                    gbSmallestPayout.Visibility = Visibility.Collapsed;
                    gbCorrectionResult.Visibility = Visibility.Visible;
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
                sb.Append(MarkBet.RaceDayInfo.RaceDayDate.ToShortDateString());
                sb.Append("\t");
                sb.Append(MarkBet.BetType.Code);
                sb.Append("\t");
                sb.Append(MarkBet.RaceDayInfo.Trackname);
                sb.Append("\t");
                sb.Append(MarkBet.RaceDayInfo.PayOutList[0].PayOutAmount);
                sb.Append("\t");
                sb.Append(MarkBet.SystemCost);
                sb.Append("\t");
                sb.Append(MarkBet.CouponCorrector.CouponHelper.TotalWinnings);
                sb.Append("\t");
                sb.Append(MarkBet.CouponCorrector.CouponHelper.TotalWinnings - MarkBet.SystemCost);
                Clipboard.SetDataObject(sb.ToString());
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void PrepareForSave()
        {
            string fileName = MarkBet.SaveDirectory + MarkBet.ToFileNameString();
            string hpt3Filename = fileName + ".hpt7";
            MarkBet.MailSender.HPT3FileName = hpt3Filename;
            MarkBet.SetSerializerValues();
        }

        private bool isSneakCorrection = false;
        private void btnSneak_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int racesToCorrect = (int)btn.Tag;

                if (racesToCorrect == MarkBet.BetType.NumberOfRaces)
                {
                    // TODO; Ny lösning
                    //HPTServiceToHPTHelper.ConvertResultMarkingBet(MarkBet.RaceDayInfo.ResultMarkingBet, CouponCorrector.RaceDayInfo, true);
                }
                numberOfFinishedRaces = racesToCorrect;
                CouponCorrector.CorrectCoupons(racesToCorrect);
                isSneakCorrection = true;
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
                CouponCorrector.RetrieveResult(false);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        //private bool selectionInProgress = false;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chkSimulate.IsChecked == false)
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
                        CouponCorrector.RaceDayInfo.CalculateSimulatedRaceValues(MarkBet);
                    }
                    else if (horse.ParentRace.LegResult.WinnerList.Count() > 0 && horse != removedHorse)
                    {
                        horse.ParentRace.LegResult.WinnerList[0] = horse;
                        CouponCorrector.RaceDayInfo.CalculateSimulatedRaceValues(MarkBet);
                    }

                    HPTLegResult legResult = horse.ParentRace.LegResult;
                    if (legResult.Winners is null || legResult.Winners.Count() == 0)
                    {
                        legResult.Winners = [horse.StartNr];
                        legResult.WinnerStrings = [horse.HorseName];
                    }
                    else
                    {
                        legResult.Winners = [horse.StartNr];
                        legResult.WinnerStrings = [horse.HorseName];
                    }
                }
                if (e.AddedItems.Count > 0 && (bool)chkSimulate.IsChecked)
                {
                    var numberOfSimulatedRaces = MarkBet.RaceDayInfo.RaceList
                        .Where(r => r.LegResult != null && r.LegResult.Winners != null && r.LegResult.Winners.Length > 0)
                        .Count();
                    MarkBet.RaceDayInfo.NumberOfFinishedRaces = numberOfSimulatedRaces;
                    numberOfFinishedRaces = numberOfSimulatedRaces;
                    CouponCorrector.CorrectCouponsSimulatedResult();
                    Correct();
                    SortByNumberOfCorrect();
                }
            }
            catch (Exception)
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
            foreach (HPTHorse horse in CouponCorrector.HorseList)
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

            foreach (var race in MarkBet.RaceDayInfo.RaceList)
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

            if (CouponCorrector.RaceDayInfo.PayOutList != null)
            {
                CouponCorrector.RaceDayInfo.PayOutList.Clear();
            }
            MarkBet.RaceDayInfo.NumberOfFinishedRaces = 0;
            MarkBet.RaceDayInfo.ResultComplete = false;
            numberOfFinishedRaces = 0;
            CouponCorrector.RaceDayInfo.HasResult = false;
            CouponCorrector.HorseList.Clear();
            CouponCorrector.ClearCorrection();
            BindingOperations.GetBindingExpression(icResult, ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        private void chkLockCoupons_Unchecked(object sender, RoutedEventArgs e)
        {
            MarkBet.UpdateCoupons();
        }

        private void btnAnalyzeResult_Click(object sender, RoutedEventArgs e)
        {
            if (CouponCorrector.HorseList == null || CouponCorrector.HorseList.Count == 0)
            {
                return;
            }
            CouponCorrector.ResultAnalyzer = new HPTResultAnalyzer(CouponCorrector.HorseList, MarkBet);
            var wndResultAnalyzer = new Window()
            {
                Content = new UCResultAnalyzer()
                {
                    DataContext = CouponCorrector.ResultAnalyzer
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
                if (singleRowsAfterReservHandling == null || singleRowsAfterReservHandling.Count != MarkBet.ReducedSize)
                {
                    singleRowsAfterReservHandling = CouponCorrector.CouponHelper.CreateSingleRowsFromCoupons();
                }
                return singleRowsAfterReservHandling;
            }
        }

        internal void HandleScratchedHorsesInCoupons(ATGGameBase game)
        {
            // TODO: Använd ATGGameBase istället
            //var serviceConnector = new HPTServiceConnector();
            //var rdi = serviceConnector.GetRaceDayInfoUpdateNoMerge(this.MarkBet.RaceDayInfo);
            bool reserverReplaced = false;
            foreach (var leg in game.Races)
            {
                //var scratchedHorses = leg.HorseList
                //    .Where(h => h.Scratched == true)
                //    .Select(h => h.StartNr)
                //    .ToList();

                //if (scratchedHorses.Count > 0)
                //{
                //    reserverReplaced = true;

                //    var race = MarkBet.RaceDayInfo.RaceList
                //        .FirstOrDefault(r => r.LegNr == leg.LegNr);

                //    if (race != null)
                //    {
                //        foreach (var startNr in scratchedHorses)
                //        {
                //            var horse = race.HorseList
                //                .FirstOrDefault(h => h.Selected && h.StartNr == startNr);

                //            if (horse != null)
                //            {
                //                foreach (var coupon in MarkBet.CouponCorrector.CouponHelper.CouponList)
                //                {
                //                    var couponRace = coupon.CouponRaceList.FirstOrDefault(cr => cr.LegNr == race.LegNr);
                //                    if (couponRace != null && couponRace.HorseList.Contains(horse))
                //                    {
                //                        if (!couponRace.HorseList.Contains(couponRace.Reserv1Horse))
                //                        {
                //                            couponRace.HorseList.Add(couponRace.Reserv1Horse);
                //                            couponRace.HorseList.Remove(horse);
                //                        }
                //                        else if (!couponRace.HorseList.Contains(couponRace.Reserv2Horse))
                //                        {
                //                            couponRace.HorseList.Add(couponRace.Reserv2Horse);
                //                            couponRace.HorseList.Remove(horse);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }
            if (reserverReplaced)
            {
                singleRowsAfterReservHandling = null;
            }
            CouponCorrector.CouponHelper.CreateStartnumberListsForCoupons();
        }

        private void chkShowOnlyCouponsWithPotential_Checked(object sender, RoutedEventArgs e)
        {
            var collectionView = CollectionViewSource.GetDefaultView(lvwCoupons.ItemsSource);
            collectionView.Filter = new Predicate<object>(FilterCoupons);
        }

        public bool FilterCoupons(object obj)
        {
            var coupon = obj as HPTCoupon;
            return coupon.CanWin;
        }

        private void chkShowOnlyCouponsWithPotential_Unchecked(object sender, RoutedEventArgs e)
        {
            var collectionView = CollectionViewSource.GetDefaultView(lvwCoupons.ItemsSource);
            collectionView.Filter = null;
        }

        private void lvwCoupons_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (chkShowOnlyCouponsWithPotential.IsChecked == true)
            {
                var collectionView = CollectionViewSource.GetDefaultView(lvwCoupons.ItemsSource);
                collectionView.Filter = new Predicate<object>(FilterCoupons);
            }
        }

        #region Optimerad rättning

        private void AutoCorrectOptimized(object timerData)
        {
            Dispatcher.Invoke(new Action(AutoCorrectOptimized), null);
        }

        private void AutoCorrectOptimized()
        {
            try
            {
                if (DateTime.Now.AddMinutes(-4D) < MarkBet.RaceDayInfo.RaceList.Min(r => r.PostTime))
                {
                    if (chkAutomaticCorrection.IsChecked == true)
                    {
                        TimeSpan nextCorrectionTime = MarkBet.RaceDayInfo.RaceList.Min(r => r.PostTime).AddMinutes(4D) - DateTime.Now;
                        tmrCorrection.Change(nextCorrectionTime, new TimeSpan(0, 0, 30));
                    }
                    return;
                }

                // Hitta första loppet som inte är rättat
                var race = MarkBet.RaceDayInfo.RaceList
                    .OrderByDescending(r => r.PostTime)
                    .FirstOrDefault(r => r.PostTime.AddMinutes(4D) < DateTime.Now);

                if (race != null)
                {
                    if (numberOfFinishedRaces < race.LegNr || (numberOfFinishedRaces == MarkBet.RaceDayInfo.RaceList.Count && MarkBet.RaceDayInfo.PayOutList.Count == 0))
                    {
                        // Lopp som körts är inte klart än
                        UpdateResult();

                        if (chkAutomaticCorrection.IsChecked == true && numberOfFinishedRaces < race.LegNr)
                        {
                            // Kör igen om 30 sekunder tills resultatet är hämtat
                            tmrCorrection.Change(30000, 30000);
                        }
                    }
                    else if (chkAutomaticCorrection.IsChecked == true)
                    {
                        var nextRace = MarkBet.RaceDayInfo.RaceList
                            .OrderBy(r => r.LegNr)
                            .FirstOrDefault(r => r.LegNr > race.LegNr);

                        if (nextRace != null)
                        {
                            // Kör fyra minuter efter att nästa lopp startat
                            TimeSpan nextCorrectionTime = nextRace.PostTime.AddMinutes(4D) - DateTime.Now;
                            tmrCorrection.Change(nextCorrectionTime, new TimeSpan(0, 0, 30));
                        }
                        else if (!MarkBet.RaceDayInfo.AllResultsComplete)
                        {
                            // Kör om 30 sekunder igen
                            tmrCorrection.Change(30000, 30000);
                        }
                        else
                        {
                            // Stäng av timern, allt är klart
                            btnSuggestNextTimers.IsEnabled = true;
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
                chkAutomaticCorrection.IsChecked = false;
                //MessageBox.Show("Problem vid rättning av kuponger:\r\nObservera att endast V4, V5, V64, V65, V75 och V86 som är mindre än en vecka gamla kan automaträttas i nuläget.\r\n" + exc.Message, "Rättning av kuponger misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
                HPTConfig.Config.AddToErrorLog(exc);
            }
        }

        internal bool IsCorrectionFinished
        {
            get
            {
                return MarkBet.RaceDayInfo.PayOutList.Count > 0
                    && MarkBet.RaceDayInfo.PayOutList.Sum(po => po.PayOutAmount) > 0;
            }
        }

        #endregion

        private void chkAutomaticCorrection_Checked(object sender, RoutedEventArgs e)
        {
            if (chkAutomaticCorrection.IsChecked == true)
            {
                btnCorrect.IsEnabled = false;
                numberOfFinishedRaces = 0;
                AutoCorrectOptimized();
            }
            else
            {
                btnCorrect.IsEnabled = true;
                tmrCorrection.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        // TODO: Finns väl inte på atg.se?
        //private HPTService.HPTVinstlista winnerList;
        private void btnWinnerList_Click(object sender, RoutedEventArgs e)
        {
            //if (winnerList != null)
            //{
            //    var wndWinnerList = new Window()
            //    {
            //        Content = new UCWinnerList()
            //        {
            //            DataContext = winnerList
            //        },
            //        Title = "Vinstlista",
            //        SizeToContent = SizeToContent.Width,
            //        MaxHeight = HPTConfig.Config.ApplicationHeight,
            //        ShowInTaskbar = false,
            //        ResizeMode = ResizeMode.NoResize,
            //        Owner = App.Current.MainWindow
            //    };

            //    wndWinnerList.ShowDialog();
            //}
        }

        private void CalculateWorstCaseScenario()
        {
            try
            {
                // Vilka är de rätta hästarna hittills
                var correctHorses = MarkBet.RaceDayInfo.RaceList
                    .Where(r => r.LegNr <= numberOfFinishedRaces)
                    .SelectMany(r => r.HorseList)
                    .Where(h => h.Correct)
                    .ToList();

                MarkBet.RaceDayInfo.RaceList
                    .Where(r => r.LegNr > numberOfFinishedRaces)
                    .ToList()
                    .ForEach(r => correctHorses.Add(r.HorseList.OrderByDescending(h => h.StakeDistributionShareFinal).First()));

                // Beräkna utdelning
                var sr = new HPTMarkBetSingleRow(correctHorses.ToArray());
                sr.EstimateRowValueFinalStakeShare(MarkBet);

                // Antal vinstgrupper
                int numberOfPools = MarkBet.BetType.PayOutDummyList.Length;
                MarkBet.BetType.PayOutDummyList[0].PayOutAmount = sr.RowValueFinalStakeShare;
                if (numberOfPools > 1)
                {
                    MarkBet.BetType.PayOutDummyList[1].PayOutAmount = sr.RowValueOneErrorFinalStakeShare;
                    if (numberOfPools > 2)
                    {
                        MarkBet.BetType.PayOutDummyList[2].PayOutAmount = sr.RowValueThreeErrorsFinalStakeShare;
                    }
                }
                icWorstCaseScenario.ItemsSource = MarkBet.BetType.PayOutDummyList;
                BindingOperations.GetBindingExpression(icWorstCaseScenario, ItemsControl.ItemsSourceProperty)?.UpdateTarget();
            }
            catch (Exception)
            {
                //this.MarkBet.MaxPotentialWinnings = null;
            }
        }

        private void CalculateBestCaseScenario()
        {
            try
            {
                // Vilka är de rätta hästarna hittills
                var correctHorses = MarkBet.RaceDayInfo.RaceList
                    .Where(r => r.LegNr <= numberOfFinishedRaces)
                    .SelectMany(r => r.HorseList)
                    .Where(h => h.Correct)
                    .ToList();

                // Beräkna utdelning
                SingleRowsAfterReservHandling.ForEach(sr =>
                {
                    sr.EstimateRowValueFinalStakeShare(MarkBet);
                });

                // Raden med högst potential
                var bestRow = singleRowsAfterReservHandling
                    .OrderByDescending(sr => sr.HorseList.Intersect(correctHorses).Count())
                    .ThenByDescending(sr => sr.RowValueFinalStakeShare * sr.BetMultiplier)
                    .First();

                // Antal fel på raden med bäst potential
                int numberOfCorrect = bestRow.HorseList.Intersect(correctHorses).Count();
                int numberOfErrors = numberOfFinishedRaces - numberOfCorrect;

                // Antal vinstgrupper
                int numberOfPools = MarkBet.BetType.PayOutDummyList.Length;

                // Man kan inte vinna några pengar alls
                if (numberOfErrors >= numberOfPools)
                {
                    MarkBet.MaxPotentialWinnings = 0;
                    return;
                }

                // Kombinera rad med alla rätt hittills och egen rad som ger bäst utdelning
                if (numberOfErrors > 0)
                {
                    var bestHorseCombination = correctHorses
                        .Take(numberOfFinishedRaces).Concat(bestRow.HorseList.Skip(numberOfFinishedRaces))
                        .ToArray();

                    bestRow = new HPTMarkBetSingleRow(bestHorseCombination);
                    bestRow.EstimateRowValueFinalStakeShare(MarkBet);
                }

                // "Alla" rader med 0 fel, kan bara bli lista om det blivit dött lopp...
                //var rowsWith0Errors = this.MarkBet.SingleRowCollection.SingleRows
                var rowsWith0Errors = singleRowsAfterReservHandling
                    .Where(sr => sr.HorseList.Intersect(bestRow.HorseList).Count() == MarkBet.RaceDayInfo.RaceList.Count)
                    .ToList();

                // Hur mycket kommer man kunna vinna i bästa fall
                int totalPotentialWinnings = rowsWith0Errors.Sum(sr => sr.RowValueFinalStakeShare * sr.BetMultiplier);

                if (numberOfPools > 1)
                {
                    // Utdelning på ett fel i bästa fall
                    bestRow.RowValueOneError = MarkBet.CouponCorrector.CalculatePayOutOneError(bestRow.HorseList, MarkBet.BetType.PoolShareOneError * MarkBet.BetType.RowCost);

                    // Blir bara vinst om utdelningen överstiger jackpottgränsen
                    if (bestRow.RowValueOneErrorFinalStakeShare >= MarkBet.BetType.JackpotLimit)
                    {
                        // Alla rader som har ett fel givet gynnsammaste utfallet
                        var rowsWith1Error = SingleRowsAfterReservHandling
                            .Where(sr => !sr.V6)
                            .Where(sr => sr.HorseList.Intersect(bestRow.HorseList).Count() == MarkBet.RaceDayInfo.RaceList.Count - 1)
                            .ToList();

                        //totalPotentialWinnings += rowsWith1Error.Sum(sr => sr.BetMultiplier) * bestRow.RowValueOneError;
                        totalPotentialWinnings += rowsWith1Error.Sum(sr => sr.BetMultiplier) * bestRow.RowValueOneErrorFinalStakeShare;

                        if (numberOfPools > 2)
                        {
                            // Utdelning på två fel i bästa fall
                            bestRow.RowValueTwoErrors = MarkBet.CouponCorrector.CalculatePayOutTwoErrors(bestRow.HorseList, MarkBet.BetType.PoolShareTwoErrors * MarkBet.BetType.RowCost);

                            if (bestRow.RowValueTwoErrorsFinalStakeShare >= MarkBet.BetType.JackpotLimit)
                            {
                                // Alla rader som har två fel givet gynsammaste utfallet
                                //var rowsWith2Errors = this.MarkBet.SingleRowCollection.SingleRows
                                var rowsWith2Errors = SingleRowsAfterReservHandling
                                    .Where(sr => !sr.V6)
                                    .Where(sr => sr.HorseList.Intersect(bestRow.HorseList).Count() == MarkBet.RaceDayInfo.RaceList.Count - 2)
                                    .ToList();

                                totalPotentialWinnings += rowsWith2Errors.Sum(sr => sr.BetMultiplier) * bestRow.RowValueTwoErrorsFinalStakeShare;

                                if (numberOfPools > 3)
                                {
                                    // Utdelning på två fel i bästa fall
                                    bestRow.RowValueThreeErrors = MarkBet.CouponCorrector.CalculatePayOutTwoErrors(bestRow.HorseList, MarkBet.BetType.PoolShareTwoErrors * MarkBet.BetType.RowCost);

                                    if (bestRow.RowValueTwoErrorsFinalStakeShare >= MarkBet.BetType.JackpotLimit)
                                    {
                                        // Alla rader som har två fel givet gynsammaste utfallet
                                        //var rowsWith2Errors = this.MarkBet.SingleRowCollection.SingleRows
                                        var rowsWith3Errors = SingleRowsAfterReservHandling
                                            .Where(sr => !sr.V6)
                                            .Where(sr => sr.HorseList.Intersect(bestRow.HorseList).Count() == MarkBet.RaceDayInfo.RaceList.Count - 2)
                                            .ToList();

                                        totalPotentialWinnings += rowsWith3Errors.Sum(sr => sr.BetMultiplier) * bestRow.RowValueThreeErrorsFinalStakeShare;
                                    }
                                }
                            }
                        }
                    }
                }
                MarkBet.MaxPotentialWinnings = totalPotentialWinnings;
            }
            catch (Exception)
            {
                MarkBet.MaxPotentialWinnings = null;
            }
        }

        private void CalculateRemainingRowsForHorses()
        {
            // Ingen mening att beräkna nåt...
            if (numberOfFinishedRaces < 1 || numberOfFinishedRaces > MarkBet.RaceDayInfo.RaceList.Count)
            {
                return;
            }

            Enumerable.Range(1, numberOfFinishedRaces)
                .ToList()
                .ForEach(leg => CalculateRemainingRowsForHorses(leg));
        }

        private void CalculateRemainingRowsForHorses(int legToCalculate)
        {
            // Vilka är de rätta hästarna hittills
            var correctHorses = MarkBet.RaceDayInfo.RaceList
                .Where(r => r.LegNr <= legToCalculate)
                .SelectMany(r => r.HorseList)
                .Where(h => h.Correct)
                .ToList();

            // Alla valda hästar i framtida lopp
            var futureHorses = MarkBet.RaceDayInfo.RaceList
                .Where(r => r.LegNr >= legToCalculate)
                .SelectMany(r => r.HorseListSelected)
                .ToList();

            // Rader med alla rätt
            //var rowsWithAllCorrect = this.MarkBet.SingleRowCollection.SingleRows
            var rowsWithAllCorrect = SingleRowsAfterReservHandling
                .Where(sr => sr.HorseList.Intersect(correctHorses).Count() == legToCalculate)
                .ToList();

            futureHorses
                .ForEach(h =>
                {
                    h.NumberOfRowsWithAllCorrect = rowsWithAllCorrect.Count(sr => sr.HorseList.Contains(h));
                    h.NumberOfRowsWithPossibility = "(" + h.NumberOfRowsWithAllCorrect.ToString();
                });

            if (numberOfFinishedRaces > 0 && MarkBet.BetType.PayOutDummyList.Length > 1)
            {
                // Rader med ett fel
                var rowsWithOneError = SingleRowsAfterReservHandling
                    .Where(sr => sr.HorseList.Intersect(correctHorses).Count() == legToCalculate - 1)
                    .ToList();

                futureHorses
                    .ForEach(h =>
                    {
                        h.NumberOfRowsWithOneError = rowsWithOneError.Count(sr => sr.HorseList.Contains(h));
                        h.NumberOfRowsWithPossibility += "/" + h.NumberOfRowsWithOneError.ToString();
                    });
            }

            if (numberOfFinishedRaces > 1 && MarkBet.BetType.PayOutDummyList.Length > 2)
            {
                // Rader med två fel
                var rowsWithTwoErrors = SingleRowsAfterReservHandling
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
            MarkBet.SuggestNextTimers();
        }
    }
}

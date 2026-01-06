using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCCombinationsGameHandling.xaml
    /// </summary>
    public partial class UCCombinationsGameHandling : UCCombBetControl
    {
        public UCCombinationsGameHandling()
        {
            atgUpdateTimer = new Timer(new TimerCallback(UpdateFromATG));

            InitializeComponent();

            // Hantering av gratisanvändare som öppnar fil med icke stödda spelformer
            if (!HPTConfig.Config.IsPayingCustomer)
            {
                btnCreateCoupons.IsEnabled = false;
                btnCreateCouponsAs.IsEnabled = false;
                btnUpdate.IsEnabled = false;
            }
        }

        #region Button clicks

        private void btnCreateCoupons_Click(object sender, RoutedEventArgs e)
        {
            string fileName = CombBet.SaveDirectory + CombBet.ToFileNameString();
            CombBet.SystemFilename = fileName + ".xml";
            ATGCouponHelper couponHelper = new ATGCouponHelper(CombBet);
            couponHelper.CreateCoupons();
            couponHelper.CreateATGFile();
            HPTSerializer.SerializeHPTCombinationSystem(fileName + ".hpt7", CombBet);
        }

        void sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    SaveFileDialog sfd = (SaveFileDialog)sender;
                    string fileName = sfd.FileName;
                    CombBet.SystemFilename = fileName;
                    ATGCouponHelper couponHelper = new ATGCouponHelper(CombBet);
                    couponHelper.CreateCoupons();
                    couponHelper.CreateATGFile();
                    HPTSerializer.SerializeHPTCombinationSystem(fileName.Replace(".xml", ".hpt7"), CombBet);
                }
                catch (Exception exc)
                {
                    string error = exc.Message;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            HPTSerializer.SerializeHPTCombinationSystem(CombBet.SaveDirectory + CombBet.ToFileNameString() + ".hpt5", CombBet);
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfdSaveHPT = new SaveFileDialog();
            sfdSaveHPT.InitialDirectory = CombBet.SaveDirectory;
            sfdSaveHPT.FileName = CombBet.ToFileNameString() + ".hpt7";
            sfdSaveHPT.Filter = "Hjälp på traven-system|*.hpt7";
            sfdSaveHPT.FileOk += new System.ComponentModel.CancelEventHandler(sfdSaveHPT_FileOk);
            sfdSaveHPT.ShowDialog();
        }

        void sfdSaveHPT_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    SaveFileDialog sfd = (SaveFileDialog)sender;
                    HPTSerializer.SerializeHPTCombinationSystem(sfd.FileName, CombBet);
                }
                catch (Exception exc)
                {
                    string error = exc.Message;
                }
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //UCMarkingBetSystemDocument uc = new UCMarkingBetSystemDocument();
            //uc.DataContext = this.combBet;
            //PrintDialog pd = new PrintDialog();
            //if ((bool)pd.ShowDialog().GetValueOrDefault())
            //{
            //    uc.Measure(new Size(816, 1500));
            //    uc.Arrange(new Rect(new Size(816, 1300)));
            //    uc.UpdateLayout();
            //    pd.PrintVisual(uc, "HPT");
            //}
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            string systemInfo = CombBet.ToClipboardString();
            Clipboard.SetDataObject(systemInfo);
        }

        private void btnCreateCouponsAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = CombBet.SaveDirectory;
            sfd.FileName = CombBet.ToFileNameString() + ".xml";
            sfd.Filter = "ATG-kupongfiler (*.xml)|*.xml|Alla filer (*.*)|*.*";
            sfd.FileOk += new System.ComponentModel.CancelEventHandler(sfd_FileOk);
            sfd.ShowDialog();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateFromATG();
        }

        #endregion

        #region Update handling

        Timer atgUpdateTimer;
        private void cmbUpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (atgUpdateTimer == null)
            {
                atgUpdateTimer = new Timer(new TimerCallback(UpdateFromATG));
            }
            if (cmbUpdateInterval.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)cmbUpdateInterval.SelectedItem;
                int updatePeriod = Convert.ToInt32(cbi.Tag) * 60 * 1000;
                if (updatePeriod == 0)
                {
                    atgUpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
                else
                {
                    atgUpdateTimer.Change(0, updatePeriod);
                }
            }
        }

        private void UpdateFromATG(object timerData)
        {
            Dispatcher.Invoke(new Action(UpdateFromATG), null);
        }

        private void UpdateFromATG()
        {
            try
            {
                //Cursor = Cursors.Wait;

                // Deaktivera knappen
                btnUpdate.IsEnabled = false;

                var serviceConnector = new HPTServiceConnector();
                // TODO: DD osv i framtiden
                //serviceConnector.GetRaceDayInfoUpdate(CombBet.RaceDayInfo.BetType.Code, CombBet.RaceDayInfo.TrackId, CombBet.RaceDayInfo.RaceDayDate, UpdateFromATG);
            }
            catch (Exception)
            {
                btnUpdate.IsEnabled = true;
            }
        }

        // TODO: Ny lösning, i framtiden...
        //public void UpdateFromATG(HPTService.HPTRaceDayInfo rdi)
        //{
        //    try
        //    {
        //        // TODO: DD osv i framtiden
        //        //Dispatcher.Invoke(new Action<HPTService.HPTRaceDayInfo>(UpdateFromATGInvoke), rdi);
        //    }
        //    catch (Exception)
        //    {
        //        btnUpdate.IsEnabled = true;
        //    }
        //}

        // TODO: Ny lösning
        //private void UpdateFromATGInvoke(HPTService.HPTRaceDayInfo rdi)
        //{
        //    try
        //    {
        //        CombBet.RaceDayInfo.Merge(rdi);
        //        CombBet.TimeStamp = DateTime.Now;
        //        DeselectScratchedHorses();

        //    }
        //    catch (Exception exc)
        //    {
        //        btnUpdate.IsEnabled = true;
        //        HPTConfig.AddToErrorLogStatic(exc);
        //    }
        //    Cursor = Cursors.Arrow;
        //    btnUpdate.IsEnabled = true;
        //}

        internal void DeselectScratchedHorses()
        {
            CombBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList)
                .Where(h => h.Selected)
                .Where(h => h.Scratched == true)
                .ToList()
                .ForEach(h =>
                {
                    h.Selected = false;
                    if (h.TrioInfo != null)
                    {
                        h.TrioInfo.PlaceInfo1.Selected = false;
                        h.TrioInfo.PlaceInfo2.Selected = false;
                        h.TrioInfo.PlaceInfo3.Selected = false;
                    }
                });
        }

        #endregion

        #region Target return handling

        private void cmbTargetReturn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBoxItem cbi = (ComboBoxItem)e.AddedItems[0];
                string s = (string)cbi.Content;
                int targetReturn = 0;
                if (int.TryParse(s, out targetReturn))
                {
                    CombBet.TargetReturn = targetReturn;
                }
            }
        }

        //private void btnUpdateTargetReturn_Click(object sender, RoutedEventArgs e)
        //{
        //    int targetReturn = 0;
        //    if (int.TryParse(this.txtTargetReturn.Text, out targetReturn))
        //    {
        //        this.CombBet.TargetReturn = targetReturn;
        //    }
        //}
    }

    #endregion
}

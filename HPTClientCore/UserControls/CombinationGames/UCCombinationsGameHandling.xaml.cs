using Microsoft.Win32;
using System;
using System.Linq;
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
            this.atgUpdateTimer = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateFromATG));

            InitializeComponent();

            // Hantering av gratisanvändare som öppnar fil med icke stödda spelformer
            if (!HPTConfig.Config.IsPayingCustomer)
            {
                this.btnCreateCoupons.IsEnabled = false;
                this.btnCreateCouponsAs.IsEnabled = false;
                this.btnUpdate.IsEnabled = false;
            }
        }

        #region Button clicks

        private void btnCreateCoupons_Click(object sender, RoutedEventArgs e)
        {
            string fileName = this.CombBet.SaveDirectory + this.CombBet.ToFileNameString();
            this.CombBet.SystemFilename = fileName + ".xml";
            ATGCouponHelper couponHelper = new ATGCouponHelper(this.CombBet);
            couponHelper.CreateCoupons();
            couponHelper.CreateATGFile();
            HPTSerializer.SerializeHPTCombinationSystem(fileName + ".hpt7", this.CombBet);
        }

        void sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    SaveFileDialog sfd = (SaveFileDialog)sender;
                    string fileName = sfd.FileName;
                    this.CombBet.SystemFilename = fileName;
                    ATGCouponHelper couponHelper = new ATGCouponHelper(this.CombBet);
                    couponHelper.CreateCoupons();
                    couponHelper.CreateATGFile();
                    HPTSerializer.SerializeHPTCombinationSystem(fileName.Replace(".xml", ".hpt7"), this.CombBet);
                }
                catch (Exception exc)
                {
                    string error = exc.Message;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            HPTSerializer.SerializeHPTCombinationSystem(this.CombBet.SaveDirectory + this.CombBet.ToFileNameString() + ".hpt5", this.CombBet);
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfdSaveHPT = new SaveFileDialog();
            sfdSaveHPT.InitialDirectory = this.CombBet.SaveDirectory;
            sfdSaveHPT.FileName = this.CombBet.ToFileNameString() + ".hpt7";
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
                    HPTSerializer.SerializeHPTCombinationSystem(sfd.FileName, this.CombBet);
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
            string systemInfo = this.CombBet.ToClipboardString();
            Clipboard.SetDataObject(systemInfo);
        }

        private void btnCreateCouponsAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = this.CombBet.SaveDirectory;
            sfd.FileName = this.CombBet.ToFileNameString() + ".xml";
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

        System.Threading.Timer atgUpdateTimer;
        private void cmbUpdateInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (atgUpdateTimer == null)
            {
                this.atgUpdateTimer = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateFromATG));
            }
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
                    this.atgUpdateTimer.Change(0, updatePeriod);
                }
            }
        }

        private void UpdateFromATG(object timerData)
        {
            this.Dispatcher.Invoke(new Action(UpdateFromATG), null);
        }

        private void UpdateFromATG()
        {
            try
            {
                //Cursor = Cursors.Wait;

                // Deaktivera knappen
                this.btnUpdate.IsEnabled = false;

                var serviceConnector = new HPTServiceConnector();
                serviceConnector.GetRaceDayInfoUpdate(this.CombBet.RaceDayInfo.BetType.Code, this.CombBet.RaceDayInfo.TrackId, this.CombBet.RaceDayInfo.RaceDayDate, UpdateFromATG);
            }
            catch (Exception)
            {
                this.btnUpdate.IsEnabled = true;
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
                this.CombBet.RaceDayInfo.Merge(rdi);
                this.CombBet.TimeStamp = DateTime.Now;
                DeselectScratchedHorses();

            }
            catch (Exception exc)
            {
                this.btnUpdate.IsEnabled = true;
                HPTConfig.AddToErrorLogStatic(exc);
            }
            Cursor = Cursors.Arrow;
            this.btnUpdate.IsEnabled = true;
        }

        internal void DeselectScratchedHorses()
        {
            this.CombBet.RaceDayInfo.RaceList
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
                    this.CombBet.TargetReturn = targetReturn;
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

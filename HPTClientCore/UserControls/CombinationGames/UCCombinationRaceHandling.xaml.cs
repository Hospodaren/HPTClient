using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCCombinationRaceHandling.xaml
    /// </summary>
    public partial class UCCombinationRaceHandling : UCCombBetControl
    {
        public UCCombinationRaceHandling()
        {
            InitializeComponent();

            // Hantering av gratisanvändare som öppnar fil med icke stödda spelformer
            if (!HPTConfig.Config.IsPayingCustomer)
            {
                this.btnCreateCoupons.IsEnabled = false;
                this.btnCreateCouponsAs.IsEnabled = false;
                this.btnCopy.IsEnabled = false;
                this.btnPrint.IsEnabled = false;
            }
        }

        #region Dependency properties

        private HPTCombinationListInfo combinationListInfo;
        public HPTCombinationListInfo CombinationListInfo
        {
            get
            {
                if (this.DataContext != null && this.DataContext.GetType() == typeof(HPTCombinationListInfo))
                {
                    this.combinationListInfo = (HPTCombinationListInfo)this.DataContext;
                }
                return this.combinationListInfo;
            }
        }

        #endregion

        #region Button clicks

        private void btnCreateCoupons_Click(object sender, RoutedEventArgs e)
        {
            var race = CombinationListInfo.CombinationList.First().Horse1.ParentRace;

            string fileName = this.CombBet.SaveDirectory + this.CombBet.ToFileNameString();
            this.CombBet.SystemFilename = fileName + ".xml";

            HPTSerializer.SerializeHPTCombinationSystem(this.CombBet.SaveDirectory + this.CombBet.ToFileNameString(race, this.CombinationListInfo) + ".hpt5", this.CombBet);
            ATGCouponHelper couponHelper = new ATGCouponHelper(this.CombBet);
            couponHelper.CreateCombinationCoupons(this.CombinationListInfo);
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
                    couponHelper.CreateCombinationCoupons(this.CombinationListInfo);
                    HPTSerializer.SerializeHPTCombinationSystem(fileName.Replace(".xml", ".hpt5"), this.CombBet);
                }
                catch (Exception exc)
                {
                    string error = exc.Message;
                }
            }
        }

        #endregion
    }
}

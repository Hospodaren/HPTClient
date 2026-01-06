using Microsoft.Win32;
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
                btnCreateCoupons.IsEnabled = false;
                btnCreateCouponsAs.IsEnabled = false;
                btnCopy.IsEnabled = false;
                btnPrint.IsEnabled = false;
            }
        }

        #region Dependency properties

        private HPTCombinationListInfo combinationListInfo;
        public HPTCombinationListInfo CombinationListInfo
        {
            get
            {
                if (DataContext != null && DataContext.GetType() == typeof(HPTCombinationListInfo))
                {
                    combinationListInfo = (HPTCombinationListInfo)DataContext;
                }
                return combinationListInfo;
            }
        }

        #endregion

        #region Button clicks

        private void btnCreateCoupons_Click(object sender, RoutedEventArgs e)
        {
            var race = CombinationListInfo.CombinationList.First().Horse1.ParentRace;

            string fileName = CombBet.SaveDirectory + CombBet.ToFileNameString();
            CombBet.SystemFilename = fileName + ".xml";

            HPTSerializer.SerializeHPTCombinationSystem(CombBet.SaveDirectory + CombBet.ToFileNameString(race, CombinationListInfo) + ".hpt7", CombBet);
            ATGCouponHelper couponHelper = new ATGCouponHelper(CombBet);
            couponHelper.CreateCombinationCoupons(CombinationListInfo);
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
                    CombBet.SystemFilename = fileName;
                    ATGCouponHelper couponHelper = new ATGCouponHelper(CombBet);
                    couponHelper.CreateCombinationCoupons(CombinationListInfo);
                    HPTSerializer.SerializeHPTCombinationSystem(fileName.Replace(".xml", ".hpt7"), CombBet);
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

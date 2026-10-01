using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCUserCommentsUpload.xaml
    /// </summary>
    public partial class UCUserSystemUpload : UCMarkBetControl
    {
        public UCUserSystemUpload()
        {
            InitializeComponent();
        }



        private void hlSystemURL_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(this.MarkBet.SystemURL);
            GoToUrl(this.MarkBet.SystemURL);
        }

        private void GoToUrl(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        #region Obsolete
        //private void btnUploadComments_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.txtCommentDescription.Text.Trim() == string.Empty && this.chkMakeSystemPublic.IsChecked == true)
        //    {
        //        MessageBox.Show("Du måste fylla i en beskrivning av ditt system", "Beskrivning saknas", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    else
        //    {
        //        this.spSystemUploaded.Visibility = System.Windows.Visibility.Collapsed;
        //        var dr = MessageBox.Show("Ladda upp komplett system till HPTs server?", "Ladda upp system", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        //        if (dr == MessageBoxResult.Yes)
        //        {
        //            this.MarkBet.PrepareForSave();
        //            if (this.MarkBet.LastSaveTime == DateTime.MinValue)
        //            {
        //                HPTSerializer.SerializeHPTSystem(this.MarkBet.MailSender.HPT3FileName, this.MarkBet);
        //            }
        //            var serviceConnector = new HPTServiceConnector();
        //            //if (serviceConnector.UploadSystem(this.MarkBet))
        //            if (serviceConnector.UploadCompleteSystem(this.MarkBet, (bool)this.chkMakeSystemPublic.IsChecked, true))
        //            {
        //                this.spSystemUploaded.Visibility = System.Windows.Visibility.Visible;
        //                this.lblSystemURL.Visibility = System.Windows.Visibility.Visible;
        //                this.tbUploadComplete.Visibility = System.Windows.Visibility.Visible;
        //                this.tbError.Visibility = System.Windows.Visibility.Collapsed;
        //                //MessageBox.Show("System uppladdat till HPTs server.", "System uppladdat", MessageBoxButton.OK, MessageBoxImage.Information);
        //            }
        //            else
        //            {
        //                this.spSystemUploaded.Visibility = System.Windows.Visibility.Visible;
        //                this.lblSystemURL.Visibility = System.Windows.Visibility.Collapsed;
        //                this.tbUploadComplete.Visibility = System.Windows.Visibility.Collapsed;
        //                this.tbError.Visibility = System.Windows.Visibility.Visible;
        //                //MessageBox.Show("Något gick snett vid uppladdning av system. Se fellogg för detaljer", "Problem vid systemuppladdning", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}

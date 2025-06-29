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
//using System.Net;
//using System.Net.Mail;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCSendMail.xaml
    /// </summary>
    public partial class UCSendMail : UCMarkBetControl
    {
        public UCSendMail()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (this.IsVisible)
                {
                    UpdateAll();
                    //if (this.MarkBet.UploadedSystemGUID != null && this.MarkBet.UploadedSystemGUID != string.Empty)
                    //{
                    //    string url = "http://correction.hpt.nu/Default.aspx?SystemGUID=" + this.MarkBet.UploadedSystemGUID;
                    //    this.hlCorrectionURL.NavigateUri = new Uri(url);
                    //    this.txtCorrectionURL.Text = url;
                    //}
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void UpdateAll()
        {
            this.MarkBet.MailSender.AttachATGSystemFile = (bool)this.chkAttachSystemFile.IsChecked;
            this.MarkBet.MailSender.AttachHPT3File = (bool)this.chkAttachHPTFile.IsChecked;

            if ((bool)this.chkAttachSystemFile.IsChecked)
            {
                this.MarkBet.MailSender.ATGSystemFileName = this.MarkBet.SystemFilename;
            }

            HPTSerializer.SerializeHPTSystem(this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString() + ".hpt5", this.MarkBet);
            if ((bool)this.chkAttachHPTFile.IsChecked)
            {
                this.MarkBet.MailSender.HPT3FileName = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString() + ".hpt5";
            }

            SetSubject();
            SetBody();
        }

        //private HPTMarkBet markBet;
        //public HPTMarkBet MarkBet
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (this.markBet == null)
        //            {
        //                this.markBet = (HPTMarkBet)this.DataContext;
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            string s = exc.Message;
        //            return new HPTMarkBet();
        //        }
        //        return this.markBet;
        //    }
        //}

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ucMailList.SelectedMailList == null || this.ucMailList.SelectedMailList.RecipientList.Count == 0)
                {
                    MessageBox.Show("Det saknas mottagare. Du måste välja en av mottagarlistorna till vänster, eller skapa en ny om befintliga saknas.", "Inga mottagare", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                this.MarkBet.MailSender.MailRecipients = this.ucMailList.SelectedMailList.RecipientList;
                this.MarkBet.MailSender.Subject = this.txtSubject.Text;
                this.MarkBet.MailSender.Body = this.txtBody.Text;
                
                HPTServiceConnector.SendMail(this.MarkBet.MailSender, this.MarkBet);
                MessageBox.Show("Mailet har nu skickats till valda mottagare.", "E-Post skickat", MessageBoxButton.OK);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
                MessageBox.Show("Det gick inte att sända mailet:\r\n" + exc.Message, "E-Postskickning misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateAll();
        }

        private void chkAttachHPTFile_Checked(object sender, RoutedEventArgs e)
        {
            if (this.MarkBet != null)
            {
                this.MarkBet.MailSender.AttachHPT3File = (bool)this.chkAttachHPTFile.IsChecked;
            }            
        }

        private void chkAttachSystemFile_Checked(object sender, RoutedEventArgs e)
        {
            if (this.MarkBet != null)
            {
                this.MarkBet.MailSender.AttachATGSystemFile = (bool)this.chkAttachSystemFile.IsChecked;

                if (this.MarkBet.SystemFilename != null && this.MarkBet.SystemFilename != string.Empty && this.MarkBet.MailSender.AttachATGSystemFile && this.MarkBet.MailSender.ATGSystemFileName == null)
                {
                    this.MarkBet.MailSender.ATGSystemFileName = this.MarkBet.SystemFilename;
                }                
            }            
        }

        private void chkInsertSystemInfo_Checked(object sender, RoutedEventArgs e)
        {
            SetBody();
        }

        private void chkInsertCoupons_Checked(object sender, RoutedEventArgs e)
        {
            SetBody();
        }

        private void chkInsertSingleRows_Checked(object sender, RoutedEventArgs e)
        {
            SetBody();
        }

        private void SetBody()
        {
            if (this.MarkBet == null)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            if ((bool)this.chkInsertSystemInfo.IsChecked)
            {
                sb.AppendLine(this.MarkBet.ToClipboardString());
            }
            if ((bool)this.chkInsertCoupons.IsChecked)
            {
                sb.AppendLine(this.MarkBet.CouponCorrector.CouponHelper.ToCouponsString());
            }
            if ((bool)this.chkInsertSingleRows.IsChecked)
            {
                if (this.MarkBet.ReducedSize < this.MarkBet.SystemSize)
                {
                    sb.AppendLine(this.MarkBet.SingleRowCollection.ToSingleRowsString());
                }
            }
            if (!string.IsNullOrEmpty(this.MarkBet.SystemURL))
            {
                sb.Append("RättningsURL: ");
                sb.AppendLine(this.MarkBet.SystemURL);
            }
            //if (this.MarkBet.UploadedSystemGUID != null && this.MarkBet.UploadedSystemGUID != string.Empty)
            //{
            //    sb.Append("RättningsURL: http://correction.hpt.nu/Default.aspx?SystemGUID=");
            //    sb.AppendLine(this.MarkBet.UploadedSystemGUID);
            //}
            this.txtBody.Text = sb.ToString();
        }

        private void SetSubject()
        {
            this.MarkBet.MailSender.Subject = this.MarkBet.BetType.Code + " " + this.MarkBet.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd") + ", " + this.MarkBet.ReducedSize.ToString() + " rader.";
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
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

            this.hlCorrectionURL.NavigateUri = new Uri(this.MarkBet.SystemURL);
            this.txtCorrectionURL.Text = this.MarkBet.SystemURL;
            
            //try
            //{
            //    MessageBoxResult result = MessageBox.Show("Ladda upp system till HPTs server. HPT- och ATG-filer kommer automatiskt att sparas med automatgenerade namn först. Gör du några ändringar i systemet innan inlämning eller mailskickning måste du ladda upp det på nytt.", "Ladda upp system", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

            //    if (result == MessageBoxResult.Yes)
            //    {
            //        // Spara hpt5- och ATG-fil
            //        if (this.MarkBet.CouponCorrector.CouponHelper.CouponList.Count == 0)
            //        {
            //            this.MarkBet.CouponCorrector.CouponHelper.CreateCoupons();
            //            this.MarkBet.CouponCorrector.CouponHelper.CreateATGFile();
            //        }
            //        string hpt3Filename = this.MarkBet.SaveDirectory + this.MarkBet.ToFileNameString() + ".hpt5";
            //        this.MarkBet.MailSender.HPT3FileName = hpt3Filename;
            //        HPTSerializer.SerializeHPTSystem(hpt3Filename, this.MarkBet);

            //        // Ladda upp fil till servern
            //        var serviceConnector = new HPTServiceConnector();
            //        serviceConnector.SaveSystem(this.MarkBet);

            //        // Fixa till mailet
            //        SetBody();
            //        string url = "http://correction.hpt.nu/Default.aspx?SystemGUID=" + this.MarkBet.UploadedSystemGUID;
            //        this.hlCorrectionURL.NavigateUri = new Uri(url);
            //        this.txtCorrectionURL.Text = url;
            //        MessageBox.Show("System uppladdat till HPTs server.", "Uppladning klar", MessageBoxButton.OK);
            //    }                
            //}
            //catch (Exception exc)
            //{
            //    string s = exc.Message;
            //    MessageBox.Show("Uppladdningen av system misslyckades, var vänlig försök igen senare.", "Uppladning misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void hlCorrectionURL_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(this.hlCorrectionURL.NavigateUri.ToString());
            e.Handled = true;
        }

        private void btnClearRecipients_Click(object sender, RoutedEventArgs e)
        {
            // Kod här
            this.ucMailList.SelectedMailList.RecipientList.Clear();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(this.txtBody.Text);
            }
            catch (Exception exc)
            {
                string fel = exc.Message;
            }
        }
    }
}

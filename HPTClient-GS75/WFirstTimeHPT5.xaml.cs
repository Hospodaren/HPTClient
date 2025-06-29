using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for WFirstTimeHPT5.xaml
    /// </summary>
    public partial class WFirstTimeHPT5 : Window
    {
        public WFirstTimeHPT5()
        {
            InitializeComponent();
        }

        #region Eventhantering

        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBoxResult result = MessageBox.Show("Spara alla ändringar och skriva över tidigare värden?\r\n\r\nOBS! Om du ändrar E-postadress kommer eventuell produktnyckel inte fungera.", "Spara ändringar", MessageBoxButton.YesNoCancel);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        HPTConfig.Config.Password = this.txtPassword.Text;
        //        //HPTConfig.Config.UserName = this.txtUserName.Text;
        //        HPTConfig.Config.EMailAddress = this.txtEMailAddress.Text;
        //        //HPTConfig.Config.SetValuesInIsolatedStorage();
        //    }
        //}

        //private void btnCancel_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBoxResult result = MessageBox.Show("Stäng fönstret utan att spara ändrade värden?", "Avbryt", MessageBoxButton.YesNoCancel);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        this.Close();
        //    }
        //}

        private void btnGetKey_Click(object sender, RoutedEventArgs e)
        {
            //string messageBoxText = "Vill du få din produktnyckel skickad till adressen " + HPTConfig.Config.EMailAddress + "?";
            string messageBoxText = $"Maila hjalp.pa.traven@gmail.com från adressen {this.txtEMailAddress.Text} om du glömt nyckel.";
            MessageBoxResult result = MessageBox.Show(messageBoxText, "Hämta nyckel", MessageBoxButton.OK);
            //if (result == MessageBoxResult.Yes)
            //{
            //    HPTConfig.Config.EMailAddress = this.txtEMailAddress.Text;
            //    HPTConfig.Config.Password = string.Empty;
            //    HPTServiceConnector.GetKey();
            //}
        }

        internal bool GoToPayson;   // Meddela huvudfönstret att en flik med Payson-sidan ska öppnas
        //private void btnPROTrial_Click(object sender, RoutedEventArgs e)
        //{
        //    string messageBoxText = "Vill du registera dig för att kunna prova 'Hjälp på traven PRO' gratis i 30 dagar? Du kommer att få en produktnyckel skickad till adressen " + this.txtEMailAddress.Text + " inom 24 timmar.";
        //    MessageBoxResult result = MessageBox.Show(messageBoxText, "Prova HPT Pro gratis i 30 dagar", MessageBoxButton.YesNoCancel);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        if (!IsCorrectEMailAddress(this.txtEMailAddress.Text))
        //        {
        //            MessageBox.Show("Ogiltigt format på E-Postadress!", "Ogiltig E-Postadress", MessageBoxButton.OK);
        //            this.txtEMailAddress.Focus();
        //            this.txtEMailAddress.SelectAll();
        //            return;
        //        }
        //        SaveChanges();
        //        try
        //        {
        //            HPTService.HPTRegistration registration = HPTServiceConnector.Register();

        //            if (registration.AlreadyRegistered)
        //            {
        //                MessageBoxResult resultRegistration = MessageBox.Show("Provperiod redan utnyttjad. Tryck Ja för att gå till Payson och skaffa ett årsabonnemang.", "Du har redan utnyttjat din gratis provperiod!", MessageBoxButton.YesNoCancel);
        //                if (resultRegistration == MessageBoxResult.Yes)
        //                {
        //                    this.GoToPayson = true;
        //                    this.Close();
        //                }
        //            }
        //            else if (!string.IsNullOrEmpty(registration.ErrorMessage) && !string.IsNullOrEmpty(registration.ErrorMessage))
        //            {
        //                //MessageBox.Show("Något gick fel vid registreringen, var vänlig försök senare.", "Fel vid registrering", MessageBoxButton.OK);
        //                MessageBox.Show(registration.ErrorMessage, registration.Message, MessageBoxButton.OK);
        //            }
        //            else
        //            {
        //                MessageBox.Show("Du kommer inom 24 timmar att få en produktnyckel skickad med E-post till " + HPTConfig.Config.EMailAddress + ".\r\n\r\nDenna nyckel ska anges i fältet 'Nyckel'. Tryck därefter på knappen 'Validera' för att få åtkomst till alla funktioner i HPT Pro.\r\n\r\nOBS! Om inte nyckeln dyker upp, börja med att kontrollera skräpposten.", "Registrering genomförd", MessageBoxButton.OK);
        //            }
        //        }
        //        catch (Exception exc)
        //        {
        //            string s = exc.Message;
        //            MessageBox.Show("Något gick fel vid registreringen, var vänlig försök senare.", "Registrering inte tillgänglig", MessageBoxButton.OK);
        //        }
        //    }
        //}

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Vill du validera produktnyckel mot server för att kunna använda HPT Pro?", "Validera produktnyckel", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                SaveChanges();

                if (!IsCorrectKey(HPTConfig.Config.Password))
                {
                    MessageBox.Show("Felaktigt format och/eller längd på produktnyckeln. Ingen validering kommer att göras.", "Felaktig produktnyckel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                ATGCalendar wCalendar = (ATGCalendar)this.Owner;
                wCalendar.HandleFreeAndPro();
                try
                {
                    Cursor = Cursors.Wait;
                    wCalendar.HandleValidatedPro();
                    HPTConfig.Config.SetDefaultsForPayingCustomer();
                }
                catch (Exception exc)
                {
                    wCalendar.Config.AddToErrorLog(exc);
                }
                SetTextAndButtonStatus();
                Cursor = Cursors.Arrow;
            }
        }

        private bool IsCorrectEMailAddress(string emailAddress)
        {
            Regex rexEMail = new Regex("\\b[A-Z0-9\\._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}\\b", RegexOptions.IgnoreCase);
            return rexEMail.IsMatch(emailAddress);
        }

        private bool IsCorrectKey(string keyString)
        {
            Regex rexGUID = new Regex(@"[\da-fA-F]{32}");
            return rexGUID.IsMatch(keyString);
        }

        #endregion

        #region Metodanrop

        private void SaveChanges()
        {
            HPTConfig.Config.Password = this.txtPassword.Text.Trim();
            //HPTConfig.Config.UserName = this.txtUserName.Text.Trim();
            HPTConfig.Config.EMailAddress = this.txtEMailAddress.Text.Trim();
        }

        #endregion

        private void txtEMailAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtEMailAddress.Text == HPTConfig.Config.EMailAddress && this.txtPassword.Text == HPTConfig.Config.Password)
            {
                this.btnStartProgram.IsEnabled = true;
                return;
            }
            this.btnStartProgram.IsEnabled = false;
            if (IsCorrectEMailAddress(this.txtEMailAddress.Text))
            {
                this.btnGetKey.IsEnabled = true;
                //this.btnPROTrial.IsEnabled = true;
                if (IsCorrectKey(this.txtPassword.Text))
                {
                    this.btnValidate.IsEnabled = true;
                }
            }
            else
            {
                this.btnGetKey.IsEnabled = false;
                //this.btnPROTrial.IsEnabled = false;
                this.btnValidate.IsEnabled = false;
            }
        }

        private void btnBuyAYearsLicense_Click(object sender, RoutedEventArgs e)
        {
            var url = HPTConfig.PaysonURL.Replace("&amp;", "&");
            System.Diagnostics.Process.Start(url);
        }

        private void btnBuyThreeMonthsLicense_Click(object sender, RoutedEventArgs e)
        {
            var url = HPTConfig.PaysonURLThreeMonths.Replace("&amp;", "&");
            System.Diagnostics.Process.Start(url);
        }

        private void rbSimple_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                e.Handled = true;
                var fe = (FrameworkElement)sender;
                HPTConfig.Config.Profile = (GUIProfile)Convert.ToInt32(fe.Tag);
                HPTConfig.Config.SetMarkBetProfile(HPTConfig.Config.Profile);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                e.Handled = true;
                SetTextAndButtonStatus();
                switch (HPTConfig.Config.Profile)
                {
                    case GUIProfile.Simple:
                        this.rbSimple.IsChecked = true;
                        break;
                    case GUIProfile.Normal:
                        this.rbStandard.IsChecked = true;
                        break;
                    case GUIProfile.Advanced:
                        this.rbAdvanced.IsChecked = true;
                        break;
                    case GUIProfile.Custom:
                        break;
                    default:
                        break;
                }
            }
        }

        internal void SetTextAndButtonStatus()
        {
            try
            {
                if (HPTConfig.Config.IsPayingCustomer)
                {
                    // Dölj onödiga knappar
                    this.btnGetKey.IsEnabled = false;
                    //this.btnOneYearPro.IsEnabled = false;
                    //this.btnPROTrial.IsEnabled = false;
                    //this.btnThreeMonthsPro.IsEnabled = false;
                    this.btnValidate.IsEnabled = false;

                    // Nu kan man starta programmet
                    this.btnStartProgram.IsEnabled = true;

                    this.txtRegistrationInformation.Text = "Ditt abonnemang gäller till " + HPTConfig.Config.PROVersionExpirationDate.ToString("yyyy-MM-dd");
                }
                else if (HPTConfig.Config.PROVersionExpirationDate > DateTime.Now.AddYears(-30))
                {
                    // Dölj onödiga knappar
                    this.btnGetKey.IsEnabled = false;
                    this.btnOneYearPro.IsEnabled = true;
                    //this.btnPROTrial.IsEnabled = false;
                    this.btnThreeMonthsPro.IsEnabled = true;
                    this.btnValidate.IsEnabled = true;

                    // Nu kan man starta programmet
                    this.btnStartProgram.IsEnabled = true;

                    this.txtRegistrationInformation.Text = "Ditt abonnemang slutade gälla " + HPTConfig.Config.PROVersionExpirationDate.ToString("yyyy-MM-dd")
                        + ". Använd knapparna nedan för att förlänga abonnemanget, eller tryck 'Testa' för att köra programmet utan möjlughet att lämna in.";
                }
                else
                {
                    // Dölj onödiga knappar
                    this.btnGetKey.IsEnabled = true;
                    this.btnOneYearPro.IsEnabled = true;
                    //this.btnPROTrial.IsEnabled = true;
                    this.btnThreeMonthsPro.IsEnabled = true;
                    this.btnValidate.IsEnabled = true;

                    // Nu kan man starta programmet
                    this.btnStartProgram.IsEnabled = false;

                    this.txtRegistrationInformation.Text = "Man måste köpa abonnemang för att använda HPT version 5 fullt ut. Har du fått nyckel tidigare kan du fylla i den tillsammans med nyckeln och validera med 'Validera nyckel'. "
                        + "Du kan testa alla funktioner i programmet utan möjlighet att lämna in systemet genom att trycka på 'Testa'. "
                        + "Har du glömt din nyckel kan du fylla i din e-postadress och klicka på 'Glömt nyckel'. Du får då din nyckel via e-post inom kort.";
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnStartProgram_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //if (HPTConfig.Config.PROVersionExpirationDate < DateTime.Now.AddYears(-30))
            //{
            //    var result = MessageBox.Show("Du måste ha ett konto för att kunna använda HPT. Tryck Ja för att fylla i giltig information eller skaffa ett gratisabonnemang. Tryck Nej för att stänga programmet.", "Skaffa ett abbonemang?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //    if (result == MessageBoxResult.Yes)
            //    {
            //        e.Cancel = true;
            //    }
            //    else
            //    {
            //        this.DialogResult = false;
            //        base.OnClosing(e);
            //    }
            //}
            //else
            //{
            //    base.OnClosing(e);
            //}
        }

        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var b = (SolidColorBrush)this.txtPassword.Background;
                if (b.Color == Colors.White)
                {
                    this.txtPassword.Background = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    this.txtPassword.Background = new SolidColorBrush(Colors.White);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void btnTryFree_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Vill du testa funktionerna i 'Hjälp på traven PRO' utan att registrera dig? Alla funktioner utom att spara och lämna in är tillgängliga.";
            MessageBoxResult result = MessageBox.Show(messageBoxText, "Testa funktionerna i HPT Pro", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                SaveChanges();
                HPTConfig.Config.PROVersionExpirationDate = DateTime.Now.AddDays(-1D);
                HPTConfig.Config.IsPayingCustomer = false;
                HPTConfig.Config.FirstTimeHPT5User = true;
                this.DialogResult = true;
                this.Close();
            }
            else if (result == MessageBoxResult.No)
            {
                Application.Current.Shutdown();
            }
            }
    }
}

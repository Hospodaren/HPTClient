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
    /// Interaction logic for WRegistration.xaml
    /// </summary>
    public partial class WRegistration : Window
    {
        public WRegistration()
        {
            InitializeComponent();
        }

        #region Eventhantering

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Spara alla ändringar och skriva över tidigare värden?\r\n\r\nOBS! Om du ändrar E-postadress kommer eventuell produktnyckel inte fungera.", "Spara ändringar", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                HPTConfig.Config.Password = this.txtPassword.Text;
                HPTConfig.Config.UserName = this.txtUserName.Text;
                HPTConfig.Config.EMailAddress = this.txtEMailAddress.Text;
                //HPTConfig.Config.SetValuesInIsolatedStorage();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Stäng fönstret utan att spara ändrade värden?", "Avbryt", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnGetKey_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = $"Maila hjalp.pa.traven@gmail.com från adressen {this.txtEMailAddress.Text} om du glömt nyckel.";
            MessageBoxResult result = MessageBox.Show(messageBoxText, "Hämta nyckel", MessageBoxButton.OK);
        }

        internal bool GoToPayson;   // Meddela huvudfönstret att en flik med Payson-sidan ska öppnas
        //private void btnPROTrial_Click(object sender, RoutedEventArgs e)
        //{
        //    //MessageBox.Show("Registrera testperioden från Version 3.63", "Använd version 3.63", MessageBoxButton.OK);
        //    //return;

        //    string messageBoxText = "Vill du registera dig för att kunna prova 'Hjälp på traven PRO' gratis i 30 dagar? Du kommer att få en produktnyckel skickad till adressen " + this.txtEMailAddress.Text +" inom 24 timmar.";
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
        //            else if (registration.ErrorMessage != null && registration.ErrorMessage != string.Empty)
        //            {
        //                //MessageBox.Show("Något gick fel vid registreringen, var vänlig försök senare.", "Fel vid registrering", MessageBoxButton.OK);
        //                MessageBox.Show("Något gick fel vid registreringen. " + registration.ErrorMessage, "Fel vid registrering", MessageBoxButton.OK);
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
                Regex rexGUID = new Regex(@"[\da-fA-F]{32}");
                if (!rexGUID.IsMatch(HPTConfig.Config.Password))
                {
                    MessageBox.Show("Felaktigt format och/eller längd på produktnyckeln. Ingen validering kommer att göras.", "Felaktig produktnyckel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ATGCalendar wCalendar = (ATGCalendar)this.Owner;
                wCalendar.HandleFreeAndPro();
                try
                {
                    Cursor = Cursors.Wait;
                    //var serviceConnector = new HPTServiceConnector();
                    //HPTCalendar calendar = serviceConnector.GetCalendar();
                    //wCalendar.hptCalendar.RaceDayInfoList.Clear();
                    //foreach (HPTRaceDayInfo hptRdi in calendar.RaceDayInfoList)
                    //{
                    //    wCalendar.hptCalendar.RaceDayInfoList.Add(hptRdi);
                    //}
                    wCalendar.HandleValidatedPro();
                    HPTConfig.Config.SetDefaultsForPayingCustomer();
                    //HPTConfig.Config.SetValuesInIsolatedStorage();
                    //HPTSerializer.SerializeHPTConfig(HPTConfig.MyDocumentsPath + HPTConfig.ConfigFileName, HPTConfig.Config);
                    //HPTConfig.Config.SaveConfig();
                }
                catch (Exception exc)
                {
                    wCalendar.Config.AddToErrorLog(exc);
                }
                Cursor = Cursors.Arrow;
            }
        }

        private bool IsCorrectEMailAddress(string emailAddress)
        {
            Regex rexEMail = new Regex("\\b[A-Z0-9\\._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}\\b", RegexOptions.IgnoreCase);
            return rexEMail.IsMatch(emailAddress);
        }

        #endregion

        #region Metodanrop

        private void SaveChanges()
        {
            HPTConfig.Config.Password = this.txtPassword.Text.Trim();
            HPTConfig.Config.UserName = this.txtUserName.Text.Trim();
            HPTConfig.Config.EMailAddress = this.txtEMailAddress.Text.Trim();
        }

        #endregion
    }
}

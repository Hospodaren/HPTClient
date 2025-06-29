using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCUserSystemList.xaml
    /// </summary>
    public partial class UCUserSystemList : UserControl
    {
        public UCUserSystemList()
        {
            InitializeComponent();
        }

        private ATGCalendar atgCalendar;
        public UCUserSystemList(ATGCalendar atgCalendar)
        {
            this.atgCalendar = atgCalendar;
            InitializeComponent();
        }



        public HPTRaceDayInfoLight RaceDayInfoLight
        {
            get { return (HPTRaceDayInfoLight)GetValue(RaceDayInfoLightProperty); }
            set { SetValue(RaceDayInfoLightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RaceDayInfoLight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RaceDayInfoLightProperty =
            DependencyProperty.Register("RaceDayInfoLight", typeof(HPTRaceDayInfoLight), typeof(UCUserSystemList), new UIPropertyMetadata(null));



        //public List<HPTUserSystem> UserSystemsToDownloadList { get; set; }

        //private void btnDownloadSelectedSystems_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.UserSystemsToDownloadList == null || this.UserSystemsToDownloadList.Count == 0)
        //    {
        //        return;
        //    }

        //    if (this.UserSystemsToDownloadList.Count > 3)
        //    {
        //        var drSuccess = MessageBox.Show("Du får ladda ner maximalt tre system åt gången.", "För många valda", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }

        //    Cursor = Cursors.Wait;
        //    bool allSucceeded = true;
        //    foreach (var userSystem in this.UserSystemsToDownloadList)
        //    {
        //        var userMarkBet = HPTServiceConnector.DownloadSystemByUniqueId(this.RaceDayInfoLight, userSystem.UniqueId);
        //        if (userMarkBet == null)
        //        {
        //            allSucceeded = false;
        //        }
        //        else
        //        {
        //            HPTServiceToHPTHelper.SetNonSerializedValues(userMarkBet);
        //            userMarkBet.Config = HPTConfig.Config;
        //            userMarkBet.SaveDirectory = HPTConfig.MyDocumentsPath + userMarkBet.RaceDayInfo.ToDateAndTrackString() + "\\";
        //            userMarkBet.IsDeserializing = false;
        //            this.atgCalendar.AddTabItem(userMarkBet);
        //        }
        //    }
        //    if (allSucceeded)
        //    {
        //        var drSuccess = MessageBox.Show("Nedladdning av system slutförd utan problem.", "Nedladdning klar", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    else
        //    {
        //        var drError = MessageBox.Show("Nedladdning av minst ett system misslyckades, försök igen senare. Se fellogg för orsak.", "Nedladdning misslyckades", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //    Cursor = Cursors.Arrow;

        //    var ownerWindow = (Window)this.Parent;
        //    ownerWindow.Close();
        //}

        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (this.UserSystemsToDownloadList == null)
        //    {
        //        this.UserSystemsToDownloadList = new List<HPTUserSystem>();
        //    }
        //    var chk = (CheckBox)sender;
        //    var userSystem = (HPTUserSystem)chk.DataContext;
        //    if (chk.IsChecked == true)
        //    {
        //        this.UserSystemsToDownloadList.Add(userSystem);
        //    }
        //    else
        //    {
        //        this.UserSystemsToDownloadList.Remove(userSystem);
        //    }
        //}

        //private void btnDeleteSelectedSystems_Click_1(object sender, RoutedEventArgs e)
        //{
        //    if (this.UserSystemsToDownloadList == null || this.UserSystemsToDownloadList.Count == 0)
        //    {
        //        return;
        //    }
        //    var userSystemsToDelete = this.UserSystemsToDownloadList.Where(us => us.EMail == HPTConfig.Config.EMailAddress).ToList();
        //    if (userSystemsToDelete.Count > 0)
        //    {
        //        var drDelete = MessageBox.Show("Ta bort valda system från HPTs server?", "Ta bort system", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        //        if (drDelete == MessageBoxResult.Yes)
        //        {
        //            try
        //            {
        //                foreach (var userSystem in userSystemsToDelete)
        //                {
        //                    HPTServiceConnector.DeleteSystemByUniqueId(this.RaceDayInfoLight, userSystem.UniqueId);
        //                    var userSystemList = (ObservableCollection<HPTUserSystem>)this.lvwAvailableComments.ItemsSource;
        //                    userSystemList.Remove(userSystem);
        //                }                        
        //                MessageBox.Show("Borttagning av system slutförd utan problem.", "Borttagning klar", MessageBoxButton.OK, MessageBoxImage.Information);
        //            }
        //            catch (Exception exc)
        //            {
        //                HPTConfig.Config.AddToErrorLog(exc);
        //                MessageBox.Show("Borttagning av minst ett system misslyckades, försök igen senare. Se fellogg för orsak.", "Borttagning misslyckades", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //        }
        //    }
        //}

        private void btnRemoveSystem_Click_1(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)e.OriginalSource;
            if (fe.DataContext.GetType() != typeof(HPTUserSystem))
            {
                return;
            }
            var userSystemToDelete = (HPTUserSystem)fe.DataContext;
            try
            {
                HPTServiceConnector.DeleteSystemByUniqueId(this.RaceDayInfoLight, userSystemToDelete.UniqueId);
                var userSystemList = (ObservableCollection<HPTUserSystem>)this.lvwAvailableComments.ItemsSource;
                userSystemList.Remove(userSystemToDelete);
                MessageBox.Show("Borttagning av system slutförd utan problem.", "Borttagning klar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exc)
            {
                HPTConfig.Config.AddToErrorLog(exc);
                MessageBox.Show("Borttagning av system misslyckades, försök igen senare. Se fellogg för orsak.", "Borttagning misslyckades", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //private void btnGetCommentsList_Click(object sender, RoutedEventArgs e)
        //{
        //    this.UserSystemsToDownloadList = new List<HPTService.HPTUserSystem>();
        //    var userCommentsCollection = HPTServiceConnector.DownloadCommentsAll(this.MarkBet);
        //    this.DataContext = userCommentsCollection;
        //    BindingOperations.GetBindingExpression(this.lvwAvailableComments, ListView.ItemsSourceProperty).UpdateTarget();
        //}
    }
}

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

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCUserRaceDayInfoCommentsList.xaml
    /// </summary>
    public partial class UCUserRaceDayInfoCommentsList : UserControl
    {
        public UCUserRaceDayInfoCommentsList()
        {
            InitializeComponent();
        }

        public HPTMarkBet MarkBet
        {
            get { return (HPTMarkBet)GetValue(MarkBetProperty); }
            set { SetValue(MarkBetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkBet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkBetProperty =
            DependencyProperty.Register("MarkBet", typeof(HPTMarkBet), typeof(UCUserRaceDayInfoCommentsList), new UIPropertyMetadata(null));


        public List<HPTService.HPTUserComments> UserCommentsToDownloadList { get; set; }

        //private void btnDownloadSelectedComments_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.UserCommentsToDownloadList == null || this.UserCommentsToDownloadList.Count == 0)
        //    {
        //        return;
        //    }
        //    Cursor = Cursors.Wait;
        //    bool allSucceeded = true;
        //    foreach (var userComments in this.UserCommentsToDownloadList)
        //    {
        //        var userCommentsComplete = HPTServiceConnector.DownloadCommentsByEMail(this.MarkBet, userComments.EMail);
        //        if (userCommentsComplete == null)
        //        {
        //            allSucceeded = false;
        //        }
        //    }
        //    if (allSucceeded)
        //    {
        //        var drSuccess = MessageBox.Show("Nedladdning av kommentarer slutförd utan problem.", "Nedladdning klar", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    else
        //    {
        //        var drError = MessageBox.Show("Nedladdning av minst en uppsättning kommentarer misslyckades, försök igen senare. Se fellogg för orsak.", "Nedladdning misslyckades", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //    Cursor = Cursors.Arrow;
        //}

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.UserCommentsToDownloadList == null)
            {
                this.UserCommentsToDownloadList = new List<HPTService.HPTUserComments>();
            }
            var chk = (CheckBox)sender;
            var userComments = (HPTService.HPTUserComments)chk.DataContext;
            if (chk.IsChecked == true)
            {
                this.UserCommentsToDownloadList.Add(userComments);
            }
            else
            {
                this.UserCommentsToDownloadList.Remove(userComments);
            }
        }

        //private void btnGetCommentsList_Click(object sender, RoutedEventArgs e)
        //{
        //    this.UserCommentsToDownloadList = new List<HPTService.HPTUserComments>();
        //    var userCommentsCollection = HPTServiceConnector.DownloadCommentsAll(this.MarkBet);
        //    this.DataContext = userCommentsCollection;
        //    BindingOperations.GetBindingExpression(this.lvwAvailableComments, ListView.ItemsSourceProperty).UpdateTarget();
        //}
    }
}

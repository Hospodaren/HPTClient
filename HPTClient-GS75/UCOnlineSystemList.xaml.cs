using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for UCOnlineSystemList.xaml
    /// </summary>
    public partial class UCOnlineSystemList : UserControl
    {
        public UCOnlineSystemList()
        {
            InitializeComponent();
        }
        
        private ATGCalendar atgCalendar;
        public UCOnlineSystemList(ATGCalendar atgCalendar)
        {
            this.atgCalendar = atgCalendar;
            InitializeComponent();
        }

        public List<HPTService.HPTUserSystem> UserSystemsToDownloadList { get; set; }

        private void btnDownloadSelectedSystems_Click(object sender, RoutedEventArgs e)
        {
            if (this.UserSystemsToDownloadList == null || this.UserSystemsToDownloadList.Count == 0)
            {
                return;
            }

            if (this.UserSystemsToDownloadList.Count > 3)
            {
                var drSuccess = MessageBox.Show("Du får ladda ner maximalt tre system åt gången.", "För många valda", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Cursor = Cursors.Wait;
            //bool allSucceeded = true;
            foreach (var userSystem in this.UserSystemsToDownloadList)
            {
                this.atgCalendar.DownloadOnlineSystem(userSystem);
            }
            Cursor = Cursors.Arrow;

            var ownerWindow = (Window)this.Parent;
            ownerWindow.Close();
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.UserSystemsToDownloadList == null)
            {
                this.UserSystemsToDownloadList = new List<HPTService.HPTUserSystem>();
            }
            var chk = (CheckBox)sender;
            var userSystem = (HPTService.HPTUserSystem)chk.DataContext;
            if (chk.IsChecked == true)
            {
                this.UserSystemsToDownloadList.Add(userSystem);
            }
            else
            {
                this.UserSystemsToDownloadList.Remove(userSystem);
            }
        }

    }
}

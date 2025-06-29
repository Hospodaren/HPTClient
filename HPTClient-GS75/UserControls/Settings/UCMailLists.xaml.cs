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
    /// Interaction logic for UCMailLists.xaml
    /// </summary>
    public partial class UCMailLists : UserControl
    {
        public UCMailLists()
        {
            this.SelectedMailList = new HPTMailList();
            this.SelectedMailList.RecipientList = new System.Collections.ObjectModel.ObservableCollection<HPTMailRecipient>();
            InitializeComponent();
        }
        
        private void btnNewMailList_Click(object sender, RoutedEventArgs e)
        {
            HPTMailList mailList = new HPTMailList();
            mailList.RecipientList = new System.Collections.ObjectModel.ObservableCollection<HPTMailRecipient>();
            HPTConfig.Config.MailListCollection.Add(mailList);
        }

        private void btnAddRecipient_Click(object sender, RoutedEventArgs e)
        {
            Control ctrl = (Control)sender;
            HPTMailList mailList = (HPTMailList)ctrl.DataContext;
            HPTMailRecipient recipient = new HPTMailRecipient();
            mailList.RecipientList.Add(recipient);
            mailList.Expanded = true;
            recipient.Selected = true;
            //this.tvwMailLists.Items.
            //try
            //{
            //    Expander exp = (Expander)ctrl.Tag;
            //    exp.IsExpanded = true;
            //}
            //catch (Exception exc)
            //{
            //    string s = exc.Message;
            //}
        }

        private void btnDeleteList_Click(object sender, RoutedEventArgs e)
        {
            Control ctrl = (Control)sender;
            HPTMailList mailList = (HPTMailList)ctrl.DataContext;
            HPTConfig.Config.MailListCollection.Remove(mailList);
        }

        private void btnDeleteRecipient_Click(object sender, RoutedEventArgs e)
        {
            Control ctrl = (Control)sender;
            HPTMailRecipient recipient = (HPTMailRecipient)ctrl.DataContext;
            foreach (HPTMailList mailList in HPTConfig.Config.MailListCollection)
            {
                if (mailList.RecipientList.Contains(recipient))
                {
                    mailList.RecipientList.Remove(recipient);
                }
            }
        }

        public HPTMailList SelectedMailList
        {
            get { return (HPTMailList)GetValue(SelectedMailListProperty); }
            set { SetValue(SelectedMailListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedMailList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedMailListProperty =
            DependencyProperty.Register("SelectedMailList", typeof(HPTMailList), typeof(UCMailLists), new UIPropertyMetadata(null));


        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Control ctrl = (Control)sender;
                HPTMailList mailList = (HPTMailList)ctrl.DataContext;
                this.SelectedMailList.RecipientList.Clear();
                foreach (HPTMailRecipient recipient in mailList.RecipientList)
                {
                    this.SelectedMailList.RecipientList.Add(recipient);
                }

            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }        
    }
}

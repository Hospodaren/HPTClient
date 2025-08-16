using System;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCHorseCommentList.xaml
    /// </summary>
    public partial class UCHorseCommentList : UserControl
    {
        public UCHorseCommentList()
        {
            InitializeComponent();
        }

        private void btnAddComment_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null || this.DataContext.GetType() != typeof(HPTHorse))
            {
                return;
            }
            AddComment();
        }

        public void AddComment()
        {
            var horse = (HPTHorse)this.DataContext;
            var horseComment = new HPTHorseOwnInformationComment()
            {
                CommentDate = DateTime.Now,
                CommentUser = HPTConfig.Config.UserNameForUploads,
                HasComment = true,
                IsOwnComment = true
            };
            if (horse.OwnInformation == null)
            {
                horse.OwnInformation = new HPTHorseOwnInformation()
                {
                    CreationDate = DateTime.Now
                };
            }
            if (horse.OwnInformation.HorseOwnInformationCommentList == null)
            {
                horse.OwnInformation.HorseOwnInformationCommentList = new System.Collections.ObjectModel.ObservableCollection<HPTHorseOwnInformationComment>();
            }
            horse.OwnInformation.Name = horse.HorseName;
            horse.OwnInformation.ATGId = horse.ATGId;
            horse.OwnInformation.StartNr = horse.StartNr;
            horse.OwnInformation.HomeTrack = horse.HomeTrack;
            horse.OwnInformation.Sex = horse.Sex;
            horse.OwnInformation.Age = horse.Age;
            horse.OwnInformation.Owner = horse.OwnerName;
            horse.OwnInformation.Trainer = horse.TrainerName;
            horse.OwnInformation.HasComment = true;
            horse.OwnInformation.HorseOwnInformationCommentList.Insert(0, horseComment);
            horse.OwnInformation.Comment = horseComment.Comment;
            horse.OwnInformation.Updated = true;
            //horse.OwnInformation.CreationDate = DateTime.Now;

            // Lägg till informationen i den gemensamma kommentarsfilen
            HPTConfig.Config.HorseOwnInformationCollection.MergeHorseOwnInformation(horse);
        }

        private void txtComment_Loaded_1(object sender, RoutedEventArgs e)
        {
            var txt = (TextBox)sender;
            txt.Focus();
        }

        private void btnRemoveComment_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var horse = (HPTHorse)this.DataContext;
                var fe = (FrameworkElement)e.OriginalSource;
                var commentToRemove = (HPTHorseOwnInformationComment)fe.DataContext;
                horse.OwnInformation.HorseOwnInformationCommentList.Remove(commentToRemove);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

        }
    }
}

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
    /// Interaction logic for UCHorseCommentView.xaml
    /// </summary>
    public partial class UCHorseCommentView : UCMarkBetControl
    {
        public UCHorseCommentView()
        {
            InitializeComponent();
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (this.ucHorseCommentList.DataContext == null || this.ucHorseCommentList.DataContext.GetType() != typeof(HPTHorse))
                {
                    e.Handled = true;
                    return;
                }
                var horse = (HPTHorse)this.ucHorseCommentList.DataContext;
                switch (e.Key)
                {
                    case Key.Left:
                        var previousHorse = horse;
                        if (horse.StartNr > 1)
                        {
                            previousHorse = horse.ParentRace.HorseList.First(h => h.StartNr == horse.StartNr - 1);
                        }
                        else if (horse.ParentRace.LegNr > 1)
                        {
                            previousHorse = horse.ParentRace.ParentRaceDayInfo.RaceList
                                .First(r => r.LegNr == horse.ParentRace.LegNr - 1)
                                .HorseList.Last();
                        }
                        FindAndSelectHorse(previousHorse);
                        break;
                    case Key.Right:
                        var nextHorse = horse;
                        if (horse.StartNr < horse.ParentRace.HorseList.Count)
                        {
                            nextHorse = horse.ParentRace.HorseList.First(h => h.StartNr == horse.StartNr + 1);
                        }
                        else if (horse.ParentRace.LegNr < horse.ParentRace.ParentRaceDayInfo.RaceList.Count)
                        {
                            nextHorse = horse.ParentRace.ParentRaceDayInfo.RaceList
                                .First(r => r.LegNr == horse.ParentRace.LegNr + 1)
                                .HorseList.First();
                        }
                        FindAndSelectHorse(nextHorse);
                        break;
                    case Key.Up:
                        var previousRace = horse.ParentRace;
                        if (previousRace.LegNr == 1)
                        {
                            return;
                        }
                        else
                        {
                            previousRace = previousRace.ParentRaceDayInfo.RaceList
                                .First(r => r.LegNr == previousRace.LegNr - 1);
                        }
                        FindAndSelectRace(previousRace);
                        break;
                    case Key.Down:
                        var nextRace = horse.ParentRace;
                        if (nextRace.LegNr == nextRace.ParentRaceDayInfo.RaceList.Count)
                        {
                            return;
                        }
                        else
                        {
                            nextRace = nextRace.ParentRaceDayInfo.RaceList
                                .First(r => r.LegNr == nextRace.LegNr + 1);
                        }
                        FindAndSelectRace(nextRace);
                        break;
                    case Key.N:
                        this.ucHorseCommentList.AddComment();
                        break;
                }
            }
            e.Handled = true;
        }

        private void FindAndSelectHorse(HPTHorse horse)
        {
            try
            {
                var itemParent = this.tvwHorseComments.ItemContainerGenerator.ContainerFromItem(horse.ParentRace) as TreeViewItem;
                itemParent.IsExpanded = true;

                var itemToSelect = itemParent.ItemContainerGenerator.ContainerFromItem(horse) as TreeViewItem;
                itemToSelect.IsSelected = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void FindAndSelectRace(HPTRace race)
        {
            try
            {
                var itemParent = this.tvwHorseComments.ItemContainerGenerator.ContainerFromItem(race) as TreeViewItem;
                itemParent.IsExpanded = true;
                //itemParent.IsSelected = true;

                //var itemToSelect = itemParent.ItemContainerGenerator.ContainerFromItem(race.HorseList.First()) as TreeViewItem;
                var itemToSelect = itemParent.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                itemToSelect.IsSelected = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        //private void btnDownloadComments_Click(object sender, RoutedEventArgs e)
        //{
        //    var userCommentsCollection = HPTServiceConnector.DownloadCommentsAll(this.MarkBet);
        //    var wndDownloadUserComments = new Window()
        //    {
        //        Content = new UCUserRaceDayInfoCommentsList()
        //        {
        //            DataContext = userCommentsCollection,
        //            MarkBet = this.MarkBet
        //        },
        //        Title = "Ladda ner kommentarer",
        //        SizeToContent = SizeToContent.WidthAndHeight,
        //        ShowInTaskbar = false,
        //        ResizeMode = ResizeMode.NoResize,
        //        Owner = App.Current.MainWindow 
        //    };

        //    wndDownloadUserComments.ShowDialog();
        //}

        //private void btnUploadComments_Click(object sender, RoutedEventArgs e)
        //{
        //    var wndUploadUserComments = new Window()
        //    {
        //        Content = new UCUserCommentsUpload()
        //        {
        //            DataContext = this.MarkBet,
        //            MarkBet = this.MarkBet
        //        },
        //        Title = "Ladda upp kommentarer",
        //        SizeToContent = SizeToContent.WidthAndHeight,
        //        ShowInTaskbar = false,
        //        ResizeMode = ResizeMode.NoResize,
        //        Owner = App.Current.MainWindow 
        //    };
        //    wndUploadUserComments.ShowDialog();
        //}

        private void ucHorseCommentList_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnCopyComments_Click(object sender, RoutedEventArgs e)
        {
            // Alla hästar med information
            var raceCommentList = this.MarkBet.RaceDayInfo.RaceList
                .Select(r => new HPTRaceComment()
                {
                    Comment = string.Empty,
                    LegNr = r.LegNr,
                    RaceNr = r.RaceNr,
                    HorseOwnInformationList = r.HorseList
                    .Where(h => h.OwnInformation.HasComment)
                    .Select(h => h.OwnInformation).ToList()
                }).ToList();
            
            // Sätt namn och nummer om det inte redan finns
            this.MarkBet.RaceDayInfo.RaceList
                .AsParallel()
                .ForAll(r =>
                {
                    foreach (var horse in r.HorseList.Where(h => h.OwnInformation != null))
                    {
                        horse.OwnInformation.StartNr = horse.StartNr;
                        horse.OwnInformation.Name = horse.HorseName;
                        horse.OwnInformation.ATGId = horse.ATGId;
                        horse.OwnInformation.HomeTrack = horse.HomeTrack;
                        horse.OwnInformation.Sex = horse.Sex;
                        horse.OwnInformation.Age = horse.Age;
                        horse.OwnInformation.Owner = horse.OwnerName;
                        horse.OwnInformation.Trainer = horse.TrainerName;
                    }
                });

            var sb = new StringBuilder();
            raceCommentList.ForEach(rc =>
                {
                    sb.Append(this.MarkBet.BetType.Code);
                    sb.Append("-");
                    sb.Append(rc.LegNr);
                    sb.AppendLine();
                    sb.AppendLine();
                    rc.HorseOwnInformationList.ForEach(hoi =>
                        {
                            sb.Append(hoi.StartNr);
                            sb.Append(" - ");
                            sb.AppendLine(hoi.Name);
                            hoi.HorseOwnInformationCommentList
                                .ToList()
                                .ForEach(c => sb.AppendLine(c.ToString()));
                        });
                });

            Clipboard.SetDataObject(sb.ToString());
        }

        //static void ShowOnlyWithCommentsChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        //{
        //    var uc = (UCHorseCommentView)property;
        //    uc.lstHorseCommentList.Items.Filter = new Predicate<object>(uc.FilterWithComments);
        //}

        //public bool ShowOnlyWithComments
        //{
        //    get { return (bool)GetValue(ShowOnlyWithCommentsProperty); }
        //    set { SetValue(ShowOnlyWithCommentsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ShowOnlyWithComments.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ShowOnlyWithCommentsProperty =
        //    DependencyProperty.Register("ShowOnlyWithComments", typeof(bool), typeof(UCHorseCommentView), new PropertyMetadata(true, new PropertyChangedCallback(ShowOnlyWithCommentsChangedCallBack)));

        //public void FilterWithComments()
        //{
        //    this.lstHorseCommentList.Items.Filter = new Predicate<object>(FilterWithComments);
        //}

        //public bool FilterWithComments(object obj)
        //{
        //    var horse = obj as HPTHorse;
        //    if (this.ShowOnlyWithComments)
        //    {
        //        return horse.OwnInformation.HasComment == this.ShowOnlyWithComments;
        //    }
        //    return true;
        //}
    }
}

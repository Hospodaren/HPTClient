using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCCompactHorseWide.xaml
    /// </summary>
    public partial class UCCompactHorseWide : UserControl
    {
        public UCCompactHorseWide()
        {
            InitializeComponent();
        }

        private System.Windows.Controls.Primitives.Popup pu;
        private void txtHorseName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            TextBlock tb = (TextBlock)sender;
            if (this.pu == null)
            {
                this.pu = new System.Windows.Controls.Primitives.Popup()
                {
                    Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                    HorizontalOffset = -10D,
                    VerticalOffset = -10D
                };
                this.pu.MouseLeave += new MouseEventHandler(pu_MouseLeave);
            }
            this.pu.DataContext = tb.DataContext;
            this.pu.Child = new UCResultView();
            this.pu.IsOpen = true;
        }

        void pu_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Controls.Primitives.Popup pu = (System.Windows.Controls.Primitives.Popup)sender;
            pu.Child = null;
            pu.IsOpen = false;
        }

        private void chkNextTimer_Checked(object sender, RoutedEventArgs e)
        {
            var chk = (CheckBox)sender;
            var horse = (HPTHorse)this.DataContext;
            if (horse.OwnInformation == null)
            {
                horse.OwnInformation = new HPTHorseOwnInformation()
                {
                    Name = horse.HorseName,
                    ATGId = horse.ATGId,
                    HorseOwnInformationCommentList = new System.Collections.ObjectModel.ObservableCollection<HPTHorseOwnInformationComment>(),
                    StartNr = horse.StartNr,
                    CreationDate = DateTime.Now,
                    NextTimer = chk.IsChecked
                };
                HPTConfig.Config.HorseOwnInformationCollection.MergeHorseOwnInformation(horse);
            }
            else
            {
                horse.OwnInformation.NextTimer = chk.IsChecked;
            }
            horse.OwnInformation.Updated = true;
        }
    }
}

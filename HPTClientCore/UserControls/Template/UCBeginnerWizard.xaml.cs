using System.Windows;
using System.Windows.Input;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for WBeginnerWizard.xaml
    /// </summary>
    public partial class UCBeginnerWizard : UCMarkBetControl
    {
        public UCBeginnerWizard()
        {
            InitializeComponent();
        }

        private Window ownerWindow;
        public UCBeginnerWizard(Window owner)
        {
            ownerWindow = owner;
            InitializeComponent();
        }

        private void btnCreateSystem_Click(object sender, RoutedEventArgs e)
        {
            if (MarkBet.RaceDayInfo.HorseListSelected.Count > 0)
            {
                var result = MessageBox.Show("Du har redan valt hästar, vill du behålla dessa på ditt system?", "Behåll valda hästar?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var horse in MarkBet.RaceDayInfo.HorseListSelected)
                    {
                        horse.Locked = true;
                    }
                }
                else
                {
                    MarkBet.ClearAll();
                }
            }
            Cursor = Cursors.Wait;
            try
            {
                btnSaveAndClose.Visibility = Visibility.Visible;

                // Skapa systemförslag utifrån valen...
                MarkBet.SelectFromBeginnerTemplate();
                btnSaveAndClose.IsEnabled = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            Cursor = Cursors.Arrow;

            //if (this.MarkBet.SystemCost == 0M)
            //{
            //    MessageBox.Show("Det gick inte att hitta några bra system med de inställningar du gjort. Prova att ändra kostnaden eller andra inställningar och försök igen.", "Inget system skapat", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }

        private void btnSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            MarkBet.SaveFiles();
            if (ownerWindow != null)
            {
                ownerWindow.Close();
            }
            else
            {
                var parentWindow = (Window)Parent;
                parentWindow.Close();
            }
        }

        private void btnClearSystem_Click(object sender, RoutedEventArgs e)
        {
            // Välj ut de vanligaste rankvariablerna
            MarkBet.TemplateForBeginners.HorseRankVariableList = new List<HPTHorseRankVariable>(MarkBet.HorseRankVariableList.Where(rv => rv.HorseRankInfo.UseForBeginner));
            MarkBet.TemplateForBeginners.NumberOfSpikes = 1;
            MarkBet.TemplateForBeginners.ReductionRisk = HPTReductionRisk.Medium;
            MarkBet.TemplateForBeginners.Stake = 300;
        }
    }
}

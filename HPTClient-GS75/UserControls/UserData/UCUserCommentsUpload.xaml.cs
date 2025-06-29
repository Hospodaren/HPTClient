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
    /// Interaction logic for UCUserCommentsUpload.xaml
    /// </summary>
    public partial class UCUserCommentsUpload : UCMarkBetControl
    {
        public UCUserCommentsUpload()
        {
            InitializeComponent();
        }

        private void btnUploadComments_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            if (this.txtCommentDescription.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Du måste fylla i en beskrivning av dina kommentarer", "Beskrivning saknas", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var dr = MessageBox.Show("Ladda upp alla kommentarer till HPTs server?", "Ladda upp kommentarer", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.Yes)
                {
                    try
                    {
                        HPTServiceConnector.UploadComments(this.MarkBet);
                        var drSuccess = MessageBox.Show("Upladdning till HPTs server slutförd utan problem.", "Uppladdning klar", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception exc)
                    {
                        var drError = MessageBox.Show("Uppladdning misslyckades, försök igen senare. Se fellogg för orsak.", "Uppladdning misslyckades", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            Cursor = Cursors.Arrow;
        }
    }
}

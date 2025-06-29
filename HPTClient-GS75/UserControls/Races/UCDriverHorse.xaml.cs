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
    /// Interaction logic for UCDriverHorse.xaml
    /// </summary>
    public partial class UCDriverHorse : UserControl
    {
        public UCDriverHorse()
        {
            InitializeComponent();
        }

        private void lvwLopp_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.lvwLopp.SelectedItem != null)
            {
                HPTHorse horse = (HPTHorse)this.lvwLopp.SelectedItem;
                switch (e.Key)
                {
                    //case Key.A:
                    //    horse.RankedA = true;
                    //    break;
                    //case Key.B:
                    //    horse.RankedB = true;
                    //    break;
                    //case Key.C:
                    //    horse.RankedC = true;
                    //    break;
                    //case Key.D:
                    //    horse.RankedD = true;
                    //    break;
                    case Key.D0:
                        break;
                    case Key.D1:
                        break;
                    case Key.D2:
                        break;
                    case Key.D3:
                        break;
                    case Key.D4:
                        break;
                    case Key.D5:
                        break;
                    case Key.D6:
                        break;
                    case Key.D7:
                        break;
                    case Key.D8:
                        break;
                    case Key.D9:
                        break;
                    case Key.R:
                        // Reserv
                        break;
                    case Key.Space:
                        horse.Selected = !horse.Selected;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

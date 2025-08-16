using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCCompactHorse.xaml
    /// </summary>
    public partial class UCCompactHorseBig : UserControl
    {
        public UCCompactHorseBig()
        {
            InitializeComponent();
        }

        private void spABCD_Checked(object sender, RoutedEventArgs e)
        {
            //if (e.OriginalSource.GetType() == typeof(RadioButton))
            //{
            //    RadioButton rb = (RadioButton)e.OriginalSource;
            //    object o = rb.BindingGroup;
            //    o = rb.InputBindings;
            //    HPTHorse horse = (HPTHorse)rb.DataContext;
            //    switch ((string)rb.Tag)
            //    {
            //        case "A":
            //            if (!horse.RankedA)
            //            {
            //                horse.RankedA = true;
            //            }
            //            break;
            //        case "B":
            //            if (!horse.RankedB)
            //            {
            //                horse.RankedB = true;
            //            }
            //            break;
            //        case "C":
            //            if (!horse.RankedC)
            //            {
            //                horse.RankedC = true;
            //            }
            //            break;
            //        case "D":
            //            if (!horse.RankedD)
            //            {
            //                horse.RankedD = true;
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private void spABCD_Unchecked(object sender, RoutedEventArgs e)
        {
            //    if (e.OriginalSource.GetType() == typeof(RadioButton))
            //    {
            //        RadioButton rb = (RadioButton)e.OriginalSource;
            //        object o = rb.BindingGroup;
            //        o = rb.InputBindings;
            //        HPTHorse horse = (HPTHorse)rb.DataContext;
            //        switch ((string)rb.Tag)
            //        {
            //            case "A":
            //                if (horse.RankedA)
            //                {
            //                    horse.RankedA = false;
            //                }
            //                break;
            //            case "B":
            //                if (horse.RankedB)
            //                {
            //                    horse.RankedB = false;
            //                }
            //                break;
            //            case "C":
            //                if (horse.RankedC)
            //                {
            //                    horse.RankedC = false;
            //                }
            //                break;
            //            case "D":
            //                if (horse.RankedD)
            //                {
            //                    horse.RankedD = false;
            //                }
            //                break;
            //            default:
            //                break;
            //        }
            //    }
        }
    }
}

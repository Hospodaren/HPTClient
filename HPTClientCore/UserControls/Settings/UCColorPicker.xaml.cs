using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCColorPicker.xaml
    /// </summary>
    public partial class UCColorPicker : UserControl
    {
        public UCColorPicker()
        {
            InitializeComponent();
        }

        #region Color changed event handling

        private void rctColorGood_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            switch (cpGood.Visibility)
            {
                case Visibility.Collapsed:
                case Visibility.Hidden:
                    cpGood.Visibility = Visibility.Visible;
                    cpMedium.Visibility = Visibility.Collapsed;
                    cpBad.Visibility = Visibility.Collapsed;
                    break;
                case Visibility.Visible:
                    cpGood.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private void rctColorMedium_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            switch (cpMedium.Visibility)
            {
                case Visibility.Collapsed:
                case Visibility.Hidden:
                    cpGood.Visibility = Visibility.Collapsed;
                    cpMedium.Visibility = Visibility.Visible;
                    cpBad.Visibility = Visibility.Collapsed;
                    break;
                case Visibility.Visible:
                    cpMedium.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private void rctColorBad_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            switch (cpBad.Visibility)
            {
                case Visibility.Collapsed:
                case Visibility.Hidden:
                    cpGood.Visibility = Visibility.Collapsed;
                    cpMedium.Visibility = Visibility.Collapsed;
                    cpBad.Visibility = Visibility.Visible;
                    break;
                case Visibility.Visible:
                    cpBad.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private void cpGood_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            HPTConfig.Config.ColorIntervalVinnarOdds.LowColor = (Color)e.NewValue;
            HPTConfig.Config.ColorGood = (Color)e.NewValue;
            cpGood.Visibility = Visibility.Collapsed;
        }

        private void cpBad_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            HPTConfig.Config.ColorIntervalVinnarOdds.HighColor = (Color)e.NewValue;
            HPTConfig.Config.ColorBad = (Color)e.NewValue;
            cpBad.Visibility = Visibility.Collapsed;
        }

        private void cpMedium_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            HPTConfig.Config.ColorIntervalVinnarOdds.MediumColor = (Color)e.NewValue;
            HPTConfig.Config.ColorMedium = (Color)e.NewValue;
            cpMedium.Visibility = Visibility.Collapsed;
        }

        #endregion

        private void btnResetColors_Click(object sender, RoutedEventArgs e)
        {
            HPTConfig.Config.ColorGood = Colors.LightGreen;
            HPTConfig.Config.ColorMedium = Colors.LightYellow;
            HPTConfig.Config.ColorBad = Colors.LightCoral;
        }

        private void btnResetIntervals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HPTConfig.Config.ColorIntervalVinnarOdds.LowerBoundary = 50M;
                HPTConfig.Config.ColorIntervalVinnarOdds.UpperBoundary = 100M;

                //HPTConfig.Config.ColorIntervalMarksPercent.LowerBoundary = 10M;
                //HPTConfig.Config.ColorIntervalMarksPercent.UpperBoundary = 50M;

                HPTConfig.Config.ColorIntervalStakePercent.LowerBoundary = 10M;
                HPTConfig.Config.ColorIntervalStakePercent.UpperBoundary = 30M;

                //HPTConfig.Config.ColorIntervalMarkability.LowerBoundary = 0.9M;
                //HPTConfig.Config.ColorIntervalMarkability.UpperBoundary = 1.2M;
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }
    }
}

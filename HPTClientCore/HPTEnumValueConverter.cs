using System.Windows.Data;

namespace HPTClient
{
    public class ReductionRiskToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32((HPTReductionRisk)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (HPTReductionRisk)((int)value);
        }
    }

    public class DesiredProfitToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32((HPTDesiredProfit)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (HPTDesiredProfit)((int)value);
        }
    }
}

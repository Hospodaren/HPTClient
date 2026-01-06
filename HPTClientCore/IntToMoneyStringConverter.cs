using System.Windows.Data;

namespace HPTClient
{
    public class IntToMoneyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int intValue = System.Convert.ToInt32(value);
            if (culture == null)
            {
                culture = new System.Globalization.CultureInfo("sv-SE");
            }


            if (intValue < 10000)
            {
                return intValue.ToString("N0", culture) + " kr";
            }

            if (intValue < 100000)
            {
                decimal dividedAndRounded = intValue / 1000M;
                dividedAndRounded = Math.Round(dividedAndRounded, 1);
                return dividedAndRounded.ToString(culture) + " K";
            }

            if (intValue < 1000000)
            {
                decimal dividedAndRounded = intValue / 1000M;
                dividedAndRounded = Math.Round(dividedAndRounded, 0);
                return dividedAndRounded.ToString(culture) + " K";
            }

            if (intValue < 10000000)
            {
                decimal dividedAndRounded = intValue / 1000000M;
                dividedAndRounded = Math.Round(dividedAndRounded, 1);
                return dividedAndRounded.ToString(culture) + " M";
            }

            //if (intValue < 100000000)
            //{
            //    decimal dividedAndRounded = intValue / 1000000M;
            //    dividedAndRounded = Math.Round(dividedAndRounded, 0);
            //    return dividedAndRounded.ToString(culture) + " M";
            //}

            if (intValue >= 10000000)
            {
                decimal dividedAndRounded = intValue / 1000000M;
                dividedAndRounded = Math.Round(dividedAndRounded, 0);
                return dividedAndRounded.ToString(culture) + " M";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
            //return (HPTReductionRisk)((int)value);
        }
    }
}

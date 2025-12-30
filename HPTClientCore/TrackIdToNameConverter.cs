using System.Globalization;
using System.Windows.Data;

namespace HPTClient
{
    public class TrackIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int trackId = (int)value;
                return EnumHelper.GetTrackNameFromTrackId(trackId);

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}

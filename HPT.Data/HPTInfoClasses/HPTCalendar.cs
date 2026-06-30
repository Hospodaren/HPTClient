using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCalendar
    {
        public HPTCalendar()
        {
            RaceDayInfoList = new ObservableCollection<HPTRaceDayInfo>();
        }

        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public DateTime ToDate { get; set; }

        [DataMember]
        public ObservableCollection<HPTRaceDayInfo> RaceDayInfoList { get; set; }

        public override string ToString()
        {
            return $"{FromDate:yyyy-MM-dd} - {ToDate:yyyy-MM-dd}";
        }
    }
}

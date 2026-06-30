using System.Runtime.Serialization;
using System.Text;
// using System.Windows.Controls;
// using System.Windows.Media;
// using System.Windows.Media.Imaging;
using System.Xml.Serialization;


namespace HPTClient
{
    [DataContract]
    public class HPTRaceDayInfoHistory
    {
        [DataMember]
        public string BetTypeCode { get; set; }

        [DataMember]
        public int TrackId { get; set; }

        [DataMember]
        public string TrackName { get; set; }

        [DataMember]
        public DateTime RaceDayDate { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public decimal Turnover { get; set; }

        [DataMember]
        public IEnumerable<HPTRaceHistory> RaceList  { get; set; }

    }

    public class HPTRaceHistory
    {
        [DataMember]
        public int RaceNumber { get; set; }

        [DataMember]
        public int LegNumber { get; set; }

        [DataMember]
        public IEnumerable<HPTHorseHistory> HorseList { get; set; }
    }

    public class HPTHorseHistory
    {
        [DataMember]
        public int StartNumber { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public decimal StakeShare { get; set; }

        [DataMember]
        public decimal VinnarOdds { get; set; }

        [DataMember]
        public decimal PlatsOdds { get; set; }

        [DataMember]
        public decimal ATGTrend { get; set; }
    }
}

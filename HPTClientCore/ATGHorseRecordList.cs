using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class KmTime
    {
        [DataMember]
        public int minutes { get; set; }

        [DataMember]
        public int seconds { get; set; }

        [DataMember]
        public int tenths { get; set; }

        [DataMember]
        public string code { get; set; }
    }

    [DataContract]
    public class Race
    {
        [DataMember]
        public string sport { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public int number { get; set; }

        [DataMember]
        public string startMethod { get; set; }

        [DataMember]
        public int firstPrize { get; set; }
    }

    [DataContract]
    public class Track
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string condition { get; set; }
    }

    [DataContract]
    public class Driver
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string lastName { get; set; }

        [DataMember]
        public string shortName { get; set; }
    }

    [DataContract]
    public class Shoes
    {
        [DataMember]
        public bool front { get; set; }

        [DataMember]
        public bool back { get; set; }
    }

    [DataContract]
    public class Horse
    {
        public Shoes shoes { get; set; }
    }

    [DataContract]
    public class Start
    {
        [DataMember]
        public int distance { get; set; }

        [DataMember]
        public int postPosition { get; set; }

        [DataMember]
        public Driver driver { get; set; }

        [DataMember]
        public Horse horse { get; set; }
    }

    [DataContract]
    public class Record
    {
        [DataMember]
        public string date { get; set; }

        [DataMember]
        public bool link { get; set; }

        [DataMember]
        public KmTime kmTime { get; set; }

        [DataMember]
        public string odds { get; set; }

        [DataMember]
        public string oddsCode { get; set; }

        [DataMember]
        public string place { get; set; }

        [DataMember]
        public string mediaId { get; set; }

        [DataMember]
        public Race race { get; set; }

        [DataMember]
        public Track track { get; set; }

        [DataMember]
        public Start start { get; set; }

        [DataMember]
        public bool? scratched { get; set; }

        [DataMember]
        public bool? galloped { get; set; }

        [DataMember]
        public bool? disqualified { get; set; }
    }

    [DataContract]
    public class ATGHorseRecordList
    {
        [DataMember]
        public List<Record> records { get; set; }

        [DataMember]
        public bool hasMoreRecords { get; set; }
    }

}
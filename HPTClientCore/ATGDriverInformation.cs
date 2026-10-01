using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HomeTrack
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }
    }

    //[DataContract]
    //public class Placement
    //{
    //    [DataMember]
    //    public int __invalid_name__3 { get; set; }

    //    [DataMember]
    //    public int __invalid_name__2 { get; set; }

    //    [DataMember]
    //    public int __invalid_name__1 { get; set; }
    //}

    [DataContract]
    public class YearStatistic
    {
        [DataMember]
        public int starts { get; set; }

        [DataMember]
        public long earnings { get; set; }

        //[DataMember]
        //public Placement placement { get; set; }

        [DataMember]
        public Dictionary<int, int> placement { get; set; }

        [DataMember]
        public decimal winPercentage { get; set; }
    }

    [DataContract]
    public class Placement2
    {
        [DataMember]
        public int __invalid_name__3 { get; set; }

        [DataMember]
        public int __invalid_name__2 { get; set; }

        [DataMember]
        public int __invalid_name__1 { get; set; }
    }

    [DataContract]
    public class __invalid_type__2014
    {
        [DataMember]
        public int starts { get; set; }

        [DataMember]
        public long earnings { get; set; }

        [DataMember]
        public Placement2 placement { get; set; }

        [DataMember]
        public int winPercentage { get; set; }
    }

    //[DataContract]
    //public class Years
    //{
    //    [DataMember]
    //    public __invalid_type__2015 __invalid_name__2015 { get; set; }

    //    [DataMember]
    //    public __invalid_type__2014 __invalid_name__2014 { get; set; }
    //}

    [DataContract]
    public class Statistics
    {
        [DataMember]
        public Dictionary<string, YearStatistic> years { get; set; }
    }

    [DataContract]
    public class ATGDriverInformation
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string lastName { get; set; }

        [DataMember]
        public string shortName { get; set; }

        [DataMember]
        public string location { get; set; }

        [DataMember]
        public int birth { get; set; }

        [DataMember]
        public HomeTrack homeTrack { get; set; }

        [DataMember]
        public string license { get; set; }

        [DataMember]
        public string silks { get; set; }

        [DataMember]
        public Statistics statistics { get; set; }
    }

    [DataContract]
    public class ATGTrainerInformation
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string lastName { get; set; }

        [DataMember]
        public string shortName { get; set; }

        [DataMember]
        public string location { get; set; }

        [DataMember]
        public int birth { get; set; }

        [DataMember]
        public HomeTrack homeTrack { get; set; }

        [DataMember]
        public string license { get; set; }

        [DataMember]
        public string silks { get; set; }

        [DataMember]
        public Statistics statistics { get; set; }
    }
}

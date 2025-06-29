using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class ATGHorseRaceInformation
    {
        [DataMember]
        public ATGDriverInformation driver { get; set; }

        [DataMember]
        public ATGHorseStartInformation horse { get; set; }

        [DataMember]
        public string raceId { get; set; }

        [DataMember]
        public int startNumber { get; set; }

        [DataMember]
        public string sport { get; set; }
    }

    [DataContract]
    public class ATGHorseStartInformation
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public int age { get; set; }

        [DataMember]
        public string sex { get; set; }

        [DataMember]
        public ATGTrainerInformation trainer { get; set; }

        [DataMember]
        public int money { get; set; }

        [DataMember]
        public string color { get; set; }
        //public homeTrack { get; set; }

        [DataMember]
        public Owner owner { get; set; }

        [DataMember]
        public Breeder breeder { get; set; }
        //public statistics { get; set; }

        [DataMember]
        public Pedigree pedigree { get; set; }

        [DataMember]
        public ATGHorseRecordList results { get; set; }
    }

//        public class HomeTrack
//{
//    public int id { get; set; }
//    public string name { get; set; }
//}

//public class Placement
//{
//    public int 3 { get; set; }
//public int 2 { get; set; }
//        public int 1 { get; set; }
//    }

//    public class 2016
//    {
//        public int starts { get; set; }
//public int earnings { get; set; }
//public Placement placement { get; set; }
//public int winPercentage { get; set; }
//    }

//    public class 2015
//    {
//        public int starts { get; set; }
//public int earnings { get; set; }
//public placement { get; set; }
//        public int winPercentage { get; set; }
//    }

    public class Years
{
//    public 2016 2016 { get; set; }
//public 2015 2015 { get; set; }
    }

//    public class Statistics
//{
//    public Years years { get; set; }
//}

public class Trainer
{
    public int id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string shortName { get; set; }
    public string location { get; set; }
    public int birth { get; set; }
    public HomeTrack homeTrack { get; set; }
    public string license { get; set; }
    public string silks { get; set; }
    public Statistics statistics { get; set; }
}

public class Owner
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Breeder
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Father
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Mother
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Grandfather
{
    public int id { get; set; }
    public string name { get; set; }
    public string nationality { get; set; }
}

public class Pedigree
{
    public Father father { get; set; }
    public Mother mother { get; set; }
    public Grandfather grandfather { get; set; }
}

    //public class KmTime
    //{
    //    public string code { get; set; }
    //    public int? minutes { get; set; }
    //    public int? seconds { get; set; }
    //    public int? tenths { get; set; }
    //}

    //public class Race
    //{
    //    public string id { get; set; }
    //    public string sport { get; set; }
    //    public string type { get; set; }
    //    public int number { get; set; }
    //    public string startMethod { get; set; }
    //    public int firstPrize { get; set; }
    //}

    //public class Track
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public string condition { get; set; }
    //    public string countryCode { get; set; }
    //}

    //public class Driver
    //{
    //    public int id { get; set; }
    //    public string firstName { get; set; }
    //    public string lastName { get; set; }
    //    public string shortName { get; set; }
    //    public string level { get; set; }
    //}

    //public class Shoes
    //{
    //    public bool front { get; set; }
    //    public bool back { get; set; }
    //}

    //public class Horse
    //{
    //    public Shoes shoes { get; set; }
    //}

    //public class ATGStartInformation
    //{
    //    public int distance { get; set; }
    //    public int postPosition { get; set; }
    //    public Driver driver { get; set; }
    //    public Horse horse { get; set; }
    //}

    //public class Record
    //{
    //    public string date { get; set; }
    //    public bool link { get; set; }
    //    public KmTime kmTime { get; set; }
    //    public int odds { get; set; }
    //    public bool disqualified { get; set; }
    //    public bool galloped { get; set; }
    //    public string mediaId { get; set; }
    //    public Race race { get; set; }
    //    public Track track { get; set; }
    //    public Start start { get; set; }
    //    public string place { get; set; }
    //    public bool? scratched { get; set; }
    //    public string oddsCode { get; set; }
    //}

//    public class Results
//{
//    public IList<Record> records { get; set; }
//    public bool hasMoreRecords { get; set; }
//}

//public class Example
//{
//    public int id { get; set; }
//    public string name { get; set; }
//    public int age { get; set; }
//    public string sex { get; set; }
//    public Trainer trainer { get; set; }
//    public int money { get; set; }
//    public string color { get; set; }
//    //public homeTrack { get; set; }
//public Owner owner { get; set; }
//public Breeder breeder { get; set; }
////public statistics { get; set; }
//        public Pedigree pedigree { get; set; }
////public Results results { get; set; }
//    }

}

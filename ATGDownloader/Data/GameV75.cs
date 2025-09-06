using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader.V75
{
    // https://www.atg.se/services/racinginfo/v1/api/games/V75_2025-05-31_22_5
    public class GameV75
    {
        public string type { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public Pools pools { get; set; }
        public Race[] races { get; set; }
        public long version { get; set; }
        public bool newBettingSystem { get; set; }
        //public string type { get; set; }
    }

    public class Pools
    {
        public V75 V75 { get; set; }
    }

    public class V75
    {
        public string type { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string timestamp { get; set; }
        public long turnover { get; set; }
        public string[] addOns { get; set; }
        public string betType { get; set; }
        public bool harry { get; set; }
        public int systemCount { get; set; }
        public Payouts payouts { get; set; }
    }

    public class Payouts
    {
        public int _5 { get; set; }
        public int _6 { get; set; }
        public int _7 { get; set; }
    }

    public class Race
    {
        public string id { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public int number { get; set; }
        public int distance { get; set; }
        public string startMethod { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public string prize { get; set; }
        public string[] terms { get; set; }
        public string sport { get; set; }
        public Track track { get; set; }
        public string status { get; set; }
        public Pools1 pools { get; set; }
        public Start[] starts { get; set; }
        public Mergedpool[] mergedPools { get; set; }
    }

    public class Track
    {
        public int id { get; set; }
        public string name { get; set; }
        public string condition { get; set; }
        public string countryCode { get; set; }
    }

    public class Pools1
    {
        public Vinnare vinnare { get; set; }
        public Plats plats { get; set; }
    }

    public class Vinnare
    {
        public string type { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string timestamp { get; set; }
        public int turnover { get; set; }
        public string betType { get; set; }
    }

    public class Plats
    {
        public string type { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string timestamp { get; set; }
        public int turnover { get; set; }
        public string betType { get; set; }
    }

    public class Start
    {
        public string id { get; set; }
        public int number { get; set; }
        public int postPosition { get; set; }
        public int distance { get; set; }
        public Horse horse { get; set; }
        public Driver driver { get; set; }
        public Pools2 pools { get; set; }
        public Originaldriver originalDriver { get; set; }
        public bool scratched { get; set; }
    }

    public class Horse
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string sex { get; set; }
        public Record record { get; set; }
        public Trainer trainer { get; set; }
        public Shoes shoes { get; set; }
        public Sulky sulky { get; set; }
        public int money { get; set; }
        public string color { get; set; }
        public Hometrack1 homeTrack { get; set; }
        public Owner owner { get; set; }
        public Breeder breeder { get; set; }
        public Statistics1 statistics { get; set; }
        public Pedigree pedigree { get; set; }
        public string nationality { get; set; }
        public bool foreignOwned { get; set; }
    }

    public class Record
    {
        public string code { get; set; }
        public string startMethod { get; set; }
        public string distance { get; set; }
        public Time time { get; set; }
    }

    public class Time
    {
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int tenths { get; set; }
    }

    public class Trainer
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string shortName { get; set; }
        public string location { get; set; }
        public int birth { get; set; }
        public Hometrack homeTrack { get; set; }
        public string license { get; set; }
        public string silks { get; set; }
        public Statistics statistics { get; set; }
    }

    public class Hometrack
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Statistics
    {
        public Years years { get; set; }
    }

    public class Years
    {
        public _2024 _2024 { get; set; }
        public _2025 _2025 { get; set; }
    }

    public class _2024
    {
        public int starts { get; set; }
        public long earnings { get; set; }
        public Placement placement { get; set; }
        public int winPercentage { get; set; }
    }

    public class Placement
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class _2025
    {
        public int starts { get; set; }
        public int earnings { get; set; }
        public Placement1 placement { get; set; }
        public int winPercentage { get; set; }
    }

    public class Placement1
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class Shoes
    {
        public bool reported { get; set; }
        public Front front { get; set; }
        public Back back { get; set; }
    }

    public class Front
    {
        public bool hasShoe { get; set; }
        public bool changed { get; set; }
    }

    public class Back
    {
        public bool hasShoe { get; set; }
        public bool changed { get; set; }
    }

    public class Sulky
    {
        public bool reported { get; set; }
        public Type type { get; set; }
        public Colour colour { get; set; }
    }

    public class Type
    {
        public string code { get; set; }
        public string text { get; set; }
        public string engText { get; set; }
        public bool changed { get; set; }
    }

    public class Colour
    {
        public string code { get; set; }
        public string text { get; set; }
        public string engText { get; set; }
        public bool changed { get; set; }
    }

    public class Hometrack1
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Owner
    {
        public int id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
    }

    public class Breeder
    {
        public int id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
    }

    public class Statistics1
    {
        public Years1 years { get; set; }
        public Life life { get; set; }
        public Lastfivestarts lastFiveStarts { get; set; }
    }

    public class Years1
    {
        public _20241 _2024 { get; set; }
        public _20251 _2025 { get; set; }
    }

    public class _20241
    {
        public int starts { get; set; }
        public int earnings { get; set; }
        public Placement2 placement { get; set; }
        public Record1[] records { get; set; }
    }

    public class Placement2
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class Record1
    {
        public string code { get; set; }
        public string startMethod { get; set; }
        public string distance { get; set; }
        public Time1 time { get; set; }
        public int place { get; set; }
    }

    public class Time1
    {
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int tenths { get; set; }
    }

    public class _20251
    {
        public int starts { get; set; }
        public int earnings { get; set; }
        public Placement3 placement { get; set; }
        public Record2[] records { get; set; }
    }

    public class Placement3
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class Record2
    {
        public string code { get; set; }
        public string startMethod { get; set; }
        public string distance { get; set; }
        public Time2 time { get; set; }
        public int place { get; set; }
    }

    public class Time2
    {
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int tenths { get; set; }
    }

    public class Life
    {
        public int starts { get; set; }
        public int earnings { get; set; }
        public Placement4 placement { get; set; }
        public Record3[] records { get; set; }
        public int winPercentage { get; set; }
        public int placePercentage { get; set; }
        public int earningsPerStart { get; set; }
        public int startPoints { get; set; }
    }

    public class Placement4
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class Record3
    {
        public string code { get; set; }
        public string startMethod { get; set; }
        public string distance { get; set; }
        public Time3 time { get; set; }
        public int place { get; set; }
        public string year { get; set; }
    }

    public class Time3
    {
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int tenths { get; set; }
    }

    public class Lastfivestarts
    {
        public int averageOdds { get; set; }
    }

    public class Pedigree
    {
        public Father father { get; set; }
        public Mother mother { get; set; }
        public Grandfather grandfather { get; set; }
    }

    public class Father
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nationality { get; set; }
    }

    public class Mother
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nationality { get; set; }
    }

    public class Grandfather
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nationality { get; set; }
    }

    public class Driver
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string shortName { get; set; }
        public string location { get; set; }
        public int birth { get; set; }
        public Hometrack2 homeTrack { get; set; }
        public string license { get; set; }
        public string silks { get; set; }
        public Statistics2 statistics { get; set; }
    }

    public class Hometrack2
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Statistics2
    {
        public Years2 years { get; set; }
    }

    public class Years2
    {
        public _20242 _2024 { get; set; }
        public _20252 _2025 { get; set; }
    }

    public class _20242
    {
        public int starts { get; set; }
        public long earnings { get; set; }
        public Placement5 placement { get; set; }
        public int winPercentage { get; set; }
    }

    public class Placement5
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class _20252
    {
        public int starts { get; set; }
        public int earnings { get; set; }
        public Placement6 placement { get; set; }
        public int winPercentage { get; set; }
    }

    public class Placement6
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class Pools2
    {
        public Vinnare1 vinnare { get; set; }
        public Plats1 plats { get; set; }
        public V751 V75 { get; set; }
    }

    public class Vinnare1
    {
        public string type { get; set; }
        public int odds { get; set; }
    }

    public class Plats1
    {
        public string type { get; set; }
        public int minOdds { get; set; }
        public int maxOdds { get; set; }
    }

    public class V751
    {
        public string type { get; set; }
        public int betDistribution { get; set; }
        public float trend { get; set; }
    }

    public class Originaldriver
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string shortName { get; set; }
        public string location { get; set; }
        public int birth { get; set; }
        public Hometrack3 homeTrack { get; set; }
        public string license { get; set; }
        public string silks { get; set; }
        public Statistics3 statistics { get; set; }
    }

    public class Hometrack3
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Statistics3
    {
        public Years3 years { get; set; }
    }

    public class Years3
    {
        public _20243 _2024 { get; set; }
        public _20253 _2025 { get; set; }
    }

    public class _20243
    {
        public int starts { get; set; }
        public int earnings { get; set; }
        public Placement7 placement { get; set; }
        public int winPercentage { get; set; }
    }

    public class Placement7
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class _20253
    {
        public int starts { get; set; }
        public int earnings { get; set; }
        public Placement8 placement { get; set; }
        public int winPercentage { get; set; }
    }

    public class Placement8
    {
        public int _3 { get; set; }
        public int _2 { get; set; }
        public int _1 { get; set; }
    }

    public class Mergedpool
    {
        public string name { get; set; }
        public string[] betTypes { get; set; }
    }
}
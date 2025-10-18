using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader.Calendar
{

    // https://www.atg.se/services/racinginfo/v1/api/calendar/day/2025-05-31
    public class CalendarDay
    {
        public string date { get; set; }
        public Track[] tracks { get; set; }
        public Games games { get; set; }
        public long version { get; set; }
    }

    public class Games
    {
        public V75[] V75 { get; set; }
        public V65[] V65 { get; set; }
        public V5[] V5 { get; set; }
        public V4[] V4 { get; set; }
        public V3[] V3 { get; set; }
        public Dd[] dd { get; set; }
        public Ld[] ld { get; set; }
        public Trio[] trio { get; set; }
        public Komb[] komb { get; set; }
        public Tvilling[] tvilling { get; set; }
        public Vp[] vp { get; set; }
        public Vinnare[] vinnare { get; set; }
        public Plat[] plats { get; set; }
        public Raket[] raket { get; set; }
        public Top7[] top7 { get; set; }
    }

    public class V75
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public string[] addOns { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class V65
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class V5
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class V4
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class V3
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Dd
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
    }

    public class Ld
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
    }

    public class Trio
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool allowFlex { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Komb
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Tvilling
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Vp
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Vinnare
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Plat
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Raket
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
    }

    public class Top7
    {
        public string id { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public int[] tracks { get; set; }
        public string[] races { get; set; }
        public int returnToPlayer { get; set; }
        public bool newBettingSystem { get; set; }
    }

    public class Track
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime startTime { get; set; }
        public string nextRace { get; set; }
        public Race[] races { get; set; }
        public string biggestGameType { get; set; }
        public string sport { get; set; }
        public string countryCode { get; set; }
        public bool trackChanged { get; set; }
    }

    public class Race
    {
        public string id { get; set; }
        public int number { get; set; }
        public string status { get; set; }
        public DateTime startTime { get; set; }
        public Mergedpool[] mergedPools { get; set; }
    }

    public class Mergedpool
    {
        public string name { get; set; }
        public string[] betTypes { get; set; }
    }
}
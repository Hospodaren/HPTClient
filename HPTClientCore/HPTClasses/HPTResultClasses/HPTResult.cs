using System;

namespace HPTClient
{
    public class HPTResult
    {
        public int Turnover { get; set; }

        public string BetType { get; set; }

        public DateTime Timestamp { get; set; }

        public bool PoolClosed { get; set; }

        public int TrackId { get; set; }

        public string TrackCode { get; set; }

        public DateTime RaceDate { get; set; }

        public int NumberOfFinishedRaces { get; set; }
    }

    public class CombinationOdds
    {
        public int[] WinningHorses1 { get; set; }

        public int[] WinningHorses2 { get; set; }

        public int Odds { get; set; }
    }

    public class LegResult
    {
        public int[] WinningHorses { get; set; }

        //public int Horse1Nr { get; set; }

        //public int Horse2Nr { get; set; }

        public int Odds { get; set; }
    }
}

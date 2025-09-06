using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader
{
    public class ATGGameInfoBase
    {
        public string Code { get; set; }

        public string Id { get; set; }

        public string Status { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime ScheduledStartTime { get; set; }

        public ATGTrack[] Tracks { get; set; }

        public string?[] Races { get; set; }

        public int ReturnToPlayer { get; set; }

        public bool NewBettingSystem { get; set; }
    }
    public class ATGTrack
    {
        public string TrackName { get; set; }

        public int TrackId { get; set; }
    }
    }

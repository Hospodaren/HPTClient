using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader
{
    public class ATGRaceBase
    {
        public string Id { get; set; }

        public int Number { get; set; }

        public int Distance { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime ScheduledStartTime { get; set; }

        public string Status { get; set; }

        public IEnumerable<ATGStartBase> StartList { get; set; }
    }
}

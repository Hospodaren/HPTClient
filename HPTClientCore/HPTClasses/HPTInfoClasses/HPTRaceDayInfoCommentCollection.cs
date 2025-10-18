using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRaceDayInfoCommentCollection
    {
        public List<HPTRaceComment> RaceCommentList { get; set; }

        public string UserName { get; set; }

        public string Comment { get; set; }

        public string BetTypeCode { get; set; }

        public string RaceDayDateString { get; set; }

        public int TrackId { get; set; }

        public string TrackName { get; set; }

        public string TrackCode { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    [DataContract]
    public class HPTRaceComment
    {
        public List<HPTHorseOwnInformation> HorseOwnInformationList { get; set; }

        public int RaceNr { get; set; }

        public int LegNr { get; set; }

        public string Comment { get; set; }
    }
}

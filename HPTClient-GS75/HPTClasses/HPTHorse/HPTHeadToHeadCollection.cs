using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPTClient
{
    public class HPTHeadToHeadCollection
    {
        public HPTHeadToHeadCollection(List<HPTHorseResult> headToHeadResultList)
        {
            this.HeadToHeadResultList = headToHeadResultList;
            this.RaceNr = this.HeadToHeadResultList.First().RaceNr;
            //this.DateString = this.HeadToHeadResultList.First().DateString;
            this.Trackname = this.HeadToHeadResultList.First().TrackCode;   //EnumHelper.GetTrackNameFromTrackId(this.HeadToHeadResultList.First().TrackCode);
            this.Distance = this.HeadToHeadResultList.Min(r => r.Distance);
            this.FirstPrize = Convert.ToDecimal(this.HeadToHeadResultList.First().FirstPrize);
        }

        public string Trackname { get; set; }

        //public string DateString { get; set; }

        public int RaceNr { get; set; }

        public int Distance { get; set; }

        public decimal FirstPrize { get; set; }

        public List<HPTHorseResult> HeadToHeadResultList { get; set; }
    }
}

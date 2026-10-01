namespace HPTClient
{
    public class HPTHeadToHeadCollection
    {
        public HPTHeadToHeadCollection(List<HPTHorseResult> headToHeadResultList)
        {
            HeadToHeadResultList = headToHeadResultList;
            RaceNr = HeadToHeadResultList.First().RaceNr;
            //this.DateString = this.HeadToHeadResultList.First().DateString;
            Trackname = HeadToHeadResultList.First().TrackCode;   //EnumHelper.GetTrackNameFromTrackId(this.HeadToHeadResultList.First().TrackCode);
            Distance = HeadToHeadResultList.Min(r => r.Distance);
            FirstPrize = Convert.ToDecimal(HeadToHeadResultList.First().FirstPrize);
        }

        public string Trackname { get; set; }

        //public string DateString { get; set; }

        public int RaceNr { get; set; }

        public int Distance { get; set; }

        public decimal FirstPrize { get; set; }

        public List<HPTHorseResult> HeadToHeadResultList { get; set; }
    }
}

namespace HPTClient
{
    public class HPTUserSystem
    {
        public string EMail { get; set; }

        public string UserName { get; set; }

        public string Comment { get; set; }

        public string ReductionNames { get; set; }

        public DateTime RaceDayDate { get; set; }

        public int TrackId { get; set; }

        public string BetType { get; set; }

        public int ReducedSize { get; set; }

        public int OriginalSize { get; set; }

        public string UniqueId { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsOwnSystem
        {
            get
            {
                return EMail == HPTConfig.Config.EMailAddress;
            }
        }
    }
}

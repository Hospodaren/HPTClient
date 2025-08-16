using System;
using System.Collections.ObjectModel;

namespace HPTClient
{
    public class HPTUserSystemCollection
    {
        public ObservableCollection<HPTUserSystem> UserSystemList { get; set; }

        public string BetType { get; set; }

        public int TrackId { get; set; }

        public DateTime RaceDayDate { get; set; }
    }
}

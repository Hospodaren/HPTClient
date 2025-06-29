using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

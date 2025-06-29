using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace HPTClient
{
    class HPTHorseListContainer : Notifier, IHorseListContainer
    {
        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        public ICollection<HPTHorse> HorseList { get; set; }

        //public bool ShowComplimentaryRuleSelect { get; set; }

        //public bool ShowLegNrText { get; set; }

        //public bool ShowSystemsLeft { get; set; }

        //public bool ShowSystemValue { get; set; }

        //public bool ShowTrio { get; set; }        
    }
}

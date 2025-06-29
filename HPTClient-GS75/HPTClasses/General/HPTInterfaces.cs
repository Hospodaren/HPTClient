using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace HPTClient
{
    interface IHorseListContainer
    {
        //ICollection<HPTHorse> HorseList { get; set; }

        ICollection<HPTHorse> HorseList { get; set; }

        HPTRaceDayInfo ParentRaceDayInfo { get; set; }
    }
}

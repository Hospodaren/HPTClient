using System.Collections.Generic;

namespace HPTClient
{
    interface IHorseListContainer
    {
        //ICollection<HPTHorse> HorseList { get; set; }

        ICollection<HPTHorse> HorseList { get; set; }

        HPTRaceDayInfo ParentRaceDayInfo { get; set; }
    }
}

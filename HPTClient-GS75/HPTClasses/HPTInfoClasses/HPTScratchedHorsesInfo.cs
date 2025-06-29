using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTScratchedHorsesInfo : Notifier
    {
        public HPTScratchedHorsesInfo(HPTRaceDayInfo raceDayInfo)
        {
            //this.HorseList = new ObservableCollection<HPTHorse>();
            this.HorseList = new List<HPTHorse>();
            this.HorseListAllScratched = new ObservableCollection<HPTHorse>();
            this.ParentRaceDayInfo = raceDayInfo;
        }

        public void DeSelectAll()
        {
            foreach (HPTHorse horse in this.HorseList)
            {
                horse.Selected = false;
            }
            HaveSelectedScratchedHorse = false;
        }

        [DataMember]
        public ICollection<HPTHorse> HorseList { get; set; }

        [DataMember]
        public ObservableCollection<HPTHorse> HorseListAllScratched { get; set; }

        private HPTRaceDayInfo parentRaceDayInfo;
        public HPTRaceDayInfo ParentRaceDayInfo
        {
            get
            {
                return parentRaceDayInfo;
            }
            set
            {
                parentRaceDayInfo = value;
                OnPropertyChanged("ParentRaceDayInfo");
            }
        }

        private bool haveSelectedScratchedHorse;
        public bool HaveSelectedScratchedHorse
        {
            get
            {
                return haveSelectedScratchedHorse;
            }
            set
            {
                haveSelectedScratchedHorse = value;
                OnPropertyChanged("HaveSelectedScratchedHorse");
            }
        }
    }
}

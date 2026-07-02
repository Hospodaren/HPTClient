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
            HorseList = new List<HPTHorse>();
            HorseListAllScratched = new ObservableCollection<HPTHorse>();
            ParentRaceDayInfo = raceDayInfo;
        }

        public void DeSelectAll()
        {
            foreach (var horse in HorseList)
            {
                horse.Selected = false;
            }
            HaveSelectedScratchedHorse = false;
        }

        [DataMember]
        public ICollection<HPTHorse> HorseList { get; set; }

        [DataMember]
        public ObservableCollection<HPTHorse> HorseListAllScratched { get; set; }

        public HPTRaceDayInfo ParentRaceDayInfo
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool HaveSelectedScratchedHorse
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}

using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTPerson : Notifier, IHorseListContainer
    {
        public string Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfSelectedHorse
        {
            get
            {
                var numberOfSelected = HorseList.Count(h => h.Selected);
                return numberOfSelected;
            }
        }

        public void SetNameAndNumberOfHorse()
        {
            NameAndNumberOfHorses = $"{Name} ({NumberOfSelectedHorse}/{HorseList.Count})";
        }

        public string NameAndNumberOfHorses
        {
            get
            {
                if (field == null || field == string.Empty)
                {
                    SetNameAndNumberOfHorse();
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string ShortName { get; set; }

        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        public ICollection<HPTHorse> HorseList { get; set; }

        public bool Selected
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string ATGId { get; set; }
    }
}

using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTPerson : Notifier, IHorseListContainer
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public int NumberOfSelectedHorse
        {
            get
            {
                int numberOfSelected = HorseList.Count(h => h.Selected);
                return numberOfSelected;
            }
        }

        public void SetNameAndNumberOfHorse()
        {
            NameAndNumberOfHorses = Name + " (" + NumberOfSelectedHorse.ToString() + "/" + HorseList.Count.ToString() + ")";
        }

        private string nameAndNumberOfHorses;
        public string NameAndNumberOfHorses
        {
            get
            {
                if (nameAndNumberOfHorses == null || nameAndNumberOfHorses == string.Empty)
                {
                    SetNameAndNumberOfHorse();
                }
                return nameAndNumberOfHorses;
            }
            set
            {
                nameAndNumberOfHorses = value;
                OnPropertyChanged("NameAndNumberOfHorses");
            }
        }

        public string ShortName { get; set; }

        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        public ICollection<HPTHorse> HorseList { get; set; }

        private bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public string ATGId { get; set; }
    }
}

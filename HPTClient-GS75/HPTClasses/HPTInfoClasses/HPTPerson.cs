using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
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
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        public int NumberOfSelectedHorse
        {
            get
            {
                int numberOfSelected = this.HorseList.Count(h => h.Selected);
                return numberOfSelected;
            }
        }

        public void SetNameAndNumberOfHorse()
        {            
            this.NameAndNumberOfHorses = this.Name + " (" + this.NumberOfSelectedHorse.ToString() + "/" + this.HorseList.Count.ToString() + ")";
        }

        private string nameAndNumberOfHorses;
        public string NameAndNumberOfHorses
        {
            get
            {
                if (this.nameAndNumberOfHorses == null || this.nameAndNumberOfHorses == string.Empty)
                {
                    SetNameAndNumberOfHorse();
                }
                return this.nameAndNumberOfHorses;
            }
            set
            {
                this.nameAndNumberOfHorses = value;
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
                return this.selected;
            }
            set
            {
                this.selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public string ATGId { get; set; }
    }
}

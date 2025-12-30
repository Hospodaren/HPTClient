using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCouponRace : Notifier
    {
        public HPTCouponRace()
        {
        }

        [DataMember]
        public int LegNr { get; set; }

        private int reserv1;
        [DataMember]
        public int Reserv1
        {
            get
            {
                return this.reserv1;
            }
            set
            {
                this.reserv1 = value;
                OnPropertyChanged("Reserv1");
                if (this.HorseList != null && this.HorseList.Count > 0)
                {
                    try
                    {
                        this.Reserv1Horse = this.HorseList.First().ParentRace.HorseList.First(h => h.StartNr == value);
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }
            }
        }

        private int reserv2;
        [DataMember]
        public int Reserv2
        {
            get
            {
                return this.reserv2;
            }
            set
            {
                this.reserv2 = value;
                OnPropertyChanged("Reserv2");
                if (this.HorseList != null && this.HorseList.Count > 0)
                {
                    try
                    {
                        this.Reserv2Horse = this.HorseList.First().ParentRace.HorseList.First(h => h.StartNr == value);
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }
            }
        }

        [XmlIgnore]
        internal HPTHorse Reserv1Horse;

        [XmlIgnore]
        internal HPTHorse Reserv2Horse;

        public int NumberOfChosen { get; set; }

        public bool ReplaceScratchedHorses(HPTRace race)
        {
            try
            {
                List<HPTHorse> selectedScratchedHorses = this.HorseList.Where(h => h.Scratched == true).ToList();

                if (selectedScratchedHorses.Count() == 0)
                {
                    return false;
                }

                HPTHorse scratchedHorse1 = selectedScratchedHorses[0];
                if (this.Reserv1 != 0)
                {
                    HPTHorse horse = race.GetHorseByNumber(this.Reserv1);
                    if (horse != null)
                    {
                        if (horse.Scratched == false || horse.Scratched == null)
                        {
                            this.HorseList.Remove(scratchedHorse1);
                            this.HorseList.Add(horse);
                        }
                    }
                }
                if (selectedScratchedHorses.Count() > 1)
                {
                    HPTHorse scratchedHorse2 = selectedScratchedHorses[1];
                    if (this.Reserv2 != 0)
                    {
                        HPTHorse horse = race.GetHorseByNumber(this.Reserv2);
                        if (horse != null)
                        {
                            if (horse.Scratched == false || horse.Scratched == null)
                            {
                                this.HorseList.Remove(scratchedHorse2);
                                this.HorseList.Add(horse);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                HPTConfig.Config.AddToErrorLog(exc);
                return false;
            }
            return true;
        }

        private string uniqueCode;
        public string UniqueCode
        {
            get
            {
                if (this.uniqueCode == null)
                {
                    this.uniqueCode = string.Join(",", this.StartNrList);
                }
                return this.uniqueCode;
            }
        }

        private List<int> startNrList;
        [DataMember]
        public List<int> StartNrList
        {
            get
            {
                //if (this.startNrList == null)
                //{
                //    this.startNrList = this.HorseList.Select(h => h.StartNr).ToList();
                //}
                return this.startNrList;
            }
            set
            {
                this.startNrList = value;
            }
        }

        private List<HPTHorse> horseList;
        [XmlIgnore]
        public List<HPTHorse> HorseList
        {
            get
            {
                //if (this.horseList == null)
                //{
                //    this.horseList = this.StartNrList.Select(h => h.StartNr).ToList();
                //}
                return this.horseList;
            }
            set
            {
                this.horseList = value;
                //if (value != null)
                //{
                //    this.startNrList = this.HorseList.Select(h => h.StartNr).ToList();
                //}
            }
        }

        //[XmlIgnore]
        //public List<HPTHorse> HorseList { get; set; }
    }
}
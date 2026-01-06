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
                return reserv1;
            }
            set
            {
                reserv1 = value;
                OnPropertyChanged("Reserv1");
                if (HorseList != null && HorseList.Count > 0)
                {
                    try
                    {
                        Reserv1Horse = HorseList.First().ParentRace.HorseList.First(h => h.StartNr == value);
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
                return reserv2;
            }
            set
            {
                reserv2 = value;
                OnPropertyChanged("Reserv2");
                if (HorseList != null && HorseList.Count > 0)
                {
                    try
                    {
                        Reserv2Horse = HorseList.First().ParentRace.HorseList.First(h => h.StartNr == value);
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
                List<HPTHorse> selectedScratchedHorses = HorseList.Where(h => h.Scratched == true).ToList();

                if (selectedScratchedHorses.Count() == 0)
                {
                    return false;
                }

                HPTHorse scratchedHorse1 = selectedScratchedHorses[0];
                if (Reserv1 != 0)
                {
                    HPTHorse horse = race.GetHorseByNumber(Reserv1);
                    if (horse != null)
                    {
                        if (horse.Scratched == false || horse.Scratched == null)
                        {
                            HorseList.Remove(scratchedHorse1);
                            HorseList.Add(horse);
                        }
                    }
                }
                if (selectedScratchedHorses.Count() > 1)
                {
                    HPTHorse scratchedHorse2 = selectedScratchedHorses[1];
                    if (Reserv2 != 0)
                    {
                        HPTHorse horse = race.GetHorseByNumber(Reserv2);
                        if (horse != null)
                        {
                            if (horse.Scratched == false || horse.Scratched == null)
                            {
                                HorseList.Remove(scratchedHorse2);
                                HorseList.Add(horse);
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
                if (uniqueCode == null)
                {
                    uniqueCode = string.Join(",", StartNrList);
                }
                return uniqueCode;
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
                return startNrList;
            }
            set
            {
                startNrList = value;
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
                return horseList;
            }
            set
            {
                horseList = value;
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
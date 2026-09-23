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

        [DataMember]
        public int Reserv1
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                if (HorseList != null && HorseList.Count > 0)
                {
                    try
                    {
                        Reserv1Horse = HorseList.First().ParentRace.HorseList.First(h => h.StartNr == value);
                    }
                    catch (Exception exc)
                    {
                        var s = exc.Message;
                    }
                }
            }
        }

        [DataMember]
        public int Reserv2
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                if (HorseList != null && HorseList.Count > 0)
                {
                    try
                    {
                        Reserv2Horse = HorseList.First().ParentRace.HorseList.First(h => h.StartNr == value);
                    }
                    catch (Exception exc)
                    {
                        var s = exc.Message;
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
                var selectedScratchedHorses = HorseList.Where(h => h.Scratched == true).ToList();

                if (selectedScratchedHorses.Count() == 0)
                {
                    return false;
                }

                var scratchedHorse1 = selectedScratchedHorses[0];
                if (Reserv1 != 0)
                {
                    var horse = race.HorseList.First(h => h.StartNr == Reserv1);
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
                    var scratchedHorse2 = selectedScratchedHorses[1];
                    if (Reserv2 != 0)
                    {
                        var horse = race.HorseList.First(h => h.StartNr == Reserv2);
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

        public string UniqueCode
        {
            get
            {
                if (field == null)
                {
                    field = string.Join(",", StartNrList);
                }
                return field;
            }
        }

        [DataMember]
        public List<int> StartNrList
        {
            get
            {
                //if (this.startNrList == null)
                //{
                //    this.startNrList = this.HorseList.Select(h => h.StartNr).ToList();
                //}
                return field;
            }
            set;
        }

        [XmlIgnore]
        public List<HPTHorse> HorseList
        {
            get
            {
                //if (this.horseList == null)
                //{
                //    this.horseList = this.StartNrList.Select(h => h.StartNr).ToList();
                //}
                return field;
            }
            set;
        }

        //[XmlIgnore]
        //public List<HPTHorse> HorseList { get; set; }
    }
}
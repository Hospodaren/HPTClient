using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTV6BetMultiplierRule : Notifier
    {
        public HPTV6BetMultiplierRule()
        {
            //this.HorseList = new ObservableCollection<HPTHorse>();
            HorseList = new List<HPTHorse>();
        }

        public HPTV6BetMultiplierRule(HPTMarkBet markBet, int ruleNumber)
        {
            MarkBet = markBet;
            //this.HorseList = new ObservableCollection<HPTHorse>();
            HorseList = new List<HPTHorse>();
            BetMultiplier = 1;
            BetMultiplierList = markBet.BetType.BetMultiplierList;
            RuleNumber = ruleNumber;

            RaceList = markBet.RaceDayInfo.RaceList
                .Select(r => new HPTRaceLight()
                {
                    LegNr = r.LegNr,
                    LegNrString = $"{MarkBet.BetType.Code}-{r.LegNr}",
                    HorseList = r.HorseList
                            .Select(h => new HPTHorseLightSelectable()
                            {
                                LegNr = r.LegNr,
                                LegNrString = $"{MarkBet.BetType.Code}-{r.LegNr}",
                                Name = h.HorseName,
                                Selectable = h.Selected,
                                StartNr = h.StartNr,
                                GroupCode = ruleNumber.ToString() + r.LegNr.ToString(),
                                Horse = h
                            }).ToList()
                }).ToList();

            InitializeEventHandlers();
        }

        internal void RemoveHorse(HPTHorse horse)
        {
            var horseLight = RaceList
                .SelectMany(r => r.HorseList)
                .FirstOrDefault(h => h.StartNr == horse.StartNr && h.LegNr == horse.ParentRace.LegNr);

            if (horseLight != null)
            {
                horseLight.Selectable = false;
            }
            if (HorseList.Contains(horse))
            {
                HorseList.Remove(horse);
            }
        }

        public void InitializeEventHandlers()
        {
            foreach (var horseLightSelectable in RaceList.SelectMany(r => r.HorseList))
            {
                horseLightSelectable.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(horseLightSelectable_PropertyChanged);
            }
        }

        void horseLightSelectable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                var horseLightSelectable = (HPTHorseLightSelectable)sender;
                var horse = MarkBet.RaceDayInfo.HorseListSelected
                    .FirstOrDefault(h => h.StartNr == horseLightSelectable.StartNr && h.ParentRace.LegNr == horseLightSelectable.LegNr);

                if (horse != null)
                {
                    if (horseLightSelectable.Selected)
                    {
                        var raceLight = RaceList.First(r => r.LegNr == horseLightSelectable.LegNr);
                        raceLight.SelectedHorse = horseLightSelectable;
                        HorseList.Add(horse);
                    }
                    else
                    {
                        HorseList.Remove(horse);
                    }
                }
            }
            ;
        }

        [DataMember]
        public List<HPTRaceLight> RaceList { get; set; }

        [XmlIgnore]
        public HPTMarkBet MarkBet { get; set; }

        [XmlIgnore]
        public ICollection<HPTHorse> HorseList { get; set; }

        [DataMember]
        public bool Use
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool V6
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int BetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int RuleNumber
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public int[] BetMultiplierList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public int NumberOfRowsAffected
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string ClipboardString { get; set; }

        public string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();
            //sb.AppendLine("Regel för V6/V7/V8/Flerbong");
            sb.AppendLine("Regel för V6/Flerbong");
            if (Use)
            {
                if (V6)
                {
                    sb.Append("V");
                    sb.Append(markBet.BetType.NumberOfRaces);
                }
                if (V6 && BetMultiplier > 1)
                {
                    sb.Append(" och ");
                }
                if (BetMultiplier > 1)
                {
                    sb.Append(BetMultiplier);
                    sb.Append(" x Flerbong");
                }
            }
            sb.AppendLine();

            foreach (var horse in HorseList)
            {
                sb.AppendLine(horse.HorseName);
            }
            ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}

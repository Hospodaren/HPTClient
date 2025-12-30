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
            this.HorseList = new List<HPTHorse>();
        }

        public HPTV6BetMultiplierRule(HPTMarkBet markBet, int ruleNumber)
        {
            this.MarkBet = markBet;
            //this.HorseList = new ObservableCollection<HPTHorse>();
            this.HorseList = new List<HPTHorse>();
            this.BetMultiplier = 1;
            this.BetMultiplierList = markBet.BetType.BetMultiplierList;
            this.RuleNumber = ruleNumber;

            this.RaceList = markBet.RaceDayInfo.RaceList
                .Select(r => new HPTRaceLight()
                {
                    LegNr = r.LegNr,
                    LegNrString = this.MarkBet.BetType.Code + "-" + r.LegNr.ToString(),
                    HorseList = r.HorseList
                            .Select(h => new HPTHorseLightSelectable()
                            {
                                LegNr = r.LegNr,
                                LegNrString = this.MarkBet.BetType.Code + "-" + r.LegNr.ToString(),
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
            var horseLight = this.RaceList
                .SelectMany(r => r.HorseList)
                .FirstOrDefault(h => h.StartNr == horse.StartNr && h.LegNr == horse.ParentRace.LegNr);

            if (horseLight != null)
            {
                horseLight.Selectable = false;
            }
            if (this.HorseList.Contains(horse))
            {
                this.HorseList.Remove(horse);
            }
        }

        public void InitializeEventHandlers()
        {
            foreach (var horseLightSelectable in this.RaceList.SelectMany(r => r.HorseList))
            {
                horseLightSelectable.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(horseLightSelectable_PropertyChanged);
            }
        }

        void horseLightSelectable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                HPTHorseLightSelectable horseLightSelectable = (HPTHorseLightSelectable)sender;
                HPTHorse horse = this.MarkBet.RaceDayInfo.HorseListSelected
                    .FirstOrDefault(h => h.StartNr == horseLightSelectable.StartNr && h.ParentRace.LegNr == horseLightSelectable.LegNr);

                if (horse != null)
                {
                    if (horseLightSelectable.Selected)
                    {
                        var raceLight = this.RaceList.First(r => r.LegNr == horseLightSelectable.LegNr);
                        raceLight.SelectedHorse = horseLightSelectable;
                        this.HorseList.Add(horse);
                    }
                    else
                    {
                        this.HorseList.Remove(horse);
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

        private bool use;
        [DataMember]
        public bool Use
        {
            get
            {
                return this.use;
            }
            set
            {
                this.use = value;
                OnPropertyChanged("Use");
            }
        }

        private bool v6;
        [DataMember]
        public bool V6
        {
            get
            {
                return this.v6;
            }
            set
            {
                this.v6 = value;
                OnPropertyChanged("V6");
            }
        }

        private int betMultiplier;
        [DataMember]
        public int BetMultiplier
        {
            get
            {
                return this.betMultiplier;
            }
            set
            {
                this.betMultiplier = value;
                OnPropertyChanged("BetMultiplier");
            }
        }

        private int ruleNumber;
        [DataMember]
        public int RuleNumber
        {
            get
            {
                return this.ruleNumber;
            }
            set
            {
                this.ruleNumber = value;
                OnPropertyChanged("RuleNumber");
            }
        }

        private int[] betMultiplierList;
        [XmlIgnore]
        public int[] BetMultiplierList
        {
            get
            {
                return this.betMultiplierList;
            }
            set
            {
                this.betMultiplierList = value;
                OnPropertyChanged("BetMultiplierList");
            }
        }

        private int numberOfRowsAffected;
        [XmlIgnore]
        public int NumberOfRowsAffected
        {
            get
            {
                return this.numberOfRowsAffected;
            }
            set
            {
                this.numberOfRowsAffected = value;
                OnPropertyChanged("NumberOfRowsAffected");
            }
        }

        [XmlIgnore]
        public string ClipboardString { get; set; }

        public string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("Regel för V6/V7/V8/Flerbong");
            sb.AppendLine("Regel för V6/Flerbong");
            if (this.Use)
            {
                if (this.V6)
                {
                    sb.Append("V");
                    sb.Append(markBet.BetType.NumberOfRaces);
                }
                if (this.V6 && this.BetMultiplier > 1)
                {
                    sb.Append(" och ");
                }
                if (this.BetMultiplier > 1)
                {
                    sb.Append(this.BetMultiplier);
                    sb.Append(" x Flerbong");
                }
            }
            sb.AppendLine();

            foreach (HPTHorse horse in this.HorseList)
            {
                sb.AppendLine(horse.HorseName);
            }
            this.ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}

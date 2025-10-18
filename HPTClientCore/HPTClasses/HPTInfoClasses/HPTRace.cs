using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRace : Notifier, IHorseListContainer
    {
        // TEST
        internal HPTService.HPTRace race;

        #region egna events

        public event Action<int, int, bool> NumberOfSelectedChanged;

        public event Action<HPTHorse, EventArgs> TrioSelectionChanged;

        public event Action<HPTRace, bool> SetAllSelected;

        public event Action<HPTRace> ClearABCD;

        #endregion

        public HPTRace()
        {
            this.HorseList = new List<HPTHorse>();
        }

        public HPTRace(HPTService.HPTRace race, HPTRaceDayInfo raceDayInfo)
        {
            //this.HorseList = new ObservableCollection<HPTHorse>();
            this.ParentRaceDayInfo = raceDayInfo;
            this.race = race;
        }

        public void Merge(HPTService.HPTRace race)
        {
            try
            {
                if (race.SharedInfo.RaceNr == this.RaceNr)
                {
                    this.TurnoverPlats = race.SharedInfo.TurnoverPlats > 0 ? race.SharedInfo.TurnoverPlats : this.TurnoverPlats;
                    this.TurnoverTvilling = race.SharedInfo.TurnoverTvilling > 0 ? race.SharedInfo.TurnoverTvilling : this.TurnoverTvilling;
                    this.TurnoverVinnare = race.SharedInfo.TurnoverVinnare > 0 ? race.SharedInfo.TurnoverVinnare : this.TurnoverVinnare;

                    this.NumberOfStartingHorses = 0;
                    foreach (HPTService.HPTHorse horse in race.HorseList)
                    {
                        var hptHorse = GetHorseByNumber((int)horse.StartNr);
                        hptHorse.Merge(horse);
                        hptHorse.SetColors();
                    }
                    switch (this.ParentRaceDayInfo.BetType.Code)
                    {
                        case "V4":
                        case "V5":
                        case "V64":
                        case "V65":
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                            break;
                        case "T":
                            this.CombinationListInfoTrio.CalculatePercentages(this.HorseList);
                            break;
                        //case "TV":
                        //    this.CombinationListInfoTvilling.CalculatePercentages(this.HorseList);
                        //    break;
                        case "DD":
                        case "LD":
                            // Hantera eventuellt strukna hästar
                            this.HorseList
                                .Where(h => h.Selected && h.Scratched == true)
                                .ToList()
                                .ForEach(h => h.Selected = false);
                            break;
                        default:
                            break;
                    }

                    //PerformCalculations();
                    SetCorrectStakeDistributionShare();
                    SetCorrectStakeDistributionShareAlt1();
                    SetCorrectStakeDistributionShareAlt2();

                    if (race.SharedInfo.TvillingCombinationList != null && this.CombinationListInfoTvilling.CombinationList != null)
                    {
                        // Hantera eventuellt strukna hästar
                        var selectedScratchedHorses = this.HorseList.Where(h => h.Selected && h.Scratched == true).ToList();
                        selectedScratchedHorses.ForEach(h =>
                            {
                                h.Selected = false;
                                this.CombinationListInfoTvilling.CombinationList
                                    .Where(c => c.Horse1 == h || c.Horse2 == h)
                                    .ToList()
                                    .ForEach(c =>
                                    {
                                        c.Selected = false;
                                        c.Stake = null;
                                    });
                            });

                        // Uppdatera alla kombinationer
                        foreach (HPTService.HPTCombination comb in race.SharedInfo.TvillingCombinationList)
                        {
                            var hptComb = this.CombinationListInfoTvilling.CombinationList.First(c => c.UniqueCode == this.CombinationListInfoTvilling.GetUniqueCodeFromCombination(comb));
                            hptComb.CombinationOdds = comb.CombinationOdds;
                            hptComb.CombinationOddsExact = comb.CombinationOddsExact;
                            hptComb.CalculateQuotas("TV");
                        }
                        this.CombinationListInfoTvilling.SortCombinationValues();
                    }

                    if (race.SharedInfo.TrioCombinationList != null && this.CombinationListInfoTrio.CombinationList != null)
                    {
                        CreateAllTrioCombinations(race.SharedInfo.TrioCombinationList);
                        CombinationListInfoTrio.SortCombinationValues();
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public void ConvertRace(HPTService.HPTRace race)
        {
            this.Distance = race.SharedInfo.Distance;
            this.StartMethodCode = race.SharedInfo.StartMethodCode;
            this.PostTime = race.SharedInfo.PostTime;

            Regex rexExtractNumber = new Regex("\\d+");
            if (!string.IsNullOrEmpty(this.Distance) && rexExtractNumber.IsMatch(this.Distance))
            {
                Match m = rexExtractNumber.Match(this.Distance);
                int iDistance = int.Parse(m.Value);
                if (iDistance < 1800)
                {
                    this.DistanceCode = "K";
                }
                else if (iDistance > 2500)
                {
                    this.DistanceCode = "L";
                }
                else
                {
                    this.DistanceCode = "M";
                }

                if (this.StartMethodCode == "A")
                {
                    this.StartMethodAndDistanceCode = this.StartMethodCode + this.DistanceCode;
                }
                else
                {
                    this.StartMethodAndDistanceCode = this.DistanceCode;
                }
            }
            else
            {
                this.DistanceCode = string.Empty;
            }

            this.RaceNr = race.SharedInfo.RaceNr;
            this.LegNr = race.LegNr;
            this.RaceName = race.SharedInfo.RaceName;
            this.RaceInfoShort = race.SharedInfo.RaceInfoShort.Replace("\n", string.Empty);
            this.RaceInfoLong = race.SharedInfo.RaceInfoLong;
            this.RaceShortText = "Lopp " + this.RaceNr.ToString();
            this.ReservOrder = race.ReservOrder;

            this.ReservOrderList = new int[0];
            if (this.ReservOrder != string.Empty)
            {
                string[] reservStringArray = this.ReservOrder.Split('-');
                this.ReservOrderList = new int[reservStringArray.Length];
                for (int i = 0; i < reservStringArray.Length; i++)
                {
                    this.ReservOrderList[i] = int.Parse(reservStringArray[i]);
                }
            }

            this.StartMethod = race.SharedInfo.StartMethod;
            this.StartMethodCode = race.SharedInfo.StartMethodCode;
            this.TurnoverPlats = race.SharedInfo.TurnoverPlats;
            this.TurnoverTvilling = race.SharedInfo.TurnoverTvilling;
            this.TurnoverVinnare = race.SharedInfo.TurnoverVinnare;
            this.TrackId = race.TrackId;

            if (race.HorseList != null)
            {
                //this.HorseList = new ObservableCollection<HPTHorse>();
                this.HorseList = new List<HPTHorse>();

                Parallel.ForEach(race.HorseList, horse =>
                {
                    var startNumberRankCollection = HPTConfig.Config.StartNumberRankCollectionList.FirstOrDefault(snr => snr.StartMethodCode == this.StartMethodCode);
                    var hptHorse = new HPTHorse()
                    {
                        ParentRace = this
                    };
                    hptHorse.ConvertHorse(horse);
                    this.HorseList.Add(hptHorse);
                    hptHorse.CreateXReductionRuleList();

                    // Sätt Spårrank utifrån Config
                    if (startNumberRankCollection != null)
                    {
                        var startNumberRank = startNumberRankCollection.StartNumberRankList.FirstOrDefault(sr => sr.StartNumber == hptHorse.StartNr);
                        if (startNumberRank != null && hptHorse.Scratched != true)
                        {
                            hptHorse.Selected = startNumberRank.Select;
                            hptHorse.RankStartNumber = startNumberRank.Rank;
                        }
                    }
                });

                //foreach (HPTService.HPTHorse horse in race.HorseList)
                //{
                //    var startNumberRankCollection = HPTConfig.Config.StartNumberRankCollectionList.FirstOrDefault(snr => snr.StartMethodCode == this.StartMethodCode);
                //    var hptHorse = this.HorseList.FirstOrDefault(h => h.StartNr == horse.StartInfo.StartNr);
                //    if (hptHorse == null)
                //    {
                //        hptHorse = new HPTHorse();
                //        hptHorse.ParentRace = this;
                //        hptHorse.ConvertHorse(horse);
                //        this.HorseList.Add(hptHorse);
                //        hptHorse.CreateXReductionRuleList();
                //    }

                //    // Sätt Spårrank utifrån Config
                //    if (startNumberRankCollection != null)
                //    {
                //        var startNumberRank = startNumberRankCollection.StartNumberRankList.FirstOrDefault(sr => sr.StartNumber == hptHorse.StartNr);
                //        if (startNumberRank != null && hptHorse.Scratched != true)
                //        {
                //            hptHorse.Selected = startNumberRank.Select;
                //            hptHorse.RankStartNumber = startNumberRank.Rank;
                //        }
                //    }
                //}

                // Sätt ATG-rank utifrån reservordning
                for (int i = 0; i < this.ReservOrderList.Length; i++)
                {
                    HPTHorse hptHorse = this.GetHorseByNumber(this.ReservOrderList[i]);
                    hptHorse.RankATG = i + 1;
                }

                this.SetCorrectStakeDistributionShare();
                this.SetCorrectStakeDistributionShareAlt1();
                this.SetCorrectStakeDistributionShareAlt2();
            }

            if (this.ParentRaceDayInfo == null)
            {
                return;
            }

            // Tvilling
            if (this.ParentRaceDayInfo.BetType.Code == "TV")
            {
                this.CombinationListInfoTvilling = new HPTCombinationListInfo()
                {
                    CombinationList = new List<HPTCombination>(),
                    BetType = this.ParentRaceDayInfo.BetType.Code
                };

                if (race.SharedInfo.TvillingCombinationList != null && race.SharedInfo.TvillingCombinationList.Length > 0)
                {
                    foreach (HPTService.HPTCombination comb in race.SharedInfo.TvillingCombinationList)
                    {
                        HPTCombination hptComb = new HPTCombination();
                        hptComb.ParentRace = this;
                        hptComb.ParentRaceDayInfo = this.ParentRaceDayInfo;
                        hptComb.CombinationOdds = comb.CombinationOdds;
                        hptComb.Horse1Nr = comb.Horse1Nr;
                        hptComb.Horse2Nr = comb.Horse2Nr;
                        hptComb.Horse3Nr = comb.Horse3Nr;

                        hptComb.Horse1 = this.GetHorseByNumber(comb.Horse1Nr);
                        hptComb.Horse2 = this.GetHorseByNumber(comb.Horse2Nr);

                        hptComb.CalculateQuotas("TV");
                        this.CombinationListInfoTvilling.CombinationList.Add(hptComb);
                    }
                    this.CombinationListInfoTvilling.SortCombinationValues();
                }
                else
                {
                    for (int horse1 = 0; horse1 < this.HorseList.Count - 1; horse1++)
                    {
                        //HPTHorse hptHorse1 = this.HorseList[horse1];
                        HPTHorse hptHorse1 = this.HorseList.ElementAt(horse1);
                        for (int horse2 = horse1 + 1; horse2 < this.HorseList.Count; horse2++)
                        {
                            //HPTHorse hptHorse2 = this.HorseList[horse2];
                            HPTHorse hptHorse2 = this.HorseList.ElementAt(horse2);
                            HPTCombination hptComb = new HPTCombination();
                            hptComb.ParentRace = this;
                            hptComb.ParentRaceDayInfo = this.ParentRaceDayInfo;
                            hptComb.CombinationOdds = 0M;
                            hptComb.Horse1Nr = hptHorse1.StartNr;
                            hptComb.Horse2Nr = hptHorse2.StartNr;

                            hptComb.Horse1 = hptHorse1;
                            hptComb.Horse2 = hptHorse2;

                            hptComb.CalculateQuotas("TV");
                            this.CombinationListInfoTvilling.CombinationList.Add(hptComb);
                        }
                    }
                }
                foreach (var horse in this.HorseList)
                {
                    horse.OwnProbability = horse.StakeShareRounded;
                }
            }

            // Trio
            if (this.ParentRaceDayInfo.BetType.Code == "T")
            {
                this.CombinationListInfoTrio = new HPTCombinationListInfo()
                {
                    CombinationList = new List<HPTCombination>(),
                    BetType = this.ParentRaceDayInfo.BetType.Code
                };
                this.CombinationListInfoTrio.CalculatePercentages(this.HorseList);
                this.CreateAllTrioCombinations(race.SharedInfo.TrioCombinationList);
                this.CombinationListInfoTrio.SortCombinationValues();
            }

            // Inbördes möte hästar emellan
            this.FindHeadToHead();
        }


        internal void CreateAllTrioCombinations(HPTService.HPTCombination[] combArray)
        {
            // Beräkna äkta insatsfördelning per häst
            decimal turnoverTrio1 = this.HorseList.Sum(h => h.TrioInfo.PlaceInfo1.Investment);
            decimal turnoverTrio2 = this.HorseList.Sum(h => h.TrioInfo.PlaceInfo2.Investment);
            decimal turnoverTrio3 = this.HorseList.Sum(h => h.TrioInfo.PlaceInfo3.Investment);
            if (turnoverTrio1 > 0M && turnoverTrio2 > 0M && turnoverTrio3 > 0M)
            {
                this.HorseList.ToList().ForEach(h =>
                    {
                        h.TrioInfo.PlaceInfo1.InvestmentShare = h.TrioInfo.PlaceInfo1.Investment / turnoverTrio1;
                        h.TrioInfo.PlaceInfo2.InvestmentShare = h.TrioInfo.PlaceInfo2.Investment / turnoverTrio2;
                        h.TrioInfo.PlaceInfo3.InvestmentShare = h.TrioInfo.PlaceInfo3.Investment / turnoverTrio3;
                    });
            }

            // Hantera eventuellt strukna hästar
            var selectedScratchedHorses = this.HorseList.Where(h => h.Selected && h.Scratched == true).ToList();
            selectedScratchedHorses.ForEach(h =>
            {
                h.Selected = false;
                this.CombinationListInfoTrio.CombinationList
                    .Where(c => c.Horse1 == h || c.Horse2 == h || c.Horse3 == h)
                    .ToList()
                    .ForEach(c =>
                    {
                        c.Stake = null;
                    });
            });

            int maxOdds = combArray.Max(c => c.CombinationOdds) + 1;
            foreach (var horse1 in this.HorseList.Where(h => h.Scratched == false || h.Scratched == null))
            {
                foreach (var horse2 in this.HorseList.Where(h => (h.Scratched == false || h.Scratched == null) && h.StartNr != horse1.StartNr))
                {
                    foreach (var horse3 in this.HorseList.Where(h => (h.Scratched == false || h.Scratched == null) && h.StartNr != horse1.StartNr && h.StartNr != horse2.StartNr))
                    {
                        var serviceComb = combArray.FirstOrDefault(c => c.Horse1Nr == horse1.StartNr && c.Horse2Nr == horse2.StartNr && c.Horse3Nr == horse3.StartNr);
                        var hptComb = this.CombinationListInfoTrio.CombinationList.FirstOrDefault(c => c.Horse1Nr == horse1.StartNr && c.Horse2Nr == horse2.StartNr && c.Horse3Nr == horse3.StartNr);

                        if (hptComb == null)
                        {
                            hptComb = new HPTCombination()
                            {
                                ParentRace = this,
                                ParentRaceDayInfo = this.ParentRaceDayInfo,
                                Horse1Nr = horse1.StartNr,
                                Horse2Nr = horse2.StartNr,
                                Horse3Nr = horse3.StartNr,
                                Horse1 = horse1,
                                Horse2 = horse2,
                                Horse3 = horse3,
                                CombinationOdds = serviceComb.CombinationOdds,
                                CombinationOddsExact = serviceComb.CombinationOddsExact
                            };
                            hptComb.CalculateQuotas("T");
                            this.CombinationListInfoTrio.CombinationList.Add(hptComb);
                        }
                        if (serviceComb != null)
                        {
                            hptComb.CombinationOdds = serviceComb.CombinationOdds;
                            hptComb.CombinationOddsExact = serviceComb.CombinationOddsExact;
                        }
                        else
                        {
                            hptComb.CombinationOdds = maxOdds;
                            hptComb.CombinationOddsExact = maxOdds;
                        }
                    }
                }
            }
        }

        internal void SetHorseTrioInfoValues()
        {
            this.HorseList
                .Where(h => h.Scratched == false || h.Scratched == null)
                .ToList()
                .ForEach(h =>
                {
                    h.TrioInfo = new HPTHorseTrioInfo()
                    {
                        PlaceInfo1 = new HPTHorseTrioPlaceInfo()
                        {
                            //Investment = this.tu
                        }
                    };
                });
        }

        #region Properties

        [DataMember]
        public int RaceNr { get; set; }

        [DataMember]
        public int LegNr { get; set; }

        [DataMember]
        public int? TrackId { get; set; }   // För Saxade banor

        public string TrackName   // För Saxade banor
        {
            get
            {
                if (this.TrackId == null || this.TrackId == 0)
                {
                    if (this.ParentRaceDayInfo != null)
                    {
                        return this.ParentRaceDayInfo.Trackname;
                    }
                    return null;
                }
                return EnumHelper.GetTrackNameFromTrackId(Convert.ToInt32(this.TrackId));
            }
        }

        public string ATGLink   // För Saxade banor
        {
            get
            {
                return ATGLinkCreator.CreateRaceStartlistLink(this);
            }
        }

        public string TrackCode   // För Saxade banor
        {
            get
            {
                if (this.TrackId == null || this.TrackId == 0)
                {
                    return null;
                }
                return EnumHelper.GetTrackCodeFromTrackId(Convert.ToInt32(this.TrackId));
            }
        }

        [XmlIgnore]
        public string LegNrString { get; set; }

        [XmlIgnore]
        public string Reserv1GroupName { get; set; }

        [XmlIgnore]
        public string Reserv2GroupName { get; set; }

        [DataMember]
        public string RaceName { get; set; }

        [DataMember]
        public string RaceInfoShort { get; set; }

        [DataMember]
        public string RaceInfoLong { get; set; }

        [DataMember]
        public string RaceShortText { get; set; }

        [DataMember]
        public DateTime PostTime { get; set; }

        private DateTime updatedPostTime;
        [XmlIgnore]
        public DateTime UpdatedPostTime
        {
            get
            {
                return this.updatedPostTime;
            }
            set
            {
                this.updatedPostTime = value;
                OnPropertyChanged("UpdatedPostTime");
            }
        }

        [DataMember]
        public string StartMethodAndDistanceCode { get; set; }

        [DataMember]
        public string StartMethod { get; set; }

        [DataMember]
        public string StartMethodCode { get; set; }

        [DataMember]
        public string DistanceCode { get; set; }

        [DataMember]
        public string Distance { get; set; }

        private decimal turnoverPlats;
        [DataMember]
        public decimal TurnoverPlats
        {
            get
            {
                return turnoverPlats;
            }
            set
            {
                this.turnoverPlats = value;
                OnPropertyChanged("TurnoverPlats");
            }
        }

        private decimal turnoverVinnare;
        [DataMember]
        public decimal TurnoverVinnare
        {
            get
            {
                return turnoverVinnare;
            }
            set
            {
                this.turnoverVinnare = value;
                OnPropertyChanged("TurnoverVinnare");
            }
        }

        //[DataMember]
        //public decimal TurnoverTrio { get; set; }

        //[DataMember]
        //public decimal TurnoverTvilling { get; set; }

        private decimal turnoverTvilling;
        [DataMember]
        public decimal TurnoverTvilling
        {
            get
            {
                return turnoverTvilling;
            }
            set
            {
                this.turnoverTvilling = value;
                OnPropertyChanged("TurnoverTvilling");
                if (value > 0M)
                {
                    this.TurnoverCombination = (int)value;
                }
            }
        }

        private int turnoverTrio;
        [DataMember]
        public int TurnoverTrio
        {
            get
            {
                return turnoverTrio;
            }
            set
            {
                this.turnoverTrio = value;
                OnPropertyChanged("TurnoverTrio");
                if (value > 0M)
                {
                    this.TurnoverCombination = (int)value;
                }
            }
        }

        private int turnoverCombination;
        [DataMember]
        public int TurnoverCombination
        {
            get
            {
                return turnoverCombination;
            }
            set
            {
                this.turnoverCombination = value;
                OnPropertyChanged("TurnoverCombination");
            }
        }

        [DataMember]
        public int MarksQuantity { get; set; }

        public string[] BetTypes { get; set; }

        private int reserv1Nr;
        [DataMember]
        public int Reserv1Nr
        {
            get
            {
                return reserv1Nr;
            }
            set
            {
                this.reserv1Nr = value;
                OnPropertyChanged("Reserv1Nr");
            }
        }

        private int reserv2Nr;
        [DataMember]
        public int Reserv2Nr
        {
            get
            {
                return reserv2Nr;
            }
            set
            {
                this.reserv2Nr = value;
                OnPropertyChanged("Reserv2Nr");
            }
        }


        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public int Reserv2Nr { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string ReservOrder { get; set; }

        private int[] reservOrderList;
        public int[] ReservOrderList
        {
            get
            {
                //if (this.reservOrderList == null)
                //{
                //    this.reservOrderList = this.HorseList
                //        .OrderByDescending(h => h.StartPoint)
                //        .Select(h => h.StartNr)
                //        .ToArray();
                //}
                if (this.reservOrderList == null)
                {
                    this.reservOrderList = this.HorseList
                        .OrderByDescending(h => h.StakeDistribution)
                        .Select(h => h.StartNr)
                        .ToArray();
                }
                return this.reservOrderList;
            }
            set
            {
                this.reservOrderList = value;
            }
        }

        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public ICollection<HPTHorse> HorseList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ICollection<HPTHorse> HorseList { get; set; }

        private List<HPTHorse> horseListSelected;
        [XmlIgnore]
        public List<HPTHorse> HorseListSelected
        {
            get
            {
                //if (this.horseListSelected == null || this.horseListSelected.Count == 0)
                if (this.horseListSelected == null)
                {
                    this.horseListSelected = this.HorseList.Where(h => h.Selected == true).ToList();
                    //this.horseListSelected = this.HorseList.Where(h => h.Selected == true).ToList();
                    //this.NumberOfSelectedHorses = this.horseListSelected.Count;
                }
                return this.horseListSelected;
            }
            set
            {
                this.horseListSelected = value;
            }
        }

        [XmlIgnore]
        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        private bool locked;
        [DataMember]
        public bool Locked
        {
            get
            {
                return this.locked;
            }
            set
            {
                this.locked = value;
                OnPropertyChanged("Locked");
            }
        }

        #endregion

        #region Calculated properties

        [XmlIgnore]
        public string ClipboardString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.LegNrString);
                sb.Append(" (");
                sb.Append(this.PostTime.ToString("HH:mm"));
                sb.Append(") :");
                //sb.Append(this.RaceName);
                //sb.Append(" - ");
                sb.Append(this.RaceInfoShort);
                if (this.Reserv1Nr != 0)
                {
                    sb.Append(" (R1:");
                    sb.Append(this.Reserv1Nr);
                    if (this.Reserv2Nr != 0)
                    {
                        sb.Append(", R2:");
                        sb.Append(this.Reserv2Nr);
                    }
                    sb.Append(")");
                }
                return sb.ToString();
            }
        }

        public string ToClipboardString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.ClipboardString);
            foreach (HPTHorse horse in this.HorseListSelected)
            {
                sb.AppendLine(horse.ClipboardString);
            }
            return sb.ToString();
        }

        public string ToCompactClipboardString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(LegNrString);
            //sb.AppendLine(this.ClipboardString);

            for (int i = 0; i < 6; i++)
            {
                sb.Append(GetHorsesWithPrio(i));
            }
            return sb.ToString();
        }

        internal string GetHorsesWithPrio(int prioValue)
        {
            var prio = (HPTPrio)prioValue;
            var sb = new StringBuilder();
            var startNumberListWithPrio = this.HorseListSelected
                .Where(h => h.Prio == prio)
                .Select(h => h.StartNr.ToString());

            if (startNumberListWithPrio.Count() > 0)
            {
                sb.Append(" ");
                if (prio != HPTPrio.M)
                {
                    sb.Append(prio.ToString());
                    sb.Append(":");
                }
                string startNumbersString = startNumberListWithPrio.Aggregate((startnr, next) => startnr + ", " + next);
                sb.Append(startNumbersString);
            }
            return sb.ToString();
        }

        #endregion

        #region Methods

        public int GetNumberOfX(HPTPrio prio)
        {
            int numberOfX = this.HorseListSelected.Count(h => h.Prio == prio);
            return numberOfX;
        }

        public void SetTrioSelectionChanged(HPTHorse horse)
        {
            if (this.TrioSelectionChanged != null)
            {
                this.TrioSelectionChanged(horse, new EventArgs());
            }
        }

        public void SetNumberOfSelectedHorses(int legNr, int startNr, bool selected)
        {
            //var selectedHorses = this.HorseList.Where(h => h.Selected == true);
            //if (this.HorseListSelected == null)
            //{
            //    this.HorseListSelected = new List<HPTHorse>(selectedHorses);
            //}
            //else
            //{
            //    this.HorseListSelected.Clear();
            //    this.HorseListSelected.AddRange(selectedHorses);
            //}

            ////this.HorseListSelected = this.HorseList.Where(h => h.Selected == true).ToList();
            //this.NumberOfSelectedHorses = this.HorseListSelected.Count;

            //if (this.NumberOfSelectedHorses == 0 && this.Locked)
            //{
            //    this.Locked = false;
            //}
            if (this.NumberOfSelectedChanged != null)
            {
                //this.NumberOfSelectedChanged(horse, new EventArgs());
                this.NumberOfSelectedChanged(legNr, startNr, selected);
            }
        }

        //public void SetNumberOfSelectedHorses(HPTHorse horse)
        //{
        //    this.HorseListSelected = this.HorseList.Where(h => h.Selected == true).ToList();
        //    this.NumberOfSelectedHorses = this.HorseListSelected.Count;

        //    if (this.NumberOfSelectedHorses == 0 && this.Locked)
        //    {
        //        this.Locked = false;
        //    }
        //    if (this.NumberOfSelectedChanged != null)
        //    {
        //        this.NumberOfSelectedChanged(horse, new EventArgs());
        //    }
        //}

        public void SelectAll(bool selected)
        {
            if (this.SetAllSelected != null)
            {
                this.SetAllSelected(this, selected);
            }
        }

        public void ClearABCDRace()
        {
            if (this.ClearABCD != null)
            {
                this.ClearABCD(this);
            }
        }

        private int noSelected;
        [XmlIgnore]
        public int NumberOfSelectedHorses
        {
            get
            {
                if (this.noSelected == 0)
                {
                    this.horseListSelected = this.HorseList.Where(h => h.Selected == true).ToList();
                    this.noSelected = this.horseListSelected.Count;
                }
                return this.noSelected;
            }
            set
            {
                this.noSelected = value;
                OnPropertyChanged("NumberOfSelectedHorses");
            }
        }

        private int numberOfStartingHorses;
        [XmlIgnore]
        public int NumberOfStartingHorses
        {
            get
            {
                if (this.numberOfStartingHorses == 0)
                {
                    this.numberOfStartingHorses = this.HorseList.Count(h => h.Scratched == false || h.Scratched == null);
                }
                return this.numberOfStartingHorses;
            }
            set
            {
                this.numberOfStartingHorses = value;
            }
        }

        private decimal? ownProbabilitySum;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? OwnProbabilitySum
        {
            get
            {
                return ownProbabilitySum;
            }
            set
            {
                ownProbabilitySum = value;
                OnPropertyChanged("OwnProbabilitySum");
            }
        }

        private int rankOwnSum;
        public int RankOwnSum
        {
            get
            {
                return rankOwnSum;
            }
            set
            {
                rankOwnSum = value;
                OnPropertyChanged("RankOwnSum");
            }
        }

        private int rankAlternateSum;
        public int RankAlternateSum
        {
            get
            {
                return rankAlternateSum;
            }
            set
            {
                rankAlternateSum = value;
                OnPropertyChanged("RankAlternateSum");
            }
        }

        private int rankABCSum;
        public int RankABCSum
        {
            get
            {
                return rankABCSum;
            }
            set
            {
                rankABCSum = value;
                OnPropertyChanged("RankABCSum");
            }
        }

        internal void SetCorrectStakeDistributionShare()
        {
            // Rätta till exakt insatsfördelning
            switch (this.ParentRaceDayInfo.BetType.Code)
            {
                case "V64":
                case "V65":
                case "V75":
                case "GS75":
                case "V85":
                case "V86":
                    decimal shareSum = this.HorseList.Sum(h => h.StakeDistributionShare);
                    decimal shareSumWithoutScratchings = this.HorseList.Where(h => h.Scratched != true).Sum(h => h.StakeDistributionShare);
                    decimal shareSumWithoutScratchingsQuota = 1M;
                    if (shareSum > 0)
                    {
                        shareSumWithoutScratchingsQuota = shareSumWithoutScratchings / shareSum;
                    }
                    foreach (var horse in this.HorseList)
                    {
                        if (shareSum > 0)
                        {
                            horse.StakeDistributionShare /= shareSum;
                            horse.StakeShareRounded = Math.Round(horse.StakeDistributionShare, 3);
                            if (horse.OwnProbability == null || horse.OwnProbability == 0M)
                            {
                                horse.OwnProbability = horse.StakeShareRounded;
                            }
                            horse.StakeShareWithoutScratchings = horse.StakeDistributionShare / shareSumWithoutScratchingsQuota;
                        }
                    }
                    break;
                case "V4":
                case "V5":
                    decimal shareSumWithoutScratchingsVx = this.HorseList.Where(h => h.Scratched != true).Sum(h => h.StakeDistributionShare);
                    foreach (var horse in this.HorseList)
                    {
                        horse.StakeShareRounded = Math.Round(horse.StakeDistributionShare, 3);
                        if (horse.OwnProbability == null || horse.OwnProbability == 0M)
                        {
                            horse.OwnProbability = horse.StakeShareRounded;
                        }
                        if (shareSumWithoutScratchingsVx > 0)
                        {
                            horse.StakeShareWithoutScratchings = horse.StakeDistributionShare / shareSumWithoutScratchingsVx;
                        }
                    }
                    break;
                default:
                    break;
            }

            // Sätt ackumulerad insatsfördelning
            foreach (var horse in this.HorseList)
            {
                horse.StakeDistributionShareAccumulated = this.HorseList.Where(h => h.StakeDistributionShare >= horse.StakeDistributionShare).Sum(h => h.StakeDistributionShare);
            }
        }

        internal void SetCorrectStakeDistributionShareAlt1()
        {
            // Rätta till alternativ insatsfördelning 1
            decimal shareSum = this.HorseList.Sum(h => Convert.ToDecimal(h.StakeShareAlternate));
            foreach (var horse in this.HorseList)
            {
                if (shareSum > 0)
                {
                    horse.StakeShareAlternate /= shareSum;
                }
            }
        }

        internal void SetCorrectStakeDistributionShareAlt2()
        {
            // Rätta till alternativ insatsfördelning 2
            decimal shareSum = this.HorseList.Sum(h => Convert.ToDecimal(h.StakeShareAlternate2));
            foreach (var horse in this.HorseList)
            {
                if (shareSum > 0)
                {
                    horse.StakeShareAlternate2 /= shareSum;
                }
            }
        }

        #endregion

        #region Tvilling and Trio

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTCombinationListInfo CombinationListInfoTvilling { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTCombinationListInfo CombinationListInfoTrio { get; set; }

        #endregion

        public void FindHeadToHead()
        {
            IEnumerable<List<HPTHorseResult>> horseResultList = this.HorseList
                .SelectMany(h => h.ResultList)
                .GroupBy(hr => new { hr.Date, hr.RaceNr, hr.TrackCode })
                .Where(g => g.Count() > 1)
                .Select(g => g.ToList());

            foreach (var resultList in horseResultList)
            {
                foreach (var result in resultList)
                {
                    result.HeadToHeadResultList = resultList.Where(r => r != result).ToList();
                }
            }

            foreach (var horse in this.HorseList)
            {
                horse.NumberOfHeadToHeadRaces = horse.ResultList.Count(r => r.HeadToHeadResultList != null);    // Antal lopp där det finns jämförelser

                horse.NumberOfHeadToHeadResults = horse.ResultList  // Totalt antal jämförelseresultat
                    .Where(r => r.HeadToHeadResultList != null)
                    .SelectMany(r => r.HeadToHeadResultList)
                    .Count();

                horse.NumberOfHeadToHeadWins = 0;
                horse.NumberOfHeadToHeadLosses = 0;
                horse.NumberOfHeadToHeadEqual = 0;

                foreach (var horseResult in horse.ResultList.Where(r => r.HeadToHeadResultList != null))
                {
                    horse.NumberOfHeadToHeadWins += horseResult.HeadToHeadResultList.Count(r => horseResult.Place < r.Place);
                    horse.NumberOfHeadToHeadLosses += horseResult.HeadToHeadResultList.Count(r => horseResult.Place > r.Place);
                    horse.NumberOfHeadToHeadEqual += horseResult.HeadToHeadResultList.Count(r => horseResult.Place == r.Place);
                }
                horse.HasHeadToHeadResults = horse.NumberOfHeadToHeadWins > 0 || horse.NumberOfHeadToHeadLosses > 0 || horse.NumberOfHeadToHeadEqual > 0;
            }

            //for (int i = 0; i < this.HorseList.Count - 1; i++)
            //{
            //    HPTHorse horse1 = this.HorseList[i];
            //    for (int j = i + 1; j < this.HorseList.Count; j++)
            //    {
            //        HPTHorse horse2 = this.HorseList[j];
            //        foreach (HPTHorseResult result1 in horse1.ResultList)
            //        {
            //            foreach (HPTHorseResult result2 in horse2.ResultList)
            //            {
            //                if (result1.Date == result2.Date && result1.TrackCode == result2.TrackCode && result1.RaceNr == result2.RaceNr)
            //                {
            //                    result1.HeadToHeadResultList.Add(result2);
            //                    result2.HeadToHeadResultList.Add(result1);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        internal HPTHorse GetHorseByNumber(int startNr)
        {
            return this.HorseList.Where(h => h.StartNr == startNr).FirstOrDefault();
        }

        internal HPTHorse GetHorseByName(string name)
        {
            return this.HorseList.Where(h => h.HorseName == name).FirstOrDefault();
        }

        //#region Result properties

        private HPTLegResult legResult;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTLegResult LegResult
        {
            get
            {
                return this.legResult;
            }
            set
            {
                this.legResult = value;
                OnPropertyChanged("LegResult");
            }
        }

        private bool hasResult;
        [XmlIgnore]
        public bool HasResult
        {
            get
            {
                return this.hasResult;
            }
            set
            {
                this.hasResult = value;
                OnPropertyChanged("HasResult");
            }
        }

        private bool splitVictory;
        [DataMember]
        public bool SplitVictory
        {
            get
            {
                return this.splitVictory;
            }
            set
            {
                this.splitVictory = value;
                OnPropertyChanged("SplitVictory");
            }
        }

        private string atgResultLink;
        [DataMember]
        public string ATGResultLink
        {
            get
            {
                return this.atgResultLink;
            }
            set
            {
                this.atgResultLink = value;
                OnPropertyChanged("ATGResultLink");
            }
        }

        //public string ATGResultLink { get; set; }
    }
}

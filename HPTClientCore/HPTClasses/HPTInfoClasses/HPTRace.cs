using ATGDownloader;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTRace : Notifier, IHorseListContainer
    {
        // TEST
        internal ATGRaceBase race;

        #region egna events

        public event Action<int, int, bool> NumberOfSelectedChanged;

        public event Action<HPTHorse, EventArgs> TrioSelectionChanged;

        public event Action<HPTRace, bool> SetAllSelected;

        public event Action<HPTRace> ClearABCD;

        #endregion

        public HPTRace()
        {
            HorseList = new List<HPTHorse>();
        }

        public HPTRace(ATGRaceBase race, HPTRaceDayInfo raceDayInfo)
        {
            //this.HorseList = new ObservableCollection<HPTHorse>();
            ParentRaceDayInfo = raceDayInfo;
            this.race = race;
        }

        public void Merge(ATGRaceBase race)
        {
            // TODO: Använd ATGRaceBase istället
            //try
            //{
            //    if (race.SharedInfo.RaceNr == RaceNr)
            //    {
            //        TurnoverPlats = race.SharedInfo.TurnoverPlats > 0 ? race.SharedInfo.TurnoverPlats : TurnoverPlats;
            //        TurnoverTvilling = race.SharedInfo.TurnoverTvilling > 0 ? race.SharedInfo.TurnoverTvilling : TurnoverTvilling;
            //        TurnoverVinnare = race.SharedInfo.TurnoverVinnare > 0 ? race.SharedInfo.TurnoverVinnare : TurnoverVinnare;

            //        NumberOfStartingHorses = 0;
            //        foreach (HPTService.HPTHorse horse in race.HorseList)
            //        {
            //            var hptHorse = GetHorseByNumber((int)horse.StartNr);
            //            hptHorse.Merge(horse);
            //            hptHorse.SetColors();
            //        }
            //        switch (ParentRaceDayInfo.BetType.Code)
            //        {
            //            case "V4":
            //            case "V5":
            //            case "V64":
            //            case "V65":
            //            case "V75":
            //            case "V85":
            //            case "GS75":
            //            case "V86":
            //                break;
            //            case "T":
            //                CombinationListInfoTrio.CalculatePercentages(HorseList);
            //                break;
            //            //case "TV":
            //            //    this.CombinationListInfoTvilling.CalculatePercentages(this.HorseList);
            //            //    break;
            //            case "DD":
            //            case "LD":
            //                // Hantera eventuellt strukna hästar
            //                HorseList
            //                    .Where(h => h.Selected && h.Scratched == true)
            //                    .ToList()
            //                    .ForEach(h => h.Selected = false);
            //                break;
            //            default:
            //                break;
            //        }

            //        //PerformCalculations();
            //        SetCorrectStakeDistributionShare();
            //        SetCorrectStakeDistributionShareAlt1();
            //        SetCorrectStakeDistributionShareAlt2();

            //        if (race.SharedInfo.TvillingCombinationList != null && CombinationListInfoTvilling.CombinationList != null)
            //        {
            //            // Hantera eventuellt strukna hästar
            //            var selectedScratchedHorses = HorseList.Where(h => h.Selected && h.Scratched == true).ToList();
            //            selectedScratchedHorses.ForEach(h =>
            //                {
            //                    h.Selected = false;
            //                    CombinationListInfoTvilling.CombinationList
            //                        .Where(c => c.Horse1 == h || c.Horse2 == h)
            //                        .ToList()
            //                        .ForEach(c =>
            //                        {
            //                            c.Selected = false;
            //                            c.Stake = null;
            //                        });
            //                });

            //            // Uppdatera alla kombinationer
            //            foreach (HPTService.HPTCombination comb in race.SharedInfo.TvillingCombinationList)
            //            {
            //                var hptComb = CombinationListInfoTvilling.CombinationList.First(c => c.UniqueCode == CombinationListInfoTvilling.GetUniqueCodeFromCombination(comb));
            //                hptComb.CombinationOdds = comb.CombinationOdds;
            //                hptComb.CombinationOddsExact = comb.CombinationOddsExact;
            //                hptComb.CalculateQuotas("TV");
            //            }
            //            CombinationListInfoTvilling.SortCombinationValues();
            //        }

            //        if (race.SharedInfo.TrioCombinationList != null && CombinationListInfoTrio.CombinationList != null)
            //        {
            //            CreateAllTrioCombinations(race.SharedInfo.TrioCombinationList);
            //            CombinationListInfoTrio.SortCombinationValues();
            //        }
            //    }
            //}
            //catch (Exception exc)
            //{
            //    string s = exc.Message;
            //}
        }

        public void ConvertRace(ATGRaceBase race)
        {
            // TODO: Använd ATGRaceBase istället
            //Distance = race.SharedInfo.Distance;
            //StartMethodCode = race.SharedInfo.StartMethodCode;
            //PostTime = race.SharedInfo.PostTime;

            //Regex rexExtractNumber = new Regex("\\d+");
            //if (!string.IsNullOrEmpty(Distance) && rexExtractNumber.IsMatch(Distance))
            //{
            //    Match m = rexExtractNumber.Match(Distance);
            //    int iDistance = int.Parse(m.Value);
            //    if (iDistance < 1800)
            //    {
            //        DistanceCode = "K";
            //    }
            //    else if (iDistance > 2500)
            //    {
            //        DistanceCode = "L";
            //    }
            //    else
            //    {
            //        DistanceCode = "M";
            //    }

            //    if (StartMethodCode == "A")
            //    {
            //        StartMethodAndDistanceCode = StartMethodCode + DistanceCode;
            //    }
            //    else
            //    {
            //        StartMethodAndDistanceCode = DistanceCode;
            //    }
            //}
            //else
            //{
            //    DistanceCode = string.Empty;
            //}

            //RaceNr = race.SharedInfo.RaceNr;
            //LegNr = race.LegNr;
            //RaceName = race.SharedInfo.RaceName;
            //RaceInfoShort = race.SharedInfo.RaceInfoShort.Replace("\n", string.Empty);
            //RaceInfoLong = race.SharedInfo.RaceInfoLong;
            //RaceShortText = "Lopp " + RaceNr.ToString();
            ////this.ReservOrder = race.ReservOrder;

            //ReservOrderList = new int[0];
            ////if (this.ReservOrder != string.Empty)
            ////{
            ////    string[] reservStringArray = this.ReservOrder.Split('-');
            ////    this.ReservOrderList = new int[reservStringArray.Length];
            ////    for (int i = 0; i < reservStringArray.Length; i++)
            ////    {
            ////        this.ReservOrderList[i] = int.Parse(reservStringArray[i]);
            ////    }
            ////}

            //StartMethod = race.SharedInfo.StartMethod;
            //StartMethodCode = race.SharedInfo.StartMethodCode;
            //TurnoverPlats = race.SharedInfo.TurnoverPlats;
            //TurnoverTvilling = race.SharedInfo.TurnoverTvilling;
            //TurnoverVinnare = race.SharedInfo.TurnoverVinnare;
            //TrackId = race.TrackId;

            //if (race.HorseList != null)
            //{
            //    //this.HorseList = new ObservableCollection<HPTHorse>();
            //    HorseList = new List<HPTHorse>();

            //    Parallel.ForEach(race.HorseList, horse =>
            //    {
            //        var startNumberRankCollection = HPTConfig.Config.StartNumberRankCollectionList.FirstOrDefault(snr => snr.StartMethodCode == StartMethodCode);
            //        var hptHorse = new HPTHorse()
            //        {
            //            ParentRace = this
            //        };
            //        hptHorse.ConvertHorse(horse);
            //        HorseList.Add(hptHorse);
            //        hptHorse.CreateXReductionRuleList();

            //        // Sätt Spårrank utifrån Config
            //        if (startNumberRankCollection != null)
            //        {
            //            var startNumberRank = startNumberRankCollection.StartNumberRankList.FirstOrDefault(sr => sr.StartNumber == hptHorse.StartNr);
            //            if (startNumberRank != null && hptHorse.Scratched != true)
            //            {
            //                hptHorse.Selected = startNumberRank.Select;
            //                hptHorse.RankStartNumber = startNumberRank.Rank;
            //            }
            //        }
            //    });

            //    //foreach (HPTService.HPTHorse horse in race.HorseList)
            //    //{
            //    //    var startNumberRankCollection = HPTConfig.Config.StartNumberRankCollectionList.FirstOrDefault(snr => snr.StartMethodCode == this.StartMethodCode);
            //    //    var hptHorse = this.HorseList.FirstOrDefault(h => h.StartNr == horse.StartInfo.StartNr);
            //    //    if (hptHorse == null)
            //    //    {
            //    //        hptHorse = new HPTHorse();
            //    //        hptHorse.ParentRace = this;
            //    //        hptHorse.ConvertHorse(horse);
            //    //        this.HorseList.Add(hptHorse);
            //    //        hptHorse.CreateXReductionRuleList();
            //    //    }

            //    //    // Sätt Spårrank utifrån Config
            //    //    if (startNumberRankCollection != null)
            //    //    {
            //    //        var startNumberRank = startNumberRankCollection.StartNumberRankList.FirstOrDefault(sr => sr.StartNumber == hptHorse.StartNr);
            //    //        if (startNumberRank != null && hptHorse.Scratched != true)
            //    //        {
            //    //            hptHorse.Selected = startNumberRank.Select;
            //    //            hptHorse.RankStartNumber = startNumberRank.Rank;
            //    //        }
            //    //    }
            //    //}

            //    // Sätt ATG-rank utifrån reservordning
            //    for (int i = 0; i < ReservOrderList.Length; i++)
            //    {
            //        HPTHorse hptHorse = GetHorseByNumber(ReservOrderList[i]);
            //        hptHorse.RankATG = i + 1;
            //    }

            //    SetCorrectStakeDistributionShare();
            //    SetCorrectStakeDistributionShareAlt1();
            //    SetCorrectStakeDistributionShareAlt2();
            //}

            //if (ParentRaceDayInfo == null)
            //{
            //    return;
            //}

            //// Tvilling
            //if (ParentRaceDayInfo.BetType.Code == "TV")
            //{
            //    CombinationListInfoTvilling = new HPTCombinationListInfo()
            //    {
            //        CombinationList = new List<HPTCombination>(),
            //        BetType = ParentRaceDayInfo.BetType.Code
            //    };

            //    if (race.SharedInfo.TvillingCombinationList != null && race.SharedInfo.TvillingCombinationList.Length > 0)
            //    {
            //        foreach (HPTService.HPTCombination comb in race.SharedInfo.TvillingCombinationList)
            //        {
            //            HPTCombination hptComb = new HPTCombination();
            //            hptComb.ParentRace = this;
            //            hptComb.ParentRaceDayInfo = ParentRaceDayInfo;
            //            hptComb.CombinationOdds = comb.CombinationOdds;
            //            hptComb.Horse1Nr = comb.Horse1Nr;
            //            hptComb.Horse2Nr = comb.Horse2Nr;
            //            hptComb.Horse3Nr = comb.Horse3Nr;

            //            hptComb.Horse1 = GetHorseByNumber(comb.Horse1Nr);
            //            hptComb.Horse2 = GetHorseByNumber(comb.Horse2Nr);

            //            hptComb.CalculateQuotas("TV");
            //            CombinationListInfoTvilling.CombinationList.Add(hptComb);
            //        }
            //        CombinationListInfoTvilling.SortCombinationValues();
            //    }
            //    else
            //    {
            //        for (int horse1 = 0; horse1 < HorseList.Count - 1; horse1++)
            //        {
            //            //HPTHorse hptHorse1 = this.HorseList[horse1];
            //            HPTHorse hptHorse1 = HorseList.ElementAt(horse1);
            //            for (int horse2 = horse1 + 1; horse2 < HorseList.Count; horse2++)
            //            {
            //                //HPTHorse hptHorse2 = this.HorseList[horse2];
            //                HPTHorse hptHorse2 = HorseList.ElementAt(horse2);
            //                HPTCombination hptComb = new HPTCombination();
            //                hptComb.ParentRace = this;
            //                hptComb.ParentRaceDayInfo = ParentRaceDayInfo;
            //                hptComb.CombinationOdds = 0M;
            //                hptComb.Horse1Nr = hptHorse1.StartNr;
            //                hptComb.Horse2Nr = hptHorse2.StartNr;

            //                hptComb.Horse1 = hptHorse1;
            //                hptComb.Horse2 = hptHorse2;

            //                hptComb.CalculateQuotas("TV");
            //                CombinationListInfoTvilling.CombinationList.Add(hptComb);
            //            }
            //        }
            //    }
            //    foreach (var horse in HorseList)
            //    {
            //        horse.OwnProbability = horse.StakeShareRounded;
            //    }
            //}

            //// Trio
            //if (ParentRaceDayInfo.BetType.Code == "T")
            //{
            //    CombinationListInfoTrio = new HPTCombinationListInfo()
            //    {
            //        CombinationList = new List<HPTCombination>(),
            //        BetType = ParentRaceDayInfo.BetType.Code
            //    };
            //    CombinationListInfoTrio.CalculatePercentages(HorseList);
            //    CreateAllTrioCombinations(race.SharedInfo.TrioCombinationList);
            //    CombinationListInfoTrio.SortCombinationValues();
            //}

            // Inbördes möte hästar emellan
            FindHeadToHead();
        }


        //internal void CreateAllTrioCombinations(HPTService.HPTCombination[] combArray)
        //{
        //    // Beräkna äkta insatsfördelning per häst
        //    decimal turnoverTrio1 = HorseList.Sum(h => h.TrioInfo.PlaceInfo1.Investment);
        //    decimal turnoverTrio2 = HorseList.Sum(h => h.TrioInfo.PlaceInfo2.Investment);
        //    decimal turnoverTrio3 = HorseList.Sum(h => h.TrioInfo.PlaceInfo3.Investment);
        //    if (turnoverTrio1 > 0M && turnoverTrio2 > 0M && turnoverTrio3 > 0M)
        //    {
        //        HorseList.ToList().ForEach(h =>
        //            {
        //                h.TrioInfo.PlaceInfo1.InvestmentShare = h.TrioInfo.PlaceInfo1.Investment / turnoverTrio1;
        //                h.TrioInfo.PlaceInfo2.InvestmentShare = h.TrioInfo.PlaceInfo2.Investment / turnoverTrio2;
        //                h.TrioInfo.PlaceInfo3.InvestmentShare = h.TrioInfo.PlaceInfo3.Investment / turnoverTrio3;
        //            });
        //    }

        //    // Hantera eventuellt strukna hästar
        //    var selectedScratchedHorses = HorseList.Where(h => h.Selected && h.Scratched == true).ToList();
        //    selectedScratchedHorses.ForEach(h =>
        //    {
        //        h.Selected = false;
        //        CombinationListInfoTrio.CombinationList
        //            .Where(c => c.Horse1 == h || c.Horse2 == h || c.Horse3 == h)
        //            .ToList()
        //            .ForEach(c =>
        //            {
        //                c.Stake = null;
        //            });
        //    });

        //    int maxOdds = combArray.Max(c => c.CombinationOdds) + 1;
        //    foreach (var horse1 in HorseList.Where(h => h.Scratched == false || h.Scratched == null))
        //    {
        //        foreach (var horse2 in HorseList.Where(h => (h.Scratched == false || h.Scratched == null) && h.StartNr != horse1.StartNr))
        //        {
        //            foreach (var horse3 in HorseList.Where(h => (h.Scratched == false || h.Scratched == null) && h.StartNr != horse1.StartNr && h.StartNr != horse2.StartNr))
        //            {
        //                var serviceComb = combArray.FirstOrDefault(c => c.Horse1Nr == horse1.StartNr && c.Horse2Nr == horse2.StartNr && c.Horse3Nr == horse3.StartNr);
        //                var hptComb = CombinationListInfoTrio.CombinationList.FirstOrDefault(c => c.Horse1Nr == horse1.StartNr && c.Horse2Nr == horse2.StartNr && c.Horse3Nr == horse3.StartNr);

        //                if (hptComb == null)
        //                {
        //                    hptComb = new HPTCombination()
        //                    {
        //                        ParentRace = this,
        //                        ParentRaceDayInfo = ParentRaceDayInfo,
        //                        Horse1Nr = horse1.StartNr,
        //                        Horse2Nr = horse2.StartNr,
        //                        Horse3Nr = horse3.StartNr,
        //                        Horse1 = horse1,
        //                        Horse2 = horse2,
        //                        Horse3 = horse3,
        //                        CombinationOdds = serviceComb.CombinationOdds,
        //                        CombinationOddsExact = serviceComb.CombinationOddsExact
        //                    };
        //                    hptComb.CalculateQuotas("T");
        //                    CombinationListInfoTrio.CombinationList.Add(hptComb);
        //                }
        //                if (serviceComb != null)
        //                {
        //                    hptComb.CombinationOdds = serviceComb.CombinationOdds;
        //                    hptComb.CombinationOddsExact = serviceComb.CombinationOddsExact;
        //                }
        //                else
        //                {
        //                    hptComb.CombinationOdds = maxOdds;
        //                    hptComb.CombinationOddsExact = maxOdds;
        //                }
        //            }
        //        }
        //    }
        //}

        //internal void SetHorseTrioInfoValues()
        //{
        //    HorseList
        //        .Where(h => h.Scratched == false || h.Scratched == null)
        //        .ToList()
        //        .ForEach(h =>
        //        {
        //            h.TrioInfo = new HPTHorseTrioInfo()
        //            {
        //                PlaceInfo1 = new HPTHorseTrioPlaceInfo()
        //                {
        //                    //Investment = this.tu
        //                }
        //            };
        //        });
        //}

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
                if (TrackId == null || TrackId == 0)
                {
                    if (ParentRaceDayInfo != null)
                    {
                        return ParentRaceDayInfo.Trackname;
                    }
                    return null;
                }
                return EnumHelper.GetTrackNameFromTrackId(Convert.ToInt32(TrackId));
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
                if (TrackId == null || TrackId == 0)
                {
                    return null;
                }
                return EnumHelper.GetTrackCodeFromTrackId(Convert.ToInt32(TrackId));
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
                return updatedPostTime;
            }
            set
            {
                updatedPostTime = value;
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
                turnoverPlats = value;
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
                turnoverVinnare = value;
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
                turnoverTvilling = value;
                OnPropertyChanged("TurnoverTvilling");
                if (value > 0M)
                {
                    TurnoverCombination = (int)value;
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
                turnoverTrio = value;
                OnPropertyChanged("TurnoverTrio");
                if (value > 0M)
                {
                    TurnoverCombination = (int)value;
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
                turnoverCombination = value;
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
                reserv1Nr = value;
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
                reserv2Nr = value;
                OnPropertyChanged("Reserv2Nr");
            }
        }


        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public int Reserv2Nr { get; set; }

        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public string ReservOrder { get; set; }

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
                if (reservOrderList == null)
                {
                    reservOrderList = HorseList
                        .OrderByDescending(h => h.StakeDistribution)
                        .Select(h => h.StartNr)
                        .ToArray();
                }
                return reservOrderList;
            }
            set
            {
                reservOrderList = value;
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
                if (horseListSelected == null)
                {
                    horseListSelected = HorseList.Where(h => h.Selected == true).ToList();
                    //this.horseListSelected = this.HorseList.Where(h => h.Selected == true).ToList();
                    //this.NumberOfSelectedHorses = this.horseListSelected.Count;
                }
                return horseListSelected;
            }
            set
            {
                horseListSelected = value;
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
                return locked;
            }
            set
            {
                locked = value;
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
                sb.Append(LegNrString);
                sb.Append(" (");
                sb.Append(PostTime.ToString("HH:mm"));
                sb.Append(") :");
                //sb.Append(this.RaceName);
                //sb.Append(" - ");
                sb.Append(RaceInfoShort);
                if (Reserv1Nr != 0)
                {
                    sb.Append(" (R1:");
                    sb.Append(Reserv1Nr);
                    if (Reserv2Nr != 0)
                    {
                        sb.Append(", R2:");
                        sb.Append(Reserv2Nr);
                    }
                    sb.Append(")");
                }
                return sb.ToString();
            }
        }

        public string ToClipboardString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ClipboardString);
            foreach (HPTHorse horse in HorseListSelected)
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
            var startNumberListWithPrio = HorseListSelected
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
            int numberOfX = HorseListSelected.Count(h => h.Prio == prio);
            return numberOfX;
        }

        public void SetTrioSelectionChanged(HPTHorse horse)
        {
            if (TrioSelectionChanged != null)
            {
                TrioSelectionChanged(horse, new EventArgs());
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
            if (NumberOfSelectedChanged != null)
            {
                //this.NumberOfSelectedChanged(horse, new EventArgs());
                NumberOfSelectedChanged(legNr, startNr, selected);
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
            if (SetAllSelected != null)
            {
                SetAllSelected(this, selected);
            }
        }

        public void ClearABCDRace()
        {
            if (ClearABCD != null)
            {
                ClearABCD(this);
            }
        }

        private int noSelected;
        [XmlIgnore]
        public int NumberOfSelectedHorses
        {
            get
            {
                if (noSelected == 0)
                {
                    horseListSelected = HorseList.Where(h => h.Selected == true).ToList();
                    noSelected = horseListSelected.Count;
                }
                return noSelected;
            }
            set
            {
                noSelected = value;
                OnPropertyChanged("NumberOfSelectedHorses");
            }
        }

        private int numberOfStartingHorses;
        [XmlIgnore]
        public int NumberOfStartingHorses
        {
            get
            {
                if (numberOfStartingHorses == 0)
                {
                    numberOfStartingHorses = HorseList.Count(h => h.Scratched == false || h.Scratched == null);
                }
                return numberOfStartingHorses;
            }
            set
            {
                numberOfStartingHorses = value;
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
            switch (ParentRaceDayInfo.BetType.Code)
            {
                case "V64":
                case "V65":
                case "V75":
                case "GS75":
                case "V85":
                case "V86":
                    decimal shareSum = HorseList.Sum(h => h.StakeDistributionShare);
                    decimal shareSumWithoutScratchings = HorseList.Where(h => h.Scratched != true).Sum(h => h.StakeDistributionShare);
                    decimal shareSumWithoutScratchingsQuota = 1M;
                    if (shareSum > 0)
                    {
                        shareSumWithoutScratchingsQuota = shareSumWithoutScratchings / shareSum;
                    }
                    foreach (var horse in HorseList)
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
                    decimal shareSumWithoutScratchingsVx = HorseList.Where(h => h.Scratched != true).Sum(h => h.StakeDistributionShare);
                    foreach (var horse in HorseList)
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
            foreach (var horse in HorseList)
            {
                horse.StakeDistributionShareAccumulated = HorseList.Where(h => h.StakeDistributionShare >= horse.StakeDistributionShare).Sum(h => h.StakeDistributionShare);
            }
        }

        internal void SetCorrectStakeDistributionShareAlt1()
        {
            // Rätta till alternativ insatsfördelning 1
            decimal shareSum = HorseList.Sum(h => Convert.ToDecimal(h.StakeShareAlternate));
            foreach (var horse in HorseList)
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
            decimal shareSum = HorseList.Sum(h => Convert.ToDecimal(h.StakeShareAlternate2));
            foreach (var horse in HorseList)
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
            return;
            // TODO: Så småningom kan det här vara bra att ha...
            IEnumerable<List<HPTHorseResult>> horseResultList = HorseList
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

            foreach (var horse in HorseList)
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
            return HorseList.Where(h => h.StartNr == startNr).FirstOrDefault();
        }

        internal HPTHorse GetHorseByName(string name)
        {
            return HorseList.Where(h => h.HorseName == name).FirstOrDefault();
        }

        //#region Result properties

        private HPTLegResult legResult;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTLegResult LegResult
        {
            get
            {
                return legResult;
            }
            set
            {
                legResult = value;
                OnPropertyChanged("LegResult");
            }
        }

        private bool hasResult;
        [XmlIgnore]
        public bool HasResult
        {
            get
            {
                return hasResult;
            }
            set
            {
                hasResult = value;
                OnPropertyChanged("HasResult");
            }
        }

        private bool splitVictory;
        [DataMember]
        public bool SplitVictory
        {
            get
            {
                return splitVictory;
            }
            set
            {
                splitVictory = value;
                OnPropertyChanged("SplitVictory");
            }
        }

        private string atgResultLink;
        [DataMember]
        public string ATGResultLink
        {
            get
            {
                return atgResultLink;
            }
            set
            {
                atgResultLink = value;
                OnPropertyChanged("ATGResultLink");
            }
        }

        //public string ATGResultLink { get; set; }
    }
}

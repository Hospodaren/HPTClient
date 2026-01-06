using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCombBet : HPTBet
    {
        public HPTCombBet()
        {
        }

        public HPTCombBet(HPTRaceDayInfo rdi, HPTBetType bt)
            : base(rdi, bt)
        {
            RaceDayInfo.RankTemplateChanged -= ApplyConfigRankVariables;
            RaceDayInfo.RankTemplateChanged += ApplyConfigRankVariables;
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            RaceDayInfo.RankTemplateChanged -= ApplyConfigRankVariables;
            RaceDayInfo.RankTemplateChanged += ApplyConfigRankVariables;
        }

        internal override void ApplyConfigRankVariables(HPTRankTemplate rankTemplate)
        {
            base.ApplyConfigRankVariables(rankTemplate);
            RecalculateAllRanks();
            RecalculateRank();
        }

        [XmlIgnore]
        public HPTCombinationDataToShow DataToShow { get; set; }

        public void SetStakeAndNumberOfSelected()
        {
            int numberOfSelected = 0;
            int totalStake = 0;
            switch (BetType.Code)
            {
                case "DD":
                case "LD":
                    totalStake = RaceDayInfo.CombinationListInfoDouble.CombinationList.Where(c => c.Selected && c.Stake != null).Sum(c => (int)c.Stake);
                    numberOfSelected = RaceDayInfo.CombinationListInfoDouble.CombinationList.Count(c => c.Selected);
                    break;
                case "TV":
                    foreach (HPTRace hptRace in RaceDayInfo.RaceList)
                    {
                        hptRace.CombinationListInfoTvilling.SetStakeAndNumberOfSelected();
                    }
                    numberOfSelected = RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTvilling.NumberOfSelectedCombinations);
                    totalStake = RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTvilling.TotalStake);
                    break;
                case "T":
                    foreach (HPTRace hptRace in RaceDayInfo.RaceList)
                    {
                        hptRace.CombinationListInfoTrio.SetStakeAndNumberOfSelected();
                    }
                    numberOfSelected = RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTrio.NumberOfSelectedCombinations);
                    totalStake = RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTrio.TotalStake);
                    break;
                default:
                    break;
            }

            NumberOfSelected = numberOfSelected;
            TotalStake = totalStake;
        }

        internal void CalculateStake()
        {
            IEnumerable<HPTCombination> combList = null;
            switch (RaceDayInfo.BetType.Code)
            {
                case "DD":
                case "LD":
                    combList = RaceDayInfo.CombinationListInfoDouble.CombinationList.Where(c => c.Selected);
                    break;
                case "T":
                    combList = RaceDayInfo.RaceList
                        .SelectMany(r => r.CombinationListInfoTrio.CombinationList)
                        .Where(c => c.Selected);
                    break;
                case "TV":
                    combList = RaceDayInfo.RaceList
                        .SelectMany(r => r.CombinationListInfoTvilling.CombinationList)
                        .Where(c => c.Selected);
                    break;
                default:
                    break;
            }
            if (combList != null)
            {
                foreach (HPTCombination comb in combList)
                {
                    CalculateStake(comb);
                    if (comb.Stake < BetType.LowestStake)
                    {
                        comb.Stake = BetType.LowestStake;
                    }
                }
                SetStakeAndNumberOfSelected();
            }
        }

        internal void CalculateStake(HPTCombination comb)
        {
            if (comb.CombinationOdds == 0)
            {
                comb.Stake = 0;
                return;
            }
            decimal targetBet = TargetReturn / comb.CombinationOdds * 10M;
            comb.Stake = Convert.ToInt32(targetBet);
        }

        private int numberOfSelected;
        [DataMember]
        public int NumberOfSelected
        {
            get
            {
                return numberOfSelected;
            }
            set
            {
                numberOfSelected = value;
                OnPropertyChanged("NumberOfSelected");
            }
        }

        private int totalStake;
        [DataMember]
        public int TotalStake
        {
            get
            {
                return totalStake;
            }
            set
            {
                totalStake = value;
                OnPropertyChanged("TotalStake");
            }
        }

        private int targetReturn;
        [DataMember]
        public int TargetReturn
        {
            get
            {
                return targetReturn;
            }
            set
            {
                targetReturn = value;
                OnPropertyChanged("TargetReturn");
                CalculateStake();
            }
        }

        private static SortedList<int, int> stakeIndexList;
        internal static SortedList<int, int> StakeIndexList
        {
            get
            {
                if (stakeIndexList == null)
                {
                    stakeIndexList = new SortedList<int, int>();
                    stakeIndexList.Add(5, 0);
                    stakeIndexList.Add(10, 1);
                    stakeIndexList.Add(20, 2);
                    stakeIndexList.Add(30, 3);
                    stakeIndexList.Add(40, 4);
                    stakeIndexList.Add(50, 5);
                    stakeIndexList.Add(100, 6);
                    stakeIndexList.Add(200, 7);
                    stakeIndexList.Add(500, 8);
                    stakeIndexList.Add(1000, 9);
                }
                return stakeIndexList;
            }
        }

        #region ToString-versions

        public string ToFileNameString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BetType.Code);
            sb.Append("_");
            sb.Append(RaceDayInfo.TracknameFile);
            sb.Append("_");
            sb.Append(RaceDayInfo.TrackId);
            sb.Append("_");
            sb.Append(RaceDayInfo.RaceDayDateString);

            return sb.ToString();
        }

        public string ToFileNameString(HPTRace hptRace, HPTCombinationListInfo combListInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BetType.Name);
            sb.Append("_");
            sb.Append(RaceDayInfo.TracknameFile);
            sb.Append("_");
            sb.Append(RaceDayInfo.TrackId);
            sb.Append("_");
            sb.Append(RaceDayInfo.RaceDayDateString);
            sb.Append("_");
            sb.Append(hptRace.LegNrString);
            sb.Append("_");
            sb.Append(combListInfo.NumberOfSelectedCombinations);
            sb.Append("_");
            sb.Append(combListInfo.TotalStake);

            return sb.ToString();
        }

        public string ToTrioFileNameString(HPTRace hptRace)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("T");
            sb.Append("_");
            sb.Append(RaceDayInfo.Trackname);
            sb.Append("_");
            sb.Append(RaceDayInfo.TrackId);
            sb.Append("_");
            sb.Append(RaceDayInfo.RaceDayDateString);
            sb.Append("_");
            sb.Append(hptRace.RaceNr);
            //sb.Append("_");
            //sb.Append(hptRace.NumberOfSelectedCombinationsTr);
            sb.Append("_");
            sb.Append(hptRace.CombinationListInfoTrio.TotalStake);

            return sb.ToString();
        }

        public string ToClipboardString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(this.ClipboardString);

            //foreach (HPTRace race in this.RaceDayInfo.RaceList)
            //{
            //    sb.AppendLine();
            //    sb.Append(race.ToClipboardString());
            //}

            return sb.ToString();
        }

        public string ToClipboardString(HPTRace hptRace)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(this.ClipboardString);

            //foreach (HPTRace race in this.RaceDayInfo.RaceList)
            //{
            //    sb.AppendLine();
            //    sb.Append(race.ToClipboardString());
            //}

            return sb.ToString();
        }

        #endregion        
    }
}

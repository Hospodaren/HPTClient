using System;
using System.Collections.Generic;
using System.Linq;
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
            this.RaceDayInfo.RankTemplateChanged -= ApplyConfigRankVariables;
            this.RaceDayInfo.RankTemplateChanged += ApplyConfigRankVariables;
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            this.RaceDayInfo.RankTemplateChanged -= ApplyConfigRankVariables;
            this.RaceDayInfo.RankTemplateChanged += ApplyConfigRankVariables;
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
            switch (this.BetType.Code)
            {
                case "DD":
                case "LD":
                    totalStake = this.RaceDayInfo.CombinationListInfoDouble.CombinationList.Where(c => c.Selected && c.Stake != null).Sum(c => (int)c.Stake);
                    numberOfSelected = this.RaceDayInfo.CombinationListInfoDouble.CombinationList.Count(c => c.Selected);
                    break;
                case "TV":
                    foreach (HPTRace hptRace in this.RaceDayInfo.RaceList)
                    {
                        hptRace.CombinationListInfoTvilling.SetStakeAndNumberOfSelected();
                    }
                    numberOfSelected = this.RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTvilling.NumberOfSelectedCombinations);
                    totalStake = this.RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTvilling.TotalStake);
                    break;
                case "T":
                    foreach (HPTRace hptRace in this.RaceDayInfo.RaceList)
                    {
                        hptRace.CombinationListInfoTrio.SetStakeAndNumberOfSelected();
                    }
                    numberOfSelected = this.RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTrio.NumberOfSelectedCombinations);
                    totalStake = this.RaceDayInfo.RaceList.Sum(r => r.CombinationListInfoTrio.TotalStake);
                    break;
                default:
                    break;
            }

            this.NumberOfSelected = numberOfSelected;
            this.TotalStake = totalStake;
        }

        internal void CalculateStake()
        {
            IEnumerable<HPTCombination> combList = null;
            switch (this.RaceDayInfo.BetType.Code)
            {
                case "DD":
                case "LD":
                    combList = this.RaceDayInfo.CombinationListInfoDouble.CombinationList.Where(c => c.Selected);
                    break;
                case "T":
                    combList = this.RaceDayInfo.RaceList
                        .SelectMany(r => r.CombinationListInfoTrio.CombinationList)
                        .Where(c => c.Selected);
                    break;
                case "TV":
                    combList = this.RaceDayInfo.RaceList
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
                    if (comb.Stake < this.BetType.LowestStake)
                    {
                        comb.Stake = this.BetType.LowestStake;
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
            decimal targetBet = this.TargetReturn / comb.CombinationOdds * 10M;
            comb.Stake = Convert.ToInt32(targetBet);
        }

        private int numberOfSelected;
        [DataMember]
        public int NumberOfSelected
        {
            get
            {
                return this.numberOfSelected;
            }
            set
            {
                this.numberOfSelected = value;
                OnPropertyChanged("NumberOfSelected");
            }
        }

        private int totalStake;
        [DataMember]
        public int TotalStake
        {
            get
            {
                return this.totalStake;
            }
            set
            {
                this.totalStake = value;
                OnPropertyChanged("TotalStake");
            }
        }

        private int targetReturn;
        [DataMember]
        public int TargetReturn
        {
            get
            {
                return this.targetReturn;
            }
            set
            {
                this.targetReturn = value;
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
            sb.Append(this.BetType.Code);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.TracknameFile);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.TrackId);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.RaceDayDateString);

            return sb.ToString();
        }

        public string ToFileNameString(HPTRace hptRace, HPTCombinationListInfo combListInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.BetType.Name);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.TracknameFile);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.TrackId);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.RaceDayDateString);
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
            sb.Append(this.RaceDayInfo.Trackname);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.TrackId);
            sb.Append("_");
            sb.Append(this.RaceDayInfo.RaceDayDateString);
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

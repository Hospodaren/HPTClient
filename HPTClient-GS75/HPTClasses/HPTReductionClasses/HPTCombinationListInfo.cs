using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCombinationListInfo : Notifier
    {
        #region Tvilling and Trio

        public HPTCombinationListInfo()
        {
            this.CombinationsToShowList = new ObservableCollection<HPTCombination>();
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            this.CombinationsToShowList = new ObservableCollection<HPTCombination>();
        }

        [XmlIgnore]
        public ObservableCollection<HPTCombination> CombinationsToShowList { get; set; }

        [DataMember]
        public List<HPTCombination> CombinationList { get; set; }

        [DataMember]
        public string BetType { get; set; }

        public void SetStakeAndNumberOfSelected()
        {
            int numberOfSelected = this.CombinationList.Where(c => c.Selected).Count();
            int totalStake = this.CombinationList.Where(c => c.Selected && c.Stake != null).Sum(c => (int)c.Stake);

            if (this.NumberOfSelectedCombinations != numberOfSelected)
            {
                this.NumberOfSelectedCombinations = numberOfSelected;
            }
            if (this.TotalStake != totalStake)
            {
                this.TotalStake = totalStake;
            }
        }

        private int numberOfSelectedCombinations;
        public int NumberOfSelectedCombinations
        {
            get
            {
                return this.numberOfSelectedCombinations;
            }
            set
            {
                this.numberOfSelectedCombinations = value;
                OnPropertyChanged("NumberOfSelectedCombinations");
            }
        }

        private int totalStake;
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

        public void SortCombinationValues()
        {
            if (this.CombinationList != null)
            {
                List<HPTCombination> orderedCombinations = this.CombinationList.OrderBy(c => c.CombinationOddsExact).ToList();
                for (int i = 0; i < orderedCombinations.Count(); i++)
                {
                    orderedCombinations[i].CombinationOddsRank = i + 1;
                }

                orderedCombinations = this.CombinationList.OrderBy(c => c.CombinationOddsExact).ToList();
                for (int i = 0; i < this.CombinationList.Count; i++)
                {
                    orderedCombinations[i].MultipliedOddsRank = i + 1;
                }
            }
        }

        private string GetHexCode(int number)
        {
            switch (number)
            {
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                case 16:
                    return "G";
                case 17:
                    return "H";
                case 18:
                    return "I";
                case 19:
                    return "J";
                case 20:
                    return "K";
                case 0:
                    return string.Empty;
                default:
                    return number.ToString();
            }
        }

        internal string GetUniqueCodeFromCombination(HPTService.HPTCombination comb)
        {
            string code = GetHexCode(comb.Horse1Nr) + GetHexCode(comb.Horse2Nr) + GetHexCode(comb.Horse3Nr);

            return code;
        }

        // Trio
        public int TotalPercentage { get; set; }

        // Trio
        public void CalculatePercentages(IEnumerable<HPTHorse> horseList)
        {
            this.TotalPercentage = horseList
                .Sum(h => h.TrioInfo.PlaceInfo1.Percent);

            //horseList.ToList().ForEach(h =>
            //{
            //    var horse = this.horsel
            //});

            if (this.TotalPercentage > 0)
            {
                //foreach (HPTHorse horse in horseList)
                //{
                //    horse.PercentFirstRelative = Convert.ToDecimal(horse.PercentFirst) / Convert.ToDecimal(this.TotalPercentage);
                //    horse.PercentSecondRelative = Convert.ToDecimal(horse.PercentSecond) / Convert.ToDecimal(this.TotalPercentage);
                //    horse.PercentThirdRelative = Convert.ToDecimal(horse.PercentThird) / Convert.ToDecimal(this.TotalPercentage);
                //}
            }

            //this.TotalPercentage = 0;
            //foreach (HPTHorse horse in horseList)
            //{
            //    this.TotalPercentage += (int)horse.PercentFirst;
            //}
            //if (this.TotalPercentage > 0)
            //{
            //    foreach (HPTHorse horse in horseList)
            //    {
            //        horse.PercentFirstRelative = Convert.ToDecimal(horse.PercentFirst) / Convert.ToDecimal(this.TotalPercentage);
            //        horse.PercentSecondRelative = Convert.ToDecimal(horse.PercentSecond) / Convert.ToDecimal(this.TotalPercentage);
            //        horse.PercentThirdRelative = Convert.ToDecimal(horse.PercentThird) / Convert.ToDecimal(this.TotalPercentage);
            //    }
            //}
        }

        public void UpdateCombinationsToShow()
        {
            try
            {
                this.CombinationsToShowList.Clear();
                IEnumerable<HPTCombination> combinationsToShow = new List<HPTCombination>();

                switch (this.BetType)
                {
                    case "T":
                        combinationsToShow = this.CombinationList
                        .Where(c => c.Horse1.TrioInfo.PlaceInfo1.Selected && c.Horse2.TrioInfo.PlaceInfo2.Selected && c.Horse3.TrioInfo.PlaceInfo3.Selected)
                        .ToList();
                        break;
                    case "DD":
                    case "LD":
                        combinationsToShow = this.CombinationList
                        .Where(c => c.Horse1.Selected && c.Horse2.Selected)
                        .ToList();
                        break;
                    case "TV":
                        combinationsToShow = this.CombinationList
                        .Where(c => c.Horse1.Selected || c.Horse2.Selected)
                        .ToList();
                        break;
                    default:
                        break;
                }

                foreach (var combinationToShow in combinationsToShow)
                {
                    this.CombinationsToShowList.Add(combinationToShow);
                }

                this.CombinationList
                    .Except(this.CombinationsToShowList)
                    .Where(c => c.Selected)
                    .ToList()
                    .ForEach(c => c.Selected = false);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        #endregion
    }
}

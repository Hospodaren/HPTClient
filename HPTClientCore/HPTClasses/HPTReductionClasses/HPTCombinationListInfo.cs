using System.Collections.ObjectModel;
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
            CombinationsToShowList = new ObservableCollection<HPTCombination>();
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            CombinationsToShowList = new ObservableCollection<HPTCombination>();
        }

        [XmlIgnore]
        public ObservableCollection<HPTCombination> CombinationsToShowList { get; set; }

        [DataMember]
        public List<HPTCombination> CombinationList { get; set; }

        [DataMember]
        public string BetType { get; set; }

        public void SetStakeAndNumberOfSelected()
        {
            int numberOfSelected = CombinationList.Where(c => c.Selected).Count();
            int totalStake = CombinationList.Where(c => c.Selected && c.Stake != null).Sum(c => (int)c.Stake);

            if (NumberOfSelectedCombinations != numberOfSelected)
            {
                NumberOfSelectedCombinations = numberOfSelected;
            }
            if (TotalStake != totalStake)
            {
                TotalStake = totalStake;
            }
        }

        private int numberOfSelectedCombinations;
        public int NumberOfSelectedCombinations
        {
            get
            {
                return numberOfSelectedCombinations;
            }
            set
            {
                numberOfSelectedCombinations = value;
                OnPropertyChanged("NumberOfSelectedCombinations");
            }
        }

        private int totalStake;
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

        public void SortCombinationValues()
        {
            if (CombinationList != null)
            {
                List<HPTCombination> orderedCombinations = CombinationList.OrderBy(c => c.CombinationOddsExact).ToList();
                for (int i = 0; i < orderedCombinations.Count(); i++)
                {
                    orderedCombinations[i].CombinationOddsRank = i + 1;
                }

                orderedCombinations = CombinationList.OrderBy(c => c.CombinationOddsExact).ToList();
                for (int i = 0; i < CombinationList.Count; i++)
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

        //internal string GetUniqueCodeFromCombination(HPTService.HPTCombination comb)
        //{
        //    string code = GetHexCode(comb.Horse1Nr) + GetHexCode(comb.Horse2Nr) + GetHexCode(comb.Horse3Nr);

        //    return code;
        //}

        // Trio
        public int TotalPercentage { get; set; }

        // Trio
        public void CalculatePercentages(IEnumerable<HPTHorse> horseList)
        {
            TotalPercentage = horseList
                .Sum(h => h.TrioInfo.PlaceInfo1.Percent);

            //horseList.ToList().ForEach(h =>
            //{
            //    var horse = this.horsel
            //});

            if (TotalPercentage > 0)
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
                CombinationsToShowList.Clear();
                IEnumerable<HPTCombination> combinationsToShow = new List<HPTCombination>();

                switch (BetType)
                {
                    case "T":
                        combinationsToShow = CombinationList
                        .Where(c => c.Horse1.TrioInfo.PlaceInfo1.Selected && c.Horse2.TrioInfo.PlaceInfo2.Selected && c.Horse3.TrioInfo.PlaceInfo3.Selected)
                        .ToList();
                        break;
                    case "DD":
                    case "LD":
                        combinationsToShow = CombinationList
                        .Where(c => c.Horse1.Selected && c.Horse2.Selected)
                        .ToList();
                        break;
                    case "TV":
                        combinationsToShow = CombinationList
                        .Where(c => c.Horse1.Selected || c.Horse2.Selected)
                        .ToList();
                        break;
                    default:
                        break;
                }

                foreach (var combinationToShow in combinationsToShow)
                {
                    CombinationsToShowList.Add(combinationToShow);
                }

                CombinationList
                    .Except(CombinationsToShowList)
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

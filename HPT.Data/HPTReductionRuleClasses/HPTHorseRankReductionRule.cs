using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseRankReductionRule : HPTNumberOfWinnersReductionRule
    {
        public HPTHorseRankReductionRule()
            : base()
        {
        }

        public HPTHorseRankReductionRule(int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
            foreach (HPTNumberOfWinners now in NumberOfWinnersList)
            {
                now.Selectable = true;
            }

            InitializeLegSelectionList(numberOfRaces);
        }

        //public bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow, List<int> rankList)
        //{
        //    if (!this.OnlyInSpecifiedLegs)
        //    {
        //        int numberInInterval = rankList.Count(r => r >= this.LowerBoundary && r <= this.UpperBoundary);
        //        if (!this.NumberOfWinnersList.First(now => now.NumberOfWinners == numberInInterval).Selected)
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        int numberOfX = 0;
        //        foreach (var legNumber in this.LegList)
        //        {
        //            int rankValue = rankList[legNumber - 1];
        //            numberOfX += rankValue >= this.LowerBoundary && rankValue <= this.UpperBoundary ? 1 : 0;
        //        }
        //        if (!this.NumberOfWinnersList[numberOfX].Selected)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Skapa lista med alla rankpoäng
            var rankList = singleRow.HorseList
                .Select(h => h.RankList.First(r => r.Name == ParentHorseRankSumReductionRule.PropertyName))
                .Select(r => r.Rank)
                .ToList();

            if (!OnlyInSpecifiedLegs)
            {
                int numberInInterval = rankList.Count(r => r >= LowerBoundary && r <= UpperBoundary);
                if (!NumberOfWinnersList.First(now => now.NumberOfWinners == numberInInterval).Selected)
                {
                    return false;
                }
            }
            else
            {
                int numberOfX = 0;
                foreach (var legNumber in LegList)
                {
                    int rankValue = rankList[legNumber - 1];
                    numberOfX += rankValue >= LowerBoundary && rankValue <= UpperBoundary ? 1 : 0;
                }
                if (!NumberOfWinnersList[numberOfX].Selected)
                {
                    return false;
                }
            }
            return true;
        }

        private int lowerBoundary;
        [DataMember]
        public int LowerBoundary
        {
            get
            {
                return lowerBoundary;
            }
            set
            {
                lowerBoundary = value;
                OnPropertyChanged("LowerBoundary");
            }
        }

        private int upperBoundary;
        [DataMember]
        public int UpperBoundary
        {
            get
            {
                return upperBoundary;
            }
            set
            {
                upperBoundary = value;
                OnPropertyChanged("UpperBoundary");
            }
        }

        [XmlIgnore]
        public HPTHorseRankSumReductionRule ParentHorseRankSumReductionRule { get; set; }

        public override string ReductionTypeString
        {
            get
            {
                return string.Empty;    // "Gruppintervallvillkor (rank): " + this.HorseRankVariable.CategoryText + " - " + this.HorseRankVariable.Text;
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();
            sb.Append("Rank ");
            sb.Append(LowerBoundary);
            sb.Append(" - ");
            sb.Append(UpperBoundary);
            sb.Append(": ");
            sb.Append(NumberOfWinnersString);
            ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}

using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTRankReductionRule : HPTReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Gör beräkningar på enkelraden
            if (!singleRow.ValuesCalculated)
            {
                singleRow.CalculateValues();
            }
            if (singleRow.RankSum < markBet.MinRankSum || singleRow.RankSum > markBet.MaxRankSum)
            {
                return false;
            }
            return true;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            decimal partialSum = horseList.Take(numberOfRacesToTest).Sum(h => h.RankWeighted);
            return markBet.MaxRankSum > partialSum;
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Rankvillkor";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append("Ranksumma ");
            sb.Append(markBet.MinRankSum);
            sb.Append(" - ");
            sb.Append(markBet.MaxRankSum);
            this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}

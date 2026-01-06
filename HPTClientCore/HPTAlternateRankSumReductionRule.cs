using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTAlternateRankSumReductionRule : HPTIntervalReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Gör beräkningar på enkelraden
            if (!singleRow.ValuesCalculated)
            {
                singleRow.CalculateValues();
            }
            return (MinSum <= singleRow.AlternateRankSum && MaxSum >= singleRow.AlternateRankSum);
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            int partialSum = horseList.Take(numberOfRacesToTest).Sum(h => h.RankAlternate);
            return MaxSum > partialSum;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            RuleResultForCorrectRow = "Summa " + markBet.CouponCorrector.HorseList.Sum(h => h.RankAlternate).ToString();
            return true;
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Summa Poäng";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append("Summa egen poäng: ");
            sb.Append(MinSum);
            sb.Append(" - ");
            sb.Append(MaxSum);
            sb.AppendLine();

            if (MinPercentSum > 0 || MaxPercentSum < 100)
            {
                sb.Append("Procentintervall: ");
                sb.Append(MinPercentSum);
                sb.Append(" - ");
                sb.Append(MaxPercentSum);
                sb.AppendLine();
            }

            ClipboardString = ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}

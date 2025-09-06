using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTOwnRankSumReductionRule : HPTIntervalReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Gör beräkningar på enkelraden
            if (!singleRow.ValuesCalculated)
            {
                singleRow.CalculateValues();
            }
            return (this.MinSum <= singleRow.OwnRankSum && this.MaxSum >= singleRow.OwnRankSum);
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            int partialSum = horseList.Take(numberOfRacesToTest).Sum(h => h.RankOwn);
            return this.MaxSum > partialSum;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            this.RuleResultForCorrectRow = "Summa " + markBet.CouponCorrector.HorseList.Sum(h => h.RankOwn).ToString();
            return true;
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Summa egen rank";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append("Summa egen rank: ");
            sb.Append(this.MinSum);
            sb.Append(" - ");
            sb.Append(this.MaxSum);
            sb.AppendLine();

            if (this.MinPercentSum > 0 || this.MaxPercentSum < 100)
            {
                sb.Append("Procentintervall: ");
                sb.Append(this.MinPercentSum);
                sb.Append(" - ");
                sb.Append(this.MaxPercentSum);
                sb.AppendLine("%");
            }

            this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}

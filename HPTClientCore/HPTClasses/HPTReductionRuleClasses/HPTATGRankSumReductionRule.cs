using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTATGRankSumReductionRule : HPTIntervalReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Gör beräkningar på enkelraden
            if (!singleRow.ValuesCalculated)
            {
                singleRow.CalculateValues();
            }
            return (this.MinSum <= singleRow.ATGRankSum && this.MaxSum >= singleRow.ATGRankSum);
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            int partialSum = horseList.Take(numberOfRacesToTest).Sum(h => h.RankATG);
            return this.MaxSum > partialSum;

        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            this.RuleResultForCorrectRow = "Summa " + markBet.CouponCorrector.HorseList.Sum(h => h.RankATG).ToString();
            return true;
        }

        public override string ReductionTypeString
        {
            get
            {
                return "ATG-ranksumma";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append("Summa ATG-rank: ");
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

using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTStartNrSumReductionRule : HPTIntervalReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Gör beräkningar på enkelraden
            if (!singleRow.ValuesCalculated)
            {
                singleRow.CalculateValues();
            }
            return (MinSum <= singleRow.StartNrSum && MaxSum >= singleRow.StartNrSum);
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            int partialSum = horseList
                .Take(numberOfRacesToTest)
                .Sum(h => h.StartNr);

            return MaxSum > partialSum;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            RuleResultForCorrectRow = "Summa " + markBet.CouponCorrector.HorseList.Sum(h => h.StartNr).ToString();
            return true;
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Spårsumma";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();

            sb.Append("Spårsumma: ");
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
                sb.AppendLine("%");
            }

            ClipboardString = ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}

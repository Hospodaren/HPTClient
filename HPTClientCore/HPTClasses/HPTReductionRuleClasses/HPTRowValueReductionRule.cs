using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTRowValueReductionRule : HPTIntervalReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Gör beräkningar på enkelraden
            if (!singleRow.ValuesCalculated)
            {
                singleRow.CalculateValues();
            }
            if (singleRow.RowValue == 0)
            {
                singleRow.EstimateRowValue(markBet);
            }
            return MinSum <= singleRow.RowValue && MaxSum >= singleRow.RowValue;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            var singleRow = new HPTMarkBetSingleRow(markBet.CouponCorrector.HorseList.ToArray());
            singleRow.CalculateValues();
            singleRow.EstimateRowValue(markBet);
            RuleResultForCorrectRow = "Radvärde " + singleRow.RowValue.ToString("## ### ###");
            return true;
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Radvärde";
            }
        }

        public override void SetReductionSpecificationString()
        {
            ReductionSpecificationString = "Radvärde " + MinSum.ToString() + " kr - " + MaxSum.ToString() + " kr";
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append("Beräknat radvärde: ");
            sb.Append(MinSum);
            sb.Append(" - ");
            sb.Append(MaxSum);
            sb.AppendLine(" kr");

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

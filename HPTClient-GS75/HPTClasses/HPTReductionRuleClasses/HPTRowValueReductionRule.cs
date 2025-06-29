using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

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
            return this.MinSum <= singleRow.RowValue && this.MaxSum >= singleRow.RowValue;
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            var singleRow = new HPTMarkBetSingleRow(markBet.CouponCorrector.HorseList.ToArray());
            singleRow.CalculateValues();
            singleRow.EstimateRowValue(markBet);
            this.RuleResultForCorrectRow = "Radvärde " + singleRow.RowValue.ToString("## ### ###");
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
            this.ReductionSpecificationString = "Radvärde " + this.MinSum.ToString() + " kr - " + this.MaxSum.ToString() + " kr";
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append("Beräknat radvärde: ");
            sb.Append(this.MinSum);
            sb.Append(" - ");
            sb.Append(this.MaxSum);
            sb.AppendLine(" kr");

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

using System.Runtime.Serialization;
using System.Text;

namespace HPTClient // KOMMANDE
{
    [DataContract]
    public class HPTOwnProbabilityReductionRule : HPTReductionRule
    {
        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            // Gör beräkningar på enkelraden
            if (!singleRow.ValuesCalculated)
            {
                singleRow.CalculateValues();
            }
            return MinProbability <= singleRow.OwnProbabilityQuota;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            return true;
        }

        private bool use;
        [DataMember]
        public bool Use
        {
            get
            {
                return use;
            }
            set
            {
                if (use == value)
                {
                    return;
                }
                use = value;
                OnPropertyChanged("Use");
            }
        }

        private decimal minProbability;
        [DataMember]
        public decimal MinProbability
        {
            get
            {
                return minProbability;
            }
            set
            {
                if (minProbability == value)
                {
                    return;
                }
                minProbability = value;
                OnPropertyChanged("MinProbability");
            }
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Chansvärderingsvillkor";
            }
        }

        public override void SetReductionSpecificationString()
        {
            ReductionSpecificationString = "Värderad sannolikhet över " + MinProbability.ToString("P1");
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();
            sb.Append(ReductionTypeString);
            sb.Append(": ");
            sb.Append(ReductionSpecificationString);
            //this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}

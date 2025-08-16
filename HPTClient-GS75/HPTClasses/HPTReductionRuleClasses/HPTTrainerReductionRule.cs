using System.Runtime.Serialization;
using System.Text;

namespace HPTClient
{
    [DataContract]
    public class HPTTrainerReductionRule : HPTPersonReductionRule
    {
        public HPTTrainerReductionRule()
            : base()
        {
        }

        public HPTTrainerReductionRule(int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
        }

        public override string ReductionTypeString
        {
            get
            {
                return "Tränarvillkor: " + this.NumberOfWinnersString + " vinnare";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            foreach (string person in this.PersonNameList)
            {
                sb.AppendLine(person);
            }
            this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}

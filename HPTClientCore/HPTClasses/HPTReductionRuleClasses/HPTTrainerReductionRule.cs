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
                return $"Tränarvillkor: {NumberOfWinnersString} vinnare";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();
            foreach (var person in PersonNameList)
            {
                sb.AppendLine(person);
            }
            ClipboardString = $"{ReductionTypeString}\r\n{sb}";
            return sb.ToString();
        }
    }
}

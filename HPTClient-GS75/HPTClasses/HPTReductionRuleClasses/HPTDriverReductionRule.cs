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
    public class HPTDriverReductionRule : HPTPersonReductionRule
    {
        public HPTDriverReductionRule()
            : base()
        {
        }

        public HPTDriverReductionRule(int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
        }

        //public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        //{
        //    int numberOfHorses = this.HorseList.Intersect(singleRow.HorseList).Count();
        //    return this.NumberOfWinnersList.First(now => now.NumberOfWinners == numberOfHorses).Selected;
        //}

        public override string ReductionTypeString
        {
            get
            {
                return "Kuskvillkor: " + this.NumberOfWinnersString + " vinnare";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            var sb = new StringBuilder();
            foreach (string person in this.PersonNameList)
            {
                sb.AppendLine(person);
            }

            this.ClipboardString = this.ReductionTypeString + "\r\n" + sb.ToString();
            return sb.ToString();
        }
    }
}

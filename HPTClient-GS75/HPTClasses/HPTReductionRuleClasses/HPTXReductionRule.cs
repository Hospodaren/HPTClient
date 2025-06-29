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
    public class HPTXReductionRule : HPTNumberOfWinnersReductionRule
    {
        public HPTXReductionRule()
        {
        }

        public HPTXReductionRule(HPTPrio prio, int numberOfRaces, bool use)
            : base(numberOfRaces, use)
        {
            this.Prio = prio;
            InitializeLegSelectionList(numberOfRaces);
        }
        
        public HPTXReductionRule Clone()
        {
            HPTXReductionRule rule = new HPTXReductionRule(this.Prio, this.NumberOfWinnersList.Count - 1, this.Use);
            foreach (HPTNumberOfWinners now in this.NumberOfWinnersList)
            {
                HPTNumberOfWinners hptNow = rule.NumberOfWinnersList.FirstOrDefault(n => n.NumberOfWinners == now.NumberOfWinners);
                if (hptNow != null)
                {
                    hptNow.Selected = now.Selected;
                    hptNow.Selectable = now.Selectable;
                }
            }
            return rule;
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTMarkBetSingleRow singleRow)
        {
            if (this.NumberOfRacesWithX == 0 || this.SkipRule)
            {
                return true;
            }
            if (!this.OnlyInSpecifiedLegs)
            {
                int numberOfX = singleRow.HorseList.Count(h => h.Prio == this.Prio);
                return this.NumberOfWinnersList[numberOfX].Selected;
            }
            else
            {
                int numberOfX = 0;
                foreach (var legNumber in this.LegList)
                {
                    numberOfX += singleRow.HorseList[legNumber - 1].Prio == this.Prio ? 1 : 0;
                }
                return this.NumberOfWinnersList[numberOfX].Selected;
            }
        }

        public override bool IncludeRow(HPTMarkBet markBet, HPTHorse[] horseList, int numberOfRacesToTest)
        {
            if (this.NumberOfRacesWithX == 0 || numberOfRacesToTest <= this.MaxNumberOfX || this.SkipRule)
            {
                return true;
            }

            if (!this.OnlyInSpecifiedLegs)
            {
                int numberOfX = horseList.Take(numberOfRacesToTest).Count(h => h.Prio == this.Prio);
                if (numberOfX > this.MaxNumberOfX)  // Högsta antal har överskridits redan innan alla lopp kontrollerats
                {
                    return false;
                }
                if (numberOfX + markBet.BetType.NumberOfRaces - numberOfRacesToTest < this.MinNumberOfX)    // Det går inte att komma upp i minimiantal på resterande lopp
                {
                    return false;
                }
                return true; 
            }
            else
            {
                int numberOfX = 0;
                foreach (var legNumber in this.LegList.Where(ln => ln <= numberOfRacesToTest))
                {
                    numberOfX += horseList[legNumber - 1].Prio == this.Prio ? 1 : 0;
                }
                return numberOfX <= this.MaxNumberOfX;
            }
        }

        public override bool GetRuleResultForCorrectRow(HPTMarkBet markBet)
        {
            // Skapa dictionary för att kontrollera hur många vinstrader villkoret skulle gett
            if (markBet.RaceDayInfo.ResultComplete)
            {
                int numberOfXHorses = markBet.CouponCorrector.HorseList.Count(h => h.Prio == this.Prio);
                this.RuleResultForCorrectRow = numberOfXHorses.ToString() + " " + this.Prio.ToString() + "-Häst(ar)";
            }
            return true;
        }

        public override void SetReductionSpecificationString()
        {
            var sb = new StringBuilder();
            sb.Append(this.NumberOfWinnersString);
            sb.Append(" ");
            sb.Append(this.Prio);
            sb.Append("-Hästar");
            if (this.OnlyInSpecifiedLegs)
            {
                sb.Append(" (");
                sb.Append(this.SelectedRacesString);
                sb.Append(")");
            }
            this.ReductionSpecificationString = sb.ToString();
        }

        private HPTPrio prio;
        [DataMember]
        public HPTPrio Prio
        {
            get
            {
                return this.prio;
            }
            set
            {
                this.prio = value;
                OnPropertyChanged("Prio");
            }
        }

        private int numberOfX;
        [DataMember]
        public int NumberOfX
        {
            get
            {
                return this.numberOfX;
            }
            set
            {
                this.numberOfX = value;
                OnPropertyChanged("NumberOfX");
            }
        }

        private int numberOfRacesWithX;
        [DataMember]
        public int NumberOfRacesWithX
        {
            get
            {
                return this.numberOfRacesWithX;
            }
            set
            {
                this.numberOfRacesWithX = value;
                OnPropertyChanged("NumberOfRacesWithX");
            }
        }

        public int NumberOfXInRace;

        [XmlIgnore]
        public List<System.Collections.BitArray> CombinationsToTest;


        public override string ReductionTypeString
        {
            get
            {
                return this.Prio.ToString() + "-villkor";
            }
        }

        public override string ToString(HPTMarkBet markBet)
        {
            // Create String representation
            StringBuilder sb = new StringBuilder();
            sb.Append(this.NumberOfWinnersString);
            //for (int i = this.MinNumberOfX; i <= this.MaxNumberOfX; i++)
            //{
            //    var hptNow = this.NumberOfWinnersList.FirstOrDefault(now => now.NumberOfWinners == i);
            //    if (hptNow != null && hptNow.Selected)
            //    {
            //        sb.Append(i);
            //        sb.Append(",");
            //    }
            //}
            if (sb.Length > 1)
            {
                //sb.Remove(sb.Length - 1, 1);
                sb.Append(" ");
                sb.Append(this.Prio);
                sb.Append("-Hästar");
            }

            this.ClipboardString = sb.ToString();
            return sb.ToString();
        }
    }
}

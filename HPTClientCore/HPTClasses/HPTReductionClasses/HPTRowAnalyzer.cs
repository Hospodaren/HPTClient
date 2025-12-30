namespace HPTClient
{
    public class HPTRowAnalyzer
    {
        internal IEnumerable<HPTHorse> HorseListToAnalyze;
        internal HPTHorse[] HorseList;
        internal bool IsFinished;
        internal int NumberOfRaces;

        public bool StopCalculation { get; set; }

        public Action<HPTMarkBetSingleRow> AnalyzeRow;

        public Func<int, HPTRowAnalyzer, bool> AnalyzeRowInAdvance;

        public event Action AnalyzingFinished;

        public event Action InterruptionSuceeded;

        public HPTRowAnalyzer(IEnumerable<HPTHorse> horseListToAnalyze, int numberOfRaces, Action<HPTMarkBetSingleRow> analyzeRow, Func<int, HPTRowAnalyzer, bool> analyzeRowInAdvance)
        {
            this.HorseListToAnalyze = horseListToAnalyze;
            this.NumberOfRaces = numberOfRaces;
            this.HorseList = new HPTHorse[numberOfRaces];
            this.AnalyzeRow = analyzeRow;
            this.AnalyzeRowInAdvance = analyzeRowInAdvance;
        }

        public void MakeSingleRowCollection(object stateInfo)
        {
            try
            {
                MakeSingleRowCollection(0);
                this.IsFinished = true;
                if (this.AnalyzingFinished != null)
                {
                    this.AnalyzingFinished();
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal int numberOfAnalyzedRows = 0;
        private void MakeSingleRowCollection(int raceNumber)
        {
            if (raceNumber == this.NumberOfRaces)
            {
                if (this.StopCalculation)
                {
                    if (this.InterruptionSuceeded != null)
                    {
                        this.IsFinished = true;
                        this.InterruptionSuceeded();
                    }
                    return;
                }
                HPTMarkBetSingleRow singleRow = new HPTMarkBetSingleRow(this.HorseList);
                AnalyzeRow(singleRow);
                this.numberOfAnalyzedRows++;
                return;
            }
            else if (raceNumber > 1)    // Andra loppet eller senare
            {
                if (!AnalyzeRowInAdvance(raceNumber, this))    // raceNumber är index i array... :-(
                {
                    return;
                }
            }
            foreach (HPTHorse horse in this.HorseListToAnalyze.Where(h => h.ParentRace.LegNr == raceNumber + 1))
            {
                this.HorseList[raceNumber] = horse;
                MakeSingleRowCollection(raceNumber + 1);
            }
        }
    }
}

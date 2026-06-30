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
            HorseListToAnalyze = horseListToAnalyze;
            NumberOfRaces = numberOfRaces;
            HorseList = new HPTHorse[numberOfRaces];
            AnalyzeRow = analyzeRow;
            AnalyzeRowInAdvance = analyzeRowInAdvance;
        }

        public void MakeSingleRowCollection(object stateInfo)
        {
            try
            {
                MakeSingleRowCollection(0);
                IsFinished = true;
                if (AnalyzingFinished != null)
                {
                    AnalyzingFinished();
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
            if (raceNumber == NumberOfRaces)
            {
                if (StopCalculation)
                {
                    if (InterruptionSuceeded != null)
                    {
                        IsFinished = true;
                        InterruptionSuceeded();
                    }
                    return;
                }
                HPTMarkBetSingleRow singleRow = new HPTMarkBetSingleRow(HorseList);
                AnalyzeRow(singleRow);
                numberOfAnalyzedRows++;
                return;
            }
            else if (raceNumber > 1)    // Andra loppet eller senare
            {
                if (!AnalyzeRowInAdvance(raceNumber, this))    // raceNumber är index i array... :-(
                {
                    return;
                }
            }
            foreach (HPTHorse horse in HorseListToAnalyze.Where(h => h.ParentRace.LegNr == raceNumber + 1))
            {
                HorseList[raceNumber] = horse;
                MakeSingleRowCollection(raceNumber + 1);
            }
        }
    }
}

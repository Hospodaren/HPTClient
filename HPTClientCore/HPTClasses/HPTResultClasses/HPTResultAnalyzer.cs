using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTResultAnalyzer : Notifier
    {
        #region egna events

        public static event Action<IEnumerable<HPTHorse>, HPTMarkBet> ResultAnalyzerAdded;

        #endregion

        internal static string ResultAnalyzerFileName = HPTConfig.MyDocumentsPath + "HPTResultAnalyzerList.hptral";

        public HPTResultAnalyzer()
        {
        }

        public HPTResultAnalyzer(IEnumerable<HPTHorse> horseList, HPTMarkBet markBet)
        {
            if (horseList == null || horseList.Count() == 0)
            {
                return;
            }

            this.MarkBet = markBet;

            var firstHorse = horseList.First();
            if (firstHorse.ParentRace != null && firstHorse.ParentRace.ParentRaceDayInfo != null)
            {
                this.TrackId = firstHorse.ParentRace.ParentRaceDayInfo.TrackId;
                this.TrackName = firstHorse.ParentRace.ParentRaceDayInfo.Trackname;
                this.BetTypeCode = firstHorse.ParentRace.ParentRaceDayInfo.BetType.Code;
                this.RaceDate = firstHorse.ParentRace.ParentRaceDayInfo.RaceDayDate;
            }

            this.HorseList = new ObservableCollection<HPTHorseLightAnalyzed>(horseList.Select(h => new HPTHorseLightAnalyzed()
            {
                Horse = h,
                LegNr = h.ParentRace.LegNr,
                LegNrString = h.ParentRace.LegNrString,
                Name = h.HorseName,
                StartNr = h.StartNr,
                RankvariableMean = Convert.ToDecimal(h.RankList.Where(r => r.Name != "RankOwn" && r.Name != "RankTip" && r.Name != "RankABC" && r.Name != "RankAlternate").Average(r => r.Rank)),
                RankvariableSum = Convert.ToDecimal(h.RankList.Where(r => r.Name != "RankOwn" && r.Name != "RankTip" && r.Name != "RankABC" && r.Name != "RankAlternate").Sum(r => r.Rank))

            }));

            var rankVariableList = HPTHorseRankVariable.CreateVariableList();
            this.RankVariableHorseRankList = new ObservableCollection<HPTRankVariableHorseRankList>(
                rankVariableList
                .Where(rv => rv.PropertyName != "RankOwn" && rv.PropertyName != "RankTip" && rv.PropertyName != "RankABC" && rv.PropertyName != "RankAlternate")
                .Select(rv => new HPTRankVariableHorseRankList()
                {
                    CategoryText = rv.CategoryText,
                    Name = rv.Text,
                    PropertyName = rv.PropertyName,
                    BackColor = new SolidColorBrush(Colors.LightGray),
                    HorseRankList = new ObservableCollection<HPTHorseRank>()
                }));

            foreach (var horse in horseList)
            {
                int minRank = horse.RankList.Where(r => r.Name != "RankOwn" && r.Name != "RankTip" && r.Name != "RankABC" && r.Name != "RankAlternate").Min(r => r.Rank);
                int maxRank = horse.RankList.Where(r => r.Name != "RankOwn" && r.Name != "RankTip" && r.Name != "RankABC" && r.Name != "RankAlternate").Max(r => r.Rank);
                foreach (var rankVariableHorseRank in this.RankVariableHorseRankList)
                {
                    var rankVariable = rankVariableList.First(rv => rv.PropertyName == rankVariableHorseRank.PropertyName);
                    var horseRank = horse.RankList.FirstOrDefault(r => r.Name == rankVariableHorseRank.PropertyName);
                    var horseRankCopy = new HPTHorseRank()
                    {
                        Name = horseRank.Name,
                        Rank = horseRank.Rank,
                        RankWeighted = horseRank.RankWeighted,
                        RankValueString = CreateRankValueString(rankVariable, horse),
                        BackColor = SetBackColorOnHorseRank(horseRank.Rank, minRank, maxRank)
                    };
                    rankVariableHorseRank.HorseRankList.Add(horseRankCopy);
                }
            }
            CalculateMeanAndStDev();

            // Utdelning för omgången
            this.PayOutList = new List<HPTPayOut>(markBet.RaceDayInfo.PayOutList);
        }

        internal string CreateRankValueString(HPTHorseRankVariable rankVariable, HPTHorse horse)
        {
            // Textproperty för hästens variabelvärde
            string displayValue = null;
            if (string.IsNullOrEmpty(rankVariable.DisplayPropertyName))
            {
                displayValue = Convert.ToString(rankVariable.HorseProperty.GetValue(horse, null));
            }
            else
            {
                displayValue = Convert.ToString(horse.GetType().GetProperty(rankVariable.DisplayPropertyName).GetValue(horse, null));
            }
            decimal propertyValue = 0M;
            bool isNumber = decimal.TryParse(displayValue, out propertyValue);

            // Lägg i strängvariabel och formatera om det behövs
            string textToShow = displayValue;
            if (!isNumber)
            {
            }
            else if (propertyValue == rankVariable.ValueForMissing || !rankVariable.Sort)
            {
                textToShow = "-";
            }
            else
            {
                textToShow = propertyValue.ToString(rankVariable.StringFormat);
            }
            return textToShow;
        }

        [OnDeserialized]
        public void SetNonSerializedValues(StreamingContext sc)
        {
            for (int i = 0; i < this.HorseList.Count; i++)
            {
                var horseRankList = this.RankVariableHorseRankList.Select(hrl => hrl.HorseRankList.ElementAt(i)).ToList();
                int minRank = horseRankList.Min(r => r.Rank);
                int maxRank = horseRankList.Max(r => r.Rank);
                var horseRanksToSetColorOn = horseRankList.Where(r => r.Rank == minRank || r.Rank == maxRank);
                foreach (var horseRank in horseRanksToSetColorOn)
                {
                    horseRank.BackColor = SetBackColorOnHorseRank(horseRank.Rank, minRank, maxRank);
                }
            }
            foreach (var rvhr in this.RankVariableHorseRankList)
            {

                rvhr.BackColor = new SolidColorBrush(Colors.LightGray);
            }
            CalculateMeanAndStDev();
        }

        public HPTMarkBet MarkBet { get; set; }

        internal SolidColorBrush SetBackColorOnHorseRank(int rank, int minRank, int maxRank)
        {
            var c = Colors.White;
            if (rank == minRank)
            {
                c = Colors.LightGreen;
            }
            else if (rank == maxRank)
            {
                c = Colors.LightCoral;
            }
            return new SolidColorBrush(c);
        }

        private ObservableCollection<HPTHorseLightAnalyzed> horseList;
        [DataMember]
        public ObservableCollection<HPTHorseLightAnalyzed> HorseList
        {
            get
            {
                return this.horseList;
            }
            set
            {
                this.horseList = value;
                OnPropertyChanged("HorseList");
            }
        }

        private ObservableCollection<HPTRankVariableHorseRankList> rankVariableHorseRankList;
        [DataMember]
        public ObservableCollection<HPTRankVariableHorseRankList> RankVariableHorseRankList
        {
            get
            {
                return this.rankVariableHorseRankList;
            }
            set
            {
                this.rankVariableHorseRankList = value;
                OnPropertyChanged("RankVariableHorseRankList");
            }
        }

        #region Informationsvariabler

        private DateTime raceDate;
        [DataMember]
        public DateTime RaceDate
        {
            get
            {
                return this.raceDate;
            }
            set
            {
                this.raceDate = value;
                OnPropertyChanged("RaceDate");
            }
        }

        private string dateString;
        public string DateString
        {
            get
            {
                return this.dateString;
            }
            set
            {
                this.dateString = value;
                OnPropertyChanged("DateString");
            }
        }

        private string betTypeCode;
        [DataMember]
        public string BetTypeCode
        {
            get
            {
                return this.betTypeCode;
            }
            set
            {
                this.betTypeCode = value;
                OnPropertyChanged("BetTypeCode");
            }
        }

        private string trackName;
        [DataMember]
        public string TrackName
        {
            get
            {
                return this.trackName;
            }
            set
            {
                this.trackName = value;
                OnPropertyChanged("TrackName");
            }
        }

        private int trackId;
        [DataMember]
        public int TrackId
        {
            get
            {
                return this.trackId;
            }
            set
            {
                this.trackId = value;
                OnPropertyChanged("TrackId");
            }
        }

        private List<HPTPayOut> payOutList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTPayOut> PayOutList
        {
            get
            {
                return this.payOutList;
            }
            set
            {
                this.payOutList = value;
                OnPropertyChanged("PayOutList");
            }
        }

        #endregion

        #region Analys av resultat (rankvariabler)

        internal void CalculateMeanAndStDev()
        {
            foreach (var rankVariableHorseRank in this.RankVariableHorseRankList)
            {
                // Snitt och summa
                rankVariableHorseRank.Mean = Convert.ToDecimal(rankVariableHorseRank.HorseRankList.Select(hr => hr.Rank).Average());
                rankVariableHorseRank.Sum = rankVariableHorseRank.HorseRankList.Sum(hr => hr.Rank);

                // Standardavvikelse
                decimal variance = rankVariableHorseRank.HorseRankList.Select(hr => hr.Rank).Average(r => (r - rankVariableHorseRank.Mean) * (r - rankVariableHorseRank.Mean));
                rankVariableHorseRank.StDev = Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(variance)));
            }
            var orderedList = this.RankVariableHorseRankList.OrderBy(rv => rv.Mean);
            orderedList.First().BackColor = new SolidColorBrush(Colors.LightGreen);
            orderedList.Last().BackColor = new SolidColorBrush(Colors.LightCoral);
        }

        #endregion

        #region Hantering av historik

        internal static void SaveResultAnalyzerList()
        {
            HPTSerializer.SerializeHPTResultAnalyzerList(ResultAnalyzerFileName, resultAnalyzerList);
        }

        private static ObservableCollection<HPTResultAnalyzer> resultAnalyzerList;
        public static ObservableCollection<HPTResultAnalyzer> ResultAnalyzerList
        {
            get
            {
                if (resultAnalyzerList == null)
                {
                    resultAnalyzerList = HPTSerializer.DeserializeHPTResultAnalyzerList(ResultAnalyzerFileName);
                    if (resultAnalyzerList == null)
                    {
                        resultAnalyzerList = new ObservableCollection<HPTResultAnalyzer>();
                        lastResultAnalyzerSave = DateTime.MinValue;
                    }
                }
                return resultAnalyzerList;
            }
            set
            {
                resultAnalyzerList = value;
            }
        }

        public static void AddResultAnalyzer(HPTResultAnalyzer resultAnalyzer)
        {
            var existingResultAnalyzer = ResultAnalyzerList
                .FirstOrDefault(ra => ra.BetTypeCode == resultAnalyzer.BetTypeCode
                    && ra.RaceDate == resultAnalyzer.RaceDate
                    && ra.TrackId == resultAnalyzer.TrackId);

            if (existingResultAnalyzer == null)
            {
                ResultAnalyzerList.Add(resultAnalyzer);
            }
        }

        public string ExportToExcel()
        {
            var sb = new StringBuilder();
            sb.Append("Datum\tSpelform\tBana");
            foreach (var payOut in this.PayOutList.OrderByDescending(po => po.NumberOfCorrect))
            {
                sb.Append("\t");
                sb.Append(payOut.NumberOfCorrect);
                sb.Append(" rätt");
            }
            sb.Append("\r\n\r\n");

            if (this.RankVariableHorseRankList != null && this.RankVariableHorseRankList.Count > 0)
            {
                sb.Append(this.RaceDate.ToShortDateString());
                sb.Append("\t");
                sb.Append(this.BetTypeCode);
                sb.Append("\t");
                sb.Append(this.TrackName);
                //sb.Append("\t");
                //sb.Append(0);
                //sb.Append("\r\n\r\n");
            }
            sb.Append("\t");
            foreach (var payOut in this.PayOutList.OrderByDescending(po => po.NumberOfCorrect))
            {
                sb.Append(payOut.PayOutAmount);
                sb.Append("\t");
            }
            sb.Append("\r\n\r\n");

            sb.Append("Startnummer\t\t");
            foreach (var horse in this.HorseList.OrderBy(h => h.LegNr).ToList())
            {
                sb.Append(horse.StartNr);
                sb.Append("\t");
            }
            sb.Append("Summa");
            sb.Append("\t\t");

            foreach (var horse in this.HorseList.OrderBy(h => h.LegNr).ToList())
            {
                sb.Append(horse.StartNr);
                sb.Append("\t");
            }
            sb.Append("\r\n\r\n");

            foreach (var rv in this.RankVariableHorseRankList.OrderBy(rvhr => rvhr.CategoryText).ThenBy(rvhr => rvhr.Name).ToList())
            {
                sb.Append(rv.CategoryText);
                sb.Append("\t");
                sb.Append(rv.Name);
                sb.Append("\t");
                var sbValues = new StringBuilder();

                for (int i = 0; i < rv.HorseRankList.Count; i++)
                {
                    var horseRank = rv.HorseRankList.ElementAt(i);
                    if (horseRank != null)
                    {
                        sb.Append(horseRank.Rank);
                        sb.Append("\t");

                        sbValues.Append(horseRank.RankValueString);
                        sbValues.Append("\t");
                    }
                }
                sb.Append(rv.Sum);
                sb.Append("\t\t");
                sb.Append(sbValues.ToString());
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        public static void ImportResultAnalyzerList(string fileName)
        {

        }

        public static void ExportResultAnalyzerList(string fileName)
        {
        }

        public string ExportToExcel(bool includeHeaders)
        {
            var sb = new StringBuilder();
            if (includeHeaders)
            {
                sb.Append("Datum\tSpelform\tBana");
                foreach (var payOut in this.PayOutList.OrderByDescending(po => po.NumberOfCorrect))
                {
                    sb.Append("\t");
                    sb.Append(payOut.NumberOfCorrect);
                    sb.Append(" rätt");
                }
                foreach (var rv in this.RankVariableHorseRankList.OrderBy(rvhr => rvhr.CategoryText).ThenBy(rvhr => rvhr.Name).ToList())
                {
                    sb.Append("\t");
                    sb.Append(rv.Name);
                    sb.Append(" (");
                    sb.Append(rv.CategoryText);
                    sb.Append(")");
                }
                sb.Append("\r\n");
            }

            sb.Append(this.RaceDate.ToShortDateString());
            sb.Append("\t");
            sb.Append(this.BetTypeCode);
            sb.Append("\t");
            sb.Append(this.TrackName);
            sb.Append("\t");
            foreach (var payOut in this.PayOutList.OrderByDescending(po => po.NumberOfCorrect))
            {
                sb.Append(payOut.PayOutAmount);
                sb.Append("\t");
            }

            foreach (var rv in this.RankVariableHorseRankList.OrderBy(rvhr => rvhr.CategoryText).ThenBy(rvhr => rvhr.Name).ToList())
            {
                sb.Append(rv.Mean);
                sb.Append("\t");
            }
            return sb.ToString();
        }

        public static string ExportResultAnalyzerListToExcel()
        {
            var sb = new StringBuilder();
            sb.Append("Datum\tSpelform\tBana\tUtdelning\tAntal");
            var raFirst = HPTResultAnalyzer.ResultAnalyzerList.FirstOrDefault();
            if (raFirst != null && raFirst.RankVariableHorseRankList != null && raFirst.RankVariableHorseRankList.Count > 0)
            {
                foreach (var rv in raFirst.RankVariableHorseRankList.OrderBy(rvhr => rvhr.CategoryText).ThenBy(rvhr => rvhr.Name).ToList())
                {
                    sb.Append("\t");
                    sb.Append(rv.Name);
                    sb.Append(" (");
                    sb.Append(rv.CategoryText);
                    sb.Append(")");
                }
            }
            sb.Append("\r\n");
            foreach (var ra in HPTResultAnalyzer.ResultAnalyzerList)
            {
                if (ra.RankVariableHorseRankList != null && ra.RankVariableHorseRankList.Count > 0)
                {
                    sb.Append(ra.RaceDate.ToShortDateString());
                    sb.Append("\t");
                    sb.Append(ra.BetTypeCode);
                    sb.Append("\t");
                    sb.Append(ra.TrackName);
                    sb.Append("\t");
                    sb.Append(0);
                    sb.Append("\t");
                    sb.Append(ra.HorseList.Count);

                    foreach (var rv in ra.RankVariableHorseRankList.OrderBy(rvhr => rvhr.CategoryText).ThenBy(rvhr => rvhr.Name).ToList())
                    {
                        sb.Append("\t");
                        sb.Append(rv.Mean);
                    }
                    sb.Append("\r\n");
                }
            }
            return sb.ToString();
        }

        internal static void CreateResultAnalyzerList(object stateInfo)
        {
            CreateResultAnalyzerList();
        }

        private static DateTime lastResultAnalyzerSave;
        internal static DateTime LastResultAnalyzerSave
        {
            get
            {
                if (lastResultAnalyzerSave == DateTime.MinValue)
                {
                    if (File.Exists(ResultAnalyzerFileName))
                    {
                        var fi = new FileInfo(ResultAnalyzerFileName);
                        lastResultAnalyzerSave = fi.LastWriteTime;
                    }
                }
                return lastResultAnalyzerSave;
            }
        }

        internal static void CreateResultAnalyzerList()
        {
            var diMyDocuments = new DirectoryInfo(HPTConfig.MyDocumentsPath);
            var rexRaceDayDirectory = new Regex(@"\d{4}-\d{2}-\d{2}\s\w+?");
            var rexMarkBetSystem = new Regex(@"(V\d{1,2}_\w+?_\d{1,2}_\d{4}-\d{2}-\d{2})\w+?");
            var directoriesToSearch = diMyDocuments.GetDirectories()
                .Where(di => rexRaceDayDirectory.IsMatch(di.Name)).ToList();

            foreach (var raceDayDirectory in directoriesToSearch)
            {
                var markBetFiles = raceDayDirectory.GetFiles("*.hpt?")
                    .Where(fi => fi.LastWriteTime > LastResultAnalyzerSave && rexMarkBetSystem.IsMatch(fi.Name))
                    .ToList();

                var distinctMarkBetTypes = markBetFiles
                    .Select(fi => rexMarkBetSystem.Match(fi.Name).Groups[1].Value)
                    .Distinct()
                    .ToList();

                foreach (var distinctMarkBet in distinctMarkBetTypes)
                {
                    try
                    {
                        var fi = markBetFiles
                            .Where(mbf => mbf.Name.StartsWith(distinctMarkBet))
                            .OrderByDescending(mbf => mbf.LastWriteTime)
                            .First();

                        var markBet = HPTSerializer.DeserializeHPTSystem(fi.FullName);
                        markBet.RecalculateAllRanks();
                        markBet.RecalculateRank();

                        var existingResultAnalyzer = ResultAnalyzerList
                            .FirstOrDefault(ra => ra.RaceDate.Date == markBet.RaceDayInfo.RaceDayDate.Date && ra.BetTypeCode == markBet.BetType.Code && ra.TrackId == markBet.RaceDayInfo.TrackId);

                        if (existingResultAnalyzer == null)
                        {
                            if (!markBet.RaceDayInfo.ResultComplete)
                            {
                                var connector = new HPTServiceConnector();
                                connector.GetRaceDayInfoUpdate(markBet.RaceDayInfo);
                                connector.GetResultMarkingBetByTrackAndDate(markBet.RaceDayInfo.BetType.Code, markBet.RaceDayInfo.TrackId, markBet.RaceDayInfo.RaceDayDate, markBet.RaceDayInfo, true);
                            }

                            var horseList = new HPTHorse[markBet.RaceDayInfo.RaceList.Count];
                            try
                            {
                                for (int i = 0; i < markBet.RaceDayInfo.RaceList.Count; i++)
                                {
                                    var leg = markBet.RaceDayInfo.RaceList[i];
                                    int startNr = leg.LegResult.Winners[0];
                                    var horse = leg.HorseList.First(h => h.StartNr == startNr);
                                    horseList[i] = horse;
                                }
                            }
                            catch (Exception excInner)
                            {
                                string s = excInner.Message;
                            }
                            //var resultAnalyzer = new HPTResultAnalyzer(horseList, markBet);
                            ResultAnalyzerAdded(horseList, markBet);
                            //AddResultAnalyzer(resultAnalyzer);
                        }
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }
            }
            HPTResultAnalyzer.SaveResultAnalyzerList();
        }

        #endregion
    }

    [DataContract]
    public class HPTRankVariableHorseRankList : Notifier
    {
        private string name;
        [DataMember]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        private string propertyName;
        [DataMember]
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                this.propertyName = value;
                OnPropertyChanged("PropertyName");
            }
        }

        private string categoryText;
        [DataMember]
        public string CategoryText
        {
            get
            {
                return this.categoryText;
            }
            set
            {
                this.categoryText = value;
                OnPropertyChanged("CategoryText");
            }
        }

        private SolidColorBrush backColor;
        [XmlIgnore]
        public SolidColorBrush BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                this.backColor = value;
                OnPropertyChanged("BackColor");
            }
        }

        private ObservableCollection<HPTHorseRank> horseRankList;
        [DataMember]
        public ObservableCollection<HPTHorseRank> HorseRankList
        {
            get
            {
                return this.horseRankList;
            }
            set
            {
                this.horseRankList = value;
                OnPropertyChanged("HorseRankList");
            }
        }

        private decimal sum;
        [DataMember]
        public decimal Sum
        {
            get
            {
                return this.sum;
            }
            set
            {
                this.sum = value;
                OnPropertyChanged("Sum");
            }
        }

        private decimal stDev;
        [DataMember]
        public decimal StDev
        {
            get
            {
                return this.stDev;
            }
            set
            {
                this.stDev = value;
                OnPropertyChanged("StDev");
            }
        }

        private decimal mean;
        [DataMember]
        public decimal Mean
        {
            get
            {
                return this.mean;
            }
            set
            {
                this.mean = value;
                OnPropertyChanged("Mean");
            }
        }
    }
}

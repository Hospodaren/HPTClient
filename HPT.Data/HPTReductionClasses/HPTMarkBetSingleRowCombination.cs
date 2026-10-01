using System.Text;
using System.Text.RegularExpressions;

namespace HPTClient
{
    public class HPTMarkBetSingleRowCombination
    {
        //internal List<string> UniqueCodes;
        internal List<string> RowList;
        internal int NumberOfRaces = 0;
        internal Dictionary<int, List<HPTHorse>> horseDictionary;

        public HPTMarkBetSingleRowCombination(HPTMarkBetSingleRow startRow)
        {
            NumberOfRaces = startRow.UniqueCode.Length;

            //this.UniqueCodes = Enumerable.Range(0, startRow.UniqueCode.Length)
            //    .Select(i => startRow.UniqueCode.Substring(i, 1))
            //    .ToList();

            // Skapa listan med unika radkoder
            Size = 1;
            RowList = new List<string>()
                {
                    startRow.UniqueCode
                };

            // Skapa uppslagslista med loppnummer och hästar
            horseDictionary = startRow.HorseList
                .Select(h => new List<HPTHorse>() { h })
                .ToDictionary(hl => hl.First().ParentRace.LegNr);
        }

        //public HPTMarkBetSingleRowCombination()
        //{
        //    this.RowList = new List<string>();
        //}

        #region Methods

        HPTHorse addedHorse;
        List<string> newRowsToFind = new List<string>();
        public void AddHorseNew(HPTHorse horse)
        {
            addedHorse = horse;
            newRowsToFind.Clear();
            int position = horse.ParentRace.LegNr - 1;
            RowList.ForEach(r =>
                {
                    string newRow = r.Remove(position, 1).Insert(position, horse.HexCode);
                    if (!newRowsToFind.Contains(newRow))
                    {
                        newRowsToFind.Add(newRow);
                    }
                });
        }

        //public void AddHorse(HPTHorse horse)
        //{            
        //    this.UniqueCodes[horse.ParentRace.LegNr - 1] += horse.HexCode;
        //    SetUniqueCodesRegexAndSystemSize();
        //}

        //public void RemoveHorse(HPTHorse horse)
        //{
        //    this.UniqueCodes[horse.ParentRace.LegNr - 1] = this.UniqueCodes[horse.ParentRace.LegNr - 1].Replace(horse.HexCode, string.Empty);
        //    SetUniqueCodesRegexAndSystemSize();
        //}

        //internal void SetUniqueCodesRegexAndSystemSize()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    int size = 1;
        //    foreach (string uniqueCode in this.UniqueCodes)
        //    {
        //        size *= uniqueCode.Length;
        //        sb.Append("[" + uniqueCode + "]");
        //    }
        //    this.Size = size;
        //    this.UniqueCodesRegex = new Regex(sb.ToString());
        //}

        //private void CreateUniqueCodes()
        //{
        //    this.UniqueCodes = Enumerable.Range(0, this.numberOfRaces)
        //            .Select(i => this.RowList.Select(r => r.Substring(i, 1))
        //            .Distinct()
        //            .Aggregate((code, next) => code + next))
        //            .ToList();
        //}

        //public bool CheckAddedRow(List<HPTMarkBetSingleRow> rowsToCompress, int currentCouponNumber)
        //{
        //    var matchingRows = rowsToCompress
        //                .AsParallel()
        //                .Where(sr => (sr.CouponNumber == 0 || sr.CouponNumber == currentCouponNumber) && this.UniqueCodesRegex.IsMatch(sr.UniqueCode))
        //                .ToList();

        //    if (matchingRows.Count != this.Size)
        //    {
        //        return false;
        //    }
        //    matchingRows.ForEach(sr => sr.CouponNumber = currentCouponNumber);
        //    return true;
        //}

        public bool CheckAddedRow(Dictionary<string, HPTMarkBetSingleRow> rowsToCompress, int currentCouponNumber)
        {
            foreach (var newRow in newRowsToFind)
            {
                if (!rowsToCompress.ContainsKey(newRow))
                {
                    return false;
                }
            }

            horseDictionary[addedHorse.ParentRace.LegNr].Add(addedHorse);
            foreach (var newRow in newRowsToFind)
            {
                RowList.Add(newRow);
                var addedRow = rowsToCompress[newRow];
                addedRow.CouponNumber = currentCouponNumber;
                rowsToCompress.Remove(newRow);
            }
            Size = RowList.Count;
            return true;
        }

        internal List<HPTHorse> GetCouponHorseList(int legNr)
        {
            return horseDictionary[legNr];
        }

        #endregion

        #region Properties

        public Regex UniqueCodesRegex { get; set; }

        public int Size { get; set; }

        public int CouponNumber { get; set; }

        public int BetMultiplier { get; set; }

        public bool V6 { get; set; }

        #endregion

        public string ToCouponString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Kupong ");
            sb.Append(CouponNumber);
            sb.AppendLine();

            foreach (var leg in horseDictionary.OrderBy(kv => kv.Key))
            {
                sb.Append("Avd ");
                sb.Append(leg.Key);
                sb.Append(": ");
                string horses = string.Join(" ,", leg.Value.OrderBy(h => h.StartNr));
                sb.Append(horses);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        //public string ToCouponString()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("Kupong ");
        //    sb.Append(this.CouponNumber);
        //    sb.AppendLine();
        //    for (int i = 0; i < this.UniqueCodes.Count; i++)
        //    {
        //        sb.Append("Avd ");
        //        sb.Append(i + 1);
        //        sb.Append(": ");
        //        char[] codeArray = this.UniqueCodes[i].ToCharArray();
        //        for (int j = 0; j < codeArray.Length; j++)
        //        {
        //            string startNumber = "0";
        //            switch (codeArray[j])
        //            {
        //                case 'A':
        //                    startNumber = "10";
        //                    break;
        //                case 'B':
        //                    startNumber = "11";
        //                    break;
        //                case 'C':
        //                    startNumber = "12";
        //                    break;
        //                case 'D':
        //                    startNumber = "13";
        //                    break;
        //                case 'E':
        //                    startNumber = "14";
        //                    break;
        //                case 'F':
        //                    startNumber = "15";
        //                    break;
        //                case 'G':
        //                    startNumber = "16";
        //                    break;
        //                case 'H':
        //                    startNumber = "17";
        //                    break;
        //                case 'I':
        //                    startNumber = "18";
        //                    break;
        //                case 'J':
        //                    startNumber = "19";
        //                    break;
        //                case 'K':
        //                    startNumber = "20";
        //                    break;
        //                default:
        //                    startNumber = codeArray[j].ToString();
        //                    break;
        //            }
        //            sb.Append(startNumber);
        //            sb.Append(", ");
        //        }
        //        sb.Remove(sb.Length - 2, 2);
        //        sb.AppendLine();
        //    }
        //    return sb.ToString();
        //}

        #region Obsolete code



        //public void AddRow(string uniqueCode)
        //{            
        //    if (this.UniqueCodes == null)
        //    {
        //        this.UniqueCodes = new List<string>();
        //        this.numberOfRaces = uniqueCode.Length;
        //        for (int i = 0; i < uniqueCode.Length; i++)
        //        {
        //            string horseCode = uniqueCode.Substring(i, 1);
        //            this.UniqueCodes.Add(horseCode);
        //        }
        //        this.UniqueRowCodes = new List<string>();
        //        this.UniqueRowCodes.Add(uniqueCode);
        //    }
        //    else
        //    {
        //        for (int i = 0; i < uniqueCode.Length; i++)
        //        {
        //            string horseCode = uniqueCode.Substring(i, 1);
        //            if (!this.UniqueCodes[i].Contains(horseCode))
        //            {
        //                this.UniqueCodes[i] += horseCode;
        //            }
        //        }
        //        this.UniqueRowCodes = new List<string>();
        //        CreateUniqueCodes(string.Empty, 0);
        //    }
        //    this.RowList.Add(uniqueCode);
        //}

        //public void RemoveLastRow()
        //{
        //    this.RowList.RemoveAt(this.RowList.Count - 1);
        //    for (int i = 0; i < this.numberOfRaces; i++)
        //    {
        //        this.UniqueCodes[i] = string.Empty;
        //    }
        //    foreach (string uniqueCode in this.RowList)
        //    {
        //        for (int i = 0; i < uniqueCode.Length; i++)
        //        {
        //            string horseCode = uniqueCode.Substring(i, 1);
        //            if (!this.UniqueCodes[i].Contains(horseCode))
        //            {
        //                this.UniqueCodes[i] += horseCode;
        //            }
        //        }
        //    }            
        //    this.UniqueRowCodes = new List<string>();
        //    CreateUniqueCodes(string.Empty, 0);
        //}

        //public bool AllCombinationsInCollection(HPTMarkBetSingleRowCollection rowCollection)
        //{
        //    //IEnumerable<string> rowCodesNotInSystem = this.UniqueRowCodes.Except(rowCollection.SingleRowsObservable.Select(sr => sr.UniqueCode));
        //    //if (rowCodesNotInSystem.Count() > 0)
        //    //{
        //    //    RemoveLastRow();
        //    //    return false;
        //    //}

        //    //IEnumerable<HPTMarkBetSingleRow> rowsAlreadyCovered = rowCollection.SingleRowsObservable
        //    //    .Where(sr => sr.CouponNumber > 0 && sr.CouponNumber != rowCollection.CurrentCouponNumber);

        //    foreach (string uniqueCode in this.UniqueRowCodes)
        //    {
        //        HPTMarkBetSingleRow row = rowCollection.SingleRows.FirstOrDefault(r => r.UniqueCode == uniqueCode);
        //        if (row == null)
        //        {
        //            RemoveLastRow();
        //            return false;
        //        }
        //        else
        //        {
        //            if (row.CouponNumber > 0 && row.CouponNumber != rowCollection.CurrentCouponNumber)
        //            {
        //                RemoveLastRow();
        //                return false;
        //            }
        //        }
        //    }
        //    foreach (string uniqueCode in this.UniqueRowCodes)
        //    {
        //        rowCollection.SingleRows.First(r => r.UniqueCode == uniqueCode).CouponNumber = rowCollection.CurrentCouponNumber;
        //    }
        //    return true;
        //}

        //public bool AllCombinationsInCollection(HPTMarkBetSingleRowCollection rowCollection)
        //{
        //    foreach (string uniqueCode in this.UniqueRowCodes)
        //    {
        //        if (!rowCollection.SingleRows.ContainsKey(uniqueCode))
        //        {
        //            RemoveLastRow();
        //            return false;
        //        }
        //        else
        //        {
        //            HPTMarkBetSingleRow row = rowCollection.SingleRows[uniqueCode];
        //            if (row.CouponNumber > 0 && row.CouponNumber != rowCollection.CurrentCouponNumber)
        //            {
        //                RemoveLastRow();
        //                return false;
        //            }
        //        }
        //    }
        //    foreach (string uniqueCode in this.UniqueRowCodes)
        //    {
        //        rowCollection.SingleRows[uniqueCode].CouponNumber = rowCollection.CurrentCouponNumber;
        //    } 
        //    return true;
        //}

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader
{
    public class ATGGameBase
    {
        public ATGGameBase(ATGGameInfoBase gameInfo)
        {
            GameInfo = gameInfo;
            SetStaticGameValues();
        }
        public ATGGameInfoBase GameInfo { get; set; }

        public int? JackpotAmount { get; set; }

        public decimal Turnover { get; set; }

        public int SystemCount { get; set; }

        public long? Version { get; set; }

        public DateTime ATGTimestamp { get; set; }

        public string Status { get; set; }

        public IEnumerable<ATGRaceBase> Races { get; set; }

        #region Variabla värden som ska uppdateras näsr startlistorna uppdateras
        internal void CalculateDynamicGameValues()
        {
            Races.ToList().ForEach(r =>
            {
                // Rangornding utifrån insatsfördelning
                int betRank = 1;
                r.StartList
                    .Where(s => s.Scratched != true)
                    .OrderByDescending(s => s.BetDistribution)
                    .ToList()
                    .ForEach(s =>
                    {
                        s.CategoryCodes.Remove(StartCategoryCode.Favorit);
                        if (betRank == 1)
                        {
                            s.CategoryCodes.Add(StartCategoryCode.Favorit);
                        }
                        s.BetRank = betRank++;
                    });

                r.StartList
                    .ToList()
                    .ForEach(s => 
                    {
                        StartCategoryCode categoryCode = s.BetDistributionShare switch
                        {
                            < 1.5m => StartCategoryCode.Storskrall,
                            < 4m => StartCategoryCode.Skrall,
                            < 10m => StartCategoryCode.Overraskning,
                            _ => StartCategoryCode.None
                        };
                        s.CategoryCodes.Add(categoryCode);
                    });

                var topTwoStarts = r.StartList
                .OrderByDescending(s => s.BetDistributionShare)
                .Take(2);

                var favourite = topTwoStarts.First();
                favourite.CategoryCodes.Remove(StartCategoryCode.None);
                favourite.CategoryCodes.Add(StartCategoryCode.Favorit);
                decimal quotient = favourite.BetDistributionShare / topTwoStarts.Last().BetDistributionShare;

                StartCategoryCode categoryCode = (favourite.BetDistributionShare, quotient) switch
                {
                    (_, < 1.1m) => StartCategoryCode.KnappFavorit,
                    (> 0.55m, > 3m) => StartCategoryCode.Megafavorit,
                    (> 0.45m, > 2m) => StartCategoryCode.Storfavorit,
                    (_, > 1.5m) => StartCategoryCode.KlarFavorit,
                    (_, _) => StartCategoryCode.None
                };
                favourite.CategoryCodes.Add(categoryCode);
            });
        }
        #endregion

        #region Hårdkodade värden beroende på spelform
        internal void SetStaticGameValues()
        {
            // Hur mycket som ligger i i varje delpool för 0, 1, 2, 3 fel
            switch (GameInfo.Code)
            {
                case "V4":
                    Poolshares = new Dictionary<int, decimal> { [0] = 0.75m };
                    break;
                case "V5":
                    Poolshares = new Dictionary<int, decimal> { [0] = 0.65m };
                    break;
                case "V64":
                case "V75":
                case "GS75":
                case "V86":
                    Poolshares = new Dictionary<int, decimal> { [0] = 0.26m, [1] = 0.13m, [2] = 0.26m};
                    break;
                case "V85":
                    Poolshares = new Dictionary<int, decimal> { [0] = 0.2275m, [1] = 0.975m, [2] = 0.975m, [2] = 0.2275m };
                    break;
                case "V65":
                    Poolshares = new Dictionary<int, decimal> { [0] = 0.325m, [1] = 0.325m};
                    break;
                default:
                    Poolshares = new Dictionary<int, decimal> { [0] = 1m };
                    break;
            }

            // Hur mycket som går tillbaka till spelarna
            switch (GameInfo.Code)
            {
                case "V4":
                    ReturnToPlayer = 0.75m;
                    break;
                case "V5":
                case "V64":
                case "V75":
                case "GS75":
                case "V86":
                case "V65":
                case "V85":
                    Poolshares = new Dictionary<int, decimal> { [0] = 0.325m, [1] = 0.325m };
                    ReturnToPlayer = 0.65m;
                    break;
                default:
                    ReturnToPlayer = 1m;
                    break;
            }

            // Radkostnad
            switch (GameInfo.Code)
            {
                case "V4":
                    RowCost = 2m;
                    break;
                case "V5":
                case "V64":
                case "V65":
                case "GS75":
                    RowCost = 1m;
                    break;
                case "V75":
                case "V85":
                    RowCost = 0.5m;
                    break;
                case "V86":
                    RowCost = 0.25m;
                    break;
                default:
                    ReturnToPlayer = 1m;
                    break;
            }

            // Vilka värden kan man ha på flerbong, för att kunna spela på målvinst
            switch (GameInfo.Code)
            {
                case "V75":
                case "GS75":
                case "V86":
                case "V5":
                case "V85":
                    AllowedBetMultipliers = [1, 2, 5, 10, 20, 50, 100];
                    break;
                case "V4":
                case "V64":
                case "V65":
                    AllowedBetMultipliers = [1, 2, 3, 4, 5, 10, 50, 100];
                    break;
                default:
                    AllowedBetMultipliers = [1];
                    break;
            }

            // Vad är utgångsvärdet på en enkelrad
            RowStartValue = RowCost * Poolshares[0];

        }

        public Dictionary<int, decimal> Poolshares { get; set; }

        public decimal RowCost { get; set; }

        public decimal VxFactor { get; set; }

        public decimal RowStartValue { get; set; }

        public int MaxNumberOfSystemsInFile { get; set; }

        public IEnumerable<int> AllowedBetMultipliers { get; set; }

        public decimal ReturnToPlayer { get; set; }
        #endregion
    }
}

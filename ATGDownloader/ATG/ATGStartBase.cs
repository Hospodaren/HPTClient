using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader
{
    public class ATGStartBase
    {
        public string Id { get; set; }

        public int Number { get; set; }

        public decimal VinnarOdds { get; set; }

        public decimal BetDistribution { get; set; }

        public decimal BetDistributionShare { get; set; }

        public int BetRank { get; set; }

        public decimal Trend { get; set; }

        public bool? Scratched { get; set; }

        public ATGHorseBase Horse { get; set; }

        public bool Selected { get; set; }

        public IList<StartCategoryCode> CategoryCodes { get; set; } = new List<StartCategoryCode>();
        //public IList<StartCategoryCode> CategoryCodes { get; set; } = [StartCategoryCode.None];
    }

    public enum StartCategoryCode
    {
        None,
        A,
        B,
        C,
        D,
        E,
        F,
        Favorit,
        Storfavorit,
        Megafavorit,
        KlarFavorit,
        KnappFavorit,
        Overraskning,
        Skrall,
        Storskrall,
        EgetDrag,
        Overspelad,
        Underspelad,
        TrendarUpp,
        TrendarNer,
        Own1,
        Own2,
        Own3,
        Own4,
        Own5
    }
}

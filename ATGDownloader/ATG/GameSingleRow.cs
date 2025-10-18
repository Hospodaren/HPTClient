using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace ATGDownloader
{
    internal class GameSingleRow
    {
        internal IEnumerable<ATGStartBase> HorsesOnRow;
        internal string UniqueCode;
        internal decimal RowShare { get; set; }

        public GameSingleRow(IEnumerable<ATGStartBase> horsesOnRow)
        {
            HorsesOnRow = horsesOnRow;
            
            RowShare = HorsesOnRow
                .Select(h => h.BetDistributionShare)
                .Aggregate((bd, next) => bd * next);

            UniqueCode = HorsesOnRow
                .Select(h => $"{h.Number:00}")
                .Aggregate((h, next) => h + next);
        }
    }
}

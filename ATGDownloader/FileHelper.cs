using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader
{
    public class FileHelper
    {
        public static bool FileExistForDate(DateTime fileDate)
        {
            return File.Exists($"*{fileDate:yyyy-MM-dd}*.json");
        }

        public static void SaveFile(ATGGameInfoBase gameInfo, string jsonData)
        {
            string trackNames = gameInfo.Tracks.Select(t => t.TrackName).Aggregate((n, next) => n + "_" + next);
            string filePath = $"{gameInfo.ScheduledStartTime:yyyy-MM-dd}_{gameInfo.Code}_{trackNames}.json";
            using var writer = new StreamWriter(filePath);
            writer.Write(jsonData);
        }
    }
}

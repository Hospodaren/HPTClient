using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ATGDownloader
{
    public class ATGDownloadHelper
    {
        private const string _baseUrl = "https://www.atg.se/services/racinginfo/v1/api/";

        public static string DownloadCalendar(DateTime date)
        {
            return DownloadATGData($"calendar/day/{date:yyyy-MM-dd}");
        }

        public static string DownloadGameInfo(string gameId)
        {
            return DownloadATGData($"games/{gameId}");
        }

        public static string DownloadGameInfo(ATGGameInfoBase gameInfo)
        {
            return DownloadATGData($"games/{gameInfo.Id}");
        }

        internal static string DownloadATGData(string resourcePath)
        {
            var options = new RestClientOptions(_baseUrl);
            var client = new RestClient(options);
            var request = new RestRequest(resourcePath);
            var response = client.Get(request);

            return response.Content;
        }
    }
}
// https://www.atg.se/services/racinginfo/v1/api/results/V75
// 

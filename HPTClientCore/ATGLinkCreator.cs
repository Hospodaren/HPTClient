using System.Text.RegularExpressions;

namespace HPTClient
{
    class ATGLinkCreator
    {
        internal const string ATGBaseUrl = "https://www.atg.se/spel/";
        //internal const string STBaseUrl = "https://www.travsport.se/hast/visa/";
        //internal string STBaseUrl = $"https://sportapp.travsport.se/sportinfo/horse/ts{}/pedigree";
        //https://sportapp.travsport.se/sportinfo/horse/ts771078/pedigree

        internal static string CreateSTHorseLink(HPTHorse horse)
        {
            return $"https://sportapp.travsport.se/sportinfo/horse/ts{horse.ATGId}/pedigree";
            //return STBaseUrl + horse.ATGId;
        }

        internal static string CreateSTHorseLink(string atgId)
        {
            return $"https://sportapp.travsport.se/sportinfo/horse/ts{atgId}/pedigree";
            //return STBaseUrl + atgId;
        }

        // Resultat i Json-format: https://www.atg.se/services/v1/horses/693542/results?stopdate=2015-09-24
        // Info på travsport.se: https://www.travsport.se/hast/visa/694361
        // Startlista: https://www.atg.se/spel/2015-09-24/vp/hagmyren/lopp1
        // Resultat:   https://www.atg.se/spel/2015-09-24/vp/hagmyren/lopp1/resultat
        internal static string CreateRaceStartlistLink(HPTRace race)
        {
            string result = ATGBaseUrl +
                race.ParentRaceDayInfo.RaceDayDateString +
                "/vp/" +
                CreateTrackNameForUrl(race) +
                "/lopp" +
                race.RaceNr.ToString();

            return result;
        }

        internal static string CreateRaceStartlistLink(int trackId, DateTime startDate, int raceNumber)
        {
            string result = ATGBaseUrl +
                startDate.ToString("yyyy-MM-dd") +
                "/vp/" +
                EnumHelper.GetTrackNameATGSEFromId(trackId) +
                "/lopp" +
                raceNumber.ToString();

            return result;
        }

        // internal static string CreateRaceResultLink(HPTRace race)
        // {
        //     string result = CreateRaceStartlistLink(race) +
        //         "/resultat";
        //
        //     race.ATGResultLink = result;
        //
        //     return result;
        // }

        internal static string CreateRaceResultLink(HPTHorseResult horseResult)
        {
            // TODO: https://www.atg.se/spel/2026-01-03/vinnare/jagersro/lopp/8/resultat
            // 2025-08-30_18_2

            var rexRaceParts = new Regex(@"(\d{4}-\d{2}-\d{2})_(\d{1,2})_(\d{1,2})", RegexOptions.IgnoreCase);
            var result = rexRaceParts.Match(horseResult.ATGId);
            int trackId = int.Parse(result.Groups[2].Value);
            string trackName = EnumHelper.GetTrackNameForResultLinkFromTrackId(trackId);
            
            return $"{ATGBaseUrl}{result.Groups[1].Value}/vinnare/{trackName}/lopp/{result.Groups[2].Value}/resultat" ;
        } 

        internal static string CreateTrackNameForUrl(HPTRace race)
        {
            string trackName = race.ParentRaceDayInfo.Trackname.ToLower();
            if (race.TrackName != null)
            {
                trackName = race.TrackName.ToLower();
            }
            trackName = trackName.Replace("å", "a");
            trackName = trackName.Replace("ä", "a");
            trackName = trackName.Replace("ö", "o");
            return trackName;
        }

        //internal static string CreateTrackNameForUrl(string trackCode)
        //{
        //    string trackName = race.ParentRaceDayInfo.Trackname.ToLower();
        //    if (race.TrackName != null)
        //    {
        //        trackName = race.TrackName.ToLower();
        //    }
        //    trackName = trackName.Replace("å", "a");
        //    trackName = trackName.Replace("ä", "a");
        //    trackName = trackName.Replace("ö", "o");
        //    return trackName;
        //}

        // https://www.atg.se/video/archive/22321221281/vinnare_2015-10-26_15_7
        // internal static string CreateRaceVideoLink(HPTHorseResult horseResult)
        // {
        //     string result = ATGBaseUrl +
        //         horseResult.Date.ToString("yyyy-MM-dd") +
        //         "/vp/" +
        //         EnumHelper.GetTrackNameATGSEFromShortString(horseResult.TrackCode) +
        //         "/lopp" +
        //         horseResult.RaceNr.ToString() +
        //         "/resultat";
        //
        //     return result;
        // }
    }
}

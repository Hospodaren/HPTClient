using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPTClient
{
    class ATGLinkCreator
    {
        internal const string ATGBaseUrl = "https://www.atg.se/spel/";
        internal const string STBaseUrl = "https://www.travsport.se/hast/visa/";

        internal static string CreateSTHorseLink(HPTHorse horse)
        {
            return STBaseUrl + horse.ATGId;
        }

        internal static string CreateSTHorseLink(string atgId)
        {
            return STBaseUrl + atgId;
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

        internal static string CreateRaceResultLink(HPTRace race)
        {
            string result = CreateRaceStartlistLink(race) +
                "/resultat";

            race.ATGResultLink = result;

            return result;
        }

        internal static string CreateRaceResultLink(HPTHorseResult horseResult)
        {
            string result = ATGBaseUrl +
                horseResult.Date.ToString("yyyy-MM-dd") +
                "/vp/" +
                EnumHelper.GetTrackNameATGSEFromShortString(horseResult.TrackCode) +
                "/lopp" +
                horseResult.RaceNr.ToString() +
                "/resultat";

            return result;
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
        internal static string CreateRaceVideoLink(HPTHorseResult horseResult)
        {
            string result = ATGBaseUrl +
                horseResult.Date.ToString("yyyy-MM-dd") +
                "/vp/" +
                EnumHelper.GetTrackNameATGSEFromShortString(horseResult.TrackCode) +
                "/lopp" +
                horseResult.RaceNr.ToString() +
                "/resultat";

            return result;
        }
    }
}

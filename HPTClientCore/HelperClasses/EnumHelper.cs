namespace HPTClient
{
    public class EnumHelper
    {
        public EnumHelper()
        {
        }

        public static string GetTextFromRankCategory(HPTRankCategory category)
        {
            switch (category)
            {
                case HPTRankCategory.MarksAndOdds:
                    return "Spelarnas rank";
                case HPTRankCategory.Record:
                    return "Rekord";
                case HPTRankCategory.Winnings:
                    return "Intj�nat";
                case HPTRankCategory.Place:
                    return "Placering";
                case HPTRankCategory.Top3:
                    return "Plats";
                case HPTRankCategory.Rest:
                    return "�vrigt";
                default:
                    return string.Empty;
            }
        }

        public static string GetTrackNameFromTrackId(int trackId)
        {
            try
            {
                var trackName = ((TrackNameEnum)trackId).ToString();
                return trackName;
            }
            catch (Exception exc)
            {
                var s = exc.Message;
                return string.Empty;
            }
        }

        public static string GetTrackNameForResultLinkFromTrackId(int trackId)
        {
            try
            {
                var trackName = ((TrackNameEnum)trackId)
                    .ToString()
                    .ToLowerInvariant()
                    .Replace('�', 'a')
                    .Replace('�', 'a')
                    .Replace('�', 'o');

                return trackName;
            }
            catch (Exception exc)
            {
                var s = exc.Message;
                return "Ok�nd bana";
            }
        }

        public static string GetTrackCodeFromTrackId(int trackId)
        {
            try
            {
                var trackCode = ((TrackCodeEnum)trackId).ToString();
                return trackCode;
            }
            catch (Exception exc)
            {
                var s = exc.Message;
                return string.Empty;
            }
        }

        public static List<TrackNameEnum> GetSwedishTracks()
        {
            var trackNameList = new List<TrackNameEnum>();
            for (var i = 5; i < 50; i++)
            {
                switch (i)
                {
                    // Strunta i specialevenemang...
                    case 20:
                    case 30:
                    case 40:
                    case 47:
                    case 49:
                        break;
                    default:
                        trackNameList.Add((TrackNameEnum)i);
                        break;
                }
            }
            return trackNameList;
        }

        public static List<BetTypeEnum> GetSupportedBetTypes()
        {
            var betTypeList = new List<BetTypeEnum>();
            for (var i = 0; i < 13; i++)
            {
                switch ((BetTypeEnum)i)
                {
                    case BetTypeEnum.V3:
                    case BetTypeEnum.Trio:
                    case BetTypeEnum.VP:
                    case BetTypeEnum.Komb:
                    case BetTypeEnum.Raket:
                        break;
                    default:
                        betTypeList.Add((BetTypeEnum)i);
                        break;
                }
            }
            return betTypeList;
        }

        public static string GetTrackNameATGSEFromShortString(string trackCode)
        {
            if (trackCode.Length > 2)
            {
                var trackName = trackCode.ToLower();
                trackName = trackName.Replace("�", "a");
                trackName = trackName.Replace("�", "a");
                trackName = trackName.Replace("�", "o");
                return trackName;
            }
            for (var i = 1; i < 99; i++)
            {
                if (((TrackCodeEnum)i).ToString() == trackCode)
                {
                    var trackName = ((TrackNameEnum)i).ToString().ToLower();
                    trackName = trackName.Replace("�", "a");
                    trackName = trackName.Replace("�", "a");
                    trackName = trackName.Replace("�", "o");
                    return trackName;
                }
            }
            return string.Empty;
        }

        public static TrackNameEnum GetTrackNameFromShortString(string trackCode)
        {
            for (var i = 1; i < 99; i++)
            {
                if (((TrackCodeEnum)i).ToString() == trackCode)
                {
                    return (TrackNameEnum)i;
                }
            }
            for (var i = 1; i < 99; i++)
            {
                if (((TrackNameEnum)i).ToString() == trackCode)
                {
                    return (TrackNameEnum)i;
                }
            }
            return TrackNameEnum.Okänd;
        }

        public static string GetTrackNameATGSEFromId(int trackId)
        {
            //if (trackId < 1)
            //{                
            //    return string.Empty;
            //}
            var trackName = ((TrackNameEnum)trackId).ToString().ToLower();
            trackName = trackName.Replace("�", "a");
            trackName = trackName.Replace("�", "a");
            trackName = trackName.Replace("�", "o");
            return trackName;
        }

        public static TrackCodeEnum GetTrackCodeFromShortString(string trackCode)
        {
            for (var i = 1; i < 99; i++)
            {
                if (((TrackCodeEnum)i).ToString() == trackCode)
                {
                    return (TrackCodeEnum)i;
                }
            }
            return TrackCodeEnum.NN;
        }

        public static string GetIntBankodFromBankod(TrackCodeEnum trackCode)
        {
            return ((int)trackCode).ToString();
        }

        public static BetTypeEnum GetBetTypeFromString(string betTypeString)
        {
            for (var i = 0; i < 12; i++)
            {
                if (((BetTypeEnum)i).ToString() == betTypeString)
                {
                    return (BetTypeEnum)i;
                }
            }
            switch (betTypeString)
            {
                case "TV":
                    return BetTypeEnum.Tvilling;
                case "T":
                    return BetTypeEnum.Trio;
                case "LK":
                    return BetTypeEnum.Komb;
                default:
                    break;
            }
            return BetTypeEnum.VP;
        }

        public static HPTPrio GetHPTPrioFromShortString(string prioString)
        {
            switch (prioString)
            {
                case "A":
                    return HPTPrio.A;
                case "B":
                    return HPTPrio.B;
                case "C":
                    return HPTPrio.C;
                case "D":
                    return HPTPrio.D;
                case "E":
                    return HPTPrio.E;
                case "F":
                    return HPTPrio.F;
                default:
                    return HPTPrio.M;
            }
        }

        public static ABCDPriorityEnum GetPriorityFromShortString(string priorityString)
        {
            for (var i = 0; i < 5; i++)
            {
                if (((ABCDPriorityEnum)i).ToString() == priorityString)
                {
                    return (ABCDPriorityEnum)i;
                }
            }
            return ABCDPriorityEnum.EJ;
        }

        public static string GetLongStringFromBetType(BetTypeEnum betType)
        {
            switch (betType)
            {
                case BetTypeEnum.DD:
                    return "Dagens Dubbel";
                case BetTypeEnum.LD:
                    return "Lunchdubbel";
                case BetTypeEnum.VP:
                    return "Vinnare/Plats";
                case BetTypeEnum.Komb:
                    return "Komben";
                default:
                    return betType.ToString();
            }
            //return "Ok�nd";
        }

        public static string GetStringFromSpeltyp(BetTypeEnum betType)
        {
            for (var i = 0; i < 12; i++)
            {
                if ((BetTypeEnum)i == betType)
                {
                    return ((BetTypeEnum)i).ToString();
                }
            }
            switch (betType)
            {
                case BetTypeEnum.Tvilling:
                    return "TV";
                case BetTypeEnum.Trio:
                    return "T";
                case BetTypeEnum.Komb:
                    return "LK";
                default:
                    break;
            }
            return "V5";
        }
    }
}

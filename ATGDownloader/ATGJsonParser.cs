using ATGDownloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ATGDownloader
{
    public class ATGJsonParser
    {
        //public ParseATGJson() { }

        #region Kalender

        internal ATGGameInfoBase ParseCalendarFile(string fileName)
        {
            using var reader = new StreamReader(fileName);
            var json = reader.ReadToEnd();
            return ParseCalendar(json);
        }

        public static ATGGameInfoBase ParseCalendar(string jsonContent)
        {
            var doc = JsonDocument.Parse(jsonContent);
            var gamesElement = doc.RootElement.GetProperty("games");
            var game = GetMainGame(gamesElement);
            var tracksElement = doc.RootElement.GetProperty("tracks");
            var tracks = tracksElement.EnumerateArray().Select(t => new
            {
                Id = t.GetProperty("id").GetInt32(),
                Name = t.GetProperty("name").GetString()
            });
            game.Tracks
                .ToList()
                .ForEach(track => track.TrackName = tracks.FirstOrDefault(t => t.Id == track.TrackId)?.Name);

            return game;
        }

        internal static ATGGameInfoBase GetMainGame(JsonElement gamesElement)
        {
            IEnumerable<string> mainGames = ["V75", "V85", "V86", "GS75", "V64"];

            foreach (var game in mainGames)
            {
                if (gamesElement.TryGetProperty(game, out var mainGameArray))
                {
                    var mainGame = mainGameArray.EnumerateArray().First();
                    return new ATGGameInfoBase()
                    {
                        Code = game,
                        Id = mainGame.GetProperty("id").GetString(),
                        Status = mainGame.GetProperty("status").GetString(),
                        StartTime = mainGame.GetProperty("startTime").GetDateTime(),
                        ScheduledStartTime = mainGame.GetProperty("scheduledStartTime").GetDateTime(),
                        Tracks = mainGame.GetProperty("tracks")
                            .EnumerateArray()
                            .Select(t => new ATGTrack() { TrackId = t.GetInt32() })
                            .ToArray(),
                        Races = mainGame.GetProperty("races")
                            .EnumerateArray()
                            .Select(t => t.GetString())
                            .ToArray()
                    };
                }
            }
            return new ATGGameInfoBase();
        }
        #endregion

        #region Startlistor
        internal ATGGameBase ParseGameFile(string fileName, ATGGameInfoBase gameInfoBase)
        {
            using var reader = new StreamReader(fileName);
            var json = reader.ReadToEnd();
            return ParseGameJson(json, gameInfoBase);
        }

        public static ATGGameBase ParseGameJson(string jsonContent, ATGGameInfoBase gameInfoBase)
        {
            var doc = JsonDocument.Parse(jsonContent);
            var game = ParseGame(doc.RootElement, gameInfoBase);
            return game;
        }

        internal static ATGGameBase ParseGame(JsonElement gameElement, ATGGameInfoBase gameInfoBase)
        {
            var gameInfo = new ATGGameBase(gameInfoBase)
            {
                GameInfo = gameInfoBase,
                Races = gameElement.GetProperty("races").EnumerateArray().Select(ge => ParseRace(ge, gameInfoBase.Code))
            };
            var mainPoolElement = gameElement.GetProperty("pools").GetProperty(gameInfoBase.Code);
            gameInfo.Turnover = mainPoolElement.GetProperty("turnover").GetDecimal();
            gameInfo.SystemCount = mainPoolElement.GetProperty("systemCount").GetInt32();            
            gameInfo.Status = mainPoolElement.GetProperty("status").GetString();
            gameInfo.ATGTimestamp = DateTime.Parse(mainPoolElement.GetProperty("timestamp").GetString());
            if (mainPoolElement.TryGetProperty("jackpotAmount", out var jackpotElement))
            {
                gameInfo.JackpotAmount = jackpotElement.GetInt32();
            }
            if (gameElement.TryGetProperty("version", out var versionElement))
            {
                gameInfo.Version = versionElement.GetInt64();
            }

            return gameInfo;
        }


        internal static ATGRaceBase ParseRace(JsonElement raceElement, string gameCode)
        {
            var race = new ATGRaceBase()
            {
                Id = raceElement.GetProperty("id").GetString(),
                Distance = raceElement.GetProperty("distance").GetInt32(),
                StartTime = raceElement.GetProperty("startTime").GetDateTime(),
                ScheduledStartTime = raceElement.GetProperty("scheduledStartTime").GetDateTime(),
                Number = raceElement.GetProperty("number").GetInt32(),
                Status = raceElement.GetProperty("status").GetString(),
                StartList = raceElement.GetProperty("starts")
                    .EnumerateArray()
                    .Select(se => ParseStart(se, gameCode))
                    .ToArray(),
            };


            return race;
        }

        internal static ATGStartBase ParseStart(JsonElement startElement, string gameCode)
        {
            var start = new ATGStartBase()
            {
                Id = startElement.GetProperty("id").GetString(),
                Horse = ParseHorse(startElement.GetProperty("horse")),
                Number = startElement.GetProperty("number").GetInt32(),
            };

            if (startElement.TryGetProperty("scratched", out var scratchedElement))
            {
                start.Scratched = scratchedElement.GetBoolean();
            }

            var poolsElement = startElement.GetProperty("pools");
            start.VinnarOdds = poolsElement.GetProperty("vinnare").GetProperty("odds").GetDecimal() / 100M;
            start.BetDistribution = poolsElement.GetProperty(gameCode).GetProperty("betDistribution").GetDecimal();
            start.Trend = poolsElement.GetProperty(gameCode).GetProperty("trend").GetDecimal();
            start.BetDistributionShare = start.BetDistribution / 10000M;

            return start;
        }

        internal static ATGHorseBase ParseHorse(JsonElement horseElement)
        {
            var horse = new ATGHorseBase()
            {
                Name = horseElement.GetProperty("name").GetString(),
            };


            return horse;
        }

        #endregion

        #region Hjälpmetoder



        #endregion
    }
}

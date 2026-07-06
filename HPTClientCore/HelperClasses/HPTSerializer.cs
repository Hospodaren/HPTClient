//using ICSharpCode.SharpZipLib.Zip;
using ATGDownloader;
using System.Collections.ObjectModel;
using System.IO;
//using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace HPTClient
{
    class HPTSerializer
    {
        internal static bool SerializeHPTObject(Type typeOfObject, string fileName, object hptObject)
        {
            try
            {
                using var fileStream = File.OpenWrite(fileName);
                var serializer = new DataContractSerializer(typeOfObject);
                serializer.WriteObject(fileStream, hptObject);

                return true;
            }
            catch (Exception exc)
            {
                var s = exc.Message;
            }
            return false;
        }

        internal static object DeserializeHPTObject(Type typeOfObject, string fileName)
        {
            try
            {
                var serializer = new DataContractSerializer(typeOfObject);
                var stream = File.OpenRead(fileName);
                var response = serializer.ReadObject(stream);
                return response;
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            return null;
        }

        #region Serialisering/deserialisering av vanliga HPT-objekt

        internal static HPTConfig DeserializeHPTConfig(string fileName)
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(HPTConfig));
                using var stream = File.OpenRead(fileName);
                var response = serializer.ReadObject(stream);
                return (HPTConfig)response;
            }
            catch (Exception exc)
            {
                return HPTConfig.ResetHPTConfig();
            }
        }

        //internal static HPTConfig DeserializeOldHPTConfig(string fileName)
        //{
        //    Stream stream = UnzipAndCreateStream(fileName);
        //    XmlSerializer serializer = new XmlSerializer(typeof(HPTConfig));
        //    XmlTextReader xtr = new XmlTextReader(stream);
        //    HPTConfig hptConfig = (HPTConfig)serializer.Deserialize(xtr);
        //    xtr.Close();
        //    xtr = null;
        //    return hptConfig;
        //}

        internal static void SerializeHPTConfig(string fileName, HPTConfig hptConfig)
        {
            SerializeHPTObject(typeof(HPTConfig), fileName, hptConfig);
        }

        internal static HPTMarkBet DeserializeHPTSystem(string fileName)
        {
            var stream = File.OpenRead(fileName);
            HPTMarkBet hmb = null;

            var fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);
            if (fileExtension == "hpt7")
            {
                var serializer = new DataContractSerializer(typeof(HPTMarkBet));
                hmb = (HPTMarkBet)serializer.ReadObject(stream);
            }
            hmb.Config = HPTConfig.Config;

            return hmb;
        }

        internal static void SerializeHPTSystem(string fileName, HPTMarkBet hmb)
        {
            SerializeHPTObject(typeof(HPTMarkBet), fileName, hmb);
            hmb.LastSaveTime = DateTime.Now;

            try
            {
                _ = Task.Run(() => HPTConfig.Config.UpdateHPTSystemDirectories());
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        internal static void SerializeHPTRaceDayInfoHistory(HPTMarkBet hmb)
        {
            if (hmb.RaceDayInfo.RaceDayDate.Date > DateTime.Today)
            {
                return;
            }
            try
            {
                var raceDayInfoHistory = new HPTRaceDayInfoHistory()
                {
                    BetTypeCode = hmb.BetType.Code,
                    RaceDayDate = hmb.RaceDayInfo.RaceDayDate,
                    Timestamp = DateTime.Now,
                    TrackId = hmb.RaceDayInfo.TrackId,
                    TrackName = hmb.RaceDayInfo.Trackname,
                    Turnover = hmb.RaceDayInfo.Turnover,
                    RaceList = hmb.RaceDayInfo.RaceList.Select(r => new HPTRaceHistory()
                    {
                        LegNumber = r.LegNr,
                        RaceNumber = r.RaceNr,
                        HorseList = r.HorseList.Select(h => new HPTHorseHistory()
                        {
                            Name = h.HorseName,
                            StartNumber = h.StartNr,
                            ATGTrend = h.ATGTrend,
                            StakeShare = h.StakeDistributionShare,
                            VinnarOdds = h.VinnarOdds,
                            PlatsOdds = h.MinPlatsOdds,
                        })
                    })
                };

                var dirName = Path.GetDirectoryName(hmb.SaveDirectory);
                dirName = Path.Combine(dirName, "Historik");
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                var fileName = Path.Combine(dirName, $"{hmb.BetType.Code}_{DateTime.Now:yyyyMMddHHmmss}.xml");

                SerializeHPTObject(typeof(HPTRaceDayInfoHistory), fileName, raceDayInfoHistory);
            }
            catch (Exception exc)
            {
                var s = exc.Message;
            }
        }

        internal static string SerializeHPTRaceDayInfoHistory(ATGGameBase game, string saveDirectory)
        {
            try
            {
                var raceDayInfoHistory = new HPTRaceDayInfoHistory()
                {
                    BetTypeCode = game.GameInfo.Code,
                    RaceDayDate = game.GameInfo.ScheduledStartTime.Date,
                    Timestamp = DateTime.Now,
                    TrackId = game.GameInfo.BetTrack.TrackId,
                    TrackName = game.GameInfo.BetTrack.TrackName,
                    Turnover = game.Turnover,
                    RaceList = game.Races.Select(r => new HPTRaceHistory()
                    {
                        LegNumber = (int)r.LegNumber,
                        RaceNumber = r.Number,
                        HorseList = r.StartList.Select(s => new HPTHorseHistory()
                        {
                            Name = s.Horse.Name,
                            StartNumber = s.Number,
                            ATGTrend = s.Trend,
                            StakeShare = s.BetDistributionShare,
                            VinnarOdds = s.VinnarOdds,
                            PlatsOdds = s.PlatsOdds,
                        })
                    })
                };

                var dirName = Path.Combine(saveDirectory, "Historik");
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                var fileName = Path.Combine(dirName, $"{game.GameInfo.Code}_{DateTime.Now:yyyyMMddHHmmss}.xml");

                SerializeHPTObject(typeof(HPTRaceDayInfoHistory), fileName, raceDayInfoHistory);
                return fileName;
            }
            catch (Exception exc)
            {
                return string.Empty;
            }
        }

        // TODO: Fixa kombinationsspel i framtiden...
        //internal static HPTCombBet DeserializeHPTCombinationSystem(string fileName)
        //{
        //    Stream stream = UnzipAndCreateStream(fileName);
        //    HPTCombBet hcb = null;

        //    string fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);
        //    if (fileExtension == "hpt5")
        //    {
        //        var serializer = new DataContractSerializer(typeof(HPTCombBet));
        //        hcb = (HPTCombBet)serializer.ReadObject(stream);
        //    }
        //    else if (fileExtension == "hpt4")
        //    {
        //        var serializer = new XmlSerializer(typeof(HPTCombBet));
        //        var xtr = new XmlTextReader(stream);
        //        hcb = (HPTCombBet)serializer.Deserialize(xtr);
        //        xtr.Close();
        //        xtr = null;
        //    }

        //    hcb.Config = HPTConfig.Config;
        //    HPTServiceToHPTHelper.SetNonSerializedValues(hcb);
        //    hcb.IsDeserializing = false;
        //    hcb.RecalculateAllRanks();
        //    hcb.RecalculateRank();

        //    return hcb;
        //}

        internal static void SerializeHPTCombinationSystem(string fileName, HPTCombBet hcb)
        {
            SerializeHPTObject(typeof(HPTCombBet), fileName, hcb);
        }

        internal static HPTCalendar DeserializeHPTCalendar(string fileName)
        {
            try
            {
                var o = DeserializeHPTObject(typeof(HPTCalendar), fileName);
                var hptCalendar = (HPTCalendar)o;
                return hptCalendar;
            }
            catch (Exception)
            {
            }
            return new HPTCalendar();
        }

        internal static void SerializeHPTCalendar(string fileName, HPTCalendar hptCalendar)
        {
            SerializeHPTObject(typeof(HPTCalendar), fileName, hptCalendar);
        }

        internal static HPTHorseOwnInformationCollection DeserializeHPTHorseOwnInformation(string fileName)
        {
            try
            {
                var stream = File.OpenRead(fileName);
                var serializer = new DataContractSerializer(typeof(HPTHorseOwnInformationCollection));
                var response = serializer.ReadObject(stream);
                var horseOwnInformationCollection = (HPTHorseOwnInformationCollection)response;

                stream.Flush();
                stream.Close();

                return horseOwnInformationCollection;
            }
            catch (InvalidOperationException)
            {
                var stream = File.OpenRead(fileName);
                var serializer = new XmlSerializer(typeof(HPTHorseOwnInformationCollection));
                var xtr = new XmlTextReader(stream);
                var hptHorseOwnInformation = (HPTHorseOwnInformationCollection)serializer.Deserialize(xtr);
                xtr.Close();
                return hptHorseOwnInformation;
            }
            catch (Exception)
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Copy(fileName, $"{fileName}.OLD", true);
                    }
                    catch (Exception)
                    {

                    }
                }
                return new HPTHorseOwnInformationCollection()
                {
                    HorseOwnInformationList = new ObservableCollection<HPTHorseOwnInformation>()
                };
            }
        }

        internal static void SerializeHPTHorseOwnInformation(string fileName, HPTHorseOwnInformationCollection hptHorseOwnInformation)
        {
            SerializeHPTObject(typeof(HPTHorseOwnInformationCollection), fileName, hptHorseOwnInformation);
        }

        #endregion

        internal static HPTCalendar GetCalendarFromFile()
        {
            try
            {
                var hptFiles = Directory.GetFiles(HPTConfig.MyDocumentsPath, "HPT7Calendar.xml");
                if (hptFiles.Length > 0)
                {
                    var hptCalendar = DeserializeHPTCalendar(hptFiles[0]);

                    // Ta bort gamla tävlingar och sortera stigande efter datum
                    if (hptCalendar.RaceDayInfoList != null)
                    {
                        var orderedRaceDayInfoList = hptCalendar.RaceDayInfoList
                            .Where(rdi => rdi.RaceDayDate > DateTime.Now.AddHours(-14))
                            .OrderBy(rdi => rdi.RaceDayDate);

                        //// Ta bort de spelformer som inte ingår i gratisversionen
                        //if (!HPTConfig.Config.IsPayingCustomer)
                        //{
                        //    foreach (var raceDayInfo in orderedRaceDayInfoList)
                        //    {
                        //        raceDayInfo.BetTypeList = raceDayInfo.BetTypeList
                        //                .Where(bt => bt.Code == "V65" || bt.Code == "V75" || bt.Code == "V86" || bt.Code == "V64")
                        //                .ToList();
                        //    }
                        //}

                        // Ta bara med de tävlingar där det finns spelbara spelformer
                        var finalRaceDayInfoList = orderedRaceDayInfoList
                            .Where(rdi => rdi.BetTypeList.Count > 0);

                        hptCalendar.RaceDayInfoList = new ObservableCollection<HPTRaceDayInfo>(finalRaceDayInfoList);

                        hptCalendar.FromDate = DateTime.Now;
                        //hptCalendar.FromDateString = hptCalendar.FromDate.ToString("yyyy-MM-dd");
                        if (hptCalendar.RaceDayInfoList.Count == 0)
                        {
                            return null;
                        }
                    }
                    return hptCalendar;
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            return null;
        }

        #region Trendsiffor

        public static List<(string FileName, DateTime Timestamp)> GetTrendsFromDisk(HPTMarkBet markBet)
        {
            var historyDir = Path.Combine(markBet.SaveDirectory, "Historik");
            var trendFiles = new List<(string FileName, DateTime Timestamp)>();
            if (!Directory.Exists(historyDir))
            {
                Directory.CreateDirectory(historyDir);
                return trendFiles;
            }
            if (markBet.RaceDayInfo.Turnover == 0)
            {
                return trendFiles;
            }

            var rexTimestampFromFileName = new Regex($@"{markBet.BetType.Code}_(\d{{14}})");

            Directory.GetFiles(historyDir, $"{markBet.BetType.Code}*.xml")
                .ToList()
                .ForEach(f =>
                {
                    if (rexTimestampFromFileName.IsMatch(f))
                    {
                        var fileTimeStamp = DateTime.ParseExact(rexTimestampFromFileName.Match(f).Groups[1].Value, "yyyyMMddHHmmss", null);
                        // TODO: ShortTrend ska åtminstone vara en stund (30 minuter?) gammal
                        if (fileTimeStamp.Date == markBet.RaceDayInfo.RaceDayDate.Date)
                        {
                            trendFiles.Add(new(f, fileTimeStamp));
                        }
                    }
                });

            if (trendFiles.Any())
            {
                var shortDiff = 0.667M;
                var longDiff = 0.333M;

                var allRaceDayInfoHistory = trendFiles
                    .Select(tf => (HPTRaceDayInfoHistory)DeserializeHPTObject(typeof(HPTRaceDayInfoHistory), tf.FileName))
                    .Where(h => h != null);
                var raceDayInfoHistoryLongTrend = allRaceDayInfoHistory
                    .OrderBy(rdi => Math.Abs(rdi.Turnover / markBet.RaceDayInfo.Turnover - longDiff))
                    .First();

                var raceDayInfoHistoryShortTrend = allRaceDayInfoHistory
                    .OrderBy(rdi => Math.Abs(rdi.Turnover / markBet.RaceDayInfo.Turnover - shortDiff))
                    .First();

                markBet.RaceDayInfo.RaceList.ToList().ForEach(r =>
                {
                    var raceLong = raceDayInfoHistoryLongTrend.RaceList.First(rl => rl.RaceNumber == r.RaceNr);
                    var raceShort = raceDayInfoHistoryShortTrend.RaceList.First(rs => rs.RaceNumber == r.RaceNr);
                    r.HorseList.ToList().ForEach(h =>
                    {
                        var horseLong = raceLong.HorseList.First(hl => hl.StartNumber == h.StartNr);
                        var horseShort = raceShort.HorseList.First(hl => hl.StartNumber == h.StartNr);

                        h.LongTrend = h.StakeDistributionShare / horseLong.StakeShare - 1M;
                        h.ShortTrend = h.StakeDistributionShare / horseShort.StakeShare - 1M;

                    });
                    r.CalculateDynamicGameValues();
                });
            }

            return trendFiles;
        }

        #endregion

        #region Rankvariabelmallar

        internal static HPTTemplateCollection DeserializeHPTTemplateCollection(string fileName)
        {
            Stream stream = File.OpenRead(fileName);
            HPTTemplateCollection templateCollection = null;

            var fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);
            if (fileExtension == "hpt5m")
            {
                var serializer = new DataContractSerializer(typeof(HPTTemplateCollection));
                templateCollection = (HPTTemplateCollection)serializer.ReadObject(stream);
            }
            else if (fileExtension == "hptm")
            {
                var serializer = new XmlSerializer(typeof(HPTTemplateCollection));
                var xtr = new XmlTextReader(stream);
                templateCollection = (HPTTemplateCollection)serializer.Deserialize(xtr);
                xtr.Close();
                xtr = null;
            }
            return templateCollection;
        }

        //internal static HPTTemplateCollection DeserializeHPTTemplateCollection(byte[] templateCollectionZip)
        //{
        //    Stream stream = UnzipAndCreateStream(templateCollectionZip);
        //    XmlSerializer serializer = new XmlSerializer(typeof(HPTTemplateCollection));
        //    XmlTextReader xtr = new XmlTextReader(stream);
        //    var hptTemplateCollection = (HPTTemplateCollection)serializer.Deserialize(xtr);
        //    xtr.Close();
        //    xtr = null;
        //    return hptTemplateCollection;
        //}

        internal static void SerializeHPTTemplateCollection(string fileName, HPTTemplateCollection hptTemplateCollection)
        {
            SerializeHPTObject(typeof(HPTTemplateCollection), fileName, hptTemplateCollection);
        }

        internal static object CreateDeepCopy(object o)
        {
            // Skriv objektet till en ström
            var ms = new MemoryStream();
            var serializer = new DataContractSerializer(o.GetType());
            serializer.WriteObject(ms, o);
            ms.Position = 0;

            // Läs tillbaka det från strömmen och vips så har du en kopia!
            var clone = serializer.ReadObject(ms);
            return clone;
        }

        #endregion

        #region Resultatanalys

        internal static ObservableCollection<HPTResultAnalyzer> DeserializeHPTResultAnalyzerList(string fileName)
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(ObservableCollection<HPTResultAnalyzer>));
                var stream = File.OpenRead(fileName);
                var response = serializer.ReadObject(stream);
                var resultAnalyzerList = (ObservableCollection<HPTResultAnalyzer>)response;
                return resultAnalyzerList;
            }
            catch (Exception exc)
            {
                var s = exc.Message;
            }
            return null;
        }

        internal static void SerializeHPTResultAnalyzerList(string fileName, ObservableCollection<HPTResultAnalyzer> resultAnalyzerList)
        {
            SerializeHPTObject(typeof(ObservableCollection<HPTResultAnalyzer>), fileName, resultAnalyzerList);
        }

        #endregion

        #region Json

        internal static string CreateJson(object o)
        {
            var jsonSerializer = new DataContractJsonSerializer(o.GetType());
            var ms = new MemoryStream();
            jsonSerializer.WriteObject(ms, o);
            ms.Position = 0;
            var sr = new StreamReader(ms);
            var jsonString = sr.ReadToEnd();
            return jsonString;
        }

        internal static object DeserializeJson(Type type, string json)
        {
            var settings = new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            };
            var jsonSerializer = new DataContractJsonSerializer(type, settings);
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(json);
            sw.Flush();
            ms.Position = 0;
            var o = jsonSerializer.ReadObject(ms);

            return o;
        }

        #endregion
    }
}

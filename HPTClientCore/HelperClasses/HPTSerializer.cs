using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
//using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace HPTClient
{
    class HPTSerializer
    {
        #region Generell Zip-funktionalitet

        // Lösenord för zip-filer
        private const string ZipKey = "455807A1-440B-4F50-821F-AA5584D174D3";

        internal static object DeserializeObjectFromStream(Type typeOfObject, Stream stream)
        {
            var serializer = new DataContractSerializer(typeOfObject);
            var response = serializer.ReadObject(stream);
            return response;
        }

        internal static Stream SerializeHPTServiceObject(Type objectType, object hptObject)
        {
            var ms = new MemoryStream();
            var serializer = new DataContractSerializer(objectType);
            serializer.WriteObject(ms, hptObject);
            ms.Position = 0;
            return ms;
        }

        internal static object DeserializeHPTServiceObject(Type typeOfObject, byte[] binaryZip)
        {
            var serializer = new DataContractSerializer(typeOfObject);
            var stream = UnzipAndCreateStream(binaryZip);
            var response = serializer.ReadObject(stream);
            return response;
        }

        internal static object DeserializeHPTServiceObject(Type typeOfObject, Stream stream)
        {
            var serializer = new DataContractSerializer(typeOfObject);
            var response = serializer.ReadObject(stream);
            return response;
        }

        internal static bool SerializeHPTObject(Type typeOfObject, string fileName, object hptObject)
        {
            try
            {
                var ms = new MemoryStream();
                var serializer = new DataContractSerializer(typeOfObject);
                serializer.WriteObject(ms, hptObject);
                ms.Position = 0;
                ZipAndCreateFile(ms, fileName);
                ms.Flush();
                ms.Close();
                ms = null;

                return true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return false;
        }

        internal static object DeserializeHPTObject(Type typeOfObject, byte[] binaryZip)
        {
            var serializer = new DataContractSerializer(typeOfObject);
            var stream = UnzipAndCreateStream(binaryZip);
            var response = serializer.ReadObject(stream);
            return response;
        }

        internal static object DeserializeHPTObject(Type typeOfObject, string filename)
        {
            var serializer = new DataContractSerializer(typeOfObject);
            var stream = UnzipAndCreateStream(filename);
            var response = serializer.ReadObject(stream);
            return response;
        }

        internal static byte[] SerializeHPTObject(XmlSerializer serializer, object hptObject)
        {
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, hptObject);
            ms.Position = 0;
            var baObject = ZipAndCreateBinary(ms);
            return baObject;
        }
        internal static Stream Decompress(string zipFilePath)
        {
            var zipInputStream = new ZipInputStream(File.OpenRead(zipFilePath))
            {
                Password = ZipKey
            };

            var entry = zipInputStream.GetNextEntry();
            byte[] buffer = new byte[entry.Size];
            zipInputStream.ReadExactly(buffer, 0, buffer.Length);
            var ms = new MemoryStream(buffer);
            ms.Position = 0;

            return ms;
        }

        internal static Stream UnzipAndCreateStream(byte[] binaryZip)
        {
            var msIn = new MemoryStream(binaryZip);
            msIn.Position = 0;
            var zipInputStream = new ZipInputStream(msIn)
            {
                Password = ZipKey
            };

            var entry = zipInputStream.GetNextEntry();
            byte[] buffer = new byte[entry.Size];
            zipInputStream.ReadExactly(buffer, 0, buffer.Length);
            var msOut = new MemoryStream(buffer);
            msOut.Position = 0;

            return msOut;
        }

        internal static byte[] ZipAndCreateBinary(Stream stream)
        {
            byte[] bytesIn = new byte[stream.Length];
            stream.ReadExactly(bytesIn, 0, bytesIn.Length);
            var ms = new MemoryStream();
            using (var zipOutputStream = new ZipOutputStream(ms))
            {
                zipOutputStream.Password = ZipKey;
                zipOutputStream.SetLevel(5); // Set compression level (0-9), 5 as a mid-range
                var entry = new ZipEntry("System"); // Create a new entry for each file
                zipOutputStream.PutNextEntry(entry);                
                zipOutputStream.Write(bytesIn, 0, bytesIn.Length);
                zipOutputStream.Finish();
                ms.Position = 0;
                byte[] buffer = ms.ToArray();
                zipOutputStream.Close();
                
                return buffer;
            }

            return null;
            //Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile()
            //{
            //    CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression,
            //    Password = ZipKey
            //};

            //MemoryStream ms = new MemoryStream();
            //zf.AddEntry("System", stream);
            //zf.Save(ms);
            //ms.Position = 0;
            //byte[] resultArray = ms.ToArray();
            //return resultArray;
        }

        internal static Stream UnzipAndCreateStream(string fileName)
        {
            try
            {                
                return Decompress(fileName);

                //Ionic.Zip.ZipFile zf = Ionic.Zip.ZipFile.Read(fileName);
                //Ionic.Zip.ZipEntry ze = zf.Entries.First();
                //MemoryStream outputStream = new MemoryStream();
                //ze.ExtractWithPassword(outputStream, ZipKey);
                //outputStream.Position = 0;
                //return outputStream;
                //return new MemoryStream();
            }
            catch (Exception)
            {
                var fs = new FileStream(fileName, FileMode.Open);
                return fs;
            }
        }

        internal static bool ZipAndCreateFile(Stream stream, string fileName)
        {
            byte[] bytesIn = new byte[stream.Length];
            stream.ReadExactly(bytesIn, 0, bytesIn.Length);
            var fs = new FileStream(fileName, FileMode.OpenOrCreate);
            using (var zipOutputStream = new ZipOutputStream(fs))
            {
                zipOutputStream.Password = ZipKey;
                zipOutputStream.SetLevel(5); // Set compression level (0-9), 5 as a mid-range
                var entry = new ZipEntry("System"); // Create a new entry for each file
                zipOutputStream.PutNextEntry(entry);
                zipOutputStream.Write(bytesIn, 0, bytesIn.Length);
                zipOutputStream.Finish();
                //ms.Position = 0;
                //byte[] buffer = ms.ToArray();
                zipOutputStream.Close();

                return true;
            }
            //try
            //{
            //    Ionic.Zip.ZipFile zf = new Ionic.Zip.ZipFile()
            //    {
            //        CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression,
            //        Password = ZipKey
            //    };

            //    zf.AddEntry("System", stream);
            //    zf.Save(fileName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        #endregion

        #region Serialisering/deserialisering av HPTService-objekt

        internal static HPTService.AuthenticationResponse DeserializeAuthenticationResponse(byte[] binaryZip)
        {
            var response = DeserializeHPTServiceObject(typeof(HPTService.AuthenticationResponse), binaryZip);
            return (HPTService.AuthenticationResponse)response;
        }

        internal static HPTService.HPTSaveSystem DeserializeHPTSaveSystem(byte[] binaryZip)
        {
            var response = DeserializeHPTServiceObject(typeof(HPTService.HPTSaveSystem), binaryZip);
            return (HPTService.HPTSaveSystem)response;
        }

        internal static HPTService.HPTRegistration DeserializeHPTRegistration(byte[] binaryZip)
        {
            var response = DeserializeHPTServiceObject(typeof(HPTService.HPTRegistration), binaryZip);
            return (HPTService.HPTRegistration)response;
        }

        internal static HPTService.HPTCalendar DeserializeHPTCalendar(byte[] binaryZip)
        {
            var response = DeserializeHPTServiceObject(typeof(HPTService.HPTCalendar), binaryZip);
            return (HPTService.HPTCalendar)response;
        }

        internal static HPTService.HPTRaceDayInfo DeserializeHPTRaceDayInfo(byte[] binaryZip)
        {
            var response = DeserializeHPTServiceObject(typeof(HPTService.HPTRaceDayInfo), binaryZip);
            return (HPTService.HPTRaceDayInfo)response;
        }

        internal static HPTService.HPTResultMarkingBet DeserializeHPTResultMarkingBet(byte[] binaryZip)
        {
            var response = DeserializeHPTServiceObject(typeof(HPTService.HPTResultMarkingBet), binaryZip);
            return (HPTService.HPTResultMarkingBet)response;
        }

        internal static HPTService.HPTUserRaceDayInfoCommentsCollection DeserializeHPTUserRaceDayInfoCommentsCollection(byte[] binaryZip)
        {
            var response = DeserializeHPTServiceObject(typeof(HPTService.HPTUserRaceDayInfoCommentsCollection), binaryZip);
            return (HPTService.HPTUserRaceDayInfoCommentsCollection)response;
        }

        //internal static byte[] SerializeHPTResultMarkingBet(HPTService.HPTResultMarkingBet hptRmb)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    XmlSerializer serializer = new XmlSerializer(typeof(HPTService.HPTResultMarkingBet));
        //    XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8);
        //    serializer.Serialize(xtw, hptRmb);
        //    xtw.Flush();
        //    ms.Position = 0;
        //    byte[] binaryZip = ZipAndCreateBinary(ms);
        //    xtw.Close();
        //    return binaryZip;
        //}

        //internal static List<HPTService.HPTRaceHistory> DeserializeHPTRaceHistoryList(byte[] binaryZip)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(List<HPTService.HPTRaceHistory>));
        //    Stream stream = UnzipAndCreateStream(binaryZip);
        //    XmlTextReader xtr = new XmlTextReader(stream);
        //    List<HPTService.HPTRaceHistory> hptRaceHistoryList = (List<HPTService.HPTRaceHistory>)serializer.Deserialize(xtr);
        //    xtr.Close();
        //    xtr = null;
        //    return hptRaceHistoryList;
        //}

        //internal static byte[] SerializeHPTRaceHistoryList(List<HPTRaceHistory> hptRaceHistoryList)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    XmlSerializer serializer = new XmlSerializer(typeof(List<HPTRaceHistory>));
        //    XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8);
        //    serializer.Serialize(xtw, hptRaceHistoryList);
        //    xtw.Flush();
        //    ms.Position = 0;
        //    byte[] binaryZip = ZipAndCreateBinary(ms);
        //    xtw.Close();
        //    return binaryZip;
        //}

        #endregion

        #region Serialisering/deserialisering av vanliga HPT-objekt

        internal static HPTConfig DeserializeHPTConfig(string fileName)
        {
            var serializer = new DataContractSerializer(typeof(HPTConfig));
            var stream = UnzipAndCreateStream(fileName);
            var response = serializer.ReadObject(stream);
            HPTConfig hptConfig = (HPTConfig)response;
            return hptConfig;
        }

        internal static HPTConfig DeserializeOldHPTConfig(string fileName)
        {
            Stream stream = UnzipAndCreateStream(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(HPTConfig));
            XmlTextReader xtr = new XmlTextReader(stream);
            HPTConfig hptConfig = (HPTConfig)serializer.Deserialize(xtr);
            xtr.Close();
            xtr = null;
            return hptConfig;
        }

        internal static void SerializeHPTConfig(string fileName, HPTConfig hptConfig)
        {
            //MemoryStream ms = new MemoryStream();
            //var serializer = new XmlSerializer(typeof(HPTConfig));
            //serializer.Serialize(ms, hptConfig);
            //ms.Position = 0;
            //ZipAndCreateFile(ms, fileName);
            SerializeHPTObject(typeof(HPTConfig), fileName, hptConfig);
        }

        internal static HPTMarkBet DeserializeHPTSystem(string fileName)
        {
            Stream stream = UnzipAndCreateStream(fileName);
            HPTMarkBet hmb = null;

            string fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);
            if (fileExtension == "hpt5")
            {
                var serializer = new DataContractSerializer(typeof(HPTMarkBet));
                hmb = (HPTMarkBet)serializer.ReadObject(stream);
            }
            else if (fileExtension == "hpt4")
            {
                var serializer = new XmlSerializer(typeof(HPTMarkBet));
                var xtr = new XmlTextReader(stream);
                hmb = (HPTMarkBet)serializer.Deserialize(xtr);
                xtr.Close();
                xtr = null;
            }

            hmb.Config = HPTConfig.Config;

            DateTime dt = DateTime.Now;
            HPTServiceToHPTHelper.SetNonSerializedValues(hmb);
            TimeSpan ts = DateTime.Now - dt;
            string s = ts.TotalMilliseconds.ToString();

            return hmb;
        }

        internal static void SerializeHPTSystem(string fileName, HPTMarkBet hmb)
        {
            SerializeHPTObject(typeof(HPTMarkBet), fileName, hmb);
            hmb.LastSaveTime = DateTime.Now;

            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(HPTConfig.Config.UpdateHPTSystemDirectories), ThreadPriority.Normal);
                //HPTConfig.Config.UpdateHPTSystemDirectories();
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        internal static HPTCombBet DeserializeHPTCombinationSystem(string fileName)
        {
            Stream stream = UnzipAndCreateStream(fileName);
            HPTCombBet hcb = null;

            string fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);
            if (fileExtension == "hpt5")
            {
                var serializer = new DataContractSerializer(typeof(HPTCombBet));
                hcb = (HPTCombBet)serializer.ReadObject(stream);
            }
            else if (fileExtension == "hpt4")
            {
                var serializer = new XmlSerializer(typeof(HPTCombBet));
                var xtr = new XmlTextReader(stream);
                hcb = (HPTCombBet)serializer.Deserialize(xtr);
                xtr.Close();
                xtr = null;
            }

            hcb.Config = HPTConfig.Config;
            HPTServiceToHPTHelper.SetNonSerializedValues(hcb);
            hcb.IsDeserializing = false;
            hcb.RecalculateAllRanks();
            hcb.RecalculateRank();

            return hcb;
        }

        internal static void SerializeHPTCombinationSystem(string fileName, HPTCombBet hcb)
        {
            SerializeHPTObject(typeof(HPTCombBet), fileName, hcb);
        }

        internal static HPTCalendar DeserializeHPTCalendar(string fileName)
        {
            try
            {
                object o = DeserializeHPTObject(typeof(HPTCalendar), fileName);
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
                var stream = UnzipAndCreateStream(fileName);
                var serializer = new DataContractSerializer(typeof(HPTHorseOwnInformationCollection));
                var response = serializer.ReadObject(stream);
                var horseOwnInformationCollection = (HPTHorseOwnInformationCollection)response;

                stream.Flush();
                stream.Close();
                stream = null;

                return horseOwnInformationCollection;
            }
            catch (InvalidOperationException)
            {
                var stream = UnzipAndCreateStream(fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(HPTHorseOwnInformationCollection));
                XmlTextReader xtr = new XmlTextReader(stream);
                var hptHorseOwnInformation = (HPTHorseOwnInformationCollection)serializer.Deserialize(xtr);
                xtr.Close();
                xtr = null;
                return hptHorseOwnInformation;
            }
            catch (Exception)
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Copy(fileName, fileName + ".OLD", true);
                    }
                    catch (Exception)
                    {

                    }
                }
                return new HPTHorseOwnInformationCollection()
                {
                    HorseOwnInformationList = new System.Collections.ObjectModel.ObservableCollection<HPTHorseOwnInformation>()
                };
            }
        }

        internal static HPTHorseOwnInformationCollection DeserializeHPTHorseOwnInformation(byte[] horseOwnInformationZip)
        {
            Stream stream = UnzipAndCreateStream(horseOwnInformationZip);
            XmlSerializer serializer = new XmlSerializer(typeof(HPTHorseOwnInformationCollection));
            XmlTextReader xtr = new XmlTextReader(stream);
            var hptHorseOwnInformation = (HPTHorseOwnInformationCollection)serializer.Deserialize(xtr);
            xtr.Close();
            xtr = null;
            return hptHorseOwnInformation;
        }

        internal static void SerializeHPTHorseOwnInformation(string fileName, HPTHorseOwnInformationCollection hptHorseOwnInformation)
        {
            //var fi = new FileInfo("");
            //fi.Replace
            SerializeHPTObject(typeof(HPTHorseOwnInformationCollection), fileName, hptHorseOwnInformation);
        }

        //internal static void SerializeHPTHorseOwnInformation(string fileName, HPTHorseOwnInformation horseOwnInformation)
        //{
        //    try
        //    {
        //        var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        //        var serializer = new DataContractSerializer(typeof(HPTHorseOwnInformation));
        //        serializer.WriteObject(fs, horseOwnInformation);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        internal static byte[] SerializeHPTRaceDayInfoCommentCollection(HPTRaceDayInfoCommentCollection hptRaceDayInfoCommentCollection)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HPTRaceDayInfoCommentCollection));
            var binaryZip = SerializeHPTObject(serializer, hptRaceDayInfoCommentCollection);
            return binaryZip;
        }

        internal static HPTRaceDayInfoCommentCollection DeserializeHPTRaceDayInfoCommentCollection(byte[] raceDayInfoCommentCollection)
        {
            Stream stream = UnzipAndCreateStream(raceDayInfoCommentCollection);
            XmlSerializer serializer = new XmlSerializer(typeof(HPTRaceDayInfoCommentCollection));
            XmlTextReader xtr = new XmlTextReader(stream);
            var hptRaceDayInfoCommentCollection = (HPTRaceDayInfoCommentCollection)serializer.Deserialize(xtr);
            xtr.Close();
            xtr = null;
            return hptRaceDayInfoCommentCollection;
        }

        #endregion

        internal static HPTCalendar GetCalendarFromFile()
        {
            try
            {
                string[] hptFiles = Directory.GetFiles(HPTConfig.MyDocumentsPath, "HPTCalendar.hptc");
                if (hptFiles.Length > 0)
                {
                    HPTCalendar hptCalendar = DeserializeHPTCalendar(hptFiles[0]);

                    // Ta bort gamla tävlingar och sortera stigande efter datum
                    if (hptCalendar.RaceDayInfoList != null)
                    {
                        IOrderedEnumerable<HPTRaceDayInfo> orderedRaceDayInfoList = hptCalendar.RaceDayInfoList
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
                        IEnumerable<HPTRaceDayInfo> finalRaceDayInfoList = orderedRaceDayInfoList
                            .Where(rdi => rdi.BetTypeList.Count > 0);

                        hptCalendar.RaceDayInfoList = new System.Collections.ObjectModel.ObservableCollection<HPTRaceDayInfo>(finalRaceDayInfoList);

                        hptCalendar.FromDate = DateTime.Now;
                        hptCalendar.FromDateString = hptCalendar.FromDate.ToString("yyyy-MM-dd");
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

        #region Rankvariabelmallar

        internal static HPTTemplateCollection DeserializeHPTTemplateCollection(string fileName)
        {
            Stream stream = UnzipAndCreateStream(fileName);
            HPTTemplateCollection templateCollection = null;

            string fileExtension = Path.GetExtension(fileName).Replace(".", string.Empty);
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

        internal static HPTTemplateCollection DeserializeHPTTemplateCollection(byte[] templateCollectionZip)
        {
            Stream stream = UnzipAndCreateStream(templateCollectionZip);
            XmlSerializer serializer = new XmlSerializer(typeof(HPTTemplateCollection));
            XmlTextReader xtr = new XmlTextReader(stream);
            var hptTemplateCollection = (HPTTemplateCollection)serializer.Deserialize(xtr);
            xtr.Close();
            xtr = null;
            return hptTemplateCollection;
        }

        internal static void SerializeHPTTemplateCollection(string fileName, HPTTemplateCollection hptTemplateCollection)
        {
            SerializeHPTObject(typeof(HPTTemplateCollection), fileName, hptTemplateCollection);
            //XmlSerializer serializer = new XmlSerializer(typeof(HPTTemplateCollection));
            //SerializeHPTObject(typeof(HPTTemplateCollection), fileName, hptTemplateCollection);
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
                var stream = UnzipAndCreateStream(fileName);
                var response = serializer.ReadObject(stream);
                var resultAnalyzerList = (ObservableCollection<HPTResultAnalyzer>)response;
                return resultAnalyzerList;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
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
            string jsonString = sr.ReadToEnd();
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
            object o = jsonSerializer.ReadObject(ms);

            return o;
        }

        #endregion
    }
}

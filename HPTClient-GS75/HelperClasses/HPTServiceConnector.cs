using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ServiceModel;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

namespace HPTClient
{
    public class HPTServiceConnector
    {
        #region local variables used for threading

        //private static string webserviceURL = "http://cloud1.hpt.nu/HPTService.svc";
        //private static string webserviceURL = "http://cloud2.hpt.nu/HPTService.svc";
        //private static string webserviceURL = "http://localhost:8731/Design_Time_Addresses/HPTServiceLibrary/Service1/HPTRestService";
        //private static string webserviceURL = "http://hospodaren.asuscomm.com/HPTService.svc";  // 85.228.139.26    
        //private static string webserviceURL = "http://85.228.139.26/HPTService.svc";  // 85.228.139.26
        //private static string webserviceURLAlt = "http://hospodaren.asuscomm.com/HPTService.svc";

        private static IEnumerable<string> webserviceURLList = new List<string>()
        {
            "http://cloud1.hpt.nu/HPTService.svc",
            "http://cloud2.hpt.nu/HPTService.svc",
            "http://hospodaren.zapto.org/HPTService.svc",
            "http://hospodar.asuscomm.com/HPTService.svc",
        };

        HPTBetType betType;
        int trackId;
        DateTime raceDate;

        Action<HPTRaceDayInfo> rdiDelegate;
        Action<HPTService.HPTRaceDayInfo> updateDelegate;
        Action<HPTService.HPTResultMarkingBet> resultDelegate;
        Action<HPTService.HPTRaceDayInfoHistoryInfoGrouped> historyDelegate;
        
        #endregion

        #region Hantering av nyheter på siten


        //internal static string LatestNewsHeadline { get; set; }
        //internal static bool HasNewsSinceLastUse()
        //{
        //    try
        //    {
        //        string rssXml = GetRSSFeed();
        //        XmlDocument xd = new XmlDocument();
        //        xd.LoadXml(rssXml);

        //        XmlNamespaceManager nsm = new XmlNamespaceManager(xd.NameTable);
        //        nsm.AddNamespace("ns", "http://www.w3.org/2005/Atom");

        //        XmlNode xn = xd.DocumentElement.SelectSingleNode("//ns:entry", nsm);
        //        XmlNode xnPublished = xn.SelectSingleNode("//ns:published", nsm);
        //        XmlNode xnTitle = xn.SelectSingleNode("ns:title", nsm);

        //        DateTime published = ParseDate(xnPublished.InnerText);
        //        LatestNewsHeadline = xnTitle.InnerText;

        //        return published > HPTConfig.Config.LastUse;
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //        return false;
        //    }
        //}

        //internal static string GetRSSFeed()
        //{
        //    string rss = "http://www.hpt.nu/nyheter/posts.xml";
        //    var wc = new WebClient();

        //    var utf8 = new UTF8Encoding();
        //    string requestXml = string.Empty;
        //    try
        //    {
        //        requestXml = utf8.GetString(wc.DownloadData(rss));
        //    }
        //    catch (WebException we)
        //    {
        //        string s = we.Message;
        //    }
        //    return requestXml;
        //}

        //private static DateTime ParseDate(string s)
        //{
        //    DateTime result;
        //    if (!DateTime.TryParse(s, out result))
        //    {
        //        result = DateTime.ParseExact(s, "yyyy-MM-ddT24:mm:ssK", System.Globalization.CultureInfo.InvariantCulture);
        //        result = result.AddDays(1);
        //    }
        //    return result;
        //}

        #endregion

        internal bool GetHorseNextStartList()
        {
            try
            {
                var atgIdList = HPTConfig.Config.HorseOwnInformationCollection.HorseOwnInformationList
                    .Where(oi => !string.IsNullOrEmpty(oi.ATGId))
                    .Where(oi => oi.NextStart == null || oi.NextStart.StartDate < DateTime.Now)
                    .Select(h => Convert.ToInt32(h.ATGId))
                    .Where(ai => ai > 0)
                    .ToList();

                var atgIdStream = HPTSerializer.SerializeHPTServiceObject(typeof(IEnumerable<int>), atgIdList);
                var request = CreateWebRequestForPOST("NextStartForHorses", atgIdStream);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                string strData = sr.ReadToEnd();
                var rexBinary = new Regex(">(.+?)<");
                strData = rexBinary.Match(strData).Groups[1].Value;

                var baNextStartList = Convert.FromBase64String(strData);
                var nextStartList = (List<List<HPTService.HPTHorseNextStart>>)HPTSerializer.DeserializeHPTServiceObject(typeof(List<List<HPTService.HPTHorseNextStart>>), baNextStartList);

                nextStartList.ForEach(nsList =>
                {
                    var ns = nsList.First();
                    var ownInformation = HPTConfig.Config.HorseOwnInformationCollection.HorseOwnInformationList
                        .FirstOrDefault(oi => oi.ATGId == ns.ATGId.ToString());

                    if (ownInformation != null)
                    {
                        ownInformation.NextStart = new HPTHorseNextStart()
                        {
                            BetTypes = ns.BetTypeList.ToList(),
                            RaceNumber = ns.RaceNumber,
                            StartDate = ns.RaceDate,
                            TrackId = ns.TrackId
                        };
                    }
                });

                return !string.IsNullOrEmpty(strData);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            return false;
        }

        internal bool SaveSystem(HPTMarkBet markBet)
        {
            try
            {
                var fs = new FileStream(markBet.SystemFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] baAtgXml = HPTSerializer.ZipAndCreateBinary(fs);

                var userSystem = new HPTService.HPTATGSystem()
                {
                    BetType = markBet.BetType.Code,
                    EMail = HPTConfig.Config.EMailAddress,
                    RaceDayDate = markBet.RaceDayInfo.RaceDayDate,
                    TrackId = markBet.RaceDayInfo.TrackId,
                    ATGXml = baAtgXml
                };

                var userSystemStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTATGSystem), userSystem);
                var request = CreateWebRequestForPOST("SaveSystem", userSystemStream);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                string strData = sr.ReadToEnd();
                var rexBinary = new Regex(">(.+?)<");
                strData = rexBinary.Match(strData).Groups[1].Value;

                var baSaveSystem = Convert.FromBase64String(strData);
                var saveSystem = (HPTService.HPTSaveSystem)HPTSerializer.DeserializeHPTServiceObject(typeof(HPTService.HPTSaveSystem), baSaveSystem);
                markBet.UploadedSystemGUID = saveSystem.SystemUniqueIdentifier;

                return !string.IsNullOrEmpty(strData);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
                throw;
            }
        }

        //internal static void UploadComments(HPTMarkBet markBet)
        //{
        //    try
        //    {
        //        // Alla hästar med information
        //        var raceCommentList = markBet.RaceDayInfo.RaceList
        //            .Select(r => new HPTRaceComment()
        //            {
        //                Comment = string.Empty,
        //                LegNr = r.LegNr,
        //                RaceNr = r.RaceNr,
        //                HorseOwnInformationList = r.HorseList
        //                .Where(h => h.OwnInformation.HasComment)
        //                .Select(h => h.OwnInformation).ToList()
        //            }).ToList();

        //        // Rotobjektet för hästkommentarer
        //        var raceDayInfoCommentCollection = new HPTRaceDayInfoCommentCollection()
        //        {
        //            BetTypeCode = markBet.BetType.Code,
        //            Comment = markBet.UserCommentsDescription,
        //            LastUpdated = DateTime.Now,
        //            RaceDayDateString = markBet.RaceDayInfo.RaceDayDateString,
        //            TrackCode = markBet.RaceDayInfo.Trackcode,
        //            TrackId = markBet.RaceDayInfo.TrackId,
        //            TrackName = markBet.RaceDayInfo.Trackname,
        //            UserName = HPTConfig.Config.UserNameForUploads,
        //            RaceCommentList = raceCommentList
        //        };

        //        // Sätt namn och nummer om det inte redan finns
        //        markBet.RaceDayInfo.RaceList
        //            .AsParallel()
        //            .ForAll(r =>
        //            {
        //                foreach (var horse in r.HorseList.Where(h => h.OwnInformation != null))
        //                {
        //                    horse.OwnInformation.StartNr = horse.StartNr;
        //                    horse.OwnInformation.Name = horse.HorseName;
        //                    horse.OwnInformation.ATGId = horse.ATGId;
        //                    horse.OwnInformation.HomeTrack = horse.HomeTrack;
        //                    horse.OwnInformation.Sex = horse.Sex;
        //                    horse.OwnInformation.Age = horse.Age;
        //                    horse.OwnInformation.Owner = horse.OwnerName;
        //                    horse.OwnInformation.Trainer = horse.TrainerName;
        //                }
        //            });

        //        byte[] baRaceDayInfoCommentCollection = HPTSerializer.SerializeHPTRaceDayInfoCommentCollection(raceDayInfoCommentCollection);

        //        var userComments = new HPTService.HPTUserComments()
        //        {
        //            Comment = string.Empty,
        //            CommentXml = baRaceDayInfoCommentCollection,
        //            EMail = HPTConfig.Config.EMailAddress,
        //            UploadDate = DateTime.Now,
        //            UserName = HPTConfig.Config.UserNameForUploads
        //        };

        //        var userCommentsStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTUserComments), userComments);
        //        var request = CreateWebRequestForPOST("UploadRaceDayInfoComments", userCommentsStream);
        //        var response = request.GetResponse();
        //        var stream = response.GetResponseStream();
        //        var sr = new StreamReader(stream);
        //        string strData = sr.ReadToEnd();

        //        //var saveSystem = Client.UploadRaceDayInfoComments(baRaceDayInfoCommentCollection, HPTConfig.Config.EMailAddress, HPTConfig.Config.UserNameForUploads,
        //        //    raceDayInfoCommentCollection.Comment, markBet.RaceDayInfo.BetType.Code, markBet.RaceDayInfo.RaceDayDate, markBet.RaceDayInfo.TrackId);

        //        //if (saveSystem.ErrorMessage != null && saveSystem.ErrorMessage != string.Empty)
        //        //{
        //        //    throw new Exception(saveSystem.ErrorMessage);
        //        //}
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.AddToErrorLogStatic(exc);
        //        throw;
        //    }
        //}

        //internal static void UploadComments(HPTMarkBet markBet)
        //{
        //    try
        //    {
        //        // Alla hästar med information
        //        var raceCommentList = markBet.RaceDayInfo.RaceList
        //            .Select(r => new HPTRaceComment()
        //            {
        //                Comment = string.Empty,
        //                LegNr = r.LegNr,
        //                RaceNr = r.RaceNr,
        //                HorseOwnInformationList = r.HorseList
        //                .Where(h => h.OwnInformation.HasComment)
        //                .Select(h => h.OwnInformation).ToList()
        //            }).ToList();

        //        // Rotobjektet för hästkommentarer
        //        var raceDayInfoCommentCollection = new HPTRaceDayInfoCommentCollection()
        //        {
        //            BetTypeCode = markBet.BetType.Code,
        //            Comment = markBet.UserCommentsDescription,
        //            LastUpdated = DateTime.Now,
        //            RaceDayDateString = markBet.RaceDayInfo.RaceDayDateString,
        //            TrackCode = markBet.RaceDayInfo.Trackcode,
        //            TrackId = markBet.RaceDayInfo.TrackId,
        //            TrackName = markBet.RaceDayInfo.Trackname,
        //            UserName = HPTConfig.Config.UserNameForUploads,
        //            RaceCommentList = raceCommentList
        //        };

        //        // Sätt namn och nummer om det inte redan finns
        //        foreach (var race in markBet.RaceDayInfo.RaceList)
        //        {
        //            foreach (var horse in race.HorseList)
        //            {
        //                if (horse.OwnInformation != null)
        //                {
        //                    horse.OwnInformation.StartNr = horse.StartNr;
        //                    horse.OwnInformation.Name = horse.HorseName;
        //                }
        //            }
        //        }

        //        byte[] baRaceDayInfoCommentCollection = HPTSerializer.SerializeHPTRaceDayInfoCommentCollection(raceDayInfoCommentCollection);

        //        var saveSystem = Client.UploadRaceDayInfoComments(baRaceDayInfoCommentCollection, HPTConfig.Config.EMailAddress, HPTConfig.Config.UserNameForUploads,
        //            raceDayInfoCommentCollection.Comment, markBet.RaceDayInfo.BetType.Code, markBet.RaceDayInfo.RaceDayDate, markBet.RaceDayInfo.TrackId);

        //        if (saveSystem.ErrorMessage != null && saveSystem.ErrorMessage != string.Empty)
        //        {
        //            throw new Exception(saveSystem.ErrorMessage);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.AddToErrorLogStatic(exc);
        //        throw exc;
        //    }
        //}

        // /UserRaceDayInfoCommentsAll?betType={betType}&trackId={trackid}&raceDate={raceDate}

        //internal static HPTService.HPTUserRaceDayInfoCommentsCollection DownloadCommentsAll(HPTMarkBet markBet)
        //{
        //    try
        //    {
        //        byte[] baUserRaceDayInfoCommentsCollection = DownloadAndCreateByteArrayStatic(webserviceURL, "UserRaceDayInfoCommentsAll?betType=" + markBet.RaceDayInfo.BetType.Code
        //            + "&trackid=" + markBet.RaceDayInfo.TrackId.ToString()
        //            + "&raceDate=" + markBet.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd")
        //            );

        //        var userRaceDayInfoCommentsCollection = HPTSerializer.DeserializeHPTUserRaceDayInfoCommentsCollection(baUserRaceDayInfoCommentsCollection);
        //        return userRaceDayInfoCommentsCollection;
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.AddToErrorLogStatic(exc);
        //        throw;
        //    }
        //}

        //// /UserRaceDayInfoCommentsByEmail?eMail={eMail}&betType={betType}&trackId={trackId}&raceDate={raceDate}
        //internal static HPTRaceDayInfoCommentCollection DownloadCommentsByEMail(HPTMarkBet markBet, string eMail)
        //{
        //    try
        //    {
        //        byte[] baUserRaceDayInfoComments = DownloadAndCreateByteArrayStatic(webserviceURL, "UserRaceDayInfoCommentsByEmail?eMail=" + eMail
        //            + "&betType=" + markBet.RaceDayInfo.BetType.Code
        //            + "&trackId=" + markBet.RaceDayInfo.TrackId.ToString()
        //            + "&raceDate=" + markBet.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd")
        //            );

        //        var userRaceDayInfoComment = HPTSerializer.DeserializeHPTRaceDayInfoCommentCollection(baUserRaceDayInfoComments);

        //        foreach (var raceComment in userRaceDayInfoComment.RaceCommentList)
        //        {
        //            foreach (var horseInformation in raceComment.HorseOwnInformationList)
        //            {
        //                var horse = markBet.RaceDayInfo
        //                    .RaceList.First(r => r.LegNr == raceComment.LegNr)
        //                    .HorseList.First(h => h.HorseName == horseInformation.Name);

        //                if (horse.OwnInformation == null)
        //                {
        //                    horse.OwnInformation = new HPTHorseOwnInformation()
        //                    {
        //                        Name = horse.HorseName,
        //                        ATGId = horse.ATGId,
        //                        HomeTrack = horse.HomeTrack,
        //                        Sex = horse.Sex,
        //                        Age = horse.Age,
        //                        Owner = horse.OwnerName,
        //                        Trainer = horse.TrainerName,
        //                        CreationDate = DateTime.Now,
        //                        HorseOwnInformationCommentList = new System.Collections.ObjectModel.ObservableCollection<HPTHorseOwnInformationComment>()
        //                    };
        //                }
        //                else if (horse.OwnInformation.HorseOwnInformationCommentList == null)
        //                {
        //                    horse.OwnInformation.HorseOwnInformationCommentList = new System.Collections.ObjectModel.ObservableCollection<HPTHorseOwnInformationComment>();
        //                }

        //                foreach (var horseComment in horseInformation.HorseOwnInformationCommentList)
        //                {
        //                    var horseInformationComment = horse.OwnInformation.HorseOwnInformationCommentList
        //                        .FirstOrDefault(hc => hc.CommentDate == horseComment.CommentDate && hc.CommentUser == horseComment.CommentUser);

        //                    if (horseInformationComment != null)
        //                    {
        //                        horseInformationComment.Comment = horseComment.Comment;
        //                    }
        //                    else
        //                    {
        //                        horse.OwnInformation.HorseOwnInformationCommentList.Add(horseComment);
        //                    }
        //                    horse.OwnInformation.HasComment = true;
        //                    horseComment.NextTimer = horseInformation.NextTimer;
        //                }
        //                horse.OwnInformation.Updated = true;
        //            }
        //        }
        //        return userRaceDayInfoComment;
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.AddToErrorLogStatic(exc);
        //        //throw exc;
        //    }
        //    return null;
        //}

        internal byte[] GetCalendar(HPTCalendar hptCalendar)
        {
            foreach (var webserviceURL in webserviceURLList)
            {
                // Hämta kalender
                try
                {
                    byte[] baCalendar = DownloadAndCreateByteArray(webserviceURL, "CalendarZip");
                    string calendarPath = HPTConfig.MyDocumentsPath + "\\HPTCalendar.hptc";
                    HPTServiceToHPTHelper.CreateCalendar(baCalendar, hptCalendar);

                    // Sätt sökväg och spara ner kalender på disk
                    HPTSerializer.SerializeHPTCalendar(HPTConfig.MyDocumentsPath + "HPTCalendar.hptc", hptCalendar);
                    return baCalendar;
                }
                catch (Exception exc)
                {
                    HPTConfig.AddToErrorLogStatic(exc);
                }
            }
            return null;
        }

        internal static HttpWebRequest CreateWebRequestForPOST(string restPath, Stream bodyData)
        {
            //var request = (HttpWebRequest)HttpWebRequest.Create(webserviceURL + "/HPTRestService/" + restPath);
            var request = (HttpWebRequest)HttpWebRequest.Create(webserviceURLList.First() + "/HPTRestService/" + restPath);
            request.Method = "POST";
            request.Timeout = 50000;
            request.ReadWriteTimeout = 50000;
            var stream = request.GetRequestStream();
            var baBodyData = new byte[bodyData.Length];
            bodyData.Read(baBodyData, 0, Convert.ToInt32(bodyData.Length));
            stream.Write(baBodyData, 0, baBodyData.Length);

            return request;
        }

        internal static WebClient WC
        {
            get
            {
                return new WebClient()
                           {
                               BaseAddress = webserviceURLList.First() + "/HPTRestService/",
                               Encoding = Encoding.UTF8                                                            
                           };
            }
        }

        //internal static HttpWebRequest WR
        //{
        //    get
        //    {
        //        var request = (HttpWebRequest)HttpWebRequest.Create(webserviceURL + "/HPTRestService/");
        //        request.Method = "GET";
        //        request.Timeout = 15000;
        //        request.ReadWriteTimeout = 15000;

        //        return request;
        //    }
        //}

        //internal static HttpWebRequest WRAlt
        //{
        //    get
        //    {
        //        var request = (HttpWebRequest)HttpWebRequest.Create(webserviceURLAlt + "/HPTRestService/");
        //        request.Method = "GET";
        //        request.Timeout = 15000;
        //        request.ReadWriteTimeout = 15000;

        //        return request;
        //    }
        //}

        internal static HttpWebRequest GetWebRequest(string url, string query)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url + "/HPTRestService/" + query);
            request.Method = "GET";
            request.Timeout = 15000;
            request.ReadWriteTimeout = 15000;

            return request;
        }

        //internal static HttpWebRequest GetWebRequest(string url)
        //{
        //    var request = (HttpWebRequest)HttpWebRequest.Create(webserviceURL + "/HPTRestService/" + url);
        //    request.Method = "GET";
        //    request.Timeout = 15000;
        //    request.ReadWriteTimeout = 15000;

        //    return request;
        //}

        internal byte[] DownloadAndCreateByteArray(string url, string query)
        {
            try
            {
                var wr = GetWebRequest(url, query);
                var stream = wr.GetResponse().GetResponseStream();
                var sr = new StreamReader(stream);
                string strData = sr.ReadToEnd();
                Regex rexBinary = new Regex(">(.+?)<");
                strData = rexBinary.Match(strData).Groups[1].Value;
                byte[] baData = Convert.FromBase64String(strData);
                return baData;
            }
            catch (WebException wExc)
            {
                HPTConfig.AddToErrorLogStatic(wExc);
                return null;
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
                return new byte[0];
            }
        }

        //internal static string GetAlternateUrl(string url)
        //{
        //    return url.Replace(webserviceURL, webserviceURLAlt);
        //}

        //internal byte[] DownloadAndCreateByteArray(string url)
        //{
        //    string strData = WC.DownloadString(url);
        //    Regex rexBinary = new Regex(">(.+?)<");
        //    strData = rexBinary.Match(strData).Groups[1].Value;
        //    byte[] baData = Convert.FromBase64String(strData);
        //    return baData;
        //}

        internal static byte[] DownloadAndCreateByteArrayStatic(string url, string query)
        {
            try
            {
                var wr = GetWebRequest(url, query);
                var stream = wr.GetResponse().GetResponseStream();
                var sr = new StreamReader(stream);
                string strData = sr.ReadToEnd();
                Regex rexBinary = new Regex(">(.+?)<");
                strData = rexBinary.Match(strData).Groups[1].Value;
                byte[] baData = Convert.FromBase64String(strData);
                return baData;
            }
            catch (WebException wExc)
            {
                HPTConfig.AddToErrorLogStatic(wExc);
                return null;
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
                return new byte[0];
            }
        }

        internal static string DownloadStringStatic(string url)
        {
            string strData = WC.DownloadString(url);
            Regex rexBinary = new Regex(">(.+?)<");
            strData = rexBinary.Match(strData).Groups[1].Value;
            return strData;
        }

        //internal static HPTService.AuthenticationResponse authResponse;
        //internal static HPTService.AuthenticationResponse AuthenticateAndGetCalendar()
        //{
        //    var request = CreateAuthenticationRequest();
        //    request.IPAddress = "XXX.XXX.XXX.XXX";// FindIPAddress();

        //    HPTService.AuthenticationResponse response = new HPTService.AuthenticationResponse()
        //    {
        //        ErrorMessage = "Autenticering misslyckades",
        //        IsPayingCustomer = false
        //    };

        //    if (string.IsNullOrWhiteSpace(HPTConfig.Config.EMailAddress) || string.IsNullOrWhiteSpace(HPTConfig.Config.Password))
        //    {
        //        response.CalendarZip = DownloadAndCreateByteArrayStatic(webserviceURLAlt, "CalendarZip");
        //    }
        //    else
        //    {
        //        string query = "AuthenticateAndGetCalendarZip?eMailAddress=" + request.EMailAddress
        //            + "&password=" + request.Password
        //            + "&ipAddress=" + request.IPAddress;

        //        try
        //        {
        //            byte[] baAuthenticationResponse = DownloadAndCreateByteArrayStatic(webserviceURL, query);
        //            response = HPTSerializer.DeserializeAuthenticationResponse(baAuthenticationResponse);
        //            HPTConfig.Config.LastIPAddress = request.IPAddress;
        //            HPTConfig.Config.PROVersionExpirationDate = response.PROVersionExpirationDate;
        //        }
        //        catch (Exception exc)
        //        {
        //            if (HPTConfig.Config.PROVersionExpirationDate.Date < DateTime.Today)
        //            {
        //                //byte[] baAuthenticationResponse = DownloadAndCreateByteArrayStatic(webserviceURLAlt, query);
        //                byte[] baAuthenticationResponse = DownloadAndCreateByteArrayStatic(webserviceURL, query);
        //                response = HPTSerializer.DeserializeAuthenticationResponse(baAuthenticationResponse);
        //                HPTConfig.Config.LastIPAddress = request.IPAddress;
        //                HPTConfig.Config.PROVersionExpirationDate = response.PROVersionExpirationDate;
        //            }
        //            else
        //            {
        //                response.PROVersionExpirationDate = HPTConfig.Config.PROVersionExpirationDate;
        //                response.CalendarZip = DownloadAndCreateByteArrayStatic(webserviceURLAlt, "CalendarZip");
        //                response.IsPayingCustomer = true;
        //            }
        //            HPTConfig.AddToErrorLogStatic(exc);
        //        }
        //    }

        //    authResponse = response;
        //    return response;
        //}

        //internal static bool AuthenticationNeeded(HPTService.AuthenticationRequest request, HPTService.AuthenticationResponse response)
        //{
        //    // Ingen registrerad IP-adress, så autenticering krävs.
        //    if (string.IsNullOrEmpty(HPTConfig.Config.LastIPAddress) || HPTConfig.Config.PROVersionExpirationDate < DateTime.Now)
        //    {
        //        return true;
        //    }

        //    // Samma IP-adress och betalande användare, hoppa över autenticering.
        //    var rexDomain = new Regex(@"[xX\d]{1,3}\.[xX\d]{1,3}\.");
        //    string oldIPAdress = rexDomain.Match(HPTConfig.Config.LastIPAddress).Value;
        //    string newIPAdress = rexDomain.Match(request.IPAddress).Value;
        //    if (oldIPAdress == newIPAdress)
        //    {
        //        response.IsPayingCustomer = true;
        //        response.PROVersionExpirationDate = HPTConfig.Config.PROVersionExpirationDate;
        //        response.ErrorMessage = string.Empty;
        //        return false;
        //    }

        //    return true;
        //}

        //internal static void LogOut()
        //{
        //    try
        //    {
        //        HPTService.AuthenticationRequest request = CreateAuthenticationRequest();
        //        //request.IPAddress = IPAddress;
        //        request.IPAddress = "XXX.XXX.XXX.XXX";

        //        byte[] baLogOut = DownloadAndCreateByteArrayStatic(webserviceURL, "Logout?eMailAddress=" + request.EMailAddress 
        //                                                         + "&password=" + request.Password 
        //                                                         + "&ipAddress=" + request.IPAddress);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //        string s = exc.Message;
        //    }
        //}

        private static HPTService.AuthenticationRequest CreateAuthenticationRequest()
        {
            //// Om epost/nyckel inte lästs upp från hptcon-filen
            //if (string.IsNullOrEmpty(HPTConfig.Config.EMailAddress) || string.IsNullOrEmpty(HPTConfig.Config.Password))
            //{
            //    bool configRead = HPTConfig.Config.GetValuesFromIsolatedStorage();
            //}

            HPTService.AuthenticationRequest request = new HPTClient.HPTService.AuthenticationRequest();
            request.ClientName = "HPT53";
            request.ClientVersionNumber = 5.33M;
            request.ClientBeta = false;
            request.ClientBetaVersion = 0;
            request.Password = HPTConfig.Config.Password == null ? string.Empty : HPTConfig.Config.Password;
            request.UserName = HPTConfig.Config.UserName == null ? string.Empty : HPTConfig.Config.UserName;
            request.EMailAddress = HPTConfig.Config.EMailAddress == null ? string.Empty : HPTConfig.Config.EMailAddress;
            
            return request;
        }

        //internal static HPTService.HPTRegistration Register()
        //{
        //    byte[] baRegister = DownloadAndCreateByteArrayStatic(webserviceURL, "RegisterUser?username=" + HPTConfig.Config.UserName + "&eMailAddress=" + HPTConfig.Config.EMailAddress);
        //    HPTService.HPTRegistration response = HPTSerializer.DeserializeHPTRegistration(baRegister);
        //    return response;
        //}

        //internal static void GetKey()
        //{
        //    byte[] baRetrieveKey = DownloadAndCreateByteArrayStatic(webserviceURL, "RetrieveKey?eMailAddress=" + HPTConfig.Config.EMailAddress);
        //    //HPTService.HPTUser response = Client.RetrieveKey(HPTConfig.Config.EMailAddress);
        //}

        //internal static string IPAddress = "XXX.XXX.XXX.XXX";
        //internal static string FindIPAddress()
        //{
        //    //string ipAddress = "XXX.XXX.XXX.XXX";
        //    //string whatIsMyIp = "http://automation.whatismyip.com/n09230945.asp";
        //    //string whatIsMyIp = "http://checkip.dyndns.org/";
        //    WebClient wc = new WebClient();
        //    UTF8Encoding utf8 = new UTF8Encoding();
        //    //string requestHtml = string.Empty;
        //    try
        //    {
        //        wc.DownloadStringCompleted += (sender, e) =>
        //            {
        //                try
        //                {
        //                    var rexIPAddress = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
        //                    if (rexIPAddress.IsMatch(e.Result))
        //                    {
        //                        IPAddress = rexIPAddress.Match(e.Result).Value;
        //                    }
        //                }
        //                catch (Exception exc)
        //                {
        //                    string s = exc.Message;
        //                }
        //            };

        //        var uri = new Uri("http://checkip.dyndns.org/");
        //        wc.DownloadStringAsync(uri);
        //        //requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
        //        //var rexIPAddress = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
        //        //if (rexIPAddress.IsMatch(requestHtml))
        //        //{
        //        //    ipAddress = rexIPAddress.Match(requestHtml).Value;
        //        //}
                
        //    }
        //    catch (WebException we)
        //    {
        //        HPTConfig.AddToErrorLogStatic(we);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.AddToErrorLogStatic(exc);
        //    }
        //    return IPAddress;

        //}
        
        #region GetRaceDayInfoByTrackAndDate

        internal void GetRaceDayInfoByTrackAndDate()
        {
            HPTRaceDayInfo hptRdi = new HPTRaceDayInfo();
            string query = "RaceDayInfo?betType="
                            + this.betType.Code + "&trackId=" +
                            this.trackId.ToString() + "&raceDate=" +
                            this.raceDate.ToString("yyyy-MM-dd");
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, query);

                    var rdi = HPTSerializer.DeserializeHPTRaceDayInfo(baRaceDayInfo);
                    HPTServiceToHPTHelper.ConvertRaceDayInfo(rdi, hptRdi);

                    hptRdi.BetTypeString = this.betType.Code;
                    hptRdi.BetType = this.betType;
                    this.rdiDelegate(hptRdi);
                    rdi = null;
                    GC.Collect();
                }
                catch (Exception exc)
                {
                    HPTConfig.AddToErrorLogStatic(exc);
                } 
            }

            hptRdi = new HPTRaceDayInfo()
            {
                BetTypeString = this.betType.Code,
                TrackId = this.trackId,
                RaceDayDate = this.raceDate
            };
            this.rdiDelegate(hptRdi);
        }
        
        internal void GetRaceDayInfoByTrackAndDate(HPTBetType betType, int trackId, DateTime raceDate, Action<HPTRaceDayInfo> rdiDelegate)
        {
            this.betType = betType;
            this.raceDate = raceDate;
            this.rdiDelegate = rdiDelegate;
            this.trackId = trackId;
            ThreadStart starter = new ThreadStart(GetRaceDayInfoByTrackAndDate);
            Thread thread = new Thread(starter);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        internal HPTRaceDayInfo GetRaceDayInfoByTrackAndDate(string betType, string raceDate, string trackId)
        {
            var hptRdi = new HPTRaceDayInfo();
            foreach (var webserviceURL in webserviceURLList)
            {
                byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, "RaceDayInfo?betType="
                                                                  + betType + "&trackId=" +
                                                                  trackId + "&raceDate=" +
                                                                  raceDate);

                HPTService.HPTRaceDayInfo rdi = HPTSerializer.DeserializeHPTRaceDayInfo(baRaceDayInfo);
                
                HPTServiceToHPTHelper.ConvertRaceDayInfo(rdi, hptRdi);
                hptRdi.BetTypeString = betType;
                hptRdi.BetType = new HPTBetType()
                {
                    Code = betType,
                    Name = betType
                };
                return hptRdi;
            }
            return hptRdi;
        }

        internal void GetRaceDayInfoUpdate(string betTypeString, int trackId, DateTime raceDate, Action<HPTService.HPTRaceDayInfo> updateRaceDayInfoDelegate)
        {
            this.betType = new HPTBetType() { Code = betTypeString };
            this.raceDate = raceDate;
            this.updateDelegate = updateRaceDayInfoDelegate;
            this.trackId = trackId;
            ThreadStart starter = new ThreadStart(GetRaceDayInfoUpdate);
            Thread thread = new Thread(starter);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        internal void GetRaceDayInfoUpdate()
        {
            string query = "RaceDayInfoUpdate?betType="
                            + this.betType.Code + "&trackId=" +
                            this.trackId.ToString() + "&raceDate=" +
                            this.raceDate.ToString("yyyy-MM-dd");
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, query);

                    //if (baRaceDayInfo == null)
                    //{
                    //    baRaceDayInfo = DownloadAndCreateByteArray(webserviceURLAlt, query);
                    //}

                    HPTService.HPTRaceDayInfo rdi = HPTSerializer.DeserializeHPTRaceDayInfo(baRaceDayInfo);
                    this.updateDelegate(rdi);
                }
                catch (Exception exc)
                {
                } 
            }
            this.updateDelegate(null);
        }

        internal void GetRaceDayInfoUpdate(HPTRaceDayInfo hptRdi)
        {
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, "RaceDayInfoUpdate?betType="
                                                                          + hptRdi.BetType.Code + "&trackId=" +
                                                                          hptRdi.TrackId.ToString() + "&raceDate=" +
                                                                          hptRdi.RaceDayDate.ToString("yyyy-MM-dd"));

                    HPTService.HPTRaceDayInfo rdi = HPTSerializer.DeserializeHPTRaceDayInfo(baRaceDayInfo);
                    hptRdi.Merge(rdi);
                    rdi = null;
                    GC.Collect();
                    return;
                }
                catch (Exception exc)
                {
                } 
            }
        }

        internal HPTService.HPTRaceDayInfo GetRaceDayInfoUpdateNoMerge(HPTRaceDayInfo hptRdi)
        {
            if (hptRdi.RaceList.Min(r => r.PostTime) > DateTime.Now.AddMinutes(4D))
            {
                return null;
            }

            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, "RaceDayInfoUpdate?betType="
                                                                          + hptRdi.BetType.Code + "&trackId=" +
                                                                          hptRdi.TrackId.ToString() + "&raceDate=" +
                                                                          hptRdi.RaceDayDate.ToString("yyyy-MM-dd"));

                    var rdi = HPTSerializer.DeserializeHPTRaceDayInfo(baRaceDayInfo);

                    var horsesWithoutResultInfo = hptRdi.RaceList
                        .SelectMany(r => r.HorseList)
                        .ToList();

                    int finalTurnover = rdi.LegList.First().HorseList.Sum(h => h.StakeDistribution);

                    if (finalTurnover > 0)
                    {
                        horsesWithoutResultInfo.ForEach(h =>
                            {
                                var horse = rdi.LegList
                                .First(l => l.LegNr == h.ParentRace.LegNr)
                                .HorseList
                                .First(hInner => hInner.StartNr == h.StartNr);

                                // Hur gick det i loppet?
                                HPTHorse.HandleHorseResultInfo(horse, h);

                                // Sätt den slutliga insatsfördelningen
                                h.StakeDistributionShareFinal = Convert.ToDecimal(horse.StakeDistribution) / finalTurnover;
                                h.VinnarDddsFinal = horse.VPInfo.VinnarOddsExact;
                            });
                    }

                    return rdi;
                }
                catch (Exception exc)
                {
                } 
            }
            return null;
        }

        internal HPTService.HPTRaceDayInfo GetScratchedHorses(HPTRaceDayInfo hptRdi)
        {
            foreach (var webserviceURL in webserviceURLList)
            {
                byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, "RaceDayInfoUpdate?betType="
                                                                      + hptRdi.BetType.Code + "&trackId=" +
                                                                      hptRdi.TrackId.ToString() + "&raceDate=" +
                                                                      hptRdi.RaceDayDate.ToString("yyyy-MM-dd"));

                var rdi = HPTSerializer.DeserializeHPTRaceDayInfo(baRaceDayInfo);
                return rdi; 
            }
            return null;
        }
        
        // /RaceDayInfoHistoryGrouped?betType={betType}&trackId={trackid}&raceDate={raceDate}
        internal bool GetRaceDayInfoHistoryGrouped(HPTRaceDayInfo hptRdi)
        {
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, "RaceDayInfoHistoryGrouped?betType=" + hptRdi.BetType.Code +
                                                                            "&trackId=" + hptRdi.TrackId.ToString() +
                                                                            "&raceDate=" + hptRdi.RaceDayDate.ToString("yyyy-MM-dd"));

                    var rdiHistory = (HPTService.HPTRaceDayInfoHistoryInfoGrouped)HPTSerializer.DeserializeHPTServiceObject(typeof(HPTService.HPTRaceDayInfoHistoryInfoGrouped), baRaceDayInfo);

                    HPTServiceToHPTHelper.ConvertRaceDayInfoHistory(rdiHistory, hptRdi);

                    return true;
                }
                catch (Exception exc)
                {
                } 
            }
            return false;
        }

        internal void GetRaceDayInfoHistoryGrouped(string betTypeString, int trackId, DateTime raceDate, Action<HPTService.HPTRaceDayInfoHistoryInfoGrouped> historyDelegate)
        {
            this.betType = new HPTBetType() { Code = betTypeString };
            this.raceDate = raceDate;
            this.historyDelegate = historyDelegate;
            this.trackId = trackId;
            ThreadStart starter = new ThreadStart(GetRaceDayInfoHistoryGrouped);
            Thread thread = new Thread(starter);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        internal void GetRaceDayInfoHistoryGrouped()
        {
            string query = "RaceDayInfoHistoryGrouped?betType=" + this.betType.Code +
                                                    "&trackId=" + this.trackId.ToString() +
                                                    "&raceDate=" + this.raceDate.ToString("yyyy-MM-dd");
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baRaceDayInfo = DownloadAndCreateByteArray(webserviceURL, query);

                    //if (baRaceDayInfo == null)
                    //{
                    //    baRaceDayInfo = DownloadAndCreateByteArray(webserviceURLAlt, query);
                    //}

                    var rdiHistory = (HPTService.HPTRaceDayInfoHistoryInfoGrouped)HPTSerializer.DeserializeHPTServiceObject(typeof(HPTService.HPTRaceDayInfoHistoryInfoGrouped), baRaceDayInfo);
                    this.historyDelegate(rdiHistory);
                }
                catch (Exception exc)
                {
                } 
            }
            this.historyDelegate(null);
        }

        #endregion

        #region Resultat

        internal void GetResultMarkingBetByTrackAndDate(string betType, int trackId, DateTime raceDate, HPTRaceDayInfo hptRdi, bool setValues)
        {
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baResultMarkingBet = DownloadAndCreateByteArray(webserviceURL, "Result?betType="
                                                                                  + betType + "&trackId=" +
                                                                                  trackId.ToString() + "&raceDate=" +
                                                                                  raceDate.ToString("yyyy-MM-dd"));

                    HPTService.HPTResultMarkingBet rmb = HPTSerializer.DeserializeHPTResultMarkingBet(baResultMarkingBet);
                    if (rmb.ResultComplete)
                    {
                        SaveResult(rmb, baResultMarkingBet);
                    }
                    HPTServiceToHPTHelper.ConvertResultMarkingBet(rmb, hptRdi, setValues);
                    rmb = null;
                    GC.Collect();
                    return;
                }
                catch (Exception exc)
                {
                }
            }
        }

        internal void GetResultMarkingBetByTrackAndDate(string betType, string raceDate, string trackId, HPTRaceDayInfo hptRdi)
        {
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baResultMarkingBet = DownloadAndCreateByteArray(webserviceURL, "Result?betType="
                                                                          + betType + "&trackId=" +
                                                                          trackId + "&raceDate=" +
                                                                          raceDate);

                    HPTService.HPTResultMarkingBet rmb = HPTSerializer.DeserializeHPTResultMarkingBet(baResultMarkingBet);
                    if (rmb.ResultComplete)
                    {
                        SaveResult(rmb, baResultMarkingBet);
                    }
                    HPTServiceToHPTHelper.ConvertResultMarkingBet(rmb, hptRdi, true);
                    rmb = null;
                    GC.Collect();

                    return;
                }
                catch (Exception exc)
                {
                } 
            }
        }

        internal void GetResultMarkingBetByTrackAndDate(string betTypeString, int trackId, DateTime raceDate, Action<HPTService.HPTResultMarkingBet> resultDelegate)
        {
            this.betType = new HPTBetType() { Code = betTypeString };
            this.raceDate = raceDate;
            this.resultDelegate = resultDelegate;
            this.trackId = trackId;
            ThreadStart starter = new ThreadStart(GetResultMarkingBetByTrackAndDate);
            Thread thread = new Thread(starter);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        internal void SaveResult(HPTService.HPTResultMarkingBet rmb, byte[] baResultMarkingBet)
        {
            try
            {
                string resultPath = HPTConfig.MyDocumentsPath + "Resultat";
                if (!Directory.Exists(resultPath))
                {
                    Directory.CreateDirectory(resultPath);
                }
                string raceDayPath = resultPath + "\\" + rmb.RaceDate.ToString("yyyy-MM-dd") + "_" + rmb.TrackCode;
                if (!Directory.Exists(raceDayPath))
                {
                    Directory.CreateDirectory(raceDayPath);
                }
                string filename = raceDayPath + "\\" + rmb.BetType + ".hptresult";
                File.WriteAllBytes(filename, baResultMarkingBet);
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
        }

        internal HPTService.HPTResultMarkingBet GetResultFromDisk(HPTRaceDayInfo raceDayInfo)
        {
            string filename = HPTConfig.MyDocumentsPath 
                + "Resultat\\" + raceDayInfo.RaceDayDate.ToString("yyyy-MM-dd") + "_" + raceDayInfo.Trackcode 
                + "\\" + raceDayInfo.BetType.Code + ".hptresult";

            if (File.Exists(filename))
            {
                byte[] baResultMarkingBet = File.ReadAllBytes(filename);
                return HPTSerializer.DeserializeHPTResultMarkingBet(baResultMarkingBet);
            }
            return null;
        }

        internal void GetResultMarkingBetByTrackAndDate()
        {
            HPTService.HPTResultMarkingBet rmb = new HPTService.HPTResultMarkingBet();
            string query = "Result?betType="
                            + this.betType.Code + "&trackId=" +
                            this.trackId.ToString() + "&raceDate=" +
                            this.raceDate.ToString("yyyy-MM-dd");
            foreach (var webserviceURL in webserviceURLList)
            {
                try
                {
                    byte[] baResultMarkingBet = DownloadAndCreateByteArray(webserviceURL, query);

                    //if (baResultMarkingBet == null)
                    //{
                    //    baResultMarkingBet = DownloadAndCreateByteArray(webserviceURLAlt, query);
                    //}

                    rmb = HPTSerializer.DeserializeHPTResultMarkingBet(baResultMarkingBet);
                    GC.Collect();
                }
                catch (Exception exc)
                {
                    HPTConfig.Config.AddToErrorLog(exc);
                } 
            }
            this.resultDelegate(rmb);
        }

        #endregion

        #region Ladda upp och ner användardata

        //internal void UploadTemplates()
        //{
        //    var serializer = new XmlSerializer(typeof(HPTTemplateCollection));
        //    var templateCollection = new HPTTemplateCollection()
        //    {
        //        GroupIntervalTemplateList = HPTConfig.Config.GroupIntervalRulesCollectionList.ToList(),
        //        RankTemplateList = HPTConfig.Config.RankTemplateList.Where(rt => !rt.IsDefault).ToList(),
        //        MarkBetTemplateABCDList = HPTConfig.Config.MarkBetTemplateABCDList.Where(t => !t.IsDefault).ToList(),
        //        MarkBetTemplateRankList = HPTConfig.Config.MarkBetTemplateRankList.Where(t => !t.IsDefault).ToList()
        //    };
        //    var baTemplateCollection = HPTSerializer.SerializeHPTObject(serializer, templateCollection);
        //    var response = Client.UploadTemplates(baTemplateCollection, HPTConfig.Config.EMailAddress, HPTConfig.Config.Password, string.Empty, string.Empty);
        //}

        //internal void GetUserTemplates()
        //{
        //    try
        //    {
        //        byte[] baUserTemplatesList = DownloadAndCreateByteArray(webserviceURL, "UserTemplatesAll");

        //        //rmb = HPTSerializer.DeserializeHPTResultMarkingBet(baUserTemplatesList);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //    }
        //}

        //internal void GetUserTemplatesByEMail(string eMailAddress)
        //{
        //    try
        //    {
        //        byte[] baUserTemplateCollection = DownloadAndCreateByteArray(webserviceURL, "UserTemplatesByEmail?eMail=" + eMailAddress);
        //        var templateCollection = HPTSerializer.DeserializeHPTTemplateCollection(baUserTemplateCollection);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //    }
        //}

        //internal static bool UploadSystem(HPTMarkBet markBet)
        //{
        //    try
        //    {
        //        var serializer = new XmlSerializer(typeof(HPTMarkBet));
        //        var baMarkBet = HPTSerializer.SerializeHPTObject(serializer, markBet);
        //        var response = Client.UploadSystem(baMarkBet, HPTConfig.Config.EMailAddress, HPTConfig.Config.UserNameForUploads, markBet.BetType.Code, markBet.RaceDayInfo.RaceDayDate, markBet.RaceDayInfo.TrackId, markBet.ReducedSize, markBet.SystemSize, markBet.ToReductionNamesString(), markBet.SystemComment, markBet.ToClipboardString());
        //        return response.SystemSaved;
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //        return false;
        //    }
        //}

        //internal static bool SendMail(HPTMailSender mailSender, HPTMarkBet markBet)
        //{
        //    try
        //    {
        //        byte[] baMarkBet = null;
        //        byte[] baATGFile = null;

        //        if (mailSender.AttachHPT3File)
        //        {
        //            var serializedHPTFile = HPTSerializer.SerializeHPTServiceObject(typeof(HPTMarkBet), markBet);
        //            baMarkBet = HPTSerializer.ZipAndCreateBinary(serializedHPTFile);
        //        }

        //        if (mailSender.AttachATGSystemFile)
        //        {
        //            var serializedATGFile = new FileStream(mailSender.ATGSystemFileName, FileMode.Open, FileAccess.Read);
        //            baATGFile = new byte[serializedATGFile.Length];
        //            int result = serializedATGFile.Read(baATGFile, 0, (int)serializedATGFile.Length);
        //        }

        //        var userSystem = new HPTService.HPTUserSystemToMail()
        //        {
        //            EMail = HPTConfig.Config.EMailAddress,
        //            UserName = HPTConfig.Config.UserNameForUploads,
        //            HPTSystem = baMarkBet,
        //            HPT5FileName = Path.GetFileName(markBet.ToFileNameString() + ".hpt5"),
        //            ATGXml = baATGFile,
        //            ATGXmlFileName = Path.GetFileName(mailSender.ATGSystemFileName),
        //            //EMailAddressList = mailSender.MailRecipients.Where(mr => mr.Selected).Select(mr => mr.EMailAddress).ToArray(),
        //            EMailAddressList = mailSender.MailRecipients.Select(mr => mr.EMailAddress).ToArray(),
        //            Subject = mailSender.Subject,
        //            Body = mailSender.Body,
        //            Timestamp = markBet.LastSaveTime
        //        };

        //        var userSystemStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTUserSystemToMail), userSystem);
        //        var request = CreateWebRequestForPOST("SendMail", userSystemStream);
        //        var response = request.GetResponse();
        //        var stream = response.GetResponseStream();
        //        var sr = new StreamReader(stream);
        //        string strData = sr.ReadToEnd();
        //        return !string.IsNullOrEmpty(strData);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //        return false;
        //    }
        //}

        //internal static bool SendMail(HPTMailSender mailSender)
        //{
        //    try
        //    {
        //        var userSystem = new HPTService.HPTUserSystemToMail()
        //        {
        //            EMail = HPTConfig.Config.EMailAddress,
        //            UserName = HPTConfig.Config.UserNameForUploads,
        //            HPTSystem = null,
        //            HPT5FileName = null,
        //            ATGXml = null,
        //            ATGXmlFileName = null,
        //            EMailAddressList = mailSender.MailRecipients.Select(mr => mr.EMailAddress).ToArray(),
        //            Subject = mailSender.Subject,
        //            Body = mailSender.Body,
        //            Timestamp = DateTime.Now
        //        };

        //        var userSystemStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTUserSystemToMail), userSystem);
        //        var request = CreateWebRequestForPOST("SendMail", userSystemStream);
        //        var response = request.GetResponse();
        //        var stream = response.GetResponseStream();
        //        var sr = new StreamReader(stream);
        //        string strData = sr.ReadToEnd();
        //        return !string.IsNullOrEmpty(strData);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //        return false;
        //    }
        //}

        internal bool UploadSystem(HPTMarkBet markBet)
        {
            try
            {
                var serializedStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTMarkBet), markBet);
                var baMarkBet = HPTSerializer.ZipAndCreateBinary(serializedStream);

                var userSystem = new HPTService.HPTUserSystem()
                {
                    BetType = markBet.BetType.Code,
                    Comment = markBet.SystemComment,
                    EMail = HPTConfig.Config.EMailAddress,
                    UserName = HPTConfig.Config.UserNameForUploads,
                    HPTSystem = baMarkBet,
                    OriginalSize = markBet.SystemSize,
                    RaceDayDate = markBet.RaceDayInfo.RaceDayDate,
                    ReducedSize = markBet.ReducedSize,
                    ReductionNames = markBet.ToReductionNamesString(),
                    TrackId = markBet.RaceDayInfo.TrackId,
                    Timestamp = markBet.LastSaveTime
                };

                var userSystemStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTUserSystem), userSystem);
                var request = CreateWebRequestForPOST("UploadSystem", userSystemStream);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                string strData = sr.ReadToEnd();
                return !string.IsNullOrEmpty(strData);
            }
            catch (Exception exc)
            {
                HPTConfig.Config.AddToErrorLog(exc);
                return false;
            }
        }

        internal bool UploadCompleteSystem(HPTMarkBet markBet, bool isPublic, bool locked)
        {
            try
            {
                var serializedStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTMarkBet), markBet);
                var baMarkBet = HPTSerializer.ZipAndCreateBinary(serializedStream);

                // Skapa kupongfilen
                if (!File.Exists(markBet.SystemFilename))
                {
                    markBet.CouponCorrector.CouponHelper.CreateATGFile();
                }
                var srATGXml = new StreamReader(markBet.SystemFilename);
                string atgXml = srATGXml.ReadToEnd();

                // Konvertera systemet till ett HPT Online-system
                var raceDayInfoReduction = HPTServiceToHPTHelper.CreateHPTOnlineFromHPTMarkBet(markBet);
                raceDayInfoReduction.Locked = locked;

                // Skapa uppladdningsobjektet
                var userSystem = new HPTService.HPTUserSystemComplete()
                {
                    HPTSystem = baMarkBet,
                    RaceDayInfoReduction = raceDayInfoReduction,
                    IsPublic = isPublic,
                    Timestamp = markBet.LastSaveTime,
                    EMail = HPTConfig.Config.EMailAddress,
                    UserName = HPTConfig.Config.UserNameForUploads                    
                };

                // Fyll på med reduceringsresutatet
                userSystem.RaceDayInfoReduction.ReductionResultFile = new HPTService.HPTReductionResultFile()
                {
                    ATGSystemCount = markBet.NumberOfSystems,
                    CorrectionUrl = string.Empty,
                    CouponFile = atgXml,
                    CouponList = markBet.CouponCorrector.CouponHelper.CouponList.Select(c => new HPTService.HPTMarkBetSingleRowCombination()
                    {
                        BM = c.BetMultiplier,
                        CN = c.CouponId,
                        CRL = c.CouponRaceList.Select(cr => new HPTService.HPTMarkBetSingleRowCombinationRace()
                        {
                            CR = cr.StartNrList.ToArray(),
                            LN = cr.LegNr,
                            R1 = cr.Reserv1,
                            R2 = cr.Reserv2
                        }).ToArray(),
                        S = c.SystemSize,
                        V6 = c.V6
                    }).ToArray(),
                    ReducedSize = markBet.ReducedSize,
                    ReductionPercentage = markBet.ReductionQuota,
                    SystemGUID = string.Empty,
                    SystemCost = markBet.SystemCost,                   
                    SystemSize = markBet.SystemSize,
                    LastUpdated = markBet.LastSaveTime,
                    LegList = markBet.RaceDayInfo.RaceList.Select(r => new HPTService.HPTRaceReductionResponse()
                    {
                        LegNr = r.LegNr,
                        HorseList = r.HorseList.Select(h => new HPTService.HPTHorseReductionResponse()
                        {
                            StartNr = h.StartNr,
                            NumberOfRows = h.NumberOfCoveredRows,
                            PercentageOfRows = h.SystemCoverage
                        }).ToArray()
                    }).ToArray()
                };

                string json = HPTSerializer.CreateJson(userSystem);                

                var userSystemStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTUserSystemComplete), userSystem);
                var request = CreateWebRequestForPOST("UploadCompleteSystem", userSystemStream);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                var saveSystem = (HPTService.HPTSaveSystem)HPTSerializer.DeserializeHPTServiceObject(typeof(HPTService.HPTSaveSystem), stream);
                markBet.SystemURL = saveSystem.UrlForMobile;
                markBet.UniqueIdentifier = saveSystem.SystemUniqueIdentifier;

                return true;

                //var sr = new StreamReader(stream);
                //string strData = sr.ReadToEnd();
                //return !string.IsNullOrEmpty(strData);
            }
            catch (Exception exc)
            {
                HPTConfig.Config.AddToErrorLog(exc);
                return false;
            }
        }

        // /UserSystems?betType={betType}&trackId={trackId}&raceDate={raceDate}
        //internal static HPTUserSystemCollection DownloadSystemList(HPTRaceDayInfoLight raceDayInfoLight)
        //{
        //    //CultureInfo ci = new CultureInfo("sv-SE");
        //    byte[] baUserSystemCollection = DownloadAndCreateByteArrayStatic(webserviceURL, "UserSystems?betType=" + raceDayInfoLight.BetTypeCode
        //        + "&trackId=" + raceDayInfoLight.TrackId.ToString()
        //        + "&raceDate=" + raceDayInfoLight.RaceDayDate.ToString("yyyy-MM-dd")
        //        );

        //    var userSystemCollection = (HPTService.HPTUserSystemCollection)HPTSerializer.DeserializeHPTServiceObject(typeof(HPTService.HPTUserSystemCollection), baUserSystemCollection);

        //    var distinctUserSystemList = userSystemCollection.UserSystemList
        //        .GroupBy(us => new { us.EMail, us.ReducedSize, us.OriginalSize })
        //        .Select(grp => grp.FirstOrDefault())
        //        .ToList();

        //    var hptUserSystemCollection = new HPTUserSystemCollection()
        //    {
        //        BetType = userSystemCollection.BetType,
        //        RaceDayDate = userSystemCollection.RaceDayDate,
        //        TrackId = userSystemCollection.TrackId,
        //        UserSystemList = new ObservableCollection<HPTUserSystem>(distinctUserSystemList
        //            .Select(us => new HPTUserSystem()
        //            {
        //                BetType = us.BetType,
        //                Comment = us.Comment,
        //                EMail = us.EMail,
        //                OriginalSize = us.OriginalSize,
        //                RaceDayDate = us.RaceDayDate,
        //                ReducedSize = us.ReducedSize,
        //                ReductionNames = us.ReductionNames,
        //                TrackId = us.TrackId,
        //                UniqueId = us.UniqueId,
        //                UserName = us.UserName,
        //                Timestamp = us.Timestamp
        //            }))
        //    };

        //    return hptUserSystemCollection;
        //}

        // /UserSystemByEmail?eMail={eMail}&betType={betType}&trackId={trackId}&raceDate={raceDate}
        //internal static HPTMarkBet DownloadSystemByEMail(HPTRaceDayInfoLight raceDayInfoLight, string eMail)
        //{
        //    //CultureInfo ci = new CultureInfo("sv-SE");
        //    byte[] baUserSystem = DownloadAndCreateByteArrayStatic(webserviceURL, "UserSystemByEmail?eMail=" + eMail
        //        + "&betType=" + raceDayInfoLight.BetTypeCode
        //        + "&trackId=" + raceDayInfoLight.TrackId.ToString()
        //        + "&raceDate=" + raceDayInfoLight.RaceDayDate.ToString("yyyy-MM-dd")
        //        );

        //    var userSystem = HPTSerializer.DeserializeHPTObject(typeof(HPTMarkBet), baUserSystem);
        //    return (HPTMarkBet)userSystem;
        //}

        //// /UserSystemByUniqueId?uniqueId={uniqueId}&betType={betType}&trackId={trackId}&raceDate={raceDate}
        //internal static HPTMarkBet DownloadSystemByUniqueId(HPTRaceDayInfoLight raceDayInfoLight, string uniqueId)
        //{
        //    //CultureInfo ci = new CultureInfo("sv-SE");
        //    byte[] baUserSystem = DownloadAndCreateByteArrayStatic(webserviceURL, "UserSystemByUniqueId?uniqueId=" + uniqueId
        //        + "&betType=" + raceDayInfoLight.BetTypeCode
        //        + "&trackId=" + raceDayInfoLight.TrackId.ToString()
        //        + "&raceDate=" + raceDayInfoLight.RaceDayDate.ToString("yyyy-MM-dd")
        //        );

        //    var userSystem = HPTSerializer.DeserializeHPTObject(typeof(HPTMarkBet), baUserSystem);
        //    return (HPTMarkBet)userSystem;
        //}

        // /UserSystemDeleteByUniqueId?uniqueId={uniqueId}&betType={betType}&trackId={trackId}&raceDate={raceDate}
        internal static bool DeleteSystemByUniqueId(HPTRaceDayInfoLight raceDayInfoLight, string uniqueId)
        {
            //CultureInfo ci = new CultureInfo("sv-SE");
            var success = DownloadStringStatic("UserSystemDeleteByUniqueId?uniqueId=" + uniqueId
                + "&betType=" + raceDayInfoLight.BetTypeCode
                + "&trackId=" + raceDayInfoLight.TrackId.ToString()
                + "&raceDate=" + raceDayInfoLight.RaceDayDate.ToString("yyyy-MM-dd")
                );

            return bool.Parse(success);
        }

        //internal void UploadOwnInformation()
        //{
        //    var serializer = new XmlSerializer(typeof(HPTHorseOwnInformationCollection));
        //    var baHorseOwnInformationCollection = HPTSerializer.SerializeHPTObject(serializer, HPTConfig.Config.HorseOwnInformationCollection);
        //    var response = client.UploadComments(baHorseOwnInformationCollection, HPTConfig.Config.EMailAddress, HPTConfig.Config.Password, HPTConfig.Config.HorseOwnInformationCollection.Comment);
        //}

        //internal void GetUserOwnInformationByEMail(string eMailAddress)
        //{
        //    try
        //    {
        //        byte[] baUserOwnInformation = DownloadAndCreateByteArray("UserOwnInformationByEmail?eMail=" + eMailAddress);
        //        var userOwnInformation = HPTSerializer.DeserializeHPTHorseOwnInformation(baUserOwnInformation);
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //    }
        //}

        #endregion

        #region Säkerhetskopiera information

        //internal static bool UploadConfiguration()
        //{
        //    try
        //    {
        //        var serializedStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTConfig), HPTConfig.Config);
        //        var baUserConfiguration = HPTSerializer.ZipAndCreateBinary(serializedStream);

        //        var hptConfiguration = new HPTService.HPTUserConfiguration()
        //        {
        //            EMail = HPTConfig.Config.EMailAddress,
        //            Timestamp = DateTime.Now,
        //            UserName = HPTConfig.Config.UserNameForUploads,
        //            UserConfiguration = baUserConfiguration
        //        };

        //        var userconfigurationStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTUserConfiguration), hptConfiguration);
        //        var request = CreateWebRequestForPOST("UploadConfiguration", userconfigurationStream);
        //        var response = request.GetResponse();
        //        var stream = response.GetResponseStream();
        //        var sr = new StreamReader(stream);
        //        string strData = sr.ReadToEnd();
        //        if (strData.Contains("true"))
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception exc)
        //    {
        //        HPTConfig.Config.AddToErrorLog(exc);
        //        return false;
        //    }
        //}

        // RetrieveConfiguration?eMailAddress={emailaddress}&password={password}
        //internal static HPTService.HPTUserConfiguration DownloadConfigurationByEMail(string eMail, string password)
        //{
        //    try
        //    {
        //        //CultureInfo ci = new CultureInfo("sv-SE");
        //        byte[] baUserConfiguration = DownloadAndCreateByteArrayStatic(webserviceURL, "RetrieveConfiguration?emailaddress=" + eMail
        //            + "&password=" + password);

        //        var userConfiguration = HPTSerializer.DeserializeHPTObject(typeof(HPTService.HPTUserConfiguration), baUserConfiguration);
        //        return (HPTService.HPTUserConfiguration)userConfiguration;
        //    }
        //    catch (Exception exc)
        //    {
        //        return null;
        //    }
        //}

        #endregion

        #region HPT Online

        internal bool UploadSystemOnline(HPTMarkBet markBet)
        {
            try
            {
                var raceDayInfoReduction = HPTServiceToHPTHelper.CreateHPTOnlineFromHPTMarkBet(markBet);
                var serializedStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTRaceDayInfoReduction), raceDayInfoReduction);
                var baRaceDayInfoReduction = HPTSerializer.ZipAndCreateBinary(serializedStream);

                var userSystem = new HPTService.HPTUserSystem()
                {
                    BetType = markBet.BetType.Code,
                    Comment = markBet.SystemComment,
                    EMail = HPTConfig.Config.EMailAddress,
                    UserName = markBet.SystemName, // FULLÖSNING
                    //UserName = HPTConfig.Config.UserNameForUploads,
                    HPTSystem = baRaceDayInfoReduction,
                    OriginalSize = markBet.SystemSize,
                    RaceDayDate = markBet.RaceDayInfo.RaceDayDate,
                    ReducedSize = markBet.ReducedSize,
                    ReductionNames = markBet.ToReductionNamesString(),
                    TrackId = markBet.RaceDayInfo.TrackId,
                    Timestamp = markBet.LastSaveTime
                };

                var userSystemStream = HPTSerializer.SerializeHPTServiceObject(typeof(HPTService.HPTUserSystem), userSystem);
                var request = CreateWebRequestForPOST("UploadOnlineSystem", userSystemStream);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                //var sr = new StreamReader(stream);
                //string strData = sr.ReadToEnd();

                var saveSystem = (HPTService.HPTSaveSystem)HPTSerializer.DeserializeObjectFromStream(typeof(HPTService.HPTSaveSystem), stream);
                string s = saveSystem.ToString();

                //var baSaveSystem = Convert.FromBase64String(strData);
                //var saveSystem = (HPTService.HPTSaveSystem)HPTSerializer.DeserializeHPTServiceObject(typeof(HPTService.HPTSaveSystem), baSaveSystem);

                return true;
            }
            catch (Exception exc)
            {
                HPTConfig.Config.AddToErrorLog(exc);
                return false;
            }
        }

        //// /UserSystemOnlineByEmail?eMail={eMail}&startDate={startDate}&endDate={endDate}
        //internal static List<HPTService.HPTUserSystem> DownloadSystemOnlineList(DateTime startDate, DateTime endDate)
        //{
        //    byte[] baUserSystemList = DownloadAndCreateByteArrayStatic(webserviceURL, "UserSystemOnlineByEmail?eMail=" + HPTConfig.Config.EMailAddress
        //        + "&startDate=" + startDate.ToString("yyyy-MM-dd")
        //        + "&endDate=" + endDate.ToString("yyyy-MM-dd")
        //        );

        //    var userSystemList = (List<HPTService.HPTUserSystem>)HPTSerializer.DeserializeHPTServiceObject(typeof(List<HPTService.HPTUserSystem>), baUserSystemList);
        //    return userSystemList;
        //}

        //// /UserSystemOnlineByUniqueId?systemGUID={systemGUID}
        //internal static HPTMarkBet DownloadSystemOnlineByUniqueId(string systemGUID)
        //{
        //    byte[] baUserSystem = DownloadAndCreateByteArrayStatic(webserviceURL, "UserSystemOnlineByUniqueId?systemGUID=" + systemGUID);

        //    var raceDayInfoReduction = (HPTService.HPTRaceDayInfoReduction)HPTSerializer.DeserializeHPTObject(typeof(HPTService.HPTRaceDayInfoReduction), baUserSystem);
        //    var serviceConnector = new HPTServiceConnector();
        //    var raceDayInfo = serviceConnector.GetRaceDayInfoByTrackAndDate(raceDayInfoReduction.BetTypeCode, raceDayInfoReduction.RaceDayDateString, raceDayInfoReduction.TrackId.ToString());

        //    string raceDayDirectory = HPTConfig.MyDocumentsPath + raceDayInfo.ToDateAndTrackString();
        //    if (!Directory.Exists(raceDayDirectory))
        //    {
        //        Directory.CreateDirectory(raceDayDirectory);
        //    }
        //    raceDayInfo.DataToShow = HPTConfig.Config.DataToShowVxx;

        //    var markBet = new HPTMarkBet(raceDayInfo, raceDayInfo.BetType);
        //    markBet.SaveDirectory = raceDayDirectory + "\\";
        //    markBet.Config = HPTConfig.Config;
        //    HPTServiceToHPTHelper.ApplyHPTOnlineToHPTMarkBet(markBet, raceDayInfoReduction);
        //    HPTServiceToHPTHelper.SetNonSerializedValues(markBet);
        //    markBet.SystemSize = raceDayInfoReduction.RaceList
        //        .Select(r => r.HorseList.Count(h => h.Selected))
        //        .Aggregate((numberOfSelected, next) => numberOfSelected * next);

        //    return markBet;
        //}

        #endregion

        #region Hämtning från ATGs REST-tjänst

        // https://www.atg.se/services/v1/horses/693542/results?stopdate=2015-09-24
        internal void GetHorseResultListFromATG(HPTHorse horse)
        {
            if (horse.ResultList.Count < 5)
            {
                return;
            }
            var WCATG = new WebClient()
            {
                BaseAddress = "https://www.atg.se/services/v1/horses/",
                Encoding = Encoding.UTF8
            };

            string requestUrl = horse.ATGId + "/results?stopdate=" + horse.ResultList.Min(hr => hr.Date).AddDays(-1D).ToString("yyyy-MM-dd");
            string json = WCATG.DownloadString(requestUrl);
            var recordList = (ATGHorseRecordList)HPTSerializer.DeserializeJson(typeof(ATGHorseRecordList), json);

            var resultList = recordList.records.Select(r => new HPTHorseResult()
            {
                Date = DateTime.Parse(r.date),
                Distance = r.start.distance,
                Driver = r.start.driver.shortName,
                FirstPrize = r.race.firstPrize / 100,
                Odds = string.IsNullOrEmpty(r.odds) ? r.oddsCode : r.odds,
                Place = string.IsNullOrWhiteSpace(r.place) ? 0 : int.Parse(r.place),
                PlaceString = r.place,
                Position = r.start.postPosition,
                StartNr = 0,
                RaceNr = r.race.number,
                RaceType = r.race.type,
                Shoeinfo = CreateHorseShoeInfo(r.start.horse.shoes),
                TrackCode = EnumHelper.GetTrackCodeFromTrackId(r.track.id),
                Time = CreateKmTime(r.kmTime)
            });

            resultList
                .OrderByDescending(r => r.Date)
                .ToList()
                .ForEach(r => horse.ResultList.Add(r));
        }

        internal string CreateKmTime(KmTime kmTime)
        {
            if (kmTime == null)
            {
                return string.Empty;
            }
            if (kmTime.minutes == 0)
            {
                return kmTime.code;
            }
            string formattedTime = kmTime.seconds.ToString() + "." + kmTime.tenths.ToString();
            if (kmTime.code != null)
            {
                formattedTime += kmTime.code;
            }
            return formattedTime;
        }

        internal HPTHorseShoeInfo CreateHorseShoeInfo(Shoes shoes)
        {
            if (shoes == null)
            {
                return null;
            }
            return new HPTHorseShoeInfo()
            {
                Foreshoes = shoes.front,
                Hindshoes = shoes.back
            };
        }

        //// https://www.atg.se/services/v1/drivers/53685/
        //internal void GetDriverInfoFromATG(HPTHorse horse)
        //{
        //    try
        //    {
        //        if (horse.DriverId == "0" || horse.DriverInfo != null)
        //        {
        //            return;
        //        }
        //        var WCATG = new WebClient()
        //        {
        //            BaseAddress = "https://www.atg.se/services/v1/drivers/",
        //            Encoding = Encoding.UTF8
        //        };

        //        string requestUrl = horse.DriverId;
        //        //string requestUrl = "53685";
        //        string json = WCATG.DownloadString(requestUrl);
        //        horse.DriverInfo = (ATGDriverInformation)HPTSerializer.DeserializeJson(typeof(ATGDriverInformation), json);
        //        horse.DriverInfo.statistics.years.ToList().ForEach(y =>
        //        {
        //            y.Value.earnings /= 100;
        //            y.Value.winPercentage /= 10000M;
        //        });

        //        // Kopiera DriverInfo till övriga hästar som kusken kör
        //        if (horse.ParentRace != null && horse.ParentRace.ParentRaceDayInfo != null)
        //        {
        //            horse.ParentRace.ParentRaceDayInfo.RaceList
        //                .SelectMany(r => r.HorseList)
        //                .Where(h => h.DriverId == horse.DriverId && h != horse)
        //                .ToList()
        //                .ForEach(h => h.DriverInfo = horse.DriverInfo);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //}

        //// https://www.atg.se/services/v1/races/2016-03-11_12_4/start/2
        //internal void GetStartInfoFromATG(HPTHorse horse)
        //{
        //    try
        //    {
        //        if (horse.DriverId == "0" || horse.DriverInfo != null)
        //        {
        //            return;
        //        }
        //        var WCATG = new WebClient()
        //        {
        //            BaseAddress = "https://www.atg.se/services/v1/drivers/",
        //            Encoding = Encoding.UTF8
        //        };

        //        string requestUrl = horse.DriverId;
        //        string json = WCATG.DownloadString(requestUrl);
        //        horse.DriverInfo = (ATGDriverInformation)HPTSerializer.DeserializeJson(typeof(ATGDriverInformation), json);
        //        horse.DriverInfo.statistics.years.ToList().ForEach(y =>
        //        {
        //            y.Value.earnings /= 100;
        //            y.Value.winPercentage /= 10000M;
        //        });

        //        // Kopiera DriverInfo till övriga hästar som kusken kör
        //        if (horse.ParentRace != null && horse.ParentRace.ParentRaceDayInfo != null)
        //        {
        //            horse.ParentRace.ParentRaceDayInfo.RaceList
        //                .SelectMany(r => r.HorseList)
        //                .Where(h => h.DriverId == horse.DriverId && h != horse)
        //                .ToList()
        //                .ForEach(h => h.DriverInfo = horse.DriverInfo);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //}

        // https://www.atg.se/services/v1/races/2016-03-11_12_4/start/2
        // https://www.atg.se/services/racinginfo/v1/api/races/2017-09-19_14_4/start/3
        internal void GetHorseStartInformationFromATG(HPTHorse horse)
        {
            try
            {
                var WCATG = new WebClient()
                {
                    //BaseAddress = "https://www.atg.se/services/v1/races/",
                    BaseAddress = "https://www.atg.se/services/racinginfo/v1/api/races/",
                    Encoding = Encoding.UTF8,
                    
                };
                //WCATG.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                var rdi = horse.ParentRace.ParentRaceDayInfo;
                var race = horse.ParentRace;

                string requestUrl = string.Format("{0}_{1}_{2}/start/{3}", rdi.RaceDayDateString, race.TrackId == null ? rdi.TrackId : race.TrackId, race.RaceNr, horse.StartNr);
                string json = WCATG.DownloadString(requestUrl);
                var startInfo = (ATGHorseRaceInformation)HPTSerializer.DeserializeJson(typeof(ATGHorseRaceInformation), json);

                // Uppdatera listan med tidigare resultat
                var resultList = startInfo.horse.results.records.Select(r => new HPTHorseResult()
                {
                    Date = DateTime.Parse(r.date),
                    Distance = r.start.distance,
                    Driver = r.start.driver.shortName,
                    FirstPrize = r.race.firstPrize / 100,
                    Odds = string.IsNullOrEmpty(r.odds) ? r.oddsCode : r.odds,
                    Place = string.IsNullOrWhiteSpace(r.place) ? 0 : int.Parse(r.place),
                    PlaceString = r.place,
                    Position = r.start.postPosition,
                    StartNr = 0,
                    RaceNr = r.race.number,
                    RaceType = r.race.type,
                    Shoeinfo = CreateHorseShoeInfo(r.start.horse.shoes),
                    TrackCode = EnumHelper.GetTrackCodeFromTrackId(r.track.id),
                    Time = CreateKmTime(r.kmTime)
                });

                resultList
                    .Where(r => r.Date < horse.ResultList.Min(rOld => rOld.Date))
                    .OrderByDescending(r => r.Date)
                    .ToList()
                    .ForEach(r => horse.ResultList.Add(r));

                // Uppdatera kuskinformationen
                horse.DriverInfo = startInfo.driver;
                horse.DriverInfo.statistics.years
                    .ToList()
                    .ForEach(y =>
                    {
                        y.Value.earnings /= 100;
                        y.Value.winPercentage /= 10000M;
                    });

                // Kopiera DriverInfo till övriga hästar som kusken kör
                if (horse.ParentRace != null && horse.ParentRace.ParentRaceDayInfo != null)
                {
                    horse.ParentRace.ParentRaceDayInfo.RaceList
                        .SelectMany(r => r.HorseList)
                        .Where(h => h.DriverId == horse.DriverId && h != horse)
                        .ToList()
                        .ForEach(h => h.DriverInfo = horse.DriverInfo);
                }

                // Uppdatera tränarinformationen
                horse.TrainerInfo = startInfo.horse.trainer;
                horse.TrainerInfo.statistics.years
                    .ToList()
                    .ForEach(y =>
                    {
                        y.Value.earnings /= 100;
                        y.Value.winPercentage /= 10000M;
                    });

                // Kopiera TrainerInfo till övriga hästar som tränaren har
                if (horse.ParentRace != null && horse.ParentRace.ParentRaceDayInfo != null)
                {
                    horse.ParentRace.ParentRaceDayInfo.RaceList
                        .SelectMany(r => r.HorseList)
                        .Where(h => h.TrainerId == horse.TrainerId && h != horse)
                        .ToList()
                        .ForEach(h => h.TrainerInfo = horse.TrainerInfo);
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        #endregion
    }

    //internal class HPTWebClient : WebClient
    //{
    //    public int Timeout { get; set; }

    //    protected override WebRequest GetWebRequest(Uri uri)
    //    {
    //        WebRequest lWebRequest = base.GetWebRequest(uri);
    //        lWebRequest.Timeout = Timeout;
    //        ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
    //        return lWebRequest;
    //    }
    //}

}

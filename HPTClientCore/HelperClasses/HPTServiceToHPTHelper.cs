using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HPTClient
{
    public class HPTServiceToHPTHelper
    {
        public static void CreateCalendar(byte[] baCalendar, HPTCalendar hptCalendar)
        {
            DateTime dtStart = DateTime.Now;
            try
            {
                HPTService.HPTCalendar calendar = HPTSerializer.DeserializeHPTCalendar(baCalendar);
                HPTServiceToHPTHelper.ConvertCalendar(calendar, hptCalendar);

                if (hptCalendar.RaceDayInfoList == null)
                {
                    hptCalendar.RaceDayInfoList = new ObservableCollection<HPTRaceDayInfo>();
                }
                else if (hptCalendar.RaceDayInfoList.Count > 0)
                {
                    hptCalendar.RaceDayInfoList = new ObservableCollection<HPTRaceDayInfo>();
                }

                // Kommande tävlingar
                var orderedList = calendar.HPTRaceDayInfoList
                    .Where(rdi => rdi.SharedInfo.RaceDayDate.Date >= DateTime.Today)
                    .OrderBy(rdi => rdi.SharedInfo.FirstRaceStartTime);

                foreach (HPTService.HPTRaceDayInfo rdi in orderedList)
                {
                    try
                    {
                        HPTRaceDayInfo hptRdi = new HPTRaceDayInfo();
                        HPTServiceToHPTHelper.ConvertCalendarRaceDayInfo(rdi, hptRdi);
                        if (hptRdi.BetTypeList.Count > 0)
                        {
                            hptRdi.ShowInUI = true;
                            hptCalendar.RaceDayInfoList.Add(hptRdi);
                        }
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }

                // Tidigare tävlingar
                var orderedListOld = calendar.HPTRaceDayInfoList
                    .Where(rdi => rdi.SharedInfo.RaceDayDate.Date < DateTime.Today)
                    .OrderByDescending(rdi => rdi.SharedInfo.FirstRaceStartTime);

                foreach (HPTService.HPTRaceDayInfo rdi in orderedListOld)
                {
                    try
                    {
                        HPTRaceDayInfo hptRdi = new HPTRaceDayInfo();
                        HPTServiceToHPTHelper.ConvertCalendarRaceDayInfo(rdi, hptRdi);
                        if (hptRdi.BetTypeList.Count > 0)
                        {
                            hptCalendar.RaceDayInfoList.Insert(0, hptRdi);
                        }
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }

                // Skapa lista med de tävlingar man ska kunna ladda ner systemförslag för
                var markBetSystemList = hptCalendar.RaceDayInfoList
                    .Where(hptRdi => hptRdi.RaceDayDate < DateTime.Now.AddDays(6D) && hptRdi.RaceDayDate > DateTime.Now.AddDays(-3D));

                foreach (var markBet in markBetSystemList)
                {
                    var betTypeList = markBet.BetTypeList
                        .Where(bt => bt.Code.StartsWith("V"))
                        .Select(bt => new HPTRaceDayInfoLight()
                        {
                            BetTypeCode = bt.Code,
                            RaceDayDate = markBet.RaceDayDate,
                            TrackId = markBet.TrackId,
                            TrackName = markBet.Trackname,
                            NumberOfUploadedSystems = bt.NumberOfUploadedSystems
                        }).OrderBy(hptRdi => hptRdi.RaceDayDate);

                    foreach (var raceDayInfoLight in betTypeList)
                    {
                        HPTConfig.Config.MarkBetSystemList.Add(raceDayInfoLight);
                    }
                }

                // Sätt sökväg och spara ner kalender på disk
                string calendarPath = HPTConfig.MyDocumentsPath + "\\HPTCalendar.hptcal";

                TimeSpan ts = DateTime.Now - dtStart;
                string time = ts.TotalMilliseconds.ToString();
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
                throw exc;
            }
        }

        public static void ConvertCalendar(HPTService.HPTCalendar calendar, HPTCalendar hptCalendar)
        {
            hptCalendar.FromDate = calendar.FromDate;
            //hptCalendar.FromDateString = hptCalendar.FromDate.ToString("yyyy-MM-dd");
            hptCalendar.ToDate = calendar.ToDate;
            //hptCalendar.ToDateString = hptCalendar.ToDate.ToString("yyyy-MM-dd");
        }

        public static void ConvertCalendarRaceDayInfo(HPTService.HPTRaceDayInfo rdi, HPTRaceDayInfo hptRdi)
        {
            // Info som behövs för visning i kalendern
            hptRdi.RaceDayDate = rdi.SharedInfo.FirstRaceStartTime;
            hptRdi.TrackId = rdi.SharedInfo.TrackId;
            hptRdi.Trackcode = rdi.SharedInfo.Trackcode;
            hptRdi.Trackname = rdi.SharedInfo.Trackname;
            hptRdi.TrackCondition = rdi.SharedInfo.TrackCondition;
            hptRdi.Turnover = rdi.Turnover;

            // Lista med alla tillgägnliga speltyper
            hptRdi.BetTypeList = new List<HPTBetType>();
            if (rdi.SharedInfo.BetTypeList != null)
            {
                foreach (HPTService.HPTBetType bt in rdi.SharedInfo.BetTypeList)
                {
                    //if (bt.Jackpot)
                    //{
                    //    string s = bt.ToString();
                    //}
                    var hptBt = new HPTBetType()
                    {
                        Code = bt.Code,
                        Name = bt.Name,
                        NumberOfUploadedSystems = bt.NumberOfUploadedSystems,
                        Jackpot = bt.Jackpot,
                        StartTime = bt.StartTime,
                        EndTime = bt.EndTime,
                        IsEnabled = true,
                        TrackId = hptRdi.TrackId,
                        CalendarRaceDayInfo = hptRdi
                    };
                    hptRdi.BetTypeList.Add(hptBt);

                    //if (!HPTConfig.Config.IsPayingCustomer)
                    //{
                    //    switch (bt.Code)
                    //    {
                    //        case "V4":
                    //        case "V64":
                    //        case "V65":
                    //        case "V86":
                    //            if (rdi.SharedInfo.RaceDayDate >= DateTime.Today.AddDays(2))
                    //            {
                    //                hptBt.IsEnabled = false;
                    //            }
                    //            break;
                    //        case "V75":
                    //        case "V5":
                    //        case "DD":
                    //        case "LD":
                    //        case "TV":
                    //        case "T":
                    //            hptBt.IsEnabled = false;
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    //else
                    //{
                    //    hptBt.IsEnabled = true;
                    //}
                }
            }
        }

        public static void ConvertRaceDayInfo(HPTService.HPTRaceDayInfo rdi, HPTRaceDayInfo hptRdi)
        {
            // Typ av spel
            hptRdi.BetType = new HPTBetType()
            {
                Code = rdi.BetType.Code,
                Name = rdi.BetType.Name
            };

            // Jackpottsumma och antal streck
            hptRdi.Jackpot = rdi.Jackpot;
            hptRdi.MarksQuantity = rdi.MarksQuantity;
            //hptRdi.NumberOfUploadedSystems = rdi.NumberOfUploadedSystems;         

            hptRdi.RaceDayDate = rdi.SharedInfo.FirstRaceStartTime;
            if (rdi.SharedInfo.RaceNumberList != null)
            {
                hptRdi.RaceNumberList = rdi.SharedInfo.RaceNumberList.ToList();
            }
            else if (rdi.SharedInfo.RaceNumberInfoList != null)
            {
                hptRdi.RaceNumberList = rdi.SharedInfo.RaceNumberInfoList.Select(rni => rni.LegNumber).ToList();
            }

            hptRdi.TrackId = rdi.SharedInfo.TrackId;
            hptRdi.Trackcode = rdi.SharedInfo.Trackcode;
            hptRdi.Trackname = rdi.SharedInfo.Trackname;
            hptRdi.Turnover = rdi.Turnover;

            hptRdi.RaceList = new List<HPTRace>();

            if (rdi.LegList != null)
            {
                try
                {
                    // TEST
                    DateTime dt = DateTime.Now;
                    foreach (HPTService.HPTRace race in rdi.LegList)
                    {
                        HPTRace hptRace = new HPTRace();
                        hptRace.ParentRaceDayInfo = hptRdi;
                        ConvertRace(race, hptRace);
                        hptRdi.RaceList.Add(hptRace);

                        //// Specialhantering för saxade banor
                        //if (rdi.SharedInfo.RaceNumberInfoList != null)
                        //{
                        //    var raceNumberInfo = rdi.SharedInfo.RaceNumberInfoList.FirstOrDefault(rni => rni.LegNumber == hptRace.LegNr);
                        //    if (raceNumberInfo != null)
                        //    {
                        //        hptRace.TrackId = raceNumberInfo.HostTrackId;
                        //    }
                        //}
                    }
                    TimeSpan ts = DateTime.Now - dt;
                    string time = ts.TotalMilliseconds.ToString();
                }
                catch (ArgumentException exc)   // Something wrong with legNr
                {
                    string s = exc.Message;
                    hptRdi.RaceList = new List<HPTRace>();

                    for (int i = 0; i < rdi.LegList.Length; i++)
                    {
                        HPTService.HPTRace race = rdi.LegList[i];
                        HPTRace hptRace = new HPTRace();
                        hptRace.ParentRaceDayInfo = hptRdi;
                        ConvertRace(race, hptRace);
                        hptRace.LegNr = i + 1;
                        hptRdi.RaceList.Add(hptRace);
                        //hptRdi.MarksQuantity += race.MarksQuantity;
                    }
                }
            }

            // Hantera potterna för olika antal rätt
            CreatePayOutLists(rdi, hptRdi);
            if (hptRdi.PayOutListATG != null && hptRdi.PayOutListATG.Count > 0)
            {
                hptRdi.MaxPayOut = hptRdi.PayOutListATG
                    .OrderByDescending(po => po.NumberOfCorrect)
                    .First()
                    .TotalAmount;

                hptRdi.SetV6Factor();
            }

            // Combinationbet (Dagens Dubbel, LunchDubbel)
            if (hptRdi.BetType.Code == "DD" || hptRdi.BetType.Code == "LD")
            {
                hptRdi.CombinationListInfoDouble = new HPTCombinationListInfo()
                {
                    CombinationList = new List<HPTCombination>(),
                    BetType = hptRdi.BetType.Code
                };
                if (rdi.CombinationList != null && rdi.CombinationList.Length > 0)
                {
                    hptRdi.CombinationListInfoDouble.CombinationList = new List<HPTCombination>();

                    foreach (HPTService.HPTCombination comb in rdi.CombinationList)
                    {
                        HPTCombination hptComb = new HPTCombination();
                        hptComb.ParentRaceDayInfo = hptRdi;
                        hptComb.CombinationOdds = comb.CombinationOdds;
                        hptComb.CombinationOddsExact = comb.CombinationOddsExact;
                        hptComb.Horse1Nr = comb.Horse1Nr;
                        hptComb.Horse2Nr = comb.Horse2Nr;
                        hptComb.Horse3Nr = comb.Horse3Nr;

                        hptComb.Horse1 = hptRdi.RaceList[0].GetHorseByNumber(comb.Horse1Nr);
                        hptComb.Horse2 = hptRdi.RaceList[1].GetHorseByNumber(comb.Horse2Nr);

                        hptComb.CalculateQuotas(rdi.BetType.Code);
                        hptRdi.CombinationListInfoDouble.CombinationList.Add(hptComb);
                    }

                    // Sätt egen chansvärdering
                    foreach (var race in hptRdi.RaceList)
                    {
                        foreach (var horse in race.HorseList)
                        {
                            horse.OwnProbability = horse.StakeShareRounded;
                        }
                    }
                    hptRdi.SortCombinationValues();
                }
                else
                {
                    hptRdi.CombinationListInfoDouble.CombinationList = new List<HPTCombination>();
                    HPTRace hptRace1 = hptRdi.RaceList[0];
                    HPTRace hptRace2 = hptRdi.RaceList[1];
                    foreach (HPTHorse horse1 in hptRace1.HorseList)
                    {
                        foreach (HPTHorse horse2 in hptRace2.HorseList)
                        {
                            HPTCombination hptComb = new HPTCombination();
                            hptComb.ParentRaceDayInfo = hptRdi;
                            hptComb.CombinationOdds = 0M;
                            hptComb.CombinationOddsExact = 0M;
                            hptComb.Horse1Nr = horse1.StartNr;
                            hptComb.Horse2Nr = horse2.StartNr;

                            hptComb.Horse1 = horse1;
                            hptComb.Horse2 = horse2;

                            hptComb.CalculateQuotas(rdi.BetType.Code);
                            hptRdi.CombinationListInfoDouble.CombinationList.Add(hptComb);
                        }
                    }
                }
            }

            // Sätt avstånd till hemmabanan (i km) för alla hästar
            SetDistanceToHomeTrack(hptRdi);

            // DEBUG
            //TimeSpan ts = DateTime.Now - dtStart;
            //double d = ts.TotalMilliseconds;
        }

        internal static void CreatePayOutLists(HPTService.HPTRaceDayInfo rdi, HPTRaceDayInfo hptRdi)
        {
            if (rdi.PayoutList != null)
            {
                IEnumerable<HPTPayOut> payOutList = rdi.PayoutList.Select(po => new HPTPayOut()
                {
                    NumberOfCorrect = po.NumberOfCorrect,
                    NumberOfSystems = po.NumberOfSystems,
                    PayOutAmount = po.PayOutAmount,
                    TotalAmount = po.TotalAmount
                });
                hptRdi.PayOutListATG = new ObservableCollection<HPTPayOut>(payOutList);
                if (hptRdi.Jackpot > 0)
                {
                    var payOut = hptRdi.PayOutListATG.FirstOrDefault(po => po.NumberOfCorrect == hptRdi.RaceList.Count);
                    if (payOut.TotalAmount != hptRdi.Jackpot)
                    {
                        hptRdi.JackpotFactor = Convert.ToDecimal(payOut.TotalAmount) / Convert.ToDecimal(payOut.TotalAmount - hptRdi.Jackpot);
                        if (hptRdi.JackpotFactor > 2M && DateTime.Now < hptRdi.RaceList.First().PostTime)
                        {
                            hptRdi.JackpotFactor = 2M;
                        }
                    }
                }
                else
                {
                    hptRdi.JackpotFactor = 1M;
                }
                if (hptRdi.BetType.Code == "V4" || hptRdi.BetType.Code == "V5")
                {
                    if (hptRdi.PayOutListATG.First().TotalAmount == 0)
                    {
                        hptRdi.PayOutListATG.First().TotalAmount += Convert.ToInt32(Convert.ToDecimal(hptRdi.Turnover) * hptRdi.BetType.PoolShare);
                    }
                }
            }
        }

        //public static HPTRace ConvertRace(HPTService.HPTRace race)
        //{
        //    var hptRace = new HPTRace
        //}

        public static void ConvertRace(HPTService.HPTRace race, HPTRace hptRace)
        {
            hptRace.Distance = race.SharedInfo.Distance;
            hptRace.StartMethodCode = race.SharedInfo.StartMethodCode;
            hptRace.PostTime = race.SharedInfo.PostTime;

            Regex rexExtractNumber = new Regex("\\d+");
            if (!string.IsNullOrEmpty(hptRace.Distance) && rexExtractNumber.IsMatch(hptRace.Distance))
            {
                Match m = rexExtractNumber.Match(hptRace.Distance);
                int iDistance = int.Parse(m.Value);
                if (iDistance < 1800)
                {
                    hptRace.DistanceCode = "K";
                }
                else if (iDistance > 2500)
                {
                    hptRace.DistanceCode = "L";
                }
                else
                {
                    hptRace.DistanceCode = "M";
                }

                if (hptRace.StartMethodCode == "A")
                {
                    hptRace.StartMethodAndDistanceCode = hptRace.StartMethodCode + hptRace.DistanceCode;
                }
                else
                {
                    hptRace.StartMethodAndDistanceCode = hptRace.DistanceCode;
                }
            }
            else
            {
                hptRace.DistanceCode = string.Empty;
            }

            //hptRace.MarksQuantity = Convert.ToInt32(race.MarksQuantity);
            hptRace.RaceNr = race.SharedInfo.RaceNr;
            hptRace.LegNr = race.LegNr;
            hptRace.RaceName = race.SharedInfo.RaceName;
            hptRace.RaceInfoShort = race.SharedInfo.RaceInfoShort.Replace("\n", string.Empty);
            hptRace.RaceInfoLong = race.SharedInfo.RaceInfoLong;
            hptRace.RaceShortText = "Lopp " + hptRace.RaceNr.ToString();
            //hptRace.ReservOrder = race.ReservOrder;

            hptRace.ReservOrderList = new int[0];
            //if (!string.IsNullOrWhiteSpace(hptRace.ReservOrder))
            //{
            //    string[] reservStringArray = hptRace.ReservOrder.Split('-');
            //    hptRace.ReservOrderList = new int[reservStringArray.Length];
            //    for (int i = 0; i < reservStringArray.Length; i++)
            //    {
            //        hptRace.ReservOrderList[i] = int.Parse(reservStringArray[i]);
            //    }
            //}

            hptRace.StartMethod = race.SharedInfo.StartMethod;
            hptRace.StartMethodCode = race.SharedInfo.StartMethodCode;
            hptRace.TurnoverPlats = race.SharedInfo.TurnoverPlats;
            hptRace.TurnoverTvilling = race.SharedInfo.TurnoverTvilling;
            hptRace.TurnoverVinnare = race.SharedInfo.TurnoverVinnare;
            hptRace.TurnoverTrio = race.SharedInfo.TurnoverTrio;
            hptRace.TrackId = race.TrackId;
            //hptRace.BetTypes = race.SharedInfo.BetTypes.ToArray();

            if (race.HorseList != null)
            {
                //hptRace.HorseList = new ObservableCollection<HPTHorse>();
                hptRace.HorseList = new List<HPTHorse>();
                foreach (HPTService.HPTHorse horse in race.HorseList)
                {
                    var startNumberRankCollection = HPTConfig.Config.StartNumberRankCollectionList.FirstOrDefault(snr => snr.StartMethodCode == hptRace.StartMethodCode);
                    var hptHorse = hptRace.HorseList.FirstOrDefault(h => h.StartNr == horse.StartInfo.StartNr);
                    if (hptHorse == null)
                    {
                        hptHorse = new HPTHorse();
                        hptHorse.ParentRace = hptRace;
                        ConvertHorse(horse, hptHorse);
                        hptRace.HorseList.Add(hptHorse);
                        hptHorse.CreateXReductionRuleList();
                    }

                    // Sätt Spårank utifrån Config
                    if (startNumberRankCollection != null)
                    {
                        var startNumberRank = startNumberRankCollection.StartNumberRankList.FirstOrDefault(sr => sr.StartNumber == hptHorse.StartNr);
                        if (startNumberRank != null && hptHorse.Scratched != true)
                        {
                            hptHorse.Selected = startNumberRank.Select;
                            hptHorse.RankStartNumber = startNumberRank.Rank;
                        }
                    }
                }

                // Sätt ATG-rank utifrån reservordning
                for (int i = 0; i < hptRace.ReservOrderList.Length; i++)
                {
                    HPTHorse hptHorse = hptRace.GetHorseByNumber(hptRace.ReservOrderList[i]);
                    hptHorse.RankATG = i + 1;
                }

                // Sätt ATG-rank utifrån startpoäng
                int rank = 1;
                hptRace.HorseList
                    .OrderByDescending(h => h.StartPoint)
                    .ToList()
                    .ForEach(h => h.RankATG = rank++);

                // Sätt korrekt insatsfördelning pga V6
                hptRace.SetCorrectStakeDistributionShare();
                hptRace.SetCorrectStakeDistributionShareAlt1();
                hptRace.SetCorrectStakeDistributionShareAlt2();
            }

            // Tvilling
            if (hptRace.ParentRaceDayInfo.BetType.Code == "TV")
            {
                hptRace.CombinationListInfoTvilling = new HPTCombinationListInfo()
                {
                    CombinationList = new List<HPTCombination>(),
                    BetType = hptRace.ParentRaceDayInfo.BetType.Code
                };

                if (race.SharedInfo.TvillingCombinationList != null && race.SharedInfo.TvillingCombinationList.Length > 0)
                {
                    foreach (HPTService.HPTCombination comb in race.SharedInfo.TvillingCombinationList)
                    {
                        HPTCombination hptComb = new HPTCombination();
                        hptComb.ParentRace = hptRace;
                        hptComb.ParentRaceDayInfo = hptRace.ParentRaceDayInfo;
                        hptComb.CombinationOdds = comb.CombinationOdds;
                        hptComb.CombinationOddsExact = comb.CombinationOddsExact;
                        hptComb.Horse1Nr = comb.Horse1Nr;
                        hptComb.Horse2Nr = comb.Horse2Nr;
                        hptComb.Horse3Nr = comb.Horse3Nr;

                        hptComb.Horse1 = hptRace.GetHorseByNumber(comb.Horse1Nr);
                        hptComb.Horse2 = hptRace.GetHorseByNumber(comb.Horse2Nr);

                        hptComb.CalculateQuotas("TV");
                        hptRace.CombinationListInfoTvilling.CombinationList.Add(hptComb);
                    }
                    hptRace.CombinationListInfoTvilling.SortCombinationValues();
                }
                foreach (var horse in hptRace.HorseList)
                {
                    horse.OwnProbability = horse.StakeShareRounded;
                }
            }

            // Trio
            if (hptRace.ParentRaceDayInfo.BetType.Code == "T")
            {
                hptRace.CombinationListInfoTrio = new HPTCombinationListInfo()
                {
                    CombinationList = new List<HPTCombination>(),
                    BetType = hptRace.ParentRaceDayInfo.BetType.Code
                };

                //hptRace.CombinationListInfoTrio.CalculatePercentages(hptRace.HorseList);                
                hptRace.CreateAllTrioCombinations(race.SharedInfo.TrioCombinationList);
                hptRace.CombinationListInfoTrio.SortCombinationValues();
            }

            // Inbördes möte hästar emellan
            hptRace.FindHeadToHead();
        }

        public static void ConvertHorse(HPTService.HPTHorse horse, HPTHorse hptHorse)
        {
            // Startinfo (statisk)
            hptHorse.StartNr = horse.StartInfo.StartNr;
            hptHorse.TrainerName = horse.StartInfo.TrainerName;
            hptHorse.TrainerNameShort = horse.StartInfo.TrainerNameShort;
            hptHorse.Scratched = horse.StartInfo.Scratched;
            hptHorse.OwnerName = horse.StartInfo.Owner;
            hptHorse.BreederName = horse.StartInfo.Breeder;
            hptHorse.Age = horse.StartInfo.Age;
            hptHorse.Sex = horse.StartInfo.Sex;
            hptHorse.StartPoint = horse.StartInfo.StartPoint;
            hptHorse.Distance = horse.StartInfo.Distance;

            // Unika nycklar för häst, kusk och tränare
            if (horse.StartInfo.ATGId != 0)
            {
                hptHorse.ATGId = horse.StartInfo.ATGId.ToString();
            }
            if (horse.StartInfo.DriverId != 0)
            {
                hptHorse.DriverId = horse.StartInfo.DriverId.ToString();
            }
            if (horse.StartInfo.TrainerId != 0)
            {
                hptHorse.TrainerId = horse.StartInfo.TrainerId.ToString();
            }

            // Hemmabana
            hptHorse.HomeTrack = horse.StartInfo.HomeTrack;
            hptHorse.IsHomeTrack = hptHorse.ParentRace.ParentRaceDayInfo.Trackcode == hptHorse.HomeTrack;
            if (hptHorse.ParentRace.TrackId != null)
            {
                hptHorse.IsHomeTrack = hptHorse.ParentRace.TrackCode == hptHorse.HomeTrack;
            }

            hptHorse.PostPosition = horse.StartInfo.PostPosition;
            hptHorse.DriverName = horse.StartInfo.DriverName;
            hptHorse.DriverNameShort = horse.StartInfo.DriverNameShort;
            hptHorse.HorseName = horse.StartInfo.HorseName;
            hptHorse.DriverChanged = horse.StartInfo.DriverChanged;

            // Skoinformation
            hptHorse.ShoeInfoCurrent = CreateShoeInfo(horse.StartInfo.ShoeInfoCurrent);
            hptHorse.ShoeInfoPrevious = CreateShoeInfo(horse.StartInfo.ShoeInfoPrevious);
            if (hptHorse.ShoeInfoCurrent.Foreshoes == null && hptHorse.ShoeInfoCurrent.Hindshoes == null)
            {
                hptHorse.ShoeInfoCurrent.Foreshoes = hptHorse.ShoeInfoPrevious.Foreshoes;
                hptHorse.ShoeInfoCurrent.Hindshoes = hptHorse.ShoeInfoPrevious.Hindshoes;
                hptHorse.ShoeInfoCurrent.PreviousUsed = true;
            }
            else
            {
                hptHorse.ShoeInfoCurrent.SetChangedFlags(hptHorse.ShoeInfoPrevious);
                hptHorse.ShoeInfoCurrent.PreviousUsed = false;
            }

            // Vagninformation
            if (hptHorse.SulkyInfoCurrent == null)
            {
                hptHorse.SulkyInfoCurrent = new HPTHorseSulkyInfo()
                {
                    Text = horse.StartInfo.SulkyInfoCurrent?.Text
                };
            }
            else if (horse.StartInfo != null && horse.StartInfo.SulkyInfoCurrent != null)
            {
                hptHorse.SulkyInfoCurrent.Text = horse.StartInfo.SulkyInfoCurrent?.Text;
            }

            if (hptHorse.SulkyInfoPrevious == null)
            {
                hptHorse.SulkyInfoPrevious = new HPTHorseSulkyInfo()
                {
                    Text = horse.StartInfo.SulkyInfoPrevious?.Text
                };
            }
            else if (horse.StartInfo != null && horse.StartInfo.SulkyInfoPrevious != null)
            {
                hptHorse.SulkyInfoPrevious.Text = horse.StartInfo.SulkyInfoPrevious?.Text;
            }

            if (!string.IsNullOrEmpty(hptHorse.SulkyInfoCurrent?.Text) && !string.IsNullOrEmpty(hptHorse.SulkyInfoPrevious?.Text) && hptHorse.SulkyInfoCurrent.Text != hptHorse.SulkyInfoPrevious.Text)
            {
                hptHorse.SulkyInfoCurrent.SulkyChanged = true;
            }

            // Sätt de värden som sedan kommer att uppdateras ofta
            hptHorse.Merge(horse);

            try
            {
                // Statistik
                hptHorse.CurrentYearStatistics = ConvertHorseYearStatistics(horse.StartInfo.CurrentYearStatistics);
                hptHorse.CurrentYearStatistics.YearString = DateTime.Now.Year.ToString();

                hptHorse.PreviousYearStatistics = ConvertHorseYearStatistics(horse.StartInfo.PreviousYearStatistics);
                hptHorse.PreviousYearStatistics.YearString = DateTime.Now.AddYears(-1).Year.ToString();

                hptHorse.TotalStatistics = ConvertHorseYearStatistics(horse.StartInfo.TotalStatistics);
                hptHorse.TotalStatistics.YearString = "Totalt";

                hptHorse.YearStatisticsList = new List<HPTHorseYearStatistics>() { hptHorse.CurrentYearStatistics, hptHorse.PreviousYearStatistics, hptHorse.TotalStatistics };

                // Records
                hptHorse.RecordList = horse.StartInfo.RecordList.Select(r => new HPTHorseRecord()
                {
                    Date = r.Date.ToString("yyyy-MM-dd"),
                    Distance = r.Distance,
                    Place = r.Place,
                    RaceNr = r.RaceNr,
                    RecordType = r.RecordType,
                    Time = FormatRecordTime(r.Time),
                    TrackCode = r.TrackCode,
                    Winner = r.Winner,
                    TimeWeighed = SetWeighedRecord(r, hptHorse.ParentRace)
                }).ToList();

                // Hitta rekordet
                SetRecord(hptHorse);
            }
            catch (Exception exc)
            {
                // Data isn't sent from service
                string s = exc.Message;
            }

            // Resultat
            var resultList = horse.StartInfo.ResultList.Select(r => new HPTHorseResult()
            {
                Date = r.Date,
                //DateString = r.DateString,
                Distance = r.Distance,
                Driver = r.Driver,
                Earning = r.Earning,
                FirstPrize = r.FirstPrize,
                Odds = r.Odds,
                PlaceString = r.PlaceString,
                HorseName = hptHorse.HorseName,
                Place = SetPlace(r),
                Position = r.Position,
                RaceType = r.RaceType,
                RaceNr = r.RaceNr,
                StartNr = r.StartNr,
                Time = r.Time,
                TrackCode = r.TrackCode,
                Shoeinfo = CreateShoeInfo(r.ShoeInfo),
                TimeWeighed = SetWeighedTime(r),
                HeadToHeadResultList = new List<HPTHorseResult>()
            });
            hptHorse.ResultList = new ObservableCollection<HPTHorseResult>(resultList);

            if (hptHorse.ResultList.Count > 0)
            {
                // Flagga för ny kusk
                hptHorse.DriverChangedSinceLastStart = hptHorse.ResultList[0].Driver != hptHorse.DriverNameShort;

                // Aggregerade värden (senaste 5)
                hptHorse.EarningsMeanLast5 = Convert.ToInt32(hptHorse.ResultList.Select(r => (decimal)r.Earning).Average());
                hptHorse.RecordWeighedLast5 = hptHorse.ResultList.Select(r => (decimal)r.TimeWeighed).Average();
                hptHorse.MeanPlaceLast5 = Convert.ToDecimal(hptHorse.ResultList.Select(r => r.Place).Average());
                hptHorse.ResultRow = hptHorse.ResultList
                    .Select(r => r.PlaceString)
                    .Aggregate((place, next) => place + "-" + next);

                // Aggregerade värden (senaste 3)
                var last3Results = hptHorse.ResultList.OrderByDescending(r => r.Date).Take(3).ToList();
                hptHorse.EarningsMeanLast3 = Convert.ToInt32(last3Results.Select(r => (decimal)r.Earning).Average());
                hptHorse.RecordWeighedLast3 = last3Results.Select(r => (decimal)r.TimeWeighed).Average();
                hptHorse.MeanPlaceLast3 = Convert.ToDecimal(last3Results.Select(r => r.Place).Average());
            }

            // Resultat i loppet om det är klart
            if (horse.VPInfo != null && horse.VPInfo.HorseResultInfo != null)
            {
                hptHorse.HorseResultInfo = new HPTHorseResultInfo()
                {
                    Disqualified = horse.VPInfo.HorseResultInfo.Disqualified,
                    Earning = horse.VPInfo.HorseResultInfo.Earning,
                    FinishingPosition = horse.VPInfo.HorseResultInfo.FinishingPosition,
                    KmTime = horse.VPInfo.HorseResultInfo.KmTime,
                    Place = horse.VPInfo.HorseResultInfo.Place,
                    TotalTime = horse.VPInfo.HorseResultInfo.TotalTime
                };
                hptHorse.HorseResultInfo.SetPlaceString(hptHorse);
            }

            if (HPTConfig.Config.DataToShowVxx.ShowOwnInformation || HPTConfig.Config.DataToShowComplementaryRules.ShowOwnInformation || HPTConfig.Config.DataToShowCorrection.ShowOwnInformation)// || HPTConfig.Config.MarkBetTabsToShow.ShowComments)
            {
                // Own information
                hptHorse.OwnInformation = HPTConfig.Config.HorseOwnInformationCollection.GetOwnInformationByName(hptHorse.HorseName);
                if (hptHorse.OwnInformation != null && hptHorse.OwnInformation.ATGId == "0")
                {
                    hptHorse.OwnInformation.ATGId = hptHorse.ATGId;
                }
            }
        }

        internal static string FormatRecordTime(string timeToFormat)
        {
            if (string.IsNullOrEmpty(timeToFormat))
            {
                return timeToFormat;
            }
            string formattedTime = timeToFormat;
            var rexTime = new Regex(@"\d\.\d\.\d");
            if (rexTime.IsMatch(formattedTime))
            {
                formattedTime = formattedTime.Insert(2, "0");
            }
            return formattedTime;
        }

        internal static void SetRecord(HPTHorse hptHorse)
        {
            if (hptHorse.RecordList != null && hptHorse.RecordList.Count > 0)
            {
                // Snittrekord totalt
                hptHorse.RecordWeighedTotal = hptHorse.RecordList.Select(r => r.TimeWeighed).Average();

                // Hitta rekord
                hptHorse.Record = hptHorse.RecordList
                    .OrderBy(r => r.Time)
                    .FirstOrDefault(r => r.RecordType == hptHorse.ParentRace.StartMethodAndDistanceCode);
            }
            else
            {
                hptHorse.RecordWeighedTotal = 300M;
            }

            if (hptHorse.Record == null)
            {
                hptHorse.Record = new HPTHorseRecord();
                hptHorse.Record.Time = "-";
                hptHorse.RecordTime = 300M;
            }
            else
            {
                Regex rexKmTime = new Regex(@"\d\.(\d{1,2}\.\d)");
                if (rexKmTime.IsMatch(hptHorse.Record.Time))
                {
                    string kmTimeString = rexKmTime.Match(hptHorse.Record.Time).Groups[1].Value;
                    CultureInfo ci = new CultureInfo("en-US");
                    hptHorse.RecordTime = Convert.ToDecimal(kmTimeString, ci.NumberFormat);
                }
                else
                {
                    hptHorse.RecordTime = 30M;
                }
            }
        }

        internal static int SetPlace(HPTService.HPTHorseResult result)
        {
            switch (result.PlaceString)
            {
                case "0":
                    return 7;
                case "k":
                case "d":
                case "p":
                    return 8;
                default:
                    return result.Place;
            }
        }

        internal static HPTHorseShoeInfo CreateShoeInfo(HPTService.HPTHorseShoeInfo shoeInfo)
        {
            var hptShoeInfo = new HPTHorseShoeInfo();
            if (shoeInfo != null)
            {
                hptShoeInfo.Foreshoes = shoeInfo.Foreshoes;
                hptShoeInfo.Hindshoes = shoeInfo.Hindshoes;
            }
            return hptShoeInfo;
        }

        internal static Regex rexTime = new Regex(@"\d{1,2},\d");
        internal static CultureInfo swedishCulture = new CultureInfo("sv-SE");
        private static decimal SetWeighedTime(HPTService.HPTHorseResult horseResult)
        {
            try
            {
                string startMethodAndDistanceCode = horseResult.Time.EndsWith("a") ? "A" : string.Empty;
                if (horseResult.Distance < 1800)
                {
                    startMethodAndDistanceCode += "K";
                }
                else if (horseResult.Distance < 2600)
                {
                    startMethodAndDistanceCode += "L";
                }
                else
                {
                    startMethodAndDistanceCode += "M";
                }

                decimal secondsToAdd = 0M;
                switch (startMethodAndDistanceCode)
                {
                    case "K":
                        secondsToAdd = 2.3M;
                        break;
                    case "M":
                        secondsToAdd = 0.9M;
                        break;
                    case "L":
                        secondsToAdd = -0.1M;
                        break;
                    case "AK":
                        secondsToAdd = 1.0M;
                        break;
                    case "AM":
                        secondsToAdd = 0M;
                        break;
                    case "AL":
                        secondsToAdd = -0.6M;
                        break;
                    default:
                        break;
                }
                if (rexTime.IsMatch(horseResult.Time))
                {
                    decimal time = decimal.Parse(rexTime.Match(horseResult.Time).Value, swedishCulture);
                    return time + secondsToAdd;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return 30M;
        }

        internal static decimal SetWeighedRecord(HPTService.HPTHorseRecord record, HPTRace race)
        {
            try
            {
                decimal secondsToAdd = 0M;
                switch (record.RecordType)
                {
                    case "K":
                        secondsToAdd = 2.3M;
                        break;
                    case "M":
                        secondsToAdd = 0.9M;
                        break;
                    case "L":
                        secondsToAdd = -0.1M;
                        break;
                    case "AK":
                        secondsToAdd = 1.0M;
                        break;
                    case "AM":
                        secondsToAdd = 0M;
                        break;
                    case "AL":
                        secondsToAdd = -0.6M;
                        break;
                    default:
                        break;
                }
                decimal extractedTime = 30M;
                Regex rexExtractTime = new Regex("\\d\\.(\\d\\d\\.\\d)");
                if (rexExtractTime.IsMatch(record.Time))
                {
                    string extractedTimeString = rexExtractTime.Match(record.Time).Groups[1].Value;
                    extractedTimeString = extractedTimeString.Replace('.', ',');
                    extractedTime = Convert.ToDecimal(extractedTimeString);
                }
                return extractedTime + secondsToAdd;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return 30M;
        }

        internal static void SetWeighedTime(HPTHorseResult hptHorseResult)
        {
            try
            {
                string startMethodAndDistanceCode = hptHorseResult.Time.EndsWith("a") ? "A" : string.Empty;
                if (hptHorseResult.Distance < 1800)
                {
                    startMethodAndDistanceCode += "K";
                }
                else if (hptHorseResult.Distance < 2600)
                {
                    startMethodAndDistanceCode += "L";
                }
                else
                {
                    startMethodAndDistanceCode += "M";
                }

                decimal secondsToAdd = 0M;
                switch (startMethodAndDistanceCode)
                {
                    case "K":
                        secondsToAdd = 2.3M;
                        break;
                    case "M":
                        secondsToAdd = 0.9M;
                        break;
                    case "L":
                        secondsToAdd = -0.1M;
                        break;
                    case "AK":
                        secondsToAdd = 1.0M;
                        break;
                    case "AM":
                        secondsToAdd = 0M;
                        break;
                    case "AL":
                        secondsToAdd = -0.6M;
                        break;
                    default:
                        break;
                }
                if (rexTime.IsMatch(hptHorseResult.Time))
                {
                    decimal time = decimal.Parse(rexTime.Match(hptHorseResult.Time).Value, swedishCulture);
                    hptHorseResult.TimeWeighed = time + secondsToAdd;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                hptHorseResult.TimeWeighed = 30M;
            }
        }

        private static int CompareRecords(HPTHorseRecord r1, HPTHorseRecord r2)
        {
            return r1.Distance - r2.Distance;
        }

        public static HPTHorseYearStatistics ConvertHorseYearStatistics(HPTService.HPTHorseYearStatistics yearStatistics)
        {
            HPTHorseYearStatistics hptYearStatistics = new HPTHorseYearStatistics();
            hptYearStatistics.Earning = yearStatistics.Earning;
            if (yearStatistics.NumberOfStarts > 0)
            {
                hptYearStatistics.EarningMean = hptYearStatistics.Earning / yearStatistics.NumberOfStarts;
            }
            hptYearStatistics.FirstPlace = yearStatistics.FirstPlace;
            hptYearStatistics.NumberOfStarts = yearStatistics.NumberOfStarts;
            hptYearStatistics.Percent123 = yearStatistics.Percent123;
            hptYearStatistics.PercentFirstPlace = yearStatistics.PercentFirstPlace;
            hptYearStatistics.SecondPlace = yearStatistics.SecondPlace;
            hptYearStatistics.ThirdPlace = yearStatistics.ThirdPlace;
            return hptYearStatistics;
        }

        #region Trender

        public static void ConvertRaceDayInfoHistory(HPTService.HPTRaceDayInfoHistoryInfoGrouped raceDayInfoHistory, HPTRaceDayInfo hptRaceDayInfo)
        {
            // Skapa lista med de tidsstämplar vi har plockat ut
            hptRaceDayInfo.TimestampListMarkBetHistory = raceDayInfoHistory.SSHTL;
            hptRaceDayInfo.TimestampListVPHistory = raceDayInfoHistory.RaceList.Last().VPHTL;
            hptRaceDayInfo.TurnoverHistoryList = raceDayInfoHistory.TOHL
                .Select(to => new HPTTurnoverHistory()
                {
                    Percentage = to.Percentage,
                    Timestamp = to.Timestamp,
                    Turnover = to.Turnover
                }).ToArray();

            foreach (var raceHistory in raceDayInfoHistory.RaceList)
            {
                var race = hptRaceDayInfo.RaceList.First(r => r.LegNr == raceHistory.LegNr);
                foreach (var horseHistory in raceHistory.HorseList)
                {
                    var horse = race.HorseList.First(h => h.StartNr == horseHistory.StartNr);
                    horse.HorseHistoryInfoGroupedList = Enumerable.Range(1, 10)
                        .Select(i => new HPTHorseHistoryInfoGrouped()
                        {
                            StakeHistoryMain = new HPTStakeHistory()
                        })
                        .ToArray();

                    for (int i = 0; i < horse.HorseHistoryInfoGroupedList.Length; i++)
                    {
                        var horseHistoryInfoGrouped = horse.HorseHistoryInfoGroupedList[i];

                        // Insatsfördelning
                        var stakeHistory = horseHistory.SHL[i];
                        if (stakeHistory != null)
                        {
                            horseHistoryInfoGrouped.StakeHistoryMain.StakeDistribution = stakeHistory.SD;
                            horseHistoryInfoGrouped.StakeHistoryMain.StakeShare = stakeHistory.SS;
                            horseHistoryInfoGrouped.StakeHistoryMain.StakeSharePeriod = stakeHistory.SSP;
                        }

                        var vpHistory = horseHistory.VPHL[i];
                        if (vpHistory != null)
                        {
                            // Vinnarodds
                            horseHistoryInfoGrouped.VinnarOdds = vpHistory.VO;
                            horseHistoryInfoGrouped.VinnarOddsShare = vpHistory.VOS;
                            horseHistoryInfoGrouped.VinnarOddsSharePeriod = vpHistory.VOSP;

                            // Platsodds
                            horseHistoryInfoGrouped.MaxPlatsOdds = vpHistory.MPO;
                            horseHistoryInfoGrouped.PlatsOddsShare = vpHistory.POS;
                            horseHistoryInfoGrouped.PlatsOddsSharePeriod = vpHistory.POSP;
                        }
                    }
                }
            }
        }

        //public static void ConvertRaceDayInfoHistory(HPTService.HPTRaceDayInfo raceDayInfoHistory, HPTRaceDayInfo hptRaceDayInfo)   //, int minutesPerGroup)
        //{                        
        //    // Hur många siffror finns det att tillgå
        //    int totalNumberOfHistoricValues = raceDayInfoHistory.LegList
        //        .First()
        //        .RaceHistoryMarkingBetList
        //        .Count();

        //    // Kolla hur många vi ska plocka ut (max 10)
        //    int arrayInterval = totalNumberOfHistoricValues / 10;
        //    int numberOfHistoricValues = 10;
        //    if (totalNumberOfHistoricValues < 10)
        //    {
        //        numberOfHistoricValues = totalNumberOfHistoricValues;
        //        arrayInterval = 1;
        //    }

        //    // På vilka positioner i arrayen ska vi plocka våra värden
        //    var positionsToSelect = Enumerable.Range(0, numberOfHistoricValues)
        //        .Select(i => i * arrayInterval)
        //        .ToArray();

        //    // Skapa lista med de tidsstämplar vi har plockat ut
        //    hptRaceDayInfo.TimestampListMarkBetHistory = new DateTime[positionsToSelect.Length];

        //    var raceHistoryMarkingBetList = raceDayInfoHistory.LegList
        //        .First()
        //        .RaceHistoryMarkingBetList
        //        .OrderByDescending(rh => rh.Timestamp)
        //        .ToArray();

        //    for (int i = 0; i < positionsToSelect.Length; i++)
        //    {
        //        int position = positionsToSelect[i];
        //        hptRaceDayInfo.TimestampListMarkBetHistory[i] = raceHistoryMarkingBetList[position].Timestamp;
        //    }

        //    // Skapa listor ett lopp i taget
        //    foreach (var race in raceDayInfoHistory.LegList)
        //    {
        //        // Skapa lista i fallande tidssortering
        //        var markingBetListSorted = race
        //            .RaceHistoryMarkingBetList
        //            .OrderByDescending(rh => rh.Timestamp)
        //            .ToArray();

        //        var hptRace = hptRaceDayInfo.RaceList.FirstOrDefault(r => r.LegNr == race.LegNr);
        //        if (hptRace != null)
        //        {
        //            // Skapa array för att få plats med rätt antal värden
        //            foreach (var horse in hptRace.HorseList)
        //            {
        //                horse.HorseHistoryInfoGroupedList = new HPTHorseHistoryInfoGrouped[numberOfHistoricValues];
        //            }

        //            for (int i = 0; i < positionsToSelect.Length; i++)
        //            {
        //                int position = positionsToSelect[i];
        //                var markingBetHistoryRace = markingBetListSorted[position];
        //                int stakeSum = markingBetHistoryRace.HorseHistoryList.Sum(hh => hh.StakeDistribution);
        //                decimal stakeShareSum = markingBetHistoryRace.HorseHistoryList.Sum(hh => hh.StakeShare);    // Verkar bli större än 1.0
        //                int previousStakeSum = 0;
        //                HPTService.HPTRaceHistoryMarkingBet previousMarkingBetHistoryRace = null;
        //                if (i < positionsToSelect.Length - 1)
        //                {
        //                    int previousPosition = positionsToSelect[i + 1];
        //                    previousMarkingBetHistoryRace = markingBetListSorted[previousPosition];
        //                    previousStakeSum = previousMarkingBetHistoryRace.HorseHistoryList.Sum(hh => hh.StakeDistribution);
        //                }
        //                int periodStakeSum = stakeSum - previousStakeSum;
        //                // Uppdatera varje häst
        //                foreach (var horse in hptRace.HorseList)
        //                {
        //                    var horseHistory = markingBetHistoryRace.HorseHistoryList.FirstOrDefault(hh => hh.StartNr == horse.StartNr);
        //                    if (horseHistory != null)
        //                    {
        //                        int previousStakeDistribution = 0;
        //                        horseHistory.StakeShare /=  stakeShareSum;
        //                        //var foreColor = new System.Windows.Media.SolidColorBrush();
        //                        var fontWeight = System.Windows.FontWeights.Normal;
        //                        var backColor = new System.Windows.Media.SolidColorBrush();

        //                        // Om det finns äldre historieobjekt
        //                        if (previousMarkingBetHistoryRace != null)
        //                        {
        //                            var previousHorseHistory = previousMarkingBetHistoryRace.HorseHistoryList.FirstOrDefault(hh => hh.StartNr == horse.StartNr);
        //                            previousStakeDistribution = previousHorseHistory.StakeDistribution;

        //                            // Kontrollera om det är stora ändringar i spelandet, men bara om det är hästar med hyfsat stor insatsfördelning
        //                            decimal absoluteChange = Math.Abs(horseHistory.StakeShare - previousHorseHistory.StakeShare);
        //                            if (absoluteChange > 0.05M && previousHorseHistory.StakeShare > 0M)
        //                            {
        //                                decimal relativeChange = horseHistory.StakeShare / previousHorseHistory.StakeShare;
        //                                if (relativeChange > 1.01M)
        //                                {
        //                                    fontWeight = System.Windows.FontWeights.Bold;
        //                                    //foreColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGreen);
        //                                }
        //                                else if (relativeChange < 0.99M)
        //                                {
        //                                    fontWeight = System.Windows.FontWeights.Light;
        //                                    //foreColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkRed);
        //                                }
        //                            }
        //                        }

        //                        int periodStakeDistribution = horseHistory.StakeDistribution - previousStakeDistribution;
        //                        decimal stakeSharePeriod = 0M;
        //                        if (periodStakeSum > 0)
        //                        {
        //                            stakeSharePeriod = Convert.ToDecimal(periodStakeDistribution) / Convert.ToDecimal(periodStakeSum);

        //                            // Kontrollera om det är stora ändringar i spelandet, men bara om det är hästar med hyfsat stor insatsfördelning
        //                            decimal periodStakeShareAbsoluteDifference = Math.Abs(stakeSharePeriod - horseHistory.StakeShare);
        //                            if (periodStakeShareAbsoluteDifference > 0.02M)
        //                            {
        //                                decimal periodStakeShareInRelationToTotal = stakeSharePeriod / horseHistory.StakeShare;
        //                                if (periodStakeShareInRelationToTotal > 1.08M)
        //                                {
        //                                    backColor = new System.Windows.Media.SolidColorBrush(HPTConfig.Config.ColorGood);
        //                                }
        //                                else if (periodStakeShareInRelationToTotal < 0.92M)
        //                                {
        //                                    backColor = new System.Windows.Media.SolidColorBrush(HPTConfig.Config.ColorBad);
        //                                }
        //                            }
        //                        }

        //                        horse.HorseHistoryInfoGroupedList[i] = new HPTHorseHistoryInfoGrouped()
        //                        {
        //                            StakeHistoryMain = new HPTStakeHistory()
        //                            {
        //                                StakeDistribution = horseHistory.StakeDistribution,
        //                                StakeShare = horseHistory.StakeShare,
        //                                StakeSharePeriod = stakeSharePeriod,
        //                                BackColor = backColor,
        //                                FontWeight = fontWeight
        //                            }
        //                        };
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void SetPeriodStakeShare()
        //{
        //}

        //public static List<HPTHorseHistoryInfoGrouped> CreateHorseHistoryInfoGrouped(HPTRaceDayInfo hptRaceDayInfo, HPTService.HPTHorseHistoryInfo horseHistoryInfo, DateTime newestTime, double intervalInMinutes)
        //{
        //    HPTService.HPTHorseHistoryMarkBet[] horseHistoryMainList = null;
        //    HPTService.HPTHorseHistoryMarkBet[] horseHistoryAlternateList = null;
        //    HPTService.HPTHorseHistoryMarkBet[] horseHistoryAlternate2List = null;

        //    switch (hptRaceDayInfo.BetType.TypeCategory)
        //    {
        //        case BetTypeCategory.None:
        //            return null;
        //        case BetTypeCategory.V4:
        //            horseHistoryMainList = horseHistoryInfo.HorseHistoryV4List;
        //            horseHistoryAlternateList = horseHistoryInfo.HorseHistoryVxxList;
        //            horseHistoryAlternate2List = horseHistoryInfo.HorseHistoryV5List;
        //            break;
        //        case BetTypeCategory.V5:
        //            horseHistoryMainList = horseHistoryInfo.HorseHistoryV5List;
        //            horseHistoryAlternateList = horseHistoryInfo.HorseHistoryVxxList;
        //            horseHistoryAlternate2List = horseHistoryInfo.HorseHistoryV4List;
        //            break;
        //        case BetTypeCategory.V6X:
        //        case BetTypeCategory.V75:
        //        case BetTypeCategory.V86:
        //            horseHistoryMainList = horseHistoryInfo.HorseHistoryVxxList;
        //            horseHistoryAlternateList = horseHistoryInfo.HorseHistoryV4List;
        //            horseHistoryAlternate2List = horseHistoryInfo.HorseHistoryV5List;
        //            break;
        //        case BetTypeCategory.Double:
        //            break;
        //        case BetTypeCategory.Trio:
        //            break;
        //        case BetTypeCategory.Twin:
        //            break;
        //        default:
        //            break;
        //    }

        //    DateTime nextTime = newestTime;

        //    var hptHorseHistoryInfoGroupedList = new List<HPTHorseHistoryInfoGrouped>();

        //    for (int i = 0; i < 20; i++)
        //    {
        //        var hptHorseHistoryInfoGrouped = new HPTHorseHistoryInfoGrouped()
        //        {
        //            Timestamp = nextTime,
        //            StakeHistoryMain = CreateStakeHistory(horseHistoryMainList, nextTime),
        //            StakeHistoryAlternate = CreateStakeHistory(horseHistoryAlternateList, nextTime),
        //            StakeHistoryAlternate2 = CreateStakeHistory(horseHistoryAlternate2List, nextTime)
        //        };
        //        SetVPHistory(hptHorseHistoryInfoGrouped, horseHistoryInfo.HorseHistoryVPList, nextTime);
        //        hptHorseHistoryInfoGroupedList.Add(hptHorseHistoryInfoGrouped);
        //        nextTime = nextTime.AddMinutes(intervalInMinutes);
        //    }

        //    //if (horseHistoryInfo.HorseHistoryDoubleList != null)
        //    //{
        //    //    var horseHistoryDoubleList = horseHistoryInfo.HorseHistoryDoubleList.Where(hhd => hhd.Timestamp <= newestTime && hhd.Timestamp > oldestTime);
        //    //    if (horseHistoryDoubleList.Any())
        //    //    {
        //    //        hptHorseHistoryInfoGrouped.ShareDouble = horseHistoryDoubleList.Average(hh => hh.Share);
        //    //    }
        //    //}

        //    //if (horseHistoryInfo.HorseHistoryVPList != null)
        //    //{
        //    //    var horseHistoryVPList = horseHistoryInfo.HorseHistoryVPList.Where(hhd => hhd.Timestamp <= newestTime && hhd.Timestamp > oldestTime);
        //    //    if (horseHistoryVPList.Any())
        //    //    {
        //    //        hptHorseHistoryInfoGrouped.VinnarOdds = Convert.ToInt32(horseHistoryVPList.Average(hh => hh.VinnarOdds));
        //    //    }
        //    //}

        //    //return hptHorseHistoryInfoGrouped;

        //    return hptHorseHistoryInfoGroupedList;
        //}

        //public static HPTStakeHistory CreateStakeHistory(HPTService.HPTHorseHistoryMarkBet[] horseHistoryList, DateTime newestTime)
        //{
        //    if (horseHistoryList != null)
        //    {
        //        var horseHistoryMarkBet = horseHistoryList
        //            .OrderByDescending(hhd => hhd.Timestamp)
        //            .FirstOrDefault(hhd => hhd.Timestamp <= newestTime);

        //        if (horseHistoryMarkBet != null)
        //        {
        //            var stakeHistory = new HPTStakeHistory()
        //            {
        //                StakeDistribution = horseHistoryMarkBet.StakeDistribution,
        //                StakeShare = horseHistoryMarkBet.StakeShare,
        //                StakeSharePeriod = 0M
        //            };
        //            return stakeHistory;
        //        }
        //    }
        //    return null;
        //}

        //public static bool SetVPHistory(HPTHorseHistoryInfoGrouped horseHistoryInfoGrouped, HPTService.HPTHorseHistoryVP[] horseHistoryList, DateTime newestTime)
        //{
        //    if (horseHistoryList != null)
        //    {
        //        var horseHistoryVP = horseHistoryList
        //            .OrderByDescending(hhd => hhd.Timestamp)
        //            .FirstOrDefault(hhd => hhd.Timestamp <= newestTime);

        //        if (horseHistoryVP != null)
        //        {
        //            horseHistoryInfoGrouped.InvestmentPlats = horseHistoryVP.InvestmentPlats;
        //            horseHistoryInfoGrouped.InvestmentVinnare = horseHistoryVP.InvestmentVinnare;
        //            horseHistoryInfoGrouped.MaxPlatsOdds = horseHistoryVP.MaxPlatsOdds;
        //            horseHistoryInfoGrouped.VinnarOdds = horseHistoryVP.VinnarOdds;
        //            return true;                                        
        //        }
        //    }
        //    return false;
        //}

        //public static int GetStakeDistributionForPeriod(IOrderedEnumerable<HPTService.HPTHorseHistoryMarkBet> horseHistoryMarkBetOrdered, DateTime newestTime, DateTime oldestTime)
        //{
        //    var previousHorseHistoryMarkBet = horseHistoryMarkBetOrdered.LastOrDefault(hhmb => hhmb.Timestamp < oldestTime);
        //    var currentHorseHistoryMarkBet = horseHistoryMarkBetOrdered.LastOrDefault(hhmb => hhmb.Timestamp < newestTime);

        //    if (currentHorseHistoryMarkBet != null)
        //    {
        //        if (previousHorseHistoryMarkBet != null)
        //        {
        //            return currentHorseHistoryMarkBet.StakeDistribution - previousHorseHistoryMarkBet.StakeDistribution;
        //        }
        //        return currentHorseHistoryMarkBet.StakeDistribution;
        //    }
        //    return 0;
        //}

        //public static decimal GetAverageStakeShareForPeriod(IOrderedEnumerable<HPTService.HPTHorseHistoryMarkBet> horseHistoryMarkBetOrdered, DateTime newestTime, DateTime oldestTime)
        //{
        //    var horseHistoryMarkBetList = horseHistoryMarkBetOrdered.Where(hhd => hhd.Timestamp <= newestTime && hhd.Timestamp > oldestTime);
        //    if (horseHistoryMarkBetList.Any())
        //    {
        //        return horseHistoryMarkBetList.Average(hhmb => hhmb.StakeShare);
        //    }
        //    return 0M;
        //}

        #endregion

        public static void ConvertResultMarkingBet(HPTService.HPTResultMarkingBet resultMarkingBet, HPTRaceDayInfo hptRaceDayInfo, bool setValues)
        {
            try
            {
                if (!setValues)
                {
                    hptRaceDayInfo.ResultMarkingBet = resultMarkingBet;
                }
                hptRaceDayInfo.HasResult = resultMarkingBet.HasResult;

                if (resultMarkingBet.ResultComplete && resultMarkingBet.PayOutList != null && resultMarkingBet.PayOutList.Length > 0)
                {
                    hptRaceDayInfo.ResultComplete = resultMarkingBet.ResultComplete;
                }
                hptRaceDayInfo.NumberOfFinishedRaces = resultMarkingBet.LegResultList.Length;

                foreach (HPTService.HPTLegResult legResult in resultMarkingBet.LegResultList)
                {
                    HPTRace hptRace = hptRaceDayInfo.RaceList.First(r => r.LegNr == legResult.LegNr);
                    hptRace.LegResult = new HPTLegResult()
                    {
                        LegNr = legResult.LegNr,
                        SystemsLeft = setValues ? legResult.SystemsLeft : 0,
                        Value = setValues ? legResult.Value : 0,
                        HasResult = true,
                        Winners = legResult.Winners.ToArray()
                    };

                    hptRace.HasResult = true;
                }

                if (resultMarkingBet.PayOutList != null && setValues)
                {
                    hptRaceDayInfo.PayOutList = new ObservableCollection<HPTPayOut>();
                    foreach (HPTService.HPTPayOut payOut in resultMarkingBet.PayOutList)
                    {
                        HPTPayOut hptPayOut = new HPTPayOut();
                        hptPayOut.NumberOfCorrect = payOut.NumberOfCorrect;
                        hptPayOut.NumberOfSystems = payOut.NumberOfSystems;
                        hptPayOut.PayOutAmount = payOut.PayOutAmount;
                        hptPayOut.TotalAmount = payOut.TotalAmount;
                        hptRaceDayInfo.PayOutList.Add(hptPayOut);
                    }
                }
                else
                {
                    hptRaceDayInfo.PayOutList = new System.Collections.ObjectModel.ObservableCollection<HPTPayOut>();
                }
                hptRaceDayInfo.WinnerList = resultMarkingBet.WinnerList;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public static void SetTrainerAndDriver(HPTMarkBet markBet)
        {
            #region Driver

            markBet.DriverRulesCollection.PersonList = new ObservableCollection<HPTPerson>();

            HPTRaceDayInfo driverRdi = new HPTRaceDayInfo()
            {
                DataToShow = HPTConfig.Config.DataToShowDriverPopup
            };

            IEnumerable<HPTHorse> allHorses = markBet.RaceDayInfo.RaceList
                .SelectMany(r => r.HorseList);

            IEnumerable<string> allDrivers = allHorses
                .Select(h => h.DriverNameShort)
                .Distinct();

            foreach (var driverNameShort in allDrivers)
            {
                IEnumerable<HPTHorse> driverHorseList = allHorses.Where(h => h.DriverNameShort == driverNameShort);
                HPTHorse firstHorse = driverHorseList.First();
                HPTPerson driver = new HPTPerson()
                {
                    Name = firstHorse.DriverName,
                    ParentRaceDayInfo = driverRdi,
                    ShortName = firstHorse.DriverNameShort,
                    HorseList = new List<HPTHorse>(driverHorseList)
                    //HorseList = new ObservableCollection<HPTHorse>(driverHorseList)
                };
                foreach (var driverHorse in driverHorseList)
                {
                    driverHorse.Driver = driver;
                }
                markBet.DriverRulesCollection.PersonList.Add(driver);
            }

            #endregion

            #region Trainer

            HPTRaceDayInfo trainerRdi = new HPTRaceDayInfo()
            {
                DataToShow = HPTConfig.Config.DataToShowTrainerPopup
            };

            IEnumerable<string> allTrainers = allHorses
                .Select(h => h.TrainerNameShort)
                .Distinct();

            markBet.TrainerRulesCollection.PersonList = new ObservableCollection<HPTPerson>();

            foreach (var trainerNameShort in allTrainers)
            {
                IEnumerable<HPTHorse> trainerHorseList = allHorses.Where(h => h.TrainerNameShort == trainerNameShort);
                HPTHorse firstHorse = trainerHorseList.First();
                HPTPerson trainer = new HPTPerson()
                {
                    Name = firstHorse.TrainerName,
                    ParentRaceDayInfo = trainerRdi,
                    ShortName = firstHorse.TrainerNameShort,
                    HorseList = new List<HPTHorse>(trainerHorseList)
                    //HorseList = new ObservableCollection<HPTHorse>(trainerHorseList)
                };
                foreach (var trainerHorse in trainerHorseList)
                {
                    trainerHorse.Trainer = trainer;
                }
                markBet.TrainerRulesCollection.PersonList.Add(trainer);
            }

            #endregion
        }

        public static void SetDistanceToHomeTrack(HPTRaceDayInfo raceDayInfo)
        {
            try
            {
                HPTTrackDistance trackDistance = HPTTrackDistance.TrackDistanceArray.FirstOrDefault(td => (int)td.TrackName == (int)raceDayInfo.TrackId);
                if (trackDistance == null)
                {
                    foreach (var race in raceDayInfo.RaceList)
                    {
                        SetDistanceToHomeTrack(race);
                    }
                }
                else
                {
                    IEnumerable<HPTHorse> allHorses = raceDayInfo.RaceList.SelectMany(r => r.HorseList);
                    foreach (var horse in allHorses)
                    {
                        horse.HomeTrackInfo = horse.HomeTrack;
                        if (trackDistance != null)
                        {
                            horse.DistanceFromHomeTrack = trackDistance.GetDistance(EnumHelper.GetTrackNameFromShortString(horse.HomeTrack));
                            horse.HomeTrackInfo += (horse.DistanceFromHomeTrack == 0 ? string.Empty : " (" + horse.DistanceFromHomeTrack.ToString() + " km)");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public static void SetDistanceToHomeTrack(HPTRace race)
        {
            if (race.TrackId == null)
            {
                foreach (var horse in race.HorseList)
                {
                    horse.HomeTrackInfo = horse.HomeTrack;
                }
                return;
            }
            HPTTrackDistance trackDistance = HPTTrackDistance.TrackDistanceArray.FirstOrDefault(td => (int)td.TrackName == (int)race.TrackId);
            if (trackDistance != null)
            {
                foreach (var horse in race.HorseList)
                {
                    horse.HomeTrackInfo = horse.HomeTrack;
                    if (trackDistance != null)
                    {
                        horse.DistanceFromHomeTrack = trackDistance.GetDistance(EnumHelper.GetTrackNameFromShortString(horse.HomeTrack));
                        horse.HomeTrackInfo += (horse.DistanceFromHomeTrack == 0 ? string.Empty : " (" + horse.DistanceFromHomeTrack.ToString() + " km)");
                    }
                }
            }
        }

        private int ComparePerson(HPTPerson p1, HPTPerson p2)
        {
            return string.Compare(p1.ShortName, p2.ShortName);
        }

        public static void SetNonSerializedValues(HPTCombBet hcb)
        {
            // Undik onödig uppläsning av egen hästinformation
            bool horseOwnInformationShows = false;
            switch (hcb.BetType.Code)
            {
                case "DD":
                case "LD":
                    horseOwnInformationShows = HPTConfig.Config.DataToShowDD.ShowOwnInformation;
                    break;
                case "TV":
                    horseOwnInformationShows = HPTConfig.Config.DataToShowTvilling.ShowOwnInformation;
                    break;
                case "T":
                    horseOwnInformationShows = HPTConfig.Config.DataToShowTrio.ShowOwnInformation;
                    break;
                default:
                    break;
            }

            foreach (HPTRace hptRace in hcb.RaceDayInfo.RaceList)
            {
                hptRace.ParentRaceDayInfo = hcb.RaceDayInfo;

                // Sätt visningstext för loppet
                switch (hcb.BetType.Code)
                {
                    case "TV":
                        hptRace.LegNrString = "Lopp " + hptRace.LegNr.ToString();
                        break;
                    case "T":
                        hptRace.LegNrString = "Trio" + "-" + hptRace.LegNr.ToString();
                        break;
                    default:
                        hptRace.LegNrString = hcb.BetType.Code + "-" + hptRace.LegNr.ToString();
                        break;
                }

                foreach (HPTHorse hptHorse in hptRace.HorseList)
                {
                    // Sätt parent
                    hptHorse.ParentRace = hptRace;
                    hptHorse.CalculateDerivedValues();

                    // Skoinfo
                    if (hptHorse.ShoeInfoCurrent != null && hptHorse.ShoeInfoPrevious != null)
                    {
                        hptHorse.ShoeInfoCurrent.SetChangedFlags(hptHorse.ShoeInfoPrevious);
                    }

                    // Skoinfo på resultat
                    if (hptHorse.ResultList != null && hptHorse.ResultList.Count > 0)
                    {
                        foreach (var result in hptHorse.ResultList)
                        {
                            if (result.Shoeinfo != null)
                            {
                                result.Shoeinfo.PreviousUsed = false;
                                result.Shoeinfo.Foreshoes = result.Shoeinfo.Foreshoes;
                                result.Shoeinfo.Hindshoes = result.Shoeinfo.Hindshoes;
                            }
                        }
                    }

                    // Own information
                    if (horseOwnInformationShows)
                    {
                        HPTConfig.Config.HorseOwnInformationCollection.MergeHorseOwnInformation(hptHorse);
                    }
                }
                if (hcb.BetType.Code == "TV")   // Tvilling
                {
                    foreach (HPTCombination hptComb in hptRace.CombinationListInfoTvilling.CombinationList)
                    {
                        hptComb.ParentRace = hptRace;
                        hptComb.ParentRaceDayInfo = hcb.RaceDayInfo;
                        hptComb.Horse1 = hptRace.GetHorseByNumber(hptComb.Horse1Nr);
                        hptComb.Horse2 = hptRace.GetHorseByNumber(hptComb.Horse2Nr);
                    }
                    hptRace.CombinationListInfoTvilling.UpdateCombinationsToShow();
                }
                if (hcb.BetType.Code == "T")    // Trio
                {
                    foreach (HPTCombination hptComb in hptRace.CombinationListInfoTrio.CombinationList)
                    {
                        hptComb.ParentRace = hptRace;
                        hptComb.ParentRaceDayInfo = hcb.RaceDayInfo;
                        hptComb.Horse1 = hptRace.GetHorseByNumber(hptComb.Horse1Nr);
                        hptComb.Horse2 = hptRace.GetHorseByNumber(hptComb.Horse2Nr);
                        hptComb.Horse3 = hptRace.GetHorseByNumber(hptComb.Horse3Nr);
                    }
                    hptRace.CombinationListInfoTrio.UpdateCombinationsToShow();
                }
            }

            if (hcb.BetType.Code == "DD" || hcb.BetType.Code == "LD")
            {
                foreach (HPTCombination comb in hcb.RaceDayInfo.CombinationListInfoDouble.CombinationList)
                {
                    comb.ParentRaceDayInfo = hcb.RaceDayInfo;
                    comb.Horse1 = hcb.RaceDayInfo.RaceList[0].GetHorseByNumber(comb.Horse1Nr);
                    comb.Horse2 = hcb.RaceDayInfo.RaceList[1].GetHorseByNumber(comb.Horse2Nr);
                    string uniqueCode = comb.Horse1.HexCode + comb.Horse2.HexCode;
                }
                hcb.RaceDayInfo.CombinationListInfoDouble.UpdateCombinationsToShow();
            }

            try
            {
                if (!Directory.Exists(hcb.SaveDirectory))
                {
                    hcb.SaveDirectory = HPTConfig.MyDocumentsPath + hcb.RaceDayInfo.ToDateAndTrackString() + "\\";
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            // Sätt avstån till hemmabanan för alla hästar
            SetDistanceToHomeTrack(hcb.RaceDayInfo);
        }

        public static void SetNonSerializedValues(HPTMarkBet hmb)
        {
            // Skapa variabler som måste finnas
            if (hmb.HorseVariableList == null)
            {
                hmb.HorseVariableList = new ObservableCollection<HPTHorseVariable>(HPTHorseVariable.CreateVariableList());
            }
            hmb.RaceDayInfo.HorseListSelected = new ObservableCollection<HPTHorse>();

            // Create IntervalReductionRuleList
            if (hmb.IntervalReductionRuleList == null)
            {
                hmb.IntervalReductionRuleList = new ObservableCollection<HPTIntervalReductionRule>()
                {
                    //hmb.PercentSumReductionRule,
                    hmb.RowValueReductionRule,
                    hmb.StakePercentSumReductionRule,
                    hmb.StartNrSumReductionRule,
                    hmb.ATGRankSumReductionRule,
                    hmb.OwnRankSumReductionRule,
                    hmb.AlternateRankSumReductionRule,
                    hmb.OddsSumReductionRule
                };
            }

            //// Undvika specialkomprimering
            //if (hmb.CouponCompression == CouponCompression.V6BetMultiplier)
            //{
            //    hmb.CouponCompression = CouponCompression.Default;
            //}

            // Undvik onödig uppläsning av HorseOwnInformation
            bool horseOwnInformationShows = HPTConfig.Config.DataToShowVxx.ShowOwnInformation || HPTConfig.Config.DataToShowComplementaryRules.ShowOwnInformation || HPTConfig.Config.DataToShowCorrection.ShowOwnInformation || HPTConfig.Config.MarkBetTabsToShow.ShowComments;

            // Ta hänsyn till V6/V7/V8
            hmb.RaceDayInfo.SetV6Factor();

            foreach (HPTRace hptRace in hmb.RaceDayInfo.RaceList)
            {
                hptRace.ParentRaceDayInfo = hmb.RaceDayInfo;
                hptRace.HorseListSelected = new List<HPTHorse>();

                foreach (HPTHorse hptHorse in hptRace.HorseList)
                {
                    // Skapa listor som måste finnas

                    hptHorse.HorseXReductionList = new ObservableCollection<HPTHorseXReduction>();
                    hptHorse.RankList = new ObservableCollection<HPTHorseRank>();

                    // Sätt parent
                    hptHorse.ParentRace = hptRace;

                    // Beräknade värden
                    hptHorse.CalculateDerivedValues();
                    hptHorse.CalculateStaticDerivedValues();

                    // Skapa ABCDEF-regler
                    hptHorse.CreateXReductionRuleList();
                    if (hptHorse.Prio != HPTPrio.M)
                    {
                        hptHorse.HorseXReductionList.First(h => h.Prio == hptHorse.Prio).Selected = true;
                    }
                    if (hptHorse.Selected)
                    {
                        hmb.RaceDayInfo.HorseListSelected.Add(hptHorse);
                    }

                    // Skoinfo
                    if (hptHorse.ShoeInfoCurrent != null && hptHorse.ShoeInfoPrevious != null)
                    {
                        hptHorse.ShoeInfoCurrent.SetChangedFlags(hptHorse.ShoeInfoPrevious);
                    }

                    // Own information
                    if (horseOwnInformationShows)
                    {
                        HPTConfig.Config.HorseOwnInformationCollection.MergeHorseOwnInformation(hptHorse);
                    }
                }

                // Sätt justerad insatsfördelning
                hptRace.SetCorrectStakeDistributionShare();
                hptRace.SetCorrectStakeDistributionShareAlt1();
                hptRace.SetCorrectStakeDistributionShareAlt2();

                // Inbördes möten
                hptRace.FindHeadToHead();

                // Sätt visningstext för loppet
                switch (hmb.BetType.Code)
                {
                    case "TV":
                        hptRace.LegNrString = "Lopp " + hptRace.LegNr.ToString();
                        break;
                    case "T":
                        hptRace.LegNrString = "Trio" + "-" + hptRace.LegNr.ToString();
                        break;
                    default:
                        hptRace.LegNrString = hmb.BetType.Code + "-" + hptRace.LegNr.ToString();
                        break;
                }

                // Grupperingsnamn för reserver i GUIt
                hptRace.Reserv1GroupName = hptRace.LegNr.ToString() + "-" + "1";
                hptRace.Reserv2GroupName = hptRace.LegNr.ToString() + "-" + "2";
            }
            SetTrainerAndDriver(hmb);

            // ABCDEF-regel
            var aRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.A);
            aRule.Use = !aRule.Use ? HPTConfig.Config.UseA : true;
            var bRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.B);
            bRule.Use = !bRule.Use ? HPTConfig.Config.UseB : true;
            var cRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.C);
            cRule.Use = !cRule.Use ? HPTConfig.Config.UseC : true;
            var dRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.D);
            dRule.Use = !dRule.Use ? HPTConfig.Config.UseD : true;
            var eRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.E);
            eRule.Use = !eRule.Use ? HPTConfig.Config.UseE : true;
            var fRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.F);
            fRule.Use = !fRule.Use ? HPTConfig.Config.UseF : true;

            // Drivers
            foreach (HPTPersonReductionRule rule in hmb.DriverRulesCollection.ReductionRuleList)
            {
                rule.PersonList = new ObservableCollection<HPTPerson>();
                foreach (string personName in rule.PersonShortNameList)
                {
                    var person = hmb.DriverRulesCollection.PersonList.FirstOrDefault(p => p.ShortName.ToLower() == personName.ToLower());
                    if (person != null && !rule.PersonList.Contains(person))
                    {
                        rule.PersonList.Add(person);
                    }
                }
            }
            hmb.DriverRulesCollection.ReductionRuleFactory = hmb.DriverRulesCollection.CreateNewDriverReductionRule;
            hmb.DriverRulesCollection.Initialize();

            // Trainers
            foreach (HPTPersonReductionRule rule in hmb.TrainerRulesCollection.ReductionRuleList)
            {
                rule.PersonList = new ObservableCollection<HPTPerson>();
                foreach (string personName in rule.PersonShortNameList)
                {
                    var person = hmb.TrainerRulesCollection.PersonList.FirstOrDefault(p => p.ShortName == personName);
                    if (person != null && !rule.PersonList.Contains(person))
                    {
                        rule.PersonList.Add(person);
                    }
                }
            }
            hmb.TrainerRulesCollection.ReductionRuleFactory = hmb.TrainerRulesCollection.CreateNewTrainerReductionRule;
            hmb.TrainerRulesCollection.Initialize();

            // Complementaryreduction rules
            if (hmb.ComplementaryRulesCollection.ReductionRuleList != null)
            {
                foreach (HPTComplementaryReductionRule rule in hmb.ComplementaryRulesCollection.ReductionRuleList)
                {
                    //rule.HorseList = new ObservableCollection<HPTHorse>();
                    rule.HorseList = new ObservableCollection<HPTHorse>();
                    foreach (var horseLight in rule.HorseLightList)
                    {
                        HPTHorse horse = hmb.RaceDayInfo.HorseListSelected
                            .FirstOrDefault(h => h.ParentRace.LegNr == horseLight.LegNr && h.StartNr == horseLight.StartNr);

                        if (horse != null)
                        {
                            rule.HorseList.Add(horse);
                        }
                    }
                }
                hmb.ComplementaryRulesCollection.Initialize();
            }

            // V6BetMultiplierRule
            int ruleNumber = 1;
            foreach (var v6BetMultiplierRule in hmb.V6BetMultiplierRuleList)
            {
                //v6BetMultiplierRule.HorseList = new ObservableCollection<HPTHorse>();
                v6BetMultiplierRule.HorseList = new List<HPTHorse>();
                v6BetMultiplierRule.BetMultiplierList = hmb.BetType.BetMultiplierList;
                v6BetMultiplierRule.MarkBet = hmb;

                List<HPTHorseLightSelectable> horseLightSelectableList = v6BetMultiplierRule.RaceList
                    .SelectMany(r => r.HorseList).ToList();

                foreach (var horseLightSelectable in horseLightSelectableList)
                {
                    HPTHorse horse = hmb.RaceDayInfo.RaceList.SelectMany(r => r.HorseList)
                        .FirstOrDefault(h => h.StartNr == horseLightSelectable.StartNr && h.ParentRace.LegNr == horseLightSelectable.LegNr);

                    horseLightSelectable.Horse = horse;
                    if (horseLightSelectable.Selected)
                    {
                        v6BetMultiplierRule.HorseList.Add(horse);
                        v6BetMultiplierRule.RaceList.First(r => r.LegNr == horseLightSelectable.LegNr).SelectedHorse =
                            horseLightSelectable;

                        horseLightSelectable.GroupCode = ruleNumber.ToString() + horse.ParentRace.LegNr.ToString();
                    }
                }
                ruleNumber++;
            }

            // GroupReductionRules
            try
            {
                foreach (HPTGroupIntervalReductionRule rule in hmb.GroupIntervalRulesCollection.ReductionRuleList)
                {
                    rule.HorseVariable = HPTConfig.Config.HorseVariableList.FirstOrDefault(hv => hv.PropertyName == rule.PropertyName);
                }
                hmb.GroupIntervalRulesCollection.Initialize();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            hmb.SingleRowCollection = new HPTMarkBetSingleRowCollection(hmb);
            hmb.SingleRowCollection.AnalyzingFinished += hmb.SingleRowCollection_AnalyzingFinished;

            // Ranksummereduceringsregler
            if (hmb.HorseRankVariableList == null || hmb.HorseRankVariableList.Count == 0)
            {
                hmb.HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
            }
            if (hmb.HorseRankSumReductionRuleList != null)
            {
                var horseRankSumReductionRulesToRemove = new List<HPTHorseRankSumReductionRule>();
                foreach (var horseRankSumReductionRule in hmb.HorseRankSumReductionRuleList)
                {
                    horseRankSumReductionRule.HorseRankVariable = hmb.HorseRankVariableList.FirstOrDefault(hrv => hrv.PropertyName == horseRankSumReductionRule.PropertyName);
                    if (horseRankSumReductionRule != null)
                    {
                        if (horseRankSumReductionRule.HorseRankVariable == null)
                        {
                            horseRankSumReductionRulesToRemove.Add(horseRankSumReductionRule);
                        }
                        else
                        {
                            foreach (var rule in horseRankSumReductionRule.ReductionRuleList)
                            {
                                rule.ParentHorseRankSumReductionRule = horseRankSumReductionRule;
                            }
                        }
                    }
                }
                foreach (var horseRankSumReductionRuleToRemove in horseRankSumReductionRulesToRemove)
                {
                    hmb.HorseRankSumReductionRuleList.Remove(horseRankSumReductionRuleToRemove);
                }
            }

            // Ta bort rankvariabel för streckprocent
            var rankVariableToRemove = hmb.HorseRankVariableList.FirstOrDefault(hrv => string.IsNullOrEmpty(hrv.PropertyName) || hrv.PropertyName == "MarksQuantity");
            if (rankVariableToRemove != null)
            {
                hmb.HorseRankVariableList.Remove(rankVariableToRemove);
            }

            // Ta hand om låsta kuponger
            if (hmb.LockCoupons && hmb.CouponList != null)
            {
                try
                {
                    hmb.CouponCorrector = new HPTCouponCorrector()
                    {
                        RaceDayInfo = hmb.RaceDayInfo,
                        CouponHelper = new ATGCouponHelper(hmb)
                        {
                            CouponList = hmb.CouponList
                        }
                    };
                    hmb.CouponCorrector.CouponHelper.CreateHorseListsForCoupons();
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
            }

            try
            {
                if (!Directory.Exists(hmb.SaveDirectory))
                {
                    hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }

            // Sätt avstån till hemmabanan för alla hästar
            SetDistanceToHomeTrack(hmb.RaceDayInfo);

            // IHorseListContainer
            //hmb.HorseList = new ObservableCollection<HPTHorse>(hmb.RaceDayInfo.RaceList.SelectMany(r => r.HorseList));
            hmb.HorseList = new List<HPTHorse>(hmb.RaceDayInfo.RaceList.SelectMany(r => r.HorseList));
            hmb.ParentRaceDayInfo = hmb.RaceDayInfo;
        }

        public static void SetNonSerializedValuesParallell(HPTMarkBet hmb)
        {
            // Skapa variabler som måste finnas
            if (hmb.HorseVariableList == null)
            {
                hmb.HorseVariableList = new ObservableCollection<HPTHorseVariable>(HPTHorseVariable.CreateVariableList());
            }
            hmb.RaceDayInfo.HorseListSelected = new ObservableCollection<HPTHorse>();

            // Create IntervalReductionRuleList
            if (hmb.IntervalReductionRuleList == null)
            {
                hmb.IntervalReductionRuleList = new ObservableCollection<HPTIntervalReductionRule>()
                {
                    //hmb.PercentSumReductionRule,
                    hmb.RowValueReductionRule,
                    hmb.StakePercentSumReductionRule,
                    hmb.StartNrSumReductionRule,
                    hmb.ATGRankSumReductionRule,
                    hmb.OwnRankSumReductionRule,
                    hmb.AlternateRankSumReductionRule,
                    hmb.OddsSumReductionRule
                };
            }

            //// Undvika specialkomprimering
            //if (hmb.CouponCompression == CouponCompression.V6BetMultiplier)
            //{
            //    hmb.CouponCompression = CouponCompression.Default;
            //}

            // Undvik onödig uppläsning av HorseOwnInformation
            bool horseOwnInformationShows = HPTConfig.Config.DataToShowVxx.ShowOwnInformation || HPTConfig.Config.DataToShowComplementaryRules.ShowOwnInformation || HPTConfig.Config.DataToShowCorrection.ShowOwnInformation || HPTConfig.Config.MarkBetTabsToShow.ShowComments;

            // Ta hänsyn till V6/V7/V8
            hmb.RaceDayInfo.SetV6Factor();

            foreach (HPTRace hptRace in hmb.RaceDayInfo.RaceList)
            {
                hptRace.ParentRaceDayInfo = hmb.RaceDayInfo;
                hptRace.HorseListSelected = new List<HPTHorse>();

                // Skapa reservlista
                hptRace.ReservOrderList = new int[0];
                //if (!string.IsNullOrWhiteSpace(hptRace.ReservOrder))
                //{
                //    string[] reservStringArray = hptRace.ReservOrder.Split('-');
                //    hptRace.ReservOrderList = new int[reservStringArray.Length];
                //    for (int i = 0; i < reservStringArray.Length; i++)
                //    {
                //        hptRace.ReservOrderList[i] = int.Parse(reservStringArray[i]);
                //    }
                //}


                foreach (HPTHorse hptHorse in hptRace.HorseList)
                {
                    // Skapa listor som måste finnas

                    hptHorse.HorseXReductionList = new ObservableCollection<HPTHorseXReduction>();
                    hptHorse.RankList = new ObservableCollection<HPTHorseRank>();

                    // Sätt parent
                    hptHorse.ParentRace = hptRace;

                    // Beräknade värden
                    hptHorse.CalculateDerivedValues();
                    hptHorse.CalculateStaticDerivedValues();

                    // Skapa ABCDEF-regler
                    hptHorse.CreateXReductionRuleList();
                    if (hptHorse.Prio != HPTPrio.M)
                    {
                        hptHorse.HorseXReductionList.First(h => h.Prio == hptHorse.Prio).Selected = true;
                    }
                    if (hptHorse.Selected)
                    {
                        hmb.RaceDayInfo.HorseListSelected.Add(hptHorse);
                    }

                    // Skoinfo
                    if (hptHorse.ShoeInfoCurrent != null && hptHorse.ShoeInfoPrevious != null)
                    {
                        hptHorse.ShoeInfoCurrent.SetChangedFlags(hptHorse.ShoeInfoPrevious);
                    }

                    // Own information
                    if (horseOwnInformationShows)
                    {
                        HPTConfig.Config.HorseOwnInformationCollection.MergeHorseOwnInformation(hptHorse);
                    }
                }

                // Sätt justerad insatsfördelning
                hptRace.SetCorrectStakeDistributionShare();
                hptRace.SetCorrectStakeDistributionShareAlt1();
                hptRace.SetCorrectStakeDistributionShareAlt2();

                // Inbördes möten
                hptRace.FindHeadToHead();

                // Sätt visningstext för loppet
                switch (hmb.BetType.Code)
                {
                    case "TV":
                        hptRace.LegNrString = "Lopp " + hptRace.LegNr.ToString();
                        break;
                    case "T":
                        hptRace.LegNrString = "Trio" + "-" + hptRace.LegNr.ToString();
                        break;
                    default:
                        hptRace.LegNrString = hmb.BetType.Code + "-" + hptRace.LegNr.ToString();
                        break;
                }

                // Grupperingsnamn för reserver i GUIt
                hptRace.Reserv1GroupName = hptRace.LegNr.ToString() + "-" + "1";
                hptRace.Reserv2GroupName = hptRace.LegNr.ToString() + "-" + "2";
            }
            SetTrainerAndDriver(hmb);

            // ABCDEF-regel
            var aRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.A);
            aRule.Use = !aRule.Use ? HPTConfig.Config.UseA : true;
            var bRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.B);
            bRule.Use = !bRule.Use ? HPTConfig.Config.UseB : true;
            var cRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.C);
            cRule.Use = !cRule.Use ? HPTConfig.Config.UseC : true;
            var dRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.D);
            dRule.Use = !dRule.Use ? HPTConfig.Config.UseD : true;
            var eRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.E);
            eRule.Use = !eRule.Use ? HPTConfig.Config.UseE : true;
            var fRule = hmb.ABCDEFReductionRule.XReductionRuleList.First(r => r.Prio == HPTPrio.F);
            fRule.Use = !fRule.Use ? HPTConfig.Config.UseF : true;

            // Drivers
            foreach (HPTPersonReductionRule rule in hmb.DriverRulesCollection.ReductionRuleList)
            {
                rule.PersonList = new ObservableCollection<HPTPerson>();
                foreach (string personName in rule.PersonShortNameList)
                {
                    var person = hmb.DriverRulesCollection.PersonList.FirstOrDefault(p => p.ShortName.ToLower() == personName.ToLower());
                    if (person != null && !rule.PersonList.Contains(person))
                    {
                        rule.PersonList.Add(person);
                    }
                }
            }
            hmb.DriverRulesCollection.ReductionRuleFactory = hmb.DriverRulesCollection.CreateNewDriverReductionRule;
            hmb.DriverRulesCollection.Initialize();

            // Trainers
            foreach (HPTPersonReductionRule rule in hmb.TrainerRulesCollection.ReductionRuleList)
            {
                rule.PersonList = new ObservableCollection<HPTPerson>();
                foreach (string personName in rule.PersonShortNameList)
                {
                    var person = hmb.TrainerRulesCollection.PersonList.FirstOrDefault(p => p.ShortName == personName);
                    if (person != null && !rule.PersonList.Contains(person))
                    {
                        rule.PersonList.Add(person);
                    }
                }
            }
            hmb.TrainerRulesCollection.ReductionRuleFactory = hmb.TrainerRulesCollection.CreateNewTrainerReductionRule;
            hmb.TrainerRulesCollection.Initialize();

            // Complementaryreduction rules
            if (hmb.ComplementaryRulesCollection.ReductionRuleList != null)
            {
                foreach (HPTComplementaryReductionRule rule in hmb.ComplementaryRulesCollection.ReductionRuleList)
                {
                    rule.HorseList = new ObservableCollection<HPTHorse>();
                    foreach (var horseLight in rule.HorseLightList)
                    {
                        HPTHorse horse = hmb.RaceDayInfo.HorseListSelected
                            .FirstOrDefault(h => h.ParentRace.LegNr == horseLight.LegNr && h.StartNr == horseLight.StartNr);

                        if (horse != null)
                        {
                            rule.HorseList.Add(horse);
                        }
                    }
                }
                hmb.ComplementaryRulesCollection.Initialize();
            }

            // V6BetMultiplierRule
            int ruleNumber = 1;
            foreach (var v6BetMultiplierRule in hmb.V6BetMultiplierRuleList)
            {
                v6BetMultiplierRule.HorseList = new List<HPTHorse>();
                v6BetMultiplierRule.BetMultiplierList = hmb.BetType.BetMultiplierList;
                v6BetMultiplierRule.MarkBet = hmb;

                List<HPTHorseLightSelectable> horseLightSelectableList = v6BetMultiplierRule.RaceList
                    .SelectMany(r => r.HorseList).ToList();

                foreach (var horseLightSelectable in horseLightSelectableList)
                {
                    HPTHorse horse = hmb.RaceDayInfo.RaceList.SelectMany(r => r.HorseList)
                        .FirstOrDefault(h => h.StartNr == horseLightSelectable.StartNr && h.ParentRace.LegNr == horseLightSelectable.LegNr);

                    horseLightSelectable.Horse = horse;
                    if (horseLightSelectable.Selected)
                    {
                        v6BetMultiplierRule.HorseList.Add(horse);
                        v6BetMultiplierRule.RaceList.First(r => r.LegNr == horseLightSelectable.LegNr).SelectedHorse =
                            horseLightSelectable;

                        horseLightSelectable.GroupCode = ruleNumber.ToString() + horse.ParentRace.LegNr.ToString();
                    }
                }
                ruleNumber++;
            }

            // GroupReductionRules
            try
            {
                foreach (HPTGroupIntervalReductionRule rule in hmb.GroupIntervalRulesCollection.ReductionRuleList)
                {
                    rule.HorseVariable = HPTConfig.Config.HorseVariableList.FirstOrDefault(hv => hv.PropertyName == rule.PropertyName);
                }
                hmb.GroupIntervalRulesCollection.Initialize();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            hmb.SingleRowCollection = new HPTMarkBetSingleRowCollection(hmb);
            hmb.SingleRowCollection.AnalyzingFinished += hmb.SingleRowCollection_AnalyzingFinished;

            // Ranksummereduceringsregler
            if (hmb.HorseRankVariableList == null || hmb.HorseRankVariableList.Count == 0)
            {
                hmb.HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
            }
            if (hmb.HorseRankSumReductionRuleList != null)
            {
                var horseRankSumReductionRulesToRemove = new List<HPTHorseRankSumReductionRule>();
                foreach (var horseRankSumReductionRule in hmb.HorseRankSumReductionRuleList)
                {
                    horseRankSumReductionRule.HorseRankVariable = hmb.HorseRankVariableList.FirstOrDefault(hrv => hrv.PropertyName == horseRankSumReductionRule.PropertyName);
                    if (horseRankSumReductionRule != null)
                    {
                        if (horseRankSumReductionRule.HorseRankVariable == null)
                        {
                            horseRankSumReductionRulesToRemove.Add(horseRankSumReductionRule);
                        }
                        else
                        {
                            foreach (var rule in horseRankSumReductionRule.ReductionRuleList)
                            {
                                rule.ParentHorseRankSumReductionRule = horseRankSumReductionRule;
                            }
                        }
                    }
                }
                foreach (var horseRankSumReductionRuleToRemove in horseRankSumReductionRulesToRemove)
                {
                    hmb.HorseRankSumReductionRuleList.Remove(horseRankSumReductionRuleToRemove);
                }
            }

            // Ta bort rankvariabel för streckprocent
            var rankVariableToRemove = hmb.HorseRankVariableList.FirstOrDefault(hrv => string.IsNullOrEmpty(hrv.PropertyName) || hrv.PropertyName == "MarksQuantity");
            if (rankVariableToRemove != null)
            {
                hmb.HorseRankVariableList.Remove(rankVariableToRemove);
            }

            // Ta hand om låsta kuponger
            if (hmb.LockCoupons && hmb.CouponList != null)
            {
                try
                {
                    hmb.CouponCorrector = new HPTCouponCorrector()
                    {
                        RaceDayInfo = hmb.RaceDayInfo,
                        CouponHelper = new ATGCouponHelper(hmb)
                        {
                            CouponList = hmb.CouponList
                        }
                    };
                    hmb.CouponCorrector.CouponHelper.CreateHorseListsForCoupons();
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
            }

            try
            {
                if (!Directory.Exists(hmb.SaveDirectory))
                {
                    hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }

            // Sätt avstån till hemmabanan för alla hästar
            SetDistanceToHomeTrack(hmb.RaceDayInfo);

            // IHorseListContainer
            //hmb.HorseList = new ObservableCollection<HPTHorse>(hmb.RaceDayInfo.RaceList.SelectMany(r => r.HorseList));
            hmb.HorseList = new List<HPTHorse>(hmb.RaceDayInfo.RaceList.SelectMany(r => r.HorseList));
            hmb.ParentRaceDayInfo = hmb.RaceDayInfo;
        }

        #region Konvertera Reducto/HPT Online till och från HPTMarkBet

        public static HPTService.HPTRaceDayInfoReduction CreateHPTOnlineFromHPTMarkBet(HPTMarkBet markBet)
        {
            var raceDayInfoReduction = new HPTService.HPTRaceDayInfoReduction()
            {
                BetTypeCode = markBet.BetType.Code,
                Comment = markBet.SystemComment == null ? string.Empty : markBet.SystemComment,
                EMail = HPTConfig.Config.EMailAddress,
                LastUpdate = markBet.LastSaveTime,
                NumberOfAllowedErrors = markBet.NumberOfToleratedErrors,
                RaceDayDate = markBet.RaceDayInfo.RaceDayDate,
                RaceDayDateString = markBet.RaceDayInfo.RaceDayDateString,
                SystemName = markBet.SystemName,
                TrackId = markBet.RaceDayInfo.TrackId,
                RaceList = markBet.RaceDayInfo.RaceList.Select(r => new HPTService.HPTRaceReduction()
                {
                    LegNr = r.LegNr,
                    R1 = r.Reserv1Nr,
                    R2 = r.Reserv2Nr,
                    HorseList = r.HorseListSelected.Select(h => new HPTService.HPTHorseReduction()
                    {
                        GroupCodeList = null,
                        OwnRank = h.RankOwn,
                        Points = h.RankAlternate,
                        Prio = h.PrioString,
                        Selected = h.Selected,
                        StartNr = h.StartNr
                    }).ToArray()
                }).ToArray()
            };

            // ABCD-regel
            raceDayInfoReduction.ReductionABCD = new HPTService.HPTReductionABCD()
            {
                ReductionXArray = markBet.ABCDEFReductionRule.XReductionRuleList
                .Where(xrr => xrr.Prio == HPTPrio.A || xrr.Prio == HPTPrio.B || xrr.Prio == HPTPrio.C || xrr.Prio == HPTPrio.D)
                .Select(xrr => new HPTService.HPTReductionX()
                {
                    GroupCode = xrr.Prio.ToString(),
                    Min = xrr.MinNumberOfX,
                    Max = xrr.MaxNumberOfX,
                    Use = xrr.Use
                }).ToArray(),
                Use = markBet.ABCDEFReductionRule.Use
            };

            // Utgångar
            int ruleNumber = 1;
            var onlineReductionGroupList = new List<HPTService.HPTReductionGroup>();
            markBet.ComplementaryRulesCollection.ReductionRuleList
                .Cast<HPTComplementaryReductionRule>()
                .ToList()
                .ForEach(rr =>
                {
                    rr.Reset();
                    var reductionGroup = new HPTService.HPTReductionGroup()
                    {
                        GroupCode = "U" + ruleNumber.ToString(),
                        Min = rr.MinNumberOfX,
                        Max = rr.MaxNumberOfX,
                        Use = rr.Use
                    };

                    onlineReductionGroupList.Add(reductionGroup);

                    rr.HorseLightList
                        .ForEach(hl =>
                        {
                            var horseReduction = raceDayInfoReduction
                                .RaceList
                                .First(r => r.LegNr == hl.LegNr)
                                .HorseList
                                .First(h => h.StartNr == hl.StartNr);

                            if (horseReduction.GroupCodeList == null || horseReduction.GroupCodeList.Length == 0)
                            {
                                horseReduction.GroupCodeList = new string[] { reductionGroup.GroupCode };
                            }
                            else
                            {
                                horseReduction.GroupCodeList = horseReduction.GroupCodeList
                                    .Concat(new string[] { reductionGroup.GroupCode })
                                    .ToArray();
                            }
                        });
                    ruleNumber++;
                });

            raceDayInfoReduction.ReductionGroupList = new HPTService.HPTReductionGroupList()
            {
                Use = markBet.ComplementaryRulesCollection.Use,
                ReductionGroupArray = onlineReductionGroupList.ToArray()
            };

            // Radvärdesregel
            raceDayInfoReduction.ReductionRowValue = new HPTService.HPTReductionRowValue()
            {
                Min = markBet.RowValueReductionRule.MinSum,
                Max = markBet.RowValueReductionRule.MaxSum,
                Use = markBet.RowValueReductionRule.Use
            };

            // Egen ranksumma
            raceDayInfoReduction.ReductionOwnRank = new HPTService.HPTReductionOwnRank()
            {
                Min = markBet.OwnRankSumReductionRule.MinSum,
                Max = markBet.OwnRankSumReductionRule.MaxSum,
                Use = markBet.OwnRankSumReductionRule.Use
            };

            // Poäng/Alternativ rank
            raceDayInfoReduction.ReductionPoints = new HPTService.HPTReductionPoints()
            {
                Min = markBet.AlternateRankSumReductionRule.MinSum,
                Max = markBet.AlternateRankSumReductionRule.MaxSum,
                Use = markBet.AlternateRankSumReductionRule.Use
            };

            // Startnummersumma
            raceDayInfoReduction.ReductionStartnumberSum = new HPTService.HPTReductionStartnumberSum()
            {
                Min = markBet.StartNrSumReductionRule.MinSum,
                Max = markBet.StartNrSumReductionRule.MaxSum,
                Use = markBet.StartNrSumReductionRule.Use
            };

            // V6/V7/V8/Flerbong
            var v6BetMultiplierSettings = new HPTService.HPTV6BetMultiplierSettings()
            {
                BetMultiplier = markBet.BetMultiplier,
                RowValueTarget = markBet.SingleRowTargetProfit,
                V6 = markBet.V6,
                V6LimitOwnRankSum = markBet.V6OwnRankMax,
                V6LimitRowValue = Convert.ToInt32(markBet.V6UpperBoundary)
            };
            // Lägg bara till om det faktiskt gjorts några inställningar
            if (markBet.V6 || markBet.V6OwnRank || markBet.V6SingleRows || markBet.BetMultiplier > 1 || markBet.SingleRowBetMultiplier)
            {
                raceDayInfoReduction.V6BetMultiplierSettings = v6BetMultiplierSettings;
            }

            return raceDayInfoReduction;
        }

        public static void ApplyHPTOnlineToHPTMarkBet(HPTMarkBet markBet, HPTService.HPTRaceDayInfoReduction raceDayInfoReduction)
        {
            try
            {
                markBet.pauseRecalculation = true;
                var horsesWithGroupCode = new List<Tuple<HPTHorse, HPTService.HPTHorseReduction>>();

                foreach (var raceReduction in raceDayInfoReduction.RaceList)
                {
                    var race = markBet.RaceDayInfo.RaceList.First(r => r.LegNr == raceReduction.LegNr);
                    foreach (var horseReduction in raceReduction.HorseList)
                    {
                        var horse = race.HorseList.First(h => h.StartNr == horseReduction.StartNr);
                        horse.Selected = horseReduction.Selected;
                        horse.RankAlternate = horseReduction.OwnRank;

                        // ABCD
                        var prio = EnumHelper.GetHPTPrioFromShortString(horseReduction.Prio);
                        var xReductionHorse = horse.HorseXReductionList.FirstOrDefault(hxr => hxr.Prio == prio);
                        if (xReductionHorse != null)
                        {
                            xReductionHorse.Selected = true;
                        }

                        // Utgångar
                        if (horseReduction.GroupCodeList != null && horseReduction.GroupCodeList.Any())
                        {
                            horsesWithGroupCode.Add(new Tuple<HPTHorse, HPTService.HPTHorseReduction>(horse, horseReduction));
                        }
                    }
                }

                // ABCD-regel
                if (raceDayInfoReduction.ReductionABCD != null)
                {
                    markBet.ABCDEFReductionRule.Use = (bool)raceDayInfoReduction.ReductionABCD.Use;
                    foreach (var xReductionToConvert in raceDayInfoReduction.ReductionABCD.ReductionXArray)
                    {
                        var prio = EnumHelper.GetHPTPrioFromShortString(xReductionToConvert.GroupCode);
                        var xReduction = markBet.ABCDEFReductionRule.XReductionRuleList.FirstOrDefault(xr => xr.Prio == prio);
                        if (xReduction != null)
                        {
                            int selectionLength = xReductionToConvert.Max - xReductionToConvert.Min + 1;
                            xReduction.SelectInterval(xReductionToConvert.Min, selectionLength, true);
                        }
                        xReduction.Use = (bool)xReductionToConvert.Use;
                    }
                }

                // Utgångar
                if (raceDayInfoReduction.ReductionGroupList != null
                    && raceDayInfoReduction.ReductionGroupList.ReductionGroupArray != null
                    && raceDayInfoReduction.ReductionGroupList.ReductionGroupArray.Length > 0)
                {

                    foreach (var reductionGroup in raceDayInfoReduction.ReductionGroupList.ReductionGroupArray)
                    {
                        var horsesInGroup = horsesWithGroupCode
                            .Where(ht => ht.Item2.GroupCodeList.Contains(reductionGroup.GroupCode))
                            .Select(ht => ht.Item1)
                            .ToList();

                        var complementaryReductionRule = new HPTComplementaryReductionRule(markBet.NumberOfRaces, true)
                        {
                            Use = (bool)reductionGroup.Use,
                            HorseLightList = horsesInGroup.Select(h => new HPTHorseLight()
                            {
                                LegNr = h.ParentRace.LegNr,
                                StartNr = h.StartNr
                            }).ToList()
                            //HorseList = new ObservableCollection<HPTHorse>(horsesInGroup)
                        };

                        int numberOfDifferentRaces = horsesInGroup.Select(h => h.ParentRace.LegNr).Distinct().Count();
                        complementaryReductionRule.SetSelectable(numberOfDifferentRaces);
                        complementaryReductionRule.SelectInterval(reductionGroup.Min, reductionGroup.Max - reductionGroup.Min + 1, true);
                        markBet.ComplementaryRulesCollection.ReductionRuleList.Add(complementaryReductionRule);
                        markBet.ComplementaryRulesCollection.Use = (bool)raceDayInfoReduction.ReductionGroupList.Use;
                    }
                }

                // Poäng/Egen rank
                if (raceDayInfoReduction.ReductionOwnRank != null)
                {
                    markBet.AlternateRankSumReductionRule.Use = (bool)raceDayInfoReduction.ReductionOwnRank.Use;
                    markBet.AlternateRankSumReductionRule.MinSum = raceDayInfoReduction.ReductionOwnRank.Min;
                    markBet.AlternateRankSumReductionRule.MaxSum = raceDayInfoReduction.ReductionOwnRank.Max;
                }

                // Startnummersumma
                if (raceDayInfoReduction.ReductionStartnumberSum != null)
                {
                    markBet.StartNrSumReductionRule.Use = (bool)raceDayInfoReduction.ReductionStartnumberSum.Use;
                    markBet.StartNrSumReductionRule.MinSum = raceDayInfoReduction.ReductionStartnumberSum.Min;
                    markBet.StartNrSumReductionRule.MaxSum = raceDayInfoReduction.ReductionStartnumberSum.Max;
                }

                // Radvärde
                if (raceDayInfoReduction.ReductionRowValue != null)
                {
                    markBet.RowValueReductionRule.Use = (bool)raceDayInfoReduction.ReductionRowValue.Use;
                    markBet.RowValueReductionRule.MinSum = raceDayInfoReduction.ReductionRowValue.Min;
                    markBet.RowValueReductionRule.MaxSum = raceDayInfoReduction.ReductionRowValue.Max;
                }

                // V6/V7/V8/Flerbong
                if (raceDayInfoReduction.V6BetMultiplierSettings != null)
                {
                    // Gräns utifrån egen ranksumma
                    if (raceDayInfoReduction.V6BetMultiplierSettings.V6LimitOwnRankSum > 0)
                    {
                        markBet.V6OwnRank = true;
                        markBet.V6OwnRankMax = raceDayInfoReduction.V6BetMultiplierSettings.V6LimitOwnRankSum;
                    }

                    // Gräns utifrån beräknat radvärde
                    if (raceDayInfoReduction.V6BetMultiplierSettings.V6LimitRowValue > 0)
                    {
                        markBet.V6SingleRows = true;
                        markBet.V6UpperBoundary = raceDayInfoReduction.V6BetMultiplierSettings.V6LimitRowValue;
                    }

                    // Om inte någon av de två ovanstående används kan vi sätta den generella propertyn på MarkBet
                    if (raceDayInfoReduction.V6BetMultiplierSettings.V6LimitOwnRankSum == 0 || raceDayInfoReduction.V6BetMultiplierSettings.V6LimitRowValue == 0)
                    {
                        markBet.V6 = raceDayInfoReduction.V6BetMultiplierSettings.V6;
                    }

                    // Flerbong för att nå målvinst
                    if (raceDayInfoReduction.V6BetMultiplierSettings.RowValueTarget > 0)
                    {
                        markBet.SingleRowBetMultiplier = true;
                        markBet.SingleRowTargetProfit = raceDayInfoReduction.V6BetMultiplierSettings.RowValueTarget;
                    }

                    // Generell flerbong
                    markBet.BetMultiplier = raceDayInfoReduction.V6BetMultiplierSettings.BetMultiplier;
                }
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
            }
            markBet.BetType.IsEnabled = true;
            markBet.pauseRecalculation = false;
        }

        #endregion

        #region PLINQ-versioner

        public static HPTCalendar CreateCalendarParallell(byte[] baCalendar)
        {
            DateTime dtStart = DateTime.Now;
            var hptCalendar = new HPTCalendar();
            try
            {
                HPTService.HPTCalendar calendar = HPTSerializer.DeserializeHPTCalendar(baCalendar);
                HPTServiceToHPTHelper.ConvertCalendar(calendar, hptCalendar);

                // Temporär lista
                var hptRaceDayInfoList = new List<HPTRaceDayInfo>();
                calendar.HPTRaceDayInfoList
                    .AsParallel()
                    .ForAll(rdi =>
                    {
                        var hptRdi = new HPTRaceDayInfo();
                        HPTServiceToHPTHelper.ConvertCalendarRaceDayInfo(rdi, hptRdi);
                        if (hptRdi.BetTypeList.Count > 0)
                        {
                            hptRdi.ShowInUI = hptRdi.RaceDayDate.Date >= DateTime.Now.Date;
                            lock (hptRaceDayInfoList)
                            {
                                hptRaceDayInfoList.Add(hptRdi);
                            }
                        }
                    });

                hptCalendar.RaceDayInfoList = new ObservableCollection<HPTRaceDayInfo>(hptRaceDayInfoList.OrderBy(hptRdi => hptRdi.RaceDayDate));

                // Skapa lista med de tävlingar man ska kunna ladda ner systemförslag för
                var raceDayInfoLightList = new List<HPTRaceDayInfoLight>();
                hptCalendar.RaceDayInfoList
                    .Where(hptRdi => hptRdi.RaceDayDate < DateTime.Today.AddDays(6D) && hptRdi.RaceDayDate > DateTime.Today.AddDays(-3D))
                    .AsParallel()
                    .ForAll(hptRdi =>
                        {
                            var tempList = hptRdi.BetTypeList.Where(bt => bt.Code.StartsWith("V"))
                                .Select(bt => new HPTRaceDayInfoLight()
                                {
                                    BetTypeCode = bt.Code,
                                    RaceDayDate = hptRdi.RaceDayDate,
                                    TrackId = hptRdi.TrackId,
                                    TrackName = hptRdi.Trackname,
                                    NumberOfUploadedSystems = bt.NumberOfUploadedSystems
                                });

                            lock (raceDayInfoLightList)
                            {
                                raceDayInfoLightList.AddRange(tempList);
                            }
                        });

                raceDayInfoLightList.ForEach(rl => HPTConfig.Config.MarkBetSystemList.Add(rl));

                // Sätt sökväg och spara ner kalender på disk
                string calendarPath = HPTConfig.MyDocumentsPath + "\\HPTCalendar.hptcal";

                TimeSpan ts = DateTime.Now - dtStart;
                string time = ts.TotalMilliseconds.ToString();
            }
            catch (Exception exc)
            {
                HPTConfig.AddToErrorLogStatic(exc);
                return null;
            }
            return hptCalendar;
        }

        #endregion
    }
}

using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace HPTClient
{
    /// <summary>
    /// Summary description for ATGCouponHelper
    /// </summary>
    public class ATGCouponHelper : Notifier
    {
        public issuer ATGFile { get; set; }
        public string BetType { get; set; }
        public int CurrentCouponId { get; set; }
        private HPTMarkBet MarkBet { get; set; }
        private HPTCombBet CombBet { get; set; }
        public HPTBet Bet { get; set; }

        public string TempFileName { get; set; }
        public string CurrentFileName { get; set; }

        public ATGCouponHelper(HPTBet bet)
        {
            this.CouponList = new ObservableCollection<HPTCoupon>();
            this.CurrentCouponId = 1;
            this.Bet = bet;

            if (bet.GetType() == typeof(HPTMarkBet))
            {
                this.MarkBet = (HPTMarkBet)bet;
            }
            else if (bet.GetType() == typeof(HPTCombBet))
            {
                this.CombBet = (HPTCombBet)bet;
            }
            InitiateATGFile();
        }

        private ObservableCollection<HPTCoupon> couponList;
        public ObservableCollection<HPTCoupon> CouponList
        {
            get
            {
                return this.couponList;
            }
            set
            {
                this.couponList = value;
                OnPropertyChanged("CouponList");
            }
        }

        internal void InitiateATGFile()
        {
            this.ATGFile = new issuer()
            {
                company = "Kubin Software",
                createddate = DateTime.Now,
                createddateSpecified = true,
                createdtime = DateTime.Now.ToString("hh:mm:ss"),
                createdtimeSpecified = true,   // Borde vara true, men serialiseringen skapar ett felaktigt format
                //createdtime = DateTime.Now,
                //createdtimeSpecified = false,   // Borde vara true, men serialiseringen skapar ett felaktigt format
                product = "Hjälp på traven!",
                version = "5.33",
                betcoupons = new betcouponsType()
            };
        }

        internal issuer CloneATGFile()
        {
            issuer clonedATGFile = new issuer()
            {
                company = this.ATGFile.company,
                createddate = this.ATGFile.createddate,
                createddateSpecified = this.ATGFile.createddateSpecified,
                createdtime = this.ATGFile.createdtime,
                createdtimeSpecified = this.ATGFile.createdtimeSpecified,   // Borde vara true, men serialiseringen skapar ett felaktigt format
                product = this.ATGFile.product,
                version = this.ATGFile.version,
                betcoupons = new betcouponsType()
            };
            return clonedATGFile;
        }

        // TODO: Uppdatera när det nya schemat ska användas
        private int numberOfMarksInLeg;
        public int NumberOfMarksInLeg
        {
            get
            {
                //return 15;
                if (this.numberOfMarksInLeg == 0)
                {
                    if (this.CombBet != null)
                    {
                        // Franska banor med 20 hästar
                        if ((this.BetType == "T" || this.BetType == "TV") && this.CombBet.RaceDayInfo.TrackId >= 62 && this.CombBet.RaceDayInfo.TrackId <= 66)
                        {
                            this.numberOfMarksInLeg = 20;
                        }
                        else
                        {
                            this.numberOfMarksInLeg = 15;
                        }
                    }
                    else
                    {
                        switch (this.MarkBet.RaceDayInfo.BetType.Code)
                        {
                            case "V4":
                                this.numberOfMarksInLeg = 20;
                                break;
                            default:
                                this.numberOfMarksInLeg = 15;
                                break;
                        }
                    }
                }
                return this.numberOfMarksInLeg;
            }
        }

        private string raceNumberString = string.Empty; // Sträng för när man sparar enskilt Trio- eller Tvillinglopp
        private string partString = string.Empty;       // Sträng för när man delar upp ett system i flera filer
        public string CreateATGFile()
        {
            if (this.CombBet != null)
            {
                CreateATGFile(this.CouponList, this.ATGFile);
                return string.Empty;
            }
            if (this.MarkBet != null)
            {
                //if (!HPTConfig.Config.IsPayingCustomer && this.MarkBet.TooExpensive)
                //{
                //    return string.Empty;
                //}
                HandleReserverForCoupons(this.MarkBet.ReservHandling);
            }

            if (!this.MarkBet.HasTooManySystems)
            {
                CreateATGFile(this.CouponList, this.ATGFile);
            }
            else    // Fullständigt jävla enormt många kuponger...eller snarare än ATG tycker man ska få ha
            {
                int partNumber = 1;

                var couponListList = GetCouponListList();

                foreach (var couponList in couponListList)
                {
                    issuer clonedATGFile = CloneATGFile();
                    this.partString = "_Part" + partNumber.ToString();
                    CreateATGFile(couponList, clonedATGFile);
                    partNumber++;
                }

                this.partString = string.Empty;
            }
            return string.Empty;
        }

        internal List<List<HPTCoupon>> GetCouponListList()
        {
            var couponListList = new List<List<HPTCoupon>>();

            var couponList = new List<HPTCoupon>();
            decimal systemSizeATGSum = 0;
            int couponIdFile = 1;
            foreach (var coupon in this.CouponList)
            {
                systemSizeATGSum += coupon.SystemSizeATG;
                coupon.CouponIdFile = couponIdFile++;
                if (systemSizeATGSum > this.MarkBet.BetType.MaxNumberOfSystemsInFile)
                {
                    couponIdFile = 1;
                    coupon.CouponIdFile = couponIdFile++;
                    couponListList.Add(couponList);
                    systemSizeATGSum = coupon.BetMultiplier;
                    couponList = new List<HPTCoupon>();
                }
                couponList.Add(coupon);
            }
            couponListList.Add(couponList);

            return couponListList;
        }

        public string CreateATGFile(IEnumerable<HPTCoupon> couponList, issuer atgFile)//, int accumulatedNumberOfCoupons)
        {
            #region Skapa ATG-kuponger
            switch (this.Bet.BetType.Code)
            {
                case "V4":
                    atgFile.betcoupons.v4Coupon = couponList.Select(hc => new v4CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        date = hc.Date,
                        trackcode = hc.TrackCode,
                        leg = ConvertRaceListToLeg20Array(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "V5":
                    atgFile.betcoupons.v5Coupon = couponList.Select(hc => new v5CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        date = hc.Date,
                        trackcode = hc.TrackCode,
                        leg = ConvertRaceListToLegArray(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "V65":
                    atgFile.betcoupons.v65Coupon = couponList.Select(hc => new v65CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        date = hc.Date,
                        v6 = hc.V6,
                        leg = ConvertRaceListToLegArray(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "V64":
                    atgFile.betcoupons.v64Coupon = couponList.Select(hc => new v64CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        date = hc.Date,
                        v6 = hc.V6,
                        leg = ConvertRaceListToLegArray(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "V75":
                    atgFile.betcoupons.v75Coupon = couponList.Select(hc => new v75CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        date = hc.Date,
                        v7 = hc.V6,
                        leg = ConvertRaceListToLegArray(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "V85":
                    atgFile.betcoupons.v85Coupon = couponList.Select(hc => new v85CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        date = hc.Date,
                        trackcode = hc.TrackCode,
                        leg = ConvertRaceListToLegArray(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "GS75":
                    atgFile.betcoupons.gs75Coupon = couponList.Select(hc => new gs75CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        date = hc.Date,
                        v7 = hc.V6,
                        leg = ConvertRaceListToLegArray(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "V86":
                    atgFile.betcoupons.v86Coupon = couponList.Select(hc => new v86CouponType()
                    {
                        betmultiplier = hc.BetMultiplier.ToString(),
                        couponid = hc.CouponIdFile.ToString(),
                        trackcode = hc.TrackCode,
                        date = hc.Date,
                        v8 = hc.V6,
                        leg = ConvertRaceListToLegArray(hc.CouponRaceList)
                    }).ToArray();
                    break;

                case "DD":
                    atgFile.betcoupons.DDCoupon = couponList.Select(hc => new DDCouponType()
                    {
                        couponid = hc.CouponId.ToString(),
                        date = hc.Date,
                        marks1 = AddLeg(hc.CouponRaceList.First().HorseList.First(h => h != null).StartNr),
                        marks2 = AddLeg(hc.CouponRaceList.First().HorseList.Last(h => h != null).StartNr),
                        stake = hc.Stake.ToString()
                    }).ToArray();
                    break;

                case "LD":
                    atgFile.betcoupons.LDCoupon = couponList.Select(hc => new LDCouponType()
                    {
                        couponid = hc.CouponId.ToString(),
                        date = hc.Date,
                        marks1 = AddLeg(hc.CouponRaceList.First().HorseList.First(h => h != null).StartNr),
                        marks2 = AddLeg(hc.CouponRaceList.First().HorseList.Last(h => h != null).StartNr),
                        stake = hc.Stake.ToString()
                    }).ToArray();
                    break;

                case "TV":
                    atgFile.betcoupons.tvillingCoupon = couponList.Select(hc => new tvillingCouponType()
                    {
                        couponid = hc.CouponId.ToString(),
                        date = hc.Date,
                        marks = AddLeg(hc.CouponRaceList.First().HorseList.First(h => h != null).StartNr),
                        umarks = AddLeg(hc.CouponRaceList.First().HorseList.Last(h => h != null).StartNr),
                        stake = hc.Stake.ToString(),
                        raceno = hc.RaceNumber.ToString(),
                        trackcode = hc.TrackCode
                    }).ToArray();
                    break;

                case "T":
                    atgFile.betcoupons.trioCoupon = couponList.Select(hc => new trioCouponType()
                    {
                        couponid = hc.CouponId.ToString(),
                        date = hc.Date,
                        marks1st = AddLeg(hc.CouponRaceList.First().HorseList[0].StartNr),
                        marks2nd = AddLeg(hc.CouponRaceList.First().HorseList[1].StartNr),
                        marks3rd = AddLeg(hc.CouponRaceList.First().HorseList[2].StartNr),
                        stake = hc.Stake.ToString(),
                        raceno = hc.RaceNumber.ToString(),
                        trackcode = hc.TrackCode
                    }).ToArray();
                    break;

                default:
                    break;
            }

            //for (int i = 0; i < couponList.Count(); i++)
            //{
            //    couponList.ElementAt(i).CouponId = i + 1;
            //}

            #endregion

            SaveATGFile(atgFile);

            return string.Empty;
        }

        public bool OnlyCreateTempFile { get; set; }
        internal void SaveATGFile(issuer atgXMLFile)
        {
            // Serialisera ATG-filen
            var serializer = new XmlSerializer(typeof(issuer));
            var ms = new MemoryStream();
            var xtw = new XmlTextWriter(ms, Encoding.UTF8);
            serializer.Serialize(xtw, atgXMLFile);

            string dir = string.Empty;
            if (this.OnlyCreateTempFile)
            {
                dir = Path.GetDirectoryName(HPTConfig.TempPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string fileName = dir + "\\" + this.MarkBet.ToFileNameString() + ".xml";
                serializer = new XmlSerializer(typeof(issuer));
                xtw = new XmlTextWriter(fileName, Encoding.UTF8);
                serializer.Serialize(xtw, atgXMLFile);
                xtw.Flush();
                xtw.Close();
                this.TempFileName = fileName;
                return;
            }
            else
            {
                // Skapa katalog för systemfilen om den inte finns
                dir = Path.GetDirectoryName(this.Bet.SystemFilename);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string archiveDir = dir + "\\" + "Arkiv\\";
                if (!Directory.Exists(archiveDir))
                {
                    Directory.CreateDirectory(archiveDir);
                }
                if (!string.IsNullOrEmpty(this.CurrentFileName))
                {
                    try
                    {
                        File.Move(this.CurrentFileName, archiveDir + Path.GetFileName(this.CurrentFileName));
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                }
            }

            #region Ordinarie lösning

            // Lägg till CRC-checken
            xtw.BaseStream.Position = 0;
            string file = Path.GetFileNameWithoutExtension(this.Bet.SystemFilename);
            Crc16 crc = new Crc16();
            string crcHex = crc.GetCheckSumAsHexString(xtw.BaseStream);
            this.Bet.SystemFilename = dir + @"\" + file + this.raceNumberString + this.partString + "_" + crcHex + ".xml";

            // Spara ner på disk
            xtw.BaseStream.Position = 0;

            StreamReader sr = new StreamReader(xtw.BaseStream);
            string xml = sr.ReadToEnd();
            StreamWriter sw = new StreamWriter(this.Bet.SystemFilename);
            sw.Write(xml);
            sw.Flush();
            sw.Close();
            xtw.Close();

            if (!string.IsNullOrEmpty(this.raceNumberString) || !string.IsNullOrEmpty(this.partString))
            {
                string partToRemove = this.raceNumberString + this.partString + "_" + crcHex;
                this.Bet.SystemFilename = this.Bet.SystemFilename.Replace(partToRemove, string.Empty);
            }

            // Spara filnamn så filen kan flyttas när en ny version sparas
            this.CurrentFileName = this.Bet.SystemFilename;

            #endregion
        }

        internal legType[] ConvertRaceListToLegArray(IList<HPTCouponRace> raceList)
        {
            legType[] legArray = raceList.Select(r => new legType()
            {
                legno = r.LegNr.ToString(),
                marks = AddLeg(r.HorseList),
                r1 = r.Reserv1 == 0 ? null : r.Reserv1.ToString(),
                r2 = r.Reserv2 == 0 ? null : r.Reserv2.ToString()
            }).ToArray();

            return legArray;
        }

        internal legType20[] ConvertRaceListToLeg20Array(IList<HPTCouponRace> raceList)
        {
            legType20[] legArray = raceList.Select(r => new legType20()
            {
                legno = r.LegNr.ToString(),
                marks = AddLeg(r.HorseList),
                r1 = r.Reserv1 == 0 ? null : r.Reserv1.ToString(),
                r2 = r.Reserv2 == 0 ? null : r.Reserv2.ToString()
            }).ToArray();

            return legArray;
        }

        internal string AddLeg(string uniqueCode)
        {
            int[] numbers = uniqueCode.ToCharArray()
                .Select(c => ConvertCharToInt(c)).ToArray();

            string marks = Enumerable.Range(1, this.NumberOfMarksInLeg)
                .Select(h => numbers.Contains(h) ? "1" : "0")
                .Aggregate((selectedStrings, next) => selectedStrings + next);

            return marks;
        }

        internal string AddLeg(HPTRace race)
        {
            int[] numbers = race.HorseListSelected.Select(h => h.StartNr).ToArray();

            string marks = Enumerable.Range(1, this.NumberOfMarksInLeg)
                .Select(h => numbers.Contains(h) ? "1" : "0")
                .Aggregate((selectedStrings, next) => selectedStrings + next);

            return marks;
        }

        internal string AddLeg(IEnumerable<HPTHorse> horseList)
        {
            try
            {
                IEnumerable<int> numbers = horseList.Select(h => h.StartNr);

                string marks = Enumerable.Range(1, this.NumberOfMarksInLeg)
                    .Select(h => numbers.Contains(h) ? "1" : "0")
                    .Aggregate((selectedStrings, next) => selectedStrings + next);

                return marks;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return string.Empty;
        }

        internal string AddLeg(int startNr)
        {
            int[] numbers = new int[] { startNr };

            string marks = Enumerable.Range(1, this.NumberOfMarksInLeg)
                .Select(h => numbers.Contains(h) ? "1" : "0")
                .Aggregate((selectedStrings, next) => selectedStrings + next);

            return marks;
        }

        internal List<int> GetMarkedHorse(string marks)
        {
            Regex rexMarks = new Regex("1");
            MatchCollection matches = rexMarks.Matches(marks);

            List<int> markPositions = new List<int>();
            foreach (Match m in matches)
            {
                markPositions.Add(m.Index + 1);
            }
            return markPositions;
        }

        internal void CalculateSystemSize(HPTCoupon coupon)
        {
            coupon.SystemSize = coupon.CouponRaceList
                .Select(r => r.NumberOfChosen)
                .Aggregate((numberOfChosen, next) => numberOfChosen * next);
        }

        internal int ConvertCharToInt(char c)
        {
            switch (c)
            {
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
                case 'G':
                    return 16;
                case 'H':
                    return 17;
                case 'I':
                    return 18;
                case 'J':
                    return 19;
                case 'K':
                    return 20;
                default:
                    return Convert.ToInt32(c.ToString());
            }
        }

        #region Create Coupons

        public string CreateCoupons()
        {
            switch (this.Bet.BetType.Code)
            {
                case "V4":
                case "V5":
                case "V64":
                case "V65":
                case "V75":
                case "V85":
                case "GS75":
                case "V86":
                    // Returnera om systemet är tomt
                    if (this.MarkBet.ReducedSize == 0)
                    {
                        if (this.CouponList != null)
                        {
                            this.CouponList.Clear();
                            this.TotalSystemSize = 0;
                        }
                        return string.Empty;
                    }

                    if (this.MarkBet.SingleRowCollection.CalculationInProgress || this.MarkBet.SingleRowCollection.CompressionInProgress)
                    {
                        return string.Empty;
                    }
                    this.TotalSystemSize = this.MarkBet.ReducedSize;
                    if (this.MarkBet.ReducedSize == 0)
                    {
                        return string.Empty;
                    }
                    if (this.MarkBet.ReductionRulesToApply.Count == 0
                        && !this.MarkBet.SingleRowBetMultiplier
                        && !this.MarkBet.V6SingleRows
                        && !this.MarkBet.ReductionV6BetMultiplierRule)
                    {
                        CreateSingleCoupon();
                    }
                    else if (this.MarkBet.GuaranteeReduction && this.MarkBet.NumberOfToleratedErrors > 0)
                    {
                        CreateSingleRowCoupons();
                    }
                    else if (this.MarkBet.SingleRowBetMultiplier
                        || (this.MarkBet.V6SingleRows && this.MarkBet.V6UpperBoundary > 0M)
                        || (this.MarkBet.ReductionV6BetMultiplierRule && this.MarkBet.V6BetMultiplierRuleList != null && this.MarkBet.V6BetMultiplierRuleList.Count > 0)
                        || this.MarkBet.BetMultiplierRowAddition
                        || this.MarkBet.SingleRowCollection.SingleRows.Any(sr => sr.Edited))
                    {
                        CreateCompressedCouponsV6BetMultiplier();
                    }
                    else if (this.MarkBet.CompressCoupons
                        && !this.MarkBet.SingleRowBetMultiplier)
                    {
                        CreateCompressedCoupons();
                    }
                    CreateStartnumberListsForCoupons();
                    HandleReserverForCoupons(MarkBet.ReservHandling);
                    break;
                case "DD":
                case "LD":
                    CreateDoubleCoupons();
                    break;
                case "TV":
                    CreateTvillingCoupons();
                    break;
                case "T":
                    CreateTrioCoupons();
                    break;
                default:
                    break;
            }

            return string.Empty;
        }

        public void CreateCompressedCoupons()
        {
            var couponList = new List<HPTCoupon>();
            this.MarkBet.CreateBetMultiplierList();

            foreach (var multiplier in this.MarkBet.BetMultiplierList)
            {
                couponList.AddRange(this.MarkBet.SingleRowCollection.CompressedCoupons
                    .OrderBy(sr => sr.CouponNumber)
                    .Select(rc => new HPTCoupon()
                    {
                        BetMultiplier = multiplier,
                        BetType = this.MarkBet.BetType.Code,
                        CouponId = rc.CouponNumber,
                        TrackCode = this.MarkBet.RaceDayInfo.TrackId.ToString(),
                        V6 = rc.V6,
                        SystemSize = rc.Size,
                        Date = this.MarkBet.RaceDayInfo.RaceDayDate,
                        CouponRaceList = new System.Collections.ObjectModel.ObservableCollection<HPTCouponRace>(
                            Enumerable.Range(1, rc.NumberOfRaces)
                            .Select(i => new HPTCouponRace()
                            {
                                LegNr = i,
                                HorseList = rc.GetCouponHorseList(i)
                            }))
                    })
                    .ToList());
            }

            int couponNumber = 1;
            foreach (var coupon in couponList)
            {
                coupon.CouponId = couponNumber++;
            }

            this.CouponList = new ObservableCollection<HPTCoupon>(couponList);
        }

        public void CreateCompressedCouponsV6BetMultiplier()
        {
            var couponList = new List<HPTCoupon>();

            couponList.AddRange(this.MarkBet.SingleRowCollection.CompressedCoupons
                    .OrderBy(sr => sr.CouponNumber)
                    .Select(rc => new HPTCoupon()
                    {
                        BetMultiplier = rc.BetMultiplier,
                        BetType = this.MarkBet.BetType.Code,
                        CouponId = rc.CouponNumber,
                        TrackCode = this.MarkBet.RaceDayInfo.TrackId.ToString(),
                        V6 = rc.V6,
                        SystemSize = rc.Size,
                        Date = this.MarkBet.RaceDayInfo.RaceDayDate,
                        CouponRaceList = new System.Collections.ObjectModel.ObservableCollection<HPTCouponRace>(
                            //Enumerable.Range(1, rc.UniqueCodes.Count)
                            Enumerable.Range(1, rc.NumberOfRaces)
                            .Select(i => new HPTCouponRace()
                            {
                                LegNr = i,
                                HorseList = rc.GetCouponHorseList(i)
                            }))
                    })
                .ToList());

            this.CouponList = new ObservableCollection<HPTCoupon>(couponList);
        }

        public void CreateSingleRowCoupons()
        {
            var couponList = new List<HPTCoupon>();
            int couponNumber = 1;

            foreach (var singleRow in this.MarkBet.SingleRowCollection.SingleRows)
            {
                foreach (var betMultiplier in singleRow.BetMultiplierList)
                {
                    var coupon = new HPTCoupon()
                    {
                        BetMultiplier = betMultiplier,
                        BetType = this.MarkBet.BetType.Code,
                        CouponId = couponNumber++,
                        TrackCode = this.MarkBet.RaceDayInfo.TrackId.ToString(),
                        V6 = singleRow.V6,
                        Date = this.MarkBet.RaceDayInfo.RaceDayDate,
                        SystemSize = 1,
                        CouponRaceList = new System.Collections.ObjectModel.ObservableCollection<HPTCouponRace>(
                            Enumerable.Range(0, singleRow.HorseList.Count())
                            .Select(i => new HPTCouponRace()
                            {
                                LegNr = i + 1,
                                HorseList = new List<HPTHorse>() { singleRow.HorseList[i] }
                            }))
                    };
                    couponList.Add(coupon);
                }
            }

            this.CouponList = new ObservableCollection<HPTCoupon>(couponList);
        }

        public void CreateSingleCoupon()
        {
            var couponList = new List<HPTCoupon>();

            this.MarkBet.CreateBetMultiplierList();
            int couponNumber = 1;
            foreach (var multiplier in this.MarkBet.BetMultiplierList)
            {
                var coupon = new HPTCoupon()
                {
                    BetMultiplier = multiplier,
                    BetType = this.MarkBet.BetType.Code,
                    CouponId = couponNumber++,
                    TrackCode = this.MarkBet.RaceDayInfo.TrackId.ToString(),
                    V6 = this.MarkBet.V6,
                    Date = this.MarkBet.RaceDayInfo.RaceDayDate,
                    SystemSize = this.MarkBet.SystemSize,
                    CouponRaceList = new System.Collections.ObjectModel.ObservableCollection<HPTCouponRace>(
                        Enumerable.Range(1, this.MarkBet.RaceDayInfo.RaceList.Count)
                        .Select(i => new HPTCouponRace()
                        {
                            LegNr = i,
                            HorseList = this.MarkBet.RaceDayInfo.RaceList.First(r => r.LegNr == i).HorseListSelected
                        }))
                };
                couponList.Add(coupon);
            }

            this.CouponList = new ObservableCollection<HPTCoupon>(couponList);
            this.TotalSystemSize = this.MarkBet.ReducedSize;
        }

        public void CreateDoubleCoupons()
        {
            IEnumerable<HPTCoupon> couponList = this.CombBet.RaceDayInfo.CombinationListInfoDouble.CombinationList
                .Where(c => c.Selected)
                .Select(c => new HPTCoupon()
                {
                    Stake = (int)c.Stake,
                    Date = c.ParentRaceDayInfo.RaceDayDate,
                    CouponRaceList = new ObservableCollection<HPTCouponRace>()
                    {
                        new HPTCouponRace()
                        {
                            HorseList = new List<HPTHorse>(){c.Horse1, c.Horse2}
                        }
                    }
                }).ToList();


            this.CouponList = new ObservableCollection<HPTCoupon>(couponList);
            SetCouponID();
        }

        public void CreateCombinationCoupons(HPTCombinationListInfo combinationListInfo)
        {
            IEnumerable<HPTCoupon> couponList = combinationListInfo.CombinationList
                .Where(c => c.Selected)
                .Select(c => new HPTCoupon()
                {
                    Stake = (int)c.Stake,
                    Date = c.ParentRaceDayInfo.RaceDayDate,
                    RaceNumber = c.Horse1.ParentRace.RaceNr,
                    TrackCode = c.ParentRaceDayInfo.TrackId.ToString(),
                    CouponRaceList = new ObservableCollection<HPTCouponRace>()
                    {
                        new HPTCouponRace()
                        {
                            HorseList = new List<HPTHorse>(){c.Horse1, c.Horse2, c.Horse3}
                        }
                    }
                }).ToList();

            SetCouponID(couponList);
            this.raceNumberString = "_Lopp" + combinationListInfo.CombinationList.First().Horse1.ParentRace.LegNr.ToString();
            CreateATGFile(couponList, this.ATGFile);
            this.raceNumberString = string.Empty;
        }

        public void CreateTvillingCoupons()
        {
            IEnumerable<HPTCoupon> couponList = this.CombBet.RaceDayInfo.RaceList
                .SelectMany(r => r.CombinationListInfoTvilling.CombinationList)
                .Where(c => c.Selected)
                .Select(c => new HPTCoupon()
                {
                    Stake = (int)c.Stake,
                    Date = c.ParentRaceDayInfo.RaceDayDate,
                    RaceNumber = c.ParentRace.RaceNr,
                    TrackCode = c.ParentRace.ParentRaceDayInfo.TrackId.ToString(),
                    CouponRaceList = new ObservableCollection<HPTCouponRace>()
                    {
                        new HPTCouponRace()
                        {
                            HorseList = new List<HPTHorse>(){c.Horse1, c.Horse2}
                        }
                    }
                }).ToList();

            this.CouponList = new ObservableCollection<HPTCoupon>(couponList);
            SetCouponID();
            CreateATGFile(this.CouponList, this.ATGFile);
        }

        public void CreateTvillingCoupons(HPTRace race)
        {
            IEnumerable<HPTCoupon> couponList = race.CombinationListInfoTvilling.CombinationList
                .Where(c => c.Selected)
                .Select(c => new HPTCoupon()
                {
                    Stake = (int)c.Stake,
                    Date = c.ParentRaceDayInfo.RaceDayDate,
                    RaceNumber = c.ParentRace.RaceNr,
                    TrackCode = c.ParentRace.ParentRaceDayInfo.TrackId.ToString(),
                    CouponRaceList = new ObservableCollection<HPTCouponRace>()
                    {
                        new HPTCouponRace()
                        {
                            HorseList = new List<HPTHorse>(){c.Horse1, c.Horse2}
                        }
                    }
                }).ToList();

            SetCouponID();
            this.raceNumberString = "_Lopp" + race.LegNr.ToString();
            CreateATGFile(couponList, this.ATGFile);
            this.raceNumberString = string.Empty;
        }

        public void CreateTrioCoupons()
        {
            IEnumerable<HPTCoupon> couponList = this.CombBet.RaceDayInfo.RaceList
                .SelectMany(r => r.CombinationListInfoTrio.CombinationList)
                .Where(c => c.Selected)
                .Select(c => new HPTCoupon()
                {
                    Stake = (int)c.Stake,
                    Date = c.ParentRaceDayInfo.RaceDayDate,
                    RaceNumber = c.ParentRace.RaceNr,
                    TrackCode = c.ParentRace.ParentRaceDayInfo.TrackId.ToString(),
                    CouponRaceList = new ObservableCollection<HPTCouponRace>()
                    {
                        new HPTCouponRace()
                        {
                            HorseList = new List<HPTHorse>(){c.Horse1, c.Horse2, c.Horse3}
                        }
                    }
                }).ToList();
            this.CouponList = new ObservableCollection<HPTCoupon>(couponList);
            SetCouponID();
            CreateATGFile(this.CouponList, this.ATGFile);
        }

        public void CreateTrioCoupons(HPTRace race)
        {
            IEnumerable<HPTCoupon> couponList = race.CombinationListInfoTrio.CombinationList
                .Where(c => c.Selected)
                .Select(c => new HPTCoupon()
                {
                    Stake = (int)c.Stake,
                    Date = c.ParentRaceDayInfo.RaceDayDate,
                    RaceNumber = c.ParentRace.RaceNr,
                    TrackCode = c.ParentRace.ParentRaceDayInfo.TrackId.ToString(),
                    CouponRaceList = new ObservableCollection<HPTCouponRace>()
                    {
                        new HPTCouponRace()
                        {
                            HorseList = new List<HPTHorse>(){c.Horse1, c.Horse2, c.Horse3}
                        }
                    }
                }).ToList();

            SetCouponID(couponList);
            this.raceNumberString = "_Lopp_" + race.LegNr.ToString();
            CreateATGFile(couponList, this.ATGFile);
            this.raceNumberString = string.Empty;
        }

        private void SetCouponID()
        {
            SetCouponID(this.CouponList);
        }

        private void SetCouponID(IEnumerable<HPTCoupon> couponList)
        {
            int couponID = 1;
            foreach (HPTCoupon coupon in couponList)
            {
                coupon.CouponId = couponID++;
            }
        }

        #endregion

        #region Correction

        private int totalSystemSize;
        public int TotalSystemSize
        {
            get
            {
                return this.totalSystemSize;
            }
            set
            {
                this.totalSystemSize = value;
                OnPropertyChanged("TotalSystemSize");
            }
        }

        private int totalNumberOfAllCorrect;
        public int TotalNumberOfAllCorrect
        {
            get
            {
                return this.totalNumberOfAllCorrect;
            }
            set
            {
                this.totalNumberOfAllCorrect = value;
                OnPropertyChanged("TotalNumberOfAllCorrect");
            }
        }

        //public int TotalNumberOfAllCorrect { get; set; }

        private int totalNumberOfOneError;
        public int TotalNumberOfOneError
        {
            get
            {
                return this.totalNumberOfOneError;
            }
            set
            {
                this.totalNumberOfOneError = value;
                OnPropertyChanged("TotalNumberOfOneError");
            }
        }

        //public int TotalNumberOfOneError { get; set; }

        private int totalNumberOfTwoErrors;
        public int TotalNumberOfTwoErrors
        {
            get
            {
                return this.totalNumberOfTwoErrors;
            }
            set
            {
                this.totalNumberOfTwoErrors = value;
                OnPropertyChanged("TotalNumberOfTwoErrors ");
            }
        }

        //public int totalNumberOfThreeErrors { get; set; }

        private int totalNumberOfThreeErrors;
        public int TotalNumberOfThreeErrors
        {
            get
            {
                return this.totalNumberOfThreeErrors;
            }
            set
            {
                this.totalNumberOfThreeErrors = value;
                OnPropertyChanged("TotalNumberOfThreeErrors ");
            }
        }

        private int totalWinnings;
        public int TotalWinnings
        {
            get
            {
                return this.totalWinnings;
            }
            set
            {
                this.totalWinnings = value;
                OnPropertyChanged("TotalWinnings");
            }
        }

        #endregion

        #region Hjälpmetoder

        public void CreateHorseListsForCoupons()
        {
            foreach (var coupon in this.CouponList)
            {
                foreach (var couponRace in coupon.CouponRaceList)
                {
                    couponRace.HorseList = new List<HPTHorse>();
                    var race = this.MarkBet.RaceDayInfo.RaceList.FirstOrDefault(r => r.LegNr == couponRace.LegNr);
                    if (race != null)
                    {
                        foreach (var startNr in couponRace.StartNrList)
                        {
                            var horse = race.HorseList.FirstOrDefault(h => h.StartNr == startNr);
                            if (horse != null)
                            {
                                couponRace.HorseList.Add(horse);
                            }
                        }
                    }
                }
            }
        }

        public void CreateStartnumberListsForCoupons()
        {
            foreach (var coupon in this.CouponList)
            {
                foreach (var couponRace in coupon.CouponRaceList)
                {
                    var race = this.MarkBet.RaceDayInfo.RaceList.FirstOrDefault(r => r.LegNr == couponRace.LegNr);
                    if (race != null)
                    {
                        couponRace.StartNrList = couponRace.HorseList.Select(h => h.StartNr).ToList();
                    }
                }
            }
        }

        internal List<HPTMarkBetSingleRow> CreateSingleRowsFromCoupons()
        {
            var singleRowList = this.CouponList
                .SelectMany(c => c.CreateSingleRows())
                .ToList();

            return singleRowList;
        }

        #endregion

        #region Reservhandling

        public void HandleReserverForCoupons(ReservHandling rh)
        {
            if (this.CouponList == null || this.CouponList.Count == 0)
            {
                return;
            }
            switch (rh)
            {
                case ReservHandling.None:
                    HandleReservDefault();
                    break;
                case ReservHandling.Own:
                    HandleReservNone();
                    break;
                case ReservHandling.MarksSelected:
                    HandleReservSelectedRank("StakeDistributionShare");
                    break;
                case ReservHandling.MarksNotSelected:
                    HandleReservNotSelectedRank("StakeDistributionShare");
                    break;
                case ReservHandling.OwnRankSelected:
                    HandleReservSelectedRank("RankOwn");
                    break;
                case ReservHandling.OwnRankNotSelected:
                    HandleReservNotSelectedRank("RankOwn");
                    break;
                case ReservHandling.RankMeanSelected:
                    HandleReservSelectedRankMean();
                    break;
                case ReservHandling.RankMeanNotSelected:
                    HandleReservNotSelectedRankMean();
                    break;
                case ReservHandling.OddsSelected:
                    HandleReservSelectedRank("VinnarOdds");
                    break;
                case ReservHandling.OddsNotSelected:
                    HandleReservNotSelectedRank("VinnarOdds");
                    break;
                case ReservHandling.NextRankSelected:
                    HandleReservSelectedNextRank();
                    break;
                case ReservHandling.NextRankNotSelected:
                    HandleReservNotSelectedRankNext();
                    break;
                default:
                    break;
            }
        }

        internal int[] ExchangeReservArrayWithOwnChoices(int[] reservsToChooseFrom, HPTRace race)
        {
            // Se till att lista med reservval innehåller 2 hästar
            if (reservsToChooseFrom.Length == 0)
            {
                reservsToChooseFrom = new int[2];

            }
            if (reservsToChooseFrom.Length == 1)
            {
                reservsToChooseFrom = new int[] { reservsToChooseFrom[0], 0 };

            }

            // Byt och flytta reserv 1 om det finns eget gjorda val
            if (reservsToChooseFrom.Contains(race.Reserv1Nr) && race.Reserv1Nr != 0)
            {
                if (reservsToChooseFrom[0] != race.Reserv1Nr)
                {
                    for (int i = 1; i < reservsToChooseFrom.Length; i++)
                    {
                        if (reservsToChooseFrom[i] == race.Reserv1Nr)
                        {
                            reservsToChooseFrom[i] = reservsToChooseFrom[0];
                        }
                    }
                    reservsToChooseFrom[0] = race.Reserv1Nr;
                }
                if (reservsToChooseFrom.Contains(race.Reserv2Nr) && reservsToChooseFrom.Length > 1)
                {
                    if (reservsToChooseFrom[1] != race.Reserv2Nr)
                    {
                        reservsToChooseFrom[1] = race.Reserv2Nr;
                    }
                }
            }
            else if (reservsToChooseFrom.Contains(race.Reserv2Nr) && race.Reserv2Nr != 0)
            {
                if (reservsToChooseFrom[0] != race.Reserv2Nr)
                {
                    reservsToChooseFrom[1] = reservsToChooseFrom[0];
                    reservsToChooseFrom[0] = race.Reserv2Nr;
                }
            }
            return reservsToChooseFrom;
        }

        internal int[] ExchangeReservArrayWithReservOrder(int[] reservsToChooseFrom, int[] couponHorses, HPTRace race)
        {
            int[] reservsFromReservOrder = race.HorseList
                .Where(h => h.Scratched == false || h.Scratched == null)
                .OrderByDescending(h => h.StakeDistribution)
                .Select(h => h.StartNr)
                .Except(couponHorses)
                .Except(reservsToChooseFrom.Where(nr => nr > 0))
                .ToArray();


            var reservsToReturn = new int[2];

            //int[] reservsFromReservOrder = race.ReservOrderList.Except(couponHorses).ToArray();
            if (reservsToChooseFrom[0] == 0 && reservsFromReservOrder.Length > 0)
            {
                reservsToReturn[0] = reservsFromReservOrder[0];
                if (reservsToChooseFrom[1] == 0 && reservsFromReservOrder.Length > 1)
                {
                    reservsToReturn[1] = reservsFromReservOrder[1];
                }
            }
            else if (reservsToChooseFrom[1] == 0 && reservsFromReservOrder.Length > 0)
            {
                reservsToReturn[1] = reservsFromReservOrder[0];
            }
            return reservsToReturn;
        }

        internal void HandleReservNone()
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                int[] scratchedHorses = race.HorseList
                    .Where(h => h.Scratched == true)
                    .Select(h => h.StartNr)
                    .ToArray();

                foreach (HPTCoupon coupon in this.CouponList)
                {
                    HPTCouponRace couponRace = coupon.CouponRaceList.First(cr => cr.LegNr == race.LegNr);
                    IEnumerable<int> couponStartNrList = couponRace.HorseList.Select(h => h.StartNr);
                    int[] reservsToChooseFrom = race.ReservOrderList
                        .Except(couponStartNrList)
                        .Except(scratchedHorses)
                        .ToArray();

                    if (couponRace.StartNrList.Count > 1)
                    {
                        if (race.Reserv1Nr != 0)
                        {
                            reservsToChooseFrom = reservsToChooseFrom.Concat(new int[1] { race.Reserv1Nr }).ToArray();
                        }
                        if (race.Reserv2Nr != 0)
                        {
                            reservsToChooseFrom = reservsToChooseFrom.Concat(new int[1] { race.Reserv2Nr }).ToArray();
                        }
                    }
                    //reservsToChooseFrom = reservsToChooseFrom.Concat(new int[2] { race.Reserv1Nr, race.Reserv2Nr }).ToArray();
                    ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);

                    if (reservsToChooseFrom.Length > 0)
                    {
                        couponRace.Reserv1 = reservsToChooseFrom[0];
                        if (reservsToChooseFrom.Length > 1)
                        {
                            couponRace.Reserv2 = reservsToChooseFrom[1];
                        }
                    }
                }
            }
        }

        internal void HandleReservOwn()
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                int[] reservsToChooseFrom = race.ReservOrderList.Except(race.HorseListSelected.Select(h => h.StartNr)).ToArray();
                ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);
                foreach (HPTCoupon coupon in this.CouponList)
                {
                    HPTCouponRace couponRace = coupon.CouponRaceList.First(cr => cr.LegNr == race.LegNr);
                    if (race.Reserv1Nr != 0)
                    {
                        couponRace.Reserv1 = race.Reserv1Nr;
                    }
                    if (race.Reserv2Nr != 0)
                    {
                        couponRace.Reserv2 = race.Reserv2Nr;
                    }

                    if (couponRace.Reserv1 == 0 && reservsToChooseFrom.Length > 0)
                    {
                        couponRace.Reserv1 = reservsToChooseFrom[0];
                    }
                    if (couponRace.Reserv2 == 0 && reservsToChooseFrom.Length > 1)
                    {
                        couponRace.Reserv2 = reservsToChooseFrom[1];
                    }
                }
            }
        }

        internal void HandleReservNotSelectedRank(string rankVariableName)
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                var reservsToChooseFrom = race.HorseList
                    .Where(h => !h.Selected && h.Scratched != true)
                    .OrderBy(h => h.RankList.First(hr => hr.Name == rankVariableName).Rank)
                    //.OrderByDescending(h => h.RankList.First(hr => hr.Name == rankVariableName).Rank)
                    .Select(h => h.StartNr)
                    .ToArray();

                reservsToChooseFrom = ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);
                if (reservsToChooseFrom[0] == 0 || reservsToChooseFrom[1] == 0)
                {
                    var selectedStartNumbers = race.HorseListSelected.Select(h => h.StartNr).ToArray();
                    reservsToChooseFrom = ExchangeReservArrayWithReservOrder(reservsToChooseFrom, selectedStartNumbers, race);
                }

                foreach (HPTCoupon coupon in this.CouponList)
                {
                    HPTCouponRace couponRace = coupon.CouponRaceList.First(cr => cr.LegNr == race.LegNr);
                    if (reservsToChooseFrom[0] == 0 || reservsToChooseFrom[1] == 0)
                    {
                        var reservsToChooseFromOnCoupon = ExchangeReservArrayWithReservOrder(reservsToChooseFrom, couponRace.StartNrList.ToArray(), race);
                        couponRace.Reserv1 = reservsToChooseFromOnCoupon[0];
                        couponRace.Reserv2 = reservsToChooseFromOnCoupon[1];
                    }
                    else
                    {
                        couponRace.Reserv1 = reservsToChooseFrom[0];
                        couponRace.Reserv2 = reservsToChooseFrom[1];
                    }
                }
            }
        }

        internal void HandleReservSelectedRank(string rankVariableName)
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                IEnumerable<string> uniqueCodeList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr)
                    .Select(hcr => hcr.UniqueCode)
                    .Distinct();

                foreach (string uniqueCode in uniqueCodeList)
                {
                    IEnumerable<HPTCouponRace> couponRaceList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr && hcr.UniqueCode == uniqueCode);

                    var firstHorseList = couponRaceList.First().HorseList;

                    var reservsToChooseFrom = race.HorseListSelected
                        .Except(firstHorseList)
                        .OrderBy(h => h.RankList.First(hr => hr.Name == rankVariableName).Rank)
                        .Select(h => h.StartNr)
                        .ToArray();

                    if (firstHorseList.Count > 1)
                    {
                        reservsToChooseFrom = reservsToChooseFrom.Concat(firstHorseList.Select(h => h.StartNr)).ToArray();
                    }

                    var horsesToConcat = race.HorseList
                        .Except(firstHorseList)
                        .OrderBy(h => h.RankList.First(hr => hr.Name == rankVariableName).Rank)
                        .Select(h => h.StartNr)
                        .Except(reservsToChooseFrom)
                        .ToArray();

                    reservsToChooseFrom = reservsToChooseFrom.Concat(horsesToConcat).ToArray();

                    reservsToChooseFrom = ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);
                    if (reservsToChooseFrom[0] == 0 || reservsToChooseFrom[1] == 0)
                    {
                        var selectedStartNumbers = race.HorseListSelected.Select(h => h.StartNr).ToArray();
                        reservsToChooseFrom = ExchangeReservArrayWithReservOrder(reservsToChooseFrom, selectedStartNumbers, race);
                    }

                    foreach (HPTCouponRace couponRace in couponRaceList)
                    {
                        couponRace.Reserv1 = reservsToChooseFrom[0];
                        couponRace.Reserv2 = reservsToChooseFrom[1];
                    }
                }
            }
        }

        internal void HandleReservDefault()
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                var orderedHorses = race.HorseList
                    .Where(h => h.Scratched != true)
                    .OrderByDescending(h => h.StakeDistribution)
                    //.Select(h => h.StartNr)
                    .ToArray();

                var reservsToChooseFrom = orderedHorses
                    .Where(h => !h.Selected)
                    .OrderByDescending(h => h.StakeDistribution)
                    .Concat(orderedHorses)
                    .Select(h => h.StartNr)
                    .ToArray();

                foreach (var coupon in this.CouponList)
                {
                    var couponRace = coupon.CouponRaceList.First(cr => cr.LegNr == race.LegNr);
                    var reservsToChooseFromOnCoupon = reservsToChooseFrom
                        .Except(couponRace.StartNrList)
                        .Select(nr => nr)
                        .ToArray();

                    switch (reservsToChooseFromOnCoupon.Length)
                    {
                        case 0:
                            couponRace.Reserv1 = reservsToChooseFrom[0];
                            couponRace.Reserv2 = reservsToChooseFrom[1];
                            break;
                        case 1:
                            couponRace.Reserv1 = reservsToChooseFromOnCoupon[0];
                            couponRace.Reserv2 = reservsToChooseFrom.First(rn => rn != couponRace.Reserv1);
                            break;
                        default:
                            couponRace.Reserv1 = reservsToChooseFromOnCoupon[0];
                            couponRace.Reserv2 = reservsToChooseFromOnCoupon[1];
                            break;
                    }
                }
            }
        }

        internal void HandleReservNotSelectedRankMean()
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                var reservsToChooseFrom = race.HorseList
                    .Where(h => !h.Selected && h.Scratched != true)
                    .OrderBy(h => h.RankWeighted)
                    .Select(h => h.StartNr)
                    .ToArray();

                reservsToChooseFrom = ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);
                if (reservsToChooseFrom[0] == 0 || reservsToChooseFrom[1] == 0)
                {
                    var selectedStartNumbers = race.HorseListSelected.Select(h => h.StartNr).ToArray();
                    reservsToChooseFrom = ExchangeReservArrayWithReservOrder(reservsToChooseFrom, selectedStartNumbers, race);
                }

                foreach (HPTCoupon coupon in this.CouponList)
                {
                    HPTCouponRace couponRace = coupon.CouponRaceList.First(cr => cr.LegNr == race.LegNr);
                    couponRace.Reserv1 = reservsToChooseFrom[0];
                    couponRace.Reserv2 = reservsToChooseFrom[1];
                }
            }
        }

        internal void HandleReservSelectedRankMean()
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                IEnumerable<string> uniqueCodeList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr)
                    .Select(hcr => hcr.UniqueCode)
                    .Distinct();

                foreach (string uniqueCode in uniqueCodeList)
                {
                    IEnumerable<HPTCouponRace> couponRaceList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr && hcr.UniqueCode == uniqueCode);

                    var firstHorseList = couponRaceList.First().HorseList;

                    var reservsToChooseFrom = race.HorseListSelected
                        .Except(firstHorseList)
                        .OrderBy(h => h.RankWeighted)
                        .Select(h => h.StartNr)
                        .ToArray();

                    if (firstHorseList.Count > 1)
                    {
                        reservsToChooseFrom = reservsToChooseFrom.Concat(firstHorseList.Select(h => h.StartNr)).ToArray();
                    }

                    reservsToChooseFrom = ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);
                    if (reservsToChooseFrom[0] == 0 || reservsToChooseFrom[1] == 0)
                    {
                        var selectedStartNumbers = race.HorseListSelected.Select(h => h.StartNr).ToArray();
                        reservsToChooseFrom = ExchangeReservArrayWithReservOrder(reservsToChooseFrom, selectedStartNumbers, race);
                    }

                    foreach (HPTCouponRace couponRace in couponRaceList)
                    {
                        couponRace.Reserv1 = reservsToChooseFrom[0];
                        couponRace.Reserv2 = reservsToChooseFrom[1];
                    }
                }
            }
        }

        internal void HandleReservNotSelectedRankNext()
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                IEnumerable<string> uniqueCodeList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr)
                    .Select(hcr => hcr.UniqueCode)
                    .Distinct();

                foreach (string uniqueCode in uniqueCodeList)
                {
                    IEnumerable<HPTCouponRace> couponRaceList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr && hcr.UniqueCode == uniqueCode);

                    var firstHorseList = couponRaceList.First().HorseList;
                    int maxRankOwn = firstHorseList.Max(h => h.RankOwn);

                    var reservsToChooseFrom = race.HorseList
                        .Except(firstHorseList)
                        .Where(h => h.RankOwn > maxRankOwn)
                        .OrderBy(h => h.RankOwn)
                        .Select(h => h.StartNr)
                        .ToArray();

                    var reservsToChooseFromStep2 = race.HorseListSelected
                        .Except(firstHorseList)
                        .Where(h => h.RankOwn <= maxRankOwn)
                        .OrderBy(h => h.RankOwn)
                        .Select(h => h.StartNr)
                        .ToArray();

                    reservsToChooseFrom = reservsToChooseFrom.Concat(reservsToChooseFromStep2).ToArray();

                    if (firstHorseList.Count > 1)
                    {
                        reservsToChooseFrom = reservsToChooseFrom
                            .Concat(firstHorseList.Select(h => h.StartNr))
                            .ToArray();
                    }

                    reservsToChooseFrom = ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);
                    if (reservsToChooseFrom[0] == 0 || reservsToChooseFrom[1] == 0)
                    {
                        var selectedStartNumbers = race.HorseListSelected.Select(h => h.StartNr).ToArray();
                        reservsToChooseFrom = ExchangeReservArrayWithReservOrder(reservsToChooseFrom, selectedStartNumbers, race);
                    }

                    foreach (HPTCouponRace couponRace in couponRaceList)
                    {
                        couponRace.Reserv1 = reservsToChooseFrom[0];
                        couponRace.Reserv2 = reservsToChooseFrom[1];
                    }
                }
            }
        }

        internal void HandleReservSelectedNextRank()
        {
            foreach (HPTRace race in this.MarkBet.RaceDayInfo.RaceList)
            {
                IEnumerable<string> uniqueCodeList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr)
                    .Select(hcr => hcr.UniqueCode)
                    .Distinct();

                foreach (string uniqueCode in uniqueCodeList)
                {
                    IEnumerable<HPTCouponRace> couponRaceList = this.CouponList
                    .SelectMany(hc => hc.CouponRaceList)
                    .Where(hcr => hcr.LegNr == race.LegNr && hcr.UniqueCode == uniqueCode);

                    var firstHorseList = couponRaceList.First().HorseList;
                    int maxRankOwn = firstHorseList.Max(h => h.RankOwn);

                    var reservsToChooseFrom = race.HorseListSelected
                        .Except(firstHorseList)
                        .Where(h => h.RankOwn > maxRankOwn)
                        .OrderBy(h => h.RankOwn)
                        .Select(h => h.StartNr)
                        .ToArray();

                    var reservsToChooseFromStep2 = race.HorseListSelected
                        .Except(firstHorseList)
                        .Where(h => h.RankOwn <= maxRankOwn)
                        .OrderBy(h => h.RankOwn)
                        .Select(h => h.StartNr)
                        .ToArray();

                    reservsToChooseFrom = reservsToChooseFrom.Concat(reservsToChooseFromStep2).ToArray();
                    reservsToChooseFrom = reservsToChooseFrom.Concat(new int[2] { race.Reserv1Nr, race.Reserv2Nr }).ToArray();

                    if (firstHorseList.Count > 1)
                    {
                        reservsToChooseFrom = reservsToChooseFrom
                            .Concat(firstHorseList.Select(h => h.StartNr))
                            .ToArray();
                    }

                    reservsToChooseFrom = ExchangeReservArrayWithOwnChoices(reservsToChooseFrom, race);
                    if (reservsToChooseFrom[0] == 0 || reservsToChooseFrom[1] == 0)
                    {
                        var selectedStartNumbers = race.HorseListSelected.Select(h => h.StartNr).ToArray();
                        reservsToChooseFrom = ExchangeReservArrayWithReservOrder(reservsToChooseFrom, selectedStartNumbers, race);
                    }

                    foreach (HPTCouponRace couponRace in couponRaceList)
                    {
                        couponRace.Reserv1 = reservsToChooseFrom[0];
                        couponRace.Reserv2 = reservsToChooseFrom[1];
                    }
                }
            }
        }

        #endregion

        public string ToCouponsString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Kuponger");
            sb.AppendLine();
            foreach (var hptCoupon in CouponList.OrderBy(c => c.CouponId))
            {
                sb.AppendLine(hptCoupon.ToCouponString());
            }
            return sb.ToString();
        }

        #region Konvertera ATG-fil till lista med HPTCoupon

        public ObservableCollection<HPTCoupon> CreateHPTCouponsFromATGFile(issuer atgFile)
        {
            IEnumerable<HPTCoupon> couponInfo = null;
            if (atgFile.betcoupons.v4Coupon != null)
            {
                couponInfo = atgFile.betcoupons.v4Coupon
                    .Select(vc => new HPTCoupon()
                    {
                        BetMultiplier = Convert.ToInt32(vc.betmultiplier),
                        BetType = "V4",
                        CouponId = Convert.ToInt32(vc.couponid),
                        CouponRaceList = ConvertLeg20ListToCouponRaceList(vc.leg),
                        Date = vc.date,
                        TrackCode = vc.trackcode
                    });
            }
            else if (atgFile.betcoupons.v5Coupon != null)
            {
                couponInfo = atgFile.betcoupons.v5Coupon
                    .Select(vc => new HPTCoupon()
                    {
                        BetMultiplier = Convert.ToInt32(vc.betmultiplier),
                        BetType = "V5",
                        CouponId = Convert.ToInt32(vc.couponid),
                        CouponRaceList = ConvertLegListToCouponRaceList(vc.leg),
                        Date = vc.date,
                        TrackCode = vc.trackcode
                    });
            }
            else if (atgFile.betcoupons.v64Coupon != null)
            {
                couponInfo = atgFile.betcoupons.v64Coupon
                    .Select(vc => new HPTCoupon()
                    {
                        BetMultiplier = Convert.ToInt32(vc.betmultiplier),
                        BetType = "V64",
                        CouponId = Convert.ToInt32(vc.couponid),
                        CouponRaceList = ConvertLegListToCouponRaceList(vc.leg),
                        Date = vc.date,
                        V6 = vc.v6
                    });
            }
            else if (atgFile.betcoupons.v65Coupon != null)
            {
                couponInfo = atgFile.betcoupons.v65Coupon
                    .Select(vc => new HPTCoupon()
                    {
                        BetMultiplier = Convert.ToInt32(vc.betmultiplier),
                        BetType = "V65",
                        CouponId = Convert.ToInt32(vc.couponid),
                        CouponRaceList = ConvertLegListToCouponRaceList(vc.leg),
                        Date = vc.date,
                        V6 = vc.v6
                    });
            }
            else if (atgFile.betcoupons.v75Coupon != null)
            {
                couponInfo = atgFile.betcoupons.v75Coupon
                    .Select(vc => new HPTCoupon()
                    {
                        BetMultiplier = Convert.ToInt32(vc.betmultiplier),
                        BetType = "V75",
                        CouponId = Convert.ToInt32(vc.couponid),
                        CouponRaceList = ConvertLegListToCouponRaceList(vc.leg),
                        Date = vc.date,
                        V6 = vc.v7
                    });
            }
            else if (atgFile.betcoupons.v85Coupon != null)    // TODO
            {
                couponInfo = atgFile.betcoupons.v75Coupon
                    .Select(vc => new HPTCoupon()
                    {
                        BetMultiplier = Convert.ToInt32(vc.betmultiplier),
                        BetType = "V85",
                        CouponId = Convert.ToInt32(vc.couponid),
                        CouponRaceList = ConvertLegListToCouponRaceList(vc.leg),
                        Date = vc.date,
                        V6 = vc.v7
                    });
            }
            else if (atgFile.betcoupons.v86Coupon != null)
            {
                couponInfo = atgFile.betcoupons.v86Coupon
                    .Select(vc => new HPTCoupon()
                    {
                        BetMultiplier = Convert.ToInt32(vc.betmultiplier),
                        BetType = "V86",
                        CouponId = Convert.ToInt32(vc.couponid),
                        CouponRaceList = ConvertLegListToCouponRaceList(vc.leg),
                        Date = vc.date,
                        TrackCode = vc.trackcode,
                        V6 = vc.v8
                    });
            }
            else
            {
                return null;
            }

            return new ObservableCollection<HPTCoupon>(couponInfo);
        }

        internal ObservableCollection<HPTCouponRace> ConvertLegListToCouponRaceList(legType[] legList)
        {
            var couponRaceList = legList.Select(l => ConvertLegToCouponRace(l));
            return new ObservableCollection<HPTCouponRace>(couponRaceList);
        }

        internal ObservableCollection<HPTCouponRace> ConvertLeg20ListToCouponRaceList(legType20[] legList)
        {
            var couponRaceList = legList.Select(l => ConvertLeg20ToCouponRace(l));
            return new ObservableCollection<HPTCouponRace>(couponRaceList);
        }

        internal HPTCouponRace ConvertLegToCouponRace(legType leg)
        {
            var couponRace = new HPTCouponRace()
            {
                LegNr = Convert.ToInt32(leg.legno),
                Reserv1 = string.IsNullOrEmpty(leg.r1) ? 0 : Convert.ToInt32(leg.r1),
                Reserv2 = string.IsNullOrEmpty(leg.r2) ? 0 : Convert.ToInt32(leg.r2),
                StartNrList = ConvertMarksToStartNrList(leg.marks)
            };

            return couponRace;
        }

        internal HPTCouponRace ConvertLeg20ToCouponRace(legType20 leg)
        {
            var couponRace = new HPTCouponRace()
            {
                LegNr = Convert.ToInt32(leg.legno),
                Reserv1 = string.IsNullOrEmpty(leg.r1) ? 0 : Convert.ToInt32(leg.r1),
                Reserv2 = string.IsNullOrEmpty(leg.r2) ? 0 : Convert.ToInt32(leg.r2),
                StartNrList = ConvertMarksToStartNrList(leg.marks)
            };

            return couponRace;
        }

        internal List<int> ConvertMarksToStartNrList(string marks)
        {
            var startNrList = new List<int>();
            var marksAsChar = marks.ToCharArray();
            for (int i = 0; i < marksAsChar.Length; i++)
            {
                if (marksAsChar[i] == '1')
                {
                    startNrList.Add(i + 1); // 0-baserad array, 1-baserad startlista...
                }
            }
            return startNrList;
        }

        #endregion
    }
}
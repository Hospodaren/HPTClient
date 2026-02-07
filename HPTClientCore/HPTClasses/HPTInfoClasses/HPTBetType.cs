using ATGDownloader;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTBetType : Notifier
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string AtgId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public int TrackId { get; set; }

        //[XmlIgnore]
        public ATGGameInfoBase GameInfoBase { get; set; }

        public DateTime NextTime
        {
            get
            {
                return StartTime > DateTime.Now ? StartTime : EndTime;
            }
        }

        [DataMember]
        public bool Jackpot { get; set; }

        public string InfoText
        {
            get
            {
                if (Jackpot)
                {
                    return "JACKPOTT";
                }
                return null;
            }
        }

        internal void SetCalendarRacaDayInfoBrush()
        {
            var c = Colors.Transparent;
            if (StartTime.Date == DateTime.Now.Date)
            {
                if (StartTime > DateTime.Now)
                {
                    c = Colors.LightGreen;
                }
                else if (EndTime < DateTime.Now)
                {
                    c = Colors.IndianRed;
                }
                else
                {
                    c = Colors.LightYellow;
                }
            }
            else if (EndTime < DateTime.Now)
            {
                c = Colors.IndianRed;
            }

            CalendarRacaDayInfoBrush = new SolidColorBrush(c);
        }

        private Brush calendarRacaDayInfoBrush;
        public Brush CalendarRacaDayInfoBrush
        {
            get
            {
                if (calendarRacaDayInfoBrush == null)
                {
                    SetCalendarRacaDayInfoBrush();
                }
                return calendarRacaDayInfoBrush;
            }
            set
            {
                calendarRacaDayInfoBrush = value;
                OnPropertyChanged("CalendarRacaDayInfoBrush");
            }
        }

        //[DataMember]
        public HPTRaceDayInfo CalendarRaceDayInfo { get; set; }

        private ImageSource betTypeATGLogo;
        [XmlIgnore]
        public ImageSource BetTypeATGLogo
        {
            get
            {
                if (betTypeATGLogo == null)
                {
                    string folder = IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
                    betTypeATGLogo = GetBetTypeATGLogo();  // new BitmapImage(new Uri(folder + this.Code + "Small.png", UriKind.Relative));
                }
                return betTypeATGLogo;
            }
            set
            {
                betTypeATGLogo = value;
                OnPropertyChanged("BetTypeATGLogo");
            }
        }

        public ImageSource GetBetTypeATGLogo()
        {
            string folder = IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
            return new BitmapImage(new Uri(folder + Code + "Small.png", UriKind.Relative));
        }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public int[] RaceNumberList { get; set; }

        public bool IsMarksGame
        {
            get
            {
                return Code == "V4" || Code == "V5" || Code == "V64" || Code == "V65" || Code == "V75" || Code == "V86" || Code == "GS75" || Code == "V85";
            }
        }

        public bool HasMultiplePools
        {
            get
            {
                return Code == "GS75" || Code == "V64" || Code == "V65" || Code == "V75" || Code == "V86" || Code == "V85";
            }
        }

        private decimal poolShare;
        [XmlIgnore]
        public decimal PoolShare
        {
            get
            {
                if (poolShare == 0m)
                {
                    switch (Code)
                    {
                        case "V3":
                        case "V4":
                        case "DD":
                        case "LD":
                            poolShare = 0.75m;
                            break;
                        case "V5":
                            poolShare = 0.65m;
                            break;
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            poolShare = 0.26m;
                            break;
                        case "V85":
                            poolShare = 0.2275m;
                            break;
                        case "V65":
                            poolShare = 0.325m;
                            break;
                        case "T":
                            poolShare = 0.7m;
                            break;
                        case "V":
                        case "P":
                        case "TV":
                            poolShare = 0.8m;
                            break;
                        default:
                            poolShare = 1m;
                            break;
                    }
                }
                return poolShare;
            }
        }

        private decimal gamblerReturnPercentage;
        [XmlIgnore]
        public decimal GamblerReturnPercentage
        {
            get
            {
                if (gamblerReturnPercentage == 0m)
                {
                    switch (Code)
                    {
                        case "V3":
                        case "V4":
                        case "DD":
                        case "LD":
                            gamblerReturnPercentage = 0.75m;
                            break;
                        case "V5":
                        case "V64":
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                        case "V65":
                            gamblerReturnPercentage = 0.65m;
                            break;
                        case "T":
                            gamblerReturnPercentage = 0.7m;
                            break;
                        case "V":
                        case "P":
                        case "TV":
                            gamblerReturnPercentage = 0.8m;
                            break;
                        default:
                            gamblerReturnPercentage = 1m;
                            break;
                    }
                }
                return gamblerReturnPercentage;
            }
        }

        private decimal poolShareOneError;
        [XmlIgnore]
        public decimal PoolShareOneError
        {
            get
            {
                if (poolShareOneError == 0m)
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            poolShareOneError = 0.13m;
                            break;
                        case "V85":
                            poolShareOneError = 0.0975m;
                            break;
                        case "V65":
                            poolShareOneError = 0.325m;
                            break;
                        default:
                            poolShareOneError = 0m;
                            break;
                    }
                }
                return poolShareOneError;
            }
        }

        private decimal poolShareTwoErrors;
        [XmlIgnore]
        public decimal PoolShareTwoErrors
        {
            get
            {
                if (poolShareTwoErrors == 0m)
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            poolShareTwoErrors = 0.26m;
                            break;
                        case "V85":
                            poolShareTwoErrors = 0.0975m;
                            break;
                        default:
                            poolShareTwoErrors = 0m;
                            break;
                    }
                }
                return poolShareTwoErrors;
            }
        }

        private decimal poolShareThreeErrors;
        [XmlIgnore]
        public decimal PoolShareThreeErrors
        {
            get
            {
                if (poolShareThreeErrors == 0m)
                {
                    switch (Code)
                    {
                        case "V85":
                            poolShareThreeErrors = 0.2275m;
                            break;
                        default:
                            poolShareThreeErrors = 0m;
                            break;
                    }
                }
                return poolShareThreeErrors;
            }
        }

        private decimal v6Factor;
        [XmlIgnore]
        public decimal V6Factor
        {
            get
            {
                if (v6Factor == 0m)
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            v6Factor = 2.5m;
                            break;
                        case "V65":
                            v6Factor = 2.0m;
                            break;
                        default:
                            v6Factor = 1m;
                            break;
                    }
                }
                return v6Factor;
            }
        }

        private string v6String;
        [XmlIgnore]
        public string V6String
        {
            get
            {
                if (string.IsNullOrEmpty(v6String))
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V65":
                            v6String = "V6";
                            break;
                        case "V75":
                        case "GS75":
                            v6String = "V7";
                            break;
                        case "V86":
                            v6String = "V8";
                            break;
                        default:
                            v6String = string.Empty;
                            break;
                    }
                }
                return v6String;
            }
        }

        public HPTRowValueInterval RowValueIntervalSingleWinner { get; set; }

        private ObservableCollection<HPTRowValueInterval> rowValueIntervalList;
        [XmlIgnore]
        public ObservableCollection<HPTRowValueInterval> RowValueIntervalList
        {
            get
            {
                if (rowValueIntervalList == null)
                {
                    switch (Code)
                    {
                        case "V75":
                        case "GS75":
                        case "V86":
                        case "V85":
                            rowValueIntervalList = new ObservableCollection<HPTRowValueInterval>
                            {
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = null,
                                    UpperLimit = 1000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 1000,
                                    UpperLimit = 5000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 5000,
                                    UpperLimit = 10000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 10000,
                                    UpperLimit = 50000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 50000,
                                    UpperLimit = 100000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 100000,
                                    UpperLimit = 500000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 500000,
                                    UpperLimit = 1000000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 1000000,
                                    UpperLimit = null
                                }
                            };
                            break;
                        case "V4":
                        case "V5":
                            rowValueIntervalList = new ObservableCollection<HPTRowValueInterval>
                            {
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = null,
                                    UpperLimit = 300
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 300,
                                    UpperLimit = 1500
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 1500,
                                    UpperLimit = 3000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 3000,
                                    UpperLimit = 15000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 15000,
                                    UpperLimit = 30000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 30000,
                                    UpperLimit = 150000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 150000,
                                    UpperLimit = 300000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 300000,
                                    UpperLimit = null
                                }
                            };
                            break;
                        case "V64":
                        case "V65":
                            rowValueIntervalList = new ObservableCollection<HPTRowValueInterval>
                            {
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = null,
                                    UpperLimit = 500
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 500,
                                    UpperLimit = 2500
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 2500,
                                    UpperLimit = 5000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 5000,
                                    UpperLimit = 25000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 25000,
                                    UpperLimit = 50000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 50000,
                                    UpperLimit = 250000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 250000,
                                    UpperLimit = 500000
                                },
                                new HPTRowValueInterval()
                                {
                                    LowerLimit = 500000,
                                    UpperLimit = null
                                }
                            };
                            break;
                        default:
                            break;
                    }
                }
                return rowValueIntervalList;
            }
        }


        private HPTRowValuePercentile[] rowValuePercentileList;
        [XmlIgnore]
        public HPTRowValuePercentile[] RowValuePercentileList
        {
            get
            {
                if (rowValuePercentileList == null)
                {
                    rowValuePercentileList = new HPTRowValuePercentile[]
                            {
                                new HPTRowValuePercentile()
                                {
                                    Percentile = 0.00M,
                                    Description = "Min"
                                },
                                new HPTRowValuePercentile()
                                {
                                    Percentile = 0.25M,
                                    Description = "Undre kvartil"
                                },
                                new HPTRowValuePercentile()
                                {
                                    Percentile = 0.50M,
                                    Description = "Median"
                                },
                                new HPTRowValuePercentile()
                                {
                                    Percentile = 0.75M,
                                    Description = "Övre kvartil"
                                },
                                new HPTRowValuePercentile()
                                {
                                    Percentile = 1.00M,
                                    Description = "Max"
                                }
                            };
                }
                return rowValuePercentileList;
            }
        }

        private HPTPayOut[] payOutDummyList;
        [XmlIgnore]
        public HPTPayOut[] PayOutDummyList
        {
            get
            {
                if (payOutDummyList == null)
                {
                    switch (Code)
                    {
                        case "V75":
                        case "GS75":
                            payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 7
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 6
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 5
                                }
                            };
                            break;
                        case "V86":
                            payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 8
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 7
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 6
                                }
                            };
                            break;
                        case "V85":
                            payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 8
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 7
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 6
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 5
                                }
                            };
                            break;
                        case "V5":
                            payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 5
                                }
                            };
                            break;
                        case "V4":
                            payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 4
                                }
                            };
                            break;
                        case "V64":
                            payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 6
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 5
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 4
                                }
                            };
                            break;
                        case "V65":
                            payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 6
                                },
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 5
                                }
                            };
                            break;
                        default:
                            break;
                    }
                }
                return payOutDummyList;
            }
        }

        private int[] betMultiplierList;
        [XmlIgnore]
        public int[] BetMultiplierList
        {
            get
            {
                if (betMultiplierList == null || betMultiplierList.Count() == 0)
                {
                    switch (Code)
                    {
                        case "V75":
                        case "GS75":
                        case "V86":
                        case "V85":
                        case "V5":
                            betMultiplierList = new int[] { 1, 2, 5, 10, 20, 50, 100 };
                            break;
                        case "V4":
                        case "V64":
                        case "V65":
                            betMultiplierList = new int[] { 1, 2, 3, 4, 5, 10, 50, 100 };
                            break;
                        default:
                            betMultiplierList = new int[] { 1 };
                            break;
                    }
                }
                return betMultiplierList;
            }
        }

        private int lowestStake;
        [XmlIgnore]
        public int LowestStake
        {
            get
            {
                if (lowestStake == 0)
                {
                    switch (Code)
                    {
                        case "DD":
                        case "LD":
                        case "TV":
                            lowestStake = 5;
                            break;
                        case "T":
                            lowestStake = 2;
                            break;
                        default:
                            lowestStake = 1;
                            break;
                    }
                }
                return lowestStake;
            }
        }

        private int highestStake;
        [XmlIgnore]
        public int HighestStake
        {
            get
            {
                if (highestStake == 0)
                {
                    switch (Code)
                    {
                        case "DD":
                        case "LD":
                        case "TV":
                            highestStake = 1000;
                            break;
                        case "T":
                            highestStake = 500;
                            break;
                        default:
                            highestStake = 1;
                            break;
                    }
                }
                return highestStake;
            }
        }

        private decimal rowCost;
        [XmlIgnore]
        public decimal RowCost
        {
            get
            {
                if (rowCost == 0M)
                {
                    switch (Code)
                    {
                        case "V3":
                            rowCost = 10m;
                            break;
                        case "V4":
                            rowCost = 2m;
                            break;
                        case "V5":
                        case "V64":
                        case "V65":
                        case "GS75":
                            rowCost = 1m;
                            break;
                        case "V75":
                        case "V85":
                            rowCost = 0.5m;
                            break;
                        case "V86":
                            rowCost = 0.25m;
                            break;
                        default:
                            rowCost = 0m;
                            break;
                    }
                }
                return rowCost;
            }
        }

        [XmlIgnore]
        public int NumberOfUploadedSystems { get; set; }

        private int jackpotLimit;
        [XmlIgnore]
        public int JackpotLimit
        {
            get
            {
                if (jackpotLimit == 0)
                {
                    switch (Code)
                    {
                        case "V65":
                            jackpotLimit = 20;
                            break;
                        case "V64":
                            jackpotLimit = 7;
                            break;
                        case "V75":
                        case "GS75":
                        case "V86":
                            jackpotLimit = 15;
                            break;
                        case "V85":
                            jackpotLimit = 5;
                            break;
                        case "DD":
                        case "LD":
                        case "V3":
                        case "V4":
                        case "V5":
                        case "TV":
                        case "T":
                            jackpotLimit = 0;
                            break;
                        default:
                            return 0;
                    }
                }
                return jackpotLimit;
            }
        }

        private int numberOfRaces;
        [XmlIgnore]
        public int NumberOfRaces
        {
            get
            {
                if (numberOfRaces == 0m)
                {
                    switch (Code)
                    {
                        case "V3":
                            numberOfRaces = 3;
                            break;
                        case "V4":
                            numberOfRaces = 4;
                            break;
                        case "V5":
                            numberOfRaces = 5;
                            break;
                        case "V65":
                        case "V64":
                            numberOfRaces = 6;
                            break;
                        case "V75":
                        case "GS75":
                            numberOfRaces = 7;
                            break;
                        case "V86":
                        case "V85":
                            numberOfRaces = 8;
                            break;
                        case "DD":
                        case "LD":
                            numberOfRaces = 2;
                            break;
                        case "TV":
                        case "T":
                            numberOfRaces = 1;
                            break;
                        default:
                            return 0;
                    }
                }
                return numberOfRaces;
            }
        }

        private int maxNumberOfSystemsInFile;
        [XmlIgnore]
        public int MaxNumberOfSystemsInFile
        {
            get
            {
                if (maxNumberOfSystemsInFile == 0m)
                {
                    switch (Code)
                    {
                        case "V4":
                        case "V5":
                            maxNumberOfSystemsInFile = 500;
                            break;
                        case "V65":
                        case "V64":
                            maxNumberOfSystemsInFile = 2000;
                            break;
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                            maxNumberOfSystemsInFile = 5000;
                            break;
                        default:
                            maxNumberOfSystemsInFile = 9999;
                            break;
                    }
                }
                return maxNumberOfSystemsInFile;
            }
        }

        private BetTypeCategory typeCategory;
        [XmlIgnore]
        public BetTypeCategory TypeCategory
        {
            get
            {
                if (typeCategory == BetTypeCategory.None)
                {
                    switch (Code)
                    {
                        //case "V3":
                        //    this.typeCategory = BetTypeCategory.None;
                        //    break;
                        case "V4":
                            typeCategory = BetTypeCategory.V4;
                            break;
                        case "V5":
                            typeCategory = BetTypeCategory.V5;
                            break;
                        case "V65":
                        case "V64":
                            typeCategory = BetTypeCategory.V6X;
                            break;
                        case "V75":
                        case "GS75":
                            typeCategory = BetTypeCategory.V75;
                            break;
                        case "V86":
                            typeCategory = BetTypeCategory.V86;
                            break;
                        case "V85":
                            typeCategory = BetTypeCategory.V85;
                            break;
                        case "DD":
                        case "LD":
                            typeCategory = BetTypeCategory.Double;
                            break;
                        case "TV":
                        case "T":
                            typeCategory = BetTypeCategory.Twin;
                            break;
                        default:
                            return BetTypeCategory.None;
                    }
                }
                return typeCategory;
            }
        }

        private int maxBetForNotPayingCustomer;
        [XmlIgnore]
        public int MaxBetForNotPayingCustomer
        {
            get
            {
                switch (Code)
                {
                    case "V4":
                        maxBetForNotPayingCustomer = 50;
                        break;
                    case "V65":
                    case "V64":
                        maxBetForNotPayingCustomer = 150;
                        break;
                    //case "V75":
                    //    this.maxBetForNotPayingCustomer = 7;
                    //    break;
                    case "V86":
                        maxBetForNotPayingCustomer = 300;
                        break;
                    default:
                        maxBetForNotPayingCustomer = 0;
                        return 0;
                }
                return maxBetForNotPayingCustomer;
            }
        }

    }
}

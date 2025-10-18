using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTBetType : Notifier
    {
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

        public DateTime NextTime
        {
            get
            {
                return this.StartTime > DateTime.Now ? this.StartTime : this.EndTime;
            }
        }

        [DataMember]
        public bool Jackpot { get; set; }

        public string InfoText
        {
            get
            {
                if (this.Jackpot)
                {
                    return "JACKPOTT";
                }
                return null;
            }
        }

        internal void SetCalendarRacaDayInfoBrush()
        {
            var c = Colors.Transparent;
            if (this.StartTime.Date == DateTime.Now.Date)
            {
                if (this.StartTime > DateTime.Now)
                {
                    c = Colors.LightGreen;
                }
                else if (this.EndTime < DateTime.Now)
                {
                    c = Colors.IndianRed;
                }
                else
                {
                    c = Colors.LightYellow;
                }
            }
            else if (this.EndTime < DateTime.Now)
            {
                c = Colors.IndianRed;
            }

            this.CalendarRacaDayInfoBrush = new SolidColorBrush(c);
        }

        private Brush calendarRacaDayInfoBrush;
        public Brush CalendarRacaDayInfoBrush
        {
            get
            {
                if (this.calendarRacaDayInfoBrush == null)
                {
                    SetCalendarRacaDayInfoBrush();
                }
                return this.calendarRacaDayInfoBrush;
            }
            set
            {
                this.calendarRacaDayInfoBrush = value;
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
                if (this.betTypeATGLogo == null)
                {
                    string folder = this.IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
                    this.betTypeATGLogo = GetBetTypeATGLogo();  // new BitmapImage(new Uri(folder + this.Code + "Small.png", UriKind.Relative));
                }
                return this.betTypeATGLogo;
            }
            set
            {
                this.betTypeATGLogo = value;
                OnPropertyChanged("BetTypeATGLogo");
            }
        }

        public ImageSource GetBetTypeATGLogo()
        {
            string folder = this.IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
            return new BitmapImage(new Uri(folder + this.Code + "Small.png", UriKind.Relative));
        }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public int[] RaceNumberList { get; set; }

        public bool IsMarksGame
        {
            get
            {
                return this.Code == "V4" || this.Code == "V5" || this.Code == "V64" || this.Code == "V65" || this.Code == "V75" || this.Code == "V86" || this.Code == "GS75" || this.Code == "V85";
            }
        }

        public bool HasMultiplePools
        {
            get
            {
                return this.Code == "GS75" || this.Code == "V64" || this.Code == "V65" || this.Code == "V75" || this.Code == "V86" || this.Code == "V85";
            }
        }

        private decimal poolShare;
        [XmlIgnore]
        public decimal PoolShare
        {
            get
            {
                if (this.poolShare == 0m)
                {
                    switch (this.Code)
                    {
                        case "V3":
                        case "V4":
                        case "DD":
                        case "LD":
                            this.poolShare = 0.75m;
                            break;
                        case "V5":
                            this.poolShare = 0.65m;
                            break;
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            this.poolShare = 0.26m;
                            break;
                        case "V85":
                            this.poolShare = 0.2275m;
                            break;
                        case "V65":
                            this.poolShare = 0.325m;
                            break;
                        case "T":
                            this.poolShare = 0.7m;
                            break;
                        case "V":
                        case "P":
                        case "TV":
                            this.poolShare = 0.8m;
                            break;
                        default:
                            this.poolShare = 1m;
                            break;
                    }
                }
                return this.poolShare;
            }
        }

        private decimal gamblerReturnPercentage;
        [XmlIgnore]
        public decimal GamblerReturnPercentage
        {
            get
            {
                if (this.gamblerReturnPercentage == 0m)
                {
                    switch (this.Code)
                    {
                        case "V3":
                        case "V4":
                        case "DD":
                        case "LD":
                            this.gamblerReturnPercentage = 0.75m;
                            break;
                        case "V5":
                        case "V64":
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                        case "V65":
                            this.gamblerReturnPercentage = 0.65m;
                            break;
                        case "T":
                            this.gamblerReturnPercentage = 0.7m;
                            break;
                        case "V":
                        case "P":
                        case "TV":
                            this.gamblerReturnPercentage = 0.8m;
                            break;
                        default:
                            this.gamblerReturnPercentage = 1m;
                            break;
                    }
                }
                return this.gamblerReturnPercentage;
            }
        }

        private decimal poolShareOneError;
        [XmlIgnore]
        public decimal PoolShareOneError
        {
            get
            {
                if (this.poolShareOneError == 0m)
                {
                    switch (this.Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            this.poolShareOneError = 0.13m;
                            break;
                        case "V85":
                            this.poolShareOneError = 0.0975m;
                            break;
                        case "V65":
                            this.poolShareOneError = 0.325m;
                            break;
                        default:
                            this.poolShareOneError = 0m;
                            break;
                    }
                }
                return this.poolShareOneError;
            }
        }

        private decimal poolShareTwoErrors;
        [XmlIgnore]
        public decimal PoolShareTwoErrors
        {
            get
            {
                if (this.poolShareTwoErrors == 0m)
                {
                    switch (this.Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            this.poolShareTwoErrors = 0.26m;
                            break;
                        case "V85":
                            this.poolShareTwoErrors = 0.0975m;
                            break;
                        default:
                            this.poolShareTwoErrors = 0m;
                            break;
                    }
                }
                return this.poolShareTwoErrors;
            }
        }

        private decimal poolShareThreeErrors;
        [XmlIgnore]
        public decimal PoolShareThreeErrors
        {
            get
            {
                if (this.poolShareTwoErrors == 0m)
                {
                    switch (this.Code)
                    {
                        case "V85":
                            this.poolShareThreeErrors = 0.2275m;
                            break;
                        default:
                            this.poolShareThreeErrors = 0m;
                            break;
                    }
                }
                return this.poolShareThreeErrors;
            }
        }

        private decimal v6Factor;
        [XmlIgnore]
        public decimal V6Factor
        {
            get
            {
                if (this.v6Factor == 0m)
                {
                    switch (this.Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            this.v6Factor = 2.5m;
                            break;
                        case "V65":
                            this.v6Factor = 2.0m;
                            break;
                        default:
                            this.v6Factor = 1m;
                            break;
                    }
                }
                return this.v6Factor;
            }
        }

        private string v6String;
        [XmlIgnore]
        public string V6String
        {
            get
            {
                if (string.IsNullOrEmpty(this.v6String))
                {
                    switch (this.Code)
                    {
                        case "V64":
                        case "V65":
                            this.v6String = "V6";
                            break;
                        case "V75":
                        case "GS75":
                            this.v6String = "V7";
                            break;
                        case "V86":
                            this.v6String = "V8";
                            break;
                        default:
                            this.v6String = string.Empty;
                            break;
                    }
                }
                return this.v6String;
            }
        }

        public HPTRowValueInterval RowValueIntervalSingleWinner { get; set; }

        private ObservableCollection<HPTRowValueInterval> rowValueIntervalList;
        [XmlIgnore]
        public ObservableCollection<HPTRowValueInterval> RowValueIntervalList
        {
            get
            {
                if (this.rowValueIntervalList == null)
                {
                    switch (this.Code)
                    {
                        case "V75":
                        case "GS75":
                        case "V86":
                        case "V85":
                            this.rowValueIntervalList = new ObservableCollection<HPTRowValueInterval>
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
                            this.rowValueIntervalList = new ObservableCollection<HPTRowValueInterval>
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
                            this.rowValueIntervalList = new ObservableCollection<HPTRowValueInterval>
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
                return this.rowValueIntervalList;
            }
        }


        private HPTRowValuePercentile[] rowValuePercentileList;
        [XmlIgnore]
        public HPTRowValuePercentile[] RowValuePercentileList
        {
            get
            {
                if (this.rowValuePercentileList == null)
                {
                    this.rowValuePercentileList = new HPTRowValuePercentile[]
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
                return this.rowValuePercentileList;
            }
        }

        private HPTPayOut[] payOutDummyList;
        [XmlIgnore]
        public HPTPayOut[] PayOutDummyList
        {
            get
            {
                if (this.payOutDummyList == null)
                {
                    switch (this.Code)
                    {
                        case "V75":
                        case "GS75":
                            this.payOutDummyList = new HPTPayOut[]
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
                            this.payOutDummyList = new HPTPayOut[]
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
                            this.payOutDummyList = new HPTPayOut[]
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
                            this.payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 5
                                }
                            };
                            break;
                        case "V4":
                            this.payOutDummyList = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 4
                                }
                            };
                            break;
                        case "V64":
                            this.payOutDummyList = new HPTPayOut[]
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
                            this.payOutDummyList = new HPTPayOut[]
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
                return this.payOutDummyList;
            }
        }

        private int[] betMultiplierList;
        [XmlIgnore]
        public int[] BetMultiplierList
        {
            get
            {
                if (this.betMultiplierList == null || this.betMultiplierList.Count() == 0)
                {
                    switch (this.Code)
                    {
                        case "V75":
                        case "GS75":
                        case "V86":
                        case "V85":
                        case "V5":
                            this.betMultiplierList = new int[] { 1, 2, 5, 10, 20, 50, 100 };
                            break;
                        case "V4":
                        case "V64":
                        case "V65":
                            this.betMultiplierList = new int[] { 1, 2, 3, 4, 5, 10, 50, 100 };
                            break;
                        default:
                            this.betMultiplierList = new int[] { 1 };
                            break;
                    }
                }
                return this.betMultiplierList;
            }
        }

        private int lowestStake;
        [XmlIgnore]
        public int LowestStake
        {
            get
            {
                if (this.lowestStake == 0)
                {
                    switch (this.Code)
                    {
                        case "DD":
                        case "LD":
                        case "TV":
                            this.lowestStake = 5;
                            break;
                        case "T":
                            this.lowestStake = 2;
                            break;
                        default:
                            this.lowestStake = 1;
                            break;
                    }
                }
                return this.lowestStake;
            }
        }

        private int highestStake;
        [XmlIgnore]
        public int HighestStake
        {
            get
            {
                if (this.highestStake == 0)
                {
                    switch (this.Code)
                    {
                        case "DD":
                        case "LD":
                        case "TV":
                            this.highestStake = 1000;
                            break;
                        case "T":
                            this.highestStake = 500;
                            break;
                        default:
                            this.highestStake = 1;
                            break;
                    }
                }
                return this.highestStake;
            }
        }

        private decimal rowCost;
        [XmlIgnore]
        public decimal RowCost
        {
            get
            {
                if (this.rowCost == 0M)
                {
                    switch (this.Code)
                    {
                        case "V3":
                            this.rowCost = 10m;
                            break;
                        case "V4":
                            this.rowCost = 2m;
                            break;
                        case "V5":
                        case "V64":
                        case "V65":
                        case "GS75":
                            this.rowCost = 1m;
                            break;
                        case "V75":
                        case "V85":
                            this.rowCost = 0.5m;
                            break;
                        case "V86":
                            this.rowCost = 0.25m;
                            break;
                        default:
                            this.rowCost = 0m;
                            break;
                    }
                }
                return this.rowCost;
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
                if (this.jackpotLimit == 0)
                {
                    switch (this.Code)
                    {
                        case "V65":
                            this.jackpotLimit = 20;
                            break;
                        case "V64":
                            this.jackpotLimit = 7;
                            break;
                        case "V75":
                        case "GS75":
                        case "V86":
                            this.jackpotLimit = 15;
                            break;
                        case "V85":
                            this.jackpotLimit = 5;
                            break;
                        case "DD":
                        case "LD":
                        case "V3":
                        case "V4":
                        case "V5":
                        case "TV":
                        case "T":
                            this.jackpotLimit = 0;
                            break;
                        default:
                            return 0;
                    }
                }
                return this.jackpotLimit;
            }
        }

        private int numberOfRaces;
        [XmlIgnore]
        public int NumberOfRaces
        {
            get
            {
                if (this.numberOfRaces == 0m)
                {
                    switch (this.Code)
                    {
                        case "V3":
                            this.numberOfRaces = 3;
                            break;
                        case "V4":
                            this.numberOfRaces = 4;
                            break;
                        case "V5":
                            this.numberOfRaces = 5;
                            break;
                        case "V65":
                        case "V64":
                            this.numberOfRaces = 6;
                            break;
                        case "V75":
                        case "GS75":
                            this.numberOfRaces = 7;
                            break;
                        case "V86":
                        case "V85":
                            this.numberOfRaces = 8;
                            break;
                        case "DD":
                        case "LD":
                            this.numberOfRaces = 2;
                            break;
                        case "TV":
                        case "T":
                            this.numberOfRaces = 1;
                            break;
                        default:
                            return 0;
                    }
                }
                return this.numberOfRaces;
            }
        }

        private int maxNumberOfSystemsInFile;
        [XmlIgnore]
        public int MaxNumberOfSystemsInFile
        {
            get
            {
                if (this.maxNumberOfSystemsInFile == 0m)
                {
                    switch (this.Code)
                    {
                        case "V4":
                        case "V5":
                            this.maxNumberOfSystemsInFile = 500;
                            break;
                        case "V65":
                        case "V64":
                            this.maxNumberOfSystemsInFile = 2000;
                            break;
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                            this.maxNumberOfSystemsInFile = 5000;
                            break;
                        default:
                            this.maxNumberOfSystemsInFile = 9999;
                            break;
                    }
                }
                return this.maxNumberOfSystemsInFile;
            }
        }

        private BetTypeCategory typeCategory;
        [XmlIgnore]
        public BetTypeCategory TypeCategory
        {
            get
            {
                if (this.typeCategory == BetTypeCategory.None)
                {
                    switch (this.Code)
                    {
                        //case "V3":
                        //    this.typeCategory = BetTypeCategory.None;
                        //    break;
                        case "V4":
                            this.typeCategory = BetTypeCategory.V4;
                            break;
                        case "V5":
                            this.typeCategory = BetTypeCategory.V5;
                            break;
                        case "V65":
                        case "V64":
                            this.typeCategory = BetTypeCategory.V6X;
                            break;
                        case "V75":
                        case "GS75":
                            this.typeCategory = BetTypeCategory.V75;
                            break;
                        case "V86":
                            this.typeCategory = BetTypeCategory.V86;
                            break;
                        case "V85":
                            this.typeCategory = BetTypeCategory.V85;
                            break;
                        case "DD":
                        case "LD":
                            this.typeCategory = BetTypeCategory.Double;
                            break;
                        case "TV":
                        case "T":
                            this.typeCategory = BetTypeCategory.Twin;
                            break;
                        default:
                            return BetTypeCategory.None;
                    }
                }
                return this.typeCategory;
            }
        }

        private int maxBetForNotPayingCustomer;
        [XmlIgnore]
        public int MaxBetForNotPayingCustomer
        {
            get
            {
                switch (this.Code)
                {
                    case "V4":
                        this.maxBetForNotPayingCustomer = 50;
                        break;
                    case "V65":
                    case "V64":
                        this.maxBetForNotPayingCustomer = 150;
                        break;
                    //case "V75":
                    //    this.maxBetForNotPayingCustomer = 7;
                    //    break;
                    case "V86":
                        this.maxBetForNotPayingCustomer = 300;
                        break;
                    default:
                        this.maxBetForNotPayingCustomer = 0;
                        return 0;
                }
                return this.maxBetForNotPayingCustomer;
            }
        }

    }
}

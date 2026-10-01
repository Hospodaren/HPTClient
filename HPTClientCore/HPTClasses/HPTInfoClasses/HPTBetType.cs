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
        [DataMember]
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

        public Brush CalendarRacaDayInfoBrush
        {
            get
            {
                if (field == null)
                {
                    SetCalendarRacaDayInfoBrush();
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //[DataMember]
        public HPTRaceDayInfo CalendarRaceDayInfo { get; set; }

        [XmlIgnore]
        public ImageSource BetTypeATGLogo
        {
            get
            {
                if (field == null)
                {
                    var folder = IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
                    field = GetBetTypeATGLogo();  // new BitmapImage(new Uri(folder + this.Code + "Small.png", UriKind.Relative));
                }
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public ImageSource GetBetTypeATGLogo()
        {
            var folder = IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
            return new BitmapImage(new Uri($"{folder}{Code}Small.png", UriKind.Relative));
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

        [XmlIgnore]
        public decimal PoolShare
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V3":
                        case "V4":
                        case "DD":
                        case "LD":
                            field = 0.75m;
                            break;
                        case "V5":
                            field = 0.65m;
                            break;
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            field = 0.26m;
                            break;
                        case "V85":
                            field = 0.195m;    // TODO: Ändra till 0.195 när ATG ändrar
                            break;
                        case "V65":
                            field = 0.325m;
                            break;
                        case "T":
                            field = 0.7m;
                            break;
                        case "V":
                        case "P":
                        case "TV":
                            field = 0.8m;
                            break;
                        default:
                            field = 1m;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public decimal GamblerReturnPercentage
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V3":
                        case "V4":
                        case "DD":
                        case "LD":
                            field = 0.75m;
                            break;
                        case "V5":
                        case "V64":
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                        case "V65":
                            field = 0.65m;
                            break;
                        case "T":
                            field = 0.7m;
                            break;
                        case "V":
                        case "P":
                        case "TV":
                            field = 0.8m;
                            break;
                        default:
                            field = 1m;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public decimal PoolShareOneError
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            field = 0.13m;
                            break;
                        case "V85":
                            field = 0.13m;    //TODO: Ändra till 0.13 när ATG ändrar
                            break;
                        case "V65":
                            field = 0.325m;
                            break;
                        default:
                            field = 0m;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public decimal PoolShareTwoErrors
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            field = 0.26m;
                            break;
                        case "V85":
                            field = 0.0975m;
                            break;
                        default:
                            field = 0m;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public decimal PoolShareThreeErrors
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V85":
                            field = 0.2275m;
                            break;
                        default:
                            field = 0m;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public decimal V6Factor
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V75":
                        case "GS75":
                        case "V86":
                            field = 2.5m;
                            break;
                        case "V65":
                            field = 2.0m;
                            break;
                        default:
                            field = 1m;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public string V6String
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    switch (Code)
                    {
                        case "V64":
                        case "V65":
                            field = "V6";
                            break;
                        case "V75":
                        case "GS75":
                            field = "V7";
                            break;
                        case "V86":
                            field = "V8";
                            break;
                        default:
                            field = string.Empty;
                            break;
                    }
                }
                return field;
            }
        }

        public HPTRowValueInterval RowValueIntervalSingleWinner { get; set; }

        [XmlIgnore]
        public ObservableCollection<HPTRowValueInterval> RowValueIntervalList
        {
            get
            {
                if (field == null)
                {
                    switch (Code)
                    {
                        case "V75":
                        case "GS75":
                        case "V86":
                        case "V85":
                            field = new ObservableCollection<HPTRowValueInterval>
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
                            field = new ObservableCollection<HPTRowValueInterval>
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
                            field = new ObservableCollection<HPTRowValueInterval>
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
                return field;
            }
        }


        [XmlIgnore]
        public HPTRowValuePercentile[] RowValuePercentileList
        {
            get
            {
                if (field == null)
                {
                    field = new HPTRowValuePercentile[]
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
                return field;
            }
        }

        [XmlIgnore]
        public HPTPayOut[] PayOutDummyList
        {
            get
            {
                if (field == null)
                {
                    switch (Code)
                    {
                        case "V75":
                        case "GS75":
                            field = new HPTPayOut[]
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
                            field = new HPTPayOut[]
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
                            field = new HPTPayOut[]
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
                            field = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 5
                                }
                            };
                            break;
                        case "V4":
                            field = new HPTPayOut[]
                            {
                                new HPTPayOut()
                                {
                                    NumberOfCorrect = 4
                                }
                            };
                            break;
                        case "V64":
                            field = new HPTPayOut[]
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
                            field = new HPTPayOut[]
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
                return field;
            }
        }

        [XmlIgnore]
        public int[] BetMultiplierList
        {
            get
            {
                if (field == null || field.Count() == 0)
                {
                    switch (Code)
                    {
                        case "V75":
                        case "GS75":
                        case "V86":
                        case "V85":
                        case "V5":
                            field = new int[] { 1, 2, 5, 10, 20, 50, 100 };
                            break;
                        case "V4":
                        case "V64":
                        case "V65":
                            field = new int[] { 1, 2, 3, 4, 5, 10, 50, 100 };
                            break;
                        default:
                            field = new int[] { 1 };
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public int LowestStake
        {
            get
            {
                if (field == 0)
                {
                    switch (Code)
                    {
                        case "DD":
                        case "LD":
                        case "TV":
                            field = 5;
                            break;
                        case "T":
                            field = 2;
                            break;
                        default:
                            field = 1;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public int HighestStake
        {
            get
            {
                if (field == 0)
                {
                    switch (Code)
                    {
                        case "DD":
                        case "LD":
                        case "TV":
                            field = 1000;
                            break;
                        case "T":
                            field = 500;
                            break;
                        default:
                            field = 1;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public decimal RowCost
        {
            get
            {
                if (field == 0M)
                {
                    switch (Code)
                    {
                        case "V3":
                            field = 10m;
                            break;
                        case "V4":
                            field = 2m;
                            break;
                        case "V5":
                        case "V64":
                        case "V65":
                        case "GS75":
                            field = 1m;
                            break;
                        case "V75":
                        case "V85":
                            field = 0.5m;
                            break;
                        case "V86":
                            field = 0.25m;
                            break;
                        default:
                            field = 0m;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public int NumberOfUploadedSystems { get; set; }

        [XmlIgnore]
        public int JackpotLimit
        {
            get
            {
                if (field == 0)
                {
                    switch (Code)
                    {
                        case "V65":
                            field = 20;
                            break;
                        case "V64":
                            field = 7;
                            break;
                        case "V75":
                        case "GS75":
                        case "V86":
                            field = 15;
                            break;
                        case "V85":
                            field = 5;
                            break;
                        case "DD":
                        case "LD":
                        case "V3":
                        case "V4":
                        case "V5":
                        case "TV":
                        case "T":
                            field = 0;
                            break;
                        default:
                            return 0;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public int NumberOfRaces
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V3":
                            field = 3;
                            break;
                        case "V4":
                            field = 4;
                            break;
                        case "V5":
                            field = 5;
                            break;
                        case "V65":
                        case "V64":
                            field = 6;
                            break;
                        case "V75":
                        case "GS75":
                            field = 7;
                            break;
                        case "V86":
                        case "V85":
                            field = 8;
                            break;
                        case "DD":
                        case "LD":
                            field = 2;
                            break;
                        case "TV":
                        case "T":
                            field = 1;
                            break;
                        default:
                            return 0;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public int MaxNumberOfSystemsInFile
        {
            get
            {
                if (field == 0m)
                {
                    switch (Code)
                    {
                        case "V4":
                        case "V5":
                            field = 500;
                            break;
                        case "V65":
                        case "V64":
                            field = 2000;
                            break;
                        case "V75":
                        case "V85":
                        case "GS75":
                        case "V86":
                            field = 5000;
                            break;
                        default:
                            field = 9999;
                            break;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public BetTypeCategory TypeCategory
        {
            get
            {
                if (field == BetTypeCategory.None)
                {
                    switch (Code)
                    {
                        //case "V3":
                        //    this.typeCategory = BetTypeCategory.None;
                        //    break;
                        case "V4":
                            field = BetTypeCategory.V4;
                            break;
                        case "V5":
                            field = BetTypeCategory.V5;
                            break;
                        case "V65":
                        case "V64":
                            field = BetTypeCategory.V6X;
                            break;
                        case "V75":
                        case "GS75":
                            field = BetTypeCategory.V75;
                            break;
                        case "V86":
                            field = BetTypeCategory.V86;
                            break;
                        case "V85":
                            field = BetTypeCategory.V85;
                            break;
                        case "DD":
                        case "LD":
                            field = BetTypeCategory.Double;
                            break;
                        case "TV":
                        case "T":
                            field = BetTypeCategory.Twin;
                            break;
                        default:
                            return BetTypeCategory.None;
                    }
                }
                return field;
            }
        }

        [XmlIgnore]
        public int MaxBetForNotPayingCustomer
        {
            get
            {
                switch (Code)
                {
                    case "V4":
                        field = 50;
                        break;
                    case "V65":
                    case "V64":
                        field = 150;
                        break;
                    //case "V75":
                    //    this.maxBetForNotPayingCustomer = 7;
                    //    break;
                    case "V86":
                        field = 300;
                        break;
                    default:
                        field = 0;
                        return 0;
                }
                return field;
            }
        }

    }
}

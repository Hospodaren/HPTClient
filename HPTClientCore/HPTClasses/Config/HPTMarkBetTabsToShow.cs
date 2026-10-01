using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMarkBetTabsToShow : Notifier
    {
        public HPTMarkBetTabsToShow()
        {
        }

        public List<HPTMarkBetTabsToShowAttribute> GetMarkBetTabsToShowAttributes()
        {
            var attributeList = new List<HPTMarkBetTabsToShowAttribute>();
            foreach (var pi in (typeof(HPTMarkBetTabsToShow)).GetProperties())
            {
                foreach (var o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HPTMarkBetTabsToShowAttribute))
                    {
                        var mta = (HPTMarkBetTabsToShowAttribute)o;
                        attributeList.Add(mta);
                    }
                }
            }
            return attributeList;
        }

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        public List<string> ColumnsInOrder { get; set; }

        [DataMember]
        public GUIProfile GUIProfile
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = GUIProfile.Normal;

        private bool isPayingCustomer;
        [XmlIgnore]
        public bool IsPayingCustomer
        {
            get
            {
                return true;
            }
            set
            {
                isPayingCustomer = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Avdelningar", "ShowRaces", 1, false)]
        [DataMember]
        public bool ShowRaces
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Poäng", "ShowRacesGrouped", 12, false)] // KOMMANDE
        [DataMember]
        public bool ShowRacesGrouped
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Översikt", "ShowOverview", 3, false)]
        [DataMember]
        public bool ShowOverview
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        //private bool showTrends;
        //[HPTMarkBetTabsToShow("Trender", "ShowTrends", 3, false)]
        //[DataMember]
        //public bool ShowTrends
        //{
        //    get
        //    {
        //        return showTrends;
        //    }
        //    set
        //    {
        //        showTrends = value;
        //        OnPropertyChanged("ShowTrends");
        //    }
        //}

        [HPTMarkBetTabsToShow("Kommentarer", "ShowComments", 3, true)]
        [DataMember]
        public bool ShowComments
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Rankreducering", "ShowRankReduction", 4, true)]
        [DataMember]
        public bool ShowRankReduction
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Ranköversikt", "ShowRankOverview", 4, true)]
        [DataMember]
        public bool ShowRankOverview
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Enkelrader", "ShowSingleRows", 5, true)]
        [DataMember]
        public bool ShowSingleRows
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Utgångar", "ShowComplimentaryRules", 6, true)]
        [DataMember]
        public bool ShowComplimentaryRules
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }


        [HPTMarkBetTabsToShow("Intervall", "ShowIntervalReduction", 7, true)]
        [DataMember]
        public bool ShowIntervalReduction
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Kuskar", "ShowDriverReduction", 8, true)]
        [DataMember]
        public bool ShowDriverReduction
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Tränare", "ShowTrainerReduction", 9, true)]
        [DataMember]
        public bool ShowTrainerReduction
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Rättning", "ShowCorrection", 10, false)]
        [DataMember]
        public bool ShowCorrection
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Villkorsstatistik", "ShowReductionStatistics", 4, true)]
        [DataMember]
        public bool ShowReductionStatistics
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Gruppintervall", "ShowGroupIntervalReduction", 11, true)]
        [DataMember]
        public bool ShowGroupIntervalReduction
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Multi-ABCD", "ShowMultiABCD", 12, true)]
        [DataMember]
        public bool ShowMultiABCD
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("V6/Flerbong", "ShowV6BetMultiplier", 13, true)]
        [DataMember]
        public bool ShowV6BetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [HPTMarkBetTabsToShow("Kategorireducering", "ShowCategoryCodeReduction", 2, true)]
        [DataMember]
        public bool ShowCategoryCodeReduction
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}

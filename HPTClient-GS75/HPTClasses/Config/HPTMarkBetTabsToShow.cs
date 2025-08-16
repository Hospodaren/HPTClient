using System.Collections.Generic;
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
            List<HPTMarkBetTabsToShowAttribute> attributeList = new List<HPTMarkBetTabsToShowAttribute>();
            foreach (PropertyInfo pi in (typeof(HPTMarkBetTabsToShow)).GetProperties())
            {
                foreach (object o in pi.GetCustomAttributes(true))
                {
                    if (o.GetType() == typeof(HPTMarkBetTabsToShowAttribute))
                    {
                        HPTMarkBetTabsToShowAttribute mta = (HPTMarkBetTabsToShowAttribute)o;
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

        private GUIProfile guiProfile = GUIProfile.Normal;
        [DataMember]
        public GUIProfile GUIProfile
        {
            get
            {
                return guiProfile;
            }
            set
            {
                guiProfile = value;
                OnPropertyChanged("GUIProfile");
            }
        }

        private bool isPayingCustomer;
        [XmlIgnore]
        public bool IsPayingCustomer
        {
            get
            {
                //return this.isPayingCustomer;
                return true;
            }
            set
            {
                this.isPayingCustomer = value;
                OnPropertyChanged("IsPayingCustomer");
            }
        }

        private bool showRaces;
        [HPTMarkBetTabsToShow("Avdelningar", "ShowRaces", 1, false)]
        [DataMember]
        public bool ShowRaces
        {
            get
            {
                return showRaces;
            }
            set
            {
                showRaces = value;
                OnPropertyChanged("ShowRaces");
            }
        }

        private bool showRacesGrouped;
        [HPTMarkBetTabsToShow("Poäng", "ShowRacesGrouped", 2, false)]   // KOMMANDE
        [DataMember]
        public bool ShowRacesGrouped
        {
            get
            {
                return showRacesGrouped;
            }
            set
            {
                showRacesGrouped = value;
                OnPropertyChanged("ShowRacesGrouped");
            }
        }

        private bool showOverview;
        [HPTMarkBetTabsToShow("Översikt", "ShowOverview", 3, false)]
        [DataMember]
        public bool ShowOverview
        {
            get
            {
                return showOverview;
            }
            set
            {
                showOverview = value;
                OnPropertyChanged("ShowOverview");
            }
        }

        private bool showTrends;
        [HPTMarkBetTabsToShow("Trender", "ShowTrends", 3, false)]
        [DataMember]
        public bool ShowTrends
        {
            get
            {
                return showTrends;
            }
            set
            {
                showTrends = value;
                OnPropertyChanged("ShowTrends");
            }
        }

        private bool showComments;
        [HPTMarkBetTabsToShow("Kommentarer", "ShowComments", 3, true)]
        [DataMember]
        public bool ShowComments
        {
            get
            {
                return showComments;
            }
            set
            {
                showComments = value;
                OnPropertyChanged("ShowComments");
            }
        }

        private bool showRankReduction;
        [HPTMarkBetTabsToShow("Rankreducering", "ShowRankReduction", 4, true)]
        [DataMember]
        public bool ShowRankReduction
        {
            get
            {
                return this.showRankReduction;
            }
            set
            {
                this.showRankReduction = value;
                OnPropertyChanged("ShowRankReduction");
            }
        }

        private bool showRankOverview;
        [HPTMarkBetTabsToShow("Ranköversikt", "ShowRankOverview", 4, true)]
        [DataMember]
        public bool ShowRankOverview
        {
            get
            {
                return this.showRankOverview;
            }
            set
            {
                this.showRankOverview = value;
                OnPropertyChanged("ShowRankOverview");
            }
        }

        private bool showSingleRows;
        [HPTMarkBetTabsToShow("Enkelrader", "ShowSingleRows", 5, true)]
        [DataMember]
        public bool ShowSingleRows
        {
            get
            {
                return showSingleRows;
            }
            set
            {
                showSingleRows = value;
                OnPropertyChanged("ShowSingleRows");
            }
        }

        private bool showComplimentaryRules;
        [HPTMarkBetTabsToShow("Utgångar", "ShowComplimentaryRules", 6, true)]
        [DataMember]
        public bool ShowComplimentaryRules
        {
            get
            {
                return this.showComplimentaryRules;
            }
            set
            {
                this.showComplimentaryRules = value;
                OnPropertyChanged("ShowComplimentaryRules");
            }
        }


        private bool showIntervalReduction;
        [HPTMarkBetTabsToShow("Intervall", "ShowIntervalReduction", 7, true)]
        [DataMember]
        public bool ShowIntervalReduction
        {
            get
            {
                return this.showIntervalReduction;
            }
            set
            {
                this.showIntervalReduction = value;
                OnPropertyChanged("ShowIntervalReduction");
            }
        }

        //private bool showCoupons;
        //[HPTMarkBetTabsToShow("Rättning", "ShowCoupons", 7, false)]
        //public bool ShowCoupons
        //{
        //    get
        //    {
        //        return showCoupons;
        //    }
        //    set
        //    {
        //        showCoupons = value;
        //        OnPropertyChanged("ShowCoupons");
        //    }
        //}

        private bool showDriverReduction;
        [HPTMarkBetTabsToShow("Kuskar", "ShowDriverReduction", 8, true)]
        [DataMember]
        public bool ShowDriverReduction
        {
            get
            {
                return showDriverReduction;
            }
            set
            {
                showDriverReduction = value;
                OnPropertyChanged("ShowDriverReduction");
            }
        }

        private bool showTrainerReduction;
        [HPTMarkBetTabsToShow("Tränare", "ShowTrainerReduction", 9, true)]
        [DataMember]
        public bool ShowTrainerReduction
        {
            get
            {
                return showTrainerReduction;
            }
            set
            {
                showTrainerReduction = value;
                OnPropertyChanged("ShowTrainerReduction");
            }
        }

        private bool showCorrection;
        [HPTMarkBetTabsToShow("Rättning", "ShowCorrection", 10, false)]
        [DataMember]
        public bool ShowCorrection
        {
            get
            {
                return showCorrection;
            }
            set
            {
                showCorrection = value;
                OnPropertyChanged("ShowCorrection");
            }
        }

        private bool showReductionStatistics;
        [HPTMarkBetTabsToShow("Villkorsstatistik", "ShowReductionStatistics", 4, true)]
        [DataMember]
        public bool ShowReductionStatistics
        {
            get
            {
                return this.showReductionStatistics;
            }
            set
            {
                this.showReductionStatistics = value;
                OnPropertyChanged("ShowReductionStatistics");
            }
        }

        private bool showAdvanced;
        //[HPTMarkBetTabsToShow("Avdelningar", "ShowRaces", 1)]
        [DataMember]
        public bool ShowAdvanced
        {
            get
            {
                return this.showAdvanced;
            }
            set
            {
                this.showAdvanced = value;
                OnPropertyChanged("ShowAdvanced");
            }
        }

        private bool showGroupIntervalReduction;
        [HPTMarkBetTabsToShow("Gruppintervall", "ShowGroupIntervalReduction", 11, true)]
        [DataMember]
        public bool ShowGroupIntervalReduction
        {
            get
            {
                return this.showGroupIntervalReduction;
            }
            set
            {
                this.showGroupIntervalReduction = value;
                OnPropertyChanged("ShowGroupIntervalReduction");
            }
        }

        private bool showMultiABCD;
        [HPTMarkBetTabsToShow("Multi-ABCD", "ShowMultiABCD", 12, true)]
        [DataMember]
        public bool ShowMultiABCD
        {
            get
            {
                return this.showMultiABCD;
            }
            set
            {
                this.showMultiABCD = value;
                OnPropertyChanged("ShowMultiABCD");
            }
        }

        private bool showV6BetMultiplier;
        //[HPTMarkBetTabsToShow("V6/V7/V8/Flerbong", "ShowV6BetMultiplier", 13, true)]
        [HPTMarkBetTabsToShow("V6/Flerbong", "ShowV6BetMultiplier", 13, true)]
        [DataMember]
        public bool ShowV6BetMultiplier
        {
            get
            {
                return this.showV6BetMultiplier;
            }
            set
            {
                this.showV6BetMultiplier = value;
                OnPropertyChanged("ShowV6BetMultiplier");
            }
        }

        private bool showCompanyGambling;
        //[HPTMarkBetTabsToShow("Bolagsspel", "ShowCompanyGambling", 14, true)]
        [DataMember]
        public bool ShowCompanyGambling
        {
            get
            {
                return this.showCompanyGambling;
            }
            set
            {
                this.showCompanyGambling = value;
                OnPropertyChanged("ShowCompanyGambling");
            }
        }

        private bool showATGXmlFile;
        //[HPTMarkBetTabsToShow("XML-kuponger", "ShowATGXmlFile", 15, true)]
        [DataMember]
        public bool ShowATGXmlFile
        {
            get
            {
                return this.showATGXmlFile;
            }
            set
            {
                this.showATGXmlFile = value;
                OnPropertyChanged("ShowATGXmlFile");
            }
        }

        private bool showTemplateWorkshop;
        [HPTMarkBetTabsToShow("Mallverkstad", "ShowTemplateWorkshop", 16, true)]
        [DataMember]
        public bool ShowTemplateWorkshop
        {
            get
            {
                return this.showTemplateWorkshop;
            }
            set
            {
                this.showTemplateWorkshop = value;
                OnPropertyChanged("ShowTemplateWorkshop");
            }
        }
    }
}

using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTGUIElementsToShow : HPTDataToShow
    {
        #region Översikt

        private bool showOverview = true;
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

        private bool showRaceLock;
        [DataMember]
        public bool ShowRaceLock
        {
            get
            {
                return showRaceLock;
            }
            set
            {
                showRaceLock = value;
                OnPropertyChanged("ShowRaceLock");
            }
        }

        #endregion

        #region Inställningar

        private bool showUpdateHandling = true;
        [DataMember]
        public bool ShowUpdateHandling
        {
            get
            {
                return showUpdateHandling;
            }
            set
            {
                showUpdateHandling = value;
                OnPropertyChanged("ShowUpdateHandling");
            }
        }

        private bool showReservHandling;
        [DataMember]
        public bool ShowReservHandling
        {
            get
            {
                return showReservHandling;
            }
            set
            {
                showReservHandling = value;
                OnPropertyChanged("ShowReservHandling");
            }
        }

        private bool showCouponCompression;
        [DataMember]
        public bool ShowCouponCompression
        {
            get
            {
                return showCouponCompression;
            }
            set
            {
                showCouponCompression = value;
                OnPropertyChanged("ShowCouponCompression");
            }
        }

        private bool showV6;
        [DataMember]
        public bool ShowV6
        {
            get
            {
                return showV6;
            }
            set
            {
                showV6 = value;
                OnPropertyChanged("ShowV6");
            }
        }

        private bool showBetMultiplier;
        [DataMember]
        public bool ShowBetMultiplier
        {
            get
            {
                return showBetMultiplier;
            }
            set
            {
                showBetMultiplier = value;
                OnPropertyChanged("ShowBetMultiplier");
            }
        }

        private bool showAutomaticCalculation;
        [DataMember]
        public bool ShowAutomaticCalculation
        {
            get
            {
                return showAutomaticCalculation;
            }
            set
            {
                showAutomaticCalculation = value;
                OnPropertyChanged("ShowAutomaticCalculation");
            }
        }

        #endregion

        private bool showTemplates;
        [DataMember]
        public bool ShowTemplates
        {
            get
            {
                return showTemplates;
            }
            set
            {
                showTemplates = value;
                OnPropertyChanged("ShowTemplates");
            }
        }

        #region Systeminformation

        private bool showSystemSize = true;
        [DataMember]
        public bool ShowSystemSize
        {
            get
            {
                return showSystemSize;
            }
            set
            {
                showSystemSize = value;
                OnPropertyChanged("ShowSystemSize");
            }
        }

        private bool showReducedSize = true;
        [DataMember]
        public bool ShowReducedSize
        {
            get
            {
                return showReducedSize;
            }
            set
            {
                showReducedSize = value;
                OnPropertyChanged("ShowReducedSize");
            }
        }

        private bool showReductionPercentage = true;
        [DataMember]
        public bool ShowReductionPercentage
        {
            get
            {
                return showReductionPercentage;
            }
            set
            {
                showReductionPercentage = value;
                OnPropertyChanged("ShowReductionPercentage");
            }
        }

        private bool showCouponInfo;
        [DataMember]
        public bool ShowCouponInfo
        {
            get
            {
                return showCouponInfo;
            }
            set
            {
                showCouponInfo = value;
                OnPropertyChanged("ShowCouponInfo");
            }
        }

        private bool showNumberOfSystems;
        [DataMember]
        public bool ShowNumberOfSystems
        {
            get
            {
                return showNumberOfSystems;
            }
            set
            {
                showNumberOfSystems = value;
                OnPropertyChanged("ShowNumberOfSystems");
            }
        }

        private bool showSystemCost = true;
        [DataMember]
        public bool ShowSystemCost
        {
            get
            {
                return showSystemCost;
            }
            set
            {
                showSystemCost = value;
                OnPropertyChanged("ShowSystemCost");
            }
        }

        private bool showSystemCostChange;
        [DataMember]
        public bool ShowSystemCostChange
        {
            get
            {
                return showSystemCostChange;
            }
            set
            {
                showSystemCostChange = value;
                OnPropertyChanged("ShowSystemCostChange");
            }
        }

        private bool showNumberOfGambledRows;
        [DataMember]
        public bool ShowNumberOfGambledRows
        {
            get
            {
                return showNumberOfGambledRows;
            }
            set
            {
                showNumberOfGambledRows = value;
                OnPropertyChanged("ShowNumberOfGambledRows");
            }
        }

        private bool showRowValueInterval;
        [DataMember]
        public bool ShowRowValueInterval
        {
            get
            {
                return showRowValueInterval;
            }
            set
            {
                showRowValueInterval = value;
                OnPropertyChanged("ShowRowValueInterval");
            }
        }

        private bool showLiveCalculation;
        [DataMember]
        public bool ShowLiveCalculation
        {
            get
            {
                return showLiveCalculation;
            }
            set
            {
                showLiveCalculation = value;
                OnPropertyChanged("ShowLiveCalculation");
            }
        }

        #endregion

        private bool showReductionList;
        [DataMember]
        public bool ShowReductionList
        {
            get
            {
                return showReductionList;
            }
            set
            {
                showReductionList = value;
                OnPropertyChanged("ShowReductionList");
            }
        }

        #region Fil

        private bool showBeginner = true;
        [DataMember]
        public bool ShowBeginner
        {
            get
            {
                return showBeginner;
            }
            set
            {
                showBeginner = value;
                OnPropertyChanged("ShowBeginner");
            }
        }

        private bool showSave = true;
        [DataMember]
        public bool ShowSave
        {
            get
            {
                return showSave;
            }
            set
            {
                showSave = value;
                OnPropertyChanged("ShowSave");
            }
        }

        private bool showSaveAs;
        [DataMember]
        public bool ShowSaveAs
        {
            get
            {
                return showSaveAs;
            }
            set
            {
                showSaveAs = value;
                OnPropertyChanged("ShowSaveAs");
            }
        }

        private bool showCopy = true;
        [DataMember]
        public bool ShowCopy
        {
            get
            {
                return showCopy;
            }
            set
            {
                showCopy = value;
                OnPropertyChanged("ShowCopy");
            }
        }

        private bool showPrint;
        [DataMember]
        public bool ShowPrint
        {
            get
            {
                return showPrint;
            }
            set
            {
                showPrint = value;
                OnPropertyChanged("ShowPrint");
            }
        }

        private bool showClear = true;
        [DataMember]
        public bool ShowClear
        {
            get
            {
                return showClear;
            }
            set
            {
                showClear = value;
                OnPropertyChanged("ShowClear");
            }
        }

        private bool showUpload;
        [DataMember]
        public bool ShowUpload
        {
            get
            {
                return showUpload;
            }
            set
            {
                showUpload = value;
                OnPropertyChanged("ShowUpload");
            }
        }

        #endregion
    }
}

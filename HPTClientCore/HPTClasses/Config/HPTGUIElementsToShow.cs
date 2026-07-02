using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTGUIElementsToShow : HPTDataToShow
    {
        #region Översikt

        [DataMember]
        public bool ShowOverview
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowRaceLock
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Inställningar

        [DataMember]
        public bool ShowUpdateHandling
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowReservHandling
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowCouponCompression
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowV6
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowBetMultiplier
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowAutomaticCalculation
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [DataMember]
        public bool ShowTemplates
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #region Systeminformation

        [DataMember]
        public bool ShowSystemSize
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowReducedSize
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowReductionPercentage
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowCouponInfo
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowNumberOfSystems
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowSystemCost
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowSystemCostChange
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowNumberOfGambledRows
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowRowValueInterval
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowLiveCalculation
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [DataMember]
        public bool ShowReductionList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #region Fil

        [DataMember]
        public bool ShowBeginner
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowSave
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowSaveAs
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowCopy
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowPrint
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool ShowClear
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = true;

        [DataMember]
        public bool ShowUpload
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}

using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTBetTypesToShow : Notifier
    {
        private bool showV3;
        [DataMember]
        public bool ShowV3
        {
            get
            {
                return showV3;
            }
            set
            {
                showV3 = value;
                OnPropertyChanged();
            }
        }

        private bool showV4;
        [DataMember]
        public bool ShowV4
        {
            get
            {
                return showV4;
            }
            set
            {
                showV4 = value;
                OnPropertyChanged();
            }
        }

        private bool showV5;
        [DataMember]
        public bool ShowV5
        {
            get
            {
                return showV5;
            }
            set
            {
                showV5 = value;
                OnPropertyChanged();
            }
        }

        private bool showV65;
        [DataMember]
        public bool ShowV65
        {
            get
            {
                return showV65;
            }
            set
            {
                showV65 = value;
                OnPropertyChanged();
            }
        }

        private bool showV64;
        [DataMember]
        public bool ShowV64
        {
            get
            {
                return showV64;
            }
            set
            {
                showV64 = value;
                OnPropertyChanged();
            }
        }

        private bool showV75;
        [DataMember]
        public bool ShowV75
        {
            get
            {
                return showV75;
            }
            set
            {
                showV75 = value;
                OnPropertyChanged();
            }
        }

        private bool showV86;
        [DataMember]
        public bool ShowV86
        {
            get
            {
                return showV86;
            }
            set
            {
                showV86 = value;
                OnPropertyChanged();
            }
        }

        private bool showTvilling;
        [DataMember]
        public bool ShowTvilling
        {
            get
            {
                return showTvilling;
            }
            set
            {
                showTvilling = value;
                OnPropertyChanged();
            }
        }

        private bool showDD;
        [DataMember]
        public bool ShowDD
        {
            get
            {
                return showDD;
            }
            set
            {
                showDD = value;
                OnPropertyChanged();
            }
        }

        private bool showLD;
        [DataMember]
        public bool ShowLD
        {
            get
            {
                return showLD;
            }
            set
            {
                showLD = value;
                OnPropertyChanged();
            }
        }

        private bool showTrio;
        [DataMember]
        public bool ShowTrio
        {
            get
            {
                return showTrio;
            }
            set
            {
                showTrio = value;
                OnPropertyChanged();
            }
        }

        private bool showDouble;
        [DataMember]
        public bool ShowDouble
        {
            get
            {
                return showDouble;
            }
            set
            {
                showDouble = value;
                OnPropertyChanged();
            }
        }

        private bool showVx;
        [DataMember]
        public bool ShowVx
        {
            get
            {
                return showVx;
            }
            set
            {
                showVx = value;
                OnPropertyChanged();
            }
        }

        private bool showVxx;
        [DataMember]
        public bool ShowVxx
        {
            get
            {
                return showVxx;
            }
            set
            {
                showVxx = value;
                OnPropertyChanged();
            }
        }
    }
}

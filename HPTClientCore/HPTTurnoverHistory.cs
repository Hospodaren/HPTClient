using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTTurnoverHistory : Notifier
    {
        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public int Turnover { get; set; }

        [DataMember]
        public decimal Percentage { get; set; }

        private bool _IsSelected;
        [DataMember]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }
}

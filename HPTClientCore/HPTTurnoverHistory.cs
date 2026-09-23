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

        [DataMember]
        public bool IsSelected
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

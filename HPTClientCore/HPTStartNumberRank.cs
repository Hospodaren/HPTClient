using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTStartNumberRank : Notifier
    {
        [DataMember]
        public bool Select
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int StartNumber
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int Rank
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

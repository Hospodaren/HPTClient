using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTStartNumberRankCollection : Notifier
    {
        [DataMember]
        public string Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string StartMethodCode
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string DistanceCode
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public List<HPTStartNumberRank> StartNumberRankList
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

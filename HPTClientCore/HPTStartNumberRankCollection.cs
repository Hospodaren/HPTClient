using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTStartNumberRankCollection : Notifier
    {
        private string name;
        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private string startMethodCode;
        [DataMember]
        public string StartMethodCode
        {
            get
            {
                return startMethodCode;
            }
            set
            {
                startMethodCode = value;
                OnPropertyChanged("StartMethodCode");
            }
        }

        private string distanceCode;
        [DataMember]
        public string DistanceCode
        {
            get
            {
                return distanceCode;
            }
            set
            {
                distanceCode = value;
                OnPropertyChanged("DistanceCode");
            }
        }

        private List<HPTStartNumberRank> startNumberRankList;
        [DataMember]
        public List<HPTStartNumberRank> StartNumberRankList
        {
            get
            {
                return startNumberRankList;
            }
            set
            {
                startNumberRankList = value;
                OnPropertyChanged("StartNumberRankList");
            }
        }
    }
}

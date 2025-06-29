using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        private string startMethodCode;
        [DataMember]
        public string StartMethodCode
        {
            get
            {
                return this.startMethodCode;
            }
            set
            {
                this.startMethodCode = value;
                OnPropertyChanged("StartMethodCode");
            }
        }

        private string distanceCode;
        [DataMember]
        public string DistanceCode
        {
            get
            {
                return this.distanceCode;
            }
            set
            {
                this.distanceCode = value;
                OnPropertyChanged("DistanceCode");
            }
        }

        private List<HPTStartNumberRank> startNumberRankList;
        [DataMember]
        public List<HPTStartNumberRank> StartNumberRankList
        {
            get
            {
                return this.startNumberRankList;
            }
            set
            {
                this.startNumberRankList = value;
                OnPropertyChanged("StartNumberRankList");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTStartNumberRank : Notifier
    {
        private bool select;
        [DataMember]
        public bool Select
        {
            get
            {
                return this.select;
            }
            set
            {
                this.select = value;
                OnPropertyChanged("Select");
            }
        }

        private int startNumber;
        [DataMember]
        public int StartNumber
        {
            get
            {
                return this.startNumber;
            }
            set
            {
                this.startNumber = value;
                OnPropertyChanged("StartNumber");
            }
        }

        private int rank;
        [DataMember]
        public int Rank
        {
            get
            {
                return this.rank;
            }
            set
            {
                this.rank = value;
                OnPropertyChanged("Rank");
            }
        }
    }
}

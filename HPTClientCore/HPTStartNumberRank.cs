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
                return select;
            }
            set
            {
                select = value;
                OnPropertyChanged("Select");
            }
        }

        private int startNumber;
        [DataMember]
        public int StartNumber
        {
            get
            {
                return startNumber;
            }
            set
            {
                startNumber = value;
                OnPropertyChanged("StartNumber");
            }
        }

        private int rank;
        [DataMember]
        public int Rank
        {
            get
            {
                return rank;
            }
            set
            {
                rank = value;
                OnPropertyChanged("Rank");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCalendar
    {
        public HPTCalendar()
        {
            this.RaceDayInfoList = new ObservableCollection<HPTRaceDayInfo>();
        }

        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public DateTime ToDate { get; set; }

        [DataMember]
        public string FromDateString { get; set; }

        [DataMember]
        public string ToDateString { get; set; }

        [DataMember]
        public ObservableCollection<HPTRaceDayInfo> RaceDayInfoList { get; set; }

        public override string ToString()
        {
            return this.FromDateString + " - " + this.ToDateString;
        } 
    }
}

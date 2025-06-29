using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseYearStatistics
    {
        //internal static HPTHorseYearStatistics CreateDummyStatistics(string yearString)
        //{
        //    Random rnd = new Random();
        //    return new HPTHorseYearStatistics()
        //    {
        //        YearString = yearString,
        //        Earning = rnd.Next(1000000),
        //        EarningMean = rnd.Next(100000),
        //        FirstPlace = rnd.Next(10),
        //        NumberOfStarts = rnd.Next(1, 10),
        //        Percent123 = rnd.Next(100),
        //        PercentFirstPlace = rnd.Next(100),
        //        SecondPlace = rnd.Next(10),
        //        ThirdPlace = rnd.Next(10)
        //    };
        //}

        [XmlIgnore]
        public string YearString { get; set; }

        [DataMember]
        public int Earning { get; set; }

        [XmlIgnore]
        public int EarningMean { get; set; }

        [DataMember]
        public int FirstPlace { get; set; }

        [DataMember]
        public int SecondPlace { get; set; }

        [DataMember]
        public int ThirdPlace { get; set; }

        [DataMember]
        public int NumberOfStarts { get; set; }

        [DataMember]
        public decimal PercentFirstPlace { get; set; }

        [DataMember]
        public decimal Percent123 { get; set; }
    }
}

using System;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseHistoryInfoGrouped
    {
        [DataMember]
        public DateTime Timestamp { get; set; }

        #region VP

        [DataMember]
        public int VinnarOdds { get; set; }

        [DataMember]
        public decimal VinnarOddsShare { get; set; }

        [DataMember]
        public decimal VinnarOddsSharePeriod { get; set; }

        [DataMember]
        public int MaxPlatsOdds { get; set; }

        [DataMember]
        public decimal PlatsOddsShare { get; set; }

        [DataMember]
        public decimal PlatsOddsSharePeriod { get; set; }

        [DataMember]
        public int InvestmentVinnare { get; set; }

        [DataMember]
        public int InvestmentPlats { get; set; }

        #endregion

        #region Markbet

        [DataMember]
        public HPTStakeHistory StakeHistoryMain { get; set; }

        [DataMember]
        public HPTStakeHistory StakeHistoryAlternate { get; set; }

        [DataMember]
        public HPTStakeHistory StakeHistoryAlternate2 { get; set; }

        //[DataMember]
        //public int StakeDistributionMain { get; set; }

        //[DataMember]
        //public decimal StakeShareMain { get; set; }

        //[DataMember]
        //public int StakeDistributionAlternate { get; set; }

        //[DataMember]
        //public decimal StakeShareAlternate { get; set; }

        //[DataMember]
        //public int StakeDistributionAlternate2 { get; set; }

        //[DataMember]
        //public decimal StakeShareAlternate2 { get; set; }

        #endregion

        #region DD/LD/Tvilling/Trio

        [DataMember]
        public decimal ShareDouble { get; set; }

        [DataMember]
        public decimal ShareTvilling { get; set; }

        [DataMember]
        public decimal ShareTrio { get; set; }

        #endregion

        #region Grupperingsmetoder



        #endregion
    }

    [DataContract]
    [KnownType(typeof(System.Windows.Media.MatrixTransform))]
    public class HPTStakeHistory
    {
        [DataMember]
        public int StakeDistribution { get; set; }

        [DataMember]
        public decimal StakeShare { get; set; }

        [DataMember]
        public decimal StakeSharePeriod { get; set; }

        [DataMember]
        public System.Windows.Media.SolidColorBrush BackColor { get; set; }

        [DataMember]
        public System.Windows.FontWeight FontWeight { get; set; }
    }

    //[DataContract]
    //public class HPTHorseHistoryInfo
    //{
    //    #region Historiska data

    //    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    //    public ObservableCollection<HPTHorseHistoryVP> HorseHistoryVPList { get; set; }

    //    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    //    public ObservableCollection<HPTHorseHistoryMarkBet> HorseHistoryVxxList { get; set; }

    //    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    //    public ObservableCollection<HPTHorseHistoryMarkBet> HorseHistoryV4List { get; set; }

    //    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    //    public ObservableCollection<HPTHorseHistoryMarkBet> HorseHistoryV5List { get; set; }

    //    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    //    public ObservableCollection<HPTHorseHistoryPercentage> HorseHistoryTvillingList { get; set; }

    //    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    //    public ObservableCollection<HPTHorseHistoryPercentage> HorseHistoryTrioList { get; set; }

    //    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    //    public ObservableCollection<HPTHorseHistoryPercentage> HorseHistoryDoubleList { get; set; }

    //    #endregion
    //}

    //[DataContract]
    //public class HPTHorseHistoryVP
    //{
    //    [DataMember]
    //    public DateTime Timestamp { get; set; }

    //    [DataMember]
    //    public int VinnarOdds { get; set; }

    //    [DataMember]
    //    public int MaxPlatsOdds { get; set; }

    //    [DataMember]
    //    public int InvestmentVinnare { get; set; }

    //    [DataMember]
    //    public int InvestmentPlats { get; set; }
    //}

    //[DataContract]
    //public class HPTHorseHistoryMarkBet
    //{
    //    [DataMember]
    //    public DateTime Timestamp { get; set; }

    //    [DataMember]
    //    public int StakeDistribution { get; set; }

    //    [DataMember]
    //    public decimal StakeShare { get; set; }
    //}

    //[DataContract]
    //public class HPTHorseHistoryPercentage
    //{
    //    [DataMember]
    //    public DateTime Timestamp { get; set; }

    //    [DataMember]
    //    public decimal Share { get; set; }
    //}
}

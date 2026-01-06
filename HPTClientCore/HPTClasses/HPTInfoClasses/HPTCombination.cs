using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTCombination : Notifier
    {
        public HPTCombination()
        {
            //
            // TODO: Add constructor logic here
            //        
        }

        public HPTRaceDayInfo ParentRaceDayInfo { get; set; }

        public HPTRace ParentRace { get; set; }

        public HPTHorse Horse1 { get; set; }

        public HPTHorse Horse2 { get; set; }

        public HPTHorse Horse3 { get; set; }

        [DataMember]
        public int Horse1Nr { get; set; }

        [DataMember]
        public int Horse2Nr { get; set; }

        [DataMember]
        public int Horse3Nr { get; set; }

        private decimal multipliedOdds;
        [DataMember]
        public decimal MultipliedOdds
        {
            get
            {
                return multipliedOdds;
            }
            set
            {
                multipliedOdds = value;
                OnPropertyChanged("MultipliedOdds");
            }
        }

        private decimal multipliedPlatsOdds;
        [DataMember]
        public decimal MultipliedPlatsOdds
        {
            get
            {
                return multipliedPlatsOdds;
            }
            set
            {
                multipliedPlatsOdds = value;
                OnPropertyChanged("MultipliedPlatsOdds");
            }
        }

        private decimal combinationOdds;
        [DataMember]
        public decimal CombinationOdds
        {
            get
            {
                return combinationOdds;
            }
            set
            {
                combinationOdds = value;
                OnPropertyChanged("CombinationOdds");
                //if (this.combinationOdds > 0 && this.ParentRaceDayInfo != null)
                //{
                //    //this.CombinationOddsShare = 10 * this.ParentRaceDayInfo.BetType.PoolShare / this.combinationOdds;
                //    this.CombinationOddsShare = this.ParentRaceDayInfo.BetType.PoolShare / this.CombinationOddsExact;
                //}
            }
        }

        private decimal _CombinationOddsExact;
        [DataMember]
        public decimal CombinationOddsExact
        {
            get
            {
                return _CombinationOddsExact;
            }
            set
            {
                _CombinationOddsExact = value;
                OnPropertyChanged("CombinationOddsExact");
                if (CombinationOddsExact > 0M && ParentRaceDayInfo != null)
                {
                    CombinationOddsShare = ParentRaceDayInfo.BetType.PoolShare / CombinationOddsExact;
                }
            }
        }

        private decimal calculatedOdds;
        [DataMember]
        public decimal CalculatedOdds
        {
            get
            {
                return calculatedOdds;
            }
            set
            {
                calculatedOdds = value;
                OnPropertyChanged("CalculatedOdds");
            }
        }

        private decimal calculatedOddsQuota;
        [DataMember]
        public decimal CalculatedOddsQuota
        {
            get
            {
                return calculatedOddsQuota;
            }
            set
            {
                calculatedOddsQuota = value;
                OnPropertyChanged("CalculatedOddsQuota");
            }
        }

        //public string MultipliedOddsString { get; set; }

        private decimal oddsQuota;
        [DataMember]
        public decimal OddsQuota
        {
            get
            {
                return oddsQuota;
            }
            set
            {
                oddsQuota = value;
                OnPropertyChanged("OddsQuota");
            }
        }

        private decimal vOdds;
        [DataMember]
        public decimal VOdds
        {
            get
            {
                return vOdds;
            }
            set
            {
                vOdds = value;
                OnPropertyChanged("VOdds");
            }
        }

        private decimal vQuota;
        [DataMember]
        public decimal VQuota
        {
            get
            {
                return vQuota;
            }
            set
            {
                vQuota = value;
                OnPropertyChanged("VQuota");
            }
        }

        private decimal pOdds;
        [DataMember]
        public decimal POdds
        {
            get
            {
                return pOdds;
            }
            set
            {
                pOdds = value;
                OnPropertyChanged("POdds");
            }
        }

        private decimal pQuota;
        [DataMember]
        public decimal PQuota
        {
            get
            {
                return pQuota;
            }
            set
            {
                pQuota = value;
                OnPropertyChanged("PQuota");
            }
        }

        private decimal vpOdds;
        [DataMember]
        public decimal VPOdds
        {
            get
            {
                return vpOdds;
            }
            set
            {
                vpOdds = value;
                OnPropertyChanged("VPOdds");
            }
        }

        private decimal vpQuota;
        [DataMember]
        public decimal VPQuota
        {
            get
            {
                return vpQuota;
            }
            set
            {
                vpQuota = value;
                OnPropertyChanged("VPQuota");
            }
        }

        private decimal opQuota;
        [DataMember]
        public decimal OPQuota
        {
            get
            {
                return opQuota;
            }
            set
            {
                opQuota = value;
                OnPropertyChanged("OPQuota");
            }
        }

        private decimal tvQuota;
        [DataMember]
        public decimal TVQuota
        {
            get
            {
                return tvQuota;
            }
            set
            {
                tvQuota = value;
                OnPropertyChanged("TVQuota");
            }
        }

        private decimal? tQuota;
        [DataMember]
        public decimal? TQuota
        {
            get
            {
                return tQuota;
            }
            set
            {
                tQuota = value;
                OnPropertyChanged("TQuota");
            }
        }

        private decimal? dQuota;
        [DataMember]
        public decimal? DQuota
        {
            get
            {
                return dQuota;
            }
            set
            {
                dQuota = value;
                OnPropertyChanged("DQuota");
            }
        }

        private decimal? stakeQuota;
        [DataMember]
        public decimal? StakeQuota
        {
            get
            {
                return stakeQuota;
            }
            set
            {
                stakeQuota = value;
                OnPropertyChanged("StakeQuota");
            }
        }

        private bool selected;
        [DataMember]
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }

        private int? stake;
        [DataMember]
        public int? Stake
        {
            get
            {
                return stake;
            }
            set
            {
                stake = value;
                OnPropertyChanged("Stake");
                if (value == null)
                {
                    return;
                }
                if (CombinationOdds == 9999 && CalculatedOdds != 0M)
                {
                    Profit = Convert.ToInt32(stake * CalculatedOdds);
                }
                else
                {
                    //this.Profit = Convert.ToInt32(this.stake * this.CombinationOdds / 10);
                    Profit = Convert.ToInt32(stake * CombinationOddsExact);
                }
            }
        }

        private int profit;
        [DataMember]
        public int Profit
        {
            get
            {
                return profit;
            }
            set
            {
                profit = value;
                switch (profit)
                {
                    case 0:
                        ProfitString = string.Empty;
                        break;
                    default:
                        ProfitString = string.Format("{0:## ### ###}", profit); ;
                        break;
                }
                OnPropertyChanged("Profit");
            }
        }

        private string profitString;
        [DataMember]
        public string ProfitString
        {
            get
            {
                return profitString;
            }
            set
            {
                profitString = value;
                OnPropertyChanged("ProfitString");
            }
        }

        public decimal CombinationOddsShare { get; set; }

        private int multipliedOddsRank;
        [DataMember]
        public int MultipliedOddsRank
        {
            get
            {
                return multipliedOddsRank;
            }
            set
            {
                multipliedOddsRank = value;
                OnPropertyChanged("MultipliedOddsRank");
            }
        }

        private int combinationOddsRank;
        [DataMember]
        public int CombinationOddsRank
        {
            get
            {
                return combinationOddsRank;
            }
            set
            {
                combinationOddsRank = value;
                OnPropertyChanged("CombinationOddsRank");
            }
        }

        private decimal playability;
        [DataMember]
        public decimal Playability
        {
            get
            {
                return playability;
            }
            set
            {
                playability = value;
                OnPropertyChanged("Playability");
            }
        }

        public string UniqueCode
        {
            get
            {
                if (Horse3 == null)
                {
                    return Horse1.HexCode + Horse2.HexCode;
                }

                else
                {
                    return Horse1.HexCode + Horse2.HexCode + Horse3.HexCode;
                }
            }
        }

        public void CalculateQuotas(string betType)
        {
            switch (betType)
            {
                case "DD":
                case "LD":
                    if (ParentRaceDayInfo == null)
                    {
                        return;
                    }
                    CalculateQuotasDD();
                    break;
                case "TV":
                    if (ParentRace == null || ParentRace.HorseList == null || ParentRace.HorseList.Count == 0)
                    {
                        return;
                    }
                    CalculateQuotasTvilling();
                    break;
                case "T":
                    if (ParentRace == null || ParentRace.HorseList == null || ParentRace.HorseList.Count == 0)
                    {
                        return;
                    }
                    CalculateQuotasTrio();
                    break;
                default:
                    break;
            }
        }

        public void CalculateQuotasDD()
        {
            try
            {
                MultipliedOdds = Horse1.VinnarOddsExact * Horse2.VinnarOddsExact;
                //this.OddsQuota = this.CombinationOdds / this.MultipliedOdds / 10;
                OddsQuota = CombinationOddsExact / MultipliedOdds;
                Playability = Convert.ToDecimal(Math.Sqrt(decimal.ToDouble(MultipliedOdds))) / (OddsQuota * OddsQuota);

                // NYA MÅTT
                if (CombinationOddsShare > 0M)
                {
                    VQuota = (Horse1.VinnarOddsShare * Horse2.VinnarOddsShare) / CombinationOddsShare;
                    PQuota = (Horse1.PlatsOddsShare * Horse2.PlatsOddsShare) / CombinationOddsShare;
                    TVQuota = (Horse1.TvillingShare * Horse2.TvillingShare) / CombinationOddsShare;
                    OPQuota = (Convert.ToDecimal(Horse1.OwnProbability) * Convert.ToDecimal(Horse2.OwnProbability)) / CombinationOddsShare;

                    if (Horse1.StakeShareAlternate > 0M && Horse2.StakeShareAlternate > 0M)
                    {
                        StakeQuota = (Convert.ToDecimal(Horse1.StakeShareAlternate) * Convert.ToDecimal(Horse2.StakeShareAlternate)) / CombinationOddsShare;
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public void CalculateQuotasTvilling()
        {
            // Andel kombinationer i förhållande till totalantal om det varit DD/LD istället
            int numberOfStartingHorse = ParentRace.HorseList.Count(h => h.Scratched == null || h.Scratched == false);
            decimal numberOfCombinationsQuota =
                Convert.ToDecimal(numberOfStartingHorse * (numberOfStartingHorse - 1) * 0.5M) /
                Convert.ToDecimal(numberOfStartingHorse * numberOfStartingHorse);

            MultipliedOdds = Horse1.VinnarOddsExact * Horse2.VinnarOddsExact;
            MultipliedPlatsOdds = Convert.ToDecimal(Horse1.MaxPlatsOdds * Horse2.MaxPlatsOdds) / 100;

            if (MultipliedOdds != 0M && MultipliedPlatsOdds != 0M && combinationOdds != 0M)
            {
                //decimal vinnarOddsQuota = this.CombinationOdds / 10 / numberOfCombinationsQuota / this.MultipliedOdds;
                //decimal platsOddsQuota = this.CombinationOdds / 10 / this.MultipliedPlatsOdds;
                decimal vinnarOddsQuota = CombinationOddsExact / numberOfCombinationsQuota / MultipliedOdds;
                decimal platsOddsQuota = CombinationOddsExact / MultipliedPlatsOdds;
                OddsQuota = vinnarOddsQuota;
                Playability = Convert.ToDecimal(Math.Sqrt(decimal.ToDouble(MultipliedOdds + MultipliedPlatsOdds))) / (OddsQuota * OddsQuota * platsOddsQuota * platsOddsQuota);
            }

            // NYA MÅTT
            decimal adjustedCombinationShare = CombinationOddsShare * numberOfCombinationsQuota;
            if (CombinationOddsShare > 0M && adjustedCombinationShare > 0M)
            {
                decimal vpShare = (Horse1.VinnarOddsShare * Horse2.PlatsOddsShare) + (Horse1.PlatsOddsShare * Horse2.VinnarOddsShare);
                if (vpShare > 0M)
                {
                    VPQuota = vpShare / adjustedCombinationShare / 2M;
                    VPOdds = CombinationOdds / VPQuota;
                }

                decimal vShare = Horse1.VinnarOddsShare * Horse2.VinnarOddsShare;
                if (vShare > 0M)
                {
                    VQuota = vShare / adjustedCombinationShare;
                    VOdds = CombinationOdds / VQuota;
                }

                decimal pShare = Horse1.PlatsOddsShare * Horse2.PlatsOddsShare;
                if (pShare > 0M)
                {
                    PQuota = pShare / adjustedCombinationShare;
                    POdds = CombinationOdds / PQuota;
                }

                OPQuota = (Convert.ToDecimal(Horse1.OwnProbability) * Convert.ToDecimal(Horse2.OwnProbability)) / adjustedCombinationShare;

                if (Horse1.StakeShareAlternate > 0M && Horse2.StakeShareAlternate > 0M)
                {
                    StakeQuota = (Convert.ToDecimal(Horse1.StakeShareAlternate) * Convert.ToDecimal(Horse2.StakeShareAlternate)) / adjustedCombinationShare;
                }
                if (Horse1.DoubleShare > 0M && Horse2.DoubleShare > 0M)
                {
                    DQuota = (Convert.ToDecimal(Horse1.DoubleShare) * Convert.ToDecimal(Horse2.DoubleShare)) / adjustedCombinationShare;
                }
            }
        }

        public void CalculateQuotasTrio()
        {
            try
            {
                MultipliedOdds = Horse1.VinnarOddsExact * Horse2.VinnarOddsExact * Horse3.VinnarOddsExact;
                //this.OddsQuota = this.CombinationOdds / this.MultipliedOdds / 10;
                OddsQuota = CombinationOddsExact / MultipliedOdds;

                //this.CalculatedOdds = 0.343M / (this.Horse1.TrioInfo.PlaceInfo1.InvestmentShare * this.Horse2.TrioInfo.PlaceInfo2.InvestmentShare * this.Horse3.TrioInfo.PlaceInfo3.InvestmentShare);

                // Beräkna vad oddset "borde" vara utifrån hur Trio-spelarena spelat
                CalculatedOdds = 0.7M / (Horse1.TrioInfo.PlaceInfo1.InvestmentShare * Horse2.TrioInfo.PlaceInfo2.InvestmentShare * Horse3.TrioInfo.PlaceInfo3.InvestmentShare);


                if (CombinationOdds == 0M || CombinationOdds == 9999M)
                {
                    CalculatedOddsQuota = 0M;
                    VPQuota = 0M;
                    VQuota = 0M;
                    PQuota = 0M;
                }
                else
                {
                    // Vad oddset borde vara utifrån de olika insatserna per placering
                    CalculatedOddsQuota = CalculatedOdds / CombinationOdds * 10M;

                    // Beräkna vad oddset "borde" varit utifrån VP
                    decimal vpShare = Horse1.VinnarOddsShare * Horse2.PlatsOddsShare * Horse3.PlatsOddsShare;
                    if (vpShare > 0M)
                    {
                        VPOdds = 0.7M / vpShare * 10M;
                        VPQuota = CombinationOdds / VPOdds;
                    }

                    // Beräkna vad oddset "borde" varit utifrån Vinnare
                    decimal vShare = Horse1.VinnarOddsShare * Horse2.VinnarOddsShare * Horse3.VinnarOddsShare;
                    if (vShare > 0M)
                    {
                        VOdds = 0.7M / vShare * 10M;
                        VQuota = CombinationOdds / VOdds;
                    }

                    // Beräkna vad oddset "borde" varit utifrån Plats
                    decimal pShare = Horse1.PlatsOddsShare * Horse2.PlatsOddsShare * Horse3.PlatsOddsShare;
                    if (pShare > 0M)
                    {
                        POdds = 0.7M / pShare * 10M;
                        PQuota = CombinationOdds / POdds;
                    }
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void SetColorsDD()
        {
            Color c = Colors.White;
            if (VQuota > 1.2M)
            {
                c = HPTConfig.Config.ColorGood;
            }
            else if (VQuota > 1M)
            {
                c = HPTConfig.Config.ColorMedium;
            }
            else
            {
                c = HPTConfig.Config.ColorBad;
            }
            OddsQuotaColor = new SolidColorBrush(c);
        }

        private void SetColorsTvilling()
        {
            // Färg för oddskvot
            Color c = Colors.White;
            if (VPQuota > 1.2M)
            {
                c = HPTConfig.Config.ColorGood;
            }
            else if (VPQuota > 1M)
            {
                c = HPTConfig.Config.ColorMedium;
            }
            else
            {
                c = HPTConfig.Config.ColorBad;
            }
            OddsQuotaColor = new SolidColorBrush(c);
        }

        private void SetColorsTrio()
        {
            // Färg för oddskvot
            Color c = Colors.White;
            if (VPQuota > 1.2M)
            {
                c = HPTConfig.Config.ColorGood;
            }
            else if (VPQuota > 1M)
            {
                c = HPTConfig.Config.ColorMedium;
            }
            else
            {
                c = HPTConfig.Config.ColorBad;
            }
            OddsQuotaColor = new SolidColorBrush(c);

            //// Färg för differens i rank
            //int rankDiff = this.CombinationOddsRank - this.MultipliedOddsRank;
            //Color cRankDiff = Colors.White;
            //if (rankDiff > 1)
            //{
            //    cRankDiff = HPTConfig.Config.ColorGood;
            //}
            //else if (rankDiff < 1)
            //{
            //    cRankDiff = HPTConfig.Config.ColorBad;
            //}
            //else
            //{
            //    cRankDiff = HPTConfig.Config.ColorMedium;
            //}
            //this.RankDiffColor = new LinearGradientBrush(cRankDiff, Colors.White, 90.0);

            //// Färg för spelbarhet
            //Color cPlayabilty = Colors.White;
            //if (this.Playability < 0.9M)
            //{
            //    cPlayabilty = HPTConfig.Config.ColorBad;
            //}
            //else if (this.Playability < 1.1M)
            //{
            //    cPlayabilty = HPTConfig.Config.ColorMedium;
            //}
            //else
            //{
            //    cPlayabilty = HPTConfig.Config.ColorGood;
            //}
            //this.PlayabilityColor = new SolidColorBrush(cPlayabilty);
        }

        public string ClipboardString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Horse1.StartNr);
                sb.Append(" - ");
                sb.Append(Horse1.HorseName);
                sb.Append(" / ");
                sb.Append(Horse2.StartNr);
                sb.Append(" - ");
                sb.Append(Horse2.HorseName);
                sb.Append(" (");
                sb.Append(Stake);
                sb.Append(")");
                return sb.ToString();
            }
        }

        private Brush oddsQuotaColor;
        [XmlIgnore]
        public Brush OddsQuotaColor
        {
            get
            {
                if (oddsQuotaColor == null)
                {
                    switch (ParentRaceDayInfo.BetType.Code)
                    {
                        case "DD":
                        case "LD":
                            SetColorsDD();
                            break;
                        case "TV":
                            SetColorsTvilling();
                            break;
                        case "T":
                            SetColorsTrio();
                            break;
                        default:
                            break;
                    }
                }
                return oddsQuotaColor;
            }
            set
            {
                oddsQuotaColor = value;
                OnPropertyChanged("OddsQuotaColor");
            }
        }

        private Brush rankDiffColor;
        [XmlIgnore]
        public Brush RankDiffColor
        {
            get
            {
                if (rankDiffColor == null)
                {
                    switch (ParentRaceDayInfo.BetType.Code)
                    {
                        case "DD":
                        case "LD":
                            SetColorsDD();
                            break;
                        case "TV":
                            SetColorsTvilling();
                            break;
                        case "T":
                            SetColorsTrio();
                            break;
                        default:
                            break;
                    }
                }
                return rankDiffColor;
            }
            set
            {
                rankDiffColor = value;
                OnPropertyChanged("RankDiffColor");
            }
        }

        private Brush playabilityColor;
        [XmlIgnore]
        public Brush PlayabilityColor
        {
            get
            {
                if (playabilityColor == null)
                {
                    switch (ParentRaceDayInfo.BetType.Code)
                    {
                        case "DD":
                        case "LD":
                            SetColorsDD();
                            break;
                        case "TV":
                            SetColorsTvilling();
                            break;
                        case "T":
                            SetColorsTrio();
                            break;
                        default:
                            break;
                    }
                }
                return playabilityColor;
            }
            set
            {
                playabilityColor = value;
                OnPropertyChanged("PlayabilityColor");
            }
        }
    }
}

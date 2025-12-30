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
                return this.multipliedOdds;
            }
            set
            {
                this.multipliedOdds = value;
                OnPropertyChanged("MultipliedOdds");
            }
        }

        private decimal multipliedPlatsOdds;
        [DataMember]
        public decimal MultipliedPlatsOdds
        {
            get
            {
                return this.multipliedPlatsOdds;
            }
            set
            {
                this.multipliedPlatsOdds = value;
                OnPropertyChanged("MultipliedPlatsOdds");
            }
        }

        private decimal combinationOdds;
        [DataMember]
        public decimal CombinationOdds
        {
            get
            {
                return this.combinationOdds;
            }
            set
            {
                this.combinationOdds = value;
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
                this._CombinationOddsExact = value;
                OnPropertyChanged("CombinationOddsExact");
                if (this.CombinationOddsExact > 0M && this.ParentRaceDayInfo != null)
                {
                    this.CombinationOddsShare = this.ParentRaceDayInfo.BetType.PoolShare / this.CombinationOddsExact;
                }
            }
        }

        private decimal calculatedOdds;
        [DataMember]
        public decimal CalculatedOdds
        {
            get
            {
                return this.calculatedOdds;
            }
            set
            {
                this.calculatedOdds = value;
                OnPropertyChanged("CalculatedOdds");
            }
        }

        private decimal calculatedOddsQuota;
        [DataMember]
        public decimal CalculatedOddsQuota
        {
            get
            {
                return this.calculatedOddsQuota;
            }
            set
            {
                this.calculatedOddsQuota = value;
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
                return this.oddsQuota;
            }
            set
            {
                this.oddsQuota = value;
                OnPropertyChanged("OddsQuota");
            }
        }

        private decimal vOdds;
        [DataMember]
        public decimal VOdds
        {
            get
            {
                return this.vOdds;
            }
            set
            {
                this.vOdds = value;
                OnPropertyChanged("VOdds");
            }
        }

        private decimal vQuota;
        [DataMember]
        public decimal VQuota
        {
            get
            {
                return this.vQuota;
            }
            set
            {
                this.vQuota = value;
                OnPropertyChanged("VQuota");
            }
        }

        private decimal pOdds;
        [DataMember]
        public decimal POdds
        {
            get
            {
                return this.pOdds;
            }
            set
            {
                this.pOdds = value;
                OnPropertyChanged("POdds");
            }
        }

        private decimal pQuota;
        [DataMember]
        public decimal PQuota
        {
            get
            {
                return this.pQuota;
            }
            set
            {
                this.pQuota = value;
                OnPropertyChanged("PQuota");
            }
        }

        private decimal vpOdds;
        [DataMember]
        public decimal VPOdds
        {
            get
            {
                return this.vpOdds;
            }
            set
            {
                this.vpOdds = value;
                OnPropertyChanged("VPOdds");
            }
        }

        private decimal vpQuota;
        [DataMember]
        public decimal VPQuota
        {
            get
            {
                return this.vpQuota;
            }
            set
            {
                this.vpQuota = value;
                OnPropertyChanged("VPQuota");
            }
        }

        private decimal opQuota;
        [DataMember]
        public decimal OPQuota
        {
            get
            {
                return this.opQuota;
            }
            set
            {
                this.opQuota = value;
                OnPropertyChanged("OPQuota");
            }
        }

        private decimal tvQuota;
        [DataMember]
        public decimal TVQuota
        {
            get
            {
                return this.tvQuota;
            }
            set
            {
                this.tvQuota = value;
                OnPropertyChanged("TVQuota");
            }
        }

        private decimal? tQuota;
        [DataMember]
        public decimal? TQuota
        {
            get
            {
                return this.tQuota;
            }
            set
            {
                this.tQuota = value;
                OnPropertyChanged("TQuota");
            }
        }

        private decimal? dQuota;
        [DataMember]
        public decimal? DQuota
        {
            get
            {
                return this.dQuota;
            }
            set
            {
                this.dQuota = value;
                OnPropertyChanged("DQuota");
            }
        }

        private decimal? stakeQuota;
        [DataMember]
        public decimal? StakeQuota
        {
            get
            {
                return this.stakeQuota;
            }
            set
            {
                this.stakeQuota = value;
                OnPropertyChanged("StakeQuota");
            }
        }

        private bool selected;
        [DataMember]
        public bool Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                OnPropertyChanged("Selected");
            }
        }

        private int? stake;
        [DataMember]
        public int? Stake
        {
            get
            {
                return this.stake;
            }
            set
            {
                this.stake = value;
                OnPropertyChanged("Stake");
                if (value == null)
                {
                    return;
                }
                if (this.CombinationOdds == 9999 && this.CalculatedOdds != 0M)
                {
                    this.Profit = Convert.ToInt32(this.stake * this.CalculatedOdds);
                }
                else
                {
                    //this.Profit = Convert.ToInt32(this.stake * this.CombinationOdds / 10);
                    this.Profit = Convert.ToInt32(this.stake * this.CombinationOddsExact);
                }
            }
        }

        private int profit;
        [DataMember]
        public int Profit
        {
            get
            {
                return this.profit;
            }
            set
            {
                this.profit = value;
                switch (this.profit)
                {
                    case 0:
                        this.ProfitString = string.Empty;
                        break;
                    default:
                        this.ProfitString = string.Format("{0:## ### ###}", this.profit); ;
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
                return this.profitString;
            }
            set
            {
                this.profitString = value;
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
                return this.multipliedOddsRank;
            }
            set
            {
                this.multipliedOddsRank = value;
                OnPropertyChanged("MultipliedOddsRank");
            }
        }

        private int combinationOddsRank;
        [DataMember]
        public int CombinationOddsRank
        {
            get
            {
                return this.combinationOddsRank;
            }
            set
            {
                this.combinationOddsRank = value;
                OnPropertyChanged("CombinationOddsRank");
            }
        }

        private decimal playability;
        [DataMember]
        public decimal Playability
        {
            get
            {
                return this.playability;
            }
            set
            {
                this.playability = value;
                OnPropertyChanged("Playability");
            }
        }

        public string UniqueCode
        {
            get
            {
                if (this.Horse3 == null)
                {
                    return this.Horse1.HexCode + this.Horse2.HexCode;
                }

                else
                {
                    return this.Horse1.HexCode + this.Horse2.HexCode + this.Horse3.HexCode;
                }
            }
        }

        public void CalculateQuotas(string betType)
        {
            switch (betType)
            {
                case "DD":
                case "LD":
                    if (this.ParentRaceDayInfo == null)
                    {
                        return;
                    }
                    CalculateQuotasDD();
                    break;
                case "TV":
                    if (this.ParentRace == null || this.ParentRace.HorseList == null || this.ParentRace.HorseList.Count == 0)
                    {
                        return;
                    }
                    CalculateQuotasTvilling();
                    break;
                case "T":
                    if (this.ParentRace == null || this.ParentRace.HorseList == null || this.ParentRace.HorseList.Count == 0)
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
                this.MultipliedOdds = this.Horse1.VinnarOddsExact * this.Horse2.VinnarOddsExact;
                //this.OddsQuota = this.CombinationOdds / this.MultipliedOdds / 10;
                this.OddsQuota = this.CombinationOddsExact / this.MultipliedOdds;
                this.Playability = Convert.ToDecimal(Math.Sqrt(decimal.ToDouble(this.MultipliedOdds))) / (this.OddsQuota * this.OddsQuota);

                // NYA MÅTT
                if (this.CombinationOddsShare > 0M)
                {
                    this.VQuota = (this.Horse1.VinnarOddsShare * this.Horse2.VinnarOddsShare) / this.CombinationOddsShare;
                    this.PQuota = (this.Horse1.PlatsOddsShare * this.Horse2.PlatsOddsShare) / this.CombinationOddsShare;
                    this.TVQuota = (this.Horse1.TvillingShare * this.Horse2.TvillingShare) / this.CombinationOddsShare;
                    this.OPQuota = (Convert.ToDecimal(this.Horse1.OwnProbability) * Convert.ToDecimal(this.Horse2.OwnProbability)) / this.CombinationOddsShare;

                    if (this.Horse1.StakeShareAlternate > 0M && this.Horse2.StakeShareAlternate > 0M)
                    {
                        this.StakeQuota = (Convert.ToDecimal(this.Horse1.StakeShareAlternate) * Convert.ToDecimal(this.Horse2.StakeShareAlternate)) / this.CombinationOddsShare;
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
            int numberOfStartingHorse = this.ParentRace.HorseList.Count(h => h.Scratched == null || h.Scratched == false);
            decimal numberOfCombinationsQuota =
                Convert.ToDecimal(numberOfStartingHorse * (numberOfStartingHorse - 1) * 0.5M) /
                Convert.ToDecimal(numberOfStartingHorse * numberOfStartingHorse);

            this.MultipliedOdds = this.Horse1.VinnarOddsExact * this.Horse2.VinnarOddsExact;
            this.MultipliedPlatsOdds = Convert.ToDecimal(this.Horse1.MaxPlatsOdds * this.Horse2.MaxPlatsOdds) / 100;

            if (this.MultipliedOdds != 0M && this.MultipliedPlatsOdds != 0M && this.combinationOdds != 0M)
            {
                //decimal vinnarOddsQuota = this.CombinationOdds / 10 / numberOfCombinationsQuota / this.MultipliedOdds;
                //decimal platsOddsQuota = this.CombinationOdds / 10 / this.MultipliedPlatsOdds;
                decimal vinnarOddsQuota = this.CombinationOddsExact / numberOfCombinationsQuota / this.MultipliedOdds;
                decimal platsOddsQuota = this.CombinationOddsExact / this.MultipliedPlatsOdds;
                this.OddsQuota = vinnarOddsQuota;
                this.Playability = Convert.ToDecimal(Math.Sqrt(decimal.ToDouble(this.MultipliedOdds + this.MultipliedPlatsOdds))) / (this.OddsQuota * this.OddsQuota * platsOddsQuota * platsOddsQuota);
            }

            // NYA MÅTT
            decimal adjustedCombinationShare = this.CombinationOddsShare * numberOfCombinationsQuota;
            if (this.CombinationOddsShare > 0M && adjustedCombinationShare > 0M)
            {
                decimal vpShare = (this.Horse1.VinnarOddsShare * this.Horse2.PlatsOddsShare) + (this.Horse1.PlatsOddsShare * this.Horse2.VinnarOddsShare);
                if (vpShare > 0M)
                {
                    this.VPQuota = vpShare / adjustedCombinationShare / 2M;
                    this.VPOdds = this.CombinationOdds / this.VPQuota;
                }

                decimal vShare = this.Horse1.VinnarOddsShare * this.Horse2.VinnarOddsShare;
                if (vShare > 0M)
                {
                    this.VQuota = vShare / adjustedCombinationShare;
                    this.VOdds = this.CombinationOdds / this.VQuota;
                }

                decimal pShare = this.Horse1.PlatsOddsShare * this.Horse2.PlatsOddsShare;
                if (pShare > 0M)
                {
                    this.PQuota = pShare / adjustedCombinationShare;
                    this.POdds = this.CombinationOdds / this.PQuota;
                }

                this.OPQuota = (Convert.ToDecimal(this.Horse1.OwnProbability) * Convert.ToDecimal(this.Horse2.OwnProbability)) / adjustedCombinationShare;

                if (this.Horse1.StakeShareAlternate > 0M && this.Horse2.StakeShareAlternate > 0M)
                {
                    this.StakeQuota = (Convert.ToDecimal(this.Horse1.StakeShareAlternate) * Convert.ToDecimal(this.Horse2.StakeShareAlternate)) / adjustedCombinationShare;
                }
                if (this.Horse1.DoubleShare > 0M && this.Horse2.DoubleShare > 0M)
                {
                    this.DQuota = (Convert.ToDecimal(this.Horse1.DoubleShare) * Convert.ToDecimal(this.Horse2.DoubleShare)) / adjustedCombinationShare;
                }
            }
        }

        public void CalculateQuotasTrio()
        {
            try
            {
                this.MultipliedOdds = this.Horse1.VinnarOddsExact * this.Horse2.VinnarOddsExact * this.Horse3.VinnarOddsExact;
                //this.OddsQuota = this.CombinationOdds / this.MultipliedOdds / 10;
                this.OddsQuota = this.CombinationOddsExact / this.MultipliedOdds;

                //this.CalculatedOdds = 0.343M / (this.Horse1.TrioInfo.PlaceInfo1.InvestmentShare * this.Horse2.TrioInfo.PlaceInfo2.InvestmentShare * this.Horse3.TrioInfo.PlaceInfo3.InvestmentShare);

                // Beräkna vad oddset "borde" vara utifrån hur Trio-spelarena spelat
                this.CalculatedOdds = 0.7M / (this.Horse1.TrioInfo.PlaceInfo1.InvestmentShare * this.Horse2.TrioInfo.PlaceInfo2.InvestmentShare * this.Horse3.TrioInfo.PlaceInfo3.InvestmentShare);


                if (this.CombinationOdds == 0M || this.CombinationOdds == 9999M)
                {
                    this.CalculatedOddsQuota = 0M;
                    this.VPQuota = 0M;
                    this.VQuota = 0M;
                    this.PQuota = 0M;
                }
                else
                {
                    // Vad oddset borde vara utifrån de olika insatserna per placering
                    this.CalculatedOddsQuota = this.CalculatedOdds / this.CombinationOdds * 10M;

                    // Beräkna vad oddset "borde" varit utifrån VP
                    decimal vpShare = this.Horse1.VinnarOddsShare * this.Horse2.PlatsOddsShare * this.Horse3.PlatsOddsShare;
                    if (vpShare > 0M)
                    {
                        this.VPOdds = 0.7M / vpShare * 10M;
                        this.VPQuota = this.CombinationOdds / this.VPOdds;
                    }

                    // Beräkna vad oddset "borde" varit utifrån Vinnare
                    decimal vShare = this.Horse1.VinnarOddsShare * this.Horse2.VinnarOddsShare * this.Horse3.VinnarOddsShare;
                    if (vShare > 0M)
                    {
                        this.VOdds = 0.7M / vShare * 10M;
                        this.VQuota = this.CombinationOdds / this.VOdds;
                    }

                    // Beräkna vad oddset "borde" varit utifrån Plats
                    decimal pShare = this.Horse1.PlatsOddsShare * this.Horse2.PlatsOddsShare * this.Horse3.PlatsOddsShare;
                    if (pShare > 0M)
                    {
                        this.POdds = 0.7M / pShare * 10M;
                        this.PQuota = this.CombinationOdds / this.POdds;
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
            if (this.VQuota > 1.2M)
            {
                c = HPTConfig.Config.ColorGood;
            }
            else if (this.VQuota > 1M)
            {
                c = HPTConfig.Config.ColorMedium;
            }
            else
            {
                c = HPTConfig.Config.ColorBad;
            }
            this.OddsQuotaColor = new SolidColorBrush(c);
        }

        private void SetColorsTvilling()
        {
            // Färg för oddskvot
            Color c = Colors.White;
            if (this.VPQuota > 1.2M)
            {
                c = HPTConfig.Config.ColorGood;
            }
            else if (this.VPQuota > 1M)
            {
                c = HPTConfig.Config.ColorMedium;
            }
            else
            {
                c = HPTConfig.Config.ColorBad;
            }
            this.OddsQuotaColor = new SolidColorBrush(c);
        }

        private void SetColorsTrio()
        {
            // Färg för oddskvot
            Color c = Colors.White;
            if (this.VPQuota > 1.2M)
            {
                c = HPTConfig.Config.ColorGood;
            }
            else if (this.VPQuota > 1M)
            {
                c = HPTConfig.Config.ColorMedium;
            }
            else
            {
                c = HPTConfig.Config.ColorBad;
            }
            this.OddsQuotaColor = new SolidColorBrush(c);

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
                sb.Append(this.Horse1.StartNr);
                sb.Append(" - ");
                sb.Append(this.Horse1.HorseName);
                sb.Append(" / ");
                sb.Append(this.Horse2.StartNr);
                sb.Append(" - ");
                sb.Append(this.Horse2.HorseName);
                sb.Append(" (");
                sb.Append(this.Stake);
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
                if (this.oddsQuotaColor == null)
                {
                    switch (this.ParentRaceDayInfo.BetType.Code)
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
                return this.oddsQuotaColor;
            }
            set
            {
                this.oddsQuotaColor = value;
                OnPropertyChanged("OddsQuotaColor");
            }
        }

        private Brush rankDiffColor;
        [XmlIgnore]
        public Brush RankDiffColor
        {
            get
            {
                if (this.rankDiffColor == null)
                {
                    switch (this.ParentRaceDayInfo.BetType.Code)
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
                return this.rankDiffColor;
            }
            set
            {
                this.rankDiffColor = value;
                OnPropertyChanged("RankDiffColor");
            }
        }

        private Brush playabilityColor;
        [XmlIgnore]
        public Brush PlayabilityColor
        {
            get
            {
                if (this.playabilityColor == null)
                {
                    switch (this.ParentRaceDayInfo.BetType.Code)
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
                return this.playabilityColor;
            }
            set
            {
                this.playabilityColor = value;
                OnPropertyChanged("PlayabilityColor");
            }
        }
    }
}

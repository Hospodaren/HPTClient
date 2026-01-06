using ATGDownloader;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorse : Notifier
    {
        public HPTHorse()
        {
            HorseXReductionList = new ObservableCollection<HPTHorseXReduction>();
            RankList = new ObservableCollection<HPTHorseRank>();
        }

        #region Funktionalitet för deserialisering

        public void CreateXReductionRuleList()
        {
            foreach (HPTPrio prio in HPTConfig.Config.PrioList.Keys)
            {
                HPTHorseXReduction horseXReduction = new HPTHorseXReduction();
                horseXReduction.Prio = prio;
                horseXReduction.Selectable = HPTConfig.Config.PrioList[prio];
                horseXReduction.Horse = this;
                //horseXReduction.PropertyChanged += horseXReduction_PropertyChanged;
                HorseXReductionList.Add(horseXReduction);
            }
        }

        //void horseXReduction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    var xReduction = (HPTHorseXReduction)sender;
        //    if (this.ParentRace != null && e.PropertyName == "Selected" && xReduction.Selected)
        //    {
        //        this.ParentRace.SetNumberOfSelectedHorses(this);
        //    }
        //}


        #region Conversion

        public void Merge(ATGHorseBase horse)
        {
            // TODO: Uppdatera ALLT i den här metoden...
            //// Sen kuskändring
            //this.DriverChanged = !string.IsNullOrWhiteSpace(horse.NewDriverName);
            //if (this.DriverChanged == true)
            //{
            //    this.DriverName = horse.NewDriverName;
            //    this.DriverNameShort = horse.NewDriverNameShort;
            //}

            //// Streck
            //this.MarksPossibleValue = horse.MarksPossibleValue > 0 ? horse.MarksPossibleValue : this.MarksPossibleValue;

            //// Insatsfördelning
            //this.StakeDistribution = horse.StakeDistribution > 0 ? horse.StakeDistribution : this.StakeDistribution;
            //this.StakeDistributionPercent = horse.StakeDistributionPercent > 0 ? horse.StakeDistributionPercent : this.StakeDistributionPercent;

            //// Sen strykning
            //if (horse.Scratched == true)
            //{
            //    this.Scratched = horse.Scratched;
            //    //this.VinnaroddsColor = new SolidColorBrush(Colors.Gray);
            //}

            //// Vinnare
            //this.VinnarOdds = horse.VPInfo.VinnarOdds > 0 ? horse.VPInfo.VinnarOdds : this.VinnarOdds;  // Bakåtkompatibilitet
            //this.VinnarOddsExact = horse.VPInfo.VinnarOddsExact > 0M ? horse.VPInfo.VinnarOddsExact : this.VinnarOddsExact;
            //this.InvestmentVinnare = horse.VPInfo.InvestmentVinnare;

            //// Plats
            //this.MinPlatsOdds = horse.VPInfo.MinPlatsOdds > 0 ? horse.VPInfo.MinPlatsOdds : this.MinPlatsOdds;
            //this.MaxPlatsOdds = horse.VPInfo.MaxPlatsOdds > 0 ? horse.VPInfo.MaxPlatsOdds : this.MaxPlatsOdds;
            //this.MinPlatsOddsExact = horse.VPInfo.MinPlatsOddsExact > 0M ? horse.VPInfo.MinPlatsOddsExact : this.MinPlatsOddsExact;
            //this.MaxPlatsOddsExact = horse.VPInfo.MaxPlatsOddsExact > 0M ? horse.VPInfo.MaxPlatsOddsExact : this.MaxPlatsOddsExact;
            //this.InvestmentPlats = horse.VPInfo.InvestmentPlats;

            //// Shareinfo
            //this.DoubleShare = horse.ShareInfo.DoubleShare > 0 ? horse.ShareInfo.DoubleShare : this.DoubleShare;
            //this.TrioShare = horse.ShareInfo.TrioShare > 0 ? horse.ShareInfo.TrioShare : this.TrioShare;
            //this.TvillingShare = horse.ShareInfo.TvillingShare > 0 ? horse.ShareInfo.TvillingShare : this.TvillingShare;

            //switch (this.ParentRace.ParentRaceDayInfo.BetType.Code)
            //{
            //    case "V65":
            //    case "V75":
            //    case "V85":
            //    case "GS75":
            //    case "V86":
            //    case "V64":
            //        this.StakeShareAlternate = horse.ShareInfo.StakeShareV4 > 0 ? horse.ShareInfo.StakeShareV4 : this.StakeShareAlternate;
            //        this.StakeShareAlternate2 = horse.ShareInfo.StakeShareV5 > 0 ? horse.ShareInfo.StakeShareV5 : this.StakeShareAlternate2;
            //        break;
            //    //case "V3":
            //    case "V4":
            //        this.StakeShareAlternate = horse.ShareInfo.StakeShareVxx > 0 ? horse.ShareInfo.StakeShareVxx : this.StakeShareAlternate;
            //        this.StakeShareAlternate2 = horse.ShareInfo.StakeShareV5 > 0 ? horse.ShareInfo.StakeShareV5 : this.StakeShareAlternate2;
            //        break;
            //    case "V5":
            //        this.StakeShareAlternate = horse.ShareInfo.StakeShareVxx > 0 ? horse.ShareInfo.StakeShareVxx : this.StakeShareAlternate;
            //        this.StakeShareAlternate2 = horse.ShareInfo.StakeShareV4 > 0 ? horse.ShareInfo.StakeShareV4 : this.StakeShareAlternate2;
            //        break;
            //    case "DD":
            //    case "LD":
            //        this.StakeDistributionPercent = Convert.ToInt32(horse.ShareInfo.DoubleShare * 100);
            //        this.StakeDistributionShare = horse.ShareInfo.DoubleShare;
            //        break;
            //    case "TV":
            //        this.StakeDistributionPercent = Convert.ToInt32(horse.ShareInfo.TvillingShare * 100);
            //        this.StakeDistributionShare = horse.ShareInfo.TvillingShare;
            //        break;
            //    case "T":
            //        this.StakeDistributionPercent = Convert.ToInt32(horse.ShareInfo.TrioShare * 100);
            //        this.StakeDistributionShare = horse.ShareInfo.TrioShare;
            //        break;
            //    default:
            //        break;
            //}

            //switch (this.ParentRace.ParentRaceDayInfo.BetType.Code)
            //{
            //    case "DD":
            //    case "LD":
            //    case "TV":
            //    case "T":
            //        if (horse.ShareInfo.StakeShareVxx > 0)
            //        {
            //            this.StakeShareAlternate = horse.ShareInfo.StakeShareVxx;
            //            if (horse.ShareInfo.StakeShareV4 > 0)
            //            {
            //                this.StakeShareAlternate2 = horse.ShareInfo.StakeShareV4;
            //            }
            //            else
            //            {
            //                this.StakeShareAlternate2 = horse.ShareInfo.StakeShareV5;
            //            }
            //        }
            //        else if (horse.ShareInfo.StakeShareV4 > 0)
            //        {
            //            this.StakeShareAlternate = horse.ShareInfo.StakeShareV4;
            //            this.StakeShareAlternate2 = horse.ShareInfo.StakeShareV5;
            //        }
            //        else
            //        {
            //            this.StakeShareAlternate = horse.ShareInfo.StakeShareV5;
            //        }
            //        break;
            //    default:
            //        break;
            //}

            //// Skoinformation
            //if (horse.ShoeInfoCurrent != null)
            //{
            //    this.ShoeInfoCurrent.Foreshoes = horse.ShoeInfoCurrent.Foreshoes;
            //    this.ShoeInfoCurrent.Hindshoes = horse.ShoeInfoCurrent.Hindshoes;
            //}
            //else
            //{
            //    this.ShoeInfoCurrent = new HPTHorseShoeInfo();
            //}

            //if (horse.ShoeInfoPrevious != null)
            //{
            //    this.ShoeInfoPrevious.Foreshoes = horse.ShoeInfoPrevious.Foreshoes;
            //    this.ShoeInfoPrevious.Hindshoes = horse.ShoeInfoPrevious.Hindshoes;
            //}
            //else
            //{
            //    this.ShoeInfoPrevious = new HPTHorseShoeInfo();
            //}

            //if (horse.ShoeInfoCurrent != null && horse.ShoeInfoPrevious != null)
            //{
            //    if (this.ShoeInfoCurrent.Foreshoes == null && this.ShoeInfoCurrent.Hindshoes == null)
            //    {
            //        this.ShoeInfoCurrent.Foreshoes = this.ShoeInfoPrevious.Foreshoes;
            //        this.ShoeInfoCurrent.Hindshoes = this.ShoeInfoPrevious.Hindshoes;
            //        this.ShoeInfoCurrent.PreviousUsed = true;
            //    }
            //    else
            //    {
            //        this.ShoeInfoCurrent.SetChangedFlags(this.ShoeInfoPrevious);
            //        this.ShoeInfoCurrent.PreviousUsed = false;
            //    }
            //}

            //// Vagninformation
            //if (this.SulkyInfoCurrent == null)
            //{
            //    this.SulkyInfoCurrent = new HPTHorseSulkyInfo()
            //    {
            //        Text = horse.StartInfo.SulkyInfoCurrent.Text
            //    };
            //}
            //else if (horse.StartInfo != null && horse.StartInfo.SulkyInfoCurrent != null)
            //{
            //    this.SulkyInfoCurrent.Text = horse.StartInfo.SulkyInfoCurrent.Text;
            //}

            //if (this.SulkyInfoPrevious == null)
            //{
            //    this.SulkyInfoPrevious = new HPTHorseSulkyInfo()
            //    {
            //        Text = horse.StartInfo.SulkyInfoPrevious.Text
            //    };
            //}
            //else if (horse.StartInfo != null && horse.StartInfo.SulkyInfoPrevious != null)
            //{
            //    this.SulkyInfoPrevious.Text = horse.StartInfo.SulkyInfoPrevious.Text;
            //}

            //if (!string.IsNullOrEmpty(this.SulkyInfoCurrent?.Text) && !string.IsNullOrEmpty(this.SulkyInfoPrevious?.Text) && this.SulkyInfoCurrent.Text != this.SulkyInfoPrevious.Text)
            //{
            //    this.SulkyInfoCurrent.SulkyChanged = true;
            //}

            //// Trio
            //if (this.ParentRace.ParentRaceDayInfo.BetType.Code == "T")
            //{
            //    if (horse.TrioInfo != null)
            //    {
            //        if (this.TrioInfo == null)
            //        {
            //            this.TrioInfo = new HPTHorseTrioInfo()
            //            {
            //                TrioIndex = horse.TrioInfo.TrioIndex,
            //                PlaceInfo1 = new HPTHorseTrioPlaceInfo()
            //                {
            //                    Investment = (int)horse.TrioInfo.InvestmentFirst,
            //                    Percent = horse.TrioInfo.PercentFirst,
            //                    Place = 1
            //                },
            //                PlaceInfo2 = new HPTHorseTrioPlaceInfo()
            //                {
            //                    Investment = (int)horse.TrioInfo.InvestmentSecond,
            //                    Percent = horse.TrioInfo.PercentSecond,
            //                    Place = 2
            //                },
            //                PlaceInfo3 = new HPTHorseTrioPlaceInfo()
            //                {
            //                    Investment = (int)horse.TrioInfo.InvestmentThird,
            //                    Percent = horse.TrioInfo.PercentThird,
            //                    Place = 3
            //                }
            //            };
            //        }
            //        else
            //        {
            //            this.TrioInfo.TrioIndex = horse.TrioInfo.TrioIndex;
            //            this.TrioInfo.PlaceInfo1.Investment = (int)horse.TrioInfo.InvestmentFirst;
            //            this.TrioInfo.PlaceInfo1.Percent = horse.TrioInfo.PercentFirst;
            //            this.TrioInfo.PlaceInfo2.Investment = (int)horse.TrioInfo.InvestmentSecond;
            //            this.TrioInfo.PlaceInfo2.Percent = horse.TrioInfo.PercentSecond;
            //            this.TrioInfo.PlaceInfo3.Investment = (int)horse.TrioInfo.InvestmentThird;
            //            this.TrioInfo.PlaceInfo3.Percent = horse.TrioInfo.PercentThird;
            //        }
            //    }
            //}

            //// Resultat i loppet om det är klart
            //HandleHorseResultInfo(horse, this);

            // Beräkna härledda variabler
            CalculateDerivedValues();
        }

        public void Merge(ATGStartBase start)
        {
            // TODO: Uppdatera med ATGStartBase istället
            //// Sen kuskändring
            //DriverChanged = !string.IsNullOrWhiteSpace(start.NewDriverName);
            //if (DriverChanged == true)
            //{
            //    DriverName = start.NewDriverName;
            //    DriverNameShort = start.NewDriverNameShort;
            //}

            //// Streck
            //MarksPossibleValue = start.MarksPossibleValue > 0 ? start.MarksPossibleValue : MarksPossibleValue;

            //// Insatsfördelning
            //StakeDistribution = start.StakeDistribution > 0 ? start.StakeDistribution : StakeDistribution;
            //StakeDistributionPercent = start.StakeDistributionPercent > 0 ? start.StakeDistributionPercent : StakeDistributionPercent;

            //// Sen strykning
            //if (start.Scratched == true)
            //{
            //    Scratched = start.Scratched;
            //    //this.VinnaroddsColor = new SolidColorBrush(Colors.Gray);
            //}

            //// Vinnare
            //VinnarOdds = start.VPInfo.VinnarOdds > 0 ? start.VPInfo.VinnarOdds : VinnarOdds;  // Bakåtkompatibilitet
            //VinnarOddsExact = start.VPInfo.VinnarOddsExact > 0M ? start.VPInfo.VinnarOddsExact : VinnarOddsExact;
            //InvestmentVinnare = start.VPInfo.InvestmentVinnare;

            //// Plats
            //MinPlatsOdds = start.VPInfo.MinPlatsOdds > 0 ? start.VPInfo.MinPlatsOdds : MinPlatsOdds;
            //MaxPlatsOdds = start.VPInfo.MaxPlatsOdds > 0 ? start.VPInfo.MaxPlatsOdds : MaxPlatsOdds;
            //MinPlatsOddsExact = start.VPInfo.MinPlatsOddsExact > 0M ? start.VPInfo.MinPlatsOddsExact : MinPlatsOddsExact;
            //MaxPlatsOddsExact = start.VPInfo.MaxPlatsOddsExact > 0M ? start.VPInfo.MaxPlatsOddsExact : MaxPlatsOddsExact;
            //InvestmentPlats = start.VPInfo.InvestmentPlats;

            //// Shareinfo
            //DoubleShare = start.ShareInfo.DoubleShare > 0 ? start.ShareInfo.DoubleShare : DoubleShare;
            //TrioShare = start.ShareInfo.TrioShare > 0 ? start.ShareInfo.TrioShare : TrioShare;
            //TvillingShare = start.ShareInfo.TvillingShare > 0 ? start.ShareInfo.TvillingShare : TvillingShare;

            //switch (ParentRace.ParentRaceDayInfo.BetType.Code)
            //{
            //    case "V65":
            //    case "V75":
            //    case "V85":
            //    case "GS75":
            //    case "V86":
            //    case "V64":
            //        StakeShareAlternate = start.ShareInfo.StakeShareV4 > 0 ? start.ShareInfo.StakeShareV4 : StakeShareAlternate;
            //        StakeShareAlternate2 = start.ShareInfo.StakeShareV5 > 0 ? start.ShareInfo.StakeShareV5 : StakeShareAlternate2;
            //        break;
            //    //case "V3":
            //    case "V4":
            //        StakeShareAlternate = start.ShareInfo.StakeShareVxx > 0 ? start.ShareInfo.StakeShareVxx : StakeShareAlternate;
            //        StakeShareAlternate2 = start.ShareInfo.StakeShareV5 > 0 ? start.ShareInfo.StakeShareV5 : StakeShareAlternate2;
            //        break;
            //    case "V5":
            //        StakeShareAlternate = start.ShareInfo.StakeShareVxx > 0 ? start.ShareInfo.StakeShareVxx : StakeShareAlternate;
            //        StakeShareAlternate2 = start.ShareInfo.StakeShareV4 > 0 ? start.ShareInfo.StakeShareV4 : StakeShareAlternate2;
            //        break;
            //    case "DD":
            //    case "LD":
            //        StakeDistributionPercent = Convert.ToInt32(start.ShareInfo.DoubleShare * 100);
            //        StakeDistributionShare = start.ShareInfo.DoubleShare;
            //        break;
            //    case "TV":
            //        StakeDistributionPercent = Convert.ToInt32(start.ShareInfo.TvillingShare * 100);
            //        StakeDistributionShare = start.ShareInfo.TvillingShare;
            //        break;
            //    case "T":
            //        StakeDistributionPercent = Convert.ToInt32(start.ShareInfo.TrioShare * 100);
            //        StakeDistributionShare = start.ShareInfo.TrioShare;
            //        break;
            //    default:
            //        break;
            //}

            //switch (ParentRace.ParentRaceDayInfo.BetType.Code)
            //{
            //    case "DD":
            //    case "LD":
            //    case "TV":
            //    case "T":
            //        if (start.ShareInfo.StakeShareVxx > 0)
            //        {
            //            StakeShareAlternate = start.ShareInfo.StakeShareVxx;
            //            if (start.ShareInfo.StakeShareV4 > 0)
            //            {
            //                StakeShareAlternate2 = start.ShareInfo.StakeShareV4;
            //            }
            //            else
            //            {
            //                StakeShareAlternate2 = start.ShareInfo.StakeShareV5;
            //            }
            //        }
            //        else if (start.ShareInfo.StakeShareV4 > 0)
            //        {
            //            StakeShareAlternate = start.ShareInfo.StakeShareV4;
            //            StakeShareAlternate2 = start.ShareInfo.StakeShareV5;
            //        }
            //        else
            //        {
            //            StakeShareAlternate = start.ShareInfo.StakeShareV5;
            //        }
            //        break;
            //    default:
            //        break;
            //}

            //// Skoinformation
            //if (start.ShoeInfoCurrent != null)
            //{
            //    ShoeInfoCurrent.Foreshoes = start.ShoeInfoCurrent.Foreshoes;
            //    ShoeInfoCurrent.Hindshoes = start.ShoeInfoCurrent.Hindshoes;
            //}
            //else
            //{
            //    ShoeInfoCurrent = new HPTHorseShoeInfo();
            //}

            //if (start.ShoeInfoPrevious != null)
            //{
            //    ShoeInfoPrevious.Foreshoes = start.ShoeInfoPrevious.Foreshoes;
            //    ShoeInfoPrevious.Hindshoes = start.ShoeInfoPrevious.Hindshoes;
            //}
            //else
            //{
            //    ShoeInfoPrevious = new HPTHorseShoeInfo();
            //}

            //if (start.ShoeInfoCurrent != null && start.ShoeInfoPrevious != null)
            //{
            //    if (ShoeInfoCurrent.Foreshoes == null && ShoeInfoCurrent.Hindshoes == null)
            //    {
            //        ShoeInfoCurrent.Foreshoes = ShoeInfoPrevious.Foreshoes;
            //        ShoeInfoCurrent.Hindshoes = ShoeInfoPrevious.Hindshoes;
            //        ShoeInfoCurrent.PreviousUsed = true;
            //    }
            //    else
            //    {
            //        ShoeInfoCurrent.SetChangedFlags(ShoeInfoPrevious);
            //        ShoeInfoCurrent.PreviousUsed = false;
            //    }
            //}

            //// Vagninformation
            //if (SulkyInfoCurrent == null)
            //{
            //    SulkyInfoCurrent = new HPTHorseSulkyInfo()
            //    {
            //        Text = start.StartInfo.SulkyInfoCurrent.Text
            //    };
            //}
            //else if (start.StartInfo != null && start.StartInfo.SulkyInfoCurrent != null)
            //{
            //    SulkyInfoCurrent.Text = start.StartInfo.SulkyInfoCurrent.Text;
            //}

            //if (SulkyInfoPrevious == null)
            //{
            //    SulkyInfoPrevious = new HPTHorseSulkyInfo()
            //    {
            //        Text = start.StartInfo.SulkyInfoPrevious.Text
            //    };
            //}
            //else if (start.StartInfo != null && start.StartInfo.SulkyInfoPrevious != null)
            //{
            //    SulkyInfoPrevious.Text = start.StartInfo.SulkyInfoPrevious.Text;
            //}

            //if (!string.IsNullOrEmpty(SulkyInfoCurrent?.Text) && !string.IsNullOrEmpty(SulkyInfoPrevious?.Text) && SulkyInfoCurrent.Text != SulkyInfoPrevious.Text)
            //{
            //    SulkyInfoCurrent.SulkyChanged = true;
            //}

            //// Trio
            //if (ParentRace.ParentRaceDayInfo.BetType.Code == "T")
            //{
            //    if (start.TrioInfo != null)
            //    {
            //        if (TrioInfo == null)
            //        {
            //            TrioInfo = new HPTHorseTrioInfo()
            //            {
            //                TrioIndex = start.TrioInfo.TrioIndex,
            //                PlaceInfo1 = new HPTHorseTrioPlaceInfo()
            //                {
            //                    Investment = (int)start.TrioInfo.InvestmentFirst,
            //                    Percent = start.TrioInfo.PercentFirst,
            //                    Place = 1
            //                },
            //                PlaceInfo2 = new HPTHorseTrioPlaceInfo()
            //                {
            //                    Investment = (int)start.TrioInfo.InvestmentSecond,
            //                    Percent = start.TrioInfo.PercentSecond,
            //                    Place = 2
            //                },
            //                PlaceInfo3 = new HPTHorseTrioPlaceInfo()
            //                {
            //                    Investment = (int)start.TrioInfo.InvestmentThird,
            //                    Percent = start.TrioInfo.PercentThird,
            //                    Place = 3
            //                }
            //            };
            //        }
            //        else
            //        {
            //            TrioInfo.TrioIndex = start.TrioInfo.TrioIndex;
            //            TrioInfo.PlaceInfo1.Investment = (int)start.TrioInfo.InvestmentFirst;
            //            TrioInfo.PlaceInfo1.Percent = start.TrioInfo.PercentFirst;
            //            TrioInfo.PlaceInfo2.Investment = (int)start.TrioInfo.InvestmentSecond;
            //            TrioInfo.PlaceInfo2.Percent = start.TrioInfo.PercentSecond;
            //            TrioInfo.PlaceInfo3.Investment = (int)start.TrioInfo.InvestmentThird;
            //            TrioInfo.PlaceInfo3.Percent = start.TrioInfo.PercentThird;
            //        }
            //    }
            //}

            //// Resultat i loppet om det är klart
            //HandleHorseResultInfo(start, this);

            //// Beräkna härledda variabler
            //CalculateDerivedValues();
        }

        public void ConvertHorse(ATGStartBase start)
        {
            // TODO: Konvertera med ATGStartBase istället
            //// Startinfo (statisk)
            //StartNr = start.StartInfo.StartNr;
            //TrainerName = start.StartInfo.TrainerName;
            //TrainerNameShort = start.StartInfo.TrainerNameShort;
            //Scratched = start.StartInfo.Scratched;
            //OwnerName = start.StartInfo.Owner;
            //BreederName = start.StartInfo.Breeder;
            //Age = start.StartInfo.Age;
            //Sex = start.StartInfo.Sex;
            //StartPoint = start.StartInfo.StartPoint;
            //Distance = start.StartInfo.Distance;
            //if (start.StartInfo.ATGId != 0)
            //{
            //    ATGId = start.StartInfo.ATGId.ToString();
            //}

            //// Hemmabana
            //HomeTrack = start.StartInfo.HomeTrack;
            ////this.IsHomeTrack = this.ParentRace.ParentRaceDayInfo.Trackcode == this.HomeTrack;
            //if (ParentRace.TrackId != null)
            //{
            //    IsHomeTrack = ParentRace.TrackCode == HomeTrack;
            //}

            //PostPosition = start.StartInfo.PostPosition;
            //DriverName = start.StartInfo.DriverName;
            //DriverNameShort = start.StartInfo.DriverNameShort;
            //HorseName = start.StartInfo.HorseName;
            //DriverChanged = start.StartInfo.DriverChanged;

            //// Skoinformation
            //ShoeInfoCurrent = CreateShoeInfo(start.StartInfo.ShoeInfoCurrent);
            //ShoeInfoPrevious = CreateShoeInfo(start.StartInfo.ShoeInfoPrevious);
            //if (ShoeInfoCurrent.Foreshoes == null && ShoeInfoCurrent.Hindshoes == null)
            //{
            //    ShoeInfoCurrent.Foreshoes = ShoeInfoPrevious.Foreshoes;
            //    ShoeInfoCurrent.Hindshoes = ShoeInfoPrevious.Hindshoes;
            //    ShoeInfoCurrent.PreviousUsed = true;
            //}
            //else
            //{
            //    ShoeInfoCurrent.SetChangedFlags(ShoeInfoPrevious);
            //}

            //// Vagninformation
            //if (SulkyInfoCurrent == null)
            //{
            //    SulkyInfoCurrent = new HPTHorseSulkyInfo()
            //    {
            //        Text = start.StartInfo.SulkyInfoCurrent.Text
            //    };
            //}
            //else
            //{
            //    SulkyInfoCurrent.Text = start.StartInfo.SulkyInfoCurrent.Text;
            //}

            //if (SulkyInfoPrevious == null)
            //{
            //    SulkyInfoPrevious = new HPTHorseSulkyInfo()
            //    {
            //        Text = start.StartInfo.SulkyInfoPrevious.Text
            //    };
            //}
            //else
            //{
            //    SulkyInfoPrevious.Text = start.StartInfo.SulkyInfoPrevious.Text;
            //}

            //if (!string.IsNullOrEmpty(SulkyInfoCurrent?.Text) && !string.IsNullOrEmpty(SulkyInfoPrevious?.Text) && SulkyInfoCurrent.Text != SulkyInfoPrevious.Text)
            //{
            //    SulkyInfoCurrent.SulkyChanged = true;
            //}

            //// Sätt de värden som sedan kommer att uppdateras ofta
            //Merge(start);

            //try
            //{
            //    // Statistik
            //    CurrentYearStatistics = ConvertHorseYearStatistics(start.StartInfo.CurrentYearStatistics);
            //    CurrentYearStatistics.YearString = DateTime.Now.Year.ToString();

            //    PreviousYearStatistics = ConvertHorseYearStatistics(start.StartInfo.PreviousYearStatistics);
            //    PreviousYearStatistics.YearString = DateTime.Now.AddYears(-1).Year.ToString();

            //    TotalStatistics = ConvertHorseYearStatistics(start.StartInfo.TotalStatistics);
            //    TotalStatistics.YearString = "Totalt";

            //    YearStatisticsList = new List<HPTHorseYearStatistics>() { CurrentYearStatistics, PreviousYearStatistics, TotalStatistics };

            //    // Records
            //    RecordList = start.StartInfo.RecordList.Select(r => new HPTHorseRecord()
            //    {
            //        Date = r.Date.ToString("yyyy-MM-dd"),
            //        Distance = r.Distance,
            //        Place = r.Place,
            //        RaceNr = r.RaceNr,
            //        RecordType = r.RecordType,
            //        Time = FormatRecordTime(r.Time),
            //        TrackCode = r.TrackCode,
            //        Winner = r.Winner,
            //        TimeWeighed = SetWeighedRecord(r, ParentRace)
            //    }).ToList();

            //    // Hitta rekordet
            //    SetRecord();
            //}
            //catch (Exception exc)
            //{
            //    // Data isn't sent from service
            //    string s = exc.Message;
            //}

            //// Resultat
            //var resultList = start.StartInfo.ResultList.Select(r => new HPTHorseResult()
            //{
            //    Date = r.Date,
            //    //DateString = r.DateString,
            //    Distance = r.Distance,
            //    Driver = r.Driver,
            //    Earning = r.Earning,
            //    FirstPrize = r.FirstPrize,
            //    Odds = r.Odds,
            //    PlaceString = r.PlaceString,
            //    HorseName = HorseName,
            //    Place = SetPlace(r),
            //    Position = r.Position,
            //    RaceType = r.RaceType,
            //    RaceNr = r.RaceNr,
            //    StartNr = r.StartNr,
            //    Time = r.Time,
            //    TrackCode = r.TrackCode,
            //    Shoeinfo = CreateShoeInfo(r.ShoeInfo),
            //    TimeWeighed = SetWeighedTime(r),
            //    HeadToHeadResultList = new List<HPTHorseResult>()
            //});
            //ResultList = new ObservableCollection<HPTHorseResult>(resultList);

            //if (ResultList.Count > 0)
            //{
            //    // Flagga för ny kusk
            //    DriverChangedSinceLastStart = ResultList[0].Driver != DriverNameShort;

            //    // Aggregerade värden (senaste 5)
            //    EarningsMeanLast5 = Convert.ToInt32(ResultList.Select(r => (decimal)r.Earning).Average());
            //    RecordWeighedLast5 = ResultList.Select(r => (decimal)r.TimeWeighed).Average();
            //    MeanPlaceLast5 = Convert.ToDecimal(ResultList.Select(r => r.Place).Average());
            //    ResultRow = ResultList
            //        .Select(r => r.PlaceString)
            //        .Aggregate((place, next) => place + "-" + next);

            //    // Aggregerade värden (senaste 3)
            //    var last3Results = ResultList.OrderByDescending(r => r.Date).Take(3).ToList();
            //    EarningsMeanLast3 = Convert.ToInt32(last3Results.Select(r => (decimal)r.Earning).Average());
            //    RecordWeighedLast3 = last3Results.Select(r => (decimal)r.TimeWeighed).Average();
            //    MeanPlaceLast3 = Convert.ToDecimal(last3Results.Select(r => r.Place).Average());
            //}

            //if (HPTConfig.Config.DataToShowVxx.ShowOwnInformation || HPTConfig.Config.DataToShowComplementaryRules.ShowOwnInformation || HPTConfig.Config.DataToShowCorrection.ShowOwnInformation || HPTConfig.Config.MarkBetTabsToShow.ShowComments)
            //{
            //    // Own information
            //    OwnInformation = HPTConfig.Config.HorseOwnInformationCollection.GetOwnInformationByName(HorseName);
            //    if (OwnInformation != null && OwnInformation.ATGId == "0")
            //    {
            //        OwnInformation.ATGId = ATGId;
            //    }
            //}
        }

        internal static void HandleHorseResultInfo(ATGStartBase start, HPTHorse hptHorse)
        {
            // TODO: Använd ATGStartBase
            //// Resultat i loppet om det är klart
            //if (start.VPInfo.HorseResultInfo != null)
            //{
            //    if (hptHorse.HorseResultInfo == null)
            //    {
            //        hptHorse.HorseResultInfo = new HPTHorseResultInfo()
            //        {
            //            Disqualified = start.VPInfo.HorseResultInfo.Disqualified,
            //            Earning = start.VPInfo.HorseResultInfo.Earning,
            //            FinishingPosition = start.VPInfo.HorseResultInfo.FinishingPosition,
            //            KmTime = start.VPInfo.HorseResultInfo.KmTime,
            //            Place = start.VPInfo.HorseResultInfo.Place,
            //            TotalTime = start.VPInfo.HorseResultInfo.TotalTime
            //        };
            //    }
            //    else
            //    {
            //        hptHorse.HorseResultInfo.Disqualified = start.VPInfo.HorseResultInfo.Disqualified;
            //        hptHorse.HorseResultInfo.Earning = start.VPInfo.HorseResultInfo.Earning;
            //        hptHorse.HorseResultInfo.FinishingPosition = start.VPInfo.HorseResultInfo.FinishingPosition;
            //        hptHorse.HorseResultInfo.KmTime = start.VPInfo.HorseResultInfo.KmTime;
            //        hptHorse.HorseResultInfo.Place = start.VPInfo.HorseResultInfo.Place;
            //        hptHorse.HorseResultInfo.TotalTime = start.VPInfo.HorseResultInfo.TotalTime;
            //    }
            //    hptHorse.HorseResultInfo.SetPlaceString(hptHorse);
            //}

        }

        internal string FormatRecordTime(string timeToFormat)
        {
            if (string.IsNullOrEmpty(timeToFormat))
            {
                return timeToFormat;
            }
            string formattedTime = timeToFormat;
            var rexTime = new Regex(@"\d\.\d\.\d");
            if (rexTime.IsMatch(formattedTime))
            {
                formattedTime = formattedTime.Insert(2, "0");
            }
            return formattedTime;
        }

        internal void SetRecord()
        {
            if (RecordList != null && RecordList.Count > 0)
            {
                // Snittrekord totalt
                RecordWeighedTotal = RecordList.Select(r => r.TimeWeighed).Average();

                // Hitta rekord
                Record = RecordList
                    .OrderBy(r => r.Time)
                    .FirstOrDefault(r => r.RecordType == ParentRace.StartMethodAndDistanceCode);
            }
            else
            {
                RecordWeighedTotal = 300M;
            }

            if (Record == null)
            {
                Record = new HPTHorseRecord();
                Record.Time = "-";
                RecordTime = 300M;
            }
            else
            {
                Regex rexKmTime = new Regex(@"\d\.(\d{1,2}\.\d)");
                if (rexKmTime.IsMatch(Record.Time))
                {
                    string kmTimeString = rexKmTime.Match(Record.Time).Groups[1].Value;
                    var ci = new CultureInfo("en-US");
                    RecordTime = Convert.ToDecimal(kmTimeString, ci.NumberFormat);
                }
                else
                {
                    RecordTime = 30M;
                }
            }
        }

        //internal int SetPlace(HPTService.HPTHorseResult result)
        //{
        //    switch (result.PlaceString)
        //    {
        //        case "0":
        //            return 7;
        //        case "k":
        //        case "d":
        //        case "p":
        //            return 8;
        //        default:
        //            return result.Place;
        //    }
        //}

        //internal HPTHorseShoeInfo CreateShoeInfo(HPTService.HPTHorseShoeInfo shoeInfo)
        //{
            // TODO: Använd ATGShoes
        //    var hptShoeInfo = new HPTHorseShoeInfo();
        //    if (shoeInfo != null)
        //    {
        //        hptShoeInfo.Foreshoes = shoeInfo.Foreshoes;
        //        hptShoeInfo.Hindshoes = shoeInfo.Hindshoes;
        //    }
        //    return hptShoeInfo;
        //}

        internal static Regex rexTime = new Regex(@"\d{1,2},\d");
        internal static CultureInfo swedishCulture = new CultureInfo("sv-SE");
        //private decimal SetWeighedTime(HPTService.HPTHorseResult horseResult)
        //{
        //    try
        //    {
        //        string startMethodAndDistanceCode = horseResult.Time.EndsWith("a") ? "A" : string.Empty;
        //        if (horseResult.Distance < 1800)
        //        {
        //            startMethodAndDistanceCode += "K";
        //        }
        //        else if (horseResult.Distance < 2600)
        //        {
        //            startMethodAndDistanceCode += "L";
        //        }
        //        else
        //        {
        //            startMethodAndDistanceCode += "M";
        //        }

        //        decimal secondsToAdd = 0M;
        //        switch (startMethodAndDistanceCode)
        //        {
        //            case "K":
        //                secondsToAdd = 2.3M;
        //                break;
        //            case "M":
        //                secondsToAdd = 0.9M;
        //                break;
        //            case "L":
        //                secondsToAdd = -0.1M;
        //                break;
        //            case "AK":
        //                secondsToAdd = 1.0M;
        //                break;
        //            case "AM":
        //                secondsToAdd = 0M;
        //                break;
        //            case "AL":
        //                secondsToAdd = -0.6M;
        //                break;
        //            default:
        //                break;
        //        }
        //        if (rexTime.IsMatch(horseResult.Time))
        //        {
        //            decimal time = decimal.Parse(rexTime.Match(horseResult.Time).Value, swedishCulture);
        //            return time + secondsToAdd;
        //        }

        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //    return 30M;
        //}

        //internal decimal SetWeighedRecord(HPTService.HPTHorseRecord record, HPTRace race)
        //{
        //    try
        //    {
        //        decimal secondsToAdd = 0M;
        //        switch (record.RecordType)
        //        {
        //            case "K":
        //                secondsToAdd = 2.3M;
        //                break;
        //            case "M":
        //                secondsToAdd = 0.9M;
        //                break;
        //            case "L":
        //                secondsToAdd = -0.1M;
        //                break;
        //            case "AK":
        //                secondsToAdd = 1.0M;
        //                break;
        //            case "AM":
        //                secondsToAdd = 0M;
        //                break;
        //            case "AL":
        //                secondsToAdd = -0.6M;
        //                break;
        //            default:
        //                break;
        //        }
        //        decimal extractedTime = 30M;
        //        Regex rexExtractTime = new Regex("\\d\\.(\\d\\d\\.\\d)");
        //        if (rexExtractTime.IsMatch(record.Time))
        //        {
        //            string extractedTimeString = rexExtractTime.Match(record.Time).Groups[1].Value;
        //            extractedTimeString = extractedTimeString.Replace('.', ',');
        //            extractedTime = Convert.ToDecimal(extractedTimeString);
        //        }
        //        return extractedTime + secondsToAdd;
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //    return 30M;
        //}

        private void SetWeighedTime(HPTHorseResult hptHorseResult)
        {
            try
            {
                string startMethodAndDistanceCode = hptHorseResult.Time.EndsWith("a") ? "A" : string.Empty;
                if (hptHorseResult.Distance < 1800)
                {
                    startMethodAndDistanceCode += "K";
                }
                else if (hptHorseResult.Distance < 2600)
                {
                    startMethodAndDistanceCode += "L";
                }
                else
                {
                    startMethodAndDistanceCode += "M";
                }

                decimal secondsToAdd = 0M;
                switch (startMethodAndDistanceCode)
                {
                    case "K":
                        secondsToAdd = 2.3M;
                        break;
                    case "M":
                        secondsToAdd = 0.9M;
                        break;
                    case "L":
                        secondsToAdd = -0.1M;
                        break;
                    case "AK":
                        secondsToAdd = 1.0M;
                        break;
                    case "AM":
                        secondsToAdd = 0M;
                        break;
                    case "AL":
                        secondsToAdd = -0.6M;
                        break;
                    default:
                        break;
                }
                if (rexTime.IsMatch(hptHorseResult.Time))
                {
                    decimal time = decimal.Parse(rexTime.Match(hptHorseResult.Time).Value, swedishCulture);
                    hptHorseResult.TimeWeighed = time + secondsToAdd;
                }
                else
                {
                    hptHorseResult.TimeWeighed = 30M;
                }
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                hptHorseResult.TimeWeighed = 30M;
            }
        }

        private static int CompareRecords(HPTHorseRecord r1, HPTHorseRecord r2)
        {
            return r1.Distance - r2.Distance;
        }

        //public HPTHorseYearStatistics ConvertHorseYearStatistics(HPTService.HPTHorseYearStatistics yearStatistics)
        //{
        //    // TODO: Använd ATGHorseStatistic istället
        //    HPTHorseYearStatistics hptYearStatistics = new HPTHorseYearStatistics();
        //    hptYearStatistics.Earning = yearStatistics.Earning;
        //    if (yearStatistics.NumberOfStarts > 0)
        //    {
        //        hptYearStatistics.EarningMean = hptYearStatistics.Earning / yearStatistics.NumberOfStarts;
        //    }
        //    hptYearStatistics.FirstPlace = yearStatistics.FirstPlace;
        //    hptYearStatistics.NumberOfStarts = yearStatistics.NumberOfStarts;
        //    hptYearStatistics.Percent123 = yearStatistics.Percent123;
        //    hptYearStatistics.PercentFirstPlace = yearStatistics.PercentFirstPlace;
        //    hptYearStatistics.SecondPlace = yearStatistics.SecondPlace;
        //    hptYearStatistics.ThirdPlace = yearStatistics.ThirdPlace;
        //    return hptYearStatistics;
        //}

        public void CalculateDerivedValues()
        {
            // Alla objekt måste finnas
            if (ParentRace != null && ParentRace.ParentRaceDayInfo != null)
            {
                // Insatsfördelning
                if (ParentRace.ParentRaceDayInfo.Turnover > 0 && StakeDistribution > 0)
                {
                    StakeDistributionShare = Convert.ToDecimal(StakeDistribution) / Convert.ToDecimal(ParentRace.ParentRaceDayInfo.Turnover);
                }

                // Strukna hästar
                if (scratched == true && Selected)
                {
                    ParentRace.ParentRaceDayInfo.ScratchedHorseInfo.HorseList.Add(this);
                    ParentRace.ParentRaceDayInfo.ScratchedHorseInfo.HaveSelectedScratchedHorse = true;
                }

                // Plats
                PlatsOddsShare = ParentRace.TurnoverPlats > 0 ? InvestmentPlats / ParentRace.TurnoverPlats : 0M;
                PlatsOddsString = MinPlatsOddsExact.ToString() + "-" + MaxPlatsOddsExact.ToString();
                //this.PlatsOddsString = this.MinPlatsOdds.ToString() + "-" + this.MaxPlatsOdds.ToString();

                // Vinnare
                VinnarOddsShare = ParentRace.TurnoverVinnare > 0 ? InvestmentVinnare / ParentRace.TurnoverVinnare : 0M;
                if (ParentRace.NumberOfStartingHorses > 0)
                {
                    VinnarOddsRelative = Convert.ToDecimal(VinnarOdds) / Convert.ToDecimal(ParentRace.NumberOfStartingHorses) * 10M;
                }

                // Streckbarhet
                SetMarkability();

                //// Streck
                //if (this.ParentRace.MarksQuantity > 0 && this.ParentRace.ParentRaceDayInfo.MarksQuantity > 0)
                //{
                //    this.MarksShare = Convert.ToDecimal(this.MarksQuantity) / Convert.ToDecimal(this.ParentRace.MarksQuantity);
                //    this.MarksPercentExact = Convert.ToDecimal(this.MarksQuantity) / Convert.ToDecimal(this.ParentRace.ParentRaceDayInfo.MarksQuantity);
                //    this.Markability = (this.VinnarOddsShare + 0.05M) / (this.StakeDistributionShare + 0.05M) + this.StakeDistributionShare + this.VinnarOddsShare;
                //}
            }
        }

        public void CalculateStaticDerivedValues()
        {
            // TODO: Använd ATG-objekten
            //// Alla objekt måste finnas
            //if (ParentRace != null && ParentRace.ParentRaceDayInfo != null)
            //{
            //    // Hemmabana
            //    IsHomeTrack = ParentRace.ParentRaceDayInfo.Trackcode == HomeTrack;
            //    if (ParentRace.TrackId != null)
            //    {
            //        IsHomeTrack = ParentRace.TrackCode == HomeTrack;
            //    }

            //    // Årsstatistik
            //    CurrentYearStatistics.YearString = ParentRace.ParentRaceDayInfo.RaceDayDate.Year.ToString();
            //    PreviousYearStatistics.YearString = ParentRace.ParentRaceDayInfo.RaceDayDate.AddYears(-1).Year.ToString();
            //    TotalStatistics.YearString = "Totalt";
            //    YearStatisticsList = new List<HPTHorseYearStatistics>() { CurrentYearStatistics, PreviousYearStatistics, TotalStatistics };
            //    foreach (var yearStatistics in YearStatisticsList)
            //    {
            //        if (yearStatistics.NumberOfStarts > 0)
            //        {
            //            yearStatistics.EarningMean = yearStatistics.Earning / yearStatistics.NumberOfStarts;
            //        }
            //    }

            //    // Hitta rekord
            //    SetWeighedRecords();
            //    HPTServiceToHPTHelper.SetRecord(this);

            //    if (ResultList.Count > 0)
            //    {
            //        foreach (var result in ResultList)
            //        {
            //            SetWeighedTime(result);
            //            //result.TimeWeighed = SetWeighedTime(result);
            //            try
            //            {
            //                switch (result.PlaceString)
            //                {
            //                    case "0":
            //                        result.Place = 7;
            //                        break;
            //                    case "k":
            //                    case "d":
            //                    case "p":
            //                    case "rd":
            //                    case "r0":
            //                    case "":
            //                        result.Place = 8;
            //                        break;
            //                    default:
            //                        result.Place = int.Parse(result.PlaceString);
            //                        break;
            //                }
            //            }
            //            catch (Exception exc)
            //            {
            //                string s = exc.Message;
            //            }
            //        }

            //        // Aggregerade värden (senaste 5)
            //        var last5Results = ResultList
            //            .OrderByDescending(r => r.Date)
            //            .Take(5)
            //            .ToList();

            //        EarningsMeanLast5 = Convert.ToInt32(last5Results.Average(r => Convert.ToInt32(r.Earning)));
            //        RecordWeighedLast5 = last5Results.Average(r => r.TimeWeighed);
            //        MeanPlaceLast5 = Convert.ToDecimal(last5Results.Average(r => r.Place));
            //        ResultRow = last5Results
            //            .Select(r => r.PlaceString)
            //            .Aggregate((place, next) => place + "-" + next);


            //        // Aggregerade värden (senaste 3)
            //        var last3Results = ResultList
            //            .OrderByDescending(r => r.Date)
            //            .Take(3)
            //            .ToList();

            //        EarningsMeanLast3 = Convert.ToInt32(last3Results.Average(r => Convert.ToInt32(r.Earning)));
            //        RecordWeighedLast3 = last3Results.Average(r => r.TimeWeighed);
            //        MeanPlaceLast3 = Convert.ToDecimal(last3Results.Average(r => r.Place));
            //    }

            //}
        }

        internal void SetWeighedRecords()
        {
            foreach (var record in RecordList)
            {
                try
                {
                    decimal secondsToAdd = 0M;
                    switch (record.RecordType)
                    {
                        case "K":
                            secondsToAdd = 2.3M;
                            break;
                        case "M":
                            secondsToAdd = 0.9M;
                            break;
                        case "L":
                            secondsToAdd = -0.1M;
                            break;
                        case "AK":
                            secondsToAdd = 1.0M;
                            break;
                        case "AM":
                            secondsToAdd = 0M;
                            break;
                        case "AL":
                            secondsToAdd = -0.6M;
                            break;
                        default:
                            break;
                    }
                    decimal extractedTime = 30M;
                    Regex rexExtractTime = new Regex("\\d\\.(\\d\\d\\.\\d)");
                    if (rexExtractTime.IsMatch(record.Time))
                    {
                        string extractedTimeString = rexExtractTime.Match(record.Time).Groups[1].Value;
                        extractedTimeString = extractedTimeString.Replace('.', ',');
                        extractedTime = Convert.ToDecimal(extractedTimeString);
                    }
                    record.TimeWeighed = extractedTime + secondsToAdd;
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                    record.TimeWeighed = 30M;
                }
            }
        }

        #endregion

        //internal static void RandomizeHorseValues(IEnumerable<HPTHorse> horseList)    // För att kunna simulera en omgång
        //{            
        //    Random rnd = new Random();
        //    foreach (PropertyInfo pi in (typeof(HPTMarkBetTabsToShow)).GetProperties())
        //    {
        //        RandomIntervalAttribute ria = (RandomIntervalAttribute)pi.GetCustomAttributes(true).FirstOrDefault(piTemp => piTemp.GetType() == typeof(RandomIntervalAttribute));
        //        if (ria != null)
        //        {
        //            foreach (HPTHorse horse in horseList)
        //            {
        //                pi.SetValue(horse, rnd.Next(ria.MinValue, ria.MaxValue), null);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Properties from Start

        [DataMember]
        public string HorseName { get; set; }

        public string HorseNumberAndName
        {
            get
            {
                if (StartNr == 0)
                {
                    return HorseName;
                }
                return StartNr.ToString() + " - " + HorseName;
            }
        }

        public string HorseUniqueCode
        {
            get
            {
                return ParentRace.LegNr + "-" + StartNr.ToString() + " - " + HorseName;
            }
        }

        public string HorseNameWithoutInvalidCharacters
        {
            get
            {
                if (string.IsNullOrEmpty(HorseName))
                {
                    return "ERROR";
                }
                string correctedHorseName = HorseName;
                System.IO.Path.GetInvalidFileNameChars()
                    .ToList()
                    .ForEach(ic => correctedHorseName = correctedHorseName.Replace(ic, '_'));

                return correctedHorseName;
            }
        }

        [GroupReduction("Startnummer", 21, 1D, 15D, 1D, 1D, 1D)]
        [DataMember]
        public int StartNr { get; set; }

        [DataMember]
        public string ATGId { get; set; }

        [DataMember]
        public string DriverName { get; set; }

        [DataMember]
        public string DriverNameShort { get; set; }

        [DataMember]
        public string TrainerName { get; set; }

        [DataMember]
        public string TrainerNameShort { get; set; }

        [DataMember]
        public string BreederName { get; set; }

        [DataMember]
        public string OwnerName { get; set; }

        public int DistanceFromHomeTrack { get; set; }

        public string HomeTrackInfo { get; set; }

        private string stLink;
        public string STLink
        {
            get
            {
                if (string.IsNullOrEmpty(stLink))
                {
                    STLink = ATGLinkCreator.CreateSTHorseLink(this);
                }
                return stLink;
            }
            set
            {
                stLink = value;
                OnPropertyChanged("STLink");
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseResult> ResultList { get; set; }

        private HPTHorseResultInfo horseResultInfo;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseResultInfo HorseResultInfo
        {
            get
            {
                return horseResultInfo;
            }
            set
            {
                horseResultInfo = value;
                OnPropertyChanged("HorseResultInfo");
            }
        }

        //private HPTHorseHistoryInfo horseHistoryInfo;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public HPTHorseHistoryInfo HorseHistoryInfo
        //{
        //    get
        //    {
        //        return this.horseHistoryInfo;
        //    }
        //    set
        //    {
        //        this.horseHistoryInfo = value;
        //        OnPropertyChanged("HorseHistoryInfo");
        //    }
        //}

        //private HPTHorseHistoryInfoGrouped horseHistoryInfoGrouped;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public HPTHorseHistoryInfoGrouped HorseHistoryInfoGrouped
        //{
        //    get
        //    {
        //        return this.horseHistoryInfoGrouped;
        //    }
        //    set
        //    {
        //        this.horseHistoryInfoGrouped = value;
        //        OnPropertyChanged("HorseHistoryInfoGrouped");
        //    }
        //}

        private HPTHorseHistoryInfoGrouped[] horseHistoryInfoGroupedList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseHistoryInfoGrouped[] HorseHistoryInfoGroupedList
        {
            get
            {
                return horseHistoryInfoGroupedList;
            }
            set
            {
                horseHistoryInfoGroupedList = value;
                OnPropertyChanged("HorseHistoryInfoGroupedList");
            }
        }

        //[HorseRank("Trend", 3, true, false, HPTRankCategory.MarksAndOdds, true)]
        public decimal HistoryRelativeDifference
        {
            get
            {
                if (HorseHistoryInfoGroupedList == null || HorseHistoryInfoGroupedList.Length == 0)
                {
                    return 0M;
                }
                decimal newestStakeShare = HorseHistoryInfoGroupedList.First().StakeHistoryMain.StakeShare;
                decimal oldestStakeShare = HorseHistoryInfoGroupedList.Last().StakeHistoryMain.StakeShare;
                if (newestStakeShare > 0M && oldestStakeShare > 0M)
                {
                    decimal relativeDifference = (newestStakeShare / oldestStakeShare) - 1M;
                    decimal relativeDifferenceAdjusted = relativeDifference < 0M ? 0M : relativeDifference + StakeDistributionShare;
                    return relativeDifferenceAdjusted;
                }
                return 0M;
            }
        }

        private decimal historyRelativeDifferenceUnadjusted;
        [HorseRank("Trend", 3, true, false, HPTRankCategory.MarksAndOdds, true)]
        [DataMember]
        public decimal HistoryRelativeDifferenceUnadjusted
        {
            get
            {
                return historyRelativeDifferenceUnadjusted;
            }
            set
            {
                historyRelativeDifferenceUnadjusted = value;
                OnPropertyChanged("HistoryRelativeDifferenceUnadjusted");
            }
        }

        private decimal historyRelativeDifferenceVinnareUnadjusted;
        //[HorseRank("Trend (V)", 4, true, false, HPTRankCategory.MarksAndOdds, true)]
        public decimal HistoryRelativeDifferenceVinnareUnadjusted
        {
            get
            {
                return historyRelativeDifferenceVinnareUnadjusted;
            }
            set
            {
                historyRelativeDifferenceVinnareUnadjusted = value;
                OnPropertyChanged("HistoryRelativeDifferenceVinnareUnadjusted");
            }
        }

        private decimal historyRelativeDifferencePlatsUnadjusted;
        //[HorseRank("Trend (P)", 5, true, false, HPTRankCategory.MarksAndOdds, true)]
        public decimal HistoryRelativeDifferencePlatsUnadjusted
        {
            get
            {
                return historyRelativeDifferencePlatsUnadjusted;
            }
            set
            {
                historyRelativeDifferencePlatsUnadjusted = value;
                OnPropertyChanged("HistoryRelativeDifferencePlatsUnadjusted");
            }
        }

        internal void SetHistoryRelativeDifferenceUnadjusted()
        {
            HistoryRelativeDifferenceUnadjusted = 0M;
            HistoryRelativeDifferenceVinnareUnadjusted = 0M;
            HistoryRelativeDifferencePlatsUnadjusted = 0M;

            if (HorseHistoryInfoGroupedList == null || HorseHistoryInfoGroupedList.Length == 0)
            {
                return;
            }

            decimal newestStakeShare = HorseHistoryInfoGroupedList.First().StakeHistoryMain.StakeShare;
            if (newestStakeShare > 0M)
            {
                decimal oldestStakeShare = HorseHistoryInfoGroupedList.Last(hh => hh.StakeHistoryMain.StakeShare > 0M).StakeHistoryMain.StakeShare;
                HistoryRelativeDifferenceUnadjusted = (newestStakeShare / oldestStakeShare) - 1M;
            }
            //decimal newestVinnarShare = this.HorseHistoryInfoGroupedList.First().VinnarOddsShare;
            //var horseHistoryVinnare = this.HorseHistoryInfoGroupedList.LastOrDefault(hh => hh.VinnarOddsShare > 0M);
            //if (newestVinnarShare > 0M && horseHistoryVinnare != null)// oldestVinnarShare > 0M)
            //{
            //    this.HistoryRelativeDifferenceVinnareUnadjusted = (newestVinnarShare / horseHistoryVinnare.VinnarOddsShare) - 1M;
            //}
            //decimal newestPlatsShare = this.HorseHistoryInfoGroupedList.First().PlatsOddsShare;
            //var horseHistoryPlats = this.HorseHistoryInfoGroupedList.LastOrDefault(hh => hh.PlatsOddsShare > 0M);
            //if (newestPlatsShare > 0M && horseHistoryPlats != null)// oldestPlatsShare > 0M)
            //{
            //    this.HistoryRelativeDifferencePlatsUnadjusted = (newestPlatsShare / horseHistoryPlats.PlatsOddsShare) - 1M;
            //}
        }

        internal void SetHistoryRelativeDifferenceUnadjusted(int positionForOldestStakeShare)
        {
            if (positionForOldestStakeShare < 1)
            {
                SetHistoryRelativeDifferenceUnadjusted();
                return;
            }

            HistoryRelativeDifferenceUnadjusted = 0M;
            HistoryRelativeDifferenceVinnareUnadjusted = 0M;
            HistoryRelativeDifferencePlatsUnadjusted = 0M;

            if (HorseHistoryInfoGroupedList == null || HorseHistoryInfoGroupedList.Length == 0)
            {
                return;
            }

            decimal newestStakeShare = HorseHistoryInfoGroupedList.First().StakeHistoryMain.StakeShare;
            decimal oldestStakeShare = HorseHistoryInfoGroupedList[positionForOldestStakeShare].StakeHistoryMain.StakeShare;
            if (newestStakeShare > 0M && oldestStakeShare > 0M)
            {
                HistoryRelativeDifferenceUnadjusted = (newestStakeShare / oldestStakeShare) - 1M;
            }

            //decimal newestVinnarShare = this.HorseHistoryInfoGroupedList.First().VinnarOddsShare;
            //var horseHistoryVinnare = this.HorseHistoryInfoGroupedList[positionForOldestStakeShare];
            //if (horseHistoryVinnare != null && horseHistoryVinnare.VinnarOddsShare == 0M)
            //{
            //    horseHistoryVinnare = this.HorseHistoryInfoGroupedList.LastOrDefault(hh => hh.VinnarOddsShare > 0M);
            //}
            //if (newestVinnarShare > 0M && horseHistoryVinnare != null)
            //{
            //    this.HistoryRelativeDifferenceVinnareUnadjusted = (newestVinnarShare / horseHistoryVinnare.VinnarOddsShare) - 1M;
            //}
            //decimal newestPlatsShare = this.HorseHistoryInfoGroupedList.First().PlatsOddsShare;
            //var horseHistoryPlats = this.HorseHistoryInfoGroupedList[positionForOldestStakeShare];
            //if (horseHistoryPlats != null && horseHistoryPlats.PlatsOddsShare == 0M)
            //{
            //    horseHistoryPlats = this.HorseHistoryInfoGroupedList.LastOrDefault(hh => hh.PlatsOddsShare > 0M);
            //}
            //if (newestPlatsShare > 0M && horseHistoryPlats != null)
            //{
            //    this.HistoryRelativeDifferencePlatsUnadjusted = (newestPlatsShare / horseHistoryPlats.PlatsOddsShare) - 1M;
            //}
        }

        public Brush HistoryRelativeDifferenceColour
        {
            get
            {
                var c = Colors.White;
                if (StakeDistributionShare > 0.03M)
                {
                    if (HistoryRelativeDifferenceUnadjusted > 0.1M)
                    {
                        c = HPTConfig.Config.ColorGood;
                    }
                    else if (HistoryRelativeDifferenceUnadjusted < -0.1M)
                    {
                        c = HPTConfig.Config.ColorBad;
                    }
                }
                return new SolidColorBrush(c);
            }
        }

        public Brush HistoryRelativeDifferenceVinnareColour
        {
            get
            {
                var c = Colors.White;
                if (VinnarOddsShare > 0.03M)
                {
                    if (HistoryRelativeDifferenceVinnareUnadjusted > 0.1M)
                    {
                        c = HPTConfig.Config.ColorGood;
                    }
                    else if (HistoryRelativeDifferenceVinnareUnadjusted < -0.1M)
                    {
                        c = HPTConfig.Config.ColorBad;
                    }
                }
                return new SolidColorBrush(c);
            }
        }

        public Brush HistoryRelativeDifferencePlatsColour
        {
            get
            {
                var c = Colors.White;
                if (PlatsOddsShare > 0.03M)
                {
                    if (HistoryRelativeDifferencePlatsUnadjusted > 0.1M)
                    {
                        c = HPTConfig.Config.ColorGood;
                    }
                    else if (HistoryRelativeDifferencePlatsUnadjusted < -0.1M)
                    {
                        c = HPTConfig.Config.ColorBad;
                    }
                }
                return new SolidColorBrush(c);
            }
        }

        [XmlIgnore]
        public List<HPTHeadToHeadCollection> HeadToHeadResultCollectionList
        {
            get
            {
                List<HPTHeadToHeadCollection> headToHeadResultCollectionList = new List<HPTHeadToHeadCollection>();
                foreach (var horseResult in ResultList)
                {
                    if (horseResult.HeadToHeadResultList != null && horseResult.HeadToHeadResultList.Count > 0)
                    {
                        var h2hResultList = new List<HPTHorseResult>() { horseResult };
                        h2hResultList.AddRange(horseResult.HeadToHeadResultList);
                        var headToHeadResultCollection = new HPTHeadToHeadCollection(h2hResultList);
                        headToHeadResultCollectionList.Add(headToHeadResultCollection);
                    }
                }
                return headToHeadResultCollectionList;
            }
        }

        private int numberOfHeadToHeadWins;
        [XmlIgnore]
        public int NumberOfHeadToHeadWins
        {
            get
            {
                return numberOfHeadToHeadWins;
            }
            set
            {
                numberOfHeadToHeadWins = value;
                OnPropertyChanged("NumberOfHeadToHeadWins");
            }
        }

        private int numberOfHeadToHeadLosses;
        [XmlIgnore]
        public int NumberOfHeadToHeadLosses
        {
            get
            {
                return numberOfHeadToHeadLosses;
            }
            set
            {
                numberOfHeadToHeadLosses = value;
                OnPropertyChanged("NumberOfHeadToHeadLosses");
            }
        }

        private int numberOfHeadToHeadEqual;
        [XmlIgnore]
        public int NumberOfHeadToHeadEqual
        {
            get
            {
                return numberOfHeadToHeadEqual;
            }
            set
            {
                numberOfHeadToHeadEqual = value;
                OnPropertyChanged("NumberOfHeadToHeadEqual");
            }
        }

        private int numberOfHeadToHeadRaces;
        [XmlIgnore]
        public int NumberOfHeadToHeadRaces
        {
            get
            {
                return numberOfHeadToHeadRaces;
            }
            set
            {
                numberOfHeadToHeadRaces = value;
                OnPropertyChanged("NumberOfHeadToHeadRaces");
            }
        }

        private int numberOfHeadToHeadResults;
        [XmlIgnore]
        public int NumberOfHeadToHeadResults
        {
            get
            {
                return numberOfHeadToHeadResults;
            }
            set
            {
                numberOfHeadToHeadResults = value;
                OnPropertyChanged("NumberOfHeadToHeadResults");
            }
        }

        private bool hasHeadToHeadResults;
        [XmlIgnore]
        public bool HasHeadToHeadResults
        {
            get
            {
                return hasHeadToHeadResults;
            }
            set
            {
                hasHeadToHeadResults = value;
                OnPropertyChanged("HasHeadToHeadResults");
            }
        }

        private HPTHorseShoeInfo shoeInfoCurrent;
        [DataMember]
        public HPTHorseShoeInfo ShoeInfoCurrent
        {
            get
            {
                return shoeInfoCurrent;
            }
            set
            {
                shoeInfoCurrent = value;
                OnPropertyChanged("ShoeInfoCurrent");
            }
        }

        private HPTHorseShoeInfo shoeInfoPrevious;
        [DataMember]
        public HPTHorseShoeInfo ShoeInfoPrevious
        {
            get
            {
                return shoeInfoPrevious;
            }
            set
            {
                shoeInfoPrevious = value;
                OnPropertyChanged("ShoeInfoPrevious");
            }
        }

        private HPTHorseSulkyInfo sulkyInfoCurrent;
        [DataMember]
        public HPTHorseSulkyInfo SulkyInfoCurrent
        {
            get
            {
                return sulkyInfoCurrent;
            }
            set
            {
                sulkyInfoCurrent = value;
                OnPropertyChanged("SulkyInfoCurrent");
            }
        }

        private HPTHorseSulkyInfo sulkyInfoPrevious;
        [DataMember]
        public HPTHorseSulkyInfo SulkyInfoPrevious
        {
            get
            {
                return sulkyInfoPrevious;
            }
            set
            {
                sulkyInfoPrevious = value;
                OnPropertyChanged("SulkyInfoPrevious");
            }
        }

        #endregion

        private HPTHorseTrioInfo trioInfo;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioInfo TrioInfo
        {
            get
            {
                return trioInfo;
            }
            set
            {
                trioInfo = value;
                OnPropertyChanged("TrioInfo");
            }
        }

        //#region Properties from Trio

        //private int? percentFirst;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public int? PercentFirst 
        //{
        //    get
        //    {
        //        return this.percentFirst;
        //    }
        //    set
        //    {
        //        this.percentFirst = value;
        //        OnPropertyChanged("PercentFirst");
        //    }
        //}

        //private int? percentSecond;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public int? PercentSecond
        //{
        //    get
        //    {
        //        return this.percentSecond;
        //    }
        //    set
        //    {
        //        this.percentSecond = value;
        //        OnPropertyChanged("PercentSecond");
        //    }
        //}

        //private int? percentThird;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public int? PercentThird
        //{
        //    get
        //    {
        //        return this.percentThird;
        //    }
        //    set
        //    {
        //        this.percentThird = value;
        //        OnPropertyChanged("PercentThird");
        //    }
        //}

        //private decimal percentFirstRelative;
        //public decimal PercentFirstRelative
        //{
        //    get
        //    {
        //        return this.percentFirstRelative;
        //    }
        //    set
        //    {
        //        this.percentFirstRelative = value;
        //        OnPropertyChanged("PercentFirstRelative");
        //    }
        //}

        //private decimal percentSecondRelative;
        //public decimal PercentSecondRelative
        //{
        //    get
        //    {
        //        return this.percentSecondRelative;
        //    }
        //    set
        //    {
        //        this.percentSecondRelative = value;
        //        OnPropertyChanged("PercentSecondRelative");
        //    }
        //}

        //private decimal percentThirdRelative;
        //public decimal PercentThirdRelative
        //{
        //    get
        //    {
        //        return this.percentThirdRelative;
        //    }
        //    set
        //    {
        //        this.percentThirdRelative = value;
        //        OnPropertyChanged("PercentThirdRelative");
        //    }
        //}

        //private int? trioIndex;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public int? TrioIndex
        //{
        //    get
        //    {
        //        return this.trioIndex;
        //    }
        //    set
        //    {
        //        this.trioIndex = value;
        //        OnPropertyChanged("TrioIndex");
        //    }
        //}

        //private int trioStakeDistributionFirst;
        //[DataMember]
        //public int TrioStakeDistributionFirst
        //{
        //    get
        //    {
        //        return trioStakeDistributionFirst;
        //    }
        //    set
        //    {
        //        this.trioStakeDistributionFirst = value;
        //        OnPropertyChanged("TrioStakeDistributionFirst");
        //    }
        //}

        //private int trioStakeDistributionSecond;
        //[DataMember]
        //public int TrioStakeDistributionSecond
        //{
        //    get
        //    {
        //        return trioStakeDistributionSecond;
        //    }
        //    set
        //    {
        //        this.trioStakeDistributionSecond = value;
        //        OnPropertyChanged("TrioStakeDistributionSecond");
        //    }
        //}

        //private int trioStakeDistributionThird;
        //[DataMember]
        //public int TrioStakeDistributionThird
        //{
        //    get
        //    {
        //        return trioStakeDistributionThird;
        //    }
        //    set
        //    {
        //        this.trioStakeDistributionThird = value;
        //        OnPropertyChanged("TrioStakeDistributionThird");
        //    }
        //}

        //private bool? trioFirstPlace;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public bool? TrioFirstPlace
        //{
        //    get
        //    {
        //        return trioFirstPlace;
        //    }
        //    set
        //    {
        //        trioFirstPlace = value;
        //        OnPropertyChanged("TrioFirstPlace");
        //        if (this.ParentRace != null)
        //        {
        //            this.ParentRace.SetTrioSelectionChanged(this);
        //        }
        //    }
        //}

        //private bool? trioSecondPlace;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public bool? TrioSecondPlace
        //{
        //    get
        //    {
        //        return trioSecondPlace;
        //    }
        //    set
        //    {
        //        trioSecondPlace = value;
        //        OnPropertyChanged("TrioSecondPlace");
        //        if (this.ParentRace != null)
        //        {
        //            this.ParentRace.SetTrioSelectionChanged(this);
        //        }
        //    }
        //}

        //private bool? trioThirdPlace;
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public bool? TrioThirdPlace
        //{
        //    get
        //    {
        //        return trioThirdPlace;
        //    }
        //    set
        //    {
        //        trioThirdPlace = value;
        //        OnPropertyChanged("TrioThirdPlace");
        //        if (this.ParentRace != null)
        //        {
        //            this.ParentRace.SetTrioSelectionChanged(this);
        //        }
        //    }
        //}

        //private bool trioFirstPlaceHighlighted;
        //public bool TrioFirstPlaceHighlighted
        //{
        //    get
        //    {
        //        return trioFirstPlaceHighlighted;
        //    }
        //    set
        //    {
        //        trioFirstPlaceHighlighted = value;
        //        OnPropertyChanged("TrioFirstPlaceHighlighted");
        //    }
        //}

        //private bool trioSecondPlaceHighlighted;
        //public bool TrioSecondPlaceHighlighted
        //{
        //    get
        //    {
        //        return trioSecondPlaceHighlighted;
        //    }
        //    set
        //    {
        //        trioSecondPlaceHighlighted = value;
        //        OnPropertyChanged("TrioSecondPlaceHighlighted");
        //    }
        //}

        //private bool trioThirdPlaceHighlighted;
        //public bool TrioThirdPlaceHighlighted
        //{
        //    get
        //    {
        //        return trioThirdPlaceHighlighted;
        //    }
        //    set
        //    {
        //        trioThirdPlaceHighlighted = value;
        //        OnPropertyChanged("TrioThirdPlaceHighlighted");
        //    }
        //}

        //#endregion

        #region Properties from VPOdds

        private int vinnarOdds;
        [RandomInterval("VinnarOdds", 10, 300, 1D)]
        [HorseRank("Vinnarodds", 3, false, false, HPTRankCategory.MarksAndOdds, true, true, "Vinnarodds")]
        [GroupReduction("Vinnarodds", 1, 10D, 999D, 1D, 30D, 100D)]
        [DataMember]
        public int VinnarOdds
        {
            get
            {
                return vinnarOdds;
            }
            set
            {
                vinnarOdds = value;
                //if (value != 0 && value != 9999)
                //{
                //    this.VinnarOddsString = this.vinnarOdds.ToString();
                //    this.VinnarOddsShare = 0.8M / this.vinnarOdds * 10;
                //}
                OnPropertyChanged("VinnarOdds");
            }
        }

        private decimal vinnarOddsExact;
        public decimal VinnarOddsExact
        {
            get
            {
                return vinnarOddsExact;
            }
            set
            {
                vinnarOddsExact = value;
                if (value != 0 && value != 9999)
                {
                    VinnarOddsString = vinnarOddsExact.ToString();
                    //this.VinnarOddsShare = 0.8M / this.vinnarOdds * 10;
                }
                OnPropertyChanged("VinnarOddsExact");
            }
        }

        private decimal? _VinnarDddsFinal;
        public decimal? VinnarDddsFinal
        {
            get
            {
                return _VinnarDddsFinal;
            }
            set
            {
                _VinnarDddsFinal = value;
                OnPropertyChanged("VinnarDddsFinal");
            }
        }

        private int stakeDistributionPercent;
        [GroupReduction("Insatsfördelning", 2, 0D, 100D, 1D, 10D, 70D)]
        [DataMember]
        public int StakeDistributionPercent
        {
            get
            {
                return stakeDistributionPercent;
            }
            set
            {
                stakeDistributionPercent = value;
                OnPropertyChanged("StakeDistributionPercent");
            }
        }

        private int stakeDistribution;
        [DataMember]
        public int StakeDistribution
        {
            get
            {
                return stakeDistribution;
            }
            set
            {
                stakeDistribution = value;
                OnPropertyChanged("StakeDistribution");
            }
        }

        private decimal stakeDistributionShare;
        [HorseRank("Insatsfördelning", 1, true, false, HPTRankCategory.MarksAndOdds, true, true, "Insatsfördelning", "", "P1", 0D)]
        public decimal StakeDistributionShare
        {
            get
            {
                return stakeDistributionShare;
            }
            set
            {
                stakeDistributionShare = value;
                OnPropertyChanged("StakeDistributionShare");
            }
        }

        private decimal? _StakeDistributionShareFinal;
        public decimal? StakeDistributionShareFinal
        {
            get
            {
                return _StakeDistributionShareFinal;
            }
            set
            {
                _StakeDistributionShareFinal = value;
                OnPropertyChanged("StakeDistributionShareFinal");
            }
        }

        public decimal StakeShareRounded { get; set; }

        public decimal StakeShareWithoutScratchings { get; set; }

        private decimal stakeDistributionShareAccumulated;
        public decimal StakeDistributionShareAccumulated
        {
            get
            {
                return stakeDistributionShareAccumulated;
            }
            set
            {
                stakeDistributionShareAccumulated = value;
                OnPropertyChanged("StakeDistributionShareAccumulated");
            }
        }

        private string vinnarOddsString;
        [XmlIgnore]
        public string VinnarOddsString
        {
            get
            {
                return vinnarOddsString;
            }
            set
            {
                vinnarOddsString = value;
                OnPropertyChanged("VinnarOddsString");
            }
        }

        private decimal vinnarOddsShare;
        [GroupReduction("Vinnaroddsandel", 5, 0D, 1D, 0.01D, 0.1D, 0.5D)]
        [XmlIgnore]
        public decimal VinnarOddsShare
        {
            get
            {
                return vinnarOddsShare;
            }
            set
            {
                vinnarOddsShare = value;
                OnPropertyChanged("VinnarOddsShare");
                if (value > 0M && VinnarOddsExact == 0M)
                {
                    VinnarOddsExact = 0.8M / value;
                }
            }
        }

        [GroupReduction("Min platsodds", 7, 10D, 999D, 1D, 10D, 30D)]
        [DataMember]
        public int MinPlatsOdds { get; set; }

        [HorseRank("Platsodds", 4, false, false, HPTRankCategory.MarksAndOdds, true, true, "Platsodds", "PlatsOddsString", "", -1D)]
        [GroupReduction("Max platsodds", 6, 10D, 999D, 1D, 30D, 100D)]
        [DataMember]
        public int MaxPlatsOdds { get; set; }


        private decimal _MinPlatsOddsExact;
        [DataMember]
        public decimal MinPlatsOddsExact
        {
            get
            {
                return _MinPlatsOddsExact;
            }
            set
            {
                _MinPlatsOddsExact = value;
                OnPropertyChanged("MinPlatsOddsExact");
            }
        }

        private decimal _MaxPlatsOddsExact;
        [DataMember]
        public decimal MaxPlatsOddsExact
        {
            get
            {
                return _MaxPlatsOddsExact;
            }
            set
            {
                _MaxPlatsOddsExact = value;
                OnPropertyChanged("MaxPlatsOddsExact");
            }
        }

        private string platsOddsString;
        [XmlIgnore]
        public string PlatsOddsString
        {
            get
            {
                return platsOddsString;
            }
            set
            {
                platsOddsString = value;
                OnPropertyChanged("PlatsOddsString");
            }
        }

        private bool notScratched;
        [XmlIgnore]
        public bool NotScratched
        {
            get
            {
                return notScratched;
            }
            set
            {
                notScratched = value;
                OnPropertyChanged("NotScratched");
            }
        }

        private bool? scratched;
        [DataMember]
        public bool? Scratched
        {
            get
            {
                return scratched;
            }
            set
            {
                if (value == true)
                {
                    VinnarOdds = 9999; // Dummy value to make sure it's bigger than maximum vinnarodds
                    MinPlatsOdds = 9999;
                    MaxPlatsOdds = 9999;
                    VinnarOddsExact = 999.9M; // Dummy value to make sure it's bigger than maximum vinnarodds
                    MinPlatsOddsExact = 999.9M;
                    MaxPlatsOddsExact = 999.9M;
                    VinnarOddsString = "EJ";
                    PlatsOddsString = "EJ";
                    RankATG = 16;
                    RankOwn = 16;
                    RankAlternate = 16;
                    Reserv1 = null;
                    Reserv2 = null;
                    //this.VinnaroddsColor = new SolidColorBrush(Colors.Gray);

                }
                NotScratched = value == null || value == false;
                scratched = value;
                OnPropertyChanged("Scratched");
            }
        }

        private int investmentPlats;
        [DataMember]
        public int InvestmentPlats
        {
            get
            {
                return investmentPlats;
            }
            set
            {
                investmentPlats = value;
                OnPropertyChanged("InvestmentPlats");
            }
        }

        private int investmentVinnare;
        [DataMember]
        public int InvestmentVinnare
        {
            get
            {
                return investmentVinnare;
            }
            set
            {
                investmentVinnare = value;
                OnPropertyChanged("InvestmentVinnare");
            }
        }

        #endregion

        #region Properties from MarkInfo

        [DataMember]
        public decimal MarksPossibleValue { get; set; }

        //private int marksQuantity;
        ////[HorseRank("Streckprocent", 2, true, false, HPTRankCategory.MarksAndOdds, true, true, "Streckprocent", "MarksPercentExact", "P1", 0D)]
        ////[DataMember]
        //public int MarksQuantity
        //{
        //    get
        //    {
        //        return marksQuantity;
        //    }
        //    set
        //    {
        //        marksQuantity = value;
        //        OnPropertyChanged("MarksQuantity");
        //    }
        //}

        //private int marksPercent;
        ////[RandomInterval("Streckprocent", 1, 80, 1D)]
        ////[GroupReduction("Streckprocent", 2, 0D, 100D, 1D, 10D, 50D)]
        ////[DataMember]
        //public int MarksPercent
        //{
        //    get
        //    {
        //        return marksPercent;
        //    }
        //    set
        //    {
        //        marksPercent = value;
        //        OnPropertyChanged("MarksPercent");
        //    }
        //}

        //private decimal marksPercentExact;
        //[XmlIgnore]
        //public decimal MarksPercentExact
        //{
        //    get
        //    {
        //        return marksPercentExact;
        //    }
        //    set
        //    {
        //        marksPercentExact = value;
        //        OnPropertyChanged("MarksPercentExact");
        //    }
        //}

        //private decimal marksShare;
        //[XmlIgnore]
        //public decimal MarksShare
        //{
        //    get
        //    {
        //        return marksShare;
        //    }
        //    set
        //    {
        //        marksShare = value;
        //        OnPropertyChanged("MarksShare");
        //    }
        //}

        private int? rankTip;
        [HorseRank("Tipsrank", 53, false, false, HPTRankCategory.Rest, false, false, "", "", "", 0D)]
        [GroupReduction("Tipsrank", 8, 1D, 16D, 1D, 1D, 16D)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int? RankTip
        {
            get
            {
                return rankTip;
            }
            set
            {
                rankTip = value;
                OnPropertyChanged("RankTip");
            }
        }

        private bool selectedFromTip;
        [XmlIgnore]
        public bool SelectedFromTip
        {
            get
            {
                return selectedFromTip;
            }
            set
            {
                selectedFromTip = value;
                OnPropertyChanged("SelectedFromTip");
            }
        }

        private int numberOfSelectionsFromTips;
        [XmlIgnore]
        public int NumberOfSelectionsFromTips
        {
            get
            {
                return numberOfSelectionsFromTips;
            }
            set
            {
                numberOfSelectionsFromTips = value;
                OnPropertyChanged("NumberOfSelectionsFromTips");
            }
        }

        private HPTPrio prioFromTips;
        [XmlIgnore]
        public HPTPrio PrioFromTips
        {
            get
            {
                return prioFromTips;
            }
            set
            {
                prioFromTips = value;
                PrioStringFromTips = value == HPTPrio.M ? string.Empty : value.ToString();
                //OnPropertyChanged("PrioFromTips");
            }
        }

        private string prioStringFromTips;
        [XmlIgnore]
        public string PrioStringFromTips
        {
            get
            {
                return prioStringFromTips;
            }
            set
            {
                prioStringFromTips = value;
                OnPropertyChanged("PrioStringFromTips");
            }
        }

        [DataMember]
        public string DriverId { get; set; }

        public HPTPerson Driver { get; set; }

        public ATGDriverInformation DriverInfo { get; set; }

        public ATGTrainerInformation TrainerInfo { get; set; }

        [DataMember]
        public string TrainerId { get; set; }

        public HPTPerson Trainer { get; set; }

        public HPTPerson Breeder { get; set; }

        public HPTPerson Owner { get; set; }

        #endregion

        #region HPT properties

        private bool? reserv1;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? Reserv1
        {
            get
            {
                return reserv1;
            }
            set
            {
                reserv1 = value;
                if (reserv1 == true && ParentRace != null)
                {
                    ParentRace.Reserv1Nr = StartNr;
                }
                else if (reserv1 == null && ParentRace != null)
                {
                    ParentRace.Reserv1Nr = 0;
                }
                OnPropertyChanged("Reserv1");
                if (Reserv2 == true && value == true)
                    Reserv2 = null;
            }
        }

        private bool? reserv2;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? Reserv2
        {
            get
            {
                return reserv2;
            }
            set
            {
                reserv2 = value;
                if (reserv2 == true && ParentRace != null)
                {
                    ParentRace.Reserv2Nr = StartNr;
                }
                else if (reserv2 == null && ParentRace != null)
                {
                    ParentRace.Reserv2Nr = 0;
                }
                OnPropertyChanged("Reserv2");
                if (Reserv1 == true && value == true)
                    Reserv1 = null;
            }
        }

        [XmlIgnore]
        public HPTRace ParentRace { get; set; }

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
                if (selected == value)
                {
                    return;
                }
                selected = value;
                //this.RankABC = GetRankABC();
                OnPropertyChanged("Selected");
                if (!selected && HorseXReductionList != null)
                {
                    foreach (HPTHorseXReduction horseXReduction in HorseXReductionList.Where(xr => xr.Selected))
                    {
                        horseXReduction.Selected = false;
                    }
                    Prio = HPTPrio.M;
                    PrioString = string.Empty;
                    //this.RankABC = GetRankABC();
                    SelectedForComplimentaryRule = false;
                    Locked = false;
                }
                if (Driver != null)
                {
                    Driver.SetNameAndNumberOfHorse();
                    Trainer.SetNameAndNumberOfHorse();
                }
                if (ParentRace != null)
                {
                    ParentRace.SetNumberOfSelectedHorses(ParentRace.LegNr, StartNr, Selected);
                }
            }
        }

        private bool? locked;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
                OnPropertyChanged("Locked");
                if (locked == true && !Selected)
                {
                    Selected = true;
                }
            }
        }

        private bool isHighlighted;
        [XmlIgnore]
        public bool IsHighlighted
        {
            get
            {
                return isHighlighted;
            }
            set
            {
                isHighlighted = value;
                OnPropertyChanged("IsHighlighted");
            }
        }

        private bool isHomeTrack;
        [XmlIgnore]
        public bool IsHomeTrack
        {
            get
            {
                return isHomeTrack;
            }
            set
            {
                isHomeTrack = value;
                OnPropertyChanged("IsHomeTrack");
            }
        }

        private bool selectedForComplimentaryRule;
        [XmlIgnore]
        public bool SelectedForComplimentaryRule
        {
            get
            {
                return selectedForComplimentaryRule;
            }
            set
            {
                if (value != selectedForComplimentaryRule)
                {
                    selectedForComplimentaryRule = value;
                    OnPropertyChanged("SelectedForComplimentaryRule");
                    ComplimentaryText = value ? "U" : string.Empty;
                }
            }
        }

        private string complimentaryText;
        [XmlIgnore]
        public string ComplimentaryText
        {
            get
            {
                return complimentaryText;
            }
            set
            {
                complimentaryText = value;
                OnPropertyChanged("ComplimentaryText");
            }
        }

        private bool selectedForRowValueCalculation;
        [XmlIgnore]
        public bool SelectedForRowValueCalculation
        {
            get
            {
                return selectedForRowValueCalculation;
            }
            set
            {
                selectedForRowValueCalculation = value;
                OnPropertyChanged("SelectedForRowValueCalculation");
            }
        }

        private int numberOfCoveredRows;
        [XmlIgnore]
        public int NumberOfCoveredRows
        {
            get
            {
                return numberOfCoveredRows;
            }
            set
            {
                numberOfCoveredRows = value;
                OnPropertyChanged("NumberOfCoveredRows");
            }
        }

        private decimal systemCoverage;
        [XmlIgnore]
        public decimal SystemCoverage
        {
            get
            {
                return systemCoverage;
            }
            set
            {
                systemCoverage = value;
                OnPropertyChanged("SystemCoverage");
            }
        }

        private HPTHorseOwnInformation ownInformation;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseOwnInformation OwnInformation
        {
            get
            {
                if (ownInformation == null)
                {
                    ownInformation = HPTConfig.Config.HorseOwnInformationCollection.GetOwnInformationByName(HorseName);
                    if (ownInformation != null && ownInformation.ATGId == "0")
                    {
                        ownInformation.ATGId = ATGId;
                    }
                }
                return ownInformation;
            }
            set
            {
                ownInformation = value;
                OnPropertyChanged("OwnInformation");
            }
        }

        internal void ActivateABCDChanged()
        {
            if (ParentRace != null)
            {
                ParentRace.ParentRaceDayInfo.ActivateABCDChanged();
            }
        }

        private string prioString;
        [XmlIgnore]
        public string PrioString
        {
            get
            {
                return prioString;
            }
            set
            {
                prioString = value;
                OnPropertyChanged("PrioString");
            }
        }

        private HPTPrio prio;
        [DataMember]
        public HPTPrio Prio
        {
            get
            {
                return prio;
            }
            set
            {
                prio = value;
                RankABC = GetRankABC();
                OnPropertyChanged("Prio");
                if (ParentRace != null)
                {
                    ParentRace.RankABCSum = ParentRace.HorseList
                        .Where(h => h.selected)
                        .Sum(h => h.RankABC);
                }
            }
        }

        internal void HandlePrioChange(HPTPrio prio)
        {
            var xr = HorseXReductionList.First(x => x.Prio == prio);
            HandlePrioChange(xr);
        }

        internal void HandlePrioChange(HPTHorseXReduction xReduction)
        {
            var prio = xReduction.Prio;
            if (xReduction.Selected)
            {
                Prio = prio;
                PrioString = prio.ToString();
                var xrToDeselect = HorseXReductionList.FirstOrDefault(x => x.Prio != prio && x.Selected);
                if (xrToDeselect != null)
                {
                    xrToDeselect.Selected = false;
                }

                if (selected)
                {
                    ActivateABCDChanged();
                }
                else
                {
                    Selected = true;
                }
            }
            else
            {
                var xrSelected = HorseXReductionList.FirstOrDefault(x => x.Selected);
                if (xrSelected == null)
                {
                    Prio = HPTPrio.M;
                    PrioString = string.Empty;
                    ActivateABCDChanged();
                }
            }

            //this.RankABC = GetRankABC();
            //if (this.ParentRace != null)
            //{
            //    this.ParentRace.RankABCSum = this.ParentRace.HorseList
            //        .Where(h => h.selected)
            //        .Sum(h => h.RankABC);
            //}
        }

        internal int GetRankABC()
        {
            int result = 0;
            try
            {
                switch (Prio)
                {
                    case HPTPrio.M:
                        //result = this.Selected ? 0 : 16;
                        result = 0;
                        break;
                    case HPTPrio.A:
                        result = 1;
                        break;
                    case HPTPrio.B:
                        result = 2;
                        break;
                    case HPTPrio.C:
                        result = 3;
                        break;
                    case HPTPrio.D:
                        result = 4;
                        break;
                    case HPTPrio.E:
                        result = 5;
                        break;
                    case HPTPrio.F:
                        result = 6;
                        break;
                    default:
                        result = 0;
                        break;
                }
                //this.RankList.First(r => r.Name == "RankABC").Rank = result;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            return result;
        }

        private int rankABC;
        [HorseRank("ABC-Rank", 54, true, false, HPTRankCategory.Rest, false, false, "", "Prio", "", -1D)]
        public int RankABC
        {
            get
            {
                if (Prio == HPTPrio.M && Selected && rankABC > 0)
                {
                    rankABC = 0;
                    RankList.First(r => r.Name == "RankABC").Rank = 0;
                }
                return rankABC;
            }
            set
            {
                rankABC = value;
                RankList.First(r => r.Name == "RankABC").Rank = value;
                OnPropertyChanged("RankABC");
            }
        }

        public int PrioSort
        {
            get
            {
                switch (Prio)
                {
                    case HPTPrio.M:
                        return 9;
                    case HPTPrio.A:
                    case HPTPrio.B:
                    case HPTPrio.C:
                    case HPTPrio.D:
                    case HPTPrio.E:
                    case HPTPrio.F:
                        return (int)Prio;
                }
                return 9;
            }
        }

        private ObservableCollection<HPTHorseXReduction> horseXReductionList;
        [XmlIgnore]
        public ObservableCollection<HPTHorseXReduction> HorseXReductionList
        {
            get
            {
                return horseXReductionList;
            }
            set
            {
                horseXReductionList = value;
                OnPropertyChanged("HorseXReductionList");
            }
        }

        internal void SetColors()
        {
            Color c = Colors.White;
            if (HPTConfig.Config.SetColorFromVinnarOdds)
            {
                if (VinnarOdds < HPTConfig.Config.ColorIntervalVinnarOdds.LowerBoundary)
                {
                    c = HPTConfig.Config.ColorGood;
                }
                else if (VinnarOdds < HPTConfig.Config.ColorIntervalVinnarOdds.UpperBoundary)
                {
                    c = HPTConfig.Config.ColorMedium;
                }
                else
                {
                    c = HPTConfig.Config.ColorBad;
                }
            }
            else if (HPTConfig.Config.SetColorFromMarkability)
            {
                if (Markability < HPTConfig.Config.ColorIntervalMarkability.LowerBoundary)
                {
                    c = HPTConfig.Config.ColorBad;
                }
                else if (Markability < HPTConfig.Config.ColorIntervalMarkability.UpperBoundary)
                {
                    c = HPTConfig.Config.ColorMedium;
                }
                else
                {
                    c = HPTConfig.Config.ColorGood;
                }
            }
            //else if (HPTConfig.Config.SetColorFromMarksPercent)
            //{
            //    if (this.MarksPercent < HPTConfig.Config.ColorIntervalMarksPercent.LowerBoundary)
            //    {
            //        c = HPTConfig.Config.ColorBad;
            //    }
            //    else if (this.MarksPercent < HPTConfig.Config.ColorIntervalMarksPercent.UpperBoundary)
            //    {
            //        c = HPTConfig.Config.ColorMedium;
            //    }
            //    else
            //    {
            //        c = HPTConfig.Config.ColorGood;
            //    }
            //}
            else if (HPTConfig.Config.SetColorFromStakePercent)
            {
                if (StakeDistributionPercent < HPTConfig.Config.ColorIntervalStakePercent.LowerBoundary)
                {
                    c = HPTConfig.Config.ColorBad;
                }
                else if (StakeDistributionPercent < HPTConfig.Config.ColorIntervalStakePercent.UpperBoundary)
                {
                    c = HPTConfig.Config.ColorMedium;
                }
                else
                {
                    c = HPTConfig.Config.ColorGood;
                }
            }

            if (Scratched == true)
            {
                c = Colors.Gray;
            }
            VinnaroddsColor = new SolidColorBrush(c);
        }

        private Brush platsoddsColor;
        [XmlIgnore]
        public Brush PlatsoddsColor
        {
            get
            {
                Color c = Colors.White;
                if (VinnarOdds < 50)
                {
                    c = Colors.Green;
                }
                else if (VinnarOdds < 100)
                {
                    c = Colors.Yellow;
                }
                else
                {
                    c = Colors.Red;
                }
                LinearGradientBrush lgb = new LinearGradientBrush(c, Colors.White, 90.0);
                return lgb;
            }
            set
            {
                platsoddsColor = value;
                OnPropertyChanged("PlatsoddsColor");
            }
        }

        private Brush vinnaroddsColor;
        [XmlIgnore]
        public Brush VinnaroddsColor
        {
            get
            {
                if (vinnaroddsColor == null)
                {
                    SetColors();
                }
                return vinnaroddsColor;
            }
            set
            {
                vinnaroddsColor = value;
                OnPropertyChanged("VinnaroddsColor");
            }
        }

        private Brush markabilityColor;
        [XmlIgnore]
        public Brush MarkabilityColor
        {
            get
            {
                if (markabilityColor == null)
                {
                    Color c = Colors.White;
                    if (VinnarOdds < 50)
                    {
                        c = HPTConfig.Config.ColorGood;
                    }
                    else if (VinnarOdds < 100)
                    {
                        c = HPTConfig.Config.ColorMedium;
                    }
                    else
                    {
                        c = HPTConfig.Config.ColorBad;
                    }
                    if (Scratched == true)
                    {
                        c = Colors.Gray;
                    }
                    markabilityColor = new SolidColorBrush(c);
                    OnPropertyChanged("MarkabilityColor");
                }
                return markabilityColor;
            }
        }

        //private Brush marksPercentColor;
        //[XmlIgnore]
        //public Brush MarksPercentColor
        //{
        //    get
        //    {
        //        if (marksPercentColor == null)
        //        {
        //            Color c = Colors.White;
        //            if (this.VinnarOdds < 50)
        //            {
        //                //c = Colors.LightGreen;
        //                c = HPTConfig.Config.ColorGood;
        //            }
        //            else if (this.VinnarOdds < 100)
        //            {
        //                //c = Colors.LightYellow;
        //                c = HPTConfig.Config.ColorMedium;
        //            }
        //            else
        //            {
        //                //c = Colors.IndianRed;
        //                c = HPTConfig.Config.ColorBad;
        //            }
        //            if (this.Scratched == true)
        //            {
        //                c = Colors.Gray;
        //            }
        //            marksPercentColor = new SolidColorBrush(c);
        //            OnPropertyChanged("MarksPercentColor");
        //        }
        //        return marksPercentColor;
        //    }
        //}

        private bool? driverChanged;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? DriverChanged
        {
            get
            {
                return driverChanged;
            }
            set
            {
                driverChanged = value;
                OnPropertyChanged("DriverChanged");
            }
        }

        private bool? driverChangedSinceLastStart;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? DriverChangedSinceLastStart
        {
            get
            {
                return driverChangedSinceLastStart;
            }
            set
            {
                driverChangedSinceLastStart = value;
                OnPropertyChanged("DriverChangedSinceLastStart");
            }
        }

        #endregion

        #region Rank properties

        private ObservableCollection<HPTHorseRank> rankList;
        [XmlIgnore]
        public ObservableCollection<HPTHorseRank> RankList
        {
            get
            {
                if (rankList == null || rankList.Count == 0)
                {
                    if (HPTConfig.Config.DefaultRankTemplate.HorseRankVariableList == null || HPTConfig.Config.DefaultRankTemplate.HorseRankVariableList.Count == 0)
                    {
                        HPTConfig.Config.DefaultRankTemplate.HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
                    }
                    rankList = new ObservableCollection<HPTHorseRank>(HPTConfig.Config.DefaultRankTemplate.HorseRankVariableList.Select(rv => new HPTHorseRank()
                    {
                        Name = rv.PropertyName
                    }));
                }
                return rankList;
            }
            set
            {
                rankList = value;
                OnPropertyChanged("RankList");
            }
        }

        private int rankATG;
        [HorseRank("ATG-rank", 51, false, false, HPTRankCategory.Rest, false, false, "", "StartPoint", "## ### ##0", 0D)]
        [GroupReduction("ATG-rank", 6, 1D, 15D, 1D, 1D, 16D)]
        [DataMember]
        public int RankATG
        {
            get
            {
                return rankATG;
            }
            set
            {
                rankATG = value;
                OnPropertyChanged("RankATG");
            }
        }

        private int rankOwn;
        //[HorseRank("Egen rank", 52, false, false, HPTRankCategory.Rest, false, false, "", "", "", 0D)]
        [HorseRank("Egen rank", 52, false, false, HPTRankCategory.Rest, false, false, "", "", "", 0D)]
        [GroupReduction("Egen rank", 7, 1D, 100D, 1D, 1D, 16D)]
        [DataMember]
        public int RankOwn
        {
            get
            {
                if (rankOwn == 0 && RankList != null)
                {
                    var rank = RankList.FirstOrDefault(hr => hr.Name == "StakeDistributionShare");
                    if (rank != null)
                    {
                        rankOwn = rank.Rank;
                    }
                }
                return rankOwn;
            }
            set
            {
                rankOwn = value;
                OnPropertyChanged("RankOwn");
                if (ParentRace != null)
                {
                    ParentRace.RankOwnSum = ParentRace.HorseList.Sum(h => h.RankOwn);
                }
            }
        }

        private decimal? ownProbability;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? OwnProbability
        {
            get
            {
                return ownProbability;
            }
            set
            {
                ownProbability = value;
                OnPropertyChanged("OwnProbability");
                if (ParentRace != null)
                {
                    ParentRace.OwnProbabilitySum = ParentRace.HorseList.Where(h => h.OwnProbability != null).Sum(h => h.OwnProbability);
                    if (ParentRace.ParentRaceDayInfo != null)
                    {
                        switch (ParentRace.ParentRaceDayInfo.BetType.Code)
                        {
                            case "TV":
                                if (ParentRace.CombinationListInfoTvilling == null)
                                {
                                    return;
                                }
                                foreach (var combination in ParentRace.CombinationListInfoTvilling.CombinationList)
                                {
                                    combination.CalculateQuotasTvilling();
                                }
                                break;
                            case "DD":
                            case "LD":
                                if (ParentRace.ParentRaceDayInfo.CombinationListInfoDouble == null)
                                {
                                    return;
                                }
                                foreach (var combination in ParentRace.ParentRaceDayInfo.CombinationListInfoDouble.CombinationList)
                                {
                                    combination.CalculateQuotasDD();
                                }
                                break;
                            case "T":
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private int rankAlternate;
        [HorseRank("Poäng", 53, false, false, HPTRankCategory.Rest, false, false, "", "", "", 0D)]
        [GroupReduction("Poäng", 7, 1D, 100D, 1D, 1D, 16D)]
        [DataMember]
        public int RankAlternate
        {
            get
            {
                if (rankAlternate == 0 && RankList != null)
                {
                    var rank = RankList.FirstOrDefault(hr => hr.Name == "StakeDistributionShare");
                    if (rank != null)
                    {
                        rankAlternate = rank.Rank;
                    }
                }
                return rankAlternate;
            }
            set
            {
                rankAlternate = value;
                OnPropertyChanged("RankAlternate");
                if (ParentRace != null)
                {
                    ParentRace.RankAlternateSum = ParentRace.HorseList.Sum(h => h.RankAlternate);
                }
            }
        }

        private int rankStartNumber;
        [HorseRank("Spårrank", 55, false, false, HPTRankCategory.Rest, false, true, "Spårrank", "", "", 0D)]
        [DataMember]
        public int RankStartNumber
        {
            get
            {
                return rankStartNumber;
            }
            set
            {
                rankStartNumber = value;
                OnPropertyChanged("RankStartNumber");
            }
        }

        private decimal rankMean;
        [GroupReduction("Snittrank", 7, 1D, 15D, 0.1D, 1D, 16D)]
        [XmlIgnore]
        public decimal RankMean
        {
            get
            {
                return rankMean;
            }
            set
            {
                rankMean = value;
                OnPropertyChanged("RankMean");
            }
        }

        private decimal rankWeighted;
        [GroupReduction("Viktad snittrank", 8, 1D, 15D, 0.1D, 1D, 16D)]
        [XmlIgnore]
        public decimal RankWeighted
        {
            get
            {
                return rankWeighted;
            }
            set
            {
                rankWeighted = value;
                OnPropertyChanged("RankWeighted");
            }
        }

        #endregion

        #region Calculated properties

        public string HexCode
        {
            get
            {
                switch (StartNr)
                {
                    case 10:
                        return "A";
                    case 11:
                        return "B";
                    case 12:
                        return "C";
                    case 13:
                        return "D";
                    case 14:
                        return "E";
                    case 15:
                        return "F";
                    case 16:
                        return "G";
                    case 17:
                        return "H";
                    case 18:
                        return "I";
                    case 19:
                        return "J";
                    case 20:
                        return "K";
                    default:
                        return StartNr.ToString();
                }
            }
        }

        internal void SetMarkability()
        {
            if (VinnarOddsShare == 0M || PlatsOddsShare == 0M || StakeDistributionShare == 0M)
            {
                return;
            }
            var shareList = new List<decimal>() { VinnarOddsShare, PlatsOddsShare };
            //shareList.Add(this.StakeDistributionShare);
            //shareList.Add(this.VinnarOddsShare);
            //shareList.Add(this.PlatsOddsShare);
            //shareList.Add(this.TvillingShare);
            //if (this.DoubleShare != null && this.DoubleShare > 0M)
            //{
            //    shareList.Add(Convert.ToDecimal(this.DoubleShare));
            //}
            //if (this.TrioShare != null && this.TrioShare > 0M)
            //{
            //    shareList.Add(Convert.ToDecimal(this.TrioShare));
            //}
            if (StakeShareAlternate != null && StakeShareAlternate > 0M)
            {
                shareList.Add(Convert.ToDecimal(StakeShareAlternate));
            }
            if (StakeShareAlternate2 != null && StakeShareAlternate2 > 0M)
            {
                shareList.Add(Convert.ToDecimal(StakeShareAlternate2));
            }

            decimal avg = shareList.Average(s => s / StakeDistributionShare);
            if (avg == 0M)
            {
                return;
            }
            Markability = avg + StakeDistributionShare;

            //// TEST
            //decimal stdDev = shareList.StdDev();
            //decimal diff = this.StakeDistributionShare - avg;
            //this.Markability = -1M * (this.StakeDistributionShare / avg);// *(decimal)Math.Sqrt(Convert.ToDouble(avg));
            //this.Markability = -1M * (diff / stdDev) * (decimal)Math.Sqrt(shareList.Count);
        }

        private decimal platsQuotient;
        public decimal PlatsQuotient
        {
            get
            {
                return platsQuotient;
            }
            set
            {
                platsQuotient = value;
                OnPropertyChanged("PlatsQuotient");
            }
        }

        private decimal vinnarQuotient;
        public decimal VinnarQuotient
        {
            get
            {
                return vinnarQuotient;
            }
            set
            {
                vinnarQuotient = value;
                OnPropertyChanged("VinnarQuotient");
            }
        }

        internal void CalculateGoodVPBets()
        {
            if (VinnarOddsShare == 0M || PlatsOddsShare == 0M)
            {
                return;
            }
            var shareList = new List<decimal>() { VinnarOddsShare, PlatsOddsShare };
            if (StakeShareAlternate != null && StakeShareAlternate > 0M)
            {
                shareList.Add(Convert.ToDecimal(StakeShareAlternate));
            }
            if (StakeShareAlternate2 != null && StakeShareAlternate2 > 0M)
            {
                shareList.Add(Convert.ToDecimal(StakeShareAlternate2));
            }

            decimal avg = shareList.Average();
            if (avg == 0M || shareList.Count < 3)
            {
                return;
            }

            PlatsQuotient = avg / PlatsOddsShare;
            VinnarQuotient = avg / VinnarOddsShare;
        }

        private decimal markability;
        [HorseRank("Streckbarhet", 6, true, false, HPTRankCategory.MarksAndOdds, true, false, "", "", "F2", 0D)]
        [GroupReduction("Streckbarhet", 8, 0D, 4D, 0.01D, 0D, 4D)]
        [XmlIgnore]
        public decimal Markability
        {
            get
            {
                if (markability == 0M)
                {
                    SetMarkability();
                }
                return markability;
            }
            set
            {
                markability = value;
                OnPropertyChanged("Markability");
            }
        }

        public string ClipboardString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(StartNr);
                if (PrioString != string.Empty && PrioString != null)
                {
                    sb.Append("(");
                    sb.Append(PrioString);
                    sb.Append(")");
                }
                sb.Append(" - ");
                sb.Append(HorseName);

                if (StakeDistributionShare > 0M && HPTConfig.Config.CopyStakeShare == true)
                {
                    sb.Append(" (");
                    sb.Append(StakeDistributionShare.ToString("P1"));
                    sb.Append(")");
                }

                if (RankMean > 0M && RankMean < 16M && HPTConfig.Config.CopyRankMean == true)
                {
                    sb.Append(" (");
                    sb.Append(RankWeighted.ToString("#.0"));
                    sb.Append(")");
                }

                if (RankOwn > 0 && HPTConfig.Config.CopyOwnRank == true)
                {
                    sb.Append(" (");
                    sb.Append(RankOwn);
                    sb.Append(")");
                }

                if (RankAlternate > 0 && HPTConfig.Config.CopyAlternateRank == true)
                {
                    sb.Append(" (");
                    sb.Append(RankAlternate);
                    sb.Append("p)");
                }

                return sb.ToString();
            }
        }

        public string ToClipboardString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(StartNr);
            if (PrioString != string.Empty && PrioString != null)
            {
                sb.Append("(");
                sb.Append(PrioString);
                sb.Append(")");
            }
            sb.Append(" - ");
            sb.Append(HorseName);
            return sb.ToString();
        }

        private decimal vinnarOddsRelative;
        [XmlIgnore]
        public decimal VinnarOddsRelative
        {
            get
            {
                return vinnarOddsRelative;
            }
            set
            {
                vinnarOddsRelative = value;
                OnPropertyChanged("VinnarOddsRelative");
            }
        }

        #endregion

        #region Horsestat properties

        [DataMember]
        public int Age { get; set; }

        [DataMember]
        public string Sex { get; set; }

        [DataMember]
        public int StartPoint { get; set; }

        [DataMember]
        public HPTHorseYearStatistics CurrentYearStatistics { get; set; }

        [DataMember]
        public HPTHorseYearStatistics PreviousYearStatistics { get; set; }

        [DataMember]
        public HPTHorseYearStatistics TotalStatistics { get; set; }

        [XmlIgnore]
        public List<HPTHorseYearStatistics> YearStatisticsList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseRecord> RecordList { get; set; }

        [XmlIgnore]
        public HPTHorseRecord Record { get; set; }

        [HorseRank("Aktuell distans", 41, false, true, HPTRankCategory.Record, true, true, "Rekord", "", "F1", 300D)]
        [XmlIgnore]
        public decimal RecordTime { get; set; }

        [HorseRank("Vägt totalt", 44, false, true, HPTRankCategory.Record, true, false, "", "", "F1", 300D)]
        [XmlIgnore]
        public decimal RecordWeighedTotal { get; set; }

        [HorseRank("Vägt senaste 5", 43, false, true, HPTRankCategory.Record, true, false, "", "", "F1", 300D)]
        [XmlIgnore]
        public decimal RecordWeighedLast5 { get; set; }

        [HorseRank("Vägt senaste 3", 42, false, true, HPTRankCategory.Record, true, false, "", "", "F1", 300D)]
        [XmlIgnore]
        public decimal RecordWeighedLast3 { get; set; }

        [DataMember]
        public string Distance { get; set; }

        [DataMember]
        public string HomeTrack { get; set; }

        [DataMember]
        public int PostPosition { get; set; }

        [HorseRank("Snitt senaste 5", 12, true, true, HPTRankCategory.Winnings, true, false, "", "", "## ### ##0", -1D)]
        [XmlIgnore]
        public int EarningsMeanLast5 { get; set; }

        [HorseRank("Snitt senaste 3", 11, true, true, HPTRankCategory.Winnings, true, true, "Intjänat senaste 3", "", "## ### ##0", -1D)]
        [XmlIgnore]
        public int EarningsMeanLast3 { get; set; }

        [HorseRank("Snitt i år", 13, true, true, HPTRankCategory.Winnings, true, false, "", "", "## ### ##0", -1D)]
        [XmlIgnore]
        public int EarningsMeanThisYear
        {
            get
            {
                return CurrentYearStatistics.EarningMean;
            }
        }

        [HorseRank("Snitt förra året", 14, true, true, HPTRankCategory.Winnings, true, false, "", "", "## ### ##0", -1D)]
        [XmlIgnore]
        public int EarningsMeanLastYear
        {
            get
            {
                return PreviousYearStatistics.EarningMean;
            }
        }

        [HorseRank("Snitt totalt", 15, true, true, HPTRankCategory.Winnings, true, false, "", "", "## ### ##0", -1D)]
        [XmlIgnore]
        public int TotalEarningsMean
        {
            get
            {
                return TotalStatistics.EarningMean;
            }
        }

        [HorseRank("Totalt", 16, true, true, HPTRankCategory.Winnings, true, false, "", "", "## ### ##0", -1D)]
        [XmlIgnore]
        public int TotalEarnings
        {
            get
            {
                return TotalStatistics.Earning;
            }
        }

        [HorseRank("Andel totalt", 25, true, true, HPTRankCategory.Place, true, false, "", "", "P0", -1D)]
        [XmlIgnore]
        public decimal PercentFirstPlaceTotal
        {
            get
            {
                return TotalStatistics.PercentFirstPlace;
            }
        }

        [HorseRank("Andel i år", 23, true, true, HPTRankCategory.Place, true, true, "Vinster i år", "", "P0", -1D)]
        [XmlIgnore]
        public decimal PercentFirstPlaceThisYear
        {
            get
            {
                return CurrentYearStatistics.PercentFirstPlace;
            }
        }

        [HorseRank("Andel i fjol", 24, true, true, HPTRankCategory.Place, true, false, "", "", "P0", -1D)]
        [XmlIgnore]
        public decimal PercentFirstPlaceLastYear
        {
            get
            {
                return PreviousYearStatistics.PercentFirstPlace;
            }
        }

        private decimal meanPlaceLast5;
        [HorseRank("Snitt senaste 5", 22, false, true, HPTRankCategory.Place, true, true, "Placering senaste 5", "", "F1", -1D)]
        [XmlIgnore]
        public decimal MeanPlaceLast5
        {
            get
            {
                if (meanPlaceLast5 == 0M && ResultList != null && ResultList.Count > 0)
                {
                    meanPlaceLast5 = Convert.ToDecimal(ResultList.Average(hr => hr.Place));
                }
                return meanPlaceLast5;
            }
            set
            {
                meanPlaceLast5 = value;
            }
        }

        private decimal meanPlaceLast3;
        [HorseRank("Snitt senaste 3", 21, false, true, HPTRankCategory.Place, true, false, "", "", "F1", -1D)]
        [XmlIgnore]
        public decimal MeanPlaceLast3
        {
            get
            {
                if (meanPlaceLast3 == 0M && ResultList != null && ResultList.Count > 0)
                {
                    meanPlaceLast3 = Convert.ToDecimal(ResultList
                        .OrderByDescending(r => r.Date)
                        .Take(3)
                        .Average(hr => hr.Place));
                }
                return meanPlaceLast3;
            }
            set
            {
                meanPlaceLast3 = value;
            }
        }

        [HorseRank("Andel totalt", 33, true, true, HPTRankCategory.Top3, true, false, "", "", "P0", -1D)]
        [XmlIgnore]
        public decimal PercentTop3Total
        {
            get
            {
                return TotalStatistics.Percent123;
            }
        }

        [HorseRank("Andel i år", 31, true, true, HPTRankCategory.Top3, true, false, "", "", "P0", -1D)]
        [XmlIgnore]
        public decimal PercentTop3ThisYear
        {
            get
            {
                return CurrentYearStatistics.Percent123;
            }
        }

        [HorseRank("Andel i fjol", 32, true, true, HPTRankCategory.Top3, true, false, "", "", "P0", -1D)]
        [XmlIgnore]
        public decimal PercentTop3LastYear
        {
            get
            {
                return PreviousYearStatistics.Percent123;
            }
        }

        private int daysSinceLastStart;
        [GroupReduction("Dagar sen senaste start", 11, 1D, 365D, 1D, 1D, 365D)]
        public int DaysSinceLastStart
        {
            get
            {
                if (daysSinceLastStart == 0)
                {
                    if (ParentRace != null && ParentRace.ParentRaceDayInfo != null && ResultList.Any())
                    {
                        TimeSpan ts = ParentRace.ParentRaceDayInfo.RaceDayDate - ResultList.First().Date;
                        daysSinceLastStart = Convert.ToInt32(Math.Floor(ts.TotalDays));
                    }
                }
                return daysSinceLastStart;
            }
        }

        private string daysBetweenStarts;
        public string DaysBetweenStarts
        {
            get
            {
                if (string.IsNullOrEmpty(daysBetweenStarts))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("(");
                    for (int i = 0, j = 1; j < ResultList.Count; i++, j++)
                    {
                        TimeSpan ts = ResultList[i].Date - ResultList[j].Date;
                        int numberOfDays = Convert.ToInt32(ts.TotalDays);
                        sb.Append(numberOfDays);
                        sb.Append("-");
                    }
                    if (ResultList.Count > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                    sb.Append(")");
                    daysBetweenStarts = sb.ToString();
                }
                return daysBetweenStarts;
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? Shape { get; set; }

        [XmlIgnore]
        public string ResultRow { get; set; }

        #endregion

        #region Result properties

        private bool correct;
        public bool Correct
        {
            get
            {
                return correct;
            }
            set
            {
                correct = value;
                OnPropertyChanged("Correct");
            }
        }

        private int numberOfRowsWithAllCorrect;
        public int NumberOfRowsWithAllCorrect
        {
            get
            {
                return numberOfRowsWithAllCorrect;
            }
            set
            {
                numberOfRowsWithAllCorrect = value;
                OnPropertyChanged("NumberOfRowsWithAllCorrect");
            }
        }

        private int? numberOfRowsWithOneError;
        public int? NumberOfRowsWithOneError
        {
            get
            {
                return numberOfRowsWithOneError;
            }
            set
            {
                numberOfRowsWithOneError = value;
                OnPropertyChanged("NumberOfRowsWithOneError");
            }
        }

        private int? numberOfRowsWithTwoErrors;
        public int? NumberOfRowsWithTwoErrors
        {
            get
            {
                return numberOfRowsWithTwoErrors;
            }
            set
            {
                numberOfRowsWithTwoErrors = value;
                OnPropertyChanged("NumberOfRowsWithTwoErrors");
            }
        }

        private string numberOfRowsWithPossibility;
        public string NumberOfRowsWithPossibility
        {
            get
            {
                return numberOfRowsWithPossibility;
            }
            set
            {
                numberOfRowsWithPossibility = value;
                OnPropertyChanged("NumberOfRowsWithPossibility");
            }
        }

        #endregion

        #region Shareinfo

        private decimal platsOddsShare;
        [XmlIgnore]
        public decimal PlatsOddsShare
        {
            get
            {
                return platsOddsShare;
            }
            set
            {
                platsOddsShare = value;
                OnPropertyChanged("PlatsOddsShare");
            }
        }

        private decimal tvillingShare;
        [DataMember]
        [HorseRank("Tvillingandel", 5, true, false, HPTRankCategory.MarksAndOdds, true, false, "", "", "P1", -1D)]
        public decimal TvillingShare
        {
            get
            {
                return tvillingShare;
            }
            set
            {
                tvillingShare = value;
                OnPropertyChanged("TvillingShare");
            }
        }

        private decimal? doubleShare;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? DoubleShare
        {
            get
            {
                return doubleShare;
            }
            set
            {
                doubleShare = value;
                OnPropertyChanged("DoubleShare");
            }
        }

        private decimal? trioShare;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? TrioShare
        {
            get
            {
                return trioShare;
            }
            set
            {
                trioShare = value;
                OnPropertyChanged("TrioShare");
            }
        }

        private decimal? stakeShareAlternate;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? StakeShareAlternate
        {
            get
            {
                return stakeShareAlternate;
            }
            set
            {
                stakeShareAlternate = value;
                OnPropertyChanged("StakeShareAlternate");
            }
        }

        private decimal? stakeShareAlternate2;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? StakeShareAlternate2
        {
            get
            {
                return stakeShareAlternate2;
            }
            set
            {
                stakeShareAlternate2 = value;
                OnPropertyChanged("StakeShareAlternate2");
            }
        }

        public decimal StakeShareAlternateCombined
        {
            get
            {
                decimal stakeShareAlternate = Convert.ToDecimal(StakeShareAlternate);
                decimal stakeShareAlternate2 = Convert.ToDecimal(StakeShareAlternate2);

                if (stakeShareAlternate > 0M && stakeShareAlternate2 > 0M)
                {
                    return (stakeShareAlternate + stakeShareAlternate2) / 2M;
                }
                if (stakeShareAlternate > 0M)
                {
                    return stakeShareAlternate;
                }
                if (stakeShareAlternate2 > 0M)
                {
                    return stakeShareAlternate2;
                }
                return StakeDistributionShare;
            }
        }

        #endregion

        #region Analys av resultat (rankvariabler)

        internal void CalculateTotalMeanAndStDev()
        {
            RankMeanTotal = Convert.ToDecimal(RankList.Select(hr => hr.Rank).Average());
            decimal variance =
                RankList.Select(hr => hr.Rank).Average(r => (r - RankMeanTotal) * (r - RankMeanTotal));
            RankStDev = Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(variance)));
        }

        private decimal rankMeanTotal;
        [XmlIgnore]
        public decimal RankMeanTotal
        {
            get
            {
                return rankMeanTotal;
            }
            set
            {
                rankMeanTotal = value;
                OnPropertyChanged("RankMeanTotal");
            }
        }

        private decimal rankStDev;
        [XmlIgnore]
        public decimal RankStDev
        {
            get
            {
                return rankStDev;
            }
            set
            {
                rankStDev = value;
                OnPropertyChanged("RankStDev");
            }
        }

        #endregion

        public override string ToString()
        {
            return HorseNumberAndName;
            //return this.StartNr.ToString() + " - " + this.HorseName;
        }
    }

    [DataContract]
    public class HPTHorseStaticInformation
    {
        internal static string CreateStaticInformation(ATGStartBase start)
        {
            // TODO: Anvönd ATGStartBase istället
            var horseStaticInformation = new HPTHorseStaticInformation()
            {
                //CurrentYearStatistics = start.StartInfo.CurrentYearStatistics,
                //PreviousYearStatistics = start.StartInfo.PreviousYearStatistics,
                //TotalStatistics = start.StartInfo.TotalStatistics,
                //ResultList = start.StartInfo.ResultList.ToList(),
                //RecordList = start.StartInfo.RecordList.ToList()
            };
            string jsonString = HPTSerializer.CreateJson(horseStaticInformation);
            return jsonString;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<ATGHorseResult> ResultList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<ATGHorseRecordBase> RecordList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ATGHorseStatistics CurrentYearStatistics { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ATGHorseStatistics PreviousYearStatistics { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ATGHorseStatistics TotalStatistics { get; set; }
    }

}
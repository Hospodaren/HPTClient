using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseTrioInfo : Notifier
    {
        private int? trioIndex;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int? TrioIndex
        {
            get
            {
                return trioIndex;
            }
            set
            {
                trioIndex = value;
                OnPropertyChanged("TrioIndex");
            }
        }

        private HPTHorseTrioPlaceInfo placeInfo1;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo1
        {
            get
            {
                return placeInfo1;
            }
            set
            {
                placeInfo1 = value;
                OnPropertyChanged("PlaceInfo1");
            }
        }

        private HPTHorseTrioPlaceInfo placeInfo2;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo2
        {
            get
            {
                return placeInfo2;
            }
            set
            {
                placeInfo2 = value;
                OnPropertyChanged("PlaceInfo2");
            }
        }

        private HPTHorseTrioPlaceInfo placeInfo3;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo3
        {
            get
            {
                return placeInfo3;
            }
            set
            {
                placeInfo3 = value;
                OnPropertyChanged("PlaceInfo3");
            }
        }

        // private List<HPTHorseTrioPlaceInfo> horseTrioPlaceInfoList;
        // public List<HPTHorseTrioPlaceInfo> HorseTrioPlaceInfoList
        // {
        //     get
        //     {
        //         if (this.horseTrioPlaceInfoList == null && this.PlaceInfo1 != null)
        //         {
        //             this.horseTrioPlaceInfoList = new List<HPTHorseTrioPlaceInfo>()
        //             {
        //                 this.PlaceInfo1,
        //                 this.PlaceInfo2,
        //                 this.PlaceInfo3
        //             };
        //         }
        //         return this.horseTrioPlaceInfoList;
        //     }
        // }
    }

    [DataContract]
    public class HPTHorseTrioPlaceInfo : Notifier
    {
        public int Place { get; set; }

        private int investment;
        [DataMember]
        public int Investment
        {
            get
            {
                return investment;
            }
            set
            {
                investment = value;
                OnPropertyChanged("Investment");
            }
        }

        private decimal investmentShare;
        public decimal InvestmentShare
        {
            get
            {
                return investmentShare;
            }
            set
            {
                investmentShare = value;
                OnPropertyChanged("InvestmentShare");
            }
        }

        private int percent;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int Percent
        {
            get
            {
                return percent;
            }
            set
            {
                percent = value;
                OnPropertyChanged("Percent");
            }
        }

        private bool selected;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
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
    }
}

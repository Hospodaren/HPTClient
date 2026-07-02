using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseTrioInfo : Notifier
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int? TrioIndex
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo1
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo2
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo3
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
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

        [DataMember]
        public int Investment
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public decimal InvestmentShare
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int Percent
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool Selected
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return this.trioIndex;
            }
            set
            {
                this.trioIndex = value;
                OnPropertyChanged("TrioIndex");
            }
        }

        private HPTHorseTrioPlaceInfo placeInfo1;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo1
        {
            get
            {
                return this.placeInfo1;
            }
            set
            {
                this.placeInfo1 = value;
                OnPropertyChanged("PlaceInfo1");
            }
        }

        private HPTHorseTrioPlaceInfo placeInfo2;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo2
        {
            get
            {
                return this.placeInfo2;
            }
            set
            {
                this.placeInfo2 = value;
                OnPropertyChanged("PlaceInfo2");
            }
        }

        private HPTHorseTrioPlaceInfo placeInfo3;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseTrioPlaceInfo PlaceInfo3
        {
            get
            {
                return this.placeInfo3;
            }
            set
            {
                this.placeInfo3 = value;
                OnPropertyChanged("PlaceInfo3");
            }
        }

        private List<HPTHorseTrioPlaceInfo> horseTrioPlaceInfoList;
        public List<HPTHorseTrioPlaceInfo> HorseTrioPlaceInfoList
        {
            get
            {
                if (this.horseTrioPlaceInfoList == null && this.PlaceInfo1 != null)
                {
                    this.horseTrioPlaceInfoList = new List<HPTHorseTrioPlaceInfo>()
                    {
                        this.PlaceInfo1,
                        this.PlaceInfo2,
                        this.PlaceInfo3
                    };
                }
                return this.horseTrioPlaceInfoList;
            }            
        }
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
                this.investment = value;
                OnPropertyChanged("Investment");
            }
        }

        private decimal investmentShare;
        public decimal InvestmentShare
        {
            get
            {
                return this.investmentShare;
            }
            set
            {
                this.investmentShare = value;
                OnPropertyChanged("InvestmentShare");
            }
        }

        private int percent;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int Percent        {
            get
            {
                return this.percent;
            }
            set
            {
                this.percent = value;
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

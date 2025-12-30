using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseNextStart : Notifier, IComparable
    {
        private DateTime _StartDate;
        [DataMember]
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                this._StartDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private int _RaceNumber;
        [DataMember]
        public int RaceNumber
        {
            get
            {
                return _RaceNumber;
            }
            set
            {
                this._RaceNumber = value;
                OnPropertyChanged("RaceNumber");
            }
        }

        private int _TrackId;
        [DataMember]
        public int TrackId
        {
            get
            {
                return _TrackId;
            }
            set
            {
                this._TrackId = value;
                OnPropertyChanged("TrackId");
            }
        }

        private List<string> _BetTypes;
        [DataMember]
        public List<string> BetTypes
        {
            get
            {
                return _BetTypes;
            }
            set
            {
                this._BetTypes = value;
                OnPropertyChanged("BetTypes");
            }
        }

        private List<HPTBetType> betTypeList;
        public List<HPTBetType> BetTypeList
        {
            get
            {
                if (this.betTypeList == null)
                {
                    this.betTypeList = this.BetTypes
                        .Select(bt => new HPTBetType()
                        {
                            Code = bt,
                            Name = bt,
                            IsEnabled = true
                        })
                        //.Select(bt => new HPTBetType(new Uri("/ATGImages/" + bt + "XSmall.png", UriKind.Relative)))
                        .ToList();
                }
                return this.betTypeList;
            }
        }

        public string ATGLink   // För Saxade banor
        {
            get
            {
                return ATGLinkCreator.CreateRaceStartlistLink(this.TrackId, this.StartDate, this.RaceNumber);
            }
        }

        public int CompareTo(object obj)
        {
            return StartDate.CompareTo(obj);
        }


        //private ImageSource betTypeATGLogo;
        //public ImageSource BetTypeATGLogo
        //{
        //    get
        //    {
        //        if (this.betTypeATGLogo == null)
        //        {
        //            string folder = this.IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
        //            this.betTypeATGLogo = GetBetTypeATGLogo();  // new BitmapImage(new Uri(folder + this.Code + "Small.png", UriKind.Relative));
        //        }
        //        return this.betTypeATGLogo;
        //    }
        //    set
        //    {
        //        this.betTypeATGLogo = value;
        //        OnPropertyChanged("BetTypeATGLogo");
        //    }
        //}

        //public ImageSource GetBetTypeATGLogo()
        //{
        //    string folder = this.IsEnabled ? "/ATGImages/" : "/ATGImagesBW/";
        //    return new BitmapImage(new Uri("/ATGImages/" + this.Code + "Small.png", UriKind.Relative));
        //}

    }
}

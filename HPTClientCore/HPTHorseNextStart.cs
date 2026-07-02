using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseNextStart : Notifier, IComparable
    {
        [DataMember]
        public DateTime StartDate
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int RaceNumber
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int TrackId
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public List<string> BetTypes
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        // private List<HPTBetType> betTypeList;
        // public List<HPTBetType> BetTypeList
        // {
        //     get
        //     {
        //         if (this.betTypeList == null)
        //         {
        //             this.betTypeList = this.BetTypes
        //                 .Select(bt => new HPTBetType()
        //                 {
        //                     Code = bt,
        //                     Name = bt,
        //                     IsEnabled = true
        //                 })
        //                 //.Select(bt => new HPTBetType(new Uri("/ATGImages/" + bt + "XSmall.png", UriKind.Relative)))
        //                 .ToList();
        //         }
        //         return this.betTypeList;
        //     }
        // }

        // public string ATGLink   // För Saxade banor
        // {
        //     get
        //     {
        //         return ATGLinkCreator.CreateRaceStartlistLink(this.TrackId, this.StartDate, this.RaceNumber);
        //     }
        // }

        public int CompareTo(object? obj)
        {
            if (obj is null)
                return 1;
            if (obj is HPTHorseNextStart other)
                return StartDate.CompareTo(other.StartDate);
            throw new ArgumentException("Object must be a HPTHorseNextStart", nameof(obj));
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

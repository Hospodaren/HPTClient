using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseResultInfo : Notifier
    {

        private int _Place;
        [DataMember]
        public int Place
        {
            get
            {
                return _Place;
            }
            set
            {
                this._Place = value;
                OnPropertyChanged("Place");
            }
        }

        private int _FinishingPosition;
        [DataMember]
        public int FinishingPosition
        {
            get
            {
                return _FinishingPosition;
            }
            set
            {
                this._FinishingPosition = value;
                OnPropertyChanged("FinishingPosition");
            }
        }


        private int _Earning;
        [DataMember]
        public int Earning
        {
            get
            {
                return _Earning;
            }
            set
            {
                this._Earning = value;
                OnPropertyChanged("Earning");
            }
        }

        private TimeSpan _KmTime;
        [DataMember]
        public TimeSpan KmTime
        {
            get
            {
                return _KmTime;
            }
            set
            {
                this._KmTime = value;
                OnPropertyChanged("KmTime");
            }
        }

        private TimeSpan _TotalTime;
        [DataMember]
        public TimeSpan TotalTime
        {
            get
            {
                return _TotalTime;
            }
            set
            {
                this._TotalTime = value;
                OnPropertyChanged("TotalTime");
            }
        }
        private bool _Disqualified;
        [DataMember]
        public bool Disqualified
        {
            get
            {
                return _Disqualified;
            }
            set
            {
                this._Disqualified = value;
                OnPropertyChanged("Disqualified");
            }
        }

        private string _PlaceString;
        [DataMember]
        public string PlaceString
        {
            get
            {
                return _PlaceString;
            }
            set
            {
                this._PlaceString = value;
                OnPropertyChanged("PlaceString");
            }
        }

        public void SetPlaceString(HPTHorse hptHorse)
        {
            if (hptHorse.Scratched == true)
            {
                this.PlaceString = "-";
                this.FinishingPosition = 50 + hptHorse.StartNr;
            }
            else if (this.Place > 0 && this.Place < 4)
            {
                this.PlaceString = this.Place.ToString();
                if (this.FinishingPosition == 0 || this.FinishingPosition == 999)
                {
                    this.FinishingPosition = this.Place;
                }
            }
            else if (this.FinishingPosition > 0 && this.FinishingPosition < 21) // Diskvalificerad är 41-49 av någon anledning...
            {
                this.PlaceString = this.FinishingPosition.ToString();
            }
            else if (this.FinishingPosition > 20 && this.FinishingPosition < 51) // Diskvalificerad är 41-49 av någon anledning...
            {
                this.PlaceString = "D";
            }
            else if (this.FinishingPosition > 50 && this.FinishingPosition < 70) // Diskvalificerad är 41-49 av någon anledning...
            {
                this.PlaceString = "-";
            }
            else if (this.FinishingPosition == 0 || this.FinishingPosition == 999) // Utländskt lopp som saknar finishingPosition...
            {
                this.PlaceString = "-";
                this.FinishingPosition = 21;
            }
            else
            {
                this.PlaceString = string.Empty;
            }
        }
    }
}

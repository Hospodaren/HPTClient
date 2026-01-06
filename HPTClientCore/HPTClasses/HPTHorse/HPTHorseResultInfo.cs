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
                _Place = value;
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
                _FinishingPosition = value;
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
                _Earning = value;
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
                _KmTime = value;
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
                _TotalTime = value;
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
                _Disqualified = value;
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
                _PlaceString = value;
                OnPropertyChanged("PlaceString");
            }
        }

        public void SetPlaceString(HPTHorse hptHorse)
        {
            if (hptHorse.Scratched == true)
            {
                PlaceString = "-";
                FinishingPosition = 50 + hptHorse.StartNr;
            }
            else if (Place > 0 && Place < 4)
            {
                PlaceString = Place.ToString();
                if (FinishingPosition == 0 || FinishingPosition == 999)
                {
                    FinishingPosition = Place;
                }
            }
            else if (FinishingPosition > 0 && FinishingPosition < 21) // Diskvalificerad är 41-49 av någon anledning...
            {
                PlaceString = FinishingPosition.ToString();
            }
            else if (FinishingPosition > 20 && FinishingPosition < 51) // Diskvalificerad är 41-49 av någon anledning...
            {
                PlaceString = "D";
            }
            else if (FinishingPosition > 50 && FinishingPosition < 70) // Diskvalificerad är 41-49 av någon anledning...
            {
                PlaceString = "-";
            }
            else if (FinishingPosition == 0 || FinishingPosition == 999) // Utländskt lopp som saknar finishingPosition...
            {
                PlaceString = "-";
                FinishingPosition = 21;
            }
            else
            {
                PlaceString = string.Empty;
            }
        }
    }
}

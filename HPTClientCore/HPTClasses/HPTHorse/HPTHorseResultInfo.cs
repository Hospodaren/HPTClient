using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseResultInfo : Notifier
    {
        [DataMember]
        public int Place
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public int FinishingPosition
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }


        [DataMember]
        public int Earning
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public TimeSpan KmTime
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public TimeSpan TotalTime
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool Disqualified
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string PlaceString
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public void SetPlaceString(HPTHorse hptHorse)
        {
            //PlaceString = FinishingPosition switch
            //{

            //};

            if (hptHorse.Scratched == true)
            {
                PlaceString = "-";
                //FinishingPosition = 50 + hptHorse.StartNr;
            }
            else
            {
                PlaceString = FinishingPosition.ToString();
            }
            //else if (Place > 0 && Place < 4)
            //{
            //    PlaceString = Place.ToString();
            //    if (FinishingPosition == 0 || FinishingPosition == 999)
            //    {
            //        FinishingPosition = Place;
            //    }
            //}
            //else if (FinishingPosition > 0 && FinishingPosition < 21) // Diskvalificerad är 41-49 av någon anledning...
            //{
            //    PlaceString = FinishingPosition.ToString();
            //}
            //else if (FinishingPosition > 20 && FinishingPosition < 51) // Diskvalificerad är 41-49 av någon anledning...
            //{
            //    PlaceString = "D";
            //}
            //else if (FinishingPosition > 50 && FinishingPosition < 70) // Diskvalificerad är 41-49 av någon anledning...
            //{
            //    PlaceString = "-";
            //}
            //else if (FinishingPosition == 0 || FinishingPosition == 999) // Utländskt lopp som saknar finishingPosition...
            //{
            //    PlaceString = "-";
            //    FinishingPosition = 21;
            //}
            //else
            //{
            //    PlaceString = string.Empty;
            //}
        }
    }
}

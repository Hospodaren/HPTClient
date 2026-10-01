using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseShoeInfo : Notifier
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? Foreshoes
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                if (value == null)
                {
                    ForeshoesString1 = string.Empty;
                    ForeshoesString2 = string.Empty;
                }
                else if (value == true)
                {
                    ForeshoesString1 = "C";
                    ForeshoesString2 = string.Empty;
                }
                else
                {
                    ForeshoesString1 = "C";
                    ForeshoesString2 = "/";
                }
            }
        }

        [XmlIgnore]
        public bool ForeshoesChanged
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool PreviousUsed
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? Hindshoes
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                if (value == null)
                {
                    HindshoesString1 = string.Empty;
                    HindshoesString2 = string.Empty;
                }
                else if (value == true)
                {
                    HindshoesString1 = "C";
                    HindshoesString2 = string.Empty;
                }
                else
                {
                    HindshoesString1 = "C";
                    HindshoesString2 = "/";
                }
            }
        }

        [XmlIgnore]
        public bool HindshoesChanged
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string ForeshoesString1
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string ForeshoesString2
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string HindshoesString1
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string HindshoesString2
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public void SetChangedFlags(HPTHorseShoeInfo shoeinfoPrevious)
        {
            ForeshoesChanged = Foreshoes != shoeinfoPrevious.Foreshoes;
            HindshoesChanged = Hindshoes != shoeinfoPrevious.Hindshoes;
        }
    }
}

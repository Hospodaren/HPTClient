using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseShoeInfo : Notifier
    {
        private bool? foreshoes;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? Foreshoes
        {
            get
            {
                return foreshoes;
            }
            set
            {
                foreshoes = value;
                OnPropertyChanged("Foreshoes");
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

        private bool foreshoesChanged;
        [XmlIgnore]
        public bool ForeshoesChanged
        {
            get
            {
                return foreshoesChanged;
            }
            set
            {
                foreshoesChanged = value;
                OnPropertyChanged("ForeshoesChanged");
            }
        }

        private bool previousUsed;
        [DataMember]
        public bool PreviousUsed
        {
            get
            {
                return previousUsed;
            }
            set
            {
                previousUsed = value;
                OnPropertyChanged("PreviousUsed");
            }
        }

        private bool? hindshoes;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool? Hindshoes
        {
            get
            {
                return hindshoes;
            }
            set
            {
                hindshoes = value;
                OnPropertyChanged("Hindshoes");
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

        private bool hindshoesChanged;
        [XmlIgnore]
        public bool HindshoesChanged
        {
            get
            {
                return hindshoesChanged;
            }
            set
            {
                hindshoesChanged = value;
                OnPropertyChanged("HindshoesChanged");
            }
        }

        private string foreshoesString1;
        [XmlIgnore]
        public string ForeshoesString1
        {
            get
            {
                return foreshoesString1;
            }
            set
            {
                foreshoesString1 = value;
                OnPropertyChanged("ForeshoesString1");
            }
        }

        private string foreshoesString2;
        [XmlIgnore]
        public string ForeshoesString2
        {
            get
            {
                return foreshoesString2;
            }
            set
            {
                foreshoesString2 = value;
                OnPropertyChanged("ForeshoesString2");
            }
        }

        private string hindshoesString1;
        [XmlIgnore]
        public string HindshoesString1
        {
            get
            {
                return hindshoesString1;
            }
            set
            {
                hindshoesString1 = value;
                OnPropertyChanged("HindshoesString1");
            }
        }

        private string hindshoesString2;
        [XmlIgnore]
        public string HindshoesString2
        {
            get
            {
                return hindshoesString2;
            }
            set
            {
                hindshoesString2 = value;
                OnPropertyChanged("HindshoesString2");
            }
        }

        public void SetChangedFlags(HPTHorseShoeInfo shoeinfoPrevious)
        {
            ForeshoesChanged = Foreshoes != shoeinfoPrevious.Foreshoes;
            HindshoesChanged = Hindshoes != shoeinfoPrevious.Hindshoes;
        }
    }
}

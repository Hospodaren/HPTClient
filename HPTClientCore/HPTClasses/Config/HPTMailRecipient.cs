using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMailRecipient : Notifier
    {
        [DataMember]
        public string Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string EMailAddress
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool Selected
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"{Name} ({EMailAddress})";
        }
    }
}

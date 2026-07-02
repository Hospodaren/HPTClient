using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMailRecipient : Notifier
    {
        private string name;
        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private string eMailAddress;
        [DataMember]
        public string EMailAddress
        {
            get
            {
                return eMailAddress;
            }
            set
            {
                eMailAddress = value;
                OnPropertyChanged();
            }
        }

        private bool selected;
        [XmlIgnore]
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return Name + " (" + EMailAddress + ")";
        }
    }
}

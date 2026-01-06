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
                OnPropertyChanged("Name");
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
                OnPropertyChanged("EMailAddress");
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
                OnPropertyChanged("Selected");
            }
        }

        public override string ToString()
        {
            return Name + " (" + EMailAddress + ")";
        }
    }
}

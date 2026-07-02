using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMailList : Notifier
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
        public ObservableCollection<HPTMailRecipient> RecipientList
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool Expanded
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}

using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMailList : Notifier
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

        private ObservableCollection<HPTMailRecipient> recipientList;
        [DataMember]
        public ObservableCollection<HPTMailRecipient> RecipientList
        {
            get
            {
                return recipientList;
            }
            set
            {
                recipientList = value;
                OnPropertyChanged("RecipientList");
            }
        }

        private bool expanded;
        [XmlIgnore]
        public bool Expanded
        {
            get
            {
                return expanded;
            }
            set
            {
                expanded = value;
                OnPropertyChanged("Expanded");
            }
        }
    }
}

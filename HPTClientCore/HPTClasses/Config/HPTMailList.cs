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
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        private ObservableCollection<HPTMailRecipient> recipientList;
        [DataMember]
        public ObservableCollection<HPTMailRecipient> RecipientList
        {
            get
            {
                return this.recipientList;
            }
            set
            {
                this.recipientList = value;
                OnPropertyChanged("RecipientList");
            }
        }

        private bool expanded;
        [XmlIgnore]
        public bool Expanded
        {
            get
            {
                return this.expanded;
            }
            set
            {
                this.expanded = value;
                OnPropertyChanged("Expanded");
            }
        }
    }
}

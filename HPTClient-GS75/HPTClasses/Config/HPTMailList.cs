using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

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

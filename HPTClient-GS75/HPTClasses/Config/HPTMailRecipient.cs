using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

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
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged("Name");
            }
        }

        private string eMailAddress;
        [DataMember]
        public string EMailAddress
        {
            get
            {
                return this.eMailAddress;
            }
            set
            {
                this.eMailAddress = value;
                OnPropertyChanged("EMailAddress");
            }
        }

        private bool selected;
        [XmlIgnore]
        public bool Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public override string ToString()
        {
            return this.Name + " (" + this.EMailAddress + ")";
        }
    }
}

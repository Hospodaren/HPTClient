using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseSulkyInfo : Notifier
    {
        private bool sulkyChanged;
        [XmlIgnore]
        [DataMember]
        public bool SulkyChanged
        {
            get
            {
                return sulkyChanged;
            }
            set
            {
                sulkyChanged = value;
                OnPropertyChanged("SulkyChanged");
            }
        }

        private string text;
        [XmlIgnore]
        [DataMember]
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                //if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(this.text) && value != this.text)
                //{
                //    this.SulkyChanged = true;
                //}
                this.text = value;
                OnPropertyChanged("Text");
            }
        }        
    }
}

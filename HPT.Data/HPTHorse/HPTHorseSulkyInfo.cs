using System.Runtime.Serialization;
using System.Xml.Serialization;

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
                text = value;
                OnPropertyChanged("Text");
            }
        }
    }
}

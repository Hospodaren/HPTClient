using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseSulkyInfo : Notifier
    {
        [XmlIgnore]
        [DataMember]
        public bool SulkyChanged
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        [DataMember]
        public string Text
        {
            get;
            set
            {
                //if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(this.text) && value != this.text)
                //{
                //    this.SulkyChanged = true;
                //}
                field = value;
                OnPropertyChanged();
            }
        }
    }
}

using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    class HPTNumberOfRules : Notifier
    {
        private int numberOfRules;
        [DataMember]
        public int NumberOfRules
        {
            get
            {
                return numberOfRules;
            }
            set
            {
                numberOfRules = value;
                OnPropertyChanged("NumberOfRules");
            }
        }

        private bool selected;
        [DataMember]
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (selected == value)
                {
                    return;
                }
                selected = value;
                OnPropertyChanged("Selected");
            }
        }

        private bool selectable;
        [DataMember]
        public bool Selectable
        {
            get
            {
                return selectable;
            }
            set
            {
                if (!value && Selected)
                {
                    Selected = false;
                }
                if (selectable == value)
                {
                    return;
                }
                selectable = value;
                OnPropertyChanged("Selectable");
            }
        }
    }
}

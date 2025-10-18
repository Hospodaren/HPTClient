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
                return this.numberOfRules;
            }
            set
            {
                this.numberOfRules = value;
                OnPropertyChanged("NumberOfRules");
            }
        }

        private bool selected;
        [DataMember]
        public bool Selected
        {
            get
            {
                return this.selected;
            }
            set
            {
                if (this.selected == value)
                {
                    return;
                }
                this.selected = value;
                OnPropertyChanged("Selected");
            }
        }

        private bool selectable;
        [DataMember]
        public bool Selectable
        {
            get
            {
                return this.selectable;
            }
            set
            {
                if (!value && this.Selected)
                {
                    this.Selected = false;
                }
                if (this.selectable == value)
                {
                    return;
                }
                this.selectable = value;
                OnPropertyChanged("Selectable");
            }
        }
    }
}

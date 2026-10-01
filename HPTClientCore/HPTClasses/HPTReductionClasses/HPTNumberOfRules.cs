using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    class HPTNumberOfRules : Notifier
    {
        [DataMember]
        public int NumberOfRules
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool Selected
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public bool Selectable
        {
            get;
            set
            {
                if (!value && Selected)
                {
                    Selected = false;
                }
                if (field == value)
                {
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        }
    }
}

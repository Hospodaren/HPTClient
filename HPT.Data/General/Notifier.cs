using System.ComponentModel;

namespace HPTClient
{
    [Serializable]
    public abstract class Notifier : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class PropertySetterInfo
    {
        public string PropertyName { get; set; }

        public Type ClassType { get; set; }

        public object ClassInstance { get; set; }

        public object OldValue { get; set; }

        public object NewValue { get; set; }
    }
}

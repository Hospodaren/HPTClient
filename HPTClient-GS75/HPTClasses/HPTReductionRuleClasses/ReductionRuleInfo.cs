using System.Runtime.Serialization;
using System.Text;
using System.Windows;

namespace HPTClient
{
    [DataContract]
    public class ReductionRuleInfo : Notifier
    {
        private string headlineString;
        [DataMember]
        public string HeadlineString
        {
            get
            {
                return this.headlineString;
            }
            set
            {
                this.headlineString = value;
                OnPropertyChanged("HeadlineString");
            }
        }

        private string reductionTypeString;
        [DataMember]
        public string ReductionTypeString
        {
            get
            {
                return this.reductionTypeString;
            }
            set
            {
                this.reductionTypeString = value;
                OnPropertyChanged("ReductionTypeString");
            }
        }

        private string reductionRuleString;
        [DataMember]
        public string ReductionRuleString
        {
            get
            {
                return this.reductionRuleString;
            }
            set
            {
                this.reductionRuleString = value;
                OnPropertyChanged("ReductionRuleString");
            }
        }

        public Visibility HeadlineVisibility
        {
            get
            {
                return string.IsNullOrEmpty(this.HeadlineString) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility ReductionTypeVisibility
        {
            get
            {
                return string.IsNullOrEmpty(this.ReductionTypeString) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility ReductionRuleVisibility
        {
            get
            {
                return string.IsNullOrEmpty(this.reductionRuleString) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.HeadlineString))
            {
                sb.AppendLine(this.HeadlineString);
            }
            if (!string.IsNullOrEmpty(this.ReductionTypeString))
            {
                sb.AppendLine(this.ReductionTypeString);
            }
            if (!string.IsNullOrEmpty(this.ReductionRuleString))
            {
                sb.AppendLine(this.ReductionRuleString);
            }
            return sb.ToString();
        }
    }
}

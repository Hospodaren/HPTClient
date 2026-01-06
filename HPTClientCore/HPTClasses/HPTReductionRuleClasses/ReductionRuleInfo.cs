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
                return headlineString;
            }
            set
            {
                headlineString = value;
                OnPropertyChanged("HeadlineString");
            }
        }

        private string reductionTypeString;
        [DataMember]
        public string ReductionTypeString
        {
            get
            {
                return reductionTypeString;
            }
            set
            {
                reductionTypeString = value;
                OnPropertyChanged("ReductionTypeString");
            }
        }

        private string reductionRuleString;
        [DataMember]
        public string ReductionRuleString
        {
            get
            {
                return reductionRuleString;
            }
            set
            {
                reductionRuleString = value;
                OnPropertyChanged("ReductionRuleString");
            }
        }

        public Visibility HeadlineVisibility
        {
            get
            {
                return string.IsNullOrEmpty(HeadlineString) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility ReductionTypeVisibility
        {
            get
            {
                return string.IsNullOrEmpty(ReductionTypeString) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility ReductionRuleVisibility
        {
            get
            {
                return string.IsNullOrEmpty(reductionRuleString) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(HeadlineString))
            {
                sb.AppendLine(HeadlineString);
            }
            if (!string.IsNullOrEmpty(ReductionTypeString))
            {
                sb.AppendLine(ReductionTypeString);
            }
            if (!string.IsNullOrEmpty(ReductionRuleString))
            {
                sb.AppendLine(ReductionRuleString);
            }
            return sb.ToString();
        }
    }
}

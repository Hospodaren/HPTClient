using System.Runtime.Serialization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;


namespace HPTClient
{
    [DataContract]
    public class HPTRaceDayInfoLight
    {
        [DataMember]
        public string BetTypeCode { get; set; }

        [DataMember]
        public int TrackId { get; set; }

        [DataMember]
        public string TrackName { get; set; }

        [DataMember]
        public DateTime RaceDayDate { get; set; }

        [DataMember]
        public int NumberOfUploadedSystems { get; set; }

        private Image betTypeATGLogo;
        [XmlIgnore]
        public Image BetTypeATGLogo
        {
            get
            {
                if (betTypeATGLogo == null)
                {
                    betTypeATGLogo = new Image()
                    {
                        Height = 20,
                        Source = new BitmapImage(new Uri("/ATGImages/" + BetTypeCode + "Small.png", UriKind.Relative))
                    };
                }
                return betTypeATGLogo;
            }
        }

        private ImageSource betTypeATGSource;
        [XmlIgnore]
        public ImageSource BetTypeATGSource
        {
            get
            {
                if (betTypeATGSource == null)
                {
                    betTypeATGSource = new BitmapImage(new Uri("/ATGImages/" + BetTypeCode + "Small.png", UriKind.Relative));
                }
                return betTypeATGSource;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(this.BetTypeCode);
            //sb.Append(" ");
            sb.Append(TrackName);
            sb.Append(" (");
            sb.Append(RaceDayDate.ToString("d MMM"));
            sb.Append(")");
            if (NumberOfUploadedSystems > 0)
            {
                sb.Append(" (");
                sb.Append(NumberOfUploadedSystems);
                sb.Append(" system)");
            }
            return sb.ToString();
        }
    }
}

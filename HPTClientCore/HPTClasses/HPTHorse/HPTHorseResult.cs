using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTHorseResult
    {
        [XmlIgnore]
        public string HorseName { get; set; }

        [DataMember]
        public int Position { get; set; }

        [XmlIgnore]
        public int Place { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string PlaceString { get; set; }

        [DataMember]
        public int StartNr { get; set; }

        [DataMember]
        public int RaceNr { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string TrackCode { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? Earning { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal? FirstPrize { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Odds { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Driver { get; set; }

        [DataMember]
        public int Distance { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Time { get; set; }

        private decimal timeWeighed;
        [XmlIgnore]
        public decimal TimeWeighed
        {
            get
            {
                // TODO: Anropa ATGToHPTHelper istället
                //if (timeWeighed == 0M)
                //{
                //    HPTServiceToHPTHelper.SetWeighedTime(this);
                //}
                return timeWeighed;
            }
            set
            {
                timeWeighed = value;
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string RaceType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HPTHorseShoeInfo Shoeinfo { get; set; }

        public List<HPTHorseResult> HeadToHeadResultList { get; set; }

        public string ATGResultLink
        {
            get
            {
                return ATGLinkCreator.CreateRaceResultLink(this);
            }
        }

        public string ATGId { get; set; }

        private Brush backColor;
        [XmlIgnore]
        public Brush BackColor
        {
            get
            {
                if (backColor == null)
                {
                    Color c = Colors.White;
                    if (Place == 1)
                    {
                        //c = Colors.LightGreen;
                        c = HPTConfig.Config.ColorGood;
                    }
                    else if (Place == 2 || Place == 3)
                    {
                        //c = Colors.LightYellow;
                        c = HPTConfig.Config.ColorMedium;
                    }
                    else
                    {
                        //c = Colors.IndianRed;
                        c = HPTConfig.Config.ColorBad;
                    }
                    //backColor = new LinearGradientBrush(c, Colors.White, 90.0);
                    backColor = new SolidColorBrush(c);
                }
                return backColor;
            }
        }
    }
}

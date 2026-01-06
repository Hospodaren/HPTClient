namespace HPTClient
{
    public class ABCDTemplateSettings
    {
        public ABCDTemplateSettings()
        {
        }

        public ABCDTemplateSettings(HPTPrio prio)
        {
            Prio = prio;
        }

        public ABCDTemplateSettings Clone()
        {
            ABCDTemplateSettings abcdTemplateSettings = new ABCDTemplateSettings();

            abcdTemplateSettings.MaxNumberOfSelectedPerRace = MaxNumberOfSelectedPerRace;
            abcdTemplateSettings.MinNumberOfSelectedPerRace = MinNumberOfSelectedPerRace;
            abcdTemplateSettings.Prio = Prio;
            abcdTemplateSettings.Selected = Selected;

            return abcdTemplateSettings;
        }

        public bool Selected { get; set; }

        public HPTPrio Prio { get; set; }

        public int MinNumberOfSelectedPerRace { get; set; }

        public int MaxNumberOfSelectedPerRace { get; set; }
    }
}

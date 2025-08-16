namespace HPTClient
{
    public class ABCDTemplateSettings
    {
        public ABCDTemplateSettings()
        {
        }

        public ABCDTemplateSettings(HPTPrio prio)
        {
            this.Prio = prio;
        }

        public ABCDTemplateSettings Clone()
        {
            ABCDTemplateSettings abcdTemplateSettings = new ABCDTemplateSettings();

            abcdTemplateSettings.MaxNumberOfSelectedPerRace = this.MaxNumberOfSelectedPerRace;
            abcdTemplateSettings.MinNumberOfSelectedPerRace = this.MinNumberOfSelectedPerRace;
            abcdTemplateSettings.Prio = this.Prio;
            abcdTemplateSettings.Selected = this.Selected;

            return abcdTemplateSettings;
        }

        public bool Selected { get; set; }

        public HPTPrio Prio { get; set; }

        public int MinNumberOfSelectedPerRace { get; set; }

        public int MaxNumberOfSelectedPerRace { get; set; }
    }
}

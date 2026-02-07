using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTConfig : Notifier
    {
        #region Skapa och hämta konfiguration

        internal static string ConfigFileName = "HPT7Config.xml";

        public void SaveConfig()
        {
            try
            {
                HPTSerializer.SerializeHPTConfig(Path.Combine(MyDocumentsPath, ConfigFileName), this);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        public void InitializeConfig()
        {
            config = this;
            SetDefaultValues();
            SetDefaultColorIntervals();
            HPTSystemDirectories = new ObservableCollection<HPTSystemDirectory>();
            RecentFileList = new ObservableCollection<HPTSystemFile>();
            MarkBetSystemList = new ObservableCollection<HPTRaceDayInfoLight>();
            AvailableBets = new List<HPTBet>();
            // MailListCollection = new ObservableCollection<HPTMailList>();
            GroupIntervalRulesCollectionList = new();
        }

        private static HPTConfig config;
        public static HPTConfig Config
        {
            get
            {
                if (config == null)
                {
                    CreateHPTConfig();
                }
                return config;
            }
            set
            {
                config = value;
            }
        }

        public static HPTConfig ResetHPTConfig()
        {
            string fileName = MyDocumentsPath + ConfigFileName;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            return CreateHPTConfig();
        }

        public static HPTConfig CreateHPTConfig()
        {
            HPTConfig hptConfig = null;
            try
            {
                // Första uppstart
                if (!Directory.Exists(MyDocumentsPath))
                {
                    Directory.CreateDirectory(MyDocumentsPath);
                }

                // Konfigurationsfilen saknas av någon anledning
                if (!File.Exists(MyDocumentsPath + ConfigFileName))
                {
                    hptConfig = new HPTConfig();
                    hptConfig.InitializeConfig();

                    // Sätt den statiska variablen till rätt config
                    config = hptConfig;
                    config.FirstTimeHPT5User = true;
                }
                else    // Konfigurationsfilen finns och ska laddas in
                {
                    hptConfig = HPTSerializer.DeserializeHPTConfig(MyDocumentsPath + ConfigFileName);
                    config = hptConfig;
                }
                hptConfig.SetRankTemplates();
                //hptConfig.CreateDefaultTemplates();
                hptConfig.AvailableBets = new List<HPTBet>();
            }
            catch (Exception exc)
            {
                if (hptConfig == null)
                {
                    hptConfig = new HPTConfig();
                    hptConfig.InitializeConfig();
                    hptConfig.AddToErrorLog(exc);
                }
                HPTSerializer.SerializeHPTConfig(MyDocumentsPath + ConfigFileName, hptConfig);
            }

            if (hptConfig.RankTemplateList == null)
            {
                hptConfig.RankTemplateList = new ObservableCollection<HPTRankTemplate>();
            }

            //// Katalogen för mallar saknas
            //if (!Directory.Exists(MyDocumentsPath + "Mallar"))
            //{
            //    Directory.CreateDirectory(MyDocumentsPath + "Mallar");
            //}

            //hptConfig.CreateRankVariableLists();
            //hptConfig.CompleteRankTemplates();
            //hptConfig.HandleDataToShow();
            hptConfig.HandleMarkBetTabsToShow();
            //hptConfig.HandleGUIElementsToShow();
            //hptConfig.HandleRankTemplates();
            //hptConfig.HandleGroupIntervalCollectionList();

            config = hptConfig;

            hptConfig.HPTSystemDirectories = new ObservableCollection<HPTSystemDirectory>();
            hptConfig.RecentFileList = new ObservableCollection<HPTSystemFile>();
            hptConfig.MarkBetSystemList = new ObservableCollection<HPTRaceDayInfoLight>();

            return hptConfig;
        }

        internal void SetNonSerializedValues()
        {
            // Katalogen för mallar saknas
            if (!Directory.Exists(MyDocumentsPath + "Mallar"))
            {
                Directory.CreateDirectory(MyDocumentsPath + "Mallar");
            }

            //CreateRankVariableLists();
            CompleteRankTemplates();
            HandleDataToShow();
            //HandleMarkBetTabsToShow();
            HandleGUIElementsToShow();
            HandleRankTemplates();
            HandleGroupIntervalCollectionList();
            var ownInfoCollection = HorseOwnInformationCollection;

        }

        internal static void ExportHPTConfig(string fileName)
        {
            Config.SaveConfig();
            var hptConfig = HPTSerializer.DeserializeHPTConfig(MyDocumentsPath + ConfigFileName);
            hptConfig.UserName = string.Empty;
            hptConfig.Password = string.Empty;
            try
            {
                HPTSerializer.SerializeHPTConfig(fileName, hptConfig);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal static void ImportHPTConfig(string fileName)
        {
            Config.SaveConfig();
            var hptConfig = HPTSerializer.DeserializeHPTConfig(fileName);
            hptConfig.UserName = string.Empty;
            hptConfig.Password = string.Empty;
            try
            {
                HPTSerializer.SerializeHPTConfig(fileName, hptConfig);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal static string ExportTemplatesToDisk()
        {
            try
            {
                string templateDirectory = MyDocumentsPath + @"Mallar\";

                Config.RankTemplateList
                    .ToList()
                    .ForEach(r =>
                    {
                        string fullPath = templateDirectory + CreateFilename(r.Name, "hptrvm");
                        HPTSerializer.SerializeHPTObject(typeof(HPTRankTemplate), fullPath, r);
                    });

                Config.GroupIntervalRulesCollectionList
                    .ToList()
                    .ForEach(r =>
                    {
                        string fullPath = templateDirectory + CreateFilename(r.Name + "_" + r.TypeCategory.ToString(), "hptgim");
                        HPTSerializer.SerializeHPTObject(typeof(HPTGroupIntervalRulesCollection), fullPath, r);
                    });

                Config.MarkBetTemplateABCDList
                    .ToList()
                    .ForEach(r =>
                    {
                        string fullPath = templateDirectory + CreateFilename(r.Name + "_" + r.TypeCategory.ToString(), "hptam");
                        HPTSerializer.SerializeHPTObject(typeof(HPTMarkBetTemplateABCD), fullPath, r);
                    });

                Config.MarkBetTemplateRankList
                    .ToList()
                    .ForEach(r =>
                    {
                        string fullPath = templateDirectory + CreateFilename(r.Name + "_" + r.TypeCategory.ToString(), "hptrm");
                        HPTSerializer.SerializeHPTObject(typeof(HPTMarkBetTemplateRank), fullPath, r);
                    });

                Config.RankSumReductionRuleCollection
                    .ToList()
                    .ForEach(r =>
                    {
                        string fullPath = templateDirectory + CreateFilename(r.Name + "_" + r.TypeCategory.ToString(), "hptrsm");
                        HPTSerializer.SerializeHPTObject(typeof(HPTHorseRankSumReductionRuleCollection), fullPath, r);
                    });
                return templateDirectory;
            }
            catch (Exception exc)
            {
                AddToErrorLogStatic(exc);
                throw exc;
            }
        }

        internal static string CreateFilename(string name, string extension)
        {
            string filename = name.Replace("\\", "_");
            filename = filename.Replace("/", "_");
            foreach (char c in Path.GetInvalidPathChars())
            {
                filename = filename.Replace(c, '_');
            }
            return filename + "." + extension;
        }

        internal static void ImportTemplates(IEnumerable<string> filenameList)
        {
            try
            {
                filenameList
                    .Where(fn => fn.EndsWith(".hptrvm"))
                    .ToList()
                    .ForEach(fn =>
                    {
                        var template = (HPTRankTemplate)HPTSerializer.DeserializeHPTObject(typeof(HPTRankTemplate), fn);
                        Config.RankTemplateList.Add(template);
                    });

                filenameList
                    .Where(fn => fn.EndsWith(".hptgim"))
                    .ToList()
                    .ForEach(fn =>
                    {
                        var template = (HPTGroupIntervalRulesCollection)HPTSerializer.DeserializeHPTObject(typeof(HPTGroupIntervalRulesCollection), fn);
                        Config.GroupIntervalRulesCollectionList.Add(template);
                    });

                filenameList
                    .Where(fn => fn.EndsWith(".hptam"))
                    .ToList()
                    .ForEach(fn =>
                    {
                        var template = (HPTMarkBetTemplateABCD)HPTSerializer.DeserializeHPTObject(typeof(HPTMarkBetTemplateABCD), fn);
                        Config.MarkBetTemplateABCDList.Add(template);
                    });

                filenameList
                    .Where(fn => fn.EndsWith(".hptrm"))
                    .ToList()
                    .ForEach(fn =>
                    {
                        var template = (HPTMarkBetTemplateRank)HPTSerializer.DeserializeHPTObject(typeof(HPTMarkBetTemplateRank), fn);
                        Config.MarkBetTemplateRankList.Add(template);
                    });

                filenameList
                    .Where(fn => fn.EndsWith(".hptrsm"))
                    .ToList()
                    .ForEach(fn =>
                    {
                        var template = (HPTHorseRankSumReductionRuleCollection)HPTSerializer.DeserializeHPTObject(typeof(HPTHorseRankSumReductionRuleCollection), fn);
                        Config.RankSumReductionRuleCollection.Add(template);
                    });

                filenameList
                    .Where(fn => fn.EndsWith(".hpt7m"))
                    .ToList()
                    .ForEach(fn =>
                    {
                        CopyTemplatesToConfig(fn);
                    });
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        internal static void CopyTemplatesToConfig(string filename)
        {
            var templateCollection = HPTSerializer.DeserializeHPTTemplateCollection(filename); ;

            #region Rankvariabelmallar
            foreach (var rankTemplate in templateCollection.RankTemplateList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingRankTemplate = Config.RankTemplateList.FirstOrDefault(rt => rt.Name == rankTemplate.Name);
                if (existingRankTemplate != null)
                {
                    int rankTemplateNumber = 1;
                    string templateName = rankTemplate.Name;
                    while (existingRankTemplate != null)
                    {
                        rankTemplateNumber++;
                        templateName = rankTemplate.Name + " (" + rankTemplateNumber.ToString() + ")";
                        existingRankTemplate = Config.RankTemplateList.FirstOrDefault(rt => rt.Name == templateName);
                    }
                    ChangeRankTemplateReference(templateCollection.MarkBetTemplateABCDList, rankTemplate.Name, templateName);
                    ChangeRankTemplateReference(templateCollection.MarkBetTemplateRankList, rankTemplate.Name, templateName);
                    rankTemplate.Name = templateName;   // Sätt namn med löpnummer efter
                }
                Config.RankTemplateList.Add(rankTemplate);
            }
            #endregion

            #region ABCD-mallar
            foreach (var markBetABCDTemplate in templateCollection.MarkBetTemplateABCDList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = Config.MarkBetTemplateABCDList.FirstOrDefault(t => t.Name == markBetABCDTemplate.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = markBetABCDTemplate.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = markBetABCDTemplate.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = Config.MarkBetTemplateABCDList.FirstOrDefault(t => t.Name == templateName);
                    }
                    markBetABCDTemplate.Name = templateName;   // Sätt namn med löpnummer efter
                }
                markBetABCDTemplate.RankTemplate = Config.RankTemplateList.FirstOrDefault(rt => rt.Name == markBetABCDTemplate.RankTemplateName);
                Config.MarkBetTemplateABCDList.Add(markBetABCDTemplate);
            }
            #endregion

            #region Rankmallar
            foreach (var markBetRankTemplate in templateCollection.MarkBetTemplateRankList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = Config.MarkBetTemplateRankList.FirstOrDefault(t => t.Name == markBetRankTemplate.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = markBetRankTemplate.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = markBetRankTemplate.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = Config.MarkBetTemplateRankList.FirstOrDefault(t => t.Name == templateName);
                    }
                    markBetRankTemplate.Name = templateName;   // Sätt namn med löpnummer efter
                }
                markBetRankTemplate.RankTemplate = Config.RankTemplateList.FirstOrDefault(rt => rt.Name == markBetRankTemplate.RankTemplateName);
                Config.MarkBetTemplateRankList.Add(markBetRankTemplate);
            }
            #endregion

            #region Gruppintervallmallar
            foreach (var groupIntervalRulesCollection in templateCollection.GroupIntervalTemplateList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = Config.GroupIntervalRulesCollectionList.FirstOrDefault(t => t.Name == groupIntervalRulesCollection.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = groupIntervalRulesCollection.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = groupIntervalRulesCollection.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = Config.GroupIntervalRulesCollectionList.FirstOrDefault(t => t.Name == templateName);
                    }
                    groupIntervalRulesCollection.Name = templateName;   // Sätt namn med löpnummer efter
                }
                foreach (HPTGroupIntervalReductionRule rule in groupIntervalRulesCollection.ReductionRuleList)
                {
                    rule.HorseVariable = Config.HorseVariableList.FirstOrDefault(hv => hv.PropertyName == rule.PropertyName);
                    //rule.HorseVariable = HPTHorseVariable.SortedVariableList[rule.PropertyName];
                }
                Config.GroupIntervalRulesCollectionList.Add(groupIntervalRulesCollection);
            }
            #endregion

            #region Rankreduceringsmallar
            var rankVariableList = HPTHorseRankVariable.CreateVariableList();
            foreach (var rankSumReductionCollection in templateCollection.RankSumReductionRuleCollection)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = Config.RankSumReductionRuleCollection.FirstOrDefault(t => t.Name == rankSumReductionCollection.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = rankSumReductionCollection.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = rankSumReductionCollection.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = Config.RankSumReductionRuleCollection.FirstOrDefault(t => t.Name == templateName);
                    }
                    rankSumReductionCollection.Name = templateName;   // Sätt namn med löpnummer efter
                }
                foreach (var rule in rankSumReductionCollection.RankSumReductionRuleList)
                {
                    rule.HorseRankVariable = rankVariableList.FirstOrDefault(rv => rv.PropertyName == rule.PropertyName);
                    foreach (var subRule in rule.ReductionRuleList)
                    {
                        //subRule.
                    }
                    //rule.HorseVariable = HPTConfig.Config.HorseVariableList.FirstOrDefault(hv => hv.PropertyName == rule.PropertyName);
                }
                Config.RankSumReductionRuleCollection.Add(rankSumReductionCollection);
            }
            #endregion
        }

        internal static void ChangeRankTemplateReference(IEnumerable<HPTMarkBetTemplate> templateList, string oldName, string newName)
        {
            foreach (var template in templateList.Where(t => t.Name == oldName).ToList())
            {
                template.RankTemplateName = newName;
            }
        }

        [OnDeserialized]
        public void InitializeOnDeserialized(StreamingContext sc)
        {
            foreach (var dataToShow in DataToShowVxxList)
            {
                dataToShow.Usage = DataToShowUsage.Vxx;
            }
            foreach (var dataToShow in DataToShowVxxList)
            {
                dataToShow.Usage = DataToShowUsage.Vxx;
            }
            foreach (var dataToShow in DataToShowComplementaryRulesList)
            {
                dataToShow.Usage = DataToShowUsage.ComplementaryRule;
            }
            foreach (var dataToShow in DataToShowCorrectionList)
            {
                dataToShow.Usage = DataToShowUsage.Correction;
            }
            foreach (var dataToShow in DataToShowDDList)
            {
                dataToShow.Usage = DataToShowUsage.Double;
            }
            foreach (var dataToShow in DataToShowTrioList)
            {
                dataToShow.Usage = DataToShowUsage.Trio;
            }
            foreach (var dataToShow in DataToShowTvillingList)
            {
                dataToShow.Usage = DataToShowUsage.Tvilling;
            }

            // DD/LD, Trio och Tvilling
            CombinationDataToShowDouble.Usage = DataToShowUsage.Double;
            CombinationDataToShowTrio.Usage = DataToShowUsage.Trio;
            CombinationDataToShowTvilling.Usage = DataToShowUsage.Tvilling;
        }

        private void HandleGroupIntervalCollectionList()
        {
            HorseVariableList = new ObservableCollection<HPTHorseVariable>(HPTHorseVariable.CreateVariableList());
            if (GroupIntervalRulesCollectionList == null)
            {
                GroupIntervalRulesCollectionList = new ObservableCollection<HPTGroupIntervalRulesCollection>();
            }
            foreach (var groupIntervalRulesCollection in GroupIntervalRulesCollectionList)
            {
                foreach (HPTGroupIntervalReductionRule groupIntervalRule in groupIntervalRulesCollection.ReductionRuleList)
                {
                    if (!string.IsNullOrEmpty(groupIntervalRule.PropertyName))
                    {
                        groupIntervalRule.HorseVariable = HorseVariableList.FirstOrDefault(hv => hv.PropertyName == groupIntervalRule.PropertyName);
                    }
                }
            }
        }

        #endregion

        #region Handle Rank variable lists

        private void CompleteRankTemplates()
        {
            // Rankvariabelkmallar
            if (RankTemplateList != null && RankTemplateList.Count > 0)
            {
                var horseRankVariableList = HPTHorseRankVariable.CreateVariableList();

                // Rankvariabelmallar
                foreach (var rankTemplate in RankTemplateList)
                {
                    try
                    {
                        if (rankTemplate.HorseRankVariableList == null || rankTemplate.HorseRankVariableList.Count == 0)
                        {
                            rankTemplate.HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
                        }
                        else if (rankTemplate.HorseRankVariableList.Count < horseRankVariableList.Count)
                        {
                            // Nya som tillkommit i version 5
                            var newRankVariables = horseRankVariableList.Select(rt => rt.PropertyName).Except(rankTemplate.HorseRankVariableList.Select(rt => rt.PropertyName)).ToList();
                            foreach (var propertyName in newRankVariables)
                            {
                                var newRankVariable = horseRankVariableList.First(rv => rv.PropertyName == propertyName).Clone();
                                rankTemplate.HorseRankVariableList.Add(newRankVariable);
                            }
                            // Streckprocent som försvunnit i version 5
                            var rankVariablesToRemove = rankTemplate.HorseRankVariableList.Where(hrv => string.IsNullOrEmpty(hrv.PropertyName)).ToList();
                            foreach (var rankVariable in rankVariablesToRemove)
                            {
                                rankTemplate.HorseRankVariableList.Remove(rankVariable);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        AddToErrorLog(exc);
                    }
                }

                // Rankreduceringsmallar
                foreach (var horseRankSumReductionRuleCollection in RankSumReductionRuleCollection)
                {
                    foreach (var horseRankSumReductionRule in horseRankSumReductionRuleCollection.RankSumReductionRuleList)
                    {
                        horseRankSumReductionRule.HorseRankVariable = horseRankVariableList.FirstOrDefault(hrv => hrv.PropertyName == horseRankSumReductionRule.PropertyName);
                    }
                }
            }
        }

        private void CompleteMarkBetTemplates()
        {
            // ABCD-mallar
            if (MarkBetTemplateABCDList != null && MarkBetTemplateABCDList.Count > 0)
            {
                var rankTemplateList = HPTHorseRankVariable.CreateVariableList();
                foreach (var templateABCD in MarkBetTemplateABCDList)
                {
                    if (templateABCD.RankTemplate != null)
                    {
                        if (templateABCD.RankTemplate.HorseRankVariableList == null || templateABCD.RankTemplate.HorseRankVariableList.Count == 0)
                        {
                            templateABCD.RankTemplate.HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
                        }
                        else if (templateABCD.RankTemplate.HorseRankVariableList.Count < rankTemplateList.Count)
                        {
                            var newRankVariables = rankTemplateList.Select(rt => rt.PropertyName).Except(templateABCD.RankTemplate.HorseRankVariableList.Select(rt => rt.PropertyName)).ToList();
                            foreach (var propertyName in newRankVariables)
                            {
                                var newRankVariable = rankTemplateList.First(rv => rv.PropertyName == propertyName).Clone();
                                templateABCD.RankTemplate.HorseRankVariableList.Add(newRankVariable);
                            }
                        }
                    }
                }
            }

            // Rankmallar
            if (MarkBetTemplateRankList != null && MarkBetTemplateRankList.Count > 0)
            {
                var rankTemplateList = HPTHorseRankVariable.CreateVariableList();
                foreach (var templateRank in MarkBetTemplateRankList)
                {
                    if (templateRank.RankTemplate != null)
                    {
                        if (templateRank.RankTemplate.HorseRankVariableList == null || templateRank.RankTemplate.HorseRankVariableList.Count == 0)
                        {
                            templateRank.RankTemplate.HorseRankVariableList = HPTHorseRankVariable.CreateVariableList();
                        }
                        else if (templateRank.RankTemplate.HorseRankVariableList.Count < rankTemplateList.Count)
                        {
                            var newRankVariables = rankTemplateList.Select(rt => rt.PropertyName).Except(templateRank.RankTemplate.HorseRankVariableList.Select(rt => rt.PropertyName)).ToList();
                            foreach (var propertyName in newRankVariables)
                            {
                                var newRankVariable = rankTemplateList.First(rv => rv.PropertyName == propertyName).Clone();
                                templateRank.RankTemplate.HorseRankVariableList.Add(newRankVariable);
                            }
                        }
                    }
                }
            }
        }

        private void CreateRankVariableLists()
        {
            RankVariableListMarksAndOdds = CreateRankVariableList(HPTRankCategory.MarksAndOdds);
            RankVariableListRecords = CreateRankVariableList(HPTRankCategory.Record);
            RankVariableListWinning = CreateRankVariableList(HPTRankCategory.Winnings);
            RankVariableListPlace = CreateRankVariableList(HPTRankCategory.Place);
            RankVariableListPlace = CreateRankVariableList(HPTRankCategory.Top3);
            RankVariableListRest = CreateRankVariableList(HPTRankCategory.Rest);
        }

        private List<HPTHorseRankVariable> CreateRankVariableList(HPTRankCategory category)
        {
            if (DefaultRankTemplate != null && DefaultRankTemplate.HorseRankVariableList != null)
            {
                List<HPTHorseRankVariable> tempList = DefaultRankTemplate.HorseRankVariableList.Where(hrv => hrv.Category == category).ToList();
                return tempList;
            }
            return new List<HPTHorseRankVariable>();
        }

        [DataMember]
        public bool UseDefaultRankTemplate { get; set; }

        private HPTRankTemplate defaultRankTemplate;
        [XmlIgnore]
        public HPTRankTemplate DefaultRankTemplate
        {
            get
            {
                if (defaultRankTemplate == null)
                {
                    if (RankTemplateList != null)
                    {
                        defaultRankTemplate = RankTemplateList.FirstOrDefault(rt => rt.IsDefault);
                    }
                    if (defaultRankTemplate == null)
                    {
                        defaultRankTemplate = CreateDefaultRankTemplate();
                    }
                }
                return defaultRankTemplate;
            }
            set
            {
                defaultRankTemplate = value;
                if (value != null)
                {
                    defaultRankTemplate.IsDefault = true;
                    if (RankTemplateList != null)
                    {
                        foreach (var rankTemplate in RankTemplateList.Where(rt => rt != defaultRankTemplate))
                        {
                            rankTemplate.IsDefault = false;
                        }
                    }
                }

            }
        }

        [DataMember]
        public bool UseDefaultRankTemplateDouble { get; set; }

        private HPTRankTemplate defaultRankTemplateDouble;
        [XmlIgnore]
        public HPTRankTemplate DefaultRankTemplateDouble
        {
            get
            {
                if (defaultRankTemplateDouble == null)
                {
                    if (RankTemplateList != null)
                    {
                        defaultRankTemplateDouble = RankTemplateList.FirstOrDefault(rt => rt.IsDefaultDouble);
                    }
                    if (defaultRankTemplateDouble == null)
                    {
                        defaultRankTemplateDouble = CreateDefaultRankTemplateDouble();
                    }
                }
                return defaultRankTemplateDouble;
            }
            set
            {
                defaultRankTemplateDouble = value;
                if (value != null)
                {
                    defaultRankTemplateDouble.IsDefaultDouble = true;
                    if (RankTemplateList != null)
                    {
                        foreach (var rankTemplate in RankTemplateList.Where(rt => rt != defaultRankTemplateDouble))
                        {
                            rankTemplate.IsDefaultDouble = false;
                        }
                    }
                }

            }
        }

        [DataMember]
        public bool UseDefaultRankTemplateTvilling { get; set; }

        private HPTRankTemplate defaultRankTemplateTvilling;
        [XmlIgnore]
        public HPTRankTemplate DefaultRankTemplateTvilling
        {
            get
            {
                if (defaultRankTemplateTvilling == null)
                {
                    if (RankTemplateList != null)
                    {
                        defaultRankTemplateTvilling = RankTemplateList.FirstOrDefault(rt => rt.IsDefaultTvilling);
                    }
                    if (defaultRankTemplateTvilling == null)
                    {
                        defaultRankTemplateTvilling = CreateDefaultRankTemplateTvilling();
                    }
                }
                return defaultRankTemplateTvilling;
            }
            set
            {
                defaultRankTemplateTvilling = value;
                if (value != null)
                {
                    defaultRankTemplateTvilling.IsDefaultTvilling = true;
                    if (RankTemplateList != null)
                    {
                        foreach (var rankTemplate in RankTemplateList.Where(rt => rt != defaultRankTemplateTvilling))
                        {
                            rankTemplate.IsDefaultTvilling = false;
                        }
                    }
                }

            }
        }

        [DataMember]
        public bool UseDefaultRankTemplateTrio { get; set; }

        private HPTRankTemplate defaultRankTemplateTrio;
        [XmlIgnore]
        public HPTRankTemplate DefaultRankTemplateTrio
        {
            get
            {
                if (defaultRankTemplateTrio == null)
                {
                    if (RankTemplateList != null)
                    {
                        defaultRankTemplateTrio = RankTemplateList.FirstOrDefault(rt => rt.IsDefaultTrio);
                    }
                    if (defaultRankTemplateTrio == null)
                    {
                        defaultRankTemplateTrio = CreateDefaultRankTemplateTrio();
                    }
                }
                return defaultRankTemplateTrio;
            }
            set
            {
                defaultRankTemplateTrio = value;
                if (value != null)
                {
                    defaultRankTemplateTrio.IsDefaultTrio = true;
                    if (RankTemplateList != null)
                    {
                        foreach (var rankTemplate in RankTemplateList.Where(rt => rt != defaultRankTemplateTrio))
                        {
                            rankTemplate.IsDefaultTrio = false;
                        }
                    }
                }

            }
        }

        private ObservableCollection<HPTRankTemplate> rankTemplateList;
        [DataMember]
        public ObservableCollection<HPTRankTemplate> RankTemplateList
        {
            get
            {
                if (rankTemplateList == null)
                {
                    rankTemplateList = new ObservableCollection<HPTRankTemplate>();
                }
                return rankTemplateList;
            }
            set
            {
                rankTemplateList = value;
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTHorseVariable> HorseVariableList { get; set; }

        [DataMember]
        public ObservableCollection<HPTGroupIntervalRulesCollection> GroupIntervalRulesCollectionList { get; set; }

        [XmlIgnore]
        public List<HPTHorseRankVariable> RankVariableListMarksAndOdds { get; set; }

        [XmlIgnore]
        public List<HPTHorseRankVariable> RankVariableListRecords { get; set; }

        [XmlIgnore]
        public List<HPTHorseRankVariable> RankVariableListWinning { get; set; }

        [XmlIgnore]
        public List<HPTHorseRankVariable> RankVariableListPlace { get; set; }

        [XmlIgnore]
        public List<HPTHorseRankVariable> RankVariableListTop3 { get; set; }

        [XmlIgnore]
        public List<HPTHorseRankVariable> RankVariableListRest { get; set; }

        #endregion

        private HPTHorseOwnInformationCollection horseOwnInformationCollection;
        [XmlIgnore]
        public HPTHorseOwnInformationCollection HorseOwnInformationCollection
        {
            get
            {
                if (horseOwnInformationCollection == null)
                {
                    horseOwnInformationCollection = HPTSerializer.DeserializeHPTHorseOwnInformation(MyDocumentsPath + "HorseOwnInformationList.hptinfo");
                    //string dir = HPTConfig.MyDocumentsPath + "OwnHorseInformation\\";
                    //if (!Directory.Exists(dir))
                    //{
                    //    Directory.CreateDirectory(dir);
                    //    this.horseOwnInformationCollection.HorseOwnInformationList
                    //        .ToList()
                    //        .ForEach(hoi =>
                    //        {
                    //            // Spara på disk med tillfixat hästnamn 0 ".xml"
                    //            string filename = hoi.HorseNameWithoutInvalidCharacters + ".xml";
                    //            HPTSerializer.SerializeHPTHorseOwnInformation(filename, hoi);
                    //        });
                    //}
                }
                return horseOwnInformationCollection;
            }
        }

        private string systemSizesToShowString;
        [DataMember]
        public string SystemSizesToShowString
        {
            get
            {
                if (string.IsNullOrEmpty(systemSizesToShowString) || systemSizesToShowString.Contains("r"))
                {
                    systemSizesToShowString = "50\r\n100\r\n250\r\n500\r\n1000\r\n2500\r\n5000\r\n10000\r\n999999999";
                }
                return systemSizesToShowString;
            }
            set
            {
                systemSizesToShowString = value;
                if (string.IsNullOrEmpty(systemSizesToShowString) || systemSizesToShowString.Contains("r"))
                {
                    systemSizesToShowString = "50\r\n100\r\n250\r\n500\r\n1000r\n2500\r\n5000\r\n10000\r\n999999999";
                }
                OnPropertyChanged("SystemSizesToShowString");

                systemSizesToShow = new ObservableCollection<int>();
                var sizeStringArray = systemSizesToShowString.Split(new String[] { "\r\n" }, StringSplitOptions.None);
                foreach (var s in sizeStringArray)
                {
                    int number = 0;
                    bool result = int.TryParse(s, out number);
                    if (result)
                    {
                        systemSizesToShow.Add(number);
                    }
                }
                OnPropertyChanged("SystemSizesToShow");
            }
        }


        private ObservableCollection<int> systemSizesToShow;
        [XmlIgnore]
        public ObservableCollection<int> SystemSizesToShow
        {
            get
            {
                if (systemSizesToShow == null || systemSizesToShow.Count == 0)
                {
                    systemSizesToShow = new ObservableCollection<int>(new int[] { 50, 100, 250, 500, 1000, 2500, 5000, 10000 });
                }
                return systemSizesToShow;
            }
            set
            {
                systemSizesToShow = value;
                OnPropertyChanged("SystemSizesToShow");
            }
        }

        private string beginnerSizesToShowString;
        [DataMember]
        public string BeginnerSizesToShowString
        {
            get
            {
                if (string.IsNullOrEmpty(beginnerSizesToShowString) || systemSizesToShowString.Contains("r"))
                {
                    beginnerSizesToShowString = "50\r\n100\r\n200\r\n300\r\n400\r\n500\r\n700\r\n1000\r\n2000";
                }
                return beginnerSizesToShowString;
            }
            set
            {
                beginnerSizesToShowString = value;
                if (string.IsNullOrEmpty(systemSizesToShowString))
                {
                    beginnerSizesToShowString = "50\r\n100\r\n200\r\n300\r\n400\r\n500\r\n700\r\n1000\r\n2000";
                }
                OnPropertyChanged("SystemSizesToShowString");

                beginnerSizesToShow = new ObservableCollection<int>();
                var sizeStringArray = beginnerSizesToShowString.Split(new String[] { "\r\n" }, StringSplitOptions.None);
                foreach (var s in sizeStringArray)
                {
                    int number = 0;
                    bool result = int.TryParse(s, out number);
                    if (result)
                    {
                        beginnerSizesToShow.Add(number);
                    }
                }
                OnPropertyChanged("BeginnerSizesToShowString");
            }
        }


        private ObservableCollection<int> beginnerSizesToShow;
        [XmlIgnore]
        public ObservableCollection<int> BeginnerSizesToShow
        {
            get
            {
                if (beginnerSizesToShow == null || beginnerSizesToShow.Count == 0)
                {
                    beginnerSizesToShow = new ObservableCollection<int>(new int[] { 100, 200, 300, 500, 700, 1000, 2500, 5000, 10000 });
                }
                return beginnerSizesToShow;
            }
            set
            {
                beginnerSizesToShow = value;
                OnPropertyChanged("BeginnerSizesToShow");
            }
        }


        private ObservableCollection<HPTSystemDirectory> hptSystemDirectories;
        [XmlIgnore]
        public ObservableCollection<HPTSystemDirectory> HPTSystemDirectories
        {
            get
            {
                return hptSystemDirectories;
            }
            set
            {
                hptSystemDirectories = value;
                OnPropertyChanged("HPTSystemDirectories");

                // 10 senaste filerna
                RecentFileList = new ObservableCollection<HPTSystemFile>(
                    HPTSystemDirectories
                    .SelectMany(hsd => hsd.FileList)
                    .OrderByDescending(hsf => hsf.CreationTime)
                    .Take(10)
                    );
            }
        }

        [XmlIgnore]
        private ObservableCollection<HPTSystemFile> recentFileList;
        public ObservableCollection<HPTSystemFile> RecentFileList
        {
            get
            {
                return recentFileList;
            }
            set
            {
                recentFileList = value;
                OnPropertyChanged("RecentFileList");
            }
        }

        [XmlIgnore]
        public ObservableCollection<HPTRaceDayInfoLight> MarkBetSystemList { get; set; }

        #region Felloggning

        [XmlIgnore]
        public ObservableCollection<Exception> ErrorLog { get; set; }

        internal void MailErrorLog()
        {
            if (ErrorLog == null || ErrorLog.Count == 0)
            {
                return;
            }
            MailMessage mail = new MailMessage();

            mail.To.Add("hjalp.pa.traven@gmail.com");
            mail.Subject = "Felrapport " + DateTime.Now.ToShortTimeString();

            mail.From = new MailAddress("hpt.travsystem@gmail.com", "Hjälp på Traven-system");
            mail.Sender = new MailAddress("hpt.travsystem@gmail.com", "Hjälp på Traven-system");
            mail.IsBodyHtml = false;

            StringBuilder sb = new StringBuilder();

            if (EMailAddress != null && EMailAddress != string.Empty)
            {
                sb.Append("Felrapport från ");
                sb.AppendLine(EMailAddress);
            }

            foreach (Exception exc in ErrorLog)
            {
                sb.AppendLine(exc.Message);
                sb.AppendLine(exc.StackTrace);
                sb.AppendLine();
                Exception innerExc = exc.InnerException;

                while (innerExc != null)
                {
                    sb.AppendLine(innerExc.Message);
                    sb.AppendLine(innerExc.StackTrace);
                    sb.AppendLine();
                    innerExc = innerExc.InnerException;
                }
            }

            mail.Body = sb.ToString();

            System.Net.NetworkCredential cred = new System.Net.NetworkCredential("hpt.travsystem", "Brickleberry2");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = cred;
            smtp.Port = 587;
            smtp.Send(mail);
        }

        private SortedList<HPTPrio, bool> prioList;
        [XmlIgnore]
        public SortedList<HPTPrio, bool> PrioList
        {
            get
            {
                if (prioList == null)
                {
                    prioList = new SortedList<HPTPrio, bool>();
                    prioList.Add(HPTPrio.A, UseA);
                    prioList.Add(HPTPrio.B, UseB);
                    prioList.Add(HPTPrio.C, UseC);
                    prioList.Add(HPTPrio.D, UseD);
                    prioList.Add(HPTPrio.E, UseE);
                    prioList.Add(HPTPrio.F, UseF);
                }
                return prioList;
            }
        }

        internal static string logFilePath;
        internal void SaveLogFile()
        {
            // Inga fel att logga till fil
            if (ErrorLog == null || Config.ErrorLog.Count == 0)
            {
                return;
            }

            string directoryPath = MyDocumentsPath + @"Logg\";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (string.IsNullOrEmpty(logFilePath))
            {
                logFilePath = directoryPath + "Logg " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            }
            var sw = new StreamWriter(logFilePath, true);
            var sb = new StringBuilder();
            foreach (var error in ErrorLog)
            {
                sb.AppendLine(error.Message);
                sb.AppendLine(error.StackTrace);
                sb.AppendLine();
                sb.AppendLine();
            }
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();
        }

        internal static void SaveLogFileStatic()
        {
            // Inga fel att logga till fil
            if (Config.ErrorLog == null || Config.ErrorLog.Count == 0)
            {
                return;
            }

            string directoryPath = MyDocumentsPath + @"Logg\";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (string.IsNullOrEmpty(logFilePath))
            {
                logFilePath = directoryPath + "Logg " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            }
            var sw = new StreamWriter(logFilePath, true);
            var sb = new StringBuilder();
            foreach (var error in Config.ErrorLog)
            {
                sb.AppendLine(error.Message);
                sb.AppendLine(error.StackTrace);
                sb.AppendLine();
                sb.AppendLine();
            }
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();
        }

        public void AddToErrorLog(Exception exc)
        {
            try
            {
                if (ErrorLog == null)
                {
                    ErrorLog = new ObservableCollection<Exception>();
                }
                while (exc != null)
                {
                    ErrorLog.Insert(0, exc);
                    exc = exc.InnerException;
                }
                SaveLogFile();
            }
            catch (Exception)
            {
            }
        }

        public static void AddToErrorLogStatic(Exception exc)
        {
            try
            {
                if (Config.ErrorLog == null)
                {
                    Config.ErrorLog = new ObservableCollection<Exception>();
                }
                while (exc != null)
                {
                    Config.ErrorLog.Insert(0, exc);
                    exc = exc.InnerException;
                }
                SaveLogFileStatic();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        [XmlIgnore]
        public List<HPTBet> AvailableBets { get; set; }

        internal IEnumerable<HPTBet> GetOtherBetsFromSameMeet(HPTBet bet)
        {
            var betList = AvailableBets
                .Where(b => b.RaceDayInfo.RaceDayDateString == bet.RaceDayInfo.RaceDayDateString && b.RaceDayInfo.TrackId == bet.RaceDayInfo.TrackId)
                .Except(new HPTBet[] { bet });

            return betList;
        }

        internal void ApplyOwnRankFromOtherBet(HPTBet sourceBet, HPTBet destinationBet)
        {
            destinationBet.RaceDayInfo.RaceList.ForEach(race =>
                {
                    var sourceRace = sourceBet.RaceDayInfo.RaceList.FirstOrDefault(r => r.RaceNr == race.RaceNr);
                    if (sourceRace != null)
                    {
                        sourceRace.HorseList.ToList().ForEach(horse =>
                            {
                                var destinationHorse = race.HorseList.First(h => h.StartNr == horse.StartNr);
                                destinationHorse.RankOwn = horse.RankOwn;
                                destinationHorse.RankAlternate = horse.RankAlternate;
                            });
                    }
                });
        }

        public void UpdateHPTSystemDirectories(object timerData)
        {
            UpdateHPTSystemDirectories();
        }

        public void UpdateHPTSystemDirectories()
        {
            DirectoryInfo di = new DirectoryInfo(MyDocumentsPath);

            // Kataloger som är automatgenererade för att innehålla filer för olika tävlingar
            Regex rexSystemDirectory = new Regex("\\d{4}-\\d{2}-\\d{2}\\s[\\w\\s]+?");

            List<HPTSystemDirectory> hptSystemDirectories = di.GetDirectories()
                .Where(diSystemDir => rexSystemDirectory.IsMatch(diSystemDir.Name))
                .Select(diSystemDir => new HPTSystemDirectory()
                {
                    DirectoryName = diSystemDir.FullName,
                    DirectoryNameShort = diSystemDir.Name,
                    FileList = new ObservableCollection<HPTSystemFile>(
                        diSystemDir.GetFiles("*.hpt?")
                        .Select(fiHPTFile => new HPTSystemFile()
                        {
                            DisplayName = CreateFileDisplayName(fiHPTFile.FullName, fiHPTFile.LastWriteTime),
                            FileName = fiHPTFile.FullName,
                            FileNameShort = fiHPTFile.Name,
                            FileType = fiHPTFile.Extension.Replace(".", string.Empty),
                            CreationTime = fiHPTFile.CreationTime
                        }))
                })
                .ToList();

            // Ta bara kataloger som innehåller filer
            HPTSystemDirectories = new ObservableCollection<HPTSystemDirectory>(
                hptSystemDirectories
                .Where(hsd => hsd.FileList.Count > 0)
                .OrderByDescending(hsd => hsd.DirectoryNameShort)
                .Take(20)
                );

            // 10 senaste filerna
            //this.HPTSystemDirectories
            //    .SelectMany(hsd => hsd.FileList)
            //    .OrderByDescending(hsf => hsf.CreationTime)
            //    .Take(10)
            //    .ToList()
            //    .ForEach(hsf => this.RecentFileList.Add(hsf));

            //this.RecentFileList = new ObservableCollection<HPTSystemFile>(
            //    this.HPTSystemDirectories
            //    .SelectMany(hsd => hsd.FileList)
            //    .OrderByDescending(hsf => hsf.CreationTime)
            //    .Take(10)
            //    );
        }

        private string CreateFileDisplayName(string fileName, DateTime creationDate)
        {
            string displayName = Path.GetFileNameWithoutExtension(fileName);
            displayName = displayName.Replace("_", "-");
            displayName += " (" + creationDate.ToString("yyyy-MM-dd H:mm") + ")";

            return displayName;
        }

        #region Hantering av default-värden

        private void SetDefaultValues()
        {
            ApplicationHeight = 768D;
            ApplicationWidth = 1280D;

            #region Data to show

            //this.MarkBetTabsToShow = new HPTMarkBetTabsToShow()
            //    {
            //        ShowAdvanced = false,
            //        ShowComplimentaryRules = true,
            //        ShowCorrection = true,
            //        ShowDriverReduction = true,
            //        ShowIntervalReduction = true,
            //        ShowGroupIntervalReduction = false,
            //        ShowOverview = true,
            //        ShowRaces = true,
            //        ShowRankOverview = true,
            //        ShowSingleRows = true,
            //        ShowTrainerReduction = true,
            //        ShowCompanyGambling = true
            //    };

            //this.DataToShowVxx = new HPTHorseDataToShow()
            //    {
            //        Usage = DataToShowUsage.Vxx,
            //        EnableConfiguration = true,
            //        ShowDriver = true,
            //        ShowDriverPopup = true,
            //        ShowHorsePopup = true,
            //        ShowMarkability = true,
            //        ShowMarksPercent = true,
            //        ShowMarksQuantity = true,
            //        ShowMarksShare = true,
            //        ShowName = true,
            //        ShowPlatsOdds = true,
            //        ShowPrio = true,
            //        ShowStartNr = true,
            //        ShowSystemCoverage = true,
            //        ShowStakeDistributionPercent = true,
            //        ShowTrainer = true,
            //        ShowTrainerPopup = true,
            //        ShowVinnarOdds = true,
            //        ShowReserv = true
            //    };

            //this.DataToShowComplementaryRules = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.Vxx,
            //    EnableConfiguration = true,
            //    ShowDriver = true,
            //    ShowDriverPopup = true,
            //    ShowHorsePopup = true,
            //    ShowLegNrText = true,
            //    ShowMarksPercent = true,
            //    ShowMarksQuantity = true,
            //    ShowMarksShare = true,
            //    ShowName = true,
            //    ShowPlatsOdds = true,
            //    ShowPrio = true,
            //    ShowStartNr = true,
            //    ShowSystemCoverage = true,
            //    ShowStakeDistributionPercent = true,
            //    ShowTrainer = true,
            //    ShowTrainerPopup = true,
            //    ShowVinnarOdds = true,
            //    ShowComplimentaryRuleSelect = true
            //};

            //this.DataToShowCorrection = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.Vxx,
            //    EnableConfiguration = true,
            //    ShowDriver = true,
            //    ShowDriverPopup = true,
            //    ShowHorsePopup = true,
            //    ShowLegNrText = true,
            //    ShowMarksPercent = true,
            //    ShowMarksQuantity = true,
            //    ShowMarksShare = true,
            //    ShowName = true,
            //    ShowPrio = true,
            //    ShowStartNr = true,
            //    ShowSystemCoverage = true,
            //    ShowStakeDistributionPercent = true,
            //    ShowTrainer = true,
            //    ShowTrainerPopup = true,
            //    ShowVinnarOdds = true,
            //    ShowSystemsLeft = true,
            //    ShowSystemValue = true
            //};

            //this.DataToShowPersonReduction = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.Vxx,
            //    EnableConfiguration = true,
            //    ShowDriver = true,
            //    ShowDriverPopup = true,
            //    ShowHorsePopup = true,
            //    ShowMarksPercent = true,
            //    ShowMarksQuantity = true,
            //    ShowName = true,
            //    ShowPrio = true,
            //    ShowStartNr = true,
            //    ShowSystemCoverage = true,
            //    ShowStakeDistributionPercent = true,
            //    ShowTrainer = true,
            //    ShowTrainerPopup = true,
            //    ShowVinnarOdds = true,
            //    ShowLegNrText = true
            //};

            //this.DataToShowDriverPopup = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.None,
            //    ShowTrainer = true,
            //    ShowHorsePopup = true,
            //    ShowName = true,
            //    ShowStartNr = true,
            //    ShowVinnarOdds = true,
            //    ShowPrio = true,
            //    ShowStakeDistributionPercent = true,
            //    ShowMarksPercent = true
            //};

            //this.DataToShowTrainerPopup = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.None,
            //    ShowDriver = true,
            //    ShowHorsePopup = true,
            //    ShowName = true,
            //    ShowStartNr = true,
            //    ShowVinnarOdds = true,
            //    ShowPrio = true,
            //    ShowStakeDistributionPercent = true,
            //    ShowMarksPercent = true
            //};

            //this.DataToShowTvilling = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.Combination,
            //    EnableConfiguration = true,
            //    ShowDriver = true,
            //    ShowHorsePopup = true,
            //    ShowName = true,
            //    ShowPlatsOdds = true,
            //    ShowStartNr = true,
            //    ShowVinnarOdds = true
            //};

            //this.DataToShowTrio = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.Trio & DataToShowUsage.Combination,
            //    EnableConfiguration = true,
            //    ShowDriver = true,
            //    ShowHorsePopup = true,
            //    ShowName = true,
            //    ShowPlatsOdds = true,
            //    ShowStartNr = true,
            //    ShowVinnarOdds = true,
            //    ShowTrio = true
            //};

            //this.DataToShowDD = new HPTHorseDataToShow()
            //{
            //    Usage = DataToShowUsage.Combination,
            //    EnableConfiguration = true,
            //    ShowDriver = true,
            //    ShowHorsePopup = true,
            //    ShowName = true,
            //    ShowPlatsOdds = true,
            //    ShowStartNr = true,
            //    ShowTrainer = true,
            //    ShowVinnarOdds = true
            //};

            CombinationDataToShowDouble = new HPTCombinationDataToShow()
            {
                ShowCombinationOdds = true,
                //ShowCombinationOddsRank = true,
                //ShowMultipliedOdds = true,
                //ShowMultipliedOddsRank = true,
                //ShowOddsQuota = true,
                //ShowPlayability = true,
                ShowSelected = true,
                ShowStartNr1 = true,
                ShowStartNr2 = true,
                ShowStake = true,
                ShowProfit = true,
                //ShowVPQuota = true,
                ShowVQuota = true,
                //ShowPQuota = true,
                ShowStakeQuota = true,
                //ShowTVQuota = true,
                Usage = DataToShowUsage.Double
            };

            CombinationDataToShowTvilling = new HPTCombinationDataToShow()
            {
                ShowCombinationOdds = true,
                //ShowCombinationOddsRank = true,
                //ShowMultipliedOdds = true,
                //ShowMultipliedOddsRank = true,
                //ShowOddsQuota = true,
                //ShowPlayability = true,
                ShowSelected = true,
                ShowStartNr1 = true,
                ShowStartNr2 = true,
                //ShowStartNr3 = true,
                ShowStake = true,
                ShowProfit = true,
                ShowVPQuota = true,
                ShowVQuota = true,
                ShowPQuota = true,
                //ShowStakeQuota = true,
                //ShowDQuota = true,
                Usage = DataToShowUsage.Tvilling
            };

            CombinationDataToShowTrio = new HPTCombinationDataToShow()
            {
                ShowCombinationOdds = true,
                //ShowCombinationOddsRank = true,
                //ShowPlayability = true,
                ShowSelected = true,
                ShowStartNr1 = true,
                ShowStartNr2 = true,
                ShowStartNr3 = true,
                ShowStake = true,
                ShowProfit = true,
                ShowVPQuota = true,
                ShowVQuota = true,
                ShowPQuota = true,
                //ShowStakeQuota = true,
                //ShowDQuota = true,
                Usage = DataToShowUsage.Trio
            };

            #endregion

            #region Color intervals

            Brush bGreen = CreateBrush(Colors.LightGreen);
            Brush bYellow = CreateBrush(Colors.LightYellow);
            Brush bRed = CreateBrush(Colors.LightCoral);

            ColorIntervalDoubleOdds = new HPTColorInterval()
            {
                LowerBoundary = 100M,
                UpperBoundary = 500M
            };

            ColorIntervalMarkability = new HPTColorInterval()
            {
                LowerBoundary = 0.5M,
                UpperBoundary = 1.0M
            };

            ColorIntervalPlayability = new HPTColorInterval()
            {
                LowerBoundary = 1.0M,
                UpperBoundary = 2.0M
            };

            ColorIntervalTvillingOdds = new HPTColorInterval()
            {
                LowerBoundary = 60M,
                UpperBoundary = 300M
            };

            ColorIntervalVinnarOdds = new HPTColorInterval()
            {
                LowerBoundary = 600M,
                UpperBoundary = 1500M
            };

            ColorIntervalStakePercent = new HPTColorInterval()
            {
                LowerBoundary = 10M,
                UpperBoundary = 30M
            };

            #endregion

            BetTypesToShow = new HPTBetTypesToShow();
            BetTypesToShow.ShowDD = true;
            BetTypesToShow.ShowDouble = true;
            BetTypesToShow.ShowLD = true;
            BetTypesToShow.ShowTrio = true;
            BetTypesToShow.ShowTvilling = true;
            BetTypesToShow.ShowV3 = true;
            BetTypesToShow.ShowV4 = true;
            BetTypesToShow.ShowV5 = true;
            BetTypesToShow.ShowV64 = true;
            BetTypesToShow.ShowV65 = true;
            BetTypesToShow.ShowV75 = true;
            BetTypesToShow.ShowV86 = true;
            BetTypesToShow.ShowVx = true;
            BetTypesToShow.ShowVxx = true;

            SetDefaultColorIntervals();

            // Vilka ABC etc. ska synas
            UseA = true;
            UseB = true;
            UseC = true;
            UseD = true;
            UseE = false;
            UseF = false;

            // Övriga inställningar
            CopyCouponsToClipboard = true;
            CopySingleRowsToClipboard = true;
            ThreadedRecalculation = true;
            UseDefaultRankTemplate = true;
        }

        private void SetDefaultColorIntervals()
        {
            SetColorFromVinnarOdds = true;

            ColorGood = Colors.LightGreen;
            ColorMedium = Colors.LightYellow;
            ColorBad = Colors.LightCoral;

            ColorIntervalDoubleOdds = new HPTColorInterval()
            {
                HighColor = ColorBad,
                MediumColor = ColorMedium,
                LowColor = ColorGood,
                LowerBoundary = 50M,
                UpperBoundary = 100M
            };

            ColorIntervalMarkability = new HPTColorInterval()
            {
                HighColor = ColorBad,
                MediumColor = ColorMedium,
                LowColor = ColorGood,
                LowerBoundary = 0.9M,
                UpperBoundary = 1.2M
            };

            ColorIntervalPlayability = new HPTColorInterval()
            {
                HighColor = ColorBad,
                MediumColor = ColorMedium,
                LowColor = ColorGood,
                LowerBoundary = 0.9M,
                UpperBoundary = 1.2M
            };

            ColorIntervalTvillingOdds = new HPTColorInterval()
            {
                HighColor = ColorBad,
                MediumColor = ColorMedium,
                LowColor = ColorGood,
                LowerBoundary = 50M,
                UpperBoundary = 100M
            };

            ColorIntervalVinnarOdds = new HPTColorInterval()
            {
                HighColor = ColorBad,
                MediumColor = ColorMedium,
                LowColor = ColorGood,
                LowerBoundary = 600M,
                UpperBoundary = 1500M
            };

            ColorIntervalStakePercent = new HPTColorInterval()
            {
                HighColor = ColorBad,
                MediumColor = ColorMedium,
                LowColor = ColorGood,
                LowerBoundary = 10M,
                UpperBoundary = 30M
            };
        }

        internal void SetRankTemplates()
        {
            if (MarkBetTemplateABCDList == null)
            {
                MarkBetTemplateABCDList = new ObservableCollection<HPTMarkBetTemplateABCD>();
            }
            foreach (var markBetTemplateAbcd in MarkBetTemplateABCDList)
            {
                if (!string.IsNullOrEmpty(markBetTemplateAbcd.RankTemplateName))
                {
                    markBetTemplateAbcd.RankTemplate = RankTemplateList.FirstOrDefault(rt => rt.Name == markBetTemplateAbcd.RankTemplateName);
                }
            }

            if (MarkBetTemplateRankList == null)
            {
                MarkBetTemplateRankList = new ObservableCollection<HPTMarkBetTemplateRank>();
            }
            foreach (var markBetTemplateRank in MarkBetTemplateRankList)
            {
                if (!string.IsNullOrEmpty(markBetTemplateRank.RankTemplateName))
                {
                    markBetTemplateRank.RankTemplate = RankTemplateList.FirstOrDefault(rt => rt.Name == markBetTemplateRank.RankTemplateName);
                }
            }

        }

        internal void HandleRankTemplates()
        {
            if (FirstTimeHPT5User)
            {
                // Obsolet rankvariabelmall
                var rankTemplate = RankTemplateList.FirstOrDefault(rt => rt.Name == "Standard");
                if (rankTemplate != null)
                {
                    RankTemplateList.Remove(rankTemplate);
                }
            }
            if (RankTemplateList.Count == 0)
            {
                rankTemplateList = new ObservableCollection<HPTRankTemplate>();
                rankTemplateList.Add(DefaultRankTemplate);
                rankTemplateList.Add(DefaultRankTemplateTvilling);
                rankTemplateList.Add(DefaultRankTemplateTrio);
                rankTemplateList.Add(DefaultRankTemplateDouble);
            }
        }

        internal HPTRankTemplate CreateDefaultRankTemplate()
        {
            var rankTemplate = new HPTRankTemplate()
            {
                HorseRankVariableList = HPTHorseRankVariable.CreateVariableList(),
                Name = "Standard",
                IsDefault = true
            };

            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "VinnarOdds").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "StakeDistributionShare").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "EarningsMeanLast5").Use = true;

            return rankTemplate;
        }

        internal HPTRankTemplate CreateDefaultRankTemplateDouble()
        {
            var rankTemplate = new HPTRankTemplate()
            {
                HorseRankVariableList = HPTHorseRankVariable.CreateVariableList(),
                Name = "Standard DD/LD",
                IsDefaultDouble = true
            };

            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "VinnarOdds").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "MaxPlatsOdds").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "EarningsMeanLast5").Use = true;

            return rankTemplate;
        }

        internal HPTRankTemplate CreateDefaultRankTemplateTvilling()
        {
            var rankTemplate = new HPTRankTemplate()
            {
                HorseRankVariableList = HPTHorseRankVariable.CreateVariableList(),
                Name = "Standard Tvilling",
                IsDefaultTvilling = true
            };

            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "VinnarOdds").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "MaxPlatsOdds").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "EarningsMeanLast5").Use = true;

            return rankTemplate;
        }

        internal HPTRankTemplate CreateDefaultRankTemplateTrio()
        {
            var rankTemplate = new HPTRankTemplate()
            {
                HorseRankVariableList = HPTHorseRankVariable.CreateVariableList(),
                Name = "Standard Trio",
                IsDefaultTrio = true
            };

            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "VinnarOdds").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "MaxPlatsOdds").Use = true;
            rankTemplate.HorseRankVariableList.First(hrv => hrv.PropertyName == "EarningsMeanLast5").Use = true;

            return rankTemplate;
        }

        //internal void RemoveDefaultTemplates()
        //{
        //    if (this.MarkBetTemplateABCDList != null)
        //    {
        //        this.MarkBetTemplateABCDList = new ObservableCollection<HPTMarkBetTemplateABCD>(this.MarkBetTemplateABCDList.Where(mbt => !mbt.IsDefault));
        //        foreach (HPTMarkBetTemplateABCD template in MarkBetTemplateABCDList)
        //        {
        //            if (template.RankTemplate != null)
        //            {
        //                template.RankTemplateName = template.RankTemplate.Name;
        //            }
        //        }
        //    }
        //    if (this.MarkBetTemplateRankList != null)
        //    {
        //        this.MarkBetTemplateRankList = new ObservableCollection<HPTMarkBetTemplateRank>(this.MarkBetTemplateRankList.Where(mbt => !mbt.IsDefault));
        //        foreach (HPTMarkBetTemplateRank template in MarkBetTemplateRankList)
        //        {
        //            if (template.RankTemplate != null)
        //            {
        //                template.RankTemplateName = template.RankTemplate.Name;
        //            }
        //        }
        //    }
        //}

        //internal void CreateDefaultTemplates()
        //{
        //    HPTRankTemplate rankTemplate = null;
        //    if (this.RankTemplateList == null || this.RankTemplateList.Count == 0)
        //    {
        //        rankTemplate = CreateDefaultRankTemplate();
        //        this.DefaultRankTemplate = rankTemplate;
        //        this.RankTemplateList = new ObservableCollection<HPTRankTemplate>() { this.DefaultRankTemplate };
        //    }
        //    else
        //    {
        //        rankTemplate = this.RankTemplateList.FirstOrDefault(rt => rt.Name == "Standard");
        //        if (rankTemplate == null)
        //        {
        //            rankTemplate = CreateDefaultRankTemplate();
        //            this.RankTemplateList.Add(rankTemplate);
        //        }
        //    }

        //    if (this.MarkBetTemplateABCDList == null)
        //    {
        //        this.MarkBetTemplateABCDList = new ObservableCollection<HPTMarkBetTemplateABCD>();
        //        this.MarkBetTemplateRankList = new ObservableCollection<HPTMarkBetTemplateRank>();
        //    }
        //    else
        //    {
        //        RemoveDefaultTemplates();
        //    }

        //    foreach (var markBetTemplateAbcd in this.MarkBetTemplateABCDList)
        //    {
        //        markBetTemplateAbcd.RankTemplate = this.RankTemplateList.FirstOrDefault(rt => rt.Name == markBetTemplateAbcd.RankTemplateName);
        //    }

        //    foreach (var markBetTemplateRank in this.MarkBetTemplateRankList)
        //    {
        //        markBetTemplateRank.RankTemplate = this.RankTemplateList.FirstOrDefault(rt => rt.Name == markBetTemplateRank.RankTemplateName);
        //    }

        //    HPTPrio[] priosToUseABC = new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C };
        //    HPTPrio[] priosToUseABCD = new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C, HPTPrio.D };

        //    #region Default ABCD templates

        //    var mbtV41 = new HPTMarkBetTemplateABCD()
        //        {
        //            Name = "Litet ABC-system (V4)",
        //            RankTemplate = rankTemplate,
        //            TypeCategory = BetTypeCategory.V4,
        //            DesiredSystemSize = 25,
        //            NumberOfSpikes = 1,
        //            ReductionPercentage = 50,
        //            Use = true,
        //            IsDefault = true
        //        };
        //    mbtV41.InitializeTemplate(priosToUseABC);

        //    var mbtV42 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Stort ABC-system (V4)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V4,
        //        DesiredSystemSize = 200,
        //        NumberOfSpikes = 0,
        //        ReductionPercentage = 70,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV42.InitializeTemplate(priosToUseABC);

        //    var mbtV51 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Litet ABC-system (V5)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V5,
        //        DesiredSystemSize = 50,
        //        NumberOfSpikes = 1,
        //        ReductionPercentage = 55,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV51.InitializeTemplate(priosToUseABC);

        //    var mbtV52 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Stort ABC-system (V5)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V5,
        //        DesiredSystemSize = 400,
        //        NumberOfSpikes = 0,
        //        ReductionPercentage = 75,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV52.InitializeTemplate(priosToUseABC);

        //    var mbtV61 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Litet ABC-system (V6X)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V6X,
        //        DesiredSystemSize = 200,
        //        NumberOfSpikes = 2,
        //        ReductionPercentage = 60,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV61.InitializeTemplate(priosToUseABC);

        //    var mbtV62 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Stort ABCD-system (V6X)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V6X,
        //        DesiredSystemSize = 800,
        //        NumberOfSpikes = 1,
        //        ReductionPercentage = 75,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV62.InitializeTemplate(priosToUseABCD);

        //    var mbtV71 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Litet ABC-system (V75)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V75,
        //        DesiredSystemSize = 500,
        //        NumberOfSpikes = 2,
        //        ReductionPercentage = 65,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV71.InitializeTemplate(priosToUseABC);

        //    var mbtV72 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Stort ABCD-system (V75)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V75,
        //        DesiredSystemSize = 2000,
        //        NumberOfSpikes = 1,
        //        ReductionPercentage = 75,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV72.InitializeTemplate(priosToUseABCD);

        //    var mbtV81 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Litet ABC-system (V86)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V86,
        //        DesiredSystemSize = 1000,
        //        NumberOfSpikes = 3,
        //        ReductionPercentage = 75,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV81.InitializeTemplate(priosToUseABC);

        //    var mbtV82 = new HPTMarkBetTemplateABCD()
        //    {
        //        Name = "Stort ABC-system (V86)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V86,
        //        DesiredSystemSize = 4000,
        //        NumberOfSpikes = 1,
        //        ReductionPercentage = 80,
        //        Use = true,
        //        IsDefault = true
        //    };
        //    mbtV82.InitializeTemplate(priosToUseABCD);

        //    this.MarkBetTemplateABCDList.Add(mbtV41);
        //    this.MarkBetTemplateABCDList.Add(mbtV42);
        //    this.MarkBetTemplateABCDList.Add(mbtV51);
        //    this.MarkBetTemplateABCDList.Add(mbtV52);
        //    this.MarkBetTemplateABCDList.Add(mbtV61);
        //    this.MarkBetTemplateABCDList.Add(mbtV62);
        //    this.MarkBetTemplateABCDList.Add(mbtV71);
        //    this.MarkBetTemplateABCDList.Add(mbtV72);
        //    this.MarkBetTemplateABCDList.Add(mbtV81);
        //    this.MarkBetTemplateABCDList.Add(mbtV82);

        //    #endregion

        //    #region Default rank templates            

        //    var mbtV4 = new HPTMarkBetTemplateRank()
        //    {
        //        Name = "Standard ranksystem (V4)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V4,
        //        DesiredSystemSize = 50,
        //        NumberOfSpikes = 0,
        //        LowerPercentageLimit = 25,
        //        UpperPercentageLimit = 50,
        //        Use = true,
        //        IsDefault = true
        //    };

        //    var mbtV5 = new HPTMarkBetTemplateRank()
        //    {
        //        Name = "Standard ranksystem (V5)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V5,
        //        DesiredSystemSize = 200,
        //        NumberOfSpikes = 1,
        //        LowerPercentageLimit = 25,
        //        UpperPercentageLimit = 50,
        //        Use = true,
        //        IsDefault = true
        //    };

        //    var mbtV6X = new HPTMarkBetTemplateRank()
        //    {
        //        Name = "Standard ranksystem (V6X)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V6X,
        //        DesiredSystemSize = 500,
        //        NumberOfSpikes = 1,
        //        LowerPercentageLimit = 25,
        //        UpperPercentageLimit = 50,
        //        Use = true,
        //        IsDefault = true
        //    };

        //    var mbtV75 = new HPTMarkBetTemplateRank()
        //    {
        //        Name = "Standard ranksystem (V75)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V75,
        //        DesiredSystemSize = 1000,
        //        NumberOfSpikes = 2,
        //        LowerPercentageLimit = 25,
        //        UpperPercentageLimit = 50,
        //        Use = true,
        //        IsDefault = true
        //    };

        //    var mbtV86 = new HPTMarkBetTemplateRank()
        //    {
        //        Name = "Standard ranksystem (V86)",
        //        RankTemplate = rankTemplate,
        //        TypeCategory = BetTypeCategory.V86,
        //        DesiredSystemSize = 2000,
        //        NumberOfSpikes = 1,
        //        LowerPercentageLimit = 25,
        //        UpperPercentageLimit = 50,
        //        Use = true,
        //        IsDefault = true
        //    };

        //    this.MarkBetTemplateRankList.Add(mbtV4);
        //    this.MarkBetTemplateRankList.Add(mbtV5);
        //    this.MarkBetTemplateRankList.Add(mbtV6X);
        //    this.MarkBetTemplateRankList.Add(mbtV75);
        //    this.MarkBetTemplateRankList.Add(mbtV86);

        //    #endregion
        //}

        #endregion

        private static Brush CreateBrush(Color c)
        {
            SolidColorBrush scb = new SolidColorBrush(c);
            return scb;
        }

        public static string MyDocumentsPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\HPT Travsystem\\";
            }
        }

        public static string TempPath
        {
            get
            {
                return MyDocumentsPath + "Temp\\";
            }
        }

        internal static string PaysonURL = "https://www.payson.se/SendMoney/?De=Ett+%e5rs+%27Hj%e4lp+p%e5+traven+PRO%27&amp;Se=hjalp.pa.traven%40gmail.com&amp;Cost=299%2c00&amp;Currency=SEK&amp;Sp=1";
        internal static string PaysonURLThreeMonths = "https://www.payson.se/SendMoney/?De=Tre+m%e5naders+%27Hj%e4lp+p%e5+traven+PRO%27&amp;Se=hjalp.pa.traven%40gmail.com&amp;Cost=99%2c00&amp;Currency=SEK&amp;Sp=1";

        #region Gratis/PRO

        private bool isPayingCustomer;
        [XmlIgnore]
        internal bool IsPayingCustomer
        {
            get
            {
                //return this.isPayingCustomer;
                return true;
            }
            set
            {
                if (!value)
                {
                    //// Flikar som bara finns i PRO-versionen
                    //this.MarkBetTabsToShow.ShowAdvanced = false;
                    //this.MarkBetTabsToShow.ShowComplimentaryRules = false;
                    //this.MarkBetTabsToShow.ShowDriverReduction = false;
                    //this.MarkBetTabsToShow.ShowSingleRows = false;
                    //this.MarkBetTabsToShow.ShowRankOverview = false;
                    //this.MarkBetTabsToShow.ShowTrainerReduction = false;
                    //this.MarkBetTabsToShow.ShowGroupIntervalReduction = false;
                    //this.MarkBetTabsToShow.ShowCompanyGambling = false;
                    //this.MarkBetTabsToShow.ShowV6BetMultiplier = false;
                    //this.MarkBetTabsToShow.ShowMultiABCD = false;

                    //// DEF ska bara finnas i PRO-versionen
                    //this.UseD = false;
                    //this.UseE = false;
                    //this.UseF = false;

                    //this.AlwaysCreateSingleRows = false;
                    //this.AlwaysLoadDD = false;
                    //this.AlwaysLoadLD = false;
                    //this.AlwaysLoadMainEvent = false;
                    //this.AlwaysLoadV4 = false;
                    //this.CopyCouponsToClipboard = false;
                    //this.CopySingleRowsToClipboard = false;
                    //this.DefaultUpdateInterval = 0;
                    //this.ThreadedRecalculation = false;
                    //this.UseDefaultRankTemplate = false;
                    //this.WarnIfNoReserv = false;                    
                }
                isPayingCustomer = value;
                OnPropertyChanged("IsPayingCustomer");
            }
        }

        internal void SetColumnsForFreeloaders(HPTDataToShow dataToShow)
        {
            var hdtsAttributes = dataToShow.GetHorseDataToShowAttributes();
            var dtsType = dataToShow.GetType();
            foreach (var attr in hdtsAttributes)
            {
                if (attr.RequiresPro)
                {
                    dtsType.GetProperty(attr.PropertyName).SetValue(dataToShow, false, null);
                }
            }
        }

        internal void SetDefaultsForPayingCustomer()
        {
            if (IsPayingCustomer)
            {
                //// Prio-grupper som är default för betalande kunder
                //this.UseA = true;
                //this.UseB = true;
                //this.UseC = true;
                //this.UseD = true;

                //// Flikar som bara finns i PRO-versionen
                //this.MarkBetTabsToShow.ShowAdvanced = true;
                //this.MarkBetTabsToShow.ShowComplimentaryRules = true;
                //this.MarkBetTabsToShow.ShowDriverReduction = true;
                //this.MarkBetTabsToShow.ShowSingleRows = true;
                //this.MarkBetTabsToShow.ShowRankOverview = true;
                //this.MarkBetTabsToShow.ShowTrainerReduction = true;
                //this.MarkBetTabsToShow.ShowGroupIntervalReduction = true;
                //this.MarkBetTabsToShow.ShowCompanyGambling = true;
                //this.MarkBetTabsToShow.ShowV6BetMultiplier = true;
                //this.MarkBetTabsToShow.ShowMultiABCD = true;

                // Rankvariabelmall
                if (RankTemplateList != null && RankTemplateList.Count > 0)
                {
                    UseDefaultRankTemplate = true;
                    DefaultRankTemplate = RankTemplateList[0];
                }
            }
        }

        //private DateTime proVersionExpirationDate;
        //public DateTime PROVersionExpirationDate
        //{
        //    get
        //    {
        //        return this.proVersionExpirationDate;
        //    }
        //    set
        //    {
        //        this.proVersionExpirationDate = value;
        //        OnPropertyChanged("PROVersionExpirationDate");
        //    }
        //}

        [DataMember]
        public DateTime PROVersionExpirationDate { get; set; }

        [DataMember]
        public string LastIPAddress { get; set; }

        private bool isEligibleForPro;
        public bool IsEligibleForPro
        {
            get
            {
                return isEligibleForPro;
            }
            set
            {
                isEligibleForPro = value;
                OnPropertyChanged("IsEligibleForPro");
            }
        }

        #endregion

        #region Configuration properties

        public bool FirstTimeHPT5User { get; set; }

        private string versionText;
        [XmlIgnore]
        public string VersionText
        {
            get
            {
                return versionText;
            }
            set
            {
                versionText = value;
                OnPropertyChanged("VersionText");
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTMarkBetTemplateABCD> MarkBetTemplateABCDList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTMarkBetTemplateRank> MarkBetTemplateRankList { get; set; }

        // KOMMANDE
        private ObservableCollection<HPTHorseRankSumReductionRuleCollection> rankSumReductionRuleCollection;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTHorseRankSumReductionRuleCollection> RankSumReductionRuleCollection
        {
            get
            {
                if (rankSumReductionRuleCollection == null)
                {
                    rankSumReductionRuleCollection = new ObservableCollection<HPTHorseRankSumReductionRuleCollection>();
                }
                return rankSumReductionRuleCollection;
            }
            set
            {
                rankSumReductionRuleCollection = value;
            }
        }

        private BetTypeCategory[] betTypeCategoryList;
        [DataMember]
        public BetTypeCategory[] BetTypeCategoryList
        {
            get
            {
                if (betTypeCategoryList == null)
                {
                    betTypeCategoryList = new BetTypeCategory[] { BetTypeCategory.V4, BetTypeCategory.V5, BetTypeCategory.V6X, BetTypeCategory.V75, BetTypeCategory.V75, BetTypeCategory.V86, BetTypeCategory.V85 };
                }
                return betTypeCategoryList;
            }
        }

        private decimal zoom;
        [DataMember]
        public decimal Zoom
        {
            get
            {
                if (zoom == 0M)
                {
                    zoom = 1M;
                }
                return zoom;
            }
            set
            {
                zoom = value;
                OnPropertyChanged("Zoom");
            }
        }

        #region Varningar

        public void SetDefaultValuesForWarnings(GUIProfile profile)
        {
            switch (profile)
            {
                case GUIProfile.Simple:
                    WarnIfNoReserv = true;
                    WarnIfOverlappingComplementaryRules = true;
                    WarnIfSuperfluousXReduction = true;
                    //this.WarnIfUncoveredHorses = true;
                    break;
                case GUIProfile.Normal:
                    WarnIfNoReserv = false;
                    WarnIfOverlappingComplementaryRules = false;
                    WarnIfSuperfluousXReduction = true;
                    //this.WarnIfUncoveredHorses = true;
                    break;
                case GUIProfile.Advanced:
                    WarnIfNoReserv = false;
                    WarnIfOverlappingComplementaryRules = false;
                    WarnIfSuperfluousXReduction = false;
                    //this.WarnIfUncoveredHorses = false;
                    break;
                case GUIProfile.Custom:
                    break;
                default:
                    break;
            }
        }

        [DataMember]
        public bool WarnIfNoReserv { get; set; }

        [DataMember]
        public bool WarnIfUncoveredHorses
        {
            get
            {
                return true;
            }
            set
            {
                bool temp = value;
            }
        }

        [DataMember]
        public bool WarnIfSuperfluousXReduction { get; set; }

        [DataMember]
        public bool WarnIfOverlappingComplementaryRules { get; set; }

        #endregion

        [DataMember]
        public ReservHandling DefaultReservHandling { get; set; }

        //[DataMember]
        //public int DefaultUpdateInterval { get; set; }

        [DataMember]
        public double ApplicationWidth { get; set; }

        [DataMember]
        public double ApplicationHeight { get; set; }

        [DataMember]
        public bool ThreadedRecalculation { get; set; }

        [DataMember]
        public bool AlwaysCreateSingleRows { get; set; }

        [DataMember]
        public bool AlwaysLoadMainEvent { get; set; }

        [DataMember]
        public bool AlwaysLoadV4 { get; set; }

        [DataMember]
        public bool AlwaysLoadLD { get; set; }

        [DataMember]
        public bool AlwaysLoadDD { get; set; }

        [DataMember]
        public int DefaultUpdateInterval { get; set; }

        [DataMember]
        public bool CopyCouponsToClipboard { get; set; }

        [DataMember]
        public bool CopySingleRowsToClipboard { get; set; }

        [DataMember]
        public CouponCompression DefaultCouponCompression { get; set; }

        public bool CouponCompression { get; set; }

        [DataMember]
        public WindowState ApplicationWindowState { get; set; }

        [DataMember]
        public WindowStartupLocation ApplicationStartupLocation { get; set; }

        #endregion

        #region Färgintervall

        [DataMember]
        public HPTColorInterval ColorIntervalVinnarOdds { get; set; }

        [DataMember]
        public HPTColorInterval ColorIntervalMarksPercent { get; set; }

        [DataMember]
        public HPTColorInterval ColorIntervalMarkability { get; set; }

        [DataMember]
        public HPTColorInterval ColorIntervalStakePercent { get; set; }

        [DataMember]
        public HPTColorInterval ColorIntervalPlayability { get; set; }

        [DataMember]
        public HPTColorInterval ColorIntervalDoubleOdds { get; set; }

        [DataMember]
        public HPTColorInterval ColorIntervalTvillingOdds { get; set; }

        #endregion

        #region Kolumner som ska visas

        internal void ResetDataToShow()
        {
            var dataToShowVxxDefault = CreateDataToShow(DataToShowUsage.Vxx, GUIProfile.Simple);
            var dataToShowVxx = DataToShowVxxList.First(hdts => hdts.GUIProfile == GUIProfile.Simple);
            //ResetDataToShow(
        }

        //internal void ResetDataToShow()
        //{
        //    var dataToShowVxx = CreateDataToShow(DataToShowUsage.Vxx, GUIProfile.Simple);
        //}

        internal void ResetDataToShow(HPTHorseDataToShow horseDataToShow, HPTHorseDataToShow horseDataToShowClone)
        {
            foreach (PropertyInfo pi in (horseDataToShow.GetType()).GetProperties())
            {
                if (pi.PropertyType == typeof(bool))
                {
                    pi.SetValue(horseDataToShow, pi.GetValue(horseDataToShowClone, null), null);
                }
            }
        }

        internal void HandleDataToShow()
        {
            HandleDataToShowVxx();
            HandleDataToShowComplementaryRules();
            HandleDataToShowCorrection();
            HandleDataToShowDD();
            //HandleDataToShowPersonReduction();
            HandleDataToShowTrio();
            HandleDataToShowTvilling();

            DataToShowDriverPopup = new HPTHorseDataToShow()
            {
                Usage = DataToShowUsage.None,
                ShowTrainer = true,
                ShowHorsePopup = true,
                ShowName = true,
                ShowStartNr = true,
                ShowVinnarOdds = true,
                ShowPrio = true,
                ShowStakeDistributionPercent = true,
                ShowMarksPercent = true
            };

            DataToShowTrainerPopup = new HPTHorseDataToShow()
            {
                Usage = DataToShowUsage.None,
                ShowDriver = true,
                ShowHorsePopup = true,
                ShowName = true,
                ShowStartNr = true,
                ShowVinnarOdds = true,
                ShowPrio = true,
                ShowStakeDistributionPercent = true,
                ShowMarksPercent = true
            };

        }

        internal static HPTHorseDataToShow CreateDataToShow(DataToShowUsage usage, GUIProfile profile)
        {
            var dataToShow = new HPTHorseDataToShow()
            {
                Usage = usage,
                GUIProfile = profile,
                EnableConfiguration = true,
                ShowName = true,
                ShowStartNr = true,
                ShowVinnarOdds = true,
                ShowStakeDistributionPercent = true
            };

            switch (usage)
            {
                //case DataToShowUsage.All:
                //case DataToShowUsage.Everywhere:
                //    break;
                case DataToShowUsage.Vxx:
                    //dataToShow.ShowMarksPercent = true;
                    //dataToShow.ShowStakeDistributionPercent = true;
                    dataToShow.ShowPrio = true;
                    dataToShow.ShowSystemCoverage = true;
                    dataToShow.ShowDriver = true;
                    if (profile == GUIProfile.Normal || profile == GUIProfile.Advanced)
                    {
                        dataToShow.ShowComments = true;
                        dataToShow.ShowDoubleShare = true;
                        dataToShow.ShowHeadToHead = true;
                        dataToShow.ShowLocked = true;
                        dataToShow.ShowMarksShare = true;
                        dataToShow.ShowOwnInformation = true;
                        dataToShow.ShowReserv = true;
                        dataToShow.ShowPlatsOdds = true;
                        dataToShow.ShowPlatsShare = true;
                        dataToShow.ShowRankATG = true;
                        dataToShow.ShowRankMean = true;
                        dataToShow.ShowRankOwn = true;
                        dataToShow.ShowStakeShare = true;
                        dataToShow.ShowTrainer = true;
                        dataToShow.ShowTrioShare = true;
                        dataToShow.ShowTvillingShare = true;
                        dataToShow.ShowVinnarOddsShare = true;
                        if (profile == GUIProfile.Advanced)
                        {
                            dataToShow.ShowAge = true;
                            dataToShow.ShowBreeder = true;
                            dataToShow.ShowDaysSinceLastStart = true;
                            dataToShow.ShowDistance = true;
                            dataToShow.ShowEarnings = true;
                            dataToShow.ShowEarningsMeanLast5 = true;
                            dataToShow.ShowLastStartDate = true;
                            dataToShow.ShowMarkability = true;
                            dataToShow.ShowMarksQuantity = true;
                            dataToShow.ShowOwner = true;
                            dataToShow.ShowRankTip = true;
                            dataToShow.ShowRecord = true;
                            dataToShow.ShowResultRow = true;
                            dataToShow.ShowShoeInfo = true;
                            dataToShow.ShowSulkyInfo = true;
                            dataToShow.ShowTrack = true;
                            dataToShow.ShowVinnarOddsRelative = true;
                        }
                    }
                    break;
                //case DataToShowUsage.Combination:
                //    break;
                case DataToShowUsage.Trio:
                    dataToShow.ShowTrio = true;
                    dataToShow.ShowPlatsOdds = true;
                    dataToShow.ShowDriver = true;
                    if (profile == GUIProfile.Normal || profile == GUIProfile.Advanced)
                    {
                        dataToShow.ShowDoubleShare = true;
                        dataToShow.ShowHeadToHead = true;
                        dataToShow.ShowPlatsShare = true;
                        dataToShow.ShowRankATG = true;
                        dataToShow.ShowRankMean = true;
                        dataToShow.ShowTvillingShare = true;
                        dataToShow.ShowVinnarOddsShare = true;
                        if (profile == GUIProfile.Advanced)
                        {
                            dataToShow.ShowComments = true;
                            dataToShow.ShowOwnInformation = true;
                            dataToShow.ShowTrainer = true;
                            dataToShow.ShowAge = true;
                            dataToShow.ShowBreeder = true;
                            dataToShow.ShowDaysSinceLastStart = true;
                            dataToShow.ShowDistance = true;
                            dataToShow.ShowEarnings = true;
                            dataToShow.ShowEarningsMeanLast5 = true;
                            dataToShow.ShowLastStartDate = true;
                            dataToShow.ShowOwner = true;
                            dataToShow.ShowRecord = true;
                            dataToShow.ShowResultRow = true;
                            dataToShow.ShowSulkyInfo = true;
                            dataToShow.ShowShoeInfo = true;
                            dataToShow.ShowTrack = true;
                        }
                    }
                    break;
                case DataToShowUsage.Tvilling:
                    dataToShow.ShowPlatsOdds = true;
                    dataToShow.ShowDriver = true;
                    if (profile == GUIProfile.Normal || profile == GUIProfile.Advanced)
                    {
                        dataToShow.ShowTrioShare = true;
                        dataToShow.ShowHeadToHead = true;
                        dataToShow.ShowPlatsShare = true;
                        dataToShow.ShowRankATG = true;
                        dataToShow.ShowRankMean = true;
                        dataToShow.ShowDoubleShare = true;
                        dataToShow.ShowVinnarOddsShare = true;
                        if (profile == GUIProfile.Advanced)
                        {
                            dataToShow.ShowComments = true;
                            dataToShow.ShowOwnInformation = true;
                            dataToShow.ShowTrainer = true;
                            dataToShow.ShowAge = true;
                            dataToShow.ShowBreeder = true;
                            dataToShow.ShowDaysSinceLastStart = true;
                            dataToShow.ShowDistance = true;
                            dataToShow.ShowEarnings = true;
                            dataToShow.ShowEarningsMeanLast5 = true;
                            dataToShow.ShowLastStartDate = true;
                            dataToShow.ShowOwner = true;
                            dataToShow.ShowRecord = true;
                            dataToShow.ShowSulkyInfo = true;
                            dataToShow.ShowResultRow = true;
                            dataToShow.ShowShoeInfo = true;
                            dataToShow.ShowTrack = true;
                        }
                    }
                    break;
                case DataToShowUsage.Double:
                    dataToShow.ShowStakeDistributionPercent = true;
                    dataToShow.ShowPrio = true;
                    dataToShow.ShowDriver = true;
                    if (profile == GUIProfile.Normal || profile == GUIProfile.Advanced)
                    {
                        dataToShow.ShowComments = true;
                        dataToShow.ShowHeadToHead = true;
                        dataToShow.ShowOwnInformation = true;
                        dataToShow.ShowPlatsOdds = true;
                        dataToShow.ShowPlatsShare = true;
                        dataToShow.ShowRankATG = true;
                        dataToShow.ShowRankMean = true;
                        dataToShow.ShowStakeShare = true;
                        dataToShow.ShowTrainer = true;
                        dataToShow.ShowTrioShare = true;
                        dataToShow.ShowTvillingShare = true;
                        dataToShow.ShowVinnarOddsShare = true;
                        if (profile == GUIProfile.Advanced)
                        {
                            dataToShow.ShowAge = true;
                            dataToShow.ShowBreeder = true;
                            dataToShow.ShowDaysSinceLastStart = true;
                            dataToShow.ShowDistance = true;
                            dataToShow.ShowEarnings = true;
                            dataToShow.ShowEarningsMeanLast5 = true;
                            dataToShow.ShowLastStartDate = true;
                            dataToShow.ShowMarkability = true;
                            dataToShow.ShowMarksQuantity = true;
                            dataToShow.ShowOwner = true;
                            dataToShow.ShowRecord = true;
                            dataToShow.ShowResultRow = true;
                            dataToShow.ShowShoeInfo = true;
                            dataToShow.ShowSulkyInfo = true;
                            dataToShow.ShowTrack = true;
                            dataToShow.ShowVinnarOddsRelative = true;
                        }
                    }
                    break;
                case DataToShowUsage.ComplementaryRule:
                    dataToShow.ShowLegNrText = true;
                    dataToShow.ShowComplimentaryRuleSelect = true;
                    dataToShow.ShowMarksPercent = true;
                    dataToShow.ShowStakeDistributionPercent = true;
                    dataToShow.ShowPrio = true;
                    dataToShow.ShowSystemCoverage = true;
                    dataToShow.ShowDriver = true;
                    if (profile == GUIProfile.Normal || profile == GUIProfile.Advanced)
                    {
                        dataToShow.ShowComments = true;
                        dataToShow.ShowDoubleShare = true;
                        dataToShow.ShowHeadToHead = true;
                        dataToShow.ShowLocked = true;
                        dataToShow.ShowMarksShare = true;
                        dataToShow.ShowOwnInformation = true;
                        dataToShow.ShowReserv = true;
                        dataToShow.ShowPlatsOdds = true;
                        dataToShow.ShowPlatsShare = true;
                        dataToShow.ShowRankATG = true;
                        dataToShow.ShowRankMean = true;
                        dataToShow.ShowRankOwn = true;
                        dataToShow.ShowStakeShare = true;
                        dataToShow.ShowTrainer = true;
                        dataToShow.ShowTrioShare = true;
                        dataToShow.ShowTvillingShare = true;
                        dataToShow.ShowVinnarOddsShare = true;
                        if (profile == GUIProfile.Advanced)
                        {
                            dataToShow.ShowAge = true;
                            dataToShow.ShowBreeder = true;
                            dataToShow.ShowDaysSinceLastStart = true;
                            dataToShow.ShowDistance = true;
                            dataToShow.ShowEarnings = true;
                            dataToShow.ShowEarningsMeanLast5 = true;
                            dataToShow.ShowLastStartDate = true;
                            dataToShow.ShowMarkability = true;
                            dataToShow.ShowMarksQuantity = true;
                            dataToShow.ShowOwner = true;
                            dataToShow.ShowRankTip = true;
                            dataToShow.ShowRecord = true;
                            dataToShow.ShowResultRow = true;
                            dataToShow.ShowShoeInfo = true;
                            dataToShow.ShowSulkyInfo = true;
                            dataToShow.ShowTrack = true;
                            dataToShow.ShowVinnarOddsRelative = true;
                        }
                    }
                    break;
                //case DataToShowUsage.HorseList:
                //    break;
                case DataToShowUsage.Correction:
                    dataToShow.ShowLegNrText = true;
                    dataToShow.ShowSystemsLeft = true;
                    dataToShow.ShowSystemValue = true;
                    dataToShow.ShowATGResultLink = true;
                    dataToShow.ShowLegNrText = true;
                    dataToShow.ShowMarksPercent = true;
                    dataToShow.ShowStakeDistributionPercent = true;
                    dataToShow.ShowPrio = true;
                    dataToShow.ShowSystemCoverage = true;
                    dataToShow.ShowDriver = true;
                    if (profile == GUIProfile.Normal || profile == GUIProfile.Advanced)
                    {
                        dataToShow.ShowComments = true;
                        dataToShow.ShowDoubleShare = true;
                        dataToShow.ShowHeadToHead = true;
                        dataToShow.ShowLocked = true;
                        dataToShow.ShowMarksShare = true;
                        dataToShow.ShowOwnInformation = true;
                        dataToShow.ShowReserv = true;
                        dataToShow.ShowPlatsOdds = true;
                        dataToShow.ShowPlatsShare = true;
                        dataToShow.ShowRankATG = true;
                        dataToShow.ShowRankMean = true;
                        dataToShow.ShowRankOwn = true;
                        dataToShow.ShowStakeShare = true;
                        dataToShow.ShowTrainer = true;
                        dataToShow.ShowTrioShare = true;
                        dataToShow.ShowTvillingShare = true;
                        dataToShow.ShowVinnarOddsShare = true;
                        if (profile == GUIProfile.Advanced)
                        {
                            dataToShow.ShowAge = true;
                            dataToShow.ShowBreeder = true;
                            dataToShow.ShowDaysSinceLastStart = true;
                            dataToShow.ShowDistance = true;
                            dataToShow.ShowEarnings = true;
                            dataToShow.ShowEarningsMeanLast5 = true;
                            dataToShow.ShowLastStartDate = true;
                            dataToShow.ShowMarkability = true;
                            dataToShow.ShowMarksQuantity = true;
                            dataToShow.ShowOwner = true;
                            dataToShow.ShowRankTip = true;
                            dataToShow.ShowRecord = true;
                            dataToShow.ShowResultRow = true;
                            dataToShow.ShowShoeInfo = true;
                            dataToShow.ShowSulkyInfo = true;
                            dataToShow.ShowTrack = true;
                            dataToShow.ShowVinnarOddsRelative = true;
                        }
                    }
                    break;
                case DataToShowUsage.None:
                    break;
                default:
                    break;
            }

            return dataToShow;
        }

        internal void SetDataToShow(HPTHorseDataToShow dataToShow)
        {
            switch (dataToShow.Usage)
            {
                //case DataToShowUsage.All:
                //case DataToShowUsage.Everywhere:
                //    break;
                case DataToShowUsage.Vxx:
                    DataToShowVxx = dataToShow;
                    ReplaceDataToShow(dataToShow, DataToShowVxxList);
                    break;
                case DataToShowUsage.Combination:
                    break;
                case DataToShowUsage.Trio:
                    DataToShowTrio = dataToShow;
                    ReplaceDataToShow(dataToShow, DataToShowTrioList);
                    break;
                case DataToShowUsage.Tvilling:
                    DataToShowTvilling = dataToShow;
                    ReplaceDataToShow(dataToShow, DataToShowTvillingList);
                    break;
                case DataToShowUsage.Double:
                    DataToShowDD = dataToShow;
                    ReplaceDataToShow(dataToShow, DataToShowDDList);
                    break;
                case DataToShowUsage.ComplementaryRule:
                    DataToShowComplementaryRules = dataToShow;
                    ReplaceDataToShow(dataToShow, DataToShowComplementaryRulesList);
                    break;
                case DataToShowUsage.HorseList:
                    break;
                case DataToShowUsage.Correction:
                    DataToShowCorrection = dataToShow;
                    ReplaceDataToShow(dataToShow, DataToShowCorrectionList);
                    break;
                case DataToShowUsage.None:
                    break;
                default:
                    break;
            }
        }

        private void ReplaceDataToShow(HPTHorseDataToShow dataToShow, List<HPTHorseDataToShow> dataToShowList)
        {
            if (dataToShowList == null)
            {
                return;
            }
            var dataToShowOld = dataToShowList.FirstOrDefault(dts => dts.GUIProfile == dataToShow.GUIProfile);
            if (dataToShowOld != null)
            {
                dataToShowList.Remove(dataToShowOld);
                foreach (var dts in dataToShowList)
                {
                    dts.IsDefault = false;
                }
                dataToShow.IsDefault = true;
                dataToShowList.Add(dataToShow);
            }
        }

        internal HPTHorseDataToShow GetDataToShow(DataToShowUsage usage, GUIProfile profile)
        {
            try
            {
                switch (usage)
                {
                    //case DataToShowUsage.All:
                    //case DataToShowUsage.Everywhere:
                    //    break;
                    case DataToShowUsage.Vxx:
                        return DataToShowVxxList.First(dts => dts.GUIProfile == profile);
                    case DataToShowUsage.Combination:
                        break;
                    case DataToShowUsage.Trio:
                        return DataToShowTrioList.First(dts => dts.GUIProfile == profile);
                    case DataToShowUsage.Tvilling:
                        return DataToShowTvillingList.First(dts => dts.GUIProfile == profile);
                    case DataToShowUsage.Double:
                        return DataToShowDDList.First(dts => dts.GUIProfile == profile);
                    case DataToShowUsage.ComplementaryRule:
                        return DataToShowComplementaryRulesList.First(dts => dts.GUIProfile == profile);
                    case DataToShowUsage.HorseList:
                        break;
                    case DataToShowUsage.Correction:
                        return DataToShowCorrectionList.First(dts => dts.GUIProfile == profile);
                    case DataToShowUsage.None:
                        break;
                    default:
                        break;
                }
            }
            catch (NullReferenceException)
            {
                return CreateDataToShow(usage, profile);
            }
            return null;
        }

        internal void HandleDataToShowVxx()
        {
            try
            {
                if (DataToShowVxxList == null)
                {
                    DataToShowVxxList = new List<HPTHorseDataToShow>();
                    if (DataToShowVxx != null)
                    {
                        DataToShowVxx.IsDefault = true;
                        DataToShowVxx.GUIProfile = GUIProfile.Normal;
                        DataToShowVxxList.Add(DataToShowVxx);
                    }
                }

                var dataToShowSimple = DataToShowVxxList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
                if (dataToShowSimple == null)
                {
                    dataToShowSimple = CreateDataToShow(DataToShowUsage.Vxx, GUIProfile.Simple);
                    DataToShowVxxList.Add(dataToShowSimple);
                }

                var dataToShowNormal = DataToShowVxxList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
                if (dataToShowNormal == null)
                {
                    dataToShowNormal = CreateDataToShow(DataToShowUsage.Vxx, GUIProfile.Normal);
                    DataToShowVxxList.Add(dataToShowNormal);
                }

                var dataToShowComplete = DataToShowVxxList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
                if (dataToShowComplete == null)
                {
                    dataToShowComplete = CreateDataToShow(DataToShowUsage.Vxx, GUIProfile.Advanced);
                    DataToShowVxxList.Add(dataToShowComplete);
                }

                if (DataToShowVxx == null)
                {
                    var dataToShowVxx = DataToShowVxxList.FirstOrDefault(dts => dts.IsDefault);
                    if (dataToShowVxx == null)
                    {
                        dataToShowSimple.IsDefault = true;
                        DataToShowVxx = dataToShowSimple;
                    }
                    else
                    {
                        DataToShowVxx = dataToShowVxx;
                    }
                }
            }
            catch (Exception exc)
            {
                AddToErrorLog(exc);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseDataToShow> DataToShowVxxList { get; set; }
        //public HPTHorseDataToShow DataToShowVxx { get; set; }
        private HPTHorseDataToShow dataToShowVxx;
        [XmlIgnore]
        public HPTHorseDataToShow DataToShowVxx
        {
            get
            {
                return dataToShowVxx;
            }
            set
            {
                dataToShowVxx = value;
                OnPropertyChanged("DataToShowVxx");

                // Uppdatera alla DataToShow när en ändras
                DataToShowVxx.PropertyChanged += DataToShowVxx_PropertyChanged;
            }
        }

        void DataToShowVxx_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            object o = DataToShowVxx.GetType().GetProperty(e.PropertyName).GetValue(DataToShowVxx);
            if (o.GetType() == typeof(bool))
            {
                DataToShowComplementaryRules.GetType().GetProperty(e.PropertyName).SetValue(DataToShowComplementaryRules, o);
                DataToShowCorrection.GetType().GetProperty(e.PropertyName).SetValue(DataToShowComplementaryRules, o);
            }
        }

        internal void HandleDataToShowComplementaryRules()
        {
            try
            {
                if (DataToShowComplementaryRulesList == null)
                {
                    DataToShowComplementaryRulesList = new List<HPTHorseDataToShow>();
                    if (DataToShowComplementaryRules != null)
                    {
                        DataToShowComplementaryRules.IsDefault = true;
                        DataToShowComplementaryRules.GUIProfile = GUIProfile.Normal;
                        DataToShowComplementaryRulesList.Add(DataToShowComplementaryRules);
                    }
                }

                var dataToShowSimple = DataToShowComplementaryRulesList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
                if (dataToShowSimple == null)
                {
                    dataToShowSimple = CreateDataToShow(DataToShowUsage.ComplementaryRule, GUIProfile.Simple);
                    DataToShowComplementaryRulesList.Add(dataToShowSimple);
                }

                var dataToShowNormal = DataToShowComplementaryRulesList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
                if (dataToShowNormal == null)
                {
                    dataToShowNormal = CreateDataToShow(DataToShowUsage.ComplementaryRule, GUIProfile.Normal);
                    DataToShowComplementaryRulesList.Add(dataToShowNormal);
                }

                var dataToShowComplete = DataToShowComplementaryRulesList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
                if (dataToShowComplete == null)
                {
                    dataToShowComplete = CreateDataToShow(DataToShowUsage.ComplementaryRule, GUIProfile.Advanced);
                    DataToShowComplementaryRulesList.Add(dataToShowComplete);
                }

                if (DataToShowComplementaryRules == null)
                {
                    DataToShowComplementaryRules = DataToShowComplementaryRulesList.FirstOrDefault(dts => dts.IsDefault);
                    if (DataToShowComplementaryRules == null)
                    {
                        dataToShowSimple.IsDefault = true;
                        DataToShowComplementaryRules = dataToShowSimple;
                    }
                }
            }
            catch (Exception exc)
            {
                AddToErrorLog(exc);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseDataToShow> DataToShowComplementaryRulesList { get; set; }
        public HPTHorseDataToShow DataToShowComplementaryRules { get; set; }

        internal void HandleDataToShowCorrection()
        {
            try
            {
                if (DataToShowCorrectionList == null)
                {
                    DataToShowCorrectionList = new List<HPTHorseDataToShow>();
                    if (DataToShowCorrection != null)
                    {
                        DataToShowCorrection.IsDefault = true;
                        DataToShowCorrection.GUIProfile = GUIProfile.Normal;
                        DataToShowCorrectionList.Add(DataToShowCorrection);
                    }
                }

                var dataToShowSimple = DataToShowCorrectionList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
                if (dataToShowSimple == null)
                {
                    dataToShowSimple = CreateDataToShow(DataToShowUsage.Correction, GUIProfile.Simple);
                    DataToShowCorrectionList.Add(dataToShowSimple);
                }

                var dataToShowNormal = DataToShowCorrectionList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
                if (dataToShowNormal == null)
                {
                    dataToShowNormal = CreateDataToShow(DataToShowUsage.Correction, GUIProfile.Normal);
                    DataToShowCorrectionList.Add(dataToShowNormal);
                }

                var dataToShowComplete = DataToShowCorrectionList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
                if (dataToShowComplete == null)
                {
                    dataToShowComplete = CreateDataToShow(DataToShowUsage.Correction, GUIProfile.Advanced);
                    DataToShowCorrectionList.Add(dataToShowComplete);
                }

                if (DataToShowCorrection == null)
                {
                    DataToShowCorrection = DataToShowCorrectionList.FirstOrDefault(dts => dts.IsDefault);
                    if (DataToShowCorrection == null)
                    {
                        dataToShowSimple.IsDefault = true;
                        DataToShowCorrection = dataToShowSimple;
                    }
                }

                // Den nya kolumnen för resultatlänk
                DataToShowCorrection.ShowATGResultLink = true;
            }
            catch (Exception exc)
            {
                AddToErrorLog(exc);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseDataToShow> DataToShowCorrectionList { get; set; }
        public HPTHorseDataToShow DataToShowCorrection { get; set; }

        //internal void HandleDataToShowPersonReduction()
        //{
        //    if (this.DataToShowPersonReductionList == null)
        //    {
        //        this.DataToShowPersonReductionList = new List<HPTHorseDataToShow>();
        //        if (this.DataToShowPersonReduction != null)
        //        {
        //            this.DataToShowPersonReduction.GUIProfile = GUIProfile.Normal;
        //            this.DataToShowPersonReductionList.Add(this.DataToShowPersonReduction);
        //        }
        //    }

        //    var dataToShowSimple = this.DataToShowPersonReductionList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
        //    if (dataToShowSimple == null)
        //    {
        //        dataToShowSimple = new HPTHorseDataToShow()
        //        {
        //            Usage = DataToShowUsage.None,
        //            GUIProfile = GUIProfile.Simple,
        //            ShowDriver = true,
        //            ShowMarksPercent = true,
        //            ShowName = true,
        //            ShowPlatsOdds = true,
        //            ShowPrio = true,
        //            ShowReserv = true,
        //            ShowStakeDistributionPercent = true,
        //            ShowStartNr = true,
        //            ShowSystemCoverage = true,
        //            ShowVinnarOdds = true
        //        };
        //        this.DataToShowPersonReductionList.Add(dataToShowSimple);
        //    }

        //    var dataToShowNormal = this.DataToShowPersonReductionList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
        //    if (dataToShowNormal == null)
        //    {
        //        dataToShowNormal = new HPTHorseDataToShow()
        //        {
        //            Usage = DataToShowUsage.Vxx,
        //            GUIProfile = GUIProfile.Simple,
        //            ShowDriver = true,
        //            ShowMarksPercent = true,
        //            ShowName = true,
        //            ShowPlatsOdds = true,
        //            ShowPrio = true,
        //            ShowReserv = true,
        //            ShowStakeDistributionPercent = true,
        //            ShowStartNr = true,
        //            ShowSystemCoverage = true,
        //            ShowVinnarOdds = true
        //        };
        //        this.DataToShowPersonReductionList.Add(dataToShowNormal);
        //    }

        //    var dataToShowComplete = this.DataToShowPersonReductionList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Complete);
        //    if (dataToShowComplete == null)
        //    {
        //        dataToShowComplete = new HPTHorseDataToShow()
        //        {
        //            Usage = DataToShowUsage.Vxx,
        //            GUIProfile = GUIProfile.Simple,
        //            ShowDriver = true,
        //            ShowMarksPercent = true,
        //            ShowName = true,
        //            ShowPlatsOdds = true,
        //            ShowPrio = true,
        //            ShowReserv = true,
        //            ShowStakeDistributionPercent = true,
        //            ShowStartNr = true,
        //            ShowSystemCoverage = true,
        //            ShowVinnarOdds = true
        //        };
        //        this.DataToShowPersonReductionList.Add(dataToShowComplete);
        //    }

        //    if (this.DataToShowPersonReduction == null)
        //    {
        //        dataToShowSimple.IsDefault = true;
        //        this.DataToShowPersonReduction = dataToShowSimple;
        //    }
        //}

        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public List<HPTHorseDataToShow> DataToShowPersonReductionList { get; set; }
        public HPTHorseDataToShow DataToShowPersonReduction { get; set; }

        internal void HandleDataToShowDD()
        {
            try
            {
                if (DataToShowDDList == null)
                {
                    DataToShowDDList = new List<HPTHorseDataToShow>();
                    if (DataToShowDD != null)
                    {
                        DataToShowDD.Usage = DataToShowUsage.Double;
                        DataToShowDD.IsDefault = true;
                        DataToShowDD.GUIProfile = GUIProfile.Normal;
                        DataToShowDDList.Add(DataToShowDD);
                    }
                }

                var dataToShowSimple = DataToShowDDList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
                if (dataToShowSimple == null)
                {
                    dataToShowSimple = CreateDataToShow(DataToShowUsage.Double, GUIProfile.Simple);
                    DataToShowDDList.Add(dataToShowSimple);
                }

                var dataToShowNormal = DataToShowDDList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
                if (dataToShowNormal == null)
                {
                    dataToShowNormal = CreateDataToShow(DataToShowUsage.Double, GUIProfile.Normal);
                    DataToShowDDList.Add(dataToShowNormal);
                }

                var dataToShowComplete = DataToShowDDList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
                if (dataToShowComplete == null)
                {
                    dataToShowComplete = CreateDataToShow(DataToShowUsage.Double, GUIProfile.Advanced);
                    DataToShowDDList.Add(dataToShowComplete);
                }

                if (DataToShowDD == null)
                {
                    DataToShowDD = DataToShowDDList.FirstOrDefault(dts => dts.IsDefault);
                    if (DataToShowDD == null)
                    {
                        dataToShowSimple.IsDefault = true;
                        DataToShowDD = dataToShowSimple;
                    }
                }
            }
            catch (Exception exc)
            {
                AddToErrorLog(exc);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseDataToShow> DataToShowDDList { get; set; }
        public HPTHorseDataToShow DataToShowDD { get; set; }

        internal void HandleDataToShowTvilling()
        {
            try
            {
                if (DataToShowTvillingList == null)
                {
                    DataToShowTvillingList = new List<HPTHorseDataToShow>();
                    if (DataToShowTvilling != null)
                    {
                        DataToShowTvilling.Usage = DataToShowUsage.Tvilling;
                        DataToShowTvilling.IsDefault = true;
                        DataToShowTvilling.GUIProfile = GUIProfile.Normal;
                        DataToShowTvillingList.Add(DataToShowTvilling);
                    }
                }

                var dataToShowSimple = DataToShowTvillingList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
                if (dataToShowSimple == null)
                {
                    dataToShowSimple = CreateDataToShow(DataToShowUsage.Tvilling, GUIProfile.Simple);
                    DataToShowTvillingList.Add(dataToShowSimple);
                }

                var dataToShowNormal = DataToShowTvillingList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
                if (dataToShowNormal == null)
                {
                    dataToShowNormal = CreateDataToShow(DataToShowUsage.Tvilling, GUIProfile.Normal);
                    DataToShowTvillingList.Add(dataToShowNormal);
                }

                var dataToShowComplete = DataToShowTvillingList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
                if (dataToShowComplete == null)
                {
                    dataToShowComplete = CreateDataToShow(DataToShowUsage.Tvilling, GUIProfile.Advanced);
                    DataToShowTvillingList.Add(dataToShowComplete);
                }

                if (DataToShowTvilling == null)
                {
                    DataToShowTvilling = DataToShowTvillingList.FirstOrDefault(dts => dts.IsDefault);
                    if (DataToShowTvilling == null)
                    {
                        dataToShowSimple.IsDefault = true;
                        DataToShowTvilling = dataToShowSimple;
                    }
                }
            }
            catch (Exception exc)
            {
                AddToErrorLog(exc);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseDataToShow> DataToShowTvillingList { get; set; }
        public HPTHorseDataToShow DataToShowTvilling { get; set; }

        internal void HandleDataToShowTrio()
        {
            try
            {
                if (DataToShowTrioList == null)
                {
                    DataToShowTrioList = new List<HPTHorseDataToShow>();
                    if (DataToShowTrio != null)
                    {
                        DataToShowTrio.Usage = DataToShowUsage.Trio;
                        DataToShowTrio.IsDefault = true;
                        DataToShowTrio.GUIProfile = GUIProfile.Normal;
                        DataToShowTrioList.Add(DataToShowTrio);
                    }
                }

                var dataToShowSimple = DataToShowTrioList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
                if (dataToShowSimple == null)
                {
                    dataToShowSimple = CreateDataToShow(DataToShowUsage.Trio, GUIProfile.Simple);
                    DataToShowTrioList.Add(dataToShowSimple);
                }

                var dataToShowNormal = DataToShowTrioList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
                if (dataToShowNormal == null)
                {
                    dataToShowNormal = CreateDataToShow(DataToShowUsage.Trio, GUIProfile.Normal);
                    DataToShowTrioList.Add(dataToShowNormal);
                }

                var dataToShowComplete = DataToShowTrioList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
                if (dataToShowComplete == null)
                {
                    dataToShowComplete = CreateDataToShow(DataToShowUsage.Trio, GUIProfile.Advanced);
                    DataToShowTrioList.Add(dataToShowComplete);
                }

                if (DataToShowTrio == null)
                {
                    DataToShowTrio = DataToShowTrioList.FirstOrDefault(dts => dts.IsDefault);
                    if (DataToShowTrio == null)
                    {
                        dataToShowSimple.IsDefault = true;
                        DataToShowTrio = dataToShowSimple;
                    }
                }
            }
            catch (Exception exc)
            {
                AddToErrorLog(exc);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseDataToShow> DataToShowTrioList { get; set; }
        public HPTHorseDataToShow DataToShowTrio { get; set; }

        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public HPTHorseDataToShow DataToShowDriverPopupList { get; set; }
        private HPTHorseDataToShow dataToShowDriverPopup;
        public HPTHorseDataToShow DataToShowDriverPopup
        {
            get
            {
                if (dataToShowDriverPopup == null)
                {
                    dataToShowDriverPopup = new HPTHorseDataToShow()
                    {
                        Usage = DataToShowUsage.None,
                        ShowTrainer = true,
                        ShowHorsePopup = true,
                        ShowName = true,
                        ShowStartNr = true,
                        ShowVinnarOdds = true,
                        ShowPrio = true,
                        ShowStakeDistributionPercent = true,
                        ShowMarksPercent = true
                    };
                }
                return dataToShowDriverPopup;
            }
            set
            {
                dataToShowDriverPopup = value;
            }
        }

        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public HPTHorseDataToShow DataToShowTrainerPopupList { get; set; }
        private HPTHorseDataToShow dataToShowTrainerPopup;
        public HPTHorseDataToShow DataToShowTrainerPopup
        {
            get
            {
                if (dataToShowTrainerPopup == null)
                {
                    dataToShowTrainerPopup = new HPTHorseDataToShow()
                    {
                        Usage = DataToShowUsage.None,
                        ShowDriver = true,
                        ShowHorsePopup = true,
                        ShowName = true,
                        ShowStartNr = true,
                        ShowVinnarOdds = true,
                        ShowPrio = true,
                        ShowStakeDistributionPercent = true,
                        ShowMarksPercent = true
                    };
                }
                return dataToShowTrainerPopup;
            }
            set
            {
                dataToShowTrainerPopup = value;
            }
        }

        [DataMember]
        public HPTCombinationDataToShow CombinationDataToShowTvilling { get; set; }

        [DataMember]
        public HPTCombinationDataToShow CombinationDataToShowDouble { get; set; }

        [DataMember]
        public HPTCombinationDataToShow CombinationDataToShowTrio { get; set; }

        private HPTSingleRowDataToShow singleRowDataToShow;
        [DataMember]
        public HPTSingleRowDataToShow SingleRowDataToShow
        {
            get
            {
                if (singleRowDataToShow == null)
                {
                    singleRowDataToShow = new HPTSingleRowDataToShow()
                    {
                        EnableConfiguration = true,
                        ShowBetMultiplier = true,
                        ShowHorses = true,
                        ShowRankSum = false,
                        ShowRowNumber = true,
                        ShowRowValue = true,
                        ShowRowValue1And2Errors = false,
                        ShowRowValueBetMultiplier = true,
                        ShowRowValueV6 = true,
                        ShowStakeShareSum = false,
                        ShowStartNumberSum = false,
                        ShowV6 = true,
                        Usage = DataToShowUsage.Everywhere
                    };
                }
                return singleRowDataToShow;
            }
            set
            {
                singleRowDataToShow = value;
            }
        }

        internal static HPTMarkBetTabsToShow CreateMarkBetTabsToShow(GUIProfile profile)
        {
            var markBetTabsToShow = new HPTMarkBetTabsToShow()
            {
                GUIProfile = profile,
                ShowCorrection = true,
                ShowOverview = true,
                ShowRaces = true
            };
            if (profile == GUIProfile.Normal || profile == GUIProfile.Advanced)
            {
                markBetTabsToShow.ShowComplimentaryRules = true;
                markBetTabsToShow.ShowDriverReduction = true;
                markBetTabsToShow.ShowGroupIntervalReduction = true;
                markBetTabsToShow.ShowIntervalReduction = true;
                markBetTabsToShow.ShowMultiABCD = true;
                markBetTabsToShow.ShowRankReduction = true;
                markBetTabsToShow.ShowSingleRows = true;
                if (profile == GUIProfile.Advanced)
                {
                    markBetTabsToShow.ShowComments = false;
                    markBetTabsToShow.ShowCompanyGambling = false;
                    markBetTabsToShow.ShowRankOverview = true;
                    markBetTabsToShow.ShowTemplateWorkshop = true;
                    markBetTabsToShow.ShowTrainerReduction = true;
                    markBetTabsToShow.ShowTrends = true;
                    markBetTabsToShow.ShowV6BetMultiplier = true;
                }
            }
            return markBetTabsToShow;
        }

        internal HPTMarkBetTabsToShow GetMarkBetTabsToShow(GUIProfile profile)
        {
            var markBetTabsToShow = MarkBetTabsToShowList.FirstOrDefault(mbts => mbts.GUIProfile == profile);
            if (markBetTabsToShow == null)
            {
                markBetTabsToShow = CreateMarkBetTabsToShow(profile);
                MarkBetTabsToShowList.Add(markBetTabsToShow);
            }
            return markBetTabsToShow;
        }

        internal void SetMarkBetTabsToShow(HPTMarkBetTabsToShow markBetTabsToShow)
        {
            markBetTabsToShow.IsDefault = true;
            if (MarkBetTabsToShow == markBetTabsToShow)
            {
                return;
            }
            MarkBetTabsToShow = markBetTabsToShow;
            var markBetTabsToShowOld = MarkBetTabsToShowList.FirstOrDefault(mbts => mbts.GUIProfile == MarkBetTabsToShow.GUIProfile);
            if (markBetTabsToShowOld != null)
            {
                MarkBetTabsToShowList.Remove(markBetTabsToShowOld);
            }
            MarkBetTabsToShowList.Add(markBetTabsToShow);
        }

        internal void HandleMarkBetTabsToShow()
        {
            if (MarkBetTabsToShowList == null || MarkBetTabsToShowList.Count == 0)
            {
                MarkBetTabsToShowList = new List<HPTMarkBetTabsToShow>();
                if (MarkBetTabsToShow != null)
                {
                    MarkBetTabsToShow.IsDefault = false;
                    MarkBetTabsToShow.GUIProfile = GUIProfile.Normal;
                    MarkBetTabsToShowList.Add(MarkBetTabsToShow);
                }
            }

            var dataToShowSimple = MarkBetTabsToShowList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
            if (dataToShowSimple == null)
            {
                dataToShowSimple = CreateMarkBetTabsToShow(GUIProfile.Simple);
                dataToShowSimple.IsDefault = true;
                MarkBetTabsToShowList.Add(dataToShowSimple);
                MarkBetTabsToShow = dataToShowSimple;
            }

            var dataToShowNormal = MarkBetTabsToShowList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
            if (dataToShowNormal == null)
            {
                dataToShowNormal = CreateMarkBetTabsToShow(GUIProfile.Normal);
                MarkBetTabsToShowList.Add(dataToShowNormal);
            }

            var dataToShowComplete = MarkBetTabsToShowList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
            if (dataToShowComplete == null)
            {
                dataToShowComplete = CreateMarkBetTabsToShow(GUIProfile.Advanced);
                MarkBetTabsToShowList.Add(dataToShowComplete);
            }

            if (MarkBetTabsToShow == null)
            {
                MarkBetTabsToShow = MarkBetTabsToShowList.FirstOrDefault(mbt => mbt.IsDefault);
                if (MarkBetTabsToShow == null)
                {
                    dataToShowSimple.IsDefault = true;
                    MarkBetTabsToShow = dataToShowSimple;
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTMarkBetTabsToShow> MarkBetTabsToShowList { get; set; }
        public HPTMarkBetTabsToShow MarkBetTabsToShow { get; set; }

        internal void SetMarkBetProfile(GUIProfile profile)
        {
            Profile = profile;

            MarkBetTabsToShow.IsDefault = false;
            MarkBetTabsToShow = MarkBetTabsToShowList.First(mbts => mbts.GUIProfile == profile);
            MarkBetTabsToShow.IsDefault = true;

            DataToShowVxx.IsDefault = false;
            DataToShowVxx = DataToShowVxxList.First(dts => dts.GUIProfile == profile);
            DataToShowVxx.IsDefault = true;

            DataToShowComplementaryRules.IsDefault = false;
            DataToShowComplementaryRules = DataToShowComplementaryRulesList.First(dts => dts.GUIProfile == profile);
            DataToShowComplementaryRules.IsDefault = true;

            DataToShowCorrection.IsDefault = false;
            DataToShowCorrection = DataToShowCorrectionList.First(dts => dts.GUIProfile == profile);
            DataToShowCorrection.IsDefault = true;

            GUIElementsToShow.IsDefault = false;
            GUIElementsToShow = GUIElementsToShowList.First(gts => gts.GUIProfile == profile);
            GUIElementsToShow.IsDefault = true;
        }

        [DataMember]
        public HPTBetTypesToShow BetTypesToShow { get; set; }

        private List<HPTHorseRankVariableBase> horseRankVariablesToShow;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<HPTHorseRankVariableBase> HorseRankVariablesToShow
        {
            get
            {
                var allHorseRankVariables = HPTHorseRankVariableBase.CreateVariableBaseList();
                if (horseRankVariablesToShow == null)
                {
                    horseRankVariablesToShow = allHorseRankVariables;
                }
                if (horseRankVariablesToShow.Count < allHorseRankVariables.Count)
                {
                    var allProperties = allHorseRankVariables.Select(hrv => hrv.PropertyName);
                    var currentProperties = horseRankVariablesToShow.Select(hrv => hrv.PropertyName);
                    var newProperties = allProperties.Except(currentProperties);
                    foreach (var newProperty in newProperties)
                    {
                        var newHorseRankVariable = allHorseRankVariables.First(hrv => hrv.PropertyName == newProperty);
                        horseRankVariablesToShow.Add(newHorseRankVariable);
                    }
                }
                return horseRankVariablesToShow;
            }
            set
            {
                horseRankVariablesToShow = value;
            }
        }

        #endregion

        #region GUI-hantering

        internal void HandleGUIElementsToShow()
        {
            if (GUIElementsToShowList == null)
            {
                GUIElementsToShowList = new List<HPTGUIElementsToShow>();
                if (GUIElementsToShow != null)
                {
                    GUIElementsToShow.IsDefault = true;
                    GUIElementsToShow.GUIProfile = GUIProfile.Normal;
                    GUIElementsToShowList.Add(GUIElementsToShow);
                }
            }

            var dataToShowSimple = GUIElementsToShowList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Simple);
            if (dataToShowSimple == null)
            {
                dataToShowSimple = GetElementsToShow(GUIProfile.Simple);
                GUIElementsToShowList.Add(dataToShowSimple);
            }

            var dataToShowNormal = GUIElementsToShowList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Normal);
            if (dataToShowNormal == null)
            {
                dataToShowNormal = GetElementsToShow(GUIProfile.Normal);
                GUIElementsToShowList.Add(dataToShowNormal);
            }

            var dataToShowComplete = GUIElementsToShowList.FirstOrDefault(dts => dts.GUIProfile == GUIProfile.Advanced);
            if (dataToShowComplete == null)
            {
                dataToShowComplete = GetElementsToShow(GUIProfile.Advanced);
                GUIElementsToShowList.Add(dataToShowComplete);
            }

            if (GUIElementsToShow == null)
            {
                GUIElementsToShow = GUIElementsToShowList.FirstOrDefault(mbt => mbt.IsDefault);
                if (GUIElementsToShow == null)
                {
                    dataToShowSimple.IsDefault = true;
                    GUIElementsToShow = dataToShowSimple;
                }
            }
        }

        internal HPTGUIElementsToShow GetElementsToShow(GUIProfile guiProfile)
        {
            HPTGUIElementsToShow guiElementsToShow = null;
            if (GUIElementsToShowList != null)
            {
                guiElementsToShow = GUIElementsToShowList.FirstOrDefault(gets => gets.GUIProfile == guiProfile);
                if (guiElementsToShow != null)
                {
                    return guiElementsToShow;
                }
            }
            guiElementsToShow = new HPTGUIElementsToShow()
            {
                GUIProfile = guiProfile
            };
            if (guiProfile == GUIProfile.Normal || guiProfile == GUIProfile.Advanced)
            {
                guiElementsToShow.ShowBeginner = true;
                guiElementsToShow.ShowClear = true;
                guiElementsToShow.ShowCopy = true;
                guiElementsToShow.ShowCouponInfo = true;
                guiElementsToShow.ShowLiveCalculation = true;
                guiElementsToShow.ShowNumberOfGambledRows = true;
                guiElementsToShow.ShowOverview = true;
                guiElementsToShow.ShowPrint = true;
                guiElementsToShow.ShowReductionList = true;
                guiElementsToShow.ShowReductionPercentage = true;
                guiElementsToShow.ShowReservHandling = true;
                guiElementsToShow.ShowSaveAs = true;
                guiElementsToShow.ShowTemplates = true;
                guiElementsToShow.ShowUpload = true;
                guiElementsToShow.ShowV6 = true;

                if (guiProfile == GUIProfile.Advanced)
                {
                    guiElementsToShow.ShowAutomaticCalculation = true;
                    guiElementsToShow.ShowBetMultiplier = true;
                    guiElementsToShow.ShowCouponCompression = true;
                    guiElementsToShow.ShowRaceLock = true;
                    guiElementsToShow.ShowRowValueInterval = true;
                    guiElementsToShow.ShowSystemCostChange = true;
                }
            }
            //this.GUIElementsToShow = guiElementsToShow;
            return guiElementsToShow;
        }

        [DataMember]
        public List<HPTGUIElementsToShow> GUIElementsToShowList { get; set; }
        public HPTGUIElementsToShow GUIElementsToShow { get; set; }

        [DataMember]
        public GUIProfile Profile { get; set; }

        #endregion

        #region Data till klippbordet/utskriften

        private bool? copyStakeShare;
        [DataMember]
        public bool? CopyStakeShare
        {
            get
            {
                if (copyStakeShare == null)
                {
                    copyStakeShare = true;
                }
                return copyStakeShare;
            }
            set
            {
                copyStakeShare = value;
                OnPropertyChanged("CopyStakeShare");
            }
        }

        private bool? copyOwnRank;
        [DataMember]
        public bool? CopyOwnRank
        {
            get
            {
                if (copyOwnRank == null)
                {
                    copyOwnRank = false;
                }
                return copyOwnRank;
            }
            set
            {
                copyOwnRank = value;
                OnPropertyChanged("CopyOwnRank");
            }
        }

        private bool? copyRankMean;
        [DataMember]
        public bool? CopyRankMean
        {
            get
            {
                if (copyRankMean == null)
                {
                    copyRankMean = false;
                }
                return copyRankMean;
            }
            set
            {
                copyRankMean = value;
                OnPropertyChanged("CopyRankMean");
            }
        }

        private bool? copyAlternateRank;
        [DataMember]
        public bool? CopyAlternateRank
        {
            get
            {
                if (copyAlternateRank == null)
                {
                    copyAlternateRank = false;
                }
                return copyAlternateRank;
            }
            set
            {
                copyAlternateRank = value;
                OnPropertyChanged("CopyAlternateRank");
            }
        }

        #endregion

        // [XmlArray]
        // [DataMember]
        // public ObservableCollection<HPTMailList> MailListCollection { get; set; }

        #region Spårrankinställningar

        private ObservableCollection<HPTStartNumberRankCollection> startNumberRankCollectionList;
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ObservableCollection<HPTStartNumberRankCollection> StartNumberRankCollectionList
        {
            get
            {
                if (startNumberRankCollectionList == null)
                {
                    CreateDefaultStartNumberRankCollectionList();
                }
                return startNumberRankCollectionList;
            }
            set
            {
                startNumberRankCollectionList = value;
            }
        }

        public void CreateDefaultStartNumberRankCollectionList()
        {
            startNumberRankCollectionList = new ObservableCollection<HPTStartNumberRankCollection>();

            var startNumberRankCollectionAuto = new HPTStartNumberRankCollection()
            {
                DistanceCode = "",
                Name = "Auto",
                StartMethodCode = "A",
                StartNumberRankList = Enumerable.Range(1, 15)
                    .Select(i => new HPTStartNumberRank()
                    {
                        Rank = SetStartNumberRankForAuto(i),
                        Select = false,
                        StartNumber = i
                    }).ToList()
            };
            startNumberRankCollectionList.Add(startNumberRankCollectionAuto);

            var startNumberRankCollectionVolt = new HPTStartNumberRankCollection()
            {
                DistanceCode = "",
                Name = "Volt",
                StartMethodCode = "V",
                StartNumberRankList = Enumerable.Range(1, 15)
                    .Select(i => new HPTStartNumberRank()
                    {
                        Rank = SetStartNumberRankForVolt(i),
                        Select = false,
                        StartNumber = i
                    }).ToList()
            };
            startNumberRankCollectionList.Add(startNumberRankCollectionVolt);
        }

        internal int SetStartNumberRankForAuto(int startNumber)
        {
            switch (startNumber)
            {
                case 1:
                case 2:
                case 3:
                case 6:
                case 7:
                    return 2;
                case 4:
                case 5:
                    return 1;
                default:
                    return 3;
            }
        }

        internal int SetStartNumberRankForVolt(int startNumber)
        {
            switch (startNumber)
            {
                case 1:
                case 3:
                case 6:
                case 7:
                    return 1;
                case 2:
                    return 2;
                default:
                    return 3;
            }
        }

        #endregion

        #region ABCDEF-val

        private bool useA;
        [DataMember]
        public bool UseA
        {
            get
            {
                return useA;
            }
            set
            {
                useA = value;
                PrioList[HPTPrio.A] = value;
                OnPropertyChanged("UseA");
            }
        }

        private bool useB;
        [DataMember]
        public bool UseB
        {
            get
            {
                return useB;
            }
            set
            {
                useB = value;
                PrioList[HPTPrio.B] = value;
                OnPropertyChanged("UseB");
            }
        }

        private bool useC;
        [DataMember]
        public bool UseC
        {
            get
            {
                return useC;
            }
            set
            {
                useC = value;
                PrioList[HPTPrio.C] = value;
                OnPropertyChanged("UseC");
            }
        }

        private bool useD;
        [DataMember]
        public bool UseD
        {
            get
            {
                return useD;
            }
            set
            {
                useD = value;
                PrioList[HPTPrio.D] = value;
                OnPropertyChanged("UseD");
            }
        }

        private bool useE;
        [DataMember]
        public bool UseE
        {
            get
            {
                return useE;
            }
            set
            {
                useE = value;
                PrioList[HPTPrio.E] = value;
                OnPropertyChanged("UseE");
            }
        }

        private bool useF;
        [DataMember]
        public bool UseF
        {
            get
            {
                return useF;
            }
            set
            {
                useF = value;
                PrioList[HPTPrio.F] = value;
                OnPropertyChanged("UseF");
            }
        }

        #endregion

        #region User information

        //internal static string HPTUSERSETTINGS = "hptusersettings.txt";
        //internal void SetValuesInIsolatedStorage()
        //{
        //    return;
        //    if (string.IsNullOrEmpty(this.EMailAddress) ||string.IsNullOrEmpty(this.Password))
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
        //        if (isolatedStorageFile.FileExists(HPTUSERSETTINGS))
        //        {
        //            isolatedStorageFile.DeleteFile(HPTUSERSETTINGS);
        //        }
        //        var fileStream = isolatedStorageFile.CreateFile(HPTUSERSETTINGS);

        //        var writer = new StreamWriter(fileStream);
        //        writer.WriteLine(this.EMailAddress);
        //        writer.WriteLine(this.Password);
        //        writer.Close();
        //    }
        //    catch (Exception exc)
        //    {
        //        AddToErrorLog(exc);
        //    }
        //}

        //internal bool GetValuesFromIsolatedStorage()
        //{
        //    return false;
        //    try
        //    {
        //        var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
        //        if (isolatedStorageFile.FileExists(HPTUSERSETTINGS))
        //        {
        //            var fileStream = isolatedStorageFile.OpenFile(HPTUSERSETTINGS, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        //            var reader = new StreamReader(fileStream);
        //            string eMail = reader.ReadLine();
        //            string password = reader.ReadLine();
        //            reader.Close();

        //            if (string.IsNullOrEmpty(eMail) || string.IsNullOrEmpty(password))
        //            {
        //                return false;
        //            }
        //            this.EMailAddress = eMail;
        //            this.Password = password;
        //            return true;
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        AddToErrorLog(exc);
        //    }
        //    return false;
        //}

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string EMailAddress { get; set; }

        [DataMember]
        public string Password { get; set; }

        [XmlIgnore]
        public string UserNameForUploads
        {
            get
            {
                if (string.IsNullOrEmpty(EMailAddress))
                {
                    return "Lokal amvändare";
                }
                if (string.IsNullOrEmpty(UserName) || UserName == EMailAddress)
                {
                    if (EMailAddress.Contains('@'))
                    {
                        return EMailAddress.Split('@').First();
                    }
                    else
                    {
                        return "Lokal amvändare";
                    }
                }
                return UserName;
            }
        }

        [DataMember]
        public DateTime LastRegisterWindowPopup { get; set; }

        [DataMember]
        public DateTime LastUse { get; set; }

        #endregion

        #region Color handling

        private Color colorGood;
        [DataMember]
        public Color ColorGood
        {
            get
            {
                return colorGood;
            }
            set
            {
                colorGood = value;
                BrushGood = CreateBrush(value);
                OnPropertyChanged("ColorGood");
            }
        }

        private Color colorMedium;
        [DataMember]
        public Color ColorMedium
        {
            get
            {
                return colorMedium;
            }
            set
            {
                colorMedium = value;
                BrushMedium = CreateBrush(value);
                OnPropertyChanged("ColorMedium");
            }
        }

        private Color colorBad;
        [DataMember]
        public Color ColorBad
        {
            get
            {
                return colorBad;
            }
            set
            {
                colorBad = value;
                BrushBad = CreateBrush(value);
                OnPropertyChanged("ColorBad");
            }
        }

        private Brush brushGood;
        [XmlIgnore]
        public Brush BrushGood
        {
            get
            {
                return brushGood;
            }
            set
            {
                brushGood = value;
                OnPropertyChanged("BrushGood");
            }
        }

        private Brush brushMedium;
        [XmlIgnore]
        public Brush BrushMedium
        {
            get
            {
                return brushMedium;
            }
            set
            {
                brushMedium = value;
                OnPropertyChanged("BrushMedium");
            }
        }

        private Brush brushBad;
        [XmlIgnore]
        public Brush BrushBad
        {
            get
            {
                return brushBad;
            }
            set
            {
                brushBad = value;
                OnPropertyChanged("BrushBad");
            }
        }

        private bool setColorFromVinnarOdds;
        [DataMember]
        public bool SetColorFromVinnarOdds
        {
            get
            {
                return setColorFromVinnarOdds;
            }
            set
            {
                if (value)
                {
                    SetColorFromMarksPercent = false;
                    SetColorFromMarkability = false;
                    SetColorFromStakePercent = false;
                }
                setColorFromVinnarOdds = value;
                OnPropertyChanged("SetColorFromVinnarOdds");
            }
        }

        private bool setColorFromMarksPercent;
        [DataMember]
        public bool SetColorFromMarksPercent
        {
            get
            {
                return setColorFromMarksPercent;
            }
            set
            {
                if (value)
                {
                    SetColorFromVinnarOdds = false;
                    SetColorFromMarkability = false;
                    SetColorFromStakePercent = false;
                }
                setColorFromMarksPercent = value;
                OnPropertyChanged("SetColorFromMarksPercent");
            }
        }

        private bool setColorFromMarkability;
        [DataMember]
        public bool SetColorFromMarkability
        {
            get
            {
                return setColorFromMarkability;
            }
            set
            {
                if (value)
                {
                    SetColorFromMarksPercent = false;
                    SetColorFromVinnarOdds = false;
                    SetColorFromStakePercent = false;
                }
                setColorFromMarkability = value;
                OnPropertyChanged("SetColorFromMarkability");
            }
        }

        private bool setColorFromStakePercent;
        [DataMember]
        public bool SetColorFromStakePercent
        {
            get
            {
                return setColorFromStakePercent;
            }
            set
            {
                if (value)
                {
                    SetColorFromMarksPercent = false;
                    SetColorFromVinnarOdds = false;
                    SetColorFromMarkability = false;
                }
                setColorFromStakePercent = value;
                OnPropertyChanged("SetColorFromStakePercent");
            }
        }

        #endregion

        #region Central hantering av kommunikation mellan olika flikar

        //internal void GetLatestUpdate(HPTBet bet, int updatePeriod)
        //{
        //    HPTBet firstOtherBet = this.AvailableBets
        //        .Where(b => b.BetType.Code == bet.BetType.Code
        //        && b.RaceDayInfo.RaceDayDateString == bet.RaceDayInfo.RaceDayDateString
        //        && b.RaceDayInfo.TrackId == bet.RaceDayInfo.TrackId
        //        && b != bet)
        //        .OrderByDescending(b => b.TimeStamp)
        //        .FirstOrDefault();

        //    if (firstOtherBet != null && firstOtherBet.TimeStamp > bet.TimeStamp)
        //    {

        //    }
        //}

        //internal IEnumerable<HPTBet> GetSameBetList(HPTBet bet)
        //{
        //    IEnumerable<HPTBet> sameBetList = this.AvailableBets
        //        .Where(b => b.BetType.Code == bet.BetType.Code
        //        && b.RaceDayInfo.RaceDayDateString == bet.RaceDayInfo.RaceDayDateString
        //        && b.RaceDayInfo.TrackId == bet.RaceDayInfo.TrackId
        //        && b != bet);

        //    return sameBetList;
        //}

        //internal IEnumerable<HPTBet> GetOverlappingBetList(HPTBet bet)
        //{
        //    IEnumerable<HPTBet> sameRaceDayBetList = this.AvailableBets
        //        .Where(b => b.BetType.Code != bet.BetType.Code
        //        && b.RaceDayInfo.RaceDayDateString == bet.RaceDayInfo.RaceDayDateString
        //        && b.RaceDayInfo.TrackId == bet.RaceDayInfo.TrackId
        //        && b.RaceDayInfo.RaceNumberList.Intersect(bet.RaceDayInfo.RaceNumberList).Count() > 0);

        //    return sameRaceDayBetList;
        //}

        #endregion

        #region Resurser

        internal Stream GetEmbeddedResource(string resourceName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                //var names = asm.GetManifestResourceNames();
                //string s = names.ToString();
                return asm.GetManifestResourceStream(resourceName);
            }
            catch (Exception)
            {
                return null;
            }

        }

        #endregion

        #region PLINQ-versioner

        public void UpdateHPTSystemDirectoriesParallell()
        {
            var di = new DirectoryInfo(MyDocumentsPath);

            // Kataloger som är automatgenererade för att innehålla filer för olika tävlingar
            Regex rexSystemDirectory = new Regex("\\d{4}-\\d{2}-\\d{2}\\s[\\w\\s]+?");

            var hptSystemDirectories = di.GetDirectories()
                .Where(diSystemDir => rexSystemDirectory.IsMatch(diSystemDir.Name))
                .AsParallel()
                .Select(diSystemDir => new HPTSystemDirectory()
                {
                    DirectoryName = diSystemDir.FullName,
                    DirectoryNameShort = diSystemDir.Name,
                    FileList = new ObservableCollection<HPTSystemFile>(
                        diSystemDir.GetFiles("*.hpt?")
                        .Select(fiHPTFile => new HPTSystemFile()
                        {
                            DisplayName = CreateFileDisplayName(fiHPTFile.FullName, fiHPTFile.LastWriteTime),
                            FileName = fiHPTFile.FullName,
                            FileNameShort = fiHPTFile.Name,
                            FileType = fiHPTFile.Extension.Replace(".", string.Empty),
                            CreationTime = fiHPTFile.CreationTime
                        }))
                });

            // Ta bara kataloger som innehåller filer
            HPTSystemDirectories = new ObservableCollection<HPTSystemDirectory>(
                hptSystemDirectories
                .Where(hsd => hsd.FileList.Count > 0)
                .OrderByDescending(hsd => hsd.DirectoryNameShort)
                .Take(20)
                );

            //// 10 senaste filerna
            //this.RecentFileList = new ObservableCollection<HPTSystemFile>(
            //    this.HPTSystemDirectories
            //    .SelectMany(hsd => hsd.FileList)
            //    .OrderByDescending(hsf => hsf.CreationTime)
            //    .Take(10)
            //    );
        }

        #endregion
    }
}

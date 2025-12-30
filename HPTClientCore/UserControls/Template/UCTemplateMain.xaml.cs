using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTemplateMain.xaml
    /// </summary>
    public partial class UCTemplateMain : UserControl
    {
        public UCTemplateMain()
        {
            InitializeComponent();
        }

        public HPTConfig Config
        {
            get { return (HPTConfig)GetValue(ConfigProperty); }
            set { SetValue(ConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Config.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(UCTemplateMain), new UIPropertyMetadata(HPTConfig.Config));

        #region Importera/Exportera mallar

        private void btnImportRankVariableTemplates_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                InitialDirectory = HPTConfig.MyDocumentsPath,
                Filter = "HPT 5-mallar|*.hpt7m;*.hptam;*.hptrm;*.hptrvm;*.hptrsm;*.hptgim",
                Multiselect = true
            };

            ofd.FileOk += (sOk, eOk) =>
                {
                    var dlg = (OpenFileDialog)sOk;
                    HPTConfig.ImportTemplates(dlg.FileNames);
                };

            ofd.ShowDialog();

            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = HPTConfig.MyDocumentsPath;
            //ofd.Filter = "HPT-mallar|*.hpt5m;*.hptm";
            //ofd.FileOk += new System.ComponentModel.CancelEventHandler(ofd_FileOk);
            //ofd.ShowDialog();
        }

        private void btnExportRankVariableTemplates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = HPTConfig.ExportTemplatesToDisk();
                var dr = MessageBox.Show("Mallar exporterade till " + result + ". Vill du öppna katalog?", "Mallar exporterade", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(result);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Något gick snett vid export av mallar. Se fellogg för detaljer.", "Fel vid export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void ofd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    OpenFileDialog ofd = (OpenFileDialog)sender;
                    var templateCollection = HPTSerializer.DeserializeHPTTemplateCollection(ofd.FileName);
                    CopyTemplatesToConfig(templateCollection);
                }
                catch (Exception exc)
                {
                    HPTConfig.AddToErrorLogStatic(exc);
                }
            }
        }

        //void sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (!e.Cancel)
        //    {
        //        try
        //        {
        //            Cursor = Cursors.Wait;
        //            SaveFileDialog sfd = (SaveFileDialog)sender;
        //            string fileName = sfd.FileName;
        //            var templateCollection = new HPTTemplateCollection()
        //            {
        //                GroupIntervalTemplateList = this.Config.GroupIntervalRulesCollectionList.ToList(),
        //                RankTemplateList = this.Config.RankTemplateList.Where(rt => !rt.IsDefault).ToList(),
        //                MarkBetTemplateABCDList = this.Config.MarkBetTemplateABCDList.Where(t => !t.IsDefault).ToList(),
        //                MarkBetTemplateRankList = this.Config.MarkBetTemplateRankList.Where(t => !t.IsDefault).ToList(),
        //                RankSumReductionRuleCollection = this.Config.RankSumReductionRuleCollection.ToList()
        //            };
        //            HPTSerializer.SerializeHPTTemplateCollection(sfd.FileName, templateCollection);
        //        }
        //        catch (Exception exc)
        //        {
        //            HPTConfig.AddToErrorLogStatic(exc); ;
        //        }
        //        Cursor = Cursors.Arrow;
        //    }
        //}

        //void sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (!e.Cancel)
        //    {
        //        try
        //        {
        //            Cursor = Cursors.Wait;
        //            var sfd = (SaveFileDialog)sender;
        //            string[] fileNames = sfd.FileNames;
        //            var templateCollection = new HPTTemplateCollection()
        //            {
        //                GroupIntervalTemplateList = this.Config.GroupIntervalRulesCollectionList.ToList(),
        //                RankTemplateList = this.Config.RankTemplateList.Where(rt => !rt.IsDefault).ToList(),
        //                MarkBetTemplateABCDList = this.Config.MarkBetTemplateABCDList.Where(t => !t.IsDefault).ToList(),
        //                MarkBetTemplateRankList = this.Config.MarkBetTemplateRankList.Where(t => !t.IsDefault).ToList(),
        //                RankSumReductionRuleCollection = this.Config.RankSumReductionRuleCollection.ToList()
        //            };
        //            HPTSerializer.SerializeHPTTemplateCollection(sfd.FileName, templateCollection);
        //        }
        //        catch (Exception exc)
        //        {
        //            HPTConfig.AddToErrorLogStatic(exc); ;
        //        }
        //        Cursor = Cursors.Arrow;
        //    }
        //}

        private void CopyTemplatesToConfig(HPTTemplateCollection templateCollection)
        {
            #region Rankvariabelmallar
            foreach (var rankTemplate in templateCollection.RankTemplateList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingRankTemplate = this.Config.RankTemplateList.FirstOrDefault(rt => rt.Name == rankTemplate.Name);
                if (existingRankTemplate != null)
                {
                    int rankTemplateNumber = 1;
                    string templateName = rankTemplate.Name;
                    while (existingRankTemplate != null)
                    {
                        rankTemplateNumber++;
                        templateName = rankTemplate.Name + " (" + rankTemplateNumber.ToString() + ")";
                        existingRankTemplate = this.Config.RankTemplateList.FirstOrDefault(rt => rt.Name == templateName);
                    }
                    ChangeRankTemplateReference(templateCollection.MarkBetTemplateABCDList, rankTemplate.Name, templateName);
                    ChangeRankTemplateReference(templateCollection.MarkBetTemplateRankList, rankTemplate.Name, templateName);
                    rankTemplate.Name = templateName;   // Sätt namn med löpnummer efter
                }
                this.Config.RankTemplateList.Add(rankTemplate);
            }
            #endregion

            #region ABCD-mallar
            foreach (var markBetABCDTemplate in templateCollection.MarkBetTemplateABCDList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = this.Config.MarkBetTemplateABCDList.FirstOrDefault(t => t.Name == markBetABCDTemplate.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = markBetABCDTemplate.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = markBetABCDTemplate.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = this.Config.MarkBetTemplateABCDList.FirstOrDefault(t => t.Name == templateName);
                    }
                    markBetABCDTemplate.Name = templateName;   // Sätt namn med löpnummer efter
                }
                markBetABCDTemplate.RankTemplate = this.Config.RankTemplateList.FirstOrDefault(rt => rt.Name == markBetABCDTemplate.RankTemplateName);
                this.Config.MarkBetTemplateABCDList.Add(markBetABCDTemplate);
            }
            #endregion

            #region Rankmallar
            foreach (var markBetRankTemplate in templateCollection.MarkBetTemplateRankList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = this.Config.MarkBetTemplateRankList.FirstOrDefault(t => t.Name == markBetRankTemplate.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = markBetRankTemplate.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = markBetRankTemplate.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = this.Config.MarkBetTemplateRankList.FirstOrDefault(t => t.Name == templateName);
                    }
                    markBetRankTemplate.Name = templateName;   // Sätt namn med löpnummer efter
                }
                markBetRankTemplate.RankTemplate = this.Config.RankTemplateList.FirstOrDefault(rt => rt.Name == markBetRankTemplate.RankTemplateName);
                this.Config.MarkBetTemplateRankList.Add(markBetRankTemplate);
            }
            #endregion

            #region Gruppintervallmallar
            foreach (var groupIntervalRulesCollection in templateCollection.GroupIntervalTemplateList)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = this.Config.GroupIntervalRulesCollectionList.FirstOrDefault(t => t.Name == groupIntervalRulesCollection.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = groupIntervalRulesCollection.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = groupIntervalRulesCollection.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = this.Config.GroupIntervalRulesCollectionList.FirstOrDefault(t => t.Name == templateName);
                    }
                    groupIntervalRulesCollection.Name = templateName;   // Sätt namn med löpnummer efter
                }
                foreach (HPTGroupIntervalReductionRule rule in groupIntervalRulesCollection.ReductionRuleList)
                {
                    rule.HorseVariable = HPTConfig.Config.HorseVariableList.FirstOrDefault(hv => hv.PropertyName == rule.PropertyName);
                    //rule.HorseVariable = HPTHorseVariable.SortedVariableList[rule.PropertyName];
                }
                this.Config.GroupIntervalRulesCollectionList.Add(groupIntervalRulesCollection);
            }
            #endregion

            #region Rankreduceringsmallar
            var rankVariableList = HPTHorseRankVariable.CreateVariableList();
            foreach (var rankSumReductionCollection in templateCollection.RankSumReductionRuleCollection)
            {
                // Kontrollera om det redan finns en mall som heter likadant
                var existingTemplate = this.Config.RankSumReductionRuleCollection.FirstOrDefault(t => t.Name == rankSumReductionCollection.Name);
                if (existingTemplate != null)
                {
                    int templateNumber = 1;
                    string templateName = rankSumReductionCollection.Name;
                    while (existingTemplate != null)
                    {
                        templateNumber++;
                        templateName = rankSumReductionCollection.Name + " (" + templateNumber.ToString() + ")";
                        existingTemplate = this.Config.RankSumReductionRuleCollection.FirstOrDefault(t => t.Name == templateName);
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
                this.Config.RankSumReductionRuleCollection.Add(rankSumReductionCollection);
            }
            #endregion
        }

        private void ChangeRankTemplateReference(IEnumerable<HPTMarkBetTemplate> templateList, string oldName, string newName)
        {
            foreach (var template in templateList.Where(t => t.Name == oldName).ToList())
            {
                template.RankTemplateName = newName;
            }
        }

        #endregion
    }
}

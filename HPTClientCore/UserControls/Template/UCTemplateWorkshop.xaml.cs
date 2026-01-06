using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for UCTemplateWorkshop.xaml
    /// </summary>
    public partial class UCTemplateWorkshop : UCMarkBetControl
    {
        public UCTemplateWorkshop()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                CurrentMarkBetTemplateABCD = new HPTMarkBetTemplateABCD()
                {
                    DesiredSystemSize = 1000,
                    NumberOfSpikes = 1
                };
                CurrentMarkBetTemplateABCD.InitializeTemplate(new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C });
                CurrentMarkBetTemplateRank = new HPTMarkBetTemplateRank();
            }
            InitializeComponent();
        }

        #region Dependency properties

        public HPTRankTemplate CurrentRankTemplate
        {
            get { return (HPTRankTemplate)GetValue(CurrentRankTemplateProperty); }
            set { SetValue(CurrentRankTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentRankTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentRankTemplateProperty =
            DependencyProperty.Register("CurrentRankTemplate", typeof(HPTRankTemplate), typeof(UCTemplateWorkshop), new UIPropertyMetadata(null));



        public HPTMarkBetTemplateABCD CurrentMarkBetTemplateABCD
        {
            get { return (HPTMarkBetTemplateABCD)GetValue(CurrentMarkBetTemplateABCDProperty); }
            set { SetValue(CurrentMarkBetTemplateABCDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMarkBetTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMarkBetTemplateABCDProperty =
            DependencyProperty.Register("CurrentMarkBetTemplateABCD", typeof(HPTMarkBetTemplateABCD), typeof(UCTemplateWorkshop), new UIPropertyMetadata(null));



        public HPTMarkBetTemplateRank CurrentMarkBetTemplateRank
        {
            get { return (HPTMarkBetTemplateRank)GetValue(CurrentMarkBetTemplateRankProperty); }
            set { SetValue(CurrentMarkBetTemplateRankProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMarkBetTemplateRank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMarkBetTemplateRankProperty =
            DependencyProperty.Register("CurrentMarkBetTemplateRank", typeof(HPTMarkBetTemplateRank), typeof(UCTemplateWorkshop), new UIPropertyMetadata(null));


        #endregion

        private bool isSelecting;
        private void cmbRankTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                isSelecting = true;
                try
                {
                    RemoveEventHandlers();
                    var rankTemplate = (HPTRankTemplate)e.AddedItems[0];
                    CurrentRankTemplate = rankTemplate.Clone();
                    UpdateSelectionFromTemplate();
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
                //RemoveEventHandlers();
                AddEventHandlers();
                isSelecting = false;
            }
        }

        private void RankTemplate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (isSelecting)
            {
                return;
            }
            //this.isSelecting = true;
            UpdateSelectionFromTemplate();
            //this.isSelecting = false;
        }

        private void chkSelected_Checked(object sender, RoutedEventArgs e)
        {
            if (isSelecting)
            {
                return;
            }
            //this.isSelecting = true;
            UpdateSelectionFromTemplate();
            //this.isSelecting = false;
        }

        private bool calculationInProgress;
        internal void UpdateSelectionFromTemplate()
        {
            bool recalculationPaused = MarkBet.pauseRecalculation;
            if (MarkBet == null || CurrentRankTemplate == null || CurrentMarkBetTemplateABCD == null || calculationInProgress)
            {
                return;
            }
            //RemoveEventHandlers();
            MarkBet.pauseRecalculation = true;
            calculationInProgress = true;
            try
            {
                if (CurrentMarkBetTemplateABCD.DesiredSystemSize == 0)
                {
                    CurrentMarkBetTemplateABCD.DesiredSystemSize = 500;
                    CurrentMarkBetTemplateABCD.RankTemplate = CurrentRankTemplate;
                    //this.CurrentMarkBetTemplateABCD.RankTemplateName = this.CurrentRankTemplate.Name;
                }
                if (CurrentMarkBetTemplateABCD.RankTemplate == null)
                {
                    CurrentMarkBetTemplateABCD.RankTemplate = CurrentRankTemplate;
                }
                MarkBet.ApplyConfigRankVariables(CurrentRankTemplate);
                MarkBet.MarkBetTemplateABCD = CurrentMarkBetTemplateABCD;
                MarkBet.SelectFromTemplateABCD();

                ucRaceView.FilterSelected();
            }
            catch (Exception exc)
            {
                Config.AddToErrorLog(exc);
            }
            MarkBet.pauseRecalculation = recalculationPaused;
            calculationInProgress = false;
            //AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            lvwRankVariablesTemplate.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            lvwRankVariablesTemplate.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            lvwRankVariablesTemplate.AddHandler(DecimalUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

            gbMarkBetTemplate.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            gbMarkBetTemplate.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            gbMarkBetTemplate.AddHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

        }

        private void RemoveEventHandlers()
        {
            lvwRankVariablesTemplate.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            lvwRankVariablesTemplate.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            lvwRankVariablesTemplate.RemoveHandler(DecimalUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

            gbMarkBetTemplate.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            gbMarkBetTemplate.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            gbMarkBetTemplate.RemoveHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

        }

        private void btnSaveMarkBetABCDTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentMarkBetTemplateABCD.Name))
            {
                CurrentMarkBetTemplateABCD.Name = "Ny mall " + DateTime.Now.ToLongDateString();
            }
            if (CurrentRankTemplate != null)
            {
                btnSaveRankTemplate_Click(btnSaveRankTemplate, e);
                var rankTemplate = Config.RankTemplateList.FirstOrDefault(rt => rt.Name == CurrentRankTemplate.Name);
                CurrentMarkBetTemplateABCD.RankTemplate = rankTemplate;
                CurrentMarkBetTemplateABCD.RankTemplateName = rankTemplate.Name;
            }
            CurrentMarkBetTemplateABCD.TypeCategory = MarkBet.BetType.TypeCategory;
            Config.MarkBetTemplateABCDList.Add(CurrentMarkBetTemplateABCD);
        }

        private void btnSaveMarkBetRankTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentMarkBetTemplateRank.Name))
            {
                CurrentMarkBetTemplateRank.Name = "Ny mall " + DateTime.Now.ToLongDateString();
            }
            CurrentMarkBetTemplateRank.DesiredSystemSize = CurrentMarkBetTemplateABCD.DesiredSystemSize;
            CurrentMarkBetTemplateRank.NumberOfSpikes = CurrentMarkBetTemplateABCD.NumberOfSpikes;
            if (CurrentRankTemplate != null)
            {
                btnSaveRankTemplate_Click(btnSaveRankTemplate, e);
                var rankTemplate = Config.RankTemplateList.FirstOrDefault(rt => rt.Name == CurrentRankTemplate.Name);
                CurrentMarkBetTemplateRank.RankTemplate = rankTemplate;
                CurrentMarkBetTemplateRank.RankTemplateName = rankTemplate.Name;
            }
            CurrentMarkBetTemplateRank.TypeCategory = MarkBet.BetType.TypeCategory;
            Config.MarkBetTemplateRankList.Add(CurrentMarkBetTemplateRank);
        }

        private void btnSaveRankTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRankTemplates.SelectedItem != null)
            {
                var rankTemplate = (HPTRankTemplate)cmbRankTemplates.SelectedItem;
                if (CurrentRankTemplate.Name == rankTemplate.Name)
                {
                    foreach (var horseRankVariable in CurrentRankTemplate.HorseRankVariableList)
                    {
                        var rankVariableToUpdate = rankTemplate.HorseRankVariableList.FirstOrDefault(rv => rv.PropertyName == horseRankVariable.PropertyName);
                        if (rankVariableToUpdate != null)
                        {
                            rankVariableToUpdate.Weight = horseRankVariable.Weight;
                            rankVariableToUpdate.Use = horseRankVariable.Use;
                        }
                    }
                }
                else
                {
                    Config.RankTemplateList.Add(CurrentRankTemplate);
                }
            }
            else if (CurrentRankTemplate != null)
            {
                Config.RankTemplateList.Add(CurrentRankTemplate);
            }
        }
    }
}

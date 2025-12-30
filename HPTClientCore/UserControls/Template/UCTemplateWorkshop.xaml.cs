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
                this.CurrentMarkBetTemplateABCD = new HPTMarkBetTemplateABCD()
                {
                    DesiredSystemSize = 1000,
                    NumberOfSpikes = 1
                };
                this.CurrentMarkBetTemplateABCD.InitializeTemplate(new HPTPrio[] { HPTPrio.A, HPTPrio.B, HPTPrio.C });
                this.CurrentMarkBetTemplateRank = new HPTMarkBetTemplateRank();
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
                this.isSelecting = true;
                try
                {
                    RemoveEventHandlers();
                    var rankTemplate = (HPTRankTemplate)e.AddedItems[0];
                    this.CurrentRankTemplate = rankTemplate.Clone();
                    UpdateSelectionFromTemplate();
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
                //RemoveEventHandlers();
                AddEventHandlers();
                this.isSelecting = false;
            }
        }

        private void RankTemplate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.isSelecting)
            {
                return;
            }
            //this.isSelecting = true;
            UpdateSelectionFromTemplate();
            //this.isSelecting = false;
        }

        private void chkSelected_Checked(object sender, RoutedEventArgs e)
        {
            if (this.isSelecting)
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
            bool recalculationPaused = this.MarkBet.pauseRecalculation;
            if (this.MarkBet == null || this.CurrentRankTemplate == null || this.CurrentMarkBetTemplateABCD == null || this.calculationInProgress)
            {
                return;
            }
            //RemoveEventHandlers();
            this.MarkBet.pauseRecalculation = true;
            this.calculationInProgress = true;
            try
            {
                if (this.CurrentMarkBetTemplateABCD.DesiredSystemSize == 0)
                {
                    this.CurrentMarkBetTemplateABCD.DesiredSystemSize = 500;
                    this.CurrentMarkBetTemplateABCD.RankTemplate = this.CurrentRankTemplate;
                    //this.CurrentMarkBetTemplateABCD.RankTemplateName = this.CurrentRankTemplate.Name;
                }
                if (this.CurrentMarkBetTemplateABCD.RankTemplate == null)
                {
                    this.CurrentMarkBetTemplateABCD.RankTemplate = this.CurrentRankTemplate;
                }
                this.MarkBet.ApplyConfigRankVariables(this.CurrentRankTemplate);
                this.MarkBet.MarkBetTemplateABCD = this.CurrentMarkBetTemplateABCD;
                this.MarkBet.SelectFromTemplateABCD();

                this.ucRaceView.FilterSelected();
            }
            catch (Exception exc)
            {
                this.Config.AddToErrorLog(exc);
            }
            this.MarkBet.pauseRecalculation = recalculationPaused;
            this.calculationInProgress = false;
            //AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            this.lvwRankVariablesTemplate.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.lvwRankVariablesTemplate.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.lvwRankVariablesTemplate.AddHandler(DecimalUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

            this.gbMarkBetTemplate.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.gbMarkBetTemplate.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.gbMarkBetTemplate.AddHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

        }

        private void RemoveEventHandlers()
        {
            this.lvwRankVariablesTemplate.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.lvwRankVariablesTemplate.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.lvwRankVariablesTemplate.RemoveHandler(DecimalUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

            this.gbMarkBetTemplate.RemoveHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.gbMarkBetTemplate.RemoveHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelected_Checked));
            this.gbMarkBetTemplate.RemoveHandler(IntegerUpDown.ValueChangedEvent, new RoutedPropertyChangedEventHandler<object>(RankTemplate_ValueChanged));

        }

        private void btnSaveMarkBetABCDTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.CurrentMarkBetTemplateABCD.Name))
            {
                this.CurrentMarkBetTemplateABCD.Name = "Ny mall " + DateTime.Now.ToLongDateString();
            }
            if (this.CurrentRankTemplate != null)
            {
                btnSaveRankTemplate_Click(this.btnSaveRankTemplate, e);
                var rankTemplate = this.Config.RankTemplateList.FirstOrDefault(rt => rt.Name == this.CurrentRankTemplate.Name);
                this.CurrentMarkBetTemplateABCD.RankTemplate = rankTemplate;
                this.CurrentMarkBetTemplateABCD.RankTemplateName = rankTemplate.Name;
            }
            this.CurrentMarkBetTemplateABCD.TypeCategory = this.MarkBet.BetType.TypeCategory;
            this.Config.MarkBetTemplateABCDList.Add(this.CurrentMarkBetTemplateABCD);
        }

        private void btnSaveMarkBetRankTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.CurrentMarkBetTemplateRank.Name))
            {
                this.CurrentMarkBetTemplateRank.Name = "Ny mall " + DateTime.Now.ToLongDateString();
            }
            this.CurrentMarkBetTemplateRank.DesiredSystemSize = this.CurrentMarkBetTemplateABCD.DesiredSystemSize;
            this.CurrentMarkBetTemplateRank.NumberOfSpikes = this.CurrentMarkBetTemplateABCD.NumberOfSpikes;
            if (this.CurrentRankTemplate != null)
            {
                btnSaveRankTemplate_Click(this.btnSaveRankTemplate, e);
                var rankTemplate = this.Config.RankTemplateList.FirstOrDefault(rt => rt.Name == this.CurrentRankTemplate.Name);
                this.CurrentMarkBetTemplateRank.RankTemplate = rankTemplate;
                this.CurrentMarkBetTemplateRank.RankTemplateName = rankTemplate.Name;
            }
            this.CurrentMarkBetTemplateRank.TypeCategory = this.MarkBet.BetType.TypeCategory;
            this.Config.MarkBetTemplateRankList.Add(this.CurrentMarkBetTemplateRank);
        }

        private void btnSaveRankTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (this.cmbRankTemplates.SelectedItem != null)
            {
                var rankTemplate = (HPTRankTemplate)this.cmbRankTemplates.SelectedItem;
                if (this.CurrentRankTemplate.Name == rankTemplate.Name)
                {
                    foreach (var horseRankVariable in this.CurrentRankTemplate.HorseRankVariableList)
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
                    this.Config.RankTemplateList.Add(this.CurrentRankTemplate);
                }
            }
            else if (this.CurrentRankTemplate != null)
            {
                this.Config.RankTemplateList.Add(this.CurrentRankTemplate);
            }
        }
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HPTClient
{
    /// <summary>
    /// Interaction logic for ATGCalendar.xaml
    /// </summary>
    public partial class ATGCalendar : Window
    {
        public byte[] CalendarZip { get; set; }
        public HPTCalendar hptCalendar { get; set; }
        public ObservableCollection<HPTGUIMessage> LoadingInfoList { get; set; }
        public ObservableCollection<HPTGUIMessage> MessageList { get; set; }
        private FileSystemWatcher ftw;
        private bool isOffline;

        internal List<UCMarksGame> UCMarksGameList = new List<UCMarksGame>();

        public ATGCalendar()
        {
            ValidateAndLoad();
            InitializeComponent();
        }

        private void ValidateAndLoad()
        {
            try
            {
                Exception tempExc = null;

                // Skapa/hämta Config
                try
                {
                    //DateTime dt1 = DateTime.Now;
                    if (this.Config == null)
                    {
                        this.Config = HPTConfig.CreateHPTConfig();
                    }
                    if (tempExc != null)
                    {
                        this.Config.AddToErrorLog(tempExc);
                    }
                }
                catch (Exception configExc)
                {
                    this.Config = new HPTConfig();
                    this.Config.AddToErrorLog(configExc);
                }

                this.LoadingInfoList = new ObservableCollection<HPTGUIMessage>();
                this.MessageList = new ObservableCollection<HPTGUIMessage>();

                DateTime dt1 = DateTime.Now;
                HandleFreeAndPro();
                TimeSpan ts1 = DateTime.Now - dt1;
                string s1 = ts1.TotalMilliseconds.ToString();

                // Inställningar för huvudfönstret
                this.Width = this.Config.ApplicationWidth;
                this.Height = this.Config.ApplicationHeight;
                this.WindowState = this.Config.ApplicationWindowState;
                this.WindowStartupLocation = this.Config.ApplicationStartupLocation;


                // TEST
                this.tmrCalendarViewUpdate = new Timer(HandleCalendarViewUpdate, null, 120000, 120000);
            }
            catch (Exception exc)
            {
                this.Config.AddToErrorLog(exc);
            }
        }

        #region Config

        static void ConfigChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var ac = (ATGCalendar)property;
            ac.Config = (HPTConfig)args.NewValue;
        }

        public HPTConfig Config
        {
            get { return (HPTConfig)GetValue(ConfigProperty); }
            set { SetValue(ConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Config.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(HPTConfig), typeof(ATGCalendar), new UIPropertyMetadata(null, ConfigChangedCallBack));

        #endregion

        internal void HandleFreeAndPro()
        {
            // Autenticera mot servern
            try
            {
                try
                {
                    // Se till att Config verkligen finns
                    if (this.Config == null)
                    {
                        this.Config = HPTConfig.CreateHPTConfig();
                    }
                    //this.Config.PROVersionExpirationDate = DateTime.Today.AddMonths(3);

                    if (this.CalendarZip == null)  // Hämta kalender separat
                    {
                        if (this.hptCalendar == null)
                        {
                            this.hptCalendar = new HPTCalendar();
                        }
                        var serviceConnector = new HPTServiceConnector();
                        this.CalendarZip = serviceConnector.GetCalendar(this.hptCalendar);
                    }

                    // Hantera om vi inte får kalender från servern
                    if (this.CalendarZip == null)  // Hämta kalender från disk
                    {
                        this.hptCalendar = HPTSerializer.DeserializeHPTCalendar(HPTConfig.MyDocumentsPath + "HPTCalendar.hptc");

                        if (this.hptCalendar != null && this.hptCalendar.RaceDayInfoList != null && this.hptCalendar.RaceDayInfoList.Count > 0)
                        {
                            this.hptCalendar.RaceDayInfoList
                                            .Where(rdi => rdi.RaceDayDate.Date >= DateTime.Today)
                                            .ToList()
                                            .ForEach(rdi => rdi.ShowInUI = true);

                            BindingOperations.GetBindingExpression(this.lvwCalenda, ListView.ItemsSourceProperty).UpdateTarget();
                        }
                    }
                    else
                    {
                        this.hptCalendar = new HPTCalendar();
                        ThreadPool.QueueUserWorkItem(new WaitCallback(GetCalendar), ThreadPriority.Normal);
                    }
                }
                catch (System.ServiceModel.EndpointNotFoundException)
                {
                    this.isOffline = true;
                }

                // Hantering av Gratis/PRO
                //this.Config.VersionText = "Hjälp på Traven! 5.34";'
                this.Config.VersionText = $"Hjälp på Traven läggs ner, mer info på www.hpt.nu ({this.Config.PROVersionExpirationDate:yyyy-MM-dd})";
                this.VersionText = this.Config.VersionText;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                this.isOffline = true;
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void GetCalendar()
        {
            try
            {
                HPTServiceToHPTHelper.CreateCalendar(this.CalendarZip, this.hptCalendar);
                BindingOperations.GetBindingExpression(this.lvwCalenda, ListView.ItemsSourceProperty).UpdateTarget();
                HPTSerializer.SerializeHPTCalendar(HPTConfig.MyDocumentsPath + "HPTCalendar.hptc", this.hptCalendar);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void GetCalendar(object timerData)
        {
            try
            {
                Dispatcher.Invoke(GetCalendar);
                HPTServiceToHPTHelper.CreateCalendar(this.CalendarZip, this.hptCalendar);
            }
            catch (Exception exc)
            {
                Dispatcher.Invoke(GetCalendar);
                string s = exc.Message;
            }
        }

        //void ftw_Created(object sender, FileSystemEventArgs e)
        //{
        //    UpdateDirectoriesInBackground();
        //}

        private void UpdateDirectoriesInBackground()
        {
            try
            {
                Dispatcher.Invoke(new Action(HPTConfig.Config.UpdateHPTSystemDirectories), null);
            }
            catch (Exception)
            {
                try
                {
                    HPTConfig.Config.UpdateHPTSystemDirectories();
                }
                catch { }
            }
        }

        private void SetNonSerializeConfigValues()
        {
            try
            {
                Dispatcher.Invoke(new Action(HPTConfig.Config.SetNonSerializedValues), null);
            }
            catch (Exception)
            {
                try
                {
                    HPTConfig.Config.SetNonSerializedValues();
                }
                catch { }
            }
        }

        private void hlBetType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fe = (FrameworkElement)e.OriginalSource;
                var bt = (HPTBetType)fe.DataContext;

                //bt.

                LoadNewTabItem(bt, 0);

            }
            catch (Exception exc)
            {
                Cursor = Cursors.Arrow;
                HPTConfig.Config.AddToErrorLog(exc);
            }
        }

        private void LoadNewTabItem(HPTBetType bt, int raceNumberToLoad)
        {
            try
            {
                Cursor = Cursors.Wait;

                var hptRdi = bt.CalendarRaceDayInfo;
                if (hptRdi == null)
                {
                    hptRdi = new HPTRaceDayInfo()
                    {
                        RaceDayDate = bt.StartTime.Date,
                        TrackId = bt.TrackId
                    };
                }

                switch (bt.Code)
                {
                    case "V3":
                    case "V4":
                    case "V5":
                    case "V65":
                    case "V64":
                    case "V75":
                    case "V86":
                    case "GS75":
                        hptRdi.DataToShow = this.Config.DataToShowVxx;
                        break;
                    case "DD":
                    case "LD":
                        hptRdi.DataToShow = this.Config.DataToShowDD;
                        break;
                    case "TV":
                        hptRdi.DataToShow = this.Config.DataToShowTvilling;
                        break;
                    case "T":
                        hptRdi.DataToShow = this.Config.DataToShowTrio;
                        break;
                    default:
                        break;
                }

                hptRdi.BetType = bt;

                var mess = new HPTGUIMessage()
                {
                    ButtonVisibility = System.Windows.Visibility.Collapsed,
                    Message = "Laddar " + bt.Code + " på " + hptRdi.Trackname + " " + hptRdi.RaceDayDateString,
                    Key = bt.Code + ";" + hptRdi.Trackname + ";" + hptRdi.RaceDayDateString,
                    KeyAlt = bt.Code + ";" + hptRdi.TrackId.ToString() + ";" + hptRdi.RaceDayDate.ToString("yyyy-MM-dd"),
                    RaceDayInfo = hptRdi,
                    RaceNumberToLoad = raceNumberToLoad
                };

                this.LoadingInfoList.Add(mess);

                var connector = new HPTServiceConnector();
                connector.GetRaceDayInfoByTrackAndDate(bt, hptRdi.TrackId, hptRdi.RaceDayDate, ReceiveRaceDayInfo);
                Cursor = Cursors.Arrow;

            }
            catch (Exception exc)
            {
                Cursor = Cursors.Arrow;
                HPTConfig.Config.AddToErrorLog(exc);
            }
        }

        private void ReceiveRaceDayInfo(HPTRaceDayInfo hptRdi)
        {
            if (hptRdi.BetType == null)
            {
                string key = hptRdi.BetTypeString + ";" + hptRdi.TrackId.ToString() + ";" + hptRdi.RaceDayDate.ToString("yyyy-MM-dd");
                for (int i = 0; i < this.LoadingInfoList.Count; i++)
                {
                    HPTGUIMessage mess = this.LoadingInfoList[i];
                    if (key == mess.KeyAlt)
                    {
                        mess.ErrorString = "Hämtning misslyckades.";
                        mess.ButtonVisibility = System.Windows.Visibility.Visible;
                    }
                }
                return;
            }
            string raceDayDirectory = HPTConfig.MyDocumentsPath + hptRdi.ToDateAndTrackString();
            if (!Directory.Exists(raceDayDirectory))
            {
                Directory.CreateDirectory(raceDayDirectory);
            }

            switch (hptRdi.BetType.Code)
            {
                case "V3":
                case "V4":
                case "V5":
                case "V65":
                case "V64":
                case "V75":
                case "GS75":
                case "V86":
                    hptRdi.DataToShow = HPTConfig.Config.DataToShowVxx;
                    HPTMarkBet hmb = new HPTMarkBet(hptRdi, hptRdi.BetType);
                    hmb.SaveDirectory = raceDayDirectory + "\\";
                    Dispatcher.Invoke(new Action<HPTMarkBet>(AddTabItem), hmb);
                    break;
                case "DD":
                case "LD":
                    hptRdi.DataToShow = HPTConfig.Config.DataToShowDD;
                    HPTCombBet hcb = new HPTCombBet(hptRdi, hptRdi.BetType);
                    hcb.SaveDirectory = raceDayDirectory + "\\";
                    Dispatcher.Invoke(new Action<HPTCombBet>(AddTabItem), hcb);
                    break;
                case "TV":
                    hptRdi.DataToShow = HPTConfig.Config.DataToShowTvilling;
                    HPTCombBet hcb2 = new HPTCombBet(hptRdi, hptRdi.BetType);
                    hcb2.SaveDirectory = raceDayDirectory + "\\";
                    Dispatcher.Invoke(new Action<HPTCombBet>(AddTabItem), hcb2);
                    break;
                case "T":
                    hptRdi.DataToShow = HPTConfig.Config.DataToShowTrio;
                    HPTCombBet hcb3 = new HPTCombBet(hptRdi, hptRdi.BetType);
                    hcb3.SaveDirectory = raceDayDirectory + "\\";
                    Dispatcher.Invoke(new Action<HPTCombBet>(AddTabItem), hcb3);
                    break;
                default:
                    break;
            }
        }

        private Timer tmrCalendarViewUpdate;
        private void HandleCalendarViewUpdate(object timerData)
        {
            Dispatcher.Invoke(HandleCalendarViewUpdate);
            //HandleCalendarViewUpdate();
        }

        private void HandleCalendarViewUpdate()
        {
            try
            {
                var todaysBetTypes = this.hptCalendar.RaceDayInfoList
                    .Where(rdi => rdi.RaceDayDate.Date == DateTime.Today)
                    .SelectMany(rdi => rdi.BetTypeList)
                        .ToList();

                todaysBetTypes
                    .ForEach(bt =>
                        {
                            bt.SetCalendarRacaDayInfoBrush();
                        });

                DateTime dtNow = DateTime.Now;
                DateTime nextTimerUpdate = todaysBetTypes.Select(bt => bt.NextTime).OrderBy(nt => nt).FirstOrDefault(nt => nt > dtNow);
                if (nextTimerUpdate != null && nextTimerUpdate > DateTime.Now)
                {
                    TimeSpan ts = nextTimerUpdate - dtNow;
                    this.tmrCalendarViewUpdate.Change(ts, ts);
                }
            }
            catch (Exception exc)
            {
                // Felhantering här...
                string s = exc.Message;
            }
        }

        #region Menu handling

        private ContextMenu AddContextMenuToTabItem(TabItem ti)
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem();
            mi.Header = "Stäng";
            mi.Tag = ti;
            cm.Items.Add(mi);
            mi.Click += new RoutedEventHandler(mi_Click);
            ti.ContextMenu = cm;
            return cm;
        }

        private void AddCrossBetMenuItemsToContextMenu(HPTBet betToApplyTo, ContextMenu cm)
        {
            // Lägg till val för att hämta egen rank/poäng från annan flik
            MenuItem miSetOwnRanksFrom = new MenuItem()
            {
                Header = "Kopiera egen rank/poäng från...",
                Tag = betToApplyTo,
                DataContext = betToApplyTo
            };
            cm.Items.Add(miSetOwnRanksFrom);
            miSetOwnRanksFrom.MouseEnter += (e, o) =>
            {
                var mi = e as MenuItem;
                mi.Items.Clear();
                var bet = mi.DataContext as HPTBet;
                bet.FindBetsWithOverlappingRaces().ToList().ForEach(b =>
                {
                    var miBet = new MenuItem()
                    {
                        Header = b.SystemNameForIdentification,
                        Tag = bet,
                        DataContext = b
                    };
                    miBet.Click += (e2, o2) =>
                    {
                        var fe = o2.OriginalSource as FrameworkElement;
                        var outerBet = fe.Tag as HPTBet;
                        var innerBet = fe.DataContext as HPTBet;
                        outerBet.ApplyOwnRanks(innerBet);

                    };
                    mi.Items.Add(miBet);
                });
            };


            // Lägg till val för att välja hästar från annan flik
            MenuItem miSelectHorsesFrom = new MenuItem()
            {
                Header = "Välj hästar från...",
                Tag = betToApplyTo,
                DataContext = betToApplyTo
            };
            cm.Items.Add(miSelectHorsesFrom);
            miSelectHorsesFrom.MouseEnter += (e, o) =>
            {
                var mi = e as MenuItem;
                mi.Items.Clear();
                var bet = mi.DataContext as HPTBet;
                bet.FindBetsWithOverlappingRaces().ToList().ForEach(b =>
                {
                    var miBet = new MenuItem()
                    {
                        Header = b.SystemNameForIdentification,
                        Tag = bet,
                        DataContext = b
                    };
                    miBet.Click += (e2, o2) =>
                    {
                        var fe = o2.OriginalSource as FrameworkElement;
                        var outerBet = fe.Tag as HPTBet;
                        var innerBet = fe.DataContext as HPTBet;
                        outerBet.ApplySelection(innerBet);

                    };
                    mi.Items.Add(miBet);
                });
            };

        }

        internal void AddTabItem(HPTMarkBet hmb)
        {
            try
            {
                hmb.Config = this.Config;
                hmb.RaceDayInfo.DataToShow = this.Config.DataToShowVxx;
                this.Config.AvailableBets.Add(hmb);

                UCMarksGame ucMarksGame = new UCMarksGame(hmb);
                this.UCMarksGameList.Add(ucMarksGame);
                UCBetTabItemHeader ucItemHeader = new UCBetTabItemHeader();
                TabItem ti = new TabItem()
                {
                    DataContext = hmb,
                    Header = ucItemHeader,
                    Content = ucMarksGame
                };

                ucItemHeader.Tag = ti;
                ucItemHeader.Close += new RoutedEventHandler(ucItemHeader_Close);

                ContextMenu cm = AddContextMenuToTabItem(ti);


                // Lägg till val för kloning
                MenuItem miClone = new MenuItem()
                {
                    Header = "Klona",
                    Tag = hmb
                };
                cm.Items.Add(miClone);
                miClone.Click += new RoutedEventHandler(miClone_Click);


                // Lägg till val för uppladdning av system
                MenuItem miUploadCompleteSystem = new MenuItem()
                {
                    Header = "Ladda upp system",
                    Tag = hmb
                };
                cm.Items.Add(miUploadCompleteSystem);
                miUploadCompleteSystem.Click += miUploadCompleteSystem_Click;

                // Lägg till val för uppladdning av system
                MenuItem miPasteTips = new MenuItem()
                {
                    Header = "Klistra in tips",
                    Tag = ucMarksGame,
                    DataContext = hmb
                };
                cm.Items.Add(miPasteTips);
                miPasteTips.Click += new RoutedEventHandler(miPasteTips_Click);


                // Lägg till val för borttag av rader från fil
                MenuItem miRemoveRowsFromFile = new MenuItem()
                {
                    Header = "Ta bort rader från kupongfil",
                    Tag = ucMarksGame,
                    DataContext = hmb
                };
                cm.Items.Add(miRemoveRowsFromFile);
                miRemoveRowsFromFile.Click += miRemoveRowsFromFile_Click;


                // Lägg till val för borttag av rader från fil
                MenuItem miAddRowsFromFile = new MenuItem()
                {
                    Header = "Lägg till rader från kupongfil",
                    Tag = ucMarksGame,
                    DataContext = hmb
                };
                cm.Items.Add(miAddRowsFromFile);
                miAddRowsFromFile.Click += miAddRowsFromFile_Click;


                // Lägg till val för att beräkna jackpottrisk
                if (hmb.BetType.HasMultiplePools)
                {
                    MenuItem miCalculateJackpotRows = new MenuItem()
                    {
                        Header = "Blir det jackpott?",
                        Tag = ucMarksGame,
                        DataContext = hmb
                    };
                    cm.Items.Add(miCalculateJackpotRows);
                    miCalculateJackpotRows.Click += miCalculateJackpotRows_Click;
                }

                // Lägg till val för att beräkna antalet möjliga ensamma rader
                if (hmb.BetType.HasMultiplePools)
                {
                    MenuItem miCalculateSingleRows = new MenuItem()
                    {
                        Header = "Antal ensamma rader",
                        Tag = ucMarksGame,
                        DataContext = hmb
                    };
                    cm.Items.Add(miCalculateSingleRows);
                    miCalculateSingleRows.Click += MiCalculateSingleRows_Click;
                }

                // Lägg till val för att hämta egen rank/poäng från annan flik eller välja hästar från annan flik
                AddCrossBetMenuItemsToContextMenu(hmb, cm);


                // Välj tillagt TabItem
                int tabItemIndex = this.tcMain.Items.Add(ti);
                this.tcMain.SelectedIndex = tabItemIndex;

                string key = hmb.BetType.Code + ";" + hmb.RaceDayInfo.Trackname + ";" + hmb.RaceDayInfo.RaceDayDateString;
                var mess = this.LoadingInfoList.FirstOrDefault(gm => gm.Key == key);
                if (mess != null)
                {
                    hmb.RaceNumberToLoad = mess.RaceNumberToLoad;
                    this.LoadingInfoList.Remove(mess);
                }
            }
            catch (Exception exc)
            {
                this.Config.AddToErrorLog(exc);
            }
        }

        private void MiCalculateSingleRows_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var ucMarksGame = (UCMarksGame)mi.Tag;
            ucMarksGame.CalculateNumberOfSingleRows();
        }

        void miCalculateJackpotRows_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var ucMarksGame = (UCMarksGame)mi.Tag;
            ucMarksGame.CalculateJackpotRisk();
        }

        void miAddRowsFromFile_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var ucMarksGame = (UCMarksGame)mi.Tag;
            ucMarksGame.AddRowsFromFile();
        }

        void miRemoveRowsFromFile_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var ucMarksGame = (UCMarksGame)mi.Tag;
            ucMarksGame.RemoveRowsFromFile();
        }

        void miPasteTips_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            HPTMarkBet hmb = (HPTMarkBet)mi.DataContext;
            var uc = (UCMarksGame)mi.Tag;
            string tips = Clipboard.GetText();
            if (hmb.ParseTips(tips))
            {
                uc.ShowTipsWindow();
            }
        }

        void miUploadCompleteSystem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            HPTMarkBet hmb = (HPTMarkBet)mi.Tag;
            hmb.SetSerializerValues();
            if (hmb != null)
            {
                var wndUploadSystem = new Window()
                {
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Title = "Ladda upp system!",
                    ShowInTaskbar = false,
                    ResizeMode = ResizeMode.NoResize,
                    Owner = App.Current.MainWindow
                };
                wndUploadSystem.Content = new UCUserSystemUpload()
                {
                    DataContext = hmb,
                    MarkBet = hmb
                };
                wndUploadSystem.ShowDialog();
            }
        }

        void ucItemHeader_Close(object sender, RoutedEventArgs e)
        {
            UCBetTabItemHeader ucItemHeader = (UCBetTabItemHeader)e.Source;
            this.tcMain.Items.Remove(ucItemHeader.Tag);
        }

        void ucGenericItemHeader_Close(object sender, RoutedEventArgs e)
        {
            UCGenericTabItemHeader ucItemHeader = (UCGenericTabItemHeader)e.Source;
            this.tcMain.Items.Remove(ucItemHeader.Tag);
        }

        void miClone_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            HPTMarkBet hmb = (HPTMarkBet)mi.Tag;
            HPTMarkBet hmbClone = hmb.Clone();
            hmbClone.SetSerializerValues();
            if (hmbClone != null)
            {
                AddTabItem(hmbClone);
            }
        }

        internal void AddTabItem(HPTCombBet hcb)
        {
            try
            {
                hcb.Config = this.Config;
                this.Config.AvailableBets.Add(hcb);

                UserControl uc = null;

                switch (hcb.BetType.Code)
                {
                    case "DD":
                    case "LD":
                        hcb.RaceDayInfo.DataToShow = this.Config.DataToShowDD;
                        hcb.DataToShow = this.Config.CombinationDataToShowDouble;
                        uc = new UCDoubleGame(hcb);
                        break;
                    case "TV":
                        hcb.RaceDayInfo.DataToShow = this.Config.DataToShowTvilling;
                        hcb.DataToShow = this.Config.CombinationDataToShowTvilling;
                        uc = new UCTvillingGame(hcb);
                        break;
                    case "T":
                        hcb.RaceDayInfo.DataToShow = this.Config.DataToShowTrio;
                        hcb.DataToShow = this.Config.CombinationDataToShowTrio;
                        uc = new UCTrioGame(hcb);
                        break;
                    default:
                        break;
                }

                var ucItemHeader = new UCBetTabItemHeader();
                TabItem ti = new TabItem()
                {
                    DataContext = hcb,
                    Header = ucItemHeader,
                    Content = uc
                };

                ucItemHeader.Tag = ti;
                ucItemHeader.Close += new RoutedEventHandler(ucItemHeader_Close);

                // Lägg till menuitem för stängning
                AddContextMenuToTabItem(ti);

                // Lägg till val för att hämta egen rank/poäng från annan flik eller välja hästar från annan flik
                AddCrossBetMenuItemsToContextMenu(hcb, ti.ContextMenu);

                int tabItemIndex = this.tcMain.Items.Add(ti);
                this.tcMain.SelectedIndex = tabItemIndex;

                string key = hcb.BetType.Code + ";" + hcb.RaceDayInfo.Trackname + ";" + hcb.RaceDayInfo.RaceDayDateString;
                for (int i = 0; i < this.LoadingInfoList.Count; i++)
                {
                    HPTGUIMessage mess = this.LoadingInfoList[i];
                    if (key == mess.Key)
                    {
                        hcb.RaceNumberToLoad = mess.RaceNumberToLoad;
                        this.LoadingInfoList.RemoveAt(i);
                        i--;
                    }
                }
            }
            catch (Exception exc)
            {
                string fel = exc.Message;
                string key = hcb.BetType.Code + ";" + hcb.RaceDayInfo.Trackname + ";" + hcb.RaceDayInfo.RaceDayDateString;
                for (int i = 0; i < this.LoadingInfoList.Count; i++)
                {
                    HPTGUIMessage mess = this.LoadingInfoList[i];
                    if (key == mess.Key)
                    {
                        this.LoadingInfoList.RemoveAt(i);
                        i--;
                    }
                }
                this.Config.AddToErrorLog(exc);
            }
        }

        void mi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem mi = (MenuItem)sender;
                TabItem ti = (TabItem)mi.Tag;
                ti.Content = null;
                this.tcMain.Items.Remove(mi.Tag);
                GC.Collect();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
                GC.Collect();
            }
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdOpenHPT = new OpenFileDialog();
            ofdOpenHPT.InitialDirectory = HPTConfig.MyDocumentsPath;
            ofdOpenHPT.Filter = "Hjälp på traven-system|*.hpt4;*.hpt5";
            ofdOpenHPT.FileOk += new System.ComponentModel.CancelEventHandler(ofdOpenHPT_FileOk);
            ofdOpenHPT.ShowDialog();
        }

        void ofdOpenHPT_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    OpenFileDialog ofd = (OpenFileDialog)sender;

                    if (ofd.SafeFileName.StartsWith("DD_")
                            || ofd.SafeFileName.StartsWith("LD_")
                            || ofd.SafeFileName.ToUpper().StartsWith("TV_")
                            || ofd.SafeFileName.ToUpper().StartsWith("T_"))
                    {
                        try
                        {
                            HPTCombBet hcb = HPTSerializer.DeserializeHPTCombinationSystem(ofd.FileName);
                            AddTabItem(hcb);
                        }
                        catch (Exception)
                        {
                            HPTMarkBet hmb = HPTSerializer.DeserializeHPTSystem(ofd.FileName);
                            hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                            AddTabItem(hmb);
                        }
                    }
                    else
                    {
                        try
                        {
                            HPTMarkBet hmb = HPTSerializer.DeserializeHPTSystem(ofd.FileName);
                            hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                            AddTabItem(hmb);
                        }
                        catch (Exception)
                        {
                            HPTCombBet hcb = HPTSerializer.DeserializeHPTCombinationSystem(ofd.FileName);
                            AddTabItem(hcb);
                        }
                    }
                }
                catch (Exception exc)
                {
                    this.Config.AddToErrorLog(exc);
                }
            }
        }


        #endregion

        private void miShutDown_Click(object sender, RoutedEventArgs e)
        {
            //this.Config.ApplicationHeight = this.ActualHeight;
            //this.Config.ApplicationWidth = this.ActualWidth;
            //this.Config.SaveConfig();
            Application.Current.Shutdown();
        }

        private void miUpdateAndRestart_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "http://download.hpt.nu");
            //System.Diagnostics.Process.Start("iexplore.exe", "www.dn.se");
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Spara config
            try
            {
                this.Config.ApplicationHeight = this.ActualHeight;
                this.Config.ApplicationWidth = this.ActualWidth;
                this.Config.ApplicationStartupLocation = this.WindowStartupLocation;
                this.Config.ApplicationWindowState = this.WindowState;

                this.Config.SaveConfig();
                //this.Config.SaveLogFile();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            // Spara egen info
            try
            {
                this.Config.HorseOwnInformationCollection.SaveHorseOwnInformationList();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }

            // Ta bort temporära filer
            try
            {
                Directory.Delete(HPTConfig.TempPath);
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && this.IsVisible)
            {
                try
                {
                    // Visa bara rätt uppsättning tävlingar
                    HandleCalendarFilter();

                    // Uppdatera kataloger
                    UpdateDirectoriesInBackground();

                    // Uppdatera värden som inte sparas i konfigurationen
                    SetNonSerializeConfigValues();

                    // Filnamn på kommandoraden
                    if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Any())
                    {
                        string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
                        var fileName = new Uri(activationData[0]);
                        try
                        {
                            HPTMarkBet hmb = HPTSerializer.DeserializeHPTSystem(fileName.LocalPath);
                            hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                            AddTabItem(hmb);
                        }
                        catch (Exception)
                        {
                            HPTCombBet hcb = HPTSerializer.DeserializeHPTCombinationSystem(fileName.LocalPath);
                            AddTabItem(hcb);
                        }
                    }
                }
                catch (Exception exc)
                {
                    this.Config.AddToErrorLog(exc);
                }
            }
        }

        void miFile_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            MenuItem item = (MenuItem)e.OriginalSource;
            if (item.DataContext.GetType() != typeof(HPTSystemFile))
            {
                Cursor = Cursors.Arrow;
                return;
            }
            HPTSystemFile sysFile = (HPTSystemFile)item.DataContext;
            try
            {
                if (sysFile.FileType == "hpt5" || sysFile.FileType == "hpt4")
                {
                    if (sysFile.FileNameShort.ToUpper().StartsWith("V4_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("V5_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("V64_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("V65_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("V75_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("V86_"))
                    {
                        HPTMarkBet hmb = HPTSerializer.DeserializeHPTSystem(sysFile.FileName);
                        hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                        AddTabItem(hmb);
                    }
                    else if (sysFile.FileNameShort.ToUpper().StartsWith("DD_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("LD_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("TV_")
                        || sysFile.FileNameShort.ToUpper().StartsWith("T_"))
                    {
                        HPTCombBet hcb = HPTSerializer.DeserializeHPTCombinationSystem(sysFile.FileName);
                        AddTabItem(hcb);
                    }
                    else
                    {
                        try
                        {
                            HPTMarkBet hmb = HPTSerializer.DeserializeHPTSystem(sysFile.FileName);
                            hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                            AddTabItem(hmb);
                        }
                        catch (InvalidOperationException)
                        {
                            HPTCombBet hcb = HPTSerializer.DeserializeHPTCombinationSystem(sysFile.FileName);
                            hcb.SaveDirectory = HPTConfig.MyDocumentsPath + hcb.RaceDayInfo.ToDateAndTrackString() + "\\";
                            AddTabItem(hcb);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                try
                {
                    HPTMarkBet hmb = HPTSerializer.DeserializeHPTSystem(sysFile.FileName);
                    hmb.SaveDirectory = HPTConfig.MyDocumentsPath + hmb.RaceDayInfo.ToDateAndTrackString() + "\\";
                    AddTabItem(hmb);
                }
                catch (InvalidOperationException)
                {
                    HPTCombBet hcb = HPTSerializer.DeserializeHPTCombinationSystem(sysFile.FileName);
                    hcb.SaveDirectory = HPTConfig.MyDocumentsPath + hcb.RaceDayInfo.ToDateAndTrackString() + "\\";
                    AddTabItem(hcb);
                }
            }
            catch (Exception exc)
            {
                this.Config.AddToErrorLog(exc);
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }

        private void miUpdateCalendar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                var serviceConnector = new HPTServiceConnector();
                if (this.hptCalendar.RaceDayInfoList != null)
                {
                    this.hptCalendar.RaceDayInfoList.Clear();
                }
                ThreadPool.QueueUserWorkItem(new WaitCallback(GetCalendar), ThreadPriority.Normal);
                //serviceConnector.GetCalendar(this.hptCalendar);
            }
            catch (Exception exc)
            {
                this.Config.AddToErrorLog(exc);
            }
            Cursor = Cursors.Arrow;
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            HPTAboutBox aboutBox = new HPTAboutBox();
            aboutBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutBox.ShowDialog();
        }

        //private void btnDownload_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        //System.Diagnostics.Process.Start(this.response.DownloadURL);
        //        this.MessageList.RemoveAt(0);
        //    }
        //    catch (Exception exc)
        //    {
        //        this.Config.AddToErrorLog(exc);
        //    }
        //}

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.MessageList.RemoveAt(0);
            }
            catch (Exception exc)
            {
                this.Config.AddToErrorLog(exc);
            }
        }

        private void btnCloseFailedLoad_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            HPTGUIMessage mess = (HPTGUIMessage)btn.DataContext;
            this.LoadingInfoList.Remove(mess);
        }

        #region Hantering av menyval under Inställningar

        internal void AddTabItem(UserControl uc, string textSmall, string textBig, string imageUri, object dataContext)
        {
            var ucItemHeader = new UCGenericTabItemHeader()
            {
                TextBig = textBig,
                TextSmall = textSmall,
                Image = new BitmapImage(new Uri(imageUri, UriKind.Relative))
            };

            TabItem ti = new TabItem()
            {
                Header = ucItemHeader,
                Content = uc
            };

            if (dataContext != null)
            {
                ti.DataContext = dataContext;
            }

            ucItemHeader.Tag = ti;
            ucItemHeader.Close += new RoutedEventHandler(ucGenericItemHeader_Close);

            AddContextMenuToTabItem(ti);
            this.tcMain.Items.Add(ti);
            ti.IsSelected = true;
        }

        private void miTemplates_Click(object sender, RoutedEventArgs e)
        {
            AddTabItem(new UCTemplateMain(), string.Empty, "Mallar", "/Icons/propertiesoroptions.ico", this.Config);
        }

        //private void miSettingsAll_Click(object sender, RoutedEventArgs e)
        //{
        //    AddTabItem(new UCSettings(), string.Empty, "Inställningar", "/Icons/propertiesoroptions.ico", this.Config);
        //}

        private void miErrorLog_Click(object sender, RoutedEventArgs e)
        {
            AddTabItem(new UCErrorLog(), string.Empty, "Fellogg", "/Icons/error.ico", this.Config);
        }

        #endregion

        #region Hantering av menyalternativ under Hjälp-menyn

        //private string ManualURL
        //{
        //    get
        //    {
        //        return HPTConfig.MyDocumentsPath + "HPT53Manual.pdf";
        //    }
        //}

        #endregion

        #region Dependency properties för vad som ska visas i forma av meddelanden

        //public Visibility TryPROVisibility
        //{
        //    get { return (Visibility)GetValue(TryPROVisibilityProperty); }
        //    set { SetValue(TryPROVisibilityProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for TryPROVisibility.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TryPROVisibilityProperty =
        //    DependencyProperty.Register("TryPROVisibility", typeof(Visibility), typeof(ATGCalendar), new UIPropertyMetadata(Visibility.Collapsed));


        //public Visibility UpgradeToPROVisibility
        //{
        //    get { return (Visibility)GetValue(UpgradeToPROVisibilityProperty); }
        //    set { SetValue(UpgradeToPROVisibilityProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for UpgradeToPROVisibility.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty UpgradeToPROVisibilityProperty =
        //    DependencyProperty.Register("UpgradeToPROVisibility", typeof(Visibility), typeof(ATGCalendar), new UIPropertyMetadata(Visibility.Collapsed));


        //public Visibility NewsVisibility
        //{
        //    get { return (Visibility)GetValue(NewsVisibilityProperty); }
        //    set { SetValue(NewsVisibilityProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for NewsVisibility.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty NewsVisibilityProperty =
        //    DependencyProperty.Register("NewsVisibility", typeof(Visibility), typeof(ATGCalendar), new UIPropertyMetadata(Visibility.Collapsed));


        //public Visibility LicenseInformationVisibility
        //{
        //    get { return (Visibility)GetValue(LicenseInformationVisibilityProperty); }
        //    set { SetValue(LicenseInformationVisibilityProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for UpgradeToPROVisibility.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty LicenseInformationVisibilityProperty =
        //    DependencyProperty.Register("LicenseInformationVisibility", typeof(Visibility), typeof(ATGCalendar), new UIPropertyMetadata(Visibility.Collapsed));


        // Rubrik på senaste nyheten
        //public string LatestNewsHeadline { get; set; }
        //public string LatestNewsHeadline
        //{
        //    get { return (string)GetValue(LatestNewsHeadlineProperty); }
        //    set { SetValue(LatestNewsHeadlineProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for LatestNewsHeadline.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty LatestNewsHeadlineProperty =
        //    DependencyProperty.Register("LatestNewsHeadline", typeof(string), typeof(ATGCalendar), new PropertyMetadata(string.Empty));



        public string VersionText
        {
            get { return (string)GetValue(VersionTextProperty); }
            set { SetValue(VersionTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VersionText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionTextProperty =
            DependencyProperty.Register("VersionText", typeof(string), typeof(ATGCalendar), new UIPropertyMetadata(string.Empty));

        #endregion

        private void miResetConfiguration_Click(object sender, RoutedEventArgs e)
        {
            string eMailAddress = this.Config.EMailAddress;
            string password = this.Config.Password;
            this.Config = HPTConfig.ResetHPTConfig();
            this.Config.EMailAddress = eMailAddress;
            this.Config.Password = password;
            HandleFreeAndPro();
        }


        private void miSettingsVxx_Click(object sender, RoutedEventArgs e)
        {
            AddTabItem(new UCSettings(), string.Empty, "Streckspel", "/Icons/propertiesoroptions.ico", this.Config);
        }

        private void miSettingsCombination_Click(object sender, RoutedEventArgs e)
        {
            AddTabItem(new UCSettingsCombination(), string.Empty, "Kombinationsspel", "/Icons/propertiesoroptions.ico", this.Config);
        }

        private void atgCalendar_Initialized(object sender, EventArgs e)
        {

        }

        //private void miSelectProfile_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Cursor = Cursors.Wait;
        //        var fe = (FrameworkElement)sender;
        //        var profile = (GUIProfile)Convert.ToInt32(fe.Tag);

        //        // Sätt kolumner och flikar till vald profil
        //        HPTConfig.Config.SetMarkBetProfile(profile);

        //        //// Gränssnittselement
        //        //var elementsToShow = HPTConfig.Config.GetElementsToShow(profile);
        //        //HPTConfig.Config.GUIElementsToShow = elementsToShow;

        //        foreach (var uc in this.UCMarksGameList)
        //        {
        //            uc.ApplyGUIElementsToShow(HPTConfig.Config.GUIElementsToShow);
        //            uc.ApplyProfile(profile);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //    Cursor = Cursors.Arrow;
        //}

        //private void miResetProfiles_Click(object sender, RoutedEventArgs e)
        //{
        //    HPTConfig.Config.HandleDataToShow();
        //}

        #region Hämta historiska

        internal void GetOldRaceDayInfos()
        {
            string raceDayInfoList = Clipboard.GetText();
            var sr = new StringReader(raceDayInfoList);
            var sb = new StringBuilder();
            while (sr.Peek() != -1)
            {
                try
                {
                    //var raceDayData = sr.ReadLine().Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);
                    var raceDayData = sr.ReadLine().Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append(raceDayData[0]);
                    sb.Append("\t");
                    sb.Append(raceDayData[1]);
                    sb.Append("\t");
                    sb.Append(raceDayData[2]);
                    sb.Append("\t");
                    var connector = new HPTServiceConnector();
                    var hptRdi = connector.GetRaceDayInfoByTrackAndDate(raceDayData[0], raceDayData[1], raceDayData[2]);
                    connector.GetResultMarkingBetByTrackAndDate(raceDayData[0], raceDayData[1], raceDayData[2], hptRdi);
                    var markBet = new HPTMarkBet(hptRdi, hptRdi.BetType);
                    markBet.PrepareForSave();
                    markBet.SuggestNextTimers();

                    //// Faktisk utdelning
                    //foreach (var payOut in hptRdi.PayOutList)
                    //{
                    //    sb.Append(payOut.PayOutAmount);
                    //    sb.Append("\t");
                    //}

                    //// Simulerad utdelning                    
                    //var horseListToSelect = new List<HPTHorse>();
                    //bool splitVictory = false;

                    //hptRdi.RaceList.ForEach(r =>
                    //    {
                    //        if (r.LegResult.Winners.Length > 1)
                    //        {
                    //            splitVictory = true;
                    //        }
                    //        var horse = r.HorseList.First(h => h.StartNr == r.LegResult.Winners[0]);
                    //        horseListToSelect.Add(horse);
                    //    });

                    //hptRdi.PayOutList[0].PayOutAmount = markBet.CouponCorrector.CalculatePayOutAlt(horseListToSelect, hptRdi.BetType.PoolShare * hptRdi.BetType.RowCost);

                    //foreach (var payOut in hptRdi.PayOutList)
                    //{
                    //    sb.Append(payOut.PayOutAmount);
                    //    sb.Append("\t");
                    //}
                    //if (splitVictory)
                    //{
                    //    sb.Append("DÖTT LOPP");
                    //}

                    string fileName = @"c:\Temp\GamlaSystem\" + raceDayData[1] + "_" + markBet.RaceDayInfo.TracknameFile + "_" + raceDayData[0] + ".hpt5";
                    HPTSerializer.SerializeHPTSystem(fileName, markBet);
                }
                catch (Exception exc)
                {
                    string s = exc.Message;
                }
                sb.AppendLine();
            }
            string completeResult = sb.ToString();
            Clipboard.SetDataObject(completeResult);
        }

        internal void CreateResultData(string directory)
        {
            var sbMarkBetStatistics = new StringBuilder();
            var sbRaceStatistics = new StringBuilder();

            Directory.GetFiles(directory, "*V75*.hpt5")
                .ToList()
                .ForEach(f =>
                {
                    try
                    {
                        // Deserialisera
                        var hmb = HPTSerializer.DeserializeHPTSystem(f);
                        hmb.RecalculateAllRanks();
                        var firstLeg = hmb.RaceDayInfo.RaceList.First();

                        sbMarkBetStatistics.Append(hmb.BetType.Code);
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd"));
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(firstLeg.PostTime.ToString("HH:mm"));
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.TrackId);
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.Trackname);
                        sbMarkBetStatistics.Append("\t");

                        if (hmb.RaceDayInfo.PayOutList == null || hmb.RaceDayInfo.PayOutList.Count == 0 || hmb.RaceDayInfo.PayOutList.Sum(po => po.PayOutAmount) == 0)
                        {
                            var serviceConnector = new HPTServiceConnector();
                            serviceConnector.GetResultMarkingBetByTrackAndDate(hmb.BetType.Code, hmb.RaceDayInfo.TrackId, hmb.RaceDayInfo.RaceDayDate, hmb.RaceDayInfo, true);
                        }

                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.PayOutList[0].PayOutAmount);
                        sbMarkBetStatistics.Append("\t");

                        // Utdelning på streckspel med minst två vinstpooler
                        if (hmb.RaceDayInfo.PayOutListATG.Count > 1)
                        {
                            sbMarkBetStatistics.Append(hmb.RaceDayInfo.PayOutList[1].PayOutAmount);
                            sbMarkBetStatistics.Append("\t");
                        }
                        else
                        {
                            sbMarkBetStatistics.Append("0\t");
                        }

                        // Utdelning på streckspel med tre vinstpooler
                        if (hmb.RaceDayInfo.PayOutListATG.Count > 2)
                        {
                            sbMarkBetStatistics.Append(hmb.RaceDayInfo.PayOutList[2].PayOutAmount);
                            sbMarkBetStatistics.Append("\t");
                            sbMarkBetStatistics.Append(hmb.RaceDayInfo.PayOutList[2].PayOutAmount == 0 && hmb.RaceDayInfo.PayOutList[0].PayOutAmount > 0);
                            sbMarkBetStatistics.Append("\t");

                            // JACKPOTTBERÄKNINGAR
                            hmb.CalculateJackpotRows();
                            sbMarkBetStatistics.Append(hmb.JackpotProbability);
                            sbMarkBetStatistics.Append("\t");
                            sbMarkBetStatistics.Append(hmb.numberOfRowsUnderRowValue[1]);
                            sbMarkBetStatistics.Append("\t");
                            sbMarkBetStatistics.Append(hmb.numberOfRowsUnderRowValue[2]);
                        }
                        else
                        {
                            sbMarkBetStatistics.Append("0\t");
                        }
                        sbMarkBetStatistics.AppendLine();

                        // Lägg till info om själva spelformen
                        string markBetInfo = hmb.BetType.Code + "\t"
                            + hmb.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd") + "\t"
                            + hmb.RaceDayInfo.TrackId.ToString() + "\t"
                            + hmb.RaceDayInfo.Trackname + "\t";

                        // Gå igenom resultatet
                        hmb.RaceDayInfo.RaceList.ForEach(r =>
                            {
                                r.LegResult.Winners
                                    .ToList()
                                    .ForEach(w =>
                                    {
                                        var horse = r.HorseList.First(h => h.StartNr == w);
                                        sbRaceStatistics.Append(markBetInfo);
                                        AppendHorseStatistics(sbRaceStatistics, horse);
                                        sbRaceStatistics.Append(@"\t");

                                    });
                            });
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                });
            string markBetStatistics = sbMarkBetStatistics.ToString();
            Clipboard.SetDataObject(markBetStatistics);
            string raceStatistics = sbRaceStatistics.ToString();
            Clipboard.SetDataObject(raceStatistics);
        }

        internal void AppendHorseStatistics(StringBuilder sb, HPTHorse horse)
        {
            sb.Append(horse.ParentRace.PostTime.ToString("HH:mm"));
            sb.Append("\t");
            sb.Append(horse.ParentRace.LegNr);
            sb.Append("\t");
            sb.Append(horse.ParentRace.RaceNr);
            sb.Append("\t");
            sb.Append(horse.StartNr);
            sb.Append("\t");
            sb.Append(horse.StakeDistributionShare);
            sb.Append("\t");
            sb.Append(horse.RankList.First(r => r.Name == "StakeDistributionShare").Rank);
            sb.Append("\t");
            sb.Append(horse.VinnarOddsShare);
            sb.Append("\t");
            sb.Append(horse.RankList.First(r => r.Name == "VinnarOdds").Rank);
            sb.Append("\t");
            sb.Append(horse.PlatsOddsShare);
            sb.Append("\t");
            sb.Append(horse.RankList.First(r => r.Name == "MaxPlatsOdds").Rank);
            sb.AppendLine();
            //sb.Append("\t");
        }

        internal void InsertResultData(string directory)
        {
            var sbMarkBetStatistics = new StringBuilder();
            var sbRaceStatistics = new StringBuilder();

            Directory.GetFiles(directory, "*V64*.hpt5")
                .ToList()
                .ForEach(f =>
                {
                    try
                    {
                        // Deserialisera
                        var hmb = HPTSerializer.DeserializeHPTSystem(f);
                        var firstLeg = hmb.RaceDayInfo.RaceList.First();

                        sbMarkBetStatistics.Append(hmb.BetType.Code);
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd"));
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(firstLeg.PostTime.ToString("HH:mm"));
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.TrackId);
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.Trackname);
                        sbMarkBetStatistics.Append("\t");
                        sbMarkBetStatistics.Append(hmb.RaceDayInfo.PayOutListATG[0].PayOutAmount);
                        sbMarkBetStatistics.Append("\t");

                        if (hmb.RaceDayInfo.PayOutListATG == null || hmb.RaceDayInfo.PayOutListATG.Count == 0 || hmb.RaceDayInfo.PayOutListATG.Sum(po => po.PayOutAmount) == 0)
                        {
                            var serviceConnector = new HPTServiceConnector();
                            serviceConnector.GetResultMarkingBetByTrackAndDate(hmb.BetType.Code, hmb.RaceDayInfo.TrackId, hmb.RaceDayInfo.RaceDayDate, hmb.RaceDayInfo, true);
                        }

                        // Utdelning på streckspel med minst två vinstpooler
                        if (hmb.RaceDayInfo.PayOutListATG.Count > 1)
                        {
                            sbMarkBetStatistics.Append(hmb.RaceDayInfo.PayOutListATG[1].PayOutAmount);
                            sbMarkBetStatistics.Append("\t");
                        }
                        else
                        {
                            sbMarkBetStatistics.Append("0\t");
                        }

                        // Utdelning på streckspel med tre vinstpooler
                        if (hmb.RaceDayInfo.PayOutListATG.Count > 2)
                        {
                            sbMarkBetStatistics.Append(hmb.RaceDayInfo.PayOutListATG[2].PayOutAmount);
                            sbMarkBetStatistics.Append("\t");
                        }
                        else
                        {
                            sbMarkBetStatistics.Append("0\t");
                        }
                        sbMarkBetStatistics.AppendLine();

                        // Lägg till info om själva spelformen
                        string markBetInfo = hmb.BetType.Code + "\t"
                            + hmb.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd") + "\t"
                            + hmb.RaceDayInfo.TrackId.ToString() + "\t"
                            + hmb.RaceDayInfo.Trackname + "\t";

                        // Gå igenom resultatet
                        hmb.RaceDayInfo.RaceList.ForEach(r =>
                        {
                            r.LegResult.Winners
                                .ToList()
                                .ForEach(w =>
                                {
                                    var horse = r.HorseList.First(h => h.StartNr == w);
                                    sbRaceStatistics.Append(markBetInfo);
                                    AppendHorseStatistics(sbRaceStatistics, horse);
                                });
                        });
                    }
                    catch (Exception exc)
                    {
                        string s = exc.Message;
                    }
                });
            string markBetStatistics = sbMarkBetStatistics.ToString();
            Clipboard.SetDataObject(markBetStatistics);
            string raceStatistics = sbRaceStatistics.ToString();
            Clipboard.SetDataObject(raceStatistics);
        }

        private void miGetOldMarkbets_Click(object sender, RoutedEventArgs e)
        {
            GetOldRaceDayInfos();
        }

        #endregion


        //private void miThreeMonthsSubscription_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        System.Diagnostics.Process.Start("https://www.payson.se/myaccount/pay?De=Tre+m%e5naders+%27Hj%e4lp+p%e5+traven+PRO%27&Se=hjalp.pa.traven%40gmail.com&Cost=99%2c00&Currency=SEK&Sp=1&Lang=SE");
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //}

        //private void miOneYearSubscription_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        System.Diagnostics.Process.Start("https://www.payson.se/myaccount/pay?De=Ett+%e5rs+%27Hj%e4lp+p%e5+traven+PRO%27&Se=hjalp.pa.traven%40gmail.com&Cost=299%2c00&Currency=SEK&Sp=1&Lang=SE");
        //    }
        //    catch (Exception exc)
        //    {
        //        string s = exc.Message;
        //    }
        //}

        private void HandleCalendarFilter()
        {
            var collectionView = CollectionViewSource.GetDefaultView(this.lvwCalenda.ItemsSource);
            collectionView.Filter = new Predicate<object>(FilterCalendar);
        }

        public bool FilterCalendar(object obj)
        {
            var rdi = obj as HPTRaceDayInfo;
            return rdi.ShowInUI;
        }

        private void chkShowOnlySwedishTracks_Checked(object sender, RoutedEventArgs e)
        {
            SetRaceDayInfosToShow();
        }

        private void SetRaceDayInfosToShow()
        {
            bool showYesterday = (bool)this.chkShowYesterday.IsChecked;
            bool showOld = (bool)this.chkShowPreviousWeek.IsChecked;
            bool showOnlySwedish = (bool)this.chkShowOnlySwedishTracks.IsChecked;
            bool showOnlyWithMarksGame = (bool)this.chkShowOnlyWithMarksGame.IsChecked;

            this.hptCalendar.RaceDayInfoList.ToList().ForEach(rdi =>
            {
                bool showInUI = false;
                if (showYesterday && !showOld)
                {
                    showInUI = rdi.RaceDayDate.Date >= DateTime.Today.AddDays(-1D);
                }
                else if (showOld)
                {
                    showInUI = true;
                }
                else
                {
                    showInUI = rdi.RaceDayDate.Date >= DateTime.Today;
                }

                if (showInUI)
                {
                    if (showOnlySwedish && !rdi.IsSwedishTrack)
                    {
                        showInUI = false;
                    }
                    else if (showOnlyWithMarksGame && !rdi.HasMarksGame)
                    {
                        showInUI = false;
                    }
                }

                rdi.ShowInUI = showInUI;
            });
            HandleCalendarFilter();
        }

        private void UCOwnInformationView_RaceDayInfoSelected(int trackId, DateTime raceDate, string betType, int legNr)
        {
            // Hitta rätt RAceDayInfo i kalendern
            var raceDayInfo = this.hptCalendar.RaceDayInfoList.FirstOrDefault(rdi => rdi.RaceDayDate.Date == raceDate.Date && rdi.TrackId == trackId);
            if (raceDayInfo != null)
            {
                // Hitta rätt spelform i RaceDayInfo-objektet
                var betTypeToLoad = raceDayInfo.BetTypeList.FirstOrDefault(bt => bt.Code == betType);
                if (betTypeToLoad != null)
                {
                    LoadNewTabItem(betTypeToLoad, legNr);
                }
            }
        }

        #region Simulera olika speltyper

        private void miSimulateV4_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //    {
            //        Code = "V4",
            //        Name = "V4",
            //        RaceNumberList = new int[]{1, 2, 3, 4}
            //    };
            //HPTMarkBet markBet = HPTMarkBet.CreateDummyMarkBet(bt);
            //AddTabItem(markBet);
        }

        private void miSimulateV5_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //{
            //    Code = "V5",
            //    Name = "V5",
            //    RaceNumberList = new int[] { 1, 2, 3, 4, 5 }
            //};
            //HPTMarkBet markBet = HPTMarkBet.CreateDummyMarkBet(bt);
            //AddTabItem(markBet);
        }

        private void miSimulateV6X_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //{
            //    Code = "V65",
            //    Name = "V65",
            //    RaceNumberList = new int[] { 1, 2, 3, 4, 5, 6 }
            //};
            //HPTMarkBet markBet = HPTMarkBet.CreateDummyMarkBet(bt);
            //AddTabItem(markBet);
        }

        private void miSimulateV75_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //{
            //    Code = "V75",
            //    Name = "V75",
            //    RaceNumberList = new int[] { 1, 2, 3, 4, 5, 6, 7 }
            //};
            //HPTMarkBet markBet = HPTMarkBet.CreateDummyMarkBet(bt);
            //AddTabItem(markBet);
        }

        private void miSimulateV86_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //{
            //    Code = "V86",
            //    Name = "V86",
            //    RaceNumberList = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }
            //};
            //HPTMarkBet markBet = HPTMarkBet.CreateDummyMarkBet(bt);
            //AddTabItem(markBet);
        }

        private void miSimulateTrio_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //{
            //    Code = "T",
            //    Name = "Trio",
            //    RaceNumberList = new int[] { 1, 2}
            //};
            //HPTCombBet combBet = HPTCombBet.CreateDummyMarkBet(bt);
            //AddTabItem(combBet);
        }

        private void miSimulateTvilling_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //{
            //    Code = "TV",
            //    Name = "Tvilling",
            //    RaceNumberList = new int[] { 1, 2 }
            //};
            //HPTCombBet combBet = HPTCombBet.CreateDummyMarkBet(bt);
            //AddTabItem(combBet);
        }

        private void miSimulateDD_Click(object sender, RoutedEventArgs e)
        {
            //HPTBetType bt = new HPTBetType()
            //{
            //    Code = "DD",
            //    Name = "Dagens Dubbel",
            //    RaceNumberList = new int[] { 1, 2 }
            //};
            //HPTCombBet combBet = HPTCombBet.CreateDummyMarkBet(bt);
            //AddTabItem(combBet);
        }

        #endregion

        #region Analys

        private void miAnalysis_Click(object sender, RoutedEventArgs e)
        {
            AddTabItem(new UCResultAnalyzerList(), "Resultatanalys", "Analys", "/Icons/HPT.ico", HPTResultAnalyzer.ResultAnalyzerList);

            //try
            //{
            //    Cursor = Cursors.Wait;

            //    HPTRaceDayInfo hptRdi = new HPTRaceDayInfo()
            //    {
            //        DataToShow = this.Config.DataToShowVxx,
            //        BetType = new HPTBetType()
            //        {
            //            Code = "V64",
            //            Name = "V64"                        
            //        }
            //    };

            //    HPTServiceConnector connector = new HPTServiceConnector();
            //    connector.GetRaceDayInfoByTrackAndDate(hptRdi.BetType, 9, new DateTime(2013,8,6), new HPTServiceConnector.RaceDayInfoDelegate(ReceiveRaceDayInfo));
            //    Cursor = Cursors.Arrow;

            //}
            //catch (Exception exc)
            //{
            //    Cursor = Cursors.Arrow;
            //    HPTConfig.Config.AddToErrorLog(exc);
            //}
            //var ofd = new OpenFileDialog();
            //ofd.InitialDirectory = HPTConfig.MyDocumentsPath;
            //ofd.Filter = "Hjälp på traven-system|*.hpt4;*.hpt5";
            //ofd.FileOk += new System.ComponentModel.CancelEventHandler(ofd_FileOk);
            //ofd.ShowDialog();
        }

        void ofd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    OpenFileDialog ofd = (OpenFileDialog)sender;
                    var sb = new StringBuilder();
                    string dir = System.IO.Path.GetDirectoryName(ofd.FileName);
                    var di = new DirectoryInfo(dir);
                    foreach (var fiHmb in di.GetFiles("*.hpt?"))
                    {
                        try
                        {
                            var hmb = HPTSerializer.DeserializeHPTSystem(fiHmb.FullName);
                            hmb.ApplyConfigRankVariables(HPTConfig.Config.DefaultRankTemplate);
                            hmb.RecalculateAllRanks();
                            hmb.RecalculateRank();
                            sb.Append(hmb.RaceDayInfo.Trackname);
                            sb.Append("\t");
                            sb.Append(hmb.RaceDayInfo.RaceDayDate.ToString("yyyy-MM-dd"));
                            sb.Append("\t");
                            if (hmb.RaceDayInfo.PayOutList != null && hmb.RaceDayInfo.PayOutList.Count > 0)
                            {
                                sb.Append(hmb.RaceDayInfo.PayOutList[0].PayOutAmount);
                            }
                            sb.Append("\t");

                            hmb.RaceDayInfo.RaceList.ForEach(r =>
                            {
                                foreach (int i in r.LegResult.Winners)
                                {
                                    var horse = r.HorseList.FirstOrDefault(h => h.StartNr == i);
                                    horse.Correct = true;
                                    sb.Append(horse.StakeDistributionShare);
                                    sb.Append("\t");
                                    sb.Append(horse.RankMean);
                                    sb.Append("\t");
                                }
                            });

                            //foreach (var race in hmb.RaceDayInfo.RaceList)
                            //{
                            //    for (int i = 0; i < race.LegResult.Winners.Length; i++)
                            //    {
                            //        var horse = race.HorseList.FirstOrDefault(h => h.StartNr == race.LegResult.Winners[i]);
                            //        horse.Correct = true;
                            //        sb.Append(horse.StakeDistributionShare);
                            //        sb.Append("\t");
                            //        sb.Append(horse.RankMean);
                            //        sb.Append("\t");
                            //    }
                            //}
                        }
                        catch (Exception exc)
                        {
                            string s = exc.Message;
                        }
                        sb.Append("\r\n");
                    }
                    //Clipboard.SetText(sb.ToString());
                    Clipboard.SetDataObject(sb.ToString());
                }
                catch (Exception exc)
                {
                    this.Config.AddToErrorLog(exc);
                }
            }
        }

        #endregion

        private void miNews_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.hpt.nu");
        }
    }

    public class HPTGUIMessage : Notifier
    {
        private string errorString;
        public string ErrorString
        {
            get
            {
                return errorString;
            }
            set
            {
                errorString = value;
                OnPropertyChanged("ErrorString");
            }
        }

        private Visibility buttonVisibility;
        public Visibility ButtonVisibility
        {
            get
            {
                return this.buttonVisibility;
            }
            set
            {
                this.buttonVisibility = value;
                OnPropertyChanged("ButtonVisibility");
            }
        }

        public string Message { get; set; }

        //public string Type { get; set; }

        //public string Command { get; set; }

        public string Key { get; set; }

        public string KeyAlt { get; set; }

        public int RaceNumberToLoad { get; set; }

        private HPTRaceDayInfo raceDayInfo;
        public HPTRaceDayInfo RaceDayInfo
        {
            get
            {
                return this.raceDayInfo;
            }
            set
            {
                this.raceDayInfo = value;
                OnPropertyChanged("RaceDayInfo");
            }
        }
    }
}

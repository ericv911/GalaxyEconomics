using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Common.Physics;
using Common.Logistics;
using System.Collections.ObjectModel;
using Common.Construction;
using Common.Astronomy;
using Common.Messaging;
using Common.Constants;
using Taxonomy;
using CompoundProvider;
using CompoundProvider.Types;
using FunctionalGroups;
using FunctionalGroups.Types;
using NAudio.Wave;
using System.Threading.Tasks;
using Audio;
using AudioSwitcher.AudioApi.CoreAudio;

namespace SpaceTrader
{
    public class SpaceGameMainViewModel : BaseViewModel, INotifyPropertyChanged
    {
        #region fields
        /// private interfaces that bind via mousebehaviour and the public interfaces in this viewmodel to mouse-actions in the xaml.
        private ICommand _mouseWheelCommand;
        private ICommand _mouseClick;
        private ICommand _mouseMove;
        private ICommand _keypressedup;
        private ICommand _keypresseddown;

        private bool canExecute = true;
        private string _overviewtext;
        private string _centralhubtext;
        private string _stellarobjectsystemtext;
        private string _stardate;
        private string _selectedshipsystemtext;

        protected ImageSource _testimage;
        #endregion

        #region properties & collections

        public DateTime TickTimer { get; set; } //  used for calculating framerate
        public int TurnCounter { get; set; }
        /// Settings :  All general gameplay-unrelated settings needed to play the game, such as :
        /// Mouse settings, screen settings, graphics settings, bitmap settings, timer settings etc.
        public GeneralSettings CommonSettings { get; set; } = new GeneralSettings();
        /// BaseCollections : Set of generic collections that are needed to generate specific gamedata.
        /// Collections include, element lists, elementgroup lists, orbital-body types, stellarobject-types etc.
        public BaseCollections BaseCollections { get; set; } = new BaseCollections();
        /// class for ungeneric Ship data and methods to use in game.
        public ShipViewModel Ships { get; set; } = new ShipViewModel();
        /// class for ungeneric CelestialBody data and methods to use in game.  Initialized in ViewModel Initializer with arguments
        public CelestialBodyViewModel CelestialBodies { get; set; } = new CelestialBodyViewModel();


        /// <summary>
        /// TaxonomyCollections : Set of taxonomy collections that are needed to generate specific gamedata.
        /// Collections include, Domains of life down to individual species  
        /// </summary>
        ITaxonomyCollection TaxonomyCollections { get; set; }

        /// <summary>
        /// CompoundCollection is a class. 
        /// Compoundcollection.Compounds is an observablecollection of all possible compounds
        /// </summary>
        ICompoundCollection CompoundCollection { get; set; }

        /// <summary>
        /// FunctionalGroupcollection is a class. 
        /// FunctionalGroupcollection.FunctionalGroups is an observablecollection of all possible functionalgroups
        /// </summary>
        IFunctionalGroupCollection FunctionalGroupCollection { get; set; }

        /// interfaces for  general constants, solar constants, earth constants and such
        ISolarConstants SolarConstants { get; } = new SolarConstants();
        IPhysicalConstants PhysicalConstants { get; } = new PhysicalConstants();
        IShipConstants ShipConstants { get; } = new ShipConstants();
        IEarthConstants EarthConstants { get; } = new EarthConstants(); 

        public bool CanExecute
        {
            get { return this.canExecute; }
            set
            {
                if (this.canExecute == value) { return; }
                this.canExecute = value;
            }
        }

        public string StarDate
        {
            get { return _stardate; }
            set 
            { 
                _stardate = value;            
                OnPropertyChanged();
            }
        }
        public string OverviewText
        {
            get { return _overviewtext; }
            set
            {
                _overviewtext = value;
                OnPropertyChanged();
            }
        }
        public string SelectedShipSystemText
        {
            get { return _selectedshipsystemtext; }
            set { 
                _selectedshipsystemtext = value;
                OnPropertyChanged();
            }
        }
        public string CentralHubText
        {
            get { return _centralhubtext; }
            set
            {
                _centralhubtext = value;
                OnPropertyChanged();
            }
        }
        public string StellarobjectSystemText
        {
            get { return _stellarobjectsystemtext; }
            set
            {
                _stellarobjectsystemtext = value;
                OnPropertyChanged();
            }
        }

        public ImageSource TestImage
        {
            get { return _testimage; }
            set
            {
                _testimage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region constructor/initializer load config 
        public SpaceGameMainViewModel(Window window) 
        {
            /* window events */
            window.Loaded += (sender, e) =>
            {
                EventSystem.Subscribe<TickerSymbolGalaxyGenerationSettings>(SetGalaxyGenerationSettings);
                EventSystem.Subscribe<TickerSymbolTotalAmountofFoodandPopulation>(SetFoodandProductionStrings);
                LoadConfigIni();
                Initialise();
            };

            window.Closed += (sender, e) =>
            {
            };

            /// each RelayCommand has 2 paremeters. The Action and the Predicate
            /// the Action is the method it binds the Interface to.
            /// the Predicate checks if the method can actually execute

            IShowGalacticGenerationSettingsScreen = new RelayCommand(RelayShowGalacticGenerationSettingsScreen, param => this.canExecute);
            ISetHighLightedStellarObjects = new RelayCommand(RelaySetHighlightedStellarObjects, param => this.canExecute);
            IPauseShips = new RelayCommand(RelayPauseShips, param => this.canExecute);
            IUnpauseShips = new RelayCommand(RelayUnpauseShips, param => this.canExecute);
            ISetShipPath = new RelayCommand(RelaySetShipPath, param => this.canExecute);
            ISetNewGamedata = new RelayCommand(RelaySetNewGamedata, param => this.canExecute);
            ISetFocusOwnShip = new RelayCommand(RelaySetFocusOwnShip, param => this.canExecute);
            IShowCelestialBodyInfoonScreen = new RelayCommand(RelayShowCelestialBodyInfoonScreen, param => this.canExecute);
            IShowGameInitialisationResultsonScreen = new RelayCommand(RelayShowGameInitialisationResultsonScreen, param => this.canExecute);
            ICalculatePathtoDestinationStar = new RelayCommand(RelayCalculatePathFromHometoDestinationStar, param => this.canExecute);
            ICalculatePathFromShiptoDestinationStar = new RelayCommand(RelayCalculatePathFromShiptoDestinationStar, param => this.canExecute);
            IRedrawScreen = new RelayCommand(RelayRedrawScreen, param => this.canExecute);
        }

        public void Initialise()
        {
            CommonSettings.StellarObjectSettings.PropertyChanged += (s, e) => SetScreenSettingsFromStellarObjectSettings();
            CommonSettings.BitmapDataSettings.SetBitmapData(CommonSettings.ScreenSettings.ScreenWidth, CommonSettings.ScreenSettings.ScreenHeight);   //add loadsettingsfromfile
            CommonSettings.Timer.SetTimer();
            TurnCounter = 1;
            
            FunctionalGroupCollection = FunctionalGroupFactory.GetFunctionalGroupCollection(BaseCollections.ElementGroups, BaseCollections.Elements);
            CompoundCollection = CompoundFactory.GetCompoundCollection(BaseCollections.ElementGroups, BaseCollections.Elements, FunctionalGroupCollection.FunctionalGroups);
            TaxonomyCollections = TaxonomyFactory.GetCollection();
            SetStartScreenVariablesandData();
            SetDynamicScreenVariablesandData();
            OverviewText = SetStringGeneralInitialisationResults();
            CommonSettings.Timer.ClockTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            CommonSettings.Timer.ClockTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            SetSound();
        }
        private readonly AudioPlaybackEngine apbe = new AudioPlaybackEngine(0.9f);
        private readonly AudioPlaybackEngine backgroundmusicaudiothread = new AudioPlaybackEngine(0.1f);
        private CachedSound BackgroundMusic;
        private CachedSound leftclicksound;
        private CachedSound rightclicksound;
        private CachedSound middleclicksound;
        private void SetSound()
        {
            AudioConverter.Converter.ConvertAudioto441Khz(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/leftclick.wav"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/leftclick441.wav"));
            AudioConverter.Converter.ConvertAudioto441Khz(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/rightclick.wav"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/rightclick441.wav"));
            AudioConverter.Converter.ConvertAudioto441Khz(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/middleclick.wav"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/middleclick441.wav"));
            middleclicksound = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/middleclick.wav");
            leftclicksound = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/leftclick.wav");
            rightclicksound = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/rightclick.wav");
            BackgroundMusic = new CachedSound(AppDomain.CurrentDomain.BaseDirectory + "resources/sounds/backgroundmeditation441.mp3");          
            backgroundmusicaudiothread.PlaySound(BackgroundMusic);
        }


        private void LoadConfigIni()
        {
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/config.ini")))
            {
                if (line.Length > 0 && !(line.Substring(0, 1) == "/"))
                {
                    splitstring = line.Split('=');
                    switch (splitstring[0])
                    {
                        case "ScreenWidth":
                            CommonSettings.ScreenSettings.ScreenWidth = Convert.ToInt32(splitstring[1]);
                            break;
                        case "ScreenHeight":
                            CommonSettings.ScreenSettings.ScreenHeight = Convert.ToInt32(splitstring[1]);
                            break;
                        case "StartNumberofStars":
                            CelestialBodies.StartNumberofStellarObjects = Convert.ToInt32(splitstring[1]);
                            break;
                        case "MaximumNumberofOrbitalBodies":
                            CelestialBodies.MaximumNumberofOrbitalBodies = Convert.ToInt32(splitstring[1]);
                            break;
                        case "MinimumNumberofOrbitalBodies":
                            CelestialBodies.MinimumNumberofOrbitalBodies = Convert.ToInt32(splitstring[1]);
                            break;
                        case "MaximumOrbitalBodyMassAroundLowMassStellarObjects":
                            CelestialBodies.MaximumOrbitalBodyMassAroundLowMassStellarObjects = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "MaximumOrbitalBodyMassAroundLowMetallicityStellarObjects":
                            CelestialBodies.MaximumOrbitalBodyMassAroundLowMetallicityStellarObjects = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "LowMassStellarObjectsCutOffforOrbitalBodyGeneration":
                            CelestialBodies.LowMassStellarObjectsCutOffforOrbitalBodyGeneration = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "LowMetallicityStellarObjectsCutOffforOrbitalBodyGeneration":
                            CelestialBodies.LowMetallicityStellarObjectsCutOffforOrbitalBodyGeneration = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "MaximumBulgeRadius":
                            CelestialBodies.MaximumRadiusofBulge = Convert.ToInt32(splitstring[1]);
                            break;
                        case "MinimumDistanceFromCentreofGalaxy":
                            CelestialBodies.MinimumDistancefromCentre = Convert.ToInt32(splitstring[1]);
                            break;
                        case "DrawStarsinCentre":
                            CelestialBodies.DrawStarsinCentre = Convert.ToBoolean(Convert.ToInt32(splitstring[1]));
                            break;
                        case "SpiralWindedness":
                            CelestialBodies.SpiralWindedness = Convert.ToInt32(splitstring[1]);
                            break;
                        case "InitializeStarsInSpiralArms":
                            CelestialBodies.InitializeStellarObjectsinSpiralArms = Convert.ToBoolean(Convert.ToInt32(splitstring[1]));
                            break;
                        case "InitializeStarsInBulge":
                            CelestialBodies.InitializeStellarObjectsinBulge = Convert.ToBoolean(Convert.ToInt32(splitstring[1]));
                            break;
                        case "InitializeStarsInDisc":
                            CelestialBodies.InitializeStellarObjectsinDisc = Convert.ToBoolean(Convert.ToInt32(splitstring[1]));
                            break;
                        case "InitializeStarsInBar":
                            CelestialBodies.InitializeStellarObjectsinBar = Convert.ToBoolean(Convert.ToInt32(splitstring[1]));
                            break;
                        case "StartNumberofShips":
                            Ships.StartNumberofCargoShips = Convert.ToInt32(splitstring[1]);
                            break;
                    }
                }
            }
        }
        #endregion

        /// the following ICommands list are the Interfaces used in the Mainwindow.xaml.   
        /// RelayCommand combines the ICommand, that is  binded to the xaml button press (or mouseaction),
        /// to a function in the ViewModel.

        #region declaring command interfaces   
        public ICommand IShowGalacticGenerationSettingsScreen { get; set; }
        public ICommand ISetHighLightedStellarObjects { get; set; }
        public ICommand IPauseShips { get; set; }
        public ICommand IUnpauseShips { get; set; }
        public ICommand ISetShipPath { get; set; }
        public ICommand ISetNewGamedata { get; set; }
        public ICommand ISetFocusOwnShip { get; set; }
        public ICommand IShowCelestialBodyInfoonScreen { get; set; }
        public ICommand IShowGameInitialisationResultsonScreen { get; set; }
        public ICommand ICalculatePathtoDestinationStar { get; set; }
        public ICommand ICalculatePathFromShiptoDestinationStar { get; set; }
        public ICommand IRedrawScreen { get; set; }
        ///  the following 4 ICommands are different from the previous ones in the sense that they use Prism and MVVM-light to bind to 
        ///  mouse-actions in the xaml. They do not use the RelayCommand class to bind the Interface to the Viewmodel method.
        ///  Rather they use a new EventArgs delegate to directly call the ViewmodelMethod
        public ICommand IKeyPressedDown => _keypresseddown = _keypresseddown ?? new DelegateCommand<KeyEventArgs>(e => RelayKeyPressDownCommandExecute(e));
        public ICommand IKeyPressedUp => _keypressedup = _keypressedup ?? new DelegateCommand<KeyEventArgs>(e => RelayKeyPressUpCommandExecute(e));
        public ICommand IMouseClick => _mouseClick = _mouseClick ?? new DelegateCommand<MouseButtonEventArgs>(e => RelayMouseClickCommandExecute(e));
        public ICommand IMouseWheelCommand => _mouseWheelCommand = _mouseWheelCommand ?? new DelegateCommand<MouseWheelEventArgs>(e => RelayMouseWheelCommandExecute(e));
        public ICommand IMouseMove => _mouseMove = _mouseMove ?? new DelegateCommand<MouseEventArgs>(e => RelayMouseMoveCommandExecute(e));
        
        #endregion

        #region methods for relaycommand Icommand to xaml and back with object obj and eventargs e

        private void RelayKeyPressDownCommandExecute(KeyEventArgs e)
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn)
            {
                if(e.Key.ToString() == CommonSettings.KeyboardSettings.ShowResources)
                {
                    CommonSettings.ScreenSettings.ShowCentralHubElements = !CommonSettings.ScreenSettings.ShowCentralHubElements;
                }
                if (e.Key.ToString() == CommonSettings.KeyboardSettings.SwitchElementandSymbol)
                {
                    CommonSettings.ScreenSettings.DisplayElementSymbol = !CommonSettings.ScreenSettings.DisplayElementSymbol;
                }
                //Console.WriteLine("test" + e.Key.ToString());
                if (e.Key.ToString() == "A")
                {
                    if (CommonSettings.KeyboardSettings.PressedShift)
                    {
                        if (CommonSettings.KeyboardSettings.PressedCtrl)
                        {
                            Console.WriteLine("Ctrl + Shift + a ");
                        }
                        else
                        {
                            Console.WriteLine("Shift + a");
                        }
                    }
                    else if (CommonSettings.KeyboardSettings.PressedCtrl)
                    {
                        Console.WriteLine("Ctrl + a ");
                    }
                    else
                    {
                        Console.WriteLine("a");
                    }

                }
                if(e.Key.ToString() == "LeftShift")
                {
                    CommonSettings.KeyboardSettings.PressedShift = true;
                }
                if (e.Key.ToString() == "LeftCtrl")
                {
                    CommonSettings.KeyboardSettings.PressedCtrl = true;
                }
                if (e.Key.ToString() == "Space")
                {
                    Ships.MoveShips = !Ships.MoveShips;
                }
            }
        }
        private void RelayKeyPressUpCommandExecute(KeyEventArgs e)
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn)
            {
                if (e.Key.ToString() == "LeftShift")
                {
                    CommonSettings.KeyboardSettings.PressedShift = false;
                }
                if (e.Key.ToString() == "LeftCtrl")
                {
                    CommonSettings.KeyboardSettings.PressedCtrl = false;
                }
            }
        }

        private void RelayMouseClickCommandExecute(MouseButtonEventArgs e)
        {
            System.Windows.Point position = GetMousePosition(e.GetPosition(Application.Current.MainWindow));
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                apbe.PlaySound(leftclicksound);
                //AudioPlaybackEngine.Instance.PlaySound(leftclicksound);
                if (CommonSettings.MouseSettings.MousepressedLeft == false)
                {
                    CommonSettings.MouseSettings.MousePosWhenPressedLeft = new System.Windows.Point(position.X, position.Y);
                    CommonSettings.MouseSettings.MousepressedLeft = true;
                }
                if (CommonSettings.ScreenSettings.IsGameDataDrawn)
                {
                    CommonSettings.MouseSettings.MousePosWhenPressedLeftA = new System.Windows.Point(Ships.ShipSelectedonScreen.ScreenCoordinates.X, Ships.ShipSelectedonScreen.ScreenCoordinates.Y);   //position.X - 150, position.Y);
                    CelestialBodies.SetActiveStar(position);
                    CelestialBodies.SetActiveStarlane(position);
                    Ships.SetActiveShip(position);
                    PrepareScreenDataandDrawBitmap();
                    SelectedShipSystemText = SetStringSelectedShipInfo();
                    StellarobjectSystemText = SetStringStellarSystemInfo();
                }
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                apbe.PlaySound(middleclicksound);
                
                CelestialBodies.SetActiveStar(position);
                CommonSettings.ScreenSettings.DisplayButtonCalculateShiptoStellarObject = !CommonSettings.ScreenSettings.DisplayButtonCalculateShiptoStellarObject;
                //if (CommonSettings.ScreenSettings.DisplayButtonCalculateShiptoStellarObject)
                //{
                //    CommonSettings.ScreenSettings.VisibilityButtonCalculateShiptoStellarObject = Visibility.Visible;
                //}
                //else
                //{
                //    CommonSettings.ScreenSettings.VisibilityButtonCalculateShiptoStellarObject = Visibility.Hidden;
                //}
                PrepareScreenDataandDrawBitmap();
                StellarobjectSystemText = SetStringStellarSystemInfo();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                apbe.PlaySound(rightclicksound);
            }
        }
        private void RelayMouseMoveCommandExecute(MouseEventArgs e)
        {
            
            System.Windows.Point position = GetMousePosition(e.GetPosition(Application.Current.MainWindow)); 
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!(position.X < 200 && position.Y < 600))
                {
                    if (position.X < CommonSettings.MouseSettings.MousePosWhenPressedLeft.X)
                    {
                        CommonSettings.ScreenSettings._3DSettings.Translations.X -= 25;
                    }
                    else
                    {
                        CommonSettings.ScreenSettings._3DSettings.Translations.X += 25;
                    }
                    if (position.Y < CommonSettings.MouseSettings.MousePosWhenPressedLeft.Y)
                    {
                        CommonSettings.ScreenSettings._3DSettings.Translations.Y -= 25;
                    }
                    else
                    {
                        CommonSettings.ScreenSettings._3DSettings.Translations.Y += 25;

                    }
                }
                PrepareScreenDataandDrawBitmap();
                CommonSettings.MouseSettings.MousePosWhenPressedLeft = position; // reference for next loop
            }
            if (e.LeftButton == MouseButtonState.Released)
            {
                CommonSettings.MouseSettings.MousepressedLeft = false;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (CommonSettings.MouseSettings.bMousepressedRight == false)
                {
                    CommonSettings.MouseSettings.MousePosWhenPressedRight = position;
                    CommonSettings.MouseSettings.bMousepressedRight = true;
                }
                //all angles in radians
                if (position.X < CommonSettings.MouseSettings.MousePosWhenPressedRight.X)
                {
                    CommonSettings.ScreenSettings._3DSettings.RotationAngles.X -= CommonSettings.ScreenSettings.DeltaRotationAngle; 
                }
                else
                {
                    CommonSettings.ScreenSettings._3DSettings.RotationAngles.X += CommonSettings.ScreenSettings.DeltaRotationAngle;
                }
                if (position.Y < CommonSettings.MouseSettings.MousePosWhenPressedRight.Y)
                {
                    CommonSettings.ScreenSettings._3DSettings.RotationAngles.Z += CommonSettings.ScreenSettings.DeltaRotationAngle;
                }
                else
                {
                    CommonSettings.ScreenSettings._3DSettings.RotationAngles.Z -= CommonSettings.ScreenSettings.DeltaRotationAngle;
                }
                PrepareScreenDataandDrawBitmap();
                CommonSettings.MouseSettings.MousePosWhenPressedRight = position;
            }
            if (e.RightButton == MouseButtonState.Released)
            {
                CommonSettings.MouseSettings.bMousepressedRight = false;
            }
        }
        private void RelayMouseWheelCommandExecute(MouseWheelEventArgs e)
        {
            System.Windows.Point position = GetMousePosition(e.GetPosition(Application.Current.MainWindow));
            if ( !(position.X < 200 && position.Y < 600) )
            {
                if (e.Delta > 0)
                {
                    if (!(CommonSettings.ScreenSettings._3DSettings.ScaleFactor > 10))
                    CommonSettings.ScreenSettings._3DSettings.ScaleFactor *= 2;
                }
                else
                {
                    if (!(CommonSettings.ScreenSettings._3DSettings.ScaleFactor < 0.2))
                    {
                        CommonSettings.ScreenSettings._3DSettings.ScaleFactor /= 2;
                    }
                }
                PrepareScreenDataandDrawBitmap();
            }
        }

        private void RelayShowCelestialBodyInfoonScreen(object obj) => SendMessagetoMessageWindow(2);
        private void RelayShowGameInitialisationResultsonScreen(object obj) => SendMessagetoMessageWindow(1);
        private void RelayShowGalacticGenerationSettingsScreen(object obj) => SendGalaxyGenerationSettings();
        private void RelaySetHighlightedStellarObjects(object obj) => SetScreenSettingsFromStellarObjectSettings();
        private void RelayPauseShips(object obj) => Pause();
        private void RelayUnpauseShips(object obj) => Unpause();
        private void RelaySetShipPath(object obj)
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn)
            {
                Ships.SetShipPathtoStellarObject(Ships.CargoShips.Count - 1, CelestialBodies.StellarPathfromSourcetoDestination);//   .PathfromSourcetoDestination);
                CommonSettings.ScreenSettings.DisplayButtonCalculateShiptoStellarObject = false; 
            }
            else
            {
                OverviewText = "Gamedata not set";
            }
        }
        private void RelaySetNewGamedata(object obj) => SetNewGamedata();
        private void RelaySetFocusOwnShip(object obj) 
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn)
            {
                Ships.SetActiveShip();
            }
            else
            {
                OverviewText = "Gamedata not set";
            }
        }
        private void RelayCalculatePathFromShiptoDestinationStar(object obj) => OverviewText = $"  Number of stellar objects from current destination \n  to new destination : {CalculatePathFromShiptoDestinationStar()} ";
        private void RelayCalculatePathFromHometoDestinationStar(object obj) => OverviewText = CalculatePathtoDestinationStar();
        private void RelayRedrawScreen(object obj) => PrepareScreenDataandDrawBitmap();

        #endregion

        #region assorted viewmodel methods outside of the Icommand-Relay pipeline. most are called by the RelayCommandfunctions
        private System.Windows.Point GetMousePosition(System.Windows.Point e)
        {
            return new System.Windows.Point(e.X - 25, e.Y - 35); // variables come from width and height of other control elements on the form. For now. this is ok. It needs to be documented
        }
        private void SetScreenSettingsFromStellarObjectSettings()
        {
            CommonSettings.ScreenSettings.ShowElements = CommonSettings.StellarObjectSettings.ShowElements;
            foreach (StellarObject stellarobject in CelestialBodies.StellarObjects)
            {
                stellarobject.BHighlightonScreen = (stellarobject.StellarType.Name == CommonSettings.StellarObjectSettings.SelectedStellarObjectClass);
            }
            PrepareScreenDataandDrawBitmap();
        }
        private void SetDynamicScreenVariablesandData()
        {
            CommonSettings.ScreenSettings.ScrollViewerHeightBig = Convert.ToInt32(((CommonSettings.ScreenSettings.ScreenHeight) / 3));
            CommonSettings.ScreenSettings.ScrollViewerHeightSmall = Convert.ToInt32(CommonSettings.ScreenSettings.ScreenHeight - (2 * CommonSettings.ScreenSettings.ScrollViewerHeightBig));
            if (CommonSettings.ScreenSettings.ScrollViewerHeightBig < 50)
            {
                CommonSettings.ScreenSettings.ScrollViewerHeightBig = 50;
                CommonSettings.ScreenSettings.ScrollViewerHeightSmall = 30;
            }
        }
        private void SetStartScreenVariablesandData()
        {
            CommonSettings.StellarObjectSettings.StellarObjectClasses = SetCelestialBodyItemSource();
            CommonSettings.ScreenSettings.FontSizes = new List<string>();
            for (int i = 8; i < 11; i++)
            {
                CommonSettings.ScreenSettings.FontSizes.Add(i.ToString());
            }
        }
        private List<string> SetCelestialBodyItemSource()
        {
            List<string> celestialbodytypelist = new List<string>();
            foreach (CelestialBodyType celestialbodytype in BaseCollections.StellarObjectTypes)
            {
                celestialbodytypelist.Add(celestialbodytype.Name);
            }
            return celestialbodytypelist;
        }
        

        public void PrepareScreenDataandDrawBitmap() //prepares parameters and sends command to drawbitmap
        {
            FastRandom rand = new FastRandom();
            if (CommonSettings.ScreenSettings.IsGameDataDrawn)
            {
                if (CommonSettings.ScreenSettings.OldScreenHeight != (int)Application.Current.MainWindow.ActualHeight|| CommonSettings.ScreenSettings.OldScreenWidth != (int)Application.Current.MainWindow.ActualWidth)
                {
                    CommonSettings.ScreenSettings.ScreenWidth = (int)Application.Current.MainWindow.ActualWidth;
                    CommonSettings.ScreenSettings.ScreenHeight = (int)Application.Current.MainWindow.ActualHeight;
                    CommonSettings.ScreenSettings.OldScreenWidth = (int)Application.Current.MainWindow.ActualWidth;
                    CommonSettings.ScreenSettings.OldScreenHeight = (int)Application.Current.MainWindow.ActualHeight;
                    SetDynamicScreenVariablesandData();
                    CommonSettings.BitmapDataSettings.Pixels = null;
                    CommonSettings.BitmapDataSettings.Pixels1d = null;
                   // GC.Collect(); //find different method for resizing array without nulling and garbage collection.  At the moment, without GC, there is a memory leak here.
                    CommonSettings.BitmapDataSettings.Pixels = new byte[CommonSettings.ScreenSettings.ScreenHeight, CommonSettings.ScreenSettings.ScreenWidth, 4];
                    CommonSettings.BitmapDataSettings.Pixels1d = new byte[CommonSettings.ScreenSettings.ScreenHeight * CommonSettings.ScreenSettings.ScreenWidth * 4];
                    CommonSettings.BitmapDataSettings.Rect = new Int32Rect(0, 0, CommonSettings.ScreenSettings.ScreenWidth, CommonSettings.ScreenSettings.ScreenHeight);
                }
                BitmapDataCalculations.CalculatePointsafterChange(CelestialBodies.StellarObjects, Ships.CargoShips, Ships.StellarObjectTradingShips, CommonSettings.ScreenSettings._3DSettings, CommonSettings.BitmapDataSettings.bitmapadjustvector);
                TestImage = DisplayFunctions.SetBitmap(rand, CommonSettings.ScreenSettings.ScreenWidth, CommonSettings.ScreenSettings.ScreenHeight, CommonSettings.BitmapDataSettings.Pixels, CommonSettings.BitmapDataSettings.Pixels1d, CelestialBodies.StellarObjects, Ships.CargoShips, Ships.StellarObjectTradingShips, CommonSettings.BitmapDataSettings.GrdBmp, CommonSettings.BitmapDataSettings.Rect, CommonSettings.ScreenSettings.DrawStarlanes, CommonSettings.ScreenSettings.DrawShips, CelestialBodies.StellarObjectSelectedOnScreen, CelestialBodies.StellarPathfromSourcetoDestination, Ships.ShipSelectedonScreen, CommonSettings.ScreenSettings._3DSettings.ScaleFactor, CelestialBodies.StarlaneSelectedOnScreen);
                // (ImageSource)  Displayfunction.SetBitmap is a WriteableBitmap. TestImage is an ImageSource.   Apparently they are compatible now. But they weren't compatible before that as far as I can remember.
                // A cast  TestImage = (ImageSource)Displayfunction.SetBitmap was needed. Now, without the cast, it works. 
            }
        }
        public void InitialiseShips()
        {
            FastRandom rand = new FastRandom();
            Ships.InitializeShips(BaseCollections.CargoShipTypes, CelestialBodies.StellarObjects, BaseCollections.EconomicEntities, rand);
        }
        public void Pause()
        {
            Ships.MoveShips = false;
            CommonSettings.ScreenSettings.GamePaused = true;
        }
        public void Unpause()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn != true)
            {
                return;
            }
            TickTimer = DateTime.Now;
            Ships.MoveShips = true;
            CommonSettings.ScreenSettings.GamePaused = false;
        }

        //not used but handy for the future. string containing 3d-actions settings.
        public string SetStringGraphicalTransformationSettings() //not used currently. Was used for Debugging Results
        {
            StringBuilder VariableTransformationString = new StringBuilder();
            VariableTransformationString.AppendLine("Transformations");
            VariableTransformationString.AppendLine($"Translations   ->X : {CommonSettings.ScreenSettings._3DSettings.Translations.X} , Y : {CommonSettings.ScreenSettings._3DSettings.Translations.Y} ");
            VariableTransformationString.AppendLine($"Scalefactor    -> : {CommonSettings.ScreenSettings._3DSettings.ScaleFactor}");
            VariableTransformationString.AppendLine($"Rotationangles -> X : {CommonSettings.ScreenSettings._3DSettings.RotationAngles.X} , Y : {CommonSettings.ScreenSettings._3DSettings.RotationAngles.Y} , Z : {CommonSettings.ScreenSettings._3DSettings.RotationAngles.Z} ");
            return VariableTransformationString.ToString();
        }
        #region set overviewtextblockmethods
       
        public string SetStringGeneralInitialisationResults()
        {
            StringBuilder overviewstring = new StringBuilder();
            overviewstring.AppendLine("\nTaxonomy results : \n");
            overviewstring.AppendLine($" Domains generated : {TaxonomyCollections.Domains.Count} ");
            overviewstring.AppendLine($" Kingdoms generated : {TaxonomyCollections.Kingdoms.Count} ");
            overviewstring.AppendLine($" Phyla generated : {TaxonomyCollections.Phyla.Count} ");
            overviewstring.AppendLine($" SubPhyla generated : {TaxonomyCollections.SubPhyla.Count} ");
            overviewstring.AppendLine($" Classes generated : {TaxonomyCollections.Classes.Count} ");
            overviewstring.AppendLine($" Orders generated : {TaxonomyCollections.Orders.Count} ");
            overviewstring.AppendLine($" Families generated : {TaxonomyCollections.Families.Count} ");
            overviewstring.AppendLine($" Geni generated : {TaxonomyCollections.Geni.Count} ");
            overviewstring.AppendLine($" Species generated : {TaxonomyCollections.Species.Count} ");
            overviewstring.AppendLine("\nGeneral other collection results : \n");
            overviewstring.AppendLine($" Cargoship types generated : {BaseCollections.CargoShipTypes.Count}");
            overviewstring.AppendLine($" Elements generated : {BaseCollections.Elements.Count} ");
            overviewstring.AppendLine($" Elementgroups generated : {BaseCollections.ElementGroups.Count} ");
            overviewstring.AppendLine($" Orbital body-types generated : {BaseCollections.OrbitalbodyTypes.Count} ");
            overviewstring.AppendLine($" Stellar types generated : {BaseCollections.StellarObjectTypes.Count} ");
            overviewstring.AppendLine($" Building types generated : {BaseCollections.BuildingTypes.Count} ");
            overviewstring.AppendLine($" Economic entities generated : {BaseCollections.EconomicEntities.Count} ");
            overviewstring.AppendLine($" Technology level-types generated : {BaseCollections.TechLevelCollection.Count} ");
            overviewstring.AppendLine($"\nSolar constants loaded from file :\n");
            overviewstring.AppendLine($" Mass {SolarConstants.Mass} kg");
            overviewstring.AppendLine($" Radius {SolarConstants.Radius} km");
            overviewstring.AppendLine($" Temperature {SolarConstants.Temperature} Kelvin");
            overviewstring.AppendLine($" Luminosity {SolarConstants.Luminosity} Watt (J/s) (1 kg⋅m\u00B2⋅s\u207B\u00B3)");
            overviewstring.AppendLine($"\nPhysical constants loaded from file :\n");
            overviewstring.AppendLine($"Stefan Boltzmann constant {PhysicalConstants.StefanBoltzmannConstant} W⋅m\u207B\u00B2⋅k\u207B\u2074");
            overviewstring.AppendLine($"Earth Mass {EarthConstants.Mass} kg");
            return overviewstring.ToString();
        }
        public string SetStringSelectedShipInfo()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn != true)
            {
                return "game data not set";
            }
            StringBuilder SelectedShipInfotoString = new StringBuilder();
            SelectedShipInfotoString.AppendLine($"Ship : {Ships.ShipSelectedonScreen.Name} , Class :  {Ships.ShipSelectedonScreen.CargoShipType.Name} ");
            SelectedShipInfotoString.AppendLine($"Owned by {Ships.ShipSelectedonScreen.EconomicEntity.Name}");
            SelectedShipInfotoString.AppendLine($"Destination : {Ships.ShipSelectedonScreen.DestinationStellarObject.Name}");
            if (Ships.ShipSelectedonScreen.NeedsRefueling && !Ships.ShipSelectedonScreen.IsRefueling)
            {
                SelectedShipInfotoString.AppendLine($"Ship needs refueling");
            }
            if (Ships.ShipSelectedonScreen.NeedsRepairing && !Ships.ShipSelectedonScreen.IsRepairing)
            {
                SelectedShipInfotoString.AppendLine($"Ship needs repairing");
            }
            if (Ships.ShipSelectedonScreen.IsRefueling)
            {
                SelectedShipInfotoString.AppendLine($"Ship is refueling");
            }
            else if (Ships.ShipSelectedonScreen.IsOverhauling)
            {
                SelectedShipInfotoString.AppendLine($"Ship is overhauling");
            }
            else if (Ships.ShipSelectedonScreen.IsRepairing)
            {
                SelectedShipInfotoString.AppendLine($"Ship is repairing");
            }
            else if (Ships.ShipSelectedonScreen.IsDocked)
            {
                SelectedShipInfotoString.AppendLine($"Ship is docked, loading and unloading supplies");
            }
            SelectedShipInfotoString.AppendLine($"Hull : {Ships.ShipSelectedonScreen.HullIntegrity:F2} Level : {Ships.ShipSelectedonScreen.ShipUpgradeLevel} ");
            SelectedShipInfotoString.AppendLine($"Docking Duration : {Ships.ShipSelectedonScreen.DockingDuration}" );
            SelectedShipInfotoString.AppendLine($"CargoHolds : {Ships.ShipSelectedonScreen.NumberofCargoHolds} Per hold : {Ships.ShipSelectedonScreen.MaximumAmountofCargoperCargoHold}" );
            SelectedShipInfotoString.AppendLine($"FuelCapacity : {Ships.ShipSelectedonScreen.FuelAmount:F1} Fuel Consumption : {Ships.ShipSelectedonScreen.FuelConsumption:F2}");
            SelectedShipInfotoString.AppendLine($"Base Amount of repairs each turn : {Ships.ShipSelectedonScreen.BaseRepairAmountperTurn}");
            SelectedShipInfotoString.AppendLine(SetStringElementsinStorage(Ships.ShipSelectedonScreen.ElementssonShip));
            return SelectedShipInfotoString.ToString();
        }
        public string SetStringElementsinStorage(ObservableCollection<ElementinStorage> elementsinstorage)
        {
            StringBuilder overviewstring = new StringBuilder();
            string _elementname;
            foreach (ElementinStorage elementinstorage in elementsinstorage)
            {
                _elementname = $"          -{(!CommonSettings.ScreenSettings.DisplayElementSymbol ? elementinstorage.Element.Name.Replace("-", "") : elementinstorage.Element.Symbol)} ";
                if (elementinstorage.AmountinStorage == 0)
                {

                }
                else if (elementinstorage.AmountinStorage > 10000)
                {
                    overviewstring.AppendLine($"{_elementname} >  {elementinstorage.AmountinStorage / 1000:F0} ton");
                }
                else if (elementinstorage.AmountinStorage > 1000)
                {
                    overviewstring.AppendLine($"{_elementname} >  {(elementinstorage.AmountinStorage / 1000):F1} ton");
                }
                else if (elementinstorage.AmountinStorage > 100)
                {
                    overviewstring.AppendLine($"{_elementname} >  {(elementinstorage.AmountinStorage):F0} kilogram");
                }
                else if (elementinstorage.AmountinStorage > 10)
                {
                    overviewstring.AppendLine($"{_elementname} >  {elementinstorage.AmountinStorage:F1} kilogram");
                }
                else if (elementinstorage.AmountinStorage > 1)
                {
                    overviewstring.AppendLine($"{_elementname} >  {elementinstorage.AmountinStorage:F2} kilogram");
                }
                else if (elementinstorage.AmountinStorage > 0.1)
                {
                    overviewstring.AppendLine($"{_elementname} >  {elementinstorage.AmountinStorage * 1000:F1} gram");
                }
                else if (elementinstorage.AmountinStorage > 0.01)
                {
                    overviewstring.AppendLine($"{_elementname} >   {(elementinstorage.AmountinStorage * 1000):F2} gram");
                }
                else if (elementinstorage.AmountinStorage > 0.001)
                {
                    overviewstring.AppendLine($"{_elementname} >  {(elementinstorage.AmountinStorage * 1000):F3} gram");
                }
                else
                {
                    overviewstring.AppendLine($"{_elementname} >  < 0.001 gram");
                }
            }
            if (overviewstring.Length == 0 )
            {
                return "\n   - No Elements present ";
            }
            return "\n   - Elements present : -> \n" + overviewstring.ToString();
        }
        public string SetStringCentralHubInfo(StellarObject stellarobject)
        {
            StringBuilder centralhubstring = new StringBuilder();

            if (stellarobject.CentralHub.Buildings.Count > 0)
            {
                centralhubstring.AppendLine("   - Buildings present: ");
                foreach (Building building in stellarobject.CentralHub.Buildings)
                {
                    centralhubstring.AppendLine($"          - {building.Type.Name} size : {building.Size} level : {building.TechLevel} ");
                }
            }
            else
            {
                centralhubstring.AppendLine("   - No buildings present. ");
            }
            if (stellarobject.CentralHub.CompoundsinStorage.Count > 0)
            {
                centralhubstring.AppendLine("\n   - Compounds in Storage : ");
                foreach (CompoundinStorage compoundinstorage in stellarobject.CentralHub.CompoundsinStorage)
                {
                    switch (CommonSettings.ScreenSettings.DisplayElementSymbol)
                    {
                        case true:
                            centralhubstring.AppendLine($"          - {(compoundinstorage.Compound.ElementsinCompound[0].Element.Symbol)}{(compoundinstorage.Compound.ElementsinCompound[0].AmountinCompound == 1 ? "" : (compoundinstorage.Compound.ElementsinCompound[0].AmountinCompound == 2 ? "\x2082" : "\x2083"))}{(compoundinstorage.Compound.ElementsinCompound[1].Element.Symbol)}{(compoundinstorage.Compound.ElementsinCompound[1].AmountinCompound == 1 ? "" : (compoundinstorage.Compound.ElementsinCompound[1].AmountinCompound == 2 ? "\x2082" : (compoundinstorage.Compound.ElementsinCompound[1].AmountinCompound == 3 ? "\x2083" : "\x2084")))}     ->   : {compoundinstorage.AmountinStorage:F2} kg");
                            break;
                        case false:
                            centralhubstring.AppendLine($"          - {(compoundinstorage.Compound.ElementsinCompound[0].AmountinCompound == 1 ? "" : (compoundinstorage.Compound.ElementsinCompound[0].AmountinCompound == 2 ? "Di" : "Tri"))}{(compoundinstorage.Compound.ElementsinCompound[0].AmountinCompound == 1 ? compoundinstorage.Compound.ElementsinCompound[0].Element.Name.Substring(2).Replace("-", "") : compoundinstorage.Compound.ElementsinCompound[0].Element.Name.Substring(2).Replace("-", "").ToLower())} {(compoundinstorage.Compound.ElementsinCompound[1].AmountinCompound == 1 ? "" : (compoundinstorage.Compound.ElementsinCompound[1].AmountinCompound == 2 ? "di" : (compoundinstorage.Compound.ElementsinCompound[1].AmountinCompound == 3 ? "tri" : "tetra")))}{(compoundinstorage.Compound.ElementsinCompound[1].Element.Name.Contains("-") ? (compoundinstorage.Compound.ElementsinCompound[1].Element.Name.Substring(2, compoundinstorage.Compound.ElementsinCompound[1].Element.Name.Substring(2).IndexOf("-"))).ToLower() : compoundinstorage.Compound.ElementsinCompound[1].Element.Name.Substring(2).ToLower())}ide     ->   : {compoundinstorage.AmountinStorage:F2} kg");
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                centralhubstring.AppendLine("\n   - No compounds in storage. ");
            }
            if (stellarobject.CentralHub.FunctionalGroupsResearched.Count > 0)
            {
                centralhubstring.AppendLine("\n   - Functional groups researched : ");
                foreach (FunctionalGroup functionalgroup in stellarobject.CentralHub.FunctionalGroupsResearched)
                {
                    switch (CommonSettings.ScreenSettings.DisplayElementSymbol)
                    {
                        case true:
                            centralhubstring.AppendLine($"          - {functionalgroup.Name} : {(functionalgroup.ElementsinFunctionalGroup[0].Element.Symbol)}{(functionalgroup.ElementsinFunctionalGroup[0].AmountinCompound == 1 ? "" : (functionalgroup.ElementsinFunctionalGroup[0].AmountinCompound == 2 ? "\x2082" : "\x2083"))}{(functionalgroup.ElementsinFunctionalGroup[1].Element.Symbol)}{(functionalgroup.ElementsinFunctionalGroup[1].AmountinCompound == 1 ? "" : (functionalgroup.ElementsinFunctionalGroup[1].AmountinCompound == 2 ? "\x2082" : (functionalgroup.ElementsinFunctionalGroup[1].AmountinCompound == 3 ? "\x2083" : "\x2084")))}     charge ->  {functionalgroup.Charge}   -> mass : {functionalgroup.AtomicMass:F3} da");
                            break;
                        case false:
                            centralhubstring.AppendLine($"          - {functionalgroup.Name} : {(functionalgroup.ElementsinFunctionalGroup[0].AmountinCompound == 1 ? "" : (functionalgroup.ElementsinFunctionalGroup[0].AmountinCompound == 2 ? "Di" : "Tri"))}{(functionalgroup.ElementsinFunctionalGroup[0].AmountinCompound == 1 ? functionalgroup.ElementsinFunctionalGroup[0].Element.Name.Substring(2).Replace("-", "") : functionalgroup.ElementsinFunctionalGroup[0].Element.Name.Substring(2).Replace("-", "").ToLower())} {(functionalgroup.ElementsinFunctionalGroup[1].AmountinCompound == 1 ? "" : (functionalgroup.ElementsinFunctionalGroup[1].AmountinCompound == 2 ? "di" : (functionalgroup.ElementsinFunctionalGroup[1].AmountinCompound == 3 ? "tri" : "tetra")))}{(functionalgroup.ElementsinFunctionalGroup[1].Element.Name.Contains("-") ? (functionalgroup.ElementsinFunctionalGroup[1].Element.Name.Substring(2, functionalgroup.ElementsinFunctionalGroup[1].Element.Name.Substring(2).IndexOf("-"))).ToLower() : functionalgroup.ElementsinFunctionalGroup[1].Element.Name.Substring(2).ToLower())}ide     charge ->  {functionalgroup.Charge}     -> mass : {functionalgroup.AtomicMass:F3} da");
                            break;
                        default:
                            break;
                    }
                   // centralhubstring.AppendLine($"                  Oxidation states :   {functionalgroup.ElementsinFunctionalGroup[0].Element.Name.Substring(2)} -> {functionalgroup.ElementsinFunctionalGroup[0].OxidationState} {functionalgroup.ElementsinFunctionalGroup[1].Element.Name.Substring(2)} -> {functionalgroup.ElementsinFunctionalGroup[1].OxidationState} ");
                }
            }
            else
            {
                centralhubstring.AppendLine("\n   - No functional groups researched. ");
            }
            if (stellarobject.CentralHub.CompoundsResearched.Count > 0)
            {
                centralhubstring.AppendLine("\n   - Compounds researched : ");
                foreach (Compound compound in stellarobject.CentralHub.CompoundsResearched)
                {
                    switch (CommonSettings.ScreenSettings.DisplayElementSymbol)
                    {
                        case true:
                            centralhubstring.AppendLine($"          - {(compound.ElementsinCompound[0].Element.Symbol)}{(compound.ElementsinCompound[0].AmountinCompound == 1 ? "" : (compound.ElementsinCompound[0].AmountinCompound == 2 ? "\x2082" : "\x2083"))}{(compound.ElementsinCompound[1].Element.Symbol)}{(compound.ElementsinCompound[1].AmountinCompound == 1 ? "" : (compound.ElementsinCompound[1].AmountinCompound == 2 ? "\x2082" : (compound.ElementsinCompound[1].AmountinCompound == 3 ? "\x2083" : "\x2084")))}     -> mass : {compound.AtomicMass:F3} da");
                            break;
                        case false:
                            centralhubstring.AppendLine($"          - {(compound.ElementsinCompound[0].AmountinCompound == 1 ? "" : (compound.ElementsinCompound[0].AmountinCompound == 2 ? "Di" : "Tri"))}{(compound.ElementsinCompound[0].AmountinCompound == 1 ? compound.ElementsinCompound[0].Element.Name.Substring(2).Replace("-", "") : compound.ElementsinCompound[0].Element.Name.Substring(2).Replace("-", "").ToLower())} {(compound.ElementsinCompound[1].AmountinCompound == 1 ? "" : (compound.ElementsinCompound[1].AmountinCompound == 2 ? "di" : (compound.ElementsinCompound[1].AmountinCompound == 3 ? "tri" : "tetra")))}{(compound.ElementsinCompound[1].Element.Name.Contains("-") ? (compound.ElementsinCompound[1].Element.Name.Substring(2, compound.ElementsinCompound[1].Element.Name.Substring(2).IndexOf("-"))).ToLower() : compound.ElementsinCompound[1].Element.Name.Substring(2).ToLower())}ide    -> mass : {compound.AtomicMass:F3} da");
                            break;
                        default:
                            break;
                    }
                    //centralhubstring.AppendLine($"                  Oxidation states :   {compound.ElementsinCompound[0].Element.Name.Substring(2)} -> {compound.ElementsinCompound[0].OxidationState} {compound.ElementsinCompound[1].Element.Name.Substring(2)} -> {compound.ElementsinCompound[1].OxidationState} ");
                }
            }
            else
            {
                centralhubstring.AppendLine("\n   - No compounds researched. ");
            }

            if (CommonSettings.ScreenSettings.ShowCentralHubElements)
            {
                centralhubstring.AppendLine( SetStringElementsinStorage(stellarobject.CentralHub.ElementsinStorage));
            }

            return centralhubstring.ToString();
        }

        public StringBuilder SetStringOrbitalbodyInfo(OrbitalBody orbitalbody)
        {
            StringBuilder overviewstring = new StringBuilder();
            StringBuilder Elementsstring = new StringBuilder();
            overviewstring.AppendLine($" - {orbitalbody.Name} : {orbitalbody.OrbitalBodyType.Name}");
            overviewstring.AppendLine($"      - Mass : {orbitalbody.Mass:F3} earth mass : Radius {orbitalbody.Radius} km. ");
            overviewstring.AppendLine($"      - Density : {orbitalbody.Density:F2}");
            foreach (SpeciesperNode species in orbitalbody.SpeciesonOrbitalBody)
            {
                overviewstring.AppendLine($"      - {species.Species.Genus.Family.Order.Name} -> {species.Species.Genus.Family.Name} -> {species.Species.Name} ");
                overviewstring.AppendLine($"      - Population size : {species.PopulationSize:F0} Reproduction rate : {species.ReproductionRate:F2}");
            }
            overviewstring.AppendLine($"      - Housing available for population: {orbitalbody.PopulationHousing}");
            overviewstring.AppendLine($"      - Distance to Star : {orbitalbody.AverageDistanceToCentralStellarObject/150000000000:F2} AU ");
            overviewstring.AppendLine($"      - Solar power per m² : {orbitalbody.SolarPowerperM2:F2} Watt");
            overviewstring.AppendLine($"      - Food : {(int)orbitalbody.Food} -> Food storage : {orbitalbody.FoodStorage}");
            if (CommonSettings.ScreenSettings.ShowModifiers)
            {
                overviewstring.AppendLine($"      - FoodModifierfromBuildings : {orbitalbody.FoodModifierfromBuildings:F3}");
                overviewstring.AppendLine($"      - BasNaturalHabitationModifier : {orbitalbody.BaseNaturalHabitationModifier:F3}");
                overviewstring.AppendLine($"      - NaturalHabitationModifier : {orbitalbody.NaturalHabitationModifier:F3}");
                overviewstring.AppendLine($"      - PopulationModifierfromBuldings : {orbitalbody.PopulationModifierfromBuildings:F3}");
                overviewstring.AppendLine($"      - NaturalBirthPercentage : {orbitalbody.NaturalBirthsperTurnPercentage:F3}");
                overviewstring.AppendLine($"      - NaturalDeathPercentage : {orbitalbody.NaturalDeathsperTurnPercentage:F3}");
                overviewstring.AppendLine($"      - BaseNaturalDeathPercentage : {orbitalbody.BaseNaturalDeathsperTurnPercentage:F3}");
                overviewstring.AppendLine($"      - NaturalImmigrationperTurnLinear : {orbitalbody.NaturalImmigrationperTurnLinear:F3}");
            }

            if (CommonSettings.ScreenSettings.ShowBuildings)
            {
                overviewstring.AppendLine($"      - Buildings : ");
                foreach (Building building in orbitalbody.Buildings)
                {
                    overviewstring.Append($"                 - {building.Type.Name}  ");
                    if (building.Type.CanResize == true)
                    {
                        overviewstring.Append($" Size : {building.Size} Techlevel : {building.TechLevel} ");
                    }
                    overviewstring.Append($"\n");
                }
            }
            if (CommonSettings.ScreenSettings.ShowElementGroups)
            {
                if (orbitalbody.ElementGroups.Count == 0)
                {
                    Elementsstring.Append($"   - No element-modifiers present on {orbitalbody.Name}");
                }
                else
                {
                    overviewstring.AppendLine("   - Elementgroups present : ");
                    overviewstring.Append("        ");
                    foreach (ElementGroup elementgroup in orbitalbody.ElementGroups)
                    {
                        Elementsstring.Append($"{elementgroup.Name} , ");
                    }
                    Elementsstring.Remove(Elementsstring.Length - 1, 1);
                }
            }
            overviewstring.AppendLine(Elementsstring.ToString());
            if (CommonSettings.ScreenSettings.ShowElements)
            {
                overviewstring.AppendLine(SetStringElementsinStorage(orbitalbody.ElementsinStorage));
            }
            return overviewstring;
        }

        public string SetStringStellarSystemInfo()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn != true)
            {
                return "game data not set";
            }
            StringBuilder overviewstring = new StringBuilder();
            overviewstring.AppendLine($" {CelestialBodies.StellarObjectSelectedOnScreen.Name} -> {CelestialBodies.StellarObjectSelectedOnScreen.StellarType.Name}");
            overviewstring.AppendLine($" Mass : {CelestialBodies.StellarObjectSelectedOnScreen.Mass:F2} solar masses");
            overviewstring.AppendLine($" Age : {CelestialBodies.StellarObjectSelectedOnScreen.Age:N0}.000 years");
            overviewstring.AppendLine($" Radius : {CelestialBodies.StellarObjectSelectedOnScreen.Radius:N0} kilometer");
            overviewstring.AppendLine($" Metallicity : {CelestialBodies.StellarObjectSelectedOnScreen.Metallicity:N2} Fe/H relative to the sun");
            overviewstring.AppendLine($" Surface temperature : {CelestialBodies.StellarObjectSelectedOnScreen.SurfaceTemperature * 1000:F0} degrees °C");
            overviewstring.AppendLine($" Luminosity : {CelestialBodies.StellarObjectSelectedOnScreen.Luminosity:E4}  kg⋅m\u00B2⋅s\u207B\u00B3 ");
            overviewstring.AppendLine($" Absolute Magnitude : {CelestialBodies.StellarObjectSelectedOnScreen.AbsoluteMagnitude:F2}");
            overviewstring.AppendLine($" Lower boundary habitable zone : {CelestialBodies.StellarObjectSelectedOnScreen.MinimumHabitableZoneRadius / 1000:E3} km / {CelestialBodies.StellarObjectSelectedOnScreen.MinimumHabitableZoneRadius / 150000000000:F2} AU");
            overviewstring.AppendLine($" Upper boundary habitable zone : {CelestialBodies.StellarObjectSelectedOnScreen.MaximumHabitableZoneRadius / 1000:E3} km / {CelestialBodies.StellarObjectSelectedOnScreen.MaximumHabitableZoneRadius / 150000000000:F2} AU");
            overviewstring.AppendLine($" Maximum orbital body distance : {CelestialBodies.StellarObjectSelectedOnScreen.MaximumOrbitalBodyDistanceFromStar / 1000:E3} km / {CelestialBodies.StellarObjectSelectedOnScreen.MaximumOrbitalBodyDistanceFromStar / 150000000000:F2} AU\n");
            overviewstring.AppendLine($"STARLANES : ");
            foreach (Starlane starlane in CelestialBodies.StellarObjectSelectedOnScreen.StarLanes)
            {
                overviewstring.AppendLine($" - To {starlane.To.Name} , techlevel : {starlane.TechLevelRequiredforTravel.Level}    length : {starlane.Length:F2} lightyears. " );
            }

            if (CommonSettings.ScreenSettings.ShowOrbitalBodyInfo)
            {
                overviewstring.AppendLine("\nORBITAL BODIES : ");
                foreach (OrbitalBody orbitalbody in CelestialBodies.StellarObjectSelectedOnScreen.Orbitalbodies)
                {
                    overviewstring.Append(SetStringOrbitalbodyInfo(orbitalbody));
                    if (CommonSettings.ScreenSettings.ShowNaturalSatellitesInfo)
                    {
                        if (orbitalbody.NaturalSatellites.Count == 0)
                        {
                            overviewstring.AppendLine($"   - No natural satellites. ");
                        }

                        else
                        {
                            overviewstring.AppendLine($"   - Natural satellites : ");
                            foreach (OrbitalBody naturalsatellite in orbitalbody.NaturalSatellites)
                            {
                                overviewstring.Append(SetStringOrbitalbodyInfo(naturalsatellite));
                            }
                        }
                    }
                }
            }
            CentralHubText = SetStringCentralHubInfo(CelestialBodies.StellarObjectSelectedOnScreen);
            return overviewstring.ToString();
        }
        public string SetStringCelestialBodyInitialisationResults()
        {
            StringBuilder overviewstring = new System.Text.StringBuilder();
            var stellarinfo = new List<StellarObject>();
            var orbitalbodyinfo = new List<OrbitalBody>();
            var naturalsatelliteinfo = new List<OrbitalBody>();
            int totalcntr = 0;
            overviewstring.AppendLine($"CELESTIAL BODY GENERATION RESULTS  \n");
            overviewstring.AppendLine($" ---Stellar objects  : ");
            foreach (StellarObjectType stellartype in BaseCollections.StellarObjectTypes)
            {
                stellarinfo = CelestialBodies.StellarObjects.Where(stl => stl.StellarType.Name == stellartype.Name).ToList();
                if (stellarinfo.Count > 0)
                {
                    overviewstring.AppendLine($"  {stellartype.Name} : {stellarinfo.Count}");
                    totalcntr += stellarinfo.Count;
                }
            }

            overviewstring.AppendLine($"Total stellar objects : {totalcntr} \n\n ---Orbital bodies around stellar objects  :");
            totalcntr = 0;
            foreach (OrbitalBodyType orbitalbodytype in BaseCollections.OrbitalbodyTypes)
            {
                orbitalbodyinfo = CelestialBodies.StellarObjects.SelectMany(stl => stl.Orbitalbodies).Where(ob => ob.OrbitalBodyType.Name == orbitalbodytype.Name).ToList();
                if (orbitalbodyinfo.Count > 0)
                {
                    overviewstring.AppendLine($"  {orbitalbodytype.Name} : {orbitalbodyinfo.Count}");
                    totalcntr += orbitalbodyinfo.Count;
                }
            }
            overviewstring.AppendLine($"Total orbital bodies : {totalcntr} \n\n ---Natural sattelites around orbital bodies  :");
            totalcntr = 0;
            foreach (OrbitalBodyType naturalsatellite in BaseCollections.OrbitalbodyTypes)
            {
                naturalsatelliteinfo = CelestialBodies.StellarObjects.SelectMany(h => h.Orbitalbodies)
                            .SelectMany(j => j.NaturalSatellites).Where(rt => rt.OrbitalBodyType.Name == naturalsatellite.Name).ToList();
                if (naturalsatelliteinfo.Count > 0)
                {
                    overviewstring.AppendLine($"  {naturalsatellite.Name} : {naturalsatelliteinfo.Count}");
                    totalcntr += naturalsatelliteinfo.Count;
                }
            }
            overviewstring.AppendLine($"Total natural satellites : {totalcntr}");
            return overviewstring.ToString();
        }

        #endregion

        public async void SetNewGamedata()  
        {
            CommonSettings.ScreenSettings.IsGameDataDrawn = true;
            TurnCounter = 0;
            Pause();
            foreach (StellarObject stellarobject in CelestialBodies.StellarObjects.ToList())
            {
                foreach (OrbitalBody orbitalbody in stellarobject.Orbitalbodies.ToList())
                {
                    orbitalbody.ElementsinStorage.Clear();
                    orbitalbody.ElementGroups.Clear();
                    orbitalbody.Buildings.CollectionChanged -= (obj, e) => orbitalbody.RecalculateModifiersandProperties();
                    orbitalbody.Buildings.ItemPropertyChanged -= (obj, e) => orbitalbody.RecalculateModifiersandProperties();
                    orbitalbody.Buildings.Clear();
                    orbitalbody.NaturalSatellites.Clear();
                }
                stellarobject.Orbitalbodies.Clear();
                stellarobject.StarLanes.Clear();
            }
            Ships.StellarObjectTradingShips.Clear();
            Ships.CargoShips.Clear();
            CelestialBodies.StellarObjects.Clear();

            await CelestialBodies.SetCelestialBodyDatasAsync(CommonSettings.ScreenSettings.ScreenWidth, TaxonomyCollections.Species,   BaseCollections.OrbitalbodyTypes, BaseCollections.StellarObjectTypes, BaseCollections.ElementGroups, BaseCollections.TechLevelCollection, BaseCollections.BuildingTypes, BaseCollections.Elements, PhysicalConstants, SolarConstants);
            InitialiseShips();
            CelestialBodies.SetActiveStar(new System.Windows.Point(0, 0));
            Ships.SetActiveShip(new System.Windows.Point(0, 0));
            CommonSettings.ScreenSettings._3DSettings.Translations.X = 0;
            CommonSettings.ScreenSettings._3DSettings.Translations.Y = -200;

            PrepareScreenDataandDrawBitmap();
            StellarobjectSystemText = SetStringStellarSystemInfo();
            SendMessagetoMessageWindow(2);

        }

        public string CalculatePathFromShiptoDestinationStar()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn && CelestialBodies.StellarObjectSelectedOnScreen != null)
            {
                CelestialBodies.StellarPathfromSourcetoDestination = CelestialBodies.CalculateShortestPathfromShiptoStar(Ships.CargoShips[Ships.CargoShips.Count() - 1].DestinationStellarObject, 1);
                CommonSettings.ScreenSettings.DisplayButtonCalculateShiptoStellarObject = false;

                PrepareScreenDataandDrawBitmap();

                return CelestialBodies.StellarPathfromSourcetoDestination.Count.ToString();
            }
            return "Gamedata not set";
        }
        public string CalculatePathtoDestinationStar() //calculate path from homestar to selected star.
        {
            //if (CommonSettings.ScreenSettings.IsGameDataDrawn)
            //{
            //    CelestialBodies.PathfromSourcetoDestination = CelestialBodies.CalculateShortestpath(1);
            //    SetImageFromStarArray();
            //    return CelestialBodies.PathfromSourcetoDestination.Count.ToString();
            //}
            return "Gamedata not set";
        }
        #endregion

        private void SendGalaxyGenerationSettings()
        {
            IGalaxyGenerationSettings galaxygenerationsettings = new GalaxyGenerationSettings
            {
                StartNumberofStellarObjects = CelestialBodies.StartNumberofStellarObjects,
                StartNumberofShips = Ships.StartNumberofCargoShips,
                InitializeStellarObjectsinBar = CelestialBodies.InitializeStellarObjectsinBar,
                InitializeStellarObjectsinBulge = CelestialBodies.InitializeStellarObjectsinBulge,
                InitializeStellarObjectsinSpiralArms = CelestialBodies.InitializeStellarObjectsinSpiralArms,
                InitializeStellarObjectsintDisc = CelestialBodies.InitializeStellarObjectsinDisc,
                DrawStarsinCentre = CelestialBodies.DrawStarsinCentre,
                MinimumDistancefromCentre = CelestialBodies.MinimumDistancefromCentre,
                MaximumRadiusofBulge = CelestialBodies.MaximumRadiusofBulge,
                SpiralWindedness = CelestialBodies.SpiralWindedness
            };
            foreach (Window window in Application.Current.Windows)
            {
                if (window is GalaxyGenerationSettingsWindow)
                {
                    window.Close();
                }
            }
            GalaxyGenerationSettingsWindow galaxygenerationsettingswindow = new GalaxyGenerationSettingsWindow
            {
                ResizeMode = ResizeMode.NoResize
            };
            galaxygenerationsettingswindow.Show();
            EventSystem.Publish<TickerSymbolGalaxyGenerationSettings>(new TickerSymbolGalaxyGenerationSettings { GalaxyGenerationSettings = galaxygenerationsettings });
        }

        private void SendMessagetoMessageWindow(int whatmessagetodisplay)
        {
            string Title = "";
            string Message = "";
            
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MessageWindow)
                {
                    window.Close();
                }
            }
            switch (whatmessagetodisplay)
            {
                case 1:
                    {
                        Title = "Initialisation results";
                        Message = SetStringGeneralInitialisationResults();
                        break;
                    }

                case 2:
                    {
                        Title = "Generation results";
                        Message = SetStringCelestialBodyInitialisationResults();
                        break;
                    }
            }
            MessageWindow messagewindow = new MessageWindow
            {
                Title = Title,
                ResizeMode = ResizeMode.NoResize
            };
            messagewindow.Show();
            EventSystem.Publish<TickerSymbolSelectedMessage>(new TickerSymbolSelectedMessage { MessageString = Message });
        }
        public void SetGalaxyGenerationSettings(TickerSymbolGalaxyGenerationSettings msg)
        {
            CelestialBodies.StartNumberofStellarObjects = msg.GalaxyGenerationSettings.StartNumberofStellarObjects;
            Ships.StartNumberofCargoShips = msg.GalaxyGenerationSettings.StartNumberofShips;
            CelestialBodies.SpiralWindedness = msg.GalaxyGenerationSettings.SpiralWindedness;
            CelestialBodies.InitializeStellarObjectsinBar = msg.GalaxyGenerationSettings.InitializeStellarObjectsinBar;
            CelestialBodies.InitializeStellarObjectsinBulge = msg.GalaxyGenerationSettings.InitializeStellarObjectsinBulge;
            CelestialBodies.InitializeStellarObjectsinDisc = msg.GalaxyGenerationSettings.InitializeStellarObjectsintDisc;
            CelestialBodies.InitializeStellarObjectsinSpiralArms = msg.GalaxyGenerationSettings.InitializeStellarObjectsinSpiralArms;
            CelestialBodies.DrawStarsinCentre = msg.GalaxyGenerationSettings.DrawStarsinCentre;
            CelestialBodies.MaximumRadiusofBulge = msg.GalaxyGenerationSettings.MaximumRadiusofBulge;
            CelestialBodies.MinimumDistancefromCentre = msg.GalaxyGenerationSettings.MinimumDistancefromCentre;
        }
        private void SetFoodandProductionStrings(TickerSymbolTotalAmountofFoodandPopulation msg)
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn == true)
            {
                OverviewText = $" Total Population = {msg.TotalPopulationEndofTurn} \n Total Food = {msg.TotalFoodEndofTurn} \n Total Deaths this turn = {msg.DeathsthisTurn} \n Produced food this turn = {msg.ProducedFoodperTurn} \n Spoiled food this turn = {msg.SpoiledFoodperTurn} \n Total new people this turn = {msg.NewcomersperTurn} \n Total Births this turn {msg.BirthsperTurn}";
            }
        }
        private void SetDate()
        {
            int year = (int)(TurnCounter / 200);
            //int month = (int)(TurnCounter / 20) - year * 10;
            int day = TurnCounter - year * 200; // - month * 20;
            StarDate = $"Year  : {year}  Day : {day}"; // Month : {month + 1} 
        }

        #region windowsapio
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion
        #region What happens in a timertick
        public void DispatcherTimer_Tick(object sender, EventArgs e) //Set timer parameters and start
        {
            FastRandom Rand = new FastRandom();
            if (Ships.MoveShips == true)
            {

                if (TurnCounter % 30 == 0)
                {
                    //tickrate per second 
                    Console.WriteLine($"Turns : {TurnCounter} Seconds passed : {(DateTime.Now.Subtract(TickTimer).TotalSeconds)}   Ticks per second :  " +  (TurnCounter / (DateTime.Now.Subtract(TickTimer).TotalSeconds) ));
                }
                // yearly occurences
                if (TurnCounter % 200 == 0)
                {

                }
                ////monthly occurences 
                if (TurnCounter % 20 == 0)
                {

                }
                //daily occurences
                #region actions specific to Celestial Bodies 
                CelestialBodies.OrbitalBodyDynamics(BaseCollections.BuildingTypes, Rand, TurnCounter, FunctionalGroupCollection.FunctionalGroups, CompoundCollection.Compounds);
                #endregion
                #region actions specific to Ships and Fleets
                // Responsibilities for adding and removing cargo from ships lies with ships. Responsibility of adding and removing elements from central hubs lies with central hubs.   
                Ships.PerformActions(Rand, ShipConstants);
                #endregion

                #region actions requiring both Ships, Mining facilities and/or stellarobjects

                #endregion
                // add in Ships.Performactions some trade routines
                // to determine where the ship is headed.  the ship's destination depends on the Demand situation of neighboring star system 
                // The ship can then determine which of its neighbours has stuff it needs. 

                SelectedShipSystemText = SetStringSelectedShipInfo();
                StellarobjectSystemText = SetStringStellarSystemInfo();
                SetDate();
                PrepareScreenDataandDrawBitmap();
                TurnCounter += 1;
            }
        }
        #endregion
    }
}

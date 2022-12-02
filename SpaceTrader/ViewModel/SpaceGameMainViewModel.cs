using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
        private string _stellarobjectsystemtext;
        private string _stardate;
        private string _selectedshipsystemtext;
        
        protected List<string> _items;
        protected string _currentitem;
        protected ImageSource _testimage;
        #endregion

        #region properties. Also Subclasses
       

        public DateTime TickTimer { get; set; } //  used for calculating framerate
        public int TurnCounter { get; set; }
        /// Settings :  All general gameplay-unrelated settings needed to play the game, such as :
        /// Mouse settings, screen settings, graphics settings, bitmap settings, timer settings etc.
        public GeneralSettings CommonSettings { get; set; } = new GeneralSettings();
        /// BaseTaxonomyCollections : Set of taxonomy collections that are needed to generate specific gamedata.
        /// Collections include, Domains of life down to individual species
        public BaseTaxonomyCollections BaseTaxonomyCollections { get; set; } = new BaseTaxonomyCollections();
        /// BaseCollections : Set of generic collections that are needed to generate specific gamedata.
        /// Collections include, resource lists, resourcegroup lists, orbital-body types, stellarobject-types etc.
        public BaseCollections BaseCollections { get; set; } = new BaseCollections();
        /// class for ungeneric Ship data and methods to use in game.
        public ShipViewModel Ships { get; set; } = new ShipViewModel();
        /// class for ungeneric CelestialBody data and methods to use in game.  Initialized in ViewModel Initializer with arguments
        public CelestialBodyViewModel CelestialBodies { get; set; } = new CelestialBodyViewModel();
        /// class for general constants, solar constants, earth constants and such
        ISolarConstants SolarConstants { get; } = new BaseConstants.SolarConstants();
        IPhysicalConstants PhysicalConstants { get; } = new BaseConstants.PhysicalConstants();
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
        public string StellarobjectSystemText
        {
            get { return _stellarobjectsystemtext; }
            set
            {
                _stellarobjectsystemtext = value;
                OnPropertyChanged();
            }
        }
        public List<String> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public string CurrentItem
        {
            get { return _currentitem; }
            set
            {
                _currentitem = value;
                SetHighLightedStellarObjects();
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

        #region constructor/initializer
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
                //MessageBox.Show("Thank you for using this application!");
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
            ISetHomeStar = new RelayCommand(RelaySetHomeStar, param => this.canExecute);
            IRedrawScreen = new RelayCommand(RelayRedrawScreen, param => this.canExecute);
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
        public ICommand ISetHomeStar { get; set; }
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
                    Ships.BMoveShips = !Ships.BMoveShips;
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
        private System.Windows.Point GetMousePosition(System.Windows.Point e)
        {
            return new System.Windows.Point(e.X - 25, e.Y - 35); // variables come from width and height of other control elements on the form. For now. this is ok. It needs to be documented
        }
        private void RelayMouseClickCommandExecute(MouseButtonEventArgs e)
        {
            System.Windows.Point position = GetMousePosition(e.GetPosition(Application.Current.MainWindow)); 
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (CommonSettings.MouseSettings.MousepressedLeft == false)
                {
                    CommonSettings.MouseSettings.MousePosWhenPressedLeft = new System.Windows.Point(position.X, position.Y);
                    CommonSettings.MouseSettings.MousepressedLeft = true;
                }
                if (CommonSettings.ScreenSettings.IsGameDataDrawn)
                {
                    CommonSettings.MouseSettings.MousePosWhenPressedLeftA = new System.Windows.Point(Ships.ShipSelectedonScreen.ScreenCoordinates.X, Ships.ShipSelectedonScreen.ScreenCoordinates.Y);   //position.X - 150, position.Y);
                    CelestialBodies.SetActiveStar(position);
                    Ships.SetActiveShip(position);
                    SetImageFromStarArray();
                    SelectedShipSystemText = SetSelectedShipInfotoString();
                    StellarobjectSystemText = SetStellarSystemInfotoString();
                }
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
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
                SetImageFromStarArray();
                StellarobjectSystemText = SetStellarSystemInfotoString();
            }
            //else if (e.RightButton == MouseButtonState.Pressed)
            //{
            //    Console.WriteLine("right");
            //}
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
                        CommonSettings.ScreenSettings.Point3DSettings.Translations.X -= 25;
                    }
                    else
                    {
                        CommonSettings.ScreenSettings.Point3DSettings.Translations.X += 25;
                    }
                    if (position.Y < CommonSettings.MouseSettings.MousePosWhenPressedLeft.Y)
                    {
                        CommonSettings.ScreenSettings.Point3DSettings.Translations.Y -= 25;
                    }
                    else
                    {
                        CommonSettings.ScreenSettings.Point3DSettings.Translations.Y += 25;

                    }
                }
                SetImageFromStarArray();
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
                    CommonSettings.ScreenSettings.Point3DSettings.RotationAngles.X -= CommonSettings.ScreenSettings.DeltaRotationAngle; 
                }
                else
                {
                    CommonSettings.ScreenSettings.Point3DSettings.RotationAngles.X += CommonSettings.ScreenSettings.DeltaRotationAngle;
                }
                if (position.Y < CommonSettings.MouseSettings.MousePosWhenPressedRight.Y)
                {
                    CommonSettings.ScreenSettings.Point3DSettings.RotationAngles.Z += CommonSettings.ScreenSettings.DeltaRotationAngle;
                }
                else
                {
                    CommonSettings.ScreenSettings.Point3DSettings.RotationAngles.Z -= CommonSettings.ScreenSettings.DeltaRotationAngle;
                }
                SetImageFromStarArray();
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
                    CommonSettings.ScreenSettings.Point3DSettings.ScaleFactor *= 2;
                }
                else
                {
                    CommonSettings.ScreenSettings.Point3DSettings.ScaleFactor /= 2;
                }
                SetImageFromStarArray();
            }

        }

        private void RelayShowCelestialBodyInfoonScreen(object obj) => SendMessagetoMessageWindow(2);
        private void RelayShowGameInitialisationResultsonScreen(object obj) => SendMessagetoMessageWindow(1);
        private void RelayShowGalacticGenerationSettingsScreen(object obj) => SendGalaxyGenerationSettings();

        private void RelaySetHighlightedStellarObjects(object obj) => SetHighLightedStellarObjects();
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
        private void RelaySetHomeStar(object obj) => CelestialBodies.SetHomeStar();
        private void RelayRedrawScreen(object obj) => SetImageFromStarArray();

        #endregion

        #region assorted viewmodel methods outside of the Icommand-Relay pipeline. most are called by the RelayCommandfunctions
        public void Initialise()
        {
            //CelestialBodies.PropertyChanged  += (s, e) => Console.WriteLine(e.PropertyName);
            CommonSettings.BitmapDataSettings.SetBitmapData(CommonSettings.ScreenSettings.ScreenWidth, CommonSettings.ScreenSettings.ScreenHeight);   //add loadsettingsfromfile
            CommonSettings.Timer.SetTimer();
            TurnCounter = 1;
            OverviewText = SetOverviewInitString();
            CommonSettings.Timer.ClockTimer.Interval = new TimeSpan(0,0,0,0, 30);
            CommonSettings.Timer.ClockTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            Items = SetCelestialBodyItemSource();
        }

        private void SetHighLightedStellarObjects()
        {
            foreach (StellarObject stellarobject in CelestialBodies.StellarObjects)
            {
                stellarobject.BHighlightonScreen = stellarobject.StellarType.Name == CurrentItem;
            }
            SetImageFromStarArray();
        }
        private List<string> SetCelestialBodyItemSource()
        {
            List<string> tmplist = new List<string>();
            foreach (BaseTypes.CelestialBodyType celestialbodytype in BaseCollections.StellarObjectTypes)
            {
                tmplist.Add(celestialbodytype.Name);
            }
            return tmplist;
        }
        private void LoadConfigIni()
        {
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/config.ini")))
            {
                if (line.Length > 0 && !(line.Substring(0,1) =="/"))
                {
                    Console.WriteLine(line);
                    splitstring = line.Split('=');
                    Console.WriteLine(splitstring[0]);
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
                        case "StartNumberofShips":
                            Ships.StartNumberofCargoShips = Convert.ToInt32(splitstring[1]);
                            break;
                    }
                }
            }
            Console.WriteLine(CommonSettings.ScreenSettings.ScreenWidth + "  " + CommonSettings.ScreenSettings.ScreenHeight + " " + CelestialBodies.StartNumberofStellarObjects + " " + Ships.StartNumberofCargoShips);
        }

        public void SetImageFromStarArray()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn)
            {
                if (CommonSettings.ScreenSettings.OldScreenHeight != (int)Application.Current.MainWindow.ActualHeight|| CommonSettings.ScreenSettings.OldScreenWidth != (int)Application.Current.MainWindow.ActualWidth)
                {

                    CommonSettings.ScreenSettings.ScreenWidth = (int)Application.Current.MainWindow.ActualWidth;
                    CommonSettings.ScreenSettings.ScreenHeight = (int)Application.Current.MainWindow.ActualHeight;
                    CommonSettings.ScreenSettings.OldScreenWidth = (int)Application.Current.MainWindow.ActualWidth;
                    CommonSettings.ScreenSettings.OldScreenHeight = (int)Application.Current.MainWindow.ActualHeight;
                    CommonSettings.BitmapDataSettings.Pixels = null;
                    CommonSettings.BitmapDataSettings.Pixels1d = null;
                    GC.Collect(); //find different method for resizing array without nulling and garbage collection.  At the moment, without GC, there is a memory leak here.
                    CommonSettings.BitmapDataSettings.Pixels = new byte[CommonSettings.ScreenSettings.ScreenHeight, CommonSettings.ScreenSettings.ScreenWidth, 4];
                    CommonSettings.BitmapDataSettings.Pixels1d = new byte[CommonSettings.ScreenSettings.ScreenHeight * CommonSettings.ScreenSettings.ScreenWidth * 4];
                    CommonSettings.BitmapDataSettings.Rect = new Int32Rect(0, 0, CommonSettings.ScreenSettings.ScreenWidth, CommonSettings.ScreenSettings.ScreenHeight);
                }
                BitmapDataCalculations.CalculatePointsafterChange(CelestialBodies.StellarObjects, Ships.CargoShips, CommonSettings.ScreenSettings.Point3DSettings, CommonSettings.BitmapDataSettings.bitmapadjustvector);
                TestImage = DisplayFunctions.SetImageFromStarArray(CommonSettings.ScreenSettings.ScreenWidth, CommonSettings.ScreenSettings.ScreenHeight, CommonSettings.BitmapDataSettings.Pixels, CommonSettings.BitmapDataSettings.Pixels1d, CelestialBodies.StellarObjects, Ships.CargoShips, CommonSettings.BitmapDataSettings.GrdBmp, CommonSettings.BitmapDataSettings.Rect, CommonSettings.ScreenSettings.BDrawLines, CelestialBodies.StellarObjectSelectedOnScreen, CelestialBodies.StellarPathfromSourcetoDestination, Ships.ShipSelectedonScreen, CommonSettings.ScreenSettings.Point3DSettings.ScaleFactor);
            }
        }
        public void InitialiseShips()
        {
            Ships.InitializeShips(CelestialBodies.StellarObjects, BaseCollections.EconomicEntities);
        }
        public void Pause()
        {
            Ships.BMoveShips = false;
            CommonSettings.ScreenSettings.GamePaused = true;
        }
        public void Unpause()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn != true)
            {
                return;
            }
            TickTimer = DateTime.Now;
            Ships.BMoveShips = true;
            CommonSettings.ScreenSettings.GamePaused = false;
        }

        //not used but handy for the future. string containing 3d-actions settings.
        public string SetTransformationsString()
        {
            StringBuilder VariableTransformationString = new StringBuilder();
            VariableTransformationString.AppendLine("Transformations");
            VariableTransformationString.AppendLine($"Translations   ->X : {CommonSettings.ScreenSettings.Point3DSettings.Translations.X} , Y : {CommonSettings.ScreenSettings.Point3DSettings.Translations.Y} ");
            VariableTransformationString.AppendLine($"Scalefactor    -> : {CommonSettings.ScreenSettings.Point3DSettings.ScaleFactor}");
            VariableTransformationString.AppendLine($"Rotationangles -> X : {CommonSettings.ScreenSettings.Point3DSettings.RotationAngles.X} , Y : {CommonSettings.ScreenSettings.Point3DSettings.RotationAngles.Y} , Z : {CommonSettings.ScreenSettings.Point3DSettings.RotationAngles.Z} ");
             return VariableTransformationString.ToString();
        }
        #region set overviewtextblockmethods
       
        public string SetOverviewInitString()
        {
            StringBuilder overviewstring = new StringBuilder();
            overviewstring.AppendLine("\nTaxonomy results : \n");
            overviewstring.AppendLine($" Domains generated : {BaseTaxonomyCollections.Domains.Count} ");
            overviewstring.AppendLine($" Kingdoms generated : {BaseTaxonomyCollections.Kingdoms.Count} ");
            overviewstring.AppendLine($" Phyla generated : {BaseTaxonomyCollections.Phyla.Count} ");
            overviewstring.AppendLine($" SubPhyla generated : {BaseTaxonomyCollections.SubPhyla.Count} ");
            overviewstring.AppendLine($" Classes generated : {BaseTaxonomyCollections.Classes.Count} ");
            overviewstring.AppendLine($" Orders generated : {BaseTaxonomyCollections.Orders.Count} ");
            overviewstring.AppendLine($" Families generated : {BaseTaxonomyCollections.Families.Count} ");
            overviewstring.AppendLine($" Geni generated : {BaseTaxonomyCollections.Geni.Count} ");
            overviewstring.AppendLine($" Species generated : {BaseTaxonomyCollections.Species.Count} ");
            overviewstring.AppendLine("\nGeneral other collection results : \n");
            overviewstring.AppendLine($" Resources generated : {BaseCollections.Resources.Count} ");
            overviewstring.AppendLine($" Resourcegroups generated : {BaseCollections.ResourceGroups.Count} ");
            overviewstring.AppendLine($" Orbital body-types generated : {BaseCollections.OrbitalbodyTypes.Count} ");
            overviewstring.AppendLine($" Stellar-types generated : {BaseCollections.StellarObjectTypes.Count} ");
            overviewstring.AppendLine($" Building-types generated : {BaseCollections.BuildingTypes.Count} ");
            overviewstring.AppendLine($" Economic entities generated : {BaseCollections.EconomicEntities.Count} ");
            overviewstring.AppendLine($" Technology level-types generated : {BaseCollections.TechLevelCollection.Count} ");
            overviewstring.AppendLine($"\nSolar constants loaded from file :\n");
            overviewstring.AppendLine($" Mass {SolarConstants.Mass} kg");
            overviewstring.AppendLine($" Radius {SolarConstants.Radius} km");
            overviewstring.AppendLine($" Temperature {SolarConstants.Temperature} Kelvin");
            overviewstring.AppendLine($" Luminosity {SolarConstants.Luminosity} Watt (J/s) (1 kg⋅m\u00B2⋅s\u207B\u00B3)");
            overviewstring.AppendLine($"\nPhysical constants loaded from file :\n");
            overviewstring.AppendLine($"S tefan Boltzmann constant {PhysicalConstants.StefanBoltzmannConstant} W⋅m\u207B\u00B2⋅k\u207B\u2074");
            return overviewstring.ToString();
        }
        public string SetSelectedShipInfotoString()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn != true)
            {
                return "game data not set";
            }
            StringBuilder SelectedShipInfotoString = new StringBuilder();
            SelectedShipInfotoString.AppendLine($"Ship : {Ships.ShipSelectedonScreen.Name} ");
            SelectedShipInfotoString.AppendLine($"Owned by {Ships.ShipSelectedonScreen.EconomicEntity.Name}");
            SelectedShipInfotoString.AppendLine($"Destination : {Ships.ShipSelectedonScreen.DestinationStellarObject.Name}");
            return SelectedShipInfotoString.ToString();
        }
        public StringBuilder SetOrbitalbodystringforOverview(OrbitalBody orbitalbody)
        {
            StringBuilder overviewstring = new StringBuilder();
            StringBuilder tmpResourcestring = new StringBuilder();
            overviewstring.AppendLine($" - {orbitalbody.Name} : {orbitalbody.OrbitalBodyType.Name}");
            overviewstring.AppendLine($"      - Mass : {orbitalbody.Mass:F3} earth mass : Radius {orbitalbody.Radius} km. ");
            foreach (SpeciesperOrbitalBody tspecies in orbitalbody.SpeciesonOrbitalBody)
            {
                overviewstring.AppendLine($"      - {tspecies.Species.Genus.Family.Order.Name} -> {tspecies.Species.Genus.Family.Name} -> {tspecies.Species.Name} ");
                overviewstring.AppendLine($"      - Population size : {tspecies.PopulationSize:F0} Reproduction rate : {tspecies.ReproductionRate:F2}");
            }
            overviewstring.AppendLine($"      - Housing available for population: {orbitalbody.PopulationHousing}");
            overviewstring.AppendLine($"      - Distance to Star : {orbitalbody.AverageDistanceToCentralStellarObject/150000000000:F2} AU ");
            overviewstring.AppendLine($"      - Solar power per m² : {orbitalbody.SolarPowerperM2:F2} Watt");
            overviewstring.AppendLine($"      - Food : {(int)orbitalbody.Food} -> Food storage : {orbitalbody.FoodStorage}");
            overviewstring.AppendLine($"      - FoodModifierfromBuildings : {orbitalbody.FoodModifierfromBuildings:F3}");
            overviewstring.AppendLine($"      - BasNaturalHabitationModifier : {orbitalbody.BaseNaturalHabitationModifier:F3}");
            overviewstring.AppendLine($"      - NaturalHabitationModifier : {orbitalbody.NaturalHabitationModifier:F3}");
            overviewstring.AppendLine($"      - PopulationModifierfromBuldings : {orbitalbody.PopulationModifierfromBuildings:F3}");
            overviewstring.AppendLine($"      - NaturalBirthPercentage : {orbitalbody.NaturalBirthsperTurnPercentage:F3}");
            overviewstring.AppendLine($"      - NaturalDeathPercentage : {orbitalbody.NaturalDeathsperTurnPercentage:F3}");
            overviewstring.AppendLine($"      - BaseNaturalDeathPercentage : {orbitalbody.BaseNaturalDeathsperTurnPercentage:F3}");
            overviewstring.AppendLine($"      - NaturalImmigrationperTurnLinear : {orbitalbody.NaturalImmigrationperTurnLinear:F3}");
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
            if (orbitalbody.ResourceGroups.Count == 0)
            {
                tmpResourcestring.Append($"   - No resource-modifiers present on {orbitalbody.Name}");
            }
            else
            {
                overviewstring.AppendLine("   - Resourcegroups present : ");
                overviewstring.Append("        ");
                foreach (ResourceGroup resourcegroup in orbitalbody.ResourceGroups)
                {
                    tmpResourcestring.Append($"{resourcegroup.Name} , ");
                }
                tmpResourcestring.Remove(tmpResourcestring.Length - 1, 1);
            }

            overviewstring.AppendLine(tmpResourcestring.ToString());
            overviewstring.AppendLine($"   - Resources present : -> ");
            foreach (ResourceInStorageperBody resourceinstorage in orbitalbody.ResourcesinStorage)
            {
                if (resourceinstorage.Amount == 0)
                {

                }
                else if (resourceinstorage.Amount > 10000)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {resourceinstorage.Amount / 1000:F0} ton");
                }
                else if (resourceinstorage.Amount > 1000)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {(resourceinstorage.Amount/1000):F1} ton");
                }
                else if (resourceinstorage.Amount > 100)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {(resourceinstorage.Amount):F0} kilogram");
                }
                else if (resourceinstorage.Amount > 10)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {resourceinstorage.Amount:F1} kilogram");
                }
                else if (resourceinstorage.Amount > 1)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {resourceinstorage.Amount:F2} kilogram");
                }
                else if (resourceinstorage.Amount > 0.1)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {resourceinstorage.Amount * 1000:F1} gram");
                }
                else if (resourceinstorage.Amount > 0.01)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {(resourceinstorage.Amount * 1000):F2} gram");
                }
                else if (resourceinstorage.Amount > 0.001)
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}   {(resourceinstorage.Amount * 1000):F3} gram");
                }
                else
                {
                    overviewstring.AppendLine($"          -{resourceinstorage.Resource.Name}  < 0.001 gram");
                }
            }
            return overviewstring;
        }

        public string SetStellarSystemInfotoString()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn != true)
            {
                return "game data not set";
            }
            StringBuilder overviewstring = new StringBuilder();
            overviewstring.AppendLine("\nSTELLAR OBJECT : \n");
            overviewstring.AppendLine($" {CelestialBodies.StellarObjectSelectedOnScreen.Name} -> {CelestialBodies.StellarObjectSelectedOnScreen.StellarType.Name}");
            overviewstring.AppendLine($" Mass : {CelestialBodies.StellarObjectSelectedOnScreen.Mass:F2} solar masses.");
            overviewstring.AppendLine($" Age : {CelestialBodies.StellarObjectSelectedOnScreen.Age:N0}.000 years.");
            overviewstring.AppendLine($" Radius : {CelestialBodies.StellarObjectSelectedOnScreen.Radius:N0} kilometer.");
            overviewstring.AppendLine($" Surface temperature : {CelestialBodies.StellarObjectSelectedOnScreen.SurfaceTemperature * 1000:F0} degrees °C. ");
            overviewstring.AppendLine($" Luminosity : {CelestialBodies.StellarObjectSelectedOnScreen.Luminosity:E4}  kg⋅m\u00B2⋅s\u207B\u00B3 .");
            overviewstring.AppendLine($" Absolute Magnitude : {CelestialBodies.StellarObjectSelectedOnScreen.AbsoluteMagnitude:F2}.");
            overviewstring.AppendLine($" Lower boundary habitable zone : {CelestialBodies.StellarObjectSelectedOnScreen.MinimumHabitableZoneRadius / 1000:E3} km / {CelestialBodies.StellarObjectSelectedOnScreen.MinimumHabitableZoneRadius / 150000000000:F2} AU.");
            overviewstring.AppendLine($" Upper boundary habitable zone : {CelestialBodies.StellarObjectSelectedOnScreen.MaximumHabitableZoneRadius / 1000:E3} km / {CelestialBodies.StellarObjectSelectedOnScreen.MaximumHabitableZoneRadius / 150000000000:F2} AU..");
            overviewstring.AppendLine($" Maximum orbital body distance : {CelestialBodies.StellarObjectSelectedOnScreen.MaximumOrbitalBodyDistanceFromStar / 1000:E3} km / {CelestialBodies.StellarObjectSelectedOnScreen.MaximumOrbitalBodyDistanceFromStar / 150000000000:F2} AU..\n");
            overviewstring.AppendLine($"STARLANES : ");
            foreach (Starlane starlane in CelestialBodies.StellarObjectSelectedOnScreen.StarLanes)
            {
                overviewstring.AppendLine($" - To {starlane.To.Name} , techlevel : {starlane.TechLevelRequiredforTravel.Techlevel}    length : {starlane.Length:F2} lightyears. " );
            }    
            overviewstring.AppendLine("\nORBITAL BODIES : ");
            foreach (OrbitalBody orbitalbody in CelestialBodies.StellarObjectSelectedOnScreen.Orbitalbodies)
            {
                overviewstring.Append(SetOrbitalbodystringforOverview(orbitalbody));
                
                if (orbitalbody.NaturalSatellites.Count == 0)
                {
                    overviewstring.AppendLine($"   - No natural satellites. ");
                }

                else
                {
                    overviewstring.AppendLine($"   - Natural satellites : ");
                    foreach (OrbitalBody naturalsatellite in orbitalbody.NaturalSatellites)
                    {
                        overviewstring.Append(SetOrbitalbodystringforOverview(naturalsatellite)); 
                    }
                }
            }
            return overviewstring.ToString();
        }
        public string CelestialBodyInfoToString()
        {
            StringBuilder overviewstring = new System.Text.StringBuilder();
            var stellarinfo = new List<StellarObject>();
            var orbitalbodyinfo = new List<OrbitalBody>();
            var naturalsatelliteinfo = new List<OrbitalBody>();
            int totalcntr = 0;
            overviewstring.AppendLine($"CELESTIAL BODY GENERATION RESULTS  \n");
            overviewstring.AppendLine($" ---Stellar objects  : ");
            foreach (BaseTypes.StellarObjectType stellartype in BaseCollections.StellarObjectTypes)
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
            foreach (BaseTypes.OrbitalBodyType orbitalbodytype in BaseCollections.OrbitalbodyTypes)
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
            foreach (BaseTypes.OrbitalBodyType naturalsatellite in BaseCollections.OrbitalbodyTypes)
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
                    orbitalbody.ResourcesinStorage.Clear();
                    orbitalbody.ResourceGroups.Clear();
                    orbitalbody.Buildings.CollectionChanged -= (obj, e) => orbitalbody.RecalculateModifiersandProperties();
                    orbitalbody.Buildings.ItemPropertyChanged -= (obj, e) => orbitalbody.RecalculateModifiersandProperties();
                    orbitalbody.Buildings.Clear();
                    orbitalbody.NaturalSatellites.Clear();
                }
                stellarobject.Orbitalbodies.Clear();
                stellarobject.StarLanes.Clear();
            }
            Ships.CargoShips.Clear();
            CelestialBodies.StellarObjects.Clear();

            await CelestialBodies.SetCelestialBodyDatasAsync(CommonSettings.ScreenSettings.ScreenWidth, BaseTaxonomyCollections.Species,   BaseCollections.OrbitalbodyTypes, BaseCollections.StellarObjectTypes, BaseCollections.ResourceGroups, BaseCollections.TechLevelCollection, BaseCollections.BuildingTypes, BaseCollections.Resources, PhysicalConstants, SolarConstants);
            InitialiseShips();
            CelestialBodies.SetActiveStar(new System.Windows.Point(0, 0));
            Ships.SetActiveShip(new System.Windows.Point(0, 0));
            CommonSettings.ScreenSettings.Point3DSettings.Translations.X = 0;
            CommonSettings.ScreenSettings.Point3DSettings.Translations.Y = -200;
            
            SetImageFromStarArray();
            StellarobjectSystemText = SetStellarSystemInfotoString();
            SendMessagetoMessageWindow(2);

        }

        public string CalculatePathFromShiptoDestinationStar()
        {
            if (CommonSettings.ScreenSettings.IsGameDataDrawn && CelestialBodies.StellarObjectSelectedOnScreen != null)
            {
                CelestialBodies.StellarPathfromSourcetoDestination = CelestialBodies.CalculateShortestPathfromShiptoStar(Ships.CargoShips[Ships.CargoShips.Count() - 1].DestinationStellarObject, 1);
                CommonSettings.ScreenSettings.DisplayButtonCalculateShiptoStellarObject = false;

                SetImageFromStarArray();

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
                InitBar = CelestialBodies.InitBar,
                InitBulge = CelestialBodies.InitBulge,
                InitSpiralArms = CelestialBodies.InitSpiralArms,
                InitDisc = CelestialBodies.InitDisc,
                DrawStarsinCentre = CelestialBodies.DrawStarsinCentre,
                MinDistancefromCentre = CelestialBodies.MinDistancefromCentre,
                MaxBulgeRadius = CelestialBodies.MaxBulgeRadius,
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
                        Message = SetOverviewInitString();
                        break;
                    }

                case 2:
                    {
                        Title = "Generation results";
                        Message = CelestialBodyInfoToString();
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
            CelestialBodies.InitBar = msg.GalaxyGenerationSettings.InitBar;
            CelestialBodies.InitBulge = msg.GalaxyGenerationSettings.InitBulge;
            CelestialBodies.InitDisc = msg.GalaxyGenerationSettings.InitDisc;
            CelestialBodies.InitSpiralArms = msg.GalaxyGenerationSettings.InitSpiralArms;
            CelestialBodies.DrawStarsinCentre = msg.GalaxyGenerationSettings.DrawStarsinCentre;
            CelestialBodies.MaxBulgeRadius = msg.GalaxyGenerationSettings.MaxBulgeRadius;
            CelestialBodies.MinDistancefromCentre = msg.GalaxyGenerationSettings.MinDistancefromCentre;
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
            int tmpyear = (int)(TurnCounter / 200);
            int tmpmonth = (int)(TurnCounter / 20) - tmpyear * 10;
            int tmpday = TurnCounter - tmpyear * 200 - tmpmonth * 20;
            StarDate = $"Year  : {tmpyear}  Month : {tmpmonth + 1}  Day : {tmpday}";
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
            if (Ships.BMoveShips == true)
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
                    CelestialBodies.BuildBuildingperOrbitalbody(BaseCollections.BuildingTypes);
                }
                //daily occurences

                CelestialBodies.MineResourcesperOrbitalbody();
                CelestialBodies.GrownFoodandPopulationperOrbitalbody();
                SelectedShipSystemText = SetSelectedShipInfotoString();
                StellarobjectSystemText = SetStellarSystemInfotoString();
                SetDate();
                TurnCounter += 1;
                Ships.MoveShipsNew();
                SetImageFromStarArray();
            }
        }
        #endregion
    }
}

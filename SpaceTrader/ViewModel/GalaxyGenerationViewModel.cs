using Common.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SpaceTrader
{
    public class GalaxyGenerationViewModel : BaseViewModel, INotifyPropertyChanged
    {
        #region fields
        private bool canExecute = true;
        protected bool _initializestellarobjectsinspiralarms;
        protected int _spiralwindedness;
        protected bool _initializestellarobjectsinbulge;
        protected int _maximumradiusofbulge;
        protected bool _initializestellarobjectsinbar;
        protected bool _initializestellarobjectsindisc;
        protected int _startnumberofships;
        protected bool _drawstarsincentre;
        protected int _startnumberofstellarobjects;
        protected int _minimumdistancefromcentre;
        protected string _actionstring;
        #endregion
        #region properties
        public bool CanExecute
        {
            get { return this.canExecute; }
            set
            {
                if (this.canExecute == value) { return; }
                this.canExecute = value;
            }
        }
        public int StartNumberofCargoShips
        {
            get { return _startnumberofships; }
            set
            {
                _startnumberofships = value;
                OnPropertyChanged();
            }
        }
        public int StartNumberofStellarObjects
        {
            get { return _startnumberofstellarobjects; }
            set
            {

                _startnumberofstellarobjects = value;
                OnPropertyChanged();
            }
        }
        public bool DrawStarsinCentre
        {
            get { return _drawstarsincentre; }
            set
            {
                _drawstarsincentre = value;
                OnPropertyChanged();
            }
        }
        public int MinimumDistancefromCentre
        {
            get { return _minimumdistancefromcentre; }
            set
            {
                _minimumdistancefromcentre = value;
                OnPropertyChanged();
            }
        }

        public bool InitializeStellarObjectsinSpiralArms
        {
            get { return _initializestellarobjectsinspiralarms; }
            set
            {
                if (!value) InitializeStellarObjectsinBar = false;
                _initializestellarobjectsinspiralarms = value;
                OnPropertyChanged();
            }
        }
        public int SpiralWindedness
        {
            get { return _spiralwindedness; }
            set
            {
                if (value < 1) _spiralwindedness = 1;
                else if (value > 15) _spiralwindedness = 15;
                else _spiralwindedness = value;
                OnPropertyChanged();
            }
        }
        public int MaximumRadiusofBulge
        {
            get { return _maximumradiusofbulge; }
            set
            {
                if (value < 50) _maximumradiusofbulge = 50;
                else if (value > 500) _maximumradiusofbulge = 500;
                else _maximumradiusofbulge = value;
                OnPropertyChanged();
            }
        }
        public bool InitializeStellarObjectsinBulge
        {
            get { return _initializestellarobjectsinbulge; }
            set
            {
                _initializestellarobjectsinbulge = value;
                OnPropertyChanged();
            }
        }
        public bool InitializeStellarObjectsinBar
        {
            get { return _initializestellarobjectsinbar; }
            set
            {
                _initializestellarobjectsinbar = value;
                OnPropertyChanged();
            }
        }
        public bool InitializeStellarObjectsinDisc
        {
            get { return _initializestellarobjectsindisc; }
            set
            {
                _initializestellarobjectsindisc = value;
                OnPropertyChanged();
            }
        }
        public string ActionString
        { 
            get { return _actionstring; }
            set
            {
                _actionstring = value;
                OnPropertyChanged();
            }
        }
        #endregion
        public GalaxyGenerationViewModel(Window window)
        {
            /* window events */
            window.Loaded += (sender, e) =>
            {
                EventSystem.Subscribe<TickerSymbolGalaxyGenerationSettings>(SetGalaxyGenerationSettings);
                ActionString = "";
            };

            window.Closed += (sender, e) =>
            {
                //MessageBox.Show("Thank you for using this application!");
            };
            ISaveSettings = new RelayCommand(RelaySaveSettings, param => this.canExecute);
        }

        public ICommand ISaveSettings { get; set; }
        private void SetGalaxyGenerationSettings(TickerSymbolGalaxyGenerationSettings msg)
        {
            StartNumberofStellarObjects = msg.GalaxyGenerationSettings.StartNumberofStellarObjects;
            StartNumberofCargoShips = msg.GalaxyGenerationSettings.StartNumberofShips;
            SpiralWindedness = msg.GalaxyGenerationSettings.SpiralWindedness;
            InitializeStellarObjectsinBar = msg.GalaxyGenerationSettings.InitializeStellarObjectsinBar;
            InitializeStellarObjectsinBulge = msg.GalaxyGenerationSettings.InitializeStellarObjectsinBulge;
            InitializeStellarObjectsinDisc = msg.GalaxyGenerationSettings.InitializeStellarObjectsintDisc;
            InitializeStellarObjectsinSpiralArms = msg.GalaxyGenerationSettings.InitializeStellarObjectsinSpiralArms;
            DrawStarsinCentre = msg.GalaxyGenerationSettings.DrawStarsinCentre;
            MaximumRadiusofBulge = msg.GalaxyGenerationSettings.MaximumRadiusofBulge;
            MinimumDistancefromCentre = msg.GalaxyGenerationSettings.MinimumDistancefromCentre;
        }
        private void RelaySaveSettings (object obj)
        {
            IGalaxyGenerationSettings galaxygenerationsettings = new GalaxyGenerationSettings
            {
                StartNumberofStellarObjects = StartNumberofStellarObjects,
                StartNumberofShips = StartNumberofCargoShips,
                InitializeStellarObjectsinBar = InitializeStellarObjectsinBar,
                InitializeStellarObjectsinBulge = InitializeStellarObjectsinBulge,
                InitializeStellarObjectsinSpiralArms = InitializeStellarObjectsinSpiralArms,
                InitializeStellarObjectsintDisc = InitializeStellarObjectsinDisc,
                DrawStarsinCentre = DrawStarsinCentre,
                MinimumDistancefromCentre = MinimumDistancefromCentre,
                MaximumRadiusofBulge = MaximumRadiusofBulge,
                SpiralWindedness = SpiralWindedness
            };
            EventSystem.Publish<TickerSymbolGalaxyGenerationSettings>(new TickerSymbolGalaxyGenerationSettings { GalaxyGenerationSettings = galaxygenerationsettings});
            ActionString = "Saved Settings.";
            foreach (Window window in Application.Current.Windows)
            {
                if (window != Application.Current.MainWindow)
                {
                    window.Close();
                }
            }

        }
    }
}

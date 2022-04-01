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
        protected bool _initspirals;
        protected int _spiralwindedness;
        protected bool _initbulge;
        protected int _maxbulgeradius;
        protected bool _initbar;
        protected bool _initdisc;
        protected int _startnumberofships;
        protected bool _bdrawstarsincentre;
        protected int _startnumberofstellarobjects;
        protected int _mindistancefromcentre;
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
            get { return _bdrawstarsincentre; }
            set
            {
                _bdrawstarsincentre = value;
                OnPropertyChanged();
            }
        }
        public int MinDistancefromCentre
        {
            get { return _mindistancefromcentre; }
            set
            {
                _mindistancefromcentre = value;
                OnPropertyChanged();
            }
        }

        public bool InitSpiralArms
        {
            get { return _initspirals; }
            set
            {
                if (!value) InitBar = false;
                _initspirals = value;
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
        public int MaxBulgeRadius
        {
            get { return _maxbulgeradius; }
            set
            {
                if (value < 50) _maxbulgeradius = 50;
                else if (value > 500) _maxbulgeradius = 500;
                else _maxbulgeradius = value;
                OnPropertyChanged();
            }
        }
        public bool InitBulge
        {
            get { return _initbulge; }
            set
            {
                _initbulge = value;
                OnPropertyChanged();
            }
        }
        public bool InitBar
        {
            get { return _initbar; }
            set
            {
                _initbar = value;
                OnPropertyChanged();
            }
        }
        public bool InitDisc
        {
            get { return _initdisc; }
            set
            {
                _initdisc = value;
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
            InitBar = msg.GalaxyGenerationSettings.InitBar;
            InitBulge = msg.GalaxyGenerationSettings.InitBulge;
            InitDisc = msg.GalaxyGenerationSettings.InitDisc;
            InitSpiralArms = msg.GalaxyGenerationSettings.InitSpiralArms;
            DrawStarsinCentre = msg.GalaxyGenerationSettings.DrawStarsinCentre;
            MaxBulgeRadius = msg.GalaxyGenerationSettings.MaxBulgeRadius;
            MinDistancefromCentre = msg.GalaxyGenerationSettings.MinDistancefromCentre;
        }
        private void RelaySaveSettings (object obj)
        {
            IGalaxyGenerationSettings galaxygenerationsettings = new GalaxyGenerationSettings
            {
                StartNumberofStellarObjects = StartNumberofStellarObjects,
                StartNumberofShips = StartNumberofCargoShips,
                InitBar = InitBar,
                InitBulge = InitBulge,
                InitSpiralArms = InitSpiralArms,
                InitDisc = InitDisc,
                DrawStarsinCentre = DrawStarsinCentre,
                MinDistancefromCentre = MinDistancefromCentre,
                MaxBulgeRadius = MaxBulgeRadius,
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

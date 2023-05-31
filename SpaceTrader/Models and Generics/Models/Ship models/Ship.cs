using Common.Constants;
using Common.Economy;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public interface IShip
    {
        Color Color { get; }
        Point3D FinalPosition { get; }
        EconomicEntity EconomicEntity { get; }
    }
    public class Ship : IShip,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region fields

        protected Vector3D _movevector;
        protected Point3D _beginposition;
        protected Point3D _movedposition;
        protected Point3D _rotatedpositionz;
        protected Point3D _rotatedpositionx;
        protected Point3D _scaledposition;
        protected Point3D _translatedposition;
        protected Point3D _finalposition;
        protected Point _screencoordinates;
        #endregion

        public Ship()
        {
            EconomicEntity = new EconomicEntity();
        }
        #region constructor
        public Ship(string name, Point3D position, int speed, StellarObject destinationstellarobject, StellarObject currentstellarobject, Color color, EconomicEntity economicentity, bool ownship, int maxhullintegrity, double fuelconsumption, double fuelamount, int baseamountrepairsperturn)
        {
            //currentstellarobject at the beginning of the simulation is the homestellarobject
            FuelAmount = fuelamount;
            FuelConsumption = fuelconsumption;
            HullIntegrity = maxhullintegrity;

            IsUnloaded = true;
            NeedsRefueling = false;
            NeedsRepairing = false;
            IsDocked = false;
            IsRepairing = false;
            IsOverhauling = false;
            IsRefueling = false;
            ShipInfoVisibleonScreen = false;
            HasDestination = false;
            HasMultiNodeDestination = false;
            ShipOwnedbyPlayer = ownship;

            ShipUpgradeLevel = 1;
            DockingDuration = 0;
            BaseRepairAmountperTurn = baseamountrepairsperturn;
            Speed = speed;

            Name = name;
            Color = color;
            HomeStellarObject = currentstellarobject;
            CurrentStellarObject = currentstellarobject;
            DestinationStellarObject = destinationstellarobject;

            EconomicEntity = economicentity;

            _beginposition = position;
            _movedposition = position;
            _rotatedpositionz = position;
            _rotatedpositionx = position;
            _scaledposition = position;
            _translatedposition = position;
            _finalposition = position;

        }
        #endregion
        #region properties
        public bool IsUnloaded { get; set; }
        public bool NeedsRefueling { get; set; }
        public bool NeedsRepairing { get; set; }
        public bool IsDocked {get; set;}
        public bool IsOverhauling { get; set; }
        public bool IsRepairing { get; set; }
        public bool IsRefueling { get; set; }
        public bool ShipInfoVisibleonScreen { get; set; }
        public bool ShipOwnedbyPlayer { get; set; }
        public bool HasDestination { get; set; }
        public bool HasMultiNodeDestination { get; set; }

        public double FuelAmount { get; set; }
        public double FuelConsumption { get; set; }
        public double HullIntegrity { get; set; }

        public int BaseRepairAmountperTurn { get; set; }
        public int ShipUpgradeLevel { get; set; }
        public int DockingDuration { get; set; }
        public int MoveCounter { get; set; }
        public int Speed { get; set; }

        public string Name { get; set; }

        public Color Color { get; set; }
        public EconomicEntity EconomicEntity { get; set; }
        public StellarObject HomeStellarObject { get; set; }
        public StellarObject DestinationStellarObject { get; set; }
        public StellarObject CurrentStellarObject { get; set; }
        public List<int> PathListfromSourcetoDestination = new List<int>();
        public Queue<int> PathQueuefromSourcetoDestination = new Queue<int>();
        public List<StellarObject> PathStellarObjectsListfromSourcetoDestination = new List<StellarObject>();
        public Queue<StellarObject> PathStellarObjectsQueuefromSourcetoDestination = new Queue<StellarObject>();


        public Point ScreenCoordinates
        {
            get { return _screencoordinates; }
            set {
                _screencoordinates = value;
                OnPropertyChanged();
            }
        }

        public Vector3D MoveVector
        {
            get { return _movevector; }
            set { _movevector = value; }
        }

        public Point3D BeginPosition
        {
            get { return _beginposition; }
            set { _beginposition = value; }
        }
        public Point3D MovedPosition
        {
            get { return _movedposition; }
            set { _movedposition = value; }
        }
        public Point3D RotatedPositionZ
        {
            get { return _rotatedpositionz; }
            set { _rotatedpositionz = value; }
        }
        public Point3D RotatedPositionX
        {
            get { return _rotatedpositionx; }
            set { _rotatedpositionx = value; }
        }
        public Point3D ScaledPosition
        {
            get { return _scaledposition; }
            set { _scaledposition = value; }
        }
        public Point3D TranslatedPosition
        {
            get { return _translatedposition; }
            set { _translatedposition = value; }
        }
        public Point3D FinalPosition
        {
            get { return _finalposition; }
            set { _finalposition = value;
                ScreenCoordinates = new Point( FinalPosition.X -150, FinalPosition.Z);
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// What happens to a ship at the moment of Docking
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="maxhullintegrity"></param>
        /// <param name="shipconstants"></param>
        public void DockingShipDynamics(FastRandom rand, int maxhullintegrity, IShipConstants shipconstants)
        {
            DockingDuration = shipconstants.BaseDockingDurationforLoadingUnloading; //time for loading and unloading
            IsDocked = true;
            if (rand.NextDouble() < shipconstants.BaseChanceofOverhaulingShipwhenDocked && !NeedsRepairing && !NeedsRefueling)
            {
                NeedsRefueling = false;
                NeedsRepairing = false;
                IsOverhauling = true;
                DockingDuration = shipconstants.BaseDockingDurationforOverhauling;  //length of overhauling-period
            }
            else if (NeedsRepairing)
            {
                DockingDuration = (int)(((maxhullintegrity * ShipUpgradeLevel) - HullIntegrity) / shipconstants.BaseAmountofDamageRepairedEachTurn);
                NeedsRepairing = false;
                NeedsRefueling = false;
                IsRepairing = true;
            }
            else if (NeedsRefueling)
            {
                DockingDuration = shipconstants.BaseDockingDurationforRefueling; //time for refueling
                NeedsRefueling = false;
                IsRefueling = true;
            }
        }
        #endregion
    }
}

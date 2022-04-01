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
        protected bool _shipinfovisibleonscreen;
        protected bool _ownship;
        protected EconomicEntity _economicentity;
        protected Color _color;
        protected StellarObject _currentstellarobject;
        protected StellarObject _destinationstellarobject;
        protected bool _hasmultinodedestination;
        protected int _movecounter;
        protected Vector3D _movevector;
        protected bool _hasdestination;
        protected int _speed;
        protected string _name;
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
            _economicentity = new EconomicEntity();
        }
        #region constructor
        public Ship(string name, Point3D position, int speed, StellarObject destinationstellarobject, StellarObject currentstellarobject, Color color, EconomicEntity economicentity, bool ownship)
        {
            _shipinfovisibleonscreen = false;
            _ownship = ownship;
            _economicentity = economicentity;
            _hasdestination = false;
            //ShipClass for speed later on
            _color = color;
            _name = name;
            _beginposition = position;
            _movedposition = position;
            _rotatedpositionz = position;
            _rotatedpositionx = position;
            _scaledposition = position;
            _translatedposition = position;
            _finalposition = position;
            _currentstellarobject = currentstellarobject;
            _destinationstellarobject = destinationstellarobject;
            _speed = speed;
        }
        #endregion
        #region properties

        public bool ShipInfoVisibleonScreen
        {
            get { return _shipinfovisibleonscreen; }
            set { _shipinfovisibleonscreen = value; }
        }
        public Point ScreenCoordinates
        {
            get { return _screencoordinates; }
            set {
                _screencoordinates = value;
                OnPropertyChanged();
            }
        }
        public List<int> PathListfromSourcetoDestination = new List<int>();
        public Queue<int> PathQueuefromSourcetoDestination = new Queue<int>();
        public List<StellarObject> PathStellarObjectsListfromSourcetoDestination = new List<StellarObject>();
        public Queue<StellarObject> PathStellarObjectsQueuefromSourcetoDestination = new Queue<StellarObject>();

        public bool OwnShip
        {
            get { return _ownship; }
            set { _ownship = value; }
        }
        public EconomicEntity EconomicEntity
        {
            get { return _economicentity; }
            set { _economicentity = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public bool HasDestination
        {
            get { return _hasdestination; }
            set { _hasdestination = value; }
        }
        public bool HasMultiNodeDestination
        {
            get { return _hasmultinodedestination; }
            set { _hasmultinodedestination = value; }
        }
        public Vector3D MoveVector
        {
            get { return _movevector; }
            set { _movevector = value; }
        }
        public int MoveCounter
        {
            get { return _movecounter; }
            set { _movecounter = value; }
        }
        public StellarObject DestinationStellarObject
        {
            get { return _destinationstellarobject; }
            set { _destinationstellarobject = value; }
        }
        public StellarObject CurrentStellarObject
        {
            get { return _currentstellarobject; }
            set { _currentstellarobject = value; }
        }
        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
            }
        }
        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;
                OnPropertyChanged();
            }
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
    }
}

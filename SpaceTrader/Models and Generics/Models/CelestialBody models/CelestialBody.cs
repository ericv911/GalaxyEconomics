using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    //class for general data


    public class CelestialBody : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected EconomicEntity _economicentity;
        protected double _surfacetemperature;
        protected string _name;
        protected long _age;
        protected double _mass;
        protected int _radius;
        protected Point3D _beginposition;
        protected Point3D _rotatedpositionz;
        protected Point3D _rotatedpositionx;
        protected Point3D _scaledpositionnew;
        protected Point3D _translatedposition;
        protected Point3D _finalposition;
        protected Point3D _finalposition2ndbtn;
        public CelestialBody()
        {

        }
        public CelestialBody(string name, Point3D position)
        {
            _name = name;
            _beginposition = position;
            _rotatedpositionz = position;
            _rotatedpositionx = position;
            _scaledpositionnew = position;
            _translatedposition = position;
            _finalposition = position;
            _finalposition2ndbtn = position;
        }
        public EconomicEntity EconomicEntity
        {
            get { return _economicentity; }
            set
            {
                _economicentity = value;
                OnPropertyChanged();
            }
        }
        public double SurfaceTemperature
        {
            get { return _surfacetemperature; }
            set
            {
                _surfacetemperature = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set
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
            get { return _scaledpositionnew; }
            set { _scaledpositionnew = value; }
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
                OnPropertyChanged();
            }
        }
        public Point3D FinalPosition2ndBtn
        {
            get { return _finalposition2ndbtn; }
            set
            {
                _finalposition2ndbtn = value;
                OnPropertyChanged();
            }
        }
        public long Age
        {
            get { return _age; }
            set { _age = value; }
        }
        public int Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public double Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }
    }
}

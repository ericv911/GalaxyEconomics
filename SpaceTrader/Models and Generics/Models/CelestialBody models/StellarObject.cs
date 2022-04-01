
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public interface IStellarObject
    {
        double SurfaceTemperature { get; }
        double MinimumHabitableZoneRadius { get; }
        double MaximumHabitableZoneRadius { get; }
        double Luminosity { get; }
        double MaximumOrbitalBodyDistanceFromStar { get; }
        Color StarColorDimmed { get; }
        Color StarColor { get; }
        double AbsoluteMagnitude { get; }
        int Radius { get; }
        ObservableCollection<Starlane> StarLanes { get; }
        Point3D FinalPosition { get; }
        bool BHighlightonScreen { get; }
        ObservableCollection<OrbitalBody> Orbitalbodies { get; }
    }

    public class StellarObject : CelestialBody, INotifyPropertyChanged, IStellarObject
    {
        protected double _maximumorbitalbodydistancefromstar;
        protected double _minimumhabitablezoneradius;
        protected double _maximumhabitablezoneradius;
        protected double _luminosity;
        protected bool _bhighlightonscreen;
        protected double _absolutemagnitude;
        protected BaseTypes.StellarObjectType _stellartype;
        protected ObservableCollection<Starlane> _starlanes;
        protected Color _starcolordimmed;
        protected Color _starcolor;
        protected StellarObject _stellarobjectnearesttostart;
        protected ObservableCollection<OrbitalBody> _orbitalbodies;
        public double MaximumOrbitalBodyDistanceFromStar
        {
            get { return _maximumorbitalbodydistancefromstar; }
            set { _maximumorbitalbodydistancefromstar = value; }
        }
        public double MinimumHabitableZoneRadius
        {
            get { return _minimumhabitablezoneradius; }
            set { _minimumhabitablezoneradius = value; }
        }
        public double MaximumHabitableZoneRadius
        {
            get { return _maximumhabitablezoneradius; }
            set { _maximumhabitablezoneradius = value; }
        }
        public double Luminosity
        {
            get { return _luminosity; }
            set { _luminosity = value; }
        }
        public StellarObject(string name, Point3D position) : base(name, position)
        {
            Orbitalbodies = new ObservableCollection<OrbitalBody>();
            StarLanes = new ObservableCollection<Starlane>();
            BHighlightonScreen = false;
            //_luminosity = SurfaceTemperature * Mass;
        }

        public BaseTypes.StellarObjectType StellarType
        {
            get { return _stellartype; }
            set { _stellartype = value; }
        }
        public bool BHighlightonScreen
        {
            get { return _bhighlightonscreen; }
            set { _bhighlightonscreen = value; }
        }
        public double AbsoluteMagnitude
        {
            get { return _absolutemagnitude; }
            set { _absolutemagnitude = value; }
        }
        public StellarObject StellarObjectNearesttoStart
        {
            get { return _stellarobjectnearesttostart; }
            set { _stellarobjectnearesttostart = value;
            }
        }
        public ObservableCollection<Starlane> StarLanes
        {
            get { return _starlanes; }
            set { _starlanes = value;  }
        }
        public Color StarColorDimmed
        {
            get { return _starcolordimmed; }
            set { _starcolordimmed = value; }
        }
        public Color StarColor
        {
            get { return _starcolor; }
            set
            {
                _starcolor = value;
                _starcolordimmed = new Color
                {
                    R = (byte)(_starcolor.R / 2),
                    G = (byte)(_starcolor.G / 2),
                    B = (byte)(_starcolor.B / 2)
                };

            }
        }
        public ObservableCollection<OrbitalBody> Orbitalbodies
        {
            set { _orbitalbodies = value; }
            get { return _orbitalbodies; }
        }
    }
}

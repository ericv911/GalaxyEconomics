
using Common.Astronomy;
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
        double Mass { get; }
        string Name { get; }
        CentralHub CentralHub { get; } 
        int GlobalDisasterTimer { get; }
        double SurfaceTemperature { get; }
        double Metallicity { get; }
        double MinimumHabitableZoneRadius { get; }
        double MaximumHabitableZoneRadius { get; }
        double Luminosity { get; }
        double MaximumOrbitalBodyDistanceFromStar { get; }
        Color StarColorDimmed { get; }
        Color StarColor { get; }
        double AbsoluteMagnitude { get; }
        int Radius { get; }

        Point3D FinalPosition { get; }
        bool BHighlightonScreen { get; }

        ObservableCollection<Starlane> StarLanes { get; }
        ObservableCollection<OrbitalBody> Orbitalbodies { get; }
    }

    public class StellarObject : CelestialBody, INotifyPropertyChanged, IStellarObject
    {
        protected Color _starcolordimmed;
        protected Color _starcolor;

        public double Metallicity { get; set; }
        public CentralHub CentralHub { get; set; }
        public int GlobalDisasterTimer { get; set; }
        public double MaximumOrbitalBodyDistanceFromStar { get; set; }
        public double MinimumHabitableZoneRadius { get; set; }
        public double MaximumHabitableZoneRadius { get; set; }
        public double Luminosity { get; set; }
        public StellarObjectType StellarType { get; set; }
        public bool BHighlightonScreen { get; set; }
        public double AbsoluteMagnitude { get; set; }
        public StellarObject StellarObjectNearesttoStart { get; set; }
        public ObservableCollection<Starlane> StarLanes { get; set; }
        public ObservableCollection<OrbitalBody> Orbitalbodies { get; set; }

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
        public StellarObject(string name, Point3D position, double metallicity) : base(name, position)
        {
            Metallicity = metallicity;
            Orbitalbodies = new ObservableCollection<OrbitalBody>();
            StarLanes = new ObservableCollection<Starlane>();
            BHighlightonScreen = false;
            GlobalDisasterTimer = 0;
            CentralHub = new CentralHub(); //add all elements to ElementsinStorage
            //_luminosity = SurfaceTemperature * Mass;
        }
    }
}

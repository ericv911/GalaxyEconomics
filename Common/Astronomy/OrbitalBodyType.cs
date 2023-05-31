using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Astronomy
{
    public interface IOrbitalBodyType
    {
        int MaximumNumberofMoons { get; }
        int RelativeOccurence { get; }
        double NaturalHabitationModifier { get; }
        double Maximum_Mass { get; }
        double Minimum_Mass { get; }
        int Maximum_Radius { get; }
        int Minimum_Radius { get; }
        bool CanBeMoon { get; }
    }
    public class OrbitalBodyType : CelestialBodyType, IOrbitalBodyType
    {
        protected double _foodspoilagefactor;
        protected double _homelessdeathfactor;
        protected bool _ismineable;
        protected bool _ishabitable;
        protected bool _canbemoon;
        protected bool _canhavemoons;
        protected double _naturalhabitationmodifier;
        protected int _surfacestateofmatter;
        public double HomelessDeathFactor
        {
            get { return _homelessdeathfactor; }
            set { _homelessdeathfactor = value; }
        }
        public double FoodSpoilageFactor
        {
            get { return _foodspoilagefactor; }
            set { _foodspoilagefactor = value; }
        }
        public int MaximumNumberofMoons { get; set; }

        public int SurfaceStateofMatter
        {
            get { return _surfacestateofmatter; }
            set { _surfacestateofmatter = value; }
        }
        public double NaturalHabitationModifier
        {
            get { return _naturalhabitationmodifier; }
            set { _naturalhabitationmodifier = value; }
        }
        public bool IsHabitable
        {
            get { return _ishabitable; }
            set { _ishabitable = value; }
        }
        public bool IsMineable
        {
            get { return _ismineable; }
            set { _ismineable = value; }
        }
        public bool CanHaveMoons
        {
            get { return _canhavemoons; }
            set { _canhavemoons = value; }
        }
        public bool CanBeMoon
        {
            get { return _canbemoon; }
            set { _canbemoon = value; }
        }
    }
}


using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public interface IBuildingType
    {
        double ChanceofOccuring { get; }
        bool CanBeBuilt { get; }
        bool NeedsHabitabilitytoBuild { get; }
    }
    public interface IStellarObjectType
    {
        int RelativeOccurence { get; }
        int StarColorRed { get; }
        int StarColorGreen { get; }
        int StarColorBlue { get; }
        double Maximum_Mass { get; }
        double Minimum_Mass { get; }
        int Minimum_Age { get; }
        int Maximum_Age { get; }
        double Maximum_Temp { get; }
        double Minimum_Temp { get; }
        double Maximum_AbsoluteMagnitude { get; }
        double Minimum_AbsoluteMagnitude { get; }
        int Maximum_Radius { get; }
        int Minimum_Radius { get; }
    }

    public interface IOrbitalBodyType 
    {
        int RelativeOccurence { get; }
        double NaturalHabitationModifier { get; }
        double Maximum_Mass { get; }
        double Minimum_Mass { get; }
        int Maximum_Radius { get; }
        int Minimum_Radius { get; }
        bool CanBeMoon { get; }
    }
    //basetypes consists of a collection of types that instances of other classes can be.   An instance of building class can be of a certain buildingtype, with associated parameters
    public class BaseTypes
    {
        public class BuildingType : IBuildingType
        {
            protected bool _needshabitabilitytobuild;
            protected string _name; //name of buildingtype
            protected bool _canbebuilt; //false for buildings that cannot be built by direct player actions. Such buildings are distributed at start or created with special events.
            protected bool _canresize; // once built, building can be resized up or down. this will decrease or increase the size of it's properties
            protected bool _hastechlevel; // besides resizing,building can have a technology level. Adds new properties
            protected bool _canmodifyfood; // increases or decreases food production
            protected bool _canmodifypopulation; //increases or decreases birthrate and deathrate
            protected double _foodmodifier; // if _modifiesfood, how much will it contribute
            protected double _populationmodifier; // if _modifiespopulation, how much will it contribute
            protected int _populationhousing; // how many people can live here max
            protected double _chanceofoccuring; // if building is distributed at start, 
            protected int _foodstorage;
 
            public BuildingType() 
            { }
            public int FoodStorage
            {
                get { return _foodstorage; }
                set { _foodstorage = value; }
            }
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            public bool NeedsHabitabilitytoBuild
            {
                get { return _needshabitabilitytobuild; }
                set { _needshabitabilitytobuild = value; }
            }
            public bool CanModifyFood
            {
                get { return _canmodifyfood; }
                set { _canmodifyfood = value; }
            }
            public bool CanModifyPopulation
            {
                get { return _canmodifypopulation; }
                set { _canmodifypopulation = value; }
            }
            public bool CanResize
            {
                get { return _canresize; }
                set { _canresize = value; }
            }
            public bool HasTechLevel
            {
                get { return _hastechlevel; }
                set { _hastechlevel = value; }
            }
            public bool CanBeBuilt
            {
                get { return _canbebuilt; }
                set { _canbebuilt = value; }
            }
            public double FoodModifier
            {
                get { return _foodmodifier; }
                set { _foodmodifier = value; }
            }
            public double PopulationModifier
            {
                get { return _populationmodifier; }
                set { _populationmodifier = value; }
            }
            public int PopulationHousing
            {
                get { return _populationhousing; }
                set { _populationhousing = value; }
            }
            //Chance of structure that cannot be built occuring at the start on an orbitalbody or natural satellite
            public double ChanceofOccuring
            {
                get { return _chanceofoccuring; }
                set { _chanceofoccuring = value; }
            }
        }
        public class CelestialBodyType 
        {
            protected string _name;
            protected int _relativeOccurence;
            protected int _minimum_Radius;
            protected int _maximum_Radius;
            protected double _minimum_Mass;
            protected double _maximum_Mass;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public int RelativeOccurence
            {
                get { return _relativeOccurence; }
                set { _relativeOccurence = value; }
            }

            public double Minimum_Mass
            {
                get { return _minimum_Mass; }
                set { _minimum_Mass = value; }
            }

            public double Maximum_Mass
            {
                get { return _maximum_Mass; }
                set { _maximum_Mass = value; }
            }
            public int Minimum_Radius
            {
                get { return _minimum_Radius; }
                set { _minimum_Radius = value; }
            }

            public int Maximum_Radius
            {
                get { return _maximum_Radius; }
                set { _maximum_Radius = value; }
            }
        }
        public class StellarObjectType : CelestialBodyType, IStellarObjectType
        {
            protected string _lifePhase;
            protected double _minimum_absolutemagnitude;
            protected double _maximum_absolutemagnitude;
            protected double _minimum_Temp;
            protected double _maximum_Temp;
            protected int _minimum_Age;
            protected int _maximum_Age;
            protected int _starColorRed;
            protected int _starColorGreen;
            protected int _starColorBlue;

            public string LifePhase
            {
                get { return _lifePhase; ; }
                set { _lifePhase = value; }
            }

            public double Minimum_AbsoluteMagnitude
            {
                get { return _minimum_absolutemagnitude; }
                set { _minimum_absolutemagnitude = value; }
            }

            public double Maximum_AbsoluteMagnitude
            {
                get { return _maximum_absolutemagnitude; }
                set { _maximum_absolutemagnitude = value; }
            }
            public int Minimum_Age
            {
                get { return _minimum_Age; }
                set { _minimum_Age = value; }
            }

            public int Maximum_Age
            {
                get { return _maximum_Age; }
                set { _maximum_Age = value; }
            }

            public int StarColorRed
            {
                get { return _starColorRed; }
                set { _starColorRed = value; }
            }

            public int StarColorGreen
            {
                get { return _starColorGreen; }
                set { _starColorGreen = value; }
            }
            public int StarColorBlue
            {
                get { return _starColorBlue; }
                set { _starColorBlue = value; }
            }

            public double Minimum_Temp
            {
                get { return _minimum_Temp; }
                set { _minimum_Temp = value; }
            }
            public double Maximum_Temp
            {
                get { return _maximum_Temp; }
                set { _maximum_Temp = value; }
            }
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
}

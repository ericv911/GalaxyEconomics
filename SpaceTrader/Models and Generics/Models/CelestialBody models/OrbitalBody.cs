using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    //generic class for all habitable and or mineable bodies in a starsystem.  Mainly Planets, Dwarf Planets, Asteroids and Comets.
    //main difference between them is in the extraction modifiers and non-industrial food production .  Habitable planets will produce food, population and happiness without the need for industry or greenhouses.
    //asteroids and comets are much more useable for extracting resources to build structures in space.  Not only is their Heavy element content more easily accessible but once extracted,
    //it does not have to leave the gravity well.
    //generic Non-stellar Celestial Body types will have certain resource extraction modifiers, food production modifiers and population growth and happiness modifiers. 
    //each actual instance of these types will inherit these modifiers * some random fluctuation. 
    public interface IOrbitalBody
    {
        double Food { get; }
        double ProducedFoodthisTurn { get; }
        double Population { get; }
        double DeathsthisTurn { get; }
        double SpoiledFoodthisTurn { get; }
        double NewcomersthisTurn { get; }
        double BirthsthisTurn { get; }
        bool IsHabitable { get; }
        ObservableCollection<OrbitalBody> NaturalSatellites { get; }
        FullyObservableCollection<ResourceInStorageperBody> ResourcesinStorage { get; }

        void GrowFoodandPopulation(FastRandom rand);
        void ConstructBuildings(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand);
        void MineResources(FastRandom rand);
        void SetAvailableResourcesatStart(FastRandom rand, IReadOnlyList<IResource> resources);
        void SetResourceGroupsatStart(FastRandom rand, IReadOnlyList<IResourceGroup> resourcegroups);
        void SetBuildingsatStart(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand);
    }
    public class OrbitalBody : CelestialBody,  INotifyPropertyChanged, IOrbitalBody
    {
        new public event PropertyChangedEventHandler PropertyChanged;

        new private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        //this turn info
        protected double _solarpowerperm2;
        protected double _newcomersthisturn;
        protected double _deathsthisturn;
        protected double _birthsthisturn;
        protected double _spoiledfoodthisturn;
        protected double _producedfoodthisturn;

        // base modifiers
        protected double _basenaturalhabitationmodifier;
        protected double _basenaturaldeathsperturnpercentage;
        protected double _basenaturalbirthsperturnpercentage;

        // actual modifiers
        protected double _naturalimmigrationperturnlinear; // immigration out of nowhere. Not sure yet, how to model this in a closed system. Future :  immigration from other orbital bodies.
        protected double _naturalbirthsperturnpercentage;
        protected double _naturaldeathsperturnpercentage;
        protected double _naturalhabitationmodifier;
        protected double _foodmodifierfrombuildings; //from buildings
        protected double _populationmodifierfrombuildigns; //from buildings

        //properties
        protected double _populationhousing;
        protected double _foodstorage;
        protected double _food;
        protected double _population;

        //bools
        protected bool _isinhabitablezone;
        protected bool _ishabitable;
        protected bool _isnaturalsatellite;

        //type
        protected BaseTypes.OrbitalBodyType _orbitalbodytype;

        //collections belonging to the orbital body
        protected FullyObservableCollection<Building> _buildings;
        protected ObservableCollection<OrbitalBody> _naturalsatellites;
        protected ObservableCollection<ResourceGroup> _resourcegroups;
        protected FullyObservableCollection<ResourceInStorageperBody> _resourcesinstorage;
        //not yet used

        protected double _averagedistancetocentralstellarobject;
        protected int _surfacestateofmatter;
        protected ObservableCollection<Tradegood> _tradegoods;
        protected ObservableCollection<Resource> _resources;

        #endregion

        #region properties
        /// <summary>
        /// solarpower per m2 in watts
        /// </summary>
        
        public bool IsInHabitableZone
        {
            get { return _isinhabitablezone; }
            set { _isinhabitablezone = value; }
        }
        public double SolarPowerperM2
        {
            get { return _solarpowerperm2; }
            set { _solarpowerperm2 = value; }
        }
        public double NewcomersthisTurn
        {
            get { return _newcomersthisturn; }
            set { _newcomersthisturn = value; }
        }
        public double FoodModifierfromBuildings
        {
            get { return _foodmodifierfrombuildings; }
            set { _foodmodifierfrombuildings = value; }
        }
        public double PopulationModifierfromBuildings
        {
            get { return _populationmodifierfrombuildigns; }
            set { _populationmodifierfrombuildigns = value; }
        }
        public double NaturalImmigrationperTurnLinear
        {
            get { return _naturalimmigrationperturnlinear; }
            set { _naturalimmigrationperturnlinear = value; }
        }
        public double FoodStorage
        {
            get { return _foodstorage; }
            set { _foodstorage = value; }
        }
        public double PopulationHousing
        {
            get { return _populationhousing; }
            set
            {
                _populationhousing = value;
            }
        }
        public double Food
        {
            get { return _food; }
            set { _food = value; }
        }
        public double Population
        {
            get { return _population; }
            set { _population = value;
            }         
        }

        public double BaseNaturalHabitationModifier
        {
            get { return _basenaturalhabitationmodifier; }
            set { _basenaturalhabitationmodifier = value; }
        }
        public double BaseNaturalBirthsperTurnPercentage
        {
            get { return _basenaturalbirthsperturnpercentage; }
            set { _basenaturalbirthsperturnpercentage = value; }
        }
        public double BaseNaturalDeathsperTurnPercentage
        {
            get { return _basenaturaldeathsperturnpercentage; }
            set { _basenaturaldeathsperturnpercentage = value; }
        }

        public double NaturalHabitationModifier
        {
            get { return _naturalhabitationmodifier; }
            set { _naturalhabitationmodifier = value; }
        }

        public double NaturalBirthsperTurnPercentage
        {
            get { return _naturalbirthsperturnpercentage; }
            set { _naturalbirthsperturnpercentage = value;
            }
        }
        
        public double NaturalDeathsperTurnPercentage
        {
            get { return _naturaldeathsperturnpercentage; }
            set 
            {   
                _naturaldeathsperturnpercentage = value; 
            }
        }
        public double SpoiledFoodthisTurn
        {
            get { return _spoiledfoodthisturn; }
            set { _spoiledfoodthisturn = value; }
        }
        public double ProducedFoodthisTurn
        {
            get { return _producedfoodthisturn; }
            set { _producedfoodthisturn = value; }
        }
        public double DeathsthisTurn
        {
            get { return _deathsthisturn; }
            set { _deathsthisturn = value; }
        }
        public double BirthsthisTurn
        {
            get { return _birthsthisturn; }
            set { _birthsthisturn = value; }
        }
        public bool IsHabitable
        {
            get { return _ishabitable; }
            set { _ishabitable = value; }
        }
         public bool IsNaturalSatellite
        {
            get { return _isnaturalsatellite; }
            set { _isnaturalsatellite = value; }
        }

        public FullyObservableCollection<ResourceInStorageperBody> ResourcesinStorage
        {
            get { return _resourcesinstorage;}
            set { _resourcesinstorage = value; }
        }
        public FullyObservableCollection<Building> Buildings
        {
            get { return _buildings; }
            set { 
                _buildings = value;
            }
        }
        public ObservableCollection<OrbitalBody> NaturalSatellites
        {
            get { return _naturalsatellites; }
            set { _naturalsatellites = value; }
        }
        public ObservableCollection<ResourceGroup> ResourceGroups
        {
            get { return _resourcegroups; }
            set { _resourcegroups = value; }
        }
        public BaseTypes.OrbitalBodyType OrbitalBodyType
        {
            get { return _orbitalbodytype; }
            set { _orbitalbodytype = value; }
        }

        public int SurfaceStateOfMatter //0 = Solid, 1 = liquid, 2 = gas
        {
            get { return _surfacestateofmatter; }
            set { _surfacestateofmatter = value; }
        }


        public double AverageDistanceToCentralStellarObject
        {
            get { return _averagedistancetocentralstellarobject; }
            set { _averagedistancetocentralstellarobject = value; }
        }
        #endregion
        #region constructor
        public OrbitalBody(string name, Int64 age, Point3D position) : base(name, position)
        {
            Buildings = new FullyObservableCollection<Building>();
            Buildings.CollectionChanged += (obj, e) => RecalculateModifiersandProperties();       
            Buildings.ItemPropertyChanged += (obj, e) => RecalculateModifiersandProperties();
            ResourceGroups = new ObservableCollection<ResourceGroup>();
            NaturalSatellites = new ObservableCollection<OrbitalBody>();
            ResourcesinStorage = new FullyObservableCollection<ResourceInStorageperBody>();
            Age = age;
        }
        #endregion
        public OrbitalBody()
        {
        }
        #region methods

        public void RecalculateModifiersandProperties()//object obj, ItemPropertyChangedEventArgs e)
        {
            //recalculate max food and storage
            PopulationHousing = 0;
            FoodStorage = 0;
            FoodModifierfromBuildings = 1;
            PopulationModifierfromBuildings = 1;
            foreach (Building building in Buildings)
            {
                if (building.Type.PopulationHousing > 0)
                {
                    PopulationHousing += building.Type.PopulationHousing * building.Size;
                }
                if (building.Type.FoodStorage > 0)
                {
                    FoodStorage += building.Type.FoodStorage * building.Size;
                }
                if (building.Type.CanModifyFood)
                {
                    FoodModifierfromBuildings +=  Math.Pow(1 + (building.Type.FoodModifier / 100), building.Size);
                }
                if (building.Type.CanModifyPopulation)
                {
                    PopulationModifierfromBuildings *= Math.Pow(1 + (building.Type.PopulationModifier / 100), building.Size);
                }
            }

            //recalculate natural modifier
            NaturalHabitationModifier = BaseNaturalHabitationModifier * FoodModifierfromBuildings;

            //recalculate actual population modifiers   (perhaps use form a*b/a + b later.  work it out first
            NaturalDeathsperTurnPercentage = BaseNaturalDeathsperTurnPercentage / PopulationModifierfromBuildings;
            if (NaturalDeathsperTurnPercentage < 0.01) NaturalDeathsperTurnPercentage = 0.01;
            
            NaturalBirthsperTurnPercentage = BaseNaturalBirthsperTurnPercentage * PopulationModifierfromBuildings;
            NaturalImmigrationperTurnLinear = BaseNaturalHabitationModifier;  //additional immigration policies and immigration building modifiers. Currently none. Emigration setup, to accomodate immigration to other parts

            //set homelessdeathfactor still to do
        }
        #region  method at start to determine properties of orbital bodies and their satellites

        public void SetBuildingsatStart(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand)
        {
            foreach (IBuildingType buildingtype in buildingtypes)
            {
                if (buildingtype.ChanceofOccuring > 0)
                {
                    if (buildingtype.CanBeBuilt == false) // if building cannot be built and has initial distribution
                    {
                        if ((rand.NextDouble() * 100) < buildingtype.ChanceofOccuring && OrbitalBodyType.IsMineable)
                        {
                            Buildings.Add(new Building { Size = 0, TechLevel = 0, Type = (BaseTypes.BuildingType)buildingtype });
                        }
                        foreach (OrbitalBody naturalsatellite in NaturalSatellites)
                        {
                            if ((rand.NextDouble() * 100) < buildingtype.ChanceofOccuring)
                            {
                                naturalsatellite.Buildings.Add(new Building { Size = 0, TechLevel = 0, Type = (BaseTypes.BuildingType)buildingtype });
                            }
                        }
                    }
                    else // if building can be built and has initial distribution.  Only on orbital bodies that cannot be moons (this can change). 
                    {    // 2 possibilities. building needs habitability, or not.   All buildings of this type only on !canBeMoons 
                        if ((rand.NextDouble() * 100) < buildingtype.ChanceofOccuring)
                    {
                            if (IsHabitable && buildingtype.NeedsHabitabilitytoBuild || !buildingtype.NeedsHabitabilitytoBuild)
                            {
                                Buildings.Add(new Building { Size = 1, TechLevel = 1, Type = (BaseTypes.BuildingType)buildingtype });
                            }
                        }
                        foreach (OrbitalBody naturalsatellite in NaturalSatellites)
                        {
                            if ((rand.NextDouble() * 100) < buildingtype.ChanceofOccuring)
                            {
                                if (naturalsatellite.IsHabitable && buildingtype.NeedsHabitabilitytoBuild || !buildingtype.NeedsHabitabilitytoBuild)
                                {
                                    naturalsatellite.Buildings.Add(new Building { Size = 1, TechLevel = 1, Type = (BaseTypes.BuildingType)buildingtype });
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SetAvailableResourcesatStart(FastRandom rand, IReadOnlyList<IResource> resources)
        {
            bool haslocalresourcegroup;
            foreach (IResource resource in resources)
            {
                haslocalresourcegroup = false;
                foreach (IResourceGroup resourcegroup in ResourceGroups)
                {
                    foreach (IResource tresource in resourcegroup.Resources)
                    {
                        if (tresource == resource)
                        {
                            haslocalresourcegroup = true;
                            break;
                        }
                    }
                    if (haslocalresourcegroup)
                    {
                        break;
                    }
                }
                ResourcesinStorage.Add(new ResourceInStorageperBody { Amount = 0, Resource = (Resource)resource, HasLocalResourcegroup = haslocalresourcegroup });

                foreach (OrbitalBody naturalsatellite in NaturalSatellites)
                {
                    haslocalresourcegroup = false;
                    foreach (IResourceGroup resourcegroup in naturalsatellite.ResourceGroups)
                    {

                        foreach (IResource tresource in resourcegroup.Resources)
                        {
                            if (tresource == resource)
                            {
                                haslocalresourcegroup = true;
                                break;
                            }
                        }

                        if (haslocalresourcegroup)
                        {
                            break;
                        }
                    }
                    naturalsatellite.ResourcesinStorage.Add(new ResourceInStorageperBody { Amount = 0, Resource = (Resource)resource, HasLocalResourcegroup = haslocalresourcegroup });
                }
            }
        }
        public void SetResourceGroupsatStart(FastRandom rand, IReadOnlyList<IResourceGroup> resourcegroups)
        {
            if (OrbitalBodyType.IsMineable)
            {
                foreach (IResourceGroup resourcegroup in resourcegroups)
                {
                    if (rand.Next(0, resourcegroups.Count) < 1)
                    {
                        ResourceGroups.Add((ResourceGroup)resourcegroup);
                    }
                }
            }

            foreach (OrbitalBody naturalsatellite in NaturalSatellites)
            {
                if (naturalsatellite.OrbitalBodyType.IsMineable)
                {
                    foreach (IResourceGroup resourcegroup in resourcegroups)
                    {
                        if (rand.Next(0, resourcegroups.Count) < 1)
                        {
                            naturalsatellite.ResourceGroups.Add((ResourceGroup)resourcegroup);
                        }
                    }
                }
            }
        }
        #endregion
        #region methods to change properties of orbital bodies and their satellites during a turn or after certain other time interval (per month, or year or such)
        public void MineResources(FastRandom rand)
        {
            foreach (ResourceInStorageperBody resourceinstorage in ResourcesinStorage)
            {
                if (resourceinstorage.HasLocalResourcegroup)
                {
                    resourceinstorage.Amount += rand.NextDouble() * resourceinstorage.Resource.UniversalAbundance;
                }
            }
            foreach (OrbitalBody naturalsatellite in NaturalSatellites)
            {
                foreach (ResourceInStorageperBody resourceinstorage in naturalsatellite.ResourcesinStorage)
                {
                    if (resourceinstorage.HasLocalResourcegroup)
                    {
                        resourceinstorage.Amount += rand.NextDouble() * resourceinstorage.Resource.UniversalAbundance;
                    }
                }
            }
        }
        public void GrowFoodandPopulation(FastRandom rand) //method sets data of given parameter. its apparently byref
        {
            double popfromfoodsurplus;
            double tdeathsthisturn;

            //set deltafood
            SpoiledFoodthisTurn = 0;
            ProducedFoodthisTurn = rand.NextDouble() * NaturalHabitationModifier;
            //set deltapopulation
            BirthsthisTurn = Population * NaturalBirthsperTurnPercentage;  // dependent on the current population  . multiplicative factor
            NewcomersthisTurn = rand.NextDouble() * NaturalImmigrationperTurnLinear; //independent of the current population  . additive  factor
            DeathsthisTurn = Population * NaturalDeathsperTurnPercentage;

            //delta population deaths, births, (both %)  immigrants, emigrants (both addition)  from where immigrants are coming is not specified yet. Not from the exisiting galactic population
            Population -= DeathsthisTurn;//deaths per turn
            Population += BirthsthisTurn;
            Population += NewcomersthisTurn; //newcomers, from where is not exactly clear yet.  

            //delta food
            Food -= Population / 10; // food eaten by people
            Food += ProducedFoodthisTurn; //food generated this turn

            //food spoilage calculation
            if (Food - FoodStorage > 0)
            {
                SpoiledFoodthisTurn = (Food - FoodStorage);// * (rand.NextDouble()) * (OrbitalBodyType.FoodSpoilageFactor);
                Food -= SpoiledFoodthisTurn;
            }

            //food shortage effects, or food abundance
            if (Food < 0) // if negative food left => percentage dies from hunger and malnutrition, means people suffered from hunger;
            {
                Population -= DeathsthisTurn;
                DeathsthisTurn *= 2;
                Food = 0;
            }
            else if (Food > Population) // if there is more food left than people after eating, means food is plentiful and abundant => extra pop. growth
            {
                popfromfoodsurplus = rand.NextDouble() * NaturalImmigrationperTurnLinear;
                Population += popfromfoodsurplus;
                NewcomersthisTurn += popfromfoodsurplus;
            }

            // people have not enough houses to live in
            if (Population - PopulationHousing > 0)
            {
                tdeathsthisturn = (Population - PopulationHousing) * (rand.NextDouble()) * (OrbitalBodyType.HomelessDeathFactor);
                Population -= tdeathsthisturn;
                DeathsthisTurn += tdeathsthisturn;
            }

            if (Population < 0) // if everyone dies and population ends up negative, reset to 0
            {
                Population = 0;
            }
        }
        public void ConstructBuildings(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand)
        {
            bool AlreadyBuilt;
            Building tmpbuilding;
            if (IsHabitable || OrbitalBodyType.IsMineable)
            {
                //Console.WriteLine("test");
                if (rand.Next(1, 10) < 3)
                {
                    AlreadyBuilt = false;
                    tmpbuilding = new Building
                    {
                        Type = (BaseTypes.BuildingType)buildingtypes[rand.Next(0, buildingtypes.Count)]
                    };
                    foreach (Building building in Buildings)
                    {
                        if (tmpbuilding.Type == building.Type)
                        {
                            if (building.Type.CanResize == true)
                            {
                                building.Size += 1;
                            }
                            AlreadyBuilt = true;
                            break;
                        }
                    }
                    if (!AlreadyBuilt && tmpbuilding.Type.CanBeBuilt)
                    {
                        if (tmpbuilding.Type.CanResize == true)
                        {
                            tmpbuilding.Size = 1;
                            tmpbuilding.TechLevel = 1;
                        }
                        else
                        {
                            tmpbuilding.Size = 0;
                            tmpbuilding.TechLevel = 0;
                        }
                        Buildings.Add(tmpbuilding);
                    }
                }
            }
            foreach (OrbitalBody naturalsatellite in NaturalSatellites)
            {
                if (rand.Next(1, 10) < 3)
                {
                    AlreadyBuilt = false;
                    tmpbuilding = new Building
                    {
                        Type = (BaseTypes.BuildingType)buildingtypes[rand.Next(0, buildingtypes.Count)]
                    };
                    foreach (Building building in naturalsatellite.Buildings)
                    {
                        if (tmpbuilding.Type == building.Type)
                        {
                            if (building.Type.CanResize == true)
                            {
                                building.Size += 1;
                            }
                            AlreadyBuilt = true;
                            break;
                        }
                    }
                    if (!AlreadyBuilt && tmpbuilding.Type.CanBeBuilt)
                    {
                        if (tmpbuilding.Type.CanResize == true)
                        {
                            tmpbuilding.Size = 1;
                            tmpbuilding.TechLevel = 1;
                        }
                        else
                        {
                            tmpbuilding.Size = 0;
                            tmpbuilding.TechLevel = 0;
                        }
                        naturalsatellite.Buildings.Add(tmpbuilding);
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}

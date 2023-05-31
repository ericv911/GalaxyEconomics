using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using Common.Astronomy;
using Common.Construction;
using Common.Logistics;
using Common.Physics;
using CompoundProvider.Types;
using Taxonomy;

namespace SpaceTrader
{
    //generic class for all habitable and or mineable bodies in a starsystem.  Mainly Planets, Dwarf Planets, Asteroids and Comets.
    //main difference between them is in the extraction modifiers and non-industrial food production .  Habitable planets will produce food, population and happiness without the need for industry or greenhouses.
    //asteroids and comets are much more useable for extracting elements to build structures in space.  Not only is their Heavy element content more easily accessible but once extracted,
    //it does not have to leave the gravity well.
    //generic Non-stellar Celestial Body types will have certain element extraction modifiers, food production modifiers and population growth and happiness modifiers. 
    //each actual instance of these types will inherit these modifiers * some random fluctuation. 
    public interface IOrbitalBody
    {
        CentralHub CentralHub { get; }
        double Food { get; }
        double ProducedFoodthisTurn { get; }
        double DeathsthisTurn { get; }
        double SpoiledFoodthisTurn { get; }
        double NewcomersthisTurn { get; }
        double BirthsthisTurn { get; }
        bool IsHabitable { get; }
        ObservableCollection<OrbitalBody> NaturalSatellites { get; }
        ObservableCollection<ElementinStorage> ElementsinStorage { get; }
        ObservableCollection<ISpeciesperNode> SpeciesonOrbitalBody { get; }
        //void GrowFoodandPopulationEx(FastRandom rand);
        void GrowFoodandPopulation(FastRandom rand, IStellarObject stellarobject);
        void ConstructBuildings(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand);
        void MineElements(FastRandom rand);
        void SetAvailableElementsatStart(FastRandom rand, IReadOnlyList<IElement> elements);
        void SetElementGroupsatStart(FastRandom rand, IReadOnlyList<IElementGroup> elementgroups);
        void SetBuildingsatStart(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand);
    }
    public class OrbitalBody : CelestialBody,  INotifyPropertyChanged, IOrbitalBody
    {
        new public event PropertyChangedEventHandler PropertyChanged;

        new private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region properties
 
        public bool IsHabitable { get; set; }
        public bool IsNaturalSatellite { get; set; }
        public bool IsInHabitableZone { get; set; }

        public int SurfaceStateOfMatter { get; set; } //0 = Solid, 1 = liquid, 2 = gas
        
        public double AverageDistanceToCentralStellarObject { get; set; }
        public double SolarPowerperM2 { get; set; }
        // turn info
        public double SpoiledFoodthisTurn { get; set; }
        public double ProducedFoodthisTurn { get; set; }
        public double DeathsthisTurn { get; set; }
        public double BirthsthisTurn { get; set; }
        public double NewcomersthisTurn { get; set; }

        // actual modifiers
        public double FoodModifierfromBuildings { get; set; }
        public double PopulationModifierfromBuildings { get; set; }
        public double NaturalHabitationModifier { get; set; }
        public double NaturalImmigrationperTurnLinear { get; set; }
        public double NaturalBirthsperTurnPercentage { get; set; }
        public double NaturalDeathsperTurnPercentage { get; set; }

        //population, food etc 
        public double FoodStorage { get; set; }
        public double PopulationHousing { get; set; }
        public double Food { get; set; }

        // base modifiers 
        public double BaseNaturalHabitationModifier { get; set; }
        public double BaseNaturalBirthsperTurnPercentage { get; set; }
        public double BaseNaturalDeathsperTurnPercentage { get; set; }

        public CentralHub CentralHub { get; set; }
        public OrbitalBodyType OrbitalBodyType { get; set; }

        public ObservableCollection<ISpeciesperNode> SpeciesonOrbitalBody { get; set; }
        public ObservableCollection<ElementinStorage> ElementsinStorage { get; set; }
        /// <summary>
        /// FullyObservableCollection for the onItemProperyChanged Event van Items in the collection
        /// These make sure, that changed modifiers from buildings in this collection work their way up to orbital body modifiers
        /// </summary>
        public FullyObservableCollection<Building> Buildings { get; set; }

        public ObservableCollection<Compound> CompoundsinStorage { get; set; }
        public ObservableCollection<OrbitalBody> NaturalSatellites { get; set; }
        public ObservableCollection<ElementGroup> ElementGroups { get; set; }
        public ObservableCollection<Tradegood> TradeGoods { get; set; }

        #endregion
        #region constructor
        public OrbitalBody(string name, Int64 age, Point3D position) : base(name, position)
        {
            FastRandom fastRandom = new FastRandom();
            SpeciesonOrbitalBody = new ObservableCollection<ISpeciesperNode>();
            // make a new functionality to add different kinds of species of different size and reproduction rates to each orbital body.  Don't use Taxonomy.Species for the collection but a SpeciesperOrbitalBody type, that needs
            // to be defined first, including the fields Taxonomy.Species,SizeoPopulation, custom randomized modifiers like reproduction rate etcc. 

            CentralHub = new CentralHub(); //only for Orbitalbodies, not for the Natural Satellites.
            Buildings = new FullyObservableCollection<Building>();
            Buildings.CollectionChanged += (obj, e) => RecalculateModifiersandProperties();       
            Buildings.ItemPropertyChanged += (obj, e) => RecalculateModifiersandProperties();
            ElementGroups = new ObservableCollection<ElementGroup>();
            NaturalSatellites = new ObservableCollection<OrbitalBody>();
            ElementsinStorage = new FullyObservableCollection<ElementinStorage>();
            CompoundsinStorage = new ObservableCollection<Compound>();
            Age = age;
        }
        public OrbitalBody()
        {
        }
        #endregion

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
                            Buildings.Add(new Building { Size = 0, TechLevel = 0, Type = (BuildingType)buildingtype });
                        }
                        foreach (OrbitalBody naturalsatellite in NaturalSatellites)
                        {
                            if ((rand.NextDouble() * 100) < buildingtype.ChanceofOccuring)
                            {
                                naturalsatellite.Buildings.Add(new Building { Size = 0, TechLevel = 0, Type = (BuildingType)buildingtype });
                            }
                        }
                    }
                    else // if building can be built and has initial distribution.  Only on orbital bodies that cannot be moons (this can change). 
                    {    // 2 possibilities. building needs habitability, or not.   All buildings of this type only on !canBeMoons 
                        if ((rand.NextDouble() * 100) < buildingtype.ChanceofOccuring)
                    {
                            if (IsHabitable && buildingtype.NeedsHabitabilitytoBuild || !buildingtype.NeedsHabitabilitytoBuild)
                            {
                                Buildings.Add(new Building { Size = 1, TechLevel = 1, Type = (BuildingType)buildingtype });
                            }
                        }
                        foreach (OrbitalBody naturalsatellite in NaturalSatellites)
                        {
                            if ((rand.NextDouble() * 100) < buildingtype.ChanceofOccuring)
                            {
                                if (naturalsatellite.IsHabitable && buildingtype.NeedsHabitabilitytoBuild || !buildingtype.NeedsHabitabilitytoBuild)
                                {
                                    naturalsatellite.Buildings.Add(new Building { Size = 1, TechLevel = 1, Type = (BuildingType)buildingtype });
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SetAvailableElementsatStart(FastRandom rand, IReadOnlyList<IElement> elements)
        {
            bool haslocalelementgroup;
            foreach (IElement element in elements)
            {
                haslocalelementgroup = false;
                foreach (IElementGroup elementgroup in ElementGroups)
                {
                    foreach (IElement _element in elementgroup.Elements)
                    {
                        if (_element == element)
                        {
                            haslocalelementgroup = true;
                            break;
                        }
                    }
                    if (haslocalelementgroup)
                    {
                        break;
                    }
                }
                ElementsinStorage.Add(new ElementinStorage { AmountinStorage = 0, Element = (Element)element, HasLocalElementgroup = haslocalelementgroup });

                foreach (OrbitalBody naturalsatellite in NaturalSatellites)
                {
                    haslocalelementgroup = false;
                    foreach (IElementGroup elementgroup in naturalsatellite.ElementGroups)
                    {

                        foreach (IElement _element in elementgroup.Elements)
                        {
                            if (_element == element)
                            {
                                haslocalelementgroup = true;
                                break;
                            }
                        }

                        if (haslocalelementgroup)
                        {
                            break;
                        }
                    }
                    naturalsatellite.ElementsinStorage.Add(new ElementinStorage { AmountinStorage = 0, Element = (Element)element, HasLocalElementgroup = haslocalelementgroup });
                }
            }
        }
        public void SetElementGroupsatStart(FastRandom rand, IReadOnlyList<IElementGroup> elementgroups)
        {
            if (OrbitalBodyType.IsMineable)
            {
                foreach (IElementGroup elementgroup in elementgroups)
                {
                    if (rand.Next(0, elementgroups.Count) < 1)
                    {
                        ElementGroups.Add((ElementGroup)elementgroup);
                    }
                }
            }

            foreach (OrbitalBody naturalsatellite in NaturalSatellites)
            {
                if (naturalsatellite.OrbitalBodyType.IsMineable)
                {
                    foreach (IElementGroup elementgroup in elementgroups)
                    {
                        if (rand.Next(0, elementgroups.Count) < 1)
                        {
                            naturalsatellite.ElementGroups.Add((ElementGroup)elementgroup);
                        }
                    }
                }
            }
        }
        #endregion
        #region methods to change properties of orbital bodies and their satellites during a turn or after certain other time interval (per month, or year or such)
        public void MineElements(FastRandom rand)
        {
            foreach (ElementinStorage elementinstorage in ElementsinStorage)
            {
                if (elementinstorage.HasLocalElementgroup)
                {
                    elementinstorage.AmountinStorage += rand.NextDouble() * elementinstorage.Element.UniversalAbundance;
                }
            }
            foreach (OrbitalBody naturalsatellite in NaturalSatellites)
            {
                foreach (ElementinStorage elementinstorage in naturalsatellite.ElementsinStorage)
                {
                    if (elementinstorage.HasLocalElementgroup)
                    {
                        elementinstorage.AmountinStorage += rand.NextDouble() * elementinstorage.Element.UniversalAbundance;
                    }
                }
            }
        }
        public void GrowFoodandPopulation(FastRandom rand, IStellarObject currentstellarobject) //method sets data of given parameter. its apparently byref
        {
            double additionalpopulationgrowthfromfoodsurplus;
            double deathsthisturnfromhomelessness;


            //set deltafood
            SpoiledFoodthisTurn = 0;
            ProducedFoodthisTurn = 100*rand.NextDouble() * NaturalHabitationModifier;
            //set deltapopulation
            if (rand.NextDouble() * 10000 < 2)
            {
                foreach (ISpeciesperNode species in SpeciesonOrbitalBody)
                {
                    species.PopulationSize = 0;
                }
                Food = 0;
                //globaldisastercollection.Add((SpaceTrader.StellarObject)currentstellarobject);
                ((StellarObject)currentstellarobject).GlobalDisasterTimer = 5;
                return;
            }
            foreach (ISpeciesperNode species in SpeciesonOrbitalBody)
            {
                BirthsthisTurn = species.PopulationSize * NaturalBirthsperTurnPercentage;  // dependent on the current population  . multiplicative factor
                NewcomersthisTurn = rand.NextDouble() * NaturalImmigrationperTurnLinear; //independent of the current population  . additive  factor
                DeathsthisTurn = species.PopulationSize * NaturalDeathsperTurnPercentage;

                //delta population deaths, births, (both %)  immigrants, emigrants (both addition)  from where immigrants are coming is not specified yet. Not from the exisiting galactic population and delta food.
                species.PopulationSize += (BirthsthisTurn + NewcomersthisTurn - DeathsthisTurn);
                Food += (ProducedFoodthisTurn - (species.PopulationSize / 10));

                //food spoilage calculation
                if (Food - FoodStorage  > 0)
                {
                    SpoiledFoodthisTurn = (Food - FoodStorage );// * (rand.NextDouble()) * (OrbitalBodyType.FoodSpoilageFactor);
                    Food -= SpoiledFoodthisTurn;
                }

                //food shortage effects, or food abundance
                if (Food < 0) // if negative food left => percentage dies from hunger and malnutrition, means people suffered from hunger;
                {
                    species.PopulationSize -= DeathsthisTurn;
                    DeathsthisTurn *= 2;
                    Food = 0;
                }
                else if (Food > species.PopulationSize) // if there is more food left than people after eating, means food is plentiful and abundant => extra pop. growth
                {
                    additionalpopulationgrowthfromfoodsurplus = rand.NextDouble() * NaturalImmigrationperTurnLinear;
                    species.PopulationSize += additionalpopulationgrowthfromfoodsurplus;
                    BirthsthisTurn += additionalpopulationgrowthfromfoodsurplus;
                }

                // people have not enough houses to live in
                if (species.PopulationSize - PopulationHousing * 10 > 0)
                {
                    deathsthisturnfromhomelessness = (species.PopulationSize - PopulationHousing) * (rand.NextDouble()) * (OrbitalBodyType.HomelessDeathFactor);
                    species.PopulationSize -= deathsthisturnfromhomelessness;
                    DeathsthisTurn += deathsthisturnfromhomelessness;
                }

                if (species.PopulationSize < 0) // if everyone dies and population ends up negative, reset to 0
                {
                    species.PopulationSize = 0;
                }
            }
        }
       
        public void ConstructBuildings(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand)
        {
            bool AlreadyBuilt;
            Building _building;
            if (IsHabitable || OrbitalBodyType.IsMineable)
            {
                if (rand.Next(1, 10) < 3)
                {
                    AlreadyBuilt = false;
                    _building = new Building
                    {
                        Type = (BuildingType)buildingtypes[rand.Next(0, buildingtypes.Count)]
                    };
                    foreach (Building building in Buildings)
                    {
                        if (_building.Type == building.Type)
                        {
                            if (building.Type.CanResize == true)
                            {
                                building.Size += 1;
                            }
                            AlreadyBuilt = true;
                            break;
                        }
                    }
                    if (!AlreadyBuilt && _building.Type.CanBeBuilt)
                    {
                        if (_building.Type.CanResize == true)
                        {
                            _building.Size = 1;
                            _building.TechLevel = 1;
                        }
                        else
                        {
                            _building.Size = 0;
                            _building.TechLevel = 0;
                        }
                        Buildings.Add(_building);
                    }
                }
            }
            foreach (OrbitalBody naturalsatellite in NaturalSatellites)
            {
                if (rand.Next(1, 10) < 3)
                {
                    AlreadyBuilt = false;
                    _building = new Building
                    {
                        Type = (BuildingType)buildingtypes[rand.Next(0, buildingtypes.Count)]
                    };
                    foreach (Building building in naturalsatellite.Buildings)
                    {
                        if (_building.Type == building.Type)
                        {
                            if (building.Type.CanResize == true)
                            {
                                building.Size += 1;
                            }
                            AlreadyBuilt = true;
                            break;
                        }
                    }
                    if (!AlreadyBuilt && _building.Type.CanBeBuilt)
                    {
                        if (_building.Type.CanResize == true)
                        {
                            _building.Size = 1;
                            _building.TechLevel = 1;
                        }
                        else
                        {
                            _building.Size = 0;
                            _building.TechLevel = 0;
                        }
                        naturalsatellite.Buildings.Add(_building);
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}

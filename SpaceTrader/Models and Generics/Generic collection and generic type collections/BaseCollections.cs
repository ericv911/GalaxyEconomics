using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Common.Physics;
using Common.Economy;
using Common.Construction;
using Common.Astronomy;
using Common.Transportation;
using Common.Technology;
using FileHandlingSystem;
using CompoundProvider.Types;

namespace SpaceTrader
{
    /// <summary>
    /// This class is only used as a creation-factory from associated .dat files ,
    /// for the creation of collections of element, elementgroups, tradegoods and related other collections.
    /// These collections will be stored in ObservableCollection and List types.
    /// Other parts of the program can add these collections to Ships, ORbital Bodies, use them for economic calculation purposes, etc.
    /// Later on collections of Tradegoods, refined resources, manufactured supplies etc. will be added.
    /// </summary>
    ///     
    public class BaseCollections
    { 
        #region world collections
        public ObservableCollection<CargoShipType> CargoShipTypes = new ObservableCollection<CargoShipType>();
        public ObservableCollection<BuildingType> BuildingTypes = new ObservableCollection<BuildingType>();
        public ObservableCollection<OrbitalBodyType> OrbitalbodyTypes = new ObservableCollection<OrbitalBodyType>();
        public ObservableCollection<StellarObjectType> StellarObjectTypes = new ObservableCollection<StellarObjectType>();
        public ObservableCollection<EconomicEntity> EconomicEntities = new ObservableCollection<EconomicEntity>();
        public List<TechnologyLevel> TechLevelCollection = new List<TechnologyLevel>();
        #endregion
        #region chemistry and physics
        public ObservableCollection<Block> Blocks = new ObservableCollection<Block>();
        public ObservableCollection<Element> Elements = new ObservableCollection<Element>();
        public ObservableCollection<ElementGroup> ElementGroups = new ObservableCollection<ElementGroup>();

        #endregion 

        public BaseCollections()
        {
            #region chemistry and physics
            SetBlockCollection();
            SetElementCollection();
            SetElementGroupCollection();

            #endregion

            #region world
            SetTechLevelTypeCollection();
            SetShipTypeCollection();
            SetBuildingTypeCollection();
            SetEconomicEntityCollection();
            SetOrbitalBodyTypeCollection();
            SetStellarObjectTypeCollection();
            #endregion
        }

         private int SetShipTypeCollection()
        {
            CargoShipType _cargoshiptype;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(@"resources/ships/cargoships.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                _cargoshiptype = new CargoShipType
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    MaxHullIntegrity = Convert.ToInt32(splitstring[1]),
                    MaxHullIntegrityIncreaseperLevel = Convert.ToInt32(splitstring[2]),
                    FuelCapacity = double.Parse(splitstring[3]),
                    FuelConsumption = double.Parse(splitstring[4]),
                    BaseSpeed = Convert.ToInt32(splitstring[5]),
                    MaxNumberoFCargoHolds = Convert.ToInt32(splitstring[6]),
                    MaxLoadperCargoHold = Convert.ToInt32(splitstring[7]),
                    BaseRepairAmountperTurn = Convert.ToInt32(splitstring[8].Trim(new Char[] { '}' })),
                };
                CargoShipTypes.Add(_cargoshiptype);
            }
            return 0;
        }
        private int SetBuildingTypeCollection()
        {
            BuildingType _buildingtype;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(@"resources/building types.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                _buildingtype = new BuildingType
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    NeedsHabitabilitytoBuild = Convert.ToBoolean(Convert.ToInt32(splitstring[1])),
                    HasTechLevel = Convert.ToBoolean(Convert.ToInt32(splitstring[2])),
                    CanResize = Convert.ToBoolean(Convert.ToInt32(splitstring[3])),
                    CanBeBuilt = Convert.ToBoolean(Convert.ToInt32(splitstring[4])),
                    CanModifyFood = Convert.ToBoolean(Convert.ToInt32(splitstring[5])),
                    CanModifyPopulation = Convert.ToBoolean(Convert.ToInt32(splitstring[6])),
                    FoodModifier = double.Parse(splitstring[7], CultureInfo.InvariantCulture),
                    PopulationModifier = double.Parse(splitstring[8], CultureInfo.InvariantCulture),
                    PopulationHousing = Convert.ToInt32(splitstring[9]),
                    FoodStorage = Convert.ToInt32(splitstring[10]),
                    ChanceofOccuring = double.Parse(splitstring[11], CultureInfo.InvariantCulture),
                    CanDoChemistry = Convert.ToBoolean(Convert.ToInt32(splitstring[12])),
                    CanProduceCompounds = Convert.ToBoolean(Convert.ToInt32(splitstring[13])),
                    WhereCanItBeBuilt = Convert.ToInt32(splitstring[14].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture),
                };
                BuildingTypes.Add(_buildingtype);
            }
            return 0;
        }
        private int SetEconomicEntityCollection()
        {
            string[] splitstring;
            EconomicEntity _EconomicEntity;
            int R, B, G;
            string[] stringfromfile = FileActions.ReadData(@"resources/corporateentities.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                _EconomicEntity = new EconomicEntity();
                // Name, Color RGB
                R = Convert.ToInt32(splitstring[1]);
                B = Convert.ToInt32(splitstring[2]);
                G = Convert.ToInt32(splitstring[3].Trim(new Char[] { '}' }));
                _EconomicEntity.Name = splitstring[0].Trim(new Char[] { '{' });
                _EconomicEntity.Color = Color.FromRgb((byte)R, (byte)B, (byte)G);
                EconomicEntities.Add(_EconomicEntity);
            }
            return 0;
        }
        private int SetTechLevelTypeCollection()
        {
            TechLevelCollection.Add(new TechnologyLevel("Basic", 1, Color.FromRgb(30, 0, 60)));
            TechLevelCollection.Add(new TechnologyLevel("Advanced", 2, Color.FromRgb(0, 30, 60)));
            TechLevelCollection.Add(new TechnologyLevel("Express", 3, Color.FromRgb(30, 30, 70)));
            return 0;
        }
        private int SetOrbitalBodyTypeCollection()
        {
            string[] splitstring;
            OrbitalBodyType _OrbitalBodyType;
            string[] stringfromfile = FileActions.ReadData(@"resources/celestial bodies/orbitalbodydata.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                _OrbitalBodyType = new OrbitalBodyType
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    RelativeOccurence = Convert.ToInt32(splitstring[1]),
                    Minimum_Mass = double.Parse(splitstring[2], CultureInfo.InvariantCulture),
                    Maximum_Mass = double.Parse(splitstring[3], CultureInfo.InvariantCulture),
                    Minimum_Radius = Convert.ToInt32(splitstring[4]),
                    Maximum_Radius = Convert.ToInt32(splitstring[5]),
                    CanBeMoon = Convert.ToBoolean(Convert.ToInt32(splitstring[6])),
                    CanHaveMoons = Convert.ToBoolean(Convert.ToInt32(splitstring[7])),
                    IsMineable = Convert.ToBoolean(Convert.ToInt32(splitstring[8])),
                    IsHabitable = Convert.ToBoolean(Convert.ToInt32(splitstring[9])),
                    FoodSpoilageFactor = double.Parse(splitstring[10], CultureInfo.InvariantCulture)/100,
                    HomelessDeathFactor = double.Parse(splitstring[11], CultureInfo.InvariantCulture)/100,
                    NaturalHabitationModifier = double.Parse(splitstring[12], CultureInfo.InvariantCulture),
                    SurfaceStateofMatter = Convert.ToInt32(splitstring[13]),
                    MaximumNumberofMoons = Convert.ToInt32(splitstring[14]),
                    MinimumDensity = double.Parse(splitstring[15], CultureInfo.InvariantCulture),
                    MaximumDensity = double.Parse(splitstring[16].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture)
                };
                OrbitalbodyTypes.Add(_OrbitalBodyType);
            }
            return 0;
        }
        private int SetStellarObjectTypeCollection()
        {
            string[] splitstring;
            StellarObjectType _StellarType;
            string[] stringfromfile = FileActions.ReadData(@"resources/celestial bodies/stellarobjectdata.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                _StellarType = new StellarObjectType
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    RelativeOccurence = Convert.ToInt32(splitstring[1]),
                    Minimum_Mass = double.Parse(splitstring[2], CultureInfo.InvariantCulture),
                    Maximum_Mass = double.Parse(splitstring[3], CultureInfo.InvariantCulture),
                    Minimum_Age = Convert.ToInt32(splitstring[4]),
                    Maximum_Age = Convert.ToInt32(splitstring[5]),
                    StarColorRed = Convert.ToInt32(splitstring[6]),
                    StarColorGreen = Convert.ToInt32(splitstring[7]),
                    StarColorBlue = Convert.ToInt32(splitstring[8]),
                    Minimum_Temp = double.Parse(splitstring[9], CultureInfo.InvariantCulture),
                    Maximum_Temp = double.Parse(splitstring[10], CultureInfo.InvariantCulture),
                    Minimum_AbsoluteMagnitude = double.Parse(splitstring[11], CultureInfo.InvariantCulture),
                    Maximum_AbsoluteMagnitude = double.Parse(splitstring[12], CultureInfo.InvariantCulture),
                    Minimum_Radius = Convert.ToInt32(splitstring[13]),
                    Maximum_Radius = Convert.ToInt32(splitstring[14]),
                    LifePhase = splitstring[15].Trim(new Char[] { '}' })
                };
                StellarObjectTypes.Add(_StellarType);
            }
            return 0;
        }
        private int SetBlockCollection()
        {
            Block _block;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(@"resources/elements data/blocks.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                _block = new Block
                {
                    Name = splitstring[0].Trim(new Char[] { '{', ' '}),
                    NumberofElectronsperPeriod = Convert.ToInt32(splitstring[1]),
                    StartingPeriod = Convert.ToInt32(splitstring[2]),
                    StartingShell = Convert.ToInt32(splitstring[3].Trim(new char[] { '}' }))
                };
                Blocks.Add(_block);
            }
            return 0;
        }

        private int SetElementGroupCollection()
        {
            string[] splitstring;
            string[] splitarray;
            int elementcounter;
            ElementGroup _elementgroup;
            string[] stringfromfile = FileActions.ReadData(@"resources/elements data/elementgroups.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                elementcounter = 0;
                _elementgroup = new ElementGroup
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    ElementGroupExtractionModifier = double.Parse(splitstring[1])
                };
                for (int i = 2; i < splitstring.Count(); i++)
                {
                    splitstring[i].Replace(" ", string.Empty);
                    if (splitstring[i].Contains("-"))
                    {
                        splitarray = splitstring[i].Split('-');
                        for (int j = Convert.ToInt32(splitarray[0]); j < Convert.ToInt32(splitarray[1].Trim(new Char[] { '}' })) + 1; j++)
                        {
                            _elementgroup.Elements.Add(Elements[j - 1]);
                            elementcounter += 1;
                        }
                    }
                    else
                    {
                        _elementgroup.Elements.Add(Elements[Convert.ToInt32(splitstring[i].Trim(new Char[] { '}' })) - 1]);
                        elementcounter += 1;
                    }
                }
                ElementGroups.Add(_elementgroup);
            }
            return 0;
        }

        private int SetElementCollection()
        {          
            Element _element;
            string[] splitstring;
            string[] splitstringbracket;
            string[] stringfromfile = FileActions.ReadData(@"resources/elements data/elements.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstringbracket = line.Split('(');
                _element = new Element();
                splitstring = line.Split(',');
                _element.Name = splitstring[0].Trim(new Char[] { '{' });
                _element.Symbol = splitstring[1].Trim(new Char[] { '{' });
                var _block = (from _blocks in Blocks
                              where _blocks.Name == splitstring[2]
                              select _blocks).First();
                _element.Block = _block;
                _element.UniversalAbundance = double.Parse(splitstring[3], CultureInfo.InvariantCulture);
                if (_element.UniversalAbundance == 0)
                {
                    _element.UniversalAbundance = 0.000000001;
                }
                _element.StateofMatter = Convert.ToInt32(splitstring[4]);
                _element.IsRadioActive = Convert.ToInt32(splitstring[5].Trim(new Char[] { '}' })) == 1 ? _element.IsRadioActive = true : _element.IsRadioActive = false; // Convert.ToBoolean(splitstring[3]);
                _element.AtomicMass = double.Parse(splitstring[6], CultureInfo.InvariantCulture);
                _element.ElectroNegativity = double.Parse(splitstring[7], CultureInfo.InvariantCulture);
                splitstringbracket[1] = splitstringbracket[1].Substring(0, splitstringbracket[1].IndexOf(")") + 1);
                splitstringbracket[2] = splitstringbracket[2].Substring(0, splitstringbracket[2].IndexOf(")") + 1);
                splitstring = splitstringbracket[1].Split(',');
                for (int i = 0; i < splitstring.Count();i++)
                {
                    _element.OxidationStates.Add(Convert.ToInt32(splitstring[i].Trim(new Char[] { '}', ')' })));
                }
                splitstring = splitstringbracket[2].Split(',');
                for (int i = 0; i < splitstring.Count(); i++)
                {
                    _element.CommonOxidationStates.Add(Convert.ToInt32(splitstring[i].Trim(new Char[] { '}', ')' })));
                }
                Elements.Add(_element);
            }
            return 0;
        }
    }
}

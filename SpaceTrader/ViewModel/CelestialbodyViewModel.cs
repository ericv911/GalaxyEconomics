
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using _3DOperations;
using Common.Astronomy;
using Common.Constants;
using Common.Construction;
using Common.Logistics;
using Common.Physics;

using Common.Technology;
using CompoundProvider.Types;
using FunctionalGroups.Types;
using Taxonomy;
using Taxonomy.Types;

namespace SpaceTrader
{
    public class CelestialBodyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Class properties

        public bool StarlanesOther { get; set; }
        public bool StarlanesFirst { get; set; }
        public int StartNumberofStellarObjects { get; set; }
        public int SpiralWindedness { get; set; }
        public int MaximumRadiusofBulge { get; set; }
        public int MinimumDistancefromCentre { get; set; }
        public int MaximumNumberofOrbitalBodies { get; set; }
        public int MinimumNumberofOrbitalBodies {get;set;}
        public double LowMetallicityStellarObjectsCutOffforOrbitalBodyGeneration { get; set; }
        public double LowMassStellarObjectsCutOffforOrbitalBodyGeneration { get; set; }
        public double MaximumOrbitalBodyMassAroundLowMassStellarObjects { get; set; }
        public double MaximumOrbitalBodyMassAroundLowMetallicityStellarObjects { get; set; }
        public bool DrawStarsinCentre { get; set; } //standard off. preferably not, it get's cluttered beyond belief
        public bool InitializeStellarObjectsinSpiralArms { get; set; }
        public bool InitializeStellarObjectsinBulge { get; set; }
        public bool InitializeStellarObjectsinBar { get; set; }
        public bool InitializeStellarObjectsinDisc { get; set; }

        public Starlane StarlaneSelectedOnScreen { get; set; }
        public StellarObject StellarObjectSelectedOnScreen { get; set; }
        public ObservableCollection<StellarObject> StellarPathfromSourcetoDestination = new ObservableCollection<StellarObject>();
        public ObservableCollection<StellarObject> StellarObjects { get; set; } = new ObservableCollection<StellarObject>();

        #endregion

        #region constructor
        public CelestialBodyViewModel()
        {
            StarlanesOther = true;
            StarlanesFirst = true;
        }
        #endregion

        #region Class Methods

        #region public methods
        public async Task SetCelestialBodyDatasAsync(int width, IReadOnlyList<ISpecies> species, IEnumerable<IOrbitalBodyType> orbitalbodytypes, IEnumerable<IStellarObjectType> stellarobjectypes, IReadOnlyList<IElementGroup> elementgroups, IEnumerable<TechnologyLevel> techlevels, IReadOnlyList<IBuildingType> buildingtypes, IReadOnlyList<Element> elements, IPhysicalConstants physicalconstants, ISolarConstants solarconstants)
        {
            #region clear exisiting data;
            StellarPathfromSourcetoDestination.Clear();
            StellarObjects.Clear();
            #endregion
            #region async part of setting celestial object data and properties 
            FastRandom rand = new FastRandom();
            var getspiralstars = GetSpiralStars(StartNumberofStellarObjects, rand, width, SpiralWindedness, InitializeStellarObjectsinBar, InitializeStellarObjectsinSpiralArms);
            var getbarstars = GetBarStars(StartNumberofStellarObjects, rand, SpiralWindedness, InitializeStellarObjectsinBar);
            var getbulgestars = GetBulgeStars(StartNumberofStellarObjects, rand, MaximumRadiusofBulge, InitializeStellarObjectsinBulge);
            var getdiscstars = GetDiscStars(StartNumberofStellarObjects, rand, width, InitializeStellarObjectsinDisc);
            Task[] Tasks = new Task[4] { getspiralstars, getbarstars, getbulgestars, getdiscstars };
            Task.WaitAll(Tasks, 2000);
            IEnumerable<StellarObject> spiralstars = await getspiralstars;
            IEnumerable<StellarObject> bulgestars = await getbulgestars;
            IEnumerable<StellarObject> barstars = await getbarstars;
            IEnumerable<StellarObject> discstars = await getdiscstars;

            StellarObjects = (ObservableCollection<StellarObject>)discstars;
            StellarObjects = (ObservableCollection<StellarObject>)StellarObjects.AddRange(spiralstars);
            StellarObjects = (ObservableCollection<StellarObject>)StellarObjects.AddRange(bulgestars);
            StellarObjects = (ObservableCollection<StellarObject>)StellarObjects.AddRange(barstars);

            var setstarlanes = SetStarlanesAsync(rand, techlevels);
            await Task.WhenAll(setstarlanes);
            #endregion
            #region non-async part of setting celestial object data and properties 
            SetCelestialBodyProperties(species, orbitalbodytypes, stellarobjectypes, elementgroups, techlevels, buildingtypes, elements, physicalconstants, solarconstants);
            ElementinStorage elementinstorage;
            foreach(StellarObject stellarobject in StellarObjects)
            {
                foreach (Element element in elements)
                {
                    elementinstorage = new ElementinStorage
                    {
                        AmountinStorage = 0,
                        Element = element
                    };
                    stellarobject.CentralHub.ElementsinStorage.Add(elementinstorage);
                }
                foreach (OrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    foreach (Element element in elements)
                    {
                        elementinstorage = new ElementinStorage
                        {
                            AmountinStorage = 0,
                            Element = element
                        };
                        orbitalbody.CentralHub.ElementsinStorage.Add(elementinstorage);
                    }
                }
            }
            #endregion  
            return;
        }

        public double CalculateDistancetoStarlane(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double px = x2 - x1;
            double py = y2 - y1;
            double temp = (px * px) + (py * py);
            double u = ((x3 - x1) * px + (y3 - y1) * py) / (temp);
            if (u > 1)
            {
                u = 1;
            }
            else if (u < 0)
            {
                u = 0;
            }
            double x = x1 + u * px;
            double y = y1 + u * py;

            double dx = x - x3;
            double dy = y - y3;

            return Math.Sqrt(dx * dx + dy * dy); 
        }

        public void SetActiveStarlane(Point mouseposition)
        {
            double tdistance;
            double distance = 100000;
            foreach (StellarObject stellarobject in StellarObjects)
            {
                foreach (Starlane starlane in stellarobject.StarLanes)
                {
                    tdistance = CalculateDistancetoStarlane(starlane.To.FinalPosition.X, starlane.To.FinalPosition.Z, starlane.From.FinalPosition.X, starlane.From.FinalPosition.Z, mouseposition.X, mouseposition.Y);
                    if (tdistance < distance)
                    {
                        StarlaneSelectedOnScreen = starlane;
                        distance = tdistance;
                    }
                }
            }
        }
        public void SetActiveStar(Point mousepostion)
        {
            int distance;
            CelestialBody SmallestStellarObject = new CelestialBody();
            int smallestdistance = 1000000;
            foreach (StellarObject stellarobject in StellarObjects)
            {
                distance = (int)(Math.Pow((int)stellarobject.FinalPosition.X - (int)mousepostion.X, 2) + Math.Pow((int)stellarobject.FinalPosition.Z - (int)mousepostion.Y, 2));
                if (distance < smallestdistance)
                {
                    smallestdistance = distance;
                    SmallestStellarObject = stellarobject;
                }
            }
            StellarObjectSelectedOnScreen = (StellarObject)SmallestStellarObject;
        }

        public ObservableCollection<StellarObject> CalculateShortestPathfromShiptoStar(StellarObject currentdestinationstellarobject, int mode) 
        {
            return ReturnCalculateShortestpath(currentdestinationstellarobject, StellarObjectSelectedOnScreen, mode);
        }

        #endregion

        #region private methods
        #region generate orbital bodies and set celestialbody properties 
        //private int SetOrbitalBodyNaturalSatellites(IEnumerable<BaseTypes.OrbitalBodyType> OrbitalBodyTypes)
        //{

        //    return 0;
        //}
        private void SetCelestialBodyProperties(IReadOnlyList<ISpecies> species, IEnumerable<IOrbitalBodyType> orbitalbodytypes, IEnumerable<IStellarObjectType> stellarobjectypes, IReadOnlyList<IElementGroup> elementgroups, IEnumerable<TechnologyLevel> techlevels, IReadOnlyList<IBuildingType> buildingtypes, IReadOnlyList<IElement> elements, IPhysicalConstants physicalconstants, ISolarConstants solarconstants)
        {
            StellarPathfromSourcetoDestination.Clear();
            SetStellarObjectProperties(stellarobjectypes, physicalconstants, solarconstants); //set properties of the stellar object population
            SetOrbitalBodyProperties(species, orbitalbodytypes, physicalconstants); //set properties of the orbital bodies around stellar objects
            SetOrbitalBodyElementGroups(elementgroups); //set elementgroups of each orbital body. Used for extraction modifiers etc.
            SetOrbitalBodyInitialBuildings(buildingtypes); //set initial buildings of each orbital body
            SetOrbitalBodyElementsatStart(elements);
            foreach (StellarObject stellarobject in StellarObjects)
            {
                
                foreach (OrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.RecalculateModifiersandProperties();
                    foreach(OrbitalBody naturalsatellite in orbitalbody.NaturalSatellites)
                    {
                        naturalsatellite.RecalculateModifiersandProperties();
                    }
                }
            }
        }

        private void SetOrbitalBodyElementGroups(IReadOnlyList<IElementGroup> elementgroups)
        {
            FastRandom rand = new FastRandom();
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.SetElementGroupsatStart(rand, elementgroups);
                }
            }
        }
        private void SetOrbitalBodyElementsatStart(IReadOnlyList<IElement> elements)
        {
            FastRandom Rand = new FastRandom();

            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.SetAvailableElementsatStart(Rand, elements);
                }
            }
        }
        private void SetOrbitalBodyInitialBuildings(IReadOnlyList<IBuildingType> buildingtypes)
        {
            FastRandom rand = new FastRandom();
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.SetBuildingsatStart(buildingtypes, rand);
                }
            }
        }

        private OrbitalBodyType DetermineOrbitalType(IEnumerable<IOrbitalBodyType> orbitalbodytypes, int rand_inrange_sumrelativeoccurenceallorbitalbodytypes)
        {
            int relativeoccurencecurrentorbitalbodytype = 0;
            OrbitalBodyType orbitalbodytype = new OrbitalBodyType();
            foreach (IOrbitalBodyType _orbitalbodytype in orbitalbodytypes)
            {
                relativeoccurencecurrentorbitalbodytype += _orbitalbodytype.RelativeOccurence;
                if (rand_inrange_sumrelativeoccurenceallorbitalbodytypes < relativeoccurencecurrentorbitalbodytype)
                {
                    orbitalbodytype = (OrbitalBodyType)_orbitalbodytype;
                    break;
                }
            }
            return orbitalbodytype;
        }
        private int SetOrbitalBodyProperties(IReadOnlyList<ISpecies> species, IEnumerable<IOrbitalBodyType> OrbitalBodyTypes, IPhysicalConstants physicalconstants)
        {
            double cuberoot = (1.0 / 3.0); //
            IEnumerable<IOrbitalBodyType> _orbitalbodytypes;
            int sumrelativeoccurenceallorbitalbodytypes = 0;
            int sumrelativeoccurenceallorbitalbodytypeallowedaroundlowmassstellarobjects = 0;
            int sumrelativeoccurenceallorbitalbodytypeallowedaroundlowmetallicitystellarobjects = 0;
            int sumrelativeoccurenceallorbitalbodytypeallowedtobemoons = 0;
            int _sumrelativeoccurenceselectedorbitalbodytypes;
            OrbitalBody _naturalsatellite;
            FastRandom rand = new FastRandom();
            var _allorbitalbodytypearoundlowmassstellarobjects = from __orbitalbodytypes in OrbitalBodyTypes
                                              where __orbitalbodytypes.Maximum_Mass < MaximumOrbitalBodyMassAroundLowMassStellarObjects
                                              select __orbitalbodytypes;
            var _allorbitalbodytypesallowedaroundlowmetallicitystellarobjects = from __orbitalbodytypes in OrbitalBodyTypes
                                                   where __orbitalbodytypes.Maximum_Mass < MaximumOrbitalBodyMassAroundLowMetallicityStellarObjects
                                                   select __orbitalbodytypes;
            var _allorbitalbodytypesthatcanbemoons = from __orbitalbodytypes in OrbitalBodyTypes
                                                    where __orbitalbodytypes.CanBeMoon == true
                                                    select __orbitalbodytypes;
            foreach (IOrbitalBodyType _orbitalbodytype in OrbitalBodyTypes)
            {
                sumrelativeoccurenceallorbitalbodytypes += _orbitalbodytype.RelativeOccurence;
                if (_orbitalbodytype.Maximum_Mass < MaximumOrbitalBodyMassAroundLowMassStellarObjects)
                {
                    sumrelativeoccurenceallorbitalbodytypeallowedaroundlowmassstellarobjects += _orbitalbodytype.RelativeOccurence;
                }
                if (_orbitalbodytype.Maximum_Mass < MaximumOrbitalBodyMassAroundLowMetallicityStellarObjects)
                {
                    sumrelativeoccurenceallorbitalbodytypeallowedaroundlowmetallicitystellarobjects += _orbitalbodytype.RelativeOccurence;
                }
                if (_orbitalbodytype.CanBeMoon == true)
                {
                    sumrelativeoccurenceallorbitalbodytypeallowedtobemoons += _orbitalbodytype.RelativeOccurence;
                }
            }

            foreach (IStellarObject stellarobject in StellarObjects)
            {
                if (stellarobject.Metallicity < LowMetallicityStellarObjectsCutOffforOrbitalBodyGeneration)
                {
                    _sumrelativeoccurenceselectedorbitalbodytypes = sumrelativeoccurenceallorbitalbodytypeallowedaroundlowmetallicitystellarobjects;
                    _orbitalbodytypes = _allorbitalbodytypesallowedaroundlowmetallicitystellarobjects;
                }
                else if (stellarobject.Mass < LowMassStellarObjectsCutOffforOrbitalBodyGeneration)
                {
                    _sumrelativeoccurenceselectedorbitalbodytypes = sumrelativeoccurenceallorbitalbodytypeallowedaroundlowmassstellarobjects;
                    _orbitalbodytypes = _allorbitalbodytypearoundlowmassstellarobjects;
                }
                else
                {
                    _sumrelativeoccurenceselectedorbitalbodytypes = sumrelativeoccurenceallorbitalbodytypes;
                    _orbitalbodytypes = OrbitalBodyTypes;
                }

                foreach (OrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.OrbitalBodyType = DetermineOrbitalType(_orbitalbodytypes,rand.Next(0, _sumrelativeoccurenceselectedorbitalbodytypes));
                    orbitalbody.IsHabitable = orbitalbody.OrbitalBodyType.IsHabitable; //each orbitalbody instance can be made uninhabitable. at first though it derives habitability from it's type
                    orbitalbody.Mass = Math.Pow(rand.NextDouble(), 2) * (orbitalbody.OrbitalBodyType.Maximum_Mass - orbitalbody.OrbitalBodyType.Minimum_Mass) + orbitalbody.OrbitalBodyType.Minimum_Mass;
                    orbitalbody.Density = Math.Pow(rand.NextDouble(), 2) * (orbitalbody.OrbitalBodyType.MaximumDensity - orbitalbody.OrbitalBodyType.MinimumDensity) + orbitalbody.OrbitalBodyType.MinimumDensity;
                    orbitalbody.Radius = Convert.ToInt32(Math.Pow((orbitalbody.Mass * 3000) / (4 * Math.PI * orbitalbody.Density), cuberoot)*1000);

                    if (orbitalbody.IsHabitable)
                    {
                        orbitalbody.BaseNaturalBirthsperTurnPercentage = (1 + rand.NextDouble() * 2) / 100;
                        orbitalbody.BaseNaturalHabitationModifier = (rand.NextDouble()) * orbitalbody.OrbitalBodyType.NaturalHabitationModifier  ;
                        orbitalbody.BaseNaturalDeathsperTurnPercentage = (5 + rand.NextDouble() * 10) / 100;
                        orbitalbody.Food = 10000;
                        Species humans = new Species();
                        foreach (Species _species in species)
                        {
                            orbitalbody.SpeciesonOrbitalBody.Add(new SpeciesperNode(_species, (int)(200 * rand.NextDouble()), _species.ReproductionRate * rand.NextDouble()));
                        }
                    }
                    else
                    {
                        orbitalbody.BaseNaturalHabitationModifier = 0;
                        orbitalbody.BaseNaturalDeathsperTurnPercentage = 0;
                        orbitalbody.BaseNaturalBirthsperTurnPercentage = 0;
                    }
                    if (orbitalbody.OrbitalBodyType.CanHaveMoons == true)
                    {
                        for (int j = 0; j < orbitalbody.OrbitalBodyType.MaximumNumberofMoons; j++)
                        {
                            {
                                _naturalsatellite = new OrbitalBody(orbitalbody.Name + ((char)j+97).ToString(), orbitalbody.Age, orbitalbody.BeginPosition); //j+97 is from 'b' upwards to designate moons
                                 {
                                    _naturalsatellite.OrbitalBodyType = DetermineOrbitalType(_allorbitalbodytypesthatcanbemoons, rand.Next(0, sumrelativeoccurenceallorbitalbodytypeallowedtobemoons));
                                    _naturalsatellite.IsHabitable = _naturalsatellite.OrbitalBodyType.IsHabitable;
                                    _naturalsatellite.Mass = Math.Pow(rand.NextDouble(), 2) * (_naturalsatellite.OrbitalBodyType.Maximum_Mass - _naturalsatellite.OrbitalBodyType.Minimum_Mass) + _naturalsatellite.OrbitalBodyType.Minimum_Mass;
                                    _naturalsatellite.Density = Convert.ToInt32(Math.Pow(rand.NextDouble(), 2) * (_naturalsatellite.OrbitalBodyType.MaximumDensity - _naturalsatellite.OrbitalBodyType.MinimumDensity) + _naturalsatellite.OrbitalBodyType.MinimumDensity);
                                    _naturalsatellite.Radius = Convert.ToInt32(Math.Pow((_naturalsatellite.Mass * 3000) / (4 * Math.PI * _naturalsatellite.Density), cuberoot) * 1000);

                                    if (_naturalsatellite.IsHabitable)
                                    {
                                        _naturalsatellite.BaseNaturalBirthsperTurnPercentage = (1 + rand.NextDouble() * 5) / 100;
                                        _naturalsatellite.BaseNaturalDeathsperTurnPercentage = (5 + rand.NextDouble() * 10) / 100;
                                        _naturalsatellite.BaseNaturalHabitationModifier = (rand.NextDouble()) * _naturalsatellite.OrbitalBodyType.NaturalHabitationModifier;
                                        _naturalsatellite.Food = 100;
                                        foreach (Species _species in species)
                                        {
                                            _naturalsatellite.SpeciesonOrbitalBody.Add(new SpeciesperNode(_species, (int)(20 * rand.NextDouble()), _species.ReproductionRate * rand.NextDouble()));
                                        }
                                    }
                                    else
                                    {
                                        _naturalsatellite.BaseNaturalDeathsperTurnPercentage = 0;
                                        _naturalsatellite.BaseNaturalHabitationModifier = 0;
                                        _naturalsatellite.BaseNaturalBirthsperTurnPercentage = 0;
                                    }
                                    orbitalbody.NaturalSatellites.Add(_naturalsatellite);
                                }
                            }
                        }
                    }
                }
            }
            double distancebetweenorbitalbodies;
            double _planetcounter ; //counts how many planets are present in the habitable zone
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                distancebetweenorbitalbodies = stellarobject.MaximumOrbitalBodyDistanceFromStar / stellarobject.Orbitalbodies.Count;
                _planetcounter = 0;
                int howmanyinhabitablezone;
                foreach (OrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    howmanyinhabitablezone = rand.Next(1, 3);
                    if (_planetcounter < howmanyinhabitablezone)
                    {
                        orbitalbody.AverageDistanceToCentralStellarObject = stellarobject.MinimumHabitableZoneRadius + rand.NextDouble() * (stellarobject.MaximumHabitableZoneRadius - stellarobject.MinimumHabitableZoneRadius);
                        orbitalbody.IsInHabitableZone = true;
                    }
                    else
                    {
                        orbitalbody.AverageDistanceToCentralStellarObject = stellarobject.MaximumHabitableZoneRadius + Math.Pow(rand.NextDouble(),2) * (stellarobject.MaximumOrbitalBodyDistanceFromStar - stellarobject.MaximumHabitableZoneRadius);
                        orbitalbody.IsInHabitableZone = false;
                    }
                    orbitalbody.SolarPowerperM2 = stellarobject.Luminosity / (4 * Math.PI * Math.Pow(orbitalbody.AverageDistanceToCentralStellarObject, 2));
                    if (orbitalbody.SolarPowerperM2 < physicalconstants.WattperM2OptimalforHabitablezone)
                    {
                        orbitalbody.BaseNaturalHabitationModifier *= Math.Pow((orbitalbody.SolarPowerperM2 / physicalconstants.WattperM2OptimalforHabitablezone),2);
                    }
                    else
                    {
                        orbitalbody.BaseNaturalHabitationModifier *= Math.Pow((physicalconstants.WattperM2OptimalforHabitablezone / orbitalbody.SolarPowerperM2),2);
                    }
                    foreach (OrbitalBody naturalsatellite in orbitalbody.NaturalSatellites)
                    {
                        naturalsatellite.AverageDistanceToCentralStellarObject = orbitalbody.AverageDistanceToCentralStellarObject;
                        naturalsatellite.SolarPowerperM2 = orbitalbody.SolarPowerperM2;
                        naturalsatellite.IsInHabitableZone = orbitalbody.IsInHabitableZone;
                        if (naturalsatellite.SolarPowerperM2 < physicalconstants.WattperM2OptimalforHabitablezone)
                        {
                            naturalsatellite.BaseNaturalHabitationModifier *= Math.Pow((naturalsatellite.SolarPowerperM2 / physicalconstants.WattperM2OptimalforHabitablezone),2);
                        }
                        else
                        {
                            naturalsatellite.BaseNaturalHabitationModifier *= Math.Pow((physicalconstants.WattperM2OptimalforHabitablezone / naturalsatellite.SolarPowerperM2),2);
                        }
                    }
                    _planetcounter += 1;
                }
            }
            return 0;
        }
        private int SetStellarObjectProperties(IEnumerable<IStellarObjectType> StellarTypes, IPhysicalConstants physicalconstants, ISolarConstants solarconstants)
        {
            int totalrelativeoccurence = 0;
            int randomnumber;
            int relativeoccurencecounter;
            int stellartypeindex;
            int _red, _blue, _green;
            int deviation = 30;
            double _rand;
            Color clr;
            FastRandom rand = new FastRandom();

            foreach (IStellarObjectType stellartypes in StellarTypes)
            {
                totalrelativeoccurence += stellartypes.RelativeOccurence;
            }
            foreach (StellarObject stellarobject in StellarObjects)
            {
                foreach (Starlane starlane in stellarobject.StarLanes)
                {
                    starlane.SetLength();
                }
                relativeoccurencecounter = 0;
                stellartypeindex = 0;
                randomnumber = rand.Next(0, totalrelativeoccurence); // generates number between 0 and totalrelativeoccurence - 1
                foreach (IStellarObjectType stellartype in StellarTypes)
                {
                    relativeoccurencecounter += stellartype.RelativeOccurence;
                    if (randomnumber < relativeoccurencecounter)
                    {
                        _rand = rand.NextDouble();
                        _red = rand.Next(stellartype.StarColorRed - deviation, stellartype.StarColorRed + deviation);
                        _green = rand.Next(stellartype.StarColorGreen - deviation, stellartype.StarColorGreen + deviation);
                        _blue = rand.Next(stellartype.StarColorBlue - deviation, stellartype.StarColorBlue + deviation);
                        if (_red < 0) _red = 0; if (_red > 255) _red = 255;
                        if (_green < 0) _green = 0; if (_green > 255) _green = 255;
                        if (_blue < 0) _blue = 0; if (_blue > 255) _blue = 255;
                        clr = Color.FromRgb((byte)_green, (byte)_red, (byte)_blue);
                        stellarobject.StellarType = (StellarObjectType)stellartype;
                        stellarobject.Mass = Math.Pow(_rand, 2) * (stellartype.Maximum_Mass - stellartype.Minimum_Mass) + stellartype.Minimum_Mass;
                        stellarobject.Age = rand.Next(stellartype.Minimum_Age * 1000, stellartype.Maximum_Age * 1000);
                        stellarobject.SurfaceTemperature = Math.Pow(_rand, 2) * (stellartype.Maximum_Temp - stellartype.Minimum_Temp) + stellartype.Minimum_Temp;
                        stellarobject.AbsoluteMagnitude = Math.Pow(_rand, 2) * (stellartype.Maximum_AbsoluteMagnitude - stellartype.Minimum_AbsoluteMagnitude) + stellartype.Minimum_AbsoluteMagnitude;
                        stellarobject.Radius = Convert.ToInt32(Math.Pow(_rand, 2) * (stellartype.Maximum_Radius - stellartype.Minimum_Radius) + stellartype.Minimum_Radius);
                        stellarobject.StarColor = clr;
                        stellarobject.Luminosity = physicalconstants.StefanBoltzmannConstant * (Math.PI * 4 * Math.Pow(stellarobject.Radius * 1000, 2)) * Math.Pow(stellarobject.SurfaceTemperature * 1000, 4);
                        stellarobject.MinimumHabitableZoneRadius = Math.Sqrt(stellarobject.Luminosity / (4 * Math.PI * physicalconstants.WattperM2UpperBoundaryforHabitablezone));
                        stellarobject.MaximumHabitableZoneRadius = Math.Sqrt(stellarobject.Luminosity / (4 * Math.PI * physicalconstants.WattperM2LowerBoundaryforHabitablezone));
                        stellarobject.MaximumOrbitalBodyDistanceFromStar = Math.Sqrt(stellarobject.Luminosity / (4 * Math.PI)); //1 watt / m² is maximum distance

                        for (int i = 1; i < rand.Next(MinimumNumberofOrbitalBodies, MaximumNumberofOrbitalBodies); i++)
                        {
                            stellarobject.Orbitalbodies.Add(new OrbitalBody(stellarobject.Name + " " + Helperfunction.IntToLetters(i).ToLower(), stellarobject.Age, stellarobject.BeginPosition));
                        }
                        break;
                    }
                    stellartypeindex += 1;
                }
            }
            return 0;
        }
        #endregion
        #region stellar object generation

        //async tasks calling synchronous methods
        async Task<IEnumerable<StellarObject>> GetSpiralStars(int numberofstars, FastRandom rand, int width, int spiralwindedness, bool initbar, bool initspiral)
        {
            return await Task.Run(() => SetListStarsinSpiralArms(numberofstars, rand, width, spiralwindedness, initbar, initspiral));
        }
        async Task<IEnumerable<StellarObject>> GetBarStars(int numberofstars, FastRandom rand, int spiralwindedness, bool initbar)
        {
            return await Task.Run(() => SetListStarsinBar(numberofstars, rand, spiralwindedness, initbar));
        }
        async Task<IEnumerable<StellarObject>> GetBulgeStars(int numberofstars, FastRandom rand, int bulgesize, bool initbulge)
        {
            return await Task.Run(() => SetListStarsinBulge(numberofstars, rand, bulgesize, initbulge));
        }
        async Task<IEnumerable<StellarObject>> GetDiscStars(int numberofstars, FastRandom rand, int width, bool initdisc)
        {
            return await Task.Run(() => SetListStarsinDisc(numberofstars, rand, width, initdisc));
        }
        async Task<int> SetStarlanesAsync(FastRandom rand, IEnumerable<TechnologyLevel> techlevels)
        {
            return await Task.Run(() => SetStarlanes(rand, techlevels));
        }

        //Normal synchronous methods
        private int SetStarlanes(FastRandom rand, IEnumerable<TechnologyLevel> techlevels )
        {
            #region create first starlane
            string stringforformattingcounter;
            string stringforformattingtotal = "";
            int distance;
            int smallestdistance;
            int smallestindex = 0;
            //counters to monitor  progress in consolescreen
            long totalcounter = 0;
            int loopcounter = 0;
            long ttlcntloopotherstarlanes = StellarObjects.Count * StellarObjects.Count;
            int searchsquare = StellarObjects.Count > 30000 ? 40 : (StellarObjects.Count > 25000 ? 50 : (StellarObjects.Count > 20000 ? 75 : (StellarObjects.Count > 15000 ? 100 : (StellarObjects.Count > 10000 ? 150 : (StellarObjects.Count > 7500 ? 200 : 500)))));
            List<int> _stellarobjectlist = new List<int>();
            Stopwatch stopwatch = new Stopwatch();
            bool bbasicstarlaneskipped;
            Starlane _starlane;
            TechnologyLevel _techlevel;
            if (StarlanesFirst)
            {
                for (int i = 0; i < StellarObjects.Count; i++)
                {
                    _stellarobjectlist.Add(i);
                }
                int iremoveindexat = 0;
                int currentindex = 0;
                for (int i = 0; i < StellarObjects.Count; i++)
                {
                    smallestdistance = 100000000;
                    for (int j = 0; j < _stellarobjectlist.Count; j++)
                    {
                        if (smallestdistance < 200)
                        {
                            break;
                        }

                        if (j != currentindex)
                        {
                            distance = GetDistanceBetweenStars(StellarObjects[currentindex], StellarObjects[_stellarobjectlist[j]]);
                            if (distance < smallestdistance)
                            {
                                smallestdistance = distance;
                                smallestindex = _stellarobjectlist[j];
                                iremoveindexat = j;
                            }
                        }
                    }
                    if (smallestdistance < 300000)
                    {
                        bbasicstarlaneskipped = false;
                        _techlevel = new TechnologyLevel();
                        foreach (TechnologyLevel techlevel in techlevels)
                        {
                            if (bbasicstarlaneskipped)
                            {
                                _techlevel = techlevel;
                                break;
                            }
                            if (rand.Next(1, 17) < 16)
                            {
                                _techlevel = techlevel;
                                break;
                            }
                            else
                            {
                                bbasicstarlaneskipped = true;
                            }
                        }
                        _starlane = new Starlane
                        {
                            From = StellarObjects[currentindex],
                            To = StellarObjects[smallestindex],
                            TechLevelRequiredforTravel = _techlevel
                        };
                        StellarObjects[currentindex].StarLanes.Add(_starlane);
                        _starlane = new Starlane
                        {
                            From = StellarObjects[smallestindex],
                            To = StellarObjects[currentindex],
                            TechLevelRequiredforTravel = _techlevel
                        };
                        StellarObjects[smallestindex].StarLanes.Add(_starlane);
                        currentindex = smallestindex;
                        _stellarobjectlist.RemoveAt(iremoveindexat);
                    }
                    else
                    {
                        currentindex = smallestindex;
                        _stellarobjectlist.RemoveAt(iremoveindexat);
                    }
                }
            }
            #endregion
            Console.WriteLine($"First starlane finished.");

            #region create other starlanes

            if (StarlanesOther)
            {
                int deltax, deltay;
                stopwatch.Start();
                stringforformattingtotal = String.Format("{0:#,0}", ttlcntloopotherstarlanes);
                int _smallestdistance, _smallestindex, smallestdistance1;
                int[] _twoindices = new int[2];
                int smallestindex1 = 0;
                bool writeindextostarlane, writeindextootherstarlane;

                for (int i = 0; i < StellarObjects.Count; i++)
                {
                    smallestdistance = 100000000;
                    smallestdistance1 = 100000000;
                    for (int j = 0; j < StellarObjects.Count; j++)
                    {
                        if (j != i)
                        {
                            if (smallestdistance < 150 && smallestdistance1 < 150)
                            {
                                break;
                            }
                            deltax = (int)(StellarObjects[i].BeginPosition.X - StellarObjects[j].BeginPosition.X);
                            deltax = (deltax + (deltax >> 31)) ^ (deltax >> 31);
                            deltay = (int)(StellarObjects[i].BeginPosition.Y - StellarObjects[j].BeginPosition.Y);
                            deltay = (deltay + (deltay >> 31)) ^ (deltay >> 31);

                            loopcounter += 1;
                            if (loopcounter > 10000000)
                            {
                                stringforformattingcounter = String.Format("{0:#,0}", totalcounter);
                                Console.WriteLine($"check {stringforformattingcounter} of {stringforformattingtotal} check other starlanes");
                                loopcounter = 0;
                            }
                            if (deltax < searchsquare && deltay < searchsquare)
                            {
                                totalcounter += 1;
                                distance = GetDistanceBetweenStars(StellarObjects[i], StellarObjects[j]);
                                if (distance < smallestdistance)
                                {
                                    _smallestdistance = smallestdistance;
                                    _smallestindex = smallestindex;
                                    smallestdistance = distance;
                                    _twoindices[0] = j;
                                    smallestindex = j;
                                    if (_smallestdistance < smallestdistance1)
                                    {
                                        smallestdistance1 = _smallestdistance;
                                        _twoindices[1] = _smallestindex;
                                        smallestindex1 = _smallestindex;
                                    }
                                }
                                else if (distance < smallestdistance1)
                                {
                                    smallestdistance1 = distance;
                                    _twoindices[1] = j;
                                    smallestindex1 = j;
                                }
                            }
                        }
                    }
                    bbasicstarlaneskipped = false;
                    _techlevel = new TechnologyLevel();
                    foreach (TechnologyLevel techlevel in techlevels)
                    {
                        if (bbasicstarlaneskipped)
                        {
                            _techlevel = techlevel;
                            break;
                        }
                        if (rand.Next(1, 17) < 16) //ratio of starlane tech 1 and 2
                        {
                            _techlevel = techlevel;
                            break;
                        }
                        else
                        {
                            bbasicstarlaneskipped = true;

                        }
                    }
                    _twoindices[0] = smallestindex;
                    _twoindices[1] = smallestindex1;
                    for (int k = 0; k < 2; k++)
                    {
                        writeindextostarlane = true;
                        writeindextootherstarlane = true;
                        foreach (Starlane starlane in StellarObjects[i].StarLanes)
                        {
                            if (starlane.To == StellarObjects[_twoindices[k]]) writeindextostarlane = false;
                        }
                        foreach (Starlane otherstarlane in StellarObjects[_twoindices[k]].StarLanes)
                        {
                            if (otherstarlane.To == StellarObjects[i]) writeindextootherstarlane = false;
                        }
                        if (writeindextostarlane == true)
                        {
                            _starlane = new Starlane
                            {
                                From = StellarObjects[i],
                                To = StellarObjects[_twoindices[k]],
                                TechLevelRequiredforTravel = _techlevel
                            };

                            StellarObjects[i].StarLanes.Add(_starlane);
                        }
                        if (writeindextootherstarlane == true)
                        {
                            _starlane = new Starlane
                            {
                                From = StellarObjects[_twoindices[k]],
                                To = StellarObjects[i],
                                TechLevelRequiredforTravel = _techlevel
                            };
                            StellarObjects[_twoindices[k]].StarLanes.Add(_starlane);
                        }
                    }
                }
            }
            #endregion
            stringforformattingcounter = String.Format("{0:#,0}", totalcounter);
            stopwatch.Stop();
            Console.WriteLine($"total {stringforformattingcounter} of expected {stringforformattingtotal} loops needed to calculate other starlanes. Elapsed time : {stopwatch.Elapsed}");
            return 1;
        }
        private ObservableCollection<StellarObject> SetListStarsinSpiralArms(int numberofstars, FastRandom rand, int width, int spiralwindedness, bool initbar, bool initspiral)
        {
            ObservableCollection<StellarObject> _stellarobjects = new ObservableCollection<StellarObject>();
            if (initspiral)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    Point3D pnt;
                    int randomizer = 35;
                    int randomizerheight = 25;
                    int numberdividedbytwo = numberofstars / 2;
                    int numberdividedbythree = numberofstars / 3;
                    int numberdividedbyfour = numberofstars / 4;
                    int numberdividedbythreetimestwo = (numberofstars * 2) / 3;
                    double theta, a;
                    double thetaperstar = (Math.PI * spiralwindedness) / numberofstars;
                    double alphaperstar = ((double)((width - 100) / 2) / numberofstars);
                    int cntr = 0;
                    for (int i = 0; i < numberofstars; ++i)  //draw spiralarms
                    {
                        cntr += 1;
                        if (i > numberdividedbythreetimestwo)
                        {
                            randomizer = 20;
                            randomizerheight = 10;
                        }
                        else if (i > numberdividedbytwo)
                        {
                            randomizer = 35;
                            randomizerheight = 15;
                        }
                        else if (i > numberdividedbythree)
                        {
                            randomizer = 65;
                            randomizerheight = 15;
                        }
                        else if (i > numberdividedbyfour)
                        {
                            randomizer = 100;
                            randomizerheight = 15;
                        }
                        else
                        {
                            randomizer = 150;
                            randomizerheight = 15;
                        }
                        //if (multiplier > numberofstars) multiplier = 0;
                        //multiplier += 9;
                        theta = (thetaperstar) * i;// * multiplier;
                        a = (alphaperstar) * i; //*multiplier
                        pnt = new Point3D() { X = a * Math.Cos(theta) + rand.Next(-randomizer, randomizer), Y = a * Math.Sin(theta) + rand.Next(-randomizer, randomizer), Z = rand.Next(-randomizerheight, randomizerheight) };
                        if (!initbar || spiralwindedness > 3 || pnt.X > 90 || pnt.Y > 90 || pnt.X < -90 || pnt.Y < -90)
                        {
                            _stellarobjects.Add(new StellarObject("sp-" + cntr, pnt, rand.NextDouble()));

                        }
                        pnt = new Point3D() { X = -1 * a * Math.Cos(theta) + rand.Next(-randomizer, randomizer), Y = -1 * a * Math.Sin(theta) + rand.Next(-randomizer, randomizer), Z = rand.Next(-randomizerheight, randomizerheight) };
                        if (!initbar || spiralwindedness > 3 || pnt.X > 90 || pnt.Y > 90 || pnt.X < -90 || pnt.Y < -90)
                        {
                            _stellarobjects.Add(new StellarObject("sp-" + cntr, pnt, rand.NextDouble()));
                        }
                    }
                });
            }
            return _stellarobjects;
        }
        private ObservableCollection<StellarObject> SetListStarsinBar(int numberofstars, FastRandom rand, int spiralwindedness, bool initbar)
        {
            ObservableCollection<StellarObject> _stellarobjects = new ObservableCollection<StellarObject>();
            Task task = Task.Factory.StartNew(() =>
            {
                if (spiralwindedness < 4 && initbar)
                {
                    Point3D pnt;
                    int barlength = spiralwindedness > 2 ? 95 : 110;
                    int cntr = 0;
                    for (int i = 0; i < numberofstars && i < 200; i++) //draw bar
                    {
                        cntr += 1;
                        pnt = new Point3D(rand.Next(-18, 18), rand.Next(-barlength, barlength), rand.Next(-18, 18));
                        if (spiralwindedness == 1)
                        {
                            pnt = Rotations.Z(pnt, -65);
                        }
                        else if (spiralwindedness == 2)
                        {
                            pnt = Rotations.Z(pnt, -30);
                        }
                        else
                        {
                            pnt = Rotations.Z(pnt, -15);
                        }
                        _stellarobjects.Add(new StellarObject("br-" + cntr, pnt, rand.NextDouble() * 0.5));
                    }
                }
            });
            return _stellarobjects;
        }
        private ObservableCollection<StellarObject> SetListStarsinBulge(int numberofstars, FastRandom rand, int maxbulgeradius, bool initbulge)
        {
            ObservableCollection<StellarObject> _stellarobjects = new ObservableCollection<StellarObject>();
            if (initbulge)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    int truestarcounter = 0;
                    int cntr = 0;
                    Point3D pnt;
                    double radius, theta, phi;
                    double xylength;
                    int xyzlength;
                    if (MinimumDistancefromCentre > maxbulgeradius) maxbulgeradius = MinimumDistancefromCentre + 10;

                    for (int i = 0; truestarcounter < numberofstars; i++) //draw bulge
                    {
                        xyzlength = 0;
                        cntr += 1;
                        radius = rand.Next(1, maxbulgeradius);
                        theta = (double)rand.Next() / int.MaxValue * 2 * Math.PI;
                        phi = rand.Next(0, 2) == 0 ? (double)rand.Next() / int.MaxValue * Math.PI / 2 : (double)rand.Next() / int.MaxValue * -1 * Math.PI / 2;
                        pnt = new Point3D((int)(radius * Math.Cos(theta) * Math.Cos(phi)), (int)(radius * Math.Sin(phi)), (int)(radius * Math.Sin(theta) * Math.Cos(phi)));
                        if (DrawStarsinCentre == false)
                        {
                            xylength = Math.Pow(pnt.X, 2) + Math.Pow(pnt.Y, 2);
                            xyzlength = Convert.ToInt32(Math.Sqrt(Math.Pow(pnt.Z, 2) + xylength));

                            if (xyzlength > MinimumDistancefromCentre)
                            {
                                _stellarobjects.Add(new StellarObject("bl-" + cntr, pnt, (rand.NextDouble() - 1)* 3)); //
                                truestarcounter += 1;
                            }
                        }
                        else
                        {
                            _stellarobjects.Add(new StellarObject("bl-" + cntr, pnt, (rand.NextDouble() - 1) * 3)); //
                            truestarcounter += 1;
                        }
                    }
                });
            }
            return _stellarobjects;
        }
        private ObservableCollection<StellarObject> SetListStarsinDisc(int numberofstars, FastRandom rand, int width, bool initdisc)
        {
            ObservableCollection<StellarObject> _stellarobjects = new ObservableCollection<StellarObject>();
            if (initdisc)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    double xylength;
                    int xyzlength;
                    int cntr = 0;
                    Point3D pnt;
                    double radius, theta;
                    int truestarcounter = 0;
                    for (int i = 0; truestarcounter < numberofstars; i++) //draw disc
                    {
                        cntr += 1;
                        radius = Math.Sqrt((double)rand.Next() / int.MaxValue) * ((width / 2) - 100);
                        theta = (double)rand.Next() / int.MaxValue * 2 * Math.PI;
                        pnt = new Point3D((int)(radius * Math.Cos(theta)) + rand.Next(-15, 15), (int)(radius * Math.Sin(theta)) + rand.Next(-15, 15), rand.Next(-5, 5));
                        if (DrawStarsinCentre == false)
                        {
                            if (DrawStarsinCentre == false)
                            {
                                xylength = Math.Pow(pnt.X, 2) + Math.Pow(pnt.Y, 2);
                                xyzlength = Convert.ToInt32(Math.Sqrt(Math.Pow(pnt.Z, 2) + xylength));

                                if (xyzlength > MinimumDistancefromCentre)
                                {
                                    _stellarobjects.Add(new StellarObject("dc-" + cntr, pnt, (rand.NextDouble() - 1)* 2)); //
                                    truestarcounter += 1;
                                }
                            }
                        }
                        else
                        {
                            _stellarobjects.Add(new StellarObject("dc-" + cntr, pnt, (rand.NextDouble() - 1) * 2)); //
                            truestarcounter += 1;
                        }
                    }
                });
            }
            return _stellarobjects;
        }

        #endregion

        private ObservableCollection<StellarObject> ReturnCalculateShortestpath(StellarObject olddestinationstellarobject, StellarObject newdestinationstellarobject, int mode) //int sourcestarindex, mode 1 = fewest number of stars in between. mode 2 = shortest path
        {
            StellarPathfromSourcetoDestination.Clear();
            bool bStellarObjectAlreadyUsed;
            ObservableCollection<StellarObject> FinalStellarReversePath = new ObservableCollection<StellarObject>();
            Queue<StellarObject> StellarObjectsQue = new Queue<StellarObject>();
            ObservableCollection<StellarObject> UsedStellarObjects = new ObservableCollection<StellarObject>();
            StellarObject StellarObjectatDestination;
            StellarObject CurrentStellarObject;
            foreach(Starlane starlane in olddestinationstellarobject.StarLanes)
            {
                StellarObjectsQue.Enqueue(starlane.To);
                UsedStellarObjects.Add(starlane.To);
                starlane.To.StellarObjectNearesttoStart = olddestinationstellarobject;
            }
            UsedStellarObjects.Add(olddestinationstellarobject);

            if (mode == 1)
            {
                while (StellarObjectsQue.Count > 0)
                {
                    // each ship has a current actual destination. The next stellar object on it's list to visit.
                    // from this position a path is drawn across all paths, until the new target destination has been reached.
                    // from the fist stellar object, all connected stellar objects are put in a queue. The NearesttoStart property of these stellarobjects is set as 
                    // the 'parent' stellar object from which they were reached.  The NearesttoStart property forms a chain back to the current destination
                    // this is what the current while loop is doing. 
                    CurrentStellarObject = StellarObjectsQue.Dequeue();
                    foreach (Starlane starlane in CurrentStellarObject.StarLanes)
                    {
                        bStellarObjectAlreadyUsed = false;
                        foreach (StellarObject stellarobject in UsedStellarObjects)
                        {
                            if (stellarobject == starlane.To)
                            {
                                bStellarObjectAlreadyUsed = true;
                            }
                        }
                        if (bStellarObjectAlreadyUsed == false)
                        {
                            StellarObjectsQue.Enqueue(starlane.To);
                            starlane.To.StellarObjectNearesttoStart = CurrentStellarObject;
                        }
                        UsedStellarObjects.Add(starlane.To);
                    }
                    // check if the new target destination has been reached. If so, break the while loop
                    if (CurrentStellarObject == newdestinationstellarobject)
                    {
                        break;
                    }
                }
                StellarObjectatDestination = newdestinationstellarobject;
                //after the algorithm has reached the new destination, working backwards : -> from the new destination stellar object, each Stellar object stored in the NearesttoStart property of the chain is stored in
                // finalreversepath list until the old destination stellar object has been reached.  The result is a chain of stellar objects in the reverse order. This list starts with 
                // the new end destination and works it's way up to the destination closest to the ship.   

                foreach (StellarObject stellarobject in StellarObjects)
                {
                    if (StellarObjectatDestination == olddestinationstellarobject)
                    {
                        FinalStellarReversePath.Add(StellarObjectatDestination);
                        break;
                    }
                    FinalStellarReversePath.Add(StellarObjectatDestination);
                    StellarObjectatDestination = StellarObjectatDestination.StellarObjectNearesttoStart;
                }
            }
            return FinalStellarReversePath;
        }
        private int GetDistanceBetweenStars(StellarObject star, StellarObject otherstar)
        {
            int deltax = (int)(star.BeginPosition.X - otherstar.BeginPosition.X);
            int deltay = (int)(star.BeginPosition.Y - otherstar.BeginPosition.Y);
            int deltaz = (int)(star.BeginPosition.Z - otherstar.BeginPosition.Z);
            //deltax = (deltax + (deltax >> 31)) ^ (deltax >> 31);
            //deltay = (deltay + (deltay >> 31)) ^ (deltay >> 31);
            //deltaz = (deltaz + (deltaz >> 31)) ^ (deltaz >> 31);
            //return deltax + deltay + deltaz;
            return deltax * deltax + deltay * deltay + deltaz * deltaz;
        }
        #endregion

        #region public timer events for orbitalbodies

        public void OrbitalBodyDynamics(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand, int turncounter, IReadOnlyList<IFunctionalGroup> functionalgroups, IReadOnlyList<ICompound> compounds)
        {
            double foodproduced = 0;
            double newpeoplecounter = 0;
            double birthscounter = 0;
            double newcomerscounter = 0;
            double deathscounter = 0;
            double spoiledfoodcounter = 0;
            double foodcounter = 0;
            double populationcounter = 0;

            foreach (IStellarObject stellarobject in StellarObjects)
            {
                if (turncounter % 20 == 0)
                {
                    stellarobject.CentralHub.ConstructBuildings(buildingtypes, rand);
                    foreach (Building building in stellarobject.CentralHub.Buildings)
                    {
                        if (building.Type.CanProduceCompounds)
                        {
                            stellarobject.CentralHub.ProduceCompounds(stellarobject, rand);
                            break;
                        }
                    }
                    foreach (Building building in stellarobject.CentralHub.Buildings)
                    {
                        if (building.Type.CanDoChemistry)
                        {

                            stellarobject.CentralHub.DoChemistry(stellarobject, rand, functionalgroups, compounds);
                            break;
                        }
                    }
                }
                if (stellarobject.GlobalDisasterTimer > 0)
                {
                    ((StellarObject)stellarobject).GlobalDisasterTimer -= 1;
                }
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    if (turncounter % 20 == 0)
                    {
                        orbitalbody.ConstructBuildings(buildingtypes, rand);
                        // keep this piece. it is a lot more efficient than the foreach loop.  Try to optimize the foreach 
                        // as far as it goes, so that it might be reimplemented again.
                        for (int i = 0; i < stellarobject.CentralHub.ElementsinStorage.Count; i++)
                        {
                            if (orbitalbody.CentralHub.ElementsinStorage[i].AmountinStorage > 0)
                            {
                                stellarobject.CentralHub.ElementsinStorage[i].AmountinStorage += orbitalbody.CentralHub.ElementsinStorage[i].AmountinStorage;
                                orbitalbody.CentralHub.ElementsinStorage[i].AmountinStorage = 0; //send all the mined elements from previous turn to the stellarobject
                            }
                        }
                        //send all the mined elements from the natural satellites and the orbitalbody to the CentralHub of the orbital body.
                        for (int i = 0; i < orbitalbody.CentralHub.ElementsinStorage.Count; i++)
                        {
                            if (orbitalbody.ElementsinStorage[i].AmountinStorage > 0)
                            {
                                orbitalbody.CentralHub.ElementsinStorage[i].AmountinStorage += orbitalbody.ElementsinStorage[i].AmountinStorage;
                                orbitalbody.ElementsinStorage[i].AmountinStorage = 0; //send all the mined elements from this turn to the orbitalbody
                            }
                        }

                        foreach (IOrbitalBody naturalsatellite in orbitalbody.NaturalSatellites)
                        {
                            {
                                for (int i = 0; i < orbitalbody.CentralHub.ElementsinStorage.Count; i++)
                                {
                                    if (naturalsatellite.ElementsinStorage[i].AmountinStorage > 0)
                                    {
                                        orbitalbody.CentralHub.ElementsinStorage[i].AmountinStorage += naturalsatellite.ElementsinStorage[i].AmountinStorage;
                                        naturalsatellite.ElementsinStorage[i].AmountinStorage = 0; //send all the mined elements from previous turn to the stellarobject
                                    }
                                }
                            }
                        }
                        //foreach loops. the way it is intended? Not very efficient at the moment
                        //foreach (ResourceinStorage CentralHubResourceinStorage in stellarobject.CentralHub.ResourcesonCentralHub)
                        //{
                        //    var tresourceinStorge = orbitalbody.ResourcesinStorage.First(a => a.Resource == CentralHubResourceinStorage.Resource);
                        //    if (tresourceinStorge.Amount > 0)
                        //    {
                        //        CentralHubResourceinStorage.Amount += tresourceinStorge.Amount;
                        //    }
                        //}

                        //    foreach (ResourceinStorage CentralHubResourceinStorage in orbitalbody.CentralHub.ResourcesonCentralHub)
                        //    {
                        //        var tresourceinStorge = naturalsatellite.ResourcesinStorage.First(a => a.Resource == CentralHubResourceinStorage.Resource);
                        //        if (tresourceinStorge.Amount > 0)
                        //        {
                        //            CentralHubResourceinStorage.Amount += tresourceinStorge.Amount;
                        //        }
                        //    }
                        //}


                    }
                    orbitalbody.MineElements(rand);
                    if (orbitalbody.IsHabitable)
                    {
                        orbitalbody.GrowFoodandPopulation(rand, stellarobject); //method sets data of given parameter. its apparently byref
                        foodproduced += orbitalbody.ProducedFoodthisTurn;
                        foodcounter += orbitalbody.Food;
                        foreach (ISpeciesperNode species in orbitalbody.SpeciesonOrbitalBody)
                        {
                            populationcounter += species.PopulationSize;
                        }
                        birthscounter += orbitalbody.BirthsthisTurn;
                        newcomerscounter += orbitalbody.NewcomersthisTurn;
                        deathscounter += orbitalbody.DeathsthisTurn;
                        spoiledfoodcounter += orbitalbody.SpoiledFoodthisTurn;
                        newpeoplecounter += orbitalbody.NewcomersthisTurn;
                        newpeoplecounter += orbitalbody.BirthsthisTurn;
                    }
                    foreach (IOrbitalBody naturalsatellite in orbitalbody.NaturalSatellites)
                    {
                        naturalsatellite.MineElements(rand);
                        if (naturalsatellite.IsHabitable)
                        {
                            naturalsatellite.GrowFoodandPopulation(rand, stellarobject); //method sets data of given parameter. its apparently byref
                            foodproduced += naturalsatellite.ProducedFoodthisTurn;
                            foodcounter += naturalsatellite.Food;
                            foreach (ISpeciesperNode species in naturalsatellite.SpeciesonOrbitalBody)
                            {
                                populationcounter += species.PopulationSize;
                            }
                            birthscounter += naturalsatellite.BirthsthisTurn;
                            newcomerscounter += naturalsatellite.NewcomersthisTurn;
                            deathscounter += naturalsatellite.DeathsthisTurn;
                            spoiledfoodcounter += naturalsatellite.SpoiledFoodthisTurn;
                            newpeoplecounter += naturalsatellite.NewcomersthisTurn;
                        }
                    }
                }
            }
            EventSystem.Publish(new TickerSymbolTotalAmountofFoodandPopulation { ProducedFoodperTurn = ((int)foodproduced).ToString(), NewcomersperTurn = ((int)newcomerscounter).ToString(), BirthsperTurn = ((int)birthscounter).ToString(), SpoiledFoodperTurn = ((int)spoiledfoodcounter).ToString(), DeathsthisTurn = ((int)deathscounter).ToString(), TotalPopulationEndofTurn = ((int)(populationcounter)).ToString(), TotalFoodEndofTurn = ((int)(foodcounter)).ToString() });
        }
 
        #endregion
        #endregion

        //Engines
        //Weapons
        //Shields
        //People   
        //Factions
        //Other stuff to put in ships, planets and stars

    }
}

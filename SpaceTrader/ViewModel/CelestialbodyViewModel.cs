
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public class CelestialBodyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        protected List<StellarObject> _stellarobjectpathfromsourcetodestination;
        protected int _nodesconsidered;
        protected string _timerresults;
        protected bool _initspirals;
        protected int _spiralwindedness;
        protected bool _initbulge;
        protected int _maxbulgeradius;
        protected bool _initbar;
        protected bool _initdisc;
        protected bool _starlanesfirst;
        protected bool _starlanesother;
        protected bool _drawstarsincentre;
        protected int _startnumberofstellarobjects;
        protected int _mindistancefromcentre;
        protected StellarObject _homestellarobject;
        protected StellarObject _stellarobjectselectedonscreen;

        #endregion

        #region Class properties

        public int StartNumberofStellarObjects
        {
            get { return _startnumberofstellarobjects; }
            set
            {
                _startnumberofstellarobjects = value;
                OnPropertyChanged();
            }
        }
        public bool DrawStarsinCentre
        {
            get { return _drawstarsincentre; }
            set
            {
                _drawstarsincentre = value;
                OnPropertyChanged();
            }
        }
        public int MinDistancefromCentre
        {
            get { return _mindistancefromcentre; }
            set
            {
                _mindistancefromcentre = value;
                OnPropertyChanged();
            }
        }
        public string TimerResults
        {
            get { return _timerresults; }
            set
            {
                _timerresults = value;
                OnPropertyChanged();
            }
        }
        public bool StarlanesOther
        {
            get { return _starlanesother; }
            set
            {
                _starlanesother = value;
                OnPropertyChanged();
            }
        }
        public bool StarlanesFirst
        {
            get { return _starlanesfirst; }
            set
            {
                _starlanesfirst = value;
                OnPropertyChanged();
            }
        }
        public bool InitSpiralArms
        {
            get { return _initspirals; }
            set
            {
                if (!value) InitBar = false;
                _initspirals = value;
                OnPropertyChanged();
            }
        }
        public int SpiralWindedness
        {
            get { return _spiralwindedness; }
            set
            {
                if (value < 1) _spiralwindedness = 1;
                else if (value > 15) _spiralwindedness = 15;
                else _spiralwindedness = value;
                OnPropertyChanged();
            }
        }
        public int MaxBulgeRadius
        {
            get { return _maxbulgeradius; }
            set
            {
                if (value < 50) _maxbulgeradius = 50;
                else if (value > 500) _maxbulgeradius = 500;
                else _maxbulgeradius = value;
                OnPropertyChanged();
            }
        }
        public bool InitBulge
        {
            get { return _initbulge; }
            set
            {
                _initbulge = value;
                OnPropertyChanged();
            }
        }
        public bool InitBar
        {
            get { return _initbar; }
            set
            {
                _initbar = value;
                OnPropertyChanged();
            }
        }
        public bool InitDisc
        {
            get { return _initdisc; }
            set
            {
                _initdisc = value;
                OnPropertyChanged();
            }
        }
        public int NodesConsidered
        {
            get { return _nodesconsidered; }
            set
            {
                _nodesconsidered = value;
                OnPropertyChanged();
            }
        }
        public StellarObject HomeStellarObject
        {
            get { return _homestellarobject; }
            set
            {
                _homestellarobject = value;
                OnPropertyChanged();
            }
        }
        public StellarObject StellarObjectSelectedOnScreen
        {
            get { return _stellarobjectselectedonscreen; }
            set
            {
                _stellarobjectselectedonscreen = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StellarObject> StellarPathfromSourcetoDestination = new ObservableCollection<StellarObject>();
        public ObservableCollection<StellarObject> StellarObjects { get; set; } = new ObservableCollection<StellarObject>();
        #endregion

        #region constructor
        public CelestialBodyViewModel()
        {
            
            _mindistancefromcentre = 200;
            _drawstarsincentre = false;
            _maxbulgeradius = 205;
            _spiralwindedness = 5;
            _initspirals = false;
            _initbulge = true;
            _initbar = false;
            _initdisc = false;
            _starlanesother = true;
            _starlanesfirst = true;
            _nodesconsidered = 0;
        }
        #endregion

        #region Class Methods

        #region public methods
        public async Task SetCelestialBodyDatasAsync(int width, IEnumerable<IOrbitalBodyType> orbitalbodytypes, IEnumerable<IStellarObjectType> stellarobjectypes, IReadOnlyList<IResourceGroup> resourcegroups, IEnumerable<TechLevel> techlevels, IReadOnlyList<IBuildingType> buildingtypes, IReadOnlyList<Resource> resources, IPhysicalConstants physicalconstants, ISolarConstants solarconstants)
        {
            #region clear exisiting data;
            StellarPathfromSourcetoDestination.Clear();
            StellarObjects.Clear();
            #endregion
            #region async part of setting celestial object data and properties 
            FastRandom rand = new FastRandom();
            var getspiralstars = GetSpiralStars(StartNumberofStellarObjects, rand, width, SpiralWindedness, InitBar, InitSpiralArms);
            var getbarstars = GetBarStars(StartNumberofStellarObjects, rand, SpiralWindedness, InitBar);
            var getbulgestars = GetBulgeStars(StartNumberofStellarObjects, rand, MaxBulgeRadius, InitBulge);
            var getdiscstars = GetDiscStars(StartNumberofStellarObjects, rand, width, InitDisc);
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
            SetCelestialBodyProperties(orbitalbodytypes, stellarobjectypes, resourcegroups, techlevels, buildingtypes, resources, physicalconstants, solarconstants);
            #endregion  
            return;
        }
        public void SetHomeStar()
        {
            HomeStellarObject = StellarObjectSelectedOnScreen;
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
        private void SetCelestialBodyProperties(IEnumerable<IOrbitalBodyType> orbitalbodytypes, IEnumerable<IStellarObjectType> stellarobjectypes, IReadOnlyList<IResourceGroup> resourcegroups, IEnumerable<TechLevel> techlevels, IReadOnlyList<IBuildingType> buildingtypes, IReadOnlyList<IResource> resources, IPhysicalConstants physicalconstants, ISolarConstants solarconstants)
        {
            StellarPathfromSourcetoDestination.Clear();
            SetStellarObjectProperties(stellarobjectypes, physicalconstants, solarconstants); //set properties of the stellar object population
            SetOrbitalBodyProperties(orbitalbodytypes, physicalconstants); //set properties of the orbital bodies around stellar objects
            SetOrbitalBodyResourceGroups(resourcegroups); //set resourcegroups of each orbital body. Used for extraction modifiers etc.
            SetOrbitalBodyInitialBuildings(buildingtypes); //set initial buildings of each orbital body
            SetOrbitalBodyResourcesatStart(resources);
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

        private void SetOrbitalBodyResourceGroups(IReadOnlyList<IResourceGroup> resourcegroups)
        {
            FastRandom rand = new FastRandom();
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.SetResourceGroupsatStart(rand, resourcegroups);
                }
            }
        }
        private void SetOrbitalBodyResourcesatStart(IReadOnlyList<IResource> resources)
        {
            FastRandom Rand = new FastRandom();

            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.SetAvailableResourcesatStart(Rand, resources);
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

        private int SetOrbitalBodyProperties(IEnumerable<IOrbitalBodyType> OrbitalBodyTypes, IPhysicalConstants physicalconstants)
        {
            int totalrelativeoccurence = 0;
            int randomnumber;
            int relativeoccurencecounter;
            int orbitalbodytypeindex;
            double trand;
            OrbitalBody tmporbitalbody;
            bool bmoonset;
            int charcntr;
            FastRandom rand = new FastRandom();
            foreach (IOrbitalBodyType orbitalbodytypes in OrbitalBodyTypes)
            {
                totalrelativeoccurence += orbitalbodytypes.RelativeOccurence;
            }
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (OrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    relativeoccurencecounter = 0;
                    orbitalbodytypeindex = 0;
                    randomnumber = rand.Next(0, totalrelativeoccurence);
   
                    foreach (IOrbitalBodyType orbitalbodytype in OrbitalBodyTypes)
                    {
                        relativeoccurencecounter += orbitalbodytype.RelativeOccurence;
                        if (randomnumber < relativeoccurencecounter)
                        {
                            trand = rand.NextDouble();
                            orbitalbody.OrbitalBodyType = (BaseTypes.OrbitalBodyType)orbitalbodytype;
                            orbitalbody.IsHabitable = orbitalbody.OrbitalBodyType.IsHabitable; //each orbitalbody instance can be made uninhabitable. at first though it derives habitability from it's type
                            orbitalbody.Mass = Math.Pow(trand, 2) * (orbitalbodytype.Maximum_Mass - orbitalbodytype.Minimum_Mass) + orbitalbodytype.Minimum_Mass;
                            orbitalbody.Radius = Convert.ToInt32(Math.Pow(trand, 2) * (orbitalbodytype.Maximum_Radius - orbitalbodytype.Minimum_Radius) + orbitalbodytype.Minimum_Radius);
                            if (orbitalbody.IsHabitable)
                            {
                                orbitalbody.BaseNaturalBirthsperTurnPercentage = (1 + rand.NextDouble() * 2) / 100;
                                orbitalbody.BaseNaturalHabitationModifier = (rand.NextDouble()) * orbitalbodytype.NaturalHabitationModifier  ;
                                orbitalbody.BaseNaturalDeathsperTurnPercentage = (5 + rand.NextDouble() * 10) / 100;
                            }
                            else
                            {
                                orbitalbody.BaseNaturalHabitationModifier = 0;
                                orbitalbody.BaseNaturalDeathsperTurnPercentage = 0;
                                orbitalbody.BaseNaturalBirthsperTurnPercentage = 0;
                            }
                            orbitalbody.Population = 10;
                            orbitalbody.Food = 10;
                            break;
                        }
                        orbitalbodytypeindex += 1;
                    }
                    if (orbitalbody.OrbitalBodyType.CanHaveMoons == true)
                    {
                        if (rand.Next(1, 9) < 7)
                        {
                            charcntr = 97;
                            for (int i = 0; i < rand.Next(1, 4); i++)
                            {
                                tmporbitalbody = new OrbitalBody(orbitalbody.Name + ((char)charcntr).ToString(), orbitalbody.Age, orbitalbody.BeginPosition);
                                charcntr += 1;
                                bmoonset = rand.Next(1, 100) < 35;
                                foreach (IOrbitalBodyType orbitalbodytype in OrbitalBodyTypes)
                                {
                                    if (orbitalbodytype.CanBeMoon == true)
                                    {
                                        if (bmoonset)
                                        {
                                            trand = rand.NextDouble();
                                            tmporbitalbody.OrbitalBodyType = (BaseTypes.OrbitalBodyType)orbitalbodytype;
                                            tmporbitalbody.IsHabitable = tmporbitalbody.OrbitalBodyType.IsHabitable;
                                            tmporbitalbody.Mass = Math.Pow(trand, 2) * (orbitalbodytype.Maximum_Mass - orbitalbodytype.Minimum_Mass) + orbitalbodytype.Minimum_Mass;
                                            tmporbitalbody.Radius = Convert.ToInt32(Math.Pow(trand, 2) * (orbitalbodytype.Maximum_Radius - orbitalbodytype.Minimum_Radius) + orbitalbodytype.Minimum_Radius);
                                            if (tmporbitalbody.IsHabitable)
                                            {
                                                tmporbitalbody.BaseNaturalBirthsperTurnPercentage = (1 + rand.NextDouble() * 2) / 100;
                                                tmporbitalbody.BaseNaturalDeathsperTurnPercentage = (5 + rand.NextDouble() * 10) / 100;
                                                tmporbitalbody.BaseNaturalHabitationModifier = (rand.NextDouble()) * orbitalbodytype.NaturalHabitationModifier;
                                            }
                                            else
                                            {
                                                tmporbitalbody.BaseNaturalDeathsperTurnPercentage = 0;
                                                tmporbitalbody.BaseNaturalHabitationModifier = 0;
                                                tmporbitalbody.BaseNaturalBirthsperTurnPercentage = 0;
                                            }
                                            tmporbitalbody.Population = 10;
                                            tmporbitalbody.Food = 10;
                                            orbitalbody.NaturalSatellites.Add(tmporbitalbody);
                                            break;
                                        }
                                        bmoonset = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            double distancebetweenorbitalbodies;
            double tcntr ;
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                distancebetweenorbitalbodies = stellarobject.MaximumOrbitalBodyDistanceFromStar / stellarobject.Orbitalbodies.Count;
                tcntr = 0;
                int howmanyinhabitablezone;
                foreach (OrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    howmanyinhabitablezone = rand.Next(1, 3);
                    if (tcntr < howmanyinhabitablezone)
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
                    tcntr += 1;
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
            int tred, tblue, tgreen;
            int deviation = 30;
            double trand;
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
                        trand = rand.NextDouble();
                        tred = rand.Next(stellartype.StarColorRed - deviation, stellartype.StarColorRed + deviation);
                        tgreen = rand.Next(stellartype.StarColorGreen - deviation, stellartype.StarColorGreen + deviation);
                        tblue = rand.Next(stellartype.StarColorBlue - deviation, stellartype.StarColorBlue + deviation);
                        if (tred < 0) tred = 0; if (tred > 255) tred = 255;
                        if (tgreen < 0) tgreen = 0; if (tgreen > 255) tgreen = 255;
                        if (tblue < 0) tblue = 0; if (tblue > 255) tblue = 255;
                        clr = Color.FromRgb((byte)tgreen, (byte)tred, (byte)tblue);
                        stellarobject.StellarType = (BaseTypes.StellarObjectType)stellartype;
                        stellarobject.Mass = Math.Pow(trand, 2) * (stellartype.Maximum_Mass - stellartype.Minimum_Mass) + stellartype.Minimum_Mass;
                        stellarobject.Age = rand.Next(stellartype.Minimum_Age * 1000, stellartype.Maximum_Age * 1000);
                        stellarobject.SurfaceTemperature = Math.Pow(trand, 2) * (stellartype.Maximum_Temp - stellartype.Minimum_Temp) + stellartype.Minimum_Temp;
                        stellarobject.AbsoluteMagnitude = Math.Pow(trand, 2) * (stellartype.Maximum_AbsoluteMagnitude - stellartype.Minimum_AbsoluteMagnitude) + stellartype.Minimum_AbsoluteMagnitude;
                        stellarobject.Radius = Convert.ToInt32(Math.Pow(trand, 2) * (stellartype.Maximum_Radius - stellartype.Minimum_Radius) + stellartype.Minimum_Radius);
                        stellarobject.StarColor = clr;
                        stellarobject.Luminosity = physicalconstants.StefanBoltzmannConstant * (Math.PI * 4 * Math.Pow(stellarobject.Radius * 1000, 2)) * Math.Pow(stellarobject.SurfaceTemperature * 1000, 4);
                        stellarobject.MinimumHabitableZoneRadius = Math.Sqrt(stellarobject.Luminosity / (4 * Math.PI * physicalconstants.WattperM2UpperBoundaryforHabitablezone));
                        stellarobject.MaximumHabitableZoneRadius = Math.Sqrt(stellarobject.Luminosity / (4 * Math.PI * physicalconstants.WattperM2LowerBoundaryforHabitablezone));
                        stellarobject.MaximumOrbitalBodyDistanceFromStar = Math.Sqrt(stellarobject.Luminosity / (4 * Math.PI)); //1 watt / m² is maximum distance

                        for (int i = 1; i < rand.Next(2, 8); i++)
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
        async Task<int> SetStarlanesAsync(FastRandom rand, IEnumerable<TechLevel> techlevels)
        {
            return await Task.Run(() => SetStarlanes(rand, techlevels));
        }

        //Normal synchronous methods
        private int SetStarlanes(FastRandom rand, IEnumerable<TechLevel> techlevels )
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
            List<int> tmpstarlist = new List<int>();
            Stopwatch stopwatch = new Stopwatch();
            bool bbasicstarlaneskipped;
            Starlane tmpstarlane;
            TechLevel tmptechlevel;
            if (StarlanesFirst)
            {
                for (int i = 0; i < StellarObjects.Count; i++)
                {
                    tmpstarlist.Add(i);
                }
                int iremoveindexat = 0;
                int currentindex = 0;
                for (int i = 0; i < StellarObjects.Count; i++)
                {
                    smallestdistance = 100000000;
                    for (int j = 0; j < tmpstarlist.Count; j++)
                    {
                        if (smallestdistance < 200)
                        {
                            break;
                        }

                        if (j != currentindex)
                        {
                            distance = GetDistanceBetweenStars(StellarObjects[currentindex], StellarObjects[tmpstarlist[j]]);
                            if (distance < smallestdistance)
                            {
                                smallestdistance = distance;
                                smallestindex = tmpstarlist[j];
                                iremoveindexat = j;
                            }
                        }
                    }
                    if (smallestdistance < 300000)
                    {
                        bbasicstarlaneskipped = false;
                        tmptechlevel = new TechLevel();
                        foreach (TechLevel techlevel in techlevels)
                        {
                            if (bbasicstarlaneskipped)
                            {
                                tmptechlevel = techlevel;
                                break;
                            }
                            if (rand.Next(1, 17) < 16)
                            {
                                tmptechlevel = techlevel;
                                break;
                            }
                            else
                            {
                                bbasicstarlaneskipped = true;
                            }
                        }
                        tmpstarlane = new Starlane
                        {
                            From = StellarObjects[currentindex],
                            To = StellarObjects[smallestindex],
                            TechLevelRequiredforTravel = tmptechlevel
                        };
                        StellarObjects[currentindex].StarLanes.Add(tmpstarlane);
                        tmpstarlane = new Starlane
                        {
                            From = StellarObjects[smallestindex],
                            To = StellarObjects[currentindex],
                            TechLevelRequiredforTravel = tmptechlevel
                        };
                        StellarObjects[smallestindex].StarLanes.Add(tmpstarlane);
                        currentindex = smallestindex;
                        tmpstarlist.RemoveAt(iremoveindexat);
                    }
                    else
                    {
                        currentindex = smallestindex;
                        tmpstarlist.RemoveAt(iremoveindexat);
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
                int tmpsmallestdistance, tmpsmallestindex, smallestdistance1;
                int[] tmptwoindices = new int[2];
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
                                    tmpsmallestdistance = smallestdistance;
                                    tmpsmallestindex = smallestindex;
                                    smallestdistance = distance;
                                    tmptwoindices[0] = j;
                                    smallestindex = j;
                                    if (tmpsmallestdistance < smallestdistance1)
                                    {
                                        smallestdistance1 = tmpsmallestdistance;
                                        tmptwoindices[1] = tmpsmallestindex;
                                        smallestindex1 = tmpsmallestindex;
                                    }
                                }
                                else if (distance < smallestdistance1)
                                {
                                    smallestdistance1 = distance;
                                    tmptwoindices[1] = j;
                                    smallestindex1 = j;
                                }
                            }
                        }
                    }
                    bbasicstarlaneskipped = false;
                    tmptechlevel = new TechLevel();
                    foreach (TechLevel techlevel in techlevels)
                    {
                        if (bbasicstarlaneskipped)
                        {
                            tmptechlevel = techlevel;
                            break;
                        }
                        if (rand.Next(1, 17) < 16) //ratio of starlane tech 1 and 2
                        {
                            tmptechlevel = techlevel;
                            break;
                        }
                        else
                        {
                            bbasicstarlaneskipped = true;

                        }
                    }
                    tmptwoindices[0] = smallestindex;
                    tmptwoindices[1] = smallestindex1;
                    for (int k = 0; k < 2; k++)
                    {
                        writeindextostarlane = true;
                        writeindextootherstarlane = true;
                        foreach (Starlane starlane in StellarObjects[i].StarLanes)
                        {
                            if (starlane.To == StellarObjects[tmptwoindices[k]]) writeindextostarlane = false;
                        }
                        foreach (Starlane otherstarlane in StellarObjects[tmptwoindices[k]].StarLanes)
                        {
                            if (otherstarlane.To == StellarObjects[i]) writeindextootherstarlane = false;
                        }
                        if (writeindextostarlane == true)
                        {
                            tmpstarlane = new Starlane
                            {
                                From = StellarObjects[i],
                                To = StellarObjects[tmptwoindices[k]],
                                TechLevelRequiredforTravel = tmptechlevel
                            };

                            StellarObjects[i].StarLanes.Add(tmpstarlane);
                        }
                        if (writeindextootherstarlane == true)
                        {
                            tmpstarlane = new Starlane
                            {
                                From = StellarObjects[tmptwoindices[k]],
                                To = StellarObjects[i],
                                TechLevelRequiredforTravel = tmptechlevel
                            };
                            StellarObjects[tmptwoindices[k]].StarLanes.Add(tmpstarlane);
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
            ObservableCollection<StellarObject> tmpstars = new ObservableCollection<StellarObject>();
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
                            tmpstars.Add(new StellarObject("sp-" + cntr, pnt));

                        }
                        pnt = new Point3D() { X = -1 * a * Math.Cos(theta) + rand.Next(-randomizer, randomizer), Y = -1 * a * Math.Sin(theta) + rand.Next(-randomizer, randomizer), Z = rand.Next(-randomizerheight, randomizerheight) };
                        if (!initbar || spiralwindedness > 3 || pnt.X > 90 || pnt.Y > 90 || pnt.X < -90 || pnt.Y < -90)
                        {
                            tmpstars.Add(new StellarObject("sp-" + cntr, pnt));
                        }
                    }
                });
            }
            return tmpstars;
        }
        private ObservableCollection<StellarObject> SetListStarsinBar(int numberofstars, FastRandom rand, int spiralwindedness, bool initbar)
        {
            ObservableCollection<StellarObject> tmpstars = new ObservableCollection<StellarObject>();
            Task task = Task.Factory.StartNew(() =>
            {
                if (spiralwindedness < 4 && initbar)
                {
                    Point3D pnt;
                    int barlength = spiralwindedness > 2 ? 95 : 110;
                    int cntr = 0;
                    for (int i = 0; i < numberofstars && i < 500; i++) //draw bar
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
                        tmpstars.Add(new StellarObject("br-" + cntr, pnt));
                    }
                }
            });
            return tmpstars;
        }
        private ObservableCollection<StellarObject> SetListStarsinBulge(int numberofstars, FastRandom rand, int maxbulgeradius, bool initbulge)
        {
            ObservableCollection<StellarObject> tmpstars = new ObservableCollection<StellarObject>();
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
                    if (MinDistancefromCentre > maxbulgeradius) maxbulgeradius = MinDistancefromCentre + 10;

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

                            if (xyzlength > MinDistancefromCentre)
                            {
                                tmpstars.Add(new StellarObject("bl-" + cntr, pnt)); //
                                truestarcounter += 1;
                            }
                        }
                        else
                        {
                            tmpstars.Add(new StellarObject("bl-" + cntr, pnt)); //
                            truestarcounter += 1;
                        }
                    }
                });
            }
            return tmpstars;
        }
        private ObservableCollection<StellarObject> SetListStarsinDisc(int numberofstars, FastRandom rand, int width, bool initdisc)
        {
            ObservableCollection<StellarObject> tmpstars = new ObservableCollection<StellarObject>();
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

                                if (xyzlength > MinDistancefromCentre)
                                {
                                    tmpstars.Add(new StellarObject("dc-" + cntr, pnt)); //
                                    truestarcounter += 1;
                                }
                            }
                        }
                        else
                        {
                            tmpstars.Add(new StellarObject("dc-" + cntr, pnt)); //
                            truestarcounter += 1;
                        }
                    }
                });
            }
            return tmpstars;
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

        public void BuildBuildingperOrbitalbody(IReadOnlyList<IBuildingType> buildingtypes)
        {
            FastRandom rand = new FastRandom();
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.ConstructBuildings(buildingtypes, rand);
                }
            }
        }      
        public void MineResourcesperOrbitalbody()
        {
            FastRandom Rand = new FastRandom();
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    orbitalbody.MineResources(Rand);
                }
            }
        }

        public void GrownFoodandPopulationperOrbitalbody()
        {
            double foodproduced = 0; 
            double newpeoplecounter = 0;
            double deathscounter = 0;
            double spoiledfoodcounter = 0;
            double foodcounter = 0;
            double populationcounter = 0;
            FastRandom Rand = new FastRandom();
            //population changes
            foreach (IStellarObject stellarobject in StellarObjects)
            {
                foreach (IOrbitalBody orbitalbody in stellarobject.Orbitalbodies)
                {
                    if (orbitalbody.IsHabitable)
                    {
                        orbitalbody.GrowFoodandPopulation(Rand); //method sets data of given parameter. its apparently byref
                        foodproduced += orbitalbody.ProducedFoodthisTurn;
                        foodcounter += orbitalbody.Food;
                        populationcounter += orbitalbody.Population;
                        deathscounter += orbitalbody.DeathsthisTurn;
                        spoiledfoodcounter += orbitalbody.SpoiledFoodthisTurn;
                        newpeoplecounter += orbitalbody.NewcomersthisTurn;
                        newpeoplecounter += orbitalbody.BirthsthisTurn;
                    }
                    foreach (IOrbitalBody naturalsatellite in orbitalbody.NaturalSatellites)
                    {
                        if (naturalsatellite.IsHabitable)
                        {
                            naturalsatellite.GrowFoodandPopulation(Rand); //method sets data of given parameter. its apparently byref
                            foodproduced += naturalsatellite.ProducedFoodthisTurn;
                            foodcounter += naturalsatellite.Food;
                            populationcounter += naturalsatellite.Population;
                            deathscounter += naturalsatellite.DeathsthisTurn;
                            spoiledfoodcounter += naturalsatellite.SpoiledFoodthisTurn;
                            newpeoplecounter += naturalsatellite.NewcomersthisTurn;
                            newpeoplecounter += naturalsatellite.BirthsthisTurn;
                        }
                    }
                }
            }
            EventSystem.Publish(new TickerSymbolTotalAmountofFoodandPopulation { ProducedFoodperTurn =((int)foodproduced).ToString(), BirthsperTurn = ((int)newpeoplecounter).ToString(), SpoiledFoodperTurn = ((int)spoiledfoodcounter).ToString(), DeathsthisTurn = ((int)deathscounter).ToString(), TotalPopulationEndofTurn = ((int)(populationcounter)).ToString(), TotalFoodEndofTurn = ((int)(foodcounter)).ToString() });
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

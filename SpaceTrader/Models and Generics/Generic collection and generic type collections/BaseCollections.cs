using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace SpaceTrader
{
    /// <summary>
    /// This class is only used as a creation-factory from associated .dat files ,
    /// for the creation of collections of resources, resourcegroups, tradegoods and related other collections.
    /// These collections will be stored in ObservableCollection and List types.
    /// Other parts of the program can add these collections to Ships, ORbital Bodies, use them for economic calculation purposes, etc.
    /// For now these collections are :
    /// - Resources
    /// - Resourcegroups
    /// Later on collections of Tradegoods, refined resources, manufactured supplies etc. will be added.
    /// </summary>
    public class BaseCollections //: INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public BaseCollections()
        {
            SetBuildingTypeCollection();
            SetEconomicEntityCollection();
            SetResourceCollection();
            SetResourceGroupCollection();
            SetOrbitalBodyTypeCollection();
            SetStellarObjectTypeCollection();
            SetTechLevelTypeCollection();
        }

        public ObservableCollection<BaseTypes.BuildingType> BuildingTypes = new ObservableCollection<BaseTypes.BuildingType>();
        public ObservableCollection<EconomicEntity> EconomicEntities = new ObservableCollection<EconomicEntity>();
        public List<TechLevel> TechLevelCollection = new List<TechLevel>();
        public ObservableCollection<Resource> Resources = new ObservableCollection<Resource>();
        public ObservableCollection<ResourceGroup> ResourceGroups = new ObservableCollection<ResourceGroup>();
        public ObservableCollection<BaseTypes.OrbitalBodyType> OrbitalbodyTypes = new ObservableCollection<BaseTypes.OrbitalBodyType>();
        public ObservableCollection<BaseTypes.StellarObjectType> StellarObjectTypes = new ObservableCollection<BaseTypes.StellarObjectType>();

        private int SetBuildingTypeCollection()
        {
            BaseTypes.BuildingType tmpbuildingtype;
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/building types.dat")))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        splitstring = line.Split(',');
                        tmpbuildingtype = new BaseTypes.BuildingType
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
                            ChanceofOccuring = double.Parse(splitstring[11].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture)
                        };
                        BuildingTypes.Add(tmpbuildingtype);
                    }
                }
            }
            return 0;
        }
        private int SetEconomicEntityCollection()
        {
            string[] splitstring;
            EconomicEntity tmpEconomicEntity;
            int R, B, G;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/corporateentities.dat")))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        tmpEconomicEntity = new EconomicEntity();
                        // Name, Color RGB
                        splitstring = line.Split(',');
                        R = Convert.ToInt32(splitstring[1]);
                        B = Convert.ToInt32(splitstring[2]);
                        G = Convert.ToInt32(splitstring[3].Trim(new Char[] { '}' }));
                        tmpEconomicEntity.Name = splitstring[0].Trim(new Char[] { '{' });
                        tmpEconomicEntity.Color = Color.FromRgb((byte)R, (byte)B, (byte)G);
                        EconomicEntities.Add(tmpEconomicEntity);
                    }
                }
            }
            return 0;
        }
        private int SetTechLevelTypeCollection()
        {
            TechLevelCollection.Add(new TechLevel("Basic", 1, Color.FromRgb(0, 0, 255)));
            TechLevelCollection.Add(new TechLevel("Advanced", 2, Color.FromRgb(50, 100, 200)));
            TechLevelCollection.Add(new TechLevel("Express", 3, Color.FromRgb(0, 255, 255)));
            return 0;
        }
        private int SetOrbitalBodyTypeCollection()
        {
            string[] splitstring;
            BaseTypes.OrbitalBodyType tmpOrbitalBodyType;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/celestial bodies/orbitalbodydata.dat")))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        // Name, relative occurence, min mass, max mass, max age, color, phase
                        splitstring = line.Split(',');
                        tmpOrbitalBodyType = new BaseTypes.OrbitalBodyType
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
                            SurfaceStateofMatter = Convert.ToInt32(splitstring[13].Trim(new Char[] { '}' }))
                        };
                        OrbitalbodyTypes.Add(tmpOrbitalBodyType);
                    }
                }
            }
            return 0;
        }
        private int SetStellarObjectTypeCollection()
        {
            string[] splitstring;
            BaseTypes.StellarObjectType tmpStellarType;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/celestial bodies/stellarobjectdata.dat")))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        // Name, relative occurence, min mass, max mass, max age, color, phase
                        splitstring = line.Split(',');
                        tmpStellarType = new BaseTypes.StellarObjectType
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
                        StellarObjectTypes.Add(tmpStellarType);
                    }
                }
            }
            return 0;
        }
        private int SetResourceGroupCollection()
        {
            string[] splitstring;
            string[] splitarray;
            int resourcecounter;
            ResourceGroup tmpresourcegroup;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/resource data/resourcegroup.dat")))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        resourcecounter = 0;
                        tmpresourcegroup = new ResourceGroup();
                        splitstring = line.Split(',');
                        tmpresourcegroup.Name = splitstring[0].Trim(new Char[] { '{' });
                        tmpresourcegroup.ResourcegroupExtractionModifier = double.Parse(splitstring[1]);
                        for (int i = 2; i < splitstring.Count(); i++)
                        {
                            splitstring[i].Replace(" ", string.Empty);
                            if (splitstring[i].Contains("-"))
                            {
                                splitarray = splitstring[i].Split('-');
                                for (int j = Convert.ToInt32(splitarray[0]); j < Convert.ToInt32(splitarray[1].Trim(new Char[] { '}' })) + 1; j++)
                                {
                                    tmpresourcegroup.Resources.Add(Resources[j - 1]);
                                    tmpresourcegroup.IntResources.Add(j);
                                    resourcecounter += 1;
                                }
                            }
                            else
                            {
                                tmpresourcegroup.Resources.Add(Resources[Convert.ToInt32(splitstring[i].Trim(new Char[] { '}' })) - 1]);
                                tmpresourcegroup.IntResources.Add(Convert.ToInt32(splitstring[i].Trim(new Char[] { '}' })));
                                resourcecounter += 1;
                            }
                        }
                        ResourceGroups.Add(tmpresourcegroup);
                    }
                }
            }
            return 0;
        }

        private int SetResourceCollection()
        {
            Resource tmpresource;
            string[] splitstring;

            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/resource data/resources.dat")))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {

                        tmpresource = new Resource();
                        splitstring = line.Split(',');
                        tmpresource.Name = splitstring[0].Trim(new Char[] { '{' });
                        tmpresource.UniversalAbundance = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                        if (tmpresource.UniversalAbundance == 0)
                        {
                            tmpresource.UniversalAbundance = 0.000000001;
                        }
                        tmpresource.StateofMatter = Convert.ToInt32(splitstring[2]);
                        tmpresource.IsRadioActive = Convert.ToInt32(splitstring[3].Trim(new Char[] { '}' })) == 1 ? tmpresource.IsRadioActive = true : tmpresource.IsRadioActive = false; // Convert.ToBoolean(splitstring[3]);
                        Resources.Add(tmpresource);
                    }
                }
            }
            return 0;
        }
    }
}

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
    ///     
    public class BaseTaxonomyCollections //: INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public BaseTaxonomyCollections()
        {
            SetTaxonomyCollections();
            foreach (Taxonomy.Class tclass in Classes)
            {
                Console.WriteLine($"Class :  {tclass.Name} -> SubPhylum : {tclass.SubPhylum.Name} -> Phylum : {tclass.SubPhylum.Phylum.Name} -> Kingdom : {tclass.SubPhylum.Phylum.Kingdom.Name} -> Domain : {tclass.SubPhylum.Phylum.Kingdom.Domain.Name}");
            }
            foreach (Taxonomy.Order order in Orders)
            {
                Console.WriteLine($"Order :  {order.Name} -> Class : {order.Class.Name}");
            }
            foreach (Taxonomy.Species species in Species)
            {
                Console.WriteLine($"Species :  {species.Name} -> Genus : {species.Genus.Name} -> Family : {species.Genus.Family.Name}-> Order : {species.Genus.Family.Order.Name}");
            }
        }
        #region collections 
        public ObservableCollection<Taxonomy.Domain> Domains = new ObservableCollection<Taxonomy.Domain>();
        public ObservableCollection<Taxonomy.Kingdom> Kingdoms = new ObservableCollection<Taxonomy.Kingdom>();
        public ObservableCollection<Taxonomy.Phylum> Phyla = new ObservableCollection<Taxonomy.Phylum>();
        public ObservableCollection<Taxonomy.SubPhylum> SubPhyla = new ObservableCollection<Taxonomy.SubPhylum>();
        public ObservableCollection<Taxonomy.Class> Classes = new ObservableCollection<Taxonomy.Class>();
        public ObservableCollection<Taxonomy.Order> Orders = new ObservableCollection<Taxonomy.Order>();
        public ObservableCollection<Taxonomy.Family> Families = new ObservableCollection<Taxonomy.Family>();
        public ObservableCollection<Taxonomy.Genus> Geni = new ObservableCollection<Taxonomy.Genus>();
        public ObservableCollection<Taxonomy.Species> Species = new ObservableCollection<Taxonomy.Species>();
        #endregion

        private int SetTaxonomyCollections()
        {
            Taxonomy.Domain tmpdomain;
            Taxonomy.Kingdom tmpkingdom;
            Taxonomy.Phylum tmpphylum;
            Taxonomy.SubPhylum tmpsubphylum;
            Taxonomy.Class tmpclass;
            Taxonomy.Order tmporder;
            Taxonomy.Family tmpfamily;
            Taxonomy.Genus tmpgenus;
            Taxonomy.Species tmpspecies;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(@"resources/taxonomy/domain.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpdomain = new Taxonomy.Domain
                {
                    Name = splitstring[0].Trim(new Char[] { '{' })
                };
                Domains.Add(tmpdomain);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/kingdom.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpkingdom = new Taxonomy.Kingdom
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    Domain = Domains[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1]
                };
                Kingdoms.Add(tmpkingdom);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/phylum.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpphylum = new Taxonomy.Phylum
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    Kingdom = Kingdoms[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1]
                };
                Phyla.Add(tmpphylum);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/subphylum.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpsubphylum = new Taxonomy.SubPhylum
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    Phylum = Phyla[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1]
                };
                SubPhyla.Add(tmpsubphylum);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/class.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpclass = new Taxonomy.Class
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    SubPhylum = SubPhyla[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1]
                };
                Classes.Add(tmpclass);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/order.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmporder = new Taxonomy.Order
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    Class = Classes[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1]
                };
                Orders.Add(tmporder);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/family.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpfamily = new Taxonomy.Family
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    Order = Orders[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1]
                };
                Families.Add(tmpfamily);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/genus.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpgenus = new Taxonomy.Genus
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    Family = Families[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1]
                };
                Geni.Add(tmpgenus);
            }
            stringfromfile = FileActions.ReadData(@"resources/taxonomy/species.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpspecies = new Taxonomy.Species
                {
                    Name = splitstring[0].Trim(new Char[] { '{' }),
                    Genus = Geni[Convert.ToInt32(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) - 1],
                    ReproductionRate = double.Parse(splitstring[2].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture),
                };
                Species.Add(tmpspecies);
            }
            return 0;
        }
    }
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
            string[] stringfromfile = FileActions.ReadData(@"resources/building types.dat", 1);
            foreach (string line in stringfromfile)
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
            return 0;
        }
        private int SetEconomicEntityCollection()
        {
            string[] splitstring;
            EconomicEntity tmpEconomicEntity;
            int R, B, G;
            string[] stringfromfile = FileActions.ReadData(@"resources/corporateentities.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                tmpEconomicEntity = new EconomicEntity();
                // Name, Color RGB
                R = Convert.ToInt32(splitstring[1]);
                B = Convert.ToInt32(splitstring[2]);
                G = Convert.ToInt32(splitstring[3].Trim(new Char[] { '}' }));
                tmpEconomicEntity.Name = splitstring[0].Trim(new Char[] { '{' });
                tmpEconomicEntity.Color = Color.FromRgb((byte)R, (byte)B, (byte)G);
                EconomicEntities.Add(tmpEconomicEntity);
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
            string[] stringfromfile = FileActions.ReadData(@"resources/celestial bodies/orbitalbodydata.dat", 1);
            foreach (string line in stringfromfile)
            {
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
            return 0;
        }
        private int SetStellarObjectTypeCollection()
        {
            string[] splitstring;
            BaseTypes.StellarObjectType tmpStellarType;
            string[] stringfromfile = FileActions.ReadData(@"resources/celestial bodies/stellarobjectdata.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                // Name, relative occurence, min mass, max mass, max age, color, phase
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
            return 0;
        }
        private int SetResourceGroupCollection()
        {
            string[] splitstring;
            string[] splitarray;
            int resourcecounter;
            ResourceGroup tmpresourcegroup;
            string[] stringfromfile = FileActions.ReadData(@"resources/resource data/resourcegroup.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');
                resourcecounter = 0;
                tmpresourcegroup = new ResourceGroup();
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
            return 0;
        }

        private int SetResourceCollection()
        {

            Resource tmpresource;
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(@"resources/resource data/resources.dat", 1);
            foreach (string line in stringfromfile)
            {
                splitstring = line.Split(',');

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
            return 0;
        }
    }
}

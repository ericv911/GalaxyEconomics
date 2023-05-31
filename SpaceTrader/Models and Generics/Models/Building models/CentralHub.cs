using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Construction;
using Common.Logistics;
using Common.Physics;
using CompoundProvider.Types;
using FunctionalGroups.Types;

namespace SpaceTrader
{
    public interface ICentralHub
    {
        ObservableCollection<ElementinStorage> ElementsinStorage { get; }
    }
    public class CentralHub : ICentralHub, INotifyPropertyChanged
    {
        // this class represents a central Trading Hub and accespoint for interstellar trade. Elements, Resources and goods from orbital bodies inside the stellar system flow to this point automatically
        // ships from this and other stellar object systems can load these goods and unload them at the Trading Hub of another stellar object.  
        // Besides a trading Hub, this is the portal to other stellar systems in every way. Immigration, Cultural, Emigration, Diplomacy, etc. etc.  
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FullyObservableCollection<Building> Buildings { get; set; }
        public FullyObservableCollection<FunctionalGroupinStorage> FunctionalGroupsinStorage { get; set; }
        public ObservableCollection<ElementinStorage> ElementsinStorage { get; set; }
        public ObservableCollection<CompoundinStorage> CompoundsinStorage { get; set; }
        public ObservableCollection<Compound> CompoundsResearched { get; set; }
        public ObservableCollection<FunctionalGroup> FunctionalGroupsResearched { get; set; }

        public CentralHub()
        {
            FunctionalGroupsResearched = new ObservableCollection<FunctionalGroup>();
            FunctionalGroupsinStorage = new FullyObservableCollection<FunctionalGroupinStorage>();
            CompoundsinStorage = new ObservableCollection<CompoundinStorage>();
            CompoundsResearched = new ObservableCollection<Compound>();
            Buildings = new FullyObservableCollection<Building>();
            ElementsinStorage = new ObservableCollection<ElementinStorage>();
        }
        public void ProduceCompounds(IStellarObject stellarobject, FastRandom rand) // Compoundsinstorage have been researched. They can be produced by a Chemical Factory or something like that. 
        {
            return;
            int productioncapacity = 0;
            bool productionfacilitypresent = false;
            foreach (Building building in stellarobject.CentralHub.Buildings) //check if there is a production facility present. Add the levels of production
            {
                if (building.Type.CanProduceCompounds)
                {
                    productionfacilitypresent = true;
                    productioncapacity += building.Size;
                }
            }
            if (!productionfacilitypresent)  
            {
                return;
            }
            #region local variables
            bool compoundalreadyinstorage;
            double _amountofcompoundmade;
            double masscontributionelement1;
            double masscontributionelement2;
            #endregion
            #region production of 2-element compounds 

            foreach (Compound compound in CompoundsResearched)
            {
                compoundalreadyinstorage = false;
                _amountofcompoundmade = productioncapacity*(rand.NextDouble() * 5);
                masscontributionelement1 = _amountofcompoundmade * (((double)compound.ElementsinCompound[0].AmountinCompound * compound.ElementsinCompound[0].Element.AtomicMass) / compound.AtomicMass);
                masscontributionelement2 = _amountofcompoundmade * (((double)compound.ElementsinCompound[1].AmountinCompound * compound.ElementsinCompound[1].Element.AtomicMass) / compound.AtomicMass);
                var _elementinstorage1 = (from _elementsinstorageoncentralhub in ElementsinStorage
                                          where _elementsinstorageoncentralhub.Element == compound.ElementsinCompound[0].Element
                                          select _elementsinstorageoncentralhub).First();
                var _elementinstorage2 = (from _elementsinstorageoncentralhub in ElementsinStorage
                                          where _elementsinstorageoncentralhub.Element == compound.ElementsinCompound[1].Element
                                          select _elementsinstorageoncentralhub).First();
                _elementinstorage1.AmountinStorage -= _amountofcompoundmade * masscontributionelement1;
                _elementinstorage2.AmountinStorage -= _amountofcompoundmade * masscontributionelement2;
                foreach (CompoundinStorage compoundinstorage in CompoundsinStorage)
                {
                    if (compound == compoundinstorage.Compound)
                    {
                        compoundalreadyinstorage = true;
                        compoundinstorage.AmountinStorage += _amountofcompoundmade;
                        break;
                    }
                }
                if (compoundalreadyinstorage == false)
                {
                    CompoundsinStorage.Add(new CompoundinStorage(compound, _amountofcompoundmade));
                }
                #endregion
                //Console.WriteLine($"\nOLD-{stellarobject.Name} productioncapacity level : {productioncapacity} -> amount produced : {_amountofcompoundmade} -> ingredients : {compound.ElementsinCompound[0].AmountinCompound}-{compound.ElementsinCompound[0].Element.Name}-<>-{compound.ElementsinCompound[1].AmountinCompound}-{compound.ElementsinCompound[1].Element.Name}");
                //Console.Write($"  -> element1 : {_elementinstorage1.Element.Name} amount*atomicmass {compound.ElementsinCompound[0].AmountinCompound:F2}*{_elementinstorage1.Element.AtomicMass:F2} : {compound.ElementsinCompound[0].AmountinCompound*_elementinstorage1.Element.AtomicMass:F2} -> {masscontributionelement1:F2}");
                //Console.Write($"  -> element2 : {_elementinstorage2.Element.Name} amount*atomicmass {compound.ElementsinCompound[1].AmountinCompound:F2}*{_elementinstorage2.Element.AtomicMass:F2} : {compound.ElementsinCompound[1].AmountinCompound * _elementinstorage2.Element.AtomicMass:F2}-> {masscontributionelement2:F2} used. \n ");
                //Console.WriteLine($"  --> element1/element2 ratio amount*atomicmass {((compound.ElementsinCompound[0].AmountinCompound * _elementinstorage1.Element.AtomicMass) / (compound.ElementsinCompound[1].AmountinCompound * _elementinstorage2.Element.AtomicMass)):F2} -<>- ratio masscontributions :{(masscontributionelement1 / masscontributionelement2):F2}");
            }
        }
        public void DoChemistry(IStellarObject stellarobject, FastRandom rand, IReadOnlyList<IFunctionalGroup> functionalgroups, IReadOnlyList<ICompound> compounds)//succesful Chemistry will add compounds to the CompoundsinStorageList
        {
            //CompoundsResearched.Add((Compound)compounds[rand.Next(1, compounds.Count())]);
            bool enoughamountofelement1present;
            bool enoughamountofelement2present;
            bool compoundpresent, compoundnotallowed;
           // return;
            FunctionalGroup _functionalgroup;
            //ObservableCollection<Compound> _compounds = new ObservableCollection<Compound>();
           // _compounds = chemistry.DoChemistry(chemistry.Rules); //_compounds are the researched compounds-list of the current turn
            foreach (Compound _compound in compounds)
            {
                compoundnotallowed = false;
                compoundpresent = false;
                #region check to see if element should be added
                if (_compound.ElementsinCompound[0].Element.ElectroNegativity > _compound.ElementsinCompound[1].Element.ElectroNegativity)  //always write the more electronegative element to the right
                {
                    compoundnotallowed = true;
                }
                else
                {
                    foreach (Compound compound in CompoundsResearched)
                    {
                        if (compound.ElementsinCompound[0].Element == _compound.ElementsinCompound[0].Element && compound.ElementsinCompound[1].Element == _compound.ElementsinCompound[1].Element && compound.OxidationState == _compound.OxidationState
                            && compound.ElementsinCompound[0].AmountinCompound == _compound.ElementsinCompound[0].AmountinCompound && compound.ElementsinCompound[1].AmountinCompound == _compound.ElementsinCompound[1].AmountinCompound)
                        {
                            if (Math.Abs(compound.ElementsinCompound[0].OxidationState) == Math.Abs(_compound.ElementsinCompound[0].OxidationState))
                            {
                                //if (compound.OxidationState == _compound.OxidationState)
                                {
                                    compoundpresent = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region add element to Researched Collection
                if (compoundpresent == false && compoundnotallowed == false)
                {
                    enoughamountofelement1present = false;
                    enoughamountofelement2present = false;
                    var __element = (from _elementsoncentralhub in ElementsinStorage
                                     where _elementsoncentralhub.Element == _compound.ElementsinCompound[0].Element
                                     select _elementsoncentralhub).First();
                    if (__element.AmountinStorage > 10)
                    {
                        enoughamountofelement1present = true;
                    }
                    __element = (from _elementsoncentralhub in ElementsinStorage
                                 where _elementsoncentralhub.Element == _compound.ElementsinCompound[1].Element
                                 select _elementsoncentralhub).First();
                    if (__element.AmountinStorage > 10)
                    {
                        enoughamountofelement2present = true;
                    }
                    if (enoughamountofelement1present && enoughamountofelement2present)
                    {
                        CompoundsResearched.Add(_compound);
                        if (_compound.IsFunctionalGroup)
                        {
                            _functionalgroup = new FunctionalGroup
                            {
                                Name = " - NA ",
                                Charge = _compound.OxidationState,
                                AtomicMass = _compound.AtomicMass,
                                ElementsinFunctionalGroup = _compound.ElementsinCompound
                            };
                            foreach (FunctionalGroup functionalgroup in functionalgroups)
                            {

                                if (functionalgroup.AtomicMass == _functionalgroup.AtomicMass)
                                {
                                    _functionalgroup.Charge = functionalgroup.Charge;
                                    _functionalgroup.Name = functionalgroup.Name;
                                    break;
                                }
                            }
                            FunctionalGroupsResearched.Add(_functionalgroup);
                        }
                    }
                }
                #endregion
            }
        }
        public void ConstructBuildings(IReadOnlyList<IBuildingType> buildingtypes, FastRandom rand)
        {
            bool alreadybuilt;
            Building _building;
            if (rand.Next(1, 10) < 8)
            {
                alreadybuilt = false;
                _building = new Building
                {
                    Type = (BuildingType)buildingtypes[rand.Next(0, buildingtypes.Count)]
                };
                if (_building.Type.WhereCanItBeBuilt > 1)
                {
                    return;
                }
                foreach (Building building in Buildings)
                {
                    if (_building.Type == building.Type)
                    {
                        if (building.Type.CanResize == true)
                        {
                            building.Size += 1;
                        }
                        alreadybuilt = true;
                        break;
                    }
                }
                if (!alreadybuilt && _building.Type.CanBeBuilt)
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

        #region Central Hub -> Add and Remove Elements 
        /// <summary>
        /// add the amount of the element specified in the elementonship to the central hub
        /// </summary>
        /// <param name="elementonship"></param>
        public void AddElementtoStorage(ElementinStorage elementonship)  //elementinStorage contains both a element and a specified amount
        {
            var elementoncentralhubtochange = (from _elementsoncentralhub in ElementsinStorage
                                   where _elementsoncentralhub.Element == elementonship.Element
                                   select _elementsoncentralhub).First();
            elementoncentralhubtochange.AmountinStorage += elementonship.AmountinStorage;
        }
        /// <summary>
        /// add the amount of the element specified in the elementonship to the central hub
        /// </summary>
        /// <param name="element"></param>
        /// <param name="amount"></param>
        public void AddElementtoStorage(Element element, double amount)  //elementinStorage contains both a element and a specified amount
        {
            var elementoncentralhubtochange = (from _elementsoncentralhub in ElementsinStorage
                                              where _elementsoncentralhub.Element == element
                                              select _elementsoncentralhub).First();
            elementoncentralhubtochange.AmountinStorage += amount;
        }
        /// <summary>
        /// reduce the amount of specified element on the central hub by the specified amount
        /// </summary>
        /// <param name="elementonship"></param>
        public void RemoveElementfromStorage(ElementinStorage elementonship)
        {
            var elementoncentralhubtochange = (from _elementsoncentralhub in ElementsinStorage
                                              where _elementsoncentralhub.Element == elementonship.Element
                                              select _elementsoncentralhub).First();
            elementoncentralhubtochange.AmountinStorage -= elementonship.AmountinStorage;
        }

        /// <summary>
        /// reduce the amount of specified element on the central hub by the specified amount
        /// </summary>
        /// <param name="element"></param>
        /// <param name="amount"></param>
        public void RemoveElementfromStorage(Element element, double amount)
        {
            var elementoncentralhubtochange = (from _elementsoncentralhub in ElementsinStorage
                                                where _elementsoncentralhub.Element == element
                                                select _elementsoncentralhub).First();
            elementoncentralhubtochange.AmountinStorage -= amount;
        }

        /// <summary>
        /// set the amount of the specified element on the central hub to 0
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElementfromStorage(Element element)  //in case that all of the amount of the element loaded on ship, then amount of the element on centralhub will be set to 0
        {
            var elementoncentralhubtochange = (from _elementsoncentralhub in ElementsinStorage
                                               where _elementsoncentralhub.Element == element
                                               select _elementsoncentralhub).First();
            elementoncentralhubtochange.AmountinStorage = 0;
        }
        #endregion
    }
}

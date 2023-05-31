using ChemistryDynamics;
using ChemistryDynamics.Types;
using Common.Physics;
using CompoundProvider.Types;
using FileHandlingSystem;
using FunctionalGroups.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompoundProvider
{
    internal class CompoundCollection : ICompoundCollection
    {
        public ObservableCollection<Compound> Compounds { get; set; } = new ObservableCollection<Compound>();
        private readonly ObservableCollection<ICompoundRule> IChemistryRules = new ObservableCollection<ICompoundRule>();

        /// <summary>
        /// create the CompoundCollection.Compounds collection
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="elementgroups"></param>
        /// <param name="elements"></param>
        /// <param name="functionalgroups"></param>
        internal CompoundCollection(string filepath, ObservableCollection<ElementGroup> elementgroups, ObservableCollection<Element> elements, ObservableCollection<FunctionalGroup> functionalgroups)
        {
            IChemistryRules =(RulesFactory.GetRulesCollection(elementgroups, elements, functionalgroups).Rules);
            CreateAllCompounds();
        }
        /// <summary>
        /// Cycle through all rules, create compounds and add them all to one collection of all possible compounds.
        /// This collection will be returned. 
        /// </summary> 
        /// <param name="elementgroups"></param>
        /// <param name="elements"></param>
        /// <param name="functionalgroups"></param>
        /// <returns></returns>
        private ObservableCollection<Compound> CreateAllCompounds()
        {
            foreach (ICompoundRule chemistryrule in IChemistryRules)
            {
                foreach (Compound compound in CreateCompoundsfromRule(chemistryrule))
                {
                    Compounds.Add(compound);
                }
            }
            return Compounds;
        }
        /// <summary>
        /// Create all possible compounds from one specific chemistry rules.
        /// </summary>
        /// <param name="chemistryrule"></param>
        /// <returns></returns>
        private ObservableCollection<Compound> CreateCompoundsfromRule(ICompoundRule chemistryrule)
        {
            ObservableCollection<Compound> _compounds = new ObservableCollection<Compound>();
            ObservableCollection<Compound> __compounds;

            Compound _compound;
            foreach (ConstituentofCompoundRule constituent in chemistryrule.ConstituentsofCompoundRule)
            {
                _compound = new Compound
                {
                    IsFunctionalGroup = chemistryrule.IsFunctionalGroup,
                    OxidationState = chemistryrule.Charge
                };
                __compounds = new ObservableCollection<Compound>();
                switch (constituent.ConstituentType)
                {
                    case 1:
                        if (_compounds.Count() > 0)
                        {
                            foreach (Compound __compound in _compounds)
                            {
                                //split up the existing compound into  new compounds containing the old data and per element from the element group, a new copy with the new element data
                                foreach (Element element in constituent.ElementGroup.Elements)
                                {
                                    _compound = new Compound();
                                    // _compound = __compound does not work, due to the reference to the object not being deleted properly somewhere. 
                                    // all subsequent entries in the foreach loop will get added and added and added to the original one, resulting in a big mess.
                                    // So have to rewrite the data from the old __compound to the new _compound in the following way : 
                                    foreach (ElementinCompound eic in __compound.ElementsinCompound)
                                    {
                                        _compound.ElementsinCompound.Add(eic);
                                    }
                                    foreach (FunctionalGroupinCompound fic in __compound.FunctionalGroupsinCompound)
                                    {
                                        _compound.FunctionalGroupsinCompound.Add(fic);
                                    }
                                    _compound.IsFunctionalGroup = chemistryrule.IsFunctionalGroup;
                                    _compound.OxidationState = chemistryrule.Charge;
                                    //add new data to the compound
                                    _compound.ElementsinCompound.Add(new ElementinCompound(element, constituent.Amount, 0));
                                    __compounds.Add(_compound);
                                }
                            }
                            _compounds = new ObservableCollection<Compound>();
                            foreach (Compound ___compound in __compounds)
                            {
                                _compounds.Add(___compound);
                            }
                        }
                        else
                        {
                            foreach (Element element in constituent.ElementGroup.Elements)
                            {
                                _compound = new Compound
                                {
                                    IsFunctionalGroup = chemistryrule.IsFunctionalGroup,
                                    OxidationState = chemistryrule.Charge
                                };
                                _compound.ElementsinCompound.Add(new ElementinCompound(element, constituent.Amount, 0));
                                _compounds.Add(_compound);
                            }
                        }
                        break;
                    case 2:
                        //if (_compounds.Count() > 0)
                        //{
                        //    foreach (Compound compound in _compounds)
                        //    {
                        //        compound.FunctionalGroupsinCompound.Add(new FunctionalGroupinCompound(constituent.FunctionalGroup, constituent.Amount));
                        //    }
                        //}
                        //else
                        //{
                        //    _compound.FunctionalGroupsinCompound.Add(new FunctionalGroupinCompound(constituent.FunctionalGroup, constituent.Amount));
                        //    _compounds.Add(_compound);
                        //}
                        break;
                    case 3:
                        if (_compounds.Count() > 0)
                        {
                            foreach (Compound compound in _compounds)
                            {
                                compound.ElementsinCompound.Add(new ElementinCompound(constituent.Element, constituent.Amount, 0));
                            }
                        }
                        else
                        {
                            _compound.ElementsinCompound.Add(new ElementinCompound(constituent.Element, constituent.Amount, 0));
                            _compounds.Add(_compound);
                        }
                        break;
                    default:
                        break;
                }
            }

            // check for illegal compounds
            __compounds = new ObservableCollection<Compound>();
            // ObservableCollection<Compound> ____compounds = new ObservableCollection<Compound>();
            foreach (Compound compound in _compounds)
            {
                if (!chemistryrule.CanConstituentsbeEqual)
                {
                    if (!DuplicateElementsPresent(compound))
                    {
                        __compounds.Add(compound);
                    }
                }
                else

                {
                    __compounds.Add(compound);
                }
            }
            _compounds = new ObservableCollection<Compound>();
            foreach (Compound compound in __compounds)
            {
                if (!chemistryrule.CanElectroNegativeElementsbeOnLeftSideofFormula)
                {
                    if (!MoreElectroNegativeElementonLeft(compound))
                    {
                        _compounds.Add(compound);
                    }
                }
                else
                {
                    _compounds.Add(compound);
                }
            }
            __compounds = new ObservableCollection<Compound>();
            bool elementtooradioactivetoformcompounds;
            foreach (Compound compound in _compounds)
            {
                elementtooradioactivetoformcompounds = false;
                foreach (ElementinCompound eic in compound.ElementsinCompound)
                {
                    if (eic.Element.UniversalAbundance < 0.00001)
                    {
                        elementtooradioactivetoformcompounds = true;
                    }
                }
                if (!elementtooradioactivetoformcompounds)
                {
                    __compounds.Add(compound);
                }
            }
            CalculateCompoundMasses(__compounds);
            return __compounds;
        }

        /// <summary>
        /// Calculate masses for the compounds after the collection of all compounds has been created. 
        /// </summary>
        /// <param name="compounds"></param>
        private void CalculateCompoundMasses(ObservableCollection<Compound> compounds)
        {
            double _mass;
            foreach (Compound compound in compounds)
            {
                _mass = 0;
                foreach (ElementinCompound eic in compound.ElementsinCompound)
                {
                    _mass += eic.Element.AtomicMass * eic.AmountinCompound;
                }
                compound.AtomicMass = _mass;
            }
        }

        /// <summary>
        /// Check order of electronegativity in the order of the elements in the compound. 
        /// </summary>
        /// <param name="compound"></param>
        /// <returns></returns>
        private bool MoreElectroNegativeElementonLeft(Compound compound)
        {
            for (int i = 0; i < compound.ElementsinCompound.Count(); i++)
            {
                for (int j = 0; j < compound.ElementsinCompound.Count(); j++)
                {
                    if (i < j)
                    {
                        if (compound.ElementsinCompound[i].Element.ElectroNegativity > compound.ElementsinCompound[j].Element.ElectroNegativity)
                        {
                            //Console.WriteLine("electronegative " + compound.ElementsinCompound[i].Element.Name + "  " + compound.ElementsinCompound[j].Element.Name);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if Duplicate elements are present in the compound rule.
        /// This is to handle compound groups that react with themselves, like the Chalcogens.
        /// SulfurDioxide is a valid compound Sulfur diSulfure isn't
        /// </summary>
        /// <param name="compound"></param>
        /// <returns></returns>
        private bool DuplicateElementsPresent(Compound compound)
        {
            for (int i = 0; i < compound.ElementsinCompound.Count(); i++)
            {
                for (int j = 0; j < compound.ElementsinCompound.Count(); j++)
                {
                    if (i != j)
                    {
                        if (compound.ElementsinCompound[i].Element == compound.ElementsinCompound[j].Element)
                        {
                            //Console.WriteLine("duplicate " + compound.ElementsinCompound[i].Element.Name + "  " + compound.ElementsinCompound[j].Element.Name);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}

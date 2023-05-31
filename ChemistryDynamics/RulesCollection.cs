using ChemistryDynamics.Types;
using Common.Physics;
using FileHandlingSystem;
using FunctionalGroups.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemistryDynamics
{
    internal class RulesCollection : IRulesCollection
    {
        public ObservableCollection<ICompoundRule> Rules { get; set;  } = new ObservableCollection<ICompoundRule>();

        //public RulesCollection(string filepath, ObservableCollection<ElementGroup> elementgroups, ObservableCollection<Element> elements)
        public RulesCollection(string filepath, ObservableCollection<ElementGroup> elementgroups, ObservableCollection<Element> elements, ObservableCollection<FunctionalGroup> functionalgroups)
        {
            SetChemistryRulesCollection(filepath, elementgroups, elements, functionalgroups);
        }

        /// <summary>
        /// Converts the chemistryrules loaded from rules.dat in ObservableCollection ChemistryRules to  to an observableCollection of all possible Compounds that can be made from them
        /// Additional constraints for Compound-creation can be put here. Such as relative Cosmic abundance of elements, or lifetime of the most stable isotope.
        /// </summary>
        /// <param name="chemistryrule"></param>
        /// <returns></returns>
        /// 
        // private ObservableCollection<ICompoundRule> SetChemistryRulesCollection(string filepath, ObservableCollection<ElementGroup> elementgroups, ObservableCollection<Element> elements)
        private ObservableCollection<ICompoundRule> SetChemistryRulesCollection(string filepath, ObservableCollection<ElementGroup> elementgroups, ObservableCollection<Element> elements, ObservableCollection<FunctionalGroup> functionalgroups)
        {
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(filepath + "compoundrules.dat", 1);
            CompoundRule _chemistryrule;
            ConstituentofCompoundRule _constituentofcompoundrule;
            foreach (string line in stringfromfile)
            {
                _chemistryrule = new CompoundRule();
                splitstring = line.Split(',');
                _chemistryrule.NumberofConstituentsinCompound = Convert.ToInt32(splitstring[0].Trim(new Char[] { '{' }));
                _chemistryrule.IsFunctionalGroup = Convert.ToBoolean(Convert.ToInt32(splitstring[1]));
                _chemistryrule.Charge = Convert.ToInt32(splitstring[2]);
                _chemistryrule.CanConstituentsbeEqual = Convert.ToBoolean(Convert.ToInt32(splitstring[3]));
                _chemistryrule.CanElectroNegativeElementsbeOnLeftSideofFormula = Convert.ToBoolean(Convert.ToInt32(splitstring[4]));
                for (int i = 0; i < _chemistryrule.NumberofConstituentsinCompound; i++)
                {
                    _constituentofcompoundrule = new ConstituentofCompoundRule
                    {
                        ConstituentType = Convert.ToInt32(splitstring[i * 3 + 5]),
                        Amount = Convert.ToInt32(splitstring[i * 3 + 7].Trim(new Char[] { '}' }))
                    };
                    if (_constituentofcompoundrule.ConstituentType == 1)
                    {
                        _constituentofcompoundrule.ElementGroup = elementgroups[Convert.ToInt32(splitstring[i * 3 + 6]) - 1];
                    }
                    //else if (_constituentofcompoundrule.ConstituentType == 2)
                    //{
                    //    _constituentofcompoundrule.FunctionalGroup = functionalgroups[Convert.ToInt32(splitstring[i * 3 + 6]) - 1];
                    //}
                    else if (_constituentofcompoundrule.ConstituentType == 3)
                    {
                        _constituentofcompoundrule.Element = elements[Convert.ToInt32(splitstring[i * 3 + 6]) - 1];
                    }
                    _chemistryrule.ConstituentsofCompoundRule.Add(_constituentofcompoundrule);
                }
                Rules.Add(_chemistryrule);
            }
            return Rules;
        }
    }
}

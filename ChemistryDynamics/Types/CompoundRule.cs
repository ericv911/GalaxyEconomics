using System.Collections.ObjectModel;

namespace ChemistryDynamics.Types
{ 
    public class CompoundRule : BaseType, ICompoundRule
    {
        public bool CanElectroNegativeElementsbeOnLeftSideofFormula { get; set; }
        public bool CanConstituentsbeEqual { get; set; }
        public int Charge { get; set; }
        public bool IsFunctionalGroup { get; set; }
        public string Name { get; set; }
        public int NumberofConstituentsinCompound { get; set; }
        //gives an indication how many items should be in the collection, it's written in the file so that the filereader knows how much information
        // is present in the line

        public ObservableCollection<IConstituentofCompoundRule> ConstituentsofCompoundRule { get; set; }

        public CompoundRule()
        {
            ConstituentsofCompoundRule = new ObservableCollection<IConstituentofCompoundRule>();
        }
        public CompoundRule(int numberofelementsincompound, ObservableCollection<IConstituentofCompoundRule> elementgroupsincompound, int charge)
        {
            Charge = charge;
            ConstituentsofCompoundRule = new ObservableCollection<IConstituentofCompoundRule>();
            NumberofConstituentsinCompound = numberofelementsincompound;
            ConstituentsofCompoundRule = elementgroupsincompound;
        }
    }
}

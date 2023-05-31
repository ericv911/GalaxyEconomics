using System.Collections.ObjectModel;

namespace ChemistryDynamics.Types
{
    public interface ICompoundRule
    {
        bool CanElectroNegativeElementsbeOnLeftSideofFormula { get; }
        bool CanConstituentsbeEqual { get; }
        int Charge { get; }
        bool IsFunctionalGroup { get; }
        string Name { get; }
        int NumberofConstituentsinCompound { get; }
        ObservableCollection<IConstituentofCompoundRule> ConstituentsofCompoundRule { get; }
    }
}

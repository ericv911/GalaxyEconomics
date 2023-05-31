using Common.Physics;
using System.Collections.ObjectModel;

namespace CompoundProvider.Types
{
    public interface ICompound
    {
        string Name { get; }
        bool IsFunctionalGroup { get; }
        int OxidationState { get; }
        double AtomicMass { get; }
        ObservableCollection<ElementinCompound> ElementsinCompound { get; }
        ObservableCollection<FunctionalGroupinCompound> FunctionalGroupsinCompound { get; }
    }
}

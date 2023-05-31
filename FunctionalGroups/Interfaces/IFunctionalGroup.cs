
using Common.Physics;
using System.Collections.ObjectModel;

namespace FunctionalGroups.Types
{
    public interface IFunctionalGroup
    {
        bool IsDiscovered { get; }
        int Charge { get; }
        string Name { get; }
        double AtomicMass { get; }
        ObservableCollection<ElementinCompound> ElementsinFunctionalGroup { get; }
    }
}

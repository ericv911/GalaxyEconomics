using Common.Physics;
using System.Collections.ObjectModel;

namespace CompoundProvider.Types
{
    public class Compound : BaseType, ICompound
    {
        public string Name { get; set; }
        public bool IsFunctionalGroup { get; set; }
        public int OxidationState { get; set; }
        public double AtomicMass { get; set; }
        public ObservableCollection<ElementinCompound> ElementsinCompound { get; set; }
        public ObservableCollection<FunctionalGroupinCompound> FunctionalGroupsinCompound { get; set; }
        public Compound()
        {
            IsFunctionalGroup = false;
            FunctionalGroupsinCompound = new ObservableCollection<FunctionalGroupinCompound>();
            ElementsinCompound = new ObservableCollection<ElementinCompound>();
        }
        public Compound(Compound compound)
        {
            IsFunctionalGroup = compound.IsFunctionalGroup;
            OxidationState = compound.OxidationState;
            AtomicMass = compound.AtomicMass;
            ElementsinCompound = compound.ElementsinCompound;
            FunctionalGroupsinCompound = compound.FunctionalGroupsinCompound;
        }
    }
}

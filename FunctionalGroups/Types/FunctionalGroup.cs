using Common.Physics;
using System.Collections.ObjectModel;


namespace FunctionalGroups.Types
{
    /// <summary>
    /// Functional group used to make compounds. 
    /// </summary>
    public class FunctionalGroup : BaseType, IFunctionalGroup
    {
        /// <summary>
        /// Is the functional group discovered yet in the game. Every game it needs to be rediscovered via the normal Rules for compounds. 
        /// If it is discovered on a stellar object, the Functional group is free to be used as a constituent in more elaborate compounds
        /// </summary>
        public bool IsDiscovered { get; set; }
        /// <summary>
        /// Charge of the Ion, which is the same as the Oxidation Number
        /// </summary>
        public int Charge { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Atomic mass of the Functional Group
        /// </summary>
        public double AtomicMass { get; set; }
        /// <summary>
        /// Collection of the different elements that are present in the compound
        /// </summary>
        public ObservableCollection<ElementinCompound> ElementsinFunctionalGroup { get; set; }
        public FunctionalGroup()
        {
            ElementsinFunctionalGroup = new ObservableCollection<ElementinCompound>();
            IsDiscovered = false;
        }
        public FunctionalGroup(int charge, double atomicmass, ObservableCollection<ElementinCompound> elements)
        {
            Charge = charge;
            AtomicMass = atomicmass;
            ElementsinFunctionalGroup = elements;
        }
    }
}


using FunctionalGroups.Types;

namespace CompoundProvider.Types
{
    public class FunctionalGroupinCompound : BaseType, IFunctionalGroupinCompound
    {
        //add prices and other Trade-related information
        #region properties
        public int AmountinCompound { get; set; }
        public FunctionalGroup FunctionalGroup { get; set; }
        #endregion
        public FunctionalGroupinCompound()
        {
        }
        public FunctionalGroupinCompound(FunctionalGroupinCompound functionalgroup)
        {
            AmountinCompound = functionalgroup.AmountinCompound;
            FunctionalGroup = functionalgroup.FunctionalGroup;
        }
        public FunctionalGroupinCompound(FunctionalGroup functionalgroup, int amount)
        {
            FunctionalGroup = functionalgroup;
            AmountinCompound = amount;
        }
    }
}

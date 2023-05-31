using Common.Physics;

namespace ChemistryDynamics.Types
{
    /// <summary>
    /// A choice needs to be made between 2 choices.  
    /// 1. Use the generic ConstituentType and IndexofConstituentTypeCollection to indicate what the ChemistryRule-Constituent is 
    /// or
    /// 2. Use either of the 3 possible classes themselves "Element", "ElementGroup" or "FunctionalGroup" per constituent.
    /// The biggest drawback of 2. is that each constituent will have all 3 Classes of which 2 are null.
    /// </summary>
    public class ConstituentofCompoundRule : BaseType, IConstituentofCompoundRule
    {
        #region properties
        /// <summary>
        /// Given the Constituent type, what is the index in the collection of the specific constituent type ::::
        /// Example 1 : if value = 46 and Constituent type = 3, then this value indicates the 46th Element in the Elements Collection which is palladium ::::
        /// Example 2 : if value = 16 and Constituent type = 1, then this value indicates the 3th Element in the ElementGroups Collection, which are the Chalcogens ::::
        /// Example 3: if value = 2 and Constituent type = 2, then this value indicates the 2nd Element in the FunctionalGroup Collection, which is Carbonyl ::::
        /// </summary>
        /// <summary>
        /// Integer that specifies what the constituent of the compound is ->  an Elementgroup (1), a Functional Group (2) or an Element an sich (3)
        /// </summary>
        public int ConstituentType { get; set; } // 1 = Elementgroup, 2 = Functional Group, 3 = Element, 
        public int Amount { get; set; }
        public ElementGroup ElementGroup { get; set; }
        public Element Element { get; set; }
        //public FunctionalGroup FunctionalGroup { get; set; }
        #endregion
        #region constructors
        public ConstituentofCompoundRule()
        {

        }
        public ConstituentofCompoundRule(int constituenttype)
        {
            ConstituentType = constituenttype;
        }
        //public ConstituentofCompoundRule(int amount, FunctionalGroup functionalgroup, int constituenttype)
        //{
        //    ConstituentType = constituenttype;
        //    Amount = amount;
        //    FunctionalGroup = functionalgroup;
        //}
        public ConstituentofCompoundRule(int amount, Element element, int constituenttype)
        {
            ConstituentType = constituenttype;
            Amount = amount;
            Element = element;
        }
        public ConstituentofCompoundRule(int amount, ElementGroup elementgroup, int constituenttype)
        {
            ConstituentType = constituenttype;
            Amount = amount;
            ElementGroup = elementgroup;
        }
        #endregion
    }
}

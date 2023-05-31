using Common.Physics;

namespace ChemistryDynamics.Types
{
    public interface IConstituentofCompoundRule
    {
        int ConstituentType { get; }
        int Amount { get; }
        ElementGroup ElementGroup { get; }
        Element Element { get; }
       // FunctionalGroup FunctionalGroup { get; }
    }
}

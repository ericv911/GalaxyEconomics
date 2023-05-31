
namespace Taxonomy.Types
{
    public class Species : BaseType, ISpecies
    {
        #region constructor
        public Species()
        {
        }
        #endregion

        #region properties
        public Genus Genus { get; set; }
        public double ReproductionRate { get; set; }
        #endregion
    }
}

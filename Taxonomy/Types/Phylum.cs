
namespace Taxonomy.Types
{
    public class Phylum : BaseType, IPhylum
    {
        #region constructor
        public Phylum()
        {
        }
        #endregion

        #region properties
        public Kingdom Kingdom { get; set; }
        #endregion
    }
}

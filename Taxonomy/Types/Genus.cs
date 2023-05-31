
namespace Taxonomy.Types
{
    public class Genus : BaseType, IGenus
    {
        #region constructor
        public Genus()
        {
        }
        #endregion

        #region properties
        public Family Family { get; set; }
        #endregion
    }
}

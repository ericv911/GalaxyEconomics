
namespace Taxonomy.Types
{
    public class SubPhylum : BaseType, ISubPhylum
    {
        #region constructor
        public SubPhylum()
        {
        }
        #endregion

        #region properties
        public Phylum Phylum { get; set; }
        #endregion
    }
}

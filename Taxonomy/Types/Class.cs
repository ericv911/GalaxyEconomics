
namespace Taxonomy.Types
{
    public class Class : BaseType, IClass
    {
        #region constructor
        public Class()
        {
        }
        #endregion

        #region properties
        public SubPhylum SubPhylum { get; set; }
        #endregion
    }
}


namespace Taxonomy.Types
{
    public class Family : BaseType, IFamily
    {
        #region constructor
        public Family()
        {
        }
        #endregion

        #region properties
        public Order Order { get; set; }
        #endregion
    }
}

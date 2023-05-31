
namespace Taxonomy.Types
{
    public class Kingdom : BaseType, IKingdom
    {
        #region constructor
        public Kingdom()
        {
        }
        #endregion

        #region properties
        public IDomain Domain { get; set; }
        #endregion
    }
}

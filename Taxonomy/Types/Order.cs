
namespace Taxonomy.Types
{
    public class Order : BaseType, IOrder
    {
        #region constructor
        public Order()
        {
        }
        #endregion

        #region properties
        public Class Class { get; set; }
        #endregion
    }
}

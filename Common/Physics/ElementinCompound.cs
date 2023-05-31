

namespace Common.Physics
{
    public class ElementinCompound 
    {
        //add prices and other Trade-related information
        #region properties

        public int OxidationState { get; set; }
        public int AmountinCompound { get; set; }
        public Element Element { get; set; }
        #endregion

        #region constructors
        public ElementinCompound()
        {
        }
        public ElementinCompound(ElementinCompound element)
        {
            OxidationState = element.OxidationState;
            AmountinCompound = element.AmountinCompound;
            Element = element.Element;
        }
        public ElementinCompound(Element element, int amount, int oxidationstate)
        {
            OxidationState = oxidationstate;
            Element = element;
            AmountinCompound = amount;
        }
        #endregion
    }
}

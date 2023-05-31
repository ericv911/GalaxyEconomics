using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Physics;

namespace Common.Logistics
{
    public interface IElementinStorage
    {
        bool HasLocalElementgroup { get; }
        double AmountinStorage { get; }
        Element Element { get; }
    }
    public class ElementinStorage : IElementinStorage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //add prices and other Trade-related information
        #region properties
        public bool HasLocalElementgroup { get; set; }
        public double AmountinStorage { get; set; }
        public Element Element { get; set; }
        #endregion
        #region constructors
        public ElementinStorage()
        {
        }
        public ElementinStorage(ElementinStorage element)
        {
            AmountinStorage = element.AmountinStorage;
            Element = element.Element;
        }
        public ElementinStorage(Element element, double amount)
        {
            Element = element;
            AmountinStorage = amount;
        }
        #endregion
    }
}

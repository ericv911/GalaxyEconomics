using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface IOrder { }
    public class Order : IOrder
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region fields
        protected Class _class;
        protected string _name;
        #endregion

        #region constructor
        public Order()
        {
        }
        #endregion

        #region properties
        public Class Class { get; set; }
        public string Name { get; set; }
        #endregion
    }
}

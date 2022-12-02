using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface IFamily { }
    public class Family : IFamily
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region fields
        protected Order _order;
        protected string _name;
        #endregion

        #region constructor
        public Family()
        {
        }
        #endregion

        #region properties
        public Order Order { get; set; }
        public string Name { get; set; }
        #endregion
    }
}

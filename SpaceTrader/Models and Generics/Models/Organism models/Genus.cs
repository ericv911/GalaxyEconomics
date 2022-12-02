using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface IGenus { }
    public class Genus : IGenus
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        protected Family _family;
        protected string _name;
        #endregion

        #region constructor
        public Genus()
        {
        }
        #endregion

        #region properties
        public Family Family { get; set; }
        public string Name { get; set; }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface IPhylum { }
    public class Phylum : IPhylum
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        protected Kingdom _kingdom;
        protected string _name;
        #endregion

        #region constructor
        public Phylum()
        {
        }
        #endregion

        #region properties
        public Kingdom Kingdom { get; set; }
        public string Name { get; set; }
        #endregion
    }
}

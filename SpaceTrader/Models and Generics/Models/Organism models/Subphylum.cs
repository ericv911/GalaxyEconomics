using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface ISubPhylum { }
    public class SubPhylum : ISubPhylum
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        protected Phylum _phylum;
        protected string _name;
        #endregion

        #region constructor
        public SubPhylum()
        {
        }
        #endregion

        #region properties
        public Phylum Phylum { get; set; }
        public string Name { get; set; }
        #endregion
    }
}
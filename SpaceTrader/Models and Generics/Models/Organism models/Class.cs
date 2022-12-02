using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface IClass { }
    public class Class : IClass
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        protected SubPhylum _subphylum;
        protected string _name;
        #endregion

        #region constructor
        public Class()
        {
        }
        #endregion

        #region properties
        public SubPhylum SubPhylum { get; set; }
        public string Name { get; set; }
        #endregion
    }
}

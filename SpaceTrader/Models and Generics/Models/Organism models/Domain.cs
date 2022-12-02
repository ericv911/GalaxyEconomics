using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface IDomain { }
    public class Domain : IDomain
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region fields

        protected string _name;
        #endregion
        #region constructor
        public Domain()
        {
        }
        #endregion
        #region properties

        public string Name { get; set; }
        #endregion
    }
}
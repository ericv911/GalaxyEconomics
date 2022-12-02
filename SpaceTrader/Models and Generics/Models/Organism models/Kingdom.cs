using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface IKingdom { }
    public class Kingdom : IKingdom
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        protected string _name;
        protected Domain _domain;
        #endregion
        #region constructor
        public Kingdom()
        {

        }
        #endregion
        #region properties
        public string Name { get; set; }
        public Domain Domain { get; set; }

        #endregion
    }
}

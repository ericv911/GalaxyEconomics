using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader.Taxonomy
{
    public interface ISpecies
    {
        string Name { get; }
        double ReproductionRate { get; } 
    }
    public class Species : ISpecies, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region fields
        protected Genus _genus;
        protected string _name;
        protected double _reproductionrate;

        #endregion

        #region constructor
        public Species( )
        {
        }
        #endregion

        #region properties
        public Genus  Genus { get; set; }
        public string Name { get; set; }
        public double ReproductionRate { get; set; }

        #endregion





    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader
{
    public interface ISpeciesperOrbitalBody
    {

    }

    public class SpeciesperOrbitalBody : ISpeciesperOrbitalBody, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected double _populationsize;
        protected Taxonomy.Species _species;
        protected double _reproductionrate;
        public SpeciesperOrbitalBody(Taxonomy.Species species, double populationsize, double reproductionrate)
        {
            _species = species;
            _populationsize = populationsize;
            _reproductionrate = reproductionrate;
        }
        public double ReproductionRate
        {
            get {  return _reproductionrate;        }
            set { _reproductionrate = value; }
        }
        public double PopulationSize
        { 
            get { return _populationsize; }
            set { _populationsize = value; }
        }
        public Taxonomy.Species Species
        {
            get { return _species; }
            set { _species = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Taxonomy.Types;

namespace SpaceTrader
{
    public interface ISpeciesperNode
    {
        double PopulationSize { get; set; }
        double ReproductionRate { get; set; }
        Species Species { get; set; }
    }
    /// <summary>
    /// Template for Species per populationcentre
    /// </summary>
    public class SpeciesperNode : ISpeciesperNode, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SpeciesperNode(Species species, double populationsize, double reproductionrate)
        {
            Species = species;
            PopulationSize = populationsize;
            ReproductionRate = reproductionrate;
        }
        public double ReproductionRate { get; set; }
        public double PopulationSize { get; set; }
        public Species Species { get; set; }
    }
}

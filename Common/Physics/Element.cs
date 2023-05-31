using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Physics
{
    public interface IElement
    {
        string Symbol { get; }
        string Name { get; }
        double UniversalAbundance { get; }
        double AtomicMass { get; }
        double ElectroNegativity { get; }
        Block Block { get; set; }
    }
    public class Element : INotifyPropertyChanged, IElement
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Element()
        {
            Symbol = "";
            Block = new Block();
            OxidationStates = new ObservableCollection<int>();
            CommonOxidationStates = new ObservableCollection<int>();
        }

        public Block Block { get; set; }
        public double ElectroNegativity { get; set; }
        public double AtomicMass { get; set; }
        public ObservableCollection<int> CommonOxidationStates;
        public ObservableCollection<int> OxidationStates;
        public double UniversalAbundance { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int StateofMatter { get; set; } // 0 = gas, 1 = liquid, 2 = solid   not used
        public bool IsRadioActive { get; set; }
        public bool IsEdible { get; set; }
        public bool IsPrecious { get; set; }
        public bool Isillegal { get; set; }
    }
}

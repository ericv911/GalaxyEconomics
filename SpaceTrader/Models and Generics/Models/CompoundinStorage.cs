using Common.Physics;
using CompoundProvider.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader
{
    public interface ICompoundinStorage
    {
        double AmountinStorage { get; }
        Compound Compound { get; }
    }
    public class CompoundinStorage : ICompoundinStorage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public double AmountinStorage { get; set; }
        public Compound Compound { get; set; }

        public CompoundinStorage()
        {
        }
        public CompoundinStorage(CompoundinStorage compoundinstorage)
        {
            AmountinStorage = compoundinstorage.AmountinStorage;
            Compound = compoundinstorage.Compound;
        }
        public CompoundinStorage(Compound compound, double amount)
        {
            Compound = compound;
            AmountinStorage = amount;
        }
    }
}

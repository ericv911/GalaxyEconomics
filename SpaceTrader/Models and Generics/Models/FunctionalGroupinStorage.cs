using Common.Physics;
using CompoundProvider.Types;
using FunctionalGroups.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader
{
    public interface IFunctionalGroupinStorage
    {
        double AmountinStorage { get; }
        FunctionalGroup FunctionalGroup { get; }
    }
    public class FunctionalGroupinStorage : IFunctionalGroupinStorage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// if false, FunctionalGroup may not be produced . Not applicable for loading on  ships
        /// </summary>
        public double AmountinStorage { get; set; }
        public FunctionalGroup FunctionalGroup { get; set; }
        #region constructors
        public FunctionalGroupinStorage()
        {
        }
        public FunctionalGroupinStorage(FunctionalGroupinStorage functionalgroup)
        {
            AmountinStorage = functionalgroup.AmountinStorage;
            FunctionalGroup = functionalgroup.FunctionalGroup;
        }
        public FunctionalGroupinStorage(FunctionalGroup functionalgroup, double amount)
        {
            FunctionalGroup = functionalgroup;
            AmountinStorage = amount;
        }
        #endregion
    }
}

using CompoundProvider.Types;
using System.Collections.ObjectModel;

namespace CompoundProvider
{
    public interface ICompoundCollection
    { 
        ObservableCollection<Compound> Compounds { get; }
    }
}

using FunctionalGroups.Types;
using System.Collections.ObjectModel;

namespace FunctionalGroups
{
    public interface IFunctionalGroupCollection
    {
        ObservableCollection<FunctionalGroup> FunctionalGroups { get; }
    }
}

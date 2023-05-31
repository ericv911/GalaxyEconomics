using ChemistryDynamics.Types;
using System.Collections.ObjectModel;

namespace ChemistryDynamics
{
    public interface IRulesCollection
    {
        ObservableCollection<ICompoundRule> Rules { get; }
    }
}

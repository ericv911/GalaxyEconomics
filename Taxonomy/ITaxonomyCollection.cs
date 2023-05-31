using System.Collections.ObjectModel;
using Taxonomy.Types;

namespace Taxonomy
{
    public interface ITaxonomyCollection
    {
        ObservableCollection<Domain> Domains { get; }
        ObservableCollection<Kingdom> Kingdoms { get; }
        ObservableCollection<Phylum> Phyla { get; }
        ObservableCollection<SubPhylum> SubPhyla { get; }
        ObservableCollection<Class> Classes { get; }
        ObservableCollection<Order> Orders { get; }
        ObservableCollection<Family> Families { get; }
        ObservableCollection<Genus> Geni { get; }
        ObservableCollection<Species> Species { get; }
    }
}

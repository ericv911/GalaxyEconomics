
namespace Taxonomy.Types
{
    public interface ISpecies
    {
        Genus Genus { get; }
        string Name { get; }
        double ReproductionRate { get; }
    }
}

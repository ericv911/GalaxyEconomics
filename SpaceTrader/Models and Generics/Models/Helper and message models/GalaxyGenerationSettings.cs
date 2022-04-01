
using System.Windows;

namespace SpaceTrader
{

    public interface IGalaxyGenerationSettings
    {
        int StartNumberofStellarObjects { get; set; }
        int SpiralWindedness { get; set; }
        int MinDistancefromCentre { get; set; }
        int StartNumberofShips { get; set; }
        int MaxBulgeRadius { get; set; }
        bool InitSpiralArms { get; set; }
        bool InitBulge { get; set; }
        bool InitBar { get; set; }
        bool InitDisc { get; set; }
        bool DrawStarsinCentre { get; set; }
    }
    public class GalaxyGenerationSettings : IGalaxyGenerationSettings
    {
        public int StartNumberofStellarObjects { get; set; }
        public int SpiralWindedness { get; set; }
        public int MinDistancefromCentre { get; set; }
        public int StartNumberofShips { get; set; }
        public int MaxBulgeRadius { get; set; }
        public bool InitSpiralArms { get; set; }
        public bool InitBulge { get; set; }
        public bool InitBar { get; set; }
        public bool InitDisc { get; set; }
        public bool DrawStarsinCentre { get; set; }
    }
}

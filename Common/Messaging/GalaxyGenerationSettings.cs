using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messaging
{
    public interface IGalaxyGenerationSettings
    {
        int StartNumberofStellarObjects { get; set; }
        int SpiralWindedness { get; set; }
        int MinimumDistancefromCentre { get; set; }
        int StartNumberofShips { get; set; }
        int MaximumRadiusofBulge { get; set; }
        bool InitializeStellarObjectsinSpiralArms { get; set; }
        bool InitializeStellarObjectsinBulge { get; set; }
        bool InitializeStellarObjectsinBar { get; set; }
        bool InitializeStellarObjectsintDisc { get; set; }
        bool DrawStarsinCentre { get; set; }
    }
    public class GalaxyGenerationSettings : IGalaxyGenerationSettings
    {
        public int StartNumberofStellarObjects { get; set; }
        public int SpiralWindedness { get; set; }
        public int MinimumDistancefromCentre { get; set; }
        public int StartNumberofShips { get; set; }
        public int MaximumRadiusofBulge { get; set; }
        public bool InitializeStellarObjectsinSpiralArms { get; set; }
        public bool InitializeStellarObjectsinBulge { get; set; }
        public bool InitializeStellarObjectsinBar { get; set; }
        public bool InitializeStellarObjectsintDisc { get; set; }
        public bool DrawStarsinCentre { get; set; }
    }
}

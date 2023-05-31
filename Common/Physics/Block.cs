using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Physics
{
    public interface IBlock
    {
        string Name { get; }
        int NumberofElectronsperPeriod { get; }
        int StartingPeriod { get; }
        int StartingShell { get; }
    }

    public class Block : IBlock
    {
        public string Name { get; set; }
        public int NumberofElectronsperPeriod { get; set; }
        public int StartingPeriod { get; set; }
        public int StartingShell { get; set; }
    }
}

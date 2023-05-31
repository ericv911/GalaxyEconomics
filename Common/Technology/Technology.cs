using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Common.Technology
{
    public class TechnologyLevel
    {
        public TechnologyLevel()
        {

        }
        public TechnologyLevel(string name, int level, Color clr)
        {
            Name = name;
            Level = level;
            Color = clr;
        }
        public int Level { get; set; }
        public Color Color { get; set; }
        public string Name { get; set; }
    }
}

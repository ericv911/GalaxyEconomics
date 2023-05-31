using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Astronomy
{
    public class CelestialBodyType
    {
        /// <summary>
        /// baseclass for shared properties of Celestial Bodies
        /// </summary>
        public string Name { get; set; }
        public int RelativeOccurence { get; set; }
        public double MinimumDensity { get; set; }
        public double MaximumDensity { get; set; }
        public double Minimum_Mass { get; set; }
        public double Maximum_Mass { get; set; }
        public int Minimum_Radius { get; set; }
        public int Maximum_Radius { get; set; }
    }
}

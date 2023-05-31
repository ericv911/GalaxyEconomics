using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DOperations
{
    /// <summary>
    /// class used in Settings to store the rotation angles in the 3 planes.
    /// </summary>
    public class RotationAngles
    {
        public RotationAngles()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
        public RotationAngles(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}

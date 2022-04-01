using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader
{
    public class Translation
    {
        public Translation(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Translation()
        {
            X = 0;
            Y = 0;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
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
    public class Point3dSettings
    {
        //protected Point3D _rotationangles;

        public Point3dSettings()
        {
            RotationAngles = new RotationAngles();
            Translations = new Translation();
            ScaleFactor = 1;
        }
        public Translation Translations
        {
            get; set;
        }
        public double ScaleFactor
        {
            get; set;
        }
        public RotationAngles RotationAngles
        {
            get; set;

        }
    }
}

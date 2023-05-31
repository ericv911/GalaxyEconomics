using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace _3DOperations
{
    public class Rotations
    {
        public static Vector3D X(Vector3D pnt, double angleradians)
        {
            Vector3D tpnt = new Vector3D
            {
                X = pnt.X,
                Y = Math.Cos(angleradians) * pnt.Y - Math.Sin(angleradians) * pnt.Z,
                Z = Math.Sin(angleradians) * pnt.Y + Math.Cos(angleradians) * pnt.Z
            };
            return tpnt;
        }
        public static Vector3D Y(Vector3D pnt, double angleradians)
        {
            Vector3D tpnt = new Vector3D
            {
                X = Math.Cos(angleradians) * pnt.X + Math.Sin(angleradians) * pnt.Z,
                Y = pnt.Y,
                Z = -1 * Math.Sin(angleradians) * pnt.X + Math.Cos(angleradians) * pnt.Z
            };
            return tpnt;
        }
        public static Vector3D Z(Vector3D pnt, double angleradians)
        {
            Vector3D tpnt = new Vector3D
            {
                X = Math.Cos(angleradians) * pnt.X - Math.Sin(angleradians) * pnt.Y,
                Y = Math.Sin(angleradians) * pnt.X + Math.Cos(angleradians) * pnt.Y,
                Z = pnt.Z
            };
            return tpnt;
        }

        public static Point3D X(Point3D pnt, double angleradians)
        {
            Point3D tpnt = new Point3D
            {
                X = pnt.X,
                Y = Math.Cos(angleradians) * pnt.Y - Math.Sin(angleradians) * pnt.Z,
                Z = Math.Sin(angleradians) * pnt.Y + Math.Cos(angleradians) * pnt.Z
            };
            return tpnt;
        }
        public static Point3D Y(Point3D pnt, double angleradians)
        {
            Point3D tpnt = new Point3D
            {
                X = Math.Cos(angleradians) * pnt.X + Math.Sin(angleradians) * pnt.Z,
                Y = pnt.Y,
                Z = -1 * Math.Sin(angleradians) * pnt.X + Math.Cos(angleradians) * pnt.Z
            };
            return tpnt;
        }
        public static Point3D Z(Point3D pnt, double angleradians)
        {
            Point3D tpnt = new Point3D
            {
                X = Math.Cos(angleradians) * pnt.X - Math.Sin(angleradians) * pnt.Y,
                Y = Math.Sin(angleradians) * pnt.X + Math.Cos(angleradians) * pnt.Y,
                Z = pnt.Z
            };
            return tpnt;
        }
    }
}

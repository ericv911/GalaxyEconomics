using _3DOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public class BitmapDataCalculations
    {
        public static Point3D ScalePoints(Point3D point3d, double scalefactor)
        {
            return new Point3D(point3d.X * scalefactor, point3d.Y * scalefactor, point3d.Z * scalefactor);
        }
        public static Point3D TranslatePoints(Point3D point3d, Translation translation)
        {
            return new Point3D(point3d.X + translation.X, point3d.Y, point3d.Z + translation.Y);
        }
        public static void CalculatePointsafterChange(IEnumerable<StellarObject> stellarobjects, IEnumerable<Ship> ships, IEnumerable<Ship> stellarobjecttradingships, _3DSettings _3dsettings, Vector3D bitmapadjustvector)
        {
            //take beginposition
            //apply rotations -> begin to rotatednew
            //apply scaling -> rotatednew to scaling
            //apply translation -> scaling to final
            foreach (StellarObject stellarobject in stellarobjects)
            {
                stellarobject.RotatedPositionZ = Rotations.Z(stellarobject.BeginPosition, _3dsettings.RotationAngles.X);
                stellarobject.RotatedPositionX = Rotations.X(stellarobject.RotatedPositionZ, _3dsettings.RotationAngles.Z);
                stellarobject.ScaledPosition = ScalePoints(stellarobject.RotatedPositionX, _3dsettings.ScaleFactor);
                stellarobject.TranslatedPosition = TranslatePoints(stellarobject.ScaledPosition, _3dsettings.Translations);
                stellarobject.FinalPosition = stellarobject.TranslatedPosition + bitmapadjustvector;
                stellarobject.FinalPosition2ndBtn = new Point3D(stellarobject.FinalPosition.X, 0, stellarobject.FinalPosition.Z + 10);
            }
            foreach (Ship ship in stellarobjecttradingships)
            {
                ship.RotatedPositionZ = Rotations.Z(ship.MovedPosition, _3dsettings.RotationAngles.X);
                ship.RotatedPositionX = Rotations.X(ship.RotatedPositionZ, _3dsettings.RotationAngles.Z);
                ship.ScaledPosition = ScalePoints(ship.RotatedPositionX, _3dsettings.ScaleFactor);
                ship.TranslatedPosition = TranslatePoints(ship.ScaledPosition, _3dsettings.Translations);
                ship.FinalPosition = ship.TranslatedPosition + bitmapadjustvector;
            }
            foreach (Ship ship in ships)
            {
                ship.RotatedPositionZ = Rotations.Z(ship.MovedPosition, _3dsettings.RotationAngles.X);
                ship.RotatedPositionX = Rotations.X(ship.RotatedPositionZ, _3dsettings.RotationAngles.Z);
                ship.ScaledPosition = ScalePoints(ship.RotatedPositionX, _3dsettings.ScaleFactor);
                ship.TranslatedPosition = TranslatePoints(ship.ScaledPosition, _3dsettings.Translations);
                ship.FinalPosition = ship.TranslatedPosition + bitmapadjustvector;
                 
            }
        }
    }
}

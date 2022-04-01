using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public class CargoShip : Ship, IShip
    {
        public CargoShip()
        {

        }
        public CargoShip(string name, Point3D position, int speed, StellarObject destinationstellarobject, StellarObject currentstellarobject, Color color, EconomicEntity economicentity, bool ownship) : base(name, position, speed, destinationstellarobject, currentstellarobject, color, economicentity, ownship)
        {

        }
    }
}

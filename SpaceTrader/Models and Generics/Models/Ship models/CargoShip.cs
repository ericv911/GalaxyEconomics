using Common.Economy;
using Common.Logistics;
using Common.Transportation;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public class CargoShip : Ship, IShip
    {
        public int MaximumAmountofCargoperCargoHold { get; set; }
        public int CargoHoldsUsed { get; set; }
        public int NumberofCargoHolds { get; set; }
        public FullyObservableCollection<ElementinStorage> ElementssonShip;
        public CargoShipType CargoShipType { get; set; }
        public CargoShip()
        {
        }
        public CargoShip(CargoShipType cargoshiptype, string name, Point3D position, int speed, StellarObject destinationstellarobject, StellarObject currentstellarobject, Color color, EconomicEntity economicentity, bool ownship) : base(name, position, speed, destinationstellarobject, currentstellarobject, color, economicentity, ownship, cargoshiptype.MaxHullIntegrity, cargoshiptype.FuelConsumption, cargoshiptype.FuelCapacity, cargoshiptype.BaseRepairAmountperTurn)
        {
            ElementssonShip = new FullyObservableCollection<ElementinStorage>();
            CargoShipType = cargoshiptype;
            NumberofCargoHolds = cargoshiptype.MaxNumberoFCargoHolds;
            CargoHoldsUsed = 0;
            MaximumAmountofCargoperCargoHold = cargoshiptype.MaxLoadperCargoHold;
        }

        public void LoadElement(ElementinStorage element)
        {
            ElementssonShip.Add(element);
        }

        public void UnloadElement(ElementinStorage element)  //not used at the moment as the ship unloads its  entire cargo in one go, see UnloadAllElements
        {

        }
        public void UnloadAllElements()
        {
            ElementssonShip.Clear();
            CargoHoldsUsed = 0;
            IsUnloaded = true;
        }
    }
}

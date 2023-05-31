using Common.Logistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Transportation
{
    public class CargoShipType
    {
        protected string _name;
        protected int _maxhullintegrity;
        protected int _maxhullintegrityincreaseperlevel;
        protected double _fuelcapacity;
        protected int _basespeed;
        protected int _maxloadpercargohold;


        public CargoShipType()
        {
            Cargoholds = new ObservableCollection<ElementinStorage>();
        }
        public ObservableCollection<ElementinStorage> Cargoholds;
        public string Name { get; set; }
        public int MaxHullIntegrity { get; set; }
        public int MaxHullIntegrityIncreaseperLevel { get; set; }
        public double FuelCapacity { get; set; }
        public double FuelConsumption { get; set; }
        public int MaxNumberoFCargoHolds { get; set; }
        public int BaseSpeed { get; set; }
        public int MaxLoadperCargoHold { get; set; }
        public int BaseRepairAmountperTurn { get; set; }
    }
}

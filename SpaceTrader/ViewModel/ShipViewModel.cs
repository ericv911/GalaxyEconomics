
using Common.Constants;
using Common.Economy;
using Common.Logistics;
using Common.Transportation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    public class ShipViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //readonly FastRandom rand = new FastRandom();

        #region fields

        protected int _startnumberofcargoships;
        protected CargoShip _shipselectedonscreen;
        protected ObservableCollection<CargoShip> _playercargoship;
        #endregion

        #region Class properties

        public ObservableCollection<CargoShip> StellarObjectTradingShips = new ObservableCollection<CargoShip>();
        public ObservableCollection<CargoShip> CargoShips = new ObservableCollection<CargoShip>();
        public ObservableCollection<CargoShip> NPCCargoShips = new ObservableCollection<CargoShip>();
        public int StartNumberofCargoShips
        {
            get
            {
                return _startnumberofcargoships;
            }
            set
            {
                _startnumberofcargoships = value;
                OnPropertyChanged();
            }
        }

        public CargoShip ShipSelectedonScreen
        {
            get { return _shipselectedonscreen; }
            set
            {
                _shipselectedonscreen = value;
                OnPropertyChanged();
            }
        }

        public bool MoveShips { get; set; }
        #endregion

        #region constructor
        public ShipViewModel()
        {
            //_startnumberofcargoships = 50;
            MoveShips = false;
        }
        #endregion

        #region Class Methods
        public void SetShipPathtoStellarObject(int shipindex, IList<StellarObject> path)
        {
            CargoShips[shipindex].PathStellarObjectsQueuefromSourcetoDestination = new Queue<StellarObject>(path.Reverse());
            if (path.Count > 1)
            {
                CargoShips[shipindex].HasMultiNodeDestination = true;
            }
        }

        private void SetPlayerFirstShip(IReadOnlyList<CargoShipType> cargoshiptypes, string name, StellarObject stellarobject, IEconomicEntity economicentity, FastRandom rand)
        {
            CargoShipType cargoshiptype;
            cargoshiptype = cargoshiptypes[(int)rand.NextDouble() * cargoshiptypes.Count];

            Color color = Color.FromRgb(40,40,40);
            CargoShips.Add(new CargoShip(cargoshiptype,name, stellarobject.BeginPosition, 2, stellarobject.StarLanes[rand.Next(0, stellarobject.StarLanes.Count)].To, stellarobject, color, (EconomicEntity)economicentity, true));
        }

        public void InitializeShips(IReadOnlyList<CargoShipType> cargoshiptypes, IReadOnlyList<StellarObject> stellarobjects, IReadOnlyList<IEconomicEntity> economicentities, FastRandom rand)
        {
            CargoShipType cargoshiptype;
            StellarObjectTradingShips.Clear();
            foreach(StellarObject stellarobject in stellarobjects)
            {
                cargoshiptype = cargoshiptypes[(int)(rand.NextDouble() * (double)(cargoshiptypes.Count))];
                StellarObjectTradingShips.Add(new CargoShip(cargoshiptype, $"Trading-AI-{stellarobject.Name}", stellarobject.BeginPosition, cargoshiptype.BaseSpeed, stellarobject.StarLanes[rand.Next(0, stellarobject.StarLanes.Count)].To, stellarobject, Color.FromRgb((byte)(rand.NextDouble()*100), (byte)(rand.NextDouble() * 100), (byte)(rand.NextDouble() * 255)), (EconomicEntity)economicentities[rand.Next(0, economicentities.Count())], false));
            }
            CargoShips.Clear();
            //CargoShips intended for long-range independent hauling across the galaxy
            //int tindex;
            //int cntr = 0;
            //Point3D tpnt;
            //Color color;
            //int x, y, z;

            //for (int i = 0; i < StartNumberofCargoShips; i++)
            //{
            //    x = rand.Next(0, 100);
            //    y = rand.Next(0, 100);
            //    z = rand.Next(150, 256);
            //    color = Color.FromRgb((byte)x, (byte)y, (byte)z);
            //    tindex = rand.Next(0, stellarobjects.Count());
            //    tpnt = stellarobjects[tindex].BeginPosition;
            //    cntr += 1;
            //    CargoShips.Add(new CargoShip($"Cargo-AI-{cntr}", tpnt, 1, stellarobjects[tindex].StarLanes[rand.Next(0, stellarobjects[tindex].StarLanes.Count)].To, stellarobjects[tindex],color, (EconomicEntity)economicentities[rand.Next(0,economicentities.Count())], false));
            //}
            SetPlayerFirstShip(cargoshiptypes, "Marlinde", stellarobjects[0], economicentities[0], rand);
        }
        //use following code for 

        public double GetDistanceBetweenShipAndStar(Point3D shipposition, Point3D starposition)
        {
            int deltax = (int)(shipposition.X - starposition.X);
            int deltay = (int)(shipposition.Y - starposition.Y);
            int deltaz = (int)(shipposition.Z - starposition.Z);
            return Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
        }
        public void SetActiveShip() //set focus on own ship
        {
            ShipSelectedonScreen = CargoShips[CargoShips.Count - 1];
        }
        public void SetActiveShip(Point mousepostion)
        {
            int distance;
            CargoShip smallestship = new CargoShip();
            int smallestdistance = 1000000;
            foreach (CargoShip ship in StellarObjectTradingShips)
            {
                distance = (int)(Math.Pow((int)ship.FinalPosition.X - (int)mousepostion.X, 2) + Math.Pow((int)ship.FinalPosition.Z - (int)mousepostion.Y, 2));
                if (distance < smallestdistance)
                {
                    smallestdistance = distance;
                    smallestship = ship;
                }
            }
            ShipSelectedonScreen = smallestship;
        }
 
        //public int SetShipDestinationAI(ObservableCollection<StellarObject> stars, int currentindex, int destinationindex)
        //{
        //    //int a = rand.Next(0, stars[destinationindex].StarTravelLanes.Count());
        //    //while (stars[destinationindex].Starlanes[a] == currentindex)
        //    //{
        //    //    a = rand.Next(0, stars[destinationindex].Starlanes.Count);
        //    //}
        //    //return a;
        //    return 0;
        //}
        #endregion
        public void LoadCargo(FastRandom rand, CargoShip ship, IStellarObject stellarobject)
        {
            ship.ElementssonShip.Clear();
            foreach(ElementinStorage elementoncentralhub in stellarobject.CentralHub.ElementsinStorage)
            {
                if (ship.CargoHoldsUsed > ship.NumberofCargoHolds)
                {
                    break;
                }
                if (elementoncentralhub.AmountinStorage > 0)
                {
                    var elementspresent = from _elementspresent in stellarobject.CentralHub.ElementsinStorage where _elementspresent.AmountinStorage > 0
                                           select _elementspresent;
                    if (rand.NextDouble() * elementspresent.Count() < ship.NumberofCargoHolds)
                    {
                        ship.IsUnloaded = false;
                        ship.CargoHoldsUsed += 1;
                        if (ship.MaximumAmountofCargoperCargoHold > elementoncentralhub.AmountinStorage)
                        {
                            ship.LoadElement(new ElementinStorage(elementoncentralhub));
                            stellarobject.CentralHub.RemoveElementfromStorage(elementoncentralhub.Element); //empty element on central hub
                        }
                        else
                        {
                            ship.LoadElement(new ElementinStorage(elementoncentralhub.Element, ship.MaximumAmountofCargoperCargoHold));
                            stellarobject.CentralHub.RemoveElementfromStorage(elementoncentralhub.Element, ship.MaximumAmountofCargoperCargoHold); // reduce amount on central hub by cargohold size
                        }
                    }
                }
            }
        }
        public void UnloadCargo(CargoShip ship, IStellarObject stellarobject)
        {
            foreach (ElementinStorage elementonship in ship.ElementssonShip)
            {
                        stellarobject.CentralHub.AddElementtoStorage(elementonship);
            }
            ship.UnloadAllElements();
        }

        private void SetNewDestinationStellarObject(Ship ship)
        {
            double  timeinterval;
            double deltax, deltay, deltaz;
            deltax = (ship.DestinationStellarObject.BeginPosition.X - ship.CurrentStellarObject.BeginPosition.X);
            deltay = (ship.DestinationStellarObject.BeginPosition.Y - ship.CurrentStellarObject.BeginPosition.Y);
            deltaz = (ship.DestinationStellarObject.BeginPosition.Z - ship.CurrentStellarObject.BeginPosition.Z);
            timeinterval = GetDistanceBetweenShipAndStar(ship.BeginPosition, ship.DestinationStellarObject.BeginPosition) / ship.Speed;
            // set ship data for travel. how much to move each turn and how many turns
            ship.MoveCounter = (int)timeinterval;  //work with movecounter to stop ships from missing destination and continuing across map. eventually crashing the game
            ship.MoveVector = new Vector3D(deltax / timeinterval, deltay / timeinterval, deltaz / timeinterval);
            ship.HasDestination = true;
            ship.MovedPosition = ship.BeginPosition;
            ship.BeginPosition = ship.DestinationStellarObject.BeginPosition; ;
        }
        public void PerformActions(FastRandom rand, IShipConstants shipconstants)  //global Ship Control Function From here all Ship actions are coordinated and performed
        {
            ////double to stop accidental deviating from course
            foreach (CargoShip ship in StellarObjectTradingShips)
            {
                if (!ship.IsDocked)  // this if block deals with ships outside in space or just entering a stellar system
                {
                    //first make sure each ship without a multinode destination, has a destination;
                    if (!ship.HasMultiNodeDestination)
                    {
                        if (ship.HasDestination == false )
                        {
                            SetNewDestinationStellarObject(ship);
                        }
                    }
                    //decrease hull integrity and fuel amount. 
                    // set flags if either drops below a certain value
                    ship.HullIntegrity -=  rand.NextDouble()*3;
                    ship.FuelAmount -= ship.FuelConsumption;
                    if (ship.FuelAmount < shipconstants.BaseMinimalFuelAmountforNeedsRefuelingFlagSet)
                    {
                        ship.NeedsRefueling = true;
                    }
                    if (ship.HullIntegrity < shipconstants.BaseMinimumHullIntegrityAmountforNeedsRepairingFlagSet)
                    {
                        ship.NeedsRepairing = true;
                    }
                    //move each ship towards its destination, just 2 lines.
                    ship.MoveCounter -= 1;
                    ship.MovedPosition = Point3D.Add(ship.MovedPosition, ship.MoveVector);
                    // if shp arrived at destination, set a new destination, both single and multiple node destinations.
                    if (ship.MoveCounter < 1)  //ship has arrived
                    {
                        //perform docking actions

                        ship.DockingShipDynamics(rand, ship.CargoShipType.MaxHullIntegrity, shipconstants);

                        //reset poition, currentindex and destinationindex
                        ship.CurrentStellarObject = ship.DestinationStellarObject;
                        ship.BeginPosition = ship.DestinationStellarObject.BeginPosition;
                        if (ship.HasMultiNodeDestination == true)
                        {
                            ship.DestinationStellarObject = ship.PathStellarObjectsQueuefromSourcetoDestination.Dequeue();
                            if (ship.PathStellarObjectsQueuefromSourcetoDestination.Count() == 0)
                            {
                                ship.HasMultiNodeDestination = false;
                            }
                        }
                        else
                        {
                            if (ship.DestinationStellarObject == ship.HomeStellarObject)
                            {
                                var _collectionstarlanestopossibledestinations = from __starlanes in ship.DestinationStellarObject.StarLanes
                                                                                 where __starlanes.Distance < ship.FuelAmount* ship.FuelConsumption
                                                                                 select __starlanes;
                                List<Starlane> _starlanes = _collectionstarlanestopossibledestinations.ToList();
                                if (_starlanes.Count > 0)
                                {
                                    ship.DestinationStellarObject = _starlanes[rand.Next(0, _starlanes.Count())].To;
                                }
                            }
                            else
                            {
                                ship.DestinationStellarObject = ship.HomeStellarObject;
                            }
                        }
                        //set new destination
                        SetNewDestinationStellarObject(ship);
                    }
                }
                else  //if ship is docked
                {
                    if(!ship.IsUnloaded)
                    {
                        UnloadCargo(ship, ship.CurrentStellarObject);
                    }
                    if(ship.IsOverhauling) // if ship is overhauling, do stuff, automaticall repair and refuel and goto loading unloading
                    {
                        ship.DockingDuration -= 1;
                        if (ship.DockingDuration == 0)
                        {
                            ship.ShipUpgradeLevel += 1;
                            ship.HullIntegrity = ship.CargoShipType.MaxHullIntegrity + (ship.ShipUpgradeLevel * ship.CargoShipType.MaxHullIntegrityIncreaseperLevel);
                            ship.FuelAmount = ship.CargoShipType.FuelCapacity;
                            ship.IsOverhauling = false;
                            ship.DockingDuration = shipconstants.BaseDockingDurationforLoadingUnloading;
                        }
                    }
                    else if (ship.IsRepairing) //if ship is repairing, repair ship and refuel it, then goto loading unloading
                    {
                        ship.HullIntegrity += ship.BaseRepairAmountperTurn;
                        ship.DockingDuration -= 1;
                        if (ship.DockingDuration == 0)
                        {
                            {
                                ship.FuelAmount = ship.CargoShipType.FuelCapacity;
                                ship.HullIntegrity = ship.CargoShipType.MaxHullIntegrity + (ship.ShipUpgradeLevel * ship.CargoShipType.MaxHullIntegrityIncreaseperLevel);
                                ship.IsRepairing = false;
                                ship.DockingDuration = shipconstants.BaseDockingDurationforLoadingUnloading;
                            }
                        }
                    }
                    else if (ship.IsRefueling) // if ship is refueling, refuel, then go to loading/unloading
                    {
                        ship.DockingDuration -= 1;
                        if (ship.DockingDuration == 0)
                        {
                            ship.FuelAmount = ship.CargoShipType.FuelCapacity;
                            ship.IsRefueling = false;
                            ship.DockingDuration = shipconstants.BaseDockingDurationforLoadingUnloading;
                        }
                    }

                    else //ship is loading/unloading
                    {
                        ship.DockingDuration -= 1;
                        if (ship.DockingDuration == 0)
                        {
                            LoadCargo(rand,ship,ship.CurrentStellarObject);
                            ship.IsDocked = false;
                        }
                    }
                }
            }
            foreach (Ship ship in CargoShips)
            {
                //first make sure each ship without a multinode destination, has a destination;
                if (ship.HasMultiNodeDestination != true)
                {
                    if (ship.HasDestination == false)
                    {
                        SetNewDestinationStellarObject(ship); 
                    }
                }
                //move each ship towards its destination, just 2 lines.
                ship.MoveCounter -= 1;
                ship.MovedPosition = Point3D.Add(ship.MovedPosition, ship.MoveVector);
                // if shp arrived at destination, set a new destination, both single and multiple node destinations.
                if (ship.MoveCounter < 1)
                {
                    ship.CurrentStellarObject = ship.DestinationStellarObject;
                    //reset poition, currentindex and destinationindex
                    ship.BeginPosition = ship.DestinationStellarObject.BeginPosition;
                    if (ship.HasMultiNodeDestination == true)
                    {
                        ship.DestinationStellarObject = ship.PathStellarObjectsQueuefromSourcetoDestination.Dequeue();
                        if (ship.PathStellarObjectsQueuefromSourcetoDestination.Count() == 0)
                        {
                            ship.HasMultiNodeDestination = false;
                        }
                    }
                    else
                    {
                        ship.DestinationStellarObject = ship.DestinationStellarObject.StarLanes[rand.Next(0, ship.DestinationStellarObject.StarLanes.Count())].To;
                    }
                    if (ship.ShipOwnedbyPlayer == true)
                    {
                        foreach (Starlane starlane in ship.CurrentStellarObject.StarLanes)
                        {
                            if (ship.DestinationStellarObject == starlane.To)
                            {
                                starlane.Color = Color.FromRgb(3, 3, 25);
                                foreach (Starlane starlanedest in ship.DestinationStellarObject.StarLanes)
                                {
                                    if (ship.CurrentStellarObject == starlanedest.To)
                                    {
                                        starlanedest.Color = Color.FromRgb(2, 2, 25);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    SetNewDestinationStellarObject(ship);
                }
            }
        }
    }
}

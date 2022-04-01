
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
        readonly FastRandom rand = new FastRandom();

        #region fields
        protected int _startnumberofcargoships;
        protected bool _bmoveships;
        protected Ship _shipselectedonscreen;
        protected ObservableCollection<CargoShip> _playercargoship;
        #endregion

        #region Class properties
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

        public Ship ShipSelectedonScreen
        {
            get { return _shipselectedonscreen; }
            set
            {
                _shipselectedonscreen = value;
                OnPropertyChanged();
            }
        }

        public bool BMoveShips
        {
            get { return _bmoveships; }
            set { _bmoveships = value; }
        }
        #endregion

        #region constructor
        public ShipViewModel()
        {
            //_startnumberofcargoships = 50;
            _bmoveships = false;
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

        private void SetPlayerFirstShip(string name, StellarObject stellarobject, IEconomicEntity economicentity)
        {
            Color color = Color.FromRgb(40,40,40);
            CargoShips.Add(new CargoShip(name, stellarobject.BeginPosition, 2, stellarobject.StarLanes[rand.Next(0, stellarobject.StarLanes.Count)].To, stellarobject, color, (EconomicEntity)economicentity, true));
        }

        public void InitializeShips(IReadOnlyList<StellarObject> stellarobjects, IReadOnlyList<IEconomicEntity> economicentities)
        {
            CargoShips.Clear();
            int tindex;
            int cntr = 0;
            Point3D tpnt;
            Color color;
            int x, y, z;
            for (int i = 0; i < StartNumberofCargoShips; i++)
            {
                x = rand.Next(0, 100);
                y = rand.Next(0, 100);
                z = rand.Next(150, 256);
                color = Color.FromRgb((byte)x, (byte)y, (byte)z);
                tindex = rand.Next(0, stellarobjects.Count());
                tpnt = stellarobjects[tindex].BeginPosition;
                cntr += 1;
                CargoShips.Add(new CargoShip($"Cargo-AI-{cntr}", tpnt, 1, stellarobjects[tindex].StarLanes[rand.Next(0, stellarobjects[tindex].StarLanes.Count)].To, stellarobjects[tindex],color, (EconomicEntity)economicentities[rand.Next(0,economicentities.Count())], false));
            }
            SetPlayerFirstShip("Marlinde", stellarobjects[0], economicentities[0]);
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
            Ship smallestship = new Ship();
            int smallestdistance = 1000000;
            foreach (Ship ship in CargoShips)
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
        private void SetNewDestinationStellarObject(Ship shp)
        {
            double  timeinterval;
            double deltax, deltay, deltaz;
            deltax = (shp.DestinationStellarObject.BeginPosition.X - shp.CurrentStellarObject.BeginPosition.X);
            deltay = (shp.DestinationStellarObject.BeginPosition.Y - shp.CurrentStellarObject.BeginPosition.Y);
            deltaz = (shp.DestinationStellarObject.BeginPosition.Z - shp.CurrentStellarObject.BeginPosition.Z);
            timeinterval = GetDistanceBetweenShipAndStar(shp.BeginPosition, shp.DestinationStellarObject.BeginPosition) / shp.Speed;
            // set ship data for travel. how much to move each turn and how many turns
            shp.MoveCounter = (int)timeinterval;  //work with movecounter to stop ships from missing destination and continuing across map. eventually crashing the game
            shp.MoveVector = new Vector3D(deltax / timeinterval, deltay / timeinterval, deltaz / timeinterval);
            shp.HasDestination = true;
            shp.MovedPosition = shp.BeginPosition;
            shp.BeginPosition = shp.DestinationStellarObject.BeginPosition; ;
        }
        public void MoveShipsNew()
        {
            ////double to stop accidental deviating from course

            foreach (Ship shp in CargoShips)
            {
                //first make sure each ship without a multinode destination, has a destination;
                if (shp.HasMultiNodeDestination != true)
                {
                    if (shp.HasDestination == false)
                    {
                        SetNewDestinationStellarObject(shp);
                    }
                }
                //move each ship towards its destination, just 2 lines.
                shp.MoveCounter -= 1;
                shp.MovedPosition = Point3D.Add(shp.MovedPosition, shp.MoveVector);
                // if shp arrived at destination, set a new destination, both single and multiple node destinations.
                if (shp.MoveCounter < 1)
                {
                    StellarObject tmpdestinationstar = shp.DestinationStellarObject;
                    //reset poition, currentindex and destinationindex
                    shp.BeginPosition = shp.DestinationStellarObject.BeginPosition;
                    if (shp.HasMultiNodeDestination == true)
                    {
                        shp.DestinationStellarObject = shp.PathStellarObjectsQueuefromSourcetoDestination.Dequeue();
                        if (shp.PathStellarObjectsQueuefromSourcetoDestination.Count() == 0)
                        {
                            shp.HasMultiNodeDestination = false;
                        }
                    }
                    else
                    {
                        shp.DestinationStellarObject = shp.DestinationStellarObject.StarLanes[rand.Next(0, shp.DestinationStellarObject.StarLanes.Count())].To;
                    }

                    shp.CurrentStellarObject = tmpdestinationstar;
                    if (shp.OwnShip == true)
                    {
                        foreach (Starlane starlane in shp.CurrentStellarObject.StarLanes)
                        {
                            if (shp.DestinationStellarObject == starlane.To)
                            {
                                starlane.Color = Color.FromRgb(255, 255, 255);
                                foreach (Starlane starlanedest in shp.DestinationStellarObject.StarLanes)
                                {
                                    if (shp.CurrentStellarObject == starlanedest.To)
                                    {
                                        starlanedest.Color = Color.FromRgb(255, 255, 255);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    SetNewDestinationStellarObject(shp);
                }
            }
        }
    }
}

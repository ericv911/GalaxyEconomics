using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraderExplorer
{
    public class Ship : Construction 
    {
        //addcrew    addcargo    addbattlehistory     crew has xp.   
        private Engine _engine = new Engine();
        private Hull _hull = new Hull(); //hullsize
        private List<Weapon> _shipweapons = new List<Weapon>();
        public Ship()
        {

        }
        public Hull ShipHull
        {
            get { return _hull; }
            set { _hull = value; }
        }

        public Engine ShipEngine
        {
            get { return _engine; }
            set { _engine = value; }
        }

        public List<Weapon> ShipWeapons
        {
            get { return _shipweapons; }
        }
    }
}

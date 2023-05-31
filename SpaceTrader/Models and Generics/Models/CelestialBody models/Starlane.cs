using Common.Technology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpaceTrader
{
    public interface IStarlane
    {
        StellarObject From { get; }
        StellarObject To { get; }
        TechnologyLevel TechLevelRequiredforTravel { get; }
        Color Color { get; }
    }

    // Starlanes are stellarobject items.   Every starlane has a .from and .to property. Every stellarobject can  and should(!) have multiple starlanes.
    // Every starlane has a duplicate.  The From and To from one stellarobject and the reverse From and TO from the stellarobject on the other side.      
    // This is to ensure that calculations during the turn are kept minimal. If a ship or what enters a stellar object, the available destinations
    // do not need to be calcualated but are known. 
    // An  collection of starlanes independent of the stellar objects, with stellar objects as it's member, would result in extensive computations during the running of a turn.  
    // With the current setup, most of the computations and extra work are done during the phase of initialisation of all objects.
    // During the running of a turn, not much extra calculations are needed. Except when the mouse is clicked to determine the closest starlane. To highlighting the Starlane. 

    // IF a starlane is added to the collection, do it both ways. .From and .To for both stellarobjects.!! 


    public class Starlane : IStarlane
    {
        protected double _length;
        protected Color _color;
        protected TechnologyLevel _techlevelrequiredfortravel;
        protected StellarObject _from;
        protected StellarObject _to;
        protected double _distance;
        public Starlane()
        {

        }
        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public StellarObject From
        {
            get { return _from; }
            set { _from = value; }
        }
        public StellarObject To
        {
            get { return _to; }
            set { _to = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public TechnologyLevel TechLevelRequiredforTravel
        {
            get { return _techlevelrequiredfortravel; }
            set { _techlevelrequiredfortravel = value;
                _color = _techlevelrequiredfortravel.Color;
            }
        }
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        public void SetLength()
        {
            Length =  Math.Sqrt( Math.Pow(From.BeginPosition.X - To.BeginPosition.X, 2) + Math.Pow(From.BeginPosition.Y - To.BeginPosition.Y, 2) + Math.Pow(From.BeginPosition.Z - To.BeginPosition.Z, 2));
        }
    }
}

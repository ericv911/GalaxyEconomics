using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpaceTrader
{
    public class TechLevel
    {
        protected string _name;
        protected int _techlevel;
        protected Color _color;
        public TechLevel()
        {

        }
        public TechLevel(string name, int techlevel, Color clr)
        {
            _name = name;
            _techlevel = techlevel;
            _color = clr;
        }
        public int Techlevel
        {
            get { return _techlevel; }
            set { _techlevel = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

    }
}

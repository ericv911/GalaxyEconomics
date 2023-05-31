using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Common.Economy
{
    public interface IEconomicEntity
    {

    }
    public class EconomicEntity : IEconomicEntity
    {
        protected string _name;
        protected Color _color;

        public EconomicEntity()
        {

        }
        public EconomicEntity(string name, Color color)
        {
            _name = name;
            _color = color;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
}

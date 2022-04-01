using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraderExplorer
{
    public abstract class Construction : IConstruction
    {
        private double _condition;
        private int _type;

        public double Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public void SetCondition(double modifyByAmount)
        {
            _condition += modifyByAmount;
        }

        public void SetType(int type)
        {
            _type = type;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraderExplorer
{
    public class Person
    {
        private int _status;
        private int _morale;
        private string _lastName;
        private string _firstName;

        public string FirstName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string LastName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public int Morale
        {
            get { return _morale; }
            set { _morale = value; }
        }

        public int Status //0 dead, 1 wounded, 2 alive
        {
            get { return _status; }
            set { _status = value; }
        }

    }
}

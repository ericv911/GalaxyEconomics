using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DOperations
{
    public class Translation
    {
        /// <summary>
        /// class used in Settigns to store the X and y Translation variables 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Translation(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Translation()
        {
            X = 0;
            Y = 0;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
}

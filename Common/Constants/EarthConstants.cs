using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public interface IEarthConstants
    {
        double Mass { get; }
    }
    public class EarthConstants : IEarthConstants
    {
        public double Mass { get; set; }
        public EarthConstants()
        {
            LoadConstantsfromFile();
        }
        private void LoadConstantsfromFile()
        {
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/constants/physical constants.dat")))
            {

                if (line.Length > 0 && !(line.Substring(0, 1) == "/"))
                {
                    splitstring = line.Split('=');
                    switch (splitstring[0].Trim(' '))
                    {
                        case "EarthMass":
                            Mass = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

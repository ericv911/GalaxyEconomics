using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public interface ISolarConstants
    {
        double Mass { get; }
        double Radius { get; }
        double Temperature { get; }
        double Luminosity { get; }
    }

    public class SolarConstants : ISolarConstants
    {
        public double Temperature { get; set; } //km
        public double Radius { get; set; } //kg
        public double Mass { get; set; } //watt
        public double Luminosity { get; set; } // celcius
        public SolarConstants()
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
                        case "SolarMass":
                            Mass = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "SolarRadius":
                            Radius = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "SolarSurfaceTemperature":
                            Temperature = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "SolarLuminosity":
                            Luminosity = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

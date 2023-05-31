using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public interface IPhysicalConstants
    {
        double StefanBoltzmannConstant { get; }
        double WattperM2UpperBoundaryforHabitablezone { get; }
        double WattperM2LowerBoundaryforHabitablezone { get; }
        double WattperM2OptimalforHabitablezone { get; }
    }
    public class PhysicalConstants : IPhysicalConstants
    {
        public double WattperM2OptimalforHabitablezone { get; set; }
        public double WattperM2UpperBoundaryforHabitablezone { get; set; }
        public double WattperM2LowerBoundaryforHabitablezone { get; set; }
        public double StefanBoltzmannConstant { get; set; }

        public PhysicalConstants()
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
                        case "StefanBoltzmannConstant":
                            StefanBoltzmannConstant = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "WattperM2LowerBoundaryforHabitablezone":
                            WattperM2LowerBoundaryforHabitablezone = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "WattperM2UpperBoundaryforHabitablezone":
                            WattperM2UpperBoundaryforHabitablezone = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        case "WattperM2OptimalforHabitablezone":
                            WattperM2OptimalforHabitablezone = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

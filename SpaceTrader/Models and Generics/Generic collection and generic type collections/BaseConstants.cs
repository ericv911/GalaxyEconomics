using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpaceTrader
{
    public interface IPhysicalConstants
    {
        double StefanBoltzmannConstant { get; }
        double WattperM2UpperBoundaryforHabitablezone { get; }
        double WattperM2LowerBoundaryforHabitablezone { get; }
        double WattperM2OptimalforHabitablezone { get; }
    }
    public interface ISolarConstants
    {
        double Mass { get; }
        double Radius { get; }
        double Temperature { get; }
        double Luminosity { get; }
    }
    public class BaseConstants
    {
        public class PhysicalConstants :IPhysicalConstants
        {
            protected double _wattperm2upperboundaryforhabitablezone;
            protected double _wattperm2lowerboundaryforhabitablezone;
            protected double _wattperm2optimalforhabitablezone;
            protected double _stefanboltzmannconstant;

            public double WattperM2OptimalforHabitablezone
            {
                get { return _wattperm2optimalforhabitablezone; }
                set { _wattperm2optimalforhabitablezone = value; }
            }
            public double WattperM2UpperBoundaryforHabitablezone
            { 
                get {  return _wattperm2upperboundaryforhabitablezone; }
                set { _wattperm2upperboundaryforhabitablezone = value; }
            }
            public double WattperM2LowerBoundaryforHabitablezone 
            {
                get { return _wattperm2lowerboundaryforhabitablezone; }
                set { _wattperm2lowerboundaryforhabitablezone = value; }
            }
            public double StefanBoltzmannConstant
            {
                get { return _stefanboltzmannconstant; }
                set { _stefanboltzmannconstant = value; }

            }
            public PhysicalConstants()
            {
                LoadConstantsfromFile();
            }
            private void LoadConstantsfromFile()
            {
                int linecounter = 0;
                string[] splitstring;
                foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/constants/physical constants.dat")))
                {
                    if (line.Length > 0)
                    {
                        if (line.Substring(0, 1) == "{")
                        {
                            if (linecounter == 1)
                            {
                                splitstring = line.Split(',');
                                StefanBoltzmannConstant = double.Parse(splitstring[0].Trim(new Char[] { '{' }), CultureInfo.InvariantCulture);
                                WattperM2LowerBoundaryforHabitablezone = double.Parse(splitstring[1].Trim(new Char[] { '{' }), CultureInfo.InvariantCulture);
                                WattperM2UpperBoundaryforHabitablezone = double.Parse(splitstring[2].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture);
                                WattperM2OptimalforHabitablezone = double.Parse(splitstring[3].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture);
                            }
                            linecounter += 1;
                        }
                    }
                }
            }
        }
        public class SolarConstants : ISolarConstants
        {
            protected double _radius; // Kms
            protected double _mass; // Kgs
            protected double _luminosity;  // Watts
            protected double _temperature; // degrees Celsius

            public double Temperature
            {
                get { return _temperature; }
                set { _temperature = value; }
            }
            public double Radius
            {
                get { return _radius; }
                set { _radius = value; }
            }
            public double Mass
            {
                get { return _mass; }
                set { _mass = value;}

            }
            public double Luminosity
            {
                get { return _luminosity; }
                set { _luminosity = value; }
            }
            public SolarConstants()
            {
                LoadConstantsfromFile();
            }
            private void LoadConstantsfromFile()
            {
                int linecounter = 0;
                string[] splitstring;
                foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/constants/physical constants.dat")))
                {
                    if (line.Length > 0)
                    {
                        if (line.Substring(0, 1) == "{" && linecounter == 0)
                        {
                            splitstring = line.Split(',');
                            Mass = double.Parse(splitstring[0].Trim(new Char[] { '{' }), CultureInfo.InvariantCulture);
                            Radius = double.Parse(splitstring[1], CultureInfo.InvariantCulture);
                            Temperature = double.Parse(splitstring[2], CultureInfo.InvariantCulture);
                            Luminosity = double.Parse(splitstring[3].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture);
                            linecounter += 1;
                        }
                    }
                }
            }
        }
    }
}

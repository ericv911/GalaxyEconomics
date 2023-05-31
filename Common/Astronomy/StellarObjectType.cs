using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Astronomy
{
    public interface IStellarObjectType
    {
        int RelativeOccurence { get; }
        int StarColorRed { get; }
        int StarColorGreen { get; }
        int StarColorBlue { get; }
        double Maximum_Mass { get; }
        double Minimum_Mass { get; }
        int Minimum_Age { get; }
        int Maximum_Age { get; }
        double Maximum_Temp { get; }
        double Minimum_Temp { get; }
        double Maximum_AbsoluteMagnitude { get; }
        double Minimum_AbsoluteMagnitude { get; }
        int Maximum_Radius { get; }
        int Minimum_Radius { get; }
    }
    public class StellarObjectType : CelestialBodyType, IStellarObjectType
    {
        protected string _lifePhase;
        protected double _minimum_absolutemagnitude;
        protected double _maximum_absolutemagnitude;
        protected double _minimum_Temp;
        protected double _maximum_Temp;
        protected int _minimum_Age;
        protected int _maximum_Age;
        protected int _starColorRed;
        protected int _starColorGreen;
        protected int _starColorBlue;

        public string LifePhase
        {
            get { return _lifePhase; ; }
            set { _lifePhase = value; }
        }

        public double Minimum_AbsoluteMagnitude
        {
            get { return _minimum_absolutemagnitude; }
            set { _minimum_absolutemagnitude = value; }
        }

        public double Maximum_AbsoluteMagnitude
        {
            get { return _maximum_absolutemagnitude; }
            set { _maximum_absolutemagnitude = value; }
        }
        public int Minimum_Age
        {
            get { return _minimum_Age; }
            set { _minimum_Age = value; }
        }

        public int Maximum_Age
        {
            get { return _maximum_Age; }
            set { _maximum_Age = value; }
        }

        public int StarColorRed
        {
            get { return _starColorRed; }
            set { _starColorRed = value; }
        }

        public int StarColorGreen
        {
            get { return _starColorGreen; }
            set { _starColorGreen = value; }
        }
        public int StarColorBlue
        {
            get { return _starColorBlue; }
            set { _starColorBlue = value; }
        }

        public double Minimum_Temp
        {
            get { return _minimum_Temp; }
            set { _minimum_Temp = value; }
        }
        public double Maximum_Temp
        {
            get { return _maximum_Temp; }
            set { _maximum_Temp = value; }
        }
    }
}

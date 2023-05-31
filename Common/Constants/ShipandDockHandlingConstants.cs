using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constants
{
    public interface IShipConstants
    {
        int BaseDockingDurationforRefueling { get; }
        int BaseDockingDurationforOverhauling { get; }
        int BaseDockingDurationforLoadingUnloading { get; }
        int BaseMinimalFuelAmountforNeedsRefuelingFlagSet { get; }
        int BaseMinimumHullIntegrityAmountforNeedsRepairingFlagSet { get; }
        double BaseChanceofOverhaulingShipwhenDocked { get; }
        int BaseAmountofDamageRepairedEachTurn { get; }
    }
    public class ShipConstants : IShipConstants
    {
        public ShipConstants()
        {
            LoadConstantsfromFile();
        }
        public int BaseDockingDurationforLoadingUnloading { get; set; }
        public int BaseDockingDurationforRefueling { get; set; }
        public int BaseDockingDurationforOverhauling { get; set; }
        public double BaseChanceofOverhaulingShipwhenDocked { get; set; }
        public int BaseMinimalFuelAmountforNeedsRefuelingFlagSet { get; set; }
        public int BaseMinimumHullIntegrityAmountforNeedsRepairingFlagSet { get; set; }
        public int BaseAmountofDamageRepairedEachTurn { get; set; }

        private void LoadConstantsfromFile()
        {
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/constants/ship constants.dat")))
            {
                if (line.Length > 0 && !(line.Substring(0, 1) == "/"))
                {
                    splitstring = line.Split('=');
                    switch (splitstring[0])
                    {
                        case "BaseDockingDurationforRefueling":
                            BaseDockingDurationforRefueling = Convert.ToInt32(splitstring[1]);
                            break;
                        case "BaseDockingDurationforOverhauling":
                            BaseDockingDurationforOverhauling = Convert.ToInt32(splitstring[1]);
                            break;
                        case "BaseDockingDurationforLoadingUnloading":
                            BaseDockingDurationforLoadingUnloading = Convert.ToInt32(splitstring[1]);
                            break;
                        case "BaseMinimalFuelAmountforNeedsRefuelingFlagSet":
                            BaseMinimalFuelAmountforNeedsRefuelingFlagSet = Convert.ToInt32(splitstring[1]);
                            break;
                        case "BaseMinimumHullIntegrityAmountforNeedsRepairingFlagSet":
                            BaseMinimumHullIntegrityAmountforNeedsRepairingFlagSet = Convert.ToInt32(splitstring[1]);
                            break;
                        case "BaseAmountofDamageRepairedEachTurn":
                            BaseAmountofDamageRepairedEachTurn = Convert.ToInt32(splitstring[1]);
                            break;
                        case "BaseChanceofOverhaulingShipwhenDocked":
                            BaseChanceofOverhaulingShipwhenDocked = (double.Parse(splitstring[1].Trim(new Char[] { '}' }), CultureInfo.InvariantCulture) / 100);
                            break;
                    }
                }
            }
        }
    }
}

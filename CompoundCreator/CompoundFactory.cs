
using Common.Physics;
using CompoundProvider.Types;
using FunctionalGroups.Types;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace CompoundProvider
{
    /// <summary>
    /// Assembly that turns a rules.dat file into an ObservableCollection of Compounds hidden behind an ICompoundCollection interface
    /// It needs the Common.Dll to function for the 2 classes Element, and ElementGroup
    /// It provides the Compound Class and ICompound Interface to other assemblies
    /// </summary>
    public class CompoundFactory
    {
        #region fields
        private static ICompoundCollection _compoundcollectioninstance;
        // a lock used for multithreaded purposes, see below

        private static readonly object _lock = new object();
        #endregion

        #region properties
        private static string CompoundRulesLocation { get; set; }
        #endregion

        #region constructor
        public CompoundFactory()
        {
        }
        #endregion

        // This Static method controls the number of instances of the CompoundCollection. 
        // if the instance already exists, it provides a reference to it, otherwise it creates a new instance;
        // in any case, it will always return an instance.
        // This is a singleton and a semi-factory method in one. 
        public static ICompoundCollection GetCompoundCollection(ObservableCollection<ElementGroup> elementgroups, ObservableCollection<Element> elements, ObservableCollection<FunctionalGroup> functionalgroups)
        {
            //load variables from file. 
            LoadConfigIni();
            if (_compoundcollectioninstance == null)
            {
                lock (_lock) // lock for multithreaded purposes. Once entered by a thread, other processes will halt here until the first process is finished. 
                {
                    if (_compoundcollectioninstance == null)
                    {
                        _compoundcollectioninstance = new CompoundCollection(CompoundRulesLocation, elementgroups, elements, functionalgroups);
                    }
                }
            }
            return _compoundcollectioninstance;
        }

        /// <summary> 
        /// Function for loading data from the config.ini.
        /// Used for the location of the Compoundrules file
        /// </summary>
        private static void LoadConfigIni()
        {
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CompoundCreator.ini")))
            {
                if (line.Length > 0 && !(line.Substring(0, 1) == "/"))
                {
                    splitstring = line.Split('=');
                    switch (splitstring[0])
                    {
                        case "CompoundRulesLocation":
                            CompoundRulesLocation = splitstring[1].Trim(new Char[] { '"' }); 
                            break;
                         default:
                            break;
                    }
                }
            }
        }
    }
}

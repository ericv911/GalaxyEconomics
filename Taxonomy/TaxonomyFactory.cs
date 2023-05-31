using System;
using System.IO;

namespace Taxonomy
{
    /// <summary>
    /// Create a factory that returns Collection instances.  The data is loaded at runtime and never changes. It's also a singleton.
    /// </summary>
    public sealed class TaxonomyFactory
    {
        #region fields
        //a private instance of a taxonomy collection. a single instance will be created and returned every time a GetInstance is called
        private static ITaxonomyCollection _instance;
        // a lock used for multithreaded purposes, see below
        private static readonly object _lock = new object();
        #endregion

        #region properties
        private static string DataLocation { get; set; }
        #endregion

        #region constructor
        private TaxonomyFactory()
        {
        }
        #endregion

        // This Static method controls the number of instances of the TaxonomyCollection. 
        // if the instance already exists, it provides a reference to it, otherwise it creates a new instance;
        // in any case, it will always return an instance.
        // This is a singleton and a factory method in one. 
        public static ITaxonomyCollection GetCollection()
        {
            LoadConfigIni();
            if (_instance == null)
            {
                lock (_lock) // lock for multithreaded purposes. Once entered by a thread, other processes will halt here until the first process is finished. 
                {
                    if (_instance == null)
                    {
                        _instance = new TaxonomyCollection(DataLocation);
                    }
                }
            }
            return _instance;
        }
        private static void LoadConfigIni()
        {
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Taxonomy.ini")))
            {
                if (line.Length > 0 && !(line.Substring(0, 1) == "/"))
                {
                    splitstring = line.Split('=');
                    switch (splitstring[0])
                    {
                        case "TaxonomyResourcesLocation":
                            DataLocation = splitstring[1].Trim(new char[] { '"' });
                            break;
                    }
                }
            }
        }
    }
}


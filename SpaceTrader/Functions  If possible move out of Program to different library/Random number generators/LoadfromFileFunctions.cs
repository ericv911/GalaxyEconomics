using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader
{
    public class FileActions
    {
        static public string[] ReadData(string path, int valuesperline)
        {
            int linecounter = 0;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        linecounter += 1;
                    }
                }
            }
            string[] tmpstring = new string[linecounter];
            linecounter = 0;
            //tmpstring = System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path))
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        tmpstring[linecounter] = line;
                        linecounter += 1;
                    }
                }
            }
            return tmpstring;
        }
    }
}

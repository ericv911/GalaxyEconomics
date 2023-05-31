using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FileHandlingSystem
{
    public class FileActions
    {
        public static void ReadData<T>(string path, ObservableCollection<T> collection, T data)
        {
            
        }
        public static string[] ReadData(string path, int valuesperline)
        {
            List<string> list = new List<string>();
            foreach (string line in File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
            {
                if (line.Length > 0)
                {
                    if (line.Substring(0, 1) == "{")
                    {
                        list.Add(line.Substring(0, line.IndexOf("}") + 1));
                    }
                }
            }
            return list.ToArray();
        }
    }
}

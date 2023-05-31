using Common.Physics;
using FileHandlingSystem;
using FunctionalGroups.Types;
using System;
using System.Collections.ObjectModel;


namespace FunctionalGroups
{
    internal class FunctionalGroupCollection : IFunctionalGroupCollection
    {
        public ObservableCollection<FunctionalGroup> FunctionalGroups { get; set; } = new ObservableCollection<FunctionalGroup>();

        internal FunctionalGroupCollection(string filepath, ObservableCollection<ElementGroup> elementgroups, ObservableCollection<Element> elements)
        {
            SetFunctionalGroupCollection(filepath, elements);
        }
        private int SetFunctionalGroupCollection(string filepath, ObservableCollection<Element> elements)
        {
            FunctionalGroup _functionalgroup;
            ElementinCompound _elementinfunctionalgroup; //class ElementinCompound is used for ElementinFunctionalGroup as a functional groups IS a compound.
            string[] splitstring;
            string[] stringfromfile = FileActions.ReadData(filepath + "functionalgrouprules.dat", 1);
            double _mass;
            foreach (string line in stringfromfile)
            {
                _mass = 0;
                _functionalgroup = new FunctionalGroup();
                splitstring = line.Split(',');
                _functionalgroup.Name = splitstring[0].Trim(new Char[] { '{' });
                _functionalgroup.Charge = Convert.ToInt32(splitstring[1]);
                for (int i = 0; i < Convert.ToInt32(splitstring[2]); i++)
                {
                    _elementinfunctionalgroup = new ElementinCompound
                    {
                        //ConstituentType = Convert.ToInt32(splitstring[i * 3 + 3]),
                        Element = elements[Convert.ToInt32(splitstring[i * 3 + 4]) - 1],
                        AmountinCompound = Convert.ToInt32(splitstring[i * 3 + 5].Trim(new Char[] { '}' }))
                    };
                    _mass += _elementinfunctionalgroup.Element.AtomicMass * _elementinfunctionalgroup.AmountinCompound;
                    _functionalgroup.ElementsinFunctionalGroup.Add(_elementinfunctionalgroup);

                }
                _functionalgroup.AtomicMass = _mass;
                FunctionalGroups.Add(_functionalgroup);
            }
            return 0;
        }
    }
}

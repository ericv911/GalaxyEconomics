using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Physics
{
    public interface IElementGroup
    {
        string Name { get; }
        ObservableCollection<Element> Elements { get; }
        double ElementGroupExtractionModifier { get; }
    }
    public class ElementGroup : IElementGroup
    {
        public ElementGroup()
        {
            Elements = new ObservableCollection<Element>();
        }

        public string Name { get; set; }
        public ObservableCollection<Element> Elements { get; set; }
        public double ElementGroupExtractionModifier { get; set; }
    }
}

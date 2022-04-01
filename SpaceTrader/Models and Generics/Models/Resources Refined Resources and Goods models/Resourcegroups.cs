using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpaceTrader
{
    public interface IResourceGroup
    {
        string Name { get; }
        ObservableCollection<Resource> Resources { get; }
    }
    public class ResourceGroup : IResourceGroup
    {
        protected string _name;
        protected ObservableCollection<Resource> _resources;
        protected ObservableCollection<int> _intresources;
        protected double _resourcegroupextractionmodifier;

        public ResourceGroup()
        {
            IntResources = new ObservableCollection<int>();
            Resources = new ObservableCollection<Resource>();
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public ObservableCollection<Resource> Resources
        {
            get { return _resources; }
            set { _resources = value; }
        }
        public ObservableCollection<int> IntResources
        {
            get { return _intresources; }
            set { _intresources = value; }
        }
        public double ResourcegroupExtractionModifier
        {
            get { return _resourcegroupextractionmodifier; }
            set { _resourcegroupextractionmodifier = value; }
        }
    }
}

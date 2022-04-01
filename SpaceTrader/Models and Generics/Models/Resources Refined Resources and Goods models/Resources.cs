

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpaceTrader
{
    public interface IResource
    { }
    public class Resource : INotifyPropertyChanged, IResource
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool _isEdible;
        protected bool _isprecious;
        protected bool _isillegal;
        protected bool _isradioactive;
        protected double _universalabundance;
        protected int _stateofmatter; // 0 = gas, 1 = liquid, 2 = solid 
        protected int _resourcegroup; //from type, used to calculate extraction modifiers.  Orbital bodies will randomly belong to a certain group and have extraction modifiers for that specific group.
        protected string _name;

        public Resource()
        {

        }
        public double UniversalAbundance
        {
            get { return _universalabundance; }
            set { _universalabundance = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int StateofMatter
        {
            get { return _stateofmatter; }
            set { _stateofmatter = value; }
        }
        public bool IsRadioActive
        {
            get { return _isradioactive; }
            set { _isradioactive = value; }
        }
    }
}


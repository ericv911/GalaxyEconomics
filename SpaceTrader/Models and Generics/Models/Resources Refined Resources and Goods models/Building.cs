using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader
{
    public interface IBuilding
    { }
    public class Building : IBuilding, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected BaseTypes.BuildingType _type;
        protected int _techlevel;
        protected int _size;
        protected int _techprogress;

        public Building() { }
        public Building(BaseTypes.BuildingType type)
        {
            _techlevel = 0;
            _size = 0;
            _techprogress = 0;
            _type = type;
        }

        public BaseTypes.BuildingType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int TechLevel
        {
            get { return _techlevel; }
            set {
                _techlevel = value;
                OnPropertyChanged();
            }
        }

        public int Size
        {
            get { return _size; }
            set { 
                _size = value;
                OnPropertyChanged();
            }
        }

        public int TechProgress
        {
            get { return _techprogress; }
            set { _techprogress = value; }
        }
    }
}

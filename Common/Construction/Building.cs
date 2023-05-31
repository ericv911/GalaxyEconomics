using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Construction
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

        protected int _techlevel;
        protected int _size;

        public Building() { }
        public Building(BuildingType type)
        {
            _techlevel = 0;
            _size = 0;
            TechProgress = 0;
            Type = type;
        }

        public BuildingType Type { get; set; }

        public int TechLevel
        {
            get { return _techlevel; }
            set
            {
                _techlevel = value;
                OnPropertyChanged();
            }
        }

        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }

        public int TechProgress { get; set; }
    }
}

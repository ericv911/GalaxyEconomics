using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTrader
{
    public interface IResourceStorageperBody
    {

    }
    public class ResourceInStorageperBody : IResourceStorageperBody, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool _haslocalresourcegroup;
        protected double _amount;
        protected Resource _resource;

        public bool HasLocalResourcegroup
        {
            get { return _haslocalresourcegroup; }
            set { _haslocalresourcegroup = value; }
        }
        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        public Resource Resource
        {
            get { return _resource; }
            set { _resource = value; }
        }
        public ResourceInStorageperBody()
        { }
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChemistryDynamics.Types
{
    public class BaseType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

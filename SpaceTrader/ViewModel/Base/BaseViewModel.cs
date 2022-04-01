using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpaceTrader
{
    /// A base view model that fires Property Changed events as needed
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        ///// The event that is fired when any child property changes its value
        ///// Call this to fire a <see cref="PropertyChanged"/> event
        ///// <param name="name"></param>

    }
}

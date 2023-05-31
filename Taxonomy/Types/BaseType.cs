
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Taxonomy.Types
{
    /// <summary>
    /// BaseType has the code for InotifyPropertyChanged and has all the shared properties of the classes that use it as a base
    /// </summary>
    public class BaseType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Name { get; set; }
    }
}

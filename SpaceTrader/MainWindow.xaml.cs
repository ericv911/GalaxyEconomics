
using System.Windows;


namespace SpaceTrader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new SpaceGameMainViewModel(this); 
        }
    }
}

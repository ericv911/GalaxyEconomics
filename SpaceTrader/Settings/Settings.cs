using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace SpaceTrader
{
    public interface ITimerSettings
    {
        DispatcherTimer ClockTimer { get; }
        void SetTimer();

    }
    public interface IGeneralSettings
    {
        ScreenSettings ScreenSettings { get; }
        KeyboardSettings KeyboardSettings { get; }
        MouseSettings MouseSettings { get; }
        BitmapDataSettings BitmapDataSettings { get; }
        ITimerSettings Timer { get; }
    }
    public class GeneralSettings : IGeneralSettings
    {
        public GeneralSettings()
        {
            ScreenSettings = new ScreenSettings();
            MouseSettings = new MouseSettings();
            Timer = new TimerSettings();
            BitmapDataSettings = new BitmapDataSettings();
            KeyboardSettings = new KeyboardSettings();
        }
        public KeyboardSettings KeyboardSettings
        {
            get;set;
        }
        public ScreenSettings ScreenSettings
        {
            get; set;
        }
        public MouseSettings MouseSettings
        {
            get; set;
        }
        public ITimerSettings Timer
        {
            get; set;
        }
        public BitmapDataSettings BitmapDataSettings
        {
            get; set;
        }
    }

    public class KeyboardSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool PressedShift {get; set;}
        public bool PressedAlt { get; set; }
        public bool PressedCtrl { get; set; }
        public KeyboardSettings()
        {
            PressedShift = false;
            PressedAlt = false;
            PressedCtrl = false;
        }
    }
    public class BitmapDataSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region fields
        private int _bitmapwidth;
        private int _bitmapheight;
        protected Image _image;
        #endregion

        #region properties

        public int BitmapHeight
        {
            get { return _bitmapheight; }
            set
            {
                _bitmapheight = value;
                OnPropertyChanged();
            }
        }
        public int BitmapWidth
        {
            get { return _bitmapwidth; }
            set
            {
                _bitmapwidth = value;
                OnPropertyChanged();
            }
        }
        public Vector3D bitmapadjustvector;
        public byte[,,] Pixels { get; set; }
        public byte[] Pixels1d { get; set; }
        public WriteableBitmap GrdBmp;
        public Int32Rect Rect { get; set; }
        public Image Image
        {
            get { return _image; }
            set { _image = value;
                OnPropertyChanged();
            }
        }
      

        #endregion
        public void ResetBitmapAdjustVector(int deltamousewheel)
        {
            if (deltamousewheel > 0)
            {
                bitmapadjustvector.X *= 2;
                bitmapadjustvector.Y *= 2;
                bitmapadjustvector.Z *= 2;
            }
            else
            {
                bitmapadjustvector.X /= 2;
                bitmapadjustvector.Y /= 2;
                bitmapadjustvector.Z /= 2;
            }
        }
        public void SetBitmapData(int width, int height)
        {
            BitmapWidth = width;
            BitmapHeight = height;
            Rect = new Int32Rect(0, 0, width, height);
            GrdBmp = new WriteableBitmap(4000, 4000, 96, 96, PixelFormats.Bgra32, null);
            bitmapadjustvector = new Vector3D((int)width / 2, (int)width / 2, (int)height / 2);
            Image = new Image { Stretch = Stretch.None, Margin = new Thickness(0) };
            Pixels = new byte[height, width, 4];
            Pixels1d = new byte[height * width * 4];
        }
        public void ResizeBitmapData(int delta)
        {
            int width, height;
            BitmapWidth += delta;
            width = BitmapWidth;
            BitmapHeight += delta;
            height = BitmapHeight;
            Rect = new Int32Rect(0, 0, width, height);
            bitmapadjustvector = new Vector3D((int)width / 2, (int)width / 2, (int)height / 2);
            Image = new Image { Stretch = Stretch.None, Margin = new Thickness(0) };
            Pixels = new byte[height, width, 4];
            Pixels1d = new byte[height * width * 4];
        }
    }

    public class TimerSettings : ITimerSettings
    {
        public TimerSettings()
        {
            ClockTimer = new DispatcherTimer();
        }
        public DispatcherTimer ClockTimer { get; set; } // = new DispatcherTimer();
        public void SetTimer() //Set timer parameters and start
        {
            ClockTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            ClockTimer.Start();
        }
    }
    public class ScreenSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool _displaybuttoncalculateshiptostellarobject;
        protected int _screenwidth;
        protected int _screenheight;
        protected int _oldscreenwidth;
        protected int _oldscreenheight;

        protected Visibility _visibilityshipinfoonscreen;
        protected Visibility _visibilitybuttoncalculateshiptostellarobject;
        protected bool _displayshipinfoonscreen;
        protected bool _bdrawlines;
        protected Point3dSettings _point3dsettings;

        public Visibility VisibilityButtonCalculateShiptoStellarObject
        {
            get { return _visibilitybuttoncalculateshiptostellarobject; }
            set
            {
                _visibilitybuttoncalculateshiptostellarobject = value;
                OnPropertyChanged();
            }
        }
        public Visibility VisibilityShipInfoonScreen
        {
            get { return _visibilityshipinfoonscreen; }
            set
            {
                _visibilityshipinfoonscreen = value;
                OnPropertyChanged();
            }
        }
        public int OldScreenWidth
        {
            get { return _oldscreenwidth; }
            set
            {
                _oldscreenwidth = value;
                OnPropertyChanged();
            }
        }
        public int OldScreenHeight
        {
            get { return _oldscreenheight; }
            set
            {
                _oldscreenheight = value;
                OnPropertyChanged();
            }
        }
        public int ScreenWidth
        {
            get { return _screenwidth; }
            set
            {
                _screenwidth = value;
                OnPropertyChanged();
            }
        }
        public int ScreenHeight
        {
            get { return _screenheight; }
            set
            {
                _screenheight = value;
                OnPropertyChanged();
            }
        }
        public ScreenSettings()
        {
            _visibilityshipinfoonscreen = Visibility.Hidden;
            _visibilitybuttoncalculateshiptostellarobject = Visibility.Hidden;
            _point3dsettings = new Point3dSettings();
            IsGameDataDrawn = false;
            DeltaRotationAngle = 0.05;
        }
        public bool DisplayShipInfoonScreen 
        { get 
            { return _displayshipinfoonscreen; }
            set
            {
                _displayshipinfoonscreen = value;
                if (_displayshipinfoonscreen)
                {
                    VisibilityShipInfoonScreen = Visibility.Visible;
                }
                else
                {
                    VisibilityShipInfoonScreen = Visibility.Hidden;
                }
                OnPropertyChanged();
            }
        }
        public bool DisplayButtonCalculateShiptoStellarObject
        {
            get
            { return _displaybuttoncalculateshiptostellarobject; }
            set
            {
                _displaybuttoncalculateshiptostellarobject = value;
                if (_displaybuttoncalculateshiptostellarobject)
                {
                    VisibilityButtonCalculateShiptoStellarObject = Visibility.Visible;
                }
                else
                {
                    VisibilityButtonCalculateShiptoStellarObject = Visibility.Hidden;
                }
                OnPropertyChanged();
            }
        }

        public bool IsGameDataDrawn { get; set; }
        public bool GamePaused { get; set; }
        public double DeltaRotationAngle { get; set; }
        public Point3dSettings Point3DSettings
        {
            get { return _point3dsettings; }
            set { _point3dsettings = value; }
        }

        public bool BDrawLines
        {
            get { return _bdrawlines; }
            set
            {
                _bdrawlines = value;
                OnPropertyChanged();
            }
        }
    }
    public class MouseSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private int _x;
        private int _y;
        protected bool _mousepressedleft;
        protected Visibility _buttonatpointervisibilty;
        protected Point _mouseposwhenpressedleft;
        protected Point _mouseposwhenpressedlefta;
        protected Point _mouseposwhenpressedright;
        
        public MouseSettings()
        {
            _mousepressedleft = false;
            _buttonatpointervisibilty = Visibility.Hidden;
        }
        public bool bMousepressedRight = false;
        public bool MousepressedLeft
        {
            get { return _mousepressedleft; }
            set { _mousepressedleft = value;
                if(_mousepressedleft == true)
                {
                   //ButtonatPointerVisibility = Visibility.Visible;
                }
                else
                {
                   // ButtonatPointerVisibility = Visibility.Hidden;
                }
                OnPropertyChanged();
            }
        }

        public Visibility ButtonatPointerVisibility
        {
            get { return _buttonatpointervisibilty; }
            set {
                _buttonatpointervisibilty = value;
                OnPropertyChanged();    
            }
        }

        public Point MousePosWhenPressedRight
        {
            get { return _mouseposwhenpressedright; }
            set { _mouseposwhenpressedright = value;
                OnPropertyChanged();
            }
        }

        public Point MousePosWhenPressedLeftA
        {
            get { return _mouseposwhenpressedlefta; }
            set
            {
                _mouseposwhenpressedlefta = value;
                OnPropertyChanged();
            }
        }
        public Point MousePosWhenPressedLeft
        {
            get { return _mouseposwhenpressedleft; }
            set
            {
                _mouseposwhenpressedleft = value;
                OnPropertyChanged();
            }
        }

        public int X
        {
            get { return _x; }
            set
            {
                if (value.Equals(_x)) return;
                _x = value;
                OnPropertyChanged();
            }
        }
        public int Y
        {
            get { return _y; }
            set
            {
                if (value.Equals(_y)) return;
                _y = value;
                OnPropertyChanged();
            }
        }
        public Point MousePosition()
        {
            return new Point(X, Y);
        }
    }
}

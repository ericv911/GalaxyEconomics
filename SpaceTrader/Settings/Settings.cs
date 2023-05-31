using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using _3DOperations;
namespace SpaceTrader
{
    public interface ITimerSettings
    {
        DispatcherTimer ClockTimer { get; }
        void SetTimer();

    }
    public interface IGeneralSettings
    {
        FileSettings FileSettings { get; }
        StellarObjectSettings StellarObjectSettings { get; }
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
            FileSettings = new FileSettings();
            ScreenSettings = new ScreenSettings();
            MouseSettings = new MouseSettings();
            Timer = new TimerSettings();
            BitmapDataSettings = new BitmapDataSettings();
            KeyboardSettings = new KeyboardSettings();
            StellarObjectSettings = new StellarObjectSettings();
        }
        public StellarObjectSettings StellarObjectSettings
        {
            get; set;
        }
        public FileSettings FileSettings
        {
            get;set;
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
        public string SwitchElementandSymbol { get; set; }
        public string ShowResources { get; set; }
        public bool PressedShift {get; set;}
        public bool PressedAlt { get; set; }
        public bool PressedCtrl { get; set; }
        public KeyboardSettings()
        {
            LoadSettingsfromFile();
            PressedShift = false;
            PressedAlt = false;
            PressedCtrl = false;
        }

        private void LoadSettingsfromFile()
        {
            string[] splitstring;
            foreach (string line in System.IO.File.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources/keyboardconfig.ini")))
            {
                if (line.Length > 0 && !(line.Substring(0, 1) == "/"))
                {
                    splitstring = line.Split('=');
                    switch (splitstring[0])
                    {
                        case "SwitchElementandSymbol":
                            SwitchElementandSymbol = splitstring[1];
                            break;
                        case "ShowResources":
                            ShowResources = splitstring[1];
                            break;
                    }
                }
            }
        }
    }
    public class FileSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region fields

        #endregion
        #region properties

        #endregion
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
        //protected Image _image;
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
        //public Image Image
        //{
        //    get { return _image; }
        //    set { _image = value;
        //        OnPropertyChanged();
        //    }
        //}
      

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
            //Image = new Image { Stretch = Stretch.None, Margin = new Thickness(0) };
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
           // Image = new Image { Stretch = Stretch.None, Margin = new Thickness(0) };
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
    public class StellarObjectSettings: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool _showcentralhubelements;
        protected bool _showelements;
        protected bool _showmodifiers;
        protected bool _showbuildings;
        protected bool _showelementgroups;
        protected List<string> _stellarobjectclasses;
        protected string _selectedstellarobjectclass;

        public List<string> StellarObjectClasses
        {
            get { return _stellarobjectclasses; }
            set
            {
                _stellarobjectclasses = value;
                OnPropertyChanged();
            }
        }
        public string SelectedStellarObjectClass
        {
            get { return _selectedstellarobjectclass; }
            set
            {
                _selectedstellarobjectclass = value;
                OnPropertyChanged();
            }
        }
        public bool ShowElementGroups
        {
            get { return _showelementgroups; }
            set
            {
                _showelementgroups = value;
                OnPropertyChanged();

            }
        }
        public bool ShowCentralHubElements
        {
            get { return _showcentralhubelements; }
            set
            {
                _showcentralhubelements = value;
                OnPropertyChanged();

            }
        }
        public bool ShowElements
        {
            get { return _showelements; }
            set
            {
                _showelements = value;
                OnPropertyChanged();

            }
        }
        public bool ShowBuildings
        {
            get { return _showbuildings; }
            set
            {
                _showbuildings = value;
                OnPropertyChanged();

            }
        }
        public bool ShowModifiers
        {
            get { return _showmodifiers; }
            set
            {
                _showmodifiers = value;
                OnPropertyChanged();
            }
        }
        public StellarObjectSettings()
        {

        }
    }
    public class ScreenSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected List<string> _fontsizes;
        protected string _currentfontsize;
        protected bool _displaybuttoncalculateshiptostellarobject;
        protected int _scrollviewerheightbig;
        protected int _scrollviewerheightsmall;
        protected int _screenwidth;
        protected int _screenheight;
        protected int _oldscreenwidth;
        protected int _oldscreenheight;
        protected bool _displayelementsymbol;
        protected Visibility _visibilityshipinfoonscreen;
        protected Visibility _visibilitybuttoncalculateshiptostellarobject;
        protected bool _displayshipinfoonscreen;
        protected bool _showorbitalbodyinfo;
        protected bool _shownaturalsatellitesinfo;
        protected bool _drawstarlanes;
        protected bool _showcentralhubelements;
        protected bool _showelements;
        protected bool _showmodifiers;
        protected bool _showbuildings;
        protected bool _showelementgroups;
        protected bool _drawshipsonscreen;
        protected _3DSettings _3dsettings;

        public int ScrollViewerHeightSmall
        {
            get { return _scrollviewerheightsmall; }
            set
            {
                _scrollviewerheightsmall = value;
                OnPropertyChanged();
            }
        }
        public int ScrollViewerHeightBig
        {
            get { return _scrollviewerheightbig; }
            set
            {
                _scrollviewerheightbig = value;
                OnPropertyChanged();
            }
        }
        public List<string> FontSizes
        {
            get { return _fontsizes; }
            set
            {
                _fontsizes = value;
                OnPropertyChanged();
            }

        }
        public string CurrentFontSize
        {
            get { return _currentfontsize; }
            set
            {
                _currentfontsize = value;
                OnPropertyChanged();
            }
        }

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
            _currentfontsize = "10";
            _visibilityshipinfoonscreen = Visibility.Hidden;
            _visibilitybuttoncalculateshiptostellarobject = Visibility.Hidden;
            _3dsettings = new _3DSettings();
            IsGameDataDrawn = false;
            _drawshipsonscreen = false;
            _showcentralhubelements = true;
            _showelements = false;
            _showmodifiers = false;
            _showelementgroups = false;
            _shownaturalsatellitesinfo = false;
            _showorbitalbodyinfo = false;
            _showbuildings = false;
            _displayelementsymbol = false;
            DeltaRotationAngle = 0.05;
        }
        public bool DisplayShipInfoonScreen
        {
            get
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
        public bool DisplayElementSymbol
        {
            get { return _displayelementsymbol; }
            set 
            { 
                _displayelementsymbol = value;
                OnPropertyChanged();
            }
        }

        public bool IsGameDataDrawn { get; set; }
        public bool GamePaused { get; set; }
        public double DeltaRotationAngle { get; set; }
        public _3DSettings _3DSettings
        {
            get { return _3dsettings; }
            set { _3dsettings = value; }
        }
        public bool DrawShips
        {
            get { return _drawshipsonscreen; }
            set
            {
                _drawshipsonscreen = value;
                OnPropertyChanged();
            }
        }
        public bool DrawStarlanes
        {
            get { return _drawstarlanes; }
            set
            {
                _drawstarlanes = value;
                OnPropertyChanged();
            }
        }
        public bool ShowNaturalSatellitesInfo
        {
            get { return _shownaturalsatellitesinfo; }
            set
            {
                _shownaturalsatellitesinfo = value;
                OnPropertyChanged();

            }
        }
        public bool ShowOrbitalBodyInfo
        {
            get { return _showorbitalbodyinfo; }
            set
            {
                _showorbitalbodyinfo = value;
                OnPropertyChanged();

            }
        }
        public bool ShowElementGroups
        {
            get { return _showelementgroups; }
            set
            {
                _showelementgroups = value;
                OnPropertyChanged();

            }
        }
        public bool ShowCentralHubElements
        {
            get { return _showcentralhubelements; }
            set
            {
                _showcentralhubelements = value;
                OnPropertyChanged();

            }
        }
        public bool ShowElements
        {
            get { return _showelements; }
            set 
            { 
                _showelements = value;
                OnPropertyChanged();
                
            }
        }
        public bool ShowBuildings
        {
            get { return _showbuildings; }
            set
            {
                _showbuildings = value;
                OnPropertyChanged();

            }
        }
        public bool ShowModifiers
        {
            get { return _showmodifiers; }
            set
            {
                _showmodifiers = value;
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

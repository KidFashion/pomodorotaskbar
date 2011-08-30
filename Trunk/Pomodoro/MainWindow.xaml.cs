using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;
using Pomodoro.Properties;

using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;

namespace Pomodoro
{

    public partial class MainWindow : Window
    {
        private Doing _doing;
        private int _seconds;
        private readonly DispatcherTimer _timer;
        private Storyboard _shakeStoryBoard;
        private int _rings;
        private Icon _starticon;
        private Icon _stopicon;
        private ThumbnailToolBarButton _thumbnailToolBarButton;

        public MainWindow()
        {
            InitializeComponent();
    
            _doing = Doing.Stopped;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += TimerTick;
            _seconds = Settings.Default.SessionLength * 60;
            MinutesTextBox.Text = Minutes + " " + Properties.Resources.MainWindow_TimerTick_minutes;

            MinutesTextBox.GotFocus += OnMinutesTextBoxOnGotFocus;
            MinutesTextBox.LostFocus += OnMinutesTextBoxOnLostFocus;
            MinutesTextBox.KeyDown += MinutesTextBoxOnKeyDown;

            Player.MediaEnded += (s, e) => RepeatRinging();
        }

        private void RepeatRinging()
        {
            if(_rings++ < 5)
            {
                Debug.Print("Repeat...");
                Player.Stop();
                Player.Position = new TimeSpan(0,0,0);
                Player.Play();
            }
            else
            {
                _rings = 0;
                StopRinging();
            }
        }

        private void OnMinutesTextBoxOnLostFocus(object sender, RoutedEventArgs e)
        {
            SetNewSessionLength();
            MinutesTextBox.Text = Minutes + " " + Properties.Resources.MainWindow_TimerTick_minutes;
        }

        private void OnMinutesTextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            if (MinutesTextBox.IsReadOnly) return;
            MinutesTextBox.Text = Minutes.ToString();
        }

        private void MinutesTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            SetNewSessionLength();
            ActionButton.Focus();
        }

        private void SetNewSessionLength()
        {
            if (MinutesTextBox.IsReadOnly) return;
            try
            {
                Settings.Default.SessionLength = Int32.Parse(MinutesTextBox.Text);
                Settings.Default.Save();
            }
            catch (Exception)
            {
                MessageBox.Show("Too much input, Number 5!");
            }
            _seconds = Settings.Default.SessionLength * 60;
        }

        public int Minutes
        {
            get { return (int)Math.Ceiling((decimal)_seconds / 60); }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _seconds--;
            MinutesTextBox.Text = Minutes + " " + Properties.Resources.MainWindow_TimerTick_minutes;

            TaskbarItemInfo.Overlay = CreateTaskbarIconOverlayImage(Minutes.ToString());

            //            var preview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(new WindowInteropHelper(this).Handle);
            //            if(preview == null)
            //            {
            //                preview = new TabbedThumbnail(new WindowInteropHelper(this).Handle,
            //                                              new WindowInteropHelper(this).Handle);
            //                TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(preview);
            //            }
            //            
            //            var image = PomodoroImage;
            //            preview.SetImage(image);

            if (_seconds == 0)
            {
                RingAndFlash();
            }
        }

        private void RingAndFlash()
        {
            this.FlashWindow(5);
            _shakeStoryBoard.Begin(this, true);
            _doing = Doing.Ringing;
            ActionButtonText.Text = Properties.Resources.MainWindow_TimerTick_Stop_Ringing_;
            _timer.Stop();
            Player.Play();
        }

        private static DrawingImage CreateTaskbarIconOverlayImage(string text)
        {
            var group = new DrawingGroup();
            using (var dc = @group.Open())
            {
                dc.DrawText(new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                              new Typeface(new FontFamily("Verdana"), FontStyles.Normal, FontWeights.ExtraBold,
                                                           FontStretches.Normal), 12, Brushes.DimGray), new Point(1, 1));
                dc.DrawText(new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                              new Typeface(new FontFamily("Verdana"), FontStyles.Normal, FontWeights.ExtraBold,
                                                           FontStretches.Normal), 12, Brushes.WhiteSmoke), new Point(0, 0));
            }
            var drawingImage = new DrawingImage { Drawing = @group };
            return drawingImage;
        }



        public bool ProcessCommandLineArgs(string command)
        {
            switch (command)
            {
                case "/restart":
                    RestartTimer();
                    break;
                case "/start":
                    StartTimer();
                    break;
                case "/stop":
                    StopTimer();
                    break;
                case "/stopringing":
                    StopRinging();
                    break;
            }

            return true;
        }

        private void ActionButtonClicked(object sender, RoutedEventArgs e)
        {
            HandleActionButtons();
        }

        private void HandleActionButtons()
        {
            switch (_doing)
            {
                case Doing.Done: //restart
                    RestartTimer();
                    break;
                case Doing.Stopped: //start
                    StartTimer();
                    break;
                case Doing.Started:
                    StopTimer();
                    break;
                case Doing.Ringing:
                    StopRinging();
                    break;
            }
        }

        private void RestartTimer()
        {
            _seconds = Settings.Default.SessionLength * 60;
            StartTimer();
        }

        private void StartTimer()
        {
            MinutesTextBox.IsReadOnly = true;
            ActionButtonText.Text = Properties.Resources.MainWindow_CreateJumpList_Stop_Timer;
            _doing = Doing.Started;
            WindowState = WindowState.Minimized;
            _thumbnailToolBarButton.Icon = _stopicon;
            _thumbnailToolBarButton.Tooltip = "Stop";

            _timer.Start();
        }

        private void StopRinging()
        {
            _thumbnailToolBarButton.Icon = _starticon;
            _thumbnailToolBarButton.Tooltip = "Start";

            _shakeStoryBoard.Stop(this);
            MinutesTextBox.IsReadOnly = false;
            Player.Stop();
            ActionButtonText.Text = Properties.Resources.MainWindow_CreateJumpList_Start_New_Session;
            _doing = Doing.Done;
        }

        private void StopTimer()
        {
            if(_doing == Doing.Ringing)
            {
                StopRinging();
                return;
            }

            _thumbnailToolBarButton.Icon = _starticon;
            _thumbnailToolBarButton.Tooltip = "Start";
            
            MinutesTextBox.IsReadOnly = false;
            _timer.Stop();

            TaskbarItemInfo.Overlay = CreateTaskbarIconOverlayImage("(" + Minutes + ")");
            ActionButtonText.Text = Properties.Resources.MainWindow_CreateJumpList_Start_Timer;
            _doing = Doing.Stopped;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            JumplistBuilder.CreateJumpList();
            CreatePreviewButtons();

            _shakeStoryBoard = (Storyboard)FindResource("ShakeTomato");

            ProcessCommandLineArgs(App.Command);
        }

        public void CreatePreviewButtons()
        {
            _starticon = new Icon(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/resources/start.ico");
            _stopicon = new Icon(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/resources/stop.ico");

            _thumbnailToolBarButton = new ThumbnailToolBarButton(
                _starticon, "Start");

            _thumbnailToolBarButton.Click += ToolbarThumbnailToolBarButtonClick;
            TaskbarManager.Instance.ThumbnailToolBars.AddButtons(
                new WindowInteropHelper(Application.Current.MainWindow).Handle,
                _thumbnailToolBarButton);
        }

        private void ToolbarThumbnailToolBarButtonClick(object sender, ThumbnailButtonClickedEventArgs e)
        {

            HandleActionButtons();
            return;
            var button = sender as ThumbnailToolBarButton;
            if (button == null) return;
            if (button.Tooltip == "Start")
                StartTimer();
            else
                StopTimer();
        }
    }
}

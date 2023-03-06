using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;

namespace BeeBurn.XAML
{

    public enum PlayOptions { StopAtEnd, RepeatThisList }

    /// <summary>
    /// Interaction logic for PresenterView.xaml
    /// </summary>
        public partial class PresenterView : Window
    {
        private Projection m_proj = null;
        private DispatcherTimer m_dispatcherTimer = null;
        private DateTime m_timerStart;
        private readonly double m_panSeconds = BeeBurnVM.Get().ConfigSettings.ImagePanTime;
        private bool m_paused = true;

        public PresenterView()
        {
            InitializeComponent();
            DataContext = BeeBurnVM.Get();
        }

        public void Reload()
        {
            this.InvalidateVisual();
        }

        private void PresenterLoaded(object sender, RoutedEventArgs e)
        {
            EnsureProjectionWindow();
            m_proj.Show();
            RefreshNextImageControl();
            this.Activate();
        }

        public PlayOptions PlayOption
        {
            get
            {
                return (PlayOptions)GetValue(PlayOptionProperty);
            }
            set
            {
                SetValue(PlayOptionProperty, value);
                RefreshNextImageControl();
            }
        }

        public double PanSeconds => m_panSeconds;

        public static readonly DependencyProperty PlayOptionProperty =
         DependencyProperty.Register("PlayOption",
             typeof(PlayOptions),
             typeof(PresenterView),
             new PropertyMetadata(PlayOptions.RepeatThisList, null));

        public delegate void CloseHandler();

        public event CloseHandler OnClose;

        private void EnsureProjectionWindow()
        {
            if (m_proj == null)
            {
                m_proj = new Projection();

                int iScreen = BeeBurnVM.Get().ConfigSettings.ScreenIndex;
                if (Screen.AllScreens.Length <= iScreen)
                    iScreen = Screen.AllScreens.Length - 1;

                if (BeeBurnVM.Get().ConfigSettings.FullScreen)
                {
                    m_proj.Top = Screen.AllScreens[iScreen].WorkingArea.Top;
                    m_proj.Left = Screen.AllScreens[iScreen].WorkingArea.Left;
                    m_proj.Width = Screen.AllScreens[iScreen].WorkingArea.Width;
                    m_proj.Height = Screen.AllScreens[iScreen].WorkingArea.Height;
                }
                else
                {
                    int top = Screen.AllScreens[iScreen].WorkingArea.Top;
                    int left = Screen.AllScreens[iScreen].WorkingArea.Left;
                    int widthT = Screen.AllScreens[iScreen].WorkingArea.Width;
                    int heightT = Screen.AllScreens[iScreen].WorkingArea.Height;
                    int widthW = BeeBurnVM.Get().ConfigSettings.WindowWidth;
                    int heightW = BeeBurnVM.Get().ConfigSettings.WindowHeight;

                    m_proj.Top = Math.Max(top + 5, top + ((heightT - heightW) / 2));
                    m_proj.Left = Math.Max(left + 5, left + ((widthT - widthW) / 2));
                    m_proj.Width = widthW;
                    m_proj.Height = heightW;

                }
                // m_proj.WindowState = WindowState.Maximized;

                Console.WriteLine("Left = " + m_proj.Left);
                Console.WriteLine("Top = " + m_proj.Top);
                Console.WriteLine("Width = " + m_proj.Width);

                m_proj.OnClose += () => { m_proj = null; };
            }
        }
        private void PresenterClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Paused = true;
            m_proj?.Close();
            OnClose?.Invoke();
        }

        private BeePresentationStack PresentationStack => BeeBurnVM.Get().PresentationStack;

        private void RefreshNextImageControl()
        {
            NextImage.Source = PresentationStack.NextImage.Image;
        }

        private void DisplayCurrentImage()
        {
            EnsureProjectionWindow();
            
            CurrentImage.Source = PresentationStack.CurrentImage.Image;
            RefreshNextImageControl();

            double panSeconds = m_proj.QueueCurrentImage() + BeeBurnVM.Get().ConfigSettings.ImageFadeTime;

            //  DispatcherTimer setup
            if (m_dispatcherTimer == null)
            {
                m_dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                m_dispatcherTimer.Tick += new EventHandler(ProjectionTimerTick);
            }

            m_dispatcherTimer.Tag = panSeconds;

            m_proj.UpdateProgress(0);
            m_timerStart = DateTime.Now;
            m_dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            m_dispatcherTimer.Start();

            if (PresentationStack.CurrentImage.IsStopImage)
            {
                Paused = true;
            }
        }

        private void ProjectionTimerTick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - m_timerStart;
            double panSeconds = (double)m_dispatcherTimer.Tag;
            double pctComplete = Math.Min(1.0, elapsed.TotalSeconds / panSeconds);
            
            m_proj?.UpdateProgress(pctComplete);

            if (pctComplete >= 1.0)
            {
                m_dispatcherTimer.Stop();
                m_dispatcherTimer = null;
                if (!Paused)
                {
                    PresentationStack.CycleNextImage();
                    DisplayCurrentImage();
                }
            }
        }

        private bool Paused
        {
            get
            {
                return m_paused;
            }
            set
            {
                if (m_paused != value)
                {
                    m_paused = value;
                    if (m_paused)
                    {
                        PlayPauseButton.Content = "Play Next Image";
                    }
                    else
                    {
                        PlayPauseButton.Content = "Fade After Current Image";
                    }
                }
            }
        }

        private void ClickAddStop(object sender, EventArgs e)
        {
            PresentationStack.Images.Add(BeeImage.CreateStopImage());
        }


        private void ClickPlayOrFade(object sender, RoutedEventArgs e)
        {
            m_proj.Show();
            if (Paused)
            {
                PresentationStack.CycleNextImage();
                Paused = false;
                EnsureProjectionWindow();
                DisplayCurrentImage();
            }
            else
            {
                PresentationStack.EnsureStopImageAfterCurrent();
            }
        }

        private void ClickCutToBlack(object sender, RoutedEventArgs e)
        {
            if (PresentationStack.CurrentImage.IsStopImage)
                return;

            PresentationStack.EnsureStopImageAfterCurrent();
            PresentationStack.CycleNextImage();
            DisplayCurrentImage();
        }

        private void ClickSkip(object sender, RoutedEventArgs e)
        {
            PresentationStack.CycleNextImage();
            DisplayCurrentImage();
        }

        private void ClickPauseAfterCurrent(object sender, RoutedEventArgs e)
        {
            Paused = true;
        }


        private void ClickPasteNext(object sender, RoutedEventArgs e)
        {
            PresentationStack.PasteNextImage();
        }

        private void ClickLoadNext(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.LoadImagesToStack(PresentationStack, 1);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}

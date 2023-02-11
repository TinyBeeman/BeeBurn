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

    public enum PlayOptions { StopAtEnd, NextList, RepeatThisList }

    /// <summary>
    /// Interaction logic for PresenterView.xaml
    /// </summary>
        public partial class PresenterView : Window
    {
        private Projection m_proj = null;
        private DispatcherTimer m_dispatcherTimer = null;
        private DateTime m_timerStart;
        private readonly double m_panSeconds = BeeBurnVM.Get().ConfigSettings.ImagePanTime;
        private bool m_fadeToBlack = true;
        private bool m_paused = true;

        public PresenterView()
        {
            InitializeComponent();
            DataContext = BeeBurnVM.Get();
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
             new PropertyMetadata(PlayOptions.NextList, null));

        public delegate void CloseHandler();

        public event CloseHandler OnClose;

        private void EnsureProjectionWindow()
        {
            if (m_proj == null)
            {
                m_proj = new Projection();

                foreach (Screen s in Screen.AllScreens)
                {
                    Console.WriteLine(s.DeviceName);
                }
                m_proj.Top = Screen.AllScreens[1].WorkingArea.Top;
                m_proj.Left = Screen.AllScreens[1].WorkingArea.Left;
                m_proj.Width = Screen.AllScreens[1].WorkingArea.Width;
                m_proj.Height = Screen.AllScreens[1].WorkingArea.Height;
                // m_proj.WindowState = WindowState.Maximized;

                Console.WriteLine("Left = " + m_proj.Left);
                Console.WriteLine("Top = " + m_proj.Top);
                Console.WriteLine("Width = " + m_proj.Width);

                m_proj.OnClose += () => { m_proj = null; };
            }
        }
        private void PresenterClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PauseProjection(false);
            m_proj?.Close();
            OnClose?.Invoke();
        }

        private void StartProjection()
        {
            m_paused = false;
            EnsureProjectionWindow();
            QueueNextImage();
        }

        private BeeImage GetNextImage(bool advancePointers)
        {
            BeeStack nextStack = BeeBurnVM.Get().EnsureActiveStack();
            BeeImage biNext;
            if (advancePointers)
                biNext  = nextStack.GetNextImage(PlayOption == PlayOptions.RepeatThisList);
            else
                biNext = nextStack.PeekNextImage(PlayOption == PlayOptions.RepeatThisList);

            if (biNext == null && PlayOption == PlayOptions.NextList)
            {
                nextStack = BeeBurnVM.Get().GetNextStack(true, advancePointers);

                if (nextStack != null)
                {
                    if (advancePointers)
                        return nextStack.GetNextImage(PlayOption == PlayOptions.RepeatThisList);
                    else
                        return nextStack.PeekNextImage(PlayOption == PlayOptions.RepeatThisList);
                }
            }
            return biNext;
        }

        private void RefreshNextImageControl()
        {
            BeeImage biNext = GetNextImage(false);
            NextImage.Source = biNext?.BitmapFrame;
        }

        private void QueueNextImage()
        {
            EnsureProjectionWindow();
            BeeStack activeStack = BeeBurnVM.Get().EnsureActiveStack();
            BeeImage biNext = GetNextImage(true);
            if (biNext == null)
            {
                PauseProjection(true);
                m_proj.FadeToBlack();
                return;
            }

            CurrentImage.Source = biNext.BitmapFrame;
            RefreshNextImageControl();

            double panSeconds = m_proj.QueueImage(biNext) + BeeBurnVM.Get().ConfigSettings.ImageFadeTime;

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
                if (!m_paused)
                {
                    QueueNextImage();
                }
                else
                {
                    if (m_fadeToBlack)
                        m_proj?.FadeToBlack();
                }
            }
        }

        private void PauseProjection(bool fadeToBlack)
        {
            m_fadeToBlack = fadeToBlack;
            m_paused = true;
        }

        private void ClickPlayPause(object sender, RoutedEventArgs e)
        {
            m_proj.Show();
            if (m_paused)
            {
                StartProjection();
                PlayPauseButton.Content = "Pause";
            }
            else
            {
                PauseProjection(true);
                PlayPauseButton.Content = "Play";
            }
        }

        private void ClickSkip(object sender, RoutedEventArgs e)
        {
            QueueNextImage();
        }

        private void ClickHold(object sender, RoutedEventArgs e)
        {
            PauseProjection(false);
            PlayPauseButton.Content = "Play";
        }


        private void ClickPasteNext(object sender, RoutedEventArgs e)
        {

        }

        private void ClickLoadNext(object sender, RoutedEventArgs e)
        {

        }
    }
}

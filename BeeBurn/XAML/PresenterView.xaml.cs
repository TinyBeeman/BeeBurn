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
        private double m_panSeconds = BeeBurnVM.Get().GetConfigDouble(ConfigKey.ImagePanTime) ?? 30;
        
        private bool m_paused = true;

        public PresenterView()
        {
            InitializeComponent();
            DataContext = BeeBurnVM.Get();

            EnsureProjectionWindow();
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
            }
        }

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
                m_proj.OnClose += () => { m_proj = null; };
            }
        }
        private void PresenterClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PauseProjection();
            m_proj?.Close();
            OnClose?.Invoke();
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
                PauseProjection();
                PlayPauseButton.Content = "Play";
            }
        }

        

        private void StartProjection()
        {
            m_paused = false;
            EnsureProjectionWindow();
            QueueNextImage();
        }

        private void QueueNextImage()
        {
            EnsureProjectionWindow();
            BeeStack activeStack = BeeBurnVM.Get().EnsureActiveStack();
            BeeImage biNext = activeStack.GetNextImage(PlayOption == PlayOptions.RepeatThisList);
            
            if (biNext == null)
            {
                switch (PlayOption)
                {
                    case PlayOptions.NextList:
                        activeStack = BeeBurnVM.Get().ActivateNextStack(true);
                        if (activeStack == null)
                        {
                            PauseProjection();
                            m_proj.FadeToBlack();
                            return;
                        }
                        QueueNextImage();
                        return;
                    case PlayOptions.RepeatThisList:
                        // This list must be empty, so fade out, in case any image is showing.
                        PauseProjection();
                        m_proj.FadeToBlack();
                        return;
                    case PlayOptions.StopAtEnd:
                        // We are done... fade us out.
                        PauseProjection();
                        m_proj.FadeToBlack();
                        return;
                }
            }

            double panSeconds = m_proj.QueueImage(biNext) + BeeBurnVM.Get().GetConfigDouble(ConfigKey.ImageFadeTime) ?? 2;

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
                    QueueNextImage();
                else
                    m_proj?.FadeToBlack();
            }
        }

        private void PauseProjection()
        {
            m_paused = true;
        }

        private void ClickSkip(object sender, RoutedEventArgs e)
        {
            QueueNextImage();
        }

        private void ClickPasteNext(object sender, RoutedEventArgs e)
        {

        }

        private void ClickLoadNext(object sender, RoutedEventArgs e)
        {

        }
    }
}

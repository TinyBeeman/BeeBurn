using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BeeBurn.XAML
{

    /// <summary>
    /// Interaction logic for Projection.xaml
    /// </summary>
    public partial class Projection : Window
    {
        private ObservableCollection<BeeImage> m_imgList;

        private Image m_imgOld;
        private Image m_imgNew;

        private DateTime m_timerStart;
        private DispatcherTimer m_dispatcherTimer;
        private double m_fadeSeconds = BeeBurnVM.Get().GetConfigDouble(ConfigKey.ImageFadeTime) ?? 2;
        private double m_panSeconds = BeeBurnVM.Get().GetConfigDouble(ConfigKey.ImagePanTime) ?? 30;

        private Storyboard m_currentStoryboard = null;


        public Projection()
        {
            InitializeComponent();
        }

        public void ProjectList(ObservableCollection<BeeImage> imgList)
        {
            m_imgList = imgList;

            QueueNextImage();
        }

        private void ProjectionTimerTick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - m_timerStart;
            double pctComplete = Math.Min(1.0, elapsed.TotalSeconds / m_panSeconds);

            Progress.PercentComplete = pctComplete;

            if (pctComplete >= 1.0)
            {
                m_dispatcherTimer.Stop();
                m_dispatcherTimer = null;
                QueueNextImage();
            }
        }

        public void QueueNextImage()
        {
            
            if (m_imgOld != null)
                GridImage.Children.Remove(m_imgOld);

            m_currentStoryboard = new Storyboard();
                
            BeeImage bi = m_imgList[0];
            m_imgList.Remove(bi);
            m_imgList.Add(bi);
            m_imgOld = m_imgNew;
            m_imgNew = new Image();
            m_imgNew.Source = bi.BitmapFrame;

            // Start with 0 opacity
            m_imgNew.Opacity = 0;
            m_imgNew.Stretch = Stretch.None;

            GridImage.Children.Add(m_imgNew);

            DoubleAnimation animFadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(m_fadeSeconds));


            ScaleTransform scaleTransform = new ScaleTransform(1, 1);

            m_imgNew.RenderTransformOrigin = new Point(0, 0);
            m_imgNew.RenderTransform = scaleTransform;

            OffsetScale os1 = bi.GetStartOffsetScale(new BeeRect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));
            OffsetScale os2 = bi.GetEndOffsetScale(new BeeRect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));

            TimeSpan tsAnim = TimeSpan.FromSeconds(m_panSeconds + m_fadeSeconds);
            
            m_currentStoryboard.Duration = tsAnim;

            DoubleAnimation animScaleX = new DoubleAnimation(os1.Scale, os2.Scale, tsAnim);
            DoubleAnimation animScaleY = new DoubleAnimation(os1.Scale, os2.Scale, tsAnim);
            DoubleAnimation animOffsetX = new DoubleAnimation(-os1.OffsetX, -os2.OffsetX, tsAnim);
            DoubleAnimation animOffsetY = new DoubleAnimation(-os1.OffsetY, -os2.OffsetY, tsAnim);

            Storyboard.SetTarget(animScaleX, m_imgNew);
            Storyboard.SetTargetProperty(animScaleX, new PropertyPath("RenderTransform.ScaleX"));
            m_currentStoryboard.Children.Add(animScaleX);
            
            Storyboard.SetTarget(animScaleY, m_imgNew);
            Storyboard.SetTargetProperty(animScaleY, new PropertyPath("RenderTransform.ScaleY"));
            m_currentStoryboard.Children.Add(animScaleY);

            Storyboard.SetTarget(animOffsetX, m_imgNew);
            Storyboard.SetTargetProperty(animOffsetX, new PropertyPath("RenderTransform.CenterX"));
            m_currentStoryboard.Children.Add(animOffsetX);

            Storyboard.SetTarget(animOffsetY, m_imgNew);
            Storyboard.SetTargetProperty(animOffsetY, new PropertyPath("RenderTransform.CenterY"));
            m_currentStoryboard.Children.Add(animOffsetY);

            if (m_imgOld != null)
            {
                DoubleAnimation animFadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(m_fadeSeconds));
                Storyboard.SetTarget(animFadeOut, m_imgOld);
                Storyboard.SetTargetProperty(animFadeOut, new PropertyPath("Opacity"));
                m_currentStoryboard.Children.Add(animFadeOut);
            }

            
            Storyboard.SetTarget(animFadeIn, m_imgNew);
            Storyboard.SetTargetProperty(animFadeIn, new PropertyPath("Opacity"));
            m_currentStoryboard.Children.Add(animFadeIn);
            m_currentStoryboard.Duration = new Duration(tsAnim);

            m_currentStoryboard.Begin();

            //  DispatcherTimer setup
            if (m_dispatcherTimer == null)
            {
                m_dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                m_dispatcherTimer.Tick += new EventHandler(ProjectionTimerTick);
            }

            Progress.PercentComplete = 0;
            m_timerStart = DateTime.Now;
            m_dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            m_dispatcherTimer.Start();

            
        }
        

        public void ReplaceImage(Image img)
        {
            if (m_imgOld != null)
                GridImage.Children.Remove(m_imgOld);

            m_imgOld = m_imgNew;
            m_imgNew = img;
            m_imgNew.Opacity = 0;

            DoubleAnimation animShow = new DoubleAnimation(1, TimeSpan.FromSeconds(2));
            
            
            GridImage.Children.Add(m_imgNew);
            if (m_imgOld != null)
            {
                DoubleAnimation animHide = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
                m_imgOld.BeginAnimation(Canvas.OpacityProperty, animHide);
            }
            m_imgNew.BeginAnimation(Canvas.OpacityProperty, animShow);
        }

        public void PasteImage()
        {
            Image imgNew = new Image();

            imgNew.Source = BeeClipboard.BitmapFrameFromClipboardDib();
            ReplaceImage(imgNew);
        }

        public delegate void CloseHandler();

        public event CloseHandler OnClose;


        private void ProjectionClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_dispatcherTimer.Stop();
            OnClose?.Invoke();
        }
    }
}

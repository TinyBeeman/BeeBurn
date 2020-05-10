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

namespace BeeBurn
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
        private static double s_fadeSeconds = 2;
        private static double s_panSeconds = 30;



        public Projection()
        {
            InitializeComponent();
        }

        public void ProjectList(ObservableCollection<BeeImage> imgList)
        {
            m_imgList = imgList;

            QueueNextImage();
        }

        private void NextImageTimerTick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - m_timerStart;
            double pctComplete = Math.Min(1.0, elapsed.TotalSeconds / s_panSeconds);

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

            DoubleAnimation animFadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(s_fadeSeconds));
            DoubleAnimation animFadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(s_fadeSeconds));

            ScaleTransform scale = new ScaleTransform(1, 1);
            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);

            m_imgNew.RenderTransformOrigin = new Point(0, 0);
            m_imgNew.RenderTransform = group;

            OffsetScale os1 = bi.GetStartOffsetScale(new Rect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));
            OffsetScale os2 = bi.GetEndOffsetScale(new Rect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));

            DoubleAnimation animScaleX = new DoubleAnimation(os1.Scale, os2.Scale, TimeSpan.FromSeconds(s_panSeconds));
            DoubleAnimation animScaleY = new DoubleAnimation(os1.Scale, os2.Scale, TimeSpan.FromSeconds(s_panSeconds));
            DoubleAnimation animOffsetX = new DoubleAnimation(-os1.OffsetX, -os2.OffsetX, TimeSpan.FromSeconds(s_panSeconds));
            DoubleAnimation animOffsetY = new DoubleAnimation(-os1.OffsetY, -os2.OffsetY, TimeSpan.FromSeconds(s_panSeconds));

            m_imgNew.BeginAnimation(Canvas.OpacityProperty, animFadeIn);
            if (m_imgOld != null)
                m_imgOld.BeginAnimation(Canvas.OpacityProperty, animFadeOut);
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, animScaleX);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, animScaleY);
            scale.BeginAnimation(ScaleTransform.CenterXProperty, animOffsetX);
            scale.BeginAnimation(ScaleTransform.CenterYProperty, animOffsetY);

            //  DispatcherTimer setup
            if (m_dispatcherTimer == null)
            {
                m_dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                m_dispatcherTimer.Tick += new EventHandler(NextImageTimerTick);
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

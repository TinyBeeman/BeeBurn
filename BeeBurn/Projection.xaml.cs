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
        private int m_index;
        private DispatcherTimer m_dispatcherTimer;

        public Projection()
        {
            InitializeComponent();
        }

        public void ProjectList(ObservableCollection<BeeImage> imgList)
        {
            m_imgList = imgList;
            m_index = 0;

            QueueNextImage();
        }

        private void NextImageTimerTick(object sender, EventArgs e)
        {
            QueueNextImage();
        }

        public void QueueNextImage()
        {
            if (m_index < m_imgList.Count)
            {
                if (m_imgOld != null)
                    GridImage.Children.Remove(m_imgOld);

                BeeImage bi = m_imgList[m_index++];
                m_imgOld = m_imgNew;
                m_imgNew = new Image();
                m_imgNew.Source = bi.ImageSource;

                // Start with 0 opacity
                m_imgNew.Opacity = 0;
                m_imgNew.Stretch = Stretch.None;

                GridImage.Children.Add(m_imgNew);

                DoubleAnimation animFadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(2));

                ScaleTransform scale = new ScaleTransform(1, 1);
                m_imgNew.RenderTransformOrigin = new Point(0.5, 0.5);
                m_imgNew.RenderTransform = scale;

                OffsetScale os1 = bi.GetStartOffsetScale(new Rect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));
                OffsetScale os2 = bi.GetEndOffsetScale(new Rect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));

                DoubleAnimation animScaleX = new DoubleAnimation(os1.Scale, os2.Scale, TimeSpan.FromSeconds(10));
                DoubleAnimation animScaleY = new DoubleAnimation(os1.Scale, os2.Scale, TimeSpan.FromSeconds(10));
                PointAnimation animOrigin = new PointAnimation(new Point(os1.OriginX, os1.OriginY), new Point(os2.OriginX, os2.OriginY), TimeSpan.FromSeconds(10));
                //DoubleAnimation animOriginX = new DoubleAnimation(os1.OffsetX, os2.OffsetX, TimeSpan.FromSeconds(10));
                //DoubleAnimation animOriginY = new DoubleAnimation(os1.OffsetY, os2.OffsetY, TimeSpan.FromSeconds(10));

                m_imgNew.BeginAnimation(Canvas.OpacityProperty, animFadeIn);
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, animScaleX);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, animScaleY);
                m_imgNew.BeginAnimation(Canvas.RenderTransformOriginProperty, animOrigin);                

            }

            //  DispatcherTimer setup
            m_dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            m_dispatcherTimer.Tick += new EventHandler(NextImageTimerTick);
            m_dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
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

            imgNew.Source = BeeClipboard.ImageFromClipboardDib();
            ReplaceImage(imgNew);
        }

    }
}

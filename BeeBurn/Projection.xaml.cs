using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BeeBurn
{

    /// <summary>
    /// Interaction logic for Projection.xaml
    /// </summary>
    public partial class Projection : Window
    {
        private Image m_imgOld;
        private Image m_imgNew;

        public Projection()
        {
            InitializeComponent();
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

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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeeBurn
{

    public enum DragState
    {
        None,
        LeftStarted,
        RightStarted
    }

    /// <summary>
    /// Interaction logic for BeeImageEditor.xaml
    /// </summary>
    public partial class BeeImageEditor : UserControl
    {
        public BeeImageEditor()
        {
            InitializeComponent();
            m_dragState = DragState.None;
        }


        public BeeImage BeeImg
        {
            get
            {
                return (BeeImage)GetValue(BeeImgProperty);
            }
            set
            {
                SetValue(BeeImgProperty, value);
            }
        }

        public static readonly DependencyProperty BeeImgProperty =
                DependencyProperty.Register("BeeImg",
                    typeof(BeeImage),
                    typeof(BeeImageEditor),
                    new PropertyMetadata(null));



        private DragState m_dragState;

        private void OnImageMouseMove(object sender, MouseEventArgs e)
        {
            if (m_dragState == DragState.None)
                return;

            Point ptMouse = e.GetPosition((IInputElement)sender);

            BeeRect r = (m_dragState == DragState.LeftStarted) ? BeeImg.StartRect : BeeImg.EndRect;

            double deltaX = (ptMouse.X - r.Left);
            double deltaY = (ptMouse.Y - r.Top);
            if (deltaX > 0)
            {
                r.Width = deltaX;
            }
            else
            {
                r.Left += deltaX;
                r.Width -= deltaX;
            }
            
            if (deltaY > 0)
            {
                r.Height = deltaY;
            }
            else
            {
                r.Top += deltaY;
                r.Height -= deltaY;
            }
        }

        private void OnImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point ptMouse = e.GetPosition((IInputElement)sender);
            
            if (e.ChangedButton == MouseButton.Left)
            {
                m_dragState = DragState.LeftStarted;

                BeeImg.StartRect.Left = ptMouse.X;
                BeeImg.StartRect.Top = ptMouse.Y;
                BeeImg.StartRect.Width = 1;
                BeeImg.StartRect.Height = 1;
            }
            else
            {
                m_dragState = DragState.RightStarted;

                BeeImg.EndRect.Left = ptMouse.X;
                BeeImg.EndRect.Top = ptMouse.Y;
                BeeImg.EndRect.Width = 1;
                BeeImg.EndRect.Height = 1;
            }
        }

        private void OnImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            m_dragState = DragState.None;
        }

        private void OnImageMouseLeave(object sender, MouseEventArgs e)
        {
            m_dragState = DragState.None;
        }
    }
}

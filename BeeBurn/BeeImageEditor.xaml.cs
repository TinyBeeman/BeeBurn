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
            Point ptMouse = e.GetPosition((IInputElement)sender);

            if (m_dragState == DragState.LeftStarted)
            {
                BeeImg.StartRect.Width = ptMouse.X - BeeImg.StartRect.Left;
                BeeImg.StartRect.Height = ptMouse.Y - BeeImg.StartRect.Top;
            }
            else if (m_dragState == DragState.RightStarted)
            {
                BeeImg.EndRect.Width = ptMouse.X - BeeImg.EndRect.Left;
                BeeImg.EndRect.Height = ptMouse.Y - BeeImg.EndRect.Top; ;
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
                BeeImg.StartRect.Width = 100;
                BeeImg.StartRect.Height = 100;
            }
            else
            {
                m_dragState = DragState.RightStarted;

                BeeImg.EndRect.Left = ptMouse.X;
                BeeImg.EndRect.Top = ptMouse.Y;
                BeeImg.EndRect.Width = 100;
                BeeImg.EndRect.Height = 100;
            }
        }

        private void OnImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point ptMouse = e.GetPosition((IInputElement)sender);

            if (e.ChangedButton == MouseButton.Left)
            {
                BeeImg.StartRect.Width = ptMouse.X - BeeImg.StartRect.Left;
                BeeImg.StartRect.Height = ptMouse.Y - BeeImg.StartRect.Top;
                m_dragState = DragState.None;
            }
            else
            {
                BeeImg.EndRect.Width = ptMouse.X - BeeImg.EndRect.Left;
                BeeImg.EndRect.Height = ptMouse.Y - BeeImg.EndRect.Top; ;
                m_dragState = DragState.None;

            }
        }


    }
}

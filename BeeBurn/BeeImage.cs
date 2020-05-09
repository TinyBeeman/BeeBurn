using System.Windows.Media;
using System.Windows.Shapes;

namespace BeeBurn
{
    public struct Rect
    {
        public double Left;
        public double Top;
        public double Width;
        public double Height;

        public double Right => Left + Width;
        public double Bottom => Top + Height;

        public Rect(double l = 0, double t = 0, double w = 0, double h = 0)
        {
            Left = l;
            Top = t;
            Width = w;
            Height = h;
        }
    }
    public class BeeImage
    {
        public ImageSource ImageSource { get; set; }
        public string Name { get; set; }

        public Rect StartRect;
        public Rect EndRect;

        public BeeImage(ImageSource src, string name)
        {
            Name = name;
            ImageSource = src;
            StartRect.Left = 0;
            StartRect.Top = 0;
            StartRect.Width = src.Width;
            StartRect.Height = src.Height;
            EndRect = StartRect;
        }

        public void ShrinkEnd(double margin)
        {
            EndRect.Left = StartRect.Left + margin;
            EndRect.Top = StartRect.Top + margin;
            EndRect.Width = StartRect.Width - margin - margin;
            EndRect.Height = StartRect.Height- margin - margin;
        }
    }
}

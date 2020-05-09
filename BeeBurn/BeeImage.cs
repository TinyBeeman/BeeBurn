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

    public struct OffsetScale
    {
        public double OffsetX;
        public double OffsetY;
        public double Scale;
        public double OriginX;
        public double OriginY;

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
            StartRect = GetImageRect(src);
            EndRect = new Rect(50, 50, 100, 100);
        }

        public void ShrinkEnd(double margin)
        {
            EndRect.Left = StartRect.Left + margin;
            EndRect.Top = StartRect.Top + margin;
            EndRect.Width = StartRect.Width - margin - margin;
            EndRect.Height = StartRect.Height- margin - margin;
        }

        public Rect GetImageRect(ImageSource src)
        {
            return new Rect(0, 0, src.Width, src.Height);
        }

        private OffsetScale GetOffsetAndScaleFromRect(Rect rFocus, Rect rContainer)
        {
            
            double aspContainer = rContainer.Width / rContainer.Height;
            double aspFocus = rFocus.Width / rFocus.Height;
            bool fitWidth = (aspFocus < aspContainer);

            OffsetScale ret;
            ret.Scale = fitWidth ? (rContainer.Height / rFocus.Height) : (rContainer.Width / rFocus.Width);
            ret.OffsetX = rFocus.Left + (rFocus.Width / 2);
            ret.OffsetY = rFocus.Top + (rFocus.Height / 2);
            ret.OriginX = ret.OffsetX / ImageSource.Width;
            ret.OriginY = ret.OffsetY / ImageSource.Height;

            return ret;
        }

        public OffsetScale GetStartOffsetScale(Rect rContainer) { return GetOffsetAndScaleFromRect(StartRect, rContainer);  }
        public OffsetScale GetEndOffsetScale(Rect rContainer) { return GetOffsetAndScaleFromRect(EndRect, rContainer); }
    }
}

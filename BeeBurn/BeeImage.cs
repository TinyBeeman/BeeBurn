using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BeeBurn
{
    public class Rect : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private double m_left;
        private double m_top;
        private double m_width;
        private double m_height;

        public double Left
        {
            get => m_left;
            set
            {
                m_left = value;
                OnPropertyChanged();
            }
        }

        public double Top
        {
            get => m_top;
            set
            {
                m_top = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => m_width;
            set
            {
                m_width = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => m_height;
            set
            {
                m_height = value;
                OnPropertyChanged();
            }
        }

        public double Right => Left + Width;
        public double Bottom => Top + Height;



        public Rect(double l = 0, double t = 0, double w = 0, double h = 0)
        {
            m_left = l;
            m_top = t;
            m_width = w;
            m_height = h;
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

    public class BeeImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void UpdateAllProps()
        {
            OnPropertyChanged("BitmapFrame");
            OnPropertyChanged("StartRect");
            OnPropertyChanged("EndRect");

        }

        private BitmapFrame m_bitmapFrame;
        private Rect m_startRect;
        private Rect m_endRect;
        private string m_name;

        public BitmapFrame BitmapFrame
        {
            get => m_bitmapFrame;
            set
            {
                m_bitmapFrame = value;
                OnPropertyChanged();
            }
        }
        public string Name {
            get => m_name;
            set
            {
                m_name = value;
                OnPropertyChanged();
            }
        }

        public Rect StartRect
        {
            get => m_startRect;
            set
            {
                m_startRect = value;
                OnPropertyChanged();
            }
        }
        public Rect EndRect
        {
            get => m_endRect;
            set
            {
                m_endRect = value;
                OnPropertyChanged();
            }
        }

        public BeeImage(BitmapFrame src, string name)
        {
            Name = name;
            BitmapFrame = src;
            StartRect = GetImageRect(src);
            EndRect = new Rect(50, 50, 100, 100);
        }

        public void ShrinkEnd(double margin)
        {
            EndRect = new Rect(StartRect.Left + margin, StartRect.Top + margin, StartRect.Width - margin - margin, StartRect.Height - margin - margin);
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
            ret.OffsetX = -(rFocus.Left + (rFocus.Width / 2));
            ret.OffsetY = -(rFocus.Top + (rFocus.Height / 2));
            ret.OriginX = ret.OffsetX / BitmapFrame.Width;
            ret.OriginY = ret.OffsetY / BitmapFrame.Height;

            return ret;
        }

        public OffsetScale GetStartOffsetScale(Rect rContainer) { return GetOffsetAndScaleFromRect(StartRect, rContainer); }
        public OffsetScale GetEndOffsetScale(Rect rContainer) { return GetOffsetAndScaleFromRect(EndRect, rContainer); }


        public bool SaveImage(string filepath)
        {
            try
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame);
                using (var fileStream = new System.IO.FileStream(filepath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }
    }
}

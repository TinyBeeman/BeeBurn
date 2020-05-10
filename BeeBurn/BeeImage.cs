using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BeeBurn
{
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
        private BeeRect m_startRect;
        private BeeRect m_endRect;
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

        public BeeRect StartRect
        {
            get => m_startRect;
            set
            {
                m_startRect = value;
                OnPropertyChanged();
            }
        }
        public BeeRect EndRect
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
            EndRect = new BeeRect(50, 50, 100, 100);
        }

        public BeeImage(string serial, string childPath)
        {
            BitmapFrame = null;
            Deserialize(serial, childPath);
        }

        public void ShrinkEnd(double margin)
        {
            EndRect = new BeeRect(StartRect.Left + margin, StartRect.Top + margin, StartRect.Width - margin - margin, StartRect.Height - margin - margin);
        }

        public BeeRect GetImageRect(ImageSource src)
        {
            return new BeeRect(0, 0, src.Width, src.Height);
        }

        private OffsetScale GetOffsetAndScaleFromRect(BeeRect rFocus, BeeRect rContainer)
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

        public OffsetScale GetStartOffsetScale(BeeRect rContainer) { return GetOffsetAndScaleFromRect(StartRect, rContainer); }
        public OffsetScale GetEndOffsetScale(BeeRect rContainer) { return GetOffsetAndScaleFromRect(EndRect, rContainer); }


        public bool SaveImage(string filepath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame);
            using (var fileStream = new System.IO.FileStream(filepath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }

            return true;

        }

        public string Serialize(int index, string childPath)
        {
            string saveName = index.ToString("D" + 4) + "-" + Name + ".png";
            try
            {
                SaveImage(childPath + "\\" + saveName);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to save image: " + e.Message);
                return null;
            }
                
            return saveName + "\n" +
                   StartRect.Left + "," +
                   StartRect.Top + "," +
                   StartRect.Width + "," +
                   StartRect.Height + ";" +
                   EndRect.Left + "," +
                   EndRect.Top + "," +
                   EndRect.Width + "," +
                   EndRect.Height + ";";
        }

        public void Deserialize(string str, string childPath)
        {
            string[] rgLines = str.Split(new char[] { '\n' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string fileName = rgLines[0];
            Name = fileName.Substring(4); // Remove 0000-
            Name = Name.Substring(0, Name.Length - 4); // remove .png

            var rgRects = rgLines[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var rgStartDims = rgRects[0].Split(new char[] { ',' }, 4, StringSplitOptions.RemoveEmptyEntries);
            var rgEndDims = rgRects[1].Split(new char[] { ',' }, 4, StringSplitOptions.RemoveEmptyEntries);
            StartRect = new BeeRect(Double.Parse(rgStartDims[0]), Double.Parse(rgStartDims[1]), Double.Parse(rgStartDims[2]), Double.Parse(rgStartDims[3]));
            EndRect = new BeeRect(Double.Parse(rgEndDims[0]), Double.Parse(rgEndDims[1]), Double.Parse(rgEndDims[2]), Double.Parse(rgEndDims[3]));

            Stream pngStream = new FileStream(childPath + "\\" + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            PngBitmapDecoder decoder = new PngBitmapDecoder(pngStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame = decoder.Frames[0];
        }
    }
}

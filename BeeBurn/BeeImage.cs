using System;
using System.Collections.Generic;
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
        private int m_sessionId;
        private static int s_nextSessionId = 0;
        private static BeeImage s_nextImage = null;
        private static BeeImage s_showingImage = null;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static void SetNextImage(BeeImage biNext)
        {
            if (s_nextImage != null)
                s_nextImage.IsNext = false;

            s_nextImage = biNext;

            if (s_nextImage != null)
                biNext.IsNext = true;
        }

        public static void SetShowingImage(BeeImage biShowing)
        {
            if (s_showingImage != null)
                s_showingImage.IsShowing = false;

            s_showingImage = biShowing;

            if (s_showingImage != null)
                biShowing.IsShowing = true;
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
        private bool m_fromLibrary = false;
        private bool m_edited = false;
        private bool m_isShowing;
        private bool m_isNext;

        public int SessionId => m_sessionId;

        public BitmapFrame BitmapFrame
        {
            get => m_bitmapFrame;
            set
            {
                m_bitmapFrame = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowing
        {
            get => m_isShowing;
            set
            {
                m_isShowing = value;
                OnPropertyChanged();
            }
        }

        public bool IsNext
        {
            get => m_isNext;
            set
            {
                m_isNext = value;
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
        
        

        public bool Edited
        {
            get => m_edited;
            set
            {
                m_edited = value;
                OnPropertyChanged();
            }
        }

        public bool FromLibrary
        {
            get => m_fromLibrary;
            set
            {
                m_fromLibrary = value;
                OnPropertyChanged();
            }
        }

        private void InitializeSessionId()
        {
            m_sessionId = s_nextSessionId++;
        }

        public BeeImage(BitmapFrame src, string name)
        {
            InitializeSessionId();
            Name = name;
            BitmapFrame = src;
            StartRect = GetRectFromImageSrc(src);
            ShrinkEnd(StartRect.Width * 0.2);
        }

        public BeeImage(string serial, string childPath)
        {
            InitializeSessionId();
            BitmapFrame = null;
            Deserialize(serial, childPath);
        }

        public BeeImage(string filePath)
        {
            InitializeSessionId();
            SetBitmapFrameFromFilePath(filePath);
            StartRect = GetRectFromImageSrc(BitmapFrame);
            ShrinkEnd(StartRect.Width * 0.2);
            Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
        }

        private void ShrinkEnd(double margin)
        {
            EndRect = new BeeRect(StartRect.Left + margin, StartRect.Top + margin, StartRect.Width - margin - margin, StartRect.Height - margin - margin);
        }

        public BeeRect GetRectFromImageSrc(ImageSource src)
        {
            if (src != null)
                return new BeeRect(0, 0, src.Width, src.Height);
            else
                return new BeeRect();
        }

        public void ResetStartRect() { StartRect = new BeeRect(0, 0, BitmapFrame.Width, BitmapFrame.Height);  }
        public void ResetEndRect() { EndRect = new BeeRect(0, 0, BitmapFrame.Width, BitmapFrame.Height); }

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

            Dictionary<string, string> imgStrings = new Dictionary<string, string>();
            imgStrings["SaveName"] = saveName;
            imgStrings["StartLeft"] = StartRect.Left.ToString();
            imgStrings["StartTop"] = StartRect.Top.ToString();
            imgStrings["StartWidth"] = StartRect.Width.ToString();
            imgStrings["StartHeight"] = StartRect.Height.ToString();
            imgStrings["EndLeft"] = EndRect.Left.ToString();
            imgStrings["EndTop"] = EndRect.Top.ToString();
            imgStrings["EndWidth"] = EndRect.Width.ToString();
            imgStrings["EndHeight"] = EndRect.Height.ToString();
            imgStrings["Edited"] = Edited ? "1" : "0";
            return BeeBurnIO.SerializeDictionary(imgStrings);
        }

        private bool SetBitmapFrameFromFilePath(string filePath)
        {
            Stream imgStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            string ext = System.IO.Path.GetExtension(filePath);
            if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase))
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(imgStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapFrame = decoder.Frames[0];
                return true;
            }
            else if (ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                     ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                JpegBitmapDecoder decoder = new JpegBitmapDecoder(imgStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapFrame = decoder.Frames[0];
                return true;
            }

            return false;
        }

        public void Deserialize(string str, string childPath)
        {
            str = str.Trim(new char[] { ' ', '\t', '\n', '\r' });
            var d = BeeBurnIO.DeserializeDictionary(str, new string[] { "|", "| " });
            string fileName = d["SaveName"];
            Name = d["SaveName"].Substring(5); // Remove 0000-
            Name = Name.Substring(0, Name.Length - 4); // remove .png
            StartRect = new BeeRect(double.Parse(d["StartLeft"]), double.Parse(d["StartTop"]), double.Parse(d["StartWidth"]), double.Parse(d["StartHeight"]));
            EndRect = new BeeRect(double.Parse(d["EndLeft"]), double.Parse(d["EndTop"]), double.Parse(d["EndWidth"]), double.Parse(d["EndHeight"]));
            Edited = (d["Edited"][0] == '1');

            SetBitmapFrameFromFilePath(childPath + "\\" + fileName);
        }
    }
}

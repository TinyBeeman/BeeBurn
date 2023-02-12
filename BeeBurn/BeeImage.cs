using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
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
            OnPropertyChanged("BitmapImage");
            OnPropertyChanged("StartRect");
            OnPropertyChanged("EndRect");

        }

        // private BitmapFrame m_bitmapFrame;
        private BitmapImage m_bitmapImage;
        private BeeRect m_startRect;
        private BeeRect m_endRect;
        private string m_name;
        private bool m_fromLibrary = false;
        private bool m_edited = false;
        private bool m_isShowing;
        private bool m_isNext;

        public int SessionId => m_sessionId;

        /*public BitmapFrame BitmapFrame
        {
            get => m_bitmapFrame;
            set
            {
                m_bitmapFrame = value;
                OnPropertyChanged();
            }
        }*/

        public BitmapImage Image
        {
            get => m_bitmapImage;
            set
            {
                m_bitmapImage = value;
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

        public string Resolution
        {
            get => Image?.Width.ToString() ?? "0" + ", " + Image?.Height.ToString() ?? "0"; //m_bitmapFrame.Width.ToString() + ", " + m_bitmapFrame.Height.ToString();
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

        public BeeImage(BitmapImage src, string name)
        {
            InitializeSessionId();
            Name = name;
            Image = src;
            StartRect = GetRectFromImageSrc(src);
            ShrinkEnd(StartRect.Width * 0.2);
        }

        public BeeImage(string serial, string childPath)
        {
            InitializeSessionId();
            Image = null;
            Deserialize(serial, childPath);
        }

        public BeeImage(string filePath)
        {
            InitializeSessionId();
            SetBitmapFrameFromFilePath(filePath);
            StartRect = GetRectFromImageSrc(Image);
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

        public void ResetStartRect() { StartRect = new BeeRect(0, 0, Image.Width, Image.Height);  }
        public void ResetEndRect() { EndRect = new BeeRect(0, 0, Image.Width, Image.Height); }

        private OffsetScale GetOffsetAndScaleFromRect(BeeRect rFocus, BeeRect rContainer)
        {

            double aspContainer = rContainer.Width / rContainer.Height;
            double aspFocus = rFocus.Width / rFocus.Height;
            bool fitWidth = (aspFocus < aspContainer);

            OffsetScale ret;
            ret.Scale = fitWidth ? (rContainer.Height / rFocus.Height) : (rContainer.Width / rFocus.Width);
            ret.OffsetX = -(rFocus.Left + (rFocus.Width / 2));
            ret.OffsetY = -(rFocus.Top + (rFocus.Height / 2));
            ret.OriginX = ret.OffsetX / Image.Width;
            ret.OriginY = ret.OffsetY / Image.Height;

            return ret;
        }

        public OffsetScale GetStartOffsetScale(BeeRect rContainer) { return GetOffsetAndScaleFromRect(StartRect, rContainer); }
        public OffsetScale GetEndOffsetScale(BeeRect rContainer) { return GetOffsetAndScaleFromRect(EndRect, rContainer); }


        public bool SaveImage(string filepath)
        {
            
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Image));
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

            Dictionary<string, string> imgStrings = new Dictionary<string, string>
            {
                ["SaveName"] = saveName,
                ["StartLeft"] = StartRect.Left.ToString(),
                ["StartTop"] = StartRect.Top.ToString(),
                ["StartWidth"] = StartRect.Width.ToString(),
                ["StartHeight"] = StartRect.Height.ToString(),
                ["EndLeft"] = EndRect.Left.ToString(),
                ["EndTop"] = EndRect.Top.ToString(),
                ["EndWidth"] = EndRect.Width.ToString(),
                ["EndHeight"] = EndRect.Height.ToString(),
                ["Edited"] = Edited ? "1" : "0"
            };
            return BeeBurnIO.SerializeDictionary(imgStrings);
        }

        private bool SetBitmapFrameFromFilePath(string filePath)
        {
            BitmapImage bmpImg = new BitmapImage();
            using (Stream imgStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bmpImg.BeginInit();
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.StreamSource = imgStream;
                bmpImg.EndInit();
                Image = bmpImg;
            }

            return true;
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

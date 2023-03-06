using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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

        private BitmapImage m_bitmapImage;
        private BeeRect m_startRect = new BeeRect(0, 0, 1, 1);
        private BeeRect m_endRect = new BeeRect(0, 0, 1, 1);
        private string m_name;
        private bool m_fromLibrary = false;
        private bool m_edited = false;
        private bool m_isShowing;
        private bool m_stopImage;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static BeeImage CreateStopImage()
        {
            BeeImage bi = new BeeImage(BlankImage, "Stop");
            bi.StartRect = new BeeRect(BlankImage.Width / 3, BlankImage.Height / 3, BlankImage.Width / 3, BlankImage.Height / 3);
            bi.EndRect = new BeeRect(BlankImage.Width / 3, BlankImage.Height / 3, BlankImage.Width / 3, BlankImage.Height / 3);
            bi.m_stopImage = true;
            return bi;
        }

        public void UpdateAllProps()
        {
            OnPropertyChanged("BitmapImage");
            OnPropertyChanged("StartRect");
            OnPropertyChanged("EndRect");
        }

        public int SessionId => m_sessionId;

        public BitmapImage Image
        {
            get => m_bitmapImage;
            set
            {
                m_bitmapImage = value;
                if (m_bitmapImage != null && (m_bitmapImage.Width > 1024 || m_bitmapImage.Height > 1024))
                {
                    double scale = Math.Min(1024.0 / m_bitmapImage.Width, 1024.0 / m_bitmapImage.Height);
                    BitmapImage newSrc = new BitmapImage();
                    var targetBitmap = new TransformedBitmap(m_bitmapImage, new ScaleTransform(scale, scale));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Create a BitmapEncoder and set its properties
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        // Add the resized bitmap to the encoder
                        encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
                        // Save the encoder to the stream
                        encoder.Save(ms);

                        // Load the stream into the new BitmapImage
                        newSrc.BeginInit();
                        newSrc.CacheOption = BitmapCacheOption.OnLoad;
                        newSrc.StreamSource = new MemoryStream(ms.ToArray());
                        newSrc.EndInit();
                    }

                    m_bitmapImage = newSrc;
                }

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

        public bool IsStopImage
        {
            get => m_stopImage;
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

        static BitmapImage m_blankImage = null;
        static BitmapImage BlankImage
        {
            get
            {
                if (m_blankImage == null)
                {
                    m_blankImage = new BitmapImage();
                    Array bytes = Enumerable.Repeat<byte>(0, 100 * 100).ToArray();
                    int stride = 100;
                    var src = BitmapImage.Create(
                                        200,
                                        100,
                                        96,
                                        96,
                                        PixelFormats.Indexed1,
                                        new BitmapPalette(new List<System.Windows.Media.Color> { Colors.Black }),
                                        bytes,
                                        stride);
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(src));
                    MemoryStream ms = new MemoryStream();
                    encoder.Save(ms);
                    ms.Position = 0;
                    m_blankImage.BeginInit();
                    m_blankImage.CacheOption = BitmapCacheOption.OnLoad;
                    m_blankImage.StreamSource = new MemoryStream(ms.ToArray());
                    m_blankImage.EndInit();
                    ms.Close();
                }

                return m_blankImage;
            }
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

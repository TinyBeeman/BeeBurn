using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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

        private ImageSource ImageFromClipboardDib()
        {
            MemoryStream ms = Clipboard.GetData("DeviceIndependentBitmap") as MemoryStream;
            if (ms != null)
            {
                byte[] dibBuffer = new byte[ms.Length];
                ms.Read(dibBuffer, 0, dibBuffer.Length);

                BITMAPINFOHEADER infoHeader =
                    BinaryStructConverter.FromByteArray<BITMAPINFOHEADER>(dibBuffer);

                int fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
                int infoHeaderSize = infoHeader.biSize;
                int fileSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage;

                BITMAPFILEHEADER fileHeader = new BITMAPFILEHEADER();
                fileHeader.bfType = BITMAPFILEHEADER.BM;
                fileHeader.bfSize = fileSize;
                fileHeader.bfReserved1 = 0;
                fileHeader.bfReserved2 = 0;
                fileHeader.bfOffBits = fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed * 4;

                byte[] fileHeaderBytes =
                    BinaryStructConverter.ToByteArray<BITMAPFILEHEADER>(fileHeader);

                MemoryStream msBitmap = new MemoryStream();
                msBitmap.Write(fileHeaderBytes, 0, fileHeaderSize);
                msBitmap.Write(dibBuffer, 0, dibBuffer.Length);
                msBitmap.Seek(0, SeekOrigin.Begin);

                return BitmapFrame.Create(msBitmap);
            }
            return null;
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

            imgNew.Source = ImageFromClipboardDib();
            ReplaceImage(imgNew);
        }

    }
}

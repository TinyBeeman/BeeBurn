using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Linq;

namespace BeeBurn
{
    public class BeeStack : INotifyPropertyChanged
    {
        private static string s_sep = "---\n";
        private static int s_nextStackCounter = 1;

        private BeeImage m_nextImage = null;
        private string m_name;
        private RangeObservableCollection<string> m_tags = new RangeObservableCollection<string>();
        private RangeObservableCollection<string> m_decades = new RangeObservableCollection<string>();
        private ObservableCollection<BeeImage> m_images = new ObservableCollection<BeeImage>();
        private int m_SelectedIndex = -1;
        public event PropertyChangedEventHandler PropertyChanged;
        private BeeImage m_beeImgToDisplay = null;
        private bool m_isLibrary = false;
        private bool m_atEnd = false;
        private bool m_isActive;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BeeStack()
        {
            m_name = "Stack " + (s_nextStackCounter++).ToString("D" + 3);
        }

        public void ResetNextImage()
        {
            m_atEnd = false;
            m_nextImage = m_images.Count > 0 ? m_images[0] : null;
            BeeImage.SetNextImage(m_nextImage);
        }

        public BeeImage PeekNextImage(bool loop)
        {
            if (m_nextImage != null || m_images.Count == 0)
                return m_nextImage;

            if (m_atEnd && loop)
                return Images[0];

            return null;
        }

        public BeeImage GetNextImage(bool loop)
        {
            // Empty List? Set everything to null.
            if (m_images.Count == 0)
            {
                m_nextImage = null;
                BeeImage.SetNextImage(null);
                return null;
            }

            // If we're at the end, reset the list but return null
            // if we're not in a loop.
            if (m_atEnd)
            {
                ResetNextImage();
                if (!loop)
                    return null;
            }
            
            // If we don't have an empty list, but also no next image
            // let's reset the next image.
            if (m_nextImage == null)
            {
                ResetNextImage();
            }

            int iNext = m_images.IndexOf(m_nextImage);

            // Weird case where m_nextImage isn't in the list anymore.
            if (iNext == -1)
            {
                ResetNextImage();
                iNext = 0;
            }

            BeeImage biRet = m_nextImage;
            int iNewNext = iNext + 1;
            if (iNewNext >= m_images.Count)
            {
                // If we're at the end of the list,
                // we set the next image to null,
                // unless we're looping, in which case
                // we Reset it (to 0, presumably).
                if (loop)
                {
                    ResetNextImage();
                }
                else
                {
                    m_nextImage = null;
                    m_atEnd = true;
                    BeeImage.SetNextImage(null);
                }
            }
            else
            {
                // Set up our next image.
                m_nextImage = m_images[iNewNext];
                BeeImage.SetNextImage(m_nextImage);
            }

            return biRet;
        }

        public string Name
        {
            get => m_name;
            set
            {
                m_name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BeeImage> Images
        {
            get => m_images;
            set { m_images = value; OnPropertyChanged(); }
        }

        public bool IsLibrary
        {
            get => m_isLibrary;
            set
            {
                m_isLibrary = value;
                OnPropertyChanged();
            }
        }


        public bool IsActive
        {
            get => m_isActive;
            set
            {
                m_isActive = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get => m_SelectedIndex;
            set
            {
                m_SelectedIndex = value;
                OnPropertyChanged();
                if (value >= 0 && value < m_images.Count)
                    BeeImgToDisplay = m_images[value];
            }
        }

        public BeeImage BeeImgToDisplay
        {
            get => m_beeImgToDisplay;
            set
            {
                m_beeImgToDisplay = value;
                OnPropertyChanged();
                m_beeImgToDisplay.UpdateAllProps();
            }
        }

        public RangeObservableCollection<string> Tags
        {
            get => m_tags;
            set
            {
                m_tags = value;
                OnPropertyChanged();
                OnPropertyChanged("AllTags");
            }
        }

        public RangeObservableCollection<string> Decades
        {
            get => m_decades;
            set
            {
                m_decades = value;
                OnPropertyChanged();
                OnPropertyChanged("AllDecades");
            }
        }

        public string AllTags
        {
            get
            {
                return string.Join(", ", m_tags);
            }
            set
            {
                m_tags.Clear();
                IEnumerable<string> tags = new List<string>(value.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries)).Distinct().OrderBy(s => s);
                m_tags.AddRange(tags);
                OnPropertyChanged("Tags");
                OnPropertyChanged("AllTags");
            }
        }

        public string AllDecades
        {
            get
            {
                return string.Join(", ", m_decades);
            }
            set
            {
                m_decades.Clear();
                IEnumerable<string> decades = new List<string>(value.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries)).Distinct().OrderBy(s => s);
                m_tags.AddRange(decades);
                OnPropertyChanged("Decades");
                OnPropertyChanged("AllDecades");
            }
        }

        internal bool SaveStack(string fileNameNaked, string savePath)
        {
            try
            {
                int i = 0;
                string childPath = savePath + "\\" + fileNameNaked;
                Directory.CreateDirectory(childPath);

                string fullText = fileNameNaked + "\n" + s_sep;
                
                // TAGS
                fullText += "Tags:";
                foreach (var s in m_tags)
                {
                    fullText += s + "|";
                }
                fullText += "\n" + s_sep;

                // DECADES
                fullText += "Decades:";
                foreach (var s in m_decades)
                {
                    fullText += s + "|";
                }
                fullText += "\n" + s_sep;

                foreach (var bi in Images)
                {
                    fullText += bi.Serialize(i++, childPath) + "\n";
                    fullText += s_sep;
                }
                File.WriteAllText(savePath + "\\" + fileNameNaked + ".bstack", fullText);
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.MessageBox.Show("Failed to save " + fileNameNaked + " stack to " + savePath + ": " + ex.Message);
                return false;
            }

            return true;
        }

        internal bool LoadStack(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            string rootpath = Path.GetDirectoryName(filePath);
            string fileNameNaked = Path.GetFileNameWithoutExtension(filePath);

            try
            {
                string strAll = File.ReadAllText(filePath);
                string[] imgs = strAll.Split(new string[] { s_sep }, StringSplitOptions.RemoveEmptyEntries);
                int iLine = 0;
                // First line is the name of the path of any child images.
                string childPath = imgs[iLine++];

                for (int i = iLine; i < imgs.Length; i++)
                {
                    string line = imgs[i];
                    if (string.Compare(line.Substring(0, 5), "tags:", true) == 0)
                    {
                        string tagLine = imgs[iLine++].Trim(new char[] { '\n', ' ' });
                        // Substring is to remove "tags:"
                        Tags.AddRange(tagLine.Substring(5).Split(new string[] { "|", "| " }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else if (string.Compare(line.Substring(0, 8), "decades:", true) == 0)
                    {
                        string decadeLine = line.Trim(new char[] { '\n', ' ' });
                        // Substring is to remove "decades:"
                        Decades.AddRange(decadeLine.Substring(8).Split(new string[] { "|", "| " }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        BeeImage bi = new BeeImage(line, rootpath + "\\" + fileNameNaked + "\\");
                        Images.Add(bi);
                    }
                }


                Name = fileNameNaked;
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public void PasteImage()
        {
            BitmapFrame srcClip = BeeClipboard.BitmapFrameFromClipboardDib();
            if (srcClip != null)
            {
                Images.Add(new BeeImage(srcClip, "Paste-" + BeeBurnVM.Get().PasteCounter.ToString("D" + 4))); ;
            }
        }
    }
}

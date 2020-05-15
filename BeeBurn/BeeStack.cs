﻿using System;
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
        private static int s_nextStack = 1;

        private BeeImage m_nextImage = null;
        private string m_name;
        private RangeObservableCollection<string> m_tags = new RangeObservableCollection<string>();
        private ObservableCollection<BeeImage> m_activeImages = new ObservableCollection<BeeImage>();
        private int m_activeSelectionIndex = -1;
        public event PropertyChangedEventHandler PropertyChanged;
        private BeeImage m_beeImgToDisplay = null;
        private bool m_isLibrary = false;
        private bool m_atEnd = false;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BeeStack()
        {
            m_name = "Stack " + (s_nextStack++).ToString("D" + 3);
        }

        public void ResetNextImage()
        {
            m_atEnd = false;
            m_nextImage = m_activeImages.Count > 0 ? m_activeImages[0] : null;
            BeeImage.SetNextImage(m_nextImage);
        }

        public BeeImage GetNextImage(bool loop)
        {
            // Empty List? Set everything to null.
            if (m_activeImages.Count == 0)
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

            int iNext = m_activeImages.IndexOf(m_nextImage);

            // Weird case where m_nextImage isn't in the list anymore.
            if (iNext == -1)
            {
                ResetNextImage();
                iNext = 0;
            }

            BeeImage biRet = m_nextImage;
            if (biRet != null)
                biRet.IsNext = false;

            int iNewNext = iNext + 1;
            if (iNewNext >= m_activeImages.Count)
            {
                if (!loop)
                {
                    m_nextImage = null;
                    m_atEnd = true;
                }
                else
                {
                    ResetNextImage();
                }
            }
            else
            {
                m_nextImage = m_activeImages[iNewNext];
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

        public ObservableCollection<BeeImage> ActiveImages
        {
            get => m_activeImages;
            set { m_activeImages = value; OnPropertyChanged(); }
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


        public int ActiveSelectionIndex
        {
            get => m_activeSelectionIndex;
            set
            {
                m_activeSelectionIndex = value;
                OnPropertyChanged();
                if (value >= 0 && value < m_activeImages.Count)
                    BeeImgToDisplay = m_activeImages[value];
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

        internal bool SaveStack(string fileNameNaked, string savePath)
        {
            try
            {
                int i = 0;
                string childPath = savePath + "\\" + fileNameNaked;
                Directory.CreateDirectory(childPath);

                string fullText = fileNameNaked + "\n" + s_sep;
                fullText += "Tags:";
                foreach (var s in m_tags)
                {
                    fullText += s + "|";
                }
                fullText += "\n" + s_sep;
                foreach (var bi in ActiveImages)
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

        internal void LoadStack(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            string rootpath = Path.GetDirectoryName(filePath);
            string fileNameNaked = Path.GetFileNameWithoutExtension(filePath);

            string strAll = File.ReadAllText(filePath);
            string[] imgs = strAll.Split(new string[] { s_sep }, StringSplitOptions.RemoveEmptyEntries);
            string childPath = imgs[0];
            string tags = imgs[1].Trim(new char[] { '\n', ' ' });
            // Substring is to remove "tags:"
            Tags.AddRange(tags.Substring(5).Split(new string[] { "|", "| " }, StringSplitOptions.RemoveEmptyEntries));
            for (int i = 2; i < imgs.Length; i++)
            {
                BeeImage bi = new BeeImage(imgs[i], rootpath + "\\" + fileNameNaked + "\\");
                ActiveImages.Add(bi);
            }
        }

        public void PasteImage()
        {
            BitmapFrame srcClip = BeeClipboard.BitmapFrameFromClipboardDib();
            if (srcClip != null)
            {
                ActiveImages.Add(new BeeImage(srcClip, "Paste-" + BeeBurnVM.Get().PasteCounter.ToString("D" + 4))); ;
            }
        }
    }
}

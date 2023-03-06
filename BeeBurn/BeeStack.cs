using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Net;

namespace BeeBurn
{
    public class BeeStack : INotifyPropertyChanged
    {
        private static string s_sep = "---\n";
        private static int s_nextStackCounter = 1;

        private string m_name;
        private RangeObservableCollection<string> m_tags = new RangeObservableCollection<string>();
        private ObservableCollection<BeeImage> m_images = new ObservableCollection<BeeImage>();
        private int m_SelectedIndex = -1;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool m_isLibrary = false;
        private bool m_isActive;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BeeStack()
        {
            m_name = "Stack " + (s_nextStackCounter++).ToString("D" + 3);
        }

        public BeeStack(string name)
        {
            m_name = name;
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
            }
        }

        public int SelectedSessionId
        {
            get
            {
                if (m_SelectedIndex == -1 || m_SelectedIndex >= m_images.Count) return 0;
                return m_images[m_SelectedIndex].SessionId;
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
                
                // TAGS
                fullText += "Tags:";
                foreach (var s in m_tags)
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
                    else if (line.StartsWith("decades:", true, null))
                    {
                        // Ignore deprecated decades tag.
                        continue;
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
            BitmapImage srcClip = BeeClipboard.BitmapImageFromClipboard();
            if (srcClip != null)
            {
                Images.Add(new BeeImage(srcClip, "Paste-" + BeeBurnVM.Get().PasteCounter.ToString("D" + 4))); ;
            }
        }

        internal void RemoveImage(BeeImage img)
        {
            Images.Remove(img);
        }
    }
}

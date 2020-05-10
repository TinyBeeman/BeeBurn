using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace BeeBurn
{
    public enum ConfigKey : int
    {
        LibraryPath,
        SavePath,
        ImageFadeTime,
        ImagePanTime
    }

    public class BeeBurnVM : INotifyPropertyChanged
    {
        private static BeeBurnVM s_singleton = null;

        private Dictionary<ConfigKey, string> m_configStrings = new Dictionary<ConfigKey, string>();
        private Dictionary<ConfigKey, double> m_configDoubles = new Dictionary<ConfigKey, double>();
        private int m_pasteCounter = 0;
        private int m_activeSelectionIndex = -1;
        private ObservableCollection<BeeImage> m_activeImages = new ObservableCollection<BeeImage>();
        private BeeImage m_beeImgToDisplay;


        public static BeeBurnVM Get()
        {
            if (s_singleton == null)
                s_singleton = new BeeBurnVM();

            return s_singleton;
        }

        private BeeBurnVM()
        {
            InitializeDefaultSettings();
        }

        private void InitializeDefaultSettings()
        {
            m_configStrings.Add(ConfigKey.LibraryPath, "N:\\Data\\Dropbox\\Shared\\DocumentaryImages");
            m_configStrings.Add(ConfigKey.SavePath, "D:\\Temp\\BeeBurn");
            m_configDoubles.Add(ConfigKey.ImageFadeTime, 2.0);
            m_configDoubles.Add(ConfigKey.ImagePanTime, 10.0);
        }

        public double? GetConfigDouble(ConfigKey configKey)
        {
            if (m_configDoubles.ContainsKey(configKey))
                return m_configDoubles[configKey];

            return null;
        }

        public string GetConfigString(ConfigKey configKey)
        {
            if (m_configStrings.ContainsKey(configKey))
                return m_configStrings[configKey];

            return null;
        }


        public ObservableCollection<BeeImage> ActiveImages
        {
            get => m_activeImages;
            set { m_activeImages = value; OnPropertyChanged(); }
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void PasteToList()
        {
            BitmapFrame srcClip = BeeClipboard.BitmapFrameFromClipboardDib();
            if (srcClip != null)
            {
                ActiveImages.Add(new BeeImage(srcClip, "Paste-" + m_pasteCounter.ToString("D" + 4))); ;
                m_pasteCounter++;
            }
        }

        public void ClearList()
        {
            ActiveImages.Clear();
        }

        private static string s_sep = "---\n";

        internal void SaveStack(string fileNameNaked, string savePath)
        {

            int i = 0;
            string childPath = savePath + "\\" + fileNameNaked;
            Directory.CreateDirectory(childPath);

            string fullText = fileNameNaked + "\n" + s_sep;
            foreach (var bi in ActiveImages)
            {
                fullText += bi.Serialize(i++, childPath) + "\n";
                fullText += s_sep;
            }
            File.WriteAllText(savePath + "\\" + fileNameNaked + ".bstack", fullText);
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
            for (int i = 1; i < imgs.Length; i++)
            {
                BeeImage bi = new BeeImage(imgs[i], rootpath + "\\" + fileNameNaked + "\\");
                ActiveImages.Add(bi);
            }
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
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
        private BeeStack m_activeStack = new BeeStack();
        


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
                ActiveStack.ActiveImages.Add(new BeeImage(srcClip, "Paste-" + m_pasteCounter.ToString("D" + 4))); ;
                m_pasteCounter++;
            }
        }


        public BeeStack ActiveStack
        {
            get => m_activeStack;
            set
            {
                m_activeStack = value;
                OnPropertyChanged();
            }
        }


    }
}

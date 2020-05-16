using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeeBurn
{

    public enum ConfigKey : int
    {
        LibraryPath,
        ImageLoadPath,
        SavePath,
        ImageFadeTime,
        ImagePanTime,
        SaveEmptyStacks
    }

    public class BeeConfigSettings : INotifyPropertyChanged
    {
        private Dictionary<ConfigKey, string> m_configStrings = new Dictionary<ConfigKey, string>();
        private Dictionary<ConfigKey, double> m_configDoubles = new Dictionary<ConfigKey, double>();
        private Dictionary<ConfigKey, bool> m_configBools = new Dictionary<ConfigKey, bool>();

        public event PropertyChangedEventHandler PropertyChanged;

        public BeeConfigSettings()
        {
            InitializeDefaultSettings();
        }

        private void InitializeDefaultSettings()
        {
            m_configStrings.Add(ConfigKey.LibraryPath, "N:\\Data\\Dropbox\\Shared\\DocumentaryImages");
            m_configStrings.Add(ConfigKey.SavePath, "D:\\Temp\\BeeBurn");
            m_configStrings.Add(ConfigKey.ImageLoadPath, "D:\\Users\\tony\\Downloads");
            m_configDoubles.Add(ConfigKey.ImageFadeTime, 2.0);
            m_configDoubles.Add(ConfigKey.ImagePanTime, 60.0);
            m_configBools.Add(ConfigKey.SaveEmptyStacks, false);
        }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string LibraryPath
        {
            get { return m_configStrings[ConfigKey.LibraryPath]; }
            set
            {
                m_configStrings[ConfigKey.LibraryPath] = value;
                OnPropertyChanged();
            }
        }

        public string SavePath
        {
            get { return m_configStrings[ConfigKey.SavePath]; }
            set
            {
                m_configStrings[ConfigKey.SavePath] = value;
                OnPropertyChanged();
            }
        }

        public string ImageLoadPath
        {
            get { return m_configStrings[ConfigKey.ImageLoadPath]; }
            set
            {
                m_configStrings[ConfigKey.ImageLoadPath] = value;
                OnPropertyChanged();
            }
        }

        public double ImageFadeTime
        {
            get { return m_configDoubles[ConfigKey.ImageFadeTime]; }
            set
            {
                m_configDoubles[ConfigKey.ImageFadeTime] = value;
                OnPropertyChanged();
            }
        }

        public double ImagePanTime
        {
            get { return m_configDoubles[ConfigKey.ImagePanTime]; }
            set
            {
                m_configDoubles[ConfigKey.ImagePanTime] = value;
                OnPropertyChanged();
            }
        }

        public bool SaveEmptyStacks
        {
            get { return m_configBools[ConfigKey.SaveEmptyStacks]; }
            set
            {
                m_configBools[ConfigKey.SaveEmptyStacks] = value;
                OnPropertyChanged();
            }
        }
    }
}

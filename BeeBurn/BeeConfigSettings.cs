using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace BeeBurn
{

    public enum ConfigKey : int
    {
        LibraryPath,
        ImageLoadPath,
        SavePath,
        ImageFadeTime,
        ImagePanTime,
        SaveEmptyStacks,
        Fullscreen,
        ScreenIndex,
        WindowWidth,
        WindowHeight
    }

    public class BeeConfigSettings : INotifyPropertyChanged
    {
        private Dictionary<ConfigKey, string> m_configStrings = new Dictionary<ConfigKey, string>();
        private Dictionary<ConfigKey, double> m_configDoubles = new Dictionary<ConfigKey, double>();
        private Dictionary<ConfigKey, int> m_configInts = new Dictionary<ConfigKey, int>();
        private Dictionary<ConfigKey, bool> m_configBools = new Dictionary<ConfigKey, bool>();

        public event PropertyChangedEventHandler PropertyChanged;

        public BeeConfigSettings()
        {
            InitializeDefaultSettings(true);
        }

        public static string SettingsPath { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BeeBurnSettings.json");  }

        public void SaveToFile()
        {
            File.Delete(SettingsPath);
            File.WriteAllText(SettingsPath, System.Text.Json.JsonSerializer.Serialize(this));
        }

        public static BeeConfigSettings LoadFromFile()
        {
            if (File.Exists(SettingsPath))
            {
                string jsonString;
                using (StreamReader reader = new StreamReader(SettingsPath))
                {
                    jsonString = reader.ReadToEnd();
                }
                BeeConfigSettings settings = System.Text.Json.JsonSerializer.Deserialize<BeeConfigSettings>(jsonString) ?? new BeeConfigSettings();
                settings.InitializeDefaultSettings(false);
                return settings;
            }
            else
            {
                return new BeeConfigSettings();
            }
        }

        private void InitializeDefaultSettings(bool overrideAll)
        {
            if (overrideAll || !m_configStrings.ContainsKey(ConfigKey.LibraryPath))
                m_configStrings[ConfigKey.LibraryPath] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (overrideAll || !m_configStrings.ContainsKey(ConfigKey.SavePath))
                m_configStrings[ConfigKey.SavePath] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (overrideAll || !m_configStrings.ContainsKey(ConfigKey.ImageLoadPath))
                m_configStrings[ConfigKey.ImageLoadPath] = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!m_configDoubles.ContainsKey(ConfigKey.ImageFadeTime))
                m_configDoubles[ConfigKey.ImageFadeTime] = 2.0;
            if (overrideAll || !m_configDoubles.ContainsKey(ConfigKey.ImagePanTime))
                m_configDoubles[ConfigKey.ImagePanTime] = 60.0;
            if (overrideAll || !m_configBools.ContainsKey(ConfigKey.SaveEmptyStacks))
                m_configBools[ConfigKey.SaveEmptyStacks] = false;
            if (overrideAll || !m_configBools.ContainsKey(ConfigKey.Fullscreen))
                m_configBools[ConfigKey.Fullscreen] = true;
            if (overrideAll || !m_configInts.ContainsKey(ConfigKey.ScreenIndex))
                m_configInts[ConfigKey.ScreenIndex] = 0;
            if (overrideAll || !m_configInts.ContainsKey(ConfigKey.WindowWidth))
                m_configInts[ConfigKey.WindowWidth] = 1024;
            if (overrideAll || !m_configInts.ContainsKey(ConfigKey.WindowHeight))
                m_configInts[ConfigKey.WindowHeight] = 768;
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

        public bool FullScreen
        {
            get { return m_configBools[ConfigKey.Fullscreen]; }
            set
            {
                m_configBools[ConfigKey.Fullscreen] = value;
                OnPropertyChanged();
            }
        }

        public int ScreenIndex
        {
            get { return m_configInts[ConfigKey.ScreenIndex]; }
            set
            {
                m_configInts[ConfigKey.ScreenIndex] = value;
                OnPropertyChanged();
            }
        }

        public int WindowWidth
        {
            get { return m_configInts[ConfigKey.WindowWidth]; }
            set
            {
                m_configInts[ConfigKey.WindowWidth] = value;
                OnPropertyChanged();
            }
        }

        public int WindowHeight
        {
            get { return m_configInts[ConfigKey.WindowHeight]; }
            set
            {
                m_configInts[ConfigKey.WindowHeight] = value;
                OnPropertyChanged();
            }
        }
    }
}

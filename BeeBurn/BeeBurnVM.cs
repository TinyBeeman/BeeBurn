using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace BeeBurn
{
    public enum ConfigKey : int
    {
        LibraryPath,
        ImageLoadPath,
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

        public int PasteCounter
        {
            get { return m_pasteCounter++; }
        }

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
            m_configStrings.Add(ConfigKey.ImageLoadPath, "D:\\Users\\tony\\Downloads");
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

    public static class BeeBurnIO
    {
        public static bool LoadImagesToStack(BeeStack stack)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = BeeBurnVM.Get().GetConfigString(ConfigKey.ImageLoadPath);
            dlg.Multiselect = true;
            dlg.Filter = "Image Files(*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                foreach (string filepath in dlg.FileNames)
                {
                    BeeImage bi = new BeeImage(filepath);
                    stack.ActiveImages.Add(bi);
                }
                return true;
            }

            return false;
        }

        public static string SerializeDictionary(Dictionary<string, string> dict, char sep = '|', char assign = ':')
        {
            string ret = "";
            
            foreach (var kvp in dict)
            {
                if (ret.Length > 0)
                    ret += sep;
                ret += kvp.Key + ":" + kvp.Value;
            }

            return ret;
        }

        public static Dictionary<string, string> DeserializeDictionary(string str, string[] sep, char assign = ':')
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach(string skv in str.Split(sep, System.StringSplitOptions.RemoveEmptyEntries))
            {
                string[] kv = skv.Split(new char[] { assign });
                dict[kv[0].Trim()] = kv[1].Trim();
            }

            return dict;
        }
    }
}

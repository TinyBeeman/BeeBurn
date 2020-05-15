using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private BeeStack m_SelectedStack;
        private ObservableCollection<BeeStack> m_stacks = new ObservableCollection<BeeStack>();
        private int m_SelectedStackIndex;
        private BeeStack m_activeStack;

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
            m_stacks.Add(new BeeStack());
            m_stacks.Add(new BeeStack());  // Temporary for testing.
            m_SelectedStack = m_stacks[0];
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

        public int SelectedStackIndex
        {
            get => m_SelectedStackIndex;
            set
            {
                m_SelectedStackIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedStack");
            }
        }

        public ObservableCollection<BeeStack> Stacks
        {
            get => m_stacks;
        }

        public BeeStack SelectedStack
        {
            get => m_stacks[m_SelectedStackIndex];
            set
            {
                if (m_stacks.Contains(value))
                    SelectedStackIndex = m_stacks.IndexOf(value);
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

        public BeeStack FindNextStackWithImages(int index, bool fLoop)
        {
            for (int i = index; i < m_stacks.Count; i++)
            {
                if (m_stacks[i].ActiveImages.Count > 0)
                    return m_stacks[i];
            }

            if (fLoop)
            {
                for (int i = 0; i < index; i++)
                {
                    if (m_stacks[i].ActiveImages.Count > 0)
                        return m_stacks[i];
                }
            }

            return null;
        }


        public BeeStack ActivateNextStack(bool loopStacks)
        {
            if (ActiveStack == null)
                return EnsureActiveStack();

            int currentIndex = m_stacks.IndexOf(ActiveStack);
            ActiveStack = FindNextStackWithImages(currentIndex + 1, loopStacks);
            return ActiveStack;
        }

        public BeeStack EnsureActiveStack()
        {
            if (ActiveStack != null)
                return ActiveStack;
            
            if (SelectedStack != null)
            {
                ActiveStack = SelectedStack;
                return ActiveStack;
            }

            if (m_stacks.Count == 0)
            {
                m_stacks.Add(new BeeStack());
            }

            ActiveStack = m_stacks[0];
            return ActiveStack;
        }


    }
}

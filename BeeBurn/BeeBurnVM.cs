using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace BeeBurn
{

    public class BeeBurnVM : INotifyPropertyChanged
    {
        private static BeeBurnVM s_singleton = null;

        
        private int m_pasteCounter = 0;
        private ObservableCollection<BeeStack> m_stacks = new ObservableCollection<BeeStack>();
        private int m_selectedStackIndex;
        private BeeStack m_activeStack;
        private BeeConfigSettings m_configSettings;

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
            m_configSettings = new BeeConfigSettings();
            m_stacks.Add(new BeeStack());
            SelectedStack = m_stacks[0];
        }

        public BeeConfigSettings ConfigSettings => m_configSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int SelectedStackIndex
        {
            get => m_selectedStackIndex;
            set
            {
                m_selectedStackIndex = value;
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
            get
            {
                if (m_selectedStackIndex >= 0 && m_selectedStackIndex < m_stacks.Count)
                    return m_stacks[m_selectedStackIndex];
                else
                    return null;
            }
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
                if (m_activeStack != null)
                    m_activeStack.IsActive = false;

                m_activeStack = value;
                m_activeStack.IsActive = true;
                OnPropertyChanged();
            }
        }

        public BeeStack FindNextStackWithImages(int index, bool fLoop)
        {
            for (int i = index; i < m_stacks.Count; i++)
            {
                if (m_stacks[i].Images.Count > 0)
                    return m_stacks[i];
            }

            if (fLoop)
            {
                for (int i = 0; i < index; i++)
                {
                    if (m_stacks[i].Images.Count > 0)
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

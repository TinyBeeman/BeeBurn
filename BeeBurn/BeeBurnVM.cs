using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Collections.Generic;

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

        public BeeStack GetNextStack(bool loopStacks, bool activate)
        {
            if (ActiveStack == null)
                return EnsureActiveStack();

            int currentIndex = m_stacks.IndexOf(ActiveStack);
            BeeStack nextStack = FindNextStackWithImages(currentIndex + 1, loopStacks);
            if (activate)
                ActiveStack = nextStack;

            return nextStack;
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

        public void EnsureUniqueStacks()
        {
            var names = new HashSet<string>();
            foreach (var s in Stacks)
            {
                int iName = 0;
                string newName = s.Name;
                
                while (names.Contains(newName))
                {
                    // Iterate through numbers until we're sure names are unique.
                    newName = s.Name += iName.ToString("D" + 3);
                }

                s.Name = newName;
                names.Add(s.Name);

            }
        }



        public bool LoadCollection(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            string rootpath = Path.GetDirectoryName(filePath);

            List<string> lines = File.ReadLines(filePath).ToList();
            string fileNameNaked = lines[0];
            string childPath = rootpath + "\\" + fileNameNaked;

            for (int i = 1; i < lines.Count; i++)
            {
                BeeStack bsNew = new BeeStack();
                if (bsNew.LoadStack(childPath + "\\" + lines[i]))
                {
                    Stacks.Add(bsNew);
                }
                // TODO: Report failure in error log.
            }

            return true;
        }


        public bool SaveAll(string fileNameNaked, string savePath)
        {
            EnsureUniqueStacks();
            bool saveEmptyStacks = BeeBurnVM.Get().ConfigSettings.SaveEmptyStacks;

            try
            {
                string childPath = savePath + "\\" + fileNameNaked;
                Directory.CreateDirectory(childPath);

                string fullText = fileNameNaked + "\n";

                foreach (var s in Stacks)
                {
                    if (s.Images.Count == 0 && saveEmptyStacks == false)
                        continue;

                    fullText += s.Name + ".bstack" + "\n";
                    s.SaveStack(s.Name, childPath);
                }

                File.WriteAllText(savePath + "\\" + fileNameNaked + ".BeeBurn", fullText);
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.MessageBox.Show("Failed to save " + fileNameNaked + ".BeeBurn collection to " + savePath + ": " + ex.Message);
                return false;
            }

            return true;
        }

    }
}

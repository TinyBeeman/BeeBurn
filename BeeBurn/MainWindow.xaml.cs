using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeeBurn
{

    public class BeeBurnVM : INotifyPropertyChanged
    {
        private int m_pasteCounter = 0;
        private ObservableCollection<BeeImage> m_activeImages;

        public ObservableCollection<BeeImage> ActiveImages
        {
            get => m_activeImages;
            set { m_activeImages = value; OnPropertyChanged(); }
        }

        private int m_activeSelectionIndex = -1;

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

        private BeeImage m_beeImgToDisplay;
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
            if (ActiveImages == null)
            {
                ActiveImages = new ObservableCollection<BeeImage>();
            }

            BitmapFrame srcClip = BeeClipboard.BitmapFrameFromClipboardDib();
            if (srcClip != null)
            {
                ActiveImages.Add(new BeeImage(srcClip, "Paste-" + m_pasteCounter.ToString("D" + 4))); ;
                m_pasteCounter++;
            }
        }

        internal void SaveStack(string fileNameNaked, string savePath)
        {
            
            int i = 0;
            string childPath = savePath + "\\" + fileNameNaked;
            System.IO.Directory.CreateDirectory(childPath);

            string fullText = fileNameNaked + "\n";
            foreach (var bi in ActiveImages)
            {
                string saveName = (i++).ToString("D" + 4) + "-" + bi.Name + ".png";
                if (bi.SaveImage(childPath + "\\" + saveName))
                    fullText += saveName + "\n";
                else
                    throw new Exception("Can't Save Image?");
            }
            System.IO.File.WriteAllText(savePath + "\\" + fileNameNaked + ".bstack", fullText);

        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Projection m_proj = null;
        private BeeBurnVM m_VM = null;


        public MainWindow()
        {
            InitializeComponent();
            m_VM = new BeeBurnVM();
            DataContext = m_VM;
        }

        private void ClickProject(object sender, RoutedEventArgs e)
        {
            if (m_proj == null)
            {
                m_proj = new Projection();
                m_proj.OnClose += () => { m_proj = null; };
            }

            m_proj.Show();
            m_proj.ProjectList(m_VM.ActiveImages);
        }

        private void ClickPaste(object sender, RoutedEventArgs e)
        {
            m_VM.PasteToList();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string filespec = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var dlg = new SaveFileDialog();

            dlg.DefaultExt = ".bstack";
            dlg.Filter = "BStacks (*.bstack)|*.bstack";
            dlg.FileName = filespec + ".bstack";
                        
            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                string savePath = System.IO.Path.GetDirectoryName(dlg.FileName);
                string fileNameNaked = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                string fileExt = System.IO.Path.GetExtension(dlg.FileName);
                if (fileExt.Length < 1)
                {
                    dlg.FileName += ".bstack";
                }

                m_VM.SaveStack(fileNameNaked, savePath);

            }
            

        }
    }
}

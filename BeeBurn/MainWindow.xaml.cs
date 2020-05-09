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

        private int m_activeSelectionIndex;

        public int ActiveSelectionIndex
        {
            get => m_activeSelectionIndex;
            set
            {
                m_activeSelectionIndex = value; 
                OnPropertyChanged();
                ImageSourceToDisplay = m_activeImages[value].ImageSource;
            }
        }

        private ImageSource m_imageSourceToDisplay;
        public ImageSource ImageSourceToDisplay
        {
            get => m_imageSourceToDisplay;
            set { m_imageSourceToDisplay = value; OnPropertyChanged(); }
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

            ActiveImages.Add(new BeeImage(BeeClipboard.ImageFromClipboardDib(), "Paste-" + m_pasteCounter.ToString("D" + 4)));
            m_pasteCounter++;
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
                m_proj = new Projection();
            m_proj.Show();
        }

        private void ClickPaste(object sender, RoutedEventArgs e)
        {
            m_VM.PasteToList();
        }
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeeBurn
{


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
            m_VM = BeeBurnVM.Get();
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
            m_proj.ProjectList(m_VM.ActiveStack.ActiveImages);
        }

        private void ClickPaste(object sender, RoutedEventArgs e)
        {
            m_VM.PasteToList();
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            string filespec = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var dlg = new SaveFileDialog();

            dlg.DefaultExt = ".bstack";
            dlg.InitialDirectory = m_VM.GetConfigString(ConfigKey.SavePath);
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

                m_VM.ActiveStack.SaveStack(fileNameNaked, savePath);

            }
            

        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.InitialDirectory = m_VM.GetConfigString(ConfigKey.SavePath);
            dlg.Filter = "BStacks (*.bstack)|*.bstack";

            if (dlg.ShowDialog() == true)
            {
                m_VM.ActiveStack.LoadStack(dlg.FileName);
            }
        }
    }
}

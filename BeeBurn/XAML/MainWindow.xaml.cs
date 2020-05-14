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
            m_VM.ActiveStack.PasteImage();
        }
        private void ClickLoadImages(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.LoadImagesToStack(m_VM.ActiveStack);
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.SaveAsStack(m_VM.ActiveStack);
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

        private void ClickEditStack(object sender, RoutedEventArgs e)
        {
            var dlgEditStack = new BeeStackEditor(m_VM.ActiveStack);
            dlgEditStack.ShowDialog();
        }
    }
}

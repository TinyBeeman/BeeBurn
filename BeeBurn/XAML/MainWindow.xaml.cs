using BeeBurn.XAML;
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
        
        private BeeBurnVM m_VM = null;
        private PresenterView m_presenterView = null;

        public MainWindow()
        {
            InitializeComponent();
            m_VM = BeeBurnVM.Get();
            DataContext = m_VM;
        }

        private void EnsurePresenterView()
        {
            if (m_presenterView == null)
            {
                m_presenterView = new PresenterView();
                m_presenterView.OnClose += () => { m_presenterView = null; };
            }
        }

        private void ClickProject(object sender, RoutedEventArgs e)
        {
            EnsurePresenterView();
            m_presenterView.Show();
        }

        private void ClickPaste(object sender, RoutedEventArgs e)
        {
            if (m_VM.SelectedStack != null)
                m_VM.SelectedStack.PasteImage();
        }
        private void ClickLoadImages(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.LoadImagesToStack(m_VM.SelectedStack);
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.SaveAsStack(m_VM.SelectedStack);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.InitialDirectory = m_VM.GetConfigString(ConfigKey.SavePath);
            dlg.Filter = "BStacks (*.bstack)|*.bstack";

            if (dlg.ShowDialog() == true)
            {
                BeeStack bsNew = new BeeStack();
                m_VM.Stacks.Add(bsNew);
                bsNew.LoadStack(dlg.FileName);
                m_VM.ActiveStack = bsNew;
            }
        }

        private void ClickEditStack(object sender, RoutedEventArgs e)
        {
            var dlgEditStack = new BeeStackEditor(m_VM.SelectedStack);
            dlgEditStack.ShowDialog();
        }
    }
}

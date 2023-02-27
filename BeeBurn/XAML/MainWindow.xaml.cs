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
        
        private readonly BeeBurnVM m_VM = null;
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

        private void ClickFilterTest(object sender, RoutedEventArgs e)
        {
            StackFilterWindow sfw = new StackFilterWindow();
            sfw.Tags.Add(new BeeBooleanChoice("TagA", false));
            sfw.Tags.Add(new BeeBooleanChoice("TagB", true));

            sfw.ShowDialog();
        }

        private void ClickLoadImages(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.LoadImagesToStack(m_VM.SelectedStack);
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.SaveAsCollection();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.LoadCollection(true);
        }

        private void ClickEditStack(object sender, RoutedEventArgs e)
        {
            var dlgEditStack = new BeeStackEditor(m_VM.SelectedStack);
            dlgEditStack.ShowDialog();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            BeeConfig bc = new BeeConfig();
            bc.ShowDialog();
        }
    }
}

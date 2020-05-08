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
using System.Windows.Media.Imaging;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickProject(object sender, RoutedEventArgs e)
        {
            if (m_proj == null)
                m_proj = new Projection();
            m_proj.Show();
        }

        private void ClickPaste(object sender, RoutedEventArgs e)
        {
            m_proj.PasteImage();
        }
    }
}

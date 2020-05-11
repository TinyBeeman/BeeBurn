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
using System.Windows.Shapes;

namespace BeeBurn.XAML
{
    /// <summary>
    /// Interaction logic for BeeTagEditor.xaml
    /// </summary>
    public partial class BeeTagEditor : Window
    {
        public BeeTagEditor()
        {
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OkayBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

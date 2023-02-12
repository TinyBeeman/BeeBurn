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
    /// Interaction logic for BeeConfig.xaml
    /// </summary>
    public partial class BeeConfig : Window
    {
        public BeeConfig()
        {
            InitializeComponent();
            DataContext = BeeBurnVM.Get().ConfigSettings;
        }

        private void OnClose(object sender, EventArgs e)
        {
            BeeBurnVM.Get().ConfigSettings.SaveToFile();
        }
    }

}

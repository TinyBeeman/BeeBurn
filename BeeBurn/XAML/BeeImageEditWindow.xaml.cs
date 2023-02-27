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
    /// Interaction logic for BeeImageEditWindow.xaml
    /// </summary>
    public partial class BeeImageEditWindow : Window
    {
        public static void ShowEditImageDlg(BeeImage image)
        {
            var wnd = new BeeImageEditWindow(image);
            wnd.ShowDialog();
        }

        public BeeImageEditWindow(BeeImage image)
        {
            InitializeComponent();
            Image = image;
        }

        public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register("Image",
            typeof(BeeImage),
            typeof(BeeImageEditWindow),
            new PropertyMetadata(null));

        public BeeImage Image
        {
            get
            {
                return (BeeImage)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }
    }
}

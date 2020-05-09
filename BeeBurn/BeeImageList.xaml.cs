using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ImageList.xaml
    /// </summary>
    public partial class BeeImageList : UserControl
    {
        public ObservableCollection<BeeImage> ImageList
        {
            get
            {
                return (ObservableCollection<BeeImage>)GetValue(ImageListProperty);
            }
            set
            {
                SetValue(ImageListProperty, value);
            }
        }

        public static readonly DependencyProperty ImageListProperty =
                DependencyProperty.Register("ImageList",
                typeof(ObservableCollection<BeeImage>),
                typeof(BeeImageList),
                new PropertyMetadata(null));

        public BeeImageList()
        {
            InitializeComponent();
        }
    }
}

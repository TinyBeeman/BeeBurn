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

namespace BeeBurn
{
    /// <summary>
    /// Interaction logic for BeeStackEditor.xaml
    /// </summary>
    public partial class BeeStackEditor : Window
    {
        public BeeStackEditor(BeeStack stack)
        {
            InitializeComponent();
            Stack = stack;
        }

        public BeeStack Stack
        {
            get
            {
                return (BeeStack)GetValue(StackProperty);
            }
            set
            {
                SetValue(StackProperty, value);
            }
        }

        public static readonly DependencyProperty StackProperty =
                DependencyProperty.Register("Stack",
                    typeof(BeeStack),
                    typeof(BeeStackEditor),
                    new PropertyMetadata(null));

        private void ClickLoadImages(object sender, RoutedEventArgs e)
        {
            BeeBurnVM.LoadImagesToStack(Stack);
        }

        private void ClickPaste(object sender, RoutedEventArgs e)
        {
            Stack.PasteImage();
        }
    }
}

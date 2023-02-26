using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeeBurn.XAML
{
    /// <summary>
    /// Interaction logic for ImageList.xaml
    /// </summary>
    public partial class BeeImageGrid : UserControl
    {
        private BeeImage m_dragImg;

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
                    typeof(BeeImageGrid),
                    new PropertyMetadata(null));

        public bool AllowStackEdit
        {
            get { return (bool)GetValue(AllowStackEditProperty); }
            set { SetValue(AllowStackEditProperty, value); }
        }

        public static DependencyProperty AllowStackEditProperty =
            DependencyProperty.Register("AllowStackEdit",
                typeof(bool),
                typeof(BeeImageGrid),
                new PropertyMetadata(true));


        public int SelectionIndex
        {
            get
            {
                return (int)GetValue(SelectionIndexProperty);
            }
            set
            {
                SetValue(SelectionIndexProperty, value);
            }
        }

        public static DependencyProperty SelectionIndexProperty =
        DependencyProperty.Register("SelectionIndex",
            typeof(int),
            typeof(BeeImageGrid),
            new PropertyMetadata(-1));


        public BeeImageGrid()
        {
            InitializeComponent();
        }

        private void BtnRandom_Click(object sender, RoutedEventArgs e)
        {
            Random rng = new Random();
            int n = Stack.Images.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var val = Stack.Images[k];
                Stack.Images[k] = Stack.Images[n];
                Stack.Images[n] = val;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Stack.Images.Clear();
        }

        /*private HashSet<BeeImage> GetSelectedImages()
        {
            var images = new HashSet<BeeImage>();
            for (int iCell = 0; iCell < ActiveGrid.SelectedCells.Count; iCell++)
            {
                int iRow = ActiveGrid.Items.IndexOf(ActiveGrid.SelectedCells[iCell].Item);
                if (iRow != -1)
                {
                    images.Add(Stack.Images[iRow]);
                }
            }
            return images;
        }*/

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            //var images = GetSelectedImages();
            //foreach (BeeImage bi in images)
            //{
                //Stack.Images.Remove(bi);
            //}
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            Stack.PasteImage();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Stack.ResetNextImage();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.LoadImagesToStack(Stack);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            BeeBurnIO.SaveAsStack(Stack);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AllowStackEdit)
            {
                var dlgEditStack = new BeeStackEditor(Stack);
                dlgEditStack.ShowDialog();
            }
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).CaptureMouse();
            Image_MouseLeftButtonDown(sender, e);
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private BeeImage GetImageFromSender(object sender)
        {
            return ((FrameworkElement)sender).DataContext as BeeImage;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int srcId = (int)((Image)e.Source).Tag;
            BeeImage dragImage = GetImageFromSender(sender);
            var dragEffects = DragDropEffects.Move;
            SelectionIndex = Stack.Images.IndexOf(m_dragImg);

            if (DragDrop.DoDragDrop((Image)(e.Source), dragImage, dragEffects) != DragDropEffects.None)
            {
                m_dragImg = dragImage;
            }
        }

        private void ActiveGrid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(BeeImage)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ActiveGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(BeeImage)))
            {
                BeeImage imgDrag = e.Data.GetData(typeof(BeeImage)) as BeeImage;
                if (Stack.Images.Contains(imgDrag))
                {
                    Stack.Images.Remove(imgDrag);
                }
                Stack.Images.Add(imgDrag);
                e.Handled = true;
            }
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(BeeImage)))
            {
                e.Handled = true;

                BeeImage imgDrag = e.Data.GetData(typeof(BeeImage)) as BeeImage;
                BeeImage imgDrop = GetImageFromSender(sender);
                if (imgDrag == imgDrop)
                    return;

                int oldIndex = -1;
                if (Stack.Images.Contains(imgDrag))
                {
                    oldIndex = Stack.Images.IndexOf(imgDrag);
                    Stack.Images.RemoveAt(oldIndex);
                }

                // If the new index and the old index are the same, we should swap, because
                // that means they drug it to the next image.
                int newIndex = Stack.Images.IndexOf(imgDrop);
                Stack.Images.Insert((newIndex == oldIndex) ? newIndex + 1 : newIndex, imgDrag);
            }
        }
    }
}

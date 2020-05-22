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
    public delegate Point GetPosition(IInputElement element);

    /// <summary>
    /// Interaction logic for ImageList.xaml
    /// </summary>
    public partial class BeeImageList : UserControl
    {
        private int m_dragIndex = -1;

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
                    typeof(BeeImageList),
                    new PropertyMetadata(null));

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
            typeof(BeeImageList),
            new PropertyMetadata(-1));


        public BeeImageList()
        {
            InitializeComponent();
        }

        private void ActiveGrid_Drop(object sender, DragEventArgs e)
        {
            if (m_dragIndex < 0)
                return;
            int iInsert = this.GetCurrentRowIndex(e.GetPosition);
            if (iInsert < 0 || iInsert == m_dragIndex)
                return;

            /*if (iInsert == ActiveGrid.Items.Count - 1)
            {
                MessageBox.Show("Why can't this be dropped?");
                return;
            }*/

            var biMoved = Stack.Images[m_dragIndex];
            Stack.Images.RemoveAt(m_dragIndex);
            Stack.Images.Insert(iInsert, biMoved);


        }

        private void ActiveGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_dragIndex = GetCurrentRowIndex(e.GetPosition);
            if (m_dragIndex < 0)
                return;

            ActiveGrid.SelectedIndex = m_dragIndex;
            BeeImage biSel = ActiveGrid.Items[m_dragIndex] as BeeImage;
            if (biSel == null)
                return;
            var dragEffects = DragDropEffects.Move;
            if (DragDrop.DoDragDrop(ActiveGrid, biSel, dragEffects) != DragDropEffects.None)
            {
                ActiveGrid.SelectedItem = biSel;
            }
        }

        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            if (theTarget == null)
                return false;

            var rect = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point point = position((IInputElement)theTarget);
            return rect.Contains(point);
        }


        private DataGridRow GetRowItem(int index)
        {
            if (ActiveGrid.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return ActiveGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        private int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < ActiveGrid.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(i);
                if (itm != null && GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
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

        private HashSet<BeeImage> GetSelectedImages()
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
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var images = GetSelectedImages();
            foreach (BeeImage bi in images)
            {
                Stack.Images.Remove(bi);
            }
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
    }
}

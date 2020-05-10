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

namespace BeeBurn
{
    public delegate Point GetPosition(IInputElement element);

    /// <summary>
    /// Interaction logic for ImageList.xaml
    /// </summary>
    public partial class BeeImageList : UserControl
    {
        private int m_dragIndex = -1;

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

            var biMoved = ImageList[m_dragIndex];
            ImageList.RemoveAt(m_dragIndex);
            ImageList.Insert(iInsert, biMoved);


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
                if (GetMouseTargetRow(itm, pos))
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
            int n = ImageList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var val = ImageList[k];
                ImageList[k] = ImageList[n];
                ImageList[n] = val;
            }
        }
    }
}

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
    public partial class BeeStackList : UserControl
    {
        private int m_dragIndex = -1;

        public ObservableCollection<BeeStack> Stacks
        {
            get
            {
                return (ObservableCollection<BeeStack>)GetValue(StacksProperty);
            }
            set
            {
                SetValue(StacksProperty, value);
            }
        }

        public static readonly DependencyProperty StacksProperty =
                DependencyProperty.Register("Stacks",
                    typeof(ObservableCollection<BeeStack>),
                    typeof(BeeStackList),
                    new PropertyMetadata(null));

        public int SelectedStackIndex
        {
            get
            {
                return (int)GetValue(SelectedStackIndexProperty);
            }
            set
            {
                SetValue(SelectedStackIndexProperty, value);
            }
        }

        public static DependencyProperty SelectedStackIndexProperty =
        DependencyProperty.Register("SelectedStackIndex",
            typeof(int),
            typeof(BeeStackList),
            new PropertyMetadata(-1));


        public BeeStackList()
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

            var biMoved = Stacks[m_dragIndex];
            Stacks.RemoveAt(m_dragIndex);
            Stacks.Insert(iInsert, biMoved);


        }

        private void ActiveGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_dragIndex = GetCurrentRowIndex(e.GetPosition);
            if (m_dragIndex < 0)
                return;

            ActiveGrid.SelectedIndex = m_dragIndex;
            BeeStack bsSel = ActiveGrid.Items[m_dragIndex] as BeeStack;
            if (bsSel == null)
                return;
            var dragEffects = DragDropEffects.Move;
            if (DragDrop.DoDragDrop(ActiveGrid, bsSel, dragEffects) != DragDropEffects.None)
            {
                ActiveGrid.SelectedItem = bsSel;
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
            int n = Stacks.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var val = Stacks[k];
                Stacks[k] = Stacks[n];
                Stacks[n] = val;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Stacks.Clear();
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            int i = SelectedStackIndex;
            if (i >= 0 && i < Stacks.Count)
            {
                Stacks.RemoveAt(i);
                SelectedStackIndex = Math.Min(i, Stacks.Count - 1);
            }
        }
    }
}

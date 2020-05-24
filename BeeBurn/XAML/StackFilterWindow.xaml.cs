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
using System.Windows.Shapes;

namespace BeeBurn.XAML
{
    /// <summary>
    /// Interaction logic for StackFilterWindow.xaml
    /// </summary>
    public partial class StackFilterWindow : Window
    {
        public StackFilterWindow()
        {
            Decades = new ObservableCollection<BeeBooleanChoice>();
            Tags = new ObservableCollection<BeeBooleanChoice>();
            InitializeComponent();
        }

        public ObservableCollection<BeeBooleanChoice> Decades
        {
            get
            {
                return (ObservableCollection<BeeBooleanChoice>)GetValue(DecadesProperty);
            }
            set
            {
                SetValue(DecadesProperty, value);
            }
        }

        public static readonly DependencyProperty DecadesProperty =
                DependencyProperty.Register("Decades",
                    typeof(ObservableCollection<BeeBooleanChoice>),
                    typeof(StackFilterWindow),
                    new PropertyMetadata(null));

        public ObservableCollection<BeeBooleanChoice> Tags
        {
            get
            {
                return (ObservableCollection<BeeBooleanChoice>)GetValue(TagsProperty);
            }
            set
            {
                SetValue(TagsProperty, value);
            }
        }

        public static readonly DependencyProperty TagsProperty =
                DependencyProperty.Register("Tags",
                    typeof(ObservableCollection<BeeBooleanChoice>),
                    typeof(StackFilterWindow),
                    new PropertyMetadata(null));
    }
}

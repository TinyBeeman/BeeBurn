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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeeBurn
{
    /// <summary>
    /// Interaction logic for BeeProgress.xaml
    /// </summary>
    public partial class BeeProgress : UserControl
    {

        public BeeProgress()
        {
            InitializeComponent();
        }

        public double PercentComplete
        {
            get
            {
                return (double)GetValue(PercentCompleteProperty);
            }
            set
            {
                ColDefA.Width = new GridLength(value, GridUnitType.Star);
                ColDefB.Width = new GridLength(1.0 - value, GridUnitType.Star);
                SetValue(PercentCompleteProperty, value);
            }
        }

        public static DependencyProperty PercentCompleteProperty =
        DependencyProperty.Register("PercentComplete",
            typeof(double),
            typeof(BeeProgress),
            new PropertyMetadata(0.0));


    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BeeBurn.XAML
{

    public enum PlayOptions { StopAtEnd, NextList, RepeatThisList }

    /// <summary>
    /// Interaction logic for PresenterView.xaml
    /// </summary>
        public partial class PresenterView : Window
    {
        private Projection m_proj = null;
        private DispatcherTimer m_dispatcherTimer = null;
        bool m_playing = false;

        public PresenterView()
        {
            InitializeComponent();
            DataContext = BeeBurnVM.Get();

            EnsureProjectionWindow();
        }

        public PlayOptions PlayOption
        {
            get
            {
                return (PlayOptions)GetValue(PlayOptionProperty);
            }
            set
            {
                SetValue(PlayOptionProperty, value);
            }
        }

        public static readonly DependencyProperty PlayOptionProperty =
         DependencyProperty.Register("PlayOption",
             typeof(PlayOptions),
             typeof(PresenterView),
             new PropertyMetadata(PlayOptions.NextList, null));

        public delegate void CloseHandler();

        public event CloseHandler OnClose;

        private void EnsureProjectionWindow()
        {
            if (m_proj == null)
            {
                m_proj = new Projection();
                m_proj.OnClose += () => { m_proj = null; };
            }
        }
        private void ProjectionClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_dispatcherTimer.Stop();
            OnClose?.Invoke();
        }
        private void ClickPlayPause(object sender, RoutedEventArgs e)
        {
            m_proj.Show();
            if (!m_playing)
            {
                m_proj.ProjectList(BeeBurnVM.Get().SelectedStack.ActiveImages);
                PlayPauseButton.Content = "Pause";
            }
            else
            {
                PlayPauseButton.Content = "Play";
            }
                

        }

        private void ClickSkip(object sender, RoutedEventArgs e)
        {

        }

        private void ClickPasteNext(object sender, RoutedEventArgs e)
        {

        }

        private void ClickLoadNext(object sender, RoutedEventArgs e)
        {

        }
    }
}

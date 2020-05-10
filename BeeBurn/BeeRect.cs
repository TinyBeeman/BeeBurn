using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeeBurn
{
    public class BeeRect : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private double m_left;
        private double m_top;
        private double m_width;
        private double m_height;

        public double Left
        {
            get => m_left;
            set
            {
                m_left = value;
                OnPropertyChanged();
            }
        }

        public double Top
        {
            get => m_top;
            set
            {
                m_top = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => m_width;
            set
            {
                m_width = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => m_height;
            set
            {
                m_height = value;
                OnPropertyChanged();
            }
        }

        public double Right => Left + Width;
        public double Bottom => Top + Height;



        public BeeRect(double l = 0, double t = 0, double w = 0, double h = 0)
        {
            m_left = l;
            m_top = t;
            m_width = w;
            m_height = h;
        }
    }
}

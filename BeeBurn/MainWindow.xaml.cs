﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeeBurn
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Projection m_proj = null;
        private int m_pasteCounter = 0;

        private ObservableCollection<BeeImage> m_activeImages;
        public ObservableCollection<BeeImage> ActiveImages
        {
            get { return m_activeImages; }
            set { m_activeImages = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickProject(object sender, RoutedEventArgs e)
        {
            if (m_proj == null)
                m_proj = new Projection();
            m_proj.Show();
        }

        private void ClickPaste(object sender, RoutedEventArgs e)
        {
            if (ActiveImages == null)
            {
                ActiveImages = new ObservableCollection<BeeImage>();
            }

            ActiveImages.Add(new BeeImage(BeeClipboard.ImageFromClipboardDib(), m_pasteCounter.ToString()));
            if (m_proj != null)
                m_proj.PasteImage();
        }
    }
}

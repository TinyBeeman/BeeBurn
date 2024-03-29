﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BeeBurn.XAML
{

    /// <summary>
    /// Interaction logic for Projection.xaml
    /// </summary>
    public partial class Projection : Window
    {
        private Image m_imgOld;
        private Image m_imgNew;
        private readonly Random m_rng = new Random();

        private readonly double m_fadeSeconds = BeeBurnVM.Get().ConfigSettings.ImageFadeTime;
        private readonly double m_panSeconds = BeeBurnVM.Get().ConfigSettings.ImagePanTime;

        private Storyboard m_currentStoryboard = null;

        public Projection()
        {
            InitializeComponent();
        }

        private void ProjectionTimerTick(object sender, EventArgs e)
        {
        }

        private BeePresentationStack PresentationStack => BeeBurnVM.Get().PresentationStack;


        public void FadeToBlack()
        {
            if (!(PresentationStack.CurrentImage?.IsStopImage ?? false))
            {
                PresentationStack.EnsureStopImage(true);
                PresentationStack.CycleNextImage();
            }

            QueueCurrentImage();
        }

        public double QueueCurrentImage()
        {
            BeeImage bi = PresentationStack.CurrentImage;

            if (m_imgOld != null)
                GridImage.Children.Remove(m_imgOld);
            
            m_imgOld = m_imgNew;
            m_imgNew = new Image
            {
                Source = bi.Image,

                // Start with 0 opacity
                Opacity = 0,
                Stretch = Stretch.None
            };

            GridImage.Children.Add(m_imgNew);

            DoubleAnimation animFadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(m_fadeSeconds));

            ScaleTransform scaleTransform = new ScaleTransform(1, 1);

            m_imgNew.RenderTransformOrigin = new Point(0, 0);
            m_imgNew.RenderTransform = scaleTransform;

            OffsetScale os1 = bi.GetStartOffsetScale(new BeeRect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));
            OffsetScale os2 = bi.GetEndOffsetScale(new BeeRect(0, 0, GridImage.ActualWidth, GridImage.ActualHeight));

            // Let's animate a random amount of time between 0.25 and 1.0 of the configuration value.
            double mult = (m_rng.NextDouble() * 0.75) + 0.25;
            double panSeconds = mult * m_panSeconds;


            TimeSpan tsAnim = TimeSpan.FromSeconds(panSeconds + m_fadeSeconds);

            m_currentStoryboard = new Storyboard
            {
                Duration = tsAnim
            };

            DoubleAnimation animScaleX = new DoubleAnimation(os1.Scale, os2.Scale, tsAnim);
            DoubleAnimation animScaleY = new DoubleAnimation(os1.Scale, os2.Scale, tsAnim);
            DoubleAnimation animOffsetX = new DoubleAnimation(-os1.OffsetX, -os2.OffsetX, tsAnim);
            DoubleAnimation animOffsetY = new DoubleAnimation(-os1.OffsetY, -os2.OffsetY, tsAnim);

            Storyboard.SetTarget(animScaleX, m_imgNew);
            Storyboard.SetTargetProperty(animScaleX, new PropertyPath("RenderTransform.ScaleX"));
            m_currentStoryboard.Children.Add(animScaleX);

            Storyboard.SetTarget(animScaleY, m_imgNew);
            Storyboard.SetTargetProperty(animScaleY, new PropertyPath("RenderTransform.ScaleY"));
            m_currentStoryboard.Children.Add(animScaleY);

            Storyboard.SetTarget(animOffsetX, m_imgNew);
            Storyboard.SetTargetProperty(animOffsetX, new PropertyPath("RenderTransform.CenterX"));
            m_currentStoryboard.Children.Add(animOffsetX);

            Storyboard.SetTarget(animOffsetY, m_imgNew);
            Storyboard.SetTargetProperty(animOffsetY, new PropertyPath("RenderTransform.CenterY"));
            m_currentStoryboard.Children.Add(animOffsetY);

            if (m_imgOld != null)
            {
                DoubleAnimation animFadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(m_fadeSeconds));
                Storyboard.SetTarget(animFadeOut, m_imgOld);
                Storyboard.SetTargetProperty(animFadeOut, new PropertyPath("Opacity"));
                m_currentStoryboard.Children.Add(animFadeOut);
            }


            Storyboard.SetTarget(animFadeIn, m_imgNew);
            Storyboard.SetTargetProperty(animFadeIn, new PropertyPath("Opacity"));
            m_currentStoryboard.Children.Add(animFadeIn);
            m_currentStoryboard.Duration = new Duration(tsAnim);

            m_currentStoryboard.Begin();

            return panSeconds;
        }

        public void UpdateProgress(double pct)
        {
            Progress.PercentComplete = pct;
        }

        public delegate void CloseHandler();

        public event CloseHandler OnClose;

        private void ProjectionClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnClose?.Invoke();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (BeeBurnVM.Get().ConfigSettings.FullScreen)
                (sender as Window).WindowState = WindowState.Maximized;
            else
                (sender as Window).WindowState = WindowState.Normal;
        }
    }
}

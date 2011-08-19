using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Pomodoro.Animations
{
    public static class ControlAnimationExtensionMethods
    {
        public static void Shake(this UIElement targetControl)
        {
            var shakeAnimation = new DoubleAnimation(0, 5, new Duration(TimeSpan.FromSeconds(5)));
            Storyboard.SetTarget(shakeAnimation, targetControl);
            Storyboard.SetTargetProperty(shakeAnimation, new PropertyPath("RenderTransform.Angle"));
            var sb = new Storyboard();
            sb.Children.Add(shakeAnimation);
            sb.Begin();
        }

        public static void FadeIn(this UIElement targetControl)
        {
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.5)));
            Storyboard.SetTarget(fadeInAnimation, targetControl);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard sb = new Storyboard();
            sb.Children.Add(fadeInAnimation);
            sb.Begin();
        }
 
    }
}
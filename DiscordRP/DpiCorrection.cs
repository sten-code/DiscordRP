using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiscordRP
{
    public class DpiCorrection : Decorator
    {
        public DpiCorrection()
        {
            Loaded += (s, e) =>
            {
                Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22)
                {
                    CenterX = ActualWidth / 2,
                    CenterY = ActualHeight / 2
                };
                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();
                RenderTransform = dpiTransform;
            };
        }
    }
}

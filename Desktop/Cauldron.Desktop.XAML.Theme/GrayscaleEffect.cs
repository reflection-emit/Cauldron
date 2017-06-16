using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Cauldron.XAML.Theme.Resources
{
    internal class GrayscaleEffect : ShaderEffect
    {
        // Original can be found here: http://bursjootech.blogspot.de/2008/06/grayscale-effect-pixel-shader-effect-in.html

        public GrayscaleEffect()
        {
            PixelShader = new PixelShader() { UriSource = new Uri(@"pack://application:,,,/Cauldron.XAML.Theme;component/Resources/GrayscaleEffect.ps") };
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(DesaturationFactorProperty);
        }

        #region Dependency Property Input

        /// <summary>
        /// Identifies the <see cref="Input" /> dependency property
        /// </summary>
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty(nameof(Input), typeof(GrayscaleEffect), 0);

        /// <summary>
        /// Gets or sets the <see cref="Input" /> Property
        /// </summary>
        public Brush Input
        {
            get { return (Brush)this.GetValue(InputProperty); }
            set { this.SetValue(InputProperty, value); }
        }

        #endregion Dependency Property Input

        #region Dependency Property DesaturationFactor

        /// <summary>
        /// Identifies the <see cref="DesaturationFactor" /> dependency property
        /// </summary>
        public static readonly DependencyProperty DesaturationFactorProperty = DependencyProperty.Register(nameof(DesaturationFactor), typeof(double), typeof(GrayscaleEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), GrayscaleEffect.CoerceDesaturationFactor));

        /// <summary>
        /// Gets or sets the <see cref="DesaturationFactor" /> Property
        /// </summary>
        public double DesaturationFactor
        {
            get { return (double)this.GetValue(DesaturationFactorProperty); }
            set { this.SetValue(DesaturationFactorProperty, value); }
        }

        #endregion Dependency Property DesaturationFactor

        private static object CoerceDesaturationFactor(DependencyObject d, object value)
        {
            GrayscaleEffect effect = d as GrayscaleEffect;
            double newFactor = (double)value;

            if (newFactor < 0.0 || newFactor > 1.0)
                return effect.DesaturationFactor;

            return newFactor;
        }
    }
}
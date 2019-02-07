using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Libber_Manager.Controls;
using Windows.UI.Xaml.Controls;

namespace Libber_Manager.Helpers
{
    class VisualGraphics
    {
        public static VisualGraphics _visualGraphicsUControl;

        public VisualGraphics()
        {
            _visualGraphicsUControl = this;            
        }

        public static void doBlurOnElement(Image Element)
        {
            CompositionEffectBrush brush;
            Compositor compositor = ElementCompositionPreview.GetElementVisual(Element).Compositor;

            // we create the effect. 
            // Notice the Source parameter definition. Here we tell the effect that the source will come from another element/object
            var blurEffect = new GaussianBlurEffect
            {
                Name = "Blur",
                Source = new CompositionEffectSourceParameter("background"),
                BlurAmount = 2f,
                BorderMode = EffectBorderMode.Hard,
            };            
            
            // we convert the effect to a brush that can be used to paint the visual layer
            var blurEffectFactory = compositor.CreateEffectFactory(blurEffect);
            brush = blurEffectFactory.CreateBrush();

            // We create a special brush to get the image output of the previous layer.
            // we are basically chaining the layers (xaml grid definition -> rendered bitmap of the grid -> blur effect -> screen)
            var destinationBrush = compositor.CreateBackdropBrush();
            brush.SetSourceParameter("background", destinationBrush);

            // we create the visual sprite that will hold our generated bitmap (the blurred grid)
            // Visual Sprite are "raw" elements so there is no automatic layouting. You have to specify the size yourself
            var blurSprite = compositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2((float)Element.ActualWidth, (float)Element.ActualHeight);
            blurSprite.Brush = brush;

            // we add our sprite to the rendering pipeline
            ElementCompositionPreview.SetElementChildVisual(Element, blurSprite);

            SpriteVisual blurVisual = (SpriteVisual)ElementCompositionPreview.GetElementChildVisual(Element);


            if (blurVisual != null)
            {
                blurVisual.Size = new Vector2((float)Element.ActualWidth, (float)Element.ActualHeight);
            }
        }

        public static void removeBlurOnElement(Image Element)
        {
            ElementCompositionPreview.SetElementChildVisual(Element, null);
        }

        public static void doBlurOnWindow(Grid Element)
        {
            CompositionEffectBrush brush;
            Compositor compositor = ElementCompositionPreview.GetElementVisual(Element).Compositor;

            // we create the effect. 
            // Notice the Source parameter definition. Here we tell the effect that the source will come from another element/object
            var blurEffect = new GaussianBlurEffect
            {
                Name = "Blur",
                Source = new CompositionEffectSourceParameter("background"),
                BlurAmount = 2f,
                BorderMode = EffectBorderMode.Hard,
            };

            // we convert the effect to a brush that can be used to paint the visual layer
            var blurEffectFactory = compositor.CreateEffectFactory(blurEffect);
            brush = blurEffectFactory.CreateBrush();

            // We create a special brush to get the image output of the previous layer.
            // we are basically chaining the layers (xaml grid definition -> rendered bitmap of the grid -> blur effect -> screen)
            var destinationBrush = compositor.CreateBackdropBrush();
            brush.SetSourceParameter("background", destinationBrush);

            // we create the visual sprite that will hold our generated bitmap (the blurred grid)
            // Visual Sprite are "raw" elements so there is no automatic layouting. You have to specify the size yourself
            var blurSprite = compositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2((float)Element.ActualWidth, (float)Element.ActualHeight);
            blurSprite.Brush = brush;

            // we add our sprite to the rendering pipeline
            ElementCompositionPreview.SetElementChildVisual(Element, blurSprite);

            SpriteVisual blurVisual = (SpriteVisual)ElementCompositionPreview.GetElementChildVisual(Element);


            if (blurVisual != null)
            {
                blurVisual.Size = new Vector2((float)Element.ActualWidth, (float)Element.ActualHeight);
            }
        }

        public static void removeBlurOnWindow(Grid Element)
        {
            ElementCompositionPreview.SetElementChildVisual(Element, null);
        }
    }
}

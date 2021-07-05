using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Sprites
{
    public class SpriteBatchEffect 
    {
        int effect;
//        int effectParam;
        BlendState stateSub;
        SpriteBatch batch;
        public GraphicsDevice GraphicsDevice;
#if !WINDOWS_PHONE
        Effect monoFx;
        Effect invertFx;
#endif
        public SpriteBatchEffect(ContentManager content, GraphicsDevice device) 
        {
            // Blend state for Subtract ink effect
            stateSub = new BlendState();
            stateSub.ColorSourceBlend = Blend.SourceAlpha;
            stateSub.AlphaSourceBlend = Blend.SourceAlpha;
            stateSub.ColorDestinationBlend = Blend.One;
            stateSub.AlphaDestinationBlend = Blend.One;
            stateSub.ColorBlendFunction = BlendFunction.ReverseSubtract;
#if !WINDOWS_PHONE
            monoFx = content.Load<Effect>("mono");
            invertFx = content.Load<Effect>("invert");
#endif
            GraphicsDevice = device;
            batch=new SpriteBatch(device);
        }

        public void Begin()
        {
            effect = CSpriteGen.BOP_COPY;
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        }

        public void End()
        {
            batch.End();
        }

        public void SetEffect(int e)
        {
            if (e != effect)
            {
                effect = e;
                switch (effect)
                {
                    case CSpriteGen.BOP_ADD:
                        batch.End();
                        batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                        break;

#if !WINDOWS_PHONE
                    case CSpriteGen.BOP_SUB:
                        batch.End();
                        batch.Begin(SpriteSortMode.Immediate, stateSub);
                        break;
#endif
#if !WINDOWS_PHONE
                    case CSpriteGen.BOP_INVERT:
                        batch.End();
                        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, invertFx);
                        break;
#endif
#if !WINDOWS_PHONE
                    case CSpriteGen.BOP_MONO:
                        batch.End();
                        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, monoFx);
                        break;
#endif
                    default:
                        batch.End();
                        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                        effect = CSpriteGen.BOP_COPY;
                        break;
                }

            }
        }
/*        public void Draw(Texture2D texture, Vector2 position, Nullable<Rectangle> sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            SetEffect(CSpriteGen.BOP_COPY);
            batch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
*/ 
        public void Draw(Texture2D texture, Vector2 position, Nullable<Rectangle> sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, int e, int effectParam)
        {
            if (e!=effect)
                SetEffect(e);
            if (effect == CSpriteGen.BOP_BLEND)
            {
                float coef = (float)((128 - effectParam) / 128.0);
                color *= coef;
            }
            batch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
 
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Nullable<Rectangle>sourceRectangle, Color color, float rotation, Vector2 origin, int e)
        {
            if (e!=effect)
                SetEffect(e);
            batch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteEffects.None, 0);
        }
 
       public void Draw(Texture2D texture, Rectangle destinationRectangle, Nullable<Rectangle>sourceRectangle, Color color)
        {
            SetEffect(CSpriteGen.BOP_COPY);
            batch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Nullable<Rectangle>sourceRectangle, Color color, int effect, int effectParam)
        {
            SetEffect(effect&CSpriteGen.BOP_MASK);
            batch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public void DrawString(SpriteFont font, String s, Vector2 v, Color c)
        {
            SetEffect(CSpriteGen.BOP_COPY);
            batch.DrawString(font, s, v, c);
        }
        public void DrawString(SpriteFont font, String s, Vector2 v, Color c, int e, int effectParam)
        {
            if (e!=effect)
                SetEffect(e);
            
            if (effect==CSpriteGen.BOP_BLEND)
            {
                float coef = (float)((128 - effectParam) / 128.0);
                c*=coef;
            }
            batch.DrawString(font, s, v, c);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace DemoDurb
{
    public class AnimSprite
    {
        public List<Texture2D> frames = new List<Texture2D>();
        public float frame_current = 0;
        public float rate = 0.5f;
        public bool playing;
        public bool dead=false;
        public int carTag;

        public Vector2 position = Vector2.Zero;
        public Vector2 positionLiteral = Vector2.Zero;
        public float angle = 0;

        public AnimSprite(List<Texture2D> Frames, bool Playing,Vector2 Pos)
        {
            frames = Frames;
            playing = Playing;
            position = Pos;
            frame_current = 0;
        }
        public AnimSprite(AnimSprite sprite)
        {
            frames = sprite.frames;
            playing = sprite.playing;
            position = sprite.position;
            frame_current = 0;
        }
        public virtual AnimSprite[] update(AnimSprite[] sprites)
        {
            if (playing)
            {
                frame_current += rate;

                if (frame_current >= frames.Count)
                {
                    frame_current = 0;
                }
            }
            return sprites;
        }
        public void draw(SpriteBatch sb,Camera cam,Texture2D glow,Texture2D shadow)
        {
            positionLiteral = ((position - cam.position) * cam.zoom) + (cam.res / 2);
            positionLiteral = OrbitPivot(cam.res / 2, positionLiteral, cam.angle, 0);

            animGuy driver = this as animGuy;

            sb.Draw(shadow, positionLiteral, null, Color.White, cam.angle,
                new Vector2(shadow.Width, 70) / 2,
                cam.zoom * .25f, SpriteEffects.None, 0);

            if (!driver.zombie)
                if(carTag<0)
                    sb.Draw(glow, positionLiteral, null, Color.SkyBlue, angle + cam.angle,
                         new Vector2(glow.Width, glow.Height) / 2,
                         cam.zoom * 2, SpriteEffects.None, 0);
                else
                    sb.Draw(glow, positionLiteral, null, Game1.cars.cars[carTag].paintColor, angle + cam.angle,
                        new Vector2(glow.Width, glow.Height) / 2,
                        cam.zoom * 2, SpriteEffects.None, 0);
            else
                sb.Draw(glow, positionLiteral, null, Color.Green, angle + cam.angle,
                new Vector2(glow.Width, glow.Height) / 2,
                cam.zoom * 2, SpriteEffects.None, 0);

            sb.Draw(frames[(int)frame_current], positionLiteral, null, Color.White, angle + cam.angle,
                new Vector2(frames[0].Width, frames[0].Height) / 2,
                cam.zoom, SpriteEffects.None, 0);
        }

        private static Vector2 OrbitPivot(Vector2 pivot, Vector2 obj, float angle, float angle_old)
        {
            float dist = Vector2.Distance(pivot, obj);
            Vector2 angCalc = pivot - obj;
            float offset = PointAt(pivot, obj);//Convert.ToSingle(Math.Atan2(angCalc.Y/angCalc.X)); 
            Vector2 output = Vector2.Zero;
            output.X = Convert.ToSingle(dist * Math.Cos(offset + -(angle_old - angle)) + pivot.X);
            output.Y = Convert.ToSingle(dist * Math.Sin(offset + -(angle_old - angle)) + pivot.Y);
            return output;
        }
        private static float PointAt(Vector2 obj, Vector2 target)
        {
            float f_ang;
            f_ang = (Single)Math.Atan2((double)(obj.Y - target.Y), (double)(obj.X - target.X));
            f_ang += (float)Math.PI;
            return f_ang;
        }

    }
}

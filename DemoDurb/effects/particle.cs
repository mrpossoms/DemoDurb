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
    class particle
    {
        public Texture2D texture;
        public Vector2 posLiteral;
        public Vector2 position, velocity;
        public float angle, angVelo;
        public float friction;
        public bool fade = false;
        public behavior Behavior;
        public float size,size_init, sizeFinal;

        public int lived = 0;
        public int life;

        public particle(Texture2D tex, Vector2 pos, Vector2 velo, float angVelocity,behavior PartBehavior,float Friction,float SizeInit,float SizeFinal,int Life,bool Fade)
        {
            texture = tex;
            position = pos;
            velocity = velo;
            angle = 0;
            angVelo = angVelocity;
            Behavior = PartBehavior;
            friction = Friction;
            sizeFinal = SizeFinal;
            size_init=size = SizeInit;
            life = Life;
            fade = Fade;
        }
        public void update()
        {
            velocity *= friction;
            angVelo *= friction;
            position += velocity;
            angle += angVelo;

            switch (Behavior)
            {
                case behavior.grow:
                    size += sizeFinal / life;
                    break;

                case behavior.none:

                    break;

                case behavior.shrink:
                    size -= size_init / life;
                    break;
            }

            lived++;

        }

        public void draw(Camera cam, SpriteBatch sb)
        {
            Color color = new Color(255, 255, 255, 255);
            if (fade)
            {
                float x = Math.Abs(life - lived), y = life;
                float iAlpha = x / y;
                byte alpha = Convert.ToByte(iAlpha * 255);
                color = new Color(255, 255, 255, alpha);
            }


            posLiteral = ((position - cam.position) * cam.zoom) + (cam.res / 2);

            float dist = Vector2.Distance(posLiteral, cam.res / 2);
            posLiteral = OrbitPivot(cam.res / 2, posLiteral, cam.angle, 0);

            sb.Begin();
            sb.Draw(texture, posLiteral, null, color, angle + cam.angle,
                new Vector2(texture.Width, texture.Height) / 2,
                size* cam.zoom, SpriteEffects.None, 0);
            sb.End();
        }
        public enum behavior
        { grow, shrink, none };

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

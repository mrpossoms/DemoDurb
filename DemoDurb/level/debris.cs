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
    class debris
    {
        public Texture2D texture;
        public Vector2 position,posLiteral;
        public float angle;
        public type Type;
        public Color color = Color.White;

        public debris(Texture2D Texture, Vector2 Pos, float Angle,type _type)
        {
            texture = Texture;
            position = Pos;
            angle = Angle;
            Type = _type;
        }

        public void draw( Camera cam,SpriteBatch sb)
        {
            posLiteral = ((position - cam.position) * cam.zoom) + (cam.res / 2);

            float dist = Vector2.Distance(posLiteral, cam.res / 2);
            posLiteral = OrbitPivot(cam.res / 2, posLiteral, cam.angle, 0);

            sb.Begin();
            sb.Draw(texture, posLiteral, null, color, angle + cam.angle,
                new Vector2(texture.Width, texture.Height) / 2,
                cam.zoom, SpriteEffects.None, 0);
            sb.End();

            if (Type == type.center)
            {
                cam.center_literal = posLiteral;
                cam.center = position;
            }

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


        public enum type
        { junk, center };
    }
}

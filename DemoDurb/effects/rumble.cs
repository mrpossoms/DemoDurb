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

    public class rumble
    {
        private static Vector2[] setTime = new Vector2[4];
        private static Vector2[] duration = new Vector2[4];
        private static Vector2[] intensity = new Vector2[4];

        private static GameTime oldGt;

        public static void update(GameTime gt)
        {
            for(int i =0; i!=4;i++)
            {
                float l=0,r=0;
                if (Math.Abs(gt.TotalRealTime.Milliseconds - setTime[i].X) < duration[i].X)
                {
                    l = intensity[i].X;
                }
                else
                {
                    duration[i].X = 0;
                    intensity[i].X = 0;
                }
                if (Math.Abs(gt.TotalRealTime.Milliseconds - setTime[i].X) < duration[i].Y)
                {
                    r = intensity[i].Y;
                }
                else
                {
                    duration[i].Y = 0;
                    intensity[i].Y = 0;
                }
                GamePad.SetVibration((PlayerIndex)i, l, r);
            }
            oldGt = gt;
        }
        public static void addVib(PlayerIndex player, int leftTime, float Lintensity, int rightTime, float Rintensity)
        {
            int i = (int)player;

            if (leftTime > 0)
            {
                duration[i].X = leftTime;
                setTime[i].X = oldGt.TotalRealTime.Milliseconds + leftTime;
                intensity[i].X = Lintensity;
            }
            if (rightTime > 0)
            {
                duration[i].Y = rightTime;
                setTime[i].Y = oldGt.TotalRealTime.Milliseconds + rightTime;
                intensity[i].Y = Rintensity;
            }
        }
    }
}

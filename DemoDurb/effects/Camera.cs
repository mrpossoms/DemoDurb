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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public float zoom = 3;
        float target_zoom = 1f;
        public Vector2 posLiteral = Vector2.Zero;
        public Vector2 res = Vector2.Zero;
        public Vector2 position = new Vector2(0, 0);
        public Vector2 center_literal,center;

        public Vector2 min, max;
        public Vector2 destination;
        public float desiredAng;

        Vector2 velocity = Vector2.Zero;
        float friction = .98f;
        public float angle;

        public Camera(Game game)
            : base(game)
        {
            res = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code
            min = new Vector2(9999, 9999);
            max = new Vector2(-9999, -9999);

            /*
            res = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                Game.GraphicsDevice.PresentationParameters.BackBufferHeight);*/
            //position += (res / 2);
            //position *= zoom;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        public void grabCam(Vector2 pos)
        {
            Game1.cars.LivingCars++;
            destination += pos;

            #region min
            if (pos.X < min.X)
            {
                min.X = pos.X;
            }
            if (pos.Y < min.Y)
            {
                min.Y = pos.Y;
            } 
            #endregion
            #region max
            if (pos.X > max.X)
            {
                max.X = pos.X;
            }
            if (pos.Y > max.Y)
            {
                max.Y = pos.Y;
            } 
            #endregion


        }
        public override void Update(GameTime gameTime)
        {
            if (Game1.ui.state == UI.UIstate.game && !GameStateChecker.gamePaused)
            {
                if (Game1.cars.LivingCars == 0)
                    Game1.cars.LivingCars = Game1.cars.cars.Count;
                destination /= Game1.cars.LivingCars;//Game1.cars.cars.Count;
                if(min!=max && Game1.cars.LivingCars > 1)
                target_zoom = MathHelper.Clamp(400/Vector2.Distance(min, max),
                                               0.75f,
                                               2);

                KeyboardState ks = Keyboard.GetState();
                GamePadState gp = GamePad.GetState(PlayerIndex.One);

                if (ks.IsKeyDown(Keys.OemPlus) || gp.IsButtonDown(Buttons.DPadUp))
                {
                    if(target_zoom < 1.25f)
                    target_zoom += 0.05f;
                }
                if (ks.IsKeyDown(Keys.OemMinus) || gp.IsButtonDown(Buttons.DPadDown))
                {
                    if(target_zoom>.50f)
                    target_zoom -= 0.05f;
                }

                follow(destination, desiredAng);
                // TODO: Add your update code here
                velocity *= friction;
                position += velocity;

                posLiteral = (position) - (res / 2);

                destination = Vector2.Zero;
                Game1.cars.LivingCars = 0;
                min = new Vector2(9999, 9999);
                max = new Vector2(-9999, -9999);
            }
            base.Update(gameTime);
        }
        public void reset()
        {
            position = res / 2;
            posLiteral = Vector2.Zero;
            center = Vector2.Zero;
            center_literal = Vector2.Zero;
            zoom = 1;
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
        public void follow(Vector2 Object,float ang)
        {
            Vector2 temp = Vector2.Zero;// = res / 2;
            Object -= temp;
            float f_velocity = (Vector2.Distance(position, Object) / (240* 0.1f));
            position = TranslateOnAng(position, f_velocity, PointAt(position, Object));

            //f_velocity = (float)((ang - angle)/50);
            //angle += f_velocity;
            angle = desiredAng;

            f_velocity = (float)((target_zoom - zoom) / (720 * 0.1f));
            zoom += f_velocity;
        }
        private Vector2 TranslateOnAng(Vector2 pos, float velocity, float angle)
        {
            pos.X += (float)(velocity * Math.Cos(angle));
            pos.Y += (float)(velocity * Math.Sin(angle));

            return pos;
        }
    }
}
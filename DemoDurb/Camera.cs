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
        public float zoom = 0;
        public Vector2 posLiteral = Vector2.Zero;
        public Vector2 res = Vector2.Zero;
        public Vector2 position = new Vector2(512, 256);
        public Vector2 center;

        public Vector2 destination;
        public float desiredAng;

        Vector2 velocity = Vector2.Zero;
        float friction = .98f;
        public float angle;

        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            res = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                Game.GraphicsDevice.PresentationParameters.BackBufferHeight);
            //position += (res / 2);
            //position *= zoom;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            follow(destination,angle);
            // TODO: Add your update code here
            velocity *= friction;
            position += velocity;

            posLiteral = (position) - (res/2);
            base.Update(gameTime);
        }

        public void follow(Vector2 Object,float ang)
        {
            Vector2 temp = Vector2.Zero;// = res / 2;
            Object -= temp;
            float f_velocity = (Vector2.Distance(position, Object) / (180 * 0.1f));
            position = TranslateOnAng(position, f_velocity, PointAt(position, Object));

            f_velocity = (float)((ang - angle));
            angle += f_velocity;

            f_velocity = (float)(Math.Abs(1.5f - zoom) / (720 * 0.1f));
            zoom += f_velocity;
        }
        private float PointAt(Vector2 obj, Vector2 target)
        {
            float f_ang;
            f_ang = (Single)Math.Atan2((double)(obj.Y - target.Y), (double)(obj.X - target.X));
            f_ang += (float)Math.PI;
            return f_ang;
        }
        private Vector2 TranslateOnAng(Vector2 pos, float velocity, float angle)
        {
            pos.X += (float)(velocity * Math.Cos(angle));
            pos.Y += (float)(velocity * Math.Sin(angle));

            return pos;
        }
    }
}
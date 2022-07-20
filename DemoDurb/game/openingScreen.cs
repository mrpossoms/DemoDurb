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


namespace DemoDurb.game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class openingScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Game game;
        private ContentManager content;
        private SpriteBatch sb;

        private SpriteFont font;
        private Texture2D triangle;

        private string triad = "[Triad_Studios]";
        private char[] output =new char[15];
        private int index = 0;

        private static bool scaleInPlayed=false, spinPlayed = false, scaleDownPlayed = false;

        List<point> points = new List<point>();

        public openingScreen(Game game,GraphicsDevice g,ContentManager contentMan)
            : base(game)
        {
            content =contentMan;
            sb = new SpriteBatch(g);

            this.game = game;
        }

        protected override void LoadContent()
        {
            font = content.Load<SpriteFont>("arialopening");
            triangle = content.Load<Texture2D>("point");

            Vector2 center = Game1.cam.res / 2;
            Vector2 pt1 = new Vector2(-128, 128);
            float ang1 = (float)Math.Atan2(center.Y - (center + pt1).Y, center.X - (center + pt1).X)-MathHelper.PiOver2;

            Vector2 pt2 = new Vector2(128, 128);
            float ang2 = (float)Math.Atan2(center.Y - (center + pt2).Y, center.X - (center + pt2).X) - MathHelper.PiOver2;

            Vector2 pt3 = new Vector2(0, -128);
            float ang3 = (float)Math.Atan2(center.Y - (center + pt3).Y, center.X - (center + pt3).X) - MathHelper.PiOver2;


            points.Add(new point(center - pt1,center+new Vector2(0,center.Y/2)-pt1/2, ang1, triangle));
            points.Add(new point(center - pt2, center + new Vector2(0, center.Y / 2) - pt2 / 2, ang2, triangle));
            points.Add(new point(center - pt3, center + new Vector2(0, center.Y / 2) - pt3 / 2, ang3, triangle));

            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {



            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (point p in points)
            {
                p.update(gameTime);
            }

            if (index < triad.Length)
            {
                if (points[0].doneScaleSpin)
                {
                    string str = Convert.ToString(triad[index]);

                    output[index] = triad[index];

                    if (gameTime.TotalGameTime.Milliseconds % 75 == 0)
                    {
                        if (triad[index] == ']')
                        {
                            Game1.cars.sound.PlayCue("return");
                        }
                        else
                        {
                            Game1.cars.sound.PlayCue("typing");
                        }

                        index++;
                    }
                }
            }
            else
            {
                if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
                {
                    Game1.ui.Enabled = true;
                    Game1.ui.Visible = true;
                    Game1.cars.sound.PlayCue("menuMusic");
                    game.Components.Remove(this);
                }
            }

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sb.Begin();

            string str = "";

            for (int i = 0; i != output.Length; i++)
                if(output[i]!='\0')
                str += output[i];



            sb.DrawString(font, str, (Game1.cam.res / 2) - new Vector2(index*11, Game1.cam.res.Y / 4), Color.White);

                foreach (point p in points)
                {
                    p.draw(sb);
                }
            sb.End();
            base.Draw(gameTime);
        }
        public static void playScaleIn()
        {
            if (scaleInPlayed == false)
            {
                Game1.cars.sound.PlayCue("scaleIn");
                scaleInPlayed = true;
            }
        }
        public static void playSpin()
        {
            if (spinPlayed == false)
            {
                Game1.cars.sound.PlayCue("spin");
                spinPlayed = true;
            }
        }
        public static void playScaleDown()
        {
            if (scaleDownPlayed == false)
            {
                Game1.cars.sound.PlayCue("scaleDown");
                scaleDownPlayed = true;
            }
        }

    }
    public class point
    {
        private float scale, angle;
        private Vector2 position;

        private float targetScale, targetAngle;
        private Vector2 targetPosition;

        public bool doneScaleSpin = false;
        private Texture2D texture;

        public point(Vector2 position,Vector2 target,float angle,Texture2D texture)
        {
            this.targetPosition = target;
            this.position = position;

            scale = 0;
            this.angle = MathHelper.Pi*10;

            this.targetAngle = angle;
            this.targetScale = 1;

            this.texture = texture;
        }
        public void update(GameTime gt)
        {
            if (Math.Abs(scale - targetScale) > 0.1f && !doneScaleSpin)
            {
                this.scale += (targetScale - scale) / 30f;
                game.openingScreen.playScaleIn();
            }
            else if (Math.Abs(angle - targetAngle) > 0.01f && !doneScaleSpin)
            {
                this.angle += (targetAngle - angle) / 15f;
                game.openingScreen.playSpin();
            }
            else
            {
                doneScaleSpin = true;
                this.position.X += (targetPosition.X - position.X) / 30f;
                this.position.Y += (targetPosition.Y - position.Y) / 30f;
                this.scale += (.5f - scale) / 30f;

                game.openingScreen.playScaleDown();
            }
        }
        public void draw(SpriteBatch sb)
        {
            Color color = new Color(Color.White, (byte)(scale*255));

            if(doneScaleSpin)
            {
                color.A = 255;
            }
            sb.Draw(this.texture,
                    position,
                    null,
                    color,
                    angle,
                    new Vector2(this.texture.Width, this.texture.Height) / 2,
                    scale,
                    SpriteEffects.None,
                    0);
        }
    }
}
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
    public class debrisManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sb;

        List<debris> Debris = new List<debris>();

        List<Texture2D> parts = new List<Texture2D>();
        List<Texture2D> paint_chips = new List<Texture2D>();
        List<Texture2D> blood = new List<Texture2D>();
        Texture2D char_mark;

        Texture2D centerMarker;

        public debrisManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        /// 
        protected override void LoadContent()
        {
            centerMarker = Game.Content.Load<Texture2D>("center");
            char_mark = Game.Content.Load<Texture2D>("char");

            for (int i = 0; i != 8; i++)
            {
                parts.Add(Game.Content.Load<Texture2D>("little_parts" + i.ToString()));
            }
            for (int i = 0; i != 4; i++)
            {
                paint_chips.Add(Game.Content.Load<Texture2D>("paint_chips" + i.ToString()));
            }
            for (int i = 0; i != 5; i++)
            {
                blood.Add(Game.Content.Load<Texture2D>("blood_" + i.ToString()));
            }

            Debris.Add(new debris(centerMarker,Game1.cam.res/2,0, debris.type.center));

            base.LoadContent();
        }
        public void clearAll()
        {
            Debris.Clear();
            Debris.Add(new debris(centerMarker, Game1.cam.res / 2, 0, debris.type.center));
            /*
            int i = 0;
            while (Debris.Count > i)
            {
                debris d = Debris[i];

                if (d.Type != debris.type.center)
                {
                    Debris.Remove(d);
                }
                i++;
            }
             */
        }
        public void addCharMark(Vector2 pos)
        {
            debris temp = new debris(char_mark, pos, RandomNumber(0, 3.14159), debris.type.junk);
            temp.color = Color.White;
            Debris.Add(temp);
        }
        public void addBlood(Vector2 pos)
        {
            debris temp = new debris(blood[roundUp(RandomNumber(0,4))], pos, RandomNumber(0, 3.14159), debris.type.junk);
            temp.color = Color.White;
            Debris.Add(temp);
        }
        public void addPaintChips(Vector2 pos, Color color)
        {
            debris temp = new debris(paint_chips[roundUp(RandomNumber(0, 3))], pos, RandomNumber(0, 3.14159), debris.type.junk);
            temp.color = color;
            Debris.Add(temp);
        }
        public void addParts(Vector2 pos)
        {
            debris temp = new debris(parts[roundUp(RandomNumber(0, 7))], pos, RandomNumber(0, 3.14159), debris.type.junk);
            Debris.Add(temp);
        }
        public override void Initialize()
        {
            sb = new SpriteBatch(Game.GraphicsDevice);
            // TODO: Add your initialization code here

            base.Initialize();
        }
        public override void Draw(GameTime gameTime)
        {
            if (Game1.ui.state == UI.UIstate.game ||
                Game1.ui.state == UI.UIstate.gameOver)
            {
                foreach (debris d in Debris)
                {
                    d.draw(Game1.cam, sb);
                }
                base.Draw(gameTime);
            }
        }
        public static int roundUp(float input)
        {
            int Out;
            int NonDecimal = (int)input;
            if (input - NonDecimal >= .5f)
            {
                Out = 1 + (int)input;
            }
            else
            {
                Out = (int)input;
            }
            return Out;
        }
        public static float RandomNumber(double min, double max)
        {
            float OUT = (float)((max - min) * m_Rand.NextDouble() + min);
            return OUT;
        }
        private static Random m_Rand = new Random();
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
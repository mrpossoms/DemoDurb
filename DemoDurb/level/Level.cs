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
    public class Level : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sb;

        Texture2D[] levelTexture = new Texture2D[3];
        float[] friction = new float[3];
        levelType selectedLevel;


        public Level(Game game)
            : base(game)
        {
            friction[0] = .96f;
            friction[1] = .950f;
            friction[2] = .97f;
          

            selectedLevel = levelType.dirt;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            sb = new SpriteBatch(GraphicsDevice);

            levelTexture[0] = Game.Content.Load<Texture2D>("level0");
            levelTexture[1] = Game.Content.Load<Texture2D>("level1");
            levelTexture[2] = Game.Content.Load<Texture2D>("level2");
            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (Game1.ui.state == UI.UIstate.game ||
                Game1.ui.state == UI.UIstate.gameOver)
            {
                Vector2 pos = ((Vector2.Zero - Game1.cam.position) * Game1.cam.zoom) + (Game1.cam.res / 2);//((Game1.cam.position) * Game1.cam.zoom) + (Game1.cam.res / 2);
                //pos = pos;
                //pos.X = -pos.X/(1-Game1.cam.zoom);

                sb.Begin();
                sb.Draw(levelTexture[(int)selectedLevel], Game1.cam.res / 2, null, Color.White, Game1.cam.angle, (new Vector2(levelTexture[(int)selectedLevel].Width, levelTexture[(int)selectedLevel].Height) / 2) + Game1.cam.posLiteral, Game1.cam.zoom, SpriteEffects.None, 0);
                sb.End();
            }
        }
        public enum levelType
        { brick, dirt, ice };
        public float getFriction()
        {
            return friction[(int)selectedLevel];
        }
    }
}
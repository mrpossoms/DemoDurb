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
    public class AnimManager : DrawableGameComponent
    {
        SpriteBatch sb;

        List<AnimSprite> sprites = new List<AnimSprite>();

        List<Texture2D> driver = new List<Texture2D>();
        Texture2D glow, shadow;

        public AnimManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }
        protected override void LoadContent()
        {
            glow = Game.Content.Load<Texture2D>("driver_color");
            shadow = Game.Content.Load<Texture2D>("shadow");

            for (int i = 0; i != 12; i++)
            {
                driver.Add(Game.Content.Load<Texture2D>("driver"+i.ToString()));
            }
            //for (int i = 0; i != 20; i++)
            //{
                //addDriver(new Vector2(i * 20, 256), car.controlType.network, 2);
            //}
            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        /// 
        public void addDriver(Vector2 pos,car.controlType ct,int carTag)
        {
            sprites.Add(new animGuy(new AnimSprite(driver, false, pos), ct,carTag));
        }
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            sb = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            bool updating = true;
            int i=0;
            while(updating && i<sprites.Count)
            {
                sprites[i].update();
                if (sprites[i].remove)
                {
                    sprites.Remove(sprites[i]);
                }
                else
                {
                    i++;
                    //if (i >= sprites.Count)
                    //    updating = false;
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            sb.Begin(SpriteBlendMode.AlphaBlend);
            foreach (AnimSprite a in sprites)
            {
                a.draw(sb,Game1.cam,glow,shadow);
            }
            sb.End();
            base.Draw(gameTime);
        }
    }
}
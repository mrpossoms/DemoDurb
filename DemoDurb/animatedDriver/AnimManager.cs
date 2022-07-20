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

        public List<AnimSprite> sprites = new List<AnimSprite>();

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
        public int findDriver(string gamerTag)
        {
            int _out = -1;
            foreach(animGuy g in sprites)
            {
                if (g.gamerTag == gamerTag)
                {
                    _out = sprites.IndexOf(g);
                }
            }
            return _out;
        }
        public void addDriver(Vector2 pos,car.controlType ct,int carTag,string GamerTag)
        {
            sprites.Add(new animGuy(new AnimSprite(driver, false, pos), ct,carTag,GamerTag));
        }
        public void addDriver(Vector2 pos, car.controlType ct, int carTag, string GamerTag, bool isZombie)
        {
            sprites.Add(new animGuy(new AnimSprite(driver, false, pos), ct, carTag, GamerTag, isZombie));
        }
        public void addDriver(Vector2 pos, car.controlType ct, int carTag, string GamerTag,bool isZombie,bool isActive)
        {
            sprites.Add(new animGuy(new AnimSprite(driver, false, pos), ct, carTag, GamerTag, isActive, isZombie));
        }
        public void SpawnDriver(Vector2 pos,string GamerTag)
        {
            int d =findDriver(GamerTag);

            animGuy driver = sprites[d] as animGuy; 

            if((!sprites[d].dead || driver.zombie) && lessThan(sprites[d].position,new Vector2(-990,-998)))
            sprites[d].position = pos;
            driver.active = true;
        }
        public void KillDriver(string GamerTag)
        {
            int d = findDriver(GamerTag);

            sprites[d].position = new Vector2(-999, -999);

            animGuy driver = sprites[d] as animGuy;
            driver.active = false;
        }
        public void KillDriverTag(string GamerTag)
        {
            int d = findDriver(GamerTag);

            sprites[d].position = new Vector2(-999, -999);
            sprites[d].positionLiteral = new Vector2(-999, -999);

            animGuy driver = sprites[d] as animGuy;
            driver.dead = true;
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
                AnimSprite[] sArray = new AnimSprite[sprites.Count];
                sprites.CopyTo(sArray);

                sArray = sprites[i].update(sArray);

                sprites.Clear();
                foreach (AnimSprite s in sArray)
                {
                    sprites.Add(s);
                }

                    i++;
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
        private bool lessThan(Vector2 v1, Vector2 v2)
        {
            if(v1.X<v2.X)
                if (v1.Y < v2.X)
                {
                    return true;
                }
            return false;
        }
    }
}
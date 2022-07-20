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
    public class TitleSafe : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Rectangle safeArea;
        RenderTarget2D rt;
        GraphicsDevice graphics;
        SpriteBatch sb;
        ResolveTexture2D tex;

        public TitleSafe(Game game,int w,int h)
            : base(game)
        {
            graphics = game.GraphicsDevice;
            safeArea = game.GraphicsDevice.Viewport.TitleSafeArea;
            sb = new SpriteBatch(game.GraphicsDevice);
            rt = new RenderTarget2D(game.GraphicsDevice,
                                    w,
                                    h,
                                    1,
                                    SurfaceFormat.Color);
            tex = new ResolveTexture2D(graphics,
                                       w,
                                       h, 1,
                                       SurfaceFormat.Color);
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            /*
            graphics.SetRenderTarget(0, rt);
            //graphics.Clear(Color.TransparentBlack);
            graphics.SetRenderTarget(0, null);

            Texture2D tex = rt.GetTexture();

            graphics.SetRenderTarget(0, rt);
            */


            graphics.ResolveBackBuffer(tex,0);

            sb.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                graphics.Clear(Color.Black);
                sb.Draw(tex, safeArea, Color.White);
            sb.End();

            //graphics.SetRenderTarget(0, null);

            base.Draw(gameTime);
        }
    }
}
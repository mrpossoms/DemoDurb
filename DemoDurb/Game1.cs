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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D backGround;

        public static networkManager networkMan;
 
        public static Camera cam;
        public static particleManager particles;
        public static UI ui;
        public static debrisManager debris;
        public static GameMode gameMode;
        GameStateChecker GSchecker;
        //public static net network;
        public static Level level;
        public static AnimManager animSprites;
        public static CarManager cars;
        public static game.openingScreen opening;
        TitleSafe safeArea;

        public static bool BGActive;
        BloomComponent bloom;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
//#if XBOX360
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
//#endif
            #if XBOX360
                        graphics.PreferredBackBufferHeight = 720;
                        graphics.PreferredBackBufferWidth = 1280;
            #endif
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            //TargetElapsedTime = TimeSpan.FromSeconds(1 / 60.0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //#if XBOX360
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 640;
            
            // TODO: Add your initialization logic here
            //network = new net(true);
            //Game1.network = new net(true);
            //Game1.network.add(new NN.layer(4, 1, NN.layerType.input));
            //Game1.network.add(new NN.layer(4, 4, NN.layerType.hidden));
            //Game1.network.add(new NN.layer(3, 4, NN.layerType.output));
            //menuInput.init();

            networkMan = new networkManager(this);
            cam = new Camera(this);
            level = new Level(this);
            animSprites = new AnimManager(this);
            cars = new CarManager(this);
            debris = new debrisManager(this);
            particles = new particleManager(this);
            ui = new UI(this);
            ui.state = UI.UIstate.main_menu;
            bloom = new BloomComponent(this);
            safeArea = new TitleSafe(this, (int)cam.res.X, (int)cam.res.Y);
            opening = new DemoDurb.game.openingScreen(this, GraphicsDevice, Content);

            this.Components.Add(new GamerServicesComponent(this));
            this.Components.Add(new GameStateChecker(this));
            this.Components.Add(networkMan);
            this.Components.Add(cam);
            this.Components.Add(level);
            this.Components.Add(debris);
            this.Components.Add(particles);
            this.Components.Add(animSprites);
            this.Components.Add(cars);
            this.Components.Add(ui);
            this.Components.Add(opening);
            this.Components.Add(bloom);
            this.Components.Add(safeArea);
            base.Initialize();

            /*
            List<string> buttons = new List<string>();
            buttons.Add("BO!");
            buttons.Add("NO YOU!");
            Guide.BeginShowMessageBox("BO", "BOBOBOBOBO BO PEEP!", buttons, 0, MessageBoxIcon.Warning, null, null);*/
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backGround = Content.Load<Texture2D>("background");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (ui.state != UI.UIstate.game && ui.state != UI.UIstate.gameOver)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(backGround, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

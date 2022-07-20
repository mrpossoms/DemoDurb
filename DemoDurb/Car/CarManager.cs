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
    public class CarManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch sb;
        public AudioEngine audio;
        public WaveBank waves;
        public SoundBank sound;

        public List<car> cars = new List<car>();

        SpriteFont font;
        public gameState game_state = gameState.playing;
        public int LivingCars = 0;
        int ticks = 0;
        public List<Texture2D> body = new List<Texture2D>();
        public List<Texture2D> windows = new List<Texture2D>();
        Texture2D frame,shadow,auora;

        public CarManager(Game game)
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
            frame = Game.Content.Load<Texture2D>("frame");
            shadow = Game.Content.Load<Texture2D>("shadow");
            auora = Game.Content.Load<Texture2D>("driver_color"); 
                        for(int i = 0; i!= 5;i++)
                            windows.Add(Game.Content.Load<Texture2D>("car_windows"+i.ToString()));
                        for(int i = 0; i!= 8;i++)
                            body.Add(Game.Content.Load<Texture2D>("car_body"+i.ToString()));
            font = Game.Content.Load<SpriteFont>("arial");

            audio = new AudioEngine(Game.Content.RootDirectory + "/sounds/demoDurb.xgs");
            waves = new WaveBank(audio, Game.Content.RootDirectory + "/sounds/Wave Bank.xwb");
            sound = new SoundBank(audio, Game.Content.RootDirectory + "/sounds/Sound Bank.xsb");


           base.LoadContent();
        }
        public void initGame(bool isMultiplayer)
        {
            if (!isMultiplayer)
            {
                cars.Clear();
                Game1.animSprites.sprites.Clear();
                Game1.debris.clearAll();
                Game1.ui.countDown = 0;
                Game1.ui.hasStarted = false;
            }
            else
            {
                Game1.ui.countDown = 600;
                Game1.ui.hasStarted = true;
            }
        }

        public void start_singlePlayer()
        {
            
            if (cars.Count == 0)
            {


                for (int i = 0; i != 3; i++)
                { spawn_car_AI(); }

                foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
                {
                    //GamePadState temp = GamePad.GetState(g.PlayerIndex);
                    
                #if XBOX360
                    if(gamePlayer.isConnected(g))
                    spawn_car_local(0, g.PlayerIndex,g.Gamertag);
                #else
                    spawn_car_local(0, g.PlayerIndex, g.Gamertag);
                #endif
                }
            }
        }
        public void start_Zombie()
        {

            if (cars.Count == 0)
            {
                cars.Clear();
                Game1.animSprites.sprites.Clear();
                Game1.debris.clearAll();
                Game1.ui.countDown = 0;
                Game1.ui.hasStarted = false;

                //for (int i = 0; i != 3; i++)
                //{ spawn_car_AI(); }

                //foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
                {
                    //GamePadState temp = GamePad.GetState(g.PlayerIndex);

//#if XBOX360
                  //if(temp.IsConnected)
                    //if (Game1.cars.cars.Count < 1)
                    {
                        spawn_car_local(0, GameStateChecker.getProtector(), GameStateChecker.getGamerTag(GameStateChecker.getProtector()));
                    }
                    //else
                    {
                        Game1.animSprites.addDriver(new Vector2(Game1.cam.res.X / 2,
                                                                Game1.cam.res.Y / 2),
                                                                car.controlType.local,
                                                                -1,
                                                                GameStateChecker.getGamerTag(GameStateChecker.getSurvivor()),
                                                                false,
                                                                true
                                                                );
                        ModeZombie mode = Game1.gameMode as ModeZombie;
                        mode.setSurvivor(Game1.animSprites.sprites[Game1.animSprites.sprites.Count - 1] as animGuy);

                    }
//#else
                    /*
                    //if (Game1.cars.cars.Count < 1)
                    {
                        spawn_car_local(0, GameStateChecker.getProtector(), GameStateChecker.getGamerTag(GameStateChecker.getProtector()));
                    }
                    //else
                    {
                        Game1.animSprites.addDriver(new Vector2(Game1.cam.res.X / 2,
                                                                Game1.cam.res.Y / 2),
                                                                car.controlType.local,
                                                                -1,
                                                                GameStateChecker.getGamerTag(GameStateChecker.getSurvivor()));
                        ModeZombie mode = Game1.gameMode as ModeZombie;
                        mode.setSurvivor(Game1.animSprites.sprites[Game1.animSprites.sprites.Count-1] as animGuy);
                    }
//#endif*/
                }

                for (int i = 0; i != 50; i++)
                {
                    Game1.animSprites.addDriver(new Vector2(-999, -999), car.controlType.ai, 0, i.ToString(), true);
                }
            }
        }
        public void start_driverTest()
        {
            if (cars.Count == 0)
            {
                cars.Clear();
                Game1.animSprites.sprites.Clear();
                Game1.debris.clearAll();
                Game1.ui.countDown = 0;
                Game1.ui.hasStarted = false;

                //spawn_car_local(0, PlayerIndex.One, "MrPossoms");

                for(int i =0;i!=20;i++)
                {
                    Game1.animSprites.addDriver(new Vector2(-999, -999), car.controlType.ai, 0,i.ToString());
                }
            }
        }
        public car find_car(int tag)
        {
            car _out = null;

            foreach (car c in cars)
            {
                if (c.tag == tag)
                {
                    _out = c;
                }
            }
            return _out;
        }
        public car find_car(string gamertag)
        {
            car _out = null;

            foreach (car c in cars)
            {
                if (c.gamertag == gamertag)
                {
                    _out = c;
                }
            }
            return _out;
        }
        public int int_find_car(int tag)
        {
            int _out = -1;

            for(int i =0; i!=cars.Count;i++)
            {
                if (cars[i].tag == tag)
                {
                    _out = i;
                }
            }
            return _out;
        }
        public int int_find_car(string gamertag)
        {
            int _out = -1;

            for (int i = 0; i != cars.Count; i++)
            {
                if (cars[i].gamertag == gamertag)
                {
                    _out = i;
                }
            }
            return _out;
        }
        public void spawn_car_AI()
        {
            int count = cars.Count;

            Game1.animSprites.addDriver(new Vector2(-999, -999), car.controlType.ai, count, count.ToString());
            cars.Add(new car(body[0],
                 windows[0],
                 frame,
                 auora,
                 new Vector2(170 * (float)Math.Cos((cars.Count)) + Game1.cam.res.X / 2,
                             170 * (float)Math.Sin((cars.Count)) + Game1.cam.res.Y / 2),
                             RandomNumber(0, Math.PI),
                 new Color((byte)RandomNumber(0, 255),
                           (byte)RandomNumber(0, 255),
                           (byte)RandomNumber(0, 255)),
                 car.controlType.ai, font, count, 100, null, PlayerIndex.One, count.ToString()));
        }
        public void spawn_car_AI(Vector2 position)
        {
            int count = cars.Count;

            Game1.animSprites.addDriver(new Vector2(-999, -999), car.controlType.ai, count, count.ToString());
            cars.Add(new car(body[0],
                 windows[0],
                 frame,
                 auora,
                 position,
                             RandomNumber(0, Math.PI),
                 new Color((byte)RandomNumber(0, 255),
                           (byte)RandomNumber(0, 255),
                           (byte)RandomNumber(0, 255)),
                 car.controlType.ai, font, count, 100, null, PlayerIndex.One, count.ToString()));
        }
        public void spawn_car_network(int tag,string GamerTag)
        {
            Color paint;
            try
            {
                paint = new Color((byte)(GamerTag[0] - GamerTag[1]), (byte)(GamerTag[2] - GamerTag[3]), (byte)(GamerTag[4] - GamerTag[5]));
            }
            catch
            {
                paint = new Color((byte)(GamerTag[0] * 2), (byte)(GamerTag[0] - 2), (byte)(GamerTag[0] / 2));
            }

            int count = cars.Count;

            Game1.animSprites.addDriver(new Vector2(-999, -999), car.controlType.network, count, GamerTag);
            cars.Add(new car(windows[0],
                 windows[0],
                 frame,
                 auora,
                 new Vector2(170 * (float)Math.Cos((cars.Count)) + Game1.cam.res.X / 2,
                             170 * (float)Math.Sin((cars.Count)) + Game1.cam.res.Y / 2),//*Game1.cam.zoom,
                 0,
                 paint, car.controlType.network, font, count, 100, null, PlayerIndex.One, GamerTag));
        }
        public void spawn_car_local(int tag, PlayerIndex playerIndex, string GamerTag)
        {
            Color paint;
            try
            {
            paint = new Color((byte)(GamerTag[0] - GamerTag[1]), (byte)(GamerTag[2] - GamerTag[3]), (byte)(GamerTag[4] - GamerTag[5]));
            }
            catch{
                paint = new Color((byte)(GamerTag[0] *2), (byte)(GamerTag[0] - 2), (byte)(GamerTag[0] /2));
            }
            int count = cars.Count;
            Game1.animSprites.addDriver(new Vector2(-999, -999), car.controlType.local, count, GamerTag);
            cars.Add(new car(windows[0],
                 windows[0],
                 frame,
                 auora,
                 new Vector2(170 * (float)Math.Cos((cars.Count)) + Game1.cam.res.X / 2,
                             170 * (float)Math.Sin((cars.Count)) + Game1.cam.res.Y / 2),//*Game1.cam.zoom,
                 0,
                 paint, car.controlType.local, font, count, 100, null, playerIndex, GamerTag));
            Game1.ui.huds[(int)playerIndex].index = playerIndex;
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
            audio.SetGlobalVariable("volumn", 100f);

            rumble.update(gameTime);

            if (Game1.ui.state == UI.UIstate.game ||
                Game1.ui.state == UI.UIstate.gameOver)
            {
                // TODO: Add your update code here

                int i = 0;
                while (cars.Count > i)
                {
                    car[] cArray = new car[cars.Count];
                    cars.CopyTo(cArray); //get a list of all the other cars

                    cArray = cars[i].Update(cArray);

                    cars.Clear();
                    foreach (car item in cArray)
                    {
                        cars.Add(item);
                    }

                    i++;
                }
              
                //Stop the tires squealing for all cars
                if (Game1.ui.state == UI.UIstate.gameOver)
                    for (int j = 0; j != cars.Count; j++)
                        cars[j].squeal.Pause();
                ////////////////////////////////////////////////////////////////////////////////UNDO
                audio.Update();
                base.Update(gameTime);
            }

        }

        public override void Draw(GameTime gameTime)
        {
            if (Game1.ui.state == UI.UIstate.game ||
                Game1.ui.state == UI.UIstate.gameOver)
            {
                foreach (car c in cars)
                {
                    c.draw(Game1.cam, sb,shadow);
                }
                base.Draw(gameTime);
            }
        }

        public Vector2 randomPos(Vector2 min, Vector2 max)
        {
            Vector2 output = Vector2.Zero;
            output.X = Convert.ToSingle(RandomNumber(min.X, max.X));
            output.Y = Convert.ToSingle(RandomNumber(min.Y, max.Y));
            return output;
        }
        public float RandomNumber(double min, double max)
        {
            float OUT = (float)((max - min) * m_Rand.NextDouble() + min);
            return OUT;
        }
        private Random m_Rand = new Random();

        public enum gameState
        { playing, learning };
    }
}
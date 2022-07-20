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
    public class UI : DrawableGameComponent
    {

        public float countDown = 0;
        public bool hasStarted;

        SpriteBatch sb;

        Texture2D ui_window;
        public static Texture2D ui_kills, ui_score, ui_retry, ui_mainmenu, ui_win, ui_lose;
        public static Texture2D ui_hs_kills, ui_hs_score, ui_hs_deaths, ui_hs_kds;
        public static Texture2D win_help;
        public static Texture2D me;

        static Texture2D spedometer_gauge;
        static Texture2D spedometer_needle;
        static Texture2D spedometer_healthBar;
        Texture2D fade;
        Texture2D ui_oob;
        Texture2D main_menu,mm_title;
        Texture2D mm_quit, mm_help, mm_controls;

        Texture2D modeZombie, modeTag, modeDemo;

        SpriteFont font;
        List<Texture2D> PNameBG = new List<Texture2D>();
        List<Texture2D> count_down = new List<Texture2D>();
        List<Texture2D> digits = new List<Texture2D>();

        private static string MMselection ="";
        public static window scoreBoard;
        public static window postGame;
        public static message Message;

        static float fade_time=101;
        static gameModes fade_gameMode;
        static bool fade_isMultiplayer;
        float main_menu_angle = 0;
        int delay = 0;
        int oobTime = 0;
        static UIstate fade_change = UIstate.none;

        int mm_selection=0;
        int mm_last_selection = 0;
        float[] angle_list = { 0, MathHelper.PiOver2, MathHelper.Pi, MathHelper.PiOver2 + MathHelper.Pi };
        float angleToAdd = 0;

        //float main_menu_angle = 0;
        public hud[] huds = new hud[4];

        IAsyncResult result = null;
        StorageDevice device;

        public UIstate state;

        public UI(Game game)
            : base(game)
        {}
        protected override void LoadContent()
        {
            ui_window = Game.Content.Load<Texture2D>("ui_window");
            ui_kills = Game.Content.Load<Texture2D>("ui_kills");
            ui_score = Game.Content.Load<Texture2D>("ui_score");
            ui_retry = Game.Content.Load<Texture2D>("ui_SelRetry");
            ui_mainmenu = Game.Content.Load<Texture2D>("ui_SelMm");
            ui_win = Game.Content.Load<Texture2D>("ui_titleWin");
            ui_lose = Game.Content.Load<Texture2D>("ui_titleLose");

            fade = Game.Content.Load<Texture2D>("null");
            ui_oob = Game.Content.Load<Texture2D>("ui_oob");
            main_menu = Game.Content.Load<Texture2D>("ui_mm");
            mm_title = Game.Content.Load<Texture2D>("mm_title1");
            spedometer_gauge = Game.Content.Load<Texture2D>("spedo_gauge");
            spedometer_needle = Game.Content.Load<Texture2D>("spedo_needle");
            spedometer_healthBar = Game.Content.Load<Texture2D>("health_bar");

            modeDemo = Game.Content.Load<Texture2D>("modeDemo");
            modeTag = Game.Content.Load<Texture2D>("modeTag");
            modeZombie = Game.Content.Load<Texture2D>("modeZomb");

            ui_hs_deaths = Game.Content.Load<Texture2D>("hs_deaths");
            ui_hs_kds = Game.Content.Load<Texture2D>("hs_kd");
            ui_hs_kills = Game.Content.Load<Texture2D>("hs_kills");
            ui_hs_score = Game.Content.Load<Texture2D>("hs_score");
            mm_quit = Game.Content.Load<Texture2D>("quit");
            mm_help = Game.Content.Load<Texture2D>("help");
            mm_controls = Game.Content.Load<Texture2D>("controls");
            win_help = Game.Content.Load<Texture2D>("helpIco");
            me = Game.Content.Load<Texture2D>("kirk");

            font = Game.Content.Load<SpriteFont>("postgame");
            Message = new message("", font);

            for (int i = 0; i != 4; i++)
            {
                count_down.Add(Game.Content.Load<Texture2D>("ui_" + i.ToString()));
            }
            for (int i = 0; i != 5; i++)
            {
                PNameBG.Add(Game.Content.Load<Texture2D>("blood_" + i.ToString()));
            }
            for (int i = 0; i != 10; i++)
            {
                digits.Add(Game.Content.Load<Texture2D>("ui_digit_" + i.ToString()));
            }
            digits.Add(Game.Content.Load<Texture2D>("ui_plus"));
            digits.Add(Game.Content.Load<Texture2D>("ui_minus"));

            Vector2 windowBounds = new Vector2(ui_window.Width,ui_window.Height);
          
            //postGame = new window((Game1.cam.res / 2) - (windowBounds / 2),
            //                      "postGame",
            //                      false);
            Game1.cars.audio.SetGlobalVariable("musicVolume",1f);

           base.LoadContent();
           this.Enabled = false;
           this.Visible = false;
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            sb = new SpriteBatch(Game.GraphicsDevice);

            /*if (!Guide.IsVisible)
            {
                result = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One,
                            null, null);
                device = Guide.EndShowStorageDeviceSelector(result);

            }*/

            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);


            base.Initialize();
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            e.Gamer.Tag = new gamePlayer(e.Gamer.Gamertag,e.Gamer.GetProfile().GamerPicture, Game.Content,device);
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void loadPlayers(IAsyncResult result)
        {
            device = Guide.EndShowStorageDeviceSelector(result);

            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);

            for (int i = 0; i != SignedInGamer.SignedInGamers.Count; i++)
            {
                SignedInGamer g = SignedInGamer.SignedInGamers[i];
                g.Tag = new gamePlayer(g.Gamertag, g.GetProfile().GamerPicture, Game.Content, device);
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            Message.update();

            if (!Guide.IsVisible && device ==null)
            {
                //this creates a delegate which is called when the device has been selected
                AsyncCallback callback = new AsyncCallback(loadPlayers);

                result = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One,
                callback, null);

                

                //device = Guide.EndShowStorageDeviceSelector(result);
                
                //This chunk of code may be better off in another method
                //perhaps then it could be called when the storage device has been
                //selected

                //////////////////////////////////////////////////////////////
            }
            if (GameStateChecker.gamePaused)
            {
                GameStateChecker.timePaused = gameTime.TotalGameTime.Seconds - GameStateChecker.timeOffset;
            }

            switch (state)
            {
                case UIstate.credits:

                    oobTime++;
                    if(oobTime<10)
                    Message.newMessage("[Kirk Roerig] \n :Designer \n :Artist \n :Programmer",me);

                    if (oobTime > 150 && fade_change != UIstate.main_menu)
                    {
                        oobTime = 0;
                        FadeOut(UIstate.main_menu, false,gameModes.none);
                    }

                    break;
                case UIstate.game:
                case UIstate.gameOver:

                        UpdateGame(gameTime);

                    break;
                case UIstate.intro:

                    break;
                case UIstate.lobby:

                    break;
                case UIstate.main_menu:
                    if (!Guide.IsVisible || SignedInGamer.SignedInGamers.Count > 0)
                    {
                        if (!Guide.IsVisible)
                            gamePlayer.openGamerSignin();

                        UpdateMainMenu(gameTime);
                        postGame.update();
                    }
                    else
                    {
                        Message.newMessage("Please Sign in to continue");
                    }
                    break;
            }
            base.Update(gameTime);
        }
        public void updateHud(PlayerIndex index,float speed1,float hp,int score,Color paint)
        {
            //for(int i=0;i!=HUD.Count;i++)
            //{
                //if (HUD[i].index == index)
                //{
                    huds[(int)index].speed=speed1;
                    huds[(int)index].health = hp;
                    huds[(int)index].score = score;
                    huds[(int)index].paintColor = paint;
                    //HUD[i].health = hp;
                    //HUD[i].speed = speed;
                //}
            //}
        }
        private void UpdateGame(GameTime gt)
        {
            GamePadState gp = GamePad.GetState(gamePlayer.findConnected());
            KeyboardState ks = Keyboard.GetState();

            if (!GameStateChecker.gamePaused)
            {
                if (countDown > 400)
                {
                    hasStarted = true;
                }
                else
                { countDown += 1.25f; }
            }

            if (gp.IsButtonDown(Buttons.Start)&&!GameStateChecker.gameOver)
            {


                if (postGame.alpha == 0)
                {
                    dispPostMenu(gamePlayer.findConnected());
                    if (Game1.networkMan.getSession() == null)
                    {
                        GameStateChecker.gamePaused = true;
                        GameStateChecker.timeOffset = gt.TotalGameTime.Seconds;
                    }
                }
                if (postGame.alpha == 255)
                {
                    postGame.action = windowAction.exiting;
                    if (Game1.networkMan.getSession() == null)
                    {
                        GameStateChecker.gamePaused = false;
                    }
                }
            }
            if (gp.IsButtonDown(Buttons.Y) || ks.IsKeyDown(Keys.H))
            {
                Game1.gameMode.openHelp(gamePlayer.findConnected());
            }

            postGame.update();
            if (postGame.action == windowAction.exiting && postGame.title != null)
            {
                int disconnectedPlayers = 0;

            #if XBOX360
                foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
                {
                    GamePadState temp = GamePad.GetState(g.PlayerIndex);

                    if (!temp.IsConnected)
                        disconnectedPlayers++;
                }
            #endif

                if (GameStateChecker.gameOver)
                {
                    int playerIndex = (int)postGame.owner;
                    playerIndex++;

                    if (playerIndex < SignedInGamer.SignedInGamers.Count - disconnectedPlayers)
                    {
                        try
                        {
                            if(gamePlayer.isConnected(SignedInGamer.SignedInGamers[playerIndex]))
                                dispPostGame((PlayerIndex)playerIndex);
                        }
                        catch { dispPostMenu(gamePlayer.findConnected()); }
                    }
                    else
                    {
                        if (Game1.gameMode.Title == "zombieMode")
                        {
                            dispPostMenu(GameStateChecker.getProtector());
                        }
                        else
                            dispPostMenu(gamePlayer.findConnected());
                    }
                }
            }
        }
        private void UpdateMainMenu(GameTime gt)
        {
            KeyboardState ks = Keyboard.GetState();
            Game1.cam.zoom = 1f;
            Game1.cam.position = Game1.cam.res / 2;
#if ZUNE
            if (delay >= 4)
            {
                delay = 0;
                Game1.particles.addFireMM(Game1.cam.res / 2);
            }
            else
                delay++;
#else
            Game1.particles.addFireMM(Game1.cam.res / 2);
#endif

            if (!postGame.show)
            for (int i = 0; i != 4; i++)
            {
                if(!gamePlayer.isConnected((PlayerIndex)i))
                    break;

                PlayerIndex playerIndex = (PlayerIndex)i; 
                GamePadState gp = GamePad.GetState(playerIndex);

                if (ks.IsKeyDown(Keys.Right) || gp.IsButtonDown(Buttons.DPadRight) || gp.IsButtonDown(Buttons.LeftThumbstickRight))
                {
                    if (Math.Abs(Math.Abs(main_menu_angle) - angle_list[mm_selection]) < .1f)
                    {
                        angleToAdd = -MathHelper.PiOver2;
                        mm_last_selection = mm_selection;
                        mm_selection--;
                        if (mm_selection >= angle_list.Length)
                        {
                            mm_last_selection = 3;
                            mm_selection = 0;
                        }
                        if (mm_selection < 0)
                        {
                            mm_last_selection = 0;
                            mm_selection = 3;
                        }
                    }
                }
                if (ks.IsKeyDown(Keys.Left) || gp.IsButtonDown(Buttons.DPadLeft) || gp.IsButtonDown(Buttons.LeftThumbstickLeft))
                {
                    if (Math.Abs(Math.Abs(main_menu_angle) - angle_list[mm_selection]) < .1f)
                    {
                        angleToAdd = MathHelper.PiOver2;
                        mm_last_selection = mm_selection;
                        mm_selection++;
                        if (mm_selection >= angle_list.Length)
                        {
                            mm_last_selection = 3;
                            mm_selection = 0;
                        }
                        if (mm_selection < 0)
                        {
                            mm_last_selection = 0;
                            mm_selection = 3;
                        }
                    }
                }

                if (ks.IsKeyDown(Keys.Up) || gp.IsButtonDown(Buttons.A))
                {
                    if (fade_change == UIstate.none)
                        if (SignedInGamer.SignedInGamers.Count != 0)
                        {
                            if (main_menu_angle > -MathHelper.PiOver4 && main_menu_angle < MathHelper.PiOver4)
                            {
                                //FadeOut(UIstate.game, false);
                                dispGameModes(playerIndex);
                            }
                            else if (main_menu_angle > (MathHelper.PiOver2 + MathHelper.Pi) - MathHelper.PiOver4 && main_menu_angle < (MathHelper.PiOver2 + MathHelper.Pi) + MathHelper.PiOver4)
                            {
                                if (!Guide.IsTrialMode)
                                {
                                    Game1.networkMan.LookForSessions("demolition");
                                }
                                else
                                {
                                    Message.newMessage("System-Link only avalible in the full game.");
                                }
                            }
                            else if (main_menu_angle > (MathHelper.Pi) - MathHelper.PiOver4 && main_menu_angle < (MathHelper.Pi) + MathHelper.PiOver4)
                            {
                                FadeOut(UIstate.credits, true, gameModes.zomb);
                            }
                            else
                            {
                                dispTotalScoreCard(playerIndex);
                            }
                        }
                        else
                        {
                            Message.newMessage("Players must sign in to begin.");
                        }
                }
                if (gp.IsButtonDown(Buttons.Back))
                {
                    Game.Exit();
                }
                if (gp.IsButtonDown(Buttons.Y))
                {
                    dispControls(playerIndex);
                }
            }
            switch (mm_selection)
            {
                case 0:
                    MMselection = "Single player or split-screen";
                    break;

                case 1:
                    MMselection = "Check personal stats";
                    break;

                case 2:
                    MMselection = "";
                    break;

                case 3:
                    MMselection = "System-link";
                    break;
            }

            /*if (angleToAdd == 0)
            {
                float deltaA = 0;

                float plus = Math.Abs(angle_list[mm_last_selection] + angle_list[mm_selection]);
                float minus = Math.Abs(angle_list[mm_last_selection] - angle_list[mm_selection]);
                if (plus > minus)
                    deltaA = plus;
                else
                    deltaA = minus;

                angleToAdd = deltaA;
            }
            if (angleToAdd != 0)
            {
                main_menu_angle += angleToAdd;// / 10);
                angleToAdd = 0;
            }*/
            spinTheWheel();

        }

        private void spinTheWheel()
        {
            float newAngle = angle_list[mm_selection];

            if (mm_last_selection == 0 && mm_selection == 3)
            {
                newAngle = -MathHelper.PiOver2;

                if (Math.Abs(angle_list[mm_last_selection] - MathHelper.PiOver2 - main_menu_angle) < 0.03f)
                {
                    mm_last_selection = mm_selection;
                    newAngle = angle_list[mm_selection];
                    main_menu_angle = angle_list[mm_selection];
                    return;
                }

            }
            else if (mm_last_selection == 3 && mm_selection == 0)
            {
                newAngle = (2 * MathHelper.Pi);

                if (Math.Abs((2 * MathHelper.Pi) - main_menu_angle) < 0.03f)
                {
                    mm_last_selection = mm_selection;
                    newAngle = angle_list[mm_selection];
                    main_menu_angle = angle_list[mm_selection];
                    return;
                }
            }


            main_menu_angle += (newAngle - main_menu_angle) / 10;
        }

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            sb.Begin();
            switch (state)
            {
                case UIstate.credits:
                    //the credits just consist of messages
                    break;
                case UIstate.game:
                    drawInGame();
                    break;
                case UIstate.intro:

                    break;
                case UIstate.lobby:

                    break;
                case UIstate.main_menu:
                    drawMainMenu();
                    Game1.cam.reset();
                    break;
                case UIstate.playerSetup:

                    break;
            }
            DrawWindow(ref postGame);
            FadeOutInUpd();

            sb.End();
            Message.draw(sb);
            

            base.Draw(gameTime);
        }
        private void DrawWindow(ref window win)
        {

            if (win.show)
            {

                sb.Draw(ui_window, new Rectangle((int)win.Position.X,
                                                 (int)win.Position.Y,
                                                 win.width,
                                                 win.height),
                                                 new Color(Color.White, win.alpha));

                sb.DrawString(font, win.displayedTitle, win.Position + new Vector2(20, 10), new Color(Color.White, win.alpha));

                foreach (windowItem item in win.items)
                {

                    float scale = 1;
                    byte alpha = win.alpha;

                    if (win.items.IndexOf(item) == win.selectedItem)
                    {
                        scale *= 1.25f;
                    }
                    else
                    {
                        alpha = (byte)(0.25f * win.alpha);
                    }

                    sb.Draw(item.Texture,
                            win.Position + item.Position,
                            null,
                            new Color(Color.White, alpha),
                            0,
                            new Vector2(item.Texture.Width, item.Texture.Height) / 2,
                            scale,
                            SpriteEffects.None,
                            0
                            );
                    if (item.hasText)
                    {
                        sb.DrawString(font,
                                      item.Numbers,
                                      win.Position + item.Position + new Vector2(item.Texture.Width / 2 + 20, 0),
                                      new Color(Color.White, win.alpha));
                    }
                    else
                    {
                        drawNumbers(sb, win.Position + item.Position + new Vector2(item.Texture.Width / 2 + 20, 0), item.Numbers, win.alpha, 1.25f);
                    }
                }
                /*
                for (int i = 0; i != win.displayedTitle.Length; i++)
                {
                    sb.Draw(PNameBG[i % 4],
                        win.Position + new Vector2(1 + (15 * i), PNameBG[i % 4].Height/2),
                        null,
                        new Color(Color.White, win.alpha),
                        0,
                        new Vector2(PNameBG[i % 4].Width, PNameBG[i % 4].Height) / 2,
                        1.25f,
                        SpriteEffects.None,
                        0
                        );
                }
                 */

                if (gamePlayer.numConnected() == 1)
                {
                    //int c = Game1.cars.int_find_car(SignedInGamer.SignedInGamers[0].Gamertag);
                    int d = Game1.animSprites.findDriver(gamePlayer.SgfindConnected().Gamertag);
                    
                    if(GameStateChecker.gameOver)
                    if (!Game1.animSprites.sprites[d].dead)
                    {
                        sb.Draw(ui_win, win.Position - new Vector2(-20, 75), Color.White);
                    }
                    else
                    {
                        sb.Draw(ui_lose, win.Position - new Vector2(-20, 75), Color.White);
                    }
                }
            }
        }
        public static void FadeOut(UIstate change, bool isMultiplayer,gameModes mode)
        {
            if (fade_time == 0 || fade_time >= 100 || fade_time == 50)
            {
                fade_gameMode = mode;
                fade_isMultiplayer = isMultiplayer;
                fade_time = 0;
                fade_change = change;
            }

        }
        private void FadeOutInUpd()
        {
            if (fade_time <= 100)
            {
                float alpha = (-.1f * sqr(fade_time - 50) + 255);
                Color temp = new Color(Color.Black, (byte)alpha);
                sb.Draw(fade, new Rectangle(0, 0, (int)Game1.cam.res.X, (int)Game1.cam.res.Y), temp);

                if (state == UIstate.main_menu)
                {
                    Game1.cars.audio.SetGlobalVariable("musicVolume", (255-alpha)/255f);
                }
                if (fade_change == UIstate.game)
                {
                    //Game1.cars.audio.SetGlobalVariable("musicVolume", (alpha) / 255f);
                }

                if (fade_time == 50)
                {
                    Game1.debris.clearAll();
                    Game1.particles.clearAll();
                    postGame = new window();
                    Game1.cam.angle = 0;
                    state = fade_change;

                    if (state == UIstate.game)
                    {
                        GameStateChecker.gamePaused = false;

                        if (!fade_isMultiplayer)
                        {
                            Game1.animSprites.sprites.Clear();
                            Game1.cars.cars.Clear();
                        }

                        switch(fade_gameMode)
                        {
                            case gameModes.demo:
                                Game1.gameMode = new ModeDemolition();
                                break;
                            case gameModes.tag:
                                Game1.gameMode = new ModeTag();
                                break;
                            case gameModes.zomb:
                                Game1.gameMode = new ModeZombie();
                                break;
                        }

                        Game1.gameMode.conditionsMet();

                        if (Game1.gameMode.ConditionsGreen())
                        {
                            //the argument defines weather or not the game is multiplayer
                            Game1.gameMode.initializeMatch(fade_isMultiplayer);
                        }
                        else
                        {
                            state = UIstate.main_menu;
                        }
                    }
                    if (state == UIstate.main_menu)
                    {
                        GameStateChecker.gameOver  =false;
                        Game1.networkMan.endSession();
                        Game1.animSprites.sprites.Clear();
                        foreach (car c in Game1.cars.cars)
                        {
                            c.engine.Pause();
                            c.fire.Pause();
                            c.squeal.Pause();
                        }
                        Game1.cars.audio.Update();
                        Game1.cars.cars.Clear();
                    }
                    fade_change = UIstate.none;
                }
                fade_time += .5f;
            }
        }
        private void drawMainMenu()
        {
            int p = 0;

            gamePlayer gp=null;

            if(!Guide.IsVisible)
            foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
            {
                //gp = g.GetProfile().GamerPicture;
                gp = g.Tag as gamePlayer;
                if (gp != null)
                {
                    sb.DrawString(font, g.Gamertag, new Vector2(40, 10 + (p * 30)), Color.White);
                    sb.Draw(gp.gamerIcon, new Vector2(20, 20 + (p * 30)), null, Color.White, 0, new Vector2(gp.gamerIcon.Width, gp.gamerIcon.Height) / 2, .5f, SpriteEffects.None, 0);
                    p++;
                }
            }

            sb.DrawString(font,
                          MMselection,
                          new Vector2((Game1.cam.res.X / 2)-(MMselection.Length*6.25f), Game1.cam.res.Y * .65f),
                          Color.White);

            sb.Draw(mm_title, new Vector2(Game1.cam.res.X, Game1.cam.res.Y) / 2, null, Color.White, 0,
                    new Vector2(mm_title.Width,
                    mm_title.Height + 20) / 2,
                    1, SpriteEffects.None, 0);
            sb.Draw(mm_quit, new Vector2(0, Game1.cam.res.Y-32), null, Color.White, 0,
                    Vector2.Zero,
                    1, SpriteEffects.None, 0);
            sb.Draw(mm_help, new Vector2(Game1.cam.res.X-128, Game1.cam.res.Y - 32), null, Color.White, 0,
                     Vector2.Zero,
                     1, SpriteEffects.None, 0);
            sb.Draw(main_menu, new Vector2(Game1.cam.res.X / 2, Game1.cam.res.Y), null, Color.White, main_menu_angle,
            new Vector2(main_menu.Width,
                        main_menu.Height) / 2,
            1.25f, SpriteEffects.None, 0);
        }
        private void drawInGame()
        {
            /*
            if (false)//isOutOfBounds)
            {
                sb.Draw(ui_oob, Vector2.One, Color.White);
                sb.Draw(count_down[(int)(oobTime / 100)], Game1.cam.res / 2, null, Color.White, 0,
                    new Vector2(count_down[0].Width,
                    count_down[0].Height) / 2,
                    1, SpriteEffects.None, 0);
            }
            else
            {
                //drawNumbers(sb, Vector2.Zero, score.ToString());
            }*/

            if (Game1.gameMode.Title == "zombieMode")
            {
                ModeZombie gm = Game1.gameMode as ModeZombie;

                string str =gm.killed + "/" + gm.zombies + " Zombies killed";
                sb.DrawString(font,str,
                              new Vector2(Game1.cam.res.X/2-(str.Length*7), 20),
                              Color.White);
            }

            if (countDown < 400)
                sb.Draw(count_down[(int)(countDown / 100)], Game1.cam.res / 2, null, Color.White, 0,
                        new Vector2(count_down[0].Width,
                                    count_down[0].Height) / 2,
                        1, SpriteEffects.None, 0);

            for(int i=0;i!=SignedInGamer.SignedInGamers.Count;i++)
            {
                if(gamePlayer.isConnected(SignedInGamer.SignedInGamers[i]))
                    huds[i].draw(sb);
            }

        }
        public void dispGameModes(PlayerIndex playerIndex)
        {

            postGame.selectedItem = 0;
            //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            //{
            Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height*2);
            postGame = new window(Game1.cam.res*.1f,
              "gameMode",
              "Gamemodes",
              false, playerIndex,15);
            postGame.items.Clear();
            postGame.items.Add(new windowItem(modeDemo,
                                              new Vector2((windowBounds.X / 2) - ui_kills.Width / 2, windowBounds.Y * .22f),
                                              itemActions.modeDemo,
                                              ""));
            postGame.items.Add(new windowItem(modeTag,
                                  new Vector2((windowBounds.X / 2) - ui_kills.Width / 2, windowBounds.Y * .48f),
                                  itemActions.modeTag,
                                  ""));
            postGame.items.Add(new windowItem(modeZombie,
                                  new Vector2((windowBounds.X / 2) - ui_kills.Width / 2, windowBounds.Y * .72f),
                                  itemActions.modeZombie,
                                  ""));

            //}
            postGame.show = true;
            //postGame.action = windowAction.none;
            state = UI.UIstate.main_menu;

        }
        public void dispTotalScoreCard(PlayerIndex playerIndex)
        {

            postGame.selectedItem = 0;
            //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            //{
            Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height);
            postGame = new window((Game1.cam.res / 2) - (windowBounds / 2),
              "postGame",
              SignedInGamer.SignedInGamers[(int)playerIndex].Gamertag,
              false, playerIndex, 3);
            int car = Game1.cars.int_find_car(Gamer.SignedInGamers[(int)playerIndex].Gamertag);

            gamePlayer gp = Gamer.SignedInGamers[(int)playerIndex].Tag as gamePlayer;

            postGame.items.Clear();
            postGame.items.Add(new windowItem(ui_hs_kills,
                                              new Vector2((windowBounds.X / 2) - ui_kills.Width / 2, windowBounds.Y * .2f),
                                              itemActions.none,
                                              gp.kills.ToString()));
            postGame.items.Add(new windowItem(ui_hs_deaths,
                              new Vector2((windowBounds.X / 2) - ui_score.Width / 2, windowBounds.Y * .4f),
                              itemActions.none,
                              gp.deaths.ToString()));
            postGame.items.Add(new windowItem(ui_hs_kds,
                                new Vector2((windowBounds.X / 2) - ui_score.Width / 2, windowBounds.Y * .6f),
                  itemActions.none,
                  (gp.kills-gp.deaths).ToString()));
            postGame.items.Add(new windowItem(ui_hs_score,
                    new Vector2((windowBounds.X / 2) - ui_score.Width / 2, windowBounds.Y * .8f),
                    itemActions.none,
                    gp.totalPoints.ToString()));

            //}
            postGame.show = true;
            //state = UI.UIstate.gameOver;

        }
        public void dispControls(PlayerIndex playerIndex)
        {

            postGame.selectedItem = 0;
            //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            //{
            Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height);
            postGame = new window((Game1.cam.res / 2),
              "helpControls",
              SignedInGamer.SignedInGamers[(int)playerIndex].Gamertag,
              false, playerIndex, 3);
            int car = Game1.cars.int_find_car(Gamer.SignedInGamers[(int)playerIndex].Gamertag);

            gamePlayer gp = Gamer.SignedInGamers[(int)playerIndex].Tag as gamePlayer;

            postGame.items.Clear();
            postGame.items.Add(new windowItem(mm_controls,
                                              Vector2.Zero,
                                              itemActions.none,
                                              ""));

            //}
            postGame.show = true;
            //state = UI.UIstate.gameOver;

        }
        public void dispPostGame(PlayerIndex playerIndex)
        {

                postGame.selectedItem = 0;
                //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
                //{
                Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height);
                postGame = new window((Game1.cam.res / 2) - (windowBounds / 2),
                  "postGame",
                  SignedInGamer.SignedInGamers[(int)playerIndex].Gamertag,
                  false, playerIndex,3);
                int car = Game1.cars.int_find_car(Gamer.SignedInGamers[(int)playerIndex].Gamertag);
                postGame.items.Clear();
                postGame.items.Add(new windowItem(ui_kills,
                                                  new Vector2((windowBounds.X / 2) - ui_kills.Width / 2 - 20, windowBounds.Y * .4f),
                                                  itemActions.none,
                                                  Game1.cars.cars[car].kills.ToString()));
                postGame.items.Add(new windowItem(ui_score,
                                  new Vector2((windowBounds.X / 2) - ui_score.Width / 2, windowBounds.Y * .8f),
                                  itemActions.none,
                                  Game1.cars.cars[car].score.ToString()));

                //}
                postGame.show = true;
                state = UI.UIstate.gameOver;

        }
        public void dispScoreCard(PlayerIndex playerIndex)
        {

            postGame.selectedItem = 0;
            //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            //{
            Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height);
            postGame = new window((Game1.cam.res / 2) - (windowBounds / 2),
              "postGame",
              SignedInGamer.SignedInGamers[(int)playerIndex].Gamertag,
              false, playerIndex, 3);
            int car = Game1.cars.int_find_car(Gamer.SignedInGamers[(int)playerIndex].Gamertag);
            postGame.items.Clear();
            postGame.items.Add(new windowItem(ui_kills,
                                              new Vector2((windowBounds.X / 2) - ui_kills.Width / 2 - 20, windowBounds.Y * .4f),
                                              itemActions.none,
                                              Game1.cars.cars[car].kills.ToString()));
            postGame.items.Add(new windowItem(ui_score,
                              new Vector2((windowBounds.X / 2) - ui_score.Width / 2, windowBounds.Y * .8f),
                              itemActions.none,
                              Game1.cars.cars[car].score.ToString()));

            //}
            postGame.show = true;
            state = UI.UIstate.gameOver;

        }
        public void dispPostMenu(PlayerIndex playerIndex)
        {
            //if (postGame.title != "postMenu")
            {
                //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
                //{
                postGame.selectedItem = -1;
                Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height);
                postGame = new window((Game1.cam.res / 2) - (windowBounds / 2),
                  "postMenu",
                  "",
                  false, playerIndex,3);
                postGame.items.Clear();
                postGame.items.Add(new windowItem(ui_mainmenu,
                                                  new Vector2((windowBounds.X / 2), windowBounds.Y * .25f),
                                                  itemActions.mainMenu,
                                                  ""));
                postGame.items.Add(new windowItem(ui_retry,
                                  new Vector2(windowBounds.X / 2, windowBounds.Y * .75f),
                                  itemActions.restart,
                                  ""));

                //}
                postGame.show = true;
            }
        }
        public void dispDemoHelp(PlayerIndex playerIndex)
        {

            postGame.selectedItem = 0;
            //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            //{
            Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height);
            postGame = new window((Game1.cam.res / 2)-windowBounds,
              "help",
              "",
              false, playerIndex, 3);
            postGame.width *= 2;
            postGame.height *= 2;

            int car = Game1.cars.int_find_car(Gamer.SignedInGamers[(int)playerIndex].Gamertag);

            gamePlayer gp = Gamer.SignedInGamers[(int)playerIndex].Tag as gamePlayer;

            postGame.items.Clear();
            postGame.items.Add(new windowItem(win_help,
                                              Vector2.One*10,
                                              itemActions.none,
                                              "The object of Demolition mode \n is to crash into enemy \n cars and destroy them. \n When a car is destroyed \n the driver is exposed. Prevent\n the driver from reaching the \n center of the arena or their \ncar will respawn.",
                                              true));

            //}
            postGame.show = true;
            //state = UI.UIstate.gameOver;

        }
        public void dispTagHelp(PlayerIndex playerIndex)
        {

            postGame.selectedItem = 0;
            //foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            //{
            Vector2 windowBounds = new Vector2(ui_window.Width, ui_window.Height);
            postGame = new window((Game1.cam.res / 2) - windowBounds,
              "help",
              "",
              false, playerIndex, 3);
            postGame.width *= 2;
            postGame.height *= 2;

            int car = Game1.cars.int_find_car(Gamer.SignedInGamers[(int)playerIndex].Gamertag);

            gamePlayer gp = Gamer.SignedInGamers[(int)playerIndex].Tag as gamePlayer;

            postGame.items.Clear();
            postGame.items.Add(new windowItem(win_help,
                                              Vector2.One * 10,
                                              itemActions.none,
                                              "The goal of tag is simple. \n If you are it, a red aurora \n appears around your vehicle. \n The aurora will shrink until \n compleatly depleated. You must \n run into another car before \n it has vanished or your \n vehicle will be destroyed.",
                                              true));

            //}
            postGame.show = true;
            //state = UI.UIstate.gameOver;

        }
        private void drawNumbers(SpriteBatch sb, Vector2 pos, string input)
        {
            for (int i = 0; i != input.Length; i++)
            {
                int index = 0;

                switch (input[i])
                {
                    case '0':
                        index = 0;
                        break;
                    case '1':
                        index = 1;
                        break;
                    case '2':
                        index = 2;
                        break;
                    case '3':
                        index = 3;
                        break;
                    case '4':
                        index = 4;
                        break;
                    case '5':
                        index = 5;
                        break;
                    case '6':
                        index = 6;
                        break;
                    case '7':
                        index = 7;
                        break;
                    case '8':
                        index = 8;
                        break;
                    case '9':
                        index = 9;
                        break;
                }
                sb.Draw(digits[index], pos + new Vector2(i * 16, 0), Color.White);
            }
        }
        public void drawNumbers(SpriteBatch sb, Vector2 pos, string input, byte alpha, float scale)
        {
            for (int i = 0; i != input.Length; i++)
            {
                int index = 0;

                switch (input[i])
                {
                    case '-':
                        index = 11; 
                        break;
                    case '0':
                        index = 0;
                        break;
                    case '1':
                        index = 1;
                        break;
                    case '2':
                        index = 2;
                        break;
                    case '3':
                        index = 3;
                        break;
                    case '4':
                        index = 4;
                        break;
                    case '5':
                        index = 5;
                        break;
                    case '6':
                        index = 6;
                        break;
                    case '7':
                        index = 7;
                        break;
                    case '8':
                        index = 8;
                        break;
                    case '9':
                        index = 9;
                        break;
                }
                sb.Draw(digits[index],
                    pos + new Vector2(i * (16 * scale), 0),
                    null,
                    new Color(Color.White, alpha),
                    0,
                    new Vector2(digits[index].Width, digits[index].Height) / 2,
                    scale,
                    SpriteEffects.None,
                    0);
            }
        } 
        #endregion
        #region Random shit
        private float sqr(float input)
        {
            float _out = input * input;
            return _out;
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
        #endregion

        #region structsAndStuff
        public struct window
        {
            public Vector2 Position;
            public PlayerIndex owner;
            public List<windowItem> items;
            public windowAction action;
            public string title;
            public string displayedTitle;
            public bool show;
            public int selectedItem;
            public int fadeSpeed;
            public int width, height;
            public byte alpha;
            bool buttonsReleased;

            public window(Vector2 pos, string Title, string dispTitle, bool Show, PlayerIndex Owner,int fadeSpeed)
            {
                displayedTitle = dispTitle;
                buttonsReleased = true;
                action = windowAction.entering;
                title = Title;
                Position = pos;
                show = Show;
                alpha = 0;
                selectedItem = 0;
                items = new List<windowItem>();
                owner = Owner;
                this.fadeSpeed = fadeSpeed;

                width = Game1.ui.ui_window.Width;
                height = Game1.ui.ui_window.Height;

            }
            public void update()
            {
                if (this.title != null && this.show)
                {
                    #region transitions
                    switch (action)
                    {
                        case windowAction.entering:
                            if (alpha < 255)
                            {
                                this.alpha += (byte)fadeSpeed;
                                if (this.alpha > 255)
                                {
                                    this.alpha = 255;
                                    this.buttonsReleased = false;
                                }
                            }
                            else
                            {
                                
                                this.action = windowAction.none;
                            }
                            break;
                        case windowAction.exiting:
                            if (alpha > 0)
                            {
                                this.alpha -= (byte)fadeSpeed;
                                if (this.alpha < 0)
                                {
                                    this.alpha = 0;
                                }
                            }
                            else
                            {
                                this.show = false;
                                this.action = windowAction.none; //was commented out
                            }
                            break;
                        case windowAction.none:
                            GamePadState gs = GamePad.GetState(this.owner);
                            KeyboardState ks = Keyboard.GetState();

                            if (gs.IsButtonDown(Buttons.B) || ks.IsKeyDown(Keys.Escape))
                            {
                                this.action = windowAction.exiting;
                            }

                            if (gs.IsButtonDown(Buttons.A) || ks.IsKeyDown(Keys.Enter))
                            {
                                if (buttonsReleased)
                                {
                                    switch (items[selectedItem].Action)
                                    {
                                        case itemActions.modeDemo:
                                            FadeOut(UIstate.game, false, gameModes.demo);
                                            break;

                                        case itemActions.modeTag:
                                            if (!Guide.IsTrialMode)
                                            {
                                                FadeOut(UIstate.game, false, gameModes.tag);
                                            }
                                            else
                                            {
                                                Message.newMessage("Tag mode only avalible in full game");
                                            }
                                            break;

                                        case itemActions.modeZombie:
                                            if (!Guide.IsTrialMode)
                                            {
                                                FadeOut(UIstate.game, false, gameModes.zomb);
                                            }
                                            else
                                            {
                                                Message.newMessage("Zombie mode only avalible in full game");
                                            }
                                            break;

                                        case itemActions.mainMenu:
                                                FadeOut(UIstate.main_menu, false, gameModes.none);
                                            break;

                                        case itemActions.restart:

                                            NetworkSession session = Game1.networkMan.getSession();

                                            if (session != null)
                                            {
                                                if (Game1.networkMan.getHost()!=null)
                                                {
                                                    Game1.networkMan.sendGameModeDataUpdate(Game1.networkMan.getHost(),networkManager.SessionState.retry);
                                                    Game1.gameMode.MPreset();
                                                    
                                                    //TODO
                                                    //FadeOut(UIstate.game, fade_isMultiplayer, gameModes.none); //may have to change this 
                                                    //action = windowAction.exiting;
                                                }
                                            }
                                            else
                                            {
                                                //Game1.ui.countDown = 0;
                                                //Game1.gameMode.MPreset();
                                                for (int j = 0; j != Game1.cars.cars.Count; j++)
                                                    {
                                                        Game1.cars.cars[j].squeal.Pause();
                                                        Game1.cars.cars[j].engine.Pause();
                                                        Game1.cars.cars[j].fire.Pause();
                                                    }
                                                FadeOut(UIstate.game, fade_isMultiplayer, gameModes.none);
                                            }
                                            break;
                                    }
                                    this.action = windowAction.exiting;
                                    this.buttonsReleased = false;
                                }
                            }
                            else if (gs.IsButtonDown(Buttons.LeftThumbstickUp)||ks.IsKeyDown(Keys.Up) && selectedItem != -1)
                            {
                                if (buttonsReleased)
                                {
                                    selectedItem--;
                                    if (selectedItem < 0)
                                    {
                                        selectedItem = items.Count - 1;
                                    }

                                    Game1.particles.addExplosion(this.Position + items[selectedItem].Position + Game1.cam.posLiteral);
                                    for (int i = 0; i != 18; i++)
                                        Game1.particles.addSpark(this.Position + items[selectedItem].Position + Game1.cam.posLiteral);

                                    this.buttonsReleased = false;
                                }
                            }
                            else if ((gs.IsButtonDown(Buttons.LeftThumbstickDown)||ks.IsKeyDown(Keys.Down)) && selectedItem != -1)
                            {
                                if (buttonsReleased)
                                {
                                    selectedItem++;
                                    if (selectedItem >= items.Count)
                                    {
                                        selectedItem = 0;
                                    }

                                        Game1.particles.addExplosion(this.Position + items[selectedItem].Position + Game1.cam.posLiteral);
                                    for (int i = 0; i != 18; i++)
                                        Game1.particles.addSpark(this.Position + items[selectedItem].Position + Game1.cam.posLiteral);

                                    this.buttonsReleased = false;
                                }
                            }
                            else
                            {
                                buttonsReleased = true;
                            }
                            
                            switch (items[selectedItem].Action)
                            {
                                case itemActions.modeDemo:
                                    //set MMtitle
                                    MMselection = "1-4 Players";
                                    break;

                                case itemActions.modeTag:
                                    //set MMtitle
                                    MMselection = "1-4 Players";
                                    break;

                                case itemActions.modeZombie:
                                    //set MMtitle
                                    MMselection = "2 Players";
                                    break;
                            }

                            break;
                    }
                    #endregion
                }
            }
            public void addItem(windowItem item)
            {
                items.Add(item);
            }
        }
        public struct windowItem
        {
            public Texture2D Texture;
            public Vector2 Position;
            public itemActions Action;
            public string Numbers;
            public bool hasText;

            public windowItem(Texture2D tex, Vector2 pos, itemActions action, string numbers)
            {
                Texture = tex;
                Position = pos;
                Action = action;
                Numbers = numbers;
                hasText = false;
            }
            public windowItem(Texture2D tex, Vector2 pos, itemActions action, string text,bool hasText)
            {
                Texture = tex;
                Position = pos;
                Action = action;
                Numbers = text;
                this.hasText = hasText;
            }
        }
        public struct message
        {
            SpriteFont font;
            Texture2D texture;
            string text;
            float fade;
            bool fadingIn;


            public message(string Text, SpriteFont Font)
            {
                font = Font;
                text = Text;
                fadingIn = false;
                fade = 0;
                texture = null;
            }
            public void update()
            {
                if (fadingIn)
                {
                    fade += 0.005f;

                    if (fade > 1)
                    {
                        fade = 1;
                        fadingIn = false;
                    }

                }
                else
                {
                    fade -= 0.005f;

                    if (fade <= 0)
                    {
                        fade = 0;
                    }
                }
            }
            public void newMessage(string Message)
            {
                text = Message;
                fadingIn = true;
                texture = null;
            }
            public void newMessage(string Message,Texture2D texture)
            {
                text = Message;
                fadingIn = true;
                this.texture = texture;
            }
            public void draw(SpriteBatch sb)
            {
                byte alpha = (byte)(255 * this.fade); ;
                Vector2 offset = Vector2.Zero;

                sb.Begin();

                if (texture != null)
                {
                    sb.Draw(texture,
                            new Vector2(Game1.cam.res.X / 2, 0),
                            null,
                            new Color(Color.White, alpha),
                            0,
                            new Vector2(texture.Width, 0) / 2,
                            1,
                            SpriteEffects.None,
                            0);

                    offset.Y = texture.Height / 2;

                }

                if (font != null)
                {
                    for (int i = 0; i != 8; i++)
                    {
                        Vector2 outline = Vector2.Zero;
                        outline.X = (float)(4 * Math.Cos(i));
                        outline.Y = (float)(4 * Math.Sin(i));

                        sb.DrawString(font,
                                      text,
                                      (Game1.cam.res / 2) + outline +offset,
                                      new Color(Color.Black, alpha),
                                      0,
                                      font.MeasureString(text) / 2,
                                      1.25f,
                                      SpriteEffects.None,
                                      0);
                    }
                    sb.DrawString(font,
                                    text,
                                    (Game1.cam.res / 2)+offset,
                                    new Color(Color.White, alpha),
                                    0,
                                    font.MeasureString(text) / 2,
                                    1.25f,
                                    SpriteEffects.None,
                                    0);
                }
                sb.End();
            }
        }
        public struct hud
        {
            static bool isOutOfBounds;
            static int oobTime;
            public int score;
            public PlayerIndex index;
            public float speed, health;
            public Color paintColor;

            public hud(PlayerIndex index)
            {
                this.index = index;
                isOutOfBounds = false;
                oobTime = 0;
                score = 0;
                speed = 0;
                health = 100;
                paintColor = Color.White;
            }
            public void draw(SpriteBatch sb)
            {
                Vector2 pos = Vector2.Zero;
                Vector2 offset = Vector2.Zero;
                Vector2 scorePos = Vector2.Zero;
                Vector2 healthBarOffset = Vector2.Zero;
                int needleDir = 1;
                SpriteEffects orientation = new SpriteEffects();
                switch (index)
                {
                    case PlayerIndex.One:
                        pos = new Vector2(0, Game1.cam.res.Y);
                        healthBarOffset = new Vector2(34, 15);
                        orientation = SpriteEffects.None;
                        scorePos = new Vector2(-10, 70);
                        break;

                    case PlayerIndex.Two:
                        pos = new Vector2(Game1.cam.res.X - spedometer_gauge.Width, Game1.cam.res.Y);
                        orientation = SpriteEffects.FlipHorizontally;
                        offset.X = spedometer_gauge.Width;
                        healthBarOffset = new Vector2(28, 15);
                        scorePos = new Vector2(-10, 70);
                        needleDir = -1;
                        break;
                    case PlayerIndex.Three:
                        pos = new Vector2(Game1.cam.res.X - spedometer_gauge.Width, spedometer_gauge.Height);
                        healthBarOffset = new Vector2(28, 5);
                        orientation = SpriteEffects.FlipHorizontally;
                        offset.X = spedometer_gauge.Width;
                        needleDir = -1;
                        scorePos = new Vector2(-16, -6);
                        break;
                    case PlayerIndex.Four:
                        pos = new Vector2(0, spedometer_gauge.Height);
                        healthBarOffset = new Vector2(34, 8);
                        orientation = SpriteEffects.None;
                        scorePos = new Vector2(-10, -6);
                        break;
                }

                Game1.ui.drawNumbers(sb, pos - scorePos, score.ToString());

                sb.Draw(spedometer_healthBar,
                        new Vector2(0, -spedometer_gauge.Height) + pos + healthBarOffset,
                        new Rectangle(0, 0, (int)((health / 100) * spedometer_healthBar.Width), 40),
                        Color.White,
                        0,
                        Vector2.Zero,
                        1,
                        orientation,
                        0);
                sb.Draw(spedometer_gauge, new Vector2(0, -spedometer_gauge.Height) + pos, null, paintColor, 0,
                        Vector2.Zero, 1, orientation, 0);

                sb.Draw(spedometer_needle, pos + offset, null, Color.White, needleDir * (speed * .314f) - (float)(Math.PI / 4),
                        new Vector2(0, spedometer_needle.Height),
                        1, SpriteEffects.None, 0);

            }
        }
        public enum gameModes
        { demo, tag, zomb, none };
        public enum itemActions
        {
            mainMenu, restart, none, modeZombie, modeDemo, modeTag
        };
        public enum UIstate
        { intro, main_menu, playerSetup, credits, lobby, game, gameOver, none }
        public enum windowAction
        { entering, exiting, none };  


        #endregion
    }
}
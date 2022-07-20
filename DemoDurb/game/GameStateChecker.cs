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
    public class GameStateChecker : Microsoft.Xna.Framework.GameComponent
    {
        public static checkerAction CA_action = checkerAction.none;
        public static bool gameOver = false;
        public static bool gamePaused = false;
        public static int timePaused = 0;
        public static int timeOffset = 0;
        static GameTime _gameTime;

        static int becameIt = 0;
        public static car it;

        static PlayerIndex survivor,protector;

        public GameStateChecker(Game game)
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

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            if(Game1.gameMode != null)
                if (Game1.ui.state == UI.UIstate.game && !GameStateChecker.gamePaused)
                {
                    _gameTime = gameTime;
                    Game1.gameMode.updateGame(_gameTime);
                }

            base.Update(gameTime);
        }

        public static void CheckDeadPlayers()
        {
            //if(CA_action != checkerAction.dispPostGame)
            int dead_players = 0;

            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                int c = Game1.cars.int_find_car(gamer.Gamertag);

                if (c != -1 && Game1.cars.cars[c].hp <= 0)  //if a local player's car is dead
                {
                    int d = Game1.animSprites.findDriver(gamer.Gamertag);

                    if (d != -1 && Game1.animSprites.sprites[d].dead) //if a local player's driver is dead
                    {
                        //show post game
                        dead_players++;
                    }

                }
            }
            if (dead_players > gamePlayer.numConnected() - 1 &&
                Game1.cars.cars.Count > 1 && !gameOver)
            {
                Game1.ui.dispPostGame(gamePlayer.findConnected());
                CA_action = checkerAction.dispPostGame;
                gameOver = true;

                SaveScores();
            }
            else
            {
                gameOver = false;
            }
        }

        private static void SaveScores()
        {
            for(int i =0; i<SignedInGamer.SignedInGamers.Count;i++)
            {
                gamePlayer player = SignedInGamer.SignedInGamers[i].Tag as gamePlayer;

                foreach (car c in Game1.cars.cars)
                {
                    if (c.gamertag == SignedInGamer.SignedInGamers[i].Gamertag)
                    {
                        player.deaths += c.deaths;
                        player.kills += c.kills;
                        player.totalPoints += c.score;
                    }
                }
                player.savePlayer();

                SignedInGamer.SignedInGamers[i].Tag = player;

            }
        }
        public static void CheckDeadCars()
        {
            if (!gameOver)
            {
                int dead = 0;

                foreach (car c_car in Game1.cars.cars)
                {
                    if (c_car.hp <= 0)  //if a local player's car is dead
                    {
                        int d = Game1.animSprites.findDriver(c_car.gamertag);

                        if (d != -1 && Game1.animSprites.sprites[d].dead) //if a local player's driver is dead
                        {
                            //show post game
                            dead++;
                        }

                    }
                }

                if (dead + 1 == Game1.cars.cars.Count && dead != 0)
                {
                    SaveScores();

                    Game1.ui.dispPostGame(gamePlayer.findConnected());
                    CA_action = checkerAction.dispPostGame;
                    gameOver = true;
                }
                else
                {
                    gameOver = false;
                }
            }
        }
        #region Tag
        public static void updateModeTag(GameTime gt)
        {
            if (Game1.cars.cars.Count > 1)
            {
                if (it != null)
                {

                    if (Math.Abs((_gameTime.TotalGameTime.Seconds - GameStateChecker.timePaused) - becameIt) >= 10)
                    {
                        System.Console.Out.WriteLine("Times up!");

                        it.carExplode(ref it);
                        it.hp = 0;
                        Game1.animSprites.KillDriverTag(it.gamertag);

                        selectIt();
                    }
                }
                else
                {
                    System.Console.Out.WriteLine("Picking the first guy...");

                    selectIt();
                }
            }
        }
        private static void selectIt()
        {
            Random rnd = new Random();

            //if(Game1.cars.cars.Count>1)
            while (it == null || it.hp <= 0)
            {
                int i_car = (int)(rnd.NextDouble() * Game1.cars.cars.Count);
                it = Game1.cars.cars[i_car];
            }
            becameIt = _gameTime.TotalGameTime.Seconds -(GameStateChecker.timePaused);

            System.Console.Out.WriteLine(it.gamertag + " is it.");
        }
        public static void becomeIt(car car)
        {
            it = car;
            becameIt = _gameTime.TotalGameTime.Seconds - (GameStateChecker.timePaused);
        }
        public static float getDeathProgress()
        {
            float prog = (float)((20f - ((_gameTime.TotalGameTime.Seconds - GameStateChecker.timePaused) - becameIt)) / 20f);
            return prog;
        } 
        #endregion
        public static void CheckDeadSurvivor()
        {
            ModeZombie game = Game1.gameMode as ModeZombie;

            animGuy survivor = game.getSurvivor();

            if (survivor != null && survivor.dead)
            {
                Game1.ui.dispPostGame(protector);
                CA_action = checkerAction.dispPostGame;
                gameOver = true;
            }
        }
        public static void togglePositions()
        {
            if (survivor == protector)
            {
                List<SignedInGamer> players = new List<SignedInGamer>();

                foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
                {
                    if (players.Count < 2)
                    {
                        GamePadState gp = GamePad.GetState(g.PlayerIndex);
                        if (gp.IsConnected)
                        {
                            players.Add(g);
                        }
                    }
                }

                survivor = players[0].PlayerIndex;
                protector = players[1].PlayerIndex;
            }
            PlayerIndex temp = survivor;
            survivor = protector;
            protector = temp;
        }
        public static string getGamerTag(PlayerIndex i)
        {
            foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
            {
                if (g.PlayerIndex == i)
                    return g.Gamertag;
            }
            return "null";
        }
        public static PlayerIndex getSurvivor()
        { return survivor; }
        public static PlayerIndex getProtector()
        { return protector; }
        public enum checkerAction
        { dispPostGame, dispPostGameMenu,none };
    }
}
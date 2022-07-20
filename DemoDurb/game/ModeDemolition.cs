
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace DemoDurb
{
    class ModeDemolition : GameMode
    {
        public override void conditionsMet()
        {
            if (SignedInGamer.SignedInGamers.Count > 0)
            {
                b_conditionsGreen = true;
            }
            else
            {
                b_conditionsGreen = false;
            }
        }

        public override void initializeMatch(bool multiplayer)
        {
            Game1.cars.initGame(multiplayer);

            if (!multiplayer)
            {
                Game1.cars.start_singlePlayer();
            }
            else
            {
            }

            base.title = "demolition";

           base.initializeMatch(multiplayer);
        }

        public override void MPreset()
        {
            UI.postGame.action = UI.windowAction.exiting;

            for(int i = 0; i!=Game1.cars.cars.Count;i++)
            {

                Game1.cars.cars[i].hp = 100;
                Game1.cars.cars[i].position = new Vector2(170 * (float)Math.Cos((i)) + Game1.cam.res.X / 2,
                                                          170 * (float)Math.Sin((i)) + Game1.cam.res.Y / 2);
            }

            GameStateChecker.gameOver = false;
            GameStateChecker.CA_action = GameStateChecker.checkerAction.none;
            Game1.ui.hasStarted = true;
            Game1.ui.state = UI.UIstate.game;

            for (int i = 0; i != Game1.animSprites.sprites.Count; i++)
            {
                Game1.animSprites.sprites[i].dead = false;

                animGuy guy = Game1.animSprites.sprites[i] as animGuy;
                guy.active = false;
            }

            base.MPreset();
        }

        public override void updateCars(car[] cars,float speed,ref car thisCar)
        {
            if (!GameStateChecker.gamePaused)
            {
                if (thisCar.hp > 0)
                    switch (thisCar.user)
                    {
                        case car.controlType.ai:
                            if (Game1.ui.hasStarted)
                                thisCar.AIdemolition(cars);
                            Game1.cam.grabCam(thisCar.position);
                            break;

                        case car.controlType.local:
                            /*
                            if (thisCar.vecLessThan(thisCar.position, Game1.cam.min))
                                Game1.cam.min = thisCar.position;

                            else if (thisCar.vecLessThan(Game1.cam.max, thisCar.position))
                                Game1.cam.max = thisCar.position;*/
                            if (thisCar.hp > 0)
                            {
                                Game1.cam.grabCam(thisCar.position);
                            }
                            thisCar.LocalPlayer(speed);
                            break;

                        case car.controlType.network:
                            if (thisCar.hp > 0)
                            {
                                Game1.cam.grabCam(thisCar.position);
                            }
                            break;
                    }

                foreach (car c in cars)
                {
                    if (c.tag != thisCar.tag)
                    {
                        thisCar.CollisionHandler(c, true);
                    }
                    thisCar.CheckBounds(true);
                }
                for (int i = 0; i != cars.Length; i++)
                {
                    if (cars[i].tag == thisCar.tag)
                    {
                        cars[i] = thisCar;
                    }
                }
            }
            base.updateCars(cars, 0, ref thisCar);
        }

        public override void updateDrivers(AnimSprite[] sprites, ref animGuy thisGuy)
        {
            if (!GameStateChecker.gamePaused)
            {
                switch (thisGuy.user)
                {
                    case car.controlType.ai:
                        thisGuy.AIPlayer();

                        break;
                    case car.controlType.local:
                        thisGuy.LocalPlayer();
                        break;
                    case car.controlType.network:

                        break;
                }

                thisGuy.DriverHit();
                thisGuy.RespawnCar();
            }
            base.updateDrivers(sprites, ref thisGuy);
        }

        public override void updateGame(GameTime gt)
        {
            GameStateChecker.CheckDeadPlayers();
            GameStateChecker.CheckDeadCars();

            base.updateGame(gt);
        }

        public override void playerJoin(Gamer gamer)
        {
            /*
            LocalNetworkGamer g = gamer as LocalNetworkGamer;

            if (!g.IsLocal)
            {
                Game1.cars.spawn_car_network(Game1.cars.cars.Count, g.Gamertag);
            }
            else
            {
                Game1.cars.spawn_car_local(Game1.cars.cars.Count,g.SignedInGamer.PlayerIndex, g.Gamertag);
            }

            base.playerJoin(gamer);
             * */
        }

        public override void openHelp(PlayerIndex player)
        {
            Game1.ui.dispDemoHelp(player);
            /*
            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                if (gamePlayer.isConnected(gamer))
                {
                    GamePadState gs = GamePad.GetState(gamer.PlayerIndex);
                    KeyboardState ks =  Keyboard.GetState();

                    if (gs.IsButtonDown(Buttons.Y) || ks.IsKeyDown(Keys.H))
                    {

                    }
                }
            }*/

           base.openHelp(player);
        }

    }
}

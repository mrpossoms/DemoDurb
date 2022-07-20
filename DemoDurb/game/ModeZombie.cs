using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;

namespace DemoDurb
{
    class ModeZombie : GameMode
    {
        int timeWaited = 0, nextZombie =0;
        public int killed=0, zombies = 500;
        animGuy survivor;
        Random rnd = new Random();

        public ModeZombie()
        {
            this.title = "zombieMode";
        }

        public override void conditionsMet()
        {
            if (SignedInGamer.SignedInGamers.Count == 2)
            {
                b_conditionsGreen = true;
            }
            else
            {
                b_conditionsGreen = false;
                UI.Message.newMessage("Must have 2 players signed in");
            }
        }
        public override void initializeMatch(bool multiplayer)
        {
            Game1.cars.initGame(multiplayer);

            GameStateChecker.togglePositions();
            killed = 0;

            Game1.cars.start_Zombie();
            base.initializeMatch(multiplayer);
        }
        public override void updateCars(car[] cars, float speed, ref car thisCar)
        {
            if (!GameStateChecker.gamePaused)
            {
                if (thisCar.hp > 0)
                    switch (thisCar.user)
                    {

                        case car.controlType.local:
                            Game1.cam.grabCam(thisCar.position);
                            thisCar.LocalPlayer(speed);
                            break;

                        case car.controlType.network:
                            Game1.cam.grabCam(thisCar.position);
                            break;
                    }

                foreach (car c in cars)
                {
                    if (c.tag != thisCar.tag)
                    {
                        thisCar.CollisionHandler(c, false);
                    }
                }
                for (int i = 0; i != cars.Length; i++)
                {
                    if (cars[i].tag == thisCar.tag)
                    {
                        cars[i] = thisCar;
                    }
                }
            }
            base.updateCars(cars, speed, ref thisCar);
        }
        public override void updateDrivers(AnimSprite[] sprites, ref animGuy thisGuy)
        {
            if (!GameStateChecker.gamePaused)
            {
                switch (thisGuy.user)
                {
                    case car.controlType.ai:
                        thisGuy.AIPlayerZombie(sprites);
                        break;
                    case car.controlType.local:
                        thisGuy.LocalPlayer();
                        break;
                    case car.controlType.network:

                        break;
                }
                thisGuy.DriverHitZombieMode();
            }

            base.updateDrivers(sprites, ref thisGuy);
        }
        public override void updateGame(Microsoft.Xna.Framework.GameTime gt)
        {
            if (Game1.ui.hasStarted)
            {
                /////////////////////////////////////////////////////////////
                //this has been added
                car c = Game1.cars.find_car(gamePlayer.findGamer(GameStateChecker.getProtector()).Gamertag);

                killed = c.kills;

                if (killed >=zombies)
                {
                    GameStateChecker.gameOver = true;
                }
                //////////////////////////////////////////////////////////////

                if (gt.TotalRealTime.Milliseconds % 40 == 0)
                {
                    animGuy driver = Game1.animSprites.sprites[nextZombie] as animGuy;

                    if (driver.zombie)
                    {

                        timeWaited = gt.TotalGameTime.Seconds;

                        Vector2 origin = (Game1.cam.center_literal / Game1.cam.zoom);

                        Game1.animSprites.SpawnDriver(new Vector2(300 * (float)Math.Cos(Math.PI * 2 * rnd.NextDouble()) + Game1.cam.res.X / 2,
                                                                  300 * (float)Math.Sin(Math.PI * 2 * rnd.NextDouble()) + Game1.cam.res.Y / 2),
                                                                  driver.gamerTag);
                    }
                    nextZombie++;

                    if (nextZombie >= Game1.animSprites.sprites.Count)
                    { nextZombie = 0; }
                }
            }

            GameStateChecker.CheckDeadSurvivor();

            base.updateGame(gt);
        }
        public void setSurvivor(animGuy g)
        {
            survivor = g;
        }
        public animGuy getSurvivor()
        {
            return survivor;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace DemoDurb
{
    class ModeTag : GameMode
    {
        GameTime gt;

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

            GameStateChecker.it = null;
            Game1.cars.start_singlePlayer();
            //Game1.cars.start_driverTest();
           base.initializeMatch(multiplayer);
        }

        public override void updateCars(car[] cars,float speed,ref car thisCar)
        {
            if (thisCar.hp > 0 && !GameStateChecker.gamePaused)
            {
                switch (thisCar.user)
                {
                    case car.controlType.ai:
                        if (Game1.ui.hasStarted)
                        {
                            Game1.cam.grabCam(thisCar.position);
                            thisCar.AItag(cars); //add an ai method that avoids the it car
                        }
                        break;

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
                        thisCar.CollisionHandlerTag(c, false);
                    }
                    thisCar.CheckBounds(false);
                }
                for (int i = 0; i != cars.Length; i++)
                {
                    if (cars[i].tag == thisCar.tag)
                    {
                        cars[i] = thisCar;
                    }
                }
                if (gt.TotalGameTime.Milliseconds % 1000 == 0)
                    if (thisCar != GameStateChecker.it)
                    {
                        thisCar.score += 10;
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
                        Game1.cars.LivingCars++;

                        thisGuy.LocalPlayer();
                        break;
                    case car.controlType.network:
                        Game1.cars.LivingCars++;
                        Game1.cam.destination += thisGuy.position;
                        break;
                }

                thisGuy.DriverHit();
            }
            base.updateDrivers(sprites, ref thisGuy);
        }
        public override void openHelp(PlayerIndex p)
        {
            Game1.ui.dispTagHelp(p);

            base.openHelp(p);
        }
        public override void updateGame(GameTime gt)
        {
            this.gt = gt;

            if (Game1.ui.hasStarted)
            {
                GameStateChecker.CheckDeadPlayers();
                GameStateChecker.CheckDeadCars();
                GameStateChecker.updateModeTag(gt);
            }

            base.updateGame(gt);
        }
    }
}

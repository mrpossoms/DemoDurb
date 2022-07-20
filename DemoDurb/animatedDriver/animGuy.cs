using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using System.Linq;
using System.Text;

namespace DemoDurb
{
    public class animGuy : AnimSprite
    {
        public car.controlType user;
        Vector2 velocity;
        public string gamerTag;
        public bool active;
        PlayerIndex index;
        public bool zombie = false;
        private float fatigue = 1;

        public animGuy(AnimSprite a, car.controlType User,int CarTag,string GamerTag):base(a)
        {
            user = User;
            carTag = CarTag;
            gamerTag = GamerTag;
            positionLiteral = new Vector2(-999, -999);
            active = false;

            foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
            {
                if (g.Gamertag == GamerTag)
                {
                    index = g.PlayerIndex;
                }
            }
        }
        public animGuy(AnimSprite a, car.controlType User, int CarTag, string GamerTag,bool isZombie)
            : base(a)
        {
            user = User;
            carTag = CarTag;
            gamerTag = GamerTag;
            zombie = isZombie;
            active = false;

            foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
            {
                if (g.Gamertag == GamerTag)
                {
                    index = g.PlayerIndex;
                }
            }
        }
        public animGuy(AnimSprite a, car.controlType User, int CarTag, string GamerTag, bool isActive,bool isZombie)
            : base(a)
        {
            user = User;
            carTag = CarTag;
            gamerTag = GamerTag;
            zombie = isZombie;
            active = isActive;

            foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
            {
                if (g.Gamertag == GamerTag)
                {
                    index = g.PlayerIndex;
                }
            }
        }
        public override AnimSprite[] update(AnimSprite[] sprites)
        {


            if (Game1.ui.state == UI.UIstate.game && active && !dead )
            {
                animGuy thisGuy = this;

                if (!zombie)
                    Game1.cam.grabCam(position);

                Game1.gameMode.updateDrivers(sprites,ref thisGuy);

                bool collided = false;
                bool collidedWithOpposite = false;

                foreach (AnimSprite s in sprites)
                {
                    animGuy guy = s as animGuy;

                    if (s != this && vecLessThan(new Vector2(-998, -998), this.position) && vecLessThan(new Vector2(-998, -998), guy.position))
                    if (Vector2.Distance(positionLiteral + velocity, guy.positionLiteral) <= frames[0].Width/2) //||
                        //(Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) >= 200))
                    {
                        collided = true;

                        if (this.zombie != guy.zombie)
                        {
                            if (this.zombie)
                            {
                                guy.killDriver();
                            }
                            if (guy.zombie)
                            {
                                this.killDriver();
                            }
                            
                        }
                    }
                    if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) >= 260)
                    {
                        float length = velocity.Length();

                        Vector2 pos = Game1.cam.center_literal / Game1.cam.zoom;
                        float a = (float)Math.Atan2(pos.Y - (positionLiteral / Game1.cam.zoom).Y,
                                                    pos.X - (positionLiteral / Game1.cam.zoom).X);

                        velocity.X = (float)(length * Math.Cos(a));
                        velocity.Y = (float)(length * Math.Sin(a));
                    }
                }

                if (collided)
                {
                    position -= velocity;
                    velocity = Vector2.Zero;
                }
                else
                {
                    position += velocity;
                    velocity *= Game1.level.getFriction();
                }



                if (playing)
                {
                    frame_current += rate;

                    if (frame_current >= frames.Count)
                    {
                        frame_current = 0;
                    }
                }
                //RespawnCar();

                //if (dead && user == car.controlType.local)
                    //Game1.ui.dispPostGame();
                
            }
            return sprites;
        }
        public void GrabCamera()
        {
            if (vecLessThan(new Vector2(-998, -998),position))
            {
                Game1.cars.LivingCars++;
                Game1.cam.destination += position;
            }
        }
        public void DriverHitZombieMode()
        {


            for (int i = 0; i != Game1.cars.cars.Count; i++)
            {
                if (Game1.cars.cars[i].hp > 0)
                    if (zombie)
                        if (Vector2.Distance(Game1.cars.cars[i].positionLiteral / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) < 20)
                        {
                            Game1.cars.cars[i].kills++;
                            killZombie();
                        }
            }
        }
        public void DriverHit()
        {
            

            for (int i = 0; i != Game1.cars.cars.Count; i++)
            {
                if(Game1.cars.cars[i].hp>0) 
                if (carTag != Game1.cars.cars[i].tag || zombie)
                    if (Vector2.Distance(Game1.cars.cars[i].positionLiteral / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) < 10)
                    {
                        Game1.cars.cars[Game1.cars.int_find_car(this.gamerTag)].deaths++;
                        Game1.cars.cars[i].kills++;
                        killDriver();
                    }
            }
        }

        private void killDriver()
        {
            if (!dead)
            {
                dead = true;
                for (int j = 0; j != 10; j++)//3 for zune
                    Game1.particles.addBloodPuff(position);

                Game1.debris.addBlood(position);
                Game1.particles.addKill(position);
                Game1.animSprites.KillDriver(gamerTag);
                Game1.cars.sound.PlayCue("runover");
            }
        }
        private void killZombie()
        {
            if (!dead)
            {
                for (int j = 0; j != 10; j++)//3 for zune
                    Game1.particles.addBloodPuff(position);

                Game1.debris.addBlood(position);
                Game1.particles.addKill(position);
                Game1.animSprites.KillDriver(gamerTag);
                Game1.cars.sound.PlayCue("runover");
            }
        }
        public void RespawnCar()
        {
            if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) < 20)
            {
                Game1.cars.cars[carTag].position = position;
                Game1.cars.cars[carTag].hp = Game1.cars.cars[carTag].MaxHp;
                if(Game1.cars.cars[carTag].user == car.controlType.local)
                                    Game1.cars.cars[carTag].engine.Resume();
                Game1.cars.cars[carTag].fire.Pause();
                Game1.animSprites.KillDriver(gamerTag);
            }
        }

        public void AIPlayer()
        {
            if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) >= 10)
            {
                angle = TurnToFace(position, Game1.cam.center, angle, 0.1f);
                velocity += accelerate(.05f, angle) * (Game1.level.getFriction() * 1.1f);
                playing = true;
            }
        }
        public void AIPlayerZombie(AnimSprite[] sprites)
        {
            if (Game1.ui.hasStarted)
            {
                Vector2 target = new Vector2((float)(Game1.cam.center.X + (-200 + (rand.NextDouble() * 400))),
                                             (float)(Game1.cam.center.Y + (-200 + (rand.NextDouble() * 400))));

                foreach (animGuy g in sprites)
                {
                    if (!g.zombie && g.position != new Vector2(-999, -999))
                        target = g.position;
                }

                angle = TurnToFace(position, target, angle, 0.1f);
                velocity += accelerate(.08f, angle) * (Game1.level.getFriction() * 1.1f);
                playing = true;
            }
        }
        public void LocalPlayer()
        {
            KeyboardState ks = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(index);
            if (fatigue < 1)
                fatigue += 0.05f;
                //Game1.cam.destination = position;
                //Game1.cam.desiredAng = 0;

            playing = false;

            if (Game1.ui.hasStarted)
            {
                float speed = (float)Math.Sqrt((gp.ThumbSticks.Left.X * gp.ThumbSticks.Left.X) + (gp.ThumbSticks.Left.Y * gp.ThumbSticks.Left.Y)) * 1.5f;
                if (gp.IsButtonDown(Buttons.LeftStick))
                {
                    speed *= (3 * fatigue);

                    if(fatigue>=.1f)
                        fatigue -= .1f;
                }
                if (gp.ThumbSticks.Left != Vector2.Zero)
                {
                    playing = true;
                    Vector2 destination = gp.ThumbSticks.Left + position;
                    destination.Y = -gp.ThumbSticks.Left.Y + position.Y;
                    angle = TurnToFaceHuman(position, destination, angle, 0.1f);

                    velocity += accelerate((.05f * speed), angle) * (Game1.level.getFriction() * 1.1f);
                }
                else
                {
                    playing = false;
                }
            }
            //Game1.ui.speed = findVelocity(velocity);
            //Game1.ui.health = (hp / MaxHp) * 100;
        }
        Random rand = new Random();

        private static float TurnToFaceHuman(Vector2 position, Vector2 faceThis,
float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x) + MathHelper.PiOver2;

            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x)+ MathHelper.PiOver2;

            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        public bool vecLessThan(Vector2 vec1, Vector2 vec2)
        {
            if (vec1.X < vec2.X && vec1.Y < vec2.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Vector2 accelerate(float accel, float angle)
        {
            angle -= (Single)Math.PI / 2;
            Vector2 _velo = new Vector2(accel * (Single)Math.Cos(angle),
                                        accel * (Single)Math.Sin(angle));
            return _velo;
        }
        public enum controlType
        { local, ai, network };
    }
}

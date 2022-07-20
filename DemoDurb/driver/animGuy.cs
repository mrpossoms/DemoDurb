using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text;

namespace DemoDurb
{
    class animGuy : AnimSprite
    {
        car.controlType user;
        Vector2 velocity;

        public animGuy(AnimSprite a, car.controlType User,int CarTag):base(a)
        {
            user = User;
            carTag = CarTag;
        }
        public override void update()
        {
            if (Game1.ui.state == UI.UIstate.game)
            {
                switch (user)
                {
                    case car.controlType.ai:
                        if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) >= 10)
                        {
                            angle = TurnToFace(position, Game1.cam.center, angle, 0.1f);
                            velocity += accelerate(.05f, angle);
                            playing = true;
                        }
                        break;
                    case car.controlType.local:
                        LocalPlayer();
                        break;
                }

                velocity *= .95f;
                position += velocity;

                if (playing)
                {
                    frame_current += rate;

                    if (frame_current >= frames.Count)
                    {
                        frame_current = 0;
                    }
                }
                if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) < 20)
                {
                    Game1.cars.cars[carTag].position = position;
                    Game1.cars.cars[carTag].hp = Game1.cars.cars[carTag].MaxHp;
                    remove = true;
                }
                for(int i =0; i!=Game1.cars.cars.Count;i++)
                {
                    if (carTag != Game1.cars.cars[i].tag)
                        if (Vector2.Distance(Game1.cars.cars[i].positionLiteral / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) < 10)
                        {
                            for (int j = 0; j != 3; j++)
                                Game1.particles.addBloodPuff(position);

                            Game1.debris.addBlood(position);
                            Game1.particles.addKill(position);
                            Game1.cars.cars[i].kills++;
                            remove = true;
                        }
                }
            }

        }
        private void LocalPlayer()
        {
            KeyboardState ks = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(PlayerIndex.One);

            Game1.cam.destination = position;
            Game1.cam.desiredAng = -angle;

            playing = false;

            float turnSpeed = .08f;//(-(sqr(speed - 3.5f) / 98) + 0.125f);
            if (Game1.ui.hasStarted)
            {
                if (ks.IsKeyDown(Keys.Right) || gp.IsButtonDown(Buttons.DPadRight))
                {
                    angle += turnSpeed;
                    playing = true;
                }
                if (ks.IsKeyDown(Keys.Left) || gp.IsButtonDown(Buttons.DPadLeft))
                {
                    angle -= turnSpeed;
                    playing = true;
                }
                if (ks.IsKeyDown(Keys.Up) || gp.IsButtonDown(Buttons.A))
                {
                    velocity += accelerate(.05f, angle);
                    playing = true;
                }
            }
            //Game1.ui.speed = findVelocity(velocity);
            //Game1.ui.health = (hp / MaxHp) * 100;
        }
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x) + MathHelper.PiOver2;

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

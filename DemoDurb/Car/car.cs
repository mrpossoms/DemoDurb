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
    public class car
    {

        public Texture2D windows, body,frame,auora;
        public Color paintColor;
        public controlType user;
        public float hp,MaxHp;
        public Vector2 position, positionLiteral;
        public Vector2 velocity;
        PlayerIndex playerIndex;
        public Cue engine,squeal,fire;
        public string gamertag;

        //public net brain;

        public float angle;
        float friction;
        float mass = 1500;

        int dustAddDelay = 0;
        int smokeAddDelay = 0;
        public int score = 0, kills =0,deaths =0;
        int oob_ticks=0;

        public int tag;

        public SpriteFont font;

        public car(Texture2D Body, Texture2D Windows, Texture2D Frame, Texture2D Auora, Vector2 Pos, float Ang, Color PaintColor, controlType User, SpriteFont Font, int Tag, float HP, net Brain, PlayerIndex Pindex, string GamerTag)
        {
            gamertag = GamerTag;
            tag = Tag;

            body = Body;
            windows = Windows;
            frame = Frame;
            auora = Auora;
            position = Pos;
            angle = Ang;
            user = User;
            playerIndex = Pindex;
            font = Font;

            MaxHp = hp = HP;

            paintColor = PaintColor;
            friction = .95f;

            engine = Game1.cars.sound.GetCue("engine");
            engine.SetVariable("speed", 0);

            squeal = Game1.cars.sound.GetCue("squealingTires");
            squeal.Play();
            squeal.Pause();

            fire = Game1.cars.sound.GetCue("fire");
            fire.Play();
            fire.Pause();

            if (user == controlType.local)
                engine.Play();
            //brain = Brain;
        }
        

        public virtual car[] Update(car[] cars)
        {
            if (hp < 0)
            { hp = 0; }

            UpdateDamage();

            float speed = 0;

            if (hp > 0)
            {
                speed = findVelocity(velocity);

                car Car = this;

                if (fire.IsPlaying)
                {
                    fire.Pause();
                }

                if (oob_ticks > 0)
                {
                    oob_ticks--;
                } 
            }
            else
            {
                Game1.animSprites.SpawnDriver(position + Vector2.One, gamertag);
                if (engine.IsPlaying)
                {
                    engine.Pause();
                }
                if (squeal.IsPlaying)
                {
                    squeal.Pause();
                }
            }

            car thisCar = this as car;

            Game1.gameMode.updateCars(cars, speed, ref thisCar);

            velocity *= Game1.level.getFriction();
            position += velocity;
            return cars;
            }

        public void CheckBounds(bool doesCollision)
        {
            if (hp > 0)
                if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) >= 260)
                {
                    collide(doesCollision);
                }
        }

        public void AIdemolition(car[] cars)
        {
            float desired_angle = 0;
            int d = closestDriver(position,tag,Game1.animSprites.sprites.ToArray());
            float turnSpeed = 0.25f * (findVelocity(velocity) / 10);

            if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) >= 200)
            {
                angle = TurnToFace(position, Game1.cam.center, angle, turnSpeed);
            }
            else if(d!=-1)
            {
                angle = TurnToFace(position / Game1.cam.zoom, Game1.animSprites.sprites[d].position / Game1.cam.zoom, angle, (0.0025f * hp) * (findVelocity(velocity) / 10));
            }
            else
            {
                if (hp > 25)
                {
                    int c = closestCar(position, tag, cars);
                    angle = TurnToFace(position / Game1.cam.zoom, cars[c].position / Game1.cam.zoom, angle, turnSpeed);
                }
                else
                {
                    int c = closestCar(position, tag, cars);
                    angle = TurnToFace(position / Game1.cam.zoom, -(cars[c].position / Game1.cam.zoom), angle, turnSpeed);
                }
            }

            float acc = .132f;

            /*
            if (GameStateChecker.it == this)
                acc += .025f;
            */

            velocity += accelerate(acc, angle) * (Game1.level.getFriction() * 1.1f);
        }
        public void AItag(car[] cars)
        {
            float desired_angle = 0;
            float turnSpeed = 0.25f * (findVelocity(velocity) / 10);

            if (Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, positionLiteral / Game1.cam.zoom) >= 200)
            {
                angle = TurnToFace(position, Game1.cam.center, angle, turnSpeed);
            }
            else
            {
                int c = closestCar(position, tag, cars);

                if (GameStateChecker.it == this)
                {
                    angle = TurnToFace(position / Game1.cam.zoom, (cars[c].position / Game1.cam.zoom), angle, turnSpeed);
                }
                else
                {
                    if (GameStateChecker.it == cars[c])
                    {
                        angle = TurnToFace(position / Game1.cam.zoom, -(cars[c].position / Game1.cam.zoom), angle, turnSpeed);
                    }
                }
            }

            float acc = .132f;

            if (GameStateChecker.it == this)
                acc += .025f;

            velocity += accelerate(acc, angle) * (Game1.level.getFriction() * 1.1f);
        }

        private void UpdateDamage()
        {
            if (smokeAddDelay == 2)
            {
                float ratio = (hp / MaxHp);

                if (ratio < .75f && ratio >= .5f)
                    Game1.particles.addGreySmoke(position);
                if (ratio < .5f && ratio >= .25f)
                    Game1.particles.addBlackSmoke(position);
                if (ratio < .25f && ratio > 0f)
                {
                    Game1.particles.addLivingFire(position);
                    Game1.particles.addBlackSmoke(position);
                }
                if (ratio == 0)
                    Game1.particles.addFire(position+randomPos(-Vector2.One,Vector2.One));
                smokeAddDelay = 0;
            }
            smokeAddDelay++;
            int bodyCount = Game1.cars.body.Count - 1;
            int windowCount = Game1.cars.windows.Count - 1;

            this.body = Game1.cars.body[(int)(bodyCount - (bodyCount * (hp / MaxHp)))];
            this.windows = Game1.cars.windows[(int)(windowCount - (windowCount * (hp / MaxHp)))];
        }

        public void CollisionHandler(car c,bool collisionsDoDamage)
        {
            if (Vector2.Distance(position, c.position) < body.Width / 2+5)
            {
                //create particles
                float Speed1 = findVelocity(velocity);
                float Speed2 = findVelocity(c.velocity);
                if (Speed1 > Speed2)
                {
                    if (c.hp > 0)
                    {
                        Game1.particles.addGreySmoke((position + c.position) / 2);

                        if (collisionsDoDamage)
                        {
                            c.hp -= (mass * Speed1) / 1000;
                            if (user == controlType.local)
                                Game1.particles.add10(c.position);
                            score += 10;
                        }

                        if (RandomNumber(0, 4) < 1)
                        {
                            //Game1.debris.addParts(c.position + randomPos(-Vector2.One * 20, Vector2.One * 10));
                            //Game1.debris.addPaintChips(c.position + randomPos(-Vector2.One * 10, Vector2.One * 10), c.paintColor);
                        }

                        if (c.hp < 0)
                        {
                            //Game1.animSprites.addDriver(c.position + Vector2.One * 50,c.user,c.tag,c.gamertag); //place the driver after the car died
                            //Game1.animSprites.SpawnDriver(c.position + Vector2.One * 50, c.gamertag);
                            Game1.debris.addCharMark(c.position);
                            Game1.particles.addExplosion(c.position);
                            Game1.particles.add100(c.position);
                            score += 100;
                            fire.Resume();
                            Game1.cars.sound.PlayCue("explosion");
                            //c.deaths++;
                        }

                        //if (Game1.cars.game_state == CarManager.gameState.learning &&
                        //    user == controlType.ai)
                        //{
                        //}    for neural net implication
                    }
                }
                else
                {
                    if (hp > 0)
                    {
                        Game1.particles.addBlackSmoke((position + c.position) / 2);
                        
                        if (collisionsDoDamage)
                        {
                            hp -= (c.mass * Speed2) / 1000;
                        }
                        
                        if (RandomNumber(0, 4) < 1)
                        {
                            Game1.debris.addParts(position + randomPos(-Vector2.One * 10, Vector2.One * 10));
                            Game1.debris.addPaintChips(position + randomPos(-Vector2.One * 10, Vector2.One * 10), paintColor);
                        }
                        if (hp < 0)
                        {
                            //Game1.animSprites.addDriver(c.position + Vector2.One * 10, user,tag,gamertag); //place the driver after the car died
                            //Game1.animSprites.SpawnDriver(c.position + Vector2.One * 50, c.gamertag);
                            Game1.debris.addCharMark(c.position);
                            Game1.particles.addExplosion(position);
                            fire.Resume();
                            Game1.cars.sound.PlayCue("explosion");
                            //deaths++;
                        }

                        //if (Game1.cars.game_state == CarManager.gameState.learning &&
                        //    c.user == controlType.ai)
                        //{
                        //}
                    }
                }

                collide(c);
                //deal damage

            }
        }
        public void CollisionHandlerTag(car c,bool collisionsDoDamage)
        {
            if (Vector2.Distance(position, c.position) < body.Width / 2)
            {
                //create particles
                float Speed1 = findVelocity(velocity);
                float Speed2 = findVelocity(c.velocity);
                if (Speed1 > Speed2)
                {
                    if (c.hp > 0)
                    {
                        
                        Game1.particles.addGreySmoke((position + c.position) / 2);

                        if (collisionsDoDamage)
                        {
                            c.hp -= (mass * Speed1) / 1000;
                            if (user == controlType.local)
                                Game1.particles.add10(c.position);
                            score += 10;
                        }



                        if (c.hp < 0)
                        {
                            //Game1.animSprites.addDriver(c.position + Vector2.One * 50,c.user,c.tag,c.gamertag); //place the driver after the car died
                            ////////Game1.animSprites.SpawnDriver(c.position + Vector2.One * 50, c.gamertag);
                            carExplode(ref c);
                        }

                    }
                }
                else
                {
                    if (hp > 0)
                    {
                        if (collisionsDoDamage)
                        {
                            Game1.particles.addBlackSmoke((position + c.position) / 2);
                            hp -= (c.mass * Speed2) / 1000;
                        }

                        if (RandomNumber(0, 4) < 1)
                        {
                            Game1.debris.addParts(position + randomPos(-Vector2.One * 10, Vector2.One * 10));
                            Game1.debris.addPaintChips(position + randomPos(-Vector2.One * 10, Vector2.One * 10), paintColor);
                        }
                        if (hp < 0)
                        {
                            car Car = this as car;

                            carExplode(ref Car);
                        }

                    }
                }
                collide(c);

                if (this == GameStateChecker.it && c.hp>0 && c.oob_ticks ==0) // if this car is it. make your target it now
                {
                    this.oob_ticks = 10; //no tag backs while > 0
                    GameStateChecker.becomeIt(c);
                    return;
                }
                if (c == GameStateChecker.it && this.hp > 0 && this.oob_ticks == 0) // if this car is it. make your target it now
                {
                    c.oob_ticks = 10; //no tag backs while > 0
                    GameStateChecker.becomeIt(this);
                    return;
                }
                //deal damage

            }

        }

        public void carExplode(ref car c)
        {
            Game1.debris.addCharMark(c.position);
            Game1.particles.addExplosion(c.position);
            c.fire.Resume();
            Game1.cars.sound.PlayCue("explosion");
            //c.deaths++;
        }

        public void collide(car c)
        {
            for (int i = 0; i != 9; i++)
                Game1.particles.addSpark((position + c.position) / 2,c.velocity*2);
            //reflect
            float ang = (float)(PointAt(position, c.position) + Math.PI);
            velocity = accelerate(2f, ang);
            position += velocity;

            Game1.cars.sound.PlayCue("crash");

            if (user == controlType.local)
            {
                rumble.addVib(playerIndex, 100,1,0, 0);
            }
        }
        public void collide(bool doesCollision)
        {
            for (int i = 0; i != 9; i++)
                Game1.particles.addSpark((position),velocity*2);
            //reflect
            float ang = (float)(PointAt(position, Game1.cam.center) + Math.PI/2);
            velocity = accelerate(2f, ang);
            position += velocity;
            if (doesCollision)
            {
                hp -= (mass * findVelocity(velocity)) / 1000;
            }
            Game1.cars.sound.PlayCue("crash");

            if (user == controlType.local)
            {
                rumble.addVib(playerIndex, 100, 1, 0, 0);
            }
        }

        public void LocalPlayer(float speed)
        {
            KeyboardState ks = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(playerIndex);

            //Game1.cam.destination = position;
            //Game1.cam.desiredAng = -angle;

            float turnSpeed =speed * .02f;//(-(sqr(speed - 3.5f) / 98) + 0.125f);

            float acc = .15f;
            if (GameStateChecker.it == this)
                acc += .025f;

            float acceleration = acc * (Game1.level.getFriction()*1.1f);
            if (Game1.ui.hasStarted && Game1.ui.state != UI.UIstate.gameOver)
            {
                if (ks.IsKeyDown(Keys.LeftControl) || gp.Triggers.Right != 0)
                {
                    if (gp.Triggers.Right != 0)
                    {
                        turnSpeed *= (gp.Triggers.Right * 1.5f);
                        acceleration = MathHelper.Clamp((acceleration - (gp.Triggers.Right * acceleration)),
                                                        0.05f, 1);

                        //if (!Game1.cars.squeal.IsPlaying)
                        {
                            squeal.SetVariable("speed", MathHelper.Clamp((speed / 4), 0, 1));
                            squeal.Resume();
                        }

                    }

                    turnSpeed *= 2;
                }
                else
                {
                    squeal.Pause();
                }
                if (ks.IsKeyDown(Keys.Right) || gp.IsButtonDown(Buttons.DPadRight))
                {
                    angle += turnSpeed;
                }
                if (ks.IsKeyDown(Keys.Left) || gp.IsButtonDown(Buttons.DPadLeft))
                {
                    angle -= turnSpeed;
                }
                if (ks.IsKeyDown(Keys.Up) || gp.IsButtonDown(Buttons.A))
                {
                    if (dustAddDelay == 4)
                    {
                        Game1.particles.addDust(position);
                        dustAddDelay = 0;
                    }
                    else
                    {
                        dustAddDelay++;
                    }
                    rumble.addVib(this.playerIndex, 0,0, 50,(speed/14f));
                    velocity += accelerate(acceleration, angle);// - (gp.Triggers.Right*.15f)
                }
                if (ks.IsKeyDown(Keys.Down) || gp.IsButtonDown(Buttons.B))
                {
                    if (dustAddDelay == 4)
                    {
                        Game1.particles.addDust(position);
                        dustAddDelay = 0;
                    }
                    else
                    {
                        dustAddDelay++;
                    }
                    rumble.addVib(this.playerIndex, 0, 0, 50, (speed / 14f));
                    velocity -= accelerate(acceleration*.75f, angle);// - (gp.Triggers.Right*.15f)
                }

            }
            angle += gp.ThumbSticks.Left.X*turnSpeed;
            //Game1.ui.speed = findVelocity(velocity);
            //Game1.ui.health = (hp / MaxHp) * 100;
            //Game1.ui.score = score;
            engine.SetVariable("speed",MathHelper.Clamp((speed / 4),0,1));
            Game1.ui.updateHud(playerIndex, speed, hp,score,paintColor);
        }


        public void draw(Camera cam,SpriteBatch sb,Texture2D shadow)
        {
            positionLiteral = ((position - cam.position) * cam.zoom) + (cam.res / 2);

            float dist = Vector2.Distance(positionLiteral, cam.res / 2);
            positionLiteral = OrbitPivot(cam.res / 2, positionLiteral, cam.angle, 0);

            dist = Vector2.Distance(cam.center_literal / cam.zoom, positionLiteral / cam.zoom);


            sb.Begin();

            if (this == GameStateChecker.it)
            {
                float shrink = GameStateChecker.getDeathProgress();

                if (shrink > 1)
                {
                    shrink = 0;
                }

                sb.Draw(auora, positionLiteral, null, Color.Red, cam.angle,
                    new Vector2(auora.Width, auora.Height) / 2,
                    (cam.zoom * 6f) * shrink, SpriteEffects.None, 0);
            }

            sb.Draw(shadow, positionLiteral, null, Color.White, cam.angle,
                    new Vector2(shadow.Width, 70) / 2,
                    cam.zoom * .5f, SpriteEffects.None, 0);
            sb.Draw(frame, positionLiteral, null, Color.White, angle + cam.angle,
                    new Vector2(body.Width, body.Height) / 2,
                    cam.zoom * .5f, SpriteEffects.None, 0);
            if (hp > 0)
            {
                sb.Draw(body, positionLiteral, null, paintColor, angle + cam.angle,
                        new Vector2(body.Width, body.Height) / 2,
                        cam.zoom * .5f, SpriteEffects.None, 0);
                sb.Draw(windows, positionLiteral, null, Color.White, angle + cam.angle,
                        new Vector2(body.Width, body.Height) / 2,
                        cam.zoom * .5f, SpriteEffects.None, 0);
            }

            if(user != controlType.ai)
            if (Game1.ui.hasStarted == false)
            {
                foreach (SignedInGamer g in SignedInGamer.SignedInGamers)
                {
                    if (gamertag == g.Gamertag)
                    {
                        gamePlayer player = g.Tag as gamePlayer;

                        sb.DrawString(font,gamertag +"\n K: " + player.kills + "\n D: " + player.deaths, positionLiteral, paintColor);
                    }
                }
            }
            else
            {
                sb.DrawString(font, gamertag, positionLiteral, new Color(paintColor,120));
            }

            sb.End();
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
        public int closestCar(Vector2 pos,int tag,car[] cars)
        {
            float distance=9999;
            int out_tag=0;

            for (int i = 0; i != cars.Length;i++)
            {
                if (tag != cars[i].tag && cars[i].hp > 0)
                {
                    float tempDist = Vector2.Distance(pos, cars[i].positionLiteral);

                    if (tempDist < distance && tempDist != 0)
                    {
                        out_tag = i;
                        distance = tempDist;
                    }
                }
            }
            return out_tag;
        }
        public int closestDriver(Vector2 pos, int tag, AnimSprite[] sprites)
        {
            float distance = 9999;
            int out_tag = -1;

            for (int i = 0; i != sprites.Length; i++)
            {
                animGuy guy = sprites[i] as animGuy;

                if (gamertag != guy.gamerTag && guy.active)
                {
                    float tempDist = Vector2.Distance(pos, guy.positionLiteral);

                    if (tempDist < distance && tempDist != 0)
                    {
                        out_tag = i;
                        distance = tempDist;
                    }
                }
            }
            return out_tag;
        }

        
        #region Translation
        private Vector2 accelerate(float accel, float angle)
        {
            angle -= (Single)Math.PI / 2;
            Vector2 _velo = new Vector2(accel * (Single)Math.Cos(angle),
                                        accel * (Single)Math.Sin(angle));
            return _velo;
        }
        private static Vector2 OrbitPivot(Vector2 pivot, Vector2 obj, float angle, float angle_old)
        {
            float dist = Vector2.Distance(pivot, obj);
            Vector2 angCalc = pivot - obj;
            float offset = PointAt(pivot, obj);//Convert.ToSingle(Math.Atan2(angCalc.Y/angCalc.X)); 
            Vector2 output = Vector2.Zero;
            output.X = Convert.ToSingle(dist * Math.Cos(offset + -(angle_old - angle)) + pivot.X);
            output.Y = Convert.ToSingle(dist * Math.Sin(offset + -(angle_old - angle)) + pivot.Y);
            return output;
        }
        private static float PointAt(Vector2 obj, Vector2 target)
        {
            float f_ang;
            f_ang = (Single)Math.Atan2((double)(obj.Y - target.Y), (double)(obj.X - target.X));
            f_ang += (float)Math.PI;
            return f_ang;
        }
        private float findVelocity(Vector2 velocity)
        {

            float velo = velocity.Length();
            return velo;
        } 
        #endregion
        #region RandomShit
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
    float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x)+MathHelper.PiOver2;

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
        private float resolveImpulse(float normal, float velocity1, float mass1, float velocity2, float mass2, float restitution)
        {
            float reletiveVelocity = velocity1 - velocity2;

            float impulse = (-(1 + restitution) * reletiveVelocity * normal) /
                  ((normal * normal) * ((1 / mass1) + (1 / mass2)));
            if (float.IsNaN(impulse))
            {
                impulse = 0;
            }

            return impulse;
        }
        private float getResultantVelo(float init_velo, float impulse, float normal, float mass)
        {
            return init_velo + (impulse / mass) * normal;
        }
        private Vector2 elasticCollision(Vector2 V1i, Vector2 V2i, float M1, float M2)
        {
            Vector2 NewVelo = ((V1i * (M1 - M2) + 2 * M2 * V2i) / (M1 + M2));
            return NewVelo;
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
        public static Vector2 randomPos(Vector2 min, Vector2 max)
        {
            Vector2 output = Vector2.Zero;
            output.X = Convert.ToSingle(RandomNumber(min.X, max.X));
            output.Y = Convert.ToSingle(RandomNumber(min.Y, max.Y));
            return output;
        }
        public static float RandomNumber(double min, double max)
        {
            float OUT = (float)((max - min) * m_Rand.NextDouble() + min);
            return OUT;
        }
        private static Random m_Rand = new Random();
        private bool inBounds(Vector2 pos)
        {
            Vector2 posLit = ((pos - Game1.cam.position) * Game1.cam.zoom) + (Game1.cam.res / 2);
            posLit = OrbitPivot(Game1.cam.res / 2, positionLiteral, Game1.cam.angle, 0);

            float dist = Vector2.Distance(Game1.cam.center_literal / Game1.cam.zoom, posLit / Game1.cam.zoom);
            if (dist < 200)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float sqr(float input)
        {
            return input * input;
        } 
        #endregion

        public enum controlType
        { local, ai, network };

    }
}
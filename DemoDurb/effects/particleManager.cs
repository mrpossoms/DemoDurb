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
    public class particleManager : DrawableGameComponent
    {
        SpriteBatch sb;

        List<particle> particles = new List<particle>();

        List<Texture2D> dust = new List<Texture2D>();
        List<Texture2D> smoke_grey = new List<Texture2D>();
        List<Texture2D> smoke_black = new List<Texture2D>();
        List<Texture2D> fire = new List<Texture2D>();
        List<Texture2D> points = new List<Texture2D>();
        List<Texture2D> blood_puff = new List<Texture2D>();
        Texture2D spark;
        Texture2D explosion;

        public particleManager(Game game)
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
            sb = new SpriteBatch(Game.GraphicsDevice);
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            for (int i = 0; i != 3; i++)
            {
                blood_puff.Add(Game.Content.Load<Texture2D>("blood_puff" + i.ToString()));
            }
            for (int i = 0; i != 3; i++)
            {
                dust.Add(Game.Content.Load<Texture2D>("dust" + i.ToString()));
            }
            for (int i = 0; i != 3; i++)
            {
                points.Add(Game.Content.Load<Texture2D>("point_" + i.ToString()));
            }
            for (int i = 0; i != 3; i++)
            {
                fire.Add(Game.Content.Load<Texture2D>("fire" + i.ToString()));
            }
            for (int i = 0; i != 4; i++)
            {
                smoke_grey.Add(Game.Content.Load<Texture2D>("smoke_grey" + i.ToString()));
            }
            for (int i = 0; i != 4; i++)
            {
                smoke_black.Add(Game.Content.Load<Texture2D>("smoke_black" + i.ToString()));
            }

            spark = Game.Content.Load <Texture2D>("spark");
            explosion = Game.Content.Load<Texture2D>("explosion");

        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        public void add10(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One * 6, Vector2.One * 6);

            particles.Add(new particle(points[0], position, velocity, RandomNumber(-.2, .2), particle.behavior.none, .95f, 1, 1,50, true));
        }
        public void addKill(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One * 6, Vector2.One * 6);

            particles.Add(new particle(points[2], position, velocity, RandomNumber(-.2, .2), particle.behavior.none, .95f, 1, 1, 50, true));
        }
        public void add100(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One * 3, Vector2.One * 3);

            particles.Add(new particle(points[1], position, velocity, RandomNumber(-.2, .2), particle.behavior.none, .95f, 0, 1, 25, true));
        }
        public void addBlackSmoke(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One, Vector2.One);

            particles.Add(new particle(smoke_black[roundUp(RandomNumber(0, 3))], position, velocity, RandomNumber(-.2, .2), particle.behavior.grow, .95f, 0,1, 25, true));
        }
        public void addFireMM(Vector2 position)
        {
            Vector2 velocity = new Vector2(RandomNumber(-0.5,0.5),RandomNumber(-1,-3));//randomPos(-Vector2.One, Vector2.One);

            particles.Add(new particle(fire[roundUp(RandomNumber(0, 2))], position, velocity, RandomNumber(-.2, .2), particle.behavior.shrink, 1, 2, 0, 125, true));
        }
        public void addFire(Vector2 position)
        {
            Vector2 velocity = Vector2.Zero;//randomPos(-Vector2.One, Vector2.One);

            particles.Add(new particle(fire[roundUp(RandomNumber(0, 2))], position, velocity, RandomNumber(-.2, .2), particle.behavior.grow, .95f, 0, 2, 25, true));
        }
        public void addLivingFire(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One, Vector2.One);

            particles.Add(new particle(fire[roundUp(RandomNumber(0, 2))], position, velocity, RandomNumber(-.2, .2), particle.behavior.shrink, .95f, .75f, 0, 30, true));
        }
        public void addExplosion(Vector2 position)
        {
            Vector2 velocity = Vector2.Zero;//randomPos(-Vector2.One, Vector2.One);

            particles.Add(new particle(explosion, position, velocity,0, particle.behavior.grow, .95f, 0, 4, 20, true));
        }
        public void addGreySmoke(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One, Vector2.One);

            particles.Add(new particle(smoke_grey[roundUp(RandomNumber(0, 3))], position, velocity, RandomNumber(-.2, .2), particle.behavior.grow, .95f, 0, 1, 25, true));
        }
        public void addBloodPuff(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One * .5f, Vector2.One * .5f);

            particles.Add(new particle(blood_puff[roundUp(RandomNumber(0, 2))], position, velocity, RandomNumber(-.1, .1), particle.behavior.grow, 1, 0,2, 75, true));
        }
        public void addDust(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One, Vector2.One);
            
            particles.Add(new particle(dust[roundUp(RandomNumber(0,2))], position, velocity, RandomNumber(-.2,.2), particle.behavior.grow, .95f, .5f, 1, 25,true));
        }
        public void addSpark(Vector2 position)
        {
            Vector2 velocity = randomPos(-Vector2.One*4, Vector2.One*4);

            particles.Add(new particle(spark, position, velocity, RandomNumber(-.5, .5), particle.behavior.shrink, 1f, .75f, 0, 50, true));
        }
        public void addSpark(Vector2 position, Vector2 direction)
        {
            Vector2 velocity = randomPos(-Vector2.One * 2, Vector2.One * 2) + direction;

            particles.Add(new particle(spark, position, velocity, RandomNumber(-.5, .5), particle.behavior.shrink, 1f, .75f, 0, 50, true));
        }
        public void clearAll()
        {
            particles.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //List<par

            int i = 0;
            while(particles.Count > i)
            {
                particle p = particles[i];

                    p.update();
                    if (p.life <= p.lived)
                    {
                        particles.Remove(p);
                    }
                    i++;
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            for(int i =particles.Count-1;i!=-1;i--)
                particles[i].draw(Game1.cam, sb);
            base.Draw(gameTime);
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

    }
}
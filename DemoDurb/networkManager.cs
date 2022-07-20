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
    public class networkManager : Microsoft.Xna.Framework.GameComponent
    {
        NetworkSession session;

        LocalNetworkGamer host = null;

        PacketReader pr = new PacketReader();
        PacketWriter pw = new PacketWriter();

        public networkManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }
        public NetworkSession getSession()
        { return session; }
        public LocalNetworkGamer getHost()
        { return host; }
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
            if (session != null)
            {
                foreach (LocalNetworkGamer gamer in session.LocalGamers)
                {
                    //SendPackets(gamer);
                    if (gamer.IsHost)
                        host = gamer;

                    if(gameTime.TotalGameTime.Milliseconds % 50 ==0)
                    {
                        sendCarDataFixLag(gamer);
                    }

                        sendCarData(gamer);

                        sendDriverData(gamer);

                    if(pw.Length>0)
                    gamer.SendData(pw, SendDataOptions.InOrder);
                }

                session.Update();

                if (session == null)
                    return;

                foreach (LocalNetworkGamer gamer in session.LocalGamers)
                {
                    while (gamer.IsDataAvailable)
                    {
                        NetworkGamer netGamer;
                        gamer.ReceiveData(pr, out netGamer);

                        if (!netGamer.IsLocal)
                        {
                            decodeData(netGamer);
                        }
                        else
                            continue;
                    }
                }
            }
            base.Update(gameTime);
        }

        private void decodeData(NetworkGamer netGamer)
        {

            if (pr.Position < pr.Length)
            {

                messageType message = (messageType)pr.ReadInt32();

                switch (message)
                {
                    case messageType.updateCar:
                        getCarData(netGamer);
                        break;
                    case messageType.updateCarLagFix:
                        getCarDataFixLag(netGamer);
                        break;
                    case messageType.updateDriver:
                        getDriverData(netGamer);
                        break;
                    case messageType.gameModeSetup:
                        getGameMode();
                        break;
                    case messageType.updateGameMode:
                        getGameModeDataUpdate();
                        break;
                }
            }
            else
                return;

            if (pr.Position < pr.Length)
            {
                decodeData(netGamer);
            }
        }

        public void hostGame()
        {
            session = NetworkSession.Create(NetworkSessionType.SystemLink,
                       4, 16);
            HookSessionEvents();
        }
        public void LookForSessions(string gameMode)
        {
            try
            {
                using (AvailableNetworkSessionCollection availableSessions =
            NetworkSession.Find(NetworkSessionType.SystemLink,
                                4, null))
                {
                    if (availableSessions.Count == 0)
                    {
                        switch (gameMode)
                        {
                            case "demolition":
                                    UI.FadeOut(UI.UIstate.game, true,UI.gameModes.demo);
                                    hostGame();
                                break;
                        }
                        //Create a session if one is not present

                    }
                    else
                    {
                        session = NetworkSession.Join(availableSessions[0]);
                        HookSessionEvents();
                    }
                }
            }
            catch{}
            //hook session events here
        }

        void HookSessionEvents()
        {
            session.GamerJoined += GamerJoinedEventHandler;
            session.GamerLeft += GamerLeftEventHandler;
            session.SessionEnded += SessionEndedEventHandler;
        }
        private void playerJoin(NetworkGamer gamer)
        {
            LocalNetworkGamer g = gamer as LocalNetworkGamer;


            if (!gamer.IsLocal)
            {
                Game1.cars.spawn_car_network(Game1.cars.cars.Count, gamer.Gamertag);
            }
            else
            {
                Game1.cars.spawn_car_local(Game1.cars.cars.Count, g.SignedInGamer.PlayerIndex, g.Gamertag);
            }

            //base.playerJoin(gamer);
        }
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {

            NetworkGamer joiner = null;
            LocalNetworkGamer host = null;

            foreach (NetworkGamer ng in session.AllGamers)
            {
                if (ng.Gamertag == e.Gamer.Gamertag)
                {
                    joiner = ng;
                }
            }

            foreach (LocalNetworkGamer s in session.LocalGamers)
            {
                if (s.IsHost)
                { host = s; }
            }

            playerJoin(joiner);

            if (host != null && !joiner.IsLocal)
            {
                sendGameModeData(host, joiner);
            }

        }
        void GamerLeftEventHandler(object sender, GamerLeftEventArgs e)
        {
            car Car = Game1.cars.find_car(e.Gamer.Gamertag);

            Car.engine.Pause();
            Car.fire.Pause();
            Car.squeal.Pause();

            if (e.Gamer.IsHost)
            {
                
                UI.FadeOut(UI.UIstate.main_menu, false, UI.gameModes.none);
                //endSession();
            }
            else
            {
                int driver = Game1.animSprites.findDriver(e.Gamer.Gamertag);

                try
                {

                    Game1.cars.cars.Remove(Car);
                    Game1.animSprites.sprites.RemoveAt(driver);
                }
                catch { }
            }

        }
        public void endSession()
        {
            if (session != null)
            {
                //Game1.cars.cars.Clear();
                Game1.debris.clearAll();
                Game1.animSprites.sprites.Clear();
                session.Dispose();
                session = null;

                //UI.f
                //UI.FadeOut(UI.UIstate.main_menu, false, UI.gameModes.none);
            }
        }

        #region Car&DriverMessages
        private void sendCarData(LocalNetworkGamer sender)
        {
            car Car = Game1.cars.find_car(sender.Gamertag);

            //Indicate the message type
            pw.Write((int)messageType.updateCar);

            //send the name of the car being updated
            pw.Write(sender.Gamertag);

            //just send the pos for now
            pw.Write(Car.velocity.X);
            pw.Write(Car.velocity.Y);

            //Send the angle
            pw.Write(Car.angle);

            //Send the 
            pw.Write(Car.hp);

            //sender.SendData(pw, SendDataOptions.InOrder);
        }
        private void sendCarDataFixLag(LocalNetworkGamer sender)
        {
            car Car = Game1.cars.find_car(sender.Gamertag);

            //Indicate the message type
            pw.Write((int)messageType.updateCarLagFix);

            //send the name of the car being updated
            pw.Write(sender.Gamertag);

            //just send the pos for now
            pw.Write(Car.position.X);
            pw.Write(Car.position.Y);

            //Send the angle
            pw.Write(Car.angle);

            //Send the 
            pw.Write(Car.hp);

            //sender.SendData(pw, SendDataOptions.InOrder);
        }
        private void sendDriverData(LocalNetworkGamer sender)
        {
            int d = Game1.animSprites.findDriver(sender.Gamertag);

            pw.Write((int)messageType.updateDriver);

            pw.Write(Game1.animSprites.sprites[d].position.X);
            pw.Write(Game1.animSprites.sprites[d].position.Y);

            pw.Write(Game1.animSprites.sprites[d].angle);

            //sender.SendData(pw, SendDataOptions.InOrder);

        }


        //added the place holder for recieving data for a car
        //that belongs to a player who is nolonger in the game
        private void getCarData(NetworkGamer sender)
        {
            int c = Game1.cars.int_find_car(sender.Gamertag);
            
            //define this place holder to recieve data even if the car has been removed
            car temp = new car(null,null,null,null,
                               Vector2.Zero,0,Color.White,
                               car.controlType.network,null,
                               0,0,null,PlayerIndex.One,"");;
            
            //if it exists get a ref to it
            if(c!=-1)
            {temp = Game1.cars.cars[c];}

            //Read the name, not sure if we will need it tho
            string name = pr.ReadString();

            //Read the position vector
            temp.velocity.X = pr.ReadSingle();
            temp.velocity.Y = pr.ReadSingle();

            //Read the angle
            temp.angle = pr.ReadSingle();

            //Read the health
            temp.hp = pr.ReadSingle();

            //write back changes (may not be needed)
            Game1.cars.cars[c] = temp;
        }
        private void getCarDataFixLag(NetworkGamer sender)
        {
            int c = Game1.cars.int_find_car(sender.Gamertag);

            //Read the name, not sure if we will need it tho
            string name = pr.ReadString();

            //define this place holder to recieve data even if the car has been removed
            car temp = new car(null,null,null,null,
                               Vector2.Zero,0,Color.White,
                               car.controlType.network,null,
                               0,0,null,PlayerIndex.One,"");

            //if it exists get a ref to it
            if (c != -1)
            { temp = Game1.cars.cars[c]; }

            //Read the position vector
            temp.position.X = pr.ReadSingle();
            temp.position.Y = pr.ReadSingle();

            //Read the angle
            temp.angle = pr.ReadSingle();

            //Read the health
            temp.hp = pr.ReadSingle();

        }
        private void getDriverData(NetworkGamer sender)
        {
            int d = Game1.animSprites.findDriver(sender.Gamertag);

            animGuy guy = new animGuy(null, car.controlType.network, 0, "");

            if(d!=-1)
            { guy = Game1.animSprites.sprites[d] as animGuy; }

            //Read the position vector
            guy.position.X = pr.ReadSingle();
            guy.position.Y = pr.ReadSingle();

            //Read the angle
            guy.angle = pr.ReadSingle();

        }
        
        #endregion

        #region GameModeMessages
        public void sendGameModeDataUpdate(LocalNetworkGamer host,SessionState state)
        {
            //indicate the message type
            pw.Write((int)messageType.updateGameMode);

            //Send desired state of the session
            pw.Write((int)state);
        }
        private void sendGameModeData(LocalNetworkGamer host, NetworkGamer recipient)
        {
            pw.Flush();

            //Indicate that we are setting up the game
            pw.Write((int)messageType.gameModeSetup);
            //Send the title of the 
            pw.Write(Game1.gameMode.Title);

            host.SendData(pw, SendDataOptions.InOrder, recipient);

        }
        private void getGameMode()
        {
            string gameMode = pr.ReadString();

            switch (gameMode)
            {
                case "demolition":
                    UI.FadeOut(UI.UIstate.game, true, UI.gameModes.demo);
                    break;
            }
        }
        private void getGameModeDataUpdate()
        {
            SessionState nextState = (SessionState)pr.ReadInt32();

            switch (nextState)
            {
                case SessionState.retry:
                    Game1.gameMode.MPreset();
                    break;
            }
        }
        #endregion

        public enum messageType
        { gameModeSetup, updateGameMode, updateCars, updateCar,updateCarLagFix, updateDrivers, updateDriver };
        public enum SessionState
        { quit, retry, playing };
        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            session.Dispose();
            session = null;
        }
    }
}
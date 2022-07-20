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
    class networking
    {
        public static NetworkSession session;

        PacketReader pr = new PacketReader();
        PacketWriter pw = new PacketWriter();

        public static void LookForSessions()
        {
            try
            {
                using (AvailableNetworkSessionCollection availableSessions =
            NetworkSession.Find(NetworkSessionType.SystemLink,
                                4, null))
                {
                    if (availableSessions.Count == 0)
                    {
                        session = NetworkSession.Create(NetworkSessionType.SystemLink,
                                               4, 8);
                        //Create a session if one is not present
                    }
                    else
                    {
                        session = NetworkSession.Join(availableSessions[0]);
                    }
                }
            }
            catch
            {
            }

            //hook session events here
        }
        void HookSessionEvents()
        {
            session.GamerJoined += GamerJoinedEventHandler;
            session.SessionEnded += SessionEndedEventHandler;
        }
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            //int gamerIndex = networkSession.AllGamers.IndexOf(e.Gamer);

            e.Gamer.Tag = Game1.cars.cars.Count;
            Game1.cars.spawn_car_network(Game1.cars.cars.Count,e.Gamer.Gamertag);
        }
        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            session.Dispose();
            session = null;
        }

        public void update()
        {
            if (session != null)
            {
                for (int i = 0; i != session.LocalGamers.Count;i++)
                {
                    car temp = Game1.cars.find_car(i); //the id of the car corresponds to the local gamers
                                                       //position in the list
                    pw.Write(temp.position);
                    pw.Write(temp.angle);
                    pw.Write(temp.hp);
                }

                session.Update();


            }
        }
    }
}

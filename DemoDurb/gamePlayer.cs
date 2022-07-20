using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;

namespace DemoDurb
{
    class gamePlayer
    {
        public int kills;
        public int deaths;
        public int totalPoints;
        public Texture2D gamerIcon;
        public string gamer;
        ContentManager content;

        //IAsyncResult result = null;
        bool GameLoadRequested = true;
        StorageDevice device;


        public gamePlayer(string gamer,Texture2D icon, ContentManager content,StorageDevice device)
        {
            this.kills = 0;
            this.deaths = 0;
            this.totalPoints = 0;
            this.gamer = gamer;
            this.content = content;
            gamerIcon = icon;

            this.device = device;

            join();

        }
        public void join()
        {

            /*
                if ((result != null) && result.IsCompleted)
                {
                    //device = Guide.EndShowStorageDeviceSelector(result);
                    result = null;
                    if ((device == null) || !device.IsConnected)
                    {
                        // Reset the request flag
                        GameLoadRequested = false;
                    }
                }
             */
                if ((device != null) && device.IsConnected)
                {
                    // Load game data
                    loadPlayer();
                    // Reset the request flag
                    GameLoadRequested = false;
                }
        }
        public void loadPlayer()
        {

            using(StorageContainer container = device.OpenContainer("DemoDurb"))
            {
                // Add the container path to our filename
                string filename = Path.Combine(container.Path, gamer + ".bin");

                // There is no save file so exit
                if (!File.Exists(filename))
                    return;

                // Open save file and read high score
                using (FileStream saveGameFile = new FileStream(filename,
                    FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(saveGameFile))
                    {
                        kills = reader.ReadInt32();
                        deaths = reader.ReadInt32();
                        totalPoints = reader.ReadInt32();
                    }
                }
            }

        }
        public void savePlayer()
        {

            using (StorageContainer container = device.OpenContainer("DemoDurb"))
            {
                // Add the container path to our filename
                string filename = Path.Combine(container.Path, gamer + ".bin");

                // There is no save file so exit

                // Open save file and read high score
                using (FileStream saveGameFile = new FileStream(filename,
                    FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(saveGameFile))
                    {
                        writer.Write(kills);
                        writer.Write(deaths);
                        writer.Write(totalPoints);

                        writer.Close();
                    }
                    
                }
            }

        }
        public static bool isConnected(SignedInGamer gamer)
        {
            GamePadState gs = GamePad.GetState(gamer.PlayerIndex);
            return gs.IsConnected;
        }
        public static void openGamerSignin()
        {
            for (int i = 0; i != 4; i++)
            {
                GamePadState gs = GamePad.GetState((PlayerIndex)i);

                if (gs.IsConnected)//checks if the game pad is connectex
                {
                    if (!isConnected((PlayerIndex)i))//then checks to see if there
                                                     //is a signed in gamer associated
                                                     //with that gamepad
                    {
                        if(!Guide.IsVisible)
                            Guide.ShowSignIn(4, false);
                    }
                }
            }
        }
        public static bool isConnected(PlayerIndex index)
        {
            foreach(SignedInGamer g in SignedInGamer.SignedInGamers)
                if (g.PlayerIndex == index)
                {
                    GamePadState gs = GamePad.GetState(index);
                    return gs.IsConnected;
                }

            return false;
        }
        public static PlayerIndex findConnected()
        {
            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                GamePadState gs = GamePad.GetState(gamer.PlayerIndex);

                if (gs.IsConnected)
                {
                    return gamer.PlayerIndex;
                }

            }

            return PlayerIndex.One;
        }
        public static SignedInGamer findGamer(PlayerIndex index)
        {
            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                if (isConnected(gamer))
                {
                    if (gamer.PlayerIndex == index)
                        return gamer;
                }
            }
            return null;
        }
        public static SignedInGamer SgfindConnected()
        {
            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                GamePadState gs = GamePad.GetState(gamer.PlayerIndex);

#if XBOX360
                if (gs.IsConnected)
                {
                    return gamer;
                }
#else
                return gamer;
#endif

            }

            return null;
        }
        public static int numConnected()
        {
            int count = 0;

            foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
            {
                GamePadState gs = GamePad.GetState(gamer.PlayerIndex);

#if XBOX360
                if (gs.IsConnected)
                {
                    count ++;
                }
#else
                count++;
#endif


            }

            return count;
        }
    }

    }

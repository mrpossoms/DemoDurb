using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace DemoDurb
{
    public abstract class GameMode
    {
        protected string title { get; set; }

        public string Title { get { return title; } }

        public bool b_conditionsGreen = true;

        public virtual void playerJoin(Gamer gamer) { }

        public virtual void conditionsMet() { }

        public virtual void initializeMatch(bool multiplayer) { }

        public virtual void MPreset() { }

        public virtual void updateCars(car[] cars, float speed, ref car thisCar) { }

        public virtual void updateDrivers(AnimSprite[] otherGuys, ref animGuy thisGuy) { }

        public virtual void updateGame(GameTime gt) { }

        public virtual void recieveData(PacketReader pr, NetworkGamer sender) { }

        public virtual void sendData(PacketWriter pw, LocalNetworkGamer gamer) { }

        public virtual void openHelp(PlayerIndex p) { }

        public bool ConditionsGreen()
        {
            return b_conditionsGreen;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Script
{
    class Room
    {
        static Room ins = null;
        static public Room GetIns()
        {
            if (ins == null)
                ins = new Room();

            return ins;
        }
        
        List<Player> mPlayerList = new List<Player>();

        public bool JoinPlayer(int guid)
        {
            Player player = GetPlayer(guid);
            if (player != null)
                return false;

            player = new Player(guid);

            if (mPlayerList.Count >= 2)
                return false;

            mPlayerList.Add(player);

            return true;
        }

        public void RemovePlayer(Player player)
        {
            if (mPlayerList.Contains(player))
                mPlayerList.Remove(player);
        }

        public Player GetPlayer(int nGuid)
        {
            for(int i = 0; i < mPlayerList.Count; ++i)
            {
                if (mPlayerList[i].Guid == nGuid)
                    return mPlayerList[i];
            }

            return null;
        }
    }
}

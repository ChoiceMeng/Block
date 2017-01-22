using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1010Server.Script.CsScript.GamePlay
{
    class Room
    {
        static Room ins = null;
        static Room GetIns()
        {
            if (ins == null)
                ins = new Room();

            return ins;
        }
        
        List<Player> mPlayerList = new List<Player>();

        bool JoinPlayer(Player player)
        {
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
    }
}

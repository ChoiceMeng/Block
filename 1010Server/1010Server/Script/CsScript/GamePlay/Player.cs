using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Script
{
    class Player
    {
        public int Guid
        {
            get;
            set;
        }

        public Player(int nGuid)
        {
            Guid = nGuid;
        }
    }
}

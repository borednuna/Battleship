using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Interfaces
{
    interface IPlayer
    {
        public string GetName();
        public void SetName(string name);
    }
}

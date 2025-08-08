using Battleship.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Interfaces
{
    public interface IPlayer
    {
        public string GetName();
        public void SetName(string name);
        public PlayerType GetPlayerType();
        public void SetPlayerType(PlayerType type);
    }
}

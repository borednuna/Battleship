using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Enums;
using Battleship.Interfaces;

namespace Battleship.Classes
{
    public class Player(string name, PlayerType type = PlayerType.HUMAN) : IPlayer
    {
        private string _name = name;
        private PlayerType _type = type;

        public string GetName()
        {
            return _name;
        }
        public void SetName(string name)
        {
            _name = name;
        }
        public PlayerType GetPlayerType()
        {
            return _type;
        }
        public void SetPlayerType(PlayerType type)
        {
            _type = type;
        }
    }
}

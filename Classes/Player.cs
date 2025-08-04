using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Interfaces;

namespace Battleship.Classes
{
    public class Player(string name) : IPlayer
    {
        private string _name = name;
        private bool _isBot = false;

        public string GetName()
        {
            return _name;
        }
        public void SetName(string name)
        {
            _name = name;
        }
        public bool IsBot()
        {
            return _isBot;
        }
        public void SetIsBot(bool isBot)
        {
            _isBot = isBot;
        }
    }
}

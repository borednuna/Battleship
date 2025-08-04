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
        public bool IsBot();
        public void SetIsBot(bool isBot);
    }
}

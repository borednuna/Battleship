using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Enums;

namespace Battleship.Interfaces
{
    interface IShip
    {
        public ShipType GetType();
        public string GetName();
        public void SetName(string name);
        public int GetSize();
        public void SetSize(int size);
    }
}

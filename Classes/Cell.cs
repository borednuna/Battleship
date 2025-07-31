using Battleship.Interfaces;
using Battleship.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Classes
{
    public class Cell
    {
        Coordinate _position;
        bool _isHit;
        Ship? _ship;

        public Coordinate GetPosition()
        {
            return _position;
        }

        public Ship? GetShip()
        {
            return _ship;
        }

        public void SetShip(Ship ship)
        {
            _ship = ship;
        }
    }
}

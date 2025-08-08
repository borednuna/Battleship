using Battleship.Interfaces;
using Battleship.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Classes
{
    public class Cell (Coordinate position)
    {
        Coordinate _position = position;
        bool _isHit = false;
        Ship? _ship = null;

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

        public void setIsHit(bool isHit)
        {
            _isHit = isHit;
        }

        public bool IsHit()
        {
            return _isHit;
        }
    }
}

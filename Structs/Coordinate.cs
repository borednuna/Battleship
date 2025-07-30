using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Structs
{
    public record struct Coordinate
    {
        private int _x;
        private int _y;

        public override string ToString()
        {
            return $"({_x}, {_y})";
        }

        public int GetX()
        {
            return _x;
        }

        public void SetX(int x)
        {
            _x = x;
        }

        public int GetY()
        {
            return _y;
        }

        public void SetY(int y)
        {
            _y = y;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Interfaces;
using Battleship.Structs;
using Battleship.Enums;

namespace Battleship.Classes
{
    class Ship(string name, int size, ShipType type) : IShip
    {
        string _name = name;
        int _size = size;
        int _hits = 0;
        ShipType _type = type;
        List<Coordinate> _coordinates = new List<Coordinate>();

        public void SetName(string name)
        {
            _name = name;
        }

        public new ShipType GetType()
        {
            return _type;
        }

        public string GetName()
        {
            return _name;
        }

        public int GetSize()
        {
            return _size;
        }

        public void SetSize(int size)
        {
            _size = size;
        }
    }
}

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
    public class Ship(ShipType type) : IShip
    {
        string _name = type.ToString();
        int _size = ShipSizes[type];
        int _hits = 0;
        List<Coordinate> _coordinates = [];
        public static readonly Dictionary<ShipType, int> ShipSizes = new()
        {
            { ShipType.CARRIER, 5 },
            { ShipType.BATTLESHIP, 4 },
            { ShipType.CRUISER, 3 },
            { ShipType.SUBMARINE, 3 },
            { ShipType.DESTROYER, 2 }
        };

        public void SetName(string name)
        {
            _name = name;
        }

        public new ShipType GetType()
        {
            return type;
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

        public List<Coordinate> GetPositions()
        {
            return _coordinates;
        }

        public void SetPositions(List<Coordinate> coordinates)
        {
            _coordinates = coordinates;
        }
    }
}

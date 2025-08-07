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
        readonly ShipType _type = type;
        readonly int _size = GetShipSize(type);
        int _hits = 0;
        bool _isPlaced = false;
        List<Coordinate> _coordinates = [];

        private static readonly Dictionary<ShipType, int> ShipSizes = new()
        {
            { ShipType.CARRIER, 5 },
            { ShipType.BATTLESHIP, 4 },
            { ShipType.CRUISER, 3 },
            { ShipType.SUBMARINE, 3 },
            { ShipType.DESTROYER, 2 }
        };

        private static int GetShipSize(ShipType type)
        {
            return ShipSizes.TryGetValue(type, out int size) ? size : 0;
        }

        public ShipType GetShipType()
        {
            return _type;
        }

        public int GetSize()
        {
            return _size;
        }

        public List<Coordinate> GetPositions()
        {
            return _coordinates;
        }

        public void SetPositions(List<Coordinate> coordinates)
        {
            _coordinates = coordinates;
        }

        public int GetHits()
        {
            return _hits;
        }

        public void SetHits(int hits)
        {
            _hits = hits;
        }

        public bool GetIsPlaced()
        {
            return _isPlaced;
        }

        public void SetIsPlaced(bool isPlaced)
        {
            _isPlaced = isPlaced;
        }
    }
}

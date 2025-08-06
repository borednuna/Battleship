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
        int _size = (int)type;
        int _hits = 0;
        bool _isPlaced = false;
        List<Coordinate> _coordinates = [];

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

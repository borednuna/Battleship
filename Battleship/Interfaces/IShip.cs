using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Enums;
using Battleship.Structs;

namespace Battleship.Interfaces
{
    public interface IShip
    {
        public ShipType GetShipType();
        public int GetSize();
        public List<Coordinate> GetPositions();
        public void SetPositions(List<Coordinate> positions);
        public int GetHits();
        public void SetHits(int hits);
        public bool GetIsPlaced();
        public void SetIsPlaced(bool isPlaced);
    }
}

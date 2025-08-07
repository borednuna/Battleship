using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Classes;
using Battleship.Enums;
using Battleship.Structs;

namespace Battleship.Interfaces
{
    public interface IBoard
    {
        public Cell GetBoard(Coordinate coordinate);
        public BoardType GetBoardType();
        public Dictionary<Coordinate, IShip> GetAllShipCoordinates();
    }
}

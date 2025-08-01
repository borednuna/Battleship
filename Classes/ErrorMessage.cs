using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Classes
{
    public static class ErrorMessage
    {
        public static readonly string BOUNDARY_ERROR = "Ships cannot be placed outside the board boundaries.";
        public static readonly string SHIP_PLACEMENT_ERROR = "Ship cannot be placed here. Please choose a different location.";
        public static readonly string SHIP_NOT_SELECTED_ERROR = "Please select a ship type first.";
        public static readonly string SHIP_NOT_FOUND_ERROR = "Ship not found in the fleet.";
    }
}

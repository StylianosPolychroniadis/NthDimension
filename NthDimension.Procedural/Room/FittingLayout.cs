using System.Collections.Generic;

namespace NthDimension.Procedural.Room
{
	public class FittingLayout
	{
        // Data members

		public Room Room { get; private set; }
        public List<Fitting> PlacedFittings { get; set; } = new List<Fitting>();


        // Constructor

        public FittingLayout(Room room, List<Fitting> placedFittings)
        {
            Room = room;
            PlacedFittings = placedFittings;
        }
    }
}

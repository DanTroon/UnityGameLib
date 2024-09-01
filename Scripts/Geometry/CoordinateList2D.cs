using System;
using System.Collections.Generic;

namespace UnityGameLib.Geometry {
	/// <summary>
	/// A list of <see cref="Coordinates2D"/> that displays as a grid in the inspector.
	/// </summary>
	/// <remarks>
	/// This entity contains several metadata fields to assist layout in the inspector.
	/// The constraints specified by these fields are not enforced by CoordinateList2D,
	/// 
	/// </remarks>
	[Serializable]
	public class CoordinateList2D {
		/// <summary> The width of the grid containing the coordinates. This limit is not enforced internally. </summary>
		public int cellCountX = 0;
		/// <summary> The height of the grid containing the coordinates. This limit is not enforced internally. </summary>
		public int cellCountY = 0;
		/// <summary> The minimum X coordinate in the grid. This limit is not enforced internally. </summary>
		public int offsetX = 0;
		/// <summary> The minimum Y coordinate in the grid. This limit is not enforced internally. </summary>
		public int offsetY = 0;
		/// <summary> If true, indicates that increasing X coordinates should display from right to left. </summary>
		public bool invertX = false;
		/// <summary> If true, indicates that increasing Y coordinates should display from bottom to top. </summary>
		public bool invertY = false;
		/// <summary> The list of active coordinate pairs. </summary>
		public List<Coordinates2D> activeCells = new List<Coordinates2D>();

		/// <summary>
		/// Creates a CoordinateList2D with no dimensions or offsets.
		/// </summary>
		public CoordinateList2D() : this(0, 0, 0, 0) { }

		/// <summary>
		/// Creates a CoordinateList2D with preset dimensions and contents.
		/// </summary>
		/// <param name="cellCountX">the width of the table</param>
		/// <param name="cellCountY"></param>
		/// <param name="cells"></param>
		public CoordinateList2D(int cellCountX, int cellCountY, params Coordinates2D[] cells) : this(cellCountX, cellCountY, 0, 0, cells) { }
		public CoordinateList2D(int cellCountX, int cellCountY, int offsetX, int offsetY, params Coordinates2D[] cells) {
			this.cellCountX = cellCountX;
			this.cellCountY = cellCountY;

			for (int i = 0, count = cells.Length; i < count; ++i) {
				activeCells.Add(cells[i]);
			}
		}

		/// <summary>
		/// Determines whether this list contains the given coordinates.
		/// </summary>
		/// <param name="coordinates">a Coordinates2D containing the XY coordinates to search</param>
		/// <returns><c>true</c> if a coordinate pair matching <paramref name="coordinates"/> exists, or <c>false</c> if not</returns>
		public bool Contains(Coordinates2D coordinates) {
			return Contains(coordinates.x, coordinates.y);
		}

		/// <summary>
		/// Determines whether this list contains the given coordinates.
		/// </summary>
		/// <param name="x">the X coordinate</param>
		/// <param name="y">the Y coordinate</param>
		/// <returns><c>true</c> if a coordinate pair matching <paramref name="x"/> and <paramref name="y"/> exists, or <c>false</c> if not</returns>
		public bool Contains(int x, int y) {
			for (int i = 0, count = activeCells.Count; i < count; ++i) {
				if (activeCells[i].x == x && activeCells[i].y == y) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether the given coordinates are within the boundaries set by
		/// <see cref="cellCountX"/>, <see cref="cellCountY"/>, <see cref="offsetX"/>, and <see cref="offsetY"/>.
		/// </summary>
		/// <param name="coordinates">a coordinates2D containing the XY coordinates to test</param>
		/// <returns><c>true</c> if the coordinates are within the grid bounds, or <c>false</c> if not</returns>
		public bool IsInBounds(Coordinates2D coordinates) {
			return IsInBounds(coordinates.x, coordinates.y);
		}

		/// <summary>
		/// Determines whether the given coordinates are within the boundaries set by
		/// <see cref="cellCountX"/>, <see cref="cellCountY"/>, <see cref="offsetX"/>, and <see cref="offsetY"/>.
		/// </summary>
		/// <param name="x">the X coordinate</param>
		/// <param name="y">the Y coordinate</param>
		/// <returns><c>true</c> if the coordinates are within the grid bounds, or <c>false</c> if not</returns>
		public bool IsInBounds(int x, int y) {
			int localX = x - offsetX;
			int localY = y - offsetY;
			return localX >= 0 && localX < cellCountX && localY >= 0 && localY < cellCountY;
		}

		/// <summary>
		/// The number of coordinate pairs in this list.
		/// </summary>
		public int Count {
			get { return activeCells.Count; }
		}

		/// <summary>
		/// Get or set coordinate pairs directly by index.
		/// </summary>
		/// <param name="index">the index of the desired coordinates</param>
		/// <returns>the Coordinates2D at <paramref name="index"/></returns>
		public Coordinates2D this[int index] {
			get { return activeCells[index]; }
			set { activeCells[index] = value; }
		}
	}
}

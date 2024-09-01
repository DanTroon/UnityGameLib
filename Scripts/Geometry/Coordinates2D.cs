using System;

namespace UnityGameLib.Geometry {
	/// <summary>
	/// A pair of integer coordinates.
	/// </summary>
	[Serializable]
	public struct Coordinates2D : IEquatable<Coordinates2D> {
		/// <summary>The X coordinate</summary>
		public int x;
		/// <summary>The Y coordinate</summary>
		public int y;

		/// <summary>
		/// Creates a Coordinates2D with the specified X and Y values.
		/// </summary>
		/// <param name="x">The X coordinate</param>
		/// <param name="y">The Y coordinate</param>
		public Coordinates2D(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public static bool operator ==(Coordinates2D a, Coordinates2D b) {
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Coordinates2D a, Coordinates2D b) {
			return !(a == b);
		}

		public bool Equals(Coordinates2D other) {
			return this == other;
		}

		public override bool Equals(object obj) {
			return (obj is Coordinates2D) && Equals((Coordinates2D) obj);
		}

		public override int GetHashCode() {
			return x ^ y;
		}
	}
}

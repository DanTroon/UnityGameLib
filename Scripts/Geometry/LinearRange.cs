namespace UnityGameLib.Geometry {
	/// <summary>
	/// Defines a linear range of floating point values given two endpoints.
	/// </summary>
	[System.Serializable]
	public struct LinearRange {
		/// <summary>The first endpoint of the range.</summary>
		public float start;
		/// <summary>The second endpoint of the range.</summary>
		public float end;

		/// <summary>The lower of the two endpoints.</summary>
		/// <remarks>Setting <see cref="min"/> will modify whichever endpoint is lower, or <see cref="start"/> if equal.</remarks>
		public float min {
			get { return start > end ? end : start; }
			set {
				if (start > end) {
					end = value;
				} else {
					start = value;
				}
			}
		}

		/// <summary>The higher of the two endpoints.</summary>
		/// <remarks>Setting <see cref="max"/> will modify whichever endpoint is higher, or <see cref="end"/> if equal.</remarks>
		public float max {
			get { return start < end ? end : start; }
			set {
				if (start < end) {
					end = value;
				} else {
					start = value;
				}
			}
		}

		/// <summary>The distance between endpoints.</summary>
		/// <remarks>Setting <see cref="length"/> will modify <see cref="max"/> accordingly.</remarks>
		public float length {
			get { return max - min; }
			set { max = min + value; }
		}

		/// <summary>Whether the endpoints are inverted (<see cref="end"/> is less than <see cref="start"/>).</summary>
		/// <remarks>Toggling <see cref="inverted"/> will swap <see cref="start"/> and <see cref="end"/>.</remarks>
		public bool inverted {
			get { return start > end; }
			set {
				if (inverted != value) {
					float temp = start;
					start = end;
					end = temp;
				}
			}
		}

		/// <summary>
		/// Creates a LinearRange with zero length, using the same start and end.
		/// </summary>
		/// <param name="startAndEnd">the value for both endpoints</param>
		public LinearRange(float startAndEnd) {
			start = startAndEnd;
			end = startAndEnd;
		}

		/// <summary>
		/// Creates a LinearRange
		/// </summary>
		/// <param name="start">the value for one endpoint</param>
		/// <param name="end">the value for the other endpoint</param>
		public LinearRange(float start, float end) {
			this.start = start;
			this.end = end;
		}

		/// <summary>
		/// Determines whether a value is within this range, inclusively.
		/// </summary>
		/// <param name="value">the value to test</param>
		/// <returns><c>true</c> if <see cref="value"/> is within this range, or <c>false</c> otherwise</returns>
		public bool Contains(float value) {
			return min <= value && value <= max;
		}

		/// <summary>
		/// Determines whether a value is within this range, exclusively.
		/// </summary>
		/// <param name="value">the value to test</param>
		/// <returns><c>true</c> if <see cref="value"/> is within this range, or <c>false</c> otherwise</returns>
		public bool ContainsExclusive(float value) {
			return min < value && value < max;
		}

		/// <summary>
		/// Returns a random value within this range, inclusively.
		/// </summary>
		/// <returns></returns>
		public float GetRandom() {
			return min + length * UnityEngine.Random.value;
		}

		#region Operators
		public static LinearRange operator +(LinearRange a, LinearRange b) {
			return new LinearRange(a.start + b.start, a.end + b.end);
		}
		public static LinearRange operator -(LinearRange a, LinearRange b) {
			return new LinearRange(a.start - b.start, a.end - b.end);
		}
		public static LinearRange operator *(LinearRange a, LinearRange b) {
			return new LinearRange(a.start * b.start, a.end * b.end);
		}
		public static LinearRange operator /(LinearRange a, LinearRange b) {
			return new LinearRange(a.start / b.start, a.end / b.end);
		}

		public static LinearRange operator +(LinearRange range, float x) {
			return new LinearRange(range.start + x, range.end + x);
		}
		public static LinearRange operator -(LinearRange range, float x) {
			return new LinearRange(range.start - x, range.end - x);
		}
		public static LinearRange operator *(LinearRange range, float x) {
			return new LinearRange(range.start * x, range.end * x);
		}
		public static LinearRange operator /(LinearRange range, float x) {
			return new LinearRange(range.start / x, range.end / x);
		}
		#endregion
	}
}

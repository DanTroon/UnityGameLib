using UnityEngine;

namespace UnityGameLib.Geometry {
	/// <summary>
	/// A set of barycentric coordinates for a triangle.
	/// </summary>
	public struct BarycentricPoint {
		/// <summary>The coordinate associated with vertex A of the triangle.</summary>
		public float a;
		/// <summary>The coordinate associated with vertex B of the triangle.</summary>
		public float b;
		/// <summary>The coordinate associated with vertex C of the triangle.</summary>
		public float c;

		/// <summary>The coordinates <see cref="a"/>, <see cref="b"/>, and <see cref="c"/> as an array.</summary>
		/// <seealso cref="GetCoordinates(float[])"/>
		public float[] coordinates {
			get { return new float[] { a, b, c }; }
			set {
				a = value[0];
				b = value[1];
				c = value[2];
			}
		}

		/// <summary>Whether the point is inside the source triangle, including its edges.</summary>
		public bool inShape {
			get { return a >= 0f && b >= 0f && c >= 0f; }
		}

		/// <summary>Returns a copy of the point clamped within the source triangle.</summary>
		public BarycentricPoint clamped {
			get {
				BarycentricPoint result = new BarycentricPoint(a, b, c);
				result.Clamp();
				return result;
			}
		}

		/// <summary>
		/// Creates a BarycentricPoint with the given a, b, c coordinates.
		/// </summary>
		/// <param name="a">the a coordinate</param>
		/// <param name="b">the b coordinate</param>
		/// <param name="c">the c coordinate</param>
		public BarycentricPoint(float a, float b, float c) {
			this.a = a;
			this.b = b;
			this.c = c;
		}

		/// <summary>
		/// Creates a BarycentricPoint calculated from a cartesian point and a triangle's vertices.
		/// </summary>
		/// <remarks>
		/// When generating multiple barycentric coordinates with the same triangle, consider using
		/// a <see cref="BarycentricCache"/> to compute them more efficiently.
		/// </remarks>
		/// <param name="cartesianPoint">the cartesian (XY) coordinates of the point</param>
		/// <param name="vertexA">the first vertex of the triangle</param>
		/// <param name="vertexB">the second vertex of the triangle</param>
		/// <param name="vertexC">the third vertex of the triangle</param>
		public BarycentricPoint(Vector2 cartesianPoint, Vector2 vertexA, Vector2 vertexB, Vector2 vertexC) {
			Vector2 v0 = vertexB - vertexA;
			Vector2 v1 = vertexC - vertexA;
			Vector2 v2 = cartesianPoint - vertexA;
			float d00 = Vector2.Dot(v0, v0);
			float d01 = Vector2.Dot(v0, v1);
			float d11 = Vector2.Dot(v1, v1);
			float d20 = Vector2.Dot(v2, v0);
			float d21 = Vector2.Dot(v2, v1);
			float factor = 1f / (d00 * d11 - d01 * d01);

			b = (d11 * d20 - d01 * d21) * factor;
			c = (d00 * d21 - d01 * d20) * factor;
			a = 1f - b - c;
		}

		/// <summary>
		/// Creates a BarycentricPoint calculated from a cartesian point and a triangle.
		/// </summary>
		/// <remarks>
		/// When generating multiple barycentric coordinates with the same triangle, consider using
		/// a <see cref="BarycentricCache"/> to compute them more efficiently.
		/// </remarks>
		/// <param name="cartesianPoint">the cartesian (XY) coordinates of the point</param>
		/// <param name="triangle">the triangle</param>
		public BarycentricPoint(Vector2 cartesianPoint, Triangle2D triangle) : this(cartesianPoint, triangle.a, triangle.b, triangle.c) {

		}

		/// <summary>
		/// Sets the first three elements of an existing array to the <see cref="a"/>, <see cref="b"/>, and <see cref="c"/> coordinates.
		/// </summary>
		/// <remarks>
		/// This method throws an exception if the array has a length less than 3.
		/// </remarks>
		/// <param name="outCoordinates">the array to populate</param>
		public void GetCoordinates(float[] outCoordinates) {
			outCoordinates[0] = a;
			outCoordinates[1] = b;
			outCoordinates[2] = c;
		}

		/// <summary>
		/// Clamps the point within the source triangle.
		/// </summary>
		public void Clamp() {
			if (a >= 0f && b < 0f && c < 0f) {
				a = 1f;
				b = 0f;
				c = 0f;
			} else if (a < 0f && b >= 0f && c < 0f) {
				a = 0f;
				b = 1f;
				c = 0f;
			} else if (a < 0f && b < 0f && c >= 0f) {
				a = 0f;
				b = 0f;
				c = 1f;
			} else if (a < 0f) {
				float factor = 1 / (b + c);
				a = 0f;
				b *= factor;
				c *= factor;
			} else if (b < 0f) {
				float factor = 1 / (a + c);
				a *= factor;
				b = 0f;
				c *= factor;
			} else if (c < 0f) {
				float factor = 1 / (a + b);
				a *= factor;
				b *= factor;
				c = 0f;
			}
		}

		public override string ToString() {
			return string.Format("({0}, {1}, {2})", a.ToString("f3"), b.ToString("f3"), c.ToString("f3"));
		}

		public static implicit operator Vector3(BarycentricPoint value) {
			return new Vector3(value.a, value.b, value.c);
		}
		public static implicit operator BarycentricPoint(Vector3 value) {
			return new BarycentricPoint(value.x, value.y, value.z);
		}
	}
}

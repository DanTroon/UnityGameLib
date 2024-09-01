using UnityEngine;

namespace UnityGameLib.Geometry {
	/// <summary>
	/// A wrapper for a <see cref="Triangle2D"/> to optimize repeated barycentric calculations.
	/// </summary>
	public class BarycentricCache {
		private Triangle2D _triangle;

		/// <summary>
		/// The cached triangle. Set this property (not the vertices directly) to modify it.
		/// </summary>
		/// <seealso cref="SetTriangleVertices(Vector2, Vector2, Vector2)"/>
		public Triangle2D triangle {
			get { return _triangle; }
			set {
				_triangle = value;
				RefreshCache();
			}
		}

		/// <summary>The cached edge vector between <c>a</c> and <c>b</c>.</summary>
		public Vector2 v0 { get; private set; }
		/// <summary>The cached edge vector between <c>b</c> and <c>c</c>.</summary>
		public Vector2 v1 { get; private set; }
		/// <summary>The cached dot product of <see cref="v0"/> and <see cref="v0"/>.</summary>
		public float d00 { get; private set; }
		/// <summary>The cached dot product of <see cref="v0"/> and <see cref="v1"/>.</summary>
		public float d01 { get; private set; }
		/// <summary>The cached dot product of <see cref="v1"/> and <see cref="v1"/>.</summary>
		public float d11 { get; private set; }
		/// <summary>The cached inverse denominator of the barycentric conversion.</summary>
		public float factor { get; private set; }

		/// <summary>
		/// Constructs an empty BarycentricCache. Note that the triangle must be set before computing.
		/// </summary>
		/// <seealso cref="triangle"/>
		/// <seealso cref="SetTriangleVertices(Vector2, Vector2, Vector2)"/>
		public BarycentricCache() {
			
		}

		/// <summary>
		/// Constructs a BarycentricCache for a triangle.
		/// </summary>
		/// <param name="triangle">the triangle to cache</param>
		public BarycentricCache(Triangle2D triangle) {
			this.triangle = triangle;
		}

		/// <summary>
		/// Constructs a BarycentricCache for a triangle.
		/// </summary>
		/// <param name="a">the first vertex of the triangle</param>
		/// <param name="b">the second vertex of the triangle</param>
		/// <param name="c">the third vertex of the triangle</param>
		public BarycentricCache(Vector2 a, Vector2 b, Vector2 c) {
			SetTriangleVertices(a, b, c);
		}

		/// <summary>
		/// Set each point of the cached triangle and update all cached elements.
		/// </summary>
		/// <param name="a">the first vertex of the triangle</param>
		/// <param name="b">the second vertex of the triangle</param>
		/// <param name="c">the third vertex of the triangle</param>
		/// <seealso cref="triangle"/>
		public void SetTriangleVertices(Vector2 a, Vector2 b, Vector2 c) {
			_triangle.a = a;
			_triangle.b = b;
			_triangle.c = c;
			RefreshCache();
		}

		/// <summary>
		/// Returns a BarycentricPoint containing <see cref="triangle"/>'s barycentric coordinates for cartesian point <paramref name="p"/>.
		/// </summary>
		/// <param name="p">the cartesian (XY) coordinates to convert to barycentric</param>
		/// <returns>the computed BarycentricPoint</returns>
		public BarycentricPoint Compute(Vector2 p) {
			Vector2 v2 = p - _triangle.a;
			float d20 = Vector2.Dot(v2, v0);
			float d21 = Vector2.Dot(v2, v1);

			float b = (d11 * d20 - d01 * d21) * factor;
			float c = (d00 * d21 - d01 * d20) * factor;
			return new BarycentricPoint(1f - b - c, b, c);
		}

		/// <summary>
		/// Returns the cartesian coordinates for barycentric point <paramref name="p"/> in <see cref="triangle"/>.
		/// </summary>
		/// <param name="p">the barycentric coordinates to convert to cartesian</param>
		/// <returns>the computed cartesian point as a Vector2</returns>
		public Vector2 InverseCompute(BarycentricPoint p) {
			return p.a * _triangle.a + p.b * _triangle.b + p.c * _triangle.c;
		}

		private void RefreshCache() {
			v0 = _triangle.b - _triangle.a;
			v1 = _triangle.c - _triangle.a;
			d00 = Vector2.Dot(v0, v0);
			d01 = Vector2.Dot(v0, v1);
			d11 = Vector2.Dot(v1, v1);
			factor = 1f / (d00 * d11 - d01 * d01);
		}
	}
}

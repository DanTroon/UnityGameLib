using UnityEngine;

namespace UnityGameLib.Geometry {
	/// <summary>
	/// A set of three 2D points that define a 2D triangle.
	/// </summary>
	public struct Triangle2D {
		/// <summary>The first vertex of this triangle.</summary>
		public Vector2 a;
		/// <summary>The second vertex of this triangle.</summary>
		public Vector2 b;
		/// <summary>The third vertex of this triangle.</summary>
		public Vector2 c;

		/// <summary>Returns the vertices of this triangle as a newly allocated array.</summary>
		/// <seealso cref="GetVertices(Vector2[])"/>
		public Vector2[] vertices {
			get { return new Vector2[] { a, b, c }; }
		}

		/// <summary>Returns the vertices of this triangle as a new array of Vector3.</summary>
		public Vector3[] vertices3D {
			get { return new Vector3[] { a, b, c }; }
		}

		/// <summary>Determines whether this triangles vertices appear in clockwise order.</summary>
		/// <remarks>
		/// If the triangle has no area, e.g. because all three vertices exist on the same line, it
		/// is neither clockwise nor counter-clockwise. This property should return <c>false</c> in that
		/// case, but the result may differ due to floating point error.
		/// </remarks>
		public bool isClockwise {
			get {
				return areaSigned > 0f;
			}
		}

		/// <summary>Calculates the area, allowing it to be negative if this triangle is counter-clockwise.</summary>
		public float areaSigned {
			get {
				return .5f * ((b.x - a.x) * (b.y + a.y) + (c.x - b.x) * (c.y + b.y) + (a.x - c.x) * (a.y + c.y));
			}
		}

		/// <summary>Calculates the area of this triangle.</summary>
		public float area {
			get {
				return Mathf.Abs(areaSigned);
			}
		}

		/// <summary>
		/// Constructs a new Triangle2D with the given vertices.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public Triangle2D(Vector2 a, Vector2 b, Vector2 c) {
			this.a = a;
			this.b = b;
			this.c = c;
		}

		/// <summary>
		/// Assigns the vertices of this triangle to the first 3 indices of a preallocated array. 
		/// </summary>
		/// <remarks>
		/// This method will throw an exception if the array has a length less than 3.
		/// </remarks>
		/// <param name="verticesOut"></param>
		public void GetVertices(Vector2[] verticesOut) {
			verticesOut[0] = a;
			verticesOut[1] = b;
			verticesOut[2] = c;
		}
	}
}

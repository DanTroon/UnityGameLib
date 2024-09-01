using UnityGameLib.Geometry;
using System;
using UnityEngine;

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods for math operations.
	/// </summary>
	public static class MathUtilities {
		#region 2D to 3D Conversions
		/// <summary>
		/// Converts a 3D point into 2D space along the XZ axes.
		/// </summary>
		/// <param name="source">The 3D point as a Vector3</param>
		/// <returns>The 2D point as a Vector2</returns>
		public static Vector2 ConvertTo2D(Vector3 source) {
			return new Vector2(source.x, source.z);
		}

		/// <summary>
		/// Converts a 2D point along the XZ axes into 3D space.
		/// </summary>
		/// <param name="source">The 2D point as a Vector2</param>
		/// <returns>The 3D point as a Vector3</returns>
		public static Vector3 ConvertTo3D(Vector2 source) {
			return ConvertTo3D(source.x, source.y);
		}
		/// <summary>
		/// Converts a 2D point along the XZ axes into 3D space.
		/// </summary>
		/// <param name="source">The 2D point as a Coordinates2D</param>
		/// <returns>The 3D point as a Vector3</returns>
		public static Vector3 ConvertTo3D(Coordinates2D source) {
			return ConvertTo3D(source.x, source.y);
		}
		/// <summary>
		/// Converts a 2D point along the XZ axes into 3D space.
		/// </summary>
		/// <param name="x">The X coordinate in 2D space, which maps to the X axis in 3D space</param>
		/// <param name="y">The Y coordinate in 2D space, which maps to the Z axis in 3D space</param>
		/// <returns>The 3D point as a Vector3</returns>
		public static Vector3 ConvertTo3D(float x, float y) {
			return new Vector3(x, 0f, y);
		}
		/// <summary>
		/// Converts a 2D point along the XZ axes into 3D space.
		/// </summary>
		/// <param name="x">The X coordinate in 2D space, which maps to the X axis in 3D space</param>
		/// <param name="y">The Y coordinate in 2D space, which maps to the Z axis in 3D space</param>
		/// <returns>The 3D point as a Vector3</returns>
		public static Vector3 ConvertTo3D(int x, int y) {
			return new Vector3(x, 0f, y);
		}
		#endregion


		/// <summary>
		/// Casts a point from screen space onto a 3D plane in the camera view.
		/// </summary>
		/// <param name="screenPos">The point in screen space</param>
		/// <param name="plane">The target plane in world space</param>
		/// <param name="camera">An optional camera to use instead of the main camera</param>
		/// <returns>The point on the plane, or (0,0,0) if the point cannot be cast</returns>
		public static Vector3 ScreenToPlanePoint(Vector3 screenPos, Plane plane, Camera camera = null) {
			if (!camera)
				camera = Camera.main;

			Ray ray = camera.ScreenPointToRay(screenPos);
			float hitDistance;

			if (plane.Raycast(ray, out hitDistance)) {
				return ray.GetPoint(hitDistance);
			} else {
				return Vector3.zero;
			}
		}


		#region Interpolation and Curves
		/// <summary>
		/// Returns a sine-eased interpolant such that it accelerates at the start and decelerates at the end.
		/// </summary>
		/// <param name="interpolant">The linear interpolant</param>
		/// <returns>The eased interpolant</returns>
		public static float EaseInterpolant(float interpolant) {
			return .5f - .5f * Mathf.Cos((float) Math.PI * interpolant);
		}

		/// <summary>
		/// Returns a sine-eased interpolant such that it starts abruptly and decelerates at the end.
		/// </summary>
		/// <param name="interpolant">The linear interpolant</param>
		/// <returns>The eased interpolant</returns>
		public static float EaseOutInterpolant(float interpolant) {
			return Mathf.Cos((float) Math.PI * (interpolant * .5f - .5f));
		}

		/// <summary>
		/// Returns a sine-eased interpolant such that it accelerates at the start and stops abruptly.
		/// </summary>
		/// <param name="interpolant">The linear interpolant</param>
		/// <returns>The eased interpolant</returns>
		public static float EaseInInterpolant(float interpolant) {
			return 1f - Mathf.Cos((float) Math.PI * (interpolant * .5f));
		}

		/// <summary>
		/// Returns a point along a 1D Bezier curve.
		/// </summary>
		/// <param name="interpolant">The normalized position along the curve (typically between 0 and 1)</param>
		/// <param name="start">The starting point for the curve</param>
		/// <param name="anchors">The anchor points that define the curve, in order, including the ending point</param>
		/// <returns>The interpolated point along the curve</returns>
		public static float Bezier(float interpolant, float start, params float[] anchors) {
			int count = anchors.Length;

			float result = start;
			for (int i = 0; i < count; ++i) {
				result = Mathf.LerpUnclamped(result, anchors[i], interpolant);
			}

			return result;
		}

		/// <summary>
		/// Returns a point along a 2D Bezier curve.
		/// </summary>
		/// <param name="interpolant">The normalized position along the curve (typically between 0 and 1)</param>
		/// <param name="start">The starting point for the curve</param>
		/// <param name="anchors">The anchor points that define the curve, in order, including the ending point</param>
		/// <returns>The interpolated point along the curve</returns>
		public static Vector2 Bezier2D(float interpolant, Vector2 start, params Vector2[] anchors) {
			int count = anchors.Length;

			Vector2 result = start;
			for (int i = 0; i < count; ++i) {
				result = Vector2.LerpUnclamped(result, anchors[i], interpolant);
			}

			return result;
		}

		/// <summary>
		/// Returns a point along a 3D Bezier curve.
		/// </summary>
		/// <param name="interpolant">The normalized position along the curve (typically between 0 and 1)</param>
		/// <param name="start">The starting point for the curve</param>
		/// <param name="anchors">The anchor points that define the curve, in order, including the ending point</param>
		/// <returns>The interpolated point along the curve</returns>
		public static Vector3 Bezier3D(float interpolant, Vector3 start, params Vector3[] anchors) {
			int count = anchors.Length;

			Vector3 result = start;
			for (int i = 0; i < count; ++i) {
				result = Vector3.LerpUnclamped(result, anchors[i], interpolant);
			}

			return result;
		}
		#endregion
		

		#region Wrapping Values
		/// <summary>
		/// Cyclically wraps <paramref name="value"/> so it remains between <paramref name="min"/> (inclusive) and <paramref name="max"/> (exclusive).
		/// </summary>
		/// <param name="value">The value to wrap.</param>
		/// <param name="min">The inclusive lower bound of the range.</param>
		/// <param name="max">The exclusive upper bound of the range.</param>
		/// <returns>The value wrapped between between <paramref name="min"/> and <paramref name="max"/></returns>
		public static float WrapValue(float value, float min, float max) {
			float diff = max - min;
			while (value < min)
				value += diff;
			while (value >= max)
				value -= diff;
			return value;
		}

		/// <summary>
		/// Cyclically wraps <paramref name="value"/> so it remains between <paramref name="min"/> (inclusive) and <paramref name="max"/> (exclusive).
		/// </summary>
		/// <param name="value">The value to wrap.</param>
		/// <param name="min">The inclusive lower bound of the range.</param>
		/// <param name="max">The exclusive upper bound of the range.</param>
		/// <returns>The value wrapped between between <paramref name="min"/> and <paramref name="max"/></returns>
		public static int WrapValue(int value, int min, int max) {
			int diff = max - min;
			while (value < min)
				value += diff;
			while (value >= max)
				value -= diff;
			return value;
		}

		/// <summary>
		/// Cyclically wraps <paramref name="value"/> so it remains between 0 (inclusive) and 1 (exclusive).
		/// </summary>
		/// <param name="value">The value to wrap.</param>
		/// <returns>The value wrapped between 0 and 1</returns>
		public static float WrapValue01(float value) {
			return WrapValue(value, 0f, 1f);
		}

		/// <summary>
		/// Check whether <paramref name="value"/> is within given inclusive bounds, assuming both are wrapped within another range.
		/// </summary>
		/// <remarks>
		/// For example, determine whether a given angle is between two other angles.
		/// </remarks>
		/// <param name="value">The value to test.</param>
		/// <param name="rangeMin">The inclusive lower bound of the target range.</param>
		/// <param name="rangeMax">The inclusive upper bound of the target range.</param>
		/// <param name="wrapMin">The inclusive lower bound of the wrapping range.</param>
		/// <param name="wrapMax">The exclusive upper bound of the wrapping range.</param>
		/// <returns><c>true</c> if <paramref name="value"/> is within the target range when wrapped</returns>
		public static bool IsInWrappedRange(float value, float rangeMin, float rangeMax, float wrapMin, float wrapMax) {
			float wrappedRangeMin = WrapValue(rangeMin, wrapMin, wrapMax);
			float wrappedRangeMax = WrapValue(rangeMax, wrapMin, wrapMax);
			float wrappedValue = WrapValue(value, wrapMin, wrapMax);

			if (wrappedRangeMin > wrappedRangeMax) {
				return wrappedRangeMin <= wrappedValue || wrappedValue <= wrappedRangeMax;
			}

			return wrappedRangeMin <= wrappedValue && wrappedValue <= wrappedRangeMax;
		}

		/// <summary>
		/// Check whether <paramref name="value"/> is within given exclusive bounds, assuming both are wrapped within another range.
		/// </summary>
		/// <remarks>
		/// For example, determine whether a given angle is between two other angles.
		/// </remarks>
		/// <param name="value">The value to test.</param>
		/// <param name="rangeMin">The exclusive lower bound of the target range.</param>
		/// <param name="rangeMax">The exclusive upper bound of the target range.</param>
		/// <param name="wrapMin">The inclusive lower bound of the wrapping range.</param>
		/// <param name="wrapMax">The exclusive upper bound of the wrapping range.</param>
		/// <returns></returns>
		public static bool IsInWrappedRangeExclusive(float value, float rangeMin, float rangeMax, float wrapMin, float wrapMax) {
			float wrappedRangeMin = WrapValue(rangeMin, wrapMin, wrapMax);
			float wrappedRangeMax = WrapValue(rangeMax, wrapMin, wrapMax);
			float wrappedValue = WrapValue(value, wrapMin, wrapMax);

			if (wrappedRangeMin > wrappedRangeMax) {
				return wrappedRangeMin < wrappedValue || wrappedValue < wrappedRangeMax;
			}

			return wrappedRangeMin < wrappedValue && wrappedValue < wrappedRangeMax;
		}
		#endregion


		/// <summary>
		/// Calculates the result of a polynomial function given 'x' and its coefficients in ascending order of exponents.
		/// </summary>
		/// <param name="x">The variable to associate with the exponents and coefficients.</param>
		/// <param name="coefficients">The coefficients for each power of 'x', starting at x^0 and ascending.</param>
		/// <returns>The result of the polynomial function calculated with the given values.</returns>
		public static float Polynomial(float x, params float[] coefficients) {
			float result = 0f;
			for (int i = 0, count = coefficients.Length; i < count; ++i) {
				result += coefficients[i] * Mathf.Pow(x, i);
			}
			return result;
		}

		/// <summary>
		/// Returns the barycentric coordinates of cartesian point <paramref name="p"/> in the triangle with vertices
		/// <paramref name="a"/>, <paramref name="b"/>, and <paramref name="c"/>.
		/// </summary>
		/// <param name="p">the cartesian point to convert to barycentric</param>
		/// <param name="a">the first vertex of the triangle</param>
		/// <param name="b">the second vertex of the triangle</param>
		/// <param name="c">the third vertex of the triangle</param>
		/// <returns>the barycentric coordinates for <paramref name="p"/> in the triangle</returns>
		public static Vector3 Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c) {
			Vector2 v0 = b - a;
			Vector2 v1 = c - a;
			Vector2 v2 = p - a;
			float d00 = Vector2.Dot(v0, v0);
			float d01 = Vector2.Dot(v0, v1);
			float d11 = Vector2.Dot(v1, v1);
			float d20 = Vector2.Dot(v2, v0);
			float d21 = Vector2.Dot(v2, v1);
			float factor = 1f / (d00 * d11 - d01 * d01);

			Vector3 result = new Vector3(0f, (d11 * d20 - d01 * d21) * factor, (d00 * d21 - d01 * d20) * factor);
			result.x = 1f - result.y - result.z;
			return result;
		}
	}
}

using System;

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods for handling bitwise math and conversions.
	/// </summary>
	public static class BitwiseUtilities {
		/// <summary>
		/// Converts a float to a 32-bit integer containing its binary representation.
		/// </summary>
		/// <param name="value">the value to convert</param>
		/// <returns>an Int32 containing the binary form of <paramref name="value"/></returns>
		public static int FloatToInt32(float value) {
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		/// <summary>
		/// Converts the binary representation of a float from a 32-bit integer into a realized float.
		/// </summary>
		/// <param name="value">the binary representation as a 32-bit integer</param>
		/// <returns>the float represented by <paramref name="value"/></returns>
		public static float FloatFromInt32(int value) {
			return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
		}
	}
}

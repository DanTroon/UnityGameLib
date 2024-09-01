using System;
using UnityEngine;

namespace UnityGameLib.Serialization {
	/// <summary>
	/// Houses utility and extension methods to assist serialization and deserialization.
	/// </summary>
	public static class SerialUtilities {
		#region Extension Methods
		/// <summary>
		/// Converts the color to an RGBA signed integer.
		/// </summary>
		/// <param name="value">the color</param>
		/// <returns>an Int32 representing the color</returns>
		public static int ToInt(this Color32 value) {
			return (value.r << 24) | (value.g << 16) | (value.b << 8) | value.a;
		}

		/// <summary>
		/// Converts the integer to an RGBA color.
		/// </summary>
		/// <param name="value">the RGBA integer</param>
		/// <returns>the color represented by <paramref name="value"/></returns>
		public static Color32 ToColor(this int value) {
			return new Color32(Convert.ToByte(value >> 24 & 0xFF), Convert.ToByte(value >> 16 & 0xFF), Convert.ToByte(value >> 8 & 0xFF), Convert.ToByte(value & 0xFF));
		}

		/// <summary>
		/// Converts the color to an RGBA unsigned integer.
		/// </summary>
		/// <param name="value">the color</param>
		/// <returns></returns>
		public static uint ToUint(this Color32 value) {
			return (uint) value.ToInt();
		}

		/// <summary>
		/// Converts the integer to an RGBA color.
		/// </summary>
		/// <param name="value">the RGBA integer</param>
		/// <returns>the color represented by <paramref name="value"/></returns>
		public static Color32 ToColor(this uint value) {
			return new Color32(Convert.ToByte(value >> 24 & 0xFF), Convert.ToByte(value >> 16 & 0xFF), Convert.ToByte(value >> 8 & 0xFF), Convert.ToByte(value & 0xFF));
		}
		#endregion
	}
}

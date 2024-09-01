using System;

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods for handling strings.
	/// </summary>
	public static class StringUtilities {
		/// <summary>
		/// Converts a number of seconds or minutes to two-segment display text for a clock. (0:00).
		/// </summary>
		/// <param name="seconds">The number of seconds or minutes.</param>
		/// <returns>A clock-like text representation of the number (0:00).</returns>
		public static string GetTimeText(int seconds) {
			TimeSpan span = new TimeSpan(0, 0, seconds);
			return span.Minutes + ":" + span.Seconds.ToString().PadLeft(2, '0');
		}
	}
}

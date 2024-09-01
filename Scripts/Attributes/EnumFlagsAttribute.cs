using UnityEngine;

namespace UnityGameLib.Attributes {
	/// <summary>
	/// Allows the selection of multiple Enum flags in the inspector.
	/// </summary>
	/// <remarks>
	/// Enums or integer constants using this attribute should evaluate as powers of two (1, 2, 4, 8, 16, etc.).
	/// </remarks>
	public class EnumFlagsAttribute : PropertyAttribute {
		/// <summary>The names of the available flags.</summary>
		public string[] flagNames;

		/// <summary>
		/// Create an EnumFlagsAttribute.
		/// </summary>
		/// <param name="flagNames">An optional list of names for the flags. If empty, the Enum's flag names are used.</param>
		public EnumFlagsAttribute(params string[] flagNames) {
			this.flagNames = flagNames;
		}
	}
}

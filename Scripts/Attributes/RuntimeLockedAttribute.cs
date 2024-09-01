using System;

namespace UnityGameLib.Attributes {
	/// <summary>
	/// Disables inspector input for a field while in play mode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class RuntimeLockedAttribute : FieldModifierAttribute {
		/// <summary>
		/// If inverted, this locks the field only when NOT in play mode.
		/// </summary>
		public bool invert = false;

		/// <summary>
		/// Disables inspector input for a field while in play mode.
		/// </summary>
		/// <param name="invert">If inverted, this locks the field only when NOT in play mode.</param>
		public RuntimeLockedAttribute(bool invert = false) {
			this.invert = invert;
		}
	}
}

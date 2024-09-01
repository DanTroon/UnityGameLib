using System;

namespace UnityGameLib.Attributes {
	/// <summary>
	/// Disables inspector input for a field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class LockedAttribute : FieldModifierAttribute {

	}
}

using System;

namespace UnityGameLib.Attributes {
	/// <summary>
	/// Marks a field as optional to indicate that it can safely be null or empty.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class OptionalAttribute : FieldModifierAttribute {

	}
}

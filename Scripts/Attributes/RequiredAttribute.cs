using System;

namespace UnityGameLib.Attributes {
	/// <summary>
	/// Marks a field as required and highlights it whenever its value is null or empty.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class RequiredAttribute : FieldModifierAttribute {

	}
}

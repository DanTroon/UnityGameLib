using System;
using UnityEngine;

namespace UnityGameLib.Attributes {
	/// <summary>
	/// The base class for various attributes that can be combined to modify the inspector's field display.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class FieldModifierAttribute : PropertyAttribute {
		public FieldModifierAttribute() {

		}
	}
}

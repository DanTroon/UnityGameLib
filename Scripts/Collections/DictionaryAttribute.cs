using System;
using UnityEngine;

namespace UnityGameLib.Collections {
	/// <summary>
	/// Identifies a field as a <see cref="UnityGameLib.Collections.SerializableDictionary{TKey, TValue}"/>
	/// that should use <see cref="UnityGameLib.Editor.Drawers.DictionaryEditor"/> to render its inspector.
	/// </summary>
	/// <remarks>
	/// This should always and only be used for field types that extend <see cref="UnityGameLib.Collections.SerializableDictionary{TKey, TValue}"/>.
	/// This attribute exists as a workaround for Unity's limitations in serializing generic types.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class DictionaryAttribute : PropertyAttribute {
		public DictionaryAttribute() : base() {

		}
	}
}

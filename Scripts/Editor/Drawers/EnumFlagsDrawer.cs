using UnityEngine;
using UnityEditor;
using UnityGameLib.Attributes;
using System;

namespace UnityGameLib.Editor.Drawers {
	/// <summary>
	/// Custom property drawer for <see cref="EnumFlagsAttribute"/>.
	/// </summary>
	[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
	public class EnumFlagsDrawer : PropertyDrawer {
		
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EnumFlagsAttribute attrib = attribute as EnumFlagsAttribute;
			Type propType = fieldInfo.FieldType;

			if (property.serializedObject.isEditingMultipleObjects) {
				EditorGUI.LabelField(position, label, new GUIContent("Cannot multi-edit flags"));
			} else if (attrib.flagNames.Length > 0) {
				property.intValue = EditorGUI.MaskField(position, label, property.intValue, attrib.flagNames);
			} else if (propType.IsEnum) {
				property.intValue = EditorGUI.MaskField(position, label, property.intValue, Enum.GetNames(propType));
			} else {
				EditorGUI.LabelField(position, label, new GUIContent("Flag names required"));
			}
		}
	}
}

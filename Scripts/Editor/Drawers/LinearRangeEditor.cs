using UnityEngine;
using UnityEditor;
using UnityGameLib.Geometry;

namespace UnityGameLib.Editor.Drawers {
	/// <summary>
	/// The PropertyDrawer for <see cref="UnityGameLib.Geometry.LinearRange"/>.
	/// </summary>
	[CustomPropertyDrawer(typeof(LinearRange), true)]
	public class LinearRangeEditor : PropertyDrawer {
		private const string PROPERTY_NAME_START = "start";
		private const string PROPERTY_NAME_END = "end";
		private const float BRACKET_CHAR_WIDTH = 10f;

		private static GUIContent[] labels = new GUIContent[] { GUIContent.none, GUIContent.none };

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty start = property.FindPropertyRelative(PROPERTY_NAME_START);

			Rect valueRect = EditorGUI.PrefixLabel(position, label);

			GUIStyle bracketStyle = new GUIStyle(GUI.skin.label);

			bracketStyle.alignment = TextAnchor.MiddleRight;
			EditorGUI.LabelField(new Rect(valueRect.x, valueRect.y, BRACKET_CHAR_WIDTH, valueRect.height), "[", bracketStyle);

			EditorGUI.MultiPropertyField(new Rect(valueRect.x + BRACKET_CHAR_WIDTH, valueRect.y, valueRect.width - BRACKET_CHAR_WIDTH * 2f, valueRect.height), labels, start);

			bracketStyle.alignment = TextAnchor.MiddleLeft;
			EditorGUI.LabelField(new Rect(valueRect.xMax - BRACKET_CHAR_WIDTH, valueRect.y, BRACKET_CHAR_WIDTH, valueRect.height), "]", bracketStyle);

			EditorGUI.EndProperty();
		}
	}
}

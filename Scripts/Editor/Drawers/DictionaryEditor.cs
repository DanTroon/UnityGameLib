using UnityEngine;
using UnityEditor;
using System;
using UnityGameLib.Collections;
using UnityGameLib.Editor.Utilities;

namespace UnityGameLib.Editor.Drawers {
	/// <summary>
	/// The PropertyDrawer for fields with a <see cref="UnityGameLib.Collections.DictionaryAttribute"/>.
	/// </summary>
	[CustomPropertyDrawer(typeof(DictionaryAttribute), true)]
	public class DictionaryEditor : PropertyDrawer {
		//private const float LINE_HEIGHT = 20f;
		private const float ADD_BUTTON_HEIGHT = 18f;
		private const float DELETE_BUTTON_HEIGHT = 18f;
		private const float LINE_MARGIN_VERTICAL = 2f;
		private const float LINE_SPACING = 2f * LINE_MARGIN_VERTICAL;
		private const float LIST_MARGIN_LEFT = 18f;

		private const string KEYS_PROPERTY_NAME = "_serialKeys";
		private const string VALUES_PROPERTY_NAME = "_serialValues";

		private static float LINE_HEIGHT {
			get { return EditorGUIUtility.singleLineHeight; }
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if (property.serializedObject.isEditingMultipleObjects) {
				DrawErrorGUI(position, property, label, "Multiple objects selected");
				return;
			}

			//Find the SerializableDictionary class in the inheritance tree
			Type genericType = typeof(SerializableDictionary<,>);
			Type dictType = fieldInfo.FieldType;
			while (dictType != null && !(dictType.IsGenericType && dictType.GetGenericTypeDefinition() == genericType)) {
				dictType = dictType.BaseType;
			}

			//Abort if SerializableDictionary wasn't in the tree
			if (dictType == null) {
				Debug.LogError(string.Format("Dictionary field '{0}' in {1} does not extend SerializableDictionary.", property.name, property.serializedObject.targetObject.GetType().Name));
				DrawErrorGUI(position, property, label, "Not a SerializableDictionary");
				return;
			}

			//Reflect the key and value types
			Type[] dictArgumentTypes = dictType.GetGenericArguments();
			Type keyType = dictArgumentTypes[0];
			Type valueType = dictArgumentTypes[1];

			//Prevent broken char serialization
			if (keyType == typeof(char)) {
				Debug.LogError(string.Format("Type '{0}' cannot be used as a SerializableDictionary key. Use an integer, string, or enum instead.", keyType.Name));
				DrawErrorGUI(position, property, label, "Invalid key type");
				return;
			}

			//Get the serialized representations of the keys and values
			SerializedProperty keys = property.FindPropertyRelative(KEYS_PROPERTY_NAME);
			if (keys == null) {
				Debug.LogError(string.Format("Cannot inspect Dictionary field '{0}' in {1}: Key type '{2}' is not Serializable.", property.name, property.serializedObject.targetObject.GetType().Name, keyType.Name));
				DrawErrorGUI(position, property, label, "Unserializable key type");
				return;
			}
			SerializedProperty values = property.FindPropertyRelative(VALUES_PROPERTY_NAME);
			if (values == null) {
				Debug.LogError(string.Format("Cannot inspect Dictionary field '{0}' in {1}: Value type '{2}' is not Serializable.", property.name, property.serializedObject.targetObject.GetType().Name, valueType.Name));
				DrawErrorGUI(position, property, label, "Unserializable value type");
				return;
			}

			//Determine the number of key-value pairs to render
			int pairCount = Mathf.Max(keys.arraySize, values.arraySize);

			//Start drawing
			EditorGUI.BeginProperty(position, label, property);

			property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, LINE_HEIGHT - LINE_SPACING), property.isExpanded, property.displayName, true);
			if (property.isExpanded) {
				Rect indentRect = EditorGUI.IndentedRect(position);
				int indentLevel = EditorGUI.indentLevel;

				bool addItem = GUI.Button(new Rect(indentRect.x + LIST_MARGIN_LEFT, position.y + GetPropertyHeight(property, label) - ADD_BUTTON_HEIGHT - LINE_MARGIN_VERTICAL, indentRect.width - LIST_MARGIN_LEFT * 2f, ADD_BUTTON_HEIGHT), new GUIContent(string.Format("+ ({0}, {1})", keyType.Name, valueType.Name), "Add a new entry"));
				if (addItem) {
					keys.InsertArrayElementAtIndex(pairCount);
					values.InsertArrayElementAtIndex(pairCount);

					++pairCount;
				}

				SerializedProperty key, value;
				Rect btnRect = new Rect(indentRect.x, position.y, LIST_MARGIN_LEFT, LINE_HEIGHT);
				Rect keyRect = new Rect(position.x + LIST_MARGIN_LEFT, position.y, EditorGUIUtility.labelWidth - LIST_MARGIN_LEFT, LINE_HEIGHT - LINE_SPACING);
				Rect valRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, LINE_HEIGHT - LINE_SPACING);

				float previousY = LINE_HEIGHT + LINE_MARGIN_VERTICAL;
				float rowHeight = 0f;

				for (int i = 0; i < pairCount; ++i) {
					btnRect.y = keyRect.y = valRect.y = position.y + previousY;

					if (GUI.Button(btnRect, new GUIContent("—", "Delete this entry"))) {
						keys.DeleteArrayElementAtIndex(i);
						values.DeleteArrayElementAtIndex(i);

						--pairCount;
						if (i >= pairCount)
							break;
					}

					key = keys.GetArrayElementAtIndex(i);
					value = values.GetArrayElementAtIndex(i);

					rowHeight = Mathf.Max(EditorGUI.GetPropertyHeight(key, GUIContent.none, true), EditorGUI.GetPropertyHeight(value, GUIContent.none, true));
					keyRect.height = valRect.height = rowHeight;

					EditorGUI.PropertyField(keyRect, key, GUIContent.none, true);

					EditorGUI.indentLevel = 0;
					EditorGUI.PropertyField(valRect, value, GUIContent.none, true);
					EditorGUI.indentLevel = indentLevel;

					previousY += rowHeight + LINE_SPACING;
				}
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			//If collapsed, only the foldout label's height matters
			if (!property.isExpanded || property.serializedObject.isEditingMultipleObjects)
				return LINE_HEIGHT;

			//Get the serialized representations of the keys and values
			SerializedProperty keys = property.FindPropertyRelative(KEYS_PROPERTY_NAME);
			SerializedProperty values = property.FindPropertyRelative(VALUES_PROPERTY_NAME);

			//Determine the number of key-value pairs to render
			int pairCount = Mathf.Max(keys.arraySize, values.arraySize);

			//Account for the foldout and the add button
			float totalHeight = LINE_HEIGHT + ADD_BUTTON_HEIGHT + LINE_SPACING;

			//Add the sum of all pair heights
			for (int i = 0; i < pairCount; ++i) {
				float keyHeight = i < keys.arraySize ? EditorGUI.GetPropertyHeight(keys.GetArrayElementAtIndex(i), GUIContent.none, true) : 0f;
				float valHeight = i < values.arraySize ? EditorGUI.GetPropertyHeight(values.GetArrayElementAtIndex(i), GUIContent.none, true) : 0f;
				totalHeight += Mathf.Max(keyHeight, valHeight) + LINE_SPACING;
			}

			return totalHeight;
		}

		protected void DrawErrorGUI(Rect position, SerializedProperty property, GUIContent label, string message = "") {
			string labelText = string.Format("{0}: {1}", property.displayName, message);

			EditorGUI.BeginProperty(position, label, property);
			EditorUtilities.PushContentColor(EditorUtilities.errorTextColor);

			EditorGUI.LabelField(new Rect(position.x, position.y, position.width, LINE_HEIGHT), labelText);

			EditorUtilities.PopContentColor();
			EditorGUI.EndProperty();
		}
	}
}

using UnityEngine;
using UnityEditor;
using UnityGameLib.Geometry;

namespace UnityGameLib.Editor.Drawers {
	/// <summary>
	/// The PropertyDrawer for <see cref="UnityGameLib.Geometry.CoordinateList2D"/>.
	/// </summary>
	[CustomPropertyDrawer(typeof(CoordinateList2D))]
	public class CoordinateListEditor2D : PropertyDrawer {
		private const float DIMENSIONS_FIELD_HEIGHT = 16f;
		private const float TABLE_PADDING = 6f;
		private const float TABLE_OFFSET_Y = DIMENSIONS_FIELD_HEIGHT * 3f;
		private const float TOGGLE_SIZE = 18f;
		private const float TOGGLE_SPACING = 20f;
	
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight), property.isExpanded, property.displayName);
			if (!property.isExpanded) {
				EditorGUI.EndProperty();
				return;
			}

			position.xMin += EditorGUIUtility.fieldWidth;
			position.yMin += EditorGUIUtility.singleLineHeight;

			SerializedProperty cellCountX = property.FindPropertyRelative("cellCountX");
			SerializedProperty cellCountY = property.FindPropertyRelative("cellCountY");
			SerializedProperty offsetX = property.FindPropertyRelative("offsetX");
			SerializedProperty offsetY = property.FindPropertyRelative("offsetY");
			SerializedProperty invertX = property.FindPropertyRelative("invertX");
			SerializedProperty invertY = property.FindPropertyRelative("invertY");
			SerializedProperty activeCells = property.FindPropertyRelative("activeCells");

			int oldOffsetX = offsetX.intValue;
			int oldOffsetY = offsetY.intValue;

			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			//Draw Dimension Fields
			EditorGUI.LabelField(new Rect(position.x, position.y, 80f, DIMENSIONS_FIELD_HEIGHT), "Dimensions");
			EditorGUI.LabelField(new Rect(position.x + 80f, position.y, 15f, DIMENSIONS_FIELD_HEIGHT), "X");
			EditorGUI.DelayedIntField(new Rect(position.x + 95f, position.y, 30f, DIMENSIONS_FIELD_HEIGHT), cellCountX, GUIContent.none);
			EditorGUI.LabelField(new Rect(position.x + 135f, position.y, 15f, DIMENSIONS_FIELD_HEIGHT), "Y");
			EditorGUI.DelayedIntField(new Rect(position.x + 150f, position.y, 30f, DIMENSIONS_FIELD_HEIGHT), cellCountY, GUIContent.none);

			//Draw Offset Fields
			EditorGUI.LabelField(new Rect(position.x, position.y + DIMENSIONS_FIELD_HEIGHT, 80f, DIMENSIONS_FIELD_HEIGHT), "Offset");
			EditorGUI.LabelField(new Rect(position.x + 80f, position.y + DIMENSIONS_FIELD_HEIGHT, 15f, DIMENSIONS_FIELD_HEIGHT), "X");
			EditorGUI.DelayedIntField(new Rect(position.x + 95f, position.y + DIMENSIONS_FIELD_HEIGHT, 30f, DIMENSIONS_FIELD_HEIGHT), offsetX, GUIContent.none);
			EditorGUI.LabelField(new Rect(position.x + 135f, position.y + DIMENSIONS_FIELD_HEIGHT, 15f, DIMENSIONS_FIELD_HEIGHT), "Y");
			EditorGUI.DelayedIntField(new Rect(position.x + 150f, position.y + DIMENSIONS_FIELD_HEIGHT, 30f, DIMENSIONS_FIELD_HEIGHT), offsetY, GUIContent.none);

			//Draw Inversion Toggles
			EditorGUI.LabelField(new Rect(position.x, position.y + DIMENSIONS_FIELD_HEIGHT * 2, 80f, DIMENSIONS_FIELD_HEIGHT), "Invert");
			EditorGUI.LabelField(new Rect(position.x + 80f, position.y + DIMENSIONS_FIELD_HEIGHT * 2, 15f, DIMENSIONS_FIELD_HEIGHT), "X");
			EditorGUI.PropertyField(new Rect(position.x + 95f, position.y + DIMENSIONS_FIELD_HEIGHT * 2, 30f, DIMENSIONS_FIELD_HEIGHT), invertX, GUIContent.none);
			EditorGUI.LabelField(new Rect(position.x + 135f, position.y + DIMENSIONS_FIELD_HEIGHT * 2, 15f, DIMENSIONS_FIELD_HEIGHT), "Y");
			EditorGUI.PropertyField(new Rect(position.x + 150f, position.y + DIMENSIONS_FIELD_HEIGHT * 2, 30f, DIMENSIONS_FIELD_HEIGHT), invertY, GUIContent.none);

			//Adjust the stored values to match modified offsets
			if (offsetX.intValue != oldOffsetX || offsetY.intValue != oldOffsetY) {
				ModifyOffsets(activeCells, offsetX.intValue - oldOffsetX, offsetY.intValue - oldOffsetY);
			}

			//Trim the table as needed in case the dimensions were modified
			TrimTable(activeCells, cellCountX.intValue, cellCountY.intValue, offsetX.intValue, offsetY.intValue);


			//Draw Table
			Rect itemPosition;
			int displayX, displayY;
			int countX = cellCountX.intValue;
			int countY = cellCountY.intValue;

			GUIStyle labelStyle = new GUIStyle();

			//Draw X Labels
			if (invertY.boolValue) {
				displayY = cellCountY.intValue;
				labelStyle.alignment = TextAnchor.UpperCenter;
			} else {
				displayY = 0;
				labelStyle.alignment = TextAnchor.LowerCenter;
			}

			for (int x = 0; x < countX; ++x) {
				displayX = invertX.boolValue ? countX - x - 1 : x + 1;
				itemPosition = new Rect(position.x + displayX * TOGGLE_SPACING, TABLE_PADDING + position.y + displayY * TOGGLE_SPACING + TABLE_OFFSET_Y, TOGGLE_SIZE, TOGGLE_SIZE);
				EditorGUI.LabelField(itemPosition, (x + offsetX.intValue).ToString(), labelStyle);
			}

			labelStyle.alignment = invertX.boolValue ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
			for (int y = 0; y < countY; ++y) {
				//Draw Y Labels
				displayY = invertY.boolValue ? countY - y - 1 : y + 1;
				displayX = invertX.boolValue ? countX : 0;
				itemPosition = new Rect(position.x + displayX * TOGGLE_SPACING, TABLE_PADDING + position.y + displayY * TOGGLE_SPACING + TABLE_OFFSET_Y, TOGGLE_SIZE, TOGGLE_SIZE);
				EditorGUI.LabelField(itemPosition, (y + offsetY.intValue).ToString(), labelStyle);

				for (int x = 0; x < countX; ++x) {
					displayX = invertX.boolValue ? countX - x - 1 : x + 1;

					//Draw Cells
					itemPosition = new Rect(position.x + displayX * TOGGLE_SPACING, TABLE_PADDING + position.y + displayY * TOGGLE_SPACING + TABLE_OFFSET_Y, TOGGLE_SIZE, TOGGLE_SIZE);
					int cellIndex = GetCellIndex(activeCells, x + offsetX.intValue, y + offsetY.intValue);
					if (cellIndex == -1) {
						if (GUI.Button(itemPosition, "")) {
							DoActivateCell(activeCells, x + offsetX.intValue, y + offsetY.intValue);
						}
					} else {
						if (GUI.Button(itemPosition, "■")) {
							DoDeactivateCell(activeCells, cellIndex);
						}
					}
				}
			}

			EditorGUI.EndProperty();
			EditorGUI.indentLevel = indent;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			if (!property.isExpanded) {
				return EditorGUIUtility.singleLineHeight;
			}

			return EditorGUIUtility.singleLineHeight + TOGGLE_SPACING * (1 + property.FindPropertyRelative("cellCountY").intValue) + TABLE_PADDING * 2f + DIMENSIONS_FIELD_HEIGHT * 3f;
		}

		protected static int GetCellIndex(SerializedProperty activeCells, int x, int y) {
			for (int i = 0, count = activeCells.arraySize; i < count; ++i) {
				SerializedProperty coordinates = activeCells.GetArrayElementAtIndex(i);
				if (coordinates.FindPropertyRelative("x").intValue == x && coordinates.FindPropertyRelative("y").intValue == y) {
					return i;
				}
			}

			return -1;
		}

		protected static bool SetCellActive(SerializedProperty activeCells, int x, int y, bool active) {
			int cellIndex = GetCellIndex(activeCells, x, y);

			if (active) {
				if (cellIndex != -1)
					return false;

				DoActivateCell(activeCells, x, y);
				return true;
			} else {
				if (cellIndex == -1)
					return false;

				DoDeactivateCell(activeCells, cellIndex);
				return true;
			}
		}

		protected static void ToggleCellActive(SerializedProperty activeCells, int x, int y) {
			int cellIndex = GetCellIndex(activeCells, x, y);
			if (cellIndex == -1) {
				DoActivateCell(activeCells, x, y);
			} else {
				DoDeactivateCell(activeCells, cellIndex);
			}
		}

		protected static void DoActivateCell(SerializedProperty activeCells, int x, int y) {
			activeCells.InsertArrayElementAtIndex(activeCells.arraySize);
			SerializedProperty coordinates = activeCells.GetArrayElementAtIndex(activeCells.arraySize - 1);
			coordinates.FindPropertyRelative("x").intValue = x;
			coordinates.FindPropertyRelative("y").intValue = y;
		}

		protected static void DoDeactivateCell(SerializedProperty activeCells, int cellIndex) {
			activeCells.DeleteArrayElementAtIndex(cellIndex);
		}

		protected static void ModifyOffsets(SerializedProperty activeCells, int deltaX, int deltaY) {
			for (int i = 0, count = activeCells.arraySize; i < count; ++i) {
				SerializedProperty coordinates = activeCells.GetArrayElementAtIndex(i);
				coordinates.FindPropertyRelative("x").intValue += deltaX;
				coordinates.FindPropertyRelative("y").intValue += deltaY;
			}
		}

		protected static void TrimTable(SerializedProperty activeCells, int cellCountX, int cellCountY, int offsetX, int offsetY) {
			for (int i = activeCells.arraySize - 1; i >= 0; --i) {
				SerializedProperty coordinates = activeCells.GetArrayElementAtIndex(i);
				int localX = coordinates.FindPropertyRelative("x").intValue - offsetX;
				int localY = coordinates.FindPropertyRelative("y").intValue - offsetY;
				if (localX < 0 || localX >= cellCountX || localY < 0 || localY >= cellCountY) {
					DoDeactivateCell(activeCells, i);
				}
			}
		}
	}
}

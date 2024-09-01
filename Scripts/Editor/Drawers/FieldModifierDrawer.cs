using UnityGameLib.Attributes;
using UnityGameLib.Editor.Utilities;
using UnityGameLib.Geometry;
using System;
using UnityEditor;
using UnityEngine;

namespace UnityGameLib.Editor.Drawers {
	/// <summary>
	/// The PropertyDrawer for fields with any <see cref="UnityGameLib.Attributes.FieldModifierAttribute"/>.
	/// </summary>
	[CustomPropertyDrawer(typeof(FieldModifierAttribute), true)]
	public class FieldModifierDrawer : PropertyDrawer {
		public const string TOOLTIP = "Required - cannot be empty";

		protected bool required = false;
		protected bool lockedInPlayMode = false;
		protected bool lockedInEditMode = false;

		protected bool isValid = true;

		protected bool uiDisabled {
			get { return (lockedInPlayMode && Application.isPlaying) || (lockedInEditMode && !Application.isPlaying); }
		}

		public FieldModifierDrawer() : base() {
			
		}

		protected void RefreshAttributes() {
			required = false;
			lockedInPlayMode = false;
			lockedInEditMode = false;

			foreach (Attribute attribute in fieldInfo.GetCustomAttributes(false)) {
				if (attribute is RequiredAttribute) {
					required = true;
				} else if (attribute is RuntimeLockedAttribute) {
					if (((RuntimeLockedAttribute) attribute).invert) {
						lockedInEditMode = true;
					} else {
						lockedInPlayMode = true;
					}
				} else if (attribute is LockedAttribute) {
					lockedInEditMode = true;
					lockedInPlayMode = true;
				}
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			RefreshAttributes();
			isValid = ValidateProperty(property);
			
			Rect iconRect = new Rect(position.x - 14f, position.y, Mathf.Max(16f, 2f + EditorGUI.IndentedRect(position).x), 16f);
			EditorGUI.LabelField(iconRect, new GUIContent(GetIcon(property), GetTooltip(property)));

			if (!isValid) EditorUtilities.PushContentColor(EditorUtilities.errorTextColor);

			EditorGUI.BeginDisabledGroup(uiDisabled);
			EditorGUI.PropertyField(position, property, label, true);
			EditorGUI.EndDisabledGroup();

			if (!isValid) EditorUtilities.PopContentColor();
		}

		protected bool ValidateProperty(SerializedProperty property) {
			if (!required)
				return true;

			if (property.propertyType == SerializedPropertyType.ObjectReference)
				return property.objectReferenceValue != null;

			if (property.propertyType == SerializedPropertyType.String)
				return !string.IsNullOrEmpty(property.stringValue);

			return true;
		}

		protected string GetTooltip(SerializedProperty property) {
			string result = required ? "Required" : "Optional";
			if (lockedInEditMode) {
				if (lockedInPlayMode) {
					result = "Not editable";
				} else {
					result += ", only editable in Play Mode";
				}
			} else if (lockedInPlayMode) {
				result += ", not editable in Play Mode";
			}

			return result;
		}

		protected Texture2D GetIcon(SerializedProperty property) {
			Coordinates2D resolution = new Coordinates2D(16, 16);

			Texture2D result = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);

			if (required) {
				DrawRequiredIcon(result);
			} else if (lockedInEditMode || lockedInPlayMode) {
				DrawLockedIcon(result);
			} else {
				DrawDefaultIcon(result);
			}

			result.Apply();
			return result;
		}

		protected void DrawRequiredIcon(Texture2D texture) {
			CoordinateList2D pixels = new CoordinateList2D(texture.width, texture.height);
			pixels.activeCells.Add(new Coordinates2D(7, 2));
			pixels.activeCells.Add(new Coordinates2D(7, 3));
			pixels.activeCells.Add(new Coordinates2D(7, 4));
			pixels.activeCells.Add(new Coordinates2D(7, 5));
			pixels.activeCells.Add(new Coordinates2D(7, 6));
			pixels.activeCells.Add(new Coordinates2D(7, 7));
			pixels.activeCells.Add(new Coordinates2D(7, 8));
			pixels.activeCells.Add(new Coordinates2D(7, 9));
			pixels.activeCells.Add(new Coordinates2D(7, 10));
			pixels.activeCells.Add(new Coordinates2D(7, 11));
			pixels.activeCells.Add(new Coordinates2D(8, 2));
			pixels.activeCells.Add(new Coordinates2D(8, 3));
			pixels.activeCells.Add(new Coordinates2D(8, 4));
			pixels.activeCells.Add(new Coordinates2D(8, 5));
			pixels.activeCells.Add(new Coordinates2D(8, 6));
			pixels.activeCells.Add(new Coordinates2D(8, 7));
			pixels.activeCells.Add(new Coordinates2D(8, 8));
			pixels.activeCells.Add(new Coordinates2D(8, 9));
			pixels.activeCells.Add(new Coordinates2D(8, 10));
			pixels.activeCells.Add(new Coordinates2D(8, 11));

			pixels.activeCells.Add(new Coordinates2D(6, 7));
			pixels.activeCells.Add(new Coordinates2D(5, 7));
			pixels.activeCells.Add(new Coordinates2D(5, 8));
			pixels.activeCells.Add(new Coordinates2D(4, 8));
			pixels.activeCells.Add(new Coordinates2D(3, 8));
			pixels.activeCells.Add(new Coordinates2D(5, 9));
			pixels.activeCells.Add(new Coordinates2D(4, 9));
			pixels.activeCells.Add(new Coordinates2D(3, 9));

			pixels.activeCells.Add(new Coordinates2D(6, 6));
			pixels.activeCells.Add(new Coordinates2D(5, 6));
			pixels.activeCells.Add(new Coordinates2D(5, 5));
			pixels.activeCells.Add(new Coordinates2D(4, 5));
			pixels.activeCells.Add(new Coordinates2D(3, 5));
			pixels.activeCells.Add(new Coordinates2D(5, 4));
			pixels.activeCells.Add(new Coordinates2D(4, 4));
			pixels.activeCells.Add(new Coordinates2D(3, 4));

			pixels.activeCells.Add(new Coordinates2D(9, 7));
			pixels.activeCells.Add(new Coordinates2D(10, 7));
			pixels.activeCells.Add(new Coordinates2D(10, 8));
			pixels.activeCells.Add(new Coordinates2D(11, 8));
			pixels.activeCells.Add(new Coordinates2D(12, 8));
			pixels.activeCells.Add(new Coordinates2D(10, 9));
			pixels.activeCells.Add(new Coordinates2D(11, 9));
			pixels.activeCells.Add(new Coordinates2D(12, 9));

			pixels.activeCells.Add(new Coordinates2D(9, 6));
			pixels.activeCells.Add(new Coordinates2D(10, 6));
			pixels.activeCells.Add(new Coordinates2D(10, 5));
			pixels.activeCells.Add(new Coordinates2D(11, 5));
			pixels.activeCells.Add(new Coordinates2D(12, 5));
			pixels.activeCells.Add(new Coordinates2D(10, 4));
			pixels.activeCells.Add(new Coordinates2D(11, 4));
			pixels.activeCells.Add(new Coordinates2D(12, 4));

			Color fgColor = isValid ? EditorUtilities.iconColor : EditorUtilities.errorIconColor;
			Color bgColor = fgColor;
			bgColor.a = 0f;

			for (int y = 0; y < texture.height; ++y) {
				for (int x = 0; x < texture.width; ++x) {
					if (pixels.Contains(x, y)) {
						texture.SetPixel(x, y, fgColor);
					} else {
						texture.SetPixel(x, y, bgColor);
					}
				}
			}
		}

		protected void DrawLockedIcon(Texture2D texture) {
			CoordinateList2D pixels = new CoordinateList2D(texture.width, texture.height);

			//Lock bar
			int yOffset = 0;
			if (!uiDisabled) {
				yOffset = 1;

				pixels.activeCells.Add(new Coordinates2D(5, yOffset + 6));
				pixels.activeCells.Add(new Coordinates2D(6, yOffset + 6));
			}

			pixels.activeCells.Add(new Coordinates2D(5, yOffset + 7));
			pixels.activeCells.Add(new Coordinates2D(5, yOffset + 8));
			pixels.activeCells.Add(new Coordinates2D(5, yOffset + 9));
			pixels.activeCells.Add(new Coordinates2D(5, yOffset + 10));
			pixels.activeCells.Add(new Coordinates2D(6, yOffset + 7));
			pixels.activeCells.Add(new Coordinates2D(6, yOffset + 8));
			pixels.activeCells.Add(new Coordinates2D(6, yOffset + 9));
			pixels.activeCells.Add(new Coordinates2D(6, yOffset + 10));
			pixels.activeCells.Add(new Coordinates2D(6, yOffset + 11));
			pixels.activeCells.Add(new Coordinates2D(7, yOffset + 11));
			pixels.activeCells.Add(new Coordinates2D(7, yOffset + 12));
			pixels.activeCells.Add(new Coordinates2D(8, yOffset + 11));
			pixels.activeCells.Add(new Coordinates2D(8, yOffset + 12));
			pixels.activeCells.Add(new Coordinates2D(9, yOffset + 11));
			pixels.activeCells.Add(new Coordinates2D(9, yOffset + 12));
			pixels.activeCells.Add(new Coordinates2D(10, yOffset + 11));
			pixels.activeCells.Add(new Coordinates2D(10, yOffset + 10));
			pixels.activeCells.Add(new Coordinates2D(10, yOffset + 9));
			pixels.activeCells.Add(new Coordinates2D(10, yOffset + 8));
			pixels.activeCells.Add(new Coordinates2D(10, yOffset + 7));
			pixels.activeCells.Add(new Coordinates2D(11, yOffset + 10));
			pixels.activeCells.Add(new Coordinates2D(11, yOffset + 9));
			pixels.activeCells.Add(new Coordinates2D(11, yOffset + 8));
			pixels.activeCells.Add(new Coordinates2D(11, yOffset + 7));

			//Lock box
			pixels.activeCells.Add(new Coordinates2D(4, 1));
			pixels.activeCells.Add(new Coordinates2D(5, 1));
			pixels.activeCells.Add(new Coordinates2D(6, 1));
			pixels.activeCells.Add(new Coordinates2D(7, 1));
			pixels.activeCells.Add(new Coordinates2D(8, 1));
			pixels.activeCells.Add(new Coordinates2D(9, 1));
			pixels.activeCells.Add(new Coordinates2D(10, 1));
			pixels.activeCells.Add(new Coordinates2D(11, 1));
			pixels.activeCells.Add(new Coordinates2D(12, 1));
			pixels.activeCells.Add(new Coordinates2D(4, 2));
			pixels.activeCells.Add(new Coordinates2D(5, 2));
			pixels.activeCells.Add(new Coordinates2D(6, 2));
			pixels.activeCells.Add(new Coordinates2D(7, 2));
			pixels.activeCells.Add(new Coordinates2D(8, 2));
			pixels.activeCells.Add(new Coordinates2D(9, 2));
			pixels.activeCells.Add(new Coordinates2D(10, 2));
			pixels.activeCells.Add(new Coordinates2D(11, 2));
			pixels.activeCells.Add(new Coordinates2D(12, 2));
			pixels.activeCells.Add(new Coordinates2D(4, 3));
			pixels.activeCells.Add(new Coordinates2D(5, 3));
			pixels.activeCells.Add(new Coordinates2D(6, 3));
			pixels.activeCells.Add(new Coordinates2D(7, 3));
			pixels.activeCells.Add(new Coordinates2D(8, 3));
			pixels.activeCells.Add(new Coordinates2D(9, 3));
			pixels.activeCells.Add(new Coordinates2D(10, 3));
			pixels.activeCells.Add(new Coordinates2D(11, 3));
			pixels.activeCells.Add(new Coordinates2D(12, 3));
			pixels.activeCells.Add(new Coordinates2D(4, 4));
			pixels.activeCells.Add(new Coordinates2D(5, 4));
			pixels.activeCells.Add(new Coordinates2D(6, 4));
			pixels.activeCells.Add(new Coordinates2D(7, 4));
			pixels.activeCells.Add(new Coordinates2D(8, 4));
			pixels.activeCells.Add(new Coordinates2D(9, 4));
			pixels.activeCells.Add(new Coordinates2D(10, 4));
			pixels.activeCells.Add(new Coordinates2D(11, 4));
			pixels.activeCells.Add(new Coordinates2D(12, 4));
			pixels.activeCells.Add(new Coordinates2D(4, 5));
			pixels.activeCells.Add(new Coordinates2D(5, 5));
			pixels.activeCells.Add(new Coordinates2D(6, 5));
			pixels.activeCells.Add(new Coordinates2D(7, 5));
			pixels.activeCells.Add(new Coordinates2D(8, 5));
			pixels.activeCells.Add(new Coordinates2D(9, 5));
			pixels.activeCells.Add(new Coordinates2D(10, 5));
			pixels.activeCells.Add(new Coordinates2D(11, 5));
			pixels.activeCells.Add(new Coordinates2D(12, 5));
			pixels.activeCells.Add(new Coordinates2D(4, 6));
			pixels.activeCells.Add(new Coordinates2D(5, 6));
			pixels.activeCells.Add(new Coordinates2D(6, 6));
			pixels.activeCells.Add(new Coordinates2D(7, 6));
			pixels.activeCells.Add(new Coordinates2D(8, 6));
			pixels.activeCells.Add(new Coordinates2D(9, 6));
			pixels.activeCells.Add(new Coordinates2D(10, 6));
			pixels.activeCells.Add(new Coordinates2D(11, 6));
			pixels.activeCells.Add(new Coordinates2D(12, 6));

			Color fgColor = EditorUtilities.iconColor;
			Color bgColor = fgColor;
			bgColor.a = 0f;

			for (int y = 0; y < texture.height; ++y) {
				for (int x = 0; x < texture.width; ++x) {
					if (pixels.Contains(x, y)) {
						texture.SetPixel(x, y, fgColor);
					} else {
						texture.SetPixel(x, y, bgColor);
					}
				}
			}
		}

		protected void DrawDefaultIcon(Texture2D texture) {
			Color fgColor = EditorUtilities.iconColor;
			Color bgColor = fgColor;
			bgColor.a = 0f;

			for (int y = 0; y < texture.height; ++y) {
				for (int x = 0; x < texture.width; ++x) {
					texture.SetPixel(x, y, bgColor);
				}
			}

			int halfWidth = texture.width / 2;
			int halfHeight = texture.height / 2;

			texture.SetPixel(halfWidth - 1, halfHeight - 1, fgColor);
			texture.SetPixel(halfWidth - 1, halfHeight, fgColor);
			texture.SetPixel(halfWidth, halfHeight - 1, fgColor);
			texture.SetPixel(halfWidth, halfHeight, fgColor);

			fgColor.a *= .3f;

			texture.SetPixel(halfWidth - 1, halfHeight - 2, fgColor);
			texture.SetPixel(halfWidth - 1, halfHeight + 1, fgColor);
			texture.SetPixel(halfWidth, halfHeight - 2, fgColor);
			texture.SetPixel(halfWidth, halfHeight + 1, fgColor);
			texture.SetPixel(halfWidth - 2, halfHeight - 1, fgColor);
			texture.SetPixel(halfWidth - 2, halfHeight, fgColor);
			texture.SetPixel(halfWidth + 1, halfHeight - 1, fgColor);
			texture.SetPixel(halfWidth + 1, halfHeight, fgColor);
		}
	}
}

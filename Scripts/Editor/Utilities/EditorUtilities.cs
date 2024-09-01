using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityGameLib.Editor.Utilities {
	/// <summary>
	/// Provides static utility methods for Editor use.
	/// </summary>
	public static class EditorUtilities {
		private static Stack<Color> _contentColorStack = new Stack<Color>();

		/// <summary>
		/// The default color of Unity's inspector text
		/// </summary>
		public static Color textColor {
			get { return EditorGUIUtility.isProSkin ? new Color32(180, 180, 180, 255) : new Color32(0, 0, 0, 255); }
		}

		/// <summary>
		/// A multiply color to match white icons to Unity's inspector text color
		/// </summary>
		public static Color iconColor {
			get { return textColor; }
		}

		/// <summary>
		/// A color for text displayed in an error state
		/// </summary>
		public static Color errorTextColor {
			get { return EditorGUIUtility.isProSkin ? new Color32(255, 191, 0, 255) : new Color32(255, 31, 0, 255); }
		}

		/// <summary>
		/// A color used to draw icons in an error state
		/// </summary>
		public static Color errorIconColor {
			get { return EditorGUIUtility.isProSkin ? iconColor * errorTextColor : errorTextColor; }
		}

		/// <summary>
		/// Adds a new GUI/text color to the top of the stack, making it the new active color.
		/// Any Editor or PropertyDrawer calling this should then call PopContentColor when done.
		/// </summary>
		/// <param name="color">The new color</param>
		public static void PushContentColor(Color color) {
			InitContentColor();

			_contentColorStack.Push(color);
			GUI.contentColor = _contentColorStack.Peek();
		}

		/// <summary>
		/// Removes the last GUI/text color from the stack, reverting to the previously applied color.
		/// </summary>
		/// <returns>The color being removed, or the default color if there is none to remove</returns>
		public static Color PopContentColor() {
			InitContentColor();

			Color oldColor = _contentColorStack.Count > 1 ? _contentColorStack.Pop() : _contentColorStack.Peek();
			GUI.contentColor = _contentColorStack.Peek();
			return oldColor;
		}

		private static void InitContentColor() {
			if (_contentColorStack.Count == 0) {
				_contentColorStack.Push(GUI.contentColor);
			}
		}
	}
}

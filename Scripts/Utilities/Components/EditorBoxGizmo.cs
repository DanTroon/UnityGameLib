using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Draws a box gizmo at an object's position in the Unity Editor.
	/// </summary>
	public class EditorBoxGizmo : EditorGizmo {
		public bool wireframe = false;
		public Color color = Color.magenta;
		public Vector3 center = new Vector3();
		public Vector3 size = Vector3.one;

		protected override void Draw() {
			Color oldColor = Gizmos.color;
			Matrix4x4 oldMatrix = Gizmos.matrix;

			Gizmos.color = color;
			Gizmos.matrix = transform.localToWorldMatrix;
			if (wireframe) {
				Gizmos.DrawWireCube(center, size);
			} else {
				Gizmos.DrawCube(center, size);
			}
			Gizmos.color = oldColor;
			Gizmos.matrix = oldMatrix;
		}
	}
}

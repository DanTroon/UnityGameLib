using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Draws a sphere gizmo at an object's position in the Unity Editor.
	/// </summary>
	public class EditorSphereGizmo : EditorGizmo {
		public bool wireframe = false;
		public Color color = Color.magenta;
		public Vector3 center = new Vector3();
		public float radius = .5f;

		protected override void Draw() {
			Color oldColor = Gizmos.color;
			Matrix4x4 oldMatrix = Gizmos.matrix;

			Gizmos.color = color;
			Gizmos.matrix = transform.localToWorldMatrix;
			if (wireframe) {
				Gizmos.DrawWireSphere(center, radius);
			} else {
				Gizmos.DrawSphere(center, radius);
			}
			Gizmos.color = oldColor;
			Gizmos.matrix = oldMatrix;
		}
	}
}

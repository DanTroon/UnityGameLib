using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// An abstract class for drawing gizmos in the Unity Editor.
	/// </summary>
	public abstract class EditorGizmo : MonoBehaviour {
		/// <summary> If enabled, this gizmo only displays while the object is selected. </summary>
		[Tooltip("If enabled, this gizmo only displays while the object is selected.")]
		public bool selectedOnly = false;

		protected virtual void OnDrawGizmos() {
			if (!selectedOnly) {
				Draw();
			}
		}

		protected virtual void OnDrawGizmosSelected() {
			if (selectedOnly) {
				Draw();
			}
		}

		/// <summary>
		/// Draws the gizmo.
		/// </summary>
		protected abstract void Draw();
	}
}

using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// Applies obliqueness to a camera so the focal point is not locked to the center of the viewport.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class ObliqueCamera : MonoBehaviour {
		[SerializeField, Tooltip("The obliqueness along the X and Y axes.")]
		protected Vector2 _obliqueness = new Vector2();

		/// <summary>
		/// The obliqueness along the X and Y axes.
		/// </summary>
		public virtual Vector2 obliqueness {
			get { return _obliqueness; }
			set { _obliqueness = value; }
		}

		protected virtual void OnPreCull() {
			Apply();
		}

		protected virtual void OnDisable() {
			Unapply();
		}

		protected virtual void Apply() {
			Camera camera = GetComponent<Camera>();
			camera.ResetProjectionMatrix();

			Matrix4x4 projection = camera.projectionMatrix;
			projection[0, 2] = _obliqueness.x;
			projection[1, 2] = _obliqueness.y;
			camera.projectionMatrix = projection;
		}

		protected virtual void Unapply() {
			GetComponent<Camera>().ResetProjectionMatrix();
		}
	}
}

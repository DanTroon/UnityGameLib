using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// A struct containing position, rotation, and field of view information for a camera.
	/// </summary>
	public struct CameraSpec {
		/// <summary>The position of the camera</summary>
		public Vector3 position;
		/// <summary>The rotation of the camera</summary>
		public Quaternion rotation;
		/// <summary>The camera's field of view</summary>
		public float fieldOfView;

		/// <summary>
		/// Creates a CameraSpec with the specified properties.
		/// </summary>
		/// <param name="position">The position of the camera</param>
		/// <param name="rotation">The rotation of the camera</param>
		/// <param name="fieldOfView">The camera's field of view</param>
		public CameraSpec(Vector3 position, Quaternion rotation, float fieldOfView) {
			this.position = position;
			this.rotation = rotation;
			this.fieldOfView = fieldOfView;
		}

		/// <summary>
		/// Creates a CameraSpec based on the current properties of an existing Camera.
		/// </summary>
		/// <param name="view">The Camera to copy from</param>
		public CameraSpec(Camera view) : this(view.transform.position, view.transform.rotation, view.fieldOfView) { }

		/// <summary>
		/// Applies the settings from this CameraSpec to the specified Camera.
		/// </summary>
		/// <param name="toView">The camera to which the settings are applied</param>
		public void Apply(Camera toView) {
			toView.transform.position = position;
			toView.transform.rotation = rotation;
			toView.fieldOfView = fieldOfView;
		}

		/// <summary>
		/// Linearly interpolates from one CameraSpec to another.
		/// </summary>
		/// <param name="start">The starting CameraSpec</param>
		/// <param name="end">The ending CameraSpec</param>
		/// <param name="t">The interpolant value from 0 to 1, where 0 is <paramref name="start"/> and 1 is <paramref name="end"/></param>
		/// <returns>A new CameraSpec representing position <paramref name="t"/> on the line from <paramref name="start"/> to <paramref name="end"/></returns>
		public static CameraSpec Lerp(CameraSpec start, CameraSpec end, float t) {
			return new CameraSpec(
				Vector3.Lerp(start.position, end.position, t),
				Quaternion.Slerp(start.rotation, end.rotation, t),
				Mathf.Lerp(start.fieldOfView, end.fieldOfView, t)
			);
		}

		/// <summary>
		/// Linearly interpolates from one CameraSpec to another, unclamped.
		/// </summary>
		/// <param name="start">The starting CameraSpec</param>
		/// <param name="end">The ending CameraSpec</param>
		/// <param name="t">The unclamped interpolant value, where 0 is <paramref name="start"/> and 1 is <paramref name="end"/></param>
		/// <returns>A new CameraSpec representing position <paramref name="t"/> on the line from <paramref name="start"/> to <paramref name="end"/></returns>
		public static CameraSpec LerpUnclamped(CameraSpec start, CameraSpec end, float t) {
			return new CameraSpec(
				Vector3.LerpUnclamped(start.position, end.position, t),
				Quaternion.SlerpUnclamped(start.rotation, end.rotation, t),
				Mathf.LerpUnclamped(start.fieldOfView, end.fieldOfView, t)
			);
		}
	}
}

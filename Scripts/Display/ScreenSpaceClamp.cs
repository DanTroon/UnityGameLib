using UnityGameLib.Attributes;
using UnityGameLib.Geometry;
using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// Clamps a GameObject so that it always remains within a set portion of the screen or distance from the camera.
	/// </summary>
	public class ScreenSpaceClamp : MonoBehaviour {
		[SerializeField, Optional, Tooltip("An optional camera to face. If empty, the main camera is used.")]
		private Camera _camera;
		[SerializeField, Tooltip("The normalized rectangle (0 to 1 on each axis) that clamps the object's origin in screen space. (0,0) is the lower-left corner of the screen.")]
		private Rect _screenBounds = new Rect(0f, 0f, 1f, 1f);
		[SerializeField, Tooltip("The minimum and maximum distance between the camera and the object's origin.")]
		private LinearRange _distanceRange = new LinearRange(0f, 99999f);

		private Vector3 _originalLocalPosition;

		/// <summary>
		/// The camera this object is facing.
		/// </summary>
		/// <remarks>If no camera is explicitly set, this is the main camera.</remarks>
		public new Camera camera {
			get { return _camera == null ? Camera.main : _camera; }
			set { _camera = value; }
		}

		/// <summary> The normalized rectangle that clamps the object's origin in screen space. </summary>
		/// <remarks>
		/// The screen space ranges from 0 to 1 on each axis, with (0,0) being the lower-left corner.
		/// For example, <c>screenBounds = new Rect(0f, 0f, .9f, 1f</c> adds a 10% margin to the right side
		/// of the screen.
		/// </remarks>
		public Rect screenBounds {
			get { return _screenBounds; }
			set { _screenBounds = value; }
		}

		/// <summary> The minimum and maximum distance between the camera and the object's origin. </summary>
		public LinearRange distanceRange {
			get { return _distanceRange; }
			set { _distanceRange = value; }
		}

		protected virtual void OnEnable() {
			_originalLocalPosition = transform.localPosition;
		}

		protected virtual void OnDisable() {
			transform.localPosition = _originalLocalPosition;
		}

		protected void LateUpdate() {
			Camera camera = this.camera;

			transform.localPosition = _originalLocalPosition;

			Vector3 viewPos = camera.WorldToViewportPoint(transform.position);
			viewPos.x = Mathf.Clamp(viewPos.x, _screenBounds.xMin, _screenBounds.xMax);
			viewPos.y = Mathf.Clamp(viewPos.y, _screenBounds.yMin, _screenBounds.yMax);
			viewPos.z = Mathf.Clamp(viewPos.z, _distanceRange.min, _distanceRange.max);

			transform.position = camera.ViewportToWorldPoint(viewPos);
		}
	}
}

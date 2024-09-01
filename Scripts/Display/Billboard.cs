using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// Makes a GameObject always face the camera.
	/// </summary>
	public class Billboard : MonoBehaviour {
		[SerializeField, Optional, Tooltip("An optional camera to face. If empty, the main camera is used.")]
		private Camera _camera;
		[SerializeField, Tooltip("If enabled, the local position of the object will be used as a camera-relative offset.")]
		private bool _billboardLocalPosition = false;
		[SerializeField, Tooltip("If enabled, the billboard will face the camera instead of the camera plane (Looks better in some cases but uses more CPU).")]
		private bool _anglePerfect = false;

		private Vector3 _originalLocalPosition;

		/// <summary>
		/// The camera this object is facing.
		/// </summary>
		/// <remarks>If no camera is explicitly set, this is the main camera.</remarks>
		public new Camera camera {
			get { return _camera ? _camera : Camera.main; }
			set { _camera = value; }
		}

		/// <summary>
		/// If enabled, the local position of the object will be used as a camera-relative offset.
		/// </summary>
		public bool billboardLocalPosition {
			get { return _billboardLocalPosition; }
			set { _billboardLocalPosition = value; }
		}

		protected void OnEnable() {
			_originalLocalPosition = transform.localPosition;
		}

		protected void OnDisable() {
			if (_billboardLocalPosition) {
				transform.localPosition = _originalLocalPosition;
			}
		}

		protected void LateUpdate() {
			Camera camera = this.camera;

			Vector3 origin = transform.parent ? transform.parent.position : Vector3.zero;
			Quaternion rotation = _anglePerfect ? GetBillboardRotation(origin) : camera.transform.rotation;

			transform.rotation = rotation;
			if (_billboardLocalPosition) {
				transform.position = origin + rotation * _originalLocalPosition;
			}
		}

		protected Quaternion GetBillboardRotation(Vector3 target) {
			try {
				Ray ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(target));
				return Quaternion.LookRotation(ray.direction, camera.transform.up);
			} catch {
				return camera.transform.rotation;
			}
		}
	}
}

using UnityEngine;
using UnityGameLib.Attributes;

namespace UnityGameLib.Display {
	/// <summary>
	/// Sets the field of view for the attached Camera to match another camera every frame.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class CopyFieldOfView : MonoBehaviour {
		[SerializeField, Required, RuntimeLocked]
		protected Camera _fromCamera;

		protected Camera _toCamera;

		protected void Awake() {
			_toCamera = GetComponent<Camera>();
		}

		protected void LateUpdate() {
			if (_toCamera.fieldOfView != _fromCamera.fieldOfView) {
				_toCamera.fieldOfView = _fromCamera.fieldOfView;
			}
		}
	}
}

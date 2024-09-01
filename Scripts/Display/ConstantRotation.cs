using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// Rotates a GameObject at constant speed.
	/// </summary>
	public class ConstantRotation : MonoBehaviour {
		/// <summary>How fast to rotate along each axis.</summary>
		[Tooltip("How fast to rotate along each axis.")]
		public Vector3 degreesPerSecond = new Vector3();

		/// <summary>If enabled, this rotates relative to global world coordinates instead of local space.</summary>
		[Tooltip("If enabled, this rotates relative to global world coordinates instead of local space.")]
		public bool worldRelative = false;

		protected virtual void LateUpdate() {
			if (worldRelative) {
				transform.rotation *= Quaternion.Euler(degreesPerSecond * Time.deltaTime);
			} else {
				transform.localRotation *= Quaternion.Euler(degreesPerSecond * Time.deltaTime);
			}
		}
	}
}

using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Disables or destroys a GameObject immediately upon creation unless the DEBUG_ENABLED compiler flag is set.
	/// </summary>
	public sealed class DebugOnly : MonoBehaviour {
		[SerializeField, Tooltip("If enabled, the object is destroyed completely instead of just being set inactive.")]
		private bool _destroyObject = true;

		void Awake() {
			#if DEBUG_ENABLED
			gameObject.SetActive(true);
			#else
			gameObject.SetActive(false);
			if (_destroyObject) {
				Destroy(gameObject);
			}
			#endif
		}
	}
}

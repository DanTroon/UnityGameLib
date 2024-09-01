using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Prevents a GameObject from being destroyed automatically when changing scenes.
	/// </summary>
	public sealed class ScenePersistent : MonoBehaviour {
		void Awake() {
			DontDestroyOnLoad(gameObject);
		}
	}
}

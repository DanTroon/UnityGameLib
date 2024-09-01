using UnityEngine;

namespace UnityGameLib.Events {
	/// <summary>
	/// Triggers an event when this behavior's Start is called.
	/// </summary>
	public class ExecuteOnStart : MonoBehaviour {
		/// <summary>Triggers when this behavior's Start is called.</summary>
		public SimpleEvent onStart;

		protected virtual void Start() {
			onStart.Invoke();
		}
	}
}

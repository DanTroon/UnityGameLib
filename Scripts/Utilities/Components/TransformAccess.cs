using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Provides basic functions for manipulating a Transform in animation events or callbacks.
	/// </summary>
	public class TransformAccess : MonoBehaviour {
		/// <summary>
		/// Sets world-space position to match the <paramref name="goal"/>.
		/// </summary>
		/// <param name="goal">the Transform to copy</param>
		public void SetPosition(Transform goal) {
			transform.position = goal.position;
		}

		/// <summary>
		/// Sets world-space rotation to match the <paramref name="goal"/>.
		/// </summary>
		/// <param name="goal">the Transform to copy</param>
		public void SetRotation(Transform goal) {
			transform.rotation = goal.rotation;
		}

		/// <summary>
		/// Sets world-space position and rotation to match the <paramref name="goal"/>.
		/// </summary>
		/// <param name="goal">the Transform to copy</param>
		public void SetPositionAndRotation(Transform goal) {
			transform.SetPositionAndRotation(goal.position, goal.rotation);
		}

		/// <summary>
		/// Sets local scale to the same values as <paramref name="goal"/>.
		/// </summary>
		/// <param name="goal">the Transform to copy</param>
		public void SetLocalScale(Transform goal) {
			transform.localScale = goal.localScale;
		}
	}
}

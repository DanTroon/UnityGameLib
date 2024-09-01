using UnityEngine;

namespace UnityGameLib.Events {
	/// <summary>
	/// A convenience subclass for <see cref="TriggerHitTracker"/> that only tracks objects with the "Player" tag.
	/// </summary>
	public class PlayerHitTracker : TriggerHitTracker {
		protected virtual void Awake() {
			SetFilter(PlayerFilter);
		}

		/// <summary>
		/// Returns whether a Collider is tagged as a Player.
		/// </summary>
		/// <param name="other">The Collider to check</param>
		/// <returns><c>true</c> if <paramref name="other"/> is a Player, or <c>false</c> if not</returns>
		public virtual bool PlayerFilter(Collider other) {
			return other.tag == "Player";
		}
	}
}

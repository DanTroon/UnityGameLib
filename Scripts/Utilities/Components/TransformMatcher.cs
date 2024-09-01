using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Copies specific properties from another transform to this one each frame.
	/// All values are in world space.
	/// </summary>
	public class TransformMatcher : MonoBehaviour {
		[SerializeField, Required, Tooltip("The Transform to match.")]
		protected Transform _matchedTransform;

		/// Whether to match the X position of <see cref="matchedTransform"/>.
		public bool matchPositionX = true;
		/// Whether to match the Y position of <see cref="matchedTransform"/>.
		public bool matchPositionY = true;
		/// Whether to match the Z position of <see cref="matchedTransform"/>.
		public bool matchPositionZ = true;
		/// Whether to match the X rotation of <see cref="matchedTransform"/>.
		public bool matchRotationX = true;
		/// Whether to match the Y rotation of <see cref="matchedTransform"/>.
		public bool matchRotationY = true;
		/// Whether to match the Z rotation of <see cref="matchedTransform"/>.
		public bool matchRotationZ = true;

		/// <summary>The Transform to match.</summary>
		public Transform matchedTransform {
			get { return _matchedTransform; }
			set { _matchedTransform = value; }
		}

		protected virtual void LateUpdate() {
			if (!_matchedTransform)
				return;

			if (matchPositionX || matchPositionY || matchPositionZ) {
				Vector3 source = _matchedTransform.position;
				Vector3 result = transform.position;
				if (matchPositionX) result.x = source.x;
				if (matchPositionY) result.y = source.y;
				if (matchPositionZ) result.z = source.z;
				transform.position = result;
			}

			if (matchRotationX || matchRotationY || matchRotationZ) {
				Vector3 source = _matchedTransform.eulerAngles;
				Vector3 result = transform.eulerAngles;
				if (matchRotationX) result.x = source.x;
				if (matchRotationY) result.y = source.y;
				if (matchRotationZ) result.z = source.z;
				transform.eulerAngles = result;
			}
		}
	}
}

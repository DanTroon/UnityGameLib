using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// Makes an object constantly turn toward a target.
	/// </summary>
	public class LazyLookAt : MonoBehaviour {
		[SerializeField, Tooltip("The object's resting rotation.")]
		private Vector3 _baseRotationEulers = Vector3.zero;
		[SerializeField, Tooltip("If true, the object's current rotation when this component activates is used as the base.")]
		private bool _setBaseOnEnable = true;

		[SerializeField, Optional, Tooltip("The target to rotate toward.")]
		private Transform _followTarget;

		[SerializeField, Range(0f, 1f), Tooltip("The follow strength. 0 means no follow, and 1 means look directly at the target.")]
		private float _followStrength = 1f;

		[SerializeField, Range(0.00001f, 1f), Tooltip("The motion smoothing factor applied when the target moves. Lower values will lag farther behind the target's motion.")]
		private float _motionFactor = 1f;

		void OnEnable() {
			if (_setBaseOnEnable) {
				_baseRotationEulers = transform.eulerAngles;
			}
		}

		void Update() {
			if (!_followTarget)
				return;

			Quaternion goalRotation = Quaternion.Slerp(Quaternion.Euler(_baseRotationEulers), Quaternion.LookRotation(_followTarget.transform.position - transform.position), _followStrength);
			transform.rotation = Quaternion.Slerp(transform.rotation, goalRotation, _motionFactor);
		}
	}
}

using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// An oscillating positional shake effect for objects or cameras.
	/// </summary>
	public class Shake : MonoBehaviour {
		[SerializeField, Tooltip("The maximum offset of the object. Position will oscillate between this offset and its inverse.")]
		protected Vector3 _amplitude = new Vector3(1f, 0f, 0f);
		[SerializeField, Tooltip("The number of oscillations per second in this shake effect.")]
		protected float _frequency = 10f;
		[SerializeField, Tooltip("A multiplier for the amplitude on all axes.")]
		protected float _amplitudeMultiplier = 1f;
		[SerializeField, RuntimeLocked, Tooltip("If enabled, this is a constant shaking effect.  Otherwise it relies on impact events.")]
		protected bool _isConstant = false;
		[SerializeField, RuntimeLocked, Tooltip("If enabled, the shake applies to global transform coordinates instead of local.")]
		protected bool _useGlobalPosition = false;
		
		protected float _duration = 0f;
		protected float _interpolant = 0f;
		protected float _phase = 0f;
		protected Vector3 _previousOffset = Vector3.zero;

		protected bool _impactActive = false;

		protected virtual Vector3 position {
			get { return _useGlobalPosition ? transform.position : transform.localPosition; }
			set {
				if (_useGlobalPosition) {
					transform.position = value;
				} else {
					transform.localPosition = value;
				}
			}
		}

		/// <summary>The maximum offset of the object. The object will oscillate between this offset and its inverse.</summary>
		public Vector3 amplitude {
			get { return _amplitude; }
			set { _amplitude = value; }
		}

		/// <summary>The number of oscillations per second in this shake effect.</summary>
		public float frequency {
			get { return _frequency; }
			set { _frequency = value; }
		}

		/// <summary>A multiplier for the amplitude on all axes.</summary>
		public float amplitudeMultiplier {
			get { return _amplitudeMultiplier; }
			set { _amplitudeMultiplier = value; }
		}

		/// <summary>If true, this is a constant shaking effect.  Otherwise it only shakes in response to <see cref="Impact(float)"/>.</summary>
		/// <remarks>
		/// Setting this value to <c>true</c> will start a constant shake, interrupting any Impact shake in progress.
		/// Setting this value to <c>false</c> will stop the constant shake, if applicable.
		/// Calling <see cref="Impact(float)"/> will set this value to false in order to perform the requested impact shake.
		/// </remarks>
		public bool isConstant {
			get { return _isConstant; }
			set {
				if (value) {
					StartConstant();
				} else {
					EndConstant();
				}
			}
		}

		/// <summary>If enabled, this shake applies to global transform coordinates instead of local.</summary>
		public bool useGlobalPosition {
			get { return _useGlobalPosition; }
			set {
				if (_useGlobalPosition != value) {
					Reset();
					_useGlobalPosition = value;
				}
			}
		}

		/// <summary>
		/// Execute a shake effect that begins at the set amplitude and dissipates to nothing over <paramref name="duration"/>.
		/// </summary>
		/// <param name="duration">the time to dissipate from full amplitude to none</param>
		public virtual void Impact(float duration) {
			EndConstant();
			EndImpact();

			StartImpact(duration);
		}

		/// <summary>
		/// Immediately halt any active shake effects and return the object to its normal position.
		/// </summary>
		public virtual void StopAll() {
			EndConstant();
			EndImpact();
		}

		protected virtual void LateUpdate() {
			if (_isConstant) {
				UpdateConstant();
			} else if (_impactActive) {
				UpdateImpact();
			}
		}

		protected virtual void Reset() {
			position -= _previousOffset;
			_previousOffset = Vector3.zero;
			_phase = 0f;
			_interpolant = 0f;
		}

		protected virtual void StartConstant() {
			if (_isConstant)
				return;

			EndImpact();
			_isConstant = true;
		}

		protected virtual void EndConstant() {
			if (!_isConstant)
				return;

			Reset();
			_isConstant = false;
		}

		protected virtual void UpdateConstant() {
			_phase += Time.deltaTime * _frequency;

			Vector3 nextOffset = _amplitude * _amplitudeMultiplier * Mathf.Sin(2f * Mathf.PI * _frequency * _phase);
			position += nextOffset - _previousOffset;
			_previousOffset = nextOffset;
		}

		protected virtual void StartImpact(float duration) {
			_impactActive = true;
			_duration = duration;
		}

		protected virtual void EndImpact() {
			if (!_impactActive)
				return;

			Reset();
			_impactActive = false;
		}

		protected virtual void UpdateImpact() {
			_phase += Time.deltaTime * _frequency;
			_interpolant += Time.deltaTime / _duration;

			if (_interpolant >= 1f) {
				EndImpact();
			} else {
				Vector3 nextOffset = _amplitude * _amplitudeMultiplier * (1f - _interpolant) * Mathf.Sin(2f * Mathf.PI * _frequency * _phase);
				position += nextOffset - _previousOffset;
				_previousOffset = nextOffset;
			}
		}
	}
}

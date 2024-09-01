using UnityGameLib.Events;
using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Oscillates a value along a sine wave and invokes an event whenever the value changes.
	/// </summary>
	public class ValueOscillator : MonoBehaviour {
		[SerializeField, Tooltip("The number of wave cycles per second.")]
		protected float _frequency = 1f;
		[SerializeField, Tooltip("The amplitude of the wave.")]
		protected float _amplitude = 1f;
		[SerializeField, Tooltip("The vertical offset of the wave.")]
		protected float _offsetY = 0f;
		[SerializeField, Tooltip("The normalized time position within the wave.")]
		protected float _phase = 0f;
		[SerializeField, Tooltip("Triggered whenever the value is updated.")]
		protected FloatEvent _onValueChanged = new FloatEvent();
		
		protected float _previousPhase = 0f;
		protected float _previousValue = 0f;
		protected float _value = 0f;

		/// <summary>The number of wave cycles per second.</summary>
		public virtual float frequency {
			get { return _frequency; }
			set { _frequency = value; }
		}

		/// <summary>The amplitude of the wave.</summary>
		public virtual float amplitude {
			get { return _amplitude; }
			set { _amplitude = value; }
		}

		/// <summary>The vertical offset of the wave.</summary>
		public virtual float offsetY {
			get { return _offsetY; }
			set { _offsetY = value; }
		}

		/// <summary>The current time position within the wave, normalized against <see cref="frequency"/>.</summary>
		public virtual float phase {
			get { return _phase; }
			set { _phase = value; }
		}

		/// <summary>
		/// The current value of the wave function, given <see cref="phase"/>,
		/// <see cref="amplitude"/>, and <see cref="offsetY"/>.
		/// </summary>
		public virtual float value {
			get { return _value; }
		}

		/// <summary>The <see cref="phase"/> from the previous update.</summary>
		public virtual float previousPhase {
			get { return _previousPhase; }
		}

		/// <summary>The <see cref="value"/> from the previous update.</summary>
		public virtual float previousValue {
			get { return _previousValue; }
		}

		/// <summary>The change in <see cref="phase"/> since the previous update.</summary>
		public virtual float deltaPhase {
			get { return _phase - _previousPhase; }
		}

		/// <summary>The change in <see cref="value"/> since the previous update.</summary>
		public virtual float deltaValue {
			get { return _value - _previousValue; }
		}

		/// <summary>Triggered whenever <see cref="value"/> is updated.</summary>
		/// <seealso cref="UpdateValue"/>
		public virtual FloatEvent onValueChanged {
			get { return _onValueChanged; }
		}

		/// <summary>
		/// Refreshes <see cref="value"/> according to the current <see cref="phase"/> and <see cref="amplitude"/>.
		/// </summary>
		/// <remarks>
		/// This method is responsible for setting <see cref="value"/> and <see cref="previousValue"/>, as well as
		/// invoking <see cref="onValueChanged"/> if appropriate. This occurs automatically every frame, as long as the
		/// ValueOscillator component is enabled. After manually changing <see cref="phase"/> or <see cref="amplitude"/>,
		/// you can call this method directly to see the updated <see cref="value"/> without waiting for the next frame.
		/// </remarks>
		public virtual void UpdateValue() {
			_previousValue = _value;
			_value = _offsetY + _amplitude * Mathf.Sin(2f * Mathf.PI * _phase);

			if (_value != _previousValue) {
				_onValueChanged.Invoke(value);
			}
		}

		protected virtual void Update() {
			_previousPhase = _phase;
			_phase += Time.deltaTime * _frequency;

			if (_phase != _previousPhase) {
				UpdateValue();
			}
		}
	}
}

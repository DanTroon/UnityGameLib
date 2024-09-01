using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Events {
	/// <summary>
	/// A component that invokes events whenever its <see cref="isOn"/> value is toggled on or off.
	/// </summary>
	public class ToggleNotifier : MonoBehaviour {
		[SerializeField, RuntimeLocked, Tooltip("If true, an onActive or onInactive event fires on start to indicate the initial state.")]
		protected bool _notifyOnStart = true;
		
		[SerializeField, Tooltip("Triggered when 'Is On' changes.")]
		protected BooleanEvent _onToggle;
		/// <summary>Triggered when <see cref="isOn"/> changes.</summary>
		public BooleanEvent onToggle {
			get { return _onToggle; }
		}

		[SerializeField, Tooltip("Triggered when 'Is On' becomes true.")]
		protected SimpleEvent _onActive;
		/// <summary>Triggered when <see cref="isOn"/> becomes true.</summary>
		public SimpleEvent onActive {
			get { return _onActive; }
		}
		
		[SerializeField, Tooltip("Triggered when 'Is On' becomes false.")]
		protected SimpleEvent _onInactive;
		/// <summary>Triggered when <see cref="isOn"/> becomes false.</summary>
		public SimpleEvent onInactive {
			get { return _onInactive; }
		}

		[SerializeField, Tooltip("The current toggle state.")]
		protected bool _isOn = false;
		/// <summary>The current toggle state.</summary>
		public virtual bool isOn {
			get { return _isOn; }
			set {
				_isOn = value;
				CheckState();
			}
		}

		protected bool _started = false;
		protected bool _wasOn = false;

		protected virtual bool shouldNotify {
			get { return !_notifyOnStart || _started; }
		}

		protected virtual void Awake() {
			_wasOn = _isOn;
		}

		protected virtual void Start() {
			if (_notifyOnStart) {
				_wasOn = _isOn;
				NotifyCurrentState();
			}
			_started = true;
		}

		protected virtual void OnValidate() {
			CheckState();
		}

		protected virtual void CheckState() {
			if (_wasOn != _isOn) {
				_wasOn = _isOn;
				if (shouldNotify) {
					NotifyCurrentState();
				}
			}
		}

		/// <summary>
		/// Invokes the <see cref="onActive"/> or <see cref="onInactive"/> event, according to the current value of <see cref="isOn"/>
		/// </summary>
		public virtual void NotifyCurrentState() {
			_onToggle.Invoke(_isOn);

			if (_isOn) {
				_onActive.Invoke();
			} else {
				_onInactive.Invoke();
			}
		}
	}
}

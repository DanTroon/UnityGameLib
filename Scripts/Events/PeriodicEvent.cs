using UnityGameLib.Geometry;
using UnityEngine;

namespace UnityGameLib.Events {
	/// <summary>
	/// An scheduler that invokes an event at a set interval, with optional random time variance.
	/// </summary>
	public class PeriodicEvent : MonoBehaviour {
		[SerializeField, Tooltip("The time interval in seconds between executions.")]
		protected LinearRange _delayRangeSec;
		[SerializeField, Tooltip("The action to execute at each interval.")]
		protected SimpleEvent _action;

		protected float _nextActionTime = 0f;

		/// <summary>The range of random time intervals between executions, in seconds.</summary>
		public virtual LinearRange delayRangeSec {
			get { return _delayRangeSec; }
			set { _delayRangeSec = value; }
		}

		/// <summary>The time remaining, in seconds, until the next execution.</summary>
		public virtual float nextActionTime {
			get { return _nextActionTime; }
			set { _nextActionTime = value; }
		}

		/// <summary>The event to execute at each interval.</summary>
		public virtual SimpleEvent action {
			get { return _action; }
		}

		protected virtual void OnEnable() {
			_nextActionTime = _delayRangeSec.GetRandom();
		}

		protected virtual void Update() {
			_nextActionTime -= Time.deltaTime;
			if (_nextActionTime <= 0f) {
				_action.Invoke();
				_nextActionTime += _delayRangeSec.GetRandom();
			}
		}
		
		/// <summary>
		/// Immediately invokes <see cref="action"/> without affecting the normal period.
		/// </summary>
		public virtual void Execute() {
			_action.Invoke();
		}
	}
}

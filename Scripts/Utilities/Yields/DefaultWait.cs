using System;
using System.Collections;
using UnityEngine;

namespace UnityGameLib.Utilities.Yields {
	/// <summary>
	/// Waits for a set amount of time, ignoring any time that passes
	/// while <see cref="DefaultYield.evaluator"/> returns true.
	/// </summary>
	public class DefaultWait : IEnumerator {
		protected float _duration;
		protected float _timeRemaining;
		protected float _previousTimeCheck;

		/// <summary>
		/// Creates a new DefaultWait with the specified duration.
		/// </summary>
		/// <param name="durationSec">the amount of time to wait, in seconds</param>
		public DefaultWait(float durationSec) {
			_duration = durationSec;
			Reset();
		}

		public object Current {
			get { return null; }
		}

		public bool MoveNext() {
			float time = GetTime();
			float delta = time - _previousTimeCheck;
			_previousTimeCheck = time;

			if (DefaultYield.evaluator())
				return true;

			_timeRemaining -= delta;
			return _timeRemaining > 0f;
		}

		/// <summary>
		/// Resets the timer to its initial duration.
		/// </summary>
		public void Reset() {
			_timeRemaining = _duration;
			_previousTimeCheck = GetTime();
		}

		protected static float GetTime() {
			return Time.time;
		}
	}
}

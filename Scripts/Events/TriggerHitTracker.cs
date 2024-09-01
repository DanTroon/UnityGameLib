using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace UnityGameLib.Events {
	/// <summary>
	/// Tracks relevant objects that are currently touching the attached trigger collider
	/// and dispatches events whenever those objects enter or exit.
	/// </summary>
	public class TriggerHitTracker : MonoBehaviour {
		private List<Collider> _currentHits = new List<Collider>();
		private Func<Collider, bool> _hitFilter = null;
		[SerializeField]
		private bool _logHits = false;

		[Serializable]
		public class ColliderEvent : UnityEvent<Collider> { }

		/// <summary>Triggers when an object enters this trigger and satisfies the filter condition.</summary>
		public ColliderEvent onEnter;

		/// <summary>Triggers when a previously entered object exits the trigger.</summary>
		public ColliderEvent onExit;

		/// <summary>
		/// A list of all colliders currently being tracked.
		/// </summary>
		public virtual List<Collider> hits {
			get { return _currentHits; }
		}

		/// <summary>
		/// If enabled, debug log messages will indicate the objects that enter and exit.
		/// </summary>
		public virtual bool logHits {
			get { return _logHits; }
			set { _logHits = value; }
		}

		/// <summary>
		/// Sets a filter function that determines whether a given collider should be tracked.
		/// </summary>
		/// <param name="filter">A callback that takes a Collider and returns <c>true</c> if that collider should be tracked.</param>
		/// <param name="filterCurrentHits">If true, any currently tracked colliders that fail the filter condition are immediately removed.</param>
		public virtual void SetFilter(Func<Collider, bool> filter, bool filterCurrentHits = false) {
			_hitFilter = filter;

			if (filterCurrentHits && filter != null) {
				for (int i = _currentHits.Count - 1; i >= 0; --i) {
					if (!filter(_currentHits[i])) {
						_currentHits.RemoveAt(i);
					}
				}
			}
		}

		/// <summary>
		/// Checks whether the specified Collider is currently tracked.
		/// </summary>
		/// <param name="other">The Collider to check</param>
		/// <returns><c>true</c> if the object is currently tracked, or <c>false</c> if not</returns>
		public virtual bool IsHit(Collider other) {
			return _currentHits.Contains(other);
		}

		/// <summary>
		/// Checks whether the specified GameObject is currently tracked (via an attached Collider).
		/// </summary>
		/// <param name="other">The GameObject to check</param>
		/// <returns><c>true</c> if the object is currently tracked, or <c>false</c> if not</returns>
		public virtual bool IsHit(GameObject other) {
			for (int i = 0; i < _currentHits.Count; ++i) {
				if (_currentHits[i].gameObject == other) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks whether the specified component is currently tracked (via an attached Collider).
		/// </summary>
		/// <param name="other">The component to check</param>
		/// <returns><c>true</c> if the object is currently tracked, or <c>false</c> if not</returns>
		public virtual bool IsHit<T>(T other) where T : MonoBehaviour {
			for (int i = 0; i < _currentHits.Count; ++i) {
				if (_currentHits[i].GetComponent<T>() == other) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Immediately removes all tracked colliders.
		/// </summary>
		/// <remarks>Note that this will not fire <see cref="onExit"/> for the removed objects.</remarks>
		public virtual void ClearHits() {
			_currentHits.Clear();
		}

		protected virtual void OnEnter(Collider other) {
			if (_logHits) {
				Debug.Log(other.name + " entered " + name);
			}

			onEnter.Invoke(other);
		}

		protected virtual void OnExit(Collider other) {
			if (_logHits) {
				Debug.Log(other.name + " exited " + name);
			}

			onExit.Invoke(other);
		}

		protected virtual void OnDisable() {
			List<Collider> hits = new List<Collider>(_currentHits);
			ClearHits();
			
			foreach (Collider hit in hits) {
				OnExit(hit);
			}
		}

		protected virtual void OnTriggerEnter(Collider other) {
			if (_hitFilter != null && !_hitFilter(other))
				return;

			if (!_currentHits.Contains(other)) {
				_currentHits.Add(other);
				OnEnter(other);
			}
		}

		protected virtual void OnTriggerExit(Collider other) {
			bool found = _currentHits.Remove(other);

			if (found) {
				OnExit(other);
			}
		}
	}
}

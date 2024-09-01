using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityGameLib.Animation {
	/// <summary>
	/// Invokes callbacks whenever an Animator begins or ends a state or transition.
	/// </summary>
	/// <remarks>
	/// Use the RegisterOn... and UnRegisterOn... methods to add and remove callbacks.
	/// 
	/// If frequently adding and removing callbacks for the same states, use the static
	/// methods to generate and save a hash to use for future calls.
	/// 
	/// Based on SampleMecanimEventHandler.cs from http://blog.camposanto.com/
	/// </remarks>
	public class MecanimEventHandler : MonoBehaviour {
		public const string DEFAULT_LAYER_NAME = "Base Layer";
		public const string PATH_DELIMITER = ".";
		public const string TRANSITION_DELIMITER = " -> ";

		// The current transitions and states for each layer
		private AnimatorTransitionInfo[] previousTransitions;
		private AnimatorStateInfo[] previousStates;

		// Dictionaries that map Animator hashes to callbacks
		private Dictionary<int, Action> transitionBeginCallbackMap;
		private Dictionary<int, Action> transitionEndCallbackMap;
		private Dictionary<int, Action> stateBeginCallbackMap;
		private Dictionary<int, Action> stateEndCallbackMap;

		[SerializeField, Tooltip("The animator to watch. If none, will search in children.")]
		private Animator _animator;

		/// <summary>
		/// The animator this handler is watching.
		/// </summary>
		/// <value>The animator this handler is watching.</value>
		public Animator animator {
			get { return _animator; }
		}

		/// <summary>
		/// Initializes this handler.
		/// </summary>
		protected virtual void Awake() {
			//Look for an animator in gameObject or its descendants.
			if (_animator == null) {
				_animator = gameObject.GetComponentInChildren<Animator>();
			}

			//Initialize internal lists. 
			transitionBeginCallbackMap = new Dictionary<int, Action>();
			transitionEndCallbackMap = new Dictionary<int, Action>();
			stateBeginCallbackMap = new Dictionary<int, Action>();
			stateEndCallbackMap = new Dictionary<int, Action>();

			//Warn if no animator exists.
			if (_animator == null) {
				Debug.LogWarning("MecanimEventHandler has no animator. " + gameObject.name);
			} else {
				previousTransitions = new AnimatorTransitionInfo[_animator.layerCount];
				previousStates = new AnimatorStateInfo[_animator.layerCount];
			}
		}

		/// <summary>
		/// Checks for any transition or state changes and triggers the registered callbacks.
		/// </summary>
		protected virtual void Update() {
			if (_animator) {
				UpdateMecanimCallbacks();
			}
		}

		#region Utilities
		/// <summary>
		/// Generates the animator hash for a transition between two states.
		/// </summary>
		/// <returns>The animator hash representing the transition.</returns>
		/// <param name="fromStateName">The starting state for the transition.</param>
		/// <param name="toStateName">The ending state for the transition.</param>
		/// <param name="layerName">The name of the layer containing the transition.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the transition, in order from least to most specific.</param>
		public static int GetTransitionPathHash(string fromStateName, string toStateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			return Animator.StringToHash(GetTransitionPath(fromStateName, toStateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Generates the animator hash for a state.
		/// </summary>
		/// <returns>The animator hash representing the state.</returns>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="layerName">The name of the layer containing the state.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the state, in order from least to most specific.</param>
		public static int GetStatePathHash(string stateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			return Animator.StringToHash(GetStatePath(stateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Generates the full path string for a transition, given the names of its states and their containers.
		/// </summary>
		/// <returns>The full transition path string.</returns>
		/// <param name="fromStateName">The starting state for the transition.</param>
		/// <param name="toStateName">The ending state for the transition.</param>
		/// <param name="layerName">The name of the layer containing the transition.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the transition, in order from least to most specific.</param>
		public static string GetTransitionPath(string fromStateName, string toStateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			return GetStatePath(fromStateName, layerName, subControllerNames) + TRANSITION_DELIMITER + GetStatePath(toStateName, layerName, subControllerNames);
		}

		/// <summary>
		/// Generates the full path string for a state, given its name and the names of its containers.
		/// </summary>
		/// <returns>The full state path string.</returns>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="layerName">The name of the layer containing the state.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the state, in order from least to most specific.</param>
		public static string GetStatePath(string stateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			StringBuilder builder = new StringBuilder(layerName);
			for (int i = 0; i < subControllerNames.Length; ++i) {
				builder.Append(PATH_DELIMITER).Append(subControllerNames[i]);
			}
			builder.Append(PATH_DELIMITER).Append(stateName);

			return builder.ToString();
		}
		#endregion

		#region RegisterByHash
		/// <summary>
		/// Adds a callback to trigger when beginning the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="transitionPathHash">The animator hash representing the transition.</param>
		public void RegisterOnTransitionBegin(Action callback, int transitionPathHash) {
			RegisterCallback(transitionBeginCallbackMap, transitionPathHash, callback);
		}

		/// <summary>
		/// Removes a callback to trigger when beginning the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="transitionPathHash">The animator hash representing the transition.</param>
		public void UnRegisterOnTransitionBegin(Action callback, int transitionPathHash) {
			UnRegisterCallback(transitionBeginCallbackMap, transitionPathHash, callback);
		}

		/// <summary>
		/// Adds a callback to trigger when finishing the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="transitionPathHash">The animator hash representing the transition.</param>
		public void RegisterOnTransitionEnd(Action callback, int transitionPathHash) {
			RegisterCallback(transitionEndCallbackMap, transitionPathHash, callback);
		}

		/// <summary>
		/// Removes a callback to trigger when finishing the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="transitionPathHash">The animator hash representing the transition.</param>
		public void UnRegisterOnTransitionEnd(Action callback, int transitionPathHash) {
			UnRegisterCallback(transitionEndCallbackMap, transitionPathHash, callback);
		}

		/// <summary>
		/// Adds a callback to trigger when entering the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="statePathHash">The animator hash representing the state.</param>
		public void RegisterOnStateBegin(Action callback, int statePathHash) {
			RegisterCallback(stateBeginCallbackMap, statePathHash, callback);
		}

		/// <summary>
		/// Removes a callback to trigger when entering the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="statePathHash">The animator hash representing the state.</param>
		public void UnRegisterOnStateBegin(Action callback, int statePathHash) {
			UnRegisterCallback(stateBeginCallbackMap, statePathHash, callback);
		}

		/// <summary>
		/// Adds a callback to trigger when leaving the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="statePathHash">The animator hash representing the state.</param>
		public void RegisterOnStateEnd(Action callback, int statePathHash) {
			RegisterCallback(stateEndCallbackMap, statePathHash, callback);
		}

		/// <summary>
		/// Removes a callback to trigger when leaving the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="statePathHash">The animator hash representing the state.</param>
		public void UnRegisterOnStateEnd(Action callback, int statePathHash) {
			UnRegisterCallback(stateEndCallbackMap, statePathHash, callback);
		}
		#endregion

		#region RegisterByName
		/// <summary>
		/// Adds a callback to trigger when beginning the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="fromStateName">The starting state for the transition.</param>
		/// <param name="toStateName">The ending state for the transition.</param>
		/// <param name="layerName">The name of the layer containing the transition.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the transition, in order from least to most specific.</param>
		public void RegisterOnTransitionBegin(Action callback, string fromStateName, string toStateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			RegisterOnTransitionBegin(callback, GetTransitionPathHash(fromStateName, toStateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Removes a callback to trigger when beginning the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="fromStateName">The starting state for the transition.</param>
		/// <param name="toStateName">The ending state for the transition.</param>
		/// <param name="layerName">The name of the layer containing the transition.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the transition, in order from least to most specific.</param>
		public void UnRegisterOnTransitionBegin(Action callback, string fromStateName, string toStateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			UnRegisterOnTransitionBegin(callback, GetTransitionPathHash(fromStateName, toStateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Adds a callback to trigger when finishing the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="fromStateName">The starting state for the transition.</param>
		/// <param name="toStateName">The ending state for the transition.</param>
		/// <param name="layerName">The name of the layer containing the transition.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the transition, in order from least to most specific.</param>
		public void RegisterOnTransitionEnd(Action callback, string fromStateName, string toStateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			RegisterOnTransitionEnd(callback, GetTransitionPathHash(fromStateName, toStateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Removes a callback to trigger when finishing the specified transition.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="fromStateName">The starting state for the transition.</param>
		/// <param name="toStateName">The ending state for the transition.</param>
		/// <param name="layerName">The name of the layer containing the transition.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the transition, in order from least to most specific.</param>
		public void UnRegisterOnTransitionEnd(Action callback, string fromStateName, string toStateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			UnRegisterOnTransitionEnd(callback, GetTransitionPathHash(fromStateName, toStateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Adds a callback to trigger when entering the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="layerName">The name of the layer containing the state.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the state, in order from least to most specific.</param>
		public void RegisterOnStateBegin(Action callback, string stateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			RegisterOnStateBegin(callback, GetStatePathHash(stateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Removes a callback to trigger when entering the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="layerName">The name of the layer containing the state.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the state, in order from least to most specific.</param>
		public void UnRegisterOnStateBegin(Action callback, string stateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			UnRegisterOnStateBegin(callback, GetStatePathHash(stateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Adds a callback to trigger when leaving the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="layerName">The name of the layer containing the state.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the state, in order from least to most specific.</param>
		public void RegisterOnStateEnd(Action callback, string stateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			RegisterOnStateEnd(callback, GetStatePathHash(stateName, layerName, subControllerNames));
		}

		/// <summary>
		/// Removes a callback to trigger when leaving the specified state.
		/// </summary>
		/// <param name="callback">The callback to add.</param>
		/// <param name="stateName">The name of the state.</param>
		/// <param name="layerName">The name of the layer containing the state.</param>
		/// <param name="subControllerNames">If applicable, the name of each sub-state machine in the hierarchy containing the state, in order from least to most specific.</param>
		public void UnRegisterOnStateEnd(Action callback, string stateName, string layerName = DEFAULT_LAYER_NAME, params string[] subControllerNames) {
			UnRegisterOnStateEnd(callback, GetStatePathHash(stateName, layerName, subControllerNames));
		}
		#endregion

		private void UpdateMecanimCallbacks() {
			if (!_animator.isInitialized)
				return;

			for (int layerIndex = 0; layerIndex < _animator.layerCount; ++layerIndex) {
				// pull our current transition and state into temporary variables, since we may not need to do anything with them
				AnimatorTransitionInfo nextTransition = _animator.GetAnimatorTransitionInfo(layerIndex);
				AnimatorStateInfo nextState = _animator.GetCurrentAnimatorStateInfo(layerIndex);

				//Check for a transition change
				int previousTransitionHash = previousTransitions[layerIndex].fullPathHash;
				if (nextTransition.fullPathHash != previousTransitionHash) {
					//Debug.Log(string.Format("Changing transition from {0} to {1}", previousTransitionHash, nextTransition.fullPathHash));

					//Trigger callbacks for the transition change
					InvokeCallback(transitionEndCallbackMap, previousTransitionHash);
					InvokeCallback(transitionBeginCallbackMap, nextTransition.fullPathHash);

					//Save the current transition
					previousTransitions[layerIndex] = nextTransition;
				}

				//Check for a state change
				int previousStateHash = previousStates[layerIndex].fullPathHash;
				if (nextState.fullPathHash != previousStateHash) {
					//Debug.Log(string.Format("Changing state from {0} to {1}", previousStateHash, nextState.fullPathHash));

					//Trigger callbacks for the state change
					InvokeCallback(stateEndCallbackMap, previousStateHash);
					InvokeCallback(stateBeginCallbackMap, nextState.fullPathHash);

					//Save the current state
					previousStates[layerIndex] = nextState;
				}
			}
		}

		//Add a callback by hash
		private void RegisterCallback(Dictionary<int, Action> dictionary, int animHash, Action callback) {
			//Debug.Log("Adding callback for hash: " + animHash);

			if (dictionary.ContainsKey(animHash)) {
				dictionary[animHash] = dictionary[animHash] + callback;
			} else {
				dictionary.Add(animHash, callback);
			}
		}

		//Remove a callback by hash
		private void UnRegisterCallback(Dictionary<int, Action> dictionary, int animHash, Action callback) {
			//Debug.Log("Removing callback for hash: " + animHash);

			if (dictionary.ContainsKey(animHash)) {
				dictionary[animHash] = dictionary[animHash] - callback;
			}
		}

		//Invoke callbacks by hash if any exist
		private void InvokeCallback(Dictionary<int, Action> dictionary, int animHash) {
			Action callback;
			if (dictionary.TryGetValue(animHash, out callback) && callback != null) {
				callback();
			}
		}
	}
}

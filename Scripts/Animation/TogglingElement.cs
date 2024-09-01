using UnityEngine;
using System.Collections;
using System;
using UnityGameLib.Attributes;
using UnityGameLib.Events;

namespace UnityGameLib.Animation {
	/// <summary>
	/// Manages an Animator that transitions between On and Off states.
	/// </summary>
	/// <remarks>
	/// The Animations folder included in UnityGameLib contains an animator
	/// with the expected structure, titled "TogglingElement."
	/// You can create an Override Controller to replace the animations in
	/// the TogglingElement controller, or make a copy of it to edit the structure.
	/// 
	/// For simple fade-in and fade-out animations, the "TogglingElementSimple"
	/// and "TogglingMaterial" sample overrides are also provided.
	/// </remarks>
	public class TogglingElement : MecanimEventHandler {
		[Header("TogglingElement Animation")]
		[SerializeField, RuntimeLocked, Tooltip("The Animator layer containing On and Off states")]
		protected string _animLayer = "Base Layer";
		[SerializeField, RuntimeLocked, Tooltip("The name of the Off state in the Animator")]
		protected string _animStateOff = "Off";
		[SerializeField, RuntimeLocked, Tooltip("The name of the On state in the Animator")]
		protected string _animStateOn = "On";
		[SerializeField, RuntimeLocked, Tooltip("The name of the boolean Animator parameter that toggles the element on and off")]
		protected string _paramActive = "Active";
		[SerializeField, RuntimeLocked, Tooltip("The name of the boolean Animator parameter that determines whether transitions are animated")]
		protected string _paramAnimate = "Animate";
		[SerializeField, RuntimeLocked, Tooltip("A multiplier for the speed of the Animator")]
		protected float _animSpeed = 1f;

		protected int _animHashOff;
		protected int _animHashOn;

		protected bool _isOn = false;
		protected bool _transitioning = false;

		protected SimpleEvent _onActive = new SimpleEvent();
		protected SimpleEvent _onInactive = new SimpleEvent();

		/// <summary>
		/// Triggers when this element reaches its "on"/active state.
		/// </summary>
		public virtual SimpleEvent onActive {
			get { return _onActive; }
		}

		/// <summary>
		/// Triggers when this element reaches its "off"/inactive state.
		/// </summary>
		public virtual SimpleEvent onInactive {
			get { return _onInactive; }
		}

		/// <summary>
		/// Whether this element is currently toggled "on" (true) or "off" (false).
		/// This reflects the current target state, even if an animation to that state hasn't finished yet.
		/// Setting this property will animate to the specified state if not already in that state.
		/// </summary>
		public virtual bool isOn {
			get { return _isOn; }
			set { SetState(value); }
		}

		/// <summary>
		/// Whether this element is currently animating to its "on" or "off" state.
		/// Use the isOn property to determine the direction of the transition.
		/// </summary>
		public virtual bool isTransitioning {
			get { return _transitioning; }
		}

		protected override void Awake() {
			_animHashOff = GetStatePathHash(_animStateOff, _animLayer);
			_animHashOn = GetStatePathHash(_animStateOn, _animLayer);

			base.Awake();
		}

		protected virtual void Start() {
			animator.speed = _animSpeed;
		}

		protected virtual void OnEnable() {
			animator.SetBool(_paramAnimate, false);
			animator.SetBool(_paramActive, _isOn);
		}

		/// <summary>
		/// Starts an animated transition to the "on" state.
		/// </summary>
		/// <remarks>This is the same as calling <see cref="SetState(bool, bool)"/> with the arguments <c>(true, true)</c>.</remarks>
		public virtual void TransitionOn() {
			SetState(true, true);
		}

		/// <summary>
		/// Starts an animated transition to the "off" state.
		/// </summary>
		/// <remarks>This is the same as calling <see cref="SetState(bool, bool)"/> with the arguments <c>(false, true)</c>.</remarks>
		public virtual void TransitionOff() {
			SetState(false, true);
		}

		/// <summary>
		/// Starts an instantaneous transition to the "on" state.
		/// </summary>
		/// <remarks>This is the same as calling <see cref="SetState(bool, bool)"/> with the arguments <c>(true, false)</c>.</remarks>
		public virtual void Show() {
			SetState(true, false);
		}
		/// <summary>
		/// Starts an instantaneous transition to the "off" state.
		/// </summary>
		/// <remarks>This is the same as calling <see cref="SetState(bool, bool)"/> with the arguments <c>(false, false)</c>.</remarks>
		public virtual void Hide() {
			SetState(false, false);
		}

		/// <summary>
		/// Transitions to the "on" or "off" state.
		/// </summary>
		/// <remarks>
		/// If already in that state or transitioning to it,
		/// nothing happens (including event callbacks), and this method returns false.
		/// 
		/// Note the state change is asynchronous even if not animated. Use onActive or onInactive to know when it finishes.
		/// </remarks>
		/// <param name="on">Whether to transition to the "on" state (true) or "off" (false)</param>
		/// <param name="animate">Whether to animate the transition (true) or jump directly to the end state (false)</param>
		/// <returns>true if the call successfully started a transition, otherwise false</returns>
		public virtual bool SetState(bool on, bool animate = true) {
			if (isOn == on) {
				return false;
			}
			
			_isOn = on;

			UnRegisterOnStateBegin(OnTransitionDone, _animHashOff);
			UnRegisterOnStateBegin(OnTransitionDone, _animHashOn);

			StartTransition();

			RegisterOnStateBegin(OnTransitionDone, on ? _animHashOn : _animHashOff);
			animator.SetBool(_paramAnimate, animate);
			animator.SetBool(_paramActive, on);

			return true;
		}

		/// <summary>
		/// Removes all external delegates for transition states.
		/// </summary>
		public virtual void ClearDelegates() {
			_onActive.RemoveAllListeners();
			_onInactive.RemoveAllListeners();
		}

		/// <summary>
		/// Called immediately before either transition starts.
		/// </summary>
		protected virtual void StartTransition() {
			_transitioning = true;
		}

		/// <summary>
		/// Called immediately after either transition ends (before events are notified).
		/// </summary>
		protected virtual void EndTransition() {
			_transitioning = false;
		}

		/// <summary>
		/// Handles finishing any transition.
		/// </summary>
		protected virtual void OnTransitionDone() {
			if (isOn) {
				TransitionOn_Done();
			} else {
				TransitionOff_Done();
			}
		}

		/// <summary>
		/// Handles finishing a transition to "on."
		/// </summary>
		protected virtual void TransitionOn_Done() {
			UnRegisterOnStateBegin(OnTransitionDone, _animHashOn);
			EndTransition();
			_onActive.Invoke();
		}

		/// <summary>
		/// Handles finishing a transition to "off."
		/// </summary>
		protected virtual void TransitionOff_Done() {
			UnRegisterOnStateBegin(OnTransitionDone, _animHashOff);
			EndTransition();
			_onInactive.Invoke();
		}
	}
}

using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Animation {
	/// <summary>
	/// Manages an Animator that plays a one-shot animation.
	/// </summary>
	/// <remarks>
	/// The animator is expected to begin in an "Off" state and
	/// return to the "Off" state again when the animation ends.
	/// </remarks>
	public class OneshotDisplay : MecanimEventHandler {
		#region Fields
		[Header("Oneshot Animation")]
		[SerializeField, RuntimeLocked, Tooltip("The Animator layer containing the Off state")]
		protected string _animLayer = "Base Layer";
		[SerializeField, RuntimeLocked, Tooltip("The name of the Off state in the Animator")]
		protected string _animStateOff = "Off";
		[SerializeField, RuntimeLocked, Tooltip("The name of the Animator trigger parameter that fires the oneshot animation")]
		protected string _animTriggerParam = "Animate";
		[SerializeField, RuntimeLocked, Tooltip("If enabled, calling Animate() while the animation is already playing will trigger it again. Otherwise the call is ignored.")]
		protected bool _canRestart = false;
		#endregion

		protected int _animHashOff;
		protected bool _animating = false;

		/// <summary>
		/// If enabled, calling Animate() while the animation is already playing will trigger it again. Otherwise the call is ignored.
		/// </summary>
		public bool canRestart {
			get { return _canRestart; }
			set { _canRestart = value; }
		}

		protected override void Awake() {
			_animHashOff = GetStatePathHash(_animStateOff, _animLayer);
			base.Awake();
		}

		/// <summary>
		/// Play the animation.
		/// </summary>
		public virtual void Animate() {
			if (_animating && !_canRestart)
				return;

			_animating = true;
			UnRegisterOnStateBegin(OnAnimationComplete, _animHashOff);
			animator.SetTrigger(_animTriggerParam);
			RegisterOnStateBegin(OnAnimationComplete, _animHashOff);
		}

		protected virtual void OnAnimationComplete() {
			UnRegisterOnStateBegin(OnAnimationComplete, _animHashOff);
			_animating = false;
		}
	}
}

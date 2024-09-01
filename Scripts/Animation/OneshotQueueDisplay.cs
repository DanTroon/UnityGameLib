using UnityGameLib.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameLib.Animation {
	/// <summary>
	/// Abstract class that manages an Animator to queue one-shot animations with dynamic content.
	/// </summary>
	/// <remarks>
	/// The animator is expected to begin in an "Off" state and
	/// return to the "Off" state again when the animation ends.
	/// 
	/// Dynamic content is passed as an argument to <see cref="Show(T)"/>.
	/// </remarks>
	/// <typeparam name="T">The data type that defines any dynamic content</typeparam>
	public abstract class OneshotQueueDisplay<T> : MecanimEventHandler {
		[Header("Oneshot Animation")]
		[SerializeField, RuntimeLocked, Tooltip("The Animator layer containing the Off state")]
		protected string _animLayer = "Base Layer";
		[SerializeField, RuntimeLocked, Tooltip("The name of the Off state in the Animator")]
		protected string _animStateOff = "Off";
		[SerializeField, RuntimeLocked, Tooltip("The name of the Animator trigger parameter that fires the oneshot animation")]
		protected string _animTriggerParam = "Animate";

		[SerializeField, RuntimeLocked, Tooltip("Additional queued items will not display as long as this is set.")]
		protected bool _isQueuePaused = false;

		protected int _animHashOff;

		protected List<T> _queue = new List<T>();
		protected bool _animating = false;

		/// <summary>
		/// While paused, queued animations will not start playing.
		/// </summary>
		/// <remarks>
		/// If an animation is already playing when <c>paused</c> is set to <c>true</c>,
		/// that animation will still finish, but no new animations will play until
		/// <c>paused</c> is <c>false</c>.
		/// </remarks>
		/// <seealso cref="PauseQueue"/>
		/// <seealso cref="ResumeQueue"/>
		public bool paused {
			get { return _isQueuePaused; }
			set {
				if (value) {
					PauseQueue();
				} else {
					ResumeQueue();
				}
			}
		}

		/// <summary>
		/// Plays an animation using the given content, or queues it if one is already playing.
		/// </summary>
		/// <param name="data">An object that provides the content to display</param>
		public virtual void Show(T data) {
			_queue.Add(data);
			ShowNext();
		}

		/// <summary>
		/// Prevents any queued animations from starting.
		/// </summary>
		/// <remarks>This is the same as setting <see cref="paused"/> to <c>true</c>.</remarks>
		public virtual void PauseQueue() {
			_isQueuePaused = true;
		}

		/// <summary>
		/// Allows queued animations to start and immediately plays the next one if applicable.
		/// </summary>
		/// <remarks>This is the same as setting <see cref="paused"/> to <c>false</c>.</remarks>
		public virtual void ResumeQueue() {
			_isQueuePaused = false;
			ShowNext();
		}

		protected override void Awake() {
			_animHashOff = GetStatePathHash(_animStateOff, _animLayer);
			base.Awake();
		}

		/// <summary>
		/// Implement this in a subclass to populate the display with the specified content.
		/// </summary>
		/// <param name="data">An object that provides the content to display</param>
		protected abstract void SetContent(T data);

		protected virtual void ShowNext() {
			if (_isQueuePaused || _animating || _queue.Count == 0)
				return;

			T nextContent = _queue[0];
			_queue.RemoveAt(0);
			SetContent(nextContent);

			_animating = true;
			animator.SetTrigger(_animTriggerParam);
			RegisterOnStateBegin(OnAnimationComplete, _animHashOff);
		}

		protected virtual void OnAnimationComplete() {
			UnRegisterOnStateBegin(OnAnimationComplete, _animHashOff);
			_animating = false;

			ShowNext();
		}
	}
}

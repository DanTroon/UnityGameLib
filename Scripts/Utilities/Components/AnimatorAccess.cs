using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Provides access to a single Animator parameter for use in animation events or callbacks.
	/// </summary>
	/// <remarks>
	/// Attach this component to a GameObject to enable the included functions.  If you need to
	/// access multiple parameters or Animators, create additional GameObjects with this component.
	/// </remarks>
	public class AnimatorAccess : MonoBehaviour {
		[SerializeField, Optional, Tooltip("The Animator to control. If empty, the Animator attached to the same game object is used.")]
		protected Animator _animator;
		[SerializeField, Required, Tooltip("The parameter name to access.")]
		protected string _parameterName;

		/// <summary>The Animator to access.</summary>
		public virtual Animator animator {
			get { return _animator; }
			set { _animator = value; }
		}

		/// <summary>The parameter to access within <see cref="animator"/>.</summary>
		public virtual string parameterName {
			get { return _parameterName; }
			set { _parameterName = value; }
		}

		/// <summary>The value of the parameter as a float.</summary>
		public virtual float floatValue {
			get { return _animator.GetFloat(_parameterName); }
			set { _animator.SetFloat(_parameterName, value); }
		}

		/// <summary>The value of the parameter as an integer.</summary>
		public virtual int intValue {
			get { return _animator.GetInteger(_parameterName); }
			set { _animator.SetInteger(_parameterName, value); }
		}

		/// <summary>The value of the parameter as a boolean.</summary>
		public virtual bool boolValue {
			get { return _animator.GetBool(_parameterName); }
			set { _animator.SetBool(_parameterName, value); }
		}

		/// <summary>
		/// Trigger the parameter.
		/// </summary>
		public virtual void Trigger() {
			_animator.SetTrigger(_parameterName);
		}

		protected virtual void Awake() {
			if (!_animator) {
				_animator = GetComponent<Animator>();
			}
		}
	}
}

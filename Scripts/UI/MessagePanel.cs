using UnityGameLib.Animation;
using UnityGameLib.Audio;
using UnityGameLib.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameLib.UI {
	/// <summary>
	/// A TogglingElement with one or more buttons that simply turn off the element.
	/// </summary>
	public class MessagePanel : TogglingElement {
		[SerializeField, Tooltip("The sound ID to play when a Close Button is clicked, if any.")]
		protected AssetRef _closeClickSound;
		[SerializeField, Tooltip("If enabled, this panel destroys itself after transitioning off.")]
		protected bool _destroyWhenInactive = false;
		[SerializeField, Tooltip("A list of Buttons that should close the panel when clicked.")]
		protected List<Button> _closeButtons;

		/// <summary>
		/// If enabled, this panel destroys itself after transitioning off.
		/// </summary>
		public bool destroyWhenInactive {
			get { return _destroyWhenInactive; }
			set { _destroyWhenInactive = value; }
		}

		/// <summary>The first close button for this panel.</summary>
		public Button closeButton {
			get { return _closeButtons.Count > 0 ? _closeButtons[0] : null; }
		}

		/// <summary>All close buttons associated with this panel.</summary>
		public List<Button> closeButtons {
			get { return _closeButtons; }
		}

		protected override void TransitionOn_Done() {
			base.TransitionOn_Done();
			AddListeners();
		}

		protected override void TransitionOff_Done() {
			base.TransitionOff_Done();
			if (_destroyWhenInactive) {
				Destroy(gameObject);
			}
		}

		public override bool SetState(bool on, bool animate = true) {
			if (!on) {
				RemoveListeners();
			}
			return base.SetState(on, animate);
		}

		protected void AddListeners() {
			foreach (Button button in _closeButtons) {
				button.onClick.AddListener(CloseButton_OnClick);
			}
		}

		protected void RemoveListeners() {
			foreach (Button button in _closeButtons) {
				button.onClick.RemoveListener(CloseButton_OnClick);
			}
		}

		protected void CloseButton_OnClick() {
			if (!_closeClickSound.isEmpty) {
				SoundManager.instance.PlayOneshot(_closeClickSound.resourcePath, _closeClickSound.bundleName);
			}
			TransitionOff();
		}
	}
}

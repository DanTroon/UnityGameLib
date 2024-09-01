using UnityGameLib.Animation;
using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.UI {
	/// <summary>
	/// Displays a prompt whenever the current screen rotation is invalid according to the Screen.autorotate settings.
	/// </summary>
	public class ScreenRotationPrompt : MonoBehaviour {
		[SerializeField, Required, Tooltip("The element to toggle on whenever the current screen rotation is invalid.")]
		private TogglingElement _promptDisplay;
		[SerializeField, Optional, Tooltip("An optional animator that plays only while the prompt is active.")]
		private Animator _rotationAnim;
		[SerializeField, Tooltip("The boolean parameter on Rotation Anim that toggles between animating and idle.")]
		private string _rotationToggleParam = "Animate";

		protected virtual void Update() {
			if (!Application.isMobilePlatform)
				return;

			bool isPortrait = Screen.width < Screen.height;
			bool needsRotate = (isPortrait && !Screen.autorotateToPortrait) || (!isPortrait && !Screen.autorotateToLandscapeLeft);

			_promptDisplay.SetState(needsRotate);
			if (_rotationAnim) {
				_rotationAnim.SetBool(_rotationToggleParam, needsRotate);
			}
		}
	}
}

using UnityEngine;

namespace UnityGameLib.UI {
	/// <summary>
	/// An abstract class for displays that represent a percentage or fraction (from 0 to 1).
	/// </summary>
	public abstract class FillBar : MonoBehaviour {
		[SerializeField, Range(0f, 1f), Tooltip("The initial filled portion of the bar.")]
		protected float _fillAmount;

		/// <summary>
		/// The filled portion of the bar, between 0 and 1. The display animates to the new value when set this way.
		/// </summary>
		public virtual float fillAmount {
			get { return _fillAmount; }
			set { SetFill(value, true); }
		}

		/// <summary>
		/// Sets the filled portion of the bar, between 0 and 1.
		/// </summary>
		/// <param name="percent">The new fill amount between 0 and 1, inclusive</param>
		/// <param name="animate">Whether to animate the transition (true) or instantly set the display (false)</param>
		public virtual void SetFill(float percent, bool animate = true) {
			float newValue = Mathf.Clamp01(percent);

			if (_fillAmount != newValue) {
				float oldValue = _fillAmount;
				_fillAmount = newValue;

				if (animate) {
					ModifyFillDisplay(oldValue, newValue);
				} else {
					SetFillDisplay(newValue);
				}
			}
		}

		protected virtual void Awake() {
			SetFillDisplay(_fillAmount);
		}

		/// <summary>
		/// Directly sets the visual fill to the specified fraction.
		/// </summary>
		/// <param name="percent">The fraction of the bar to fill</param>
		protected abstract void SetFillDisplay(float percent);

		/// <summary>
		/// Moves the visual fill to the specified fraction, interpolating as needed.
		/// </summary>
		/// <param name="oldPercent">The previous filled fraction of the bar</param>
		/// <param name="newPercent">The target filled fraction of the bar</param>
		protected abstract void ModifyFillDisplay(float oldPercent, float newPercent);
	}
}

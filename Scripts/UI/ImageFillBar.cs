using UnityEngine;
using UnityEngine.UI;

namespace UnityGameLib.UI {
	/// <summary>
	/// A <see cref="FillBar"/> that displays its fill percentage using an Image with the "Filled" type.
	/// </summary>
	public class ImageFillBar : FillBar {
		[SerializeField, Tooltip("The main image to fill.")]
		protected Image _fillDisplay;
		[SerializeField, Tooltip("Optional display of the target fill amount while animating.")]
		protected Image _fillDeltaDisplay;
		[SerializeField, Tooltip("How long it takes to animate from one amount to another in seconds.")]
		protected float _animationTime = 1f;
		[SerializeField, Tooltip("How long in seconds to wait before animating a change.")]
		protected float _animationDelay = 0f;
		[SerializeField, Tooltip("If enabled, the animation always takes the same amount of time. If disabled, Animation Time is the time to animate the entire bar.")]
		protected bool _isAnimTimeConstant = false;

		protected bool _animating = false;
		protected float _currentAnimStartValue = 0f;
		protected float _currentAnimEndValue = 0f;
		protected float _currentAnimElapsed = 0f;
		protected float _currentAnimDuration = 0f;

		protected virtual float GetCurrentInterpolant() {
			return (_currentAnimElapsed - _animationDelay) / (_currentAnimDuration - _animationDelay);
		}

		protected virtual void LateUpdate() {
			if (_animating) {
				_currentAnimElapsed += Time.deltaTime;
				if (_currentAnimElapsed > _animationDelay) {
					float interpolant = GetCurrentInterpolant();
					if (interpolant >= 1f) {
						SetFillDisplay(_currentAnimEndValue);
					} else if (_fillDeltaDisplay && _currentAnimEndValue < _currentAnimStartValue) {
						_fillDeltaDisplay.fillAmount = Mathf.Lerp(_currentAnimStartValue, _currentAnimEndValue, interpolant);
					} else {
						_fillDisplay.fillAmount = Mathf.Lerp(_currentAnimStartValue, _currentAnimEndValue, interpolant);
					}
				}
			} else if (_fillDisplay.fillAmount != _fillAmount) {
				SetFillDisplay(_fillAmount);
			}
		}

		protected override void ModifyFillDisplay(float oldPercent, float newPercent) {
			//Override old value with the current position if this is interrupting an animation
			if (_animating) {
				oldPercent = Mathf.Lerp(_currentAnimStartValue, _currentAnimEndValue, GetCurrentInterpolant());
				if (oldPercent == newPercent) {
					SetFillDisplay(newPercent);
					return;
				}
			} else if (_animationTime <= 0f && _animationDelay <= 0f) {
				SetFillDisplay(newPercent);
				return;
			}

			float delta = newPercent - oldPercent;
			float animTime = _isAnimTimeConstant ? _animationTime : _animationTime * Mathf.Abs(delta);

			if (delta > 0f) {
				if (_fillDeltaDisplay) {
					_fillDeltaDisplay.fillAmount = newPercent;
				}
				_fillDisplay.fillAmount = oldPercent;
			} else {
				if (_fillDeltaDisplay) {
					_fillDeltaDisplay.fillAmount = oldPercent;
					_fillDisplay.fillAmount = newPercent;
				} else {
					_fillDisplay.fillAmount = oldPercent;
				}
			}

			_currentAnimStartValue = oldPercent;
			_currentAnimEndValue = newPercent;
			_currentAnimDuration = animTime;

			if (_animating) {
				if (_currentAnimElapsed >= _animationDelay) {
					_currentAnimElapsed = _animationDelay;
				}
			} else {
				_currentAnimElapsed = 0f;
				_animating = true;
			}
		}

		protected override void SetFillDisplay(float percent) {
			_fillDisplay.fillAmount = percent;
			if (_fillDeltaDisplay)
				_fillDeltaDisplay.fillAmount = 0f;

			ClearAnimation();
		}

		protected virtual void ClearAnimation() {
			_currentAnimStartValue = 0f;
			_currentAnimEndValue = 0f;
			_currentAnimDuration = 0f;
			_currentAnimElapsed = 0f;
			_animating = false;
		}
	}
}

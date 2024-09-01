using UnityGameLib.Attributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameLib.UI {
	/// <summary>
	/// Splits an InputField's text display across multiple <see cref="InputFieldSplitterFragment"/> elements.
	/// </summary>
	/// <remarks>
	/// This allows a user to type into one field while the resulting display is formatted in segments,
	/// such as in a date, serial number, credit card, or phone number.
	/// </remarks>
	public class InputFieldSplitter : MonoBehaviour {
		[SerializeField, Required, Tooltip("The field that handles user input")]
		protected InputField _input;
		[SerializeField, Tooltip("The fragments that display the split text")]
		protected List<InputFieldSplitterFragment> _fragments;

		protected virtual void OnEnable() {
			RefreshFragments();
			_input.onValueChanged.RemoveListener(Input_OnValueChanged);
			_input.onValueChanged.AddListener(Input_OnValueChanged);
		}

		protected virtual void OnDisable() {
			_input.onValueChanged.RemoveListener(Input_OnValueChanged);
		}

		protected virtual void RefreshFragments() {
			string value = _input.text;
			int charIndex = 0;
			int charsRemaining = value.Length;

			foreach (InputFieldSplitterFragment fragment in _fragments) {
				if (charsRemaining >= fragment.characterCount) {
					fragment.text = value.Substring(charIndex, fragment.characterCount);
					fragment.complete = true;
					charsRemaining -= fragment.characterCount;
					charIndex += fragment.characterCount;
				} else if (charsRemaining > 0) {
					fragment.text = value.Substring(charIndex);
					fragment.complete = false;
					charsRemaining = 0;
				} else {
					fragment.text = "";
					fragment.complete = false;
				}
			}
		}

		protected virtual void Input_OnValueChanged(string value) {
			RefreshFragments();
		}
	}
}

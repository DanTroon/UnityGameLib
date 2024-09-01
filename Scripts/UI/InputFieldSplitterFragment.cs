using UnityGameLib.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameLib.UI {
	/// <summary>
	/// A display fragment for one segment of <see cref="InputFieldSplitter"/>.
	/// </summary>
	[Serializable]
	public class InputFieldSplitterFragment {
		[SerializeField, Optional, Tooltip("The Text component that holds this fragment's characters")]
		protected Text _textDisplay;
		[SerializeField, Optional, Tooltip("An optional GameObject to display only when this fragment is full")]
		protected GameObject _completionDisplay;
		[SerializeField, Range(1, 1000), Tooltip("The maximum number of characters this fragment can hold")]
		protected int _characterCount = 1;

		/// <summary>
		/// The text displayed in this fragment.
		/// </summary>
		public string text {
			set {
				if (_textDisplay) {
					_textDisplay.text = value;
				}
			}
		}

		/// <summary>
		/// Sets whether this fragment is full and toggles its completion display, if applicable.
		/// </summary>
		public bool complete {
			set {
				if (_completionDisplay) {
					_completionDisplay.SetActive(value);
				}
			}
		}

		/// <summary>
		/// The number of characters to enter in this fragment before it is <see cref="complete"/>.
		/// </summary>
		public int characterCount {
			get { return _characterCount; }
			set { _characterCount = value; }
		}
	}
}

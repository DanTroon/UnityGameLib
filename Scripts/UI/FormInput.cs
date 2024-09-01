using UnityGameLib.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityGameLib.Attributes;

namespace UnityGameLib.UI {
	/// <summary>
	/// Enables form-like keyboard navigation for a list of Selectable UI controls, and optionally names the fields for use in web requests.
	/// </summary>
	/// <remarks>
	/// This component attaches to a CanvasGroup. Keyboard commands will only function while the associated CanvasGroup is interactable.
	/// 
	/// Keyboard features include:
	/// Tab = Select the next control in the list.
	/// Shift+Tab = Select the previous control in the list.
	/// Enter/Return = Click <see cref="submitButton"/>, if available.
	/// </remarks>
	[RequireComponent(typeof(CanvasGroup))]
	public class FormInput : MonoBehaviour {
		[SerializeField, Tooltip("The selectable form elements, in their tab order.")]
		protected List<Selectable> _controls;
		[SerializeField, Optional, Tooltip("An optional button to submit the form.")]
		protected Button _submitButton;
		[SerializeField, Tooltip("If enabled, Tab and Shift+Tab can be used to navigate fields.")]
		protected bool _tabEnabled = true;

		protected int _lastSelectionIndex = 0;
		protected CanvasGroup _canvasGroup;

		/// <summary>The CanvasGroup used to determine whether keyboard input should be active.</summary>
		public CanvasGroup canvasGroup {
			get { return _canvasGroup; }
		}

		/// <summary>The list of selectable controls in this form, in tab order.</summary>
		public List<Selectable> controls {
			get { return _controls; }
		}

		/// <summary>If enabled, Tab and Shift+Tab can be used to navigate fields.</summary>
		public bool tabEnabled {
			get { return _tabEnabled; }
			set { _tabEnabled = value; }
		}

		/// <summary>An optional button mapped to the Enter key.</summary>
		public Button submitButton {
			get { return _submitButton; }
			set { _submitButton = value; }
		}

		/// <summary>Whether this form will currently respond to any keyboard commands.</summary>
		public bool keyCommandsAvailable {
			get { return _canvasGroup.interactable && (_submitButton || _tabEnabled); }
		}

		/// <summary>
		/// Returns the index of the currently selected control, or -1 if nothing in this form is selected.
		/// </summary>
		/// <returns>the index of the currently selected control, or -1 if none</returns>
		public virtual int GetSelectionIndex() {
			for (int i = 0, count = _controls.Count; i < count; ++i) {
				if (EventSystem.current.currentSelectedGameObject == _controls[i].gameObject) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Selects the first field, or if applicable, the most recent field that was selected by Tab-key navigation.
		/// </summary>
		public virtual void Select() {
			if (_lastSelectionIndex >= _controls.Count) {
				_lastSelectionIndex = 0;
			}

			if (_lastSelectionIndex >= 0 && _lastSelectionIndex < _controls.Count) {
				_controls[_lastSelectionIndex].Select();
			}
		}

		/// <summary>
		/// Returns a dictionary of field names and their values as strings,
		/// using <see cref="InterpretField(Selectable)"/> to interpret the value of each field.
		/// </summary>
		/// <param name="fieldNames">the name of each field, in the order of <see cref="controls"/></param>
		/// <returns>a dictionary of field names and their values as strings</returns>
		public virtual Dictionary<string, string> GetFieldValues(IList<string> fieldNames) {
			return GetFieldValues(fieldNames, InterpretField);
		}

		/// <summary>
		/// Returns a dictionary of field names and their values as strings,
		/// using <paramref name="interpreter"/> to interpret the value of each field.
		/// </summary>
		/// <param name="fieldNames">the name of each field, in the order of <see cref="controls"/></param>
		/// <param name="interpreter">a function that takes each UI field and returns a string representing its value</param>
		/// <returns>a dictionary of field names and their values as strings</returns>
		public virtual Dictionary<string, string> GetFieldValues(IList<string> fieldNames, Func<Selectable, string> interpreter) {
			if (_controls.Count != fieldNames.Count)
				throw new UnityException(string.Format("Field name count ({0}) does not match the number of controls ({1})", fieldNames.Count, _controls.Count));

			Dictionary<string, string> result = new Dictionary<string, string>();
			for (int i = 0, count = _controls.Count; i < count; ++i) {
				string value = interpreter(_controls[i]);
				if (value != null) {
					result.Add(fieldNames[i], value);
				}
			}

			return result;
		}

		protected virtual void Awake() {
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		protected virtual void Update() {
			if (!_canvasGroup.interactable)
				return;

			if (_tabEnabled && Input.GetKeyDown(KeyCode.Tab)) {
				int selectionIndex = GetSelectionIndex();
				if (selectionIndex != -1) {
					bool invert = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
					DeltaSelect(selectionIndex, invert ? -1 : 1);
				}
			} else if (_submitButton && _submitButton.IsInteractable() && (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))) {
				Submit();
			}
		}

		protected virtual void DeltaSelect(int startIndex, int deltaStep) {
			if (deltaStep == 0 || Mathf.Abs(deltaStep) == _controls.Count)
				return;

			int i = startIndex;
			do {
				i = MathUtilities.WrapValue(i + deltaStep, 0, _controls.Count);
			} while (i != startIndex && !_controls[i].IsInteractable());

			_lastSelectionIndex = i;
			_controls[i].Select();
		}

		protected virtual void Submit() {
			_submitButton.onClick.Invoke();
		}

		/// <summary>
		/// Returns the value of a Selectable field as a string. Toggles return 0 or 1, and Dropdowns return an index.
		/// </summary>
		/// <param name="input">the Selectable form field to interpret</param>
		/// <returns>the value of the field as a string</returns>
		public static string InterpretField(Selectable input) {
			if (input is InputField)
				return ((InputField) input).text;
			if (input is Toggle)
				return ((Toggle) input).isOn ? "1" : "0";
			if (input is Slider)
				return ((Slider) input).value.ToString();
			if (input is Dropdown)
				return ((Dropdown) input).value.ToString();

			return null;
		}
	}
}

using UnityGameLib.Events;
using UnityGameLib.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityGameLib.Attributes;

namespace UnityGameLib.UI {
	/// <summary>
	/// A UI control that allows the user to select a value by cycling through a series of options in order.
	/// </summary>
	public class Spinner : MonoBehaviour {
		[SerializeField, Required, RuntimeLocked, Tooltip("The mount containing the list items. The selected item appears at (0,0,0) in this holder. The list is populated with this holder's children.")]
		protected Transform _selectionHolder;
		[SerializeField, Tooltip("The visual spacing between list items within the Selection Holder")]
		protected Vector3 _itemSpacing = new Vector3(1f, 0f, 0f);
		[SerializeField, Tooltip("The number of items to render in each direction from the selection position. Can be a decimal number to handle moving between selections.")]
		protected float _visibilityRange = .9999f;
		[SerializeField, Tooltip("The time in seconds to animate to a new selection")]
		protected float _interpTime = .25f;

		[Header("Spinner Optional Elements")]
		[SerializeField, Optional, Tooltip("Optional button to move to the previous item in the list")]
		protected Button _prevButton;
		[SerializeField, Optional, Tooltip("Optional button to move to the next item in the list")]
		protected Button _nextButton;
		[SerializeField, Optional, Tooltip("Optional DragNotifier to handle swiping from item to item")]
		protected DragNotifier _swipeHandler;

		[Header("Spinner State")]
		[SerializeField]
		protected bool _interactable = true;

		protected List<Transform> _items;
		protected int _selectedIndex = 0;
		protected float _viewPosition = 0f;
		protected bool _listenersActive = false;

		public SimpleEvent onSelect;

		public virtual int selectedIndex {
			get { return _selectedIndex; }
			set {
				SelectAt(value, false, true);
			}
		}

		public virtual bool interactable {
			get { return _interactable; }
			set {
				_interactable = value;
				RefreshInputStates();

				if (!value) {
					MoveViewPosition(_selectedIndex);
				}
			}
		}

		public virtual bool SelectAt(int index, bool animate = true, bool notify = true) {
			if (index < 0 || index >= _items.Count) {
				Debug.LogError("Invalid item index: " + index);
			}

			if (_selectedIndex == index)
				return false;

			_selectedIndex = index;

			MoveViewPosition(index, animate);

			if (notify) {
				onSelect.Invoke();
			}

			return true;
		}

		#region ListMethods
		public virtual Transform GetItemAt(int index) {
			if (index >= 0 && index < _items.Count)
				return _items[index];

			return null;
		}

		public virtual Transform GetSelectedItem() {
			return GetItemAt(_selectedIndex);
		}

		public virtual bool Add(Transform item, int index = -1) {
			if (_items.Contains(item))
				return false;

			index = MathUtilities.WrapValue(index, 0, _items.Count + 1);
			item.SetParent(_selectionHolder, false);
			item.SetSiblingIndex(index);
			_items.Insert(index, item);

			SetViewPosition(_viewPosition);

			if (_items.Count == 2) {
				RefreshInputStates();
			}

			return true;
		}

		public virtual bool Remove(Transform item) {
			return RemoveAt(_items.IndexOf(item));
		}

		public virtual Transform RemoveAt(int index) {
			if (index < 0 || index >= _items.Count)
				return null;

			Transform item = _items[index];
			item.SetParent(null, true);
			_items.RemoveAt(index);

			if (_selectedIndex >= _items.Count && _items.Count > 0) {
				if (!SelectAt(MathUtilities.WrapValue(_selectedIndex, 0, _items.Count), false, true))
					SetViewPosition(_viewPosition);
			} else {
				SetViewPosition(_viewPosition);
			}

			RefreshInputStates();

			return item;
		}

		public virtual List<Transform> Clear() {
			for (int i = 0, count = _items.Count; i < count; ++i) {
				_items[i].SetParent(null, false);
				_items[i].gameObject.SetActive(false);
			}

			List<Transform> result = _items;
			_items = new List<Transform>();

			RefreshInputStates();

			return result;
		}

		public virtual int Count {
			get { return _items.Count; }
		}
		#endregion

		#region ObjectState
		protected virtual void Awake() {
			//Populate the item list with children of selectionHolder
			int count = _selectionHolder.childCount;
			_items = new List<Transform>(count);

			for (int i = 0; i < count; ++i) {
				_items.Add(_selectionHolder.GetChild(i));
			}
		}

		protected virtual void OnEnable() {
			SetViewPosition(_selectedIndex);
			RefreshInputStates();
		}

		protected virtual void OnDisable() {
			StopAllCoroutines();
			RemoveListeners();
		}
		#endregion

		#region ViewPosition
		protected virtual void MoveViewPosition(float index, bool animate = true) {
			StopCoroutine("InterpolateViewRoutine");
			if (animate && _interpTime > 0f) {
				StartCoroutine("InterpolateViewRoutine", index);
			} else {
				SetViewPosition(index);
			}
		}

		protected virtual void SetViewPosition(float index) {
			float visibleMin = index - _visibilityRange;
			float visibleMax = index + _visibilityRange;
			Transform item;

			for (int i = 0, count = _items.Count; i < count; ++i) {
				item = _items[i];

				if (MathUtilities.IsInWrappedRange(i, visibleMin, visibleMax, 0, count)) {
					item.localPosition = _itemSpacing * MathUtilities.WrapValue(i - index, -count * .5f, count * .5f);
					item.gameObject.SetActive(true);
				} else {
					item.gameObject.SetActive(false);
				}
			}

			_viewPosition = index;
		}

		protected virtual IEnumerator InterpolateViewRoutine(float targetIndex) {
			float startIndex = _viewPosition;
			float interpolant = 0f;

			targetIndex = MathUtilities.WrapValue(targetIndex, startIndex - _items.Count * .5f, startIndex + _items.Count * .5f);

			do {
				yield return null;
				interpolant = Mathf.Min(interpolant + Time.deltaTime / _interpTime, 1f);
				SetViewPosition(Mathf.LerpUnclamped(startIndex, targetIndex, MathUtilities.EaseOutInterpolant(interpolant)));
			} while (interpolant < 1f);
		}
		#endregion

		#region Input
		protected virtual void RefreshInputStates() {
			if (_interactable && _items.Count > 1) {
				AddListeners();
			} else {
				RemoveListeners();
			}
		}

		protected virtual void AddListeners() {
			if (_listenersActive)
				return;

			_listenersActive = true;

			if (_prevButton) {
				_prevButton.onClick.AddListener(PrevButton_OnClick);
				_prevButton.interactable = true;
			}
			if (_nextButton) {
				_nextButton.onClick.AddListener(NextButton_OnClick);
				_nextButton.interactable = true;
			}
			if (_swipeHandler) {
				_swipeHandler.onDrag.AddListener(SwipeHandler_OnDrag);
				_swipeHandler.onDragEnd.AddListener(SwipeHandler_OnDragEnd);
			}
		}

		protected virtual void RemoveListeners() {
			if (!_listenersActive)
				return;

			_listenersActive = false;

			if (_prevButton) {
				_prevButton.onClick.RemoveListener(PrevButton_OnClick);
				_prevButton.interactable = false;
			}
			if (_nextButton) {
				_nextButton.onClick.RemoveListener(NextButton_OnClick);
				_nextButton.interactable = false;
			}
			if (_swipeHandler) {
				_swipeHandler.onDrag.RemoveListener(SwipeHandler_OnDrag);
				_swipeHandler.onDragEnd.RemoveListener(SwipeHandler_OnDragEnd);
			}
		}

		protected virtual void PrevButton_OnClick() {
			if (_selectedIndex > 0) {
				SelectAt(_selectedIndex - 1);
			} else {
				SelectAt(_items.Count - 1);
			}
		}

		protected virtual void NextButton_OnClick() {
			if (_selectedIndex < _items.Count - 1) {
				SelectAt(_selectedIndex + 1);
			} else {
				SelectAt(0);
			}
		}

		protected virtual void SwipeHandler_OnDrag(PointerEventData e) {
			if (!_interactable || e.delta == Vector2.zero)
				return;

			RectTransform rt = _swipeHandler.rectTransform;
			Rect rect = rt.rect;
			Vector2 oldPos, newPos, delta;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position - e.delta, e.pressEventCamera, out oldPos);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out newPos);

			delta = newPos - oldPos;
			delta.x = delta.x / rect.width;
			delta.y = delta.y / rect.height;

			Vector2 normalizedSpacing = _itemSpacing.normalized;
			Vector2 normalizedDelta = delta.normalized;

			float viewDelta = Vector3.Project(delta, normalizedSpacing).magnitude;
			if (Vector2.Dot(normalizedDelta, normalizedSpacing) > 0f)
				viewDelta = -viewDelta;

			MoveViewPosition(_viewPosition + viewDelta, false);
		}

		protected virtual void SwipeHandler_OnDragEnd(PointerEventData e) {
			if (!_interactable)
				return;

			int index = MathUtilities.WrapValue(Mathf.RoundToInt(_viewPosition), 0, _items.Count);

			if (!SelectAt(index)) {
				MoveViewPosition(index);
			}
		}
		#endregion
	}
}

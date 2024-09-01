using UnityGameLib.Attributes;
using UnityGameLib.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityGameLib.UI {
	/// <summary>
	/// Triggers pointer enter and exit events and retains state to indicate whether the pointer is over an object.
	/// Static methods allow checking whether the pointer is over any object in a group.
	/// </summary>
	public class PointerOverNotifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		#region StaticAPI
		private static Dictionary<string, bool> _groupStates = new Dictionary<string, bool>();

		/// <summary>
		/// Returns whether the pointer is currently over any object tagged with a given group ID.
		/// </summary>
		/// <param name="groupID">the ID of the group to test</param>
		/// <returns><c>true</c> if the pointer is over any object in the group, or <c>false</c> if not</returns>
		public static bool IsPointerOverGroup(string groupID) {
			return _groupStates.ContainsKey(groupID) && _groupStates[groupID];
		}

		/// <summary>
		/// Returns whether the pointer is currently over any object tagged with any of a list of group IDs.
		/// </summary>
		/// <param name="groupIDs">a list of groupIDs to test</param>
		/// <returns></returns>
		public static bool IsPointerOverGroups(params string[] groupIDs) {
			foreach (string groupID in groupIDs) {
				if (IsPointerOverGroup(groupID)) {
					return true;
				}
			}
			return false;
		}
		#endregion

		#region InstanceAPI
		/// <summary> The PointerOverNotifier group to which this instance reports, if any. </summary>
		[Optional, Tooltip("Optional identifier to group this element with other notifiers.")]
		public string _groupID = "";
		/// <summary> Triggers when the pointer enters this object. </summary>
		public SimpleEvent onPointerEnter;
		/// <summary> Triggers when the pointer exits this object. </summary>
		public SimpleEvent onPointerExit;

		[SerializeField, Locked]
		protected bool _isOver = false;
		/// <summary> Whether the pointer is currently over this object. </summary>
		public bool isOver {
			get { return _isOver; }
		}

		protected virtual void RefreshGroupState() {
			if (!string.IsNullOrEmpty(_groupID)) {
				_groupStates[_groupID] = _isOver;
			}
		}

		protected void OnDisable() {
			DoPointerExit();
		}

		protected void DoPointerEnter() {
			if (_isOver)
				return;

			_isOver = true;
			RefreshGroupState();

			onPointerEnter.Invoke();
		}

		protected void DoPointerExit() {
			if (!_isOver)
				return;

			_isOver = false;
			RefreshGroupState();

			onPointerExit.Invoke();
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			DoPointerEnter();
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			DoPointerExit();
		}
		#endregion
	}
}
